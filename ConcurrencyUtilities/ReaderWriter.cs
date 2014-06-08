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
	// Status: complete, test complete, TODO: get marked off
	public class ReaderWriter
	{
		Mutex _roomEmpty;
		LightSwitch _readSwitch;
		Mutex _turnstile;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrencyUtilities.ReaderWriter"/> class.
		/// </summary>
		public ReaderWriter() {
			_roomEmpty = new Mutex(); /* Whether the room is empty -- effectively the core permission managed 
			                           by the reader-writer lock */
			_readSwitch = new LightSwitch(_roomEmpty); /* Keeps track of how many readers are in the room (and so 
			                                            whether the room is empty) */
			_turnstile = new Mutex(); // A turnstile that controls whether new readers or writers can enter the room
		}

		/// <summary>
		/// Acquire read permisson -- join the reading group
		/// </summary>
		public void ReaderAcquire() {
			// Wait for there to be no writers in the room
			_turnstile.Acquire();
			_turnstile.Release();

			_readSwitch.Acquire(); // Enter the room using the readers' light switch (which manages _roomEmpty)

			// Critical section begins
		}
		
		/// <summary>
		/// Release read permisson -- leave the reading group
		/// </summary>
		public void ReaderRelease() {
			// Critical section ends

			_readSwitch.Release(); // Leave the room using the readers' light switch (which manages _roomEmpty)
		}

		/// <summary>
		/// Acquire write permisson -- ensure sole access
		/// </summary>
		public void WriterAcquire() {
			_turnstile.Acquire(); // Prevent new readers or writers from entering the room
			_roomEmpty.Acquire(); // Wait for there to be no readers or writers in the room
			
			// Critical section begins
		}
		
		/// <summary>
		/// Release write permisson -- make permission free for all to take
		/// </summary>
		public void WriterRelease() {
			// Critical section ends

			_turnstile.Release(); // Allow new readers or writers to enter the room
			_roomEmpty.Release(); // Signal that the room is empty
		}
	}
}