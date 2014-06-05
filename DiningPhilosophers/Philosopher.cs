using System;
using ThreadSupport = TestConcurrencyUtilities.TestSupport;

namespace DiningPhilosophers
{
	public class Philosopher
	{
		Chopstick _chopstickA; // Left, normally
		Chopstick _chopstickB; // Right, normally
		Random _rnd;
		int[] _timeRange;
		string[] _chopstickADirection;
		string[] _chopstickBDirection;

		public Philosopher(Chopstick chopstickA, Chopstick chopstickB, Random rnd, int[] timeRange, string[] chopstickDirections) {
			_chopstickADirection = new string[] { chopstickDirections[0], chopstickDirections[1] }; //new string[] { "< ", "" }; // TODO.....
			_chopstickBDirection = new string[] { chopstickDirections[2], chopstickDirections[3] }; //new string[] { "", " >" };
			_chopstickA = chopstickA;
			_chopstickB = chopstickB;
			_rnd = rnd;
			_timeRange = timeRange;
		}

		public void Philosophise() {
			while (true)
				ThinkAndEat();
		}

		private void ThinkAndEat() {
			// Think

			ThreadSupport.DebugThread("{cyan}Thinking");
			ThreadSupport.SleepThread(GetDurationForActivity(), false);

			// Eat
			
			ThreadSupport.DebugThread("{yellow}Hungry");
			_chopstickA.PickUp();
			ThreadSupport.DebugThread("{black}" + _chopstickADirection[0] + "Have A: " + _chopstickA.Name + _chopstickADirection[1]);
			_chopstickB.PickUp();
			ThreadSupport.DebugThread("{black}" + _chopstickBDirection[0] + "Have B: " + _chopstickB.Name + _chopstickBDirection[1]);
				ThreadSupport.DebugThread("{green}Eating");
				ThreadSupport.SleepThread(GetDurationForActivity(), false);
			_chopstickA.PutDown();
			ThreadSupport.DebugThread("{black}" + _chopstickADirection[0] + "Drop'd A: " + _chopstickA.Name + _chopstickADirection[1]);
			_chopstickB.PutDown();
			ThreadSupport.DebugThread("{black}" + _chopstickBDirection[0] + "Drop'd B: " + _chopstickB.Name + _chopstickBDirection[1]);
		}

		int GetDurationForActivity() {
			return _rnd.Next(_timeRange[0], _timeRange[1]);
		}
	}
}