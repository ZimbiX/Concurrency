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
			string item = TestSupport.ThreadName();
			TestSupport.DebugThread("{yellow}Tx:" + item + "...");
			_channel.Put(item); // Lock; put the item onto the channel; unlock; return
			TestSupport.DebugThread("{green}Tx:" + item);
		}

		protected static void ChannelTake() {
			TestSupport.DebugThread("{yellow}Rx...");
			string item = _channel.Take(); // Lock; take the item from the channel; unlock; return the item
			TestSupport.DebugThread("{green}Rx:" + item);
		}
	}
}