using System;
using ThreadSupport = TestConcurrencyUtilities.TestSupport;
using System.Collections.Generic;
using ConcurrencyUtilities;

namespace CigaretteSmokers
{
	public class Smoker
	{
		public string Name { get; private set; }
		Table _table;
		List<SmokerResource> _resourcesNeeded;
		int _greedyTime;
		IterationManager _iterationManager;

		public Smoker(string name, List<SmokerResource> resourcesNeeded, Table table, 
		              IterationManager iterationManager, int greedyTime = 0) {
			Name = name;
			_table = table;
			_resourcesNeeded = resourcesNeeded;
			_iterationManager = iterationManager;
			_greedyTime = greedyTime;
		}

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

		public void RunSolutionByResourceTurnstilingAndIterationChecking() {
			while (true) {
				_iterationManager.AccessToIterationID.Acquire();
					Object iterIDAtStart = _iterationManager.IterationID;
				_iterationManager.AccessToIterationID.Release();
				_resourcesNeeded[0].Take();
				_resourcesNeeded[0].Return();
				// Check that we're still on the same iteration. If not, start again
				if (_iterationManager.IterationID == iterIDAtStart) { // No need to acquire permission; not critical
					_resourcesNeeded[1].Take();
					_resourcesNeeded[1].Return();
					// Check that we're still on the same iteration. If not, start again
					bool successfullyAcquiredResourcesNeeded = false;
					_iterationManager.AccessToIterationID.Acquire(); // Need to acquire permission for final verification
						if (_iterationManager.IterationID == iterIDAtStart) {
							successfullyAcquiredResourcesNeeded = true;
							foreach (SmokerResource r in _resourcesNeeded) {
								r.Take();
								ThreadSupport.DebugThread("{black}Acquired: " + r.Name);
							}
						}
					_iterationManager.AccessToIterationID.Release();
					if (successfullyAcquiredResourcesNeeded) {
						ThreadSupport.DebugThread("{green}Smoking");
						ThreadSupport.SleepThread(_greedyTime);
						_iterationManager.NewIteration();
						_table.ReadyToStartNewIteration();
						// You'd do any actual work now
					}
				}
			}
		}
	}
}