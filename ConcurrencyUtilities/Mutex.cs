using System;

namespace ConcurrencyUtilities
{
	/// <summary>
	/// A mutex can be used in place of a lock. This isn't very useful for us in C# since the language aready supports lock
	/// The mutex is a single-token (boolean) semaphore. It starts with 1 token to signify that it is not locked
	/// </summary>
	// Status: complete, TODO: test?, TODO: get marked off?
	public class Mutex: Semaphore
	{
		public Mutex(): base(1) {}

		/// <summary>
		/// Give (release) a token back to the mutex.
		/// Increases the amount of tokens held in the mutex.
		/// There can only be a maximum of one token in the mutex, so throw an exception if this is about to be exceeded.
		/// </summary>
		public override void Release() {
			lock (_lockObjectForAccessToNumTokens) {
				if (_numTokens > 0)
					throw new System.InvalidOperationException("Cannot release a second token into the mutex");
			}
			base.Release(1);
		}

		/// <summary>
		/// Release the specified number of tokens back to the mutex -- can only ever be 1.
		/// </summary>
		/// <param name="n">Number of tokens to release.</param>
		public override void Release(int n) {
			if (n == 1)
				Release();
			else
				throw new System.ArgumentOutOfRangeException("n", n, "Cannot release more than one token at a time into the mutex");
		}
	}
}