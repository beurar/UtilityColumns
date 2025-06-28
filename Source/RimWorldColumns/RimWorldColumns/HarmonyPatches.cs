using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using Verse.AI;

namespace RimWorldColumns
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        public static RWCSettingsDef settingsDef;
        public static readonly List<ThingDef> extraColumns = new List<ThingDef>();

        static HarmonyPatches()
        {
            Log.Message("[Utility Columns] - Patching...");
            //Load..
            settingsDef = UCDefOf.ColumnSettings;
            extraColumns.Add(UCDefOf.Column);
            extraColumns.AddRange(settingsDef.columnsForRoomRequirement);

            var harmony = new Harmony("nephlite.orbitaltradecolumn");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            VEPatch();
            //LongEventHandler.QueueLongEvent(new Action(VEPatch), "LibraryStartup", false, null);
        }

        private static void VEPatch()
        {
            // Patch VE - RoyaltyPatches
            if (ModLister.GetActiveModWithIdentifier("OskarPotocki.VanillaExpanded.RoyaltyPatches") != null)
            {
                foreach (var d in DefDatabase<RoyalTitleDef>.AllDefs)
                {
                    if(d == null) continue;
                    if (d.bedroomRequirements != null)
                    {
                        foreach (var c in d.bedroomRequirements)
                        {
                            if (c is RoomRequirement_ThingAnyOf rr)
                                AddColumns(ref rr);
                        }
                    }
                    if (d.throneRoomRequirements != null)
                    {
                        foreach (var c in d.throneRoomRequirements)
                        {
                            if (c is RoomRequirement_ThingAnyOf rr)
                                AddColumns(ref rr);
                        }

                    }
                }
            }
            
            // Patch VE - IdeologyPatches
            if (ModLister.GetActiveModWithIdentifier("OskarPotocki.VanillaExpanded.IdeologyPatches") != null)
            {
                foreach (var d in DefDatabase<PreceptDef>.AllDefs)
                {
                    if (d == null) continue;
                    if (d.buildingRoomRequirements != null)
                    {
                        foreach (var c in d.buildingRoomRequirements)
                        {
                            if (c is RoomRequirement_ThingAnyOf rr)
                                AddColumns(ref rr);
                        }
                    }
                }
            }
            /* 
            if (patched)
                Log.Message("[Utility Columns] Successfully patched Vanially Expanded - Royaltys Patch");
            else
                Log.Error("[Utility Columns] Failed to patch Vanially Expanded - Royaltys Patch");
            */
        }

        private static void AddColumns(ref RoomRequirement_ThingAnyOf rr)
        {
            if (rr.things.Contains(UCDefOf.Column))
            {
                rr.things.AddRange(extraColumns);
            }
        }
    }

    [HarmonyPatch(typeof(RoomRequirement_ThingCount), "Count")]
    static class Patch_RoomRequirement_ThingCount_Count
    {
        static void Postfix(RoomRequirement_ThingCount __instance, ref int __result, Room r)
        {
            if (__instance.thingDef != UCDefOf.Column) return;

            __result += HarmonyPatches.extraColumns.Sum(r.ThingCount);
        }
    }

    [HarmonyPatch(typeof(Building_Turret), "ThreatDisabled")]
    static class Patch_Building_Turret_ThreatDisabled
    {
        static void Postfix(Building_Turret __instance, ref bool __result, IAttackTargetSearcher disabledFor)
        {
            if (__instance is Building_TurretGunColumn column)
            {
                var compAmmo = column.TryGetComp<CompRefuelable>();
                if (compAmmo != null && !compAmmo.HasFuel)
                    __result = true;
            }
        }
    }

}
