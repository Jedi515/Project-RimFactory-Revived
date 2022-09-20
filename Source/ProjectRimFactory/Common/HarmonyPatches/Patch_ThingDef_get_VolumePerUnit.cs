using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ProjectRimFactory.Common.HarmonyPatches
{
    /// <summary>
    /// Sets the VolumePerUnit of Paperclips / EMC
    /// </summary>
    [HarmonyPatch(typeof(ThingDef), "get_VolumePerUnit")]
    public class Patch_ThingDef_get_VolumePerUnit
    {
        public static bool Prefix(ThingDef __instance, ref float __result)
        {
            if (__instance == PRFDefOf.Paperclip)
            {
                __result = 0.00001f;
                return false;
            }
            return true;
        }
    }
}
