using System;
using ConcurrencyUtilities;
using System.Threading;

namespace ConcurrencyUtilities
{
	public abstract class ActiveObjectInputOutput<InT,OutT>: ActiveObject
	{
		Channel<InT> _input;
		Channel<OutT> _output;

		public ActiveObjectInputOutput(Channel<InT> input, Channel<OutT> output): base() {
			_input = input;
			_output = output;
		}

		protected override void Execute() {
			_output.Put(Process(_input.Take()));
		}

		protected abstract OutT Process(InT item);
	}
}