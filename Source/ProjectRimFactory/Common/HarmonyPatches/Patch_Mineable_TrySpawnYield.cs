using HarmonyLib;
using Verse;
using RimWorld;
using ProjectRimFactory.Drones;

namespace ProjectRimFactory.Common.HarmonyPatches
{
    /// <summary>
    /// Patch to prevent that items mined by drones to be forbidden
    /// by considering them to be Colonists for the duration of the function
    /// </summary>
    [HarmonyPatch(typeof(Mineable), "TrySpawnYield")]
    static class Patch_Mineable_TrySpawnYield
    {
        static void Prefix(Mineable __instance, Map map, float yieldChance, bool moteOnWaste, Pawn pawn)
        {
            if (pawn is Pawn_Drone)
            {
                Patch_Pawn_IsColonist.overrideIsColonist = true;
            }
        }

        static void Postfix(Mineable __instance, Map map, float yieldChance, bool moteOnWaste, Pawn pawn)
        {
            if (pawn is Pawn_Drone)
            {
                Patch_Pawn_IsColonist.overrideIsColonist = false;
            }
        }
    }
}
