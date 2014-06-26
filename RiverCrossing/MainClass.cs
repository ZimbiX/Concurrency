using System;
using ConcurrencyUtilities;
using Thread = System.Threading.Thread;
using TestConcurrencyUtilities;
using System.Collections.Generic;

namespace RiverCrossing
{
	// Status: logic complete, test complete, TODO: documentation
	class MainClass
	{
		static int _sleepTime;
		static int _columnWidth;
		static int _magnitude;
		static Random _rand;

		static Semaphore _lGroupPairer;
		static Semaphore _mGroupPairer;
		static Barrier _lGroupBarrier;
		static Barrier _mGroupBarrier;
		static Semaphore _boardPermission;
		static Semaphore _lPartnerCanBoard;
		static Semaphore _mPartnerCanBoard;
		static Barrier _boatBarrier;

		static int _numL;
		static int _numM;

		public static void Main(string[] args) {
			_lGroupPairer = new Semaphore(2);
			_mGroupPairer = new Semaphore(2);

			_lGroupBarrier = new Barrier(2);
			_mGroupBarrier = new Barrier(2);

			_boardPermission = new Semaphore(2);

			_lPartnerCanBoard = new Semaphore(0);
			_mPartnerCanBoard = new Semaphore(0);

			_boatBarrier = new Barrier(4);

			_sleepTime = 500;
			_columnWidth = 9;
			_magnitude = 8; // How many pairs
			_rand = new Random();
			int sleepTimeBetweenRounds = 3000;

			foreach (int sleepTimeBetweenThreadStarts in new int[] {_sleepTime, 0}) {
				TestSupport.SleepThread(2000, true);
				string speed = sleepTimeBetweenThreadStarts == 0 ? "Fast" : "Slow";
				do {
					TestSupport.DebugThread("{blue}" + speed + " test round\n------------------------------\n");
					_numL = 0;
					_numM = 0;

					List<Thread> threads = new List<Thread>();
					for (int i = 0; i < _magnitude * 2; i++) {
						threads.Add(CreateProgrammer(NextType(_rand)));
					}
					TestSupport.EndColumnHeader(_magnitude * 2, _columnWidth);
					TestSupport.RunThreads(threads, sleepTimeBetweenThreadStarts);

					TestSupport.SleepThread(sleepTimeBetweenRounds, false);
					//					TestSupport.DebugThread(new String('\n', 20));
					TestSupport.DebugThread("\n\n");
				} while (sleepTimeBetweenThreadStarts == 0);
			}
		}

		static bool RandBool(Random rand) {
			return rand.NextDouble() > 0.5;
		}

		static bool NextType(Random rand) {
			int numRemaining = _magnitude * 2 - (_numL + _numM);
			// Make sure the final thread has a partner
			if (numRemaining == 1)
				return (_numM % 2 == 0);
			return RandBool(rand);
		}

		static Thread CreateProgrammer(bool areLinux) {
			if (areLinux)
				return CreateProgrammer(true, _lGroupPairer, _lGroupBarrier, _lPartnerCanBoard);
			else
				return CreateProgrammer(false, _mGroupPairer, _mGroupBarrier, _mPartnerCanBoard);
		}

		static Thread CreateProgrammer(bool isLinux, Semaphore groupPairer, Barrier groupBarrier,
		                                     Semaphore partnerCanBoard) {
			Programmer programmer = new Programmer(isLinux, groupPairer, groupBarrier, _boardPermission,
			                                       partnerCanBoard, _boatBarrier);
			string name;
			if (isLinux) {
				_numL++;
				name = "LH";
			} else {
				_numM++;
				name = "ME";
			}
			int columnNum = _numL + _numM;
			int nameNum = isLinux ? _numL : _numM;
			return TestSupport.CreateThreads(programmer.Run, name, 1, nameNum, _columnWidth, columnNum)[0];
		}
	}
}