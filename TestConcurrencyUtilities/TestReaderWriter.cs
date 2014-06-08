using System;
using ConcurrencyUtilities;
using System.Threading;
using System.Collections.Generic;

namespace TestConcurrencyUtilities
{
	public static class TestReaderWriter
	{
		static ReaderWriter _readerWriter;
		static int _sleepTimeLocked;

		public static void Reader() {
			TestSupport.DebugThread("{yellow}RAcq");
			_readerWriter.ReaderAcquire();
			TestSupport.DebugThread("{green}RAcq");

			TestSupport.SleepThread(_sleepTimeLocked);

			TestSupport.DebugThread("{yellow}RRel");
			_readerWriter.ReaderRelease();
			TestSupport.DebugThread("{cyan}RRel");
		}

		public static void Writer() {
			TestSupport.DebugThread("{yellow}WAcq");
			_readerWriter.WriterAcquire();
			TestSupport.DebugThread("{red}WAcq");

			TestSupport.SleepThread(_sleepTimeLocked);

			TestSupport.DebugThread("{yellow}WRel");
			_readerWriter.WriterRelease();
			TestSupport.DebugThread("{cyan}WRel");
		}

		public static void Run(int sleepTimeLocked = 2000, int sleepTimeBetweenNewRequests = 300) {
			_sleepTimeLocked = sleepTimeLocked;
			int sleepTimeBetweenRounds = 3000;

			TestSupport.Log(ConsoleColor.Blue, "Reader-Writer Lock test\n==============================");

			RunTestRound(sleepTimeLocked);
			TestSupport.SleepThread(sleepTimeBetweenRounds);
			RunTestRound(sleepTimeBetweenNewRequests);
			TestSupport.SleepThread(sleepTimeBetweenRounds);
			RunTestRound(0);
		}

		static void RunTestRound(int sleepTimeBetweenNewRequests) {
			_readerWriter = new ReaderWriter();

			TestSupport.Log(ConsoleColor.Blue, "\nTest with access duration of " +
			                TestSupport.StringFromMilliseconds(_sleepTimeLocked) + ", and " +
			                TestSupport.StringFromMilliseconds(sleepTimeBetweenNewRequests) + " between new requests" +
			                "\n------------------------------\n");
			
			// Create the threads:
			
			const int columnWidth = 10;
			List<Thread> threads = new List<Thread>();
			threads.AddRange(TestSupport.CreateThreads(Writer, "W", 1, 0, columnWidth, threads.Count + 1));
			threads.AddRange(TestSupport.CreateThreads(Reader, "R", 4, 0, columnWidth, threads.Count + 1));
			threads.AddRange(TestSupport.CreateThreads(Writer, "W", 2, 0, columnWidth, threads.Count + 1));
			threads.AddRange(TestSupport.CreateThreads(Reader, "R", 2, 0, columnWidth, threads.Count + 1));
			threads.AddRange(TestSupport.CreateThreads(Writer, "W", 1, 0, columnWidth, threads.Count + 1));
			TestSupport.EndColumnHeader(threads.Count, columnWidth);
			
			// Start the threads:
			
			foreach (Thread t in threads) {
				TestSupport.SleepThread(sleepTimeBetweenNewRequests, false);
				t.Start();
			}
			
			// Wait for all the threads to finish:
			
			TestSupport.JoinThreads(threads);
		}
	}
}