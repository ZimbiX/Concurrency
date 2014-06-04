using System;
using ConcurrencyUtilities;
using Semaphore = ConcurrencyUtilities.Semaphore;
using Mutex = ConcurrencyUtilities.Mutex;
using System.Collections.Generic;
using System.Threading;

namespace TestConcurrencyUtilities
{
	public class TestExchanger
	{
		static int _sleepTime;
		static Exchanger _Exchanger;

		static void AttendExchangeAs(char agentCodename, int sleepTimeBeforeExchange = 0) {
			TestSupport.SleepThread(sleepTimeBeforeExchange);
			TestSupport.DebugThread("is arriving at the exchange");
			switch (agentCodename) {
			case 'A':
				_Exchanger.AArrive();
				break;
			case 'B':
				_Exchanger.BArrive();
				break;
			}
			TestSupport.DebugThread("has left the exchange");
		}

		static void AttendExchangeAsA() {
			AttendExchangeAs('A');
		}
		
		static void AttendExchangeAsB() {
			AttendExchangeAs('B', _sleepTime);
		}

		public static void Run(int sleepTime) {
			_sleepTime = sleepTime;
			_Exchanger = new Exchanger();

			TestSupport.Log(ConsoleColor.Blue, "Exchanger test\n==============================");

			TestSupport.Log(ConsoleColor.Blue, "\nTwo secret agent threads will arrive at an exchange (rendezvous), " +
				"waiting for the other to arrive before parting ways.\n");
			TestSupport.SleepThread(_sleepTime);

			List<Thread> threads = new List<Thread>();
			threads.AddRange( TestSupport.CreateThreads(AttendExchangeAsA, "Agent A", 1) );
			threads.AddRange( TestSupport.CreateThreads(AttendExchangeAsB, "Agent B", 1) );
			TestSupport.RunThreads(threads);
		}
	}
}