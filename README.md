# Concurrency utilities and demos

In the first semester of 2014 I took the Swinburne University unit [*Advanced .Net Programming*](http://www.swinburne.edu.au/study/courses/units/Advanced-.NET-Programming-COS30003/local) taught by his excellence, [Dr Andrew Cain](http://www.swinburne.edu.au/science-engineering-technology/staff-profiles/view.php?who=acain). This was an incredibly interesting class, entirely focused on programming for concurrency (multithreading), in C#. We were each required to create many small utilities that would provide functionality allowing you to easily manage threads. Later, we each also designed and implemented solutions to some concurrency problem scenarios from [*The Little Book of Semaphores*](http://greenteapress.com/semaphores/), including some classics.

## Contents

- Concurrency utilities
    + Semaphore
    + Channel
    + Bound Channel
    + Mutex
    + Latch
    + Barrier
    + Light Switch
    + Exchanger
    + Active Object
    + Semaphore FIFO
    + Reader-Writer Lock
- Concurrency utility tests
- Classical synchronization problems
    + Cigarette Smokers
    + Dining Philosophers
- Less classical synchronization problems
    + River Crossing
    + Hilzer's Barbershop
- Active Objects: Zork Server (basic, one player, multi-user)
- Explanations related to design forces

## Results

Included are many coloured log files demonstrating the programs' functionality and output:

- Concurrency utilities
	+ [Semaphore](Logs/logs_new/Semaphore.htm)
	+ [Channel](Logs/logs_new/Channel.htm)
	+ [Bound Channel](Logs/logs_new/BoundChannel.htm)
	+ [Mutex](Logs/logs_new/Mutex.htm)
	+ [Latch](Logs/logs_new/Latch.htm)
	+ [Barrier](Logs/logs_new/Barrier.htm)
	+ [Light Switch](Logs/logs_new/LightSwitch.htm)
	+ [Exchanger](Logs/logs_new/Exchanger.htm)
	+ [Active Object](Logs/logs_new/ActiveObject.htm)
	+ [Semaphore FIFO](Logs/logs_new/SemaphoreFIFO.htm)
	+ [Reader-Writer Lock](Logs/logs_new/ReaderWriter.htm)
- Classical synchronization problems
    + Cigarette Smokers
		* [CigaretteSmokers_1_issue_fast](Logs/logs_new/CigaretteSmokers_1_issue_fast.htm)
		* [CigaretteSmokers_2_fixed_slow](Logs/logs_new/CigaretteSmokers_2_fixed_slow.htm)
		* [CigaretteSmokers_3_fixed_fast](Logs/logs_new/CigaretteSmokers_3_fixed_fast.htm)
    + Dining Philosophers
		* [DiningPhilosophers_1_issue_fast](Logs/logs_new/DiningPhilosophers_1_issue_fast.htm)
		* [DiningPhilosophers_2_fixed_slow](Logs/logs_new/DiningPhilosophers_2_fixed_slow.htm)
		* [DiningPhilosophers_3_fixed_fast](Logs/logs_new/DiningPhilosophers_3_fixed_fast.htm)
- Less classical synchronization problems
	+ [RiverCrossing](Logs/logs_new/RiverCrossing.htm)
	+ [Hilzer's Barbershop](Logs/logs_new/HilzerBarbershop.htm)
- Active Objects: Zork Server (basic, one player, multi-user)
	+ [ZorkServer_server](Logs/logs_new/ZorkServer_1_server.htm)
	+ [ZorkServer_client_1](Logs/logs_new/ZorkServer_2_client_1.htm)
	+ [ZorkServer_client_2](Logs/logs_new/ZorkServer_2_client_2.htm)