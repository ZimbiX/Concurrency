using System;

namespace ConcurrencyUtilities
{
	/// <summary>
	/// A barrier is like the stalls in a horse race. It has a quota of threads. Threads enter the barrier, and stay there until the quota is met, at which point they can all leave.
	/// For safety, any threads that arrive at the barrier after it's already been opened are let through straight away.
	/// </summary>
	// Status: TODO: add reuse, TODO: test reuse, TODO: get marked off
	public class Barrier
	{
		readonly int _numThreadsNeededAtBarrier;         // The quota -- the number of threads that the barrier requires before it lets them all through
		int _numThreadsAtBarrier;                        // The number of threads currently at the barrier (stays maxed out after the barrier opens so as to let late threads through)
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
				if (_numThreadsAtBarrier < _numThreadsNeededAtBarrier) { // Check if the quota has not been met
					_numThreadsAtBarrier++;
					if (_numThreadsAtBarrier == _numThreadsNeededAtBarrier)
						_goPermission.Release(_numThreadsAtBarrier); // The current thread just met the quota, so let through everyone that's here
				} else {
					// The quota was already reached, so just go straight through
					_goPermission.Release(); // Release a single token, allowing the current thread through
				}
			_accessToNumThreadsAtBarrier.Release();

			// Wait for permission to leave the barrier:

			_goPermission.Acquire();
		}
	}
}