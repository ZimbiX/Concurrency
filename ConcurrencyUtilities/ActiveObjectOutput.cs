using System;
using ConcurrencyUtilities;
using System.Threading;

namespace ConcurrencyUtilities
{
	/// <summary>
	/// An Output Active Object is an Active Object that uses a channel to output data resulting from
	/// its processing.
	/// Being an abstract generic class, this is intended for use in creating a type of Output Active Object that can
	/// actually do some processing and put data into its output channel.
	/// </summary>
	public abstract class ActiveObjectOutput<OutT>: ActiveObject
	{
		Channel<OutT> _output;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrencyUtilities.ActiveObjectOutput`1"/> class.
		/// </summary>
		/// <param name="output">The channel to use as output.</param>
		public ActiveObjectOutput(Channel<OutT> output): base() {
			_output = output;
		}

		/// <summary>
		/// The Active Object's processing -- put processing result items into the output channel.
		/// </summary>
		protected override void Execute() {
			_output.Put(Process());
		}

		/// <summary>
		/// The Active Object's processing. Returns the result from the processing.
		/// </summary>
		/// <param name="item">The input item.</param>
		protected abstract OutT Process();
	}
}