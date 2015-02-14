# Design forces in concurrent programming

## Safety

Safety is the design force pertaining to maintaining system invariants, i.e., making sure that a value does not change while is it being observed. The locking functionality of a programming language allows you to introduce concurrency safety, ensuring mutually-exclusive access to a piece of memory.

Locking is a critical safety precaution required for creating multi-threaded programs that will operate correctly. If two threads alter some data in parallel, then it is likely that one of the alterations will be lost. Consider a bank account balance with two threads attempting to add one dollar to it. The process to do so is to: read the value in memory, perform the increment calculation, then write the result back into memory. If one thread has not written its new value back before the other reads it for the calculation input, some data will be lost, the stored result being the value from whichever thread finishes last. To prevent this from happening, a lock should be used to protect access to the value, making the critical section of code mutually-exclusive.

## Liveness

The liveness design force is about ensuring a program continues to make progress towards completion. In the course of executing, threads in a multithreaded program will have to wait for certain things to happen in the other threads. Some amount of waiting is perfectly normal, and unavoidable; waiting indefinitely, on the other hand, is not acceptable, destroying liveness. This can be caused by deadlock, starvation, or missed signals.

Locks are powerful, but when used incorrectly, can cause a dead-lock. This is a situation where some of a program's threads permanently stall due to waiting for tokens that will never be released. This can be caused by a bad order of semaphore operations. For example, two threads have a critical section of code that requires the acquisition of tokens from two mutexes. If the order of token acquisition is different between threads, then there is the possibility for a deadlock to occur -- each thread could acquire the token from its first mutex, but now becuase they are both stuck waiting for the token from their second one, both are blocked from releasing the token that the other one needs.

Starvation is where a thread is waiting to continue, but the program design allows that a number of other threads are allowed to keep cycling through the critical section of code, indefinitely blocking the starved thread. The naive reader-writer lock utility is a good example of this, where readers are continuously let through while a writer has to wait for there to be no readers -- if a torrent of readers has no end in sight, the writer will be waiting forever.

A missed signal occurs with a thread that is to be waiting for a signal. The signal is sent before the thread actually starts to wait for it.

Deadlocks are the result of poor program design and need to be avoided. As most matters of thread safety and liveness cannot be checked automatically, it comes down to programmer discipline to ensure that the concurrency has good safety and liveness properties.

## Performance

A program's performance property is a result of the measures taken to balance a program's safety and liveness. Ideally, a program should run as fast as possible (which is why we're even doing multithreading), but not at the cost of accuracy (safety) or durability (liveness).

The process of locking is quite slow. Whilst it does increase a program's safety, this mutual exclusion prevents its threads from processing in parallel. Both mutual exclusion and the locks themselves, decrease a program's speed of execution, and so should be used as infrequently as possible. There is however, a fine line between code that runs fast and correctly, and code that runs fast but is incorrect and useless.

## Reusability

Issues with safety and liveness are reduced somewhat by utilising well-designed concurrency utilities. Being able to reuse a mutex utility, in place of manually sending and waiting for pulses, reduces the immense conceptual complexity of designing a mutithreaded application. It also helps to avoid safety/liveness-affecting inconsistencies across multiple pieces of code that manage shared permissions.

## References

- Topic 3 lecture slides
- http://www.informit.com/articles/article.aspx?p=167813&seqNum=4, *Concurrent Object-Oriented Programming*, Doug Lea, May 25, 2001 -- Section 1.3: Design Forces