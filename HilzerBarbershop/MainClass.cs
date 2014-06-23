using System;
using System.Threading;
using ConcurrencyUtilities;
using Mutex = ConcurrencyUtilities.Mutex;
using Semaphore = ConcurrencyUtilities.Semaphore;
using TestConcurrencyUtilities;
using System.Collections.Generic;

namespace HilzerBarbershop
{
	class MainClass
	{
		static int _customers;
		static Mutex _mutex;
		static SemaphoreFIFO _standingRoom;
		static SemaphoreFIFO _sofa;
		static Semaphore _chair;
		static Semaphore _barber;
		static Semaphore _customer;
		static Semaphore _cash;
		static Semaphore _receipt;
		static int _sleepTime;

		static void EnterShop() {
		}

		static void SitOnSofa() {
		}

		static void SitInBarberChair() {
		}

		static void GetHairCut() {
		}

		static void CutHair() {
			TestSupport.SleepThread(_sleepTime);
		}

		static void Pay() {
			TestSupport.SleepThread(_sleepTime);
		}

		static void AcceptPayment() {
		}

		static void ExitShop() {
			TestSupport.DebugThread("{!red}{black}ExitShop");
			Thread.CurrentThread.Abort();
		}

		static void Customer() {
			TestSupport.DebugThread("{!yellow}{black}EnterShop");
			_mutex.Acquire();
				if (_customers == 20) {
					_mutex.Release();
					ExitShop();
				}
				_customers++;
			_mutex.Release();

			_standingRoom.Acquire();
			EnterShop();
			TestSupport.DebugThread("{!green}{black}EnterShop");

			TestSupport.DebugThread("{!yellow}{black}SitOnSofa");
			_sofa.Acquire();
			SitOnSofa();
			TestSupport.DebugThread("{!green}{black}SitOnSofa");
			_standingRoom.Release();

			TestSupport.DebugThread("{!yellow}{black}SitInBarberChair");
			_chair.Acquire();
			SitInBarberChair();
			TestSupport.DebugThread("{!green}{black}SitInBarberChair");
			_sofa.Release();

			// Rendezvous with the barber
			TestSupport.DebugThread("{!yellow}{black}GetHairCut");
			_customer.Release();
			_barber.Acquire();
			GetHairCut();
			TestSupport.DebugThread("{!green}{black}GetHairCut");

			TestSupport.DebugThread("{!yellow}{black}Pay");
			Pay();
			_cash.Release();
			_receipt.Acquire();
			TestSupport.DebugThread("{!green}{black}Pay");
			_chair.Release();
			TestSupport.DebugThread("{!red}{black}SitInBarberChair");

			_mutex.Acquire();
				_customers--;
				TestSupport.DebugThread("{black}_customers: " + _customers);
			_mutex.Release();

			ExitShop();
		}

		static void Barber() {
			while (true) {
				// Rendezvous with the customer
				TestSupport.DebugThread("{!yellow}{black}CutHair");
				_customer.Acquire();
				_barber.Release();
				CutHair();
				TestSupport.DebugThread("{!green}{black}CutHair");

				TestSupport.DebugThread("{!cyan}{black}AcceptPayment");
				_cash.Acquire(); // Wait for a customer to have given some cash
				AcceptPayment();
				_receipt.Release(); // Give a customer the receipt
				TestSupport.DebugThread("{!green}{black}AcceptPayment");
			}
		}

		public static void Main(string[] args) {
			_customers = 0;
			_mutex = new Mutex();
			_standingRoom = new SemaphoreFIFO(16);
			_sofa = new SemaphoreFIFO(4);
			_chair = new Semaphore(3);
			_barber = new Semaphore(0);
			_customer = new Semaphore(0);
			_cash = new Semaphore(0);
			_receipt = new Semaphore(0);

			_sleepTime = 2000;
			int columnWidth = 14;

			TestSupport.SleepThread(2000);

			List<Thread> threads = new List<Thread>();
			threads.AddRange(TestSupport.CreateThreads(Barber,   "B", 3,  1, columnWidth, threads.Count + 1));
			threads.AddRange(TestSupport.CreateThreads(Customer, "C", 10, 1, columnWidth, threads.Count + 1));
			TestSupport.EndColumnHeader(threads.Count, columnWidth);
			TestSupport.RunThreads(threads);
		}
	}
}