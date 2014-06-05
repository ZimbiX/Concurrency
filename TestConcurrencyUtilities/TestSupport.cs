using System;
using System.Threading; // Required for access to Thread
using System.Collections.Generic; // Required for access to List
using ConcurrencyUtilities;
using Colorizer = AnsiColor.AnsiColor;

namespace TestConcurrencyUtilities
{
	public class TestSupport
	{
		// Create the specified number of thread objects which will run the supplied method. These are identified by the supplied name (onto which is added the thread's ID number). The threads are returned in a list object
		public static List<Thread> CreateThreads(ThreadStart threadMethod, string threadID, int numThreads, int startingNum = 0, int columnWidth = 0, int startingColumn = 1) {
			List<Thread> threads = new List<Thread>();
			for (int i = 0; i < numThreads; i++) {
				Thread t = new Thread(threadMethod);
				if (columnWidth == 0) { // If not using columns
					t.Name = threadID;
				} else {
					string idNumSpacer = columnWidth > 0 ? "" : " ";
					string nameSuffix = idNumSpacer + (i + startingNum).ToString();
					if (columnWidth == 0) {
						t.Name = threadID + nameSuffix;
					} else {
						string realName = threadID + nameSuffix; // The real thread name without the column indicator or column padding
						t.Name = "%%%%" + new string(' ', columnWidth * (i + startingColumn - 1)) + realName; // Add column indicator and column padding to the thread name
						Console.Write(realName.PadRight(columnWidth)); // Output the header for this column. A manual newline will be required after creating all the required thread groups
					}
				}
				threads.Add(t);
			}
			return threads;
		}

		// Create a single thread with a given name, returned in a list object
		public static List<Thread> CreateThread(ThreadStart threadMethod, string threadID) {
			return CreateThreads(threadMethod, threadID, 1);
		}

		public static void EndColumnHeader(int numColumns, int columnWidth) {
			Console.WriteLine( "\n" + new string('-', numColumns * columnWidth) );
		}

		// Run the threads - start each one, and then wait for them all to finish
		public static void RunThreads(List<Thread> threads) {
			foreach (Thread t in threads)
				t.Start();
			foreach (Thread t in threads)
				t.Join();
		}

		// Join the threads - wait for them all to finish
		public static void JoinThreads(List<Thread> threads) {
			foreach (Thread t in threads)
				t.Join();
		}

		// An overloaded version of RunThreads which allows supplying the threads as an array
		public static void RunThreads(Thread[] threads) {
			RunThreads(new List<Thread>(threads));
		}

		// Return a worded string that uses seconds to describes a number of milliseconds.
		// e.g. "1 second", "0.53 seconds", "45 seconds"
		public static string StringFromMilliseconds(int milliseconds) {
			string unit;
			decimal seconds = (decimal)milliseconds / 1000;
			unit = "second";
			string secondsStr = seconds.ToString();
			if (seconds != 1) {
				unit += "s";
				if (seconds != 0)
					secondsStr = secondsStr.TrimEnd('0');
			}
			return secondsStr + " " + unit;
		}

		// Make the current thread sleep for a specified number of milliseconds, logging the fact that it is sleeping, and what its name is
		public static void SleepThread(int msSleepTime, bool blankLine = true) {
			if (msSleepTime > 0) {
				int sleepTimePreLog = 130;
				int sleepTimeA = (msSleepTime > sleepTimePreLog ? sleepTimePreLog               : (int)Math.Round((double)msSleepTime / 2));
				int sleepTimeB = (msSleepTime > sleepTimePreLog ? msSleepTime - sleepTimePreLog : (int)Math.Round((double)msSleepTime / 2));
				Thread.Sleep(sleepTimeA);
				if (blankLine)
					Console.WriteLine("..."); // Now that any logging from other threads should be finished, print a line indicating that we're sleeping
				Thread.Sleep(sleepTimeB);
			}
		}

		// Log a message to the console, indicating from which thread it originated
		public static void DebugThread(string message) {
			string longName = Thread.CurrentThread.Name;
			string shortName = ThreadName(); // Get the real thread name without the column indicator or column padding
			string outputNameAndMessageSpacer = shortName + " ";
			string columnWhitespace = "";
			if (longName.StartsWith("%%%%")) {
				// For the second column of an 8 character wide column layout, the thread's name would be:
				// %%%%        Name
				outputNameAndMessageSpacer = ""; // Disable the forced spacing between a thread's name and message
				string nameWithoutColumnIndicatorPrefix = longName.Substring(4);
				int columnWhitespaceCount = (nameWithoutColumnIndicatorPrefix.IndexOf(shortName)); // Get the horizontal offset whitespace to use for column padding
				columnWhitespace = new string(' ', columnWhitespaceCount);
			}
			// Uses 'AnsiColor', downloaded from: http://www.codeproject.com/Articles/24753/Using-ANSI-Colors-within-NET
			Console.WriteLine(Colorizer.Colorize("{white}" + columnWhitespace + outputNameAndMessageSpacer + message));
		}

		public static string ThreadName() {
			string name = Thread.CurrentThread.Name;
			if (name.StartsWith("%%%%")) {
				// For the second column of an 8 character wide column layout, the thread's name would be:
				// %%%%        Name
				string nameWithoutColumnIndicatorPrefix = name.Substring(4);
				name = nameWithoutColumnIndicatorPrefix.TrimStart(' '); // Get the real thread name without the column indicator or column padding
			}
			return name;
		}

		public static void DebugThreadWithPrefix(string prefix, string message) {
			Console.WriteLine(prefix + Thread.CurrentThread.Name + " " + message);
		}

		//		public static void DebugThreadWithPrefix(ConsoleColor colour, string message) {
		//			Console.ForegroundColor = colour;
		//			DebugThread(message);
		//			Console.ForegroundColor = ConsoleColor.White;
		//		}

		public static void Log(ConsoleColor colour, string message) {
			Console.ForegroundColor = colour;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}
