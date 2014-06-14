using System;
using ConcurrencyUtilities;
using Thread = System.Threading.Thread;
using TestConcurrencyUtilities;

namespace RiverCrossing
{
	public class Programmer
	{
		Barrier _groupBarrier;      // Size: 2
		Semaphore _boardPermission;    // Size: 2
		Semaphore _partnerCanBoard; // Size: 1...
		Barrier _boatBarrier;       // Size: 4
		Semaphore _groupPairer;

		public Programmer(Semaphore groupPairer, Barrier groupBarrier, Semaphore boardPermission,
		                  Semaphore partnerCanBoard, Barrier boatBarrier) {
			_groupPairer = groupPairer;
			_groupBarrier = groupBarrier;
			_boardPermission = boardPermission;
			_partnerCanBoard = partnerCanBoard;
			_boatBarrier = boatBarrier;
		}

		public void Run() {
			// Attempt to board the boat:

			// Pair up and decide the captain
			_groupPairer.Acquire();
				// Wait for pair to arrive
				if (_groupBarrier.Arrive()) { // If we were selected to be the captain
					_boardPermission.Acquire(); // Get permission to let our group onto the boat (reserve space for a pair)
					TestSupport.DebugThread("{!green}{black}Board*");
					_partnerCanBoard.Release(); // Allow the non-captain of our pair to board
//					TestSupport.SleepThread(1000);
				} else {
					_partnerCanBoard.Acquire(); // Wait for the captain to allow us to board
					TestSupport.DebugThread("{!green}{black}Board");
				}
			_groupPairer.Release();

//			TestSupport.DebugThread("{!green}{black}Board");
			// Board the boat
			if (_boatBarrier.Arrive()) {
				Row();
				_boardPermission.Release(2); // Free space on the boat for two pairs
			}
		}

		void Row() {
			TestSupport.DebugThread("{!yellow}{black}Row");
			TestSupport.SleepThread(1000, "-------------------------------------------------");
			TestSupport.DebugThread("{!green}{black}Row");
		}
	}
}