﻿using Base;
using Base.AI;
using Base.Core;
using Base.Defs;
using Base.Entities;
using Base.Entities.Animations;
using Base.Entities.Effects;
using Base.Levels;
using HarmonyLib;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.Entities.GameTagsTypes;
using PhoenixPoint.Common.Levels.Missions;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Entities.Interception.Equipments;
using PhoenixPoint.Geoscape.Entities.Sites;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Geoscape.View.ViewControllers.HavenDetails;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Effects;
using PhoenixPoint.Tactical.Entities.Effects.DamageTypes;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Statuses;
using PhoenixPoint.Tactical.Entities.Weapons;
using PhoenixPoint.Tactical.Levels;
using PhoenixPoint.Tactical.Levels.Missions;
using PhoenixPoint.Tactical.Levels.Mist;
using SETUtil.Extend;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace TFTV
{
    internal class TFTVExperimental
    {

        internal static Color purple = new Color32(149, 23, 151, 255);
        private static readonly DefRepository Repo = TFTVMain.Repo;
        private static readonly SharedData Shared = TFTVMain.Shared;

        //  private static readonly DefCache DefCache = TFTVMain.Main.DefCache;

        public static List<TacticalVoxel> VoxelsOnFire = new List<TacticalVoxel>();




        //Make bullets go through Umbra and Decoy, method by Dimitar "Codemite" Evtimov from Snapshot Games

        public static IDamageReceiver GetDamageReceiver(DamagePredictor predictor, GameObject gameObject, Vector3 pos, Quaternion rot)
        {
            IDamageable damageableObject = gameObject.GetComponentInParent<IDamageable>();
            if (damageableObject == null)
            {
                return null;
            }

            IDamageReceiver recv = damageableObject.GetDamageReceiverForHit(pos, rot * Vector3.forward); ;
            if (predictor != null)
            {
                recv = predictor.GetPredictingReceiver(recv);
            }

            return recv;
        }
       

        
        [HarmonyPatch(typeof(ProjectileLogic), "OnProjectileHit")]

        public static class ProjectileLogic_OnProjectileHit_Umbra_Patch
        {
            public static void Postfix(ProjectileLogic __instance, ref bool __result, DamageAccumulation ____damageAccum, CastHit hit, Vector3 dir)
            {
                try
                {
                    Vector3 pos = hit.Point;
                    Quaternion rot = Quaternion.LookRotation(dir);
                    IDamageReceiver receiver = GetDamageReceiver(__instance.Predictor, hit.Collider.gameObject, pos, rot);

                    /* TacticalLevelController controllerTactical = GameUtl.CurrentLevel().GetComponent<TacticalLevelController>();
                     TacticalFaction aliens = controllerTactical.GetFactionByCommandName("aln");*/


                    /*   TacticalActorBase tacticalactorBase = hit.Collider.GetComponent<TacticalActorBase>();

                       if (tacticalactorBase != null) 
                       {
                          TFTVLogger.Always($"On projectile hit {tacticalactorBase.name}");
                       }*/

                    //   TFTVLogger.Always($"hit position normal {hit.Normal} and position point {hit.Point}");

                    ClassTagDef umbraClassTag = DefCache.GetDef<ClassTagDef>("Umbra_ClassTagDef");
                    SpawnedActorTagDef decoy = DefCache.GetDef<SpawnedActorTagDef>("Decoy_SpawnedActorTagDef");

                  //  IDamageReceiver receiver = hit.Collider.gameObject.GetComponent<IDamageReceiver>();
                  
                    
                    if (__instance.Predictor != null)
                    {
                        receiver = __instance.Predictor.GetPredictingReceiver(receiver);
                    }

                    TacticalActor hitActor = receiver?.GetActor() as TacticalActor;


                   /*   if (hitActor != null)
                      {

                          TFTVLogger.Always($"hitactor is {hitActor.name}");

                      }*/
                    if (hitActor!=null && (hitActor.HasGameTag(umbraClassTag) || hitActor.HasGameTag(decoy)))
                    {
                      //  TFTVLogger.Always($"if passed");

                        //    foreach (TacticalActor tacticalActor in aliens.TacticalActors)
                        //    {
                        //   TFTVLogger.Always($"tac actor pos {tacticalActor.Pos}");

                        //      if ((hit.Point.x - tacticalActor.Pos.x)<=1 && (hit.Point.z - tacticalActor.Pos.z) <= 1)
                        //     {
                        //  TFTVLogger.Always("Found!");
                        ____damageAccum?.ResetToInitalAmount();
                        __result = false;
                        //   }
                        // }
                    }

                  //  PhoenixPoint.Tactical.Entities.Abilities.RagdollDieAbility
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    throw;
                }
            }
        }



        [HarmonyPatch(typeof(DieAbility), "Activate")]
        public static class DieAbility_Activate_Decoy_Patch
        {
            public static bool Prefix(DieAbility __instance)
            {
                try
                {


                    // TFTVLogger.Always("DieTriggered");
                    TacticalActorDef dcoy = DefCache.GetDef<TacticalActorDef>("Decoy_ActorDef");

                    if (!__instance.TacticalActorBase.IsObject() && __instance.TacticalActorBase.ActorDef == dcoy)
                    {
                        // TFTVLogger.Always("It's a decoy!");
                        __instance.TacticalActor.gameObject.SetActive(false);
                        return false;
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



        [HarmonyPatch(typeof(TacticalLevelController), "ActorDamageDealt")]
        public static class TacticalLevelController_ActorDamageDealt_Decoy_Patch
        {
            public static void Postfix(TacticalActor actor, IDamageDealer damageDealer)
            {
                try
                {
                    TacticalActorDef dcoy = DefCache.GetDef<TacticalActorDef>("Decoy_ActorDef");
                    ClassTagDef sirenTag = DefCache.GetDef<ClassTagDef>("Siren_ClassTagDef");
                    ClassTagDef queenTag = DefCache.GetDef<ClassTagDef>("Queen_ClassTagDef");
                    ClassTagDef tritonTag = DefCache.GetDef<ClassTagDef>("Fishman_ClassTagDef");
                    ClassTagDef acheronTag = DefCache.GetDef<ClassTagDef>("Acheron_ClassTagDef");
                    GameTagDef humanTag = DefCache.GetDef<GameTagDef>("Human_TagDef");


                    if (actor.IsAlive)
                    {
                        if (actor.TacticalActorDef == dcoy && damageDealer != null && damageDealer.GetTacticalActorBase() != null)
                        {
                            TacticalActorBase attackerBase = damageDealer.GetTacticalActorBase();
                            TacticalActor attacker = attackerBase as TacticalActor;

                            if (!attacker.IsControlledByPlayer)
                            {
                                //Decoy despawned if attacked by Siren or Scylla
                                if (attacker.GameTags.Contains(sirenTag) || attacker.GameTags.Contains(queenTag))
                                {
                                    actor.ApplyDamage(new DamageResult() { HealthDamage = actor.Health });
                                    //  TacContextHelpManager tacContextHelpManager = (TacContextHelpManager)UnityEngine.Object.FindObjectOfType(typeof(TacContextHelpManager));
                                    //  tacContextHelpManager.EventTypeTriggered(HintTrigger.ActorHurt, actor, actor);
                                }
                                //Decoy despawned if attacked within 5 tiles by human, triton or acheron
                                else if ((attacker.GameTags.Contains(tritonTag)
                                    || attacker.GameTags.Contains(humanTag)
                                    || attacker.GameTags.Contains(acheronTag))
                                    && (actor.Pos - attacker.Pos).magnitude <= 5)
                                {
                                    actor.ApplyDamage(new DamageResult() { HealthDamage = actor.Health });
                                    //  TacContextHelpManager tacContextHelpManager = (TacContextHelpManager)UnityEngine.Object.FindObjectOfType(typeof(TacContextHelpManager));
                                    //  tacContextHelpManager.EventTypeTriggered(HintTrigger.ActorHurt, actor, actor);
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
        }


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



        /*   [HarmonyPatch(typeof(TacticalActor), "OnAbilityExecuteFinished")]

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



                               if (tacticalActor != null && tacticalActor.GetAbilityWithDef<JetJumpAbility>(knockBackAbility) != null)
                               {
                                   JetJumpAbility knockBack = tacticalActor.GetAbilityWithDef<JetJumpAbility>(knockBackAbility);

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




        [HarmonyPatch(typeof(TacticalActor), "OnAbilityExecuteFinished")]

        public static class TacticalActor_OnAbilityExecuteFinished_Scylla_Experiment_patch
        {
            public static void Postfix(TacticalAbility ability, TacticalActor __instance, object parameter)
            {
                try
                {
                    ShootAbilityDef scyllaSpit = DefCache.GetDef<ShootAbilityDef>("GooSpit_ShootAbilityDef");
                    ShootAbilityDef scyllaScream = DefCache.GetDef<ShootAbilityDef>("SonicBlast_ShootAbilityDef");

                    //   TFTVLogger.Always($"ability {ability.TacticalAbilityDef.name} executed by {__instance.DisplayName}");
                    if (ability.TacticalAbilityDef == scyllaSpit || ability.TacticalAbilityDef == scyllaScream)
                    {
                        StartPreparingShootAbilityDef scyllaStartPreparing = DefCache.GetDef<StartPreparingShootAbilityDef>("Queen_StartPreparing_AbilityDef");
                        //    TFTVLogger.Always("Got here");
                        StartPreparingShootAbility startPreparingShootAbility = __instance.GetAbilityWithDef<StartPreparingShootAbility>(scyllaStartPreparing);

                        if (startPreparingShootAbility != null)
                        {

                            startPreparingShootAbility.Activate(parameter);
                        }

                    }

                 


                    /*   SpawnActorAbilityDef DecoyAbility = DefCache.GetDef<SpawnActorAbilityDef>("Decoy_AbilityDef");

                       if (ability.TacticalAbilityDef == DecoyAbility)
                       {                 
                           TacContextHelpManager tacContextHelpManager = (TacContextHelpManager)UnityEngine.Object.FindObjectOfType(typeof(TacContextHelpManager));
                           tacContextHelpManager.EventTypeTriggered(HintTrigger.ActorSeen, parameter, parameter);
                       }*/


                }

                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }

            }
        }



        [HarmonyPatch(typeof(GeoVehicle), "GetModuleBonusByType")]

        public static class TFTV_Experimental_GeoVehicle_GetModuleBonusByType_AdjustFARMRecuperationModule_patch
        {
            public static void Postfix(GeoVehicleModuleDef.GeoVehicleModuleBonusType type, ref float __result)
            {
                try
                {
                    if (type == GeoVehicleModuleDef.GeoVehicleModuleBonusType.Recuperation)
                    {
                        TFTVConfig config = TFTVMain.Main.Config;

                        if (config.ActivateStaminaRecuperatonModule)
                        {

                            __result = 0.35f;

                        }
                        else
                        {

                            __result = 0.0f;

                        }

                    }

                }

                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }

            }
        }




        [HarmonyPatch(typeof(GeoHavenLeader), "CanRecruitWithFaction")]

        public static class TFTV_Experimental_GeoHavenLeader_CanRecruitWithFaction_EnableRecruitingWhenNotAtWar_patch
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
        }

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



        [HarmonyPatch(typeof(GeoHaven), "GetRecruitCost")]
        public static class TFTV_Experimental_GeoHaven_GetRecruitCost_IncreaseCostDiplomacy_patch
        {
            public static void Postfix(GeoHaven __instance, ref ResourcePack __result, GeoFaction forFaction)
            {
                try
                {
                    GeoHavenLeader leader = __instance.Leader;
                    MethodInfo getRelationMethod = AccessTools.Method(typeof(GeoHavenLeader), "GetRelationWith");
                    PartyDiplomacy.Relation relation = (PartyDiplomacy.Relation)getRelationMethod.Invoke(leader, new object[] { forFaction });
                    ResourcePack price = new ResourcePack(__result);
                    float multiplier = 1f;
                    if (relation.Diplomacy > -50 && relation.Diplomacy <= -25)
                    {
                        multiplier = 1.5f;
                    }
                    else if (relation.Diplomacy > -25 && relation.Diplomacy <= 0)
                    {
                        multiplier = 1.25f;

                    }

                    for (int i = 0; i < price.Count; i++)
                    {
                        TFTVLogger.Always("Price component is " + price[i].Type + " amount " + price[i].Value);
                        ResourceUnit resourceUnit = price[i];
                        price[i] = new ResourceUnit(resourceUnit.Type, resourceUnit.Value * multiplier);
                    }

                    __result = price;


                }

                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }

            }
        }




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




        [HarmonyPatch(typeof(GeoHaven), "CheckShouldSpawnRecruit")]
        public static class TFTV_Experimental_GeoHaven_CheckShouldSpawnRecruit_IncreaseCostDiplomacy_patch
        {
            public static void Postfix(GeoHaven __instance, ref bool __result, float modifier)
            {
                try
                {
                    if (__result == false)
                    {
                        if (!__instance.IsRecruitmentEnabled || !__instance.ZonesStats.CanGenerateRecruit)
                        {
                            __result = false;
                        }
                        else
                        {
                            GeoFaction phoenixFaction = __instance.Site.GeoLevel.PhoenixFaction;
                            PartyDiplomacy.Relation relation = __instance.Leader.Diplomacy.GetRelation(phoenixFaction);
                            if (relation.Diplomacy <= 0 && relation.Diplomacy > -50)
                            {
                                int num = __instance.HavenDef.RecruitmentBaseChance;
                                num = Mathf.RoundToInt((float)num * modifier);


                                __result = UnityEngine.Random.Range(0, 100) < num;
                            }
                        }
                    }
                }

                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }

            }
        }





        [HarmonyPatch(typeof(GeoHaven), "TakeRecruit")]

        public static class TFTV_Experimental_GeoHaven_TakeRecruit_VanillaBugBix_patch
        {
            public static void Postfix(GeoHaven __instance, IGeoCharacterContainer __result, ref int ____population)
            {
                try
                {
                    if (__result != null)
                    {
                        ____population -= 1;
                        HavenInfoController havenInfo = (HavenInfoController)UnityEngine.Object.FindObjectOfType(typeof(HavenInfoController));


                        int populationChange = __instance.GetPopulationChange(__instance.ZonesStats.GetTotalHavenOutput());
                        if (populationChange > 0)
                        {
                            havenInfo.PopulationValueText.text = string.Format(havenInfo.PopulationPositiveTextPattern, __instance.Population.ToString(), populationChange);
                        }
                        else if (populationChange == 0)
                        {
                            havenInfo.PopulationValueText.text = __instance.Population.ToString();
                        }
                        else
                        {
                            havenInfo.PopulationValueText.text = string.Format(havenInfo.PopulationNegativeTextPattern, __instance.Population.ToString(), populationChange);
                        }


                    }

                }

                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }

            }
        }

        public static void CheckUseFireWeaponsAndDifficulty(GeoLevelController controller)
        {
            try
            {
                if (controller.EventSystem.GetVariable("FireQuenchersAdded") == 0 && controller.CurrentDifficultyLevel.Order > 1)
                {
                    TFTVLogger.Always("Checking fire weapons usage to decide wether to add Fire Quenchers");

                    PhoenixStatisticsManager statisticsManager = (PhoenixStatisticsManager)UnityEngine.Object.FindObjectOfType(typeof(PhoenixStatisticsManager));

                    List<SoldierStats> allSoldierStats = new List<SoldierStats>(statisticsManager.CurrentGameStats.LivingSoldiers.Values.ToList());
                    allSoldierStats.AddRange(statisticsManager.CurrentGameStats.DeadSoldiers.Values.ToList());
                    List<UsedWeaponStat> usedWeapons = new List<UsedWeaponStat>();

                    int scoreFireDamage = 0;
                    StandardDamageTypeEffectDef fireDamage = DefCache.GetDef<StandardDamageTypeEffectDef>("Fire_StandardDamageTypeEffectDef");

                    foreach (SoldierStats stat in allSoldierStats)
                    {
                        if (stat.ItemsUsed.Count > 0)
                        {
                            usedWeapons.AddRange(stat.ItemsUsed);
                        }
                    }

                    if (usedWeapons.Count() > 0)
                    {
                        // TFTVLogger.Always("Checking use of each weapon... ");
                        foreach (UsedWeaponStat stat in usedWeapons)
                        {
                            //   TFTVLogger.Always("This item is  " + stat.UsedItem.ViewElementDef.DisplayName1.LocalizeEnglish());
                            if (Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Contains(stat.UsedItem.ToString())))
                            {
                                WeaponDef weaponDef = stat.UsedItem as WeaponDef;
                                if (weaponDef != null && weaponDef.DamagePayload.DamageType == fireDamage)
                                {
                                    scoreFireDamage += stat.UsedCount;
                                }
                            }
                        }
                    }

                    TFTVLogger.Always("Fire weapons used " + scoreFireDamage + " times");

                    if (scoreFireDamage > 1)
                    {
                        UnityEngine.Random.InitState((int)Stopwatch.GetTimestamp());
                        int roll = UnityEngine.Random.Range(0, 6) + controller.CurrentDifficultyLevel.Order + scoreFireDamage;

                        TFTVLogger.Always("The roll is " + roll);

                        if (roll >= 10)
                        {
                            TFTVLogger.Always("The roll is passed!");
                            controller.EventSystem.SetVariable("FireQuenchersAdded", 1);
                        }

                    }
                }
            }

            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void CheckForFireQuenchers(GeoLevelController controller)
        {
            try
            {
                CheckUseFireWeaponsAndDifficulty(controller);

                if (controller.EventSystem.GetVariable("FireQuenchersAdded") == 1)
                {
                    AddFireQuencherAbility();
                    TFTVLogger.Always("Fire Quenchers added!");
                }
                else
                {

                    DefCache.GetDef<TacticalItemDef>("Crabman_Head_Humanoid_BodyPartDef").Abilities = new TacticalAbilityDef[] { };
                    DefCache.GetDef<TacticalItemDef>("Crabman_Head_EliteHumanoid_BodyPartDef").Abilities = new TacticalAbilityDef[] { };

                }
            }

            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static void AddFireQuencherAbility()
        {
            try
            {
                ApplyStatusAbilityDef fireQuencherAbility = DefCache.GetDef<ApplyStatusAbilityDef>("FireQuencherAbility");
                DamageMultiplierAbilityDef fireImmunity = DefCache.GetDef<DamageMultiplierAbilityDef>("FireImmunityInvisibleAbility");
                List<TacticalAbilityDef> abilities = new List<TacticalAbilityDef>() { fireQuencherAbility, fireImmunity };
                //  DefCache.GetDef<TacticalItemDef>("Crabman_Legs_Armoured_ItemDef").Abilities = abilities.ToArray();
                //  DefCache.GetDef<TacticalItemDef>("Crabman_Legs_EliteArmoured_ItemDef").Abilities = new TacticalAbilityDef[] { fireImmunity };
                //    DefCache.GetDef<TacCharacterDef>("Crabman9_Shielder_AlienMutationVariationDef").Data.Abilites = abilities.ToArray();

                DefCache.GetDef<TacticalItemDef>("Crabman_Head_Humanoid_BodyPartDef").Abilities = abilities.ToArray();
                DefCache.GetDef<TacticalItemDef>("Crabman_Head_EliteHumanoid_BodyPartDef").Abilities = abilities.ToArray();
                // DefCache.GetDef<TacticalItemDef>("Crabman_LeftLeg_Armoured_BodyPartDef").Abilities = abilities.ToArray();
                //  DefCache.GetDef<TacticalItemDef>("Crabman_RightLeg_Armoured_BodyPartDef").Abilities = abilities.ToArray();
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }



        [HarmonyPatch(typeof(TacticalPerceptionBase), "IsTouchingVoxel")]

        public static class TFTV_Experimental_Evaluate_Experiment_patch
        {
            public static void Postfix(TacticalPerceptionBase __instance)
            {
                try
                {
                    DamageMultiplierStatusDef fireQuencherStatus = DefCache.GetDef<DamageMultiplierStatusDef>("FireQuencherStatus");
                    //   DamageMultiplierAbilityDef fireImmunity = DefCache.GetDef<DamageMultiplierAbilityDef>("FireImmunity_DamageMultiplierAbilityDef");

                    TacticalActorBase tacticalActorBase = __instance.TacActorBase;

                    //
                    if (tacticalActorBase is TacticalActor && tacticalActorBase.GetActor().Status.HasStatus(fireQuencherStatus)) //tacticalActorBase.GetActor().GetAbility<DamageMultiplierAbility>(fireImmunity)!=null 
                                                                                                                                 // && tacticalActorBase.GetActor().HasGameTag(DefCache.GetDef<GameTagDef>("Crabman_ClassTagDef")))
                    {
                        foreach (TacticalVoxel voxel in tacticalActorBase.TacticalLevel.VoxelMatrix.GetVoxels(__instance.GetBounds()))
                        {
                            if (voxel.GetVoxelType() == TacticalVoxelType.Fire)
                            {
                                VoxelsOnFire.Add(voxel);
                                VoxelsOnFire.Add(tacticalActorBase.TacticalLevel.VoxelMatrix.GetVoxel(voxel.Position + new Vector3(0.5f, 0, 0.5f)));
                                VoxelsOnFire.Add(tacticalActorBase.TacticalLevel.VoxelMatrix.GetVoxel(voxel.Position + new Vector3(-0.5f, 0, -0.5f)));
                                VoxelsOnFire.Add(tacticalActorBase.TacticalLevel.VoxelMatrix.GetVoxel(voxel.Position + new Vector3(-0.5f, 0, 0.5f)));
                                VoxelsOnFire.Add(tacticalActorBase.TacticalLevel.VoxelMatrix.GetVoxel(voxel.Position + new Vector3(0.5f, 0, -0.5f)));
                                // VoxelsOnFire.Add(tacticalActorBase.TacticalLevel.VoxelMatrix.GetVoxel(voxel.Position + new Vector3(1.5f, 0, 1.5f)));
                                //  VoxelsOnFire.Add(tacticalActorBase.TacticalLevel.VoxelMatrix.GetVoxel(voxel.Position + new Vector3(-1.5f, 0, -1.5f)));
                                VoxelsOnFire.Add(tacticalActorBase.TacticalLevel.VoxelMatrix.GetVoxel(voxel.Position + new Vector3(1, 0, 1)));
                                VoxelsOnFire.Add(tacticalActorBase.TacticalLevel.VoxelMatrix.GetVoxel(voxel.Position + new Vector3(-1, 0, -1)));
                                VoxelsOnFire.Add(tacticalActorBase.TacticalLevel.VoxelMatrix.GetVoxel(voxel.Position + new Vector3(1, 0, -1)));
                                VoxelsOnFire.Add(tacticalActorBase.TacticalLevel.VoxelMatrix.GetVoxel(voxel.Position + new Vector3(-1, 0, 1)));
                                //  VoxelsOnFire.Add(tacticalActorBase.TacticalLevel.VoxelMatrix.GetVoxel(voxel.Position + new Vector3(1.5f, 0, 0.5f)));
                                // VoxelsOnFire.Add(tacticalActorBase.TacticalLevel.VoxelMatrix.GetVoxel(voxel.Position + new Vector3(-1.5f, 0, -0.5f)));
                            }
                        }
                    }
                }

                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }

            }
        }



        [HarmonyPatch(typeof(TacticalActor), "OnAbilityExecuteFinished")]

        public static class TacticalActor_OnAbilityExecuteFinished_Experiment_patch
        {
            public static void Postfix()
            {
                try
                {
                    if (VoxelsOnFire != null)
                    {

                        if (VoxelsOnFire.Count > 0)
                        {
                            //  TFTVLogger.Always("Voxels on fire count is " + TFTVExperimental.VoxelsOnFire.Count);
                            // List<TacticalVoxel> voxelsForMist = new List<TacticalVoxel>();
                            foreach (TacticalVoxel voxel in TFTVExperimental.VoxelsOnFire)
                            {
                                if (voxel != null && voxel.GetVoxelType() == TacticalVoxelType.Fire)
                                {
                                    //    TFTVLogger.Always("Got past the if check");

                                    voxel.SetVoxelType(TacticalVoxelType.Empty, 1);
                                }
                            }
                        }

                        if (VoxelsOnFire.Count > 0)
                        {
                            foreach (TacticalVoxel voxel in TFTVExperimental.VoxelsOnFire)
                            {
                                if (voxel != null && voxel.GetVoxelType() == TacticalVoxelType.Empty)
                                {
                                    //TFTVLogger.Always("Got past the if check for Mist");
                                    voxel.SetVoxelType(TacticalVoxelType.Mist, 2, 10);
                                }
                            }
                        }

                        VoxelsOnFire.Clear();
                    }
                }

                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }

            }
        }




        //  public static float Score = 0;
        //  public static List<float> ScoresBeforeCulling = new List<float>();
        //  public static int CounterAIActionsInfluencedBySafetyConsideration = 0;

        


        //Method by Dimitar "Codemite" Evtimov from Snapshot Games
        public static void PatchInAllBaseDefenseDefs()
        {
            try
            {

                CustomMissionTypeDef alienDef = DefCache.GetDef<CustomMissionTypeDef>("PXBaseAlien_CustomMissionTypeDef");
                CustomMissionTypeDef anuDef = DefCache.GetDef<CustomMissionTypeDef>("PXBaseAnu_CustomMissionTypeDef");
                CustomMissionTypeDef njDef = DefCache.GetDef<CustomMissionTypeDef>("PXBaseNJ_CustomMissionTypeDef");
                CustomMissionTypeDef syDef = DefCache.GetDef<CustomMissionTypeDef>("PXBaseSY_CustomMissionTypeDef");
                CustomMissionTypeDef infestationDef = DefCache.GetDef<CustomMissionTypeDef>("PXBaseInfestationAlien_CustomMissionTypeDef");

                TacMissionTypeDef[] defenseMissions = { alienDef, anuDef, njDef, syDef, infestationDef };

                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    if (!scene.isLoaded)
                        continue;

                    foreach (var root in scene.GetRootGameObjects())
                    {
                        foreach (var transform in root.GetTransformsInChildrenStable())
                        {
                            var objActivator = transform.GetComponent<TacMissionObjectActivator>();
                            if (objActivator && objActivator.Missions.Length == 1 && objActivator.Missions.Contains(alienDef))
                            {
                                objActivator.Missions = defenseMissions;
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


        [HarmonyPatch(typeof(TacMission), "PrepareMissionActivators")]

        public static class TacMission_PrepareMissionActivators_Experiment_patch
        {
            public static void Prefix(TacMission __instance)
            {
                try
                {

                    TFTVLogger.Always("PrepareMissionActivators");
                    PatchInAllBaseDefenseDefs();

                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }


        }

        public static TacticalActor mutoidReceivingHealing = null;

        [HarmonyPatch(typeof(HealAbility), "HealTargetCrt")]

        public static class HealAbility_HealTargetCrt_Mutoid_Patch
        {
            public static void Prefix(PlayingAction action)
            {
                try
                {
                    if ((TacticalActor)((TacticalAbilityTarget)action.Param).Actor != null)
                    {
                        TacticalActor actor = (TacticalActor)((TacticalAbilityTarget)action.Param).Actor;
                        if (actor.HasGameTag(DefCache.GetDef<GameTagDef>("Mutoid_ClassTagDef")))
                        {
                            mutoidReceivingHealing = actor;
                        }
                    }
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }
        }


        [HarmonyPatch(typeof(HealAbility), "get_GeneralHealAmount")]

        public static class HealAbility_Mutoid_Patch
        {

            public static void Postfix(HealAbility __instance, ref float __result)
            {
                try
                {

                    if (mutoidReceivingHealing != null)
                    {
                        __result = 0;
                        mutoidReceivingHealing = null;
                    }

                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }
        }


        [HarmonyPatch(typeof(DamageAccumulation), "GenerateStandardDamageTargetData")]
        class DamageMultiplier_BugFix
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> listInstructions = new List<CodeInstruction>(instructions);
                IEnumerable<CodeInstruction> insert = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldloc_3),
                new CodeInstruction(OpCodes.Div)
            };

                // insert after each of the first 3 divide opcodes
                int divs = 0;
                for (int index = 0; index < instructions.Count(); index++)
                {
                    if (listInstructions[index].opcode == OpCodes.Div)
                    {
                        listInstructions.InsertRange(index + 1, insert);
                        index += 2;
                        divs++;
                        if (divs == 3)
                        {
                            break;
                        }
                    }
                }

                if (divs != 3)
                {
                    return instructions; // didn't find three, function signature changed, abort
                }
                return listInstructions;
            }

        }

        private static readonly DefCache DefCache = TFTVMain.Main.DefCache;

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


