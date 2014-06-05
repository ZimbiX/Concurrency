using System;
using ConcurrencyUtilities;
using System.Threading;
using System.Collections.Generic;

namespace TestConcurrencyUtilities
{
	public static class TestReaderWriter
	{
		static ReaderWriter _readerWriter;
		static int _sleepTime;

		public static void Reader() {
			TestSupport.DebugThread("{yellow}RAcq");
			_readerWriter.ReaderAcquire();
			TestSupport.DebugThread("{green}RAcq");

			TestSupport.SleepThread(_sleepTime);

			TestSupport.DebugThread("{yellow}RRel");
			_readerWriter.ReaderRelease();
			TestSupport.DebugThread("{cyan}RRel");
		}

		public static void Writer() {
			TestSupport.DebugThread("{yellow}WAcq");
			_readerWriter.WriterAcquire();
			TestSupport.DebugThread("{red}WAcq");

			TestSupport.SleepThread(_sleepTime);

			TestSupport.DebugThread("{yellow}WRel");
			_readerWriter.WriterRelease();
			TestSupport.DebugThread("{cyan}WRel");
		}

		public static void Run(int sleepTime) {
			_readerWriter = new ReaderWriter();
			_sleepTime = sleepTime;
			_sleepTime = 2000; // TODO: remove

			// Create the threads:

			const int columnWidth = 8;
			List<Thread> threads = new List<Thread>();
			threads.AddRange(TestSupport.CreateThreads(Writer, "W", 2, 0, columnWidth, threads.Count + 1));
			threads.AddRange(TestSupport.CreateThreads(Reader, "R", 2, 0, columnWidth, threads.Count + 1));
			threads.AddRange(TestSupport.CreateThreads(Writer, "W", 2, 0, columnWidth, threads.Count + 1));
			threads.AddRange(TestSupport.CreateThreads(Reader, "R", 2, 0, columnWidth, threads.Count + 1));
//			threads.AddRange(TestSupport.CreateThreads(Writer, "W", 1, 0, columnWidth, threads.Count + 1));
			TestSupport.EndColumnHeader(threads.Count, columnWidth);

			// Start the threads:

			foreach (Thread t in threads) {
				TestSupport.SleepThread(100, false);
				t.Start();
			}

			// Wait for all the threads to finish:

			TestSupport.JoinThreads(threads);
		}
	}
}