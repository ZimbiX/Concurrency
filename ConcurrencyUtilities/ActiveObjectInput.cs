using System;
using ConcurrencyUtilities;
using System.Threading;

namespace ConcurrencyUtilities
{
	/// <summary>
	/// An Input Active Object is an Active Object that uses a channel to take data as input to use in its processing.
	/// Being an abstract generic class, this is intended for use in creating a type of Input Active Object that can
	/// actually do some processing with data from its input channel.
	/// </summary>
	public abstract class ActiveObjectInput<InT>: ActiveObject
	{
		Channel<InT> _input;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrencyUtilities.ActiveObjectInput`1"/> class.
		/// </summary>
		/// <param name="input">The channel to use as input.</param>
		public ActiveObjectInput(Channel<InT> input): base() {
			_input = input;
		}

		/// <summary>
		/// The Active Object's processing -- process items from the input channel.
		/// </summary>
		protected override void Execute() {
			Process(_input.Take());
		}
		
		/// <summary>
		/// The Active Object's processing on an item taken from the input channel.
		/// </summary>
		/// <param name="item">The input item.</param>
		protected abstract void Process(InT item);
	}
}