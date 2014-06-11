using System;
using System.Threading; // Required for access to Thread
using System.Collections.Generic; // Required for access to List
using ConcurrencyUtilities;
using Colorizer = AnsiColor.AnsiColor;

namespace TestConcurrencyUtilities
{
	public static class TestSupport
	{
		public struct TSpec {
			public ThreadStart Method;
			public string Name;
			public int NumThreads;
			public int StartingNum;
			
			public TSpec(string name, ThreadStart method, int numThreads, int startingNum) {
				Method = method;
				Name = name;
				NumThreads = numThreads;
				StartingNum = startingNum;
			}
		}
		
		/// <summary>
		/// Creates threads from the given array of thread specifications.
		/// </summary>
		/// <returns>The threads created from thethread specifications.</returns>
		/// <param name="threadSpecs">An array of thread specifications.</param>
		/// <param name="columnWidth">The character width for the thread logging columns, including padding.</param>
		public static List<Thread> CreateThreadsFromThreadSpecs(TSpec[] threadSpecs, int columnWidth) {
			List<Thread> threads = new List<Thread>();
			foreach (TSpec ts in threadSpecs)
				threads.AddRange(TestSupport.CreateThreads(ts.Method, ts.Name, ts.NumThreads, ts.StartingNum,
				                                           columnWidth, threads.Count + 1));
			TestSupport.EndColumnHeader(threads.Count, columnWidth);
			return threads;
		}

		// Create the specified number of thread objects which will run the supplied method. These are identified by the supplied name (onto which is added the thread's ID number). The threads are returned in a list object
		public static List<Thread> CreateThreads(ThreadStart threadMethod, string threadID, int numThreads, int startingNum = 0, int columnWidth = 0, int startingColumn = 1) {
			List<Thread> threads = new List<Thread>();
			for (int i = 0; i < numThreads; i++) {
				Thread t = new Thread(threadMethod);
				if (columnWidth == 0) { // If not using columns
					t.Name = threadID;
				} else {
					string idNumSpacer = columnWidth > 0 ? "" : " ";
					string nameSuffix = "";;
					if (startingNum >= 0)
						nameSuffix = idNumSpacer + (i + startingNum).ToString();
					if (columnWidth == 0) {
						t.Name = threadID + nameSuffix;
					} else {
						string realName = threadID + nameSuffix; // The real thread name without the column indicator or column padding
						t.Name = CreateColumnIndicator(columnWidth, i + startingColumn - 1) + realName; // Add column indicator and column padding to the thread name
						Console.Write(realName.PadRight(columnWidth)); // Output the header for this column. A manual newline will be required after creating all the required thread groups
					}
				}
				threads.Add(t);
			}
			return threads;
		}

		public static string CreateColumnIndicator(int columnWidth, int columnNum) {
			return "%%%%" + new string(' ', columnWidth * columnNum);
		}

		public static string ColumnPaddingFromLongName() {
			string longName = ThreadLongName();
			if (longName == null)
				longName = "%%%%"; // Column 1 (fixes unnecessarily including the spacer for no thread name)
			string shortName = ThreadName(); // Get the real thread name without the column indicator or column padding
			string columnWhitespace = "";
			if (longName.StartsWith("%%%%")) {
				// For the second column of an 8 character wide column layout, the thread's name would be:
				// %%%%        Name
				string nameWithoutColumnIndicatorPrefix = longName.Substring(4);
				// Get the horizontal offset whitespace to use for column padding
				int columnWhitespaceCount = (nameWithoutColumnIndicatorPrefix.IndexOf(shortName));
				columnWhitespace = new string(' ', columnWhitespaceCount);
			}
			return columnWhitespace;
		}

		public static bool IsColumnised() {
			return ThreadLongName().StartsWith("%%%%");
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
			TestSupport.JoinThreads(threads); // Wait for all the threads to finish
		}

		public static void RunThreads(List<Thread> threads, int sleepTimeBetweenStarts) {
			foreach (Thread t in threads) { // Start the threads
				TestSupport.SleepThread(sleepTimeBetweenStarts, false);
				t.Start();
			}
			TestSupport.JoinThreads(threads); // Wait for all the threads to finish
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
		private static void SleepThreadWithMessage(int msSleepTime, string sleepMessage) {
			if (msSleepTime > 0) {
				int sleepTimePreLog = 130;
				int sleepTimeA = (msSleepTime > sleepTimePreLog ? sleepTimePreLog               : (int)Math.Round((double)msSleepTime / 2));
				int sleepTimeB = (msSleepTime > sleepTimePreLog ? msSleepTime - sleepTimePreLog : (int)Math.Round((double)msSleepTime / 2));
				Thread.Sleep(sleepTimeA);
				Console.Write(Colorizer.Colorize(sleepMessage)); // Now that any logging from other threads should be finished, print a line indicating that we're sleeping
				Thread.Sleep(sleepTimeB);
			}
		}

		public static void SleepThread(int msSleepTime, bool newLineWithEllipsis = true) {
			SleepThreadWithMessage(msSleepTime, newLineWithEllipsis ? "{white}...\n" : "");
		}

		public static void SleepThread(int msSleepTime, string sleepMessage) {
			SleepThreadWithMessage(msSleepTime, sleepMessage + "\n");
		}

		// Log a message to the console, indicating from which thread it originated
		public static void DebugThread(string message) {
			string shortNameToLog = IsColumnised() ? "" : ThreadName() + " ";
			string columnWhitespace = ColumnPaddingFromLongName();
			Log(columnWhitespace + shortNameToLog + message);
		}
		
		public static void DebugThreadWithPrefix(string prefix, string message) {
			Console.WriteLine(prefix + Thread.CurrentThread.Name + " " + message);
		}
		
		public static void Log(string message) {
			// Uses 'AnsiColor', downloaded from: http://www.codeproject.com/Articles/24753/Using-ANSI-Colors-within-NET
			Console.WriteLine(Colorizer.Colorize("{reset}" + message + "{reset}"));
		}

		public static string ThreadLongName() {
			return Thread.CurrentThread.Name;
		}

		public static string ThreadName() {
			string name = Thread.CurrentThread.Name;
			if (name == null) {
				name = "";
			} else if (name.StartsWith("%%%%")) {
				// For the second column of an 8 character wide column layout, the thread's name would be:
				// %%%%        Name
				string nameWithoutColumnIndicatorPrefix = name.Substring(4);
				name = nameWithoutColumnIndicatorPrefix.TrimStart(' '); // Get the real thread name without the column indicator or column padding
			}
			return name;
		}

		public static void Log(ConsoleColor colour, string message) {
			Console.ForegroundColor = colour;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}
