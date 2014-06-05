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

			if (strategy == "SWAP_FIRST_CHOPSTICK_FOR_ONE")
				philosophers.Add(new Philosopher(chopsticks[1], chopsticks[0], rnd, timeRange, directionsReversed)); // Chopsticks reversed: fixes issue
			else
				philosophers.Add(new Philosopher(chopsticks[0], chopsticks[1], rnd, timeRange, directionsStandard)); // Original: with issue

			philosophers.Add(new Philosopher(chopsticks[1], chopsticks[2], rnd, timeRange, directionsStandard));
			philosophers.Add(new Philosopher(chopsticks[2], chopsticks[3], rnd, timeRange, directionsStandard));
			philosophers.Add(new Philosopher(chopsticks[3], chopsticks[4], rnd, timeRange, directionsStandard));
			philosophers.Add(new Philosopher(chopsticks[4], chopsticks[0], rnd, timeRange, directionsStandard));

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