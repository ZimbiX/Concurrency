using System;
using System.Collections.Generic;
using System.Threading;
using ConcurrencyUtilities;
using Mutex = ConcurrencyUtilities.Mutex;
using Colorizer = AnsiColor.AnsiColor;
using ThreadSupport = TestConcurrencyUtilities.TestSupport;

/// <summary>
/// A program to demonstrate the problem and one of the solutions to the Cigarette Smokers problem.
/// There are three resources required in order to make a cigarette: tobacco, paper, and a match.
/// There are three smokers, who each individually have an unlimited supply of a particular resource.
/// An 'agent' supplies two of the three resources (one unit of each) at any one time. The smoker who has the
/// complementing resource should grab the two supplied, make their cigarette, and smoke it. When a smoker takes both
/// of the required resources from the 'agent', they should signal to the agent that it needs to supply another two
/// resources, which it should then do.
/// </summary>
namespace CigaretteSmokers
{
	class MainClass
	{
		public static void Main(string[] args) {
			Console.WriteLine(Colorizer.Colorize("{blue}Cigarette Smokers problem" +
			                                     "\n===============================\n"));

			// Determine smoker greediness delay time
			
			int smokerGreedyTime;
			Console.Write("Do you want to have lightning-fast smokers?\nEnter [y/N]:");
			string response = Console.ReadLine();
//			response = "Y"; // Can preload with an option to use regardless
			if (response.ToUpper() == "Y")
				smokerGreedyTime = 0;
			else
				smokerGreedyTime = 2000;

			// Determine which strategy to use

			string strategy = "";
			// I'd like to implement the LBoS solution strategy for this problem if I have time
			Console.WriteLine("\nWhich strategy do you want to use?" +
			                  "\n  0. None (issue demonstration)" +
			                  "\n  1. Resource turnstiling and iteration checking" +
			                  "\n  Q. Quit");
			response = "INVALID_RESPONSE";
//			response = "1"; // Can preload with an option to choose automatically
			do {
				if (response == "INVALID_RESPONSE") {
					Console.Write("Enter: ");
					response = Console.ReadLine().ToUpper();
				}
				switch (response) {
				case "0": strategy = "NONE";						 				break;
				case "1": strategy = "RESOURCE_TURNSTILING_AND_ITERATION_CHECKING";	break;
				case "Q": return; // Exit
				default:
					Console.WriteLine("Invalid response");
					response = "INVALID_RESPONSE";
					break;
				}
			} while (response == "INVALID_RESPONSE");
			Console.WriteLine();

			// Create resources

			List<SmokerResource> resources = new List<SmokerResource>();
			foreach (string name in new string[] { "Tobacco", "Paper", "Match" })
				resources.Add(new SmokerResource(name));

			// Create table

			Table table = new Table(resources);

			// Set up iteration tracking -- for resource turnstiling and iteration checking solution strategy

			IterationManager iterationManager = new IterationManager(table);

			// Create smokers

			List<Smoker> smokers = new List<Smoker>();
			for (int i = 0; i < 3; i++) {
				List<SmokerResource> resourcesNeededBySmoker = new List<SmokerResource>(resources);
				string smokerName = "S+" + resources[i].Name;
				resourcesNeededBySmoker.RemoveAt(i); // Remove the resource that they don't require any more of
				smokers.Add(new Smoker(smokerName, resourcesNeededBySmoker, table, iterationManager, smokerGreedyTime));
			}

			// Create threads

			int columnWidth = 23;
			List<Thread> threads = new List<Thread>();
			threads.AddRange(ThreadSupport.CreateThreads(table.Run, "Table", 1, -1, columnWidth, 1));
			for (int i = 0; i < 3; i++) {
				// Select the method used to run the smoker based on the chosen solution strategy
				ThreadStart threadMethod;
				if (strategy == "RESOURCE_TURNSTILING_AND_ITERATION_CHECKING")
					threadMethod = smokers[i].RunSolutionByResourceTurnstilingAndIterationChecking;
				else
					threadMethod = smokers[i].RunIssueDemonstration;
				threads.AddRange(ThreadSupport.CreateThreads(threadMethod, smokers[i].Name, 1, -1, columnWidth, 2+i));
			}
			ThreadSupport.EndColumnHeader(4, columnWidth);
			ThreadSupport.RunThreads(threads);
		}
	}
}