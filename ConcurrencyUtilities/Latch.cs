using System;

namespace ConcurrencyUtilities
{
	// Turnstile
	// Don't implement Turnstile; it's not a utility. It's a solution to later problems
	// The turnstile is not a concurrency utility, but a technique. It can be used to stop the progression of some threads -- until a token is released into the turnstile semaphore. A turnstile is passed through by acquiring a token from, and then immediately releasing it back to, a semaphore in order to verify that there is at least one token in the semaphore in order to continue.
	// Pretty much, a turnstile can be disabled (0 tokens) or enabled (1+ tokens). This controls the ability of threads to pass a certain point in the code where it needs to pass through the turnstile.

	// A latch can be used to temporarily prevent threads from passing a certain point in their execution. Each one calls Acquire on the latch, and they only continue past that once something calls Release
	// Latch uses the turnstile technique
	// A latch is like a one-shot shotgun -- it should not be reloaded. Once the turnstile is activated, it should never be deactivated
	// Status: complete, test complete, marked off
	public class Latch
	{
		Semaphore _turnstile;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrencyUtilities.Latch"/> class.
		/// </summary>
		public Latch() {
			_turnstile = new Semaphore();
		}

		/// <summary>
		/// Temporarily aquire a token to verify that the latch is open (turnstile technique), otherwise wait for it to be opened
		/// </summary>
		public void Visit() {
			_turnstile.Acquire();
			_turnstile.Release();
		}

		/// <summary>
		/// Release a token into the semaphore to open the latch
		/// </summary>
		public void Open() {
			_turnstile.Release();
		}
	}
}