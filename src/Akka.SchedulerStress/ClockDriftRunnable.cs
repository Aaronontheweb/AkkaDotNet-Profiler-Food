using Akka.Dispatch;

namespace Akka.SchedulerStress;

public class ClockDriftRunnable : IRunnable
{
    private readonly DateTime _startNow;
    private DateTime? _lastIntervalComplete = null;
    private readonly TimeSpan _tickInterval;
    private readonly ILoggingAdapter _log;
    private long _tickCount = 0;

    public ClockDriftRunnable(DateTime startNow, ILoggingAdapter log, TimeSpan tickInterval)
    {
        _startNow = startNow;
        _log = log;
        _tickInterval = tickInterval;
    }

    public void Execute()
    {
        if (_tickCount % 100 == 0)
        {
            var expectedCurrentTime = _startNow + _tickCount * _tickInterval;
            var actualCurrentTime = DateTime.UtcNow;
            var cumulativeDrift = actualCurrentTime - expectedCurrentTime;

            _log.Info("Tick {0} - Cumulative Time: expected [{1:MM/dd/yyyy hh:mm:ss.fffK}], found [{2:MM/dd/yyyy hh:mm:ss.fffK}] Drift: [{3}]", _tickCount,
                expectedCurrentTime,
                actualCurrentTime,  cumulativeDrift);

            if (_lastIntervalComplete != null)
            {
                _log.Info("Tick {0} - Interval Drift: expected [{1}], found [{2}]", _tickCount, _tickInterval,
                    actualCurrentTime - _lastIntervalComplete.Value);
            }
        }
     
        _lastIntervalComplete = DateTime.UtcNow;
        _tickCount++;
    }

    public void Run()
    {
        Execute();
    }
}