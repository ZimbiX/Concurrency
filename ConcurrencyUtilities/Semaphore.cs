using System;
using System.Threading; // Required for access to Thread
using System.Collections.Generic; // Required for access to List

namespace ConcurrencyUtilities
{
	// TODO: describe
	// Status: complete, test complete, marked off
	public class Semaphore
	{
		protected int _numTokens; // The number of tokens held by the semaphore
		protected Object _lockObjectForAccessToNumTokens = new Object(); // The object to be locked - determines whether _numTokens can be accessed. Avoids using 'lock this', to prevent things screwing up if some idiot locks using this semaphore as the lock object

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrencyUtilities.Semaphore"/> class.
		/// </summary>
		/// <param name="tokens">The number of tokens to start with (0 if unspecified).</param>
		public Semaphore(int tokens = 0) {
			_numTokens = tokens;
		}

		/// <summary>
		/// Take (acquire) a token from the semaphore.
		/// Decreases the amount of tokens held in the semaphore.
		/// Does not let you specify the number of tokens to acquire since you should never really need to acquire more than one at a time.
		/// </summary>
		public void Acquire() {
			lock (_lockObjectForAccessToNumTokens) {
				while (_numTokens == 0)
					Monitor.Wait(_lockObjectForAccessToNumTokens);
				_numTokens -= 1;
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
			lock (_lockObjectForAccessToNumTokens) {
				_numTokens += n;
				Monitor.PulseAll(_lockObjectForAccessToNumTokens);
			}
		}

		public virtual void Release() {
			Release(1);
		}
	}
}

