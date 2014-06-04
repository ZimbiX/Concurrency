using System;
using ConcurrencyUtilities;
using Semaphore = ConcurrencyUtilities.Semaphore;
using Mutex = ConcurrencyUtilities.Mutex;
using System.Collections.Generic;
using System.Threading;

namespace TestConcurrencyUtilities
{
	public class TestRendezvous
	{
		static int _sleepTime;
		static Rendezvous _rendezvous;

		static void AttendRendezvousAs(char agentCodename, int sleepTimeBeforeRendezvous = 0) {
			TestSupport.SleepThread(sleepTimeBeforeRendezvous);
			TestSupport.DebugThread("is arriving at the rendezvous");
			switch (agentCodename) {
			case 'A':
				_rendezvous.AArrive();
				break;
			case 'B':
				_rendezvous.BArrive();
				break;
			}
			TestSupport.DebugThread("has left the rendezvous");
		}

		static void AttendRendezvousAsA() {
			AttendRendezvousAs('A');
		}
		
		static void AttendRendezvousAsB() {
			AttendRendezvousAs('B', _sleepTime);
		}

		public static void Run(int sleepTime) {
			_sleepTime = sleepTime;
			_rendezvous = new Rendezvous();

			TestSupport.Log(ConsoleColor.Blue, "Rendezvous test\n==============================");

			TestSupport.Log(ConsoleColor.Blue, "\nTwo secret agent threads will arrive at a rendezvous, waiting " +
				"for the other to arrive before parting ways.\n");
			TestSupport.SleepThread(_sleepTime);

			List<Thread> threads = new List<Thread>();
			threads.AddRange( TestSupport.CreateThreads(AttendRendezvousAsA, "Agent A", 1) );
			threads.AddRange( TestSupport.CreateThreads(AttendRendezvousAsB, "Agent B", 1) );
			TestSupport.RunThreads(threads);
		}
	}
}