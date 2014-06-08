using System;
using Colorizer = AnsiColor.AnsiColor;
using System.Threading;

namespace ConcurrencyUtilities
{
	/// <summary>
	/// A barrier is like the stalls in a horse race. It has a quota of threads. Threads enter the barrier,
	/// and stay there until the quota is met, at which point they can all leave.
	/// Once the barrier opens, it is reset; the next thread will wait for the quota to be met.
	/// </summary>
	// Status: complete, test complete, TODO: get marked off
	public class Barrier
	{
		readonly int _numThreadsNeededAtBarrier;         /* The quota -- the number of threads that the barrier
			requires before it lets them all through */
		int _numThreadsAtBarrier;                        // The number of threads currently waiting at the barrier
		readonly Mutex _accessToNumThreadsAtBarrier;     /* Mutex providing thread-safe access to the
			variable: _numThreadsAtBarrier */
		readonly Semaphore _leavePermission;         // Semaphore that determines whether threads can leave the barrier
		readonly Semaphore _entryTicket;             // Semaphore that determines whether threads can enter the barrier
		readonly Semaphore _vacancy;                 /* Semaphore that lets the captain know when all the other
			threads in the group have left */
		readonly bool _isTesting;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrencyUtilities.Barrier"/> class.
		/// </summary>
		/// <param name="numThreadsNeededAtBarrier">Number threads needed at the barrier for it to release
		/// all the threads.</param>
		public Barrier(int numThreadsNeededAtBarrier, bool isTesting = false) {
			_numThreadsAtBarrier = 0;
			_accessToNumThreadsAtBarrier = new Mutex();
			_numThreadsNeededAtBarrier = numThreadsNeededAtBarrier;
			_leavePermission = new Semaphore(0);
			_entryTicket = new Semaphore(numThreadsNeededAtBarrier);
			_vacancy = new Semaphore();
			_isTesting = isTesting;
		}

		/// <summary>
		/// Arrive at the barrier, and wait for the thread quota to be met before leaving the barrier.
		/// Returns whether the thread was chosen to be captain of the barrier
		/// </summary>
		public bool Arrive() {
			// Prepare for console logging in columns -- used if _isTesting
			string threadNameWithoutPrefix = Thread.CurrentThread.Name.Replace("%%%%","");
			string threadColumnOffset = threadNameWithoutPrefix.Replace(threadNameWithoutPrefix.TrimStart(' '), "");

			bool isCaptain = false;
			// Arrive at the barrier

			// Enter if the barrier is not yet full, otherwise wait for a new group to begin
			_entryTicket.Acquire();
			if (_isTesting)
				Console.WriteLine(threadColumnOffset + Colorizer.Colorize("{cyan}E"));

			_accessToNumThreadsAtBarrier.Acquire();
				_numThreadsAtBarrier++;
				if (_numThreadsAtBarrier == _numThreadsNeededAtBarrier) {
					_leavePermission.Release(_numThreadsAtBarrier); /* The current thread just met the quota, so let
						through everyone that's here */
					_numThreadsAtBarrier = 0; // Reset the thread count to that of non-existent Egyptian cotton
					isCaptain = true; // The last thread in the group becomes the captain
					// Ensure that the captain is the last thread of the group to leave (wait for the others to leave)
					for (int i = 0; i < _numThreadsNeededAtBarrier - 1; i++)
						_vacancy.Acquire(); // Wait until every thread has effectively left the barrier
				}
			_accessToNumThreadsAtBarrier.Release();

			// Wait for permission to leave the barrier (wait for the quota to be met):
			_leavePermission.Acquire();
			if (_isTesting)
				Console.WriteLine(threadColumnOffset + Colorizer.Colorize("{green}L" + (isCaptain ? "*" : "")));

			// Now that this thread has effectively left the barrier...
			if (isCaptain)
				_entryTicket.Release(_numThreadsNeededAtBarrier); /* The captain is the last to leave, and has
					now left, so allow the next group of threads to enter */
			else
				_vacancy.Release(); // Signal to the captain that another non-captain thread has left

			// Leave:
			return isCaptain;
		}
	}
}