using System;
using System.Threading;
using System.Collections.Generic;
using ConcurrencyUtilities;
using TestConcurrencyUtilities;
using Colorizer = AnsiColor.AnsiColor;
using ThreadSupport = TestConcurrencyUtilities.TestSupport;

namespace DiningPhilosophers
{
	class MainClass
	{

		public static void Main(string[] args) {
			Console.WriteLine(Colorizer.Colorize("{blue}Dining Philosopher problem" +
				"\n===============================\n"));

			int numPhilosophers = 5;
			int numChopsticks = numPhilosophers;
			Random rnd = new Random();
			int[] timeRange = new int[] { 2000, 6000 };
			timeRange = new int[] { 0, 0 };

			List<Chopstick> chopsticks = new List<Chopstick>();
			List<Philosopher> philosophers = new List<Philosopher>();

			for (int i = 1; i <= numChopsticks; i++)
				chopsticks.Add(new Chopstick("C" + i));

			philosophers.Add(new Philosopher(chopsticks[0], chopsticks[1], rnd, timeRange));
			philosophers.Add(new Philosopher(chopsticks[1], chopsticks[2], rnd, timeRange));
			philosophers.Add(new Philosopher(chopsticks[2], chopsticks[3], rnd, timeRange));
			philosophers.Add(new Philosopher(chopsticks[3], chopsticks[4], rnd, timeRange));
			philosophers.Add(new Philosopher(chopsticks[4], chopsticks[0], rnd, timeRange));

			int columnWidth = 9+1;
			List<Thread> threads = new List<Thread>();
			for (int i = 0; i < numPhilosophers; i++) {
				threads.AddRange( ThreadSupport.CreateThreads(philosophers[i].Philosophise,
				                                              "P", 1, i+1, columnWidth, i+1) );
			}
			ThreadSupport.EndColumnHeader(numPhilosophers, columnWidth);
			ThreadSupport.RunThreads(threads);
		}
	}
}