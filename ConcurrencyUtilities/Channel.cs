using System;
using System.Threading; // Required for access to Thread
using System.Collections.Generic; // Required for access to List

namespace ConcurrencyUtilities
{
	/// <summary>
	/// A channel is effectively a data queue that can be used as a thread-safe method of communicating data between threads
	/// </summary>
	// Status: complete, test complete, marked off
	public class Channel<T>
	{
		protected readonly Semaphore _takePermission; // The tokens held by this semaphore correspond to the number of items currently held in the channel's queue. Decrementing this is required before something can be taken from the queue; it equates to permission to take
		protected readonly Queue<T> _queue; // The queue which holds the channel's data
		protected readonly Mutex _accessToQueue; // Determines whether _queue can be accessed

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrencyUtilities.Channel`1"/> class.
		/// </summary>
		/// <param name="sleepTime">Sleep time.</param>
		public Channel() {
			_takePermission = new Semaphore(0);
			_queue = new Queue<T>();
			_accessToQueue = new Mutex();
		}

		/// <summary>
		/// Add an item to the end of the (unlimited) queue
		/// </summary>
		/// <param name="thingToEnqueue">Thing to enqueue.</param>
		public virtual void Put(T thingToEnqueue) {
			_accessToQueue.Acquire();
				_queue.Enqueue(thingToEnqueue); // Add the item to the queue
			_accessToQueue.Release();
			_takePermission.Release(); // This can occur outside of the queue access because the semaphore has its own concurrency management
		}

		/// <summary>
		/// Take an item from the start of the queue. If the queue is empty, it will wait for there to be an item to take
		/// </summary>
		public virtual T Take() {
			_takePermission.Acquire(); // This can occur outside of the queue access because the semaphore has its own concurrency management
			T item;
			_accessToQueue.Acquire();
				item = _queue.Dequeue(); // Take the item off the queue
			_accessToQueue.Release();
			return item;
		}
	}
}