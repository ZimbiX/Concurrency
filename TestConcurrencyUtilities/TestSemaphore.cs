using System;
using ConcurrencyUtilities;
using Semaphore = ConcurrencyUtilities.Semaphore;
using System.Collections.Generic;
using System.Threading;

namespace TestConcurrencyUtilities
{
	// Test my Semaphore class
	public class TestSemaphore
	{
		private static int _sleepTime;
		private static int _magnitude; // The number of threads to use that are attempting to acquire tokens

		private static Semaphore _semaphore = new Semaphore(0);

		private static void TokenRelease() {
			for (int remaining = _magnitude; remaining > 0; ) {
				// DebugThread("is about to sleep for " + StringFromMilliseconds(_sleepTime));
				// int halfSleepTime = (int)Math.Round(_sleepTime/2f);
				Random rnd = new Random();
				// Create a random number within the range
				int upperLimit = (remaining >= 3 ? 3 : remaining);
				int releaseAmount = rnd.Next(1, upperLimit + 1);
				remaining -= releaseAmount;
				TestSupport.SleepThread(_sleepTime);
				Console.WriteLine();
				TestSupport.DebugThread("is about to release " + releaseAmount + " token(s) to the semaphore (after a sleep)");
				TestSupport.SleepThread(_sleepTime);
				TestSupport.DebugThread("is releasing " + releaseAmount + " token(s) to the semaphore");
				_semaphore.Release(releaseAmount);
			}
		}
		
		private static void TokenAcquire() {
			TestSupport.DebugThread("is now attempting to acquire a token from the semaphore");
			_semaphore.Acquire();
			TestSupport.DebugThread("has acquired a token from the semaphore");
		}

		public static void Run(int magnitude, int sleepTime = 0) {
			_magnitude = magnitude;
			_sleepTime = sleepTime;
			List<Thread> threads = new List<Thread>();
			threads.AddRange( TestSupport.CreateThreads(TokenRelease, "T_Sem_TokenRel", 1) );
			threads.AddRange( TestSupport.CreateThreads(TokenAcquire, "    T_Sem_TokenAcq", _magnitude) );
			TestSupport.RunThreads(threads);
			Console.WriteLine("\nFinished");
			// Can also use a lambda expression: 
			//   new Thread( () => TokenRelease(parameter) )
		}
	}
}

