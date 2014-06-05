using System;

namespace TestConcurrencyUtilities
{
	public class MainClass
	{
		public static void Main(string[] args) {
			// Config:
			double sleepTimeSeconds = 1.0;
//			int testMagnitude = 10;

			int sleepTimeMs = (int)Math.Round(sleepTimeSeconds*1000);

//			TestSemaphore.Run(10, sleepTimeMs);
//			TestChannel.Run(10, sleepTimeMs);
//			TestBoundChannel.Run(10, 3, sleepTimeMs);
//			TestMutex.Run(100000);
//			TestLatch.Run(4, sleepTimeMs);
			TestBarrier.Run(4, sleepTimeMs);
//			TestLightSwitch.Run(5, sleepTimeMs);
//			TestExchanger.Run(12, sleepTimeMs);

			TestSupport.Log(ConsoleColor.DarkGreen, "\nAll threads have finished");
		}
	}
}