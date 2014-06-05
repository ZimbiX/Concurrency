using System;
using ConcurrencyUtilities;

namespace DiningPhilosophers
{
	public class Chopstick
	{
		Mutex _held;
		public string Name { get; private set; }

		public Chopstick(string name) {
			_held = new Mutex();
			Name = name;
		}

		public void PickUp() {
			_held.Acquire();
		}

		public void PutDown() {
			_held.Release();
		}
	}
}