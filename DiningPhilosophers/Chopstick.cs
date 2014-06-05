using System;
using ConcurrencyUtilities;

namespace DiningPhilosophers
{
	public class Chopstick
	{
		Mutex _held; // Whether the chopstick is held by a philosopher
		public string Name { get; private set; } // The chopstick's name

		/// <summary>
		/// Initializes a new instance of the <see cref="DiningPhilosophers.Chopstick"/> class.
		/// </summary>
		/// <param name="name">The name of the chopstick.</param>
		public Chopstick(string name) {
			_held = new Mutex();
			Name = name;
		}

		/// <summary>
		/// Be picked up by a philosopher.
		/// </summary>
		public void PickUp() {
			_held.Acquire();
		}
		
		/// <summary>
		/// Be put down by a philosopher.
		/// </summary>
		public void PutDown() {
			_held.Release();
		}
	}
}