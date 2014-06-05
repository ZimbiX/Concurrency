using System;
using ConcurrencyUtilities;

namespace CigaretteSmokers
{
	/// <summary>
	/// The iteration manager manages the iteration ID, used for the 'Resource turnstiling and
	/// iteration checking' solution. The iteration ID is set to be a new Object so that a smoker can compare it
	/// to what the iteration ID was at the start of its process of checking for its required resources.
	/// Iteration ID objects that are no longer in use can be garbage-collected.
	/// </summary>
	public class IterationManager
	{
		public Object IterationID { get; private set; } // The iteration ID object
		public Mutex AccessToIterationID { get; private set; } // The mutex controlling access to the iteration ID
		Table _table; /* The 'agent' table -- so we can tell it when we're ready to start a new
			resource restocking iteration */

		/// <summary>
		/// Initializes a new instance of the <see cref="CigaretteSmokers.IterationManager"/> class.
		/// </summary>
		/// <param name="table">The 'agent' table -- so we can tell it when we're ready to start a new
		/// resource restocking iteration.</param>
		public IterationManager(Table table) {
			_table = table;
			IterationID = new Object();
			AccessToIterationID = new Mutex();
		}

		/// <summary>
		/// Set the iteration ID to be a new Object, and tell the 'agent' table that we're ready to start a new
		/// resource restocking iteration.
		/// </summary>
		public void NewIteration() {
			AccessToIterationID.Acquire();
				IterationID = new Object();
			AccessToIterationID.Release();
			_table.ReadyToStartNewIteration();
		}
	}
}