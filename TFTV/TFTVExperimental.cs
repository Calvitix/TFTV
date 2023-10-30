﻿using Base;
using Base.Core;
using Base.Defs;
using Base.Entities;
using Base.Entities.Statuses;
using Base.Serialization;
using Base.UI.MessageBox;
using HarmonyLib;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.Entities.GameTagsTypes;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Common.Game;
using PhoenixPoint.Common.Levels.ActorDeployment;
using PhoenixPoint.Common.Levels.MapGeneration;
using PhoenixPoint.Common.Levels.Missions;
using PhoenixPoint.Common.Saves;
using PhoenixPoint.Common.View.ViewControllers.Inventory;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Events;
using PhoenixPoint.Geoscape.Events.Eventus;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Geoscape.Levels.Factions;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.ActorsInstance;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Statuses;
using PhoenixPoint.Tactical.Entities.StructuralTargets;
using PhoenixPoint.Tactical.Entities.Weapons;
using PhoenixPoint.Tactical.Levels;
using PhoenixPoint.Tactical.Levels.ActorDeployment;
using PhoenixPoint.Tactical.Levels.FactionObjectives;
using PhoenixPoint.Tactical.View.ViewModules;
using PhoenixPoint.Tactical.View.ViewStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static PhoenixPoint.Geoscape.Entities.GeoHaven;

namespace TFTV
{
    internal class TFTVExperimental
    {

        internal static Color purple = new Color32(149, 23, 151, 255);
        private static readonly DefRepository Repo = TFTVMain.Repo;
        private static readonly SharedData Shared = TFTVMain.Shared;
        private static readonly DefCache DefCache = TFTVMain.Main.DefCache;

        public static Dictionary<int, int> AttackedLairSites;

       


       






      

        [HarmonyPatch(typeof(ItemDef), "get_ScrapPrice")]
        public static class ItemDef_get_ScrapPrice_patch
        {
            public static void Postfix(ItemDef __instance, ref ResourcePack __result, ResourcePack ____scrapPrice)
            {
                try
                {

                    if (__instance.Tags.Contains(Shared.SharedGameTags.AmmoTag))
                    {
                        TacticalItemDef tacticalItemDef = __instance as TacticalItemDef;

                        WeaponDef weaponDef = (WeaponDef)AmmoWeaponDatabase.AmmoToWeaponDictionary[tacticalItemDef][0];

                        float costMultiplier = Math.Max(weaponDef.DamagePayload.AutoFireShotCount, 2);
                        __result = new ResourcePack(new ResourceUnit[]
                             {
                        new ResourceUnit(ResourceType.Tech, Mathf.Floor(__instance.ManufactureTech / costMultiplier)),
                        new ResourceUnit(ResourceType.Materials, Mathf.Floor(__instance.ManufactureMaterials / costMultiplier)),
                        new ResourceUnit(ResourceType.Mutagen, Mathf.Floor(__instance.ManufactureMutagen / costMultiplier)),
                        new ResourceUnit(ResourceType.LivingCrystals, Mathf.Floor(__instance.ManufactureLivingCrystals / costMultiplier)),
                        new ResourceUnit(ResourceType.Orichalcum, Mathf.Floor(__instance.ManufactureOricalcum / costMultiplier)),
                        new ResourceUnit(ResourceType.ProteanMutane, Mathf.Floor(__instance.ManufactureProteanMutane / costMultiplier))
                             });


                    }


                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    throw;
                }
            }
        }



        //Code provided by Codemite
        [HarmonyPatch(typeof(UIInventorySlot), "UpdateItem")]
        public static class UIInventorySlot_UpdateItem_patch
        {
            public static void Postfix(UIInventorySlot __instance, ICommonItem ____item)
            {
                try
                {
                    if (____item == null || ____item.CommonItemData.Count == 1 && (____item.CommonItemData.CurrentCharges == ____item.ItemDef.ChargesMax || ____item.CommonItemData.CurrentCharges == 0))
                    {
                        __instance.NumericBackground.gameObject.SetActive(false);
                    }
                    else
                    {
                        __instance.NumericBackground.gameObject.SetActive(true);

                        if (____item.CommonItemData.CurrentCharges == ____item.ItemDef.ChargesMax)
                        {
                            __instance.NumericField.text = ____item.CommonItemData.Count.ToString();
                        }
                        else
                        {
                            string ammoCount = $"{____item.CommonItemData.CurrentCharges}/{____item.ItemDef.ChargesMax}";
                            string textToShow;
                            string greyColor = "<color=#b6b6b6>";

                            if (____item.CommonItemData.Count - 1 == 0)
                            {
                                if (____item.ItemDef.Tags.Contains(Shared.SharedGameTags.AmmoTag))
                                {
                                    textToShow = $"{greyColor}(1) {ammoCount}</color>";
                                }
                                else
                                {
                                    textToShow = $"{greyColor} {ammoCount}</color>";
                                }
                            }
                            else
                            {

                                textToShow = $"{____item.CommonItemData.Count - 1} {greyColor}+ {____item.CommonItemData.CurrentCharges}/{____item.ItemDef.ChargesMax}</color>";
                            }

                            __instance.NumericField.text = textToShow;
                            __instance.NumericField.alignment = TextAnchor.MiddleLeft;
                        }
                    }

                    // return false;
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    throw;
                }
            }
        }

        public static void CheckBCR5Mission(TacticalLevelController controller)
        {
            try
            {

                if (CheckIfMissionISBCR_Rescue(controller))
                {
                    TFTVLogger.Always($"Instantiating rescue VIP objectives on Mission Start, Load or Restart");
                    ChangeRescueeAllegiance(controller);
                    CreateStructuralTargetForObjective(controller);
                    MoveAndCheckStatusOfTalkingPoint(controller);
                }
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }


        //for later, to put all the special missions here
        public static void ImplementSpecialMission(TacticalLevelController controller)
        {
            try
            {


            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
                throw;
            }

        }





        private static bool CheckIfMissionISBCR_Rescue(TacticalLevelController controller)
        {
            try
            {
                CustomMissionTypeDef rescueSparkMisson = DefCache.GetDef<CustomMissionTypeDef>("Bcr5_CustomMissionTypeDef");
                CustomMissionTypeDef rescueFelipeMisson = DefCache.GetDef<CustomMissionTypeDef>("Bcr7_CustomMissionTypeDef");
                CustomMissionTypeDef rescueHelenaMission = DefCache.GetDef<CustomMissionTypeDef>("StoryLE0_CustomMissionTypeDef");

                if (controller.TacMission.MissionData.MissionType == rescueSparkMisson || controller.TacMission.MissionData.MissionType == rescueFelipeMisson || controller.TacMission.MissionData.MissionType == rescueHelenaMission)
                {
                    //TFTVLogger.Always($"The mission is to rescue Mr. Sparks!");
                    return true;
                }
                return false;
            }

            catch (Exception e)
            {
                TFTVLogger.Error(e);
                throw;
            }
        }



        private static void ChangeRescueeAllegiance(TacticalLevelController controller)
        {
            try
            {
                if (CheckIfMissionISBCR_Rescue(controller)) //need another check after Sparks becomes Phoenix
                {
                    TFTVLogger.Always($"The mission is to rescue Mr. Sparks or Felipe and we have to make him Neutral so he doesn't die!");
                    TacticalActor phoenixCivilian = controller.GetFactionByCommandName("px").TacticalActors.FirstOrDefault(a => a.HasGameTag(Shared.SharedGameTags.CivilianTag));

                    if (phoenixCivilian != null)
                    {
                        phoenixCivilian.SetFaction(controller.GetFactionByCommandName("neut"), TacMissionParticipant.Environment);
                    }
                    else
                    {
                        TacticalActor civilian = controller.GetFactionByCommandName("neut").TacticalActors.FirstOrDefault(a => a.HasGameTag(Shared.SharedGameTags.CivilianTag));

                        TriggerAbilityZoneOfControlStatusDef zoneOfControlStatusDef = DefCache.GetDef<TriggerAbilityZoneOfControlStatusDef>("CanBeRecruitedIntoPhoenix_1x1_StatusDef");

                        if (civilian != null && civilian.Status.HasStatus(zoneOfControlStatusDef))
                        {
                            civilian.Status.UnapplyStatus(civilian.Status.GetStatusByName(zoneOfControlStatusDef.EffectName));

                        }

                    }
                }
            }

            catch (Exception e)
            {
                TFTVLogger.Error(e);
                throw;
            }
        }

        private static TacticalActor FindNeutralCivilian(TacticalLevelController controller)
        {

            try
            {
                if (CheckIfMissionISBCR_Rescue(controller))
                {
                    return controller.GetFactionByCommandName("neut").TacticalActors.FirstOrDefault(a => a.HasGameTag(Shared.SharedGameTags.CivilianTag));
                }
                return null;
            }

            catch (Exception e)
            {
                TFTVLogger.Error(e);
                throw;
            }
        }


        private static Vector3 FindSpotForSpawnPoint(TacticalActor tacticalActor, TacticalLevelController controller)
        {
            try
            {
                Vector3 position = tacticalActor.Pos;

                List<Vector3> vector3s = new List<Vector3>()
                {
                    new Vector3(-1f, 0.0f, -1f) +position,
                     new Vector3(1f, 0.0f, -1f) +position,
                      new Vector3(-1f, 0.0f, 1f) +position,
                       new Vector3(1f, 0.0f, 1f) +position,
                       new Vector3(0.0f, 0.0f, -1f) +position,
                          new Vector3(0.0f, 0.0f, 1f) +position,
                          new Vector3(-1f, 0.0f, 0.0f) +position,
                new Vector3(1f, 0.0f, 0.0f) +position,

                };


                TacCharacterDef siren = DefCache.GetDef<TacCharacterDef>("Siren1_Basic_AlienMutationVariationDef");

                ActorDeployData actorDeployData = siren.GenerateActorDeployData();
                actorDeployData.InitializeInstanceData();



                foreach (Vector3 vector3 in vector3s)
                {
                    if (controller.Map.CanStandAt(tacticalActor.NavigationComponent.NavMeshDef, tacticalActor.TacticalPerception.TacticalPerceptionDef, vector3) &&
                        TacticalFactionVision.CheckVisibleLineBetweenActorsInTheory(tacticalActor, tacticalActor.Pos, actorDeployData.ComponentSetDef, vector3))
                    {
                        TFTVLogger.Always($"found suitable position {vector3}");
                        return vector3;

                    }


                }

                return vector3s.GetRandomElement();


            }

            catch (Exception e)
            {
                TFTVLogger.Error(e);
                throw;
            }


        }


        private static void CreateStructuralTargetForObjective(TacticalLevelController controller)
        {
            try
            {

                TFTVLogger.Always($"Creating TalkingPointConsole");
                Vector3 position = FindSpotForSpawnPoint(FindNeutralCivilian(controller), controller);
                string name = "TalkingPoint";

                StructuralTargetTypeTagDef structuralTargetTypeTagDef = DefCache.GetDef<StructuralTargetTypeTagDef>("TalkingPointConsoleTag");

                StructuralTargetDeploymentDef stdDef = DefCache.GetDef<StructuralTargetDeploymentDef>("HackableConsoleStructuralTargetDeploymentDef");

                TacActorData tacActorData = new TacActorData
                {
                    ComponentSetTemplate = stdDef.ComponentSet
                };


                StructuralTargetInstanceData structuralTargetInstanceData = tacActorData.GenerateInstanceData() as StructuralTargetInstanceData;
                structuralTargetInstanceData.SourceTemplate = stdDef;
                structuralTargetInstanceData.Source = tacActorData;


                StructuralTarget structuralTarget = ActorSpawner.SpawnActor<StructuralTarget>(tacActorData.GenerateInstanceComponentSetDef(), structuralTargetInstanceData, callEnterPlayOnActor: false);
                GameObject obj = structuralTarget.gameObject;
                structuralTarget.name = name;
                structuralTarget.Source = obj;
                structuralTarget.GameTags.Add(structuralTargetTypeTagDef);

                var ipCols = new GameObject("InteractionPointColliders");
                ipCols.transform.SetParent(obj.transform);
                ipCols.tag = InteractWithObjectAbilityDef.ColliderTag;

                ipCols.transform.SetPositionAndRotation(position, Quaternion.identity);
                var collider = ipCols.AddComponent<BoxCollider>();


                structuralTarget.Initialize();
                //TFTVLogger.Always($"Spawning interaction point with name {name} at position {position}");
                structuralTarget.DoEnterPlay();

                TFTVLogger.Always($"structural target {name} created at position {position}");



                CheckStatusInteractionPoint(controller, name);
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);

            }


        }

        private static void AdjustObjectives()
        {
            try
            {
                TacticalLevelController controller = GameUtl.CurrentLevel().GetComponent<TacticalLevelController>();
                ObjectivesManager factionObjectives = controller.GetFactionByCommandName("px").Objectives;
                //    ActivateConsoleFactionObjectiveDef convinceCivilianObjectiveDef = DefCache.GetDef<ActivateConsoleFactionObjectiveDef>("ConvinceCivilianObjective");
                WipeEnemyFactionObjective dummyObjective = (WipeEnemyFactionObjective)factionObjectives.FirstOrDefault(obj => obj is WipeEnemyFactionObjective objective);

                TFTVLogger.Always($"dummyObjective is {dummyObjective.Description.LocalizationKey}");

                factionObjectives.Add(dummyObjective.NextOnSuccess[0]);
                factionObjectives.Remove(dummyObjective);
                //   factionObjectives.Add(convinceCivilianObjective);

                //  TFTVLogger.Always($"objective should have been removed");


            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);

            }



        }

        internal static void MoveAndCheckStatusOfTalkingPoint(TacticalLevelController controller)
        {
            try
            {

                AdjustObjectives();
            }

            catch (Exception e)
            {
                TFTVLogger.Error(e);

            }
        }


        /// <summary>
        /// This attempts to assign correct status to objective
        /// </summary>
        internal static void CheckStatusInteractionPoint(TacticalLevelController controller, string name)
        {
            try
            {


                StatusDef activeHackableChannelingStatusDef = DefCache.GetDef<StatusDef>("ConvinceCivilianOnObjectiveStatus");
                StatusDef hackingStatusDef = DefCache.GetDef<StatusDef>("ConvinceCivilianOnActorStatus");
                StatusDef consoleToActorBridgingStatusDef = DefCache.GetDef<StatusDef>("ConvinceCivilianObjectiveToActorBridgeStatus");
                StatusDef actorToConsoleBridgingStatusDef = DefCache.GetDef<StatusDef>("ConvinceCivilianActorToObjectiveBridgeStatus");
                StatusDef activatedStatusDef = DefCache.GetDef<StatusDef>("ConsoleActivated_StatusDef");


                StructuralTarget structuralTarget = UnityEngine.Object.FindObjectsOfType<StructuralTarget>().FirstOrDefault(b => b.name.Equals(name));

                TacticalActor tacticalActor = controller.Map.FindActorOverlapping(structuralTarget.Pos);

                if (tacticalActor != null && tacticalActor.HasStatus(hackingStatusDef))
                {
                    void reflectionSet(TacStatus status, int value)
                    {
                        var prop = status.GetType().GetProperty("TurnApplied", BindingFlags.Public | BindingFlags.Instance);
                        prop.SetValue(status, value);
                    }

                    TacStatus actorBridge = (TacStatus)tacticalActor.Status.GetStatusByName(actorToConsoleBridgingStatusDef.EffectName);
                    int turnApplied = actorBridge.TurnApplied;

                    tacticalActor.Status.UnapplyStatus(actorBridge);
                    //  TFTVLogger.Always($"{actorToConsoleBridgingStatusDef.EffectName} unapplied");
                    TacStatus newTargetBridge = (TacStatus)structuralTarget.Status.ApplyStatus(consoleToActorBridgingStatusDef, tacticalActor.GetActor());
                    TacStatus newActorBridge = (TacStatus)tacticalActor.Status.GetStatusByName(actorToConsoleBridgingStatusDef.EffectName);

                    TFTVLogger.Always($"found {tacticalActor?.DisplayName} trying to convince civillian");

                    reflectionSet(newTargetBridge, turnApplied);
                    reflectionSet(newActorBridge, turnApplied);

                }
                else
                {
                    TFTVLogger.Always($"are we here? has the activated status?{structuralTarget.Status.HasStatus(activatedStatusDef) == true} has the activatable status? {structuralTarget.Status.HasStatus(activeHackableChannelingStatusDef) == true} ");

                    if (!structuralTarget.Status.HasStatus(activatedStatusDef) && !structuralTarget.Status.HasStatus(activeHackableChannelingStatusDef))
                    {
                        structuralTarget.Status.ApplyStatus(activeHackableChannelingStatusDef);//(activeConsoleStatusDef);

                        TFTVLogger.Always($"applying {activeHackableChannelingStatusDef.name} to TalkingPointConsole");

                    }
                }

            }

            catch (Exception e)
            {
                TFTVLogger.Error(e);

            }
        }

        private static void TurnRescueeOverToPhoenix(TacticalLevelController controller)
        {
            try
            {
                TacticalActor sparks = FindNeutralCivilian(controller);
                sparks.SetFaction(controller.GetFactionByCommandName("px"), TacMissionParticipant.Player);
                sparks.ForceRestartTurn();


            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);

            }
        }


        public static void TalkingPointConsoleActivated(StatusComponent statusComponent, Status status, TacticalLevelController controller)
        {
            try
            {
                if (controller != null && CheckIfMissionISBCR_Rescue(controller) && !controller.IsLoadingSavedGame)
                {
                    if (status.Def == DefCache.GetDef<StatusDef>("ConsoleActivated_StatusDef"))
                    {
                        StructuralTarget console = statusComponent.transform.GetComponent<StructuralTarget>();
                        TFTVLogger.Always($"console name {console.name} at position {console.Pos}");

                        TurnRescueeOverToPhoenix(controller);



                    }
                }
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);

            }
        }




        [HarmonyPatch(typeof(GeoMission), "AddCratesToMissionData")]
        public static class GeoMission_AddEquipmentCrates_patch
        {

            public static bool Prefix(GeoMission __instance, TacMissionData missionData, MapPlotDef plotDef)
            {
                try
                {

                    MissionTagDef lairTag = DefCache.GetDef<MissionTagDef>("MissionTypeAlienLairAssault_MissionTagDef");
                    MissionTagDef citadelTag = DefCache.GetDef<MissionTagDef>("MissionTypeAlienLairAssault_MissionTagDef");

                    if (__instance.MissionDef.Tags.Contains(lairTag) || __instance.MissionDef.Tags.Contains(citadelTag))
                    {
                        int siteId = __instance.Site.SiteId;
                        TFTVLogger.Always($"AddEquipmentCrates running on Lair or Citadel Map.");

                        if (AttackedLairSites == null)
                        {
                            AttackedLairSites = new Dictionary<int, int>
                            {
                                { siteId, 1 }
                            };

                        }
                        else
                        {

                            if (AttackedLairSites.ContainsKey(siteId) && AttackedLairSites[siteId] > 0)
                            {
                                MethodInfo addEnvironmentParticipantdMethod = AccessTools.Method(typeof(GeoMission), "AddEnvironmentParticipant");
                                GeoLevelController geoLevel = __instance.Site.GeoLevel;
                                GeoPhoenixFaction phoenixFaction = geoLevel.PhoenixFaction;
                                PPFactionDef environmentFactionDef = __instance.GameController.GetComponent<SharedData>().EnvironmentFactionDef;
                                TacMissionFactionData tacMissionFactionData = (TacMissionFactionData)addEnvironmentParticipantdMethod.Invoke(__instance, new object[] { missionData, environmentFactionDef });
                                if (missionData.MissionType.MissionSpecificCrates != null)
                                {
                                    tacMissionFactionData.InitialDeploymentPoints = Math.Max(__instance.MissionDef.CratesDeploymentPointsRange.RandomValue() - AttackedLairSites[siteId] * 50, 0);
                                    ActorDeployData actorDeployData = missionData.MissionType.MissionSpecificCrates.EquipmentCratesDeployData.Clone();
                                    TacEquipmentCrateDef tacEquipmentCrateDef = actorDeployData.InstanceDef as TacEquipmentCrateDef;
                                    TacEquipmentCrateData tacEquipmentCrateData = new TacEquipmentCrateData
                                    {
                                        ComponentSetTemplate = actorDeployData.InstanceDef.InstanceData.ComponentSetTemplate,
                                        Quantity = tacEquipmentCrateDef.Data.Quantity,
                                        Items = tacEquipmentCrateDef.Data.Items
                                    };
                                    if (missionData.MissionType.FactionItemsRange.Max > 0)
                                    {
                                        List<TacticalItemDef> list = null;
                                        list = ((__instance.Site.Owner.Manufacture == null) ? geoLevel.GetAvailableFactionEquipment(onlyDiscoveredFactions: true) : (from t in __instance.Site.Owner.Manufacture.GetEquipment()
                                                                                                                                                                     select (TacticalItemDef)t.RelatedItemDef).ToList());
                                        if (list.Count != 0)
                                        {
                                            for (int i = 0; i < list.Count(); i++)
                                            {
                                                ItemDef itemDef = list[i];
                                                tacEquipmentCrateData.AdditionalItems.Add(new InventorySupplementComponentDef.ItemChancePair
                                                {
                                                    ChanceToPresent = itemDef.CrateSpawnWeight,
                                                    ItemDef = itemDef
                                                });
                                            }

                                            tacEquipmentCrateData.AdditionalQuantity = missionData.MissionType.FactionItemsRange;
                                        }
                                    }

                                    actorDeployData.InstanceDef = null;
                                    actorDeployData.ActorInstance = tacEquipmentCrateData;
                                    tacMissionFactionData.ActorDeployData.Add(actorDeployData);
                                }

                                AttackedLairSites[siteId] += 1;



                                return false;

                            }
                            else
                            {

                                AttackedLairSites.Add(siteId, 1);
                            }
                        }


                    }
                    return true;

                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    throw;
                }
            }
        }





        public static void CorrrectPhoenixSaveManagerDifficulty()
        {
            try
            {
                PhoenixSaveManager phoenixSaveManager = GameUtl.GameComponent<PhoenixGame>().SaveManager;
                FieldInfo currentDifficultyField = typeof(PhoenixSaveManager).GetField("_currentDifficulty", BindingFlags.NonPublic | BindingFlags.Instance);

                GameDifficultyLevelDef difficulty = (GameDifficultyLevelDef)currentDifficultyField.GetValue(phoenixSaveManager);

                if (difficulty != null)
                {
                    TFTVLogger.Always($"difficulty is {difficulty}");
                }
                else
                {
                    TFTVLogger.Always($"No difficulty set as current difficulty!");


                    GeoLevelController geoLevelController = GameUtl.CurrentLevel().GetComponent<GeoLevelController>();

                    if (geoLevelController != null)
                    {

                        currentDifficultyField.SetValue(phoenixSaveManager, geoLevelController.CurrentDifficultyLevel);

                        GameDifficultyLevelDef newDifficulty = (GameDifficultyLevelDef)currentDifficultyField.GetValue(phoenixSaveManager);


                        TFTVLogger.Always($"Current difficulty set to {newDifficulty?.name}");
                    }
                    else
                    {
                        GameDifficultyLevelDef gameDifficultyLevelDef = null;


                        if (TFTVNewGameOptions.InternalDifficultyCheck != 0)
                        {
                            DefCache.GetDef<GameDifficultyLevelDef>("Easy_GameDifficultyLevelDef").Order = 2;
                            DefCache.GetDef<GameDifficultyLevelDef>("Standard_GameDifficultyLevelDef").Order = 3;
                            DefCache.GetDef<GameDifficultyLevelDef>("Hard_GameDifficultyLevelDef").Order = 4;
                            DefCache.GetDef<GameDifficultyLevelDef>("VeryHard_GameDifficultyLevelDef").Order = 5;

                            switch (TFTVNewGameOptions.InternalDifficultyCheck)
                            {
                                case 1:
                                    gameDifficultyLevelDef = DefCache.GetDef<GameDifficultyLevelDef>("StoryMode_DifficultyLevelDef");
                                    break;

                                case 2:
                                    gameDifficultyLevelDef = DefCache.GetDef<GameDifficultyLevelDef>("Easy_GameDifficultyLevelDef");
                                    break;

                                case 3:
                                    gameDifficultyLevelDef = DefCache.GetDef<GameDifficultyLevelDef>("Standard_GameDifficultyLevelDef");
                                    break;

                                case 4:
                                    gameDifficultyLevelDef = DefCache.GetDef<GameDifficultyLevelDef>("Hard_GameDifficultyLevelDef");
                                    break;

                                case 5:
                                    gameDifficultyLevelDef = DefCache.GetDef<GameDifficultyLevelDef>("VeryHard_GameDifficultyLevelDef");
                                    break;

                                case 6:
                                    gameDifficultyLevelDef = DefCache.GetDef<GameDifficultyLevelDef>("Etermes_DifficultyLevelDef");
                                    break;
                            }
                            currentDifficultyField.SetValue(phoenixSaveManager, geoLevelController.CurrentDifficultyLevel);

                            GameDifficultyLevelDef newDifficulty = (GameDifficultyLevelDef)currentDifficultyField.GetValue(phoenixSaveManager);


                            TFTVLogger.Always($"Current difficulty set to {newDifficulty?.name}");


                        }
                        else
                        {
                            string warning = $"Could not find difficulty! This is a tactical save made before Update# 36. Please load a Geoscape save before this mission; this save is doomed!";

                            GameUtl.GetMessageBox().ShowSimplePrompt(warning, MessageBoxIcon.Warning, MessageBoxButtons.OK, null);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
                throw;
            }



        }



        /*  if (TFTVNewGameOptions.InternalDifficultyCheck != 0) 
          {
              switch  (TFTVNewGameOptions.InternalDifficultyCheck)
              {
                  case 1: return 8;

                  case 2: return 8;

                  case 3: return 7;

                  case 4: return 5;

                  case 5: return 3;

                  case 6: return 1;


                      // { 0: "25%", 1: "50%", 2: "75%", 3: "100%", 4: "125%", 5: "150%", 6: "175%", 7: "200", 8: "250%", 9: "300%", 10 "400%"}

                      //      By default, this is set by the difficulty level: 250% on Rookie, 200% on Veteran, 150% on Hero, 100% on Legend, 50% on ETERMES
              }






          }*/

        [HarmonyPatch(typeof(PhoenixGame), "FinishLevelAndLoadGame")]
        public static class DieAbility_LoadGame_patch
        {

            public static void Prefix(PPSavegameMetaData gameData)
            {
                try
                {


                    if (gameData.DifficultyDef != null)
                    {
                        TFTVLogger.Always($"{gameData?.DifficultyDef}");
                    }
                    else
                    {
                        gameData.DifficultyDef = DefCache.GetDef<GameDifficultyLevelDef>("Etermes_DifficultyLevelDef");
                        TFTVLogger.Always($"{gameData?.DifficultyDef}");
                    }

                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    throw;
                }
            }
        }

        public static void AddReinforcementTagToImplementNoDropsOption(TacticalActorBase actor, TacticalLevelController __instance)
        {
            try
            {
                TFTVConfig config = TFTVMain.Main.Config;

                if (config.ReinforcementsNoDrops && __instance.TurnNumber > 1 && !__instance.IsLoadingSavedGame && actor is TacticalActor tacticalActor)
                {
                    if (tacticalActor.TacticalFaction != __instance.GetFactionByCommandName("PX"))
                    {
                        GameTagDef reinforcementTag = DefCache.GetDef<GameTagDef>("ReinforcementTag_GameTagDef");

                        //  TFTVLogger.Always($"reinforcementTag is {reinforcementTag?.name}");

                        // TFTVLogger.Always("The turn number is " + __instance.TurnNumber);

                        if (!tacticalActor.HasGameTag(reinforcementTag))
                        {
                            tacticalActor?.GameTags?.Add(reinforcementTag);
                            TFTVLogger.Always($"Reinforcement tag added to {actor?.name} {actor.HasGameTag(reinforcementTag)}");

                        }
                    }
                }

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }








        [HarmonyPatch(typeof(DieAbility), "ShouldDestroyItem")]
        public static class DieAbility_ShouldDestroyItem_patch
        {

            public static void Postfix(DieAbility __instance, TacticalItem item, ref bool __result)
            {
                try
                {
                    TFTVConfig config = TFTVMain.Main.Config;
                    GameTagDef reinforcementTag = DefCache.GetDef<GameTagDef>("ReinforcementTag");

                    if (config.ReinforcementsNoDrops && (__instance.TacticalActorBase.HasGameTag(reinforcementTag) || __instance.TacticalActor.DeathInfo.IsAlreadyResurrected))
                    {
                        TFTVLogger.Always($"{__instance?.TacticalActorBase?.name} has reinforcement tag, so should drop no items on death");
                        __result = false;
                    }



                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    throw;
                }
            }
        }



        [HarmonyPatch(typeof(GeoHaven), "IncreaseAlertness")]
        public static class GeoHaven_IncreaseAlertness_patch
        {

            public static bool Prefix(GeoHaven __instance)
            {
                try
                {
                    TFTVConfig config = TFTVMain.Main.Config;

                    if (config.LimitedRaiding)
                    {
                        // Get the type of the GeoHaven class
                        Type geoHavenType = typeof(GeoHaven);

                        // Get the PropertyInfo object representing the AlertLevel property
                        PropertyInfo alertLevelProperty = geoHavenType.GetProperty("AlertLevel", BindingFlags.Public | BindingFlags.Instance);

                        // Set the value of the AlertLevel property using reflection
                        if (alertLevelProperty != null && alertLevelProperty.CanWrite)
                        {
                            alertLevelProperty.SetValue(__instance, HavenAlertLevel.HighAlert);
                        }

                        __instance.AlertCooldownDaysLeft = 7;
                    }

                    return false;
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    throw;
                }
            }
        }








        /*  public static int KludgeStartingWeight = 0;
          public static int KludgeCurrentWeight = 0;


          [HarmonyPatch(typeof(UIStateInventory), "RefreshUI")]
          public static class UIStateInventory_RefreshUI_patch
          {
              public static void Postfix(UIStateInventory __instance, TacticalActor ____secondaryActor)
              {
                  try
                  {
                      TFTVLogger.Always($"RefreshUI");

                      MethodInfo refreshCostMessageMethod = typeof(UIStateInventory).GetMethod("RefreshCostMessage", BindingFlags.Instance | BindingFlags.NonPublic);

                      ApplyStatusAbilityDef rfAAbility = DefCache.GetDef<ApplyStatusAbilityDef>("ReadyForAction_AbilityDef");
                      ChangeAbilitiesCostStatusDef rfAStatus = DefCache.GetDef<ChangeAbilitiesCostStatusDef>("E_ReadyForActionStatus [ReadyForAction_AbilityDef]");

                      if (KludgeStartingWeight == KludgeCurrentWeight && ____secondaryActor!=null)
                      {                       
                          if (__instance.PrimaryActor.GetAbilityWithDef<ApplyStatusAbility>(rfAAbility) != null && !__instance.PrimaryActor.HasStatus(rfAStatus))
                          {
                              __instance.PrimaryActor.Status.ApplyStatus(rfAStatus);
                              TFTVLogger.Always($"{__instance.PrimaryActor.name} has the RfA ability but is missing the status, personal inventory case");
                              refreshCostMessageMethod.Invoke(__instance, null);

                          }
                      }
                      else if(KludgeStartingWeight > KludgeCurrentWeight && ____secondaryActor != null && __instance.PrimaryActor.HasStatus(rfAStatus)) 
                      {

                          __instance.PrimaryActor.Status.UnapplyStatus(__instance.PrimaryActor.Status.GetStatusByName(rfAStatus.EffectName));
                          TFTVLogger.Always($"{__instance.PrimaryActor.name} has the RfA status, but is taking items away from inventory");
                          refreshCostMessageMethod.Invoke(__instance, null);

                      }


                      //  TFTVLogger.Always($"KludgeCurrentWeight is {KludgeCurrentWeight}, will set to 0");
                      //  KludgeCurrentWeight = 0;
                  }
                  catch (Exception e)
                  {
                      TFTVLogger.Error(e);
                      throw;
                  }
              }
          }
        */


        /*    [HarmonyPatch(typeof(UIStateInventory), "InitInventory")]
            public static class UIStateInventory_InitInventory_patch
            {

                public static bool Prefix(UIStateInventory __instance, TacticalActor ____secondaryActor)
                {
                    try
                    {
                        if (____secondaryActor == KludgeActor)
                        {

                            TFTVLogger.Always($"InitInventory prefix {____secondaryActor?.DisplayName}");

                            return false;
                        }
                        else
                        {

                            return true;

                        }
                    }
                    catch (Exception e)
                    {
                        TFTVLogger.Error(e);
                        throw;
                    }
                }
            }


            [HarmonyPatch(typeof(UIStateInventory), "InitVehicleInventory")]
            public static class UIStateInventory_InitVehicleInventory_patch
            {

                public static bool Prefix(UIStateInventory __instance, TacticalActor ____secondaryActor)
                {
                    try
                    {
                        if (____secondaryActor == KludgeActor)
                        {

                            TFTVLogger.Always($"InitVehicleInventory prefix {____secondaryActor?.DisplayName}");

                            return false;
                        }
                        else 
                        {

                            return true;

                        }
                    }
                    catch (Exception e)
                    {
                        TFTVLogger.Error(e);
                        throw;
                    }
                }
            }*/

        /*   [HarmonyPatch(typeof(UIStateInventory), "ExitState")]
           public static class UIStateInventory_RefreshCostMessage_patch
           {
               public static void Postfix(UIStateInventory __instance)
               {
                   try
                   {
                      // TFTVLogger.Always($"Exit State");
                       ApplyStatusAbilityDef rfAAbility = DefCache.GetDef<ApplyStatusAbilityDef>("ReadyForAction_AbilityDef");
                       ChangeAbilitiesCostStatusDef rfAStatus = DefCache.GetDef<ChangeAbilitiesCostStatusDef>("E_ReadyForActionStatus [ReadyForAction_AbilityDef]");

                       if (__instance.PrimaryActor.GetAbilityWithDef<ApplyStatusAbility>(rfAAbility) != null && !__instance.PrimaryActor.HasStatus(rfAStatus))
                       {
                           __instance.PrimaryActor.Status.ApplyStatus(rfAStatus);
                           TFTVLogger.Always($"{__instance.PrimaryActor.name} has the RfA ability but is missing the status");
                       }

                    //   KludgeCurrentWeight = 0;
                    //   KludgeStartingWeight = 0;

                   }
                   catch (Exception e)
                   {
                       TFTVLogger.Error(e);
                       throw;
                   }
               }
           }*/



        /*  [HarmonyPatch(typeof(UIModuleSoldierEquip), "GetPrimaryWeight")]
          public static class UIModuleSoldierEquip_GetPrimaryWeight_patch
          {
              public static void Postfix(UIModuleSoldierEquip __instance, int __result)
              {
                  try

                  {
                      TFTVLogger.Always($"GetPrimaryWeight");
                      TFTVLogger.Always($"kludgeWeight is {KludgeStartingWeight} and result is {__result}");

                      if (KludgeStartingWeight != 0)
                      {
                          KludgeCurrentWeight = __result;
                          TFTVLogger.Always($"setting from GetPrimaryWeight KludgeCurrentWeight to {KludgeCurrentWeight}");
                      }
                  }
                  catch (Exception e)
                  {
                      TFTVLogger.Error(e);
                      throw;
                  }
              }
          }*/




        /*
        [HarmonyPatch(typeof(UIStateInventory), "EnterState")]
         public static class UIStateInventory_EnterState_patch
         {
             public static void Postfix(UIStateInventory __instance, TacticalActor ____secondaryActor, InventoryComponent ____groundInventory, bool ____isSecondaryVehicleInventory)
             {
                 try
                 {

                     TFTVLogger.Always($"primary actor is {__instance.PrimaryActor.DisplayName}, groundInventory actor is {____groundInventory?.Actor?.name}, is secondary vehicle inventory: {____isSecondaryVehicleInventory}");

                     if(____groundInventory.Actor is TacticalActor actor && actor == ____secondaryActor) 
                     {
                        MethodInfo methodCreateGroundInventory = typeof(UIStateInventory).GetMethod("CreateGroundInventory", BindingFlags.Instance | BindingFlags.NonPublic);

                        MethodInfo methodResetInventoryQueries = typeof(UIStateInventory).GetMethod("ResetInventoryQueries", BindingFlags.Instance | BindingFlags.NonPublic);

                        MethodInfo methodRefreshStorageLabel = typeof(UIStateInventory).GetMethod("RefreshStorageLabel", BindingFlags.Instance | BindingFlags.NonPublic);

                        MethodInfo methodInitInitialItems = typeof(UIStateInventory).GetMethod("InitInitialItems", BindingFlags.Instance | BindingFlags.NonPublic);

                        MethodInfo methodSetupGroundMarkers = typeof(UIStateInventory).GetMethod("SetupGroundMarkers", BindingFlags.Instance | BindingFlags.NonPublic);


                        ____groundInventory = (InventoryComponent)methodCreateGroundInventory.Invoke(__instance, null);

                        methodResetInventoryQueries.Invoke(__instance, null);
                        methodSetupGroundMarkers.Invoke(__instance, null);
                        methodRefreshStorageLabel.Invoke(__instance, null);
                        methodInitInitialItems.Invoke(__instance, null);


                        //
                        // __instance.ResetInventoryQueries();
                        TFTVLogger.Always($"ground inventory set active to false");

                     }

                     TFTVLogger.Always($"{__instance.PrimaryActor.GetAbility<InventoryAbility>()?.TacticalAbilityDef?.name}");

                     foreach (TacticalAbilityTarget target in __instance.PrimaryActor.GetAbility<InventoryAbility>().GetTargets())
                     {
                         InventoryComponent inventoryComponent = target.InventoryComponent;

                         TFTVLogger.Always($"inventory component {inventoryComponent?.name}");

                         if (inventoryComponent.GetType() != typeof(EquipmentComponent))
                         {
                             TFTVLogger.Always($" {inventoryComponent?.name} is no equipmentComponent");

                             TacticalActor tacticalActor = inventoryComponent.Actor as TacticalActor;
                             if (tacticalActor != null && TacUtil.CanTradeWith(__instance.PrimaryActor, tacticalActor))
                             {
                                 TFTVLogger.Always($"{__instance.PrimaryActor.DisplayName} can trade with {tacticalActor.DisplayName}");
                                 InventoryAbility ability = tacticalActor.GetAbility<InventoryAbility>();
                                 if (ability != null && !(ability.GetDisabledState() != AbilityDisabledState.NotDisabled))
                                 {
                                     TFTVLogger.Always($"{tacticalActor.DisplayName} inventory ability is not null");
                                 }
                             }


                         }
                     }


                 }
                 catch (Exception e)
                 {
                     TFTVLogger.Error(e);
                     throw;
                 }
             }
         }
        */











        /*   [HarmonyPatch(typeof(UIModuleWeaponSelection), "HandleEquipments")]
            public static class UIModuleWeaponSelection_HandleEquipments_patch
           {
               public static void Postfix(UIModuleWeaponSelection __instance, Equipment ____selectedEquipment)
               {
                   try
                   {
                       EquipmentDef repairKit = DefCache.GetDef<EquipmentDef>("FieldRepairKit_EquipmentDef");

                       if (____selectedEquipment.EquipmentDef==repairKit) 
                       {
                           TFTVLogger.Always("got here");
                           __instance.DamageTypeVisualsTemplate.DamageTypeIcon.gameObject.SetActive(false);
                           __instance.DamageTypeVisualsTemplate.DamageText.gameObject.SetActive(false);


                       }



                   }
                   catch (Exception e)
                   {
                       TFTVLogger.Error(e);
                       throw;
                   }
               }
           }*/





        // UIModuleAbilities




        /* [HarmonyPatch(typeof(TacticalActorBase), "GetDamageMultiplierFor")]
         public static class TacticalActorBase_GetDamageMultiplierFor_patch
         {
             public static void Postfix(TacticalActorBase __instance, ref float __result, DamageTypeBaseEffectDef damageType)
             {
                 try
                 {
                     AcidDamageTypeEffectDef acidDamageTypeEffectDef = DefCache.GetDef<AcidDamageTypeEffectDef>("Acid_DamageOverTimeDamageTypeEffectDef");

                     if (damageType == acidDamageTypeEffectDef)
                     {
                         TFTVLogger.Always($"GetDamageMultiplierFor  {__instance.name} and result is {__result}");
                         __result = 1;
                     }

                 }
                 catch (Exception e)
                 {
                     TFTVLogger.Error(e);
                     throw;
                 }
             }
         }*/


        /*  [HarmonyPatch(typeof(DamageAccumulation), "GetPureDamageBonusFor")]
          public static class DamageAccumulation_GetPureDamageBonusFor_patch
          {
              public static void Postfix(DamageAccumulation __instance, IDamageReceiver target, float __result)
              {
                  try
                  {
                      if (__result != 0)
                      {

                          TFTVLogger.Always($"GetPureDamageBonusFor {target.GetDisplayName()}, result is {__result}");
                      }

                  }
                  catch (Exception e)
                  {
                      TFTVLogger.Error(e);
                      throw;
                  }
              }
          }*/





        /*     [HarmonyPatch(typeof(AddStatusDamageKeywordData), "ProcessKeywordDataInternal")]
          public static class AddStatusDamageKeywordData_ProcessKeywordDataInternal_Patch
          {
              public static void Postfix(AddStatusDamageKeywordData __instance, DamageAccumulation.TargetData data)
              {
                  try
                  {
                      if (__instance.DamageKeywordDef == Shared.SharedDamageKeywords.AcidKeyword)
                      {
                          TFTVLogger.Always($"target {data.Target.GetSlotName()}");

                          if (data.Target is ItemSlot)
                          {

                              ItemSlot itemSlot = (ItemSlot) data.Target;

                              if (itemSlot.DisplayName == "LEG")
                              {
                                  TacticalActor tacticalActor = data.Target.GetActor() as TacticalActor;

                                  itemSlot = tacticalActor.BodyState.GetSlot("Legs");
                                  TFTVLogger.Always($"itemslot name now {itemSlot.GetSlotName()}");
                                  TacticalItem tacticalItem = itemSlot.GetAllDirectItems(onlyBodyparts: true).FirstOrDefault();
                                  if (tacticalItem != null && tacticalItem.GameTags.Contains(Shared.SharedGameTags.BionicalTag))
                                  {
                                      TFTVLogger.Always($"Found bionic item {tacticalItem.DisplayName}");
                                      data.Target.GetActor().RemoveAbilitiesFromSource(tacticalItem);
                                      // SlotStateStatusDef source = DefCache.GetDef<SlotStateStatusDef>("DisabledElectronicsAcidSlot_StatusDef");

                                  }
                              }
                              else
                              {
                                  TFTVLogger.Always($"target {data.Target.GetSlotName()} is itemslot {itemSlot.DisplayName}");

                                  TacticalItem tacticalItem = itemSlot.GetAllDirectItems(onlyBodyparts: true).FirstOrDefault();
                                  if (tacticalItem != null && tacticalItem.GameTags.Contains(Shared.SharedGameTags.BionicalTag))
                                  {
                                      TFTVLogger.Always($"Found bionic item {tacticalItem.DisplayName}");
                                      data.Target.GetActor().RemoveAbilitiesFromSource(tacticalItem);
                                      // SlotStateStatusDef source = DefCache.GetDef<SlotStateStatusDef>("DisabledElectronicsAcidSlot_StatusDef");

                                  }
                              }

                          }


                      }

                  }
                  catch (Exception e)
                  {
                      TFTVLogger.Error(e);
                      throw;
                  }
              }
          }

          */




        /*   [HarmonyPatch(typeof(DamageKeyword), "AddKeywordStatus")]
             public static class DamageOverTimeResistanceStatus_ApplyResistance_Patch
             {
                 public static void Postfix(IDamageReceiver recv, DamageAccumulation.TargetData data, StatusDef statusDef, int value, object customStatusTarget = null)
                 {
                     try
                     {


                       TFTVLogger.Always($"AddKeywordStatus value {value}");


                     }
                     catch (Exception e)
                     {
                         TFTVLogger.Error(e);
                         throw;
                     }
                 }
             }*/




        /* [HarmonyPatch(typeof(DamageAccumulation), "AddTargetStatus")]
         public static class DamageAccumulation_AddTargetStatus_Patch
         {
             public static void Prefix(DamageAccumulation __instance, StatusDef statusDef, int tacStatusValue, IDamageReceiver target)
             {
                 try
                 {


                     if (statusDef == DefCache.GetDef<AcidStatusDef>("Acid_StatusDef"))
                     {




                         TFTVLogger.Always($"tacstatusvalue is {tacStatusValue}");
                     }


                 }
                 catch (Exception e)
                 {
                     TFTVLogger.Error(e);
                     throw;
                 }
             }
         }*/



        /*  [HarmonyPatch(typeof(DamageOverTimeStatus), "GetDamageMultiplier")]
          public static class DamageOverTimeStatus_GetDamageMultiplier_Patch
          {
              public static void Postfix(DamageOverTimeStatus __instance, ref float __result)
              {
                  try
                  {
                      TFTVLogger.Always($"GetDamageMultiplier for {__instance.DamageOverTimeStatusDef.name} and result is {__result}");

                      AcidStatusDef acidDamage = DefCache.GetDef<AcidStatusDef>("Acid_StatusDef");

                      if (__instance.DamageOverTimeStatusDef == acidDamage) 
                      {
                          TFTVLogger.Always($"dot status acid {__result}");
                          __result = 1;
                          TFTVLogger.Always($"new dot status acid {__result}");
                      }


                  }
                  catch (Exception e)
                  {
                      TFTVLogger.Error(e);
                      throw;
                  }
              }
          }*/

        /*  [HarmonyPatch(typeof(DamageAccumulation), "GetSourceDamageMultiplier")]
          public static class DamageAccumulation_GetSourceDamageMultiplier_Patch
          {
              public static void Postfix(DamageAccumulation __instance, DamageTypeBaseEffectDef damageType, float __result)
              {
                  try
                  {
                      if (!damageType.name.Equals("Projectile_StandardDamageTypeEffectDef"))
                          {

                          TFTVLogger.Always($"source actor {__instance?.SourceActor?.name} damageType is {damageType.name} and multiplier is {__result}");
                      }


                  }
                  catch (Exception e)
                  {
                      TFTVLogger.Error(e);
                      throw;
                  }
              }
          }*/

        /* [HarmonyPatch(typeof(Equipment), "SetActive")]
         public static class Equipment_RemoveAbilitiesFromSource_Patch
         {
             public static void Postfix(Equipment __instance, bool active)
             {
                 try
                 {
                     TFTVLogger.Always($"equipment is {__instance.DisplayName}, and is it active? {active}");




                 }
                 catch (Exception e)
                 {
                     TFTVLogger.Error(e);
                     throw;
                 }
             }
         }*/



        /*   [HarmonyPatch(typeof(TacticalItem), "RemoveAbilitiesFromActor")]
           public static class TacticalItem_RemoveAbilitiesFromActor_patch
           {
               public static void Prefix(TacticalItem __instance)
               {
                   try
                   {
                       TFTVLogger.Always($"RemoveAbilitiesFromActor from item {__instance.ItemDef.name}");


                   }
                   catch (Exception e)
                   {
                       TFTVLogger.Error(e);
                       throw;
                   }
               }
           }*/



        /*   [HarmonyPatch(typeof(ActorComponent), "RemoveAbilitiesFromSource")]
           public static class ActorComponent_RemoveAbilitiesFromSource_patch
           {
               public static void Prefix(ActorComponent __instance, object source)
               {
                   try
                   {
                       TFTVLogger.Always($"RemoveAbilitiesFromSource from {__instance.name} with source {source}");

                       foreach (Ability item in __instance.GetAbilities<Ability>().Where((Ability a) => a.Source == source).ToList())
                       {
                           TFTVLogger.Always($"ability is {item.AbilityDef.name} and it's source is {item.Source}, while parameter source is {source}");
                       }

                   }
                   catch (Exception e)
                   {
                       TFTVLogger.Error(e);
                       throw;
                   }
               }
           }*/



        /*
                [HarmonyPatch(typeof(SlotStateStatus), "SetAbilitiesState")]
                public static class SlotStateStatus_GetDamageMultiplierFor_patch
                {
                    public static void Prefix(SlotStateStatus __instance, ItemSlot ____targetSlot)
                    {
                        try
                        {
                            TFTVLogger.Always($"Gets at least to here {__instance.Source}");

                            foreach (TacticalItem allDirectItem in ____targetSlot.GetAllDirectItems(onlyBodyparts: true))
                            {
                                if (__instance.SlotStateStatusDef.BodypartsEnabled && !allDirectItem.Enabled)
                                {
                                    TFTVLogger.Always($"landed here: looking at {allDirectItem.ItemDef.name}");
                                }
                                else if (!__instance.SlotStateStatusDef.BodypartsEnabled && allDirectItem.Enabled)
                                {
                                    TFTVLogger.Always($"landed in the else if: looking at {allDirectItem.ItemDef.name}");
                                }


                            }
                        }
                        catch (Exception e)
                        {
                            TFTVLogger.Error(e);
                            throw;
                        }
                    }
                }*/

        /*    [HarmonyPatch(typeof(AbilitySummaryData), "ProcessHealAbilityDef")]
            public static class AbilitySummaryData_ProcessHealAbilityDef_Patch
            {
                public static void Postfix(AbilitySummaryData __instance, HealAbilityDef healAbilityDef)
                {
                    try
                    {
                        TFTVLogger.Always($"ProcessHealAbilityDef running");
                        if ((bool)healAbilityDef.GeneralHealSummary && healAbilityDef.GeneralHealAmount > 0f)
                        {
                            TFTVLogger.Always($"{healAbilityDef.GeneralHealSummary} and {healAbilityDef.GeneralHealAmount}");

                        }

                        TFTVLogger.Always($"Keywords count is {__instance.Keywords.Count}");

                    }
                    catch (Exception e)
                    {
                        TFTVLogger.Error(e);
                        throw;
                    }
                }
            }

            [HarmonyPatch(typeof(AbilitySummaryData), "ProcessHealAbility")]
            public static class AbilitySummaryData_ProcessHealAbility_Patch
            {
                public static void Prefix(AbilitySummaryData __instance, HealAbility healAbility)
                {
                    try
                    {
                        TFTVLogger.Always($"ProcessHealAbility running");
                        TFTVLogger.Always($"Keywords count is {__instance.Keywords.Count}");

                        if (__instance.Keywords.Count() > 0)
                        {
                            KeywordData keywordData = __instance.Keywords.First((KeywordData kd) => kd.Id == "GeneralHeal");

                            if (keywordData == null)
                            {
                                TFTVLogger.Always("somehow null!");
                            }
                        }



                    }
                    catch (Exception e)
                    {
                        TFTVLogger.Error(e);
                        throw;
                    }
                }
            }
        */


        /*   internal virtual void TriggerHurt(DamageResult damageResult)
           {
               var hurtReactionAbility = GetAbility<TacticalHurtReactionAbility>();
               if (IsDead || (hurtReactionAbility != null && hurtReactionAbility.TacticalHurtReactionAbilityDef.TriggerOnDamage && hurtReactionAbility.IsEnabled(IgnoredAbilityDisabledStatesFilter.IgnoreNoValidTargetsFilter)))
               {
                   return;
               }

               bool useModFlinching = true; // Use a global flag for the mod 
               if (useModFlinching && _ragdollDummy != null && _ragdollDummy.CanFlinch)
               {
                   DoTriggerHurt(damageResult, damageResult.forceHurt);
                   return;
               }

               _pendingHurtDamage = damageResult;
               if (_waitingForHurtReactionCrt == null || _waitingForHurtReactionCrt.Stopped)
               {
                   _waitingForHurtReactionCrt = Timing.Start(PollForPendingHurtReaction(damageResult.forceHurt));
               }
           }*/


        /*
        [HarmonyPatch(typeof(TacticalActor), "TriggerHurt")]
        public static class TacticalActor_TriggerHurt_Patch
        {
            public static bool Prefix(TacticalActor __instance, DamageResult damageResult, RagdollDummy ____ragdollDummy, IUpdateable ____waitingForHurtReactionCrt,
                DamageResult ____pendingHurtDamage)
            {
                try
                {


                    MethodInfo doTriggerHurtMethod = typeof(TacticalActor).GetMethod("DoTriggerHurt", BindingFlags.NonPublic | BindingFlags.Instance);
                    MethodInfo pollForPendingHurtReaction = typeof(TacticalActor).GetMethod("PollForPendingHurtReaction", BindingFlags.NonPublic | BindingFlags.Instance); 



                 var hurtReactionAbility = __instance.GetAbility<TacticalHurtReactionAbility>();



                    if (__instance.IsDead || (hurtReactionAbility != null && hurtReactionAbility.TacticalHurtReactionAbilityDef.TriggerOnDamage && hurtReactionAbility.IsEnabled(IgnoredAbilityDisabledStatesFilter.IgnoreNoValidTargetsFilter)))
                    {
                        TFTVLogger.Always("Early exit triggers");
                        return true;
                    }

                    bool useModFlinching = true; // Use a global flag for the mod 
                    if (useModFlinching && ____ragdollDummy != null && ____ragdollDummy.CanFlinch)
                    {
                        doTriggerHurtMethod.Invoke(__instance, new object[] { damageResult, damageResult.forceHurt });
                        TFTVLogger.Always("Takes to do trigger hurt method");

                        return false;
                    }

                    ____pendingHurtDamage = damageResult;
                    if (____waitingForHurtReactionCrt == null || ____waitingForHurtReactionCrt.Stopped)
                    {
                        TFTVLogger.Always("waiting for hurt reaction or it is stopped");
                        object[] parameters = new object[] { damageResult.forceHurt };
                        //Timing timingInstance = new Timing();
                        ____waitingForHurtReactionCrt = __instance.Timing.Start((IEnumerator<NextUpdate>)pollForPendingHurtReaction.Invoke(__instance, parameters));

                    }


                    return false;

                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    throw;
                }
            }
        }




        [HarmonyPatch(typeof(TacticalActor), "SetFlinchingEnabled")]
        public static class TacticalActor_AddFlinch_Patch
        {
            public static void Postfix(TacticalActor __instance, ref RagdollDummy ____ragdollDummy)
            {
                try
                {
                    TFTVLogger.Always($"SetFlinchingEnabled invoked");
                    ____ragdollDummy.SetFlinchingEnabled(true);

                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    throw;
                }
            }
        }


        [HarmonyPatch(typeof(RagdollDummy), "AddFlinch")]
        public static class RagdollDummy_AddFlinch_Patch
        {
            public static void Prefix(RagdollDummy __instance, float ____ragdollBlendTimeTotal)
            {
                try
                {
                    TFTVLogger.Always($"AddFlinch invoked prefix, ragdollBlendtimeTotal is {____ragdollBlendTimeTotal}");


                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    throw;
                }
            }
            public static void Postfix(RagdollDummy __instance, float ____ragdollBlendTimeTotal, Vector3 force, CastHit hit)
            {
                try
                {
                    RagdollDummyDef ragdollDummyDef = DefCache.GetDef<RagdollDummyDef>("Generic_RagdollDummyDef");
                    TFTVLogger.Always($"AddFlinch invoked postfix, ragdollBlendtimeTotal is {____ragdollBlendTimeTotal}. original force is {force}, the hit body part is {hit.Collider?.attachedRigidbody?.name}" +
                        $" mass is {hit.Collider?.attachedRigidbody?.mass}, force applied on first hit is {force*ragdollDummyDef.FlinchForceMultiplier}");







                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    throw;
                }
            }
        }



        [HarmonyPatch(typeof(RagdollDummy), "get_CanFlinch")]
        public static class RagdollDummy_SetFlinchingEnabled_Patch
        {
            public static void Postfix(RagdollDummy __instance, ref bool __result)
            {
                try
                {
                    TFTVLogger.Always($"get_CanFlinch invoked for {__instance?.Actor?.name} and result is {__result}");

                    __result = true;

                    TFTVLogger.Always($"And now result is {__result}");




                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    throw;
                }
            }
        }
        */




        /*private bool OnProjectileHit(CastHit hit, Vector3 dir)
        {
            if (hit.Collider.gameObject.CompareTag("WindowPane"))
            {
                return false;
            }

            if (Projectile != null)
            {
                Projectile.OnProjectileHit(hit);
            }

            AffectTarget(hit, dir);
            if (DamagePayload.StopOnFirstHit)
            {
                return true;
            }

            if (DamagePayload.StopWhenNoRemainingDamage)
            {
                DamageAccumulation damageAccum = _damageAccum;
                return damageAccum == null || !damageAccum.HasRemainingDamage;
            }

            _damageAccum?.ResetToInitalAmount();
            return false;
        }*/







        public static Vector3 FindPushToTile(TacticalActor attacker, TacticalActor defender, int numTiles)
        {

            try
            {


                Vector3 diff = defender.Pos - attacker.Pos;
                Vector3 pushToPosition = defender.Pos + numTiles * diff.normalized;

                // TFTVLogger.Always($"attacker position is {attacker.Pos} and defender position is {defender.Pos}, so difference is {diff} and pushtoposition is {pushToPosition}");



                return pushToPosition;

            }


            catch (Exception e)
            {
                TFTVLogger.Error(e);
                throw;
            }
        }

        /*  [HarmonyPatch(typeof(TacticalActor), "OnAbilityExecuteFinished")]

          public static class TacticalActor_OnAbilityExecuteFinished_KnockBack_Experiment_patch
          {
              public static void Prefix(TacticalAbility ability, TacticalActor __instance, object parameter)
              {
                  try
                  {
                      TFTVLogger.Always($"ability {ability.TacticalAbilityDef.name} executed by {__instance.DisplayName}");

                      RepositionAbilityDef knockBackAbility = DefCache.GetDef<RepositionAbilityDef>("KnockBackAbility");
                      BashAbilityDef strikeAbility = DefCache.GetDef<BashAbilityDef>("BashStrike_AbilityDef");
                      if (ability.TacticalAbilityDef != null && ability.TacticalAbilityDef == strikeAbility)
                      {
                          if (parameter is TacticalAbilityTarget abilityTarget && abilityTarget.GetTargetActor() != null)
                          {
                              TFTVLogger.Always($"got here, target is {abilityTarget.GetTargetActor()}");

                              TacticalActor tacticalActor = abilityTarget.GetTargetActor() as TacticalActor;

                              if (tacticalActor != null)
                              {
                                  tacticalActor.AddAbility(knockBackAbility, tacticalActor);
                                     TFTVLogger.Always($"got here, added {knockBackAbility.name} to {tacticalActor.name}");
                              }
                          }
                      }
                  }

                  catch (Exception e)
                  {
                      TFTVLogger.Error(e);
                  }

              }

              public static void Postfix(TacticalAbility ability, TacticalActor __instance, object parameter)
              {
                  try
                  {
                      RepositionAbilityDef knockBackAbility = DefCache.GetDef<RepositionAbilityDef>("KnockBackAbility");
                      BashAbilityDef strikeAbility = DefCache.GetDef<BashAbilityDef>("BashStrike_AbilityDef");

                      if (ability.TacticalAbilityDef != null && ability.TacticalAbilityDef == strikeAbility)
                      {
                             TFTVLogger.Always($"got here, ability is {ability.TacticalAbilityDef.name}");

                          if (parameter is TacticalAbilityTarget abilityTarget && abilityTarget.GetTargetActor() != null)
                          {

                              TFTVLogger.Always($"got here, target is {abilityTarget.GetTargetActor()}");

                              TacticalActor tacticalActor = abilityTarget.GetTargetActor() as TacticalActor;



                              if (tacticalActor != null && tacticalActor.GetAbilityWithDef<RepositionAbility>(knockBackAbility) != null && tacticalActor.IsAlive)
                              {
                                  RepositionAbility knockBack = tacticalActor.GetAbilityWithDef<RepositionAbility>(knockBackAbility);

                                  IEnumerable<TacticalAbilityTarget> targets = knockBack.GetTargets();

                                  TacticalAbilityTarget pushPosition = new TacticalAbilityTarget();
                                  TacticalAbilityTarget attack = parameter as TacticalAbilityTarget;

                                  foreach (TacticalAbilityTarget target in targets)
                                  {
                                      // TFTVLogger.Always($"possible position {target.PositionToApply} and magnitude is {(target.PositionToApply - FindPushToTile(__instance, tacticalActor)).magnitude} ");

                                      if ((target.PositionToApply - FindPushToTile(__instance, tacticalActor, 2)).magnitude <= 1f)
                                      {
                                          TFTVLogger.Always($"chosen position {target.PositionToApply}");

                                          pushPosition = target;

                                      }
                                  }


                                  //  MoveAbilityDef moveAbilityDef = DefCache.GetDef<MoveAbilityDef>("Move_AbilityDef");

                                  //  MoveAbility moveAbility = tacticalActor.GetAbilityWithDef<MoveAbility>(moveAbilityDef);
                                  //  moveAbility.Activate(pushPosition);

                                  knockBack.Activate(pushPosition);



                                  TFTVLogger.Always($"knocback executed position should be {pushPosition.GetActorOrWorkingPosition()}");

                              }
                          }
                      }

                      if (ability.TacticalAbilityDef == knockBackAbility)
                      {
                          __instance.RemoveAbility(ability);

                      }
                  }

                  catch (Exception e)
                  {
                      TFTVLogger.Error(e);
                  }

              }

          }

          */


        /* [HarmonyPatch(typeof(TacticalActor), "OnAbilityExecuteFinished")]

           public static class TacticalActor_OnAbilityExecuteFinished_KnockBack_Experiment_patch
           {
               public static void Prefix(TacticalAbility ability, TacticalActor __instance, object parameter)
               {
                   try
                   {
                      // TFTVLogger.Always($"ability {ability.TacticalAbilityDef.name} executed by {__instance.DisplayName}");

                       JetJumpAbilityDef knockBackAbility = DefCache.GetDef<JetJumpAbilityDef>("KnockBackAbility");
                       BashAbilityDef strikeAbility = DefCache.GetDef<BashAbilityDef>("BashStrike_AbilityDef");
                       if (ability.TacticalAbilityDef!=null && ability.TacticalAbilityDef == strikeAbility)
                       {
                           if (parameter is TacticalAbilityTarget abilityTarget && abilityTarget.GetTargetActor() != null)
                           {

                           //    TFTVLogger.Always($"got here, target is {abilityTarget.GetTargetActor()}");

                               TacticalActor tacticalActor = abilityTarget.GetTargetActor() as TacticalActor;

                               if (tacticalActor != null)
                               {
                                   tacticalActor.AddAbility(knockBackAbility, tacticalActor);
                                //   TFTVLogger.Always($"got here, added {knockBackAbility.name} to {tacticalActor.name}");
                               }
                           }
                       }
                   }

                   catch (Exception e)
                   {
                       TFTVLogger.Error(e);
                   }

               }

               public static void Postfix(TacticalAbility ability, TacticalActor __instance, object parameter)
               {
                   try
                   {


                       JetJumpAbilityDef knockBackAbility = DefCache.GetDef<JetJumpAbilityDef>("KnockBackAbility");
                       BashAbilityDef strikeAbility = DefCache.GetDef<BashAbilityDef>("BashStrike_AbilityDef");

                       if (ability.TacticalAbilityDef != null && ability.TacticalAbilityDef == strikeAbility)
                       {
                        //   TFTVLogger.Always($"got here, ability is {ability.TacticalAbilityDef.name}");

                           if (parameter is TacticalAbilityTarget abilityTarget && abilityTarget.GetTargetActor() != null)
                           {

                              // TFTVLogger.Always($"got here, target is {abilityTarget.GetTargetActor()}");

                               TacticalActor tacticalActor = abilityTarget.GetTargetActor() as TacticalActor;



                               if (tacticalActor != null && tacticalActor.GetAbilityWithDef<JetJumpAbility>(knockBackAbility) != null && tacticalActor.IsAlive)
                               {
                                   JetJumpAbility knockBack = tacticalActor.GetAbilityWithDef<JetJumpAbility>(knockBackAbility);

                                   IEnumerable<TacticalAbilityTarget> targets = knockBack.GetTargets();

                                   TacticalAbilityTarget pushPosition = new TacticalAbilityTarget();
                                   TacticalAbilityTarget attack = parameter as TacticalAbilityTarget;

                                   foreach (TacticalAbilityTarget target in targets)  
                                   {
                                      // TFTVLogger.Always($"possible position {target.PositionToApply} and magnitude is {(target.PositionToApply - FindPushToTile(__instance, tacticalActor)).magnitude} ");

                                       if ((target.PositionToApply - FindPushToTile(__instance, tacticalActor, 1)).magnitude <= 1f) 
                                       {
                                           TFTVLogger.Always($"chosen position {target.PositionToApply}");

                                           pushPosition = target;

                                       }
                                   }


                                   //  MoveAbilityDef moveAbilityDef = DefCache.GetDef<MoveAbilityDef>("Move_AbilityDef");

                                   //  MoveAbility moveAbility = tacticalActor.GetAbilityWithDef<MoveAbility>(moveAbilityDef);
                                   //  moveAbility.Activate(pushPosition);

                                   knockBack.Activate(pushPosition);



                                   TFTVLogger.Always($"knocback executed position should be {pushPosition.GetActorOrWorkingPosition()}");

                               }
                           }
                       }

                       if (ability.TacticalAbilityDef == knockBackAbility)
                       {
                           __instance.RemoveAbility(ability);

                       }
                   }

                   catch (Exception e)
                   {
                       TFTVLogger.Error(e);
                   }

               }

           }


        */









        /*  [HarmonyPatch(typeof(AIFaction), "GetActionScore")]

           public static class TFTV_Experimental_AIActionMoveAndEscape_GetModuleBonusByType_AdjustFARMRecuperationModule_patch
           {
               public static void Prefix(AIFaction __instance, AIAction action, IAIActor actor, object context, LazyCache<AIConsiderationDef, AIConsideration> ____considerationsCache)
               {
                   try
                   {
                       if (action.ActionDef.name == "Flee_AIActionDef")
                       {
                           StatusDef autoRepairStatusDef = DefCache.GetDef<StatusDef>("RoboticSelfRepair_AddAbilityStatusDef");
                           TacticalLevelController tacticalLevelController = GameUtl.CurrentLevel().GetComponent<TacticalLevelController>();

                           foreach (TacticalActor tacticalActor in tacticalLevelController.GetFactionByCommandName("pu").TacticalActors)
                           {
                               if (tacticalActor.HasStatus(autoRepairStatusDef))
                               {
                                   TFTVLogger.Always($"{tacticalActor.name} has autorepair status");
                               }


                           }


                           float num = action.ActionDef.Weight;
                           TFTVLogger.Always($"get action score for action {action.ActionDef.name} with a weight of {num}");
                           AIAdjustedConsideration[] earlyExitConsiderations = action.ActionDef.EarlyExitConsiderations;

                           foreach (AIAdjustedConsideration aIAdjustedConsideration in earlyExitConsiderations)
                           {

                               if (aIAdjustedConsideration.Consideration == null)
                               {
                                   throw new InvalidOperationException($"Missing consideration for {actor} at {action.ActionDef.name}");
                               }

                               float time = ____considerationsCache.Get(aIAdjustedConsideration.Consideration).Evaluate(actor, null, context);
                               float num2 = aIAdjustedConsideration.ScoreCurve.Evaluate(time);

                               num *= num2;

                               TFTVLogger.Always($"early consideration is {aIAdjustedConsideration.Consideration.name} and num2 is {num2}, so score is now {num}");
                               if (num < 0.0001f)
                               {
                                   TFTVLogger.Always($"aIAdjustedConsideration {aIAdjustedConsideration.Consideration.name} reduced score to nearly 0");
                                   break;

                               }
                           }

                       }



                     /*  if (action.ActionDef.name == "MoveAndQuickAim_AIActionDef")
                       {
                           StatusDef autoRepairStatusDef = DefCache.GetDef<StatusDef>("RoboticSelfRepair_AddAbilityStatusDef");
                           TacticalLevelController tacticalLevelController = GameUtl.CurrentLevel().GetComponent<TacticalLevelController>();

                           ApplyStatusAbilityDef quickaim = DefCache.GetDef<ApplyStatusAbilityDef>("BC_QuickAim_AbilityDef");

                           foreach (TacticalActor tacticalActor in tacticalLevelController.GetFactionByCommandName("pu").TacticalActors) 
                           {
                               if (tacticalActor.GetAbilityWithDef <ApplyStatusAbility> (quickaim)!=null) 
                               {
                                   TFTVLogger.Always($"{tacticalActor.name} has quickaim ability");
                               }


                           }


                           float num = action.ActionDef.Weight;
                           TFTVLogger.Always($"get action score for action {action.ActionDef.name} with a weight of {num}");
                           AIAdjustedConsideration[] earlyExitConsiderations = action.ActionDef.EarlyExitConsiderations;

                           foreach (AIAdjustedConsideration aIAdjustedConsideration in earlyExitConsiderations)
                           {

                               if (aIAdjustedConsideration.Consideration == null)
                               {
                                   throw new InvalidOperationException($"Missing consideration for {actor} at {action.ActionDef.name}");
                               }

                               float time = ____considerationsCache.Get(aIAdjustedConsideration.Consideration).Evaluate(actor, null, context);
                               float num2 = aIAdjustedConsideration.ScoreCurve.Evaluate(time);

                               num *= num2;

                               TFTVLogger.Always($"early consideration is {aIAdjustedConsideration.Consideration.name} and num2 is {num2}, so score is now {num}");
                               if (num < 0.0001f)
                               {
                                   TFTVLogger.Always($"aIAdjustedConsideration {aIAdjustedConsideration.Consideration.name} reduced score to nearly 0");
                                   break;

                               }
                           }

                       }
                   }

                   catch (Exception e)
                   {
                       TFTVLogger.Error(e);
                   }

               }
           }*/









        /* [HarmonyPatch(typeof(GeoHavenLeader), "CanTradeWithFaction")]

         public static class TFTV_Experimental_GeoHavenLeader_CanTradeWithFaction_EnableTradingWhenNotAtWar_patch
         {
             public static void Postfix(GeoHavenLeader __instance, IDiplomaticParty faction, ref bool __result)
             {
                 try
                 {
                     MethodInfo getRelationMethod = AccessTools.Method(typeof(GeoHavenLeader), "GetRelationWith");
                     PartyDiplomacy.Relation relation = (PartyDiplomacy.Relation)getRelationMethod.Invoke(__instance, new object[] { faction });

                     __result = relation.Diplomacy > -50;

                 }

                 catch (Exception e)
                 {
                     TFTVLogger.Error(e);
                 }

             }
         }*/








        /*  [HarmonyPatch(typeof(GeoHaven), "GetResourceTrading")]
          public static class TFTV_Experimental_GeoHaven_GetResourceTrading_IncreaseCostDiplomacy_patch
          {
              public static void Postfix(GeoHaven __instance, ref List<HavenTradingEntry> __result)
              {
                  try
                  {
                      GeoFaction phoenixFaction = __instance.Site.GeoLevel.PhoenixFaction;
                      PartyDiplomacy.Relation relation = __instance.Leader.Diplomacy.GetRelation(phoenixFaction);
                      float multiplier = 1f;
                      List<HavenTradingEntry> offeredTrade = new List<HavenTradingEntry>(__result);

                      if (relation.Diplomacy > -50 && relation.Diplomacy <= -25)
                      {
                          multiplier = 0.5f;
                      }
                      else if (relation.Diplomacy > -25 && relation.Diplomacy <= 0)
                      {
                          multiplier = 0.75f;
                          TFTVLogger.Always("GetResourceTrading");
                      }

                      for (int i = 0; i < offeredTrade.Count; i++)
                      {
                         HavenTradingEntry havenTradingEntry = offeredTrade[i];
                          offeredTrade[i] = new HavenTradingEntry
                          {
                              HavenOfferQuantity = (int)(havenTradingEntry.HavenOfferQuantity*multiplier),
                              HavenOffers = havenTradingEntry.HavenOffers,
                              HavenWants = havenTradingEntry.HavenWants,
                              ResourceStock = havenTradingEntry.ResourceStock,
                              HavenReceiveQuantity = havenTradingEntry.HavenReceiveQuantity,
                          };
                          TFTVLogger.Always("New value is " + offeredTrade[i].HavenOfferQuantity);
                      }

                      __result = offeredTrade;

                  }

                  catch (Exception e)
                  {
                      TFTVLogger.Error(e);
                  }

              }
          }*/














        //  public static float Score = 0;
        //  public static List<float> ScoresBeforeCulling = new List<float>();
        //  public static int CounterAIActionsInfluencedBySafetyConsideration = 0;









        /* [HarmonyPatch(typeof(GeoMission), "PrepareLevel")]
         public static class GeoMission_PrepareLevel_VOObjectives_Patch
         {
             public static void Postfix(TacMissionData missionData, GeoMission __instance)
             {
                 try
                 {
                    // TFTVLogger.Always("PrepareLevel invoked");
                     GeoLevelController controller = __instance.Level;
                     List<int> voidOmens = new List<int> { 3, 5, 7, 10, 15, 16, 19 };

                     List<FactionObjectiveDef> listOfFactionObjectives = missionData.MissionType.CustomObjectives.ToList();

                     // Remove faction objectives that correspond to void omens that are not in play
                     for (int i = listOfFactionObjectives.Count - 1; i >= 0; i--)
                     {
                         FactionObjectiveDef objective = listOfFactionObjectives[i];
                         if (objective.name.StartsWith("VOID_OMEN_TITLE_"))
                         {
                             int vo = int.Parse(objective.name.Substring("VOID_OMEN_TITLE_".Length));
                             if (!TFTVVoidOmens.CheckFordVoidOmensInPlay(controller).Contains(vo))
                             {
                                 TFTVLogger.Always("Removing VO " + vo + " from faction objectives");
                                 listOfFactionObjectives.RemoveAt(i);
                             }
                         }
                     }

                     // Add faction objectives for void omens that are in play
                     foreach (int vo in voidOmens)
                     {
                         if (TFTVVoidOmens.CheckFordVoidOmensInPlay(controller).Contains(vo))
                         {
                             if (!listOfFactionObjectives.Any(o => o.name == "VOID_OMEN_TITLE_" + vo))
                             {
                                 TFTVLogger.Always("Adding VO " + vo + " to faction objectives");
                                 listOfFactionObjectives.Add(DefCache.GetDef<FactionObjectiveDef>("VOID_OMEN_TITLE_" + vo));
                             }
                         }
                     }

                     missionData.MissionType.CustomObjectives = listOfFactionObjectives.ToArray();
                 }
                 catch (Exception e)
                 {
                     TFTVLogger.Error(e);
                 }
             }
         }*/




        /* [HarmonyPatch(typeof(GeoMission), "PrepareLevel")]
         public static class GeoMission_ModifyMissionData_AddVOObjectives_Patch
         {
             public static void Postfix(TacMissionData missionData, GeoMission __instance)
             {
                 try
                 {
                     TFTVLogger.Always("ModifyMissionData invoked");
                     GeoLevelController controller = __instance.Level;
                     List<int> voidOmens = new List<int> { 3, 5, 7, 10, 15, 16, 19 };

                     foreach (int vo in voidOmens)
                     {
                         if (TFTVVoidOmens.CheckFordVoidOmensInPlay(controller).Contains(vo))
                         {
                             TFTVLogger.Always("VO " + vo + " found");
                             List<FactionObjectiveDef> listOfFactionObjectives = missionData.MissionType.CustomObjectives.ToList();

                             if (!listOfFactionObjectives.Contains(DefCache.GetDef<FactionObjectiveDef>("VOID_OMEN_TITLE_" + vo)))
                             {
                                 listOfFactionObjectives.Add(DefCache.GetDef<FactionObjectiveDef>("VOID_OMEN_TITLE_" + vo));
                                 missionData.MissionType.CustomObjectives = listOfFactionObjectives.ToArray();
                             }
                         }
                     }

                 }
                 catch (Exception e)
                 {
                     TFTVLogger.Error(e);
                 }
             }
         }*/











    }
    /* [HarmonyPatch(typeof(AIStrategicPositionConsideration), "Evaluate")]

        public static class AIStrategicPositionConsideration_Evaluate_Experiment_patch
        {
            public static void Postfix(AIStrategicPositionConsideration __instance, float __result)
            {
                try
                {
                    if (__instance.BaseDef.name == "StrategicPosition_AIConsiderationDef" && __result != 1)
                    {

                        TFTVLogger.Always("StrategicPosition_AIConsiderationDef " + __result);
                        Score = __result;
                    }
                }

                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }

            }
        }*/

}


