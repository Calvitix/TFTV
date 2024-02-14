using System;
using System.Collections.Generic;
using System.Linq;
using Base.Core;
using Base.Defs;
using HarmonyLib;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Geoscape.Entities.PhoenixBases;
using PhoenixPoint.Geoscape.Entities.PhoenixBases.FacilityComponents;
using PhoenixPoint.Geoscape.Entities.Sites;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Geoscape.Levels.Factions;
using PhoenixPoint.Geoscape.View.ViewControllers.PhoenixBase;

namespace TFTV.Patches
{
    internal static class FacilityAdjustments
    {
        internal static float currentHealFacilityHealOutput;
        internal static float currentHealFacilityStaminaHealOutput;
        internal static float currentVehicleSlotFacilityAircraftHealOuput;
        internal static float currentVehicleSlotFacilityVehicleHealOuput;
        internal static float currentHealFacilityMutogHealOutput;
        internal static float currentFoodProductionFacilitySuppliesOutput;
        internal static int currentExperienceFacilityExperienceOutput;



        public static void Apply()
        {
            //HarmonyInstance harmony = HarmonyInstance.Create(typeof(EconomyAdjustments).Namespace);
            DefRepository defRepository = GameUtl.GameComponent<DefRepository>();

            if (TFTVMain.Main.Config.ApplyCalvitixChanges)
            {
            List<HealFacilityComponentDef> healFacilityComponentDefs = defRepository.DefRepositoryDef.AllDefs.OfType<HealFacilityComponentDef>().ToList();
            foreach (HealFacilityComponentDef hfcDef in healFacilityComponentDefs)
            {
                if (hfcDef.name.Contains("MedicalBay"))
                {
                    hfcDef.BaseHeal = 4.0f;// AssortedAdjustments.Settings.MedicalBayBaseHeal;
                    currentHealFacilityHealOutput = hfcDef.BaseHeal;
                }
                else if (hfcDef.name.Contains("MutationLab"))
                {
                    hfcDef.BaseHeal = 20.0f;// AssortedAdjustments.Settings.MutationLabMutogHealAmount;
                    currentHealFacilityMutogHealOutput = hfcDef.BaseHeal;
                }
                else if (hfcDef.name.Contains("LivingQuarters"))
                {
                    hfcDef.BaseStaminaHeal = 2.0f;// AssortedAdjustments.Settings.LivingQuartersBaseStaminaHeal;
                    currentHealFacilityStaminaHealOutput = hfcDef.BaseStaminaHeal;
                }
                TFTVLogger.Info($"[FacilityAdjustments_Apply] hfcDef: {hfcDef.name}, GUID: {hfcDef.Guid}, BaseHeal: {hfcDef.BaseHeal}, BaseStaminaHeal: {hfcDef.BaseStaminaHeal}");
            }

            List<ExperienceFacilityComponentDef> experienceFacilityComponentDefs = defRepository.DefRepositoryDef.AllDefs.OfType<ExperienceFacilityComponentDef>().ToList();
            foreach (ExperienceFacilityComponentDef efcDef in experienceFacilityComponentDefs)
            {
                if (efcDef.name.Contains("TrainingFacility"))
                {
                    efcDef.ExperiencePerUser = 1;// AssortedAdjustments.Settings.TrainingFacilityBaseExperienceAmount;
                    currentExperienceFacilityExperienceOutput = efcDef.ExperiencePerUser;

                    efcDef.SkillPointsPerDay = 1;// AssortedAdjustments.Settings.TrainingFacilityBaseSkillPointsAmount;
                }
                TFTVLogger.Info($"[FacilityAdjustments_Apply] efcDef: {efcDef.name}, GUID: {efcDef.Guid}, ExperiencePerUser: {efcDef.ExperiencePerUser}, SkillPointsPerDay: {efcDef.SkillPointsPerDay}");
            }

            List<VehicleSlotFacilityComponentDef> vehicleSlotFacilityComponentDefs = defRepository.DefRepositoryDef.AllDefs.OfType<VehicleSlotFacilityComponentDef>().Where(vsfDef => vsfDef.name.Contains("VehicleBay")).ToList();
            foreach (VehicleSlotFacilityComponentDef vsfDef in vehicleSlotFacilityComponentDefs)
            {
                vsfDef.AircraftHealAmount = 25;// AssortedAdjustments.Settings.VehicleBayAircraftHealAmount;
                vsfDef.VehicleHealAmount = 20;// AssortedAdjustments.Settings.VehicleBayVehicleHealAmount;

                currentVehicleSlotFacilityAircraftHealOuput = vsfDef.AircraftHealAmount;
                currentVehicleSlotFacilityVehicleHealOuput = vsfDef.VehicleHealAmount;

                TFTVLogger.Info($"[FacilityAdjustments_Apply] vsfDef: {vsfDef.name}, GUID: {vsfDef.Guid}, AircraftHealAmount: {vsfDef.AircraftHealAmount}, VehicleHealAmount: {vsfDef.VehicleHealAmount}");
            }

            List<ResourceGeneratorFacilityComponentDef> resourceGeneratorFacilityComponentDefs = defRepository.DefRepositoryDef.AllDefs.OfType<ResourceGeneratorFacilityComponentDef>().ToList();
            foreach (ResourceGeneratorFacilityComponentDef rgfDef in resourceGeneratorFacilityComponentDefs)
            {
                if (rgfDef.name.Contains("FabricationPlant"))
                {
                    ResourcePack resources = rgfDef.BaseResourcesOutput;
                    ResourceUnit supplies = new ResourceUnit(ResourceType.Production, 4); //AssortedAdjustments.Settings.FabricationPlantGenerateProductionAmount
                    resources.Set(supplies);

                    // When added here they are also affected by general research buffs. This is NOT intended.
                    /*
                    if(AssortedAdjustments.Settings.FabricationPlantGenerateMaterialsAmount > 0f)
                    {
                        float value = AssortedAdjustments.Settings.FabricationPlantGenerateMaterialsAmount / AssortedAdjustments.Settings.GenerateResourcesBaseDivisor;
                        ResourceUnit materials = new ResourceUnit(ResourceType.Materials, value);
                        resources.AddUnique(materials);
                    }
                    */
                }
                else if (rgfDef.name.Contains("ResearchLab"))
                {
                    ResourcePack resources = rgfDef.BaseResourcesOutput;
                    ResourceUnit supplies = new ResourceUnit(ResourceType.Research, 3); //AssortedAdjustments.Settings.ResearchLabGenerateResearchAmount
                    resources.Set(supplies);

                    // When added here they are also affected by general research buffs (Synedrion research). This is NOT intended.
                    /*
                    if (AssortedAdjustments.Settings.ResearchLabGenerateTechAmount > 0f)
                    {
                        float value = AssortedAdjustments.Settings.ResearchLabGenerateTechAmount / AssortedAdjustments.Settings.GenerateResourcesBaseDivisor;
                        ResourceUnit tech = new ResourceUnit(ResourceType.Tech, value);
                        resources.AddUnique(tech);
                    }
                    */
                }
                else if (rgfDef.name.Contains("FoodProduction"))
                {
                    ResourcePack resources = rgfDef.BaseResourcesOutput;
                    ResourceUnit supplies = new ResourceUnit(ResourceType.Supplies, 0.5f);
                    resources.Set(supplies);

                    currentFoodProductionFacilitySuppliesOutput = 0.5f;// AssortedAdjustments.Settings.FoodProductionGenerateSuppliesAmount;
                }
                else if (rgfDef.name.Contains("BionicsLab"))
                {
                    ResourcePack resources = rgfDef.BaseResourcesOutput;
                    ResourceUnit supplies = new ResourceUnit(ResourceType.Research, 2); //AssortedAdjustments.Settings.BionicsLabGenerateResearchAmount
                    resources.Set(supplies);
                }
                else if (rgfDef.name.Contains("MutationLab"))
                {
                    ResourcePack resources = rgfDef.BaseResourcesOutput;
                    ResourceUnit supplies = new ResourceUnit(ResourceType.Mutagen, 0.25f); //AssortedAdjustments.Settings.MutationLabGenerateMutagenAmount
                    resources.Set(supplies);
                }

                TFTVLogger.Info($"[FacilityAdjustments_Apply] rgfDef: {rgfDef.name}, GUID: {rgfDef.Guid}, BaseResourcesOutput: {rgfDef.BaseResourcesOutput.ToString()}");
            }

            }

            //HarmonyHelpers.Patch(harmony, typeof(ResourceGeneratorFacilityComponent), "UpdateOutput", typeof(FacilityAdjustments), null, "Postfix_ResourceGeneratorFacilityComponent_UpdateOutput");
            //HarmonyHelpers.Patch(harmony, typeof(HealFacilityComponent), "UpdateOutput", typeof(FacilityAdjustments), null, "Postfix_HealFacilityComponent_UpdateOutput");
            //HarmonyHelpers.Patch(harmony, typeof(ExperienceFacilityComponent), "UpdateOutput", typeof(FacilityAdjustments), null, "Postfix_ExperienceFacilityComponent_UpdateOutput");
            //HarmonyHelpers.Patch(harmony, typeof(VehicleSlotFacilityComponent), "UpdateOutput", typeof(FacilityAdjustments), null, "Postfix_VehicleSlotFacilityComponent_UpdateOutput");
            //if(AssortedAdjustments.Settings.TrainingFacilitiesGenerateSkillpoints && AssortedAdjustments.Settings.TrainingFacilityBaseSkillPointsAmount > 0)
            {
                //HarmonyHelpers.Patch(harmony, typeof(GeoLevelController), "DailyUpdate", typeof(FacilityAdjustments), null, "Postfix_GeoLevelController_DailyUpdate");
            }

            // UI
            //HarmonyHelpers.Patch(harmony, typeof(UIFacilityTooltip), "Show", typeof(FacilityAdjustments), null, "Postfix_UIFacilityTooltip_Show");
            //HarmonyHelpers.Patch(harmony, typeof(UIFacilityInfoPopup), "Show", typeof(FacilityAdjustments), null, "Postfix_UIFacilityInfoPopup_Show");
        }



        // Patches
        [HarmonyPatch(typeof(ResourceGeneratorFacilityComponent), "UpdateOutput")]
        public static class ResourceGeneratorFacilityComponent_UpdateOutput_patch
        {
            public static void Postfix(ResourceGeneratorFacilityComponent __instance)
            {
                if (TFTVMain.Main.Config.ApplyCalvitixChanges)
            {
                try
                {
                    if (__instance.Facility.PxBase.Site.Owner is GeoPhoenixFaction)
                    {
                        string owningFaction = __instance.Facility.PxBase.Site.Owner.Name.Localize();
                        string facilityName = __instance.Facility.ViewElementDef.DisplayName1.Localize();
                        string facilityId = __instance.Facility.FacilityId.ToString();
                        //TFTVLogger.Info($"[ResourceGeneratorFacilityComponent_UpdateOutput_POSTFIX] owningFaction: {owningFaction}, facilityName: {facilityName}, facilityId: {facilityId}, ResourceOutput: {__instance.ResourceOutput}");

                        if (__instance.Def.name.Contains("FoodProduction"))
                        {
                            currentFoodProductionFacilitySuppliesOutput = __instance.ResourceOutput.Values.Where(u => u.Type == ResourceType.Supplies).First().Value;

                            /*
                            foreach (ResourceUnit resourceUnit in __instance.ResourceOutput.Values)
                            {
                                if (resourceUnit.Type == ResourceType.Supplies)
                                {
                                    currentFoodProductionFacilitySuppliesOutput = resourceUnit.Value;
                                }
                            }
                            */
                        }
                    }

                    // All factions
                    if (__instance.Def.name.Contains("FabricationPlant")) // && AssortedAdjustments.Settings.FabricationPlantGenerateMaterialsAmount > 0)
                    {
                        float value = 2f / 24f; // AssortedAdjustments.Settings.GenerateResourcesBaseDivisor;
                        ResourceUnit materials = new ResourceUnit(ResourceType.Materials, value);
                        __instance.ResourceOutput.AddUnique(materials);
                    }
                    else if (__instance.Def.name.Contains("ResearchLab"))// && AssortedAdjustments.Settings.ResearchLabGenerateTechAmount > 0)
                    {
                        float value = 1f / 24f;
                        ResourceUnit tech = new ResourceUnit(ResourceType.Tech, value);
                        __instance.ResourceOutput.AddUnique(tech);
                    }
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }
        }
        }

        [HarmonyPatch(typeof(HealFacilityComponent), "UpdateOutput")]
        public static class HealFacilityComponent_UpdateOutput_patch
        {
            public static void Postfix(HealFacilityComponent __instance)
            {
                try
                {
                    if (__instance.Facility.PxBase.Site.Owner is GeoPhoenixFaction)
                    {
                        string owningFaction = __instance.Facility.PxBase.Site.Owner.Name.Localize();
                        string facilityName = __instance.Facility.ViewElementDef.DisplayName1.Localize();
                        string facilityId = __instance.Facility.FacilityId.ToString();
                        //TFTVLogger.Info($"[ResourceGeneratorFacilityComponent_UpdateOutput_POSTFIX] owningFaction: {owningFaction}, facilityName: {facilityName}, facilityId: {facilityId}, HealOutput: {__instance.HealOutput}, StaminaHealOutput: {__instance.StaminaHealOutput}");

                        if (__instance.Def.name.Contains("MedicalBay"))
                        {
                            currentHealFacilityHealOutput = __instance.HealOutput;
                        }
                        else if (__instance.Def.name.Contains("LivingQuarters"))
                        {
                            currentHealFacilityStaminaHealOutput = __instance.StaminaHealOutput;
                        }
                        else if (__instance.Def.name.Contains("MutationLab"))
                        {
                            currentHealFacilityMutogHealOutput = __instance.HealOutput;
                        }
                    }
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(ExperienceFacilityComponent), "UpdateOutput")]
        public static class ExperienceFacilityComponent_UpdateOutput_patch
        {
            public static void Postfix(ExperienceFacilityComponent __instance)
            {
                try
                {
                    if (__instance.Facility.PxBase.Site.Owner is GeoPhoenixFaction)
                    {
                        string owningFaction = __instance.Facility.PxBase.Site.Owner.Name.Localize();
                        string facilityName = __instance.Facility.ViewElementDef.DisplayName1.Localize();
                        string facilityId = __instance.Facility.FacilityId.ToString();
                        //TFTVLogger.Info($"[ResourceGeneratorFacilityComponent_UpdateOutput_POSTFIX] owningFaction: {owningFaction}, facilityName: {facilityName}, facilityId: {facilityId}, HealOutput: {__instance.HealOutput}, StaminaHealOutput: {__instance.StaminaHealOutput}");

                        if (__instance.Def.name.Contains("TrainingFacility"))
                        {
                            currentExperienceFacilityExperienceOutput = __instance.ExperienceOutput;
                        }
                    }
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(VehicleSlotFacilityComponent), "UpdateOutput")]
        public static class VehicleSlotFacilityComponent_UpdateOutput_patch
        {
            public static void Postfix(VehicleSlotFacilityComponent __instance)
            {
                try
                {
                    if (__instance.Facility.PxBase.Site.Owner is GeoPhoenixFaction)
                    {
                        string owningFaction = __instance.Facility.PxBase.Site.Owner.Name.Localize();
                        string facilityName = __instance.Facility.ViewElementDef.DisplayName1.Localize();
                        string facilityId = __instance.Facility.FacilityId.ToString();
                        //TFTVLogger.Info($"[ResourceGeneratorFacilityComponent_UpdateOutput_POSTFIX] owningFaction: {owningFaction}, facilityName: {facilityName}, facilityId: {facilityId}, AircraftHealOuput: {__instance.AircraftHealOuput}, VehicletHealOuput: {__instance.VehicletHealOuput}");

                        currentVehicleSlotFacilityAircraftHealOuput = __instance.AircraftHealOuput;
                        currentVehicleSlotFacilityVehicleHealOuput = __instance.VehicletHealOuput;
                    }
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }
        }


        [HarmonyPatch(typeof(GeoLevelController), "DailyUpdate")]
        public static class GeoLevelController_DailyUpdate_patch
        {
            public static void Postfix(GeoLevelController __instance)
            {
                try
                {
                    foreach (GeoFaction faction in __instance.Factions)
                    {
                        if (faction.Def.UpdateFaction && faction is GeoPhoenixFaction geoPhoenixFaction)
                        {
                            geoPhoenixFaction.UpdateBasesDaily();
                        }
                    }
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }
        }


        [HarmonyPatch(typeof(UIFacilityTooltip), "Show")]
        public static class UIFacilityTooltip_Show_patch
        {
            public static void Postfix(UIFacilityTooltip __instance, PhoenixFacilityDef facility, GeoPhoenixBase currentBase)
            {
                try
                {
                    if (currentBase == null)
                    {
                        return;
                    }

                    if (facility.name.Contains("MedicalBay"))
                    {
                        //float baseHealOutput = facility.GetComponent<HealFacilityComponentDef>().BaseHeal;
                        //float currentBonusValue = currentHealFacilityHealOutput > baseHealOutput ? (currentHealFacilityHealOutput - baseHealOutput) : 0;
                        //string currentBonus = currentBonusValue > 0 ? $"({baseHealOutput} + {currentBonusValue})" : "";

                        __instance.Description.text = $"Tous les soldats présents sur la base (même s'ils sont affectés à un avion) récupèrent {currentHealFacilityHealOutput} Points de santé par heure pour chaque station médicale de la base.";
                    }
                    else if (facility.name.Contains("LivingQuarters"))
                    {
                        __instance.Description.text = $"Tous les soldats présents sur la base (même s'ils sont affectés à un avion) récupèrent {currentHealFacilityStaminaHealOutput} Points d'endurance par heure pour chaque quartier d'habitation de la base.";
                    }
                    else if (facility.name.Contains("TrainingFacility"))
                    {
                        string s1 = $"Tous les soldats présents sur la base (même s'ils sont affectés à un avion) gagnent {currentExperienceFacilityExperienceOutput} Points d'expérience par heure pour chaque centre d'entrainement de la base.";
                        string s2 = "";
                        //if(AssortedAdjustments.Settings.TrainingFacilitiesGenerateSkillpoints && AssortedAdjustments.Settings.TrainingFacilityBaseSkillPointsAmount > 0)
                        {
                            int TrainingFacilityBaseSkillPointsAmount = 1;
                            string pluralizeSP = TrainingFacilityBaseSkillPointsAmount > 1 ? "points" : "point";
                            s2 = $"Ajoute {TrainingFacilityBaseSkillPointsAmount} {pluralizeSP} de compétences global chaque jour.";
                        }
                        __instance.Description.text = $"{s1}\n{s2}";
                    }
                    else if (facility.name.Contains("MutationLab"))
                    {
                        string org = __instance.Description.text;
                        string add = $"Tous les mutogs de la base (même s'ils sont affectés à un aéronef) récupérent {currentHealFacilityMutogHealOutput} Points de santé par heure pour chaque laboratoire de mutation de la base.";
                        __instance.Description.text = $"{org}\n{add}";
                    }
                    else if (facility.name.Contains("VehicleBay"))
                    {
                        __instance.Description.text = $"Les véhicules et les avions de la base récupèrent {currentVehicleSlotFacilityVehicleHealOuput} Points de santé par heure. Permet l'entretien de 2 véhicules terrestres et de 2 avions.";
                    }
                    else if (facility.name.Contains("FabricationPlant") && TFTVMain.Main.Config.ApplyCalvitixChanges)//&& AssortedAdjustments.Settings.FabricationPlantGenerateMaterialsAmount > 0)
                    {
                        float FabricationPlantGenerateMaterialsAmount = 2f;
                        string org = __instance.Description.text;
                        string add = $"Chaque usine génère {FabricationPlantGenerateMaterialsAmount} matériaux par heure.";
                        __instance.Description.text = $"{org}\n{add}";
                    }
                    else if (facility.name.Contains("ResearchLab") && TFTVMain.Main.Config.ApplyCalvitixChanges)//&& AssortedAdjustments.Settings.ResearchLabGenerateTechAmount > 0)
                    {
                        float ResearchLabGenerateTechAmount = 1f;
                        string org = __instance.Description.text;
                        string add = $"Chaque laboratoire génère {ResearchLabGenerateTechAmount} tech par heure.";
                        __instance.Description.text = $"{org}\n{add}";
                    }
                    else if (facility.name.Contains("FoodProduction"))
                    {
                        int foodProductionUnits = (int)Math.Round(currentFoodProductionFacilitySuppliesOutput * 24);
                        __instance.Description.text = $"Une installation de production alimentaire qui génère suffisamment de nourriture pour {foodProductionUnits} soldats chaque jour.";
                    }
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(UIFacilityInfoPopup), "Show")]
        public static class UIFacilityInfoPopup_Show_patch
        {

            public static void Postfix(UIFacilityInfoPopup __instance, GeoPhoenixFacility facility)
            {
                try
                {
                    if (facility.Def.name.Contains("MedicalBay"))
                    {
                        __instance.Description.text = $"Tous les soldats présents sur la base (même s'ils sont affectés à un avion) récupèrent {currentHealFacilityHealOutput} Points de santé par heure pour chaque station médicale de la base.";
                    }
                    else if (facility.Def.name.Contains("LivingQuarters"))
                    {
                        __instance.Description.text = $"Tous les soldats présents sur la base (même s'ils sont affectés à un avion) récupèrent {currentHealFacilityStaminaHealOutput} Points d'endurance par heure pour chaque quartier d'habitation de la base.";
                    }
                    else if (facility.Def.name.Contains("TrainingFacility"))
                    {
                        string s1 = $"Tous les soldats présents sur la base (même s'ils sont affectés à un avion) gagnent {currentExperienceFacilityExperienceOutput} Points d'expérience par heure pour chaque centre d'entraînement de la base.";
                        string s2 = "";
                        //if (AssortedAdjustments.Settings.TrainingFacilitiesGenerateSkillpoints && AssortedAdjustments.Settings.TrainingFacilityBaseSkillPointsAmount > 0)
                        {
                            int TrainingFacilityBaseSkillPointsAmount = 1;
                            string pluralizeSP = TrainingFacilityBaseSkillPointsAmount > 1 ? "points" : "point";
                            s2 = $"Ajoute {TrainingFacilityBaseSkillPointsAmount} {pluralizeSP} de compétences global chaque jour.";
                        }
                        __instance.Description.text = $"{s1}\n{s2}";
                    }
                    else if (facility.Def.name.Contains("MutationLab"))
                    {
                        string org = __instance.Description.text;
                        string add = $"Tous les mutogs de la base (même s'ils sont affectés à un aéronef) récupéreront {currentHealFacilityMutogHealOutput} Points de vie supplémentaires par heure pour chaque laboratoire de mutation de la base.";
                        __instance.Description.text = $"{org}\n{add}";
                    }
                    else if (facility.Def.name.Contains("VehicleBay"))
                    {
                        __instance.Description.text = $"Les véhicules et les avions de la base récupèrent {currentVehicleSlotFacilityVehicleHealOuput} Points de santé par heure. Permet l'entretien de 2 véhicules terrestres et de 2 aéronefs.";
                    }
                    else if (facility.Def.name.Contains("FabricationPlant")) // && AssortedAdjustments.Settings.FabricationPlantGenerateMaterialsAmount > 0)
                    {
                        int FabricationPlantGenerateMaterialsAmount = 2;
                        string org = __instance.Description.text;
                        string add = $"Chaque usine génère {FabricationPlantGenerateMaterialsAmount} matériaux par heure.";
                        __instance.Description.text = $"{org}\n{add}";
                    }
                    else if (facility.Def.name.Contains("ResearchLab")) // && AssortedAdjustments.Settings.ResearchLabGenerateTechAmount > 0)
                    {
                        int ResearchLabGenerateTechAmount = 1;
                        string org = __instance.Description.text;
                        string add = $"Chaque laboratoire génère {ResearchLabGenerateTechAmount} tech par heure.";
                        __instance.Description.text = $"{org}\n{add}";
                    }
                    else if (facility.Def.name.Contains("FoodProduction"))
                    {
                        int foodProductionUnits = (int)Math.Round(currentFoodProductionFacilitySuppliesOutput * 24);
                        __instance.Description.text = $"Une installation de production alimentaire qui génère suffisamment de nourriture pour {foodProductionUnits} soldats chaque jour.";
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
