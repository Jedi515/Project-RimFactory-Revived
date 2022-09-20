using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using ProjectRimFactory.Storage;
using RimWorld;
using Verse;

namespace ProjectRimFactory.Common.HarmonyPatches
{
    /// <summary>
    /// Patch that allows items stored in Cold Storage to be Traded locally
    /// This Patch adds the Items in Cold Storage to the List of Items for Trade
    /// </summary>
    [HarmonyPatch(typeof(Pawn_TraderTracker), "ColonyThingsWillingToBuy")]
    class Patch_Pawn_TraderTracker_ColonyThingsWillingToBuy
    {
        static void Postfix(Pawn playerNegotiator, ref IEnumerable<Thing> __result)
        { 
            var map = playerNegotiator.Map;
            if (map is null) return;

            HashSet<Thing> yieldedThings = new HashSet<Thing>();
            yieldedThings.AddRange<Thing>(__result);
            foreach (ILinkableStorageParent dsu in TradePatchHelper.AllPowered(map))
            {
                //Only for Cold Storage
                if (dsu.AdvancedIOAllowed) continue;

                yieldedThings.AddRange<Thing>(dsu.StoredItems);
            }
            __result = yieldedThings;

        }

    }
}
