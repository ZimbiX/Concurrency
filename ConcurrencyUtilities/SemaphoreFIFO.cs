using System;
using System.Threading; // Required for access to Thread
using System.Collections.Generic; // Required for access to List

namespace ConcurrencyUtilities
{
	// TODO: describe
	// Status: TODO: check if complete, TODO: check if test complete, TODO: get marked off
	public class SemaphoreFIFO
	{
		protected int _numTokens; // The number of tokens held by the semaphore
		protected Object _lockObjectForAccessToFields = new Object(); // The object to be locked - determines whether _numTokens can be accessed. Avoids using 'lock this', to prevent things screwing up if some idiot locks using this semaphore as the lock object
		protected Channel<Object> _threadQueue;
		protected int _numThreadsQueued;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrencyUtilities.Semaphore"/> class.
		/// </summary>
		/// <param name="tokens">The number of tokens to start with (0 if unspecified).</param>
		public SemaphoreFIFO(int tokens = 0) {
			_numTokens = tokens;
			_threadQueue = new Channel<object>();
		}

		/// <summary>
		/// Take (acquire) a token from the semaphore.
		/// Decreases the amount of tokens held in the semaphore.
		/// Does not let you specify the number of tokens to acquire since you should never really need to acquire more than one at a time.
		/// </summary>
		public void Acquire() {
			string threadNameWithoutPrefix = Thread.CurrentThread.Name.Replace("%%%%","");
			string threadColumnOffset = threadNameWithoutPrefix.Replace(threadNameWithoutPrefix.TrimStart(' '), "");
			Object threadAwakener = new Object(); // Only used if required
			bool waitingForAwakener = false;
			lock (_lockObjectForAccessToFields) {
				if (_numTokens > 0) {
					_numTokens -= 1;
				} else {
					// Add this thread to the back of the queue
					_numThreadsQueued++;
					Console.WriteLine(threadColumnOffset + "Acq.. O=" + _numThreadsQueued);
					_threadQueue.Put(threadAwakener);
					waitingForAwakener = true;

//					Monitor.Wait(_lockObjectForAccessToFields);
				}
			}
			if (waitingForAwakener) {
				// Wait for this thread to be woken
				lock (threadAwakener) {
					Monitor.Wait(threadAwakener); // Wait for a pulse on this object
				}
				lock (_lockObjectForAccessToFields) {
					_numThreadsQueued--;
					Console.WriteLine(threadColumnOffset + "Acq'd O=" + (10-_numThreadsQueued));
				}
			}
		}

		/// <summary>
		/// Give (release) a number of tokens back to the semaphore.
		/// Increases the amount of tokens held in the semaphore.
		/// </summary>
		/// <param name="n">The number of tokens to release into / give to the semaphore.</param>
		public virtual void Release(int n) {
			if (n < 1)
				throw new System.ArgumentException("Parameter cannot be less than 1", "n");
			lock (_lockObjectForAccessToFields) {
				_numTokens += n;
				if (_numThreadsQueued > 0) {
					Object threadAwakener = _threadQueue.Take();
					lock (threadAwakener) {
						Monitor.PulseAll(threadAwakener); // Pulse on this object
					}
				}
			}
		}

		public virtual void Release() {
			Release(1);
		}
	}
}