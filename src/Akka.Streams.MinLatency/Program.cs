using Akka.Streams;

var actorSystem = ActorSystem.Create("MyActorSystem");

var source = Source.Repeat(1L) // lowest possible allocation source
    .Batch(100, i => 0L, (i, i1) => i + i1); 

var t = source.Async().AlsoTo(Flow.Create<long>().Where(i => i % 2 == 0).To(CreatePerSecondCounterSink("i % 2")))
    .Async().To(Flow.Create<long>().Where(i => i % 3 == 0).To(CreatePerSecondCounterSink("i % 3")))
    .Run(actorSystem.Materializer());

Sink<long, NotUsed> CreatePerSecondCounterSink(string name)
{
    return Flow.Create<long>()
        .GroupedWithin(int.MaxValue, TimeSpan.FromSeconds(1.0))
        .Select(c => c.LongCount())
        .Aggregate((0L, DateTime.UtcNow), (l, s) =>
        {
            var (accum, time) = l;
            accum += s;

            var timeSpan = (DateTime.UtcNow - time);
            if (timeSpan.TotalSeconds >= 1.0d)
            {
                Console.WriteLine($"[{name}] {accum} msg/{timeSpan.TotalSeconds}");
                return (0L, DateTime.UtcNow); // reset
            }

            return (accum, time);
        })
        .To(Sink.Ignore<(long, DateTime)>());
}

await actorSystem.WhenTerminated;