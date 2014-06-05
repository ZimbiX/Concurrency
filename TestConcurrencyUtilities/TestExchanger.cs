using System;
using ConcurrencyUtilities;
using Semaphore = ConcurrencyUtilities.Semaphore;
using Mutex = ConcurrencyUtilities.Mutex;
using System.Collections.Generic;
using System.Threading;
using AnsiColor;

namespace TestConcurrencyUtilities
{
	public class TestExchanger
	{
		static int _sleepTime;
		static int _magnitude;
		static Exchanger<string> _exchanger;

		static void AttendExchange() {
			string intelToGive = "I-" + TestSupport.ThreadName();
			TestSupport.DebugThread("{yellow}Tx:"+intelToGive);
			string intelReceived = _exchanger.Arrive(intelToGive);
			if (intelReceived != intelToGive)
				TestSupport.DebugThread("{green}Rx:"+intelReceived);
			else
				TestSupport.DebugThread("ERROR:Rx:"+intelReceived);
		}
		
		static void AttendExchangeAfterDelay() {
			TestSupport.SleepThread(_sleepTime);
			AttendExchange();
		}

		public static void Run(int magnitude, int sleepTime) {
			_magnitude = magnitude;
			_sleepTime = sleepTime;
			_exchanger = new Exchanger<string>();

			TestSupport.Log(ConsoleColor.Blue, "Exchanger test\n==============================");

			TestSupport.Log(ConsoleColor.Blue, "\nTwo secret agent threads will arrive at an exchange, " +
			                "waiting for the other to arrive before parting ways (rendezvous).\n");
			TestSupport.SleepThread(_sleepTime);

			List<Thread> threads = new List<Thread>();
			threads.AddRange( TestSupport.CreateThread(AttendExchange,           "Agent A") );
			threads.AddRange( TestSupport.CreateThread(AttendExchangeAfterDelay, "Agent B") );
			TestSupport.RunThreads(threads);

			TestSupport.Log(ConsoleColor.Blue, "\nMany secret agent threads will arrive at an exchange, " +
			                "waiting for the other to arrive before parting ways (rendezvous).\n");

			threads = new List<Thread>();
			threads.AddRange( TestSupport.CreateThreads(AttendExchange, "A", _magnitude, 0, 7+1, 1) );
			TestSupport.EndColumnHeader(_magnitude, 7+1); // End the column header line
			TestSupport.RunThreads(threads);
		}
	}
}