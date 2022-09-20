using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using ProjectRimFactory.Drones;

namespace ProjectRimFactory.Common.HarmonyPatches
{
    /// <summary>
    /// This Patch Prevents Drones from Uninstalling or Deconstructing their own Station
    /// </summary>
    [HarmonyPatch(typeof(WorkGiver_RemoveBuilding), "PotentialWorkThingsGlobal")]
    class Patch_WorkGiver_RemoveBuilding_PotentialWorkThingsGlobal
    {

        static void Postfix(Pawn pawn , ref IEnumerable<Thing> __result)
        {
            if (pawn.kindDef == PRFDefOf.PRFDroneKind)
            {
                Pawn_Drone drone = (Pawn_Drone)pawn;
                IntVec3 DroneStationPos = drone.station.Position;

                //Remove work on the station itself
                __result = __result.Where(u => u.Position != DroneStationPos).ToList();
            }
        }
    }

}
