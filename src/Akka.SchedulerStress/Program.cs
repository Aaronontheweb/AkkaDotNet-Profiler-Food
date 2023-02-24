var actorSystem = ActorSystem.Create("MyActorSystem");

var scheduler = actorSystem.Scheduler;

scheduler.Advanced.ScheduleRepeatedly(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(20));