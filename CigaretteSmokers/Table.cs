using System;
using System.Collections.Generic;
using ConcurrencyUtilities;
using ThreadSupport = TestConcurrencyUtilities.TestSupport;

namespace CigaretteSmokers
{
	public class Table
	{
		List<SmokerResource> _resources;

		public Object IterationID { get; private set; }
		public Mutex AccessToIterationID { get; private set; }
		Semaphore _startIteration;
		Random _rnd;

		public Table(List<SmokerResource> resources) {
			AccessToIterationID = new Mutex();
			_startIteration = new Semaphore(0);
			_resources = resources;
			_rnd = new Random();

			ReadyToStartNewIteration();
		}

		public void Run() {
			while (true) {
				// Wait for a new iteration to begin before continuing (signalled by constructor / successful smoker)
				_startIteration.Acquire();

				// Start a new iteration
				
				AccessToIterationID.Acquire();
					IterationID = new Object();
					RestockTwoResources();
				AccessToIterationID.Release();
			}
		}

		public void ReadyToStartNewIteration() {
			_startIteration.Release();
		}

		void RestockTwoResources() {
			List<SmokerResource> resourcesToRestock = new List<SmokerResource>(_resources);
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