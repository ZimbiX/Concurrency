using System;
using ConcurrencyUtilities;
using System.Threading;

namespace ConcurrencyUtilities
{
	/// <summary>
	/// An Input-Output Active Object is an Active Object that uses two channels. One is used to take data as input
	/// for its processing, and the other is used for output from the processing.
	/// Being an abstract generic class, this is intended for use in creating a type of Input-Output Active Object
	/// that can actually do some processing with data from its input channel, and put data into its output channel.
	/// </summary>
	public abstract class ActiveObjectInputOutput<InT,OutT>: ActiveObject
	{
		Channel<InT> _input;
		Channel<OutT> _output;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrencyUtilities.ActiveObjectInputOutput`2"/> class.
		/// </summary>
		/// <param name="input">The channel to use as input.</param>
		/// <param name="output">The channel to use as output.</param>
		public ActiveObjectInputOutput(Channel<InT> input, Channel<OutT> output): base() {
			_input = input;
			_output = output;
		}

		/// <summary>
		/// The Active Object's processing -- process items from the input channel, and put the result into the
		/// output channel.
		/// </summary>
		protected override void Execute() {
			_output.Put(Process(_input.Take()));
		}
		
		/// <summary>
		/// The Active Object's processing on an item taken from the input channel. Returns the result from
		/// the processing.
		/// </summary>
		/// <param name="item">The input item.</param>
		protected abstract OutT Process(InT item);
	}
}