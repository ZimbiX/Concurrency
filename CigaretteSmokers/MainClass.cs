using System;
using System.Collections.Generic;
using System.Threading;
using ThreadSupport = TestConcurrencyUtilities.TestSupport;

namespace CigaretteSmokers
{
	class MainClass
	{
		public static void Main(string[] args) {
			// Create resources

			List<SmokerResource> resources = new List<SmokerResource>();
			foreach (string name in new string[] { "Tobacco", "Paper", "Match" })
				resources.Add(new SmokerResource(name));

			// Create table

			Table table = new Table(resources);

			// Create smokers

			List<Smoker> smokers = new List<Smoker>();
			for (int i = 0; i < 3; i++) {
				List<SmokerResource> resourcesNeededBySmoker = new List<SmokerResource>(resources);
				string smokerName = "S+" + resources[i].Name;
				resourcesNeededBySmoker.RemoveAt(i); // Remove the resource that they don't require any more of
				smokers.Add(new Smoker(smokerName, table, resourcesNeededBySmoker));
			}

			// Create threads

			int columnWidth = 23;
			List<Thread> threads = new List<Thread>();
			threads.AddRange(ThreadSupport.CreateThreads(table.Run, "Table", 1, -1, columnWidth, 1));
			for (int i = 0; i < 3; i++) {
				threads.AddRange(ThreadSupport.CreateThreads(smokers[i].Run, smokers[i].Name, 1, -1, columnWidth, 2+i));
			}
			ThreadSupport.EndColumnHeader(4, columnWidth);
			ThreadSupport.RunThreads(threads);
		}
	}
}