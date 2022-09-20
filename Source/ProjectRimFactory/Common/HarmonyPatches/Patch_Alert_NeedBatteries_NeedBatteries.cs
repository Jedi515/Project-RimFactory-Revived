using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using ProjectRimFactory.Archo.Things;
using ProjectRimFactory.Industry;

namespace ProjectRimFactory.Common.HarmonyPatches
{
    /// <summary>
    /// Makes Building_CustomBattery and Building_HexCapacitor count for the NeedBatteries Alert
    /// </summary>
    [HarmonyPatch(typeof(Alert_NeedBatteries), "NeedBatteries")]
    class Patch_Alert_NeedBatteries_NeedBatteries
    {
        static void Postfix(Alert_NeedBatteries __instance, Map map, ref bool __result)
        {
            if (__result == true)
            {
                if (map.listerBuildings.ColonistsHaveBuilding((Thing building) => building is Building_HexCapacitor))
                {
                    __result = false;
                }
                else if (map.listerBuildings.ColonistsHaveBuilding((Thing building) => building is Building_CustomBattery))
                {
                    __result = false;
                }
            }
            

        }

    }
}
