using System;
using ConcurrencyUtilities;
using System.Threading;

namespace ConcurrencyUtilities
{
	public abstract class ActiveObjectOutput<OutT>: ActiveObject
	{
		Channel<OutT> _output;

		public ActiveObjectOutput(Channel<OutT> output): base() {
			_output = output;
		}

		protected override void Execute() {
			_output.Put(Process());
		}

		protected abstract OutT Process();
	}
}