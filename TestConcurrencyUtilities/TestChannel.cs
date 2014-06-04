using System;
using System.Collections.Generic;
using ConcurrencyUtilities;
using System.Threading;

namespace TestConcurrencyUtilities
{
	// Test my Channel class
	public class TestChannel: TestChannelUtilities
	{
		public static void Run(int testMagnitude, int sleepTime = 0) {
			_testMagnitude = testMagnitude;
			_sleepTime = sleepTime;
			_channel = new Channel<string>();

			List<Thread> threads;

			TestSupport.Log(ConsoleColor.Blue, "Channel test\n==============================");
			Console.WriteLine("\nThreads will complete their action (Put or Take) and then sleep (still locked) for " + TestSupport.StringFromMilliseconds(_sleepTime));

			TestSupport.Log(ConsoleColor.Blue, "\nWriting data to the channel...");
			threads = TestSupport.CreateThreads(ChannelPut, "ChannelPutThread", _testMagnitude);
			TestSupport.SleepThread(_sleepTime);
			TestSupport.RunThreads(threads);

			TestSupport.Log(ConsoleColor.Blue, "\nReading data from the channel...");
			threads = TestSupport.CreateThreads(ChannelTake, "ChannelTakeThread", _testMagnitude);
			TestSupport.SleepThread(_sleepTime);
			TestSupport.RunThreads(threads);
		}
	}
}