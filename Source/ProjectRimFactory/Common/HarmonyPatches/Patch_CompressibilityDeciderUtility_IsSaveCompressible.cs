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
    /// This has something to do with saving Things when stored in a DSU
    /// 
    /// TODO: Check why we need that
    /// </summary>
    [HarmonyPatch(typeof(CompressibilityDeciderUtility), "IsSaveCompressible")]
    public static class Patch_CompressibilityDeciderUtility_IsSaveCompressible
    {
        public static void Postfix(Thing t, ref bool __result)
        {
            if (__result && t.Map != null && t.Position.IsValid && t.Position.GetFirst<Building_MassStorageUnit>(t.Map) is Building_MassStorageUnit)
            {
                __result = false;
            }
        }
    }
}
