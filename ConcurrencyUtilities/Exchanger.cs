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
	/// An exchanger allows two threads to meet and exchange data.
	/// </summary>
	public class Exchanger<T>
	{
		Semaphore _aArrived;
		Semaphore _bArrived;
		T _dataForA;
		T _dataForB;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrencyUtilities.Exchanger"/> class.
		/// </summary>
		public Exchanger() {
			_aArrived = new Semaphore(0);
			_bArrived = new Semaphore(0);
			//_exchangeData = 
		}

		/// <summary>
		/// Arrives at the exchanger to exchange data.
		/// </summary>
		public T Arrive(T dataToGive) {
			_dataForA = dataToGive;
			_aArrived.Release();
			_bArrived.Acquire();
			return _dataForB;
		}
	}
}