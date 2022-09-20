using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;
using UnityEngine;
using HarmonyLib;
using ProjectRimFactory.Storage;

namespace ProjectRimFactory.Common.HarmonyPatches
{
    class Patch_ForbidUtility_IsForbidden
    {
        static bool Prefix(Thing t, Pawn pawn, out bool __result)
        {
            __result = true;
            if (t != null)
            {
                Map thingmap = t.Map;
                if (thingmap != null && t.def.category == ThingCategory.Item)
                {
                    if (PatchStorageUtil.GetPRFMapComponent(thingmap)?.ShouldForbidPawnOutputAtPos(t.Position) ?? false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    class Patch_Building_Storage_Accepts
    {
        static bool Prefix(Building_Storage __instance, Thing t, out bool __result)
        {
            __result = false;
            if ((__instance as IForbidPawnInputItem)?.ForbidPawnInput ?? false)
            {
                if (!__instance.slotGroup.HeldThings.Contains(t))
                {
                    return false;
                }
            }
            return true;
        }
    }

    class Patch_FloatMenuMakerMap_ChoicesAtFor
    {
        static bool Prefix(Vector3 clickPos, Pawn pawn, out List<FloatMenuOption> __result)
        {
            if (pawn.Map.GetComponent<PRFMapComponent>().iHideRightMenus.Contains(clickPos.ToIntVec3()))
            {
                __result = new List<FloatMenuOption>();
                return false;
            }
            __result = null;
            return true;
        }
    }

    class Patch_Thing_DrawGUIOverlay
    {
        static bool Prefix(Thing __instance)
        {
            if (__instance.def.category == ThingCategory.Item)
            {
                if (PatchStorageUtil.GetPRFMapComponent(__instance.Map)?.ShouldHideItemsAtPos(__instance.Position) ?? false)
                {
                    return false;
                }
            }
            return true;
        }
    }

    class Patch_ThingWithComps_DrawGUIOverlay
    {
        static bool Prefix(Thing __instance)
        {
            if (__instance.def.category == ThingCategory.Item)
            {
                if (PatchStorageUtil.GetPRFMapComponent(__instance.Map)?.ShouldHideItemsAtPos(__instance.Position) ?? false)
                {
                    return false;
                }
            }
            return true;
        }
    }

    class Patch_Thing_Print
    {
        static bool Prefix(Thing __instance, SectionLayer layer)
        {
            if (__instance.def.category == ThingCategory.Item)
            {
                if (PatchStorageUtil.GetPRFMapComponent(__instance.Map)?.ShouldHideItemsAtPos(__instance.Position) ?? false)
                {
                    return false;
                }
            }
            return true;
        }
    }

    class Patch_MinifiedThing_Print
    {
        static bool Prefix(Thing __instance, SectionLayer layer)
        {
            if (__instance.def.category == ThingCategory.Item)
            {
                if (PatchStorageUtil.GetPRFMapComponent(__instance.Map)?.ShouldHideItemsAtPos(__instance.Position) ?? false)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public interface IHideItem
    {
        bool HideItems { get; }
    }

    public interface IForbidPawnOutputItem
    {
        bool ForbidPawnOutput { get; }
    }

    public interface IForbidPawnInputItem : ISlotGroupParent, IHaulDestination
    {
        bool ForbidPawnInput { get; }
    }
}
