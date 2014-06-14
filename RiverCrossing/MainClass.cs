using System;
using ConcurrencyUtilities;
using Thread = System.Threading.Thread;
using TestConcurrencyUtilities;
using System.Collections.Generic;

namespace RiverCrossing
{
	class MainClass
	{
		static int _columnWidth;
		static bool _displayColumnHeader;
		static Semaphore _lGroupPairer;
		static Semaphore _mGroupPairer;
		static Barrier _lGroupBarrier;
		static Barrier _mGroupBarrier;
		static Semaphore _boardPermission;
//		static Semaphore _mPairCanBoard;
		static Semaphore _lPartnerCanBoard;
		static Semaphore _mPartnerCanBoard;
		static Barrier _boatBarrier;

		public static void Main(string[] args) {
			_lGroupPairer = new Semaphore(2);
			_mGroupPairer = new Semaphore(2);

			_lGroupBarrier = new Barrier(2);      // Size: 2
			_mGroupBarrier = new Barrier(2);      // Size: 2

			_boardPermission = new Semaphore(2);    // Size: 2
//			_mPairCanBoard = new Semaphore(2);    // Size: 2

			_lPartnerCanBoard = new Semaphore(0);     // Size: 1
			_mPartnerCanBoard = new Semaphore(0);     // Size: 1

			_boatBarrier = new Barrier(4);       // Size: 4
			
			_columnWidth = 9;
			_displayColumnHeader = true;

//			bool areLinux = true;
//			for (int i = 0; i < 10; i++) {
			for (int i = 0; i < 10; i++) {
				// Create two threads of each type
				List<Thread> threadsIteration = new List<Thread>();
				threadsIteration.AddRange(CreateProgrammerPair(true));
				threadsIteration.AddRange(CreateProgrammerPair(false));
				if (_displayColumnHeader) {
					TestSupport.EndColumnHeader(4, _columnWidth);
					_displayColumnHeader = false;
				}
				// Start them
				foreach (Thread thread in threadsIteration)
					thread.Start();
			}
		}

		static List<Thread> CreateProgrammerPair(bool areLinux) {
			if (areLinux)
				return CreateProgrammerPair(true, _lGroupPairer, _lGroupBarrier, _lPartnerCanBoard);
			else
				return CreateProgrammerPair(false, _mGroupPairer, _mGroupBarrier, _mPartnerCanBoard);
		}

		static List<Thread> CreateProgrammerPair(bool areLinux, Semaphore groupPairer, Barrier groupBarrier,
		                                         Semaphore partnerCanBoard) {
			List<Thread> threadPair = new List<Thread>();
			for (int i = 0; i < 2; i++) {
				Programmer programmer = new Programmer(groupPairer, groupBarrier, _boardPermission, partnerCanBoard, _boatBarrier);
				Thread thread = TestSupport.CreateThreads(programmer.Run, areLinux ? "LH" : "ME", 1, 1 + i,
				                                          _columnWidth, (areLinux ? 3 : 1) + i,
				                                          _displayColumnHeader)[0];
				threadPair.Add(thread);
			}
			return threadPair;
		}
	}
}