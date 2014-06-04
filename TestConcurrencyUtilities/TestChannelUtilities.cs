using System;
using System.Threading;
using ConcurrencyUtilities;

namespace TestConcurrencyUtilities
{
	// Provide utilities that can be used by the testing classes that test my Channel and BoundChannel classes
	public class TestChannelUtilities
	{
		protected static int _sleepTime;
		protected static int _testMagnitude;

		protected static Channel<string> _channel;

		protected static void ChannelPut() {
			TestSupport.DebugThread("will now attempt to put to the channel");
			string item = "Data from " + Thread.CurrentThread.Name;
			_channel.Put(item); // Lock (wait); put the item onto the channel; sleep; unlock; return
			TestSupport.DebugThread("    ", "enqueued: \"" + item + '"');
		}

		protected static void ChannelTake() {
			TestSupport.DebugThread("will now attempt to take from the channel");
			string item = _channel.Take(); // Lock (wait); take the item from the channel; sleep; unlock; return the item
			TestSupport.DebugThread("        ", "has dequeued: \"" + item + '"');
		}
	}
}