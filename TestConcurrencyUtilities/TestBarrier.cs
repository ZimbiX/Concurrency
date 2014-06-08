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
			TestSupport.DebugThread("{yellow}A");
			_barrier.Arrive();
//			bool isCaptain = _barrier.Arrive();
//			TestSupport.DebugThread("{green}L" + (isCaptain ? "*" : ""));
		}

		public static void Run(int magnitude, int sleepTime = 0) {
			_sleepTime = sleepTime;
			_sleepTime *= 4; // TODO: revert
			_barrier = new Barrier(magnitude, true);
			int numGroups = 6;

			TestSupport.Log(ConsoleColor.Blue, "Barrier test\n==============================");
			TestSupport.Log(ConsoleColor.Blue, "\nBarrier size: " + magnitude +
			                "\nSplit visitor thread groups will start every " +
			                TestSupport.StringFromMilliseconds(_sleepTime) +
			                "\nWe'll be testing " + numGroups + " groups of the barrier size." +
			                "\nNote how threads may arrive at the barrier at any time, but threads of a new group " +
			                "can only begin entering the barrier once it is empty.\n" +
			                "\nLegend:" +
			                "\n- A -- thread has just arrived at the barrier (now waiting to enter it)" +
			                "\n- E -- thread has just entered the barrier (now waiting to leave it)" +
			                "\n- L -- thread has just left the barrier" +
			                "\n- L* -- thread has left the barrier as the captain for the group");

			List<Thread> threads = new List<Thread>();
			int column = 1;
			int columnWidth = 2+1;
			threads.AddRange( TestSupport.CreateThreads(BarrierVisitor, "V", magnitude * numGroups, -1,
			                                            columnWidth, column) );
			TestSupport.EndColumnHeader(column-1, columnWidth); // End the column header line


			for (int i = 0; i < threads.Count; i++) {
				if (((i+2) % 4) == 0) // Sleep halfway through starting each group
					TestSupport.SleepThread(_sleepTime, "{white}" + new String('.', columnWidth * threads.Count));
				threads[i].Start();
			}
			TestSupport.JoinThreads(threads);
		}
	}
}