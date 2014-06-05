using System;
using ConcurrencyUtilities;
using System.Threading;

namespace ConcurrencyUtilities
{
	public abstract class ActiveObjectInput<InT>: ActiveObject
	{
		Channel<InT> _input;

		public ActiveObjectInput(Channel<InT> input): base() {
			_input = input;
		}

		protected override void Execute() {
			Process(_input.Take());
		}

		protected abstract void Process (InT item);
	}
}