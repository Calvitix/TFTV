using System;
using HarmonyLib;
using PhoenixPoint.Geoscape.Levels;

namespace TFTV.Patches
{
    [HarmonyPatch(typeof(GeoscapeLog), "PhoenixFaction_OnRecruitsRegenerated")]
    public static class GeoscapeLog_PhoenixFaction_OnRecruitsRegenerated_Patch
    {
        public static bool Prepare()
        {
            return true;// AssortedAdjustments.Settings.PauseOnRecruitsGenerated;
        }

        public static void Prefix(GeoscapeLog __instance, GeoLevelController ____level)
        {
            try
            {
                TFTVLogger.Info($"[GeoscapeLog_PhoenixFaction_OnRecruitsRegenerated_PREFIX] Pausing.");

                ____level.View.RequestGamePause();
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }
    }
}
