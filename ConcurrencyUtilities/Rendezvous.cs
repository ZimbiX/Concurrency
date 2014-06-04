using System;
using ConcurrencyUtilities;
using Semaphore = ConcurrencyUtilities.Semaphore;
using Mutex = ConcurrencyUtilities.Mutex;
using System.Collections.Generic;
using System.Threading;

namespace ConcurrencyUtilities
{
	// TODO: describe
	// Status: complete, test complete, TODO: get marked off
	public class Rendezvous
	{
		Semaphore _aArrived;
		Semaphore _bArrived;

		public Rendezvous() {
			_aArrived = new Semaphore(0);
			_bArrived = new Semaphore(0);
		}

		public void AArrive() {
			_aArrived.Release();
			_bArrived.Acquire();
		}

		public void BArrive() {
			_bArrived.Release();
			_aArrived.Acquire();
		}
	}
}