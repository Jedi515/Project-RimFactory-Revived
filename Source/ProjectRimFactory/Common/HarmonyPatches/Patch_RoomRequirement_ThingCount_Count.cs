using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using HarmonyLib;
using Verse;


namespace ProjectRimFactory.Common.HarmonyPatches
{
    /// <summary>
    /// This Patch make the DroneColumn to count for the Royalty Room requirements
    /// </summary>
    [HarmonyPatch(typeof(RoomRequirement_ThingCount), "Count")]
    class Patch_RoomRequirement_ThingCount_Count
    {
        static void Postfix(Room r, ref int __result , RoomRequirement_ThingCount __instance )
        {
            if (__result < __instance.count && __instance.thingDef == PRFDefOf.Column)
            {
                __result += r.ThingCount(PRFDefOf.PRF_MiniDroneColumn);
            }
        }

    }
}
