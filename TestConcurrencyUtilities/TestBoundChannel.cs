using System;
using ConcurrencyUtilities;
using System.Collections.Generic;
using System.Threading;

namespace TestConcurrencyUtilities
{
	// Test my BoundChannel class
	public class TestBoundChannel: TestChannelUtilities
	{
		static int _takeThreadsDelayTime;

		public static void Run(int testMagnitude, int upperLimit, int sleepTime = 0) {
			_testMagnitude = testMagnitude;
			_sleepTime = sleepTime;
			_channel = new BoundChannel<string>(upperLimit);

			_takeThreadsDelayTime = _sleepTime * 5;

			List<Thread> threads;

			TestSupport.Log(ConsoleColor.Blue, "Bound channel test\n==============================");
			TestSupport.Log(ConsoleColor.Blue, "\nEach thread will:" +
				               "\n- Complete its action (Put or Take)" +
			                   "\n- Sleep (still locked) for " + TestSupport.StringFromMilliseconds(_sleepTime) +
			                   "\n- Log what happened to the console\n" +
			                   "\nLogging will lag behind what's happening, but should still accurately reflect it" +
			                   "\nThe take threads will start after a delay of " + TestSupport.StringFromMilliseconds(_takeThreadsDelayTime));

			Console.WriteLine("\nStarting Put threads...");
			threads = TestSupport.CreateThreads(ChannelPut, "BoundChannelPutThread", _testMagnitude);
			threads.Add(new Thread(DelayChannelTakeThreads));
			TestSupport.RunThreads(threads);
		}

		public static void DelayChannelTakeThreads() {
			List<Thread> channelTakeThreads = TestSupport.CreateThreads(ChannelTake, "BoundChannelTakeThread", _testMagnitude);
			TestSupport.SleepThread(_takeThreadsDelayTime, false);
			Console.WriteLine("\nStarting Take threads...");
			TestSupport.RunThreads(channelTakeThreads);
		}
	}
}