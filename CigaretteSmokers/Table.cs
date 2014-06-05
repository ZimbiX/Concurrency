using System;
using System.Collections.Generic;
using ConcurrencyUtilities;
using ThreadSupport = TestConcurrencyUtilities.TestSupport;

namespace CigaretteSmokers
{
	/// <summary>
	/// The table is the 'agent'. It keeps two of the three resources in stock at any one time.
	/// </summary>
	public class Table
	{
		List<SmokerResource> _resources;

		Semaphore _startIteration; // Permission to start the next resource restocking iteration
		Random _rnd; // The random number generator to use to decide which resource should not be restocked

		/// <summary>
		/// Initializes a new instance of the <see cref="CigaretteSmokers.Table"/> class.
		/// </summary>
		/// <param name="resources">The list containing the smoker resources which should be restocked.</param>
		public Table(List<SmokerResource> resources) {
			_startIteration = new Semaphore(0);
			_resources = resources;
			_rnd = new Random();

			ReadyToStartNewIteration();
		}

		/// <summary>
		/// Runs the table thread. Restocks when allowed to.
		/// </summary>
		public void Run() {
			while (true) {
				// Wait for a new iteration to begin before continuing (signalled by constructor / successful smoker)
				_startIteration.Acquire();

				// Start a new iteration. If using the iteration checking method, the IterationID has already been
				// updated by the successful smoker -- avoiding modifying the 'agent' (table) from the original problem
				RestockTwoResources();
			}
		}

		/// <summary>
		/// Gives the table permission to restock the resources (from having been waiting in the run method for the
		/// existing resouces to be taken).
		/// </summary>
		public void ReadyToStartNewIteration() {
			_startIteration.Release();
		}

		/// <summary>
		/// Randomly restocks two of the three resources.
		/// </summary>
		void RestockTwoResources() {
			List<SmokerResource> resourcesToRestock = new List<SmokerResource>(_resources); // Clone the resource list
			int indexOfResourceToWithhold = _rnd.Next(0, 3);
			ThreadSupport.DebugThread("{yellow}Not restocking: " + resourcesToRestock[indexOfResourceToWithhold].Name +
			                          " (" + (indexOfResourceToWithhold+1) + ")");
			resourcesToRestock.RemoveAt(indexOfResourceToWithhold);
			foreach (SmokerResource resource in resourcesToRestock) {
				ThreadSupport.DebugThread("{black}Restocked: " + resource.Name);
				resource.Restock();
			}
		}
	}
}