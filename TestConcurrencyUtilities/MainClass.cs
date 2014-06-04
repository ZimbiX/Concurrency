using System;

namespace TestConcurrencyUtilities
{
	public class MainClass
	{
		public static void Main(string[] args) {
			// Config:
			double sleepTimeSeconds = 1.5;
			int testMagnitude = 10;

			int sleepTimeMs = (int)Math.Round(sleepTimeSeconds*1000);

			// TestSemaphore.Run(testMagnitude, sleepTimeMs);
			 TestChannel.Run(testMagnitude, sleepTimeMs);
//			TestBoundChannel.Run(testMagnitude, 3, sleepTimeMs);
			// TestLatch.Run(4, sleepTimeMs);
			// TestBarrier.Run(4, sleepTimeMs);

			TestSupport.Log(ConsoleColor.DarkGreen, "\nAll threads have finished");
		}
	}
}