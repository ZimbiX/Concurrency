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
			TestSupport.DebugThread("is entering the barrier");
			_barrier.Arrive();
			TestSupport.DebugThread("  ", "has left the barrier");
		}

		public static void Run(int magnitude, int sleepTime = 0) {
			_sleepTime = sleepTime;
			_barrier = new Barrier(magnitude);

			TestSupport.Log(ConsoleColor.Blue, "Barrier test\n==============================");
			TestSupport.Log(ConsoleColor.Blue, "\nBarrier size: " + magnitude +
			                   "\nVisitor threads will start every " + TestSupport.StringFromMilliseconds(_sleepTime));

			List<Thread> threads = new List<Thread>();
			foreach (string threadName in new string[] {"Barrier visitor group 1 thread", "Barrier visitor group 2 thread"})
				threads.AddRange(TestSupport.CreateThreads(BarrierVisitor, threadName, magnitude, 1));
			foreach (Thread thread in threads) {
				TestSupport.SleepThread(_sleepTime);
				thread.Start();
			}
			TestSupport.JoinThreads(threads);
		}
	}
}