using System;

namespace ConcurrencyUtilities
{
	// A lightswitch can be used to give multiple threads shared access to a single permisson. It works like a light switch in a room -- the first person to enter the room turns the light on, and the last person to leave turns the light off. Here, the light is a metaphor for a permisson. When the light is on, any thread using this light switch has (the) permission. When the light switch is off, that permission is free to be acquired by another thread
	public class LightSwitch
	{
		Semaphore _managedPermisson; // The semaphore that the lightswitch will control permisson with
		int _numThreads = 0;
		Mutex _accessToNumThreads;

		// Initialise the light switch - you need to supply it with a semaphore
		public LightSwitch(Semaphore permissonToManage) {
			_managedPermisson = permissonToManage;
			_accessToNumThreads = new Mutex();
		}

		// Start using the lightswitch's permisson in the current thread. If there was no threads using the lightswitch-managed permisson, take the permisson (acquire a token from the semaphore). If there were already threads, we already have permission.
		// In other words:
		// Enter the room. If there was no one in the room, turn on the light.
		public void Acquire() {
			_accessToNumThreads.Acquire();
				if (_numThreads == 0) // Check if there was no one in the room
					_managedPermisson.Acquire();
				_numThreads++;
			_accessToNumThreads.Release();
		}

		// Stop using the lightswitch's permisson in the current thread. If there are no other threads using the lightswitch-managed permisson, release the permisson (release a token back to the semaphore). If there are still other threads using the permisson, leave the permission still acquired.
		// In other words:
		// Leave the room. If there is now no one left in the room, turn off the light.
		public void Release() {
			_accessToNumThreads.Acquire();
				_numThreads--;
				if (_numThreads == 0) // Check if there is now no one in the room
					_managedPermisson.Release();
			_accessToNumThreads.Release();
		}
	}
}

