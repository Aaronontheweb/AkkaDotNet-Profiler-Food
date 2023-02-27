using Akka.SchedulerStress;

var actorSystem = ActorSystem.Create("MyActorSystem");
var logger = Logging.GetLogger(actorSystem, typeof(ClockDriftRunnable));
var scheduler = actorSystem.Scheduler;

var runnable = new ClockDriftRunnable(DateTime.UtcNow, logger, TimeSpan.FromMilliseconds(20));

scheduler.Advanced.ScheduleRepeatedly(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(20), runnable);

await actorSystem.WhenTerminated;