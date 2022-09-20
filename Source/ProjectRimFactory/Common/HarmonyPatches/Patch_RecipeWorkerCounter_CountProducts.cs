using RimWorld;
using Verse;
using HarmonyLib;

namespace ProjectRimFactory.Common.HarmonyPatches
{
    /// <summary>
    /// Counts items in the AssemblerQueue for do until x recipes
    /// 
    /// Art & maybe other things too need a separate patch
    /// </summary>
    [HarmonyPatch(typeof(RecipeWorkerCounter), "CountProducts")]
    class Patch_RecipeWorkerCounter_CountProducts
    {
        
        static void Postfix(RecipeWorkerCounter __instance,ref int __result, Bill_Production bill)
        {
            if (bill.includeFromZone == null) {
                int i = 0;
                ThingDef targetDef = __instance.recipe.products[0].thingDef;
                PRFGameComponent gamecomp = Current.Game.GetComponent<PRFGameComponent>();

                for (i = 0; i < gamecomp.AssemblerQueue.Count; i++)
                {
                    //Don't count Things of other maps
                    if (bill.Map != gamecomp.AssemblerQueue[i].Map) continue;
                    foreach (Thing heldThing in gamecomp.AssemblerQueue[i].GetThingQueue())
                    {
                        Thing innerIfMinified = heldThing.GetInnerIfMinified();
                        if (innerIfMinified.def == targetDef)
                        {
                            __result += innerIfMinified.stackCount;
                        }
                    }
                }
            }
        }
    }




}
