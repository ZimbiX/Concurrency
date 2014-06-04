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

		// Initialise the semaphore, starting with the specified number of tokens, otherwise 0
		public Semaphore(int tokens = 0) {
			_numTokens = tokens;
		}

		// Take (acquire) a token from the semaphore
		// Decreases the amount of tokens held in the semaphore
		// Does not let you specify the number of tokens to acquire since you should never really need to acquire more than one at a time
		public void Acquire() {
			lock (_lockObjectForAccessToNumTokens) {
				while (_numTokens == 0)
					Monitor.Wait(_lockObjectForAccessToNumTokens);
				_numTokens -= 1;
			}
		}

		// Give (release) a number of tokens back to the semaphore
		// Increases the amount of tokens held in the semaphore
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

