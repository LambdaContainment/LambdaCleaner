using System.Collections.Generic;
using System.Linq;
using Decals;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using MEC;
using Mirror;
using UnityEngine;
using System.Linq;

namespace LambdaCleaner
{
    public class PeriodicalCleanup
    {
        private readonly LambdaCleaner plugin;

        public PeriodicalCleanup(LambdaCleaner plugin)
        {
            this.plugin = plugin;
        }

        private readonly HashSet<ItemCategory> ProtectedItems = new HashSet<ItemCategory>
        {
            ItemCategory.Keycard,
            ItemCategory.SpecialWeapon,
            ItemCategory.SCPItem
        };

        private IEnumerator<float> CleanRagdoll(List<Ragdoll> ragdolls, System.Action<int> onComplete)
        {
            int count = 0;
            foreach (Ragdoll ragdoll in ragdolls)
            {
                if (ragdoll != null && ragdoll.GameObject != null)
                {
                    NetworkServer.Destroy(ragdoll.GameObject);
                    count++;
                    yield return Timing.WaitForSeconds(0.1f);
                }
            }
            onComplete?.Invoke(count);
        }

        private IEnumerator<float> CleanItems(List<Pickup> pickups, Action<int> onComplete)
        {
            int count = 0;
            foreach (Pickup pickup in pickups)
            {
                if (
                    pickup?.GameObject == null ||
                    ProtectedItems.Contains(pickup.Category) ||
                    pickup.PreviousOwner == null
                )
                    continue;

                pickup.Destroy();
                count++;
                yield return Timing.WaitForSeconds(0.1f);
            }
            onComplete?.Invoke(count);
        }

        public IEnumerator<float> RunCleanupLoop(float interval)
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(interval);

                var allRagdolls = Ragdoll.List.ToList();
                var allPickups = Pickup.List.ToList().Where(p =>
                    p != null &&
                    p.GameObject != null &&
                    !ProtectedItems.Contains(p.Category) &&
                    p.PreviousOwner != null
                ).ToList();

                if (allRagdolls.Count > 0 || allPickups.Count > 0)
                {
                    Map.Broadcast(5, "<size=25><b><color=#00FFFF>[</color>LambdaCleaner<color=#00FFFF>]</color></b> <color=#eeeeee>Limpeza geral em 15 segundos...</color></size>");

                    yield return Timing.WaitForSeconds(15f);

                    int totalCleaned = 0;

                    yield return Timing.WaitUntilDone(
                        Timing.RunCoroutine(CleanRagdoll(allRagdolls, (result) => totalCleaned += result))
                    );

                    yield return Timing.WaitUntilDone(
                        Timing.RunCoroutine(CleanItems(allPickups, (result) => totalCleaned += result))
                    );

                    Log.Info($"[LambdaCleaner] Sucesso removendo {totalCleaned} objetos (corpos + itens).");
                }
            }
        }
    }
}