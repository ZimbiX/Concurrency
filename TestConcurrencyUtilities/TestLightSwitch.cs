using System;
using ConcurrencyUtilities;
using Semaphore = ConcurrencyUtilities.Semaphore;
using Mutex = ConcurrencyUtilities.Mutex;
using System.Collections.Generic;
using System.Threading;

namespace TestConcurrencyUtilities
{
	public class TestLightSwitch
	{
		static Semaphore _permission;
		static LightSwitch _lightSwitch;
		static int _sleepTime;
		static int _magnitude;
		static Mutex _lightSwitchReleaseDelayer;

		public static void VisitRoomAsGroupMember() {
			TestSupport.DebugThread("{yellow}Acq");
			_lightSwitch.Acquire();
			TestSupport.DebugThread("{green}Acq'd");
			_lightSwitchReleaseDelayer.Acquire();
				TestSupport.SleepThread(_sleepTime);
				_lightSwitch.Release();
				TestSupport.DebugThread("{cyan}Rel'd");
			_lightSwitchReleaseDelayer.Release();
		}

		public static void VisitRoomAsIndividual() {
			TestSupport.SleepThread(_sleepTime, false);
			TestSupport.DebugThread("{yellow}Acq");
			_permission.Acquire();
			TestSupport.DebugThread("{green}Acq'd");
		}
		
		public static void Run(int magnitude, int sleepTime) {
			_sleepTime = sleepTime;
			_magnitude = magnitude;
			_permission = new Semaphore(1);
			_lightSwitch = new LightSwitch(_permission);
			_lightSwitchReleaseDelayer = new Mutex();

			TestSupport.Log(ConsoleColor.Blue, "Light Switch test\n==============================");

			TestSupport.Log(ConsoleColor.Blue, "\nA group of " + _magnitude + " threads will use the light switch " +
				"to gain access from the permission semaphore, then another thread will attempt to gain access " +
				"directly from the permission sempahore. This test will demonstrate that this individual thread " +
				"will have to wait until all the threads using the light switch have released the permission.\n");

			List<Thread> threads = new List<Thread>();
			int columnWidth = 6+1;
			threads.AddRange( TestSupport.CreateThreads(VisitRoomAsGroupMember, "LS", _magnitude, 1, columnWidth, 1) );
			threads.AddRange( TestSupport.CreateThreads(VisitRoomAsIndividual, "D", 1, 1, columnWidth, _magnitude + 1) );
			TestSupport.EndColumnHeader(_magnitude + 1, columnWidth); // End the column header line
			TestSupport.RunThreads(threads);
		}
	}
}