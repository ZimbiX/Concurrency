using System;
using System.Threading;

namespace ConcurrencyUtilities
{
	/// <summary>
	/// A reader-writer can be used to manage permisson between threads that need read access or write access
	/// to something. Multiple threads can read something simultaneously with no problem, but when a thread
	/// needs to write to it, that thread must have sole access.
	/// A reader-write allows simultaneously either:
	/// - One writer; or
	/// - Any number of readers
	/// This version of ReaderWriter gives readers and writers fairer treatment (than a writer having to wait for
	/// there to be no readers before getting its turn). Threads are cordoned off into two groups based on the kind of
	/// access that's desired before being let use the permisson. When the group in front empties, the groups behind
	/// shuffle forward.
	/// 
	/// 
	///   Readers         Writers
	///    v v v           v v v
	///    | | |           | | |
	/// ====RTS0====    ====WTS0====   Gate 0
	///    | | |    <-->   | | |                First group -- the opposite type of thread to the second group
	///    | | |   Mut.Ex. | | |
	/// ====RTS1====    ====WTS1====   Gate 1
	///    | | |    <-->   | | |                Second group -- the opposite type of thread to the first group
	///    | | |   Mut.Ex. | | |
	/// ===[RTS2]===    ===[WTS2]===   Implied gate here -- entry into the permission effectively handles this
	///     \ \ \            /
	///       \ \ \         /
	///         \ \ \      /
	///       +--|-|-|----|--+
	///       :  | | |    |  :
	///       :  | | |    |  : Permission
	///       :  | | |    |  :
	///       +--|-|-|----|--+
	///          v v v    v
	/// 
	/// </summary>
	// Status: TODO: check if complete, TODO: check if test complete, TODO: get marked off
	public class ReaderWriter
	{
		Mutex _permisson;
		LightSwitch _lightSwitch;

		Mutex _RTS0;
		Mutex _RTS1;
		Mutex _WTS0;
		Mutex _WTS1;

		LightSwitch _blockRTS0;
		LightSwitch _blockRTS1;
		LightSwitch _blockWTS0;
		LightSwitch _blockWTS1;

		public ReaderWriter() {
			_permisson = new Mutex();
			_lightSwitch = new LightSwitch(_permisson);

			_RTS0 = new Mutex();
			_RTS1 = new Mutex();
			_WTS0 = new Mutex();
			_WTS1 = new Mutex();

			_blockRTS0 = new LightSwitch(_RTS0);
			_blockRTS1 = new LightSwitch(_RTS1);
			_blockWTS0 = new LightSwitch(_WTS0);
			_blockWTS1 = new LightSwitch(_WTS1);
		}
		
//		private void DebugThread(string message) {
//			string threadNameWithoutPrefix = Thread.CurrentThread.Name.Replace("%%%%","");
//			string threadColumnOffset = threadNameWithoutPrefix.Replace(threadNameWithoutPrefix.TrimStart(' '), "");
//			Console.WriteLine(threadColumnOffset + message);
//		}

		// Acquire read permisson - join the reading group
		public void ReaderAcquire() {
//			DebugThread("Wait:RTS0");
			_RTS0.Acquire();
			_RTS0.Release();
//			DebugThread("Pass:RTS0");

//			DebugThread("Want:bWTS0");
			_blockWTS0.Acquire();
//			DebugThread("Have:bWTS0");

//			DebugThread("Wait:RTS1");
			_RTS1.Acquire();
			_RTS1.Release();
//			DebugThread("Pass:RTS1");

			_blockWTS1.Acquire();
			_blockWTS0.Release();

			_lightSwitch.Acquire();
			_blockWTS1.Release();
		}

		// Acquire write permisson - ensure sole access
		public void WriterAcquire() {
			_WTS0.Acquire();
			_WTS0.Release();

			_blockRTS0.Acquire();

			_WTS1.Acquire();
			_WTS1.Release();

			_blockRTS1.Acquire();
			_blockRTS0.Release();

			_permisson.Acquire();
			_blockRTS1.Release();
		}

		// Release read permisson - leave the reading group
		public void ReaderRelease() {
			_lightSwitch.Release();

		}

		// Release write permisson - make permission free for all to take
		public void WriterRelease() {
			_permisson.Release();
		}
	}
}