using System;
using ConcurrencyUtilities;
using Semaphore = ConcurrencyUtilities.Semaphore;
using System.Collections.Generic;
using System.Threading;

namespace TestConcurrencyUtilities
{
	// Test my Latch class
	public class TestLatch
	{
		private static int _sleepTime;
		private static Latch _latch;

		private static void LatchVisitor() {
			TestSupport.DebugThread("is visiting the latch");
			_latch.Visit();
			TestSupport.DebugThread("  ", "has left the latch");
		}

		private static void DelayedLatchOpener() {
			TestSupport.SleepThread(_sleepTime * 3);
			TestSupport.DebugThread("will now open the latch");
			TestSupport.SleepThread(_sleepTime);
			_latch.Open();
			TestSupport.DebugThread("has opened the latch");
		}

		public static void Run(int magnitude, int sleepTime = 0) {
			_sleepTime = sleepTime;
			_latch = new Latch();

			List<Thread> threads = new List<Thread>();
			threads.AddRange(TestSupport.CreateThreads(LatchVisitor, "Latch visitor thread", magnitude));
			threads.AddRange(TestSupport.CreateThreads(DelayedLatchOpener, "Delayed latch opener thread", 1));
			TestSupport.RunThreads(threads);
			
			TestSupport.SleepThread(_sleepTime);
			threads = (TestSupport.CreateThreads(LatchVisitor, "Late latch visitor thread", 2));
			TestSupport.RunThreads(threads);
		}
	}
}