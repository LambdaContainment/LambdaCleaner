using Exiled.API.Features;
using MEC;

namespace LambdaCleaner;

public class EventManager
{
    private LambdaCleaner plugin;
    public EventManager(LambdaCleaner plugin) => this.plugin = plugin;
    private static PeriodicalCleanup cleaner;

    public void OnRoundStart()
    {
        try
        {
            plugin.CleanupCoroutineHandle = Timing.RunCoroutine(cleaner.RunCleanupLoop(Config.CleanLoopInterval));
        }
        catch (Exception e)
        {
            Log.Error($"Error while starting Cleaner coroutine: {e}");
        }
    }

    public void OnRoundRestart()
    {
        Timing.KillCoroutines(cleaner.cleanupCoroutine);
    }
}