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
			TestSupport.DebugThread("is attempting to visit the room as a group member " +
				"(accessing the permission through the light switch)");
			_lightSwitch.Acquire();
			TestSupport.DebugThread("acquired permission");
			_lightSwitchReleaseDelayer.Acquire();
				TestSupport.SleepThread(_sleepTime);
				_lightSwitch.Release();
				TestSupport.DebugThread("released permission");
			_lightSwitchReleaseDelayer.Release();
		}

		public static void VisitRoomAsIndividual() {
			TestSupport.SleepThread(_sleepTime, false);
			TestSupport.DebugThread("is attempting to visit the room as an individual " +
				"(acquring the permisson directly from the semaphore)");
			_permission.Acquire();
			TestSupport.DebugThread("acquired permission");
		}
		
		public static void Run(int magnitude, int sleepTime) {
			_sleepTime = sleepTime;
			_magnitude = magnitude;
			_permission = new Semaphore(1);
			_lightSwitch = new LightSwitch(_permission);
			_lightSwitchReleaseDelayer = new Mutex();

			TestSupport.Log(ConsoleColor.Blue, "Light Switch test\n==============================");

			TestSupport.Log(ConsoleColor.Blue, "\nA group of " + _magnitude + " threads will use the light switch " +
				"to gain access to the permission semaphore, then another thread will attempt to gain access " +
				"to the permission sempahore. This test will demonstrate that this will not be possible " +
				"until all the threads using the light switch have released the permission.\n");

			List<Thread> threads = new List<Thread>();
			threads.AddRange( TestSupport.CreateThreads(VisitRoomAsGroupMember, "LS group member thread", _magnitude) );
			threads.AddRange( TestSupport.CreateThreads(VisitRoomAsIndividual, "Independent thread", 1) );
			TestSupport.RunThreads(threads);
		}
	}
}