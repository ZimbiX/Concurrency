using System;
using ConcurrencyUtilities;
using Semaphore = ConcurrencyUtilities.Semaphore;
using Barrier = ConcurrencyUtilities.Barrier;
using System.Collections.Generic;
using System.Threading;

namespace TestConcurrencyUtilities
{
	// Test my Barrier class
	public class TestBarrier
	{
		private static int _sleepTime;
		private static Barrier _barrier;

		private static void BarrierVisitor() {
			TestSupport.DebugThread("{yellow}Entering");
			_barrier.Arrive();
			TestSupport.DebugThread("{green}Leaving");
		}

		public static void Run(int magnitude, int sleepTime = 0) {
			_sleepTime = sleepTime;
			_barrier = new Barrier(magnitude);

			TestSupport.Log(ConsoleColor.Blue, "Barrier test\n==============================");
			TestSupport.Log(ConsoleColor.Blue, "\nBarrier size: " + magnitude +
			                   "\nVisitor threads will start every " + TestSupport.StringFromMilliseconds(_sleepTime) +
			                   "\nWe'll be testing 2 groups of the barrier size.\n");

			List<Thread> threads = new List<Thread>();
			int column = 1;
			int columnWidth = 7+1;
			foreach (string threadName in new string[] {"G1-", "G2-"}) {
				threads.AddRange( TestSupport.CreateThreads(BarrierVisitor, threadName, magnitude, 1, columnWidth, column) );
				column += magnitude;
			}
			TestSupport.EndColumnHeader(column-1, columnWidth); // End the column header line

			foreach (Thread thread in threads) {
				TestSupport.SleepThread(_sleepTime);
				thread.Start();
			}
			TestSupport.JoinThreads(threads);
		}
	}
}