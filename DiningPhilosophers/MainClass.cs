using System;
using System.Threading;
using System.Collections.Generic;
using ConcurrencyUtilities;
using TestConcurrencyUtilities;
using Colorizer = AnsiColor.AnsiColor;
using ThreadSupport = TestConcurrencyUtilities.TestSupport;

/// <summary>
/// A program to demonstrate the problem and one of the solutions to the Dining Philosophers problem.
/// There is a large bowl of noodle in the middle of a table surrounded by five philosophers, with one chopstick
/// between each philosopher. A philosopher spends some time philosophising, and then becomes hungry.
/// To eat, a philosopher has to pick up both adjacent chopsticks, eat some noodles, and then put the chopsticks
/// down.
/// A deadlock can occur if each philosopher has picked up the chopstick on one side, as no one will be able to eat,
/// and no one will put their first chopstick back.
/// The first solution I have implemented is where philosophers pick up first from different sides; in this
/// implementation, one philosopher picks up the right chopstick first, while the others all go for the left one first.
/// This works because there will always be at least one philosopher able to eat at any time, therefore ensuring that
/// both chopsticks will become available to the others again (because they will actually finish eating at some point).
/// </summary>
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
			int[] timeRange;

			// Determine speed of philosophers

			Console.Write("Do you want to have lightning-fast Philosophers?\nEnter (y/N):");
			string response = Console.ReadLine();
//			response = "Y"; // Can preload with an option to choose automatically
			if (response.ToUpper() == "Y")
				timeRange = new int[] { 0, 0 };
			else
				timeRange = new int[] { 3000, 6000 };

			// Determine which strategy to use

			string strategy = "";
			Console.WriteLine("\nWhich strategy do you want to use?" +
			                  "\n  0. None" +
			                  "\n  1. Swap the chopstick that's picked up first for one philosopher" +
			                  "\n  Q. Quit");
			response = "INVALID_RESPONSE";
//			response = "1"; // Can preload with an option to choose automatically
			do {
				if (response == "INVALID_RESPONSE") {
					Console.Write("Enter: ");
					response = Console.ReadLine().ToUpper();
				}
				switch (response) {
				case "0": strategy = "NONE";						 	break;
				case "1": strategy = "SWAP_FIRST_CHOPSTICK_FOR_ONE";	break;
				case "Q": return;
				default:
					Console.WriteLine("Invalid response");
					response = "INVALID_RESPONSE";
					break;
				}
			} while (response == "INVALID_RESPONSE");

			List<Chopstick> chopsticks = new List<Chopstick>();
			List<Philosopher> philosophers = new List<Philosopher>();

			for (int i = 0; i < numChopsticks; i++)
				chopsticks.Add(new Chopstick( "C" + (0.5+i) ));

			string[] directionsStandard = new string[] { "< ", ""  , ""  , " >" };
			string[] directionsReversed = new string[] { ""  , " >", "< ", ""   };

			// Create philosophers

			if (strategy == "SWAP_FIRST_CHOPSTICK_FOR_ONE")
				// Chopsticks reversed: fixes issue
				philosophers.Add(new Philosopher(chopsticks[1], chopsticks[0], rnd, timeRange, directionsReversed));
			else
				// Original: with issue
				philosophers.Add(new Philosopher(chopsticks[0], chopsticks[1], rnd, timeRange, directionsStandard));

			philosophers.Add(new Philosopher(chopsticks[1], chopsticks[2], rnd, timeRange, directionsStandard));
			philosophers.Add(new Philosopher(chopsticks[2], chopsticks[3], rnd, timeRange, directionsStandard));
			philosophers.Add(new Philosopher(chopsticks[3], chopsticks[4], rnd, timeRange, directionsStandard));
			philosophers.Add(new Philosopher(chopsticks[4], chopsticks[0], rnd, timeRange, directionsStandard));

			// Create and run philosopher threads

			int columnWidth = 17;
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