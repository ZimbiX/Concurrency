using System;
using System.Threading; // Required for access to Thread

namespace ConcurrencyUtilities
{
	/// <summary>
	/// Reader-writer lock based on Andrew's pseudocode from class.
	/// Enforces the constraint that a writer must wait for all the readers who arrived before it.
	/// A reader-writer lock can be used to manage permisson between threads that need read access or write access
	/// to something. Multiple threads can read something simultaneously with no problem, but when a thread
	/// needs to write to it, that thread must have sole access.
	/// A reader-writer lock allows simultaneously either:
	/// - One writer; or
	/// - Any number of readers
	/// This version of ReaderWriter gives readers and writers fairer treatment (than a writer being blocked out by
	/// having to wait for there to be no readers, while there actually isn't a gap where there aren't any readers).
	/// </summary>
	// Status: complete, TODO: finalise test, TODO: get marked off
	public class ReaderWriter
	{
		Mutex _corePermission;                 // The core permisson that the readers are writers are gaining access to
		LightSwitch _readersCorePremissonLS;   /* The lightswitch used by the readers to collectively gain access to
		                                        * the core permission */
		Mutex _readerTS;                       /* A turnstile that readers must immediately go through upon
		                                        * requesting access. It allows a writer to block new readers while it
		                                        * waits for existing readers to finish (before it can acquire the
		                                        * core permission) */
		Mutex _writerMutex;                    /* A mutex ensuring that only one writer can attempt to acquire
		                                        * permission at once */
		int _numReadersWaitingForAcquire;      /* The number of readers that have passed _writerTS, and are/will be
		                                        * waiting for their lightswitch to provide access to the
		                                        * core permission */
		Object _lockForNumReadersWaitingForAcquire; // The lock object protecting '_numReadersWaitingForAcquire'
		// bool wasJustWriter;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrencyUtilities.ReaderWriter"/> class.
		/// </summary>
		public ReaderWriter() {
			_corePermission = new Mutex();
			_readersCorePremissonLS = new LightSwitch(_corePermission);
			_readerTS = new Mutex();
			_writerMutex = new Mutex();
			_numReadersWaitingForAcquire = 0;
			_lockForNumReadersWaitingForAcquire = new Object();
		}

		/// <summary>
		/// Acquires an instance of the core permission through the readers light switch (read access to be held).
		/// </summary>
		public void ReaderAcquire() {
			/* Go through the readers turnstile -- if blocked, wait until the writer that blocked us has acquired
			 * the permission (allows the writer to wait for only the existing readers to finish) */
			_readerTS.Acquire();
			_readerTS.Release();
			// Increment the readers waiting count
			lock (_lockForNumReadersWaitingForAcquire) {
				_numReadersWaitingForAcquire++;
			}
			_readersCorePremissonLS.Acquire(); // Acquire a core permission instance
				// Decrement the readers waiting count, since we now have an instance of the permission
				lock (_lockForNumReadersWaitingForAcquire) {
					_numReadersWaitingForAcquire--;
					/* Signal to the waiting writer that a reader has now acquired the permission (that the writer had
					 * just released) */
					Monitor.PulseAll(_lockForNumReadersWaitingForAcquire);
				}
				// Critical section begins
		}

		/// <summary>
		/// Releases the instance of the core permission through the readers light switch (the read access that
		/// was held).
		/// </summary>
		public void ReaderRelease() {
				// Critical section ends
			_readersCorePremissonLS.Release(); // Release the core permission instance
		}

		/// <summary>
		/// Acquires the core permission (write access to be held).
		/// </summary>
		public void WriterAcquire() {
			_writerMutex.Acquire(); // Block new writers (by acquiring the writer mutex)
				// Jump in line after the current readers, making new readers wait until we've acquired the permission
				_readerTS.Acquire(); // Block new readers (but let existing readers continue/finish)
					_corePermission.Acquire(); // Acquire the core permission (waiting for existing readers to finish)
					// wasJustWriter = true;
				_readerTS.Release(); // Unblock new readers (who now have to wait for us to release the core permission)
				// _corePermission retained
					// Critical section begins
		}

		/// <summary>
		/// Releases the core permission (the write access that was held).
		/// After releasing the core permission, if there are waiting readers, wait until one of them acquires
		/// the permission before we unblock new writers (by releasing the writer mutex). This is to ensure that
		/// readers get the next turn if there were any waiting (enforcing the constraint that "after each writer,
		/// all queued readers progress through before the next queued writer")
		/// </summary>
		public void WriterRelease() {
					// Critical section ends
				_corePermission.Release(); // Release the core permission (permitting waiting readers to grab it)
				// Wait for a waiting reader to acquire the permission, ensuring they get their turn next
				lock (_lockForNumReadersWaitingForAcquire) {
					if (_numReadersWaitingForAcquire > 0)
						Monitor.Wait(_lockForNumReadersWaitingForAcquire);
				}
			_writerMutex.Release(); // Unblock new writers (by releasing the writer mutex)
		}
	}
}