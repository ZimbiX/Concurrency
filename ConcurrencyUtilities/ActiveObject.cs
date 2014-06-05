using System;
using System.Threading;

namespace ConcurrencyUtilities
{
	public abstract class ActiveObject
	{
		public Thread Thread { get; private set; }

		public ActiveObject() {
			Thread = new Thread(ExecuteContinuously);
		}

		public void Start() {
			Thread.Start();
		}

		public void Stop() {
			Thread.Interrupt();
		}

		private void ExecuteContinuously() {
			while (true) {
				Execute();
			}
		}

		protected abstract void Execute();
	}
}