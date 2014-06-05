using System;
using ConcurrencyUtilities;

namespace CigaretteSmokers
{
	public class SmokerResource
	{
		public string Name { get; private set; }
		Semaphore _resource;

		public SmokerResource(string name) {
			Name = name;
			_resource = new Semaphore(0);
		}

		public void Take() {
			_resource.Acquire();
		}

		public void Restock() {
			_resource.Release();
		}

		public void Return() {
			Restock();
		}
	}
}