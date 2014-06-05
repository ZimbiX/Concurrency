using System;
using ThreadSupport = TestConcurrencyUtilities.TestSupport;

namespace DiningPhilosophers
{
	public class Philosopher
	{
		Chopstick _chopstickA; // Left, normally
		Chopstick _chopstickB; // Right, normally
		Random _rnd; /* The random number generator to use for determining the minimum and maximum time that may be
			spent thinking or eating */
		int[] _timeRange; /* The time range determining the minimum and maximum time that may be spent thinking or
			eating */
		string[] _chopstickADirection; // An arrow and a blank indicating the direction to the first chopstick
		string[] _chopstickBDirection; // An arrow and a blank indicating the direction to the second chopstick

		/// <summary>
		/// Initializes a new instance of the <see cref="DiningPhilosophers.Philosopher"/> class.
		/// </summary>
		/// <param name="chopstickA">Chopstick A -- the first to be picked up.</param>
		/// <param name="chopstickB">Chopstick B -- the second to be picked up.</param>
		/// <param name="rnd">A random number generator to use when calculating the time to think or eat.</param>
		/// <param name="timeRange">A time range determining the minimum and maximum time that may be spent thinking or
		/// eating.</param>
		/// <param name="chopstickDirections">Chopstick directions relative to the philosopher for chopsticks
		/// A and B.</param>
		public Philosopher(Chopstick chopstickA, Chopstick chopstickB, Random rnd, int[] timeRange,
		                   string[] chopstickDirections) {
			_chopstickADirection = new string[] { chopstickDirections[0], chopstickDirections[1] };
			_chopstickBDirection = new string[] { chopstickDirections[2], chopstickDirections[3] };
			_chopstickA = chopstickA;
			_chopstickB = chopstickB;
			_rnd = rnd;
			_timeRange = timeRange;
		}

		/// <summary>
		/// Runs the Philosopher (loop thinking and eating).
		/// </summary>
		public void Philosophise() {
			while (true)
				ThinkAndEat();
		}

		/// <summary>
		/// Makes the philosopher think and eat. They think for a period, become hungry, attempt to pick up
		/// chopstick A then B, eat, and then put the chopsticks back.
		/// </summary>
		private void ThinkAndEat() {
			// Think

			ThreadSupport.DebugThread("{cyan}Thinking");
			ThreadSupport.SleepThread(GetDurationForActivity(), false);

			// Eat
			
			ThreadSupport.DebugThread("{yellow}Hungry");
			_chopstickA.PickUp();
			ThreadSupport.DebugThread("{black}" + _chopstickADirection[0] + "Have A: " +
			                          _chopstickA.Name + _chopstickADirection[1]);
			_chopstickB.PickUp();
			ThreadSupport.DebugThread("{black}" + _chopstickBDirection[0] + "Have B: " +
			                          _chopstickB.Name + _chopstickBDirection[1]);
				ThreadSupport.DebugThread("{green}Eating");
				ThreadSupport.SleepThread(GetDurationForActivity(), false);
			_chopstickA.PutDown();
			ThreadSupport.DebugThread("{black}" + _chopstickADirection[0] + "Drop'd A: " +
			                          _chopstickA.Name + _chopstickADirection[1]);
			_chopstickB.PutDown();
			ThreadSupport.DebugThread("{black}" + _chopstickBDirection[0] + "Drop'd B: " +
			                          _chopstickB.Name + _chopstickBDirection[1]);
		}

		/// <summary>
		/// Generates a random duration to spend on an activity (thinking or eating).
		/// </summary>
		/// <returns>The random duration to spend on an activity.</returns>
		int GetDurationForActivity() {
			return _rnd.Next(_timeRange[0], _timeRange[1]);
		}
	}
}