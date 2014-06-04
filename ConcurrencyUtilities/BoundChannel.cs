using System;

namespace ConcurrencyUtilities
{
	// A bound channel is a channel with a limit to the number of items that can be queued in it. If you try to put something into the bound channel's queue when it's full, it waits for there to be room in the queue before storing the item
	// Status: complete, test complete, marked off
	public class BoundChannel<T>: Channel<T>
	{
		readonly Semaphore _putPermission;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrencyUtilities.BoundChannel`1"/> class.
		/// </summary>
		/// <param name="upperLimit">The upper limit for how many items can be held in the channel's queue.</param>
		public BoundChannel(int upperLimit): base() {
			_putPermission = new Semaphore(upperLimit); // The tokens held by this semaphore correspond to the remaining number of items that can be stored in the bound channel's queue. Decrementing this is required before something can be put into the queue; it equates to permission to put
		}

		/// <summary>
		/// Add an item to the end of the queue. If there is no room, wait for there to be
		/// </summary>
		/// <param name="thingToEnqueue">Thing to enqueue.</param>
		public override void Put(T thingToEnqueue) {
			_putPermission.Acquire();
			base.Put(thingToEnqueue);
		}

		/// <summary>
		/// Take an item from the start of the queue. Use the same rule (wait for an item) as a standard channel, but also use the 'put permission' semaphore to keep track of whether we have any more room in the queue in which to put items
		/// </summary>
		public override T Take() {
			T temp = base.Take();
			_putPermission.Release();
			return temp;
		}
	}
}