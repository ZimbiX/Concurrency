using System;
using ConcurrencyUtilities;
using Semaphore = ConcurrencyUtilities.Semaphore;
using Mutex = ConcurrencyUtilities.Mutex;
using System.Collections.Generic;
using System.Threading;

namespace ConcurrencyUtilities
{
	// TODO: describe
	// Status: TODO: add data exchange, TODO: update test, TODO: get marked off
	/// <summary>
	/// The Exchanger allows two threads to meet.
	/// </summary>
	public class Exchanger
	{
		Semaphore _aArrived;
		Semaphore _bArrived;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrencyUtilities.Exchanger"/> class.
		/// </summary>
		public Exchanger() {
			_aArrived = new Semaphore(0);
			_bArrived = new Semaphore(0);
		}

		/// <summary>
		/// Arrives at the Exchanger as the A thread.
		/// </summary>
		public void AArrive() {
			_aArrived.Release();
			_bArrived.Acquire();
		}

		/// <summary>
		/// Arrives at the Exchanger as the B thread.
		/// </summary>
		public void BArrive() {
			_bArrived.Release();
			_aArrived.Acquire();
		}
	}
}