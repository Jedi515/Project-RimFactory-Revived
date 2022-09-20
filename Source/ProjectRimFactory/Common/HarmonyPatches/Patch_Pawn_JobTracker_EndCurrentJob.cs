using System;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using HarmonyLib;
using Verse.AI;
using ProjectRimFactory.Drones;

namespace ProjectRimFactory.Common.HarmonyPatches
{
    /// <summary>
    /// This Patch handles Pathing errors for Drones.
    /// This was Primarily created for a RW Bug where there was inadequate checking if a Job is possible see 
    /// https://github.com/zymex22/Project-RimFactory-Revived/issues/80#issuecomment-702288254
    /// 
    /// However there is also the possibility that a User may block a drone from reaching it's station
    /// This need separate handling
    /// </summary>
    [HarmonyPatch(typeof(Pawn_JobTracker), "EndCurrentJob")]
    class Patch_Pawn_JobTracker_EndCurrentJob
    {

        static bool Prefix(Pawn_JobTracker __instance, JobCondition condition, Pawn ___pawn, bool startNewJob = true, bool canReturnToPool = true)
        {
            //Only run the Prefix if its a Drone and the Expected Error Condition did occur
            if (___pawn.kindDef == PRFDefOf.PRFDroneKind && (condition == JobCondition.ErroredPather || condition == JobCondition.Errored) )
            {
                Pawn_Drone drone = (Pawn_Drone)___pawn;

                var stationPos = drone.station.Position;
                var targetPos = __instance.curJob.GetTarget(TargetIndex.A).Cell;

                //Run default Code (may need to update that in the Future (if RW Updates This Method))
                JobDef jobDef = (__instance.curJob != null) ? __instance.curJob.def : null;
                Traverse.Create(__instance).Method("CleanupCurrentJob", condition, true, true, canReturnToPool).GetValue();

                if (stationPos == targetPos)
                {
                    //Drone can't go anywhere Needs to Stop
                    Log.Warning($"Pathing for Drone Failed - Path to Station @{stationPos} is Blocked. Powering down drone");
                    var currentPos = ___pawn.Position;
                    var map = ___pawn.Map;
                    ___pawn.Destroy();
                    Thing module = ThingMaker.MakeThing(PRFDefOf.PRF_DroneModule);
                    module.stackCount = 1;
                    GenPlace.TryPlaceThing(module, currentPos, map, ThingPlaceMode.Direct);
                }
                else
                {
                    Log.Warning($"Pathing for Drone Failed - Returning to Station - (This is a Rimworld Pathing Bug) check Cells @{targetPos}");
                    //Send the Drone Home
                    __instance.StartJob(new Job(PRFDefOf.PRFDrone_ReturnToStation, drone.station));
                }


                return false;
            }
            return true;


        }
    }
}
