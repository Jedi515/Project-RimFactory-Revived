using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using ProjectRimFactory.Storage;
using HarmonyLib;
using Verse.AI;


namespace ProjectRimFactory.Common.HarmonyPatches
{
    /// <summary>
    /// Destructive prefix for Reserving IO Inputs
    /// This allows multiple pawns to use the Port at the same time
    /// 
    /// Might cause issues in the edge case where to many items are moved to the Port
    /// </summary>
    [HarmonyPatch(typeof(Verse.AI.ReservationManager), "Reserve")]
    class Patch_ReservationManager_Reserve
    {
        static bool Prefix(Pawn claimant, Job job, LocalTargetInfo target ,ref bool __result , Map ___map)
        {
            if (target.HasThing == false && ___map != null && target.Cell.InBounds(___map))
            {
                Building_StorageUnitIOBase building_target = (Building_StorageUnitIOBase)target.Cell.GetThingList(___map).Where(t => t is Building_StorageUnitIOBase).FirstOrDefault();
                if (building_target != null && building_target.mode == StorageIOMode.Input)
                {
                    __result = true;
                    return false;
                }
            }
            return true;
        }
    }
}