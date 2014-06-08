using System;
using ConcurrencyUtilities;
using System.Threading;
using System.Collections.Generic;
using Mutex = ConcurrencyUtilities.Mutex;

namespace TestConcurrencyUtilities
{
	public static class TestSemaphoreFIFO
	{
		static SemaphoreFIFO _semaphoreFIFO;
		static int _magnitude;
		static int _sleepTime;
		static bool _internalTesting;

		public static void TokenAcquire() {
			if (!_internalTesting) TestSupport.DebugThread("{yellow}A");
			_semaphoreFIFO.Acquire();
			if (!_internalTesting) TestSupport.DebugThread("{green}A");
		}

		public static void TokenReleaser() {
			for (int i = 0; i < _magnitude; i++) {
				TestSupport.SleepThread(_sleepTime, false);
				TestSupport.DebugThread("{yellow}R");
				_semaphoreFIFO.Release();
				TestSupport.DebugThread("{green}R");
			}
		}

		public static void Run(int magnitude, int sleepTime) {
			_magnitude = magnitude;
			_sleepTime = sleepTime;
			int interimPause = _sleepTime * 3;

			TestSupport.Log(ConsoleColor.Blue, "FIFO Semaphore test\n==============================");
			TestSupport.Log(ConsoleColor.Blue, "\nAcquire threads: " + _magnitude +
			                "\nAfter the Acquire() requests are made, a Release() will occur every " +
			                TestSupport.StringFromMilliseconds(_sleepTime) +
			                "\nUnfortunately, testing the order of fast Acquire() requests can only be verified " +
			                "using debug logging from within the actual SemaphoreFIFO utility class. We'll perform " +
			                "a test utilising this first to prove that fulfilment order for fast Acquire() requests " +
			                "does actually work. Second, we'll perform a test to demonstrate the utility as well as " +
			                "we can while avoiding the potential for interference caused by internal logging.\n" +
			                "\nLegend:" +
			                "\n- (Yellow) -- Request initiated" +
			                "\n- (Green) -- Request fulfilled" +
			                "\n- R -- releasing a token into the FIFO semaphore" +
			                "\n- A -- acquiring a token from the FIFO semaphore" +
			                "\n- A:4 -- acquring a token as the fourth thread that requested one from the " +
			                "FIFO semaphore");

			TestSupport.Log(ConsoleColor.Blue, "\nTest with internal logging (fast Acquire() requests)" +
			                "\n------------------------------\n");
			TestSupport.SleepThread(interimPause);
			RunTestRound(true);

			TestSupport.Log(ConsoleColor.Blue, "\nTest without internal logging (slower Acquire() requests)" +
			                "\n------------------------------\n");
			TestSupport.SleepThread(interimPause);
			RunTestRound(false);
		}

		static void RunTestRound(bool internalTesting) {
			_internalTesting = internalTesting;
			_semaphoreFIFO = new SemaphoreFIFO(0, _internalTesting);

			// Create the threads:

			int columnWidth = 7;
			List<Thread> acquireThreads = new List<Thread>();
			Thread releaserThread = TestSupport.CreateThreads(TokenReleaser, "R", 1, -1, columnWidth, 1)[0];
			acquireThreads.AddRange(TestSupport.CreateThreads(TokenAcquire, "A", 10, 0, columnWidth, 2));
			TestSupport.EndColumnHeader(11, columnWidth);

			// Start the threads:

			foreach (Thread t in acquireThreads) {
				if (!internalTesting) /* For the slow, external-only round, sleep between starting the acquire threads
				                       to hopefully set a particular order for the queue given that the FIFO semaphore
				                       can't tell us what the actual order of entry is */
					TestSupport.SleepThread(100, false);
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