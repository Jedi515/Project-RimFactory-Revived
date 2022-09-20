using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;
using HarmonyLib;

namespace ProjectRimFactory.Common.HarmonyPatches
{
    /// <summary>
    /// Used for Lite Mode to remove Scenarios (#549)
    /// </summary>
    [HarmonyPatch(typeof(Page_SelectScenario), "PreOpen")]
    class Patch_Page_SelectScenario_PreOpen
    {
        static bool Prefix()
        {
            if (ProjectRimFactory_ModSettings.PRF_LiteMode)
            {
                PRF_CustomizeDefs.RemoveScenario(PRF_CustomizeDefs.ExcludeScenario.ToList());
            }

            return true;
        }
    }
}
