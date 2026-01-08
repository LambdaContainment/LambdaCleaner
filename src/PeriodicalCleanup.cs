using Exiled.API.Features;
using MEC;
using Mirror;
using UnityEngine;

namespace LambdaCleaner;

public class PeriodicalCleanup
{
    public readonly LambdaCleaner plugin;
    public PeriodicalCleanup(LambdaCleaner plugin) => this.plugin = plugin;
    
    public IEnumerator<float> RunCleanupLoop(float interval)
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(interval);
            
            var allRagdolls = Ragdoll.List.ToList();
            
            if (allRagdolls.Count > 0)
            {
                Map.Broadcast(
                    8,
                    "<size=35><b><color=#00FFFF>[</color>LambdaCleaner<color=#00FFFF>]</color></b> <color=#eeeeee>Limpeza de corpos em 15 segundos...</color></size>"
                );

                yield return Timing.WaitForSeconds(15f);

                Log.Info($"[LambdaCleaner] Cleaned {allRagdolls.Count} ragdolls");
                int count = 0;

                foreach (Ragdoll ragdoll in allRagdolls)
                {
                    if (ragdoll != null && ragdoll.GameObject != null)
                    {
                        NetworkServer.Destroy(ragdoll.Base.gameObject);
                        count++;

                        yield return Timing.WaitForSeconds(0.1f);
                    }
                }

                if (count > 0)
                {
                    Log.Info($"[LambdaCleaner] Sucesso: {count} ragdolls removidos.");
                }
            }
        }
    }
}