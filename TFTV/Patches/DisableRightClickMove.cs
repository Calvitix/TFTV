using System;
using System.Reflection;
using HarmonyLib;

using PhoenixPoint.Tactical.View.ViewModules;

namespace TFTV.Patches
{
    [HarmonyPatch]
    public static class UIStateCharacterSelected_OnRightClickMove_Patch
    {
        public static bool Prepare()
        {
            return true;// AssortedAdjustments.Settings.DisableRightClickMove;
        }

        public static MethodBase TargetMethod()
        {
            Type type = AccessTools.TypeByName("UIStateCharacterSelected");
            return AccessTools.Method(type, "OnRightClickMove");
        }

        public static bool Prefix(object __instance)
        {
            try
            {
                TFTVLogger.Debug($"[UIStateCharacterSelected_OnRightClickMove_PREFIX] Preventing right click movement.");

                Type UIStateCharacterSelected = AccessTools.TypeByName("UIStateCharacterSelected");
                UIModuleTacticalContextualMenu _contextualMenuModule = (UIModuleTacticalContextualMenu)AccessTools.Property(UIStateCharacterSelected, "_contextualMenuModule").GetValue(__instance, null);

                if (_contextualMenuModule.IsContextualMenuVisible)
                {
                    Traverse CloseContextualMenu = Traverse.Create(__instance).Method("CloseContextualMenu", true);
                    CloseContextualMenu.GetValue();
                }

                return false;
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
                return true;
            }
        }
    }
}
