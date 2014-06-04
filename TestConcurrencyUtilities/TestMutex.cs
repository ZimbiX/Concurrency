using System;
using ConcurrencyUtilities;
using Mutex = ConcurrencyUtilities.Mutex;
using System.Collections.Generic;
using System.Threading;

namespace TestConcurrencyUtilities
{
	public class TestMutex
	{
		public static Mutex _accessToBankAccount;
		public static int _magnitude;
		public static int _balance;

		public static void ChangeBalanceBy(int delta) {
			_accessToBankAccount.Acquire();
				_balance = _balance + delta;
				TestSupport.DebugThread("Balance: $" + _balance);
			_accessToBankAccount.Release();
		}

		public static void Incrementer() {
			for (int i = 0; i < _magnitude; i++)
				ChangeBalanceBy(1);
		}

		public static void Decrementer() {
			for (int i = 0; i < _magnitude; i++)
				ChangeBalanceBy(-1);
		}

		public static void Run(int magnitude) {
			_balance = 0;
			_magnitude = magnitude;
			_accessToBankAccount = new Mutex();

			TestSupport.Log(ConsoleColor.Blue, "Barrier test\n==============================");

			TestSupport.Log(ConsoleColor.Blue, "\nTesting mutual exclusion\n---------------------");
			TestSupport.Log(ConsoleColor.Blue, "\nA mutex will be used to modify a bank account balance by positive and negative $" + _magnitude + " in steps of $1. The resulting balance should be $0.");
			TestSupport.SleepThread(4000);

			List<Thread> threads = new List<Thread>();
			threads.AddRange( TestSupport.CreateThreads(Incrementer, "Incrementer", 1) );
			threads.AddRange( TestSupport.CreateThreads(Decrementer, "Decrementer", 1) );
			TestSupport.RunThreads(threads);

			TestSupport.Log(ConsoleColor.Blue, "\nTesting release methods\n---------------------");
			Mutex m = new Mutex();

			TestSupport.Log(ConsoleColor.Blue, "\nTesting Release():");
			Console.WriteLine("  Acquire()...");
			m.Acquire();
			Console.WriteLine("    Release()...");
			m.Release();

			TestSupport.Log(ConsoleColor.Blue, "\nTesting Release(1):");
			Console.WriteLine("  Acquire()...");
			m.Acquire();
			Console.WriteLine("    Release(1)...");
			m.Release(1);
			
			TestSupport.Log(ConsoleColor.Blue, "\nTesting Release(2):");
			Console.WriteLine("  Acquire()...");
			m.Acquire();
			try {
				Console.WriteLine("    Release(2) (should throw exception):");
				m.Release(2);
			} catch {
				Console.WriteLine("  Exception thrown successfully for Release(2)");
			}
			
			TestSupport.Log(ConsoleColor.Blue, "\nTesting Release() * 2:");
			Console.WriteLine("  Mutex still has 0 tokens");
			try {
				Console.WriteLine("    Release()...");
				m.Release();
				Console.WriteLine("    Release() (should throw exception)...");
				m.Release();
			} catch {
				Console.WriteLine("  Exception thrown successfully for extra Release()");
			}

			Console.WriteLine("\nFinished");
		}
	}
}