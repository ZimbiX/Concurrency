using System;
using System.Threading; // Required for access to Thread
using System.Collections.Generic; // Required for access to List
using Colorizer = AnsiColor.AnsiColor;

namespace ConcurrencyUtilities
{
	/// <summary>
	/// A FIFO Semaphore is a semaphore where delayed requests for tokens are granted in the order that they were made.
	/// 
	/// </summary>
	// Status: complete, test complete, TODO: get marked off
	public class SemaphoreFIFO
	{
		int _numTokens;
		Channel<Semaphore> _threadQueue;
		int _numThreadsQueued;
		Mutex _mutex;
		bool _internalTesting;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrencyUtilities.Semaphore"/> class.
		/// </summary>
		/// <param name="tokens">The number of tokens to start with (0 if unspecified).</param>
		public SemaphoreFIFO(int tokens = 0, bool internalTesting = false) {
			_numTokens = tokens;
			_threadQueue = new Channel<Semaphore>();
			_numThreadsQueued = 0;
			_mutex = new Mutex();
			_internalTesting = internalTesting;
		}

		void DebugThread(string message, bool useColumn = true) {
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

		/// <summary>
		/// Effectively take (acquire) a token from the FIFO semaphore.
		/// Does not let you specify the number of tokens to acquire since you should never really need to
		/// acquire more than one at a time.
		/// If a token is available, the amount of tokens held in the FIFO semaphore is decreased (_numTokens is used).
		/// If a token is not available, the thread joins the queue and waits for its turn to take a virtual token
		/// (_numTokens is bypassed).
		/// </summary>
		public void Acquire() {
			_mutex.Acquire();
				if (_internalTesting)
					DebugThread("{yellow}A:" + (_numThreadsQueued + 1));
				if (_numTokens > 0) {
					_numTokens--; // Token count is only used/modified if the semaphore already had at least one token
				} else {
					Semaphore tokenWaiter = new Semaphore();
					_threadQueue.Put(tokenWaiter); // Join the end of the queue
					_numThreadsQueued++;
				_mutex.Release();
				tokenWaiter.Acquire(); // Wait for a signal that it is our turn to take a token from the core token counter
				_mutex.Acquire();
					_numThreadsQueued--;
				}
				if (_internalTesting) {
					DebugThread("{white}_numThreadsQueued: " + _numThreadsQueued);
					DebugThread("{white}_numTokens: " + _numTokens);
					DebugThread("{green}A:" + (8 - _numThreadsQueued));
				}
			_mutex.Release();
		}

		/// <summary>
		/// Effectively give (release) a number of tokens back to the FIFO semaphore.
		/// If there is a thread waiting, take the one at the front of the queue and wake it up so that it knows it
		/// effectively now has a virtual token (_numTokens is bypassed).
		/// If there aren't any threads waiting, increment the number of available tokens (_numTokens is used).
		/// Tokens/threads are released/woken iteratively. Any woken threads have to wait for all the releasing
		/// iterations to finish before being able to leave with their virtual token.
		/// If multiple tokens are being released, the exact order of the front threads' departures does not matter.
		/// </summary>
		/// <param name="n">The number of tokens to effectively release into / give to the semaphore.</param>
		public void Release(int n) {
			_mutex.Acquire(); /* This could be done inside the loop, but I feel that it'd be slightly faster to release
				all the required tokens at once, rather than context switching a few times */
				for (int i = 0; i < n; i++) {
					// If there's a thread at the front of the queue, signal to it that it's their turn to take a token from the core token counter
					if (_numThreadsQueued > 0) {
						Semaphore tokenWaiter = _threadQueue.Take();
						tokenWaiter.Release(); // Signal its turn. It has to then wait until we release the mutex
					} else { // There are no queued threads
						// Token count is only used/modified if the semaphore already had at least one token
						_numTokens++; // Add a token to the core token counter
					}
				}
				if (_internalTesting) {
					DebugThread("{white}_numThreadsQueued: " + _numThreadsQueued);
					DebugThread("{white}_numTokens: " + _numTokens);
				}
			_mutex.Release();
		}

		public void Release() {
			Release(1);
		}
	}
}