using HarmonyLib;
using Verse;
using ProjectRimFactory.Drones;

namespace ProjectRimFactory.Common.HarmonyPatches
{
    /// <summary>
    /// Patch that makes makes Drones Colonists while overrideIsColonist is True
    /// 
    /// used by Patch_Mineable_TrySpawnYield & Patch_Locks2_ConfigRuleRace_Allows
    /// </summary>
    [HarmonyPatch(typeof(Pawn), "get_IsColonist")]
    static class Patch_Pawn_IsColonist
    {
        static void Postfix(Pawn __instance, ref bool __result)
        {
            if (overrideIsColonist && __instance is Pawn_Drone && !__result && __instance.Faction != null && __instance.Faction.IsPlayer)
            {
                __result = true;
            }
        }
        public static bool overrideIsColonist = false;
    }
}
