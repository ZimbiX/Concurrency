using System;
using ConcurrencyUtilities;
using System.Threading;
using System.Collections.Generic;

namespace TestConcurrencyUtilities
{
	public static class TestSemaphoreFIFO
	{
		static SemaphoreFIFO _semaphoreFIFO;
		static int _magnitude;
		static int _sleepTime;

		public static void TokenAcquire() {
			TestSupport.DebugThread("{yellow}Acq");
			_semaphoreFIFO.Acquire();
//			TestSupport.DebugThread("{green}Acq'd");
		}

		public static void TokenReleaser() {
			for (int i = 0; i < _magnitude; i++) {
				TestSupport.SleepThread(_sleepTime, false);
				TestSupport.DebugThread("{yellow}Rel");
				_semaphoreFIFO.Release();
				TestSupport.DebugThread("{green}Rel'd");
			}
		}

		public static void Run(int magnitude, int sleepTime) {
			_semaphoreFIFO = new SemaphoreFIFO(0);
			_magnitude = magnitude;
			_sleepTime = 500; // TODO: remove

			// Create the threads:

			int columnWidth = 8;
			List<Thread> acquireThreads = new List<Thread>();
			Thread releaserThread = TestSupport.CreateThreads(TokenReleaser, "Rel'r", 1, -1, columnWidth, 1)[0];
			acquireThreads.AddRange(TestSupport.CreateThreads(TokenAcquire, "Acq'r", 10, 0, columnWidth, 2));
			TestSupport.EndColumnHeader(11, columnWidth);

			// Start the threads:

			foreach (Thread t in acquireThreads) {
//				TestSupport.SleepThread(_sleepTime, false);
				t.Start();
			}
			releaserThread.Start();

			// Wait for all the threads to finish:

			List<Thread> threadsToJoin = new List<Thread>(acquireThreads);
			threadsToJoin.Add(releaserThread);
			TestSupport.JoinThreads(threadsToJoin);
		}
	}
}