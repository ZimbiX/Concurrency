using System;
using ConcurrencyUtilities;
using System.Threading;
using System.Collections.Generic;
using TSpec = TestConcurrencyUtilities.TestSupport.TSpec;

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

			TestSupport.SleepThread(_sleepTimeLocked/*, TODO: add column prefix */);

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

		public static void Run(int sleepTimeBetweenNewRequests = 500, int sleepTimeLocked = 2250, int numRounds = 3,
		                       int sleepTimeBetweenRounds = 3000) {
			_sleepTimeLocked = sleepTimeLocked;

			TestSupport.Log(ConsoleColor.Blue, "\nReader-Writer Lock test\n==============================");

			TestSupport.Log(ConsoleColor.Blue, "\nTests have an access duration of " +
			                TestSupport.StringFromMilliseconds(_sleepTimeLocked) + ", and wait " +
			                TestSupport.StringFromMilliseconds(sleepTimeBetweenNewRequests) + " between new requests" +
			                "\n------------------------------");

			for (int i = 0; i < numRounds; i++) {
				RunTestRounds(sleepTimeBetweenNewRequests);
				TestSupport.SleepThread(sleepTimeBetweenRounds);
			}
		}

		static void RunTestRounds(int sleepTimeBetweenNewRequests) {
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

			_readerWriter = new ReaderWriter();
			const int columnWidth = 13;

			foreach (Func<int, List<Thread>> threadCreationMethod in new Func<int, List<Thread>>[] {
				CreateTest1, CreateTest2
			}) {
				List<Thread> threads = threadCreationMethod(columnWidth);
				TestSupport.RunThreads(threads, sleepTimeBetweenNewRequests);
			}
		}

		/// <summary>
		/// Creates the threads for test 1.
		/// </summary>
		/// <returns>The threads to use for the test.</returns>
		/// <param name="columnWidth">The character width for the thread logging columns, including padding.</param>
		public static List<Thread> CreateTest1(int columnWidth) {
			TestSupport.Log(ConsoleColor.Blue, "\nTest 1:\n");
			return TestSupport.CreateThreadsFromThreadSpecs(new TSpec[] {
				new TSpec("W", Writer, 2, 1),
				new TSpec("R", Reader, 2, 1)
			}, columnWidth);
		}
		
		/// <summary>
		/// Creates the threads for test 2 -- an extended version of test 1.
		/// </summary>
		/// <returns>The threads to use for the test.</returns>
		/// <param name="columnWidth">The character width for the thread logging columns, including padding.</param>
		public static List<Thread> CreateTest2(int columnWidth) {
			TestSupport.Log(ConsoleColor.Blue, "\nTest 2:\n");
			return TestSupport.CreateThreadsFromThreadSpecs(new TSpec[] {
				new TSpec("W", Writer, 2, 1),
				new TSpec("R", Reader, 2, 1),
				new TSpec("R", Reader, 2, 3),
				new TSpec("W", Writer, 1, 3)
			}, columnWidth);
		}
	}
}