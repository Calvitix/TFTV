﻿using Base;
using Base.Defs;
using Base.Entities.Abilities;
using Base.Entities.Effects;
using Base.Entities.Effects.ApplicationConditions;
using Base.Entities.Statuses;
using Base.UI;
using Base.Utils;
using HarmonyLib;
using JetBrains.Annotations;
using PhoenixPoint.Common.ContextHelp;
using PhoenixPoint.Common.ContextHelp.HintConditions;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.Characters;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.Entities.GameTagsTypes;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Common.Levels.ActorDeployment;
using PhoenixPoint.Common.Levels.Missions;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Entities.Abilities;
using PhoenixPoint.Geoscape.Entities.Interception;
using PhoenixPoint.Geoscape.Entities.Interception.Equipments;
using PhoenixPoint.Geoscape.Entities.Missions.Outcomes;
using PhoenixPoint.Geoscape.Entities.PhoenixBases.FacilityComponents;
using PhoenixPoint.Geoscape.Entities.Research;
using PhoenixPoint.Geoscape.Entities.Research.Requirement;
using PhoenixPoint.Geoscape.Entities.Research.Reward;
using PhoenixPoint.Geoscape.Events.Eventus;
using PhoenixPoint.Geoscape.Events.Eventus.Filters;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Geoscape.Levels.ContextHelp.HintConditions;
using PhoenixPoint.Tactical.AI.Considerations;
using PhoenixPoint.Tactical.ContextHelp.HintConditions;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Effects.ApplicationConditions;
using PhoenixPoint.Tactical.Entities.Effects.DamageTypes;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Statuses;
using PhoenixPoint.Tactical.Entities.Weapons;
using PhoenixPoint.Tactical.Levels.FactionObjectives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace TFTV
{
    internal class TFTVDefsInjectedOnlyOnce
    {
        //  private static readonly DefRepository Repo = TFTVMain.Repo;
        private static readonly DefCache DefCache = TFTVMain.Main.DefCache;
        private static readonly DefRepository Repo = TFTVMain.Repo;
        public static Sprite UmbraIcon = Helper.CreateSpriteFromImageFile("Void-03P.png");
        public static void InjectDefsInjectedOnlyOnce()
        {
            AugmentationEventsDefs();
            ChangesAmbushMissions();
            CreateHints();
            CreateIntro();
            Create_VoidOmen_Events();
            ChangeInfestationDefs();
            ChangesToMedbay();
            ChangeUmbra();
            InjectAlistairAhsbyLines();
            InjectOlenaKimLines();
            MistOnAllMissions();
            ModifyAirCombatDefs();
            ModifyDefsForPassengerModules();
            ModifyMissionDefsToReplaceNeutralWithBandit();
            ModifyPandoranProgress();
            RemoveCorruptionDamageBuff();
            TFTVChangesToDLC1andDLC2Events.ChangesToDLC1andDLC2Defs();
            TFTVChangesToDLC3Events.ChangesToDLC3Defs();
            TFTVChangesToDLC4Events.ChangesToDLC4Defs();
            TFTVChangesToDLC5Events.ChangesToDLC5Defs();
            ChangesToAcherons();
            RemoveCensusResearch();
            AllowMedkitsToTargetMutoids();
            ChangesToLOTAEarlyLoad();
            CreateSubject24();
           // TFTVExperimental.MakeCopyOfAlienAttackOnPhoenixBase();
            CreateVoidOmenRemindersInTactical();
        }

        public static void ChangesToLOTAEarlyLoad() 
        {
            try
            {

                CreateAbilitiesForAncients();
                CreateAncientStatusEffects();
                CreateParalysisDamageImmunity();
                CreateLOTAHints();
                CreateAncientAutomataResearch();
                CreateExoticMaterialsResearch();
                CreateLivingCrystalResearch();
                CreateProteanMutaneResearch();
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }


        public static void ChangesToLOTA2()
        {
            try
            {
                ChangeAncientsBodyParts();    
                ChangeAncientsWeapons();   
                ChangeSchemataMissionRequirement();
                ChangeAncientSiteExploration();
                ChangeImpossibleWeapons();
                RemovePandoranVirusResearchRequirement();
                CreateEventsForLOTA();      
                ChangeAncientDefenseMission();
                ChangeAncientSiteMissions();
            }

            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }


        public static void CreateVoidOmenRemindersInTactical()
        {
            try
            {
                CreateVoidOmenObjective("818B37C5-AC05-4245-A629-D84761838DE6", "VOID_OMEN_TITLE_3", 0);
                CreateVoidOmenObjective("F0CCE047-352C-4AE4-8D12-6856FA57A5C7", "VOID_OMEN_TITLE_5", 0);
                CreateVoidOmenObjective("BDBBD195-D07C-43CF-AB0F-50C7CEA8B044", "VOID_OMEN_TITLE_7", 0);
                CreateVoidOmenObjective("EC9011E4-2C01-485B-8E89-7D0A20996899", "VOID_OMEN_TITLE_10", 0);
                CreateVoidOmenObjective("3CBE9291-2241-428B-B6DD-776EFF316D4F", "VOID_OMEN_TITLE_15", 0);
                CreateVoidOmenObjective("D25FC8F1-DB31-4BA2-9B9F-3787B9D3A664", "VOID_OMEN_TITLE_16", 0);
                CreateVoidOmenObjective("BA859656-03E9-4BCD-AAAC-2A0B09506FEC", "VOID_OMEN_TITLE_19", 0);

                //3, 5, 7, 10, 15, 16, 19

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }

        public static void CreateVoidOmenObjective(string guid, string description_key, int experienceReward)
        {
            try
            {

                string objectiveName = description_key;
                KeepSoldiersAliveFactionObjectiveDef keepSoldiersAliveObjectiveSource = DefCache.GetDef<KeepSoldiersAliveFactionObjectiveDef>("KeepSoldiersAliveFactionObjectiveDef");
                KeepSoldiersAliveFactionObjectiveDef voidOmenObjective = Helper.CreateDefFromClone(keepSoldiersAliveObjectiveSource, guid, objectiveName);
                voidOmenObjective.IsVictoryObjective = false;
                voidOmenObjective.IsDefeatObjective = false;
                voidOmenObjective.MissionObjectiveData.ExperienceReward = experienceReward;
                voidOmenObjective.MissionObjectiveData.Description.LocalizationKey = description_key;
                voidOmenObjective.MissionObjectiveData.Summary.LocalizationKey = description_key;
                voidOmenObjective.IsUiSummaryHidden = true;
                //   TFTVLogger.Always("FactionObjective " + DefCache.GetDef<FactionObjectiveDef>(objectiveName).name + " created");

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }


        public static void CreateParalysisDamageImmunity()
        {
            try 
            {
                DamageMultiplierAbilityDef damageMultiplierAbilityDefSource = DefCache.GetDef<DamageMultiplierAbilityDef>("EMPResistant_DamageMultiplierAbilityDef");
                DamageMultiplierAbilityDef ParalysisNotShcokImmunity = Helper.CreateDefFromClone(damageMultiplierAbilityDefSource, "38B0CBAB-EE97-4481-876B-71427E4C11CE", "ParalysisNotShockImmunity_DamageMultiplierAbilityDef");
                ParalysisNotShcokImmunity.DamageTypeDef = DefCache.GetDef<DamageOverTimeDamageTypeEffectDef>("Paralysis_DamageOverTimeDamageTypeEffectDef");
                ParalysisNotShcokImmunity.Multiplier = 0.0f;
                ParalysisNotShcokImmunity.ViewElementDef = Helper.CreateDefFromClone(damageMultiplierAbilityDefSource.ViewElementDef, "FF08454A-CFD1-4BC9-AEF5-003E2ED646EC", "ParalysisNotShockImmunity_ViewElementDef");
                ParalysisNotShcokImmunity.ViewElementDef.DisplayName1.LocalizationKey = "IMMUNITY_TO_PARALYSIS_NAME";
                ParalysisNotShcokImmunity.ViewElementDef.Description.LocalizationKey = "IMMUNITY_TO_PARALYSIS_DESCRIPTION";
                ParalysisNotShcokImmunity.ViewElementDef.LargeIcon = Helper.CreateSpriteFromImageFile("ParalysisImmunity.png");
                ParalysisNotShcokImmunity.ViewElementDef.SmallIcon = Helper.CreateSpriteFromImageFile("ParalysisImmunity.png");

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }
        
        public static void ChangeAncientSiteMissions()
        {
            try 
            {
                CustomMissionTypeDef crystalRefinery = DefCache.GetDef<CustomMissionTypeDef>("CrystalsRefineryAttack_Ancient_CustomMissionTypeDef");
                CustomMissionTypeDef orichalcumRefinery = DefCache.GetDef<CustomMissionTypeDef>("OrichalcumRefineryAttack_Ancient_CustomMissionTypeDef");
                CustomMissionTypeDef proteanRefinery = DefCache.GetDef<CustomMissionTypeDef>("ProteanRefineryAttack_Ancient_CustomMissionTypeDef");

                CustomMissionTypeDef crystalHarvest = DefCache.GetDef<CustomMissionTypeDef>("CrystalsHarvestAttack_Ancient_CustomMissionTypeDef");
                CustomMissionTypeDef orichalcumHarvest = DefCache.GetDef<CustomMissionTypeDef>("OrichalcumHarvestAttack_Ancient_CustomMissionTypeDef");
                CustomMissionTypeDef proteanHarvest = DefCache.GetDef<CustomMissionTypeDef>("ProteanHarvestAttack_Ancient_CustomMissionTypeDef");

                List<CustomMissionTypeDef> customMissionTypeDefs = new List<CustomMissionTypeDef>() {crystalHarvest, crystalRefinery, orichalcumRefinery, proteanRefinery, orichalcumHarvest, proteanHarvest };

                ClassTagDef hoplite = DefCache.GetDef<ClassTagDef>("HumanoidGuardian_ClassTagDef");
                GameDifficultyLevelDef hero = DefCache.GetDef<GameDifficultyLevelDef>("Hard_GameDifficultyLevelDef");
                GameDifficultyLevelDef veteran = DefCache.GetDef<GameDifficultyLevelDef>("Standard_GameDifficultyLevelDef");
                GameDifficultyLevelDef legend = DefCache.GetDef<GameDifficultyLevelDef>("VeryHard_GameDifficultyLevelDef");

                foreach (CustomMissionTypeDef customMissionTypeDef in customMissionTypeDefs)
                {

                    TacMissionTypeParticipantData.UniqueChatarcterBind shielderVeteran = new TacMissionTypeParticipantData.UniqueChatarcterBind()
                    {
                        Amount = { Min = 1, Max = 1 },
                        Character = DefCache.GetDef<TacCharacterDef>("HumanoidGuardian_Shielder_TacCharacterDef"),
                        Difficulty = veteran,
                        
                    };
                    TacMissionTypeParticipantData.UniqueChatarcterBind drillerVeteran = new TacMissionTypeParticipantData.UniqueChatarcterBind()
                    {
                        Amount = { Min = 1, Max = 1 },
                        Character = DefCache.GetDef<TacCharacterDef>("HumanoidGuardian_Driller_TacCharacterDef"),
                        Difficulty = veteran,

                    };
                    TacMissionTypeParticipantData.UniqueChatarcterBind shielderHero = new TacMissionTypeParticipantData.UniqueChatarcterBind()
                    {
                        Amount = { Min = 1, Max = 1 },
                        Character = DefCache.GetDef<TacCharacterDef>("HumanoidGuardian_Shielder_TacCharacterDef"),
                        Difficulty = hero,

                    };
                    TacMissionTypeParticipantData.UniqueChatarcterBind drillerHero = new TacMissionTypeParticipantData.UniqueChatarcterBind()
                    {
                        Amount = { Min = 1, Max = 1 },
                        Character = DefCache.GetDef<TacCharacterDef>("HumanoidGuardian_Driller_TacCharacterDef"),
                        Difficulty = hero,

                    };
                    TacMissionTypeParticipantData.UniqueChatarcterBind shielderLegend = new TacMissionTypeParticipantData.UniqueChatarcterBind()
                    {
                        Amount = { Min = 1, Max = 1 },
                        Character = DefCache.GetDef<TacCharacterDef>("HumanoidGuardian_Shielder_TacCharacterDef"),
                        Difficulty = legend,

                    };
                    TacMissionTypeParticipantData.UniqueChatarcterBind drillerLegend = new TacMissionTypeParticipantData.UniqueChatarcterBind()
                    {
                        Amount = { Min = 1, Max = 1 },
                        Character = DefCache.GetDef<TacCharacterDef>("HumanoidGuardian_Driller_TacCharacterDef"),
                        Difficulty = legend,

                    };

                    List<TacMissionTypeParticipantData.UniqueChatarcterBind> uniqueUnits = customMissionTypeDef.ParticipantsData[0].UniqueUnits.ToList();
                    uniqueUnits.Add(drillerVeteran);
                    uniqueUnits.Add(shielderVeteran);
                    uniqueUnits.Add(drillerHero);
                    uniqueUnits.Add(shielderHero);
                    uniqueUnits.Add(drillerLegend);
                    uniqueUnits.Add(shielderLegend);
                    customMissionTypeDef.ParticipantsData[0].UniqueUnits = uniqueUnits.ToArray();
               
                }
            }
            
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }

        public static void ChangeAncientDefenseMission()
        {
            try 
            {
                CustomMissionTypeDef crystalHarvest= DefCache.GetDef<CustomMissionTypeDef>("CrystalsHarvestDefence_Alien_CustomMissionTypeDef");
                CustomMissionTypeDef crystalRefinery = DefCache.GetDef<CustomMissionTypeDef>("CrystalsRefineryDefence_Alien_CustomMissionTypeDef");
                CustomMissionTypeDef orichalcumHarvest = DefCache.GetDef<CustomMissionTypeDef>("OrichalcumHarvestDefence_Alien_CustomMissionTypeDef");
                CustomMissionTypeDef orichalcumRefinery = DefCache.GetDef<CustomMissionTypeDef>("OrichalcumRefineryDefence_Alien_CustomMissionTypeDef");
                CustomMissionTypeDef proteanHarvest = DefCache.GetDef<CustomMissionTypeDef>("ProteanHarvestDefence_Alien_CustomMissionTypeDef");
                CustomMissionTypeDef proteanRefinery = DefCache.GetDef<CustomMissionTypeDef>("ProteanRefineryDefence_Alien_CustomMissionTypeDef");

                List<CustomMissionTypeDef> allMissions = new List<CustomMissionTypeDef> 
                { crystalHarvest, crystalRefinery, orichalcumHarvest, orichalcumRefinery, proteanHarvest, proteanRefinery };

                ProtectActorFactionObjectiveDef protectActorObjectiveSource = DefCache.GetDef<ProtectActorFactionObjectiveDef>("RLE0_CustomMissionObjective");
                ProtectActorFactionObjectiveDef protectCyclopsObjective =
                    Helper.CreateDefFromClone(protectActorObjectiveSource, "9E3ABDDC-C49D-4433-8338-483E6192AF8E", "ProtectCyclopsObjectiveDef");
                ClassTagDef cyclopsTag = DefCache.GetDef<ClassTagDef>("MediumGuardian_ClassTagDef");
                protectCyclopsObjective.Participant = TacMissionParticipant.Player;
                protectCyclopsObjective.ProtectTargetGameTag = cyclopsTag;
                protectCyclopsObjective.IsUiHidden = false;
                protectCyclopsObjective.IsUiSummaryHidden = false;
                protectCyclopsObjective.MissionObjectiveData.Description.LocalizationKey = "PROTECT_THE_CYCLOPS";
                protectCyclopsObjective.MissionObjectiveData.Summary.LocalizationKey = "PROTECT_THE_CYCLOPS";

                foreach (CustomMissionTypeDef customMissionType in allMissions) 
                { 
                List <FactionObjectiveDef> objectives = customMissionType.CustomObjectives.ToList();
                    objectives.Add(protectCyclopsObjective);

                    customMissionType.CustomObjectives = objectives.ToArray();     
                                
                }


            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }


        }



        public static void CreateLivingCrystalResearch()
        {
            try 
            {
                string id = "PX_LivingCrystalResearchDef";
                int cost = 400;
                string keyName = "LIVING_CRYSTAL_RESEARCH_TITLE";
                string keyReveal = "";
                string keyUnlock = "LIVING_CRYSTAL_RESEARCH_REVEAL";
                string keyComplete = "LIVING_CRYSTAL_RESEARCH_COMPLETE";
                string keyBenefits = "LIVING_CRYSTAL_RESEARCH_BENEFITS";

                UnlockFunctionalityResearchRewardDef sourceUnlockFunctionality = DefCache.GetDef<UnlockFunctionalityResearchRewardDef>("PX_AtmosphericAnalysis_ResearchDef_UnlockFunctionalityResearchRewardDef_0");
                UnlockFunctionalityResearchRewardDef livingCrystalFunctionalityResearchReward = Helper.CreateDefFromClone(sourceUnlockFunctionality, "6CEA3DBE-DC8C-4454-96A9-4A4D8FCB6927", "LivingCrystalFunctionalityResearchReward");

                GameTagDef livingCrystalRequirement = DefCache.GetDef<GameTagDef>("RefineLivingCrystals_FactionFunctionalityTagDef");

                livingCrystalFunctionalityResearchReward.Tag = livingCrystalRequirement;

                ResourcesResearchRequirementDef sourceRequirement = DefCache.GetDef<ResourcesResearchRequirementDef>("ALN_CrabmanUmbra_ResearchDef_ResourcesResearchRequirementDef_0");
                ResourcesResearchRequirementDef requirementDef = Helper.CreateDefFromClone(sourceRequirement, "C61983EE-F219-467A-B478-3451273ECB84", "LivingCrystalResearchRequirementDef");
                requirementDef.Resources = new ResourcePack(new ResourceUnit { Type = ResourceType.LivingCrystals, Value = 1 });

                ExistingResearchRequirementDef requiremen2tDef = TFTVCommonMethods.CreateNewExistingResearchResearchRequirementDef(id + "ResearchReq", "F0D428A7-9D51-4746-9C60-1EFADD5457B8", "ExoticMaterialsResearch");


               
                ResearchViewElementDef imageSource = DefCache.GetDef<ResearchViewElementDef>("PX_AntediluvianArchaeology_ViewElementDef");

                string gUID = "14E1635F-6663-41C8-B04E-A8C91890BC5B";
                string gUID2 = "0E7D8BB6-1139-4CE3-9402-D3F490838E55";

                ResearchDef research = TFTVCommonMethods.CreateNewPXResearch(id, cost, gUID, gUID2, keyName, keyReveal, keyUnlock, keyComplete, keyBenefits, imageSource);

                ReseachRequirementDefOpContainer[] revealRequirementContainer = new ReseachRequirementDefOpContainer[1];
                ResearchRequirementDef[] revealResearchRequirementDefs = new ResearchRequirementDef[2];
                revealResearchRequirementDefs[0] = requirementDef; //small box
                revealResearchRequirementDefs[1] = requiremen2tDef; 
                revealRequirementContainer[0].Requirements = revealResearchRequirementDefs; //medium box
                research.RevealRequirements.Container = revealRequirementContainer;
                research.RevealRequirements.Operation = ResearchContainerOperation.ALL;
                research.Unlocks = new ResearchRewardDef[] { livingCrystalFunctionalityResearchReward };

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void CreateProteanMutaneResearch()
        {
            try
            {
                string id = "PX_ProteanMutaneResearchDef";
                int cost = 400;
                string keyName = "PROTEAN_MUTANE_RESEARCH_TITLE";
                string keyReveal = "";
                string keyUnlock = "PROTEAN_MUTANE_RESEARCH_REVEAL";
                string keyComplete = "PROTEAN_MUTANE_RESEARCH_COMPLETE";
                string keyBenefits = "PROTEAN_MUTANE_RESEARCH_BENEFITS";

                UnlockFunctionalityResearchRewardDef sourceUnlockFunctionality = DefCache.GetDef<UnlockFunctionalityResearchRewardDef>("PX_AtmosphericAnalysis_ResearchDef_UnlockFunctionalityResearchRewardDef_0");
                UnlockFunctionalityResearchRewardDef proteanMutaneFunctionalityResearchReward = Helper.CreateDefFromClone(sourceUnlockFunctionality, "E01EFEAF-70E3-4DDF-8644-AF12C1FA7AFD", "ProteanMutaneFunctionalityResearchReward");

                GameTagDef proteanMutaneRequirement = DefCache.GetDef<GameTagDef>("RefineProteanMutane_FactionFunctionalityTagDef");

                proteanMutaneFunctionalityResearchReward.Tag = proteanMutaneRequirement;

              

                ResourcesResearchRequirementDef sourceRequirement = DefCache.GetDef<ResourcesResearchRequirementDef>("ALN_CrabmanUmbra_ResearchDef_ResourcesResearchRequirementDef_0");
                ResourcesResearchRequirementDef requirementDef = Helper.CreateDefFromClone(sourceRequirement, "FA66ED06-A5F7-4A6F-B18F-F84BA4B97AB5", "ProteanMutaneResearchRequirementDef");

                ExistingResearchRequirementDef requiremen2tDef = TFTVCommonMethods.CreateNewExistingResearchResearchRequirementDef(id + "ResearchReq", "041C8F9B-0F7F-467F-9DD6-183C3B901B56", "ExoticMaterialsResearch");

               
                ResearchViewElementDef imageSource = DefCache.GetDef<ResearchViewElementDef>("PX_AntediluvianArchaeology_ViewElementDef");

                string gUID = "1BFAE176-45F4-4A8B-A1A7-8CE89CA2B224";
                string gUID2 = "BF1A9ABD-B6F7-48CD-BE20-217C338FF421";

                ResearchDef research = TFTVCommonMethods.CreateNewPXResearch(id, cost, gUID, gUID2, keyName, keyReveal, keyUnlock, keyComplete, keyBenefits, imageSource);

                ReseachRequirementDefOpContainer[] revealRequirementContainer = new ReseachRequirementDefOpContainer[1];
                ResearchRequirementDef[] revealResearchRequirementDefs = new ResearchRequirementDef[2];
                revealResearchRequirementDefs[0] = requirementDef; //small box
                revealResearchRequirementDefs[1] = requiremen2tDef;
                revealRequirementContainer[0].Requirements = revealResearchRequirementDefs; //medium box
                research.RevealRequirements.Container = revealRequirementContainer;
                research.RevealRequirements.Operation = ResearchContainerOperation.ALL;
                research.Unlocks = new ResearchRewardDef[] {proteanMutaneFunctionalityResearchReward};

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }


        public static void CreateEventsForLOTA()
        {
            try 
            {
                //muting warning events:
                DefCache.GetDef<GeoscapeEventDef>("PROG_LE1_WARN_GeoscapeEventDef").GeoscapeEventData.Mute = true;
                DefCache.GetDef<GeoscapeEventDef>("PROG_LE2_WARN_GeoscapeEventDef").GeoscapeEventData.Mute = true;
                DefCache.GetDef<GeoscapeEventDef>("PROG_LE3_WARN_GeoscapeEventDef").GeoscapeEventData.Mute = true;

                //Helena post-research Antediluvian Archaelogy event: 
                TFTVCommonMethods.CreateNewEvent("Helena_Echoes", "HELENA_ECHOES_TITLE", "HELENA_ECHOES_TEXT", null);

                //Olena Kim post-research Automata event:      
                TFTVCommonMethods.CreateNewEvent("Olena_Styx", "OLENA_STYX_TITLE", "OLENA_STYX_TEXT", null);

                //Helena post research Living Crystal event: 
                GeoscapeEventDef helenaOneiromancy = TFTVCommonMethods.CreateNewEvent("Helena_Oneiromancy", "HELENA_ONEIROMANCY_TITLE", "HELENA_ONEIROMANCY_TEXT", null);

                //Olena post-Helena event:    
                GeoscapeEventDef olenaOneiromancy=TFTVCommonMethods.CreateNewEvent("Olena_Oneiromancy", "OLENA_ONEIROMANCY_TITLE", "OLENA_ONEIROMANCY_TEXT", "OLENA_ONEIROMANCY_OUTCOME");
                olenaOneiromancy.GeoscapeEventData.Choices[0].Text.LocalizationKey = "OLENA_ONEIROMANCY_CHOICE";

                //Post research Exotic mnaterials Olena event:
                GeoscapeEventDef impossibleWeaponsEvent = DefCache.GetDef<GeoscapeEventDef>("PROG_LE1_GeoscapeEventDef");
                impossibleWeaponsEvent.GeoscapeEventData.Description[0].General.LocalizationKey = "OLENA_PUZZLE_TEXT";
                impossibleWeaponsEvent.GeoscapeEventData.Choices[0].Text.LocalizationKey = "OLENA_PUZZLE_CHOICE";
                impossibleWeaponsEvent.GeoscapeEventData.Choices[0].Outcome.OutcomeText.General.LocalizationKey = "OLENA_PUZZLE_OUTCOME";

                //Event on building the Cyclops
                TFTVCommonMethods.CreateNewEvent("Helena_Beast", "HELENA_BEAST_TITLE", "HELENA_BEAST_TEXT", null);
                
                //digitize my dreams: triggered on successful ancient site defense    
              GeoscapeEventDef cyclopsDreams=  TFTVCommonMethods.CreateNewEvent("Cyclops_Dreams", "CYCLOP_DREAMS_TITLE", "CYCLOP_DREAMS_TEXT", null);
                cyclopsDreams.GeoscapeEventData.Choices[0].Outcome.VariablesChange.Add(TFTVCommonMethods.GenerateVariableChange("SymesAlternativeCompleted", 1, true));
             
                //Alistair on manufacture of an ancient weapon:   
                TFTVCommonMethods.CreateNewEvent("Alistair_Progress", "ALISTAIR_PROGRESS_TITLE", "ALISTAIR_PROGRESS_TEXT", null);
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }


        public static void CreateLOTAHints()
        {
            try
            {
              

                ClassTagDef cyclopsTag = DefCache.GetDef<ClassTagDef>("MediumGuardian_ClassTagDef");
                ClassTagDef hopliteTag = DefCache.GetDef<ClassTagDef>("HumanoidGuardian_ClassTagDef");
                MissionTypeTagDef ancientMissionTag = DefCache.GetDef<MissionTypeTagDef>("MissionTypeAncientSiteAttack_MissionTagDef");

                DamageMultiplierStatusDef AddAutoRepairStatusAbility = DefCache.GetDef<DamageMultiplierStatusDef>("AutoRepair_AddAbilityStatusDef");
                DamageMultiplierStatusDef ancientsPowerUpStatus = DefCache.GetDef<DamageMultiplierStatusDef>("AncientsPoweredUp");

                string hintStory1 = "ANCIENTS_STORY1";
                string story1Title = "HINT_ANCIENTS_STORY1_TITLE";
                string story1Text = "HINT_ANCIENTS_STORY1_TEXT";
                string hintCyclops = "ANCIENTS_CYCLOPS";
                string cyclopsTitle = "HINT_ANCIENTS_CYCLOPS_TITLE";
                string cyclopsText = "HINT_ANCIENTS_CYCLOPS_TEXT";
                string hintCyclopsDefense = "ANCIENTS_CYCLOPSDEFENSE";
                string cyclopsDefenseTitle = "HINT_ANCIENTS_CYCLOPSDEFENSE_TITLE";
                string cyclopsDefenseText = "HINT_ANCIENTS_CYCLOPSDEFENSE_TEXT";
                string hintHoplites = "ANCIENTS_HOPLITS";
                string hoplitesTitle = "HINT_ANCIENTS_HOPLITS_TITLE";
                string hoplitesText = "HINT_ANCIENTS_HOPLITS_TEXT";
                string hintHopliteRepair = "ANCIENTS_HOPLITSREPAIR";
                string hoplitesRepairTitle = "HINT_ANCIENTS_HOPLITSREPAIR_TITLE";
                string hoplitesRepairText = "HINT_ANCIENTS_HOPLITSREPAIR_TEXT";
                string hintHopliteMaxPower = "ANCIENTS_HOPLITSMAXPOWER";
                string hopliteMaxPowerTitle = "HINT_ANCIENTS_HOPLITSMAXPOWER_TITLE";
                string hopliteMaxPowerText = "HINT_ANCIENTS_HOPLITSMAXPOWER_TEXT";

                TFTVTutorialAndStory.CreateNewTacticalHint(hintCyclops, HintTrigger.ActorSeen, cyclopsTag.name, cyclopsTitle, cyclopsText, 1, true, "41B73D60-433A-4F75-9E8B-CA30FBE45622");
                TFTVTutorialAndStory.CreateNewTacticalHint(hintHoplites, HintTrigger.ActorSeen, hopliteTag.name, hoplitesTitle, hoplitesText, 1, true, "2DC1BC66-F42F-4E84-9680-826A57C28E48");
                TFTVTutorialAndStory.CreateNewTacticalHint(hintCyclopsDefense, HintTrigger.ActorHurt, cyclopsTag.name, cyclopsDefenseTitle, cyclopsDefenseText, 1, true, "E4A4FB8B-10ED-49CF-870A-6ED9497F6895");
                TFTVTutorialAndStory.CreateNewTacticalHint(hintStory1, HintTrigger.MissionStart, ancientMissionTag.name, story1Title, story1Text, 3, true, "24C57D44-3CBA-4310-AB09-AE9444822C91");
                ContextHelpHintDef hoplitesHint = DefCache.GetDef<ContextHelpHintDef>(hintHoplites);
                hoplitesHint.Conditions.Add(TFTVTutorialAndStory.ActorHasStatusHintConditionDefCreateNewConditionForTacticalHint("Alerted_StatusDef"));
                TFTVTutorialAndStory.CreateNewTacticalHint(hintHopliteRepair, HintTrigger.ActorSeen, AddAutoRepairStatusAbility.name, hoplitesRepairTitle, hoplitesRepairText, 2, true, "B25F1794-5641-40D3-88B5-0AA104FC75A1");
               // ContextHelpHintDef hoplitesRepairHint = DefCache.GetDef<ContextHelpHintDef> (hintHopliteRepair);
               // hoplitesRepairHint.Conditions.Add(TFTVTutorialAndStory.ActorHasTagCreateNewConditionForTacticalHint(hopliteTag.name));

                TFTVTutorialAndStory.CreateNewTacticalHint(hintHopliteMaxPower, HintTrigger.ActorSeen, ancientsPowerUpStatus.name, hopliteMaxPowerTitle, hopliteMaxPowerText, 2, true, "0DC75121-325A-406E-AC37-5F1AAB4E7778");

               // GeoscapeTutorialStepsDef geoscapeTutorialStepsDef = DefCache.GetDef<GeoscapeTutorialStepsDef>("GeoscapeTutorialStepsDef");

                /*   List<GeoscapeTutorialStep> geoscapeTutorialSteps = new List<GeoscapeTutorialStep>();

                   foreach(GeoscapeTutorialStep tutorialStep in geoscapeTutorialStepsDef.Hints) 
                   {
                       if (tutorialStep.StepType == GeoscapeTutorialStepType.HarvestingSiteCaptured || tutorialStep.StepType == GeoscapeTutorialStepType.RefineryCaptured) 
                       {
                           geoscapeTutorialSteps.Add(tutorialStep);

                       }
                   }*/
               // geoscapeTutorialStepsDef.Hints.RemoveRange(3, 2);
               // TFTVLogger.Always("Geohints count " + geoscapeTutorialStepsDef.Hints.Count());

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }



        }
        
       

        public static void RemovePandoranVirusResearchRequirement()
        {
            try
            {
                ResearchDef alienPhysiologyResearch = DefCache.GetDef<ResearchDef>("NJ_AlienPhysiology_ResearchDef");
                ResearchDef pandoraKeyResearch = DefCache.GetDef<ResearchDef>("PX_PandoraKey_ResearchDef");
                ResearchDef virophageResearch = DefCache.GetDef<ResearchDef>("PX_VirophageWeapons_ResearchDef");

                ExistingResearchRequirementDef reqForAlienPhysiology = DefCache.GetDef<ExistingResearchRequirementDef>("NJ_AlienPhysiology_ResearchDef_ExistingResearchRequirementDef_1");
                reqForAlienPhysiology.ResearchID = "PX_AlienVirusInfection_ResearchDef";


                ExistingResearchRequirementDef reqForVirophage = DefCache.GetDef<ExistingResearchRequirementDef>("PX_VirophageWeapons_ResearchDef_ExistingResearchRequirementDef_1");

                /*   ResearchDef copyOfPhysiologyResearch = Helper.CreateDefFromClone(alienPhysiologyResearch, "B9110499-3186-4A4E-A448-C831F1F7BC08", "CopyOf" + alienPhysiologyResearch.name);           
                   alienPhysiologyResearch.RevealRequirements.Container = new ReseachRequirementDefOpContainer[] { 
                       copyOfPhysiologyResearch.RevealRequirements.Container[0],
                       copyOfPhysiologyResearch.RevealRequirements.Container[2], 
                       copyOfPhysiologyResearch.RevealRequirements.Container[3]};*/

                ResearchDef copyOfPandoraKeyResearch = Helper.CreateDefFromClone(pandoraKeyResearch, "C6515480-4A96-4125-AA7F-CD3B4D0D5341", "CopyOf" + pandoraKeyResearch.name);
                pandoraKeyResearch.RevealRequirements.Container = new ReseachRequirementDefOpContainer[] {
                    copyOfPandoraKeyResearch.RevealRequirements.Container[0],
                copyOfPandoraKeyResearch.RevealRequirements.Container[2],
                copyOfPandoraKeyResearch.RevealRequirements.Container[3]};

                // alienPhysiologyResearch.RevealRequirements.Container[1] = new ReseachRequirementDefOpContainer();
                //  pandoraKeyResearch.RevealRequirements.Container[1] = new ReseachRequirementDefOpContainer();
                reqForVirophage.ResearchID = "PX_YuggothianEntity_ResearchDef";
                
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void ChangeImpossibleWeapons()
        {
            try
            {
                WeaponDef shardGun = DefCache.GetDef<WeaponDef>("AC_ShardGun_WeaponDef");
                WeaponDef crystalCrossbow = DefCache.GetDef<WeaponDef>("AC_CrystalCrossbow_WeaponDef");
                WeaponDef mattock = DefCache.GetDef<WeaponDef>("AC_Mattock_WeaponDef");
                WeaponDef rebuke = DefCache.GetDef<WeaponDef>("AC_Rebuke_WeaponDef");
                WeaponDef scorpion = DefCache.GetDef<WeaponDef>("AC_Scorpion_WeaponDef");
                WeaponDef scyther = DefCache.GetDef<WeaponDef>("AC_Scyther_WeaponDef");

                mattock.ManufactureRequiredTagDefs.Clear();
                scyther.ManufactureRequiredTagDefs.RemoveAt(1);
                scorpion.ManufactureRequiredTagDefs.RemoveAt(1);

                scorpion.ManufactureProteanMutane = 75;
                scorpion.ManufactureOricalcum = 75;

               /* List<WeaponDef> allAncientWeapons = new List<WeaponDef> { shardGun, crystalCrossbow, mattock, rebuke, scorpion, scyther };

                FactionFunctionalityTagDataDef orichalcumRefineryRequirement = DefCache.GetDef<FactionFunctionalityTagDataDef>("RefineOrichalcum_FactionFunctionalityTagDef");

                //Remove the requirement of the processing facilities for all impossible weapons
                foreach (WeaponDef weaponDef in allAncientWeapons)
                {
                                      
                    
                    if(weaponDef.ManufactureRequiredTagDefs.Contains(orichalcumRefineryRequirement as GameTagDef))

                    weaponDef.ManufactureRequiredTagDefs.RemoveAt(0);
                }*/

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);

            }
        }


        public static void ChangeAncientSiteExploration()
        {
            try
            {

                AncientSiteProbeItemDef ancientSiteProbeItemDef = DefCache.GetDef<AncientSiteProbeItemDef>("AncientSiteProbeItemDef");
                ancientSiteProbeItemDef.ManufactureTech = 5;
                ancientSiteProbeItemDef.ManufactureMaterials = 25;

                DefCache.GetDef<ExcavateAbilityDef>("ExcavateAbilityDef").Cost.Values = new List<ResourceUnit>() { new ResourceUnit(ResourceType.Materials, value: 20), new ResourceUnit(ResourceType.Tech, value: 5) };
                AncientGuardianGuardAbilityDef AncientGuardianGuardAbilityDef = DefCache.GetDef<AncientGuardianGuardAbilityDef>("AncientGuardianGuardAbilityDef");
                AncientGuardianGuardAbilityDef.Cost.Values.Add(new ResourceUnit { Type = ResourceType.Tech, Value = 25 });
                AncientGuardianGuardAbilityDef.LocalSiteResourceCost = 0;

                ArcheologySettingsDef archeologySettingsDef = DefCache.GetDef<ArcheologySettingsDef>("ArcheologySettingsDef");
                archeologySettingsDef.ArcheologyPassiveOutput = 0;
                archeologySettingsDef.ArcheologyFacilityHarvestingPower = 0;
                archeologySettingsDef.ExcavationCostReductionPerFacilityPercentage = 0;
                archeologySettingsDef.MaxExcavationProbeCostReductionPerPercentage = 0;
                archeologySettingsDef.MaxProbeCostReductionPerPercentage = 0;
                archeologySettingsDef.ProbeCostReductionPerFacilityPercentage = 0;
                
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }
        public static void ChangeSchemataMissionRequirement()
        {
            try
            {

                DefCache.GetDef<GeoResearchEventFilterDef>("E_PROG_LE1_ResearchCompleted [GeoResearchEventFilterDef]").ResearchID = "ExoticMaterialsResearch";

            }

            catch (Exception e)
            {
                TFTVLogger.Error(e);

            }

        }

        public static void CreateAncientAutomataResearch()
        {
            try
            {

                ActorResearchRequirementDef sourceActorResearchRequirement = DefCache.GetDef<ActorResearchRequirementDef>("PX_Alien_Fishman_ResearchDef_ActorResearchRequirementDef_0");
                TacticalActorDef humanoidGuardianTacticalActor = DefCache.GetDef<TacticalActorDef>("HumanoidGuardian_ActorDef");
                ActorResearchRequirementDef requirementDef = Helper.CreateDefFromClone(sourceActorResearchRequirement, "14163A38-C07E-4BFF-8366-6E30F524F85D", "HumanoidActorResearchRequirement");
                requirementDef.Actor = humanoidGuardianTacticalActor;

                string ancientAutomataResearchName = "AncientAutomataResearch";
                string keyName = "AUTOMATA_RESEARCH_TITLE";
                string keyReveal = "";
                string keyUnlock = "AUTOMATA_RESEARCH_REVEAL";
                string keyComplete = "AUTOMATA_RESEARCH_COMPLETE";
                string keyBenefits = "AUTOMATA_RESEARCH_BENEFITS";
                int cost = 400;
                string gUID = "DEE22274-1E44-4D02-8773-2A22B5DA8213";
                string gUID2 = "DD59BCB8-F197-4A3D-A8AD-6B1D0B9CB958";
                ResearchViewElementDef imageSource = DefCache.GetDef<ResearchViewElementDef>("PX_AntediluvianArchaeology_ViewElementDef");

                ResearchDef research = TFTVCommonMethods.CreateNewPXResearch(ancientAutomataResearchName, cost, gUID, gUID2, keyName, keyReveal, keyUnlock, keyComplete, keyBenefits, imageSource);

                ReseachRequirementDefOpContainer[] revealRequirementContainer = new ReseachRequirementDefOpContainer[1];
                ResearchRequirementDef[] revealResearchRequirementDefs = new ResearchRequirementDef[1];
                revealResearchRequirementDefs[0] = requirementDef; //small box
                revealRequirementContainer[0].Requirements = revealResearchRequirementDefs; //medium box
                research.RevealRequirements.Container = revealRequirementContainer;
                research.RevealRequirements.Operation = ResearchContainerOperation.ALL;


            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }

        public static void CreateExoticMaterialsResearch()
        {
            try
            {

                string defName = "ExoticMaterialsResearch";
                string keyName = "EXOTIC_RESEARCH_TITLE";
                string keyReveal = "";
                string keyUnlock = "EXOTIC_RESEARCH_REVEAL";
                string keyComplete = "EXOTIC_RESEARCH_COMPLETE";
                string keyBenefits = "EXOTIC_RESEARCH_BENEFITS";
                int cost = 400;
                string gUID = "17D9D851-3B75-48A7-A856-D72F101E4CE0";
                string gUID2 = "05BA677D-52E4-4A16-B050-130EEDBB956D";
                ResearchViewElementDef imageSource = DefCache.GetDef<ResearchViewElementDef>("PX_AntediluvianArchaeology_ViewElementDef");

                ResearchDef research = TFTVCommonMethods.CreateNewPXResearch(defName, cost, gUID, gUID2, keyName, keyReveal, keyUnlock, keyComplete, keyBenefits, imageSource);

                ExistingResearchRequirementDef requirementDef = TFTVCommonMethods.CreateNewExistingResearchResearchRequirementDef(defName + "ResearchReq", "47BA0BD6-F622-4EC7-A49B-B93C0A955D3C", "AncientAutomataResearch");

                ReseachRequirementDefOpContainer[] revealRequirementContainer = new ReseachRequirementDefOpContainer[1];
                ResearchRequirementDef[] revealResearchRequirementDefs = new ResearchRequirementDef[1];
                revealResearchRequirementDefs[0] = requirementDef; //small box
                revealRequirementContainer[0].Requirements = revealResearchRequirementDefs; //medium box
                research.RevealRequirements.Container = revealRequirementContainer;
                research.RevealRequirements.Operation = ResearchContainerOperation.ALL;


            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }

        public static void CreateAbilitiesForAncients()
        {
            try
            {
                //Creating status effect to show that Guardian will repair a body part next turn. Need to create a status to show small icon.

                DamageMultiplierStatusDef sourceAbilityStatusDef = DefCache.GetDef<DamageMultiplierStatusDef>("BionicResistances_StatusDef");

                string statusSelfRepairAbilityName = "AutoRepair_AddAbilityStatusDef";
                DamageMultiplierStatusDef statusSelfRepairAbilityDef = Helper.CreateDefFromClone(sourceAbilityStatusDef, "17A2DF06-6BA5-46F3-92B5-D85F74193ABD", statusSelfRepairAbilityName);
                statusSelfRepairAbilityDef.EffectName = "Selfrepair";
                statusSelfRepairAbilityDef.ApplicationConditions = new EffectConditionDef[] { };
                statusSelfRepairAbilityDef.Visuals = Helper.CreateDefFromClone(sourceAbilityStatusDef.Visuals, "2330D0AE-547B-492E-8C49-16BFDD498653", statusSelfRepairAbilityName);
                statusSelfRepairAbilityDef.VisibleOnHealthbar = TacStatusDef.HealthBarVisibility.AlwaysVisible;
                statusSelfRepairAbilityDef.VisibleOnPassiveBar = true;
                statusSelfRepairAbilityDef.VisibleOnStatusScreen = TacStatusDef.StatusScreenVisibility.VisibleOnStatusesList;
                statusSelfRepairAbilityDef.Visuals.DisplayName1.LocalizationKey = "HOPLITES_SELF_REPAIR_NAME";
                statusSelfRepairAbilityDef.Visuals.Description.LocalizationKey = "HOPLITES_SELF_REPAIR_DESCRIPTION";
                statusSelfRepairAbilityDef.Visuals.LargeIcon = Helper.CreateSpriteFromImageFile("TFTV_status_self_repair.png");
                statusSelfRepairAbilityDef.Visuals.SmallIcon = Helper.CreateSpriteFromImageFile("TFTV_status_self_repair.png");
                statusSelfRepairAbilityDef.DamageTypeDefs = new DamageTypeBaseEffectDef[] { };
                statusSelfRepairAbilityDef.Multiplier = 1;


            }

            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

       

        public static void CreateAncientStatusEffects()
        {
            try
            {
               
                //Status for Cyclops Defense
                //Cyclops is near invulnerable while most hoplites are alive
                string statusCyclopsDefenseName = "CyclopsDefense_StatusDef";
                StandardDamageTypeEffectDef projectileDamage = DefCache.GetDef<StandardDamageTypeEffectDef>("Projectile_StandardDamageTypeEffectDef");
                StandardDamageTypeEffectDef blastDamage = DefCache.GetDef<StandardDamageTypeEffectDef>("Blast_StandardDamageTypeEffectDef");
                DamageMultiplierStatusDef source = DefCache.GetDef<DamageMultiplierStatusDef>("BionicResistances_StatusDef");
                DamageMultiplierStatusDef statusCyclopsDefense = Helper.CreateDefFromClone(
                    source,
                    "1C2318BF-9DF4-479A-B220-93471A6ED3D0",
                    statusCyclopsDefenseName);
                statusCyclopsDefense.EffectName = "CyclopsDefense";
                statusCyclopsDefense.VisibleOnHealthbar = TacStatusDef.HealthBarVisibility.AlwaysVisible;
                statusCyclopsDefense.VisibleOnPassiveBar = true;
                statusCyclopsDefense.VisibleOnStatusScreen = TacStatusDef.StatusScreenVisibility.VisibleOnStatusesList;


                statusCyclopsDefense.Visuals = Helper.CreateDefFromClone(
                    source.Visuals,
                    "041002FC-A98F-4F09-B3AC-79089D7A9C63",
                    statusCyclopsDefenseName);
                statusCyclopsDefense.Multiplier = 0.25f;
                statusCyclopsDefense.Range = -1;
                statusCyclopsDefense.DamageTypeDefs = new DamageTypeBaseEffectDef[] { projectileDamage, blastDamage };

                statusCyclopsDefense.Visuals.LargeIcon = Helper.CreateSpriteFromImageFile("TFTV_status_cyclops_defense.png");
                statusCyclopsDefense.Visuals.SmallIcon = Helper.CreateSpriteFromImageFile("TFTV_status_cyclops_defense.png");

                statusCyclopsDefense.Visuals.DisplayName1.LocalizationKey = "CYCLOPS_DEFENSE_NAME";
                statusCyclopsDefense.Visuals.Description.LocalizationKey = "CYCLOPS_DEFENSE_DESCRIPTION";

                //Status for hoplites; extra damage when charged
                string skillName = "AncientsPoweredUp";
                DamageMultiplierStatusDef ancientPowerUp = Helper.CreateDefFromClone(
                    source,
                    "94E23AE4-8089-41A2-98B1-899535FC577A",
                    skillName);
                ancientPowerUp.EffectName = "AncientPowerUP";
                ancientPowerUp.VisibleOnHealthbar = TacStatusDef.HealthBarVisibility.AlwaysVisible;
                ancientPowerUp.VisibleOnPassiveBar = false;
                ancientPowerUp.VisibleOnStatusScreen = TacStatusDef.StatusScreenVisibility.VisibleOnStatusesList;
                ancientPowerUp.HealthbarPriority = -1;
                ancientPowerUp.ExpireOnEndOfTurn = false;

                

                ancientPowerUp.Visuals = Helper.CreateDefFromClone(
                    source.Visuals,
                    "E24AFC45-FD5F-4E2A-BDFC-5D9A8B240813",
                    skillName);
                ancientPowerUp.DamageTypeDefs = new DamageTypeBaseEffectDef[] { };
                ancientPowerUp.Multiplier = 0.75f;

                ancientPowerUp.Visuals.LargeIcon = Helper.CreateSpriteFromImageFile("TFTV_status_powered_up.png");
                ancientPowerUp.Visuals.SmallIcon = Helper.CreateSpriteFromImageFile("TFTV_status_powered_up.png");

                ancientPowerUp.Visuals.DisplayName1.LocalizationKey = "POWERED_UP_NAME";
                ancientPowerUp.Visuals.Description.LocalizationKey = "POWERED_UP_DESCRIPTION";

                string abilityName = "AncientMaxPower_AbilityDef";
                PassiveModifierAbilityDef sourceAbility = DefCache.GetDef<PassiveModifierAbilityDef>("SelfDefenseSpecialist_AbilityDef");
                PassiveModifierAbilityDef ancientMaxPowerAbility = Helper.CreateDefFromClone(
                    sourceAbility,
                    "6C77E15C-B1C4-46A3-A196-2BB6A6E7EB5E",
                    abilityName);
                ancientMaxPowerAbility.CharacterProgressionData = Helper.CreateDefFromClone(
                    sourceAbility.CharacterProgressionData,
                    "5125A077-0D6A-4AC1-A7A1-B554F68AEEBB",
                    abilityName);
                ancientMaxPowerAbility.ViewElementDef = Helper.CreateDefFromClone(
                    sourceAbility.ViewElementDef,
                    "5936B6D6-9DEA-4087-8AA5-DC76DA6FFAE7",
                    abilityName);
                ancientMaxPowerAbility.ViewElementDef.ShowInStatusScreen = false;
                ancientMaxPowerAbility.StatModifications = new ItemStatModification[]
                { new ItemStatModification {TargetStat = StatModificationTarget.BonusAttackDamage, Modification = StatModificationType.Multiply, Value = 1.50f},
                };
                ancientMaxPowerAbility.ItemTagStatModifications = new EquipmentItemTagStatModification[0];
                ancientMaxPowerAbility.ViewElementDef.DisplayName1.LocalizationKey = "POWERED_UP_NAME";
                ancientMaxPowerAbility.ViewElementDef.Description.LocalizationKey = "POWERED_UP_DESCRIPTION";

                ancientMaxPowerAbility.ViewElementDef.LargeIcon = Helper.CreateSpriteFromImageFile("TFTV_status_powered_up.png");
                ancientMaxPowerAbility.ViewElementDef.SmallIcon = Helper.CreateSpriteFromImageFile("TFTV_status_powered_up.png");

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void ChangeAncientsWeapons()
        {
            try
            {
                //Reducing "base" damage of automata's weapons; the idea is to then increase them dynamically during mission
                WeaponDef drill = DefCache.GetDef<WeaponDef>("HumanoidGuardian_Drill_WeaponDef");
                drill.DamagePayload.DamageKeywords[0].Value = 60;

                WeaponDef hopliteBeam = DefCache.GetDef<WeaponDef>("HumanoidGuardian_Head_WeaponDef");
                hopliteBeam.DamagePayload.DamageKeywords[0].Value = 70;

                WeaponDef shield = DefCache.GetDef<WeaponDef>("HumanoidGuardian_RightShield_WeaponDef");
                shield.DamagePayload.DamageKeywords[0].Value = 80;

                WeaponDef cyclopsLCBeam = DefCache.GetDef<WeaponDef>("MediumGuardian_Head_LivingCrystal_WeaponDef");
                cyclopsLCBeam.DamagePayload.DamageKeywords[0].Value = 120;

                WeaponDef cyclopsOBeam = DefCache.GetDef<WeaponDef>("MediumGuardian_Head_Orichalcum_WeaponDef");
                cyclopsOBeam.DamagePayload.DamageKeywords[0].Value = 120;

                WeaponDef cyclopsPBeam = DefCache.GetDef<WeaponDef>("MediumGuardian_Head_ProteanMutane_WeaponDef");
                cyclopsPBeam.DamagePayload.DamageKeywords[0].Value = 120;
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }



        public static void ChangeAncientsBodyParts()
        {
            try
            {
                //string name = "BlackShielder";
                TacCharacterDef shielder = DefCache.GetDef<TacCharacterDef>("HumanoidGuardian_Shielder_TacCharacterDef");
                TacCharacterDef driller = DefCache.GetDef<TacCharacterDef>("HumanoidGuardian_Driller_TacCharacterDef");



                BodyPartAspectDef headBodyAspect = DefCache.GetDef<BodyPartAspectDef>("E_BodyPartAspect [HumanoidGuardian_Head_WeaponDef]");
                headBodyAspect.Endurance = 0;
                headBodyAspect.WillPower = 0;

                BodyPartAspectDef torsoDrillBodyAspect = DefCache.GetDef<BodyPartAspectDef>("E_BodyPartAspect [HumanoidGuardian_TorsoDrill_BodyPartDef]");
                BodyPartAspectDef torsoShieldsBodyAspect = DefCache.GetDef<BodyPartAspectDef>("E_BodyPartAspect [HumanoidGuardian_TorsoShields_BodyPartDef]");

                torsoDrillBodyAspect.WillPower = 30;
                torsoShieldsBodyAspect.WillPower = 30;

                TacticalItemDef proteanMutaneTorso = DefCache.GetDef<TacticalItemDef>("MediumGuardian_Torso_ProteanMutane_BodyPartDef");
                TacticalItemDef orichalcumTorso = DefCache.GetDef<TacticalItemDef>("MediumGuardian_Torso_Orichalcum_BodyPartDef");
                TacticalItemDef livingCrystalTorso = DefCache.GetDef<TacticalItemDef>("MediumGuardian_Torso_LivingCrystal_BodyPartDef");

                proteanMutaneTorso.Abilities = new AbilityDef[] { };
                orichalcumTorso.Abilities = new AbilityDef[] { };
                livingCrystalTorso.Abilities = new AbilityDef[] { };

                ApplyDamageEffectAbilityDef stomp = DefCache.GetDef<ApplyDamageEffectAbilityDef>("Guardian_Stomp_AbilityDef");
                stomp.DamagePayload.AoeRadius = 7;
                stomp.TargetingDataDef.Origin.Range = 7;


                // StanceStatusDef ancientGuardianStealthStatus = DefCache.GetDef<StanceStatusDef>("AncientGuardianStealth_StatusDef");
                //    ancientGuardianStealthStatus.StatModifications[0].Value = 0.75f;
                /* TacCharacterDef newShielder = Helper.CreateDefFromClone(shielder, "C3BDED7B-1C4E-4DAA-8418-7A35DF601875", name);
                 newShielder.SpawnCommandId = "BlackShield";
                 newShielder.Data.Name = "BlackShield";

                 CustomizationSecondaryColorTagDef blackColor = DefCache.GetDef<CustomizationSecondaryColorTagDef>("CustomizationSecondaryColorTagDef_7");
                 List<GameTagDef> gameTags = newShielder.Data.GameTags.ToList();
                 gameTags.Add(blackColor);
                 newShielder.Data.GameTags=gameTags.ToArray();*/

                /*
                string name2 = "RedShielder";
                TacCharacterDef newShielder2 = Helper.CreateDefFromClone(shielder, "052B5D66-A9A2-4281-B95F-F6787F7282E3", name2);
                newShielder2.SpawnCommandId = "RedShield";
                newShielder2.Data.Name = "RedShield";

                CustomizationSecondaryColorTagDef redColor = DefCache.GetDef<CustomizationSecondaryColorTagDef>("CustomizationSecondaryColorTagDef_7");
                List<GameTagDef> gameTags2 = newShielder2.Data.GameTags.ToList();
                gameTags2.Add(redColor);
                newShielder2.Data.GameTags = gameTags2.ToArray();

                string name3 = "GreyShielder";
                TacCharacterDef newShielder3 = Helper.CreateDefFromClone(shielder, "65B5EBFD-F4F4-436B-BC7C-CD4316686CAD", name3);
                newShielder2.SpawnCommandId = "GreyShield";
                newShielder2.Data.Name = "GreyShield";

                CustomizationSecondaryColorTagDef greyColor = DefCache.GetDef<CustomizationSecondaryColorTagDef>("CustomizationSecondaryColorTagDef_0");
                List<GameTagDef> gameTags3 = newShielder3.Data.GameTags.ToList();
                gameTags3.Add(greyColor);
                newShielder3.Data.GameTags = gameTags3.ToArray();

                string name4 = "PinkShielder";
                TacCharacterDef newShielder4 = Helper.CreateDefFromClone(shielder, "C113580D-EC1B-4EAC-9927-1AF5E8446C57", name4);
                newShielder2.SpawnCommandId = "PinkShield";
                newShielder2.Data.Name = "PinkShield";

                CustomizationSecondaryColorTagDef pinkColor = DefCache.GetDef<CustomizationSecondaryColorTagDef>("CustomizationSecondaryColorTagDef_14");
                List<GameTagDef> gameTags4 = newShielder4.Data.GameTags.ToList();
                gameTags4.Add(pinkColor);
                newShielder4.Data.GameTags = gameTags4.ToArray();

                string name5 = "GreenShielder";
                TacCharacterDef newShielder5 = Helper.CreateDefFromClone(shielder, "53208DD6-3199-4ED9-BEA3-9C3D9441F90E", name5);
                newShielder2.SpawnCommandId = "GreenShield";
                newShielder2.Data.Name = "GreenShield";

                CustomizationSecondaryColorTagDef greenColor = DefCache.GetDef<CustomizationSecondaryColorTagDef>("CustomizationSecondaryColorTagDef_14");
                List<GameTagDef> gameTags5 = newShielder5.Data.GameTags.ToList();
                gameTags5.Add(greenColor);
                newShielder5.Data.GameTags = gameTags5.ToArray();
                */

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }
        public static void ChangeAntartica()
        {
            try
            {
                CustomMissionTypeDef antarticaMission = DefCache.GetDef<CustomMissionTypeDef>("StoryPX15_CustomMissionTypeDef");

                CustomMissionTypeDef OrichalcumMission = DefCache.GetDef<CustomMissionTypeDef>("OrichalcumHarvestAttack_Ancient_CustomMissionTypeDef");

                antarticaMission.ParticipantsData = OrichalcumMission.ParticipantsData;
                antarticaMission.ParticipantsData[0].ParticipantKind = TacMissionParticipant.Intruder;
                antarticaMission.ParticipantsRelations[0].SecondParticipant = TacMissionParticipant.Intruder;
                List<TacMissionTypeParticipantData.UniqueChatarcterBind> uniqueChatarcterBinds = antarticaMission.ParticipantsData[0].UniqueUnits.ToList();
                // uniqueChatarcterBinds.Add(DefCache.GetDef<CustomMissionTypeDef>("CrystalsHarvestAttack_Ancient_CustomMissionTypeDef").ParticipantsData[0].UniqueUnits[0]);
                // antarticaMission.ParticipantsData[0].UniqueUnits = uniqueChatarcterBinds.ToArray();

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void CreateSubject24()
        {
            try
            {
                string nameDef = "Subject24_TacCharacerDef";

                TacCharacterDef subject24 = Helper.CreateDefFromClone(DefCache.GetDef<TacCharacterDef>("NJ_Jugg_TacCharacterDef"), "A4F0335E-BF41-4175-8C28-7B0DE5352224", nameDef);
                subject24.Data.Name = "Subject 24";

                // CustomizationColorTagDef_10 green
                // CustomizationColorTagDef_14 pink
                // CustomizationColorTagDef_0 grey
                // CustomizationColorTagDef_7 red

                CustomizationPrimaryColorTagDef blackColor = DefCache.GetDef<CustomizationPrimaryColorTagDef>("CustomizationColorTagDef_9");
                List<GameTagDef> gameTags = subject24.Data.GameTags.ToList();
                gameTags.Add(blackColor);
                subject24.SpawnCommandId = "Subject24TFTV";
                subject24.Data.GameTags = gameTags.ToArray();

                List<TacMissionTypeParticipantData.UniqueChatarcterBind> tacCharacterDefs = DefCache.GetDef<CustomMissionTypeDef>("StoryPU14_CustomMissionTypeDef").ParticipantsData[1].UniqueUnits.ToList();
                TacMissionTypeParticipantData.UniqueChatarcterBind uniqueChatarcterBind = new TacMissionTypeParticipantData.UniqueChatarcterBind
                {
                    Character = subject24,
                    Amount = new Base.Utils.RangeDataInt { Max = 1, Min = 1 },
                };
                tacCharacterDefs.Add(uniqueChatarcterBind);
                DefCache.GetDef<CustomMissionTypeDef>("StoryPU14_CustomMissionTypeDef").ParticipantsData[1].UniqueUnits = tacCharacterDefs.ToArray();

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }




        public static void AllowMedkitsToTargetMutoids()
        {
            try
            {
                //Allow medkits to target Mutoids
                DefCache.GetDef<TacticalTargetingDataDef>("E_TargetingData [Medkit_AbilityDef]").Origin.CullTargetTags.Clear();

                //Make Cure Spray/Cure Cloud remove acid
                string abilityName = "AcidStatusRemover";
                StatusRemoverEffectDef sourceStatusRemoverEffect = DefCache.GetDef<StatusRemoverEffectDef>("StrainedRemover_EffectDef");
                StatusRemoverEffectDef newAcidStatusRemoverEffect = Helper.CreateDefFromClone(sourceStatusRemoverEffect, "0AE26C25-A67D-4F2F-B036-F7649B26B695", abilityName);
                newAcidStatusRemoverEffect.StatusToRemove = "Acid";
                List<EffectDef> effectDefsList = DefCache.GetDef<MultiEffectDef>("Cure_MultiEffectDef").EffectDefs.ToList();
                effectDefsList.Add(newAcidStatusRemoverEffect);
                DefCache.GetDef<MultiEffectDef>("Cure_MultiEffectDef").EffectDefs = effectDefsList.ToArray();

                //Skill toss per Belial's suggestions

                //need to change/clone CharacterProgressionData 
                AbilityCharacterProgressionDef sourceFirstLevel = DefCache.GetDef<AbilityCharacterProgressionDef>("E_CharacterProgressionData [GooImmunity_AbilityDef]");


                DefCache.GetDef<AbilityCharacterProgressionDef>("E_CharacterProgressionData [VirusResistant_DamageMultiplierAbilityDef]").MutagenCost = 10;
                AbilityCharacterProgressionDef demolitionStanceCPD =
                    Helper.CreateDefFromClone(sourceFirstLevel, "F4DA4D75-8FCE-4414-BB88-7A065A45105C", "E_CharacterProgressionData [Demolition_AbilityDef]");



                AbilityCharacterProgressionDef mindControlImmunityCPD = DefCache.GetDef<AbilityCharacterProgressionDef>("E_CharacterProgressionData [MindControlImmunity_AbilityDef]");
                mindControlImmunityCPD.MutagenCost = 15;
                mindControlImmunityCPD.SkillPointCost = 0;
                mindControlImmunityCPD.RequiredSpeed = 0;
                mindControlImmunityCPD.RequiredStrength = 0;
                mindControlImmunityCPD.RequiredWill = 0;

                AbilityCharacterProgressionDef poisonImmunityCPD = Helper.CreateDefFromClone(sourceFirstLevel, "67418B3A-C666-41CE-B504-853C6C705284", "E_CharacterProgressionData [PoisonImmunity_AbilityDef]");
                poisonImmunityCPD.MutagenCost = 20;
                DamageMultiplierAbilityDef poisonImmunity = DefCache.GetDef<DamageMultiplierAbilityDef>("PoisonImmunity_DamageMultiplierAbilityDef");
                poisonImmunity.CharacterProgressionData = poisonImmunityCPD;


                AbilityCharacterProgressionDef acidResistanceCPD = Helper.CreateDefFromClone(sourceFirstLevel, "03367F73-97B9-4E65-919B-D31DF147EAA0", "E_CharacterProgressionData [AcidResistance_AbilityDef]");
                acidResistanceCPD.MutagenCost = 25;
                DamageMultiplierAbilityDef acidResistance = DefCache.GetDef<DamageMultiplierAbilityDef>("AcidResistant_DamageMultiplierAbilityDef");
                acidResistance.CharacterProgressionData = acidResistanceCPD;

                AbilityCharacterProgressionDef leapCPD = Helper.CreateDefFromClone(sourceFirstLevel, "99339FAB-3FA5-4472-89B6-52A816464637", "E_CharacterProgressionData [RocketLeap_AbilityDef]");
                leapCPD.MutagenCost = 30;

                JetJumpAbilityDef leap = DefCache.GetDef<JetJumpAbilityDef>("Exo_Leap_AbilityDef");
                leap.CharacterProgressionData = leapCPD;




                ApplyStatusAbilityDef demolitionAbility = DefCache.GetDef<ApplyStatusAbilityDef>("DemolitionMan_AbilityDef");
                demolitionAbility.CharacterProgressionData = demolitionStanceCPD;



                AbilityTrackDef arthronAbilityTrack = DefCache.GetDef<AbilityTrackDef>("E_AbilityTrack [ArthronSpecializationDef]");
                //    AbilityTrackDef tritonAbilityTrack = DefCache.GetDef<AbilityTrackDef>("E_AbilityTrack [TritonSpecializationDef]");
                AbilityTrackDef sirenAbilityTrack = DefCache.GetDef<AbilityTrackDef>("E_AbilityTrack [SirenSpecializationDef]");
                AbilityTrackDef scyllaAbilityTrack = DefCache.GetDef<AbilityTrackDef>("E_AbilityTrack [ScyllaSpecializationDef]");
                AbilityTrackDef acheronAbilityTrack = DefCache.GetDef<AbilityTrackDef>("E_AbilityTrack [AcheronSpecializationDef]");



                arthronAbilityTrack.AbilitiesByLevel[0].Ability = DefCache.GetDef<DamageMultiplierAbilityDef>("VirusResistant_DamageMultiplierAbilityDef");
                arthronAbilityTrack.AbilitiesByLevel[2].Ability = poisonImmunity;
                arthronAbilityTrack.AbilitiesByLevel[4].Ability = DefCache.GetDef<ApplyEffectAbilityDef>("MistBreather_AbilityDef");

                //Reduce cost of Mutoid Syphon attack to 1AP
                DefCache.GetDef<BashAbilityDef>("Mutoid_Syphon_Strike_AbilityDef").ActionPointCost = 0.25f;


                scyllaAbilityTrack.AbilitiesByLevel[0].Ability = demolitionAbility;
                scyllaAbilityTrack.AbilitiesByLevel[4].Ability = leap;

                sirenAbilityTrack.AbilitiesByLevel[3].Ability = acidResistance;

                acheronAbilityTrack.AbilitiesByLevel[1].Ability = DefCache.GetDef<ApplyStatusAbilityDef>("MindControlImmunity_AbilityDef");

            }


            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }
        public static void RemoveCensusResearch()
        {
            try
            {
                DefCache.GetDef<ResearchDbDef>("pp_ResearchDB").Researches.Remove(DefCache.GetDef<ResearchDef>("PX_SDI_ResearchDef"));
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }
        public static void CreateHints()
        {
            try
            {
                TFTVTutorialAndStory.CreateNewTacticalHint("UmbraSighted", HintTrigger.ActorSeen, "Oilcrab_TacCharacterDef", "UMBRA_SIGHTED_TITLE", "UMBRA_SIGHTED_TEXT", 0, true, "C63F5953-9D29-4245-8FCD-1B8B875C007D");
                TFTVTutorialAndStory.CreateNewTacticalHint("UmbraSightedTriton", HintTrigger.ActorSeen, "Oilfish_TacCharacterDef", "UMBRA_SIGHTED_TITLE", "UMBRA_SIGHTED_TEXT", 0, true, "7F85AF7F-D7F0-41F3-B6EF-839509FCCF00");
                TFTVTutorialAndStory.CreateNewTacticalHint("AcheronPrime", HintTrigger.ActorSeen, "AcheronPrime_TacCharacterDef", "HINT_ACHERON_PRIME_TITLE", "HINT_ACHERON_PRIME_DESCRIPTION", 0, true, "0266C7C5-B5A4-41B8-9987-653248113CC5");
                TFTVTutorialAndStory.CreateNewTacticalHint("AcheronAsclepius", HintTrigger.ActorSeen, "AcheronAsclepius_TacCharacterDef", "HINT_ACHERON_ASCLEPIUS_TITLE", "HINT_ACHERON_ASCLEPIUS_DESCRIPTION", 0, true, "F34ED218-BF6D-44CD-B653-9EC8C7AB0D84");
                TFTVTutorialAndStory.CreateNewTacticalHint("AcheronAsclepiusChampion", HintTrigger.ActorSeen, "AcheronAsclepiusChampion_TacCharacterDef", "HINT_ACHERON_ASCLEPIUS_CHAMPION_TITLE", "HINT_ACHERON_ASCLEPIUS_CHAMPION_DESCRIPTION", 0, true, "2FA6F938-0928-4C3A-A514-91F3BD90E048");
                TFTVTutorialAndStory.CreateNewTacticalHint("AcheronAchlys", HintTrigger.ActorSeen, "AcheronAchlys_TacCharacterDef", "HINT_ACHERON_ACHLYS_TITLE", "HINT_ACHERON_ACHLYS_DESCRIPTION", 0, true, "06EEEA6B-1264-4616-AC78-1A2A56911E72");
                TFTVTutorialAndStory.CreateNewTacticalHint("AcheronAchlysChampion", HintTrigger.ActorSeen, "AcheronAchlysChampion_TacCharacterDef", "HINT_ACHERON_ACHLYS_CHAMPION_TITLE", "HINT_ACHERON_ACHLYS_CHAMPION_DESCRIPTION", 0, true, "760FDBB6-1556-4B1D-AFE0-59C906672A5D");
                TFTVTutorialAndStory.CreateNewTacticalHint("RevenantSighted", HintTrigger.ActorSeen, "Any_Revenant_TagDef", "REVENANT_SIGHTED_TITLE", "REVENANT_SIGHTED_TEXT", 1, true, "194317EC-67DF-4775-BAFD-98499F82C2D7");

                TFTVTutorialAndStory.CreateNewTacticalHintInfestationMission("InfestationMissionIntro", "BBC5CAD0-42FF-4BBB-8E13-7611DC5695A6", "1ED63949-4375-4A9D-A017-07CF483F05D5", "2A01E924-A26B-44FB-AD67-B1B590B4E1D5");
                TFTVTutorialAndStory.CreateNewTacticalHintInfestationMission("InfestationMissionIntro2", "164A4170-F7DC-4350-90C0-D5C1A0284E0D", "CA236EF2-6E6B-4CE4-89E9-17157930F91A", "422A7D39-0110-4F5B-98BB-66B1B5F616DD");
                TFTVTutorialAndStory.CreateNewManualTacticalHint("TFTV_Tutorial1", "0D36F3D5-9A39-4A5C-B6A4-85B5A3007655", "KEY_TUT3_TFTV1_TITLE", "KEY_TUT3_TFTV1_DESCRIPTION");
                TFTVTutorialAndStory.CreateNewManualTacticalHint("TFTV_Tutorial2", "EA319607-D2F3-4293-AECE-91AC26C9BD5E", "KEY_TUT3_TFTV2_TITLE", "KEY_TUT3_TFTV2_DESCRIPTION");
                ContextHelpHintDef tutorialTFTV1 = DefCache.GetDef<ContextHelpHintDef>("TFTV_Tutorial1");
                ContextHelpHintDef tutorialTFTV2 = DefCache.GetDef<ContextHelpHintDef>("TFTV_Tutorial2");
                ContextHelpHintDef tutorial3MissionEnd = DefCache.GetDef<ContextHelpHintDef>("TUT3_MissionSuccess_HintDef");
                tutorial3MissionEnd.NextHint = tutorialTFTV1;
                tutorialTFTV1.NextHint = tutorialTFTV2;

                HasSeenHintHintConditionDef seenOilCrabConditionDef = DefCache.GetDef<HasSeenHintHintConditionDef>("UmbraSightedHasSeenHintConditionDef");
                HasSeenHintHintConditionDef seenFishCrabConditionDef = DefCache.GetDef<HasSeenHintHintConditionDef>("UmbraSightedTritonHasSeenHintConditionDef");
                ContextHelpHintDef oilCrabHint = DefCache.GetDef<ContextHelpHintDef>("UmbraSighted");
                ContextHelpHintDef oilFishHint = DefCache.GetDef<ContextHelpHintDef>("UmbraSightedTriton");
                oilCrabHint.Conditions.Add(seenFishCrabConditionDef);
                oilFishHint.Conditions.Add(seenOilCrabConditionDef);

                TFTVTutorialAndStory.CreateNewTacticalHintInfestationMissionEnd("InfestationMissionEnd");
                CreateStaminaHint();
                CreateUIDeliriumHint();
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void ChangesToAcherons()
        {
            try
            {
                ChangesAcheronResearches();
                CreateAcheronAbilitiesAndStatus();
                ChangesAcheronTemplates();
                ChangesAcheronsAI();
                ChangesAcheronAbilities();
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }


        }

        public static void ChangesAcheronAbilities()
        {
            try
            {
                string nameOfNotMetallicConditionTag = "Organic_ApplicationCondition";
                ActorHasTagEffectConditionDef sourceHasTagConditionDef = DefCache.GetDef<ActorHasTagEffectConditionDef>("HasHumanTag_ApplicationCondition");
                ActorHasTagEffectConditionDef organicEffectConditionDef = Helper.CreateDefFromClone(sourceHasTagConditionDef, "E1ADF8A5-746D-4176-9FFA-99296F96B9BE", nameOfNotMetallicConditionTag);
                organicEffectConditionDef.GameTag = DefCache.GetDef<SubstanceTypeTagDef>("Organic_SubstanceTypeTagDef");

                StatMultiplierStatusDef trembling = DefCache.GetDef<StatMultiplierStatusDef>("Trembling_StatusDef");
                trembling.ApplicationConditions = new EffectConditionDef[] { organicEffectConditionDef };


            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }


        }

        public static void ChangesAcheronsAI()
        {
            try
            {
                //Adjusting Delirium Spray
                AINonHealthDamageAttackPositionConsiderationDef deliriumSprayConsideration = DefCache.GetDef<AINonHealthDamageAttackPositionConsiderationDef>("Acheron_CorruptionSprayAttackPosition_AIConsiderationDef");
                deliriumSprayConsideration.EnemyMask = PhoenixPoint.Tactical.AI.ActorType.Combatant;
                AcidStatusDef acidStatus = DefCache.GetDef<AcidStatusDef>("Acid_StatusDef");
                deliriumSprayConsideration.DamageTypeStatusDef = acidStatus;

                //Removes exclusions to metallic and ancients targets
                HasTagSuitabilityDef deliriumSprayExcludedTags = DefCache.GetDef<HasTagSuitabilityDef>("E_TargetSuitability [Acheron_CorruptionTagsTargetsSuitability_AIConsiderationDef]");
                List<GameTagDef> deliriumSprayCheckTargetsByTag = deliriumSprayExcludedTags.GameTagDefs.ToList();
                deliriumSprayCheckTargetsByTag.Add(DefCache.GetDef<SubstanceTypeTagDef>("Organic_SubstanceTypeTagDef"));
                deliriumSprayExcludedTags.GameTagDefs = deliriumSprayCheckTargetsByTag.ToArray();
                deliriumSprayExcludedTags.HasTag = true;

                //Adjusting GooSpray
                AINonHealthDamageAttackPositionConsiderationDef gooSprayConsideration = DefCache.GetDef<AINonHealthDamageAttackPositionConsiderationDef>("Acheron_GooSprayAttackPosition_AIConsiderationDef");
                gooSprayConsideration.EnemyMask = PhoenixPoint.Tactical.AI.ActorType.Combatant;
                deliriumSprayConsideration.DamageTypeStatusDef = acidStatus;



            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void ChangesAcheronResearches()
        {
            try
            {

                //Researches
                ResearchDef acheronResearch1 = DefCache.GetDef<ResearchDef>("ALN_Acheron1_ResearchDef");
                ResearchDef acheronResearch2 = DefCache.GetDef<ResearchDef>("ALN_Acheron2_ResearchDef");
                ResearchDef acheronResearch3 = DefCache.GetDef<ResearchDef>("ALN_Acheron3_ResearchDef");
                ResearchDef acheronResearch4 = DefCache.GetDef<ResearchDef>("ALN_Acheron4_ResearchDef");
                ResearchDef acheronResearch5 = DefCache.GetDef<ResearchDef>("ALN_Acheron5_ResearchDef");
                ResearchDef acheronResearch6 = DefCache.GetDef<ResearchDef>("ALN_Acheron6_ResearchDef");



                ExistingResearchRequirementDef acheronResearchReq2 = DefCache.GetDef<ExistingResearchRequirementDef>("ALN_Acheron2_ResearchDef_ExistingResearchRequirementDef_0");
                ExistingResearchRequirementDef acheronResearchReq3 = DefCache.GetDef<ExistingResearchRequirementDef>("ALN_Acheron3_ResearchDef_ExistingResearchRequirementDef_0");
                ExistingResearchRequirementDef acheronResearchReq4 = DefCache.GetDef<ExistingResearchRequirementDef>("ALN_Acheron4_ResearchDef_ExistingResearchRequirementDef_0");
                ExistingResearchRequirementDef acheronResearchReq5 = DefCache.GetDef<ExistingResearchRequirementDef>("ALN_Acheron5_ResearchDef_ExistingResearchRequirementDef_0");
                ExistingResearchRequirementDef acheronResearchReq6 = DefCache.GetDef<ExistingResearchRequirementDef>("ALN_Acheron6_ResearchDef_ExistingResearchRequirementDef_0");

                //Acheron Prime will require heavy Chirons
                acheronResearchReq2.ResearchID = "ALN_Chiron2_ResearchDef";
                //Acheron Ascepius & Acheron Achlys will require Goo Chirons
                acheronResearchReq3.ResearchID = "ALN_Chiron7_ResearchDef";
                acheronResearchReq5.ResearchID = "ALN_Chiron7_ResearchDef";
                //Ascepius and Achlys Champions will require Bombard Chirons
                acheronResearchReq4.ResearchID = "ALN_Chiron9_ResearchDef";
                acheronResearchReq6.ResearchID = "ALN_Chiron9_ResearchDef";


            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }


        }

        public static void CreateAcheronAbilitiesAndStatus()
        {
            try
            {
                //Create Acheron Harbinger ability, to be used as a flag/counter when calculating chances of getting Void Touched
                string acheronHarbingerAbilityName = "Acheron_Harbinger_AbilityDef";
                PassiveModifierAbilityDef source = DefCache.GetDef<PassiveModifierAbilityDef>("SelfDefenseSpecialist_AbilityDef");
                PassiveModifierAbilityDef acheronHarbingerAbility = Helper.CreateDefFromClone(
                    source,
                    "3ABB6347-5ABA-4B4D-B786-C962B7A0540C",
                    acheronHarbingerAbilityName);
                acheronHarbingerAbility.CharacterProgressionData = Helper.CreateDefFromClone(
                    source.CharacterProgressionData,
                    "336671D9-281F-4985-8F7A-8EF424EF1FB8",
                    acheronHarbingerAbilityName);
                acheronHarbingerAbility.ViewElementDef = Helper.CreateDefFromClone(
                    source.ViewElementDef,
                    "FB0B38FC-CDB7-4EDF-9E39-89111528A84B",
                    acheronHarbingerAbilityName);
                acheronHarbingerAbility.StatModifications = new ItemStatModification[0];
                acheronHarbingerAbility.ItemTagStatModifications = new EquipmentItemTagStatModification[0];
                acheronHarbingerAbility.ViewElementDef.DisplayName1.LocalizationKey = "ACHERON_HARBINGER_NAME";
                acheronHarbingerAbility.ViewElementDef.Description.LocalizationKey = "ACHERON_HARBINGER_DESCRIPTION";
                acheronHarbingerAbility.ViewElementDef.LargeIcon = TFTVDefsRequiringReinjection.VoidIcon;
                acheronHarbingerAbility.ViewElementDef.SmallIcon = TFTVDefsRequiringReinjection.VoidIcon;

                //Creating Tributary to the Void, to spread TBTV on nearby allies
                string acheronTributaryAbilityName = "Acheron_Tributary_AbilityDef";
                PassiveModifierAbilityDef acheronTributaryAbility = Helper.CreateDefFromClone(
                    source,
                    "2CDB184A-4E8D-4E9A-B957-983A1FD23313",
                    acheronTributaryAbilityName);
                acheronTributaryAbility.CharacterProgressionData = Helper.CreateDefFromClone(
                    source.CharacterProgressionData,
                    "0770EB67-52CD-4E17-9A3B-CB6C91E86BC5",
                    acheronTributaryAbilityName);
                acheronTributaryAbility.ViewElementDef = Helper.CreateDefFromClone(
                    source.ViewElementDef,
                    "A82EFDD3-8ED7-46C8-8B52-D8051910419D",
                    acheronTributaryAbilityName);
                acheronTributaryAbility.StatModifications = new ItemStatModification[0];
                acheronTributaryAbility.ItemTagStatModifications = new EquipmentItemTagStatModification[0];
                acheronTributaryAbility.ViewElementDef.DisplayName1.LocalizationKey = "ACHERON_TRIBUTARY_NAME";
                acheronTributaryAbility.ViewElementDef.Description.LocalizationKey = "ACHERON_TRIBUTARY_DESCRIPTION";
                acheronTributaryAbility.ViewElementDef.LargeIcon = TFTVDefsRequiringReinjection.VoidIcon;
                acheronTributaryAbility.ViewElementDef.SmallIcon = TFTVDefsRequiringReinjection.VoidIcon;



                //Creating special status that will allow Umbra to target the character
                string umbraTargetStatusDefName = "TBTV_Target";
                DamageMultiplierStatusDef sourceForTargetAbility = DefCache.GetDef<DamageMultiplierStatusDef>("BionicResistances_StatusDef");

                DamageMultiplierStatusDef umbraTargetStatus = Helper.CreateDefFromClone(
                   sourceForTargetAbility,
                   "0C4558E8-2791-4669-8F5B-2DA1D20B2ADD",
                   umbraTargetStatusDefName);

                umbraTargetStatus.EffectName = "UmbraTarget";
                umbraTargetStatus.Visuals = Helper.CreateDefFromClone(
                    sourceForTargetAbility.Visuals,
                    "49A5DC8D-50B9-4CCC-A3D4-7576A1DDD375",
                    umbraTargetStatus.EffectName);
                umbraTargetStatus.VisibleOnHealthbar = TacStatusDef.HealthBarVisibility.AlwaysVisible;
                umbraTargetStatus.VisibleOnPassiveBar = true;
                umbraTargetStatus.VisibleOnStatusScreen = TacStatusDef.StatusScreenVisibility.VisibleOnStatusesList;
                umbraTargetStatus.Visuals.DisplayName1.LocalizationKey = "VOID_BLIGHT_NAME";
                umbraTargetStatus.Visuals.Description.LocalizationKey = "VOID_BLIGHT_DESCRIPTION";
                umbraTargetStatus.Visuals.LargeIcon = DefCache.GetDef<DeathBelcherAbilityDef>("Oilcrab_Die_DeathBelcher_AbilityDef").ViewElementDef.LargeIcon;
                umbraTargetStatus.Visuals.SmallIcon = DefCache.GetDef<DeathBelcherAbilityDef>("Oilcrab_Die_DeathBelcher_AbilityDef").ViewElementDef.SmallIcon;
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }

        public static void ChangesAcheronTemplates()
        {
            try
            {
                TacCharacterDef acheron = DefCache.GetDef<TacCharacterDef>("Acheron_TacCharacterDef");
                TacCharacterDef acheronPrime = DefCache.GetDef<TacCharacterDef>("AcheronPrime_TacCharacterDef");
                TacCharacterDef acheronAsclepius = DefCache.GetDef<TacCharacterDef>("AcheronAsclepius_TacCharacterDef");
                TacCharacterDef acheronAsclepiusChampion = DefCache.GetDef<TacCharacterDef>("AcheronAsclepiusChampion_TacCharacterDef");
                TacCharacterDef acheronAchlys = DefCache.GetDef<TacCharacterDef>("AcheronAchlys_TacCharacterDef");
                TacCharacterDef acheronAchlysChampion = DefCache.GetDef<TacCharacterDef>("AcheronAchlysChampion_TacCharacterDef");

                acheron.DeploymentCost = 180;
                acheronPrime.DeploymentCost = 240;
                acheronAsclepius.DeploymentCost = 310;
                acheronAchlys.DeploymentCost = 310;
                acheronAsclepiusChampion.DeploymentCost = 350;
                acheronAchlysChampion.DeploymentCost = 350;

                //Adding Harbinger of the Void to Acheron, Acheron Prime and Acheron Asclepius Champion
                //Adding Co-Delirium To Acheron Prime
                PassiveModifierAbilityDef harbinger = DefCache.GetDef<PassiveModifierAbilityDef>("Acheron_Harbinger_AbilityDef");
                PassiveModifierAbilityDef tributary = DefCache.GetDef<PassiveModifierAbilityDef>("Acheron_Tributary_AbilityDef");
                ApplyStatusAbilityDef coDeliriumAbility = DefCache.GetDef<ApplyStatusAbilityDef>("Acheron_CoCorruption_AbilityDef");

                List<TacticalAbilityDef> acheronBasicAbilities = acheron.Data.Abilites.ToList();
                acheronBasicAbilities.Add(harbinger);
                acheron.Data.Abilites = acheronBasicAbilities.ToArray();

                List<TacticalAbilityDef> acheronPrimeBasicAbilities = acheronPrime.Data.Abilites.ToList();
                acheronPrimeBasicAbilities.Add(harbinger);
                acheronPrimeBasicAbilities.Add(coDeliriumAbility);
                acheronPrime.Data.Abilites = acheronPrimeBasicAbilities.ToArray();

                /* List<TacticalAbilityDef> acheronAsclepiusChampionBasicAbilities = acheronAsclepiusChampion.Data.Abilites.ToList();
                 acheronAsclepiusChampionBasicAbilities.Add(harbinger);
                 acheronAsclepiusChampion.Data.Abilites = acheronAsclepiusChampionBasicAbilities.ToArray();*/

                List<TacticalAbilityDef> acheronAchlysChampionBasicAbilities = acheronAchlysChampion.Data.Abilites.ToList();
                acheronAchlysChampionBasicAbilities.Add(tributary);
                acheronAchlysChampion.Data.Abilites = acheronAchlysChampionBasicAbilities.ToArray();

                //Removes leap from all Acherons
                /*
                DefCache.GetDef<TacticalItemDef>("Acheron_RearRightLeg_BodyPartDef").Abilities = new AbilityDef[] { };
                DefCache.GetDef<TacticalItemDef>("AcheronAchlys_RearRightLeg_BodyPartDef").Abilities = new AbilityDef[] { };
                DefCache.GetDef<TacticalItemDef>("AcheronAchlysChampion_RearRightLeg_BodyPartDef").Abilities = new AbilityDef[] { };
                DefCache.GetDef<TacticalItemDef>("AcheronAsclepius_RearRightLeg_BodyPartDef").Abilities = new AbilityDef[] { };
                DefCache.GetDef<TacticalItemDef>("AcheronAsclepiusChampion_RearRightLeg_BodyPartDef").Abilities = new AbilityDef[] { };
                DefCache.GetDef<TacticalItemDef>("AcheronPrime_RearRightLeg_BodyPartDef").Abilities = new AbilityDef[] { };*/

                //Removing reinforcements from Acheron, Acheron Prime, Acheron Achlys and Acheron Achlys Champion
                DefCache.GetDef<TacticalItemDef>("Acheron_Head_BodyPartDef").Abilities = new AbilityDef[] { };
                DefCache.GetDef<TacticalItemDef>("AcheronPrime_Head_BodyPartDef").Abilities = new AbilityDef[] { };
                DefCache.GetDef<TacticalItemDef>("AcheronAchlys_Head_BodyPartDef").Abilities = new AbilityDef[] { };
                DefCache.GetDef<TacticalItemDef>("AcheronAchlysChampion_Head_BodyPartDef").Abilities = new AbilityDef[] { };

                //Limiting Delirium cloud to one use per turn
                ApplyDamageEffectAbilityDef deliriumCloud = DefCache.GetDef<ApplyDamageEffectAbilityDef>("Acheron_CorruptionCloud_AbilityDef");
                ApplyStatusAbilityDef pepperCloud = DefCache.GetDef<ApplyStatusAbilityDef>("Acheron_PepperCloud_ApplyStatusAbilityDef");
                deliriumCloud.UsesPerTurn = 1;
                ApplyEffectAbilityDef confusionCloud = DefCache.GetDef<ApplyEffectAbilityDef>("Acheron_ParalyticCloud_AbilityDef");
                ResurrectAbilityDef resurrectAbility = DefCache.GetDef<ResurrectAbilityDef>("Acheron_ResurrectAbilityDef");

                //Adding Delirium Cloud ability to Acheron Prime
                DefCache.GetDef<TacticalItemDef>("AcheronPrime_Husk_BodyPartDef").Abilities = new AbilityDef[] { deliriumCloud, pepperCloud };

                ApplyDamageEffectAbilityDef corrosiveCloud = DefCache.GetDef<ApplyDamageEffectAbilityDef>("Acheron_CorrosiveCloud_AbilityDef");

                //Removes CorrosiveCloud from AchlysChampion
                TacticalItemDef achlysChampionHusk = DefCache.GetDef<TacticalItemDef>("AcheronAchlysChampion_Husk_BodyPartDef");
                List<AbilityDef> achlysChampionHuskAbilities = achlysChampionHusk.Abilities.ToList();
                achlysChampionHuskAbilities.Remove(corrosiveCloud);
                achlysChampionHusk.Abilities = achlysChampionHuskAbilities.ToArray();

                //Remove Confusion cloud from Acheron Achlys
                TacticalItemDef achlysHusk = DefCache.GetDef<TacticalItemDef>("AcheronAchlys_Husk_BodyPartDef");
                List<AbilityDef> achlysHuskAbilities = achlysChampionHusk.Abilities.ToList();
                achlysHuskAbilities.Remove(confusionCloud);
                achlysChampionHusk.Abilities = achlysHuskAbilities.ToArray();

                //Adjust Acheron leap so it can only be used once per turn and doesn't cost any AP
                JetJumpAbilityDef acheronLeap = DefCache.GetDef<JetJumpAbilityDef>("Acheron_Leap_AbilityDef");
                acheronLeap.UsesPerTurn = 1;
                acheronLeap.ActionPointCost = 0;

                //Removing Resurrect and Delirium Clouds from Asclepius Husks

                TacticalItemDef asclepiusChampionHusk = DefCache.GetDef<TacticalItemDef>("AcheronAsclepiusChampion_Husk_BodyPartDef");
                List<AbilityDef> asclepiusChampionHuskAbilities = asclepiusChampionHusk.Abilities.ToList();
                asclepiusChampionHuskAbilities.Remove(resurrectAbility);
                asclepiusChampionHuskAbilities.Remove(deliriumCloud);
                asclepiusChampionHusk.Abilities = asclepiusChampionHuskAbilities.ToArray();


                TacticalItemDef asclepiusHusk = DefCache.GetDef<TacticalItemDef>("AcheronAsclepius_Husk_BodyPartDef");
                List<AbilityDef> asclepiusHuskAbilities = asclepiusHusk.Abilities.ToList();
                asclepiusHuskAbilities.Remove(resurrectAbility);
                asclepiusHuskAbilities.Remove(deliriumCloud);
                asclepiusHusk.Abilities = asclepiusHuskAbilities.ToArray();


                DamageKeywordDef poison = DefCache.GetDef<DamageKeywordDef>("Poisonous_DamageKeywordDataDef");
                DamageKeywordDef acid = DefCache.GetDef<DamageKeywordDef>("Acid_DamageKeywordDataDef");
                DamageKeywordDef standard = DefCache.GetDef<DamageKeywordDef>("Damage_DamageKeywordDataDef");

                DamageKeywordDef blast = DefCache.GetDef<DamageKeywordDef>("Blast_DamageKeywordDataDef");
                StandardDamageTypeEffectDef blastDamage = DefCache.GetDef<StandardDamageTypeEffectDef>("Blast_StandardDamageTypeEffectDef");

                WeaponDef spitArmsAcheronAchlysChampion = DefCache.GetDef<WeaponDef>("AcheronAchlysChampion_Arms_WeaponDef");
                spitArmsAcheronAchlysChampion.DamagePayload.DamageKeywords[1].DamageKeywordDef = blast;
                spitArmsAcheronAchlysChampion.DamagePayload.DamageKeywords[1].Value = 30;
                spitArmsAcheronAchlysChampion.DamagePayload.DamageKeywords.Add(new DamageKeywordPair
                {
                    DamageKeywordDef = poison,
                    Value = 30
                });
                spitArmsAcheronAchlysChampion.DamagePayload.DamageKeywords.Add(new DamageKeywordPair
                {
                    DamageKeywordDef = acid,
                    Value = 30
                });
                spitArmsAcheronAchlysChampion.DamagePayload.DamageType = blastDamage;
                spitArmsAcheronAchlysChampion.DamagePayload.AoeRadius = 2f;
                spitArmsAcheronAchlysChampion.DamagePayload.DamageDeliveryType = DamageDeliveryType.Cone;

                WeaponDef spitArmsAcheronAsclepiusChampion = DefCache.GetDef<WeaponDef>("AcheronAsclepiusChampion_Arms_WeaponDef");

                WeaponDef achlysArms = DefCache.GetDef<WeaponDef>("AcheronAchlys_Arms_WeaponDef");

                //   string guid = "2B294E66-1BE9-425B-B088-F5A9075167A6";
                WeaponDef neuroArmsCopy = new WeaponDef();//Repo.CreateDef<WeaponDef>(guid);
                ReflectionHelper.CopyFields(achlysArms, neuroArmsCopy);
                ReflectionHelper.CopyFields(spitArmsAcheronAchlysChampion, achlysArms);
                ReflectionHelper.CopyFields(neuroArmsCopy, spitArmsAcheronAsclepiusChampion);

                DamageKeywordDef mistDamageKeyword = DefCache.GetDef<DamageKeywordDef>("Mist_DamageKeywordEffectorDef");
                SpawnVoxelDamageTypeEffectDef mistDamageTypeEffect = DefCache.GetDef<SpawnVoxelDamageTypeEffectDef>("Mist_SpawnVoxelDamageTypeEffectDef");

                // Change_AcheronCorruptiveSpray();
                //Add acid and mist, increase range
                WeaponDef acheronArms = DefCache.GetDef<WeaponDef>("Acheron_Arms_WeaponDef");
                acheronArms.DamagePayload.DamageKeywords.Add(new DamageKeywordPair
                {
                    DamageKeywordDef = acid,
                    Value = 20
                });
                acheronArms.DamagePayload.DamageKeywords.Add(new DamageKeywordPair
                {
                    DamageKeywordDef = mistDamageKeyword,
                    Value = 1

                });

                acheronArms.DamagePayload.DamageType = mistDamageTypeEffect;
                acheronArms.DamagePayload.AoeRadius = 5;
                acheronArms.DamagePayload.Range = 30;
                acheronArms.DamagePayload.DamageDeliveryType = DamageDeliveryType.Cone;
                //   acheronArms.Abilities[0] = DefCache.GetDef<ShootAbilityDef>("MistLaunch_ShootAbilityDef"); 


                WeaponDef acheronPrimeArms = DefCache.GetDef<WeaponDef>("AcheronPrime_Arms_WeaponDef");
                acheronPrimeArms.DamagePayload.DamageKeywords.Add(new DamageKeywordPair
                {
                    DamageKeywordDef = acid,
                    Value = 30
                });
                acheronPrimeArms.DamagePayload.DamageKeywords.Add(new DamageKeywordPair
                {
                    DamageKeywordDef = mistDamageKeyword,
                    Value = 1

                });

                acheronPrimeArms.DamagePayload.DamageType = mistDamageTypeEffect;
                acheronPrimeArms.DamagePayload.AoeRadius = 5;
                acheronPrimeArms.DamagePayload.Range = 30;
                acheronPrimeArms.DamagePayload.DamageDeliveryType = DamageDeliveryType.Cone;

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }


        }

        public static void CreateUIDeliriumHint()
        {
            try
            {
                GeoTimeElapsedGeoHintConditionDef geoTimeElapsedGeoHintConditionDef = DefCache.GetDef<GeoTimeElapsedGeoHintConditionDef>("E_MinTimeElapsedForCustomizationHint [GeoscapeHintsManagerDef]");
                geoTimeElapsedGeoHintConditionDef.TimeRangeInDays.Min = 14.0f;

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }

        public static void CreateStaminaHint()
        {
            try
            {
                string name = "TFTV_StaminaHintDef";
                ContextHelpHintDef sourceHint = DefCache.GetDef<ContextHelpHintDef>("TUT4_BodyPartDisabled_HintDef");
                ContextHelpHintDef staminaHint = Helper.CreateDefFromClone(sourceHint, "DE4949BA-D178-4036-9827-00A0E1C9BE5E", name);

                staminaHint.IsTutorialHint = false;
                HasSeenHintHintConditionDef sourceHasSeenHintConditionDef = DefCache.GetDef<HasSeenHintHintConditionDef>("HasSeenHint_TUT2_Overwatch_HintDef-False_HintConditionDef");

                HasSeenHintHintConditionDef newHasSeenHintConditionDef = Helper.CreateDefFromClone(sourceHasSeenHintConditionDef, "DC1E6A07-F1DA-47F4-875B-CA18144F56C4", name + "HasSeenHintConditionDef");
                newHasSeenHintConditionDef.HintDef = staminaHint;
                staminaHint.Conditions[1] = newHasSeenHintConditionDef;
                staminaHint.AnyCondition = false;
                staminaHint.Text.LocalizationKey = "TFTV_STAMINAHINT_TEXT";
                staminaHint.Title.LocalizationKey = "TFTV_STAMINAHINT_TITLE";

                ActorHasTagHintConditionDef sourceActorHasTagHintConditionDef = DefCache.GetDef<ActorHasTagHintConditionDef>("ActorHasTag_Takeshi_Tutorial3_GameTagDef_HintConditionDef");

                ActorHasTagHintConditionDef newActorHasTemplateHintConditionDef = Helper.CreateDefFromClone(sourceActorHasTagHintConditionDef, "3DC53C38-BB43-4F2B-9165-475F7CE2D237", "ActorHasTag_" + name + "_HintConditionDef");
                GameTagDef gameTagDef = DefCache.GetDef<GameTagDef>("PhoenixPoint_UniformTagDef");
                newActorHasTemplateHintConditionDef.GameTagDef = gameTagDef;
                staminaHint.Conditions.Add(newActorHasTemplateHintConditionDef);

                ContextHelpHintDbDef alwaysDisplayedTacticalHintsDbDef = DefCache.GetDef<ContextHelpHintDbDef>("AlwaysDisplayedTacticalHintsDbDef");
                alwaysDisplayedTacticalHintsDbDef.Hints.Add(staminaHint);

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }
        public static void ChangeInfestationDefs()
        {
            try
            {
                AlienRaidsSetupDef raidsSetup = DefCache.GetDef<AlienRaidsSetupDef>("_AlienRaidsSetupDef");
                raidsSetup.RaidBands[0].RollResultMax = 60;
                raidsSetup.RaidBands[1].RollResultMax = 80;
                raidsSetup.RaidBands[2].RollResultMax = 100;
                raidsSetup.RaidBands[3].RollResultMax = 130;
                raidsSetup.RaidBands[4].RollResultMax = 9999;
                raidsSetup.RaidBands[4].AircraftTypesAllowed = 0;

                CustomMissionTypeDef Anu_Infestation = DefCache.GetDef<CustomMissionTypeDef>("HavenInfestationAN_CustomMissionTypeDef");
                CustomMissionTypeDef NewJericho_Infestation = DefCache.GetDef<CustomMissionTypeDef>("HavenInfestationSY_CustomMissionTypeDef");
                CustomMissionTypeDef Synderion_Infestation = DefCache.GetDef<CustomMissionTypeDef>("HavenInfestationNJ_CustomMissionTypeDef");

                ResourceMissionOutcomeDef sourceMissonResourceReward = DefCache.GetDef<ResourceMissionOutcomeDef>("HavenDefAN_ResourceMissionOutcomeDef");
                ResourceMissionOutcomeDef mutagenRewardInfestation = Helper.CreateDefFromClone(sourceMissonResourceReward, "2E579AB8-3744-4994-8036-B5018B5E2E15", "InfestationReward");
                mutagenRewardInfestation.Resources.Values.Clear();
                mutagenRewardInfestation.Resources.Values.Add(new ResourceUnit { Type = PhoenixPoint.Common.Core.ResourceType.Mutagen, Value = 800 });

                foreach (CustomMissionTypeDef missionTypeDef in Repo.GetAllDefs<CustomMissionTypeDef>())
                {
                    if (missionTypeDef.name.Contains("Haven") && missionTypeDef.name.Contains("Infestation"))
                    {
                        missionTypeDef.Outcomes[0].DestroySite = true;
                        missionTypeDef.Outcomes[0].Outcomes[2] = mutagenRewardInfestation;
                        missionTypeDef.Outcomes[0].BriefingModalBind.Title.LocalizationKey = "KEY_MISSION_HAVEN_INFESTED_VICTORY_NAME";
                        missionTypeDef.Outcomes[0].BriefingModalBind.Description.LocalizationKey = "KEY_MISSION_HAVEN_INFESTED_VICTORY_DESCRIPTION";
                        missionTypeDef.BriefingModalBind.Title.LocalizationKey = "KEY_MISSION_HAVEN_INFESTED_NAME";
                        missionTypeDef.BriefingModalBind.Description.LocalizationKey = "KEY_MISSION_HAVEN_INFESTED_DESCRIPTION";
                    }
                }

                //  GeoscapeEventDef rewardEvent = TFTVCommonMethods.CreateNewEvent("InfestationReward", "KEY_INFESTATION_REWARD_TITLE", "KEY_INFESTATION_REWARD_DESCRIPTION", null);
                GeoscapeEventDef lW1MissWin = DefCache.GetDef<GeoscapeEventDef>("PROG_LW1_WIN_GeoscapeEventDef");
                lW1MissWin.GeoscapeEventData.Choices[0].Outcome.SetEvents.Clear();
                lW1MissWin.GeoscapeEventData.Choices[0].Outcome.TrackEncounters.Clear();
                lW1MissWin.GeoscapeEventData.Choices[0].Outcome.UntrackEncounters.Clear();
                GeoscapeEventDef lW2MissWin = DefCache.GetDef<GeoscapeEventDef>("PROG_LW2_WIN_GeoscapeEventDef");
                lW2MissWin.GeoscapeEventData.Choices[0].Outcome.SetEvents.Clear();
                lW2MissWin.GeoscapeEventData.Choices[0].Outcome.TrackEncounters.Clear();
                lW2MissWin.GeoscapeEventData.Choices[0].Outcome.UntrackEncounters.Clear();
                GeoscapeEventDef lW3MissWin = DefCache.GetDef<GeoscapeEventDef>("PROG_LW3_WIN_GeoscapeEventDef");
                lW2MissWin.GeoscapeEventData.Choices[0].Outcome.SetEvents.Clear();
                lW2MissWin.GeoscapeEventData.Choices[0].Outcome.TrackEncounters.Clear();
                lW2MissWin.GeoscapeEventData.Choices[0].Outcome.UntrackEncounters.Clear();
                //Muting Living Weapons
                GeoscapeEventDef lwstartingEvent = DefCache.GetDef<GeoscapeEventDef>("PROG_LW1_GeoscapeEventDef");
                lwstartingEvent.GeoscapeEventData.Mute = true;
                DefCache.GetDef<KillActorFactionObjectiveDef>("KillCorruptionNode_CustomMissionObjective").MissionObjectiveData.ExperienceReward = 1000;


                //

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);

            }
        }
        public static void ModifyMissionDefsToReplaceNeutralWithBandit()
        {
            try
            {
                PPFactionDef banditFaction = DefCache.GetDef<PPFactionDef>("NEU_Bandits_FactionDef");

                foreach (CustomMissionTypeDef missionTypeDef in Repo.GetAllDefs<CustomMissionTypeDef>())
                {
                    // TFTVLogger.Always("The first foreach went ok");


                    foreach (MutualParticipantsRelations relations in missionTypeDef.ParticipantsRelations)
                    {
                        // TFTVLogger.Always("The second foreach went ok");
                        if (relations.FirstParticipant == TacMissionParticipant.Player && relations.MutualRelation == FactionRelation.Enemy)
                        {
                            //   TFTVLogger.Always("The if inside the second foreach went ok");

                            if (missionTypeDef.ParticipantsData != null)
                            {
                                foreach (TacMissionTypeParticipantData data in missionTypeDef.ParticipantsData)
                                {
                                    //TFTVLogger.Always("The third foreach went Ok");

                                    if (data.ParticipantKind == relations.SecondParticipant)
                                    {
                                        // TFTVLogger.Always("The if inside the third foreach went ok");
                                        if (data.FactionDef != null)
                                        {
                                            if (missionTypeDef.name == "StoryAN1_CustomMissionTypeDef" ||
                                                missionTypeDef.name == "StoryNJ_Chain1_CustomMissionTypeDef" ||
                                                missionTypeDef.name == "StoryPX13_CustomMissionTypeDef" ||
                                                missionTypeDef.name == "StorySYN0_CustomMissionTypeDef" ||
                                                missionTypeDef.name == "StorySYN4_CustomMissionTypeDef")
                                            {
                                                data.FactionDef = banditFaction;
                                                TFTVLogger.Always("In mission " + missionTypeDef.name + " the enemy faction is " + data.FactionDef.name);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }
        public static void ChangesAmbushMissions()
        {
            try
            {
                //Changing ambush missions so that all of them have crates
                CustomMissionTypeDef AmbushALN = DefCache.GetDef<CustomMissionTypeDef>("AmbushAlien_CustomMissionTypeDef");
                CustomMissionTypeDef SourceScavCratesALN = DefCache.GetDef<CustomMissionTypeDef>("ScavCratesALN_CustomMissionTypeDef");
                var pickResourceCratesObjective = SourceScavCratesALN.CustomObjectives[2];
                AmbushALN.ParticipantsData[0].ReinforcementsTurns.Max = 2;
                AmbushALN.ParticipantsData[0].ReinforcementsTurns.Min = 2;
                AmbushALN.CratesDeploymentPointsRange = SourceScavCratesALN.CratesDeploymentPointsRange;
                AmbushALN.MissionSpecificCrates = SourceScavCratesALN.MissionSpecificCrates;
                AmbushALN.FactionItemsRange = SourceScavCratesALN.FactionItemsRange;
                AmbushALN.CratesDeploymentPointsRange.Min = 30;
                AmbushALN.CratesDeploymentPointsRange.Max = 50;
                AmbushALN.CustomObjectives[2] = pickResourceCratesObjective;

                CustomMissionTypeDef AmbushAN = DefCache.GetDef<CustomMissionTypeDef>("AmbushAN_CustomMissionTypeDef");
                AmbushAN.ParticipantsData[0].ReinforcementsTurns.Max = 2;
                AmbushAN.ParticipantsData[0].ReinforcementsTurns.Min = 2;
                AmbushAN.CratesDeploymentPointsRange = SourceScavCratesALN.CratesDeploymentPointsRange;
                AmbushAN.MissionSpecificCrates = SourceScavCratesALN.MissionSpecificCrates;
                AmbushAN.FactionItemsRange = SourceScavCratesALN.FactionItemsRange;
                AmbushAN.CratesDeploymentPointsRange.Min = 30;
                AmbushAN.CratesDeploymentPointsRange.Max = 50;
                AmbushAN.CustomObjectives[2] = pickResourceCratesObjective;

                CustomMissionTypeDef AmbushBandits = DefCache.GetDef<CustomMissionTypeDef>("AmbushBandits_CustomMissionTypeDef");
                AmbushBandits.ParticipantsData[0].ReinforcementsTurns.Max = 2;
                AmbushBandits.ParticipantsData[0].ReinforcementsTurns.Min = 2;
                AmbushBandits.CratesDeploymentPointsRange = SourceScavCratesALN.CratesDeploymentPointsRange;
                AmbushBandits.MissionSpecificCrates = SourceScavCratesALN.MissionSpecificCrates;
                AmbushBandits.FactionItemsRange = SourceScavCratesALN.FactionItemsRange;
                AmbushBandits.CratesDeploymentPointsRange.Min = 30;
                AmbushBandits.CratesDeploymentPointsRange.Max = 50;
                AmbushBandits.CustomObjectives[2] = pickResourceCratesObjective;

                CustomMissionTypeDef AmbushFallen = DefCache.GetDef<CustomMissionTypeDef>("AmbushFallen_CustomMissionTypeDef");
                AmbushFallen.ParticipantsData[0].ReinforcementsTurns.Max = 2;
                AmbushFallen.ParticipantsData[0].ReinforcementsTurns.Min = 2;
                AmbushFallen.CratesDeploymentPointsRange = SourceScavCratesALN.CratesDeploymentPointsRange;
                AmbushFallen.MissionSpecificCrates = SourceScavCratesALN.MissionSpecificCrates;
                AmbushFallen.FactionItemsRange = SourceScavCratesALN.FactionItemsRange;
                AmbushFallen.CratesDeploymentPointsRange.Min = 30;
                AmbushFallen.CratesDeploymentPointsRange.Max = 50;
                AmbushFallen.CustomObjectives[2] = pickResourceCratesObjective;

                CustomMissionTypeDef AmbushNJ = DefCache.GetDef<CustomMissionTypeDef>("AmbushNJ_CustomMissionTypeDef");
                AmbushNJ.ParticipantsData[0].ReinforcementsTurns.Max = 2;
                AmbushNJ.ParticipantsData[0].ReinforcementsTurns.Min = 2;
                AmbushNJ.CratesDeploymentPointsRange = SourceScavCratesALN.CratesDeploymentPointsRange;
                AmbushNJ.MissionSpecificCrates = SourceScavCratesALN.MissionSpecificCrates;
                AmbushNJ.FactionItemsRange = SourceScavCratesALN.FactionItemsRange;
                AmbushNJ.CratesDeploymentPointsRange.Min = 30;
                AmbushNJ.CratesDeploymentPointsRange.Max = 50;
                AmbushNJ.CustomObjectives[2] = pickResourceCratesObjective;

                CustomMissionTypeDef AmbushPure = DefCache.GetDef<CustomMissionTypeDef>("AmbushPure_CustomMissionTypeDef");
                AmbushPure.ParticipantsData[0].ReinforcementsTurns.Max = 2;
                AmbushPure.ParticipantsData[0].ReinforcementsTurns.Min = 2;
                AmbushPure.CratesDeploymentPointsRange = SourceScavCratesALN.CratesDeploymentPointsRange;
                AmbushPure.MissionSpecificCrates = SourceScavCratesALN.MissionSpecificCrates;
                AmbushPure.FactionItemsRange = SourceScavCratesALN.FactionItemsRange;
                AmbushPure.CratesDeploymentPointsRange.Min = 30;
                AmbushPure.CratesDeploymentPointsRange.Max = 50;
                AmbushPure.CustomObjectives[2] = pickResourceCratesObjective;

                CustomMissionTypeDef AmbushSY = DefCache.GetDef<CustomMissionTypeDef>("AmbushSY_CustomMissionTypeDef");
                AmbushSY.ParticipantsData[0].ReinforcementsTurns.Max = 2;
                AmbushSY.ParticipantsData[0].ReinforcementsTurns.Min = 2;
                AmbushSY.CratesDeploymentPointsRange = SourceScavCratesALN.CratesDeploymentPointsRange;
                AmbushSY.MissionSpecificCrates = SourceScavCratesALN.MissionSpecificCrates;
                AmbushSY.FactionItemsRange = SourceScavCratesALN.FactionItemsRange;
                AmbushSY.CratesDeploymentPointsRange.Min = 30;
                AmbushSY.CratesDeploymentPointsRange.Max = 50;
                AmbushSY.CustomObjectives[2] = pickResourceCratesObjective;

                //Reduce XP for Ambush mission
                SurviveTurnsFactionObjectiveDef surviveAmbush_CustomMissionObjective = DefCache.GetDef<SurviveTurnsFactionObjectiveDef>("SurviveAmbush_CustomMissionObjective");
                surviveAmbush_CustomMissionObjective.MissionObjectiveData.ExperienceReward = 100;

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }
        public static void ModifyDefsForPassengerModules()
        {

            try
            {
                //ID all the factions for later
                GeoFactionDef PhoenixPoint = DefCache.GetDef<GeoFactionDef>("Phoenix_GeoPhoenixFactionDef");
                GeoFactionDef NewJericho = DefCache.GetDef<GeoFactionDef>("NewJericho_GeoFactionDef");
                GeoFactionDef Anu = DefCache.GetDef<GeoFactionDef>("Anu_GeoFactionDef");
                GeoFactionDef Synedrion = DefCache.GetDef<GeoFactionDef>("Synedrion_GeoFactionDef");

                //ID all craft for later
                GeoVehicleDef manticore = DefCache.GetDef<GeoVehicleDef>("PP_Manticore_Def");
                GeoVehicleDef helios = DefCache.GetDef<GeoVehicleDef>("SYN_Helios_Def");
                GeoVehicleDef thunderbird = DefCache.GetDef<GeoVehicleDef>("NJ_Thunderbird_Def");
                GeoVehicleDef blimp = DefCache.GetDef<GeoVehicleDef>("ANU_Blimp_Def");
                GeoVehicleDef manticoreMasked = DefCache.GetDef<GeoVehicleDef>("PP_MaskedManticore_Def");

                //Reduce all craft seating (except blimp) by 4 and create clones with previous seating

                GeoVehicleDef manticoreNew = Helper.CreateDefFromClone(manticore, "83A7FD03-DB85-4CEE-BAED-251F5415B82B", "PP_Manticore_Def_6_Slots");
                manticore.BaseStats.SpaceForUnits = 2;
                GeoVehicleDef heliosNew = Helper.CreateDefFromClone(helios, "4F9026CB-EF42-44B8-B9C3-21181EC4E2AB", "SYN_Helios_Def_5_Slots");
                helios.BaseStats.SpaceForUnits = 1;
                GeoVehicleDef thunderbirdNew = Helper.CreateDefFromClone(thunderbird, "FDE7F0C2-8BA7-4046-92EB-F3462F204B2B", "NJ_Thunderbird_Def_7_Slots");
                thunderbird.BaseStats.SpaceForUnits = 3;
                GeoVehicleDef blimpNew = Helper.CreateDefFromClone(blimp, "B857B76D-BDDB-4CA9-A1CA-895A540B17C8", "ANU_Blimp_Def_12_Slots");
                blimpNew.BaseStats.SpaceForUnits = 12;
                GeoVehicleDef manticoreMaskedNew = Helper.CreateDefFromClone(manticoreMasked, "19B82FD8-67EE-4277-B982-F352A53ADE72", "PP_ManticoreMasked_Def_8_Slots");
                manticoreMasked.BaseStats.SpaceForUnits = 4;

                //Change Hibernation module
                GeoVehicleModuleDef hibernationmodule = DefCache.GetDef<GeoVehicleModuleDef>("SY_HibernationPods_GeoVehicleModuleDef");
                //Increase cost to 50% of Vanilla Manti
                hibernationmodule.ManufactureMaterials = 600;
                hibernationmodule.ManufactureTech = 75;
                hibernationmodule.ManufacturePointsCost = 505;
                //Change Cruise Control module
                GeoVehicleModuleDef cruisecontrolmodule = DefCache.GetDef<GeoVehicleModuleDef>("NJ_CruiseControl_GeoVehicleModuleDef");
                //Increase cost to 50% of Vanilla Manti
                cruisecontrolmodule.ManufactureMaterials = 600;
                cruisecontrolmodule.ManufactureTech = 75;
                cruisecontrolmodule.ManufacturePointsCost = 505;
                //Change Fuel Tank module
                GeoVehicleModuleDef fueltankmodule = DefCache.GetDef<GeoVehicleModuleDef>("NJ_FuelTanks_GeoVehicleModuleDef");
                //Increase cost to 50% of Vanilla Manti
                fueltankmodule.ManufactureMaterials = 600;
                fueltankmodule.ManufactureTech = 75;
                fueltankmodule.ManufacturePointsCost = 505;


                //Make Hibernation module available for manufacture from start of game - doesn't work because HM is not an ItemDef
                //GeoPhoenixFactionDef phoenixFactionDef = DefCache.GetDef<GeoPhoenixFactionDef>("Phoenix_GeoPhoenixFactionDef");
                //EntitlementDef festeringSkiesEntitlementDef = DefCache.GetDef<EntitlementDef>("FesteringSkiesEntitlementDef");
                // phoenixFactionDef.AdditionalDLCItems.Add(new GeoFactionDef.DLCStartItems { DLC = festeringSkiesEntitlementDef, StartingManufacturableItems = hibernationmodule };               
                //Change cost of Manti to 50% of Vanilla
                VehicleItemDef mantiVehicle = DefCache.GetDef<VehicleItemDef>("PP_Manticore_VehicleItemDef");
                mantiVehicle.ManufactureMaterials = 600;
                mantiVehicle.ManufactureTech = 75;
                mantiVehicle.ManufacturePointsCost = 505;
                //Change cost of Helios to Vanilla minus cost of passenger module
                VehicleItemDef heliosVehicle = DefCache.GetDef<VehicleItemDef>("SYN_Helios_VehicleItemDef");
                heliosVehicle.ManufactureMaterials = 555;
                heliosVehicle.ManufactureTech = 173;
                heliosVehicle.ManufacturePointsCost = 510;
                //Change cost of Thunderbird to Vanilla minus cost of passenger module
                VehicleItemDef thunderbirdVehicle = DefCache.GetDef<VehicleItemDef>("NJ_Thunderbird_VehicleItemDef");
                thunderbirdVehicle.ManufactureMaterials = 900;
                thunderbirdVehicle.ManufactureTech = 113;
                thunderbirdVehicle.ManufacturePointsCost = 660;

                //Make HM research for PX, available after completing Phoenix Archives
                ResearchDef hibernationModuleResearch = DefCache.GetDef<ResearchDef>("SYN_Aircraft_HybernationPods_ResearchDef");
                ResearchDef sourcePX_SDI_ResearchDef = DefCache.GetDef<ResearchDef>("PX_SDI_ResearchDef");
                hibernationModuleResearch.Faction = PhoenixPoint;
                hibernationModuleResearch.RevealRequirements = sourcePX_SDI_ResearchDef.RevealRequirements;
                hibernationModuleResearch.ResearchCost = 100;


            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }
        public static void ModifyPandoranProgress()
        {

            try
            {

                // All sources of evolution due to scaling removed, leaving only evolution per day
                // Additional source of evolution will be number of surviving Pandoran colonies, modulated by difficulty level
                GameDifficultyLevelDef veryhard = DefCache.GetDef<GameDifficultyLevelDef>("VeryHard_GameDifficultyLevelDef");
                //Hero
                GameDifficultyLevelDef hard = DefCache.GetDef<GameDifficultyLevelDef>("Hard_GameDifficultyLevelDef");
                //Standard
                GameDifficultyLevelDef standard = DefCache.GetDef<GameDifficultyLevelDef>("Standard_GameDifficultyLevelDef");
                //Easy
                GameDifficultyLevelDef easy = DefCache.GetDef<GameDifficultyLevelDef>("Easy_GameDifficultyLevelDef");

                veryhard.NestLimitations.MaxNumber = 3; //vanilla 6
                veryhard.NestLimitations.HoursBuildTime = 90; //vanilla 45
                veryhard.LairLimitations.MaxNumber = 3; // vanilla 5
                veryhard.LairLimitations.MaxConcurrent = 3; //vanilla 4
                veryhard.LairLimitations.HoursBuildTime = 100; //vanilla 50
                veryhard.CitadelLimitations.HoursBuildTime = 180; //vanilla 60
                veryhard.EvolutionPointsGainOnMissionLoss = 0; //vanilla 10
                veryhard.AlienBaseTypeEvolutionParams[0].EvolutionPerDestroyedBase = 0; //vanilla 10
                veryhard.AlienBaseTypeEvolutionParams[1].EvolutionPerDestroyedBase = 0; //vanilla 20
                veryhard.AlienBaseTypeEvolutionParams[2].EvolutionPerDestroyedBase = 0; //vanilla 40
                veryhard.ApplyInfestationOutcomeChange = 0;
                veryhard.ApplyDamageHavenOutcomeChange = 0;
                veryhard.StartingSquadTemplate[0] = hard.TutorialStartingSquadTemplate[1];
                veryhard.StartingSquadTemplate[1] = hard.TutorialStartingSquadTemplate[2];

                //Making HPC residual
                veryhard.MinPopulationThreshold = 3;
                hard.MinPopulationThreshold = 3;
                standard.MinPopulationThreshold = 3;
                easy.MinPopulationThreshold = 3;





                // PX_Jacob_Tutorial2_TacCharacterDef replace [3], with hard starting squad [1]
                // PX_Sophia_Tutorial2_TacCharacterDef replace [1], with hard starting squad [2]

                //reducing evolution per day because there other sources of evolution points now
                veryhard.EvolutionProgressPerDay = 70; //vanilla 100



                hard.NestLimitations.MaxNumber = 3; //vanilla 5
                hard.NestLimitations.HoursBuildTime = 90; //vanilla 50
                hard.LairLimitations.MaxNumber = 3; // vanilla 4
                hard.LairLimitations.MaxConcurrent = 3; //vanilla 3
                hard.LairLimitations.HoursBuildTime = 100; //vanilla 80
                hard.CitadelLimitations.HoursBuildTime = 180; //vanilla 100
                hard.EvolutionPointsGainOnMissionLoss = 0; //vanilla 10
                hard.AlienBaseTypeEvolutionParams[0].EvolutionPerDestroyedBase = 0; //vanilla 10
                hard.AlienBaseTypeEvolutionParams[1].EvolutionPerDestroyedBase = 0; //vanilla 20
                hard.AlienBaseTypeEvolutionParams[2].EvolutionPerDestroyedBase = 0; //vanilla 40
                hard.ApplyInfestationOutcomeChange = 0;
                hard.ApplyDamageHavenOutcomeChange = 0;
                hard.StartingSquadTemplate[1] = hard.TutorialStartingSquadTemplate[1];
                hard.StartingSquadTemplate[3] = hard.TutorialStartingSquadTemplate[2];


                //reducing evolution per day because there other sources of evolution points now
                hard.EvolutionProgressPerDay = 50; //vanilla 70; moved from 60 in Update#6


                standard.NestLimitations.MaxNumber = 3; //vanilla 4
                standard.NestLimitations.HoursBuildTime = 90; //vanilla 55
                standard.LairLimitations.MaxNumber = 3; // vanilla 3
                standard.LairLimitations.MaxConcurrent = 3; //vanilla 3
                standard.LairLimitations.HoursBuildTime = 100; //vanilla 120
                standard.CitadelLimitations.HoursBuildTime = 180; //vanilla 145
                standard.EvolutionPointsGainOnMissionLoss = 0; //vanilla 10
                standard.AlienBaseTypeEvolutionParams[0].EvolutionPerDestroyedBase = 0; //vanilla 10
                standard.AlienBaseTypeEvolutionParams[1].EvolutionPerDestroyedBase = 0; //vanilla 20
                standard.AlienBaseTypeEvolutionParams[2].EvolutionPerDestroyedBase = 0; //vanilla 40
                standard.ApplyDamageHavenOutcomeChange = 0;
                standard.ApplyInfestationOutcomeChange = 0;
                standard.StartingSquadTemplate[1] = hard.TutorialStartingSquadTemplate[1];
                standard.StartingSquadTemplate[3] = hard.TutorialStartingSquadTemplate[2];

                //reducing evolution per day because there other sources of evolution points now
                standard.EvolutionProgressPerDay = 40; //vanilla 55


                easy.NestLimitations.HoursBuildTime = 90; //vanilla 60 
                easy.LairLimitations.HoursBuildTime = 100; // vanilla 150
                easy.CitadelLimitations.HoursBuildTime = 180; // vanilla 180
                easy.EvolutionPointsGainOnMissionLoss = 0; //vanilla 10
                easy.AlienBaseTypeEvolutionParams[0].EvolutionPerDestroyedBase = 0; //vanilla 10
                easy.AlienBaseTypeEvolutionParams[1].EvolutionPerDestroyedBase = 0; //vanilla 20
                easy.AlienBaseTypeEvolutionParams[2].EvolutionPerDestroyedBase = 0; //vanilla 40
                easy.ApplyInfestationOutcomeChange = 0;
                easy.ApplyDamageHavenOutcomeChange = 0;
                easy.StartingSquadTemplate[1] = hard.TutorialStartingSquadTemplate[1];
                easy.StartingSquadTemplate[3] = hard.TutorialStartingSquadTemplate[2];

                //keeping evolution per day because low enough already
                easy.EvolutionProgressPerDay = 35; //vanilla 35

                //Remove faction diplo penalties for not destroying revealed PCs and increase rewards for haven leader
                GeoAlienBaseTypeDef nestType = DefCache.GetDef<GeoAlienBaseTypeDef>("Nest_GeoAlienBaseTypeDef");
                GeoAlienBaseTypeDef lairType = DefCache.GetDef<GeoAlienBaseTypeDef>("Lair_GeoAlienBaseTypeDef");
                GeoAlienBaseTypeDef citadelType = DefCache.GetDef<GeoAlienBaseTypeDef>("Citadel_GeoAlienBaseTypeDef");
                GeoAlienBaseTypeDef palaceType = DefCache.GetDef<GeoAlienBaseTypeDef>("Palace_GeoAlienBaseTypeDef");

                nestType.FactionDiplomacyPenaltyPerHaven = 0; //vanilla -1
                nestType.HavenLeaderDiplomacyReward = 12; //vanilla 8 
                lairType.FactionDiplomacyPenaltyPerHaven = 0; //vanilla -1
                lairType.HavenLeaderDiplomacyReward = 16; //vanilla 12 
                citadelType.FactionDiplomacyPenaltyPerHaven = 0; //vanilla -1
                citadelType.HavenLeaderDiplomacyReward = 20; //vanilla 16 

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void InjectAlistairAhsbyLines()
        {
            try
            {
                //Alistair speaks about Symes after completing Symes Retreat
                GeoscapeEventDef alistairOnSymes1 = TFTVCommonMethods.CreateNewEvent("AlistairOnSymes1", "PROG_PX10_WIN_TITLE", "KEY_ALISTAIRONSYMES_1_DESCRIPTION", null);
                alistairOnSymes1.GeoscapeEventData.Flavour = "IntroducingSymes";

                //Alistair speaks about Barnabas after Barnabas asks for help
                GeoscapeEventDef alistairOnBarnabas = TFTVCommonMethods.CreateNewEvent("AlistairOnBarnabas", "PROG_CH0_TITLE", "KEY_ALISTAIRONBARNABAS_DESCRIPTION", null);
                alistairOnBarnabas.GeoscapeEventData.Flavour = "DLC4_Generic_NJ";

                //Alistair speaks about Symes after Antarctica discovery
                GeoscapeEventDef alistairOnSymes2 = TFTVCommonMethods.CreateNewEvent("AlistairOnSymes2", "PROG_PX1_WIN_TITLE", "KEY_ALISTAIRONSYMES_2_DESCRIPTION", null);
                alistairOnSymes2.GeoscapeEventData.Flavour = "AntarcticSite_Victory";

                AlistairRoadsEvent();
                CreateEventMessagesFromTheVoid();
                CreateBehemothPattern();
                CreateTrappedInMist();

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }

        public static void InjectOlenaKimLines()

        {
            try
            {
                //Helena reveal about Olena
                GeoscapeEventDef helenaOnOlena = TFTVCommonMethods.CreateNewEvent("HelenaOnOlena", "PROG_LE0_WIN_TITLE", "KEY_OLENA_HELENA_DESCRIPTION", null);
                //Olena about West
                GeoscapeEventDef olenaOnWest = TFTVCommonMethods.CreateNewEvent("OlenaOnWest", "PROG_NJ1_WIN_TITLE", "KEY_OLENAONWEST_DESCRIPTION", null);
                //Olena about Synod
                GeoscapeEventDef olenaOnSynod = TFTVCommonMethods.CreateNewEvent("OlenaOnSynod", "PROG_AN6_WIN2_TITLE", "KEY_OLENAONSYNOD_DESCRIPTION", null);
                //Olena about the Ancients
                GeoscapeEventDef olenaOnAncients = TFTVCommonMethods.CreateNewEvent("OlenaOnAncients", "KEY_OLENAONANCIENTS_TITLE", "KEY_OLENAONANCIENTS_DESCRIPTION", null);
                //Olena about the Behemeoth
                GeoscapeEventDef olenaOnBehemoth = TFTVCommonMethods.CreateNewEvent("OlenaOnBehemoth", "PROG_FS1_WIN_TITLE", "KEY_OLENAONBEHEMOTH_DESCRIPTION", null);
                //Olena about Alistair - missing an event hook!!
                GeoscapeEventDef olenaOnAlistair = TFTVCommonMethods.CreateNewEvent("OlenaOnAlistair", "", "KEY_OLENAONALISTAIR_DESCRIPTION", null);
                //Olena about Symes
                GeoscapeEventDef olenaOnSymes = TFTVCommonMethods.CreateNewEvent("OlenaOnSymes", "PROG_PX1_WIN_TITLE", "KEY_OLENAONSYMES_DESCRIPTION", null);
                //Olena about ending 
                GeoscapeEventDef olenaOnEnding = TFTVCommonMethods.CreateNewEvent("OlenaOnEnding", "KEY_ALISTAIR_ROADS_TITLE", "KEY_OLENAONENDING_DESCRIPTION", null);
                //Olena about Bionics Lab sabotage
                GeoscapeEventDef olenaOnBionicsLabSabotage = TFTVCommonMethods.CreateNewEvent("OlenaOnBionicsLabSabotage", "ANU_REALLY_PISSED_BIONICS_TITLE", "ANU_REALLY_PISSED_BIONICS_CHOICE_0_OUTCOME", null);
                //Olena about Mutations Lab sabotage
                GeoscapeEventDef olenaOnMutationsLabSabotage = TFTVCommonMethods.CreateNewEvent("OlenaOnMutationsLabSabotage", "NJ_REALLY_PISSED_MUTATIONS_TITLE", "NJ_REALLY_PISSED_MUTATIONS_CHOICE_0_OUTCOME", null);
                //Olena First LOTA Event 
                TFTVCommonMethods.CreateNewEvent("OlenaLotaStart", "TFTV_LOTA_START_EVENT_TITLE", "TFTV_LOTA_START_EVENT_DESCRIPTION", null);

                CreateEventFirstFlyer();
                CreateEventFirstHavenTarget();
                CreateEventFirstHavenAttack();

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void AlistairRoadsEvent()
        {
            try
            {
                string title = "KEY_ALISTAIR_ROADS_TITLE";
                string description = "KEY_ALISTAIR_ROADS_DESCRIPTION";
                string passToOlena = "OlenaOnEnding";

                string startingEvent = "AlistairRoads";
                string afterWest = "AlistairRoadsNoWest";
                string afterSynedrion = "AlistairRoadsNoSynedrion";
                string afterAnu = "AlistairRoadsNoAnu";
                string afterVirophage = "AlistairRoadsNoVirophage";

                string questionAboutWest = "KEY_ALISTAIRONWEST_CHOICE";
                string questionAboutSynedrion = "KEY_ALISTAIRONSYNEDRION_CHOICE";
                string questionAboutAnu = "KEY_ALISTAIRONANU_CHOICE";
                string questionAboutVirophage = "KEY_ALISTAIRONVIROPHAGE_CHOICE";
                //   string questionAboutHelena = "KEY_ALISTAIRONHELENA_CHOICE";
                string noMoreQuestions = "KEY_ALISTAIR_ROADS_ALLDONE";

                string answerAboutWest = "KEY_ALISTAIRONWEST_DESCRIPTION";
                string answerAboutSynedrion = "KEY_ALISTAIRONSYNEDRION_DESCRIPTION";
                string answerAboutAnu = "KEY_ALISTAIRONANU_DESCRIPTION";
                string answerAboutVirophage = "KEY_ALISTAIRONVIROPHAGE_DESCRIPTION";
                //   string answerAboutHelena = "KEY_ALISTAIRONHELENA_DESCRIPTION";
                string promptMoreQuestions = "KEY_ALISTAIR_ROADS_DESCRIPTION_2";

                GeoscapeEventDef alistairRoads = TFTVCommonMethods.CreateNewEvent(startingEvent, title, description, null);
                GeoscapeEventDef alistairRoadsAfterWest = TFTVCommonMethods.CreateNewEvent(afterWest, title, promptMoreQuestions, null);
                GeoscapeEventDef alistairRoadsAfterSynedrion = TFTVCommonMethods.CreateNewEvent(afterSynedrion, title, promptMoreQuestions, null);
                GeoscapeEventDef alistairRoadsAfterAnu = TFTVCommonMethods.CreateNewEvent(afterAnu, title, promptMoreQuestions, null);
                GeoscapeEventDef alistairRoadsAfterVirophage = TFTVCommonMethods.CreateNewEvent(afterVirophage, title, promptMoreQuestions, null);

                alistairRoads.GeoscapeEventData.Choices[0].Text.LocalizationKey = noMoreQuestions;
                alistairRoads.GeoscapeEventData.Choices[0].Outcome.TriggerEncounterID = passToOlena;
                TFTVCommonMethods.GenerateGeoEventChoice(alistairRoads, questionAboutWest, answerAboutWest);
                TFTVCommonMethods.GenerateGeoEventChoice(alistairRoads, questionAboutSynedrion, answerAboutSynedrion);
                TFTVCommonMethods.GenerateGeoEventChoice(alistairRoads, questionAboutAnu, answerAboutAnu);

                alistairRoadsAfterWest.GeoscapeEventData.Choices[0].Text.LocalizationKey = noMoreQuestions;
                alistairRoadsAfterWest.GeoscapeEventData.Choices[0].Outcome.TriggerEncounterID = passToOlena;
                TFTVCommonMethods.GenerateGeoEventChoice(alistairRoadsAfterWest, questionAboutSynedrion, answerAboutSynedrion);
                TFTVCommonMethods.GenerateGeoEventChoice(alistairRoadsAfterWest, questionAboutAnu, answerAboutAnu);
                TFTVCommonMethods.GenerateGeoEventChoice(alistairRoadsAfterWest, questionAboutVirophage, answerAboutVirophage);

                alistairRoadsAfterSynedrion.GeoscapeEventData.Choices[0].Text.LocalizationKey = noMoreQuestions;
                alistairRoadsAfterSynedrion.GeoscapeEventData.Choices[0].Outcome.TriggerEncounterID = passToOlena;
                TFTVCommonMethods.GenerateGeoEventChoice(alistairRoadsAfterSynedrion, questionAboutWest, answerAboutWest);
                TFTVCommonMethods.GenerateGeoEventChoice(alistairRoadsAfterSynedrion, questionAboutAnu, answerAboutAnu);
                TFTVCommonMethods.GenerateGeoEventChoice(alistairRoadsAfterSynedrion, questionAboutVirophage, answerAboutVirophage);

                alistairRoadsAfterAnu.GeoscapeEventData.Choices[0].Text.LocalizationKey = noMoreQuestions;
                alistairRoadsAfterAnu.GeoscapeEventData.Choices[0].Outcome.TriggerEncounterID = passToOlena;
                TFTVCommonMethods.GenerateGeoEventChoice(alistairRoadsAfterAnu, questionAboutWest, answerAboutWest);
                TFTVCommonMethods.GenerateGeoEventChoice(alistairRoadsAfterAnu, questionAboutSynedrion, answerAboutSynedrion);
                TFTVCommonMethods.GenerateGeoEventChoice(alistairRoadsAfterAnu, questionAboutVirophage, answerAboutVirophage);

                alistairRoadsAfterVirophage.GeoscapeEventData.Choices[0].Text.LocalizationKey = noMoreQuestions;
                alistairRoadsAfterVirophage.GeoscapeEventData.Choices[0].Outcome.TriggerEncounterID = passToOlena;
                TFTVCommonMethods.GenerateGeoEventChoice(alistairRoadsAfterVirophage, questionAboutWest, answerAboutWest);
                TFTVCommonMethods.GenerateGeoEventChoice(alistairRoadsAfterVirophage, questionAboutSynedrion, answerAboutSynedrion);
                TFTVCommonMethods.GenerateGeoEventChoice(alistairRoadsAfterVirophage, questionAboutAnu, answerAboutAnu);


                alistairRoads.GeoscapeEventData.Choices[1].Outcome.TriggerEncounterID = afterWest;
                alistairRoads.GeoscapeEventData.Choices[2].Outcome.TriggerEncounterID = afterSynedrion;
                alistairRoads.GeoscapeEventData.Choices[3].Outcome.TriggerEncounterID = afterAnu;

                alistairRoadsAfterWest.GeoscapeEventData.Choices[1].Outcome.TriggerEncounterID = afterSynedrion;
                alistairRoadsAfterWest.GeoscapeEventData.Choices[2].Outcome.TriggerEncounterID = afterAnu;
                alistairRoadsAfterWest.GeoscapeEventData.Choices[3].Outcome.TriggerEncounterID = afterVirophage;

                alistairRoadsAfterSynedrion.GeoscapeEventData.Choices[1].Outcome.TriggerEncounterID = afterWest;
                alistairRoadsAfterSynedrion.GeoscapeEventData.Choices[2].Outcome.TriggerEncounterID = afterAnu;
                alistairRoadsAfterSynedrion.GeoscapeEventData.Choices[3].Outcome.TriggerEncounterID = afterVirophage;

                alistairRoadsAfterAnu.GeoscapeEventData.Choices[1].Outcome.TriggerEncounterID = afterWest;
                alistairRoadsAfterAnu.GeoscapeEventData.Choices[2].Outcome.TriggerEncounterID = afterSynedrion;
                alistairRoadsAfterAnu.GeoscapeEventData.Choices[3].Outcome.TriggerEncounterID = afterVirophage;

                alistairRoadsAfterVirophage.GeoscapeEventData.Choices[1].Outcome.TriggerEncounterID = afterWest;
                alistairRoadsAfterVirophage.GeoscapeEventData.Choices[2].Outcome.TriggerEncounterID = afterSynedrion;
                alistairRoadsAfterVirophage.GeoscapeEventData.Choices[3].Outcome.TriggerEncounterID = afterAnu;

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }
        public static void CreateIntro()
        {
            try
            {
                string introEvent_0 = "IntroBetterGeo_0";
                string introEvent_1 = "IntroBetterGeo_1";
                string introEvent_2 = "IntroBetterGeo_2";
                GeoscapeEventDef intro0 = TFTVCommonMethods.CreateNewEvent(
                    introEvent_0, "BG_INTRO_0_TITLE", "BG_INTRO_0_DESCRIPTION", null);
                GeoscapeEventDef intro1 = TFTVCommonMethods.CreateNewEvent(
                    introEvent_1, "BG_INTRO_1_TITLE", "BG_INTRO_1_DESCRIPTION", null);
                intro1.GeoscapeEventData.Choices[0].Text.LocalizationKey = "BG_INTRO1_CHOICE_1";
                GeoscapeEventDef intro2 = TFTVCommonMethods.CreateNewEvent(
                    introEvent_2, "BG_INTRO_2_TITLE", "BG_INTRO_2_DESCRIPTION", null);
                intro2.GeoscapeEventData.Choices[0].Text.LocalizationKey = "BG_INTRO_2_CHOICE_0";


            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void ModifyAirCombatDefs()
        {
            try
            {
                //implementing Belial's proposal: 

                // ALN_VoidChamber_VehicleWeaponDef  Fire rate increased 20s-> 10s, Damage decreased 400-> 200
                // ALN_Spikes_VehicleWeaponDef	Changed to Psychic Guidance (from Visual Guidance)
                // ALN_Ram_VehicleWeaponDef Changed to Psychic Guidance(from Visual Guidance), HP 250-> 350

                // PX_Afterburner_GeoVehicleModuleDef Charges 5-> 3
                // PX_Flares_GeoVehicleModuleDef 5-> 3
                //  AN_ECMJammer_GeoVehicleModuleDef Charges 5-> 3

                //PX_ElectrolaserThunderboltHC9_VehicleWeaponDef Accuracy 95 % -> 85 %
                // PX_BasicMissileNomadAAM_VehicleWeaponDef 80 % -> 70 %
                // NJ_RailgunMaradeurAC4_VehicleWeaponDef 80 % -> 70 %
                //SY_LaserGunArtemisMkI_VehicleWeaponDef Artemis Accuracy 95 % -> 85 %


                GeoVehicleWeaponDef voidChamberWDef = DefCache.GetDef<GeoVehicleWeaponDef>("ALN_VoidChamber_VehicleWeaponDef");
                GeoVehicleWeaponDef spikesWDef = DefCache.GetDef<GeoVehicleWeaponDef>("ALN_Spikes_VehicleWeaponDef");
                GeoVehicleWeaponDef ramWDef = DefCache.GetDef<GeoVehicleWeaponDef>("ALN_Ram_VehicleWeaponDef");
                GeoVehicleWeaponDef thunderboltWDef = DefCache.GetDef<GeoVehicleWeaponDef>("PX_ElectrolaserThunderboltHC9_VehicleWeaponDef");
                GeoVehicleWeaponDef nomadWDef = DefCache.GetDef<GeoVehicleWeaponDef>("PX_BasicMissileNomadAAM_VehicleWeaponDef");
                GeoVehicleWeaponDef railGunWDef = DefCache.GetDef<GeoVehicleWeaponDef>("NJ_RailgunMaradeurAC4_VehicleWeaponDef");
                GeoVehicleWeaponDef laserGunWDef = DefCache.GetDef<GeoVehicleWeaponDef>("SY_LaserGunArtemisMkI_VehicleWeaponDef");

                //Design decision
                GeoVehicleModuleDef afterburnerMDef = DefCache.GetDef<GeoVehicleModuleDef>("PX_Afterburner_GeoVehicleModuleDef");
                GeoVehicleModuleDef flaresMDef = DefCache.GetDef<GeoVehicleModuleDef>("PX_Flares_GeoVehicleModuleDef");
                //   GeoVehicleModuleDef jammerMDef = DefCache.GetDef<GeoVehicleModuleDef>("AN_ECMJammer_GeoVehicleModuleDef");

                voidChamberWDef.ChargeTime = 10.0f;
                var voidDamagePayload = voidChamberWDef.DamagePayloads[0].Damage;
                voidChamberWDef.DamagePayloads[0] = new GeoWeaponDamagePayload { Damage = voidDamagePayload, Amount = 200 };

                spikesWDef.Guidence = GeoVehicleWeaponGuidence.Psychic;
                // ramWDef.Guidence = GeoVehicleWeaponGuidence.Psychic;
                ramWDef.HitPoints = 350;
                thunderboltWDef.Accuracy = 85;
                nomadWDef.Accuracy = 70;
                railGunWDef.Accuracy = 70;
                laserGunWDef.Accuracy = 85;

                afterburnerMDef.HitPoints = 250;
                flaresMDef.HitPoints = 250;
                //flaresMDef.AmmoCount = 3;
                //jammerMDef.AmmoCount = 3;

                ResearchDbDef ppResearchDB = DefCache.GetDef<ResearchDbDef>("pp_ResearchDB");
                ResearchDbDef anuResearchDB = DefCache.GetDef<ResearchDbDef>("anu_ResearchDB");
                ResearchDbDef njResearchDB = DefCache.GetDef<ResearchDbDef>("nj_ResearchDB");
                ResearchDbDef synResearchDB = DefCache.GetDef<ResearchDbDef>("syn_ResearchDB");

                //removing unnecessary researches 
                synResearchDB.Researches.Remove(DefCache.GetDef<ResearchDef>("SYN_Aircraft_SecurityStation_ResearchDef"));
                ppResearchDB.Researches.Remove(DefCache.GetDef<ResearchDef>("PX_Aircraft_EscapePods_ResearchDef"));
                njResearchDB.Researches.Remove(DefCache.GetDef<ResearchDef>("NJ_Aircraft_CruiseControl_ResearchDef"));
                njResearchDB.Researches.Remove(DefCache.GetDef<ResearchDef>("NJ_Aircraft_FuelTank_ResearchDef"));


                //This is testing Belial's suggestions, unlocking flares via PX Aerial Warfare, etc.
                AddItemToManufacturingReward("PX_Aircraft_Flares_ResearchDef_ManufactureResearchRewardDef_0",
                    "PX_AerialWarfare_ResearchDef_ManufactureResearchRewardDef_0", "PX_Aircraft_Flares_ResearchDef");

                ManufactureResearchRewardDef fenrirReward = DefCache.GetDef<ManufactureResearchRewardDef>("PX_Aircraft_VirophageGun_ResearchDef_ManufactureResearchRewardDef_0");
                ManufactureResearchRewardDef virophageWeaponsReward = DefCache.GetDef<ManufactureResearchRewardDef>("PX_VirophageWeapons_ResearchDef_ManufactureResearchRewardDef_0");
                List<ItemDef> rewardsVirophage = virophageWeaponsReward.Items.ToList();
                rewardsVirophage.Add(fenrirReward.Items[0]);
                virophageWeaponsReward.Items = rewardsVirophage.ToArray();
                ResearchDef fenrirResearch = DefCache.GetDef<ResearchDef>("PX_Aircraft_VirophageGun_ResearchDef");
                ppResearchDB.Researches.Remove(fenrirResearch);


                ManufactureResearchRewardDef thunderboltReward = DefCache.GetDef<ManufactureResearchRewardDef>("PX_Aircraft_Electrolaser_ResearchDef_ManufactureResearchRewardDef_0");
                ManufactureResearchRewardDef advancedLasersReward = DefCache.GetDef<ManufactureResearchRewardDef>("PX_AdvancedLaserTech_ResearchDef_ManufactureResearchRewardDef_0");
                List<ItemDef> rewardsAdvancedLasers = advancedLasersReward.Items.ToList();
                rewardsAdvancedLasers.Add(thunderboltReward.Items[0]);
                advancedLasersReward.Items = rewardsAdvancedLasers.ToArray();
                ResearchDef electroLaserResearch = DefCache.GetDef<ResearchDef>("PX_Aircraft_Electrolaser_ResearchDef");
                ppResearchDB.Researches.Remove(electroLaserResearch);

                ManufactureResearchRewardDef handOfTyrReward = DefCache.GetDef<ManufactureResearchRewardDef>("PX_Aircraft_HypersonicMissile_ResearchDef_ManufactureResearchRewardDef_0");
                ManufactureResearchRewardDef advancedShreddingReward = DefCache.GetDef<ManufactureResearchRewardDef>("PX_AdvancedShreddingTech_ResearchDef_ManufactureResearchRewardDef_0");
                List<ItemDef> rewardsAdvancedShredding = advancedShreddingReward.Items.ToList();
                rewardsAdvancedShredding.Add(handOfTyrReward.Items[0]);
                advancedShreddingReward.Items = rewardsAdvancedShredding.ToArray();
                ResearchDef handOfTyrResearch = DefCache.GetDef<ResearchDef>("PX_Aircraft_HypersonicMissile_ResearchDef");
                ppResearchDB.Researches.Remove(handOfTyrResearch);

                AddItemToManufacturingReward("NJ_Aircraft_TacticalNuke_ResearchDef_ManufactureResearchRewardDef_0",
                    "NJ_GuidanceTech_ResearchDef_ManufactureResearchRewardDef_0", "NJ_Aircraft_TacticalNuke_ResearchDef");
                ResearchDef tacticalNukeResearch = DefCache.GetDef<ResearchDef>("NJ_Aircraft_TacticalNuke_ResearchDef");
                ResearchDef njGuidanceResearch = DefCache.GetDef<ResearchDef>("NJ_GuidanceTech_ResearchDef");
                List<ResearchRewardDef> guidanceUnlocks = njGuidanceResearch.Unlocks.ToList();
                guidanceUnlocks.Add(tacticalNukeResearch.Unlocks[1]);
                njGuidanceResearch.Unlocks = guidanceUnlocks.ToArray();


                AddItemToManufacturingReward("NJ_Aircraft_FuelTank_ResearchDef_ManufactureResearchRewardDef_0",
                    "NJ_VehicleTech_ResearchDef_ManufactureResearchRewardDef_0", "NJ_Aircraft_FuelTank_ResearchDef");

                AddItemToManufacturingReward("NJ_Aircraft_CruiseControl_ResearchDef_ManufactureResearchRewardDef_0",
                    "SYN_Rover_ResearchDef_ManufactureResearchRewardDef_0", "NJ_Aircraft_CruiseControl_ResearchDef");

                ManufactureResearchRewardDef medusaAAM = DefCache.GetDef<ManufactureResearchRewardDef>("SYN_Aircraft_EMPMissile_ResearchDef_ManufactureResearchRewardDef_0");
                ManufactureResearchRewardDef synAirCombat = DefCache.GetDef<ManufactureResearchRewardDef>("SYN_NanoTech_ResearchDef_ManufactureResearchRewardDef_0");
                List<ItemDef> rewards = synAirCombat.Items.ToList();
                rewards.Add(medusaAAM.Items[0]);
                synAirCombat.Items = rewards.ToArray();

                ResearchDef nanotechResearch = DefCache.GetDef<ResearchDef>("SYN_NanoTech_ResearchDef");
                ResearchDef medusaAAMResearch = DefCache.GetDef<ResearchDef>("SYN_Aircraft_EMPMissile_ResearchDef");
                synResearchDB.Researches.Remove(medusaAAMResearch);
                if (ppResearchDB.Researches.Contains(medusaAAMResearch))
                {
                    ppResearchDB.Researches.Remove(medusaAAMResearch);
                }
                List<ResearchRewardDef> nanotechUnlocks = nanotechResearch.Unlocks.ToList();
                nanotechUnlocks.Add(medusaAAMResearch.Unlocks[1]);
                nanotechResearch.Unlocks = nanotechUnlocks.ToArray();

                //This one is the source of the gamebreaking bug:
                /* AddItemToManufacturingReward("SY_EMPMissileMedusaAAM_VehicleWeaponDef",
                         "SYN_AerialWarfare_ResearchDef_ManufactureResearchRewardDef_0", "SYN_Aircraft_EMPMissile_ResearchDef");*/
                AddItemToManufacturingReward("ANU_Aircraft_Oracle_ResearchDef_ManufactureResearchRewardDef_0",
                    "ANU_AerialWarfare_ResearchDef_ManufactureResearchRewardDef_0", "ANU_Aircraft_Oracle_ResearchDef");

                ResearchDef anuAWResearch = DefCache.GetDef<ResearchDef>("ANU_AerialWarfare_ResearchDef");
                ResearchDef oracleResearch = DefCache.GetDef<ResearchDef>("ANU_Aircraft_Oracle_ResearchDef");

                List<ResearchRewardDef> anuAWUnlocks = anuAWResearch.Unlocks.ToList();
                anuAWUnlocks.Add(oracleResearch.Unlocks[1]);
                anuAWResearch.Unlocks = anuAWUnlocks.ToArray();


                CreateManufacturingReward("ANU_Aircraft_MutogCatapult_ResearchDef_ManufactureResearchRewardDef_0",
                    "ANU_Aircraft_ECMJammer_ResearchDef_ManufactureResearchRewardDef_0", "ANU_Aircraft_ECMJammer_ResearchDef", "ANU_Aircraft_MutogCatapult_ResearchDef",
                    "ANU_AdvancedBlimp_ResearchDef");

                ResearchDef advancedBlimpResearch = DefCache.GetDef<ResearchDef>("ANU_AdvancedBlimp_ResearchDef");
                ResearchDef ecmResearch = DefCache.GetDef<ResearchDef>("ANU_Aircraft_ECMJammer_ResearchDef");
                ResearchDef mutogCatapultResearch = DefCache.GetDef<ResearchDef>("ANU_Aircraft_MutogCatapult_ResearchDef");

                List<ResearchRewardDef> advancedBlimpUnlocks = advancedBlimpResearch.Unlocks.ToList();
                advancedBlimpUnlocks.Add(ecmResearch.Unlocks[1]);
                advancedBlimpUnlocks.Add(mutogCatapultResearch.Unlocks[1]);
                advancedBlimpResearch.Unlocks = advancedBlimpUnlocks.ToArray();

                CreateManufacturingReward("PX_Aircraft_Autocannon_ResearchDef_ManufactureResearchRewardDef_0", "SYN_Aircraft_SecurityStation_ResearchDef_ManufactureResearchRewardDef_0",
                      "SYN_Aircraft_SecurityStation_ResearchDef", "PX_Aircraft_Autocannon_ResearchDef",
                      "PX_Alien_Spawnery_ResearchDef");

                EncounterVariableResearchRequirementDef charunEncounterVariableResearchRequirement = DefCache.GetDef<EncounterVariableResearchRequirementDef>("ALN_Small_Flyer_ResearchDef_EncounterVariableResearchRequirementDef_0");
                charunEncounterVariableResearchRequirement.VariableName = "CharunAreComing";

                //Changing ALN Berith research req so that they only appear after certain ODI event
                EncounterVariableResearchRequirementDef berithEncounterVariable = DefCache.GetDef<EncounterVariableResearchRequirementDef>("ALN_Medium_Flyer_ResearchDef_EncounterVariableResearchRequirementDef_0");
                berithEncounterVariable.VariableName = "BerithResearchVariable";

                //Changing ALN Abbadon research so they appear only in Third Act, or After ODI reaches apex
                EncounterVariableResearchRequirementDef sourceVarResReq =
                   DefCache.GetDef<EncounterVariableResearchRequirementDef>("NJ_Bionics1_ResearchDef_EncounterVariableResearchRequirementDef_0");

                //Creating new Research Requirements, each requiring a variable to be triggered  
                EncounterVariableResearchRequirementDef variableResReqAbbadon = Helper.CreateDefFromClone(sourceVarResReq, "F8D9463A-69C5-47B1-B52A-061D898CEEF8", "AbbadonResReqDef");
                variableResReqAbbadon.VariableName = "AbbadonResearchVariable";
                //  EncounterVariableResearchRequirementDef variableResReqAbbadonAlt = Helper.CreateDefFromClone(sourceVarResReq, "F8D9463A-69C5-47B1-B52A-061D898CEEF8", "AbbadonResReqAltDef");
                //  variableResReqAbbadonAlt.VariableName = "ODI_Complete";
                //Altering researchDef, requiring Third Act to have started and adding an alternative way of revealing research if ODI is completed 
                ResearchDef aLN_Large_Flyer_ResearchDef = DefCache.GetDef<ResearchDef>("ALN_Large_Flyer_ResearchDef");
                //  aLN_Large_Flyer_ResearchDef.RevealRequirements.Operation = ResearchContainerOperation.ANY;

                ReseachRequirementDefOpContainer[] reseachRequirementDefOpContainers = new ReseachRequirementDefOpContainer[1];
                ResearchRequirementDef[] researchRequirementDefs = new ResearchRequirementDef[1];
                researchRequirementDefs[0] = variableResReqAbbadon;

                reseachRequirementDefOpContainers[0].Requirements = researchRequirementDefs;
                aLN_Large_Flyer_ResearchDef.RevealRequirements.Container = reseachRequirementDefOpContainers;

                //Changes to FesteringSkies settings
                FesteringSkiesSettingsDef festeringSkiesSettingsDef = DefCache.GetDef<FesteringSkiesSettingsDef>("FesteringSkiesSettingsDef");
                festeringSkiesSettingsDef.SpawnInfestedAircraftChance = 0;
                festeringSkiesSettingsDef.InfestedAircraftChance = 0;
                festeringSkiesSettingsDef.InfestedAircrafts.Clear();
                festeringSkiesSettingsDef.InfestedAircraftRebuildHours = 100000;

                InterceptionGameDataDef interceptionGameDataDef = DefCache.GetDef<InterceptionGameDataDef>("InterceptionGameDataDef");
                interceptionGameDataDef.DisengageDuration = 3;

                RemoveHardFlyersTemplates();
            }


            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void RemoveHardFlyersTemplates()
        {
            try
            {
                GeoVehicleWeaponDef acidSpit = DefCache.GetDef<GeoVehicleWeaponDef>("ALN_AcidSpit_VehicleWeaponDef");
                GeoVehicleWeaponDef spikes = DefCache.GetDef<GeoVehicleWeaponDef>("ALN_Spikes_VehicleWeaponDef");
                GeoVehicleWeaponDef napalmBreath = DefCache.GetDef<GeoVehicleWeaponDef>("ALN_NapalmBreath_VehicleWeaponDef");
                GeoVehicleWeaponDef ram = DefCache.GetDef<GeoVehicleWeaponDef>("ALN_Ram_VehicleWeaponDef");
                GeoVehicleWeaponDef tick = DefCache.GetDef<GeoVehicleWeaponDef>("ALN_Tick_VehicleWeaponDef");
                GeoVehicleWeaponDef voidChamber = DefCache.GetDef<GeoVehicleWeaponDef>("ALN_VoidChamber_VehicleWeaponDef");

                /* GeoVehicleWeaponDamageDef shredDamage = DefCache.GetDef<GeoVehicleWeaponDamageDef>("Shred_GeoVehicleWeaponDamageDef"); 
                 GeoVehicleWeaponDamageDef regularDamage= DefCache.GetDef<GeoVehicleWeaponDamageDef>("Regular_GeoVehicleWeaponDamageDef");

                 tick.DamagePayloads[0] = new GeoWeaponDamagePayload { Damage = shredDamage, Amount = 20 };
                 tick.DamagePayloads.Add(new GeoWeaponDamagePayload { Damage = regularDamage, Amount = 60 });*/


                GeoVehicleLoadoutDef charun2 = DefCache.GetDef<GeoVehicleLoadoutDef>("AL_Small2_VehicleLoadout");
                GeoVehicleLoadoutDef charun4 = DefCache.GetDef<GeoVehicleLoadoutDef>("AL_Small4_VehicleLoadout");
                GeoVehicleLoadoutDef berith1 = DefCache.GetDef<GeoVehicleLoadoutDef>("AL_Medium1_VehicleLoadout");
                GeoVehicleLoadoutDef berith2 = DefCache.GetDef<GeoVehicleLoadoutDef>("AL_Medium2_VehicleLoadout");
                GeoVehicleLoadoutDef berith3 = DefCache.GetDef<GeoVehicleLoadoutDef>("AL_Medium3_VehicleLoadout");
                GeoVehicleLoadoutDef berith4 = DefCache.GetDef<GeoVehicleLoadoutDef>("AL_Medium4_VehicleLoadout");
                GeoVehicleLoadoutDef abbadon1 = DefCache.GetDef<GeoVehicleLoadoutDef>("AL_Large1_VehicleLoadout");
                GeoVehicleLoadoutDef abbadon2 = DefCache.GetDef<GeoVehicleLoadoutDef>("AL_Large2_VehicleLoadout");
                GeoVehicleLoadoutDef abbadon3 = DefCache.GetDef<GeoVehicleLoadoutDef>("AL_Large3_VehicleLoadout");

                charun2.EquippedItems[0] = napalmBreath;
                charun2.EquippedItems[1] = ram;

                charun4.EquippedItems[0] = voidChamber;
                charun4.EquippedItems[1] = spikes;

                berith1.EquippedItems[0] = acidSpit;
                berith1.EquippedItems[1] = acidSpit;
                berith1.EquippedItems[2] = spikes;
                berith1.EquippedItems[3] = ram;

                berith2.EquippedItems[0] = tick;
                berith2.EquippedItems[1] = ram;
                berith2.EquippedItems[2] = ram;
                berith2.EquippedItems[3] = spikes;

                berith3.EquippedItems[0] = napalmBreath;
                berith3.EquippedItems[1] = spikes;
                berith3.EquippedItems[2] = spikes;
                berith3.EquippedItems[3] = ram;

                berith4.EquippedItems[0] = voidChamber;
                berith4.EquippedItems[1] = napalmBreath;
                berith4.EquippedItems[2] = ram;
                berith4.EquippedItems[3] = ram;

                abbadon1.EquippedItems[0] = acidSpit;
                abbadon1.EquippedItems[1] = acidSpit;
                abbadon1.EquippedItems[2] = acidSpit;
                abbadon1.EquippedItems[3] = spikes;
                abbadon1.EquippedItems[4] = spikes;
                abbadon1.EquippedItems[5] = spikes;

                abbadon2.EquippedItems[0] = voidChamber;
                abbadon2.EquippedItems[1] = napalmBreath;
                abbadon2.EquippedItems[2] = ram;
                abbadon2.EquippedItems[3] = ram;
                abbadon2.EquippedItems[4] = ram;
                abbadon2.EquippedItems[5] = ram;

                abbadon3.EquippedItems[0] = voidChamber;
                abbadon3.EquippedItems[1] = voidChamber;
                abbadon3.EquippedItems[2] = ram;
                abbadon3.EquippedItems[3] = ram;
                abbadon3.EquippedItems[4] = spikes;
                abbadon3.EquippedItems[5] = spikes;



                /* Info about Vanilla loadouts:
               AlienFlyerResearchRewardDef aLN_Small_FlyerLoadouts= DefCache.GetDef<AlienFlyerResearchRewardDef>("ALN_Small_Flyer_ResearchDef_FlyerLoadoutResearchRewardDef_0");
                AL_Small1_VehicleLoadout: ALN_AcidSpit_VehicleWeaponDef, ALN_Spikes_VehicleWeaponDef
                AL_Small2_VehicleLoadout: ALN_NapalmBreath_VehicleWeaponDef, ALN_AcidSpit_VehicleWeaponDef
                AL_Small3_VehicleLoadout: ALN_Ram_VehicleWeaponDef, ALN_Spikes_VehicleWeaponDef

                AlienFlyerResearchRewardDef aLN_Medium_FlyerLoadouts = DefCache.GetDef<AlienFlyerResearchRewardDef>("ALN_Medium_Flyer_ResearchDef_FlyerLoadoutResearchRewardDef_0");
                AL_Medium1_VehicleLoadout: ALN_AcidSpit_VehicleWeaponDef, ALN_Spikes_VehicleWeaponDef, ALN_Spikes_VehicleWeaponDef, ALN_Tick_VehicleWeaponDef
                AL_Medium2_VehicleLoadout: ALN_NapalmBreath_VehicleWeaponDef, ALN_NapalmBreath_VehicleWeaponDef, ALN_Ram_VehicleWeaponDef, ALN_Ram_VehicleWeaponDef
                AL_Medium3_VehicleLoadout: ALN_AcidSpit_VehicleWeaponDef, ALN_Spikes_VehicleWeaponDef, ALN_Tick_VehicleWeaponDef, ALN_Ram_VehicleWeaponDef
                AL_Small4_VehicleLoadout: ALN_NapalmBreath_VehicleWeaponDef, ALN_Tick_VehicleWeaponDef

                AlienFlyerResearchRewardDef aLN_Large_FlyerLoadouts = DefCache.GetDef<AlienFlyerResearchRewardDef>("ALN_Large_Flyer_ResearchDef_FlyerLoadoutResearchRewardDef_0");
                AL_Large1_VehicleLoadout: ALN_VoidChamber_VehicleWeaponDef, ALN_VoidChamber_VehicleWeaponDef, ALN_NapalmBreath_VehicleWeaponDef, ALN_NapalmBreath_VehicleWeaponDef, ALN_Tick_VehicleWeaponDef, ALN_Tick_VehicleWeaponDef
                AL_Large2_VehicleLoadout: ALN_AcidSpit_VehicleWeaponDef, ALN_AcidSpit_VehicleWeaponDef, ALN_AcidSpit_VehicleWeaponDef, ALN_Spikes_VehicleWeaponDef, ALN_Spikes_VehicleWeaponDef, ALN_Spikes_VehicleWeaponDef
                AL_Large3_VehicleLoadout: ALN_NapalmBreath_VehicleWeaponDef, ALN_Tick_VehicleWeaponDef, ALN_Ram_VehicleWeaponDef, ALN_VoidChamber_VehicleWeaponDef, ALN_AcidSpit_VehicleWeaponDef, ALN_Spikes_VehicleWeaponDef
                AL_Small5_VehicleLoadout: ALN_Ram_VehicleWeaponDef, ALN_VoidChamber_VehicleWeaponDef
                AL_Medium4_VehicleLoadout: ALN_AcidSpit_VehicleWeaponDef, ALN_Spikes_VehicleWeaponDef, ALN_VoidChamber_VehicleWeaponDef, ALN_Tick_VehicleWeaponDef

                */


            }


            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }

        public static void AddItemToManufacturingReward(string researchReward, string reward, string research)
        {

            try
            {

                ResearchDbDef ppResearchDB = DefCache.GetDef<ResearchDbDef>("pp_ResearchDB");
                ResearchDbDef anuResearchDB = DefCache.GetDef<ResearchDbDef>("anu_ResearchDB");
                ResearchDbDef njResearchDB = DefCache.GetDef<ResearchDbDef>("nj_ResearchDB");
                ResearchDbDef synResearchDB = DefCache.GetDef<ResearchDbDef>("syn_ResearchDB");

                ManufactureResearchRewardDef researchRewardDef = DefCache.GetDef<ManufactureResearchRewardDef>(researchReward);
                ManufactureResearchRewardDef rewardDef = DefCache.GetDef<ManufactureResearchRewardDef>(reward);

                ResearchDef researchDef = DefCache.GetDef<ResearchDef>(research);
                List<ItemDef> rewards = rewardDef.Items.ToList();
                rewards.Add(researchRewardDef.Items[0]);
                rewardDef.Items = rewards.ToArray();
                if (ppResearchDB.Researches.Contains(researchDef))
                {
                    ppResearchDB.Researches.Remove(researchDef);
                }
                if (anuResearchDB.Researches.Contains(researchDef))
                {
                    anuResearchDB.Researches.Remove(researchDef);
                }
                if (njResearchDB.Researches.Contains(researchDef))
                {
                    anuResearchDB.Researches.Remove(researchDef);
                }
                if (synResearchDB.Researches.Contains(researchDef))
                {
                    anuResearchDB.Researches.Remove(researchDef);
                }


            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }

        public static void CreateManufacturingReward(string researchReward1, string researchReward2, string research, string research2, string newResearch)
        {

            try
            {
                ResearchDbDef ppResearchDB = DefCache.GetDef<ResearchDbDef>("pp_ResearchDB");
                ResearchDbDef anuResearchDB = DefCache.GetDef<ResearchDbDef>("anu_ResearchDB");
                ResearchDbDef njResearchDB = DefCache.GetDef<ResearchDbDef>("nj_ResearchDB");
                ResearchDbDef synResearchDB = DefCache.GetDef<ResearchDbDef>("syn_ResearchDB");

                ManufactureResearchRewardDef researchReward1Def = DefCache.GetDef<ManufactureResearchRewardDef>(researchReward1);
                ManufactureResearchRewardDef researchReward2Def = DefCache.GetDef<ManufactureResearchRewardDef>(researchReward2);
                ResearchDef researchDef = DefCache.GetDef<ResearchDef>(research);
                ResearchDef research2Def = DefCache.GetDef<ResearchDef>(research2);
                ResearchDef newResearchDef = DefCache.GetDef<ResearchDef>(newResearch);
                List<ItemDef> rewards = researchReward2Def.Items.ToList();
                rewards.Add(researchReward1Def.Items[0]);
                researchReward2Def.Items = rewards.ToArray();
                newResearchDef.Unlocks = researchDef.Unlocks;
                newResearchDef.Unlocks[0] = researchReward2Def;

                if (ppResearchDB.Researches.Contains(researchDef))
                {
                    ppResearchDB.Researches.Remove(researchDef);
                }
                if (anuResearchDB.Researches.Contains(researchDef))
                {
                    anuResearchDB.Researches.Remove(researchDef);
                }
                if (njResearchDB.Researches.Contains(researchDef))
                {
                    anuResearchDB.Researches.Remove(researchDef);
                }
                if (synResearchDB.Researches.Contains(researchDef))
                {
                    anuResearchDB.Researches.Remove(researchDef);
                }
                if (ppResearchDB.Researches.Contains(research2Def))
                {
                    ppResearchDB.Researches.Remove(research2Def);
                }
                if (anuResearchDB.Researches.Contains(research2Def))
                {
                    anuResearchDB.Researches.Remove(research2Def);
                }
                if (njResearchDB.Researches.Contains(research2Def))
                {
                    anuResearchDB.Researches.Remove(research2Def);
                }
                if (synResearchDB.Researches.Contains(research2Def))
                {
                    anuResearchDB.Researches.Remove(research2Def);
                }
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }

        public static void RemoveCorruptionDamageBuff()
        {
            try
            {
                CorruptionStatusDef corruption_StatusDef = DefCache.GetDef<CorruptionStatusDef>("Corruption_StatusDef");
                corruption_StatusDef.Multiplier = 0.0f;
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }

        public static void ChangesToMedbay()
        {
            try
            {
                HealFacilityComponentDef e_HealMedicalBay_PhoenixFacilityDe = DefCache.GetDef<HealFacilityComponentDef>("E_Heal [MedicalBay_PhoenixFacilityDef]");
                e_HealMedicalBay_PhoenixFacilityDe.BaseHeal = 16;
                PhoenixFacilityDef medbay = DefCache.GetDef<PhoenixFacilityDef>("MedicalBay_PhoenixFacilityDef");
                medbay.ConstructionTimeDays = 1.5f;
                medbay.ResourceCost = new ResourcePack
                {
                    new ResourceUnit { Type = ResourceType.Materials, Value = 200 },
                    new ResourceUnit { Type = ResourceType.Tech, Value = 50 }
                };

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }
        public static void MistOnAllMissions()
        {
            try
            {
                foreach (CustomMissionTypeDef missionTypeDef in Repo.GetAllDefs<CustomMissionTypeDef>())
                {
                    missionTypeDef.SpawnMistAtLevelStart = true;
                }
            }

            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }

        public static void ChangeUmbra()

        {
            try
            {
                RandomValueEffectConditionDef randomValueFishUmbra = DefCache.GetDef<RandomValueEffectConditionDef>("E_RandomValue [UmbralFishmen_FactionEffectDef]");
                RandomValueEffectConditionDef randomValueCrabUmbra = DefCache.GetDef<RandomValueEffectConditionDef>("E_RandomValue [UmbralCrabmen_FactionEffectDef]");
                randomValueCrabUmbra.ThresholdValue = 0;
                randomValueFishUmbra.ThresholdValue = 0;
                EncounterVariableResearchRequirementDef sourceVarResReq =
                   DefCache.GetDef<EncounterVariableResearchRequirementDef>("NJ_Bionics1_ResearchDef_EncounterVariableResearchRequirementDef_0");
                //Changing Umbra Crab and Triton to appear after SDI event 3;
                ResearchDef umbraCrabResearch = DefCache.GetDef<ResearchDef>("ALN_CrabmanUmbra_ResearchDef");

                //Creating new Research Requirement, requiring a variable to be triggered  
                string variableUmbraALNResReq = "Umbra_Encounter_Variable";
                EncounterVariableResearchRequirementDef variableResReqUmbra = Helper.CreateDefFromClone(sourceVarResReq, "0CCC30E0-4DB1-44CD-9A60-C1C8F6588C8A", "UmbraResReqDef");
                variableResReqUmbra.VariableName = variableUmbraALNResReq;
                // This changes the Umbra reserach so that 2 conditions have to be fulfilled: 1) a) nest has to be researched, or b) exotic material has to be found
                // (because 1)a) is fufilled at start of the game, b) is redundant but harmless), and 2) a special variable has to be triggered, assigned to event sdi3
                umbraCrabResearch.RevealRequirements.Operation = ResearchContainerOperation.ALL;
                umbraCrabResearch.RevealRequirements.Container[0].Operation = ResearchContainerOperation.ANY;
                umbraCrabResearch.RevealRequirements.Container[1].Requirements[0] = variableResReqUmbra;
                //Now same thing for Triton Umbra, but it will use same variable because we want them to appear at the same time
                ResearchDef umbraFishResearch = DefCache.GetDef<ResearchDef>("ALN_FishmanUmbra_ResearchDef");
                umbraFishResearch.RevealRequirements.Operation = ResearchContainerOperation.ALL;
                umbraFishResearch.RevealRequirements.Container[0].Operation = ResearchContainerOperation.ANY;
                umbraFishResearch.RevealRequirements.Container[1].Requirements[0] = variableResReqUmbra;
                //Because Triton research has 2 requirements in the second container, we set them to any
                umbraFishResearch.RevealRequirements.Container[1].Operation = ResearchContainerOperation.ANY;

                ViewElementDef oilCrabViewElementDef = DefCache.GetDef<ViewElementDef>("E_View [Oilcrab_Torso_BodyPartDef]");
                oilCrabViewElementDef.DisplayName1.LocalizationKey = "TFTV_KEY_UMBRA_TARGET_DISPLAY_NAME";
                oilCrabViewElementDef.Description.LocalizationKey = "TFTV_KEY_UMBRA_TARGET_DISPLAY_DESCRIPTION";
                oilCrabViewElementDef.SmallIcon = UmbraIcon;
                oilCrabViewElementDef.LargeIcon = UmbraIcon;
                oilCrabViewElementDef.InventoryIcon = UmbraIcon;

                ViewElementDef oilFishViewElementDef = DefCache.GetDef<ViewElementDef>("E_View [Oilfish_Torso_BodyPartDef]");
                oilFishViewElementDef.DisplayName1.LocalizationKey = "TFTV_KEY_UMBRA_TARGET_DISPLAY_NAME";
                oilFishViewElementDef.Description.LocalizationKey = "TFTV_KEY_UMBRA_TARGET_DISPLAY_DESCRIPTION";
                oilFishViewElementDef.SmallIcon = UmbraIcon;
                oilFishViewElementDef.LargeIcon = UmbraIcon;
                oilFishViewElementDef.InventoryIcon = UmbraIcon;

                TacticalPerceptionDef oilCrabPerceptionDef = DefCache.GetDef<TacticalPerceptionDef>("Oilcrab_PerceptionDef");
                TacticalPerceptionDef oilFishPerceptionDef = DefCache.GetDef<TacticalPerceptionDef>("Oilfish_PerceptionDef");
                oilCrabPerceptionDef.PerceptionRange = 30.0f;
                oilFishPerceptionDef.PerceptionRange = 30.0f;
                //
                AddAbilityStatusDef oilTritonAddAbilityStatus = DefCache.GetDef<AddAbilityStatusDef>("OilFish_AddAbilityStatusDef");
                oilTritonAddAbilityStatus.ApplicationConditions = new EffectConditionDef[] { };
                AddAbilityStatusDef oilCrabAddAbilityStatus = DefCache.GetDef<AddAbilityStatusDef>("OilCrab_AddAbilityStatusDef");
                oilCrabAddAbilityStatus.ApplicationConditions = new EffectConditionDef[] { };
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }


        }
        public static void Create_VoidOmen_Events()

        {
            TFTVCommonMethods.CreateNewEvent("VoidOmen", "", "", null);
            TFTVCommonMethods.CreateNewEvent("VoidOmenIntro", "", "", null);

        }

        public static void AugmentationEventsDefs()
        {
            try
            {
                //ID all the factions for later
                GeoFactionDef phoenixPoint = DefCache.GetDef<GeoFactionDef>("Phoenix_GeoPhoenixFactionDef");
                GeoFactionDef newJericho = DefCache.GetDef<GeoFactionDef>("NewJericho_GeoFactionDef");
                GeoFactionDef anu = DefCache.GetDef<GeoFactionDef>("Anu_GeoFactionDef");
                GeoFactionDef synedrion = DefCache.GetDef<GeoFactionDef>("Synedrion_GeoFactionDef");

                //Anu pissed at player for doing Bionics
                GeoscapeEventDef anuPissedAtBionics = TFTVCommonMethods.CreateNewEvent("Anu_Pissed1", "ANU_PISSED_BIONICS_TITLE", "ANU_PISSED_BIONICS_TEXT_GENERAL_0", "ANU_PISSED_BIONICS_CHOICE_0_OUTCOME");
                anuPissedAtBionics.GeoscapeEventData.Leader = "AN_Synod";

                anuPissedAtBionics.GeoscapeEventData.Choices[0].Text.LocalizationKey = "ANU_PISSED_BIONICS_CHOICE_0";

                anuPissedAtBionics.GeoscapeEventData.Choices[0].Outcome.Diplomacy.Add(TFTVCommonMethods.GenerateDiplomacyOutcome(anu, phoenixPoint, -8));
                anuPissedAtBionics.GeoscapeEventData.Choices[0].Outcome.Diplomacy.Add(TFTVCommonMethods.GenerateDiplomacyOutcome(synedrion, phoenixPoint, +2));
                TFTVCommonMethods.GenerateGeoEventChoice(anuPissedAtBionics, "ANU_PISSED_BIONICS_CHOICE_1", "ANU_PISSED_BIONICS_CHOICE_1_OUTCOME");
                anuPissedAtBionics.GeoscapeEventData.Choices[1].Outcome.Diplomacy.Add(TFTVCommonMethods.GenerateDiplomacyOutcome(anu, phoenixPoint, -8));
                anuPissedAtBionics.GeoscapeEventData.Choices[1].Outcome.Diplomacy.Add(TFTVCommonMethods.GenerateDiplomacyOutcome(newJericho, phoenixPoint, +2));
                TFTVCommonMethods.GenerateGeoEventChoice(anuPissedAtBionics, "ANU_PISSED_BIONICS_CHOICE_2", "ANU_PISSED_BIONICS_CHOICE_2_OUTCOME");
                anuPissedAtBionics.GeoscapeEventData.Choices[2].Outcome.VariablesChange.Add(TFTVCommonMethods.GenerateVariableChange("BG_Anu_Pissed_Made_Promise", 1, true));


                //Anu really pissed at player for doing Bionics
                GeoscapeEventDef anuReallyPissedAtBionics = TFTVCommonMethods.CreateNewEvent("Anu_Pissed2", "ANU_REALLY_PISSED_BIONICS_TITLE", "ANU_REALLY_PISSED_BIONICS_TEXT_GENERAL_0", null);
                anuReallyPissedAtBionics.GeoscapeEventData.Leader = "AN_Synod";
                anuReallyPissedAtBionics.GeoscapeEventData.Choices[0].Text.LocalizationKey = "ANU_REALLY_PISSED_BIONICS_CHOICE_0";
                anuReallyPissedAtBionics.GeoscapeEventData.Choices[0].Outcome.Diplomacy.Add(TFTVCommonMethods.GenerateDiplomacyOutcome(anu, phoenixPoint, -6));

                //NJ pissed at player for doing Mutations
                GeoscapeEventDef nJPissedAtMutations = TFTVCommonMethods.CreateNewEvent("NJ_Pissed1", "NJ_PISSED_MUTATIONS_TITLE", "NJ_PISSED_MUTATIONS_TEXT_GENERAL_0", "NJ_PISSED_MUTATIONS_CHOICE_0_OUTCOME");
                nJPissedAtMutations.GeoscapeEventData.Leader = "NJ_TW";
                nJPissedAtMutations.GeoscapeEventData.Choices[0].Text.LocalizationKey = "NJ_PISSED_MUTATIONS_CHOICE_0";
                nJPissedAtMutations.GeoscapeEventData.Choices[0].Outcome.Diplomacy.Add(TFTVCommonMethods.GenerateDiplomacyOutcome(newJericho, phoenixPoint, -5));
                TFTVCommonMethods.GenerateGeoEventChoice(nJPissedAtMutations, "NJ_PISSED_MUTATIONS_CHOICE_1", "NJ_PISSED_MUTATIONS_CHOICE_1_OUTCOME");
                nJPissedAtMutations.GeoscapeEventData.Choices[1].Outcome.Diplomacy.Add(TFTVCommonMethods.GenerateDiplomacyOutcome(newJericho, phoenixPoint, -8));
                nJPissedAtMutations.GeoscapeEventData.Choices[1].Outcome.Diplomacy.Add(TFTVCommonMethods.GenerateDiplomacyOutcome(anu, phoenixPoint, +2));
                TFTVCommonMethods.GenerateGeoEventChoice(nJPissedAtMutations, "NJ_PISSED_MUTATIONS_CHOICE_2", "NJ_PISSED_MUTATIONS_CHOICE_2_OUTCOME");
                nJPissedAtMutations.GeoscapeEventData.Choices[2].Outcome.VariablesChange.Add(TFTVCommonMethods.GenerateVariableChange("BG_NJ_Pissed_Made_Promise", 1, true));

                //NJ really pissed at player for doing Mutations
                GeoscapeEventDef nJReallyPissedAtMutations = TFTVCommonMethods.CreateNewEvent("NJ_Pissed2", "NJ_REALLY_PISSED_MUTATIONS_TITLE", "NJ_REALLY_PISSED_MUTATIONS_TEXT_GENERAL_0", null);
                nJReallyPissedAtMutations.GeoscapeEventData.Leader = "NJ_TW";
                nJReallyPissedAtMutations.GeoscapeEventData.Choices[0].Text.LocalizationKey = "NJ_REALLY_PISSED_MUTATIONS_CHOICE_0";
                nJReallyPissedAtMutations.GeoscapeEventData.Choices[0].Outcome.Diplomacy.Add(TFTVCommonMethods.GenerateDiplomacyOutcome(newJericho, phoenixPoint, -6));
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        //Story events:

        public static void CreateEventFirstFlyer()
        {
            try
            {
                string eventID = "OlenaOnFirstFlyer";
                GeoscapeEventDef newEvent = TFTVCommonMethods.CreateNewEvent(eventID + "_GeoscapeEventDef", "OLENA_ON_FIRST_FLYER_TITLE", "OLENA_ON_FIRST_FLYER_TEXT", null);
                newEvent.GeoscapeEventData.EventID = eventID;

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void CreateEventFirstHavenTarget()
        {
            try
            {
                string eventID = "OlenaOnFirstHavenTarget";
                GeoscapeEventDef newEvent = TFTVCommonMethods.CreateNewEvent(eventID + "_GeoscapeEventDef", "OLENA_ON_FIRST_HAVEN_TARGET_TITLE", "OLENA_ON_FIRST_HAVEN_TARGET_TEXT", null);
                newEvent.GeoscapeEventData.EventID = eventID;

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void CreateEventFirstHavenAttack()
        {
            try
            {
                string eventID = "OlenaOnFirstHavenAttack";
                GeoscapeEventDef newEvent = TFTVCommonMethods.CreateNewEvent(eventID + "_GeoscapeEventDef", "FIRST_HAVEN_ATTACK_TITLE", "FIRST_HAVEN_ATTACK_TEXT", "FIRST_HAVEN_ATTACK_OUTCOME");
                newEvent.GeoscapeEventData.EventID = eventID;

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void CreateEventMessagesFromTheVoid()
        {
            try
            {
                string eventID = "AlistairOnMessagesFromTheVoid";
                GeoscapeEventDef newEvent = TFTVCommonMethods.CreateNewEvent(eventID + "_GeoscapeEventDef", "AFTER_YE_SIGNAL_TITLE", "AFTER_YE_SIGNAL_TEXT", "AFTER_YE_SIGNAL_OUTCOME");
                newEvent.GeoscapeEventData.EventID = eventID;
                newEvent.GeoscapeEventData.Choices[0].Text.LocalizationKey = "AFTER_YE_SIGNAL_CHOICE";

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void CreateBehemothPattern()
        {
            try
            {
                string eventID = "OlenaOnBehemothPattern";
                GeoscapeEventDef newEvent = TFTVCommonMethods.CreateNewEvent(eventID + "_GeoscapeEventDef", "BEHEMOTH_PATTERN_TITLE", "BEHEMOTH_PATTERN_TEXT", null);
                newEvent.GeoscapeEventData.EventID = eventID;

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void CreateTrappedInMist()
        {
            try
            {
                string eventID = "OlenaOnHavenInfested";
                GeoscapeEventDef newEvent = TFTVCommonMethods.CreateNewEvent(eventID + "_GeoscapeEventDef", "OLENA_ON_HAVEN_INFESTED_TITLE", "OLENA_ON_HAVEN_INFESTED_TEXT", null);
                newEvent.GeoscapeEventData.EventID = eventID;

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }
    }

}
