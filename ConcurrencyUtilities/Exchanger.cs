using System;
using ConcurrencyUtilities;
using Semaphore = ConcurrencyUtilities.Semaphore;
using Mutex = ConcurrencyUtilities.Mutex;
using System.Collections.Generic;
using System.Threading;

namespace ConcurrencyUtilities
{
	// TODO: describe
	// Status: TODO: fix invalid pairing, test complete, TODO: get marked off
	/// <summary>
	/// An exchanger allows two threads to meet and exchange data.
	/// </summary>
	public class Exchanger<T>
	{
		Semaphore _aArrived;
		Semaphore _bArrived;
		T _dataForA;
		T _dataForB;
		Mutex _accessToNextThreadIsB;
		bool _nextThreadIsB;
		Semaphore _numAgentsLimiter;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrencyUtilities.Exchanger"/> class.
		/// </summary>
		public Exchanger() {
			_aArrived = new Semaphore(0);
			_bArrived = new Semaphore(0);
			_accessToNextThreadIsB = new Mutex();
			_nextThreadIsB = false;
			_numAgentsLimiter = new Semaphore(2);
		}

		/// <summary>
		/// Arrives at the exchanger to exchange data.
		/// </summary>
		/// <returns>The data received from the other thread.</returns>
		/// <param name="dataToGive">The data to give to the other thread.</param>
		public T Arrive(T dataToGive) {
			_numAgentsLimiter.Acquire(); // Doesn't seem to help...
				T dataReceived;
				_accessToNextThreadIsB.Acquire();
				if (!_nextThreadIsB) {
					// Will run as thread A
					_nextThreadIsB = !_nextThreadIsB; // Force the next thread to run as thread B
					_accessToNextThreadIsB.Release(); // Let the next thread arrive
					dataReceived = ExchangeAsA(dataToGive);
				} else {
					// Will run as thread B
					_nextThreadIsB = !_nextThreadIsB; // Force the next thread to run as thread A
					_accessToNextThreadIsB.Release(); // Let the next thread arrive
					dataReceived = ExchangeAsB(dataToGive);
				}
			_numAgentsLimiter.Release();
			return dataReceived;
		}

		/// <summary>
		/// Exchanges the data as thread A.
		/// </summary>
		/// <returns>The data received from thread B.</returns>
		/// <param name="dataToGive">The data to give to thread B.</param>
		T ExchangeAsA(T dataToGive) {
			_dataForB = dataToGive;
			_aArrived.Release(); // Indicate that our data is ready for collection by thread B
			_bArrived.Acquire(); // Wait for thread B to arrive and surrender its data
			return _dataForA;
		}
		
		/// <summary>
		/// Exchanges the data as thread B.
		/// </summary>
		/// <returns>The data received from thread A.</returns>
		/// <param name="dataToGive">The data to give to thread A.</param>
		T ExchangeAsB(T dataToGive) {
			_dataForA = dataToGive;
			_bArrived.Release(); // Indicate that our data is ready for collection by thread A
			_aArrived.Acquire(); // Wait for thread A to arrive and surrender its data
			return _dataForB;
		}
	}
}