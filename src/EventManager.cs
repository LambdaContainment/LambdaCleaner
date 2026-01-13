using Exiled.API.Features;
using HarmonyLib;
using MEC;
 
namespace LambdaCleaner;

public class EventManager
{
    private readonly LambdaCleaner plugin;
    private readonly PeriodicalCleanup cleaner;

    public EventManager(LambdaCleaner plugin)
    {
        this.plugin = plugin;
        cleaner = new PeriodicalCleanup(plugin);
    }

    public void OnRoundStart()
    {
        try
        {
            plugin.CleanupCoroutineHandle = Timing.RunCoroutine(
                cleaner.RunCleanupLoop(
                    plugin.Config.CleanLoopInterval
                )
            );
        }
        catch (Exception e)
        {
            Log.Error($"Error while starting Cleaner coroutine: {e}");
        }
    }

    public void OnRoundRestart()
    {
        if (plugin.CleanupCoroutineHandle.IsRunning)
        {
            Timing.KillCoroutines(plugin.CleanupCoroutineHandle);
        }
    }
}