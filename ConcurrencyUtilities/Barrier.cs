using System;

namespace ConcurrencyUtilities
{
	/// <summary>
	/// A barrier is like the stalls in a horse race. It has a quota of threads. Threads enter the barrier, and stay there until the quota is met, at which point they can all leave.
	/// Once the barrier opens, it is reset; the next thread will wait for the quota to be met.
	/// </summary>
	// Status: TODO: add reuse, TODO: test reuse, TODO: get marked off
	public class Barrier
	{
		readonly int _numThreadsNeededAtBarrier;         // The quota -- the number of threads that the barrier requires before it lets them all through
		int _numThreadsAtBarrier;                        // The number of threads currently waiting at the barrier
		readonly Mutex _accessToNumThreadsAtBarrier;     // Mutex providing thread-safe access to the variable: _numThreadsAtBarrier
		readonly Semaphore _goPermission;                // Semaphore that determines whether threads can leave the barrier

		public Barrier(int numThreadsNeededAtBarrier) {
			_numThreadsAtBarrier = 0;
			_accessToNumThreadsAtBarrier = new Mutex();
			_numThreadsNeededAtBarrier = numThreadsNeededAtBarrier;
			_goPermission = new Semaphore(0);
		}

		public void Arrive() {
			// Arrive at the barrier:

			_accessToNumThreadsAtBarrier.Acquire(); // Also functions as a turnstile! =)
				_numThreadsAtBarrier++;
				if (_numThreadsAtBarrier == _numThreadsNeededAtBarrier) {
					_goPermission.Release(_numThreadsAtBarrier); // The current thread just met the quota, so let through everyone that's here
					_numThreadsAtBarrier = 0; // Reset the barrier
				}
			_accessToNumThreadsAtBarrier.Release();

			// Wait for permission to leave the barrier:

			_goPermission.Acquire();
		}
	}
}