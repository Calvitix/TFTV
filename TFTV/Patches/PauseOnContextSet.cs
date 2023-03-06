using System;
using System.Linq;
using Base.Core;
using HarmonyLib;
using PhoenixPoint.Geoscape.Entities.Abilities;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Geoscape.View;
using PhoenixPoint.Geoscape.View.ViewStates;

namespace TFTV.Patches
{
    [HarmonyPatch(typeof(UIStateVehicleSelected), "AddTravelSite")]
    public static class UIStateVehicleSelected_AddTravelSite_Patch
    {
        public static bool Prepare()
        {
            return true;// AssortedAdjustments.Settings.PauseOnDestinationSet;
        }

        public static void Prefix(UIStateVehicleSelected __instance, ref bool __state)
        {
            try
            {
                GeoscapeViewContext _GeoscapeViewContext = (GeoscapeViewContext)AccessTools.Property(typeof(GeoscapeViewState), "Context").GetValue(__instance, null);
                __state = _GeoscapeViewContext.Level.Timing.Paused;

                TFTVLogger.Debug($"[UIStateVehicleSelected_AddTravelSite_PREFIX] Current time setting: {(__state ? "Paused" : "Running")}.");
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void Postfix(UIStateVehicleSelected __instance, ref bool __state)
        {
            try
            {
                TFTVLogger.Debug($"[UIStateVehicleSelected_AddTravelSite_POSTFIX] Called.");

                int craftCount = GameUtl.CurrentLevel().GetComponent<GeoLevelController>().ViewerFaction.Vehicles.Count();
                TFTVLogger.Info($"[UIStateVehicleSelected_AddTravelSite_POSTFIX] craftCount: {craftCount}");

                if (craftCount > 1)
                {
                    TFTVLogger.Debug($"[UIStateVehicleSelected_AddTravelSite_POSTFIX] New vehicle travel plan. Keeping time setting at: {(__state ? "Paused" : "Running")}.");

                    GeoscapeViewContext _GeoscapeViewContext = (GeoscapeViewContext)AccessTools.Property(typeof(GeoscapeViewState), "Context").GetValue(__instance, null);
                    _GeoscapeViewContext.Level.Timing.Paused = __state;
                }
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }
    }

    [HarmonyPatch(typeof(UIStateVehicleSelected), "OnContextualItemSelected")]
    public static class UIStateVehicleSelected_OnContextualItemSelected_Patch
    {
        public static bool Prepare()
        {
            return true;// AssortedAdjustments.Settings.PauseOnDestinationSet || AssortedAdjustments.Settings.PauseOnExplorationSet;
        }

        public static void Prefix(UIStateVehicleSelected __instance, ref bool __state)
        {
            try
            {
                GeoscapeViewContext _GeoscapeViewContext = (GeoscapeViewContext)AccessTools.Property(typeof(GeoscapeViewState), "Context").GetValue(__instance, null);
                __state = _GeoscapeViewContext.Level.Timing.Paused;

                TFTVLogger.Debug($"[UIStateVehicleSelected_OnContextualItemSelected_PREFIX] Current time setting: {(__state ? "Paused" : "Running")}.");
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void Postfix(UIStateVehicleSelected __instance, GeoAbility ability, ref bool __state)
        {
            try
            {
                if (!((ability is MoveVehicleAbility) || (ability is ExploreSiteAbility ))) //&& AssortedAdjustments.Settings.PauseOnDestinationSet && AssortedAdjustments.Settings.PauseOnExplorationSet)))
                {
                    return;
                }

                int craftCount = GameUtl.CurrentLevel().GetComponent<GeoLevelController>().ViewerFaction.Vehicles.Count();
                TFTVLogger.Info($"[UIStateVehicleSelected_OnContextualItemSelected_POSTFIX] craftCount: {craftCount}");

                if (craftCount > 1)
                {
                    TFTVLogger.Debug($"[UIStateVehicleSelected_OnContextualItemSelected_POSTFIX] Keeping time setting at: {(__state ? "Paused" : "Running")}.");

                    GeoscapeViewContext _GeoscapeViewContext = (GeoscapeViewContext)AccessTools.Property(typeof(GeoscapeViewState), "Context").GetValue(__instance, null);
                    _GeoscapeViewContext.Level.Timing.Paused = __state;
                }
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }
    }
}
