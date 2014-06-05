using System;
using ConcurrencyUtilities;

namespace CigaretteSmokers
{
	public class IterationManager
	{
		public Object IterationID { get; private set; }
		public Mutex AccessToIterationID { get; private set; }
		Table _table;

		public IterationManager(Table table) {
			_table = table;
			IterationID = new Object();
			AccessToIterationID = new Mutex();
		}

		public void NewIteration() {
			AccessToIterationID.Acquire();
				IterationID = new Object();
			AccessToIterationID.Release();
			_table.ReadyToStartNewIteration();
		}
	}
}