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

			TestSupport.Log(ConsoleColor.Blue, "\nWriting data to the channel (Put)...");
			threads = TestSupport.CreateThreads(ChannelPut, "P", _testMagnitude, 0, 5+1, 1);
			TestSupport.EndColumnHeader(_testMagnitude, 5+1); // End the column header line
			TestSupport.SleepThread(_sleepTime);
			TestSupport.RunThreads(threads);

			TestSupport.Log(ConsoleColor.Blue, "\nReading data from the channel (Take)...");
			threads = TestSupport.CreateThreads(ChannelTake, "T", _testMagnitude, 0, 5+1, 1);
			TestSupport.EndColumnHeader(_testMagnitude, 5+1); // End the column header line
			TestSupport.SleepThread(_sleepTime);
			TestSupport.RunThreads(threads);
		}
	}
}