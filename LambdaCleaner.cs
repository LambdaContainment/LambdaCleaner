using Exiled.API.Features;
using MEC;

namespace LambdaCleaner;

public class LambdaCleaner : Plugin<Config>
{
    public override string Name => "LambdaCleaner";
    public override Version Version => new Version(1, 0, 0);
    public PeriodicalCleanup cleaner;
    public CoroutineHandle cleanupCoroutine;
    
    public override void OnEnabled()
    {
        cleaner = new PeriodicalCleanup(this);
        Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
        Exiled.Events.Handlers.Server.RestartingRound += onRoundRestart;
        base.OnEnabled();
    }

    private void onRoundRestart()
    {
        Timing.KillCoroutines(cleanupCoroutine);
    }
    private void OnRoundStarted()
    {
        try
        {
            cleanupCoroutine = Timing.RunCoroutine(cleaner.RunCleanupLoop(Config.CleanLoopInterval));
        }
        catch (Exception e)
        {
            Log.Error($"Error while starting Cleaner coroutine: {e}");
        }
    }
    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
        Exiled.Events.Handlers.Server.RestartingRound -= onRoundRestart;
        Timing.KillCoroutines(cleanupCoroutine);
        base.OnDisabled();
    }
}
