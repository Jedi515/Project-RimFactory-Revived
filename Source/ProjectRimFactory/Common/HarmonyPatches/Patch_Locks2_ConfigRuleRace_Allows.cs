using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using ProjectRimFactory.Drones;

namespace ProjectRimFactory.Common.HarmonyPatches
{
    /// <summary>
    /// Adds Compatibility with "Locks2" When using the Race Filter for Drones
    /// by considering them to be Colonists for the duration of the function
    /// </summary>
    static class Patch_Locks2_ConfigRuleRace_Allows
    {

        static public void Prefix(Pawn pawn)
        {
            if (pawn is Pawn_Drone)
            {
                Patch_Pawn_IsColonist.overrideIsColonist = true;
            }
        }

        static public void Postfix(Pawn pawn)
        {
            if (pawn is Pawn_Drone)
            {
                Patch_Pawn_IsColonist.overrideIsColonist = false;
            }
        }
    }
}
