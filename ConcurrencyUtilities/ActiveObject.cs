using System;
using System.Threading;

namespace ConcurrencyUtilities
{
	/// <summary>
	/// An active object is like a self-contained autonomous machine. It manages its own thread, and does some stuff
	/// all by itself.
	/// Being an abstract class, this is intended for use in creating a type of Active Object that can actually
	/// do some processing.
	/// </summary>
	public abstract class ActiveObject
	{
		public Thread Thread { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrencyUtilities.ActiveObject"/> class.
		/// </summary>
		public ActiveObject() {
			Thread = new Thread(ExecuteContinuously);
		}

		/// <summary>
		/// Starts the Active Object, commencing processing.
		/// </summary>
		public void Start() {
			Thread.Start();
		}

		/// <summary>
		/// Stops the Active Object, halting processing.
		/// That was the idea anyway... (not currently working properly)
		/// </summary>
		public void Stop() {
			Thread.Interrupt();
		}

		/// <summary>
		/// Continuously executes the Active Object's processing.
		/// </summary>
		private void ExecuteContinuously() {
			while (true) {
				Execute();
			}
		}

		/// <summary>
		/// The Active Object's processing.
		/// </summary>
		protected abstract void Execute();
	}
}