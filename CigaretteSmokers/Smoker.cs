using System;
using ThreadSupport = TestConcurrencyUtilities.TestSupport;
using System.Collections.Generic;

namespace CigaretteSmokers
{
	public class Smoker
	{
		public string Name { get; private set; }
		Table _table;
		List<SmokerResource> _resourcesNeeded;

		public Smoker(string name, Table table, List<SmokerResource> resourcesNeeded) {
			Name = name;
			_table = table;
			_resourcesNeeded = resourcesNeeded;
		}

		public void Run() {
			while (true) {
				Object iterIDAtStart = _table.IterationID; // TODO: lock
				_resourcesNeeded[0].Take();
				_resourcesNeeded[0].Return();
				// Check that we're still on the same iteration. If not, start again
				if (_table.IterationID == iterIDAtStart) { // No need to acquire permission; not critical
					_resourcesNeeded[1].Take();
					_resourcesNeeded[1].Return();
					// Check that we're still on the same iteration. If not, start again
					_table.AccessToIterationID.Acquire(); // Need to acquire permission for final verification
						if (_table.IterationID == iterIDAtStart) { // TODO: lock
							foreach (SmokerResource r in _resourcesNeeded) {
								r.Take();
								ThreadSupport.DebugThread("{black}Acquired: " + r.Name);
							}
							ThreadSupport.DebugThread("{green}Smoking");
							ThreadSupport.SleepThread(1000);
						_table.ReadyToStartNewIteration(); /* It's ok for this to be inside the AccessToIterationID
							permisson; the table will just wait after acquiring its permission to start a
							new iteration */
						}
					_table.AccessToIterationID.Release();
				}
			}
		}
	}
}