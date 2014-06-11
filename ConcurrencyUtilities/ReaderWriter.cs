using System;
using System.Threading;
using Colorizer = AnsiColor.AnsiColor;

namespace ConcurrencyUtilities
{
	/// <summary>
	/// A reader-writer can be used to manage permisson between threads that need read access or write access
	/// to something. Multiple threads can read something simultaneously with no problem, but when a thread
	/// needs to write to it, that thread must have sole access.
	/// A reader-writer allows simultaneously either:
	/// - One writer; or
	/// - Any number of readers
	/// This version of ReaderWriter gives readers and writers fairer treatment (than a writer being blocked out by
	/// having to wait for there to be no readers, while there actually isn't a gap where there aren't any readers).
	/// When a writer wants to enter the room, new readers (and wrtiers) are blocked from doing so. When the room
	/// becomes empty, the writer progresses into the room, does its business, and leaves, opening up access to new
	/// readers and writers again.
	/// Readers use a light switch to access the room. The '_roomEmpty' core permission is managed by this light switch.
	/// </summary>
	// Status: TODO: make writer block new writers from queueing, test complete, TODO: get marked off
	public class ReaderWriter
	{
		Mutex _roomEmpty;
		LightSwitch _readSwitch;
		Mutex _readerTS;
		Mutex _writerTS;
		LightSwitch _writerTSSwitch;
		Mutex _writerMutex;
		bool _internalTesting;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrencyUtilities.ReaderWriter"/> class.
		/// </summary>
		public ReaderWriter(bool internalTesting = false) {
			_roomEmpty = new Mutex(); /* Whether the room is empty -- effectively the core permission managed 
			                           by the reader-writer lock */
			_readSwitch = new LightSwitch(_roomEmpty); /* Keeps track of how many readers are in the room (and so 
			                                            whether the room is empty) */
			_readerTS = new Mutex(); // A turnstile that controls whether new readers or writers can enter the room
			_writerTS = new Mutex(); // A turnstile that controls whether new readers or writers can enter the room
			_writerTSSwitch = new LightSwitch(_writerTS);
			_writerMutex = new Mutex();
			_internalTesting = internalTesting;
		}

		void DebugThread(string message, bool useColumn = true) {
			if (_internalTesting) {
				// Prepare for console logging in columns -- used if _isTesting
				string threadColumnOffset;
				if (useColumn && Thread.CurrentThread.Name != null) {
					string threadNameWithoutPrefix = Thread.CurrentThread.Name.Replace("%%%%", "");
					threadColumnOffset = threadNameWithoutPrefix.Replace(threadNameWithoutPrefix.TrimStart(' '), "");
				} else {
					threadColumnOffset = "";
				}
				Console.WriteLine(threadColumnOffset + Colorizer.Colorize(message));
			}
		}

		// Make the current thread sleep for a specified number of milliseconds, logging the fact that it is sleeping, and what its name is
		private static void SleepThreadWithMessage(int msSleepTime, string sleepMessage) {
			if (msSleepTime > 0) {
				int sleepTimePreLog = 130;
				int sleepTimeA = (msSleepTime > sleepTimePreLog ? sleepTimePreLog               : (int)Math.Round((double)msSleepTime / 2));
				int sleepTimeB = (msSleepTime > sleepTimePreLog ? msSleepTime - sleepTimePreLog : (int)Math.Round((double)msSleepTime / 2));
				Thread.Sleep(sleepTimeA);
				Console.Write(Colorizer.Colorize(sleepMessage)); // Now that any logging from other threads should be finished, print a line indicating that we're sleeping
				Thread.Sleep(sleepTimeB);
			}
		}

		public static void SleepThread(int msSleepTime, bool newLineWithEllipsis = true) {
			SleepThreadWithMessage(msSleepTime, newLineWithEllipsis ? "{white}...\n" : "");
		}

		/// <summary>
		/// Acquire read permisson -- join the reading group
		/// </summary>
		public void ReaderAcquire() {
			DebugThread("{yellow}_writerTS.Acq");
			_writerTSSwitch.Acquire(); // Block new writers from starting an acquire (proceeding any further than blocking new readers)
			DebugThread("{yellow}_writerTS.Acq");

			// Wait for there to not be a writer: waiting for the readers in the room to leave, or in the room
			DebugThread("{yellow}_readerTS.Acq");
			_readerTS.Acquire();
			_readerTS.Release();
			DebugThread("{green}_readerTS.Acq,Rel");

			SleepThread(5000, false);

				DebugThread("{yellow}_readSwitch.Acq");
				// Acquire a permisson instance from the light switch
				_readSwitch.Acquire(); // Enter the room using the readers' light switch (which manages _roomEmpty)
					DebugThread("{green}_readSwitch.Acq");
			
			_writerTSSwitch.Release();
			DebugThread("{yellow}_writerTS.Rel");

				// Critical section begins
		}
		
		/// <summary>
		/// Release read permisson -- leave the reading group
		/// </summary>
		public void ReaderRelease() {
				// Critical section ends
			// Release the permisson instance provided by the light switch
			_readSwitch.Release(); // Leave the room using the readers' light switch (which manages _roomEmpty)
			DebugThread("{green}_readSwitch.Rel");
		}

		/// <summary>
		/// Acquire write permisson -- ensure sole access
		/// </summary>
		public void WriterAcquire() {
			DebugThread("{yellow}_writerTS.Acq");
			_writerTS.Acquire(); // Wait for any readers in the room to leave
			_writerTS.Release();
			DebugThread("{yellow}_writerTS.Acq,Rel");

			_writerMutex.Acquire(); // Block new writers from passing this point

			DebugThread("{yellow}_readerTS.Acq");
			_readerTS.Acquire(); // Block new readers from starting an acquire (proceeding any further than blocking new writers)
			DebugThread("{green}_readerTS.Acq");
			
			DebugThread("{yellow}_writerMutex.Acq");
				DebugThread("{green}_writerMutex.Acq");
					// Acquire the permission
					DebugThread("{yellow}_roomEmpty.Acq");
					_roomEmpty.Acquire(); // Wait for there to be no readers or writers in the room
					DebugThread("{green}_roomEmpty.Acq");
				_readerTS.Release(); // Allow new readers to start an acquire, and queue on their LS for entry into the room when we leave it
				DebugThread("{green}_readerTS.Rel");

				// Critical section begins
		}
		
		/// <summary>
		/// Release write permisson -- make permission free for all to take
		/// </summary>
		public void WriterRelease() {
				// Critical section ends

			// Release the permisson
			_roomEmpty.Release(); // Signal that the room is empty
			DebugThread("{green}_roomEmpty.Rel");

			_writerMutex.Release(); // Allow new writers to queue for entry into the room
			DebugThread("{green}_writerMutex.Rel");
		}
	}
}