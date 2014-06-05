using System;

namespace ConcurrencyUtilities
{
	// A reader-writer can be used to manage permisson between threads that need read access or write access to something. Multiple threads can read something simultaneously with no problem, but when a thread needs to write to it, that thread must have sole access.
	// A reader-write allows simultaneously either:
	// - One writer; or
	// - Any number of readers
	// This version of ReaderWriter is incomplete due to not giving readers and writers fair treatment -- a writer has to wait for there to be no readers before it gets its turn
	public class ReaderWriter_incomplete
	{
		Semaphore _permisson; // Or use a mutex since it's a simple lock only requiring one simultaneous access
		LightSwitch _lightSwitch;

		public ReaderWriter_incomplete() {
			_permisson = new Semaphore(1);
			_lightSwitch = new LightSwitch(_permisson);
		}

		// Acquire read permisson - join the reading group
		public void ReaderAcquire() {
			_lightSwitch.Acquire();
		}

		// Release read permisson - leave the reading group
		public void ReaderRelease() {
			_lightSwitch.Release();
		}

		// Acquire write permisson - ensure sole access
		public void WriterAcquire() {
			_permisson.Acquire();
		}

		// Release write permisson - make permission free for all to take
		public void WriterRelease() {
			_permisson.Release();
		}
	}

	// // An unbiased reader-writer. Provides fair access for writers as it does for readers
	// // TODO: finish
	// public class ReaderWriter
	// {
	//   readonly Mutex _myLock;
	//   readonly LightSwitch _readers;
	//   readonly Mutex _writerTS;
	//   readonly Mutex _readerTS;
	//   readonly Mutex _accessToNumReadersWaiting;
	//   int _numReadersWaiting;
	//   bool _wasJustWriter; // TODO: determine if this should be used

	//   public ReaderWriter() {
	//     _myLock = new Mutex();
	//     _readers = new LightSwitch(_myLock);
	//     _writerTS = new Mutex();
	//     _readerTS = new Mutex();
	//     _numReadersWaiting = 0;
	//     _accessToNumReadersWaiting = new Mutex();
	//     _wasJustWriter = false;
	//   }

	//   public void ReaderAcquire() {
	//     // Go through the turnstile
	//     _writerTS.Acquire();
	//     _writerTS.Release();

	//     // Increment the number of readers waiting for read permisson
	//     _accessToNumReadersWaiting.Acquire();
	//       _numReadersWaiting++;
	//     _accessToNumReadersWaiting.Release();

	//     // Acquire read permisson using the lightswitch (for readers)
	//     _readers.Acquire();

	//     // Decrement the number of readers waiting for read permisson -- it has now been obtained
	//     _accessToNumReadersWaiting.Acquire();
	//       _numReadersWaiting--;
	//       Monitor.PulseAll(SOMETHING); // ???
	//     _accessToNumReadersWaiting.Release();
	//   }

	//   public void ReaderRelease() {
	//     // Release read permisson using the lightswitch (for readers)
	//     _readers.Release();
	//   }

	//   public void WriterAcquire() {
	//     _readerTS.Acquire();
	//     _writerTS.Acquire();
	//       _myLock.Acquire();
	//       _wasJustWriter = true;
	//     _writerTS.Release();
	//   }

	//   public void WriterRelease() {
	//     _myLock.Release();
	//     lock (SOMETHING) { // ???
	//       if (_numReadersWaiting > 0) // ???
	//         Monitor.Wait(SOMETHING); // ???
	//     }
	//     _readerTS.Release();
	//   }
	// }
}

