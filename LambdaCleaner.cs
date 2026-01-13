using Exiled.API.Features;
using MEC;

namespace LambdaCleaner;

public class LambdaCleaner : Plugin<Config>
{
    public override string Name => "LambdaCleaner";
    public override string Author => "WinG4merBR";
    public override Version Version => new Version(1, 0, 0);
    public CoroutineHandle CleanupCoroutineHandle;
    private EventManager EventManager;
    
    public override void OnEnabled()
    {
        EventManager = new EventManager(this);
        Exiled.Events.Handlers.Server.RoundStarted += EventManager.OnRoundStart;
        Exiled.Events.Handlers.Server.RestartingRound += EventManager.OnRoundRestart;
        base.OnEnabled();
    }
    
    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Server.RoundStarted -= EventManager.OnRoundStart;
        Exiled.Events.Handlers.Server.RestartingRound -= EventManager.OnRoundRestart;
        Timing.KillCoroutines(CleanupCoroutineHandle);
        base.OnDisabled();
    }
}
