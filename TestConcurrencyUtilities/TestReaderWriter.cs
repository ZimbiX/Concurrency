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
			TestSupport.DebugThread("{!yellow}{black}RAcq");
			_readerWriter.ReaderAcquire();
			TestSupport.DebugThread("{!green}{black}RAcq");

			TestSupport.SleepThread(_sleepTimeLocked);

			TestSupport.DebugThread("{!yellow}{black}RRel");
			_readerWriter.ReaderRelease();
			TestSupport.DebugThread("{!cyan}{black}RRel");
		}

		public static void Writer() {
			TestSupport.DebugThread("{!yellow}{black}WAcq");
			_readerWriter.WriterAcquire();
			TestSupport.DebugThread("{!red}{black}WAcq");

			TestSupport.SleepThread(_sleepTimeLocked);

			TestSupport.DebugThread("{!yellow}{black}WRel");
			_readerWriter.WriterRelease();
			TestSupport.DebugThread("{!cyan}{black}WRel");
		}

		public static void Run(int sleepTimeLocked = 2000, int sleepTimeBetweenNewRequests = 100) {
			_sleepTimeLocked = sleepTimeLocked;
			int sleepTimeBetweenRounds = 3000;

			TestSupport.Log(ConsoleColor.Blue, "Reader-Writer Lock test\n==============================");

//			RunTestRound(sleepTimeLocked);
//			TestSupport.SleepThread(sleepTimeBetweenRounds);
//			RunTestRound(sleepTimeBetweenNewRequests);
//			TestSupport.SleepThread(sleepTimeBetweenRounds);
//			RunTestRound(0);

			while (true) { // TODO: remove
				RunTestRoundForReaderPriorityFulfilmentOrder(sleepTimeBetweenNewRequests, true);
				RunTestRoundForReaderPriorityFulfilmentOrder(sleepTimeBetweenNewRequests, false);
			}

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

		static void RunTestRoundForReaderPriorityFulfilmentOrder(int sleepTimeBetweenNewRequests, bool useInternalTesting = false) {
			// Order of request:    W1, W2, R1, R2
			// Order of fulfilment: W1, R1, R2, W2
			// Desired test result log:
			// W1        W2        R1        R2
			// ----------------------------------------
			// Acq
			// ACQ
			//           Acq
			//                     Acq
			//                               Acq
			// ...
			// Rel
			// REL
			//                     ACQ
			//                               ACQ
			//                     ...
			//                               ...
			//                     Rel
			//                     REL
			//                               Rel
			//                               REL
			//           ACQ
			//           ...
			//           Rel
			//           REL

//			_readerWriter = new ReaderWriter(useInternalTesting);
			_readerWriter = new ReaderWriter(); // TODO: revert

			_sleepTimeLocked = 1000; // TODO: remove
			sleepTimeBetweenNewRequests = 220; // TODO: remove

			TestSupport.Log(ConsoleColor.Blue, "\nTest with access duration of " +
			                TestSupport.StringFromMilliseconds(_sleepTimeLocked) + ", and " +
			                TestSupport.StringFromMilliseconds(sleepTimeBetweenNewRequests) + " between new requests" +
			                "\n------------------------------\n");

			// Create the threads:

			const int columnWidth = 13;
			List<Thread> threads = new List<Thread>();
			threads.AddRange(TestSupport.CreateThreads(Writer, "W", 2, 1, columnWidth, threads.Count + 1));
			threads.AddRange(TestSupport.CreateThreads(Reader, "R", 2, 1, columnWidth, threads.Count + 1));
			threads.AddRange(TestSupport.CreateThreads(Reader, "R", 2, 1, columnWidth, threads.Count + 1));
			threads.AddRange(TestSupport.CreateThreads(Writer, "W", 1, 1, columnWidth, threads.Count + 1));
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