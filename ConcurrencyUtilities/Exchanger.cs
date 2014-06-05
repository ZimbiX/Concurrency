using System;
using ConcurrencyUtilities;
using Semaphore = ConcurrencyUtilities.Semaphore;
using Mutex = ConcurrencyUtilities.Mutex;
using System.Collections.Generic;
using System.Threading;

namespace ConcurrencyUtilities
{
	/// <summary>
	/// An exchanger allows two threads to meet and exchange data. If the other thread is not yet present at the
	/// rendezvous, it is waited upon.
	/// </summary>
	// Status: complete, test complete, TODO: get marked off
	public class Exchanger<T>
	{
		Semaphore _aArrived; // Whether thread A has arrived at the rendezvous, with data ready for thread B to collect
		Semaphore _bArrived; // Whether thread B has arrived at the rendezvous, with data ready for thread A to collect
		T _dataForA; // The data from thread B, for collection by thread A
		T _dataForB; // The data from thread A, for collection by thread B
		Mutex _accessToNextThreadIsB; // Thread-safe permisson for access to the variable '_nextThreadIsB'
		bool _nextThreadIsB; // A flip-flop used to determine the identity of the next thread
		Semaphore _agentPairer; /* Used to let through only two threads at once. Once both current threads have
			finished exchanging, two token are released into this to let another pair into the exchanger */
		Semaphore _dataHasBeenTaken; /* Used to determine whether both threads have taken their data, and so have
			finished their exchange */

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrencyUtilities.Exchanger"/> class.
		/// </summary>
		public Exchanger() {
			_aArrived = new Semaphore(0);
			_bArrived = new Semaphore(0);
			_accessToNextThreadIsB = new Mutex();
			_nextThreadIsB = false;
			_agentPairer = new Semaphore(2);
			_dataHasBeenTaken = new Semaphore(0);
		}

		/// <summary>
		/// Arrives at the exchanger and exchanges data with another thread. Threads are let through in pairs, and
		/// assigned an identity A or B so that each one uses its own one of the temporary storage variables.
		/// B threads are also in charge of letting a new pair od threads into the exchanger once the existing exchange
		/// is confirmed to be completed. If each thread were to let a new one in when finished, its partner may not
		/// have acquired the data that was left, which could then be overwritten before it is accessed.
		/// </summary>
		/// <returns>The data received from the other thread.</returns>
		/// <param name="dataToGive">The data to give to the other thread.</param>
		public T Arrive(T dataToGive) {
			_agentPairer.Acquire(); // Let only a pair of threads pass at a time

			// Assign identity to each thread

			char identity;
			_accessToNextThreadIsB.Acquire();
				identity = _nextThreadIsB ? 'B' : 'A';
				_nextThreadIsB = !_nextThreadIsB;
			_accessToNextThreadIsB.Release();

			T dataReceived;

			// Exchange the data

			if (identity == 'A') { 				// Thread A
				_dataForB = dataToGive;
				// Rendezvous
				_aArrived.Release(); // Indicate that our data is ready for collection by thread B
				_bArrived.Acquire(); // Wait for thread B to arrive and surrender its data
				dataReceived = _dataForA;
				_dataHasBeenTaken.Release();
			} else { 							// Thread B
				_dataForA = dataToGive;
				// Rendezvous
				_bArrived.Release(); // Indicate that our data is ready for collection by thread A
				_aArrived.Acquire(); // Wait for thread A to arrive and surrender its data
				dataReceived = _dataForB;
				_dataHasBeenTaken.Release();
			}

			// Once the data exchange is complete, let a new pair of threads into the exchanger

			if (identity == 'B') {
				_dataHasBeenTaken.Acquire();
				_dataHasBeenTaken.Acquire();
				_agentPairer.Release(2);
			}

			return dataReceived;
		}
	}
}