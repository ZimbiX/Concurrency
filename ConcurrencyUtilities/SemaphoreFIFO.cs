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
	// Status: TODO: check if complete, TODO: check if test complete, TODO: get marked off
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

		void DebugThread(string message) {
			// Prepare for console logging in columns -- used if _isTesting
			string threadNameWithoutPrefix = Thread.CurrentThread.Name.Replace("%%%%","");
			string threadColumnOffset = threadNameWithoutPrefix.Replace(threadNameWithoutPrefix.TrimStart(' '), "");
			Console.WriteLine(threadColumnOffset + Colorizer.Colorize(message));
		}

		/// <summary>
		/// Take (acquire) a token from the FIFO semaphore.
		/// Decreases the amount of tokens held in the FIFO semaphore.
		/// Does not let you specify the number of tokens to acquire since you should never really need to
		/// acquire more than one at a time.
		/// If a token is not available, the thread joins a queue and waits for its turn to take one.
		/// </summary>
		public void Acquire() {
			_mutex.Acquire();
				if (_internalTesting)
					DebugThread("{yellow}A:" + (_numThreadsQueued + 1));
				if (_numTokens == 0) {
					Semaphore tokenWaiter = new Semaphore();
					_threadQueue.Put(tokenWaiter); // Join the end of the queue
					_numThreadsQueued++;
				_mutex.Release();
				tokenWaiter.Acquire(); // Wait for a signal that it is our turn to take a token from the core token counter
				_mutex.Acquire();
					_numThreadsQueued--;
				}
				_numTokens--;
				if (_internalTesting)
					DebugThread("{green}A:" + (10 - _numThreadsQueued));
			_mutex.Release();
		}

		/// <summary>
		/// Give (release) a number of tokens back to the FIFO semaphore.
		/// Increases the amount of tokens held in the FIFO semaphore.
		/// Tokens are released iteratively. After releasing a token, check if there are any threads waiting; if so,
		/// take the one at the front of the queue and wake it up so that it knows it can now take a token.
		/// If multiple tokens are being released, the exact order of the front threads' departures does not matter.
		/// </summary>
		/// <param name="n">The number of tokens to release into / give to the semaphore.</param>
		public void Release(int n) {
			_mutex.Acquire(); /* This could be done inside the loop, but I feel that it'd be slightly faster to release
				all the required tokens at once, rather than context switching a few times */
				for (int i = 0; i < n; i++) {
					_numTokens++; // Add a token to the core token counter
					// If there's a thread at the front of the queue, signal to it that it's their turn to take a token from the core token counter
					if (_numThreadsQueued > 0) {
						Semaphore tokenWaiter = _threadQueue.Take();
						tokenWaiter.Release(); // Signal its turn. It has to then wait until we release the mutex
					}
				}
			_mutex.Release();
		}

		public void Release() {
			Release(1);
		}
	}
}