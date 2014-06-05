using System;
using ThreadSupport = TestConcurrencyUtilities.TestSupport;
using System.Collections.Generic;
using ConcurrencyUtilities;

namespace CigaretteSmokers
{
	/// <summary>
	/// The smoker. Runs the problem solution.
	/// </summary>
	public class Smoker
	{
		public string Name { get; private set; } // The name of the smoker (indicating the unlimited resource held)
		Table _table; // The 'agent' table
		List<SmokerResource> _resourcesNeeded; // The list of the two resources that this smoker needs
		int _greedyTime; // The time duration that should be spent smoking before telling the agent to restock
		IterationManager _iterationManager; /* The iteration manager for the 'Resource turnstiling and
			iteration checking' solution */

		/// <summary>
		/// Initializes a new instance of the <see cref="CigaretteSmokers.Smoker"/> class.
		/// </summary>
		/// <param name="name">The name of the smoker (indicating the unlimited resource held).</param>
		/// <param name="resourcesNeeded">The list of the two resources that this smoker needs.</param>
		/// <param name="table">The 'agent' table.</param>
		/// <param name="iterationManager">The iteration manager for the 'Resource turnstiling and
		/// iteration checking' solution.</param>
		/// <param name="greedyTime">The time duration that should be spent smoking before telling the agent to
		/// restock.</param>
		public Smoker(string name, List<SmokerResource> resourcesNeeded, Table table, 
		              IterationManager iterationManager, int greedyTime = 0) {
			Name = name;
			_table = table;
			_resourcesNeeded = resourcesNeeded;
			_iterationManager = iterationManager;
			_greedyTime = greedyTime;
		}

		/// <summary>
		/// Runs the smoker, demonstrating the problem issue.
		/// </summary>
		public void RunIssueDemonstration() {
			while (true) {
				foreach (SmokerResource r in _resourcesNeeded) {
					ThreadSupport.DebugThread("{yellow}Acquiring: " + r.Name + "...");
					r.Take();
					ThreadSupport.DebugThread("{green}Acquired: " + r.Name);
				}
				ThreadSupport.DebugThread("{green}Smoking");
				ThreadSupport.SleepThread(_greedyTime);
				_table.ReadyToStartNewIteration();
			}
		}

		/// <summary>
		/// Runs the smoker, using my own 'Resource turnstiling and iteration checking' solution.
		/// I'm a bit proud of this =)
		/// </summary>
		public void RunSolutionByResourceTurnstilingAndIterationChecking() {
			while (true) {
				// Remember the iteration ID from before we start checking for our required resources
				_iterationManager.AccessToIterationID.Acquire();
					Object iterIDAtStart = _iterationManager.IterationID;
				_iterationManager.AccessToIterationID.Release();

				// Check for the first required resource
				_resourcesNeeded[0].Take();
				_resourcesNeeded[0].Return();

				// Check that we're still on the same iteration. If not, start again
				if (_iterationManager.IterationID == iterIDAtStart) { // No need to acquire permission; not critical

					// Check for the second required resource
					_resourcesNeeded[1].Take();
					_resourcesNeeded[1].Return();

					// Check that we're still on the same iteration. If not, start again
					bool successfullyAcquiredResourcesNeeded = false;
					_iterationManager.AccessToIterationID.Acquire(); // Need to acquire permission for final verification
						if (_iterationManager.IterationID == iterIDAtStart) {
							
							/* Both resources are available, and it's still the same iteration as when we started, so 
							acquire the resources we need */
							successfullyAcquiredResourcesNeeded = true;
							foreach (SmokerResource r in _resourcesNeeded) {
								r.Take();
								ThreadSupport.DebugThread("{black}Acquired: " + r.Name);
							}
						}
					_iterationManager.AccessToIterationID.Release();

					// Now that we're outside the iteration ID access lock, check again that we were successful
					if (successfullyAcquiredResourcesNeeded) {
						// The resources have been acquired successfully, so commence smoking!
						ThreadSupport.DebugThread("{green}Smoking");
						ThreadSupport.SleepThread(_greedyTime);
						_iterationManager.NewIteration(); // Update the iteration ID for a new iteration
						_table.ReadyToStartNewIteration(); // Allow the 'agent' table to restock more resources
						// You'd do any actual work now
					}
				}
			}
		}
	}
}