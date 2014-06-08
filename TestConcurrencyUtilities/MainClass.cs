using System;

namespace TestConcurrencyUtilities
{
	public class MainClass
	{
		public static void Main(string[] args) {
			// Config:
			double sleepTimeSeconds = 1.0;
//			int testMagnitude = 10;

			int sleepTimeMs = (int)Math.Round(sleepTimeSeconds*1000);

			Console.WriteLine("Which test do you want to run?" +
			                  "\n  1. Semaphore" +
			                  "\n  2. Channel" +
			                  "\n  3. Bound Channel" +
			                  "\n  4. Mutex" +
			                  "\n  5. Latch" +
			                  "\n  6. Barrier" +
			                  "\n  7. Light Switch" +
			                  "\n  8. Exchanger" +
			                  "\n  9. Active Object" +
			                  "\n 10. Semaphore FIFO" +
			                  "\n 11. Reader-Writer Lock" +
			                  "\n  Q. Quit");
			string response = "INVALID_RESPONSE";
//			response = "1"; // Can preload with an option to choose automatically
			do {
				if (response == "INVALID_RESPONSE") {
					Console.Write("Enter: ");
					response = Console.ReadLine().ToUpper();
				}
				switch (response) {
				case "1" : TestSemaphore.Run(10, sleepTimeMs); 			break;
				case "2" : TestChannel.Run(10, sleepTimeMs); 			break;
				case "3" : TestBoundChannel.Run(10, 3, sleepTimeMs); 	break;
				case "4" : TestMutex.Run(100000); 						break;
				case "5" : TestLatch.Run(4, sleepTimeMs); 				break;
				case "6" : TestBarrier.Run(4, sleepTimeMs);				break;
				case "7" : TestLightSwitch.Run(5, sleepTimeMs); 		break;
				case "8" : TestExchanger.Run(12, sleepTimeMs); 			break;
				case "9" : TestActiveObjects.Run();						break;
				case "10": TestSemaphoreFIFO.Run(10, 300);				break;
				case "11": TestReaderWriter.Run(sleepTimeMs);			break;
				case "Q" : break;
				default:
					Console.WriteLine("Invalid response");
					response = "INVALID_RESPONSE";
					break;
				}
			} while (response == "INVALID_RESPONSE");

			if (response != "Q")
				TestSupport.Log(ConsoleColor.DarkGreen, "\nAll threads have finished");
		}
	}
}