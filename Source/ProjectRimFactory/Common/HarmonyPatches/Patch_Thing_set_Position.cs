using HarmonyLib;
using ProjectRimFactory.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ProjectRimFactory.Common.HarmonyPatches
{
    /// <summary>
    /// Calls Notify_LostThing on Building_MassStorageUnit if the Item was stored in a Building_MassStorageUnit
    /// This does mot seem to check if the position is actually different.
    /// 
    /// TODO: Check its use
    /// </summary>
    [HarmonyPatch(typeof(Thing), "set_Position")]
    public static class Patch_Thing_set_Position
    {
        public static void Prefix(Thing __instance, out Building_MassStorageUnit __state)
        {
            __state = null;
            IntVec3 pos = __instance.Position;
            if (__instance.def.category == ThingCategory.Item && pos.IsValid && __instance.Map != null)
            {
                __state = pos.GetFirst<Building_MassStorageUnit>(__instance.Map);
            }
        }
        public static void Postfix(Thing __instance, Building_MassStorageUnit __state)
        {
            __state?.Notify_LostThing(__instance);
        }
    }
}
