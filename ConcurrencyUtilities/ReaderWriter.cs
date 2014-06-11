using System;
using System.Threading; // Required for access to Thread

namespace ConcurrencyUtilities
{
	/// <summary>
	/// Reader writer lock based on Andrew's pseudocode from class.
	/// </summary>
	// Status: TODO: further understanding and add documentation, TODO: finalise test, TODO: get marked off
	public class ReaderWriter
	{
		Mutex _corePermission;
		LightSwitch _readersCorePremissonLS;
		Mutex _writerTS;
		Mutex _readerTS;
		int _numReadersWaitingForAcquire;
		Object _lockForNumReadersWaitingForAcquire;
		// bool wasJustWriter;

		public ReaderWriter() {
			_corePermission = new Mutex();
			_readersCorePremissonLS = new LightSwitch(_corePermission);
			_writerTS = new Mutex();
			_readerTS = new Mutex();
			_numReadersWaitingForAcquire = 0;
			_lockForNumReadersWaitingForAcquire = new Object();
		}
		
		public void ReaderAcquire() {
			_writerTS.Acquire();
			_writerTS.Release();
			lock (_lockForNumReadersWaitingForAcquire) {
				_numReadersWaitingForAcquire++;
			}
			_readersCorePremissonLS.Acquire(); // Acquire a core permission instance
				lock (_lockForNumReadersWaitingForAcquire) {
					_numReadersWaitingForAcquire--;
					Monitor.PulseAll(_lockForNumReadersWaitingForAcquire);
				}
				// Critical section begins
		}

		public void ReaderRelease() {
				// Critical section ends
			_readersCorePremissonLS.Release(); // Release the core permission instance
		}

		public void WriterAcquire() {
			_readerTS.Acquire();
				_writerTS.Acquire();
					_corePermission.Acquire();
					// wasJustWriter = true;
				_writerTS.Release();
				// _corePermission retained
					// Critical section begins
		}

		public void WriterRelease() {
					// Critical section ends
				_corePermission.Release();
				lock (_lockForNumReadersWaitingForAcquire) {
					if (_numReadersWaitingForAcquire > 0)
						Monitor.Wait(_lockForNumReadersWaitingForAcquire);
				}
			_readerTS.Release();
		}
	}
}