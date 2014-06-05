using System;
using ConcurrencyUtilities;

namespace CigaretteSmokers
{
	/// <summary>
	/// A smoker resource. Provides a way to assign a name and semaphore to the concept of a smoker resource.
	/// </summary>
	public class SmokerResource
	{
		public string Name { get; private set; }
		Semaphore _resourceUse; // Semaphore controlling singlar use of the resource -- could be a mutex

		/// <summary>
		/// Initializes a new instance of the <see cref="CigaretteSmokers.SmokerResource"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		public SmokerResource(string name) {
			Name = name;
			_resourceUse = new Semaphore(0);
		}

		/// <summary>
		/// Take the resource (to use it). Acquires a token from the semaphore.
		/// </summary>
		public void Take() {
			_resourceUse.Acquire();
		}

		/// <summary>
		/// Restock the resource (to allow use of it). Releases a token into the semaphore.
		/// </summary>
		public void Restock() {
			_resourceUse.Release();
		}

		/// <summary>
		/// An alias for Restock(). This term makes a little more sense from the point of view of a smoker
		/// giving it back.
		/// </summary>
		public void Return() {
			Restock();
		}
	}
}