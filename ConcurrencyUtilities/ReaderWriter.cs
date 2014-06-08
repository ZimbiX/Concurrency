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
	/// there to be no readers before getting its turn).
	/// </summary>
	// Status: TODO: finish, TODO: update test, TODO: get marked off
	public class ReaderWriter
	{
		Mutex _permisson;
		LightSwitch _lightSwitch;

		public ReaderWriter() {
			_permisson = new Mutex();
			_lightSwitch = new LightSwitch(_permisson);
		}

		//		private void DebugThread(string message) {
		//			string threadNameWithoutPrefix = Thread.CurrentThread.Name.Replace("%%%%","");
		//			string threadColumnOffset = threadNameWithoutPrefix.Replace(threadNameWithoutPrefix.TrimStart(' '), "");
		//			Console.WriteLine(threadColumnOffset + message);
		//		}

		// Acquire read permisson - join the reading group
		public void ReaderAcquire() {
			// DebugThread("Wait:...");
			// ....
		}

		// Acquire write permisson - ensure sole access
		public void WriterAcquire() {
			// ....
			_permisson.Acquire();
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