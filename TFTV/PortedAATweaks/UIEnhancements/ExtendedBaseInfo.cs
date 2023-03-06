using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Entities.PhoenixBases.FacilityComponents;
using PhoenixPoint.Geoscape.Entities.Sites;
using PhoenixPoint.Geoscape.View.DataObjects;
using PhoenixPoint.Geoscape.View.ViewControllers;
using PhoenixPoint.Geoscape.View.ViewControllers.BaseRecruits;
using PhoenixPoint.Geoscape.View.ViewModules;
using PhoenixPoint.Geoscape.View.ViewStates;
using UnityEngine;

namespace TFTV.Patches.UIEnhancements
{
    internal static class ExtendedBaseInfo
    {
        internal static class PhoenixBaseExtendedInfoData
        {
            public static string HealOutput;
            public static string StaminaOutput;
            public static string ExperienceOutput;
            public static string SkillpointOutput;

            public static new string ToString()
            {
                return $"[PhoenixBaseExtendedInfoData] {HealOutput}, {StaminaOutput}, {ExperienceOutput}, {SkillpointOutput}";
            }
        }



        [HarmonyPatch(typeof(UIModuleGeoAssetDeployment), "SetBaseButtonElement")]
        public static class UIModuleGeoAssetDeployment_SetBaseButtonElement_Patch
        {
            public static bool Prepare()
            {
                return true;// AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowExtendedBaseInfo;
            }

            public static void Postfix(UIModuleGeoAssetDeployment __instance, GeoDeployAssetBaseElementController element, GeoSite site)
            {
                try
                {
                    GeoPhoenixBase phoenixBase = site.GetComponent<GeoPhoenixBase>();
                    if(phoenixBase == null)
                    {
                        return;
                    }

                    Transform anchor = element.PhoenixBaseDetailsRoot.transform?.parent?.parent;
                    if (anchor == null)
                    {
                        throw new InvalidOperationException("Anchor not found. Cannot attach tooltip.");
                    }

                    //TFTVLogger.Debug($"{anchor.name}");
                    //TFTVLogger.Debug($"{anchor.parent.name}");
                    //TFTVLogger.Debug($"{anchor.parent.parent.name}");

                    string tipText = "<!--FONTSIZE:30-->";
                    tipText += $"<size=42><color=#ECBA62>{phoenixBase.Site.Name}</color></size>";
                    tipText += "\n";
                    tipText += $"Santé: {phoenixBase.Stats.HealSoldiersHP} ({phoenixBase.Stats.HealMutogHP}) PV/h";
                    tipText += "\n";
                    tipText += $"Récupération: {phoenixBase.Stats.HealSoldiersStamina} EN/h";
                    tipText += "\n";
                    tipText += $"Entrainement: {phoenixBase.Stats.TrainSoldiersXP} XP/h";

                    List<GeoCharacter> soldiers = phoenixBase.SoldiersInBase.Where(c => c.TemplateDef.IsHuman).ToList();
                    if (soldiers.Count > 0)
                    {
                        tipText += "\n";
                        tipText += "\n";
                        tipText += $"<size=36><color=#ECBA62>SOLDATS</color></size>";
                        foreach (GeoCharacter soldier in soldiers)
                        {
                            tipText += "\n";
                            tipText += $"{soldier.DisplayName.Split((char)32).First()} ({soldier.GetClassViewElementDefs().FirstOrDefault().DisplayName1.Localize()}, Niveau {soldier.Progression.LevelProgression.Level})";
                        }
                    }

                    // Mutogs
                    List<GeoCharacter> mutogs = phoenixBase.SoldiersInBase.Where(c => c.TemplateDef.IsMutog).ToList();
                    if (mutogs.Count > 0)
                    {
                        tipText += "\n";
                        tipText += "\n";
                        tipText += $"<size=36><color=#ECBA62>MUTOGS</color></size>";
                        foreach (GeoCharacter mutog in mutogs)
                        {
                            tipText += "\n";
                            tipText += $"{mutog.DisplayName}";
                        }
                    }

                    // Vehicles
                    List<GeoCharacter> vehicles = phoenixBase.SoldiersInBase.Where(c => c.TemplateDef.IsVehicle).ToList();
                    if (vehicles.Count > 0)
                    {
                        tipText += "\n";
                        tipText += "\n";
                        tipText += $"<size=36><color=#ECBA62>VÉHICULES</color></size>";
                        foreach (GeoCharacter vehicle in vehicles)
                        {
                            tipText += "\n";
                            tipText += $"{vehicle.DisplayName}";
                        }
                    }

                    // Aircraft
                    List<GeoVehicle> aircrafts = phoenixBase.VehiclesAtBase.ToList();
                    if (aircrafts.Count > 0)
                    {
                        tipText += "\n";
                        tipText += "\n";
                        tipText += $"<size=36><color=#ECBA62>VAISSEAUX</color></size>";
                        foreach (GeoVehicle aircraft in aircrafts)
                        {
                            tipText += "\n";
                            tipText += $"{aircraft.Name} ({aircraft.UsedCharacterSpace}/{aircraft.MaxCharacterSpace})";
                        }
                    }

                    // Attach tooltip
                    GameObject anchorGo = anchor.gameObject;
                    if (anchorGo.GetComponent<UITooltipText>() != null)
                    {
                        TFTVLogger.Debug($"[UIModuleGeoAssetDeployment_SetBaseButtonElement_POSTFIX] Tooltip already exists. Refreshing.");
                        anchorGo.GetComponent<UITooltipText>().TipText = tipText;
                        return;
                    }
                    else
                    {
                        anchorGo.AddComponent<UITooltipText>();
                        //anchorGo.AddComponent<CanvasRenderer>();

                        TFTVLogger.Debug($"[UIModuleGeoAssetDeployment_SetBaseButtonElement_POSTFIX] Tooltip not found. Creating.");
                        anchorGo.GetComponent<UITooltipText>().MaxWidth = 280; // Doesn't seem to work
                        anchorGo.GetComponent<UITooltipText>().Position = UITooltip.Position.RightMiddle;
                        anchorGo.GetComponent<UITooltipText>().TipText = tipText;
                    }
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(RecruitsBaseDeployInfoController), "SetBaseInfo")]
        public static class RecruitsBaseDeployInfoController_SetBaseInfo_Patch
        {
            public static bool Prepare()
            {
                return true;// AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowExtendedBaseInfo;
            }

            public static void Prefix(RecruitsBaseDeployInfoController __instance, ref RecruitsBaseDeployData baseInfo)
            {
                try
                {
                    // Fix wrong soldier count (vanilla does not exclude vehicles)
                    baseInfo.SoldiersAtBase = baseInfo.PhoenixBase.SoldiersInBase.Where(c => c.TemplateDef.IsHuman).Count();

                    // Include Mutogs?
                    //baseInfo.SoldiersAtBase = baseInfo.PhoenixBase.SoldiersInBase.Where(c => c.TemplateDef.IsHuman || c.TemplateDef.IsMutog).Count();
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }

            public static void Postfix(RecruitsBaseDeployInfoController __instance, RecruitsBaseDeployData baseInfo)
            {
                try
                {
                    Transform anchor = __instance.BaseNameText.transform?.parent;
                    if (anchor == null)
                    {
                        throw new InvalidOperationException("Anchor not found. Cannot attach tooltip.");
                    }

                    //TFTVLogger.Debug($"{anchor.name}");
                    //TFTVLogger.Debug($"{anchor.parent.name}");
                    //TFTVLogger.Debug($"{anchor.parent.parent.name}");

                    string tipText = "<!--FONTSIZE:36-->";
                    tipText += $"<size=52><color=#ECBA62>{baseInfo.PhoenixBase.Site.Name}</color></size>";
                    tipText += "\n";
                    tipText += $"Santé: {baseInfo.PhoenixBase.Stats.HealSoldiersHP} ({baseInfo.PhoenixBase.Stats.HealMutogHP}) PV/h";
                    tipText += "\n";
                    tipText += $"Récupération: {baseInfo.PhoenixBase.Stats.HealSoldiersStamina} EN/h";
                    tipText += "\n";
                    tipText += $"Entraînement: {baseInfo.PhoenixBase.Stats.TrainSoldiersXP} XP/h";

                    List<GeoCharacter> soldiers = baseInfo.PhoenixBase.SoldiersInBase.Where(c => c.TemplateDef.IsHuman).ToList();
                    if (soldiers.Count > 0)
                    {
                        tipText += "\n";
                        tipText += "\n";
                        tipText += $"<size=42><color=#ECBA62>SOLDATS</color></size>";
                        foreach (GeoCharacter soldier in soldiers)
                        {
                            tipText += "\n";
                            tipText += $"{soldier.DisplayName.Split((char)32).First()} ({soldier.GetClassViewElementDefs().FirstOrDefault().DisplayName1.Localize()}, Niveau {soldier.Progression.LevelProgression.Level})";
                        }
                    }

                    // Mutogs
                    List<GeoCharacter> mutogs = baseInfo.PhoenixBase.SoldiersInBase.Where(c => c.TemplateDef.IsMutog).ToList();
                    if (mutogs.Count > 0)
                    {
                        tipText += "\n";
                        tipText += "\n";
                        tipText += $"<size=42><color=#ECBA62>MUTOGS</color></size>";
                        foreach (GeoCharacter mutog in mutogs)
                        {
                            tipText += "\n";
                            tipText += $"{mutog.DisplayName}";
                        }
                    }

                    // Vehicles
                    List<GeoCharacter> vehicles = baseInfo.PhoenixBase.SoldiersInBase.Where(c => c.TemplateDef.IsVehicle).ToList();
                    if (vehicles.Count > 0)
                    {
                        tipText += "\n";
                        tipText += "\n";
                        tipText += $"<size=42><color=#ECBA62>VÉHICULES</color></size>";
                        foreach (GeoCharacter vehicle in vehicles)
                        {
                            tipText += "\n";
                            tipText += $"{vehicle.DisplayName}";
                        }
                    }

                    // Aircraft
                    List<GeoVehicle> aircrafts = baseInfo.PhoenixBase.VehiclesAtBase.ToList();
                    if (aircrafts.Count > 0)
                    {
                        tipText += "\n";
                        tipText += "\n";
                        tipText += $"<size=42><color=#ECBA62>VAISSEAUX</color></size>";
                        foreach (GeoVehicle aircraft in aircrafts)
                        {
                            tipText += "\n";
                            tipText += $"{aircraft.Name} ({aircraft.UsedCharacterSpace}/{aircraft.MaxCharacterSpace})";
                        }
                    }

                    // Attach tooltip
                    GameObject anchorGo = anchor.gameObject;
                    if (anchorGo.GetComponent<UITooltipText>() != null)
                    {
                        TFTVLogger.Debug($"[RecruitsBaseDeployInfoController_SetBaseInfo_POSTFIX] Tooltip already exists. Refreshing.");
                        anchorGo.GetComponent<UITooltipText>().TipText = tipText;
                        return;
                    }
                    else
                    {
                        anchorGo.AddComponent<UITooltipText>();
                        //anchorGo.AddComponent<CanvasRenderer>();

                        TFTVLogger.Debug($"[RecruitsBaseDeployInfoController_SetBaseInfo_POSTFIX] Tooltip not found. Creating.");
                        anchorGo.GetComponent<UITooltipText>().MaxWidth = 280; // Doesn't seem to work
                        anchorGo.GetComponent<UITooltipText>().Position = UITooltip.Position.RightMiddle;
                        anchorGo.GetComponent<UITooltipText>().TipText = tipText;
                    }
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(UIStateVehicleSelected), "OnSiteMouseHover")]
        public static class UIStateVehicleSelected_OnSiteMouseHover_Patch
        {
            public static bool Prepare()
            {
                return true;// AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowExtendedBaseInfo;
            }

            public static void Prefix(UIStateVehicleSelected __instance, GeoSite site)
            {
                try
                {
                    if (site == null)
                    {
                        return;
                    }

                    if (site.Type == GeoSiteType.PhoenixBase)
                    {
                        TFTVLogger.Debug($"[UIStateVehicleSelected_OnSiteMouseHover_PREFIX] site: {site.Name}");

                        GeoPhoenixBase geoPhoenixBase = site.GetComponent<GeoPhoenixBase>();
                        geoPhoenixBase.UpdateStats();

                        ////PhoenixBaseExtendedInfoData.HealOutput = $"<color=#16aceb>HEALING: {geoPhoenixBase.Stats.HealSoldiersHP} ({geoPhoenixBase.Stats.HealMutogHP}) PV/h</color>";
                        //PhoenixBaseExtendedInfoData.HealOutput = $"<color=#16aceb>HEALING: {geoPhoenixBase.Stats.HealSoldiersHP} PV/h</color>";
                        //PhoenixBaseExtendedInfoData.StaminaOutput = $"<color=#ffffff>RECREATION: {geoPhoenixBase.Stats.HealSoldiersStamina} EN/h</color>";
                        //PhoenixBaseExtendedInfoData.ExperienceOutput = $"<color=#ecba62>TRAINING: {geoPhoenixBase.Stats.TrainSoldiersXP} XP/h</color>";
                        //PhoenixBaseExtendedInfoData.SkillpointOutput = $"<color=#ffa800>SKILL: {geoPhoenixBase.Stats.GainSP} SP/d</color>";

                        PhoenixBaseExtendedInfoData.HealOutput = $"SANTÉ: {geoPhoenixBase.Stats.HealSoldiersHP} PV/h";
                        PhoenixBaseExtendedInfoData.StaminaOutput = $"RÉCUPÉRATION: {geoPhoenixBase.Stats.HealSoldiersStamina} EN/h";
                        PhoenixBaseExtendedInfoData.ExperienceOutput = $"ENTRAINEMENT: {geoPhoenixBase.Stats.TrainSoldiersXP} XP/h";

                        TFTVLogger.Info($"[UIStateVehicleSelected_OnSiteMouseHover_PREFIX] {PhoenixBaseExtendedInfoData.ToString()}");
                    }    
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(UIModuleShortPhoenixBaseTooltip), "SetTooltipData")]
        public static class UIModuleShortPhoenixBaseTooltip_SetTooltipData_Patch
        {
            public static bool Prepare()
            {
                return true;// AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowExtendedBaseInfo;
            }

            public static void Postfix(UIModuleShortPhoenixBaseTooltip __instance, PhoenixBaseShortInfoData baseData)
            {
                try
                {
                    string tab = "    ";
                    __instance.AvailableSlotsText.fontSize = 26;
                    __instance.AvailableSlotsText.lineSpacing = 0.9f;
                    //__instance.AvailableSlotsText.color = Color.white;
                    __instance.AvailableSlotsText.alignment = TextAnchor.UpperLeft;
                    __instance.AvailableSlotsText.resizeTextForBestFit = false;
                    __instance.AvailableSlotsText.alignByGeometry = false;
                    //__instance.AvailableSlotsText.horizontalOverflow = HorizontalWrapMode.Wrap;

                    __instance.AvailableSlotsText.text = $"{tab}{__instance.AvailableSlotsText.text}";



                    if (baseData.IsActivated)
                    {
                        TFTVLogger.Debug($"[UIModuleShortPhoenixBaseTooltip_SetTooltipData_POSTFIX] baseData.BaseName: {baseData.BaseName.Localize()}");

                        string org = __instance.AvailableSlotsText.text.ToUpper();
                        string postfix = "";
                        postfix += $"{tab}{PhoenixBaseExtendedInfoData.HealOutput}\n";
                        postfix += $"{tab}{PhoenixBaseExtendedInfoData.StaminaOutput}\n";
                        postfix += $"{tab}{PhoenixBaseExtendedInfoData.ExperienceOutput}";
                        //postfix += $"{tab}{PhoenixBaseExtendedInfoData.SkillpointOutput}";

                        __instance.AvailableSlotsText.text = $"{org}\n{postfix}";
                    }
                    else if (!baseData.IsBaseInfested)
                    {
                        __instance.InfestationThreatHeaderText.text = "Infestation";
                    }
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(UIModuleBaseLayout), "SetLeftSideInfo")]
        public static class UIModuleBaseLayout_SetLeftSideInfo_Patch
        {
            public static bool Prepare()
            {
                return true;// AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowExtendedBaseInfo;
            }

            public static void Postfix(UIModuleBaseLayout __instance)
            {
                try
                {
                    Transform anchor = __instance.PersonnelAtBaseText.transform?.parent?.parent?.parent?.parent;
                    if (anchor == null)
                    {
                        throw new InvalidOperationException("Anchor not found. Cannot attach tooltip.");
                    }

                    //TFTVLogger.Debug($"{anchor.name}");
                    //TFTVLogger.Debug($"{anchor.parent.name}");
                    //TFTVLogger.Debug($"{anchor.parent.parent.name}");

                    String info = "";// "<!--FONTSIZE:30-->";

                    info += "\n";
                    info += "<size=42><color=#ECBA62>BASE ACTUELLE</color></size>";
                    info += "\n";
                    info += $"<size=36><color=#FFFFFF>Caractéristiques</color></size>";
                    info += "\n";
                    info += "<size=30>";

                    if (__instance.PxBase.Layout.QueryFacilitiesWithComponent<SatelliteUplinkFacilityComponent>(true).Any())
                    {
                        info += $"Portée de détection : {__instance.PxBase.SiteScanner.MaxRange.InMeters / 1000} km";
                        info += "\n";
                    }

                    info += $"Stockage: {__instance.PxBase.Stats.MaxItemCapacity} unités";
                    info += "\n";
                    info += $"Santé: {__instance.PxBase.Stats.HealSoldiersHP} ({__instance.PxBase.Stats.HealMutogHP}) PV/h";
                    info += "\n";
                    info += $"Récupération: {__instance.PxBase.Stats.HealSoldiersStamina} EN/h";
                    info += "\n";
                    info += $"Entraînement: {__instance.PxBase.Stats.TrainSoldiersXP} XP/h";
                    info += "\n";
                    info += $"Réparations: {__instance.PxBase.Stats.RepairVehiclesHP} PV/h";
                    info += "\n";

                    if (__instance.PxBase.Stats.ResourceOutput.Values.Count > 0) {
                        info += "\n";
                        info += $"<size=36><color=#FFFFFF>Ressources</color></size>";
                        info += "\n";
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (ResourceUnit resourceUnit in __instance.PxBase.Stats.ResourceOutput.Values)
                        {
                            stringBuilder.Append(resourceUnit.Type).Append(": ").Append(resourceUnit.Value * 24 + "/j").Append("\n");
                        }
                        info += $"{stringBuilder.ToString()}";
                    }

                    info += "</size>";


                    List<GeoCharacter> allSoldiers = __instance.PxBase.SoldiersInBase.Where(c => c.TemplateDef.IsHuman).ToList();
                    List<GeoCharacter> bruisedSoldiers = allSoldiers.Where(c => c.Health.IntValue < c.Health.IntMax || c.Fatigue.Stamina.IntValue < c.Fatigue.Stamina.IntMax).OrderBy(c => c.Health.IntValue / c.Health.IntMax).ThenBy(c => c.Fatigue.Stamina.IntValue / c.Fatigue.Stamina.IntMax).ToList();

                    // For testing
                    //bruisedSoldiers = allSoldiers;

                    if (allSoldiers.Count > 0)
                    {
                        info += "\n";
                        info += "<size=42><color=#ECBA62>TRAITEMENT</color></size>";
                        info += "\n";
                        if (bruisedSoldiers.Count > 0)
                        {
                            foreach (GeoCharacter soldier in bruisedSoldiers)
                            {
                                info += $"<size=36><color=#FFFFFF>{soldier.DisplayName}</color></size>";
                                info += "\n";
                                if (soldier.Health.IntValue < soldier.Health.IntMax)
                                {
                                    info += $"PV: <color=#CC3333>{soldier.Health.IntValue}/{soldier.Health.IntMax}</color>";
                                }
                                else
                                {
                                    info += $"PV: {soldier.Health.IntValue}/{soldier.Health.IntMax}";
                                }
                                info += ", ";
                                if (soldier.Fatigue.Stamina.IntValue < soldier.Fatigue.Stamina.IntMax)
                                {
                                    info += $"EN: <color=#CC3333>{soldier.Fatigue.Stamina.IntValue}/{soldier.Fatigue.Stamina.IntMax}</color>";
                                }
                                else
                                {
                                    info += $"EN: {soldier.Fatigue.Stamina.IntValue}/{soldier.Fatigue.Stamina.IntMax}";
                                }
                                info += ", ";
                                info += $"XP: {soldier.Progression.LevelProgression.CurrentLevelExperience}/{soldier.Progression.LevelProgression.ExperienceNeededForNextLevel}";
                                info += "\n";
                            }
                        }
                        else
                        {
                            info += $"<size=36><color=#FFFFFF>Tous les soldats sont guéris et reposés.</color></size>";
                            info += "\n";
                        }
                    }

                    List<GeoCharacter> mutogs = __instance.PxBase.SoldiersInBase.Where(c => c.TemplateDef.IsMutog).ToList();
                    if (mutogs.Count > 0)
                    {
                        info += "\n";
                        info += "<size=42><color=#ECBA62>MUTOGS</color></size>";
                        foreach (GeoCharacter mutog in mutogs)
                        {
                            info += "\n";
                            info += $"<size=36><color=#FFFFFF>{mutog.DisplayName}</color></size>";
                            info += "\n";
                            info += "<size=30>";
                            if (mutog.Health.IntValue < mutog.Health.IntMax)
                            {
                                info += $"Santé: <color=#CC3333>{mutog.Health.IntValue}/{mutog.Health.IntMax}</color>";
                            }
                            else
                            {
                                info += $"Santé: {mutog.Health.IntValue}/{mutog.Health.IntMax}";
                            }
                            info += "</size>";
                            info += "\n";
                        }
                    }

                    List<GeoCharacter> vehicles = __instance.PxBase.SoldiersInBase.Where(c => c.TemplateDef.IsVehicle).ToList();
                    if (vehicles.Count > 0)
                    {
                        info += "\n";
                        info += "<size=42><color=#ECBA62>VEHICLES</color></size>";
                        foreach (GeoCharacter vehicle in vehicles)
                        {
                            info += "\n";
                            info += $"<size=36><color=#FFFFFF>{vehicle.DisplayName}</color></size>";
                            info += "\n";
                            info += "<size=30>";
                            if (vehicle.Health.IntValue < vehicle.Health.IntMax)
                            {
                                info += $"Santé: <color=#CC3333>{vehicle.Health.IntValue}/{vehicle.Health.IntMax}</color>";
                            }
                            else
                            {
                                info += $"Santé: {vehicle.Health.IntValue}/{vehicle.Health.IntMax}";
                            }
                            info += "</size>";
                            info += "\n";   
                        }
                    }

                    List<GeoVehicle> aircrafts = __instance.PxBase.VehiclesAtBase.ToList();
                    if (aircrafts.Count > 0)
                    {
                        info += "\n";
                        info += "<size=42><color=#ECBA62>VAISSEAUX</color></size>";
                        foreach (GeoVehicle aircraft in aircrafts)
                        {
                            info += "\n";
                            info += $"<size=36><color=#FFFFFF>{aircraft.Name}</color></size>";
                            info += "\n";
                            info += "<size=30>";
                            info += $"Santé: {(aircraft.VehicleDef.BaseStats.HitPoints)}/{aircraft.VehicleDef.BaseStats.MaxHitPoints}";
                            info += ", ";
                            info += $"Places: {aircraft.UsedCharacterSpace}/{aircraft.MaxCharacterSpace}";
                            info += "</size>";
                            info += "\n";
                        }
                    }


                    // Attach tooltip
                    GameObject anchorGo = anchor.gameObject;
                    if (anchorGo.GetComponent<UITooltipText>() != null)
                    {
                        TFTVLogger.Debug($"[UIModuleBaseLayout_SetLeftSideInfo_POSTFIX] Tooltip already exists. Refreshing.");
                        anchorGo.GetComponent<UITooltipText>().MaxWidth = 280; //Default: 140
                        anchorGo.GetComponent<UITooltipText>().TipText = info;
                        anchorGo.GetComponent<UITooltipText>().UpdateText(info);
                    }
                    else
                    {
                        anchorGo.AddComponent<UITooltipText>();
                        //anchorGo.AddComponent<CanvasRenderer>();

                        TFTVLogger.Debug($"[UIModuleBaseLayout_SetLeftSideInfo_POSTFIX] Tooltip not found. Creating.");
                        anchorGo.GetComponent<UITooltipText>().MaxWidth = 280; //Default: 140
                        anchorGo.GetComponent<UITooltipText>().TipText = info;
                        anchorGo.GetComponent<UITooltipText>().UpdateText(info);
                    }
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }
        }
    }
}
