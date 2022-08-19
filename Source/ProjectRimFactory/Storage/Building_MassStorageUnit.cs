using System.Collections.Generic;
using System.Text;
using System.Linq;
using ProjectRimFactory.Common;
using ProjectRimFactory.Common.HarmonyPatches;
using ProjectRimFactory.Storage.Editables;
using ProjectRimFactory.Storage.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace ProjectRimFactory.Storage
{
    [StaticConstructorOnStartup]
    public abstract class Building_MassStorageUnit : Building, IRenameBuilding, IHaulDestination, IStoreSettingsParent
    {
        private static readonly Texture2D RenameTex = ContentFinder<Texture2D>.Get("UI/Buttons/Rename");

        private List<Thing> items = new List<Thing>();

        private List<Building_StorageUnitIOBase> ports = new List<Building_StorageUnitIOBase>();

        public string UniqueName { get => uniqueName; set => uniqueName = value; }
        private string uniqueName;
        public Building Building => this;

        public StorageSettings settings;

        //Initialized at spawn
        public DefModExtension_Crate ModExtension_Crate = null; 

        public abstract bool CanStoreMoreItems { get; }
        // The maximum number of item stacks at this.Position:
        //   One item on each cell and the rest multi-stacked on Position?
        public int MaxNumberItemsInternal => (ModExtension_Crate?.limit ?? int.MaxValue)
                                              - def.Size.Area + 1;
        public List<Thing> StoredItems => items;
        public int StoredItemsCount => items.Count;
        public override string LabelNoCount => uniqueName ?? base.LabelNoCount;
        public override string LabelCap => uniqueName ?? base.LabelCap;
        public virtual bool CanReceiveIO => true;
        public virtual bool Powered => true;

        public bool ForbidPawnAccess => ModExtension_Crate?.forbidPawnAccess ?? false;

        public virtual bool ForbidPawnInput => ForbidPawnAccess;

        public virtual bool ForbidPawnOutput => ForbidPawnAccess;

        public virtual bool HideItems => ModExtension_Crate?.hideItems ?? false;

        public virtual bool HideRightClickMenus =>
            ModExtension_Crate?.hideRightClickMenus ?? false;

        IntVec3 IHaulDestination.Position => this.Position;

        Map IHaulDestination.Map => this.Map;

        bool IStoreSettingsParent.StorageTabVisible => true;

        public void DeregisterPort(Building_StorageUnitIOBase port)
        {
            ports.Remove(port);
        }

        public void RegisterPort(Building_StorageUnitIOBase port)
        {
            ports.Add(port);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos())
                yield return g;
            yield return new Command_Action
            {
                icon = RenameTex,
                action = () => Find.WindowStack.Add(new Dialog_RenameMassStorageUnit(this)),
                hotKey = KeyBindingDefOf.Misc1,
                defaultLabel = "PRFRenameMassStorageUnitLabel".Translate(),
                defaultDesc = "PRFRenameMassStorageUnitDesc".Translate()
            };
        }

        public virtual string GetUIThingLabel()
        {
            return "PRFMassStorageUIThingLabel".Translate(StoredItemsCount);
        }

        public virtual string GetITabString(int itemsSelected)
        {
            return "PRFItemsTabLabel".Translate(StoredItemsCount, itemsSelected);
        }

        public virtual void RegisterNewItem(Thing newItem)
        {

            Log.Message($"RegisterNewItem: {newItem}");
            if (items.Contains(newItem))
            {
                Log.Message($"dup: {newItem}");
                return;
            }
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item.def.category == ThingCategory.Item && item.CanStackWith(newItem))
                    item.TryAbsorbStack(newItem, true);
                if (newItem.Destroyed) break;
            }

            //Add a new stack of a thing
            if (!newItem.Destroyed)
            {
                items.Add(newItem);

                //What appens if its full?
                if (CanStoreMoreItems)
                {
                    newItem.Position = Position;
                }
                if (newItem.Spawned) newItem.DeSpawn();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref ports, "ports", LookMode.Reference);
            Scribe_Collections.Look(ref items, "items", LookMode.Deep);
            Scribe_Values.Look(ref uniqueName, "uniqueName");
            Scribe_Deep.Look(ref settings, "settings", this);
            ModExtension_Crate ??= def.GetModExtension<DefModExtension_Crate>();
        }

        public override string GetInspectString()
        {
            var original = base.GetInspectString();
            var stringBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(original)) stringBuilder.AppendLine(original);
            stringBuilder.Append("PRF_TotalStacksNum".Translate(items.Count));
            return stringBuilder.ToString();
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            var thingsToSplurge = new List<Thing>(Position.GetThingList(Map));
            for (var i = 0; i < thingsToSplurge.Count; i++)
                if (thingsToSplurge[i].def.category == ThingCategory.Item)
                {
                    thingsToSplurge[i].DeSpawn();
                    GenPlace.TryPlaceThing(thingsToSplurge[i], Position, Map, ThingPlaceMode.Near);
                }
            base.DeSpawn(mode);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            ModExtension_Crate ??= def.GetModExtension<DefModExtension_Crate>();

            Log.Message("----------------");
            foreach (var item in this.StoredItems)
            {
                Log.Message(item.ToString());
            }

        }

        public override void DrawGUIOverlay()
        {
            base.DrawGUIOverlay();
            if (Current.CameraDriver.CurrentZoom <= CameraZoomRange.Close)
                GenMapUI.DrawThingLabel(this, LabelCap + "\n\r" + GetUIThingLabel());
        }

        public bool OutputItem(Thing item)
        {
            var outputCell = GetComp<CompOutputAdjustable>()?.CurrentCell ?? Position + new IntVec3(0, 0, -2);
            return GenPlace.TryPlaceThing(item.SplitOff(item.stackCount), outputCell, Map, ThingPlaceMode.Near);
        }

        //-----------    For compatibility with Pick Up And Haul:    -----------
        //                  (not used internally in any way)
        // true if can store, capacity is how many can store (more than one stack possible)
        public bool CapacityAt(Thing thing, IntVec3 cell, Map map, out int capacity)
        {
            //Some Sanity Checks
            capacity = 0;
            if (thing == null || map == null || map != this.Map || cell == null || !this.Spawned) {
                Log.Error("PRF DSU CapacityAt Sanity Check Error");
                return false;
            }
            thing = thing.GetInnerIfMinified();

            //Check if thing can be stored based upon the storgae settings
            if (!this.Accepts(thing)) {
                return false;
            }

            //TODO Check if we want to forbid access if power is off
            //if (!GetComp<CompPowerTrader>().PowerOn) return false;

            //Get List of items stored in the DSU
            var storedItems = Position.GetThingList(Map).Where(t => t.def.category == ThingCategory.Item);

            //Find the Stack size for the thing
            int maxstacksize = thing.def.stackLimit;
            //Get capacity of partial Stacks
            //  So 45 Steel and 75 Steel and 11 Steel give 30+64 more capacity for steel
            foreach (Thing partialStack in storedItems.Where(t => t.def == thing.def && t.stackCount < maxstacksize)) {
                capacity += maxstacksize - partialStack.stackCount;
            }

            //capacity of empy slots
            capacity += (MaxNumberItemsInternal - storedItems.Count()) * maxstacksize;

            //Access point:
            if (cell != Position) {
                var maybeThing = Map.thingGrid.ThingAt(cell, ThingCategory.Item);
                if (maybeThing != null) {
                    if (maybeThing.def == thing.def) capacity += (thing.def.stackLimit - maybeThing.stackCount);
                } else {
                    capacity += thing.def.stackLimit;
                }
            }
            return capacity > 0;
        }

        // ...The above? I think?  But without needing to know how many
        public bool StackableAt(Thing thing, IntVec3 cell, Map map)
        {
            return CapacityAt(thing, cell, map, out _);
        }

        public bool Accepts(Thing t)
        {
            return settings.AllowedToAccept(t);
        }

        StorageSettings IStoreSettingsParent.GetStoreSettings()
        {
            return settings;
        }

        StorageSettings IStoreSettingsParent.GetParentStoreSettings()
        {
            StorageSettings fixedStorageSettings = def.building.fixedStorageSettings;
            if (fixedStorageSettings != null)
            {
                return fixedStorageSettings;
            }
            return StorageSettings.EverStorableFixedSettings();
        }

        public override void PostMake()
        {
            base.PostMake();
            settings = new StorageSettings(this);
            if (def.building.defaultStorageSettings != null)
            {
                settings.CopyFrom(def.building.defaultStorageSettings);
            }
        }

    }
}