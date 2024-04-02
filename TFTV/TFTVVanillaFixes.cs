﻿using Assets.Code.PhoenixPoint.Geoscape.Entities.Sites.TheMarketplace;
using Base;
using Base.Core;
using Base.Entities;
using Base.Rendering.ObjectRendering;
using Base.Utils.Maths;
using HarmonyLib;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Common.Levels.Missions;
using PhoenixPoint.Common.View.ViewControllers.Inventory;
using PhoenixPoint.Geoscape.Core;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Entities.PhoenixBases;
using PhoenixPoint.Geoscape.Entities.PhoenixBases.FacilityComponents;
using PhoenixPoint.Geoscape.Entities.Research.Requirement;
using PhoenixPoint.Geoscape.Entities.Sites;
using PhoenixPoint.Geoscape.Events;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Geoscape.View.ViewControllers.HavenDetails;
using PhoenixPoint.Geoscape.View.ViewControllers.SiteEncounters;
using PhoenixPoint.Geoscape.View.ViewModules;
using PhoenixPoint.Geoscape.View.ViewStates;
using PhoenixPoint.Tactical.AI;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Weapons;
using PhoenixPoint.Tactical.Levels;
using PhoenixPoint.Tactical.Levels.Missions;
using PhoenixPoint.Tactical.UI.SoldierPortraits;
using PhoenixPoint.Tactical.View;
using SETUtil.Common.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PhoenixPoint.Tactical.Entities.SquadPortraitsDef;

namespace TFTV
{
    internal class TFTVVanillaFixes
    {
        private static readonly DefCache DefCache = TFTVMain.Main.DefCache;
        private static readonly SharedData Shared = TFTVMain.Shared;

        //Fixes size of ground marker for eggs/sentinels etc.
        public static void FixSurveillanceAbilityGroundMarker(Harmony harmony)
        {
            try
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                Assembly assembly = null;
                foreach (Assembly a in assemblies)
                {
                    if (a.GetName().Name.Contains("Assembly-CSharp"))
                    {
                        assembly = a;
                    }
                }
                Type internalType = assembly.GetType("PhoenixPoint.Tactical.View.ViewStates.UIStateCharacterSelected");

                if (internalType != null)
                {
                    MethodInfo methodToPatch = internalType.GetMethod("ZoneOfControlMarkerCreator", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (methodToPatch != null)
                    {
                        harmony.Patch(methodToPatch, postfix: new HarmonyMethod(typeof(TFTVVanillaFixes), nameof(PatchResizeGroundMarker)));
                    }
                    else
                    {

                    }
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
                throw;
            }

        }

        public static void PatchResizeGroundMarker(MethodBase __originalMethod, object context, ref GroundMarker __result)
        {
            try
            {
                if (__result != null)
                {
                    __result.StartScale /= 2.05f;
                    __result.StartScale *= 1.6f;
                }
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
                throw;
            }
        }


        //Prevents AI from consindering unseen enemies when evaluating attacks with explosives/cone weapons
        private static bool CheckVisibility(TacticalActorBase tacticalActorBase, TacticalActor tacticalActor, DamagePayload damagePayload)
        {
            try
            {
                if (damagePayload.DamageDeliveryType == DamageDeliveryType.Sphere || damagePayload.DamageDeliveryType == DamageDeliveryType.Cone)
                {
                    if (tacticalActor.TacticalFaction == tacticalActorBase.TacticalFaction)
                    {
                        return true;
                    }

                    if (tacticalActor.TacticalFaction.GetAllAliveFriendlyActors<TacticalActorBase>(tacticalActor).Contains(tacticalActorBase))
                    {
                        return true;
                    }

                    if (tacticalActor.TacticalFaction.AIBlackboard.GetEnemies(ActorType.All, true).Contains(tacticalActorBase))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
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


        [HarmonyPatch(typeof(AIUtil), "GetAffectedTargetsByShooting")]
        public static class TFTV_AIUtil_GetAffectedTargetsByShooting_patch
        {
            private static IEnumerable<TacticalActorBase> Postfix(IEnumerable<TacticalActorBase> results, Vector3 shootPos, TacticalActor sourceActor, Weapon sourceWeapon, TacticalAbilityTarget target, ShootAbilityDef shootAbility = null)
            {

                DamagePayload damagePayload = sourceWeapon.GetDamagePayload();

                foreach (TacticalActorBase actorBase in results)
                {
                    if (CheckVisibility(actorBase, sourceActor, damagePayload))
                    {
                        yield return actorBase;
                    }

                }
            }

        }



        //Prevents evacuated characters from spotting enemies

        [HarmonyPatch(typeof(TacticalFactionVision), "ReUpdateVisibilityTowardsActorImpl")]
        public static class TFTV_TacticalFactionVision_ReUpdateVisibilityTowardsActorImpl_patch
        {
            private static bool Prefix(TacticalActorBase fromActor, TacticalActorBase targetActor, float basePerceptionRange, ref bool __result)
            {
                try
                {
                    if (fromActor is TacticalActor tacticalActor && tacticalActor.IsEvacuated)
                    {
                        __result = false;
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

        //fixes requiring killing actor required for research even when it is already captured

        [HarmonyPatch(typeof(ActorResearchRequirement), "OnMissionEnd")]
        public static class TFTV_ActorResearchRequirement_OnMissionEnd
        {
            public static bool Prefix(ActorResearchRequirement __instance, GeoFaction faction, GeoMission mission, GeoSite site, GeoFaction ____faction)
            {
                try
                {
                    _ = site.GeoLevel;
                    ActorResearchRequirementDef actorResearchRequirementDef = __instance.ActorResearchRequirementDef;
                    foreach (FactionResult factionResult in mission.Result.FactionResults)
                    {
                        if (factionResult.FactionDef == ____faction.Def.PPFactionDef || (__instance.ActorResearchRequirementDef.Faction != null && factionResult.FactionDef != actorResearchRequirementDef.Faction))
                        {
                            continue;
                        }

                        foreach (TacActorUnitResult item in from t in factionResult.UnitResults.Select((UnitResult s) => s.Data).OfType<TacActorUnitResult>()
                                                            where !t.IsAlive || t.Statuses.Any(s => s.Def.EffectName == "Paralysed")
                                                            select t)
                        {
                            // TFTVLogger.Always($"considering {item.SourceTemplate.name}");

                            if (__instance.IsValidUnit(item))
                            {
                                MethodInfo updateProgressMethod = typeof(ResearchRequirement).GetMethod("UpdateProgress", BindingFlags.Instance | BindingFlags.NonPublic);

                                updateProgressMethod.Invoke(__instance, new object[] { 1 });

                                if (__instance.IsCompleted)
                                {
                                    break;
                                }
                            }
                        }
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


        //fixes events reducing health to 0 and killing soldiers
        [HarmonyPatch(typeof(GeoFactionReward), "AddInjuriesToAllSoldiers")]
        public static class TFTV_GeoFactionReward_AddInjuriesToAllSoldiers
        {
            public static bool Prefix(GeoFactionReward __instance, GeoFaction faction)
            {
                try
                {
                    if (__instance.AddAllSoldiersTiredness != 0)
                    {
                        foreach (GeoCharacter character in faction.Characters)
                        {
                            if (character.Fatigue != null)
                            {
                                character.Fatigue.Stamina.AddRestrictedToMax(-__instance.AddAllSoldiersTiredness);
                            }
                        }

                        __instance.ApplyResult.AllSoldiersTiredness += __instance.AddAllSoldiersTiredness;
                    }

                    if (__instance.AddAllSoldiersDamage == 0)
                    {
                        return false;
                    }

                    foreach (GeoCharacter character2 in faction.Characters)
                    {
                        if ((float)character2.Health > 1f && character2.TemplateDef.IsHuman)
                        {
                            int addAllSoldiersDamage = Math.Min(__instance.AddAllSoldiersDamage, (int)character2.Health - 1);
                            character2.Health.AddRestrictedToMax(-addAllSoldiersDamage);
                            TFTVLogger.Always($"applied {addAllSoldiersDamage} damage to {character2.DisplayName}");
                        }
                    }

                    __instance.ApplyResult.AllSoldiersDamage += __instance.AddAllSoldiersDamage;

                    return false;
                }

                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    throw;
                }
            }
        }



        //Codemite's solution to pink portrait backgrounds on Macs
        [HarmonyPatch(typeof(RenderingEnvironment), MethodType.Constructor, new Type[]
{
    typeof(Vector2Int),
    typeof(RenderingEnvironmentOption),
    typeof(Color),
    typeof(Camera)
  })]
        public static class RenderingEnvironmentPatch
        {

            public static bool Prefix(ref RenderingEnvironment __instance, ref Transform ____origin, ref bool ____isExternalCamera, ref Camera ____camera,
                Vector2Int resolution, RenderingEnvironmentOption option, Color? backgroundColor, Camera cam)
            {
                try
                {

                    ____origin = new GameObject("_RenderingEnvironmentOrigin_").transform;
                    ____origin.position = new Vector3(0f, 1500f, 0f);
                    ____origin.gameObject.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;

                    ____isExternalCamera = cam != null;

                    ____camera = cam != null ? cam : new GameObject("_RenderingCamera_").AddComponent<Camera>();
                    ____camera.gameObject.SetActive(false);
                    ____camera.clearFlags = option.ContainsFlag(RenderingEnvironmentOption.NoBackground) && backgroundColor == null
                        ? CameraClearFlags.Depth
                        : (backgroundColor != null ? CameraClearFlags.SolidColor : CameraClearFlags.Skybox);
                    ____camera.fieldOfView = 60;
                    ____camera.orthographicSize = 2;
                    ____camera.orthographic = option.ContainsFlag(RenderingEnvironmentOption.Orthographic);
                    ____camera.farClipPlane = 100;
                    ____camera.renderingPath = RenderingPath.Forward;
                    ____camera.enabled = false;
                    ____camera.allowHDR = false;
                    ____camera.allowDynamicResolution = false;
                    ____camera.usePhysicalProperties = false;

                    // Use reflection to set the value of RenderTexture
                    var renderTextureField = typeof(RenderingEnvironment).GetField("RenderTexture", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (renderTextureField != null)
                    {
                        renderTextureField.SetValue(__instance, RenderTexture.GetTemporary(resolution.x, resolution.y, 1, RenderTextureFormat.ARGB32));
                    }

                    ____camera.targetTexture = (RenderTexture)renderTextureField.GetValue(__instance);

                    if (backgroundColor != null)
                    {
                        ____camera.backgroundColor = (Color)backgroundColor;
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


        [HarmonyPatch(typeof(SoldierPortraitUtil), "RenderSoldierNoCopy")]
        public static class SoldierPortraitUtil_RenderSoldierNoCopy_patch
        {

            public static bool Prefix(GameObject soldierToRender, RenderPortraitParams renderParams, Camera usedCamera, ref Texture2D __result)
            {
                try
                {

                    if (Application.platform == RuntimePlatform.OSXPlayer ||
                 Application.platform == RuntimePlatform.OSXEditor)
                    {

                        var outputImage = new Texture2D(renderParams.RenderedPortraitsResolution.x, renderParams.RenderedPortraitsResolution.y, TextureFormat.RGBA32, true);
                        using (var renderingEnvironment = new RenderingEnvironment(renderParams.RenderedPortraitsResolution, RenderingEnvironmentOption.NoBackground, Color.black, usedCamera))
                        {
                            Transform headTransform = soldierToRender.transform.FindTransformInChildren(renderParams.TargetBoneName) ?? soldierToRender.transform;
                            var t = soldierToRender.transform;
                            var initialPosition = t.position;
                            var initialRotation = t.rotation;
                            t.position = renderingEnvironment.OriginPosition;
                            t.rotation = renderingEnvironment.OriginRotation;

                            SoldierFrame soldierFrame = new SoldierFrame(headTransform,
                                renderParams.CameraFoV, renderParams.CameraDistance, renderParams.CameraHeight, renderParams.CameraSide);
                            renderingEnvironment.Render(soldierFrame, false);
                            renderingEnvironment.WriteResultsToTexture(outputImage);

                            t.position = initialPosition;
                            t.rotation = initialRotation;
                        }

                        __result = outputImage;

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


        //Madskunky's replacement of a trig function to reduce AI processing time 
        [HarmonyPatch(typeof(Weapon), "GetDamageModifierForDistance")]
        public static class Weapon_GetDamageModifierForDistance_patch
        {

            public static bool Prefix(Weapon __instance, TacticalActorBase targetActor, ref float __result, float distance)
            {
                try
                {
                    float num = (float)Math.PI / 180f * __instance.WeaponDef.SpreadDegrees;
                    if (num < Mathf.Epsilon)
                    {
                        __result = 1f;
                        return false;
                    }

                    TacticalPerceptionBase tacticalPerceptionBase = targetActor.TacticalPerceptionBase;
                    float num2 = tacticalPerceptionBase.Height / 2f;
                    float r = tacticalPerceptionBase.GetCapsuleLocal().r;
                    float time = Mathf.Clamp(((num2 + r) / 2f / distance) / num, 0f, 1f);
                    __result = time < 0.5 ? 0f : time;
                    return false;
                }

                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    throw;
                }
            }
        }

        //Prevents ammo from disappearing on pressing replinish ammo if the class of the soldier is not proficient with the weapon and ALL filter is switched off 
        [HarmonyPatch(typeof(UIStateEditSoldier), "SoldierSlotItemChangedHandler")]
        public static class UIStateEditSoldier_SoldierSlotItemChangedHandler_patch
        {

            public static bool Prefix(UIStateEditSoldier __instance, UIInventorySlot slot)
            {
                try
                {
                    if (slot == null)
                    {
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

        //Prevents targeting body parts with Destiny and similar of unrevealed characters.

        [HarmonyPatch(typeof(ShootAbility), "GetShootTarget")]
        public static class ShootAbility_GetShootTarget_Patch
        {
            public static void Postfix(ShootAbility __instance,
                TacticalAbilityTarget target, ref TacticalAbilityTarget __result)// Vector3? sourcePosition = null, TacticalTargetData targetData = null, )
            {
                try
                {
                    if (__instance.ShootAbilityDef.SnapToBodyparts)
                    {
                        TacticalActor tacticalActor = target.Actor as TacticalActor;
                        if (tacticalActor != null && !tacticalActor.IsRevealedToViewer)
                        {
                            __result = null;

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


        //MadSkunky's OW, fumble and Throwing range fix copied over from BC

        /// <summary> 
        /// Fix:
        /// The UI icon element of overwatch abilities does not take AP modifications for the default shooting ability into account.
        /// </summary>
        [HarmonyPatch(typeof(TacticalAbility), "FractActionPointCost", MethodType.Getter)]
        internal class TacticalAbility_get_FractActionPointCost_OWFix
        {
            public static bool Prefix(TacticalAbility __instance, ref float __result)
            {
                try
                {
                    if (!(__instance is OverwatchAbility overwatchAbility))
                    {
                        // do nothing and call the patched method if this is not an overwatch ability
                        return true;
                    }

                    // copy-pasted from the original get_FractActionPointCost()
                    float baseFract;
                    if (overwatchAbility.TacticalAbilityDef.ActionPointCost >= 0f)
                    {
                        baseFract = overwatchAbility.TacticalAbilityDef.ActionPointCost;
                    }
                    else
                    {
                        Equipment equipment = overwatchAbility.Equipment;
                        baseFract = ((equipment != null) ? equipment.FractApToUse : 0f);
                    }
                    // copy-pasted from the original and private method GetActionPointFractCost(baseFract);
                    if (!baseFract.IsZero(1E-05f) && overwatchAbility.IsTacticalActor)
                    {
                        baseFract = overwatchAbility.TacticalActor.CalcFractActionPointCost(baseFract, overwatchAbility);
                    }

                    // adding comparison with default shooting ability and take the minimum of both
                    __result = Mathf.Min(baseFract, overwatchAbility.GetDefaultShootAbility().FractActionPointCost);

                    // skip the patched method
                    return false;
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Fix:
        /// The method TacticalAbility.EnqueueAction() is an alternative to TacticalAbility.PlayAction().
        /// The assumption is that this method will play an action synchronously with a currently running action, as opposed to playing it directly as with PlayAction().
        /// This method appears to be called only by ShootAbility.Activate().
        /// In contrast to PlayAction(), however, a possible fumble (property FumbledAction = true) is not checked with this method and therefore no fumble will happen.
        /// This patch fixes the problem and queues the FumbleAction() to play when the FumbledAction property is set (= true).
        /// </summary>
        [HarmonyPatch(typeof(TacticalAbility), nameof(TacticalAbility.EnqueueAction))]
        public static class TacticalAbility_EnqueueAction_FumbleFix
        {
            public static bool Prefix(TacticalAbility __instance,
                                      ref PlayingAction ____nextAction,
                                      Func<PlayingAction, IEnumerator<NextUpdate>> action,
                                      object parameter,
                                      bool soloAfterCurrent = false)
            {
                try
                {
                    // Change action to play FumbleAction dependent on the property FumbledAction
                    if (__instance.FumbledAction)
                    {
                        action = GetDelegate<Func<PlayingAction, IEnumerator<NextUpdate>>>(__instance, "FumbleAction");
                        TFTVLogger.Always($"TacticalAbility_EnqueueAction_FumbleFixPatch: FumbledAction for {__instance} is true, changed action to play to TacticalAbility.FumbleAction().");
                    }

                    // Get delegate for CreateWaitingForCameraBlendingAction method
                    Func<Func<PlayingAction, IEnumerator<NextUpdate>>, Func<PlayingAction, IEnumerator<NextUpdate>>> instance_CreateWaitingForCameraBlendingAction =
                        GetDelegate<Func<Func<PlayingAction, IEnumerator<NextUpdate>>, Func<PlayingAction, IEnumerator<NextUpdate>>>>(__instance, "CreateWaitingForCameraBlendingAction");
                    // Go on with original code
                    ____nextAction = __instance.ActionComponent.PlayActionAfterCurrent(ActionChannel.ActorActions,
                                                                                       soloAfterCurrent,
                                                                                       parameter,
                                                                                       instance_CreateWaitingForCameraBlendingAction(action),
                                                                                       GetDelegate<Action<PlayingAction>>(__instance, "StartPlayingAction"),
                                                                                       GetDelegate<Action<PlayingAction>>(__instance, "ClearPlayingAction"));

                    __instance.TacticalActorBase.OnAbilityEnqueued(__instance);
                    return false; // Don't execute (skip) original method, this is a fix for the original one in the end ;-)
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    return true;  // Execute original method
                }
            }

            private static DelegateType GetDelegate<DelegateType>(object instance, string name) where DelegateType : Delegate
            {
                return AccessTools.MethodDelegate<DelegateType>(AccessTools.Method(instance.GetType(), name), instance); ;
            }
        }

        /// <summary>
        /// Harmony patch that fixes the vanilla throw range calculation.
        /// The attenuation tag allows Harmony to find the targeted class/object method and apply the patch from the following class.
        /// </summary>
        [HarmonyPatch(typeof(Weapon), "GetThrowingRange")]
        internal class Weapon_GetThrowingRange_Fix
        {
            /// Using Postfix patch to be guaranteed to get executed.
            public static void Postfix(ref float __result, Weapon __instance, float rangeMultiplier)
            {
                try
                {
                    float num = __instance.TacticalActor.CharacterStats.Endurance * __instance.TacticalActor.TacticalActorDef.EnduranceToThrowMultiplier;
                    float num2 = __instance.TacticalActor.CharacterStats.BonusAttackRange.CalcModValueBasedOn(num);
                    // MadSkunky: Extension of calculation with range multiplier divided by 12 for normalization and multiplier from configuration.
                    num *= __instance.GetDamagePayload().Range / 12f;
                    float multiplier = 1f; // (Main.Config as GrenadeThrowRangeFixConfig).ThrowRangeMultiplier / 100f;
                    __result = ((num / __instance.Weight * rangeMultiplier) + num2) * multiplier;
                    // End of changes
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }
        }


        /// <summary>
        /// Fix to prevent characters given in events from spawning with wrong faction origin
        /// </summary>

        private static List<string> _eventsRewardingNJCharacters = new List<string>() { "AN11", "EX7", "SY22" };

        [HarmonyPatch(typeof(GeoEventChoiceOutcome), "GenerateFactionReward")]
        public static class GeoEventChoiceOutcome_GenerateFactionReward_patch
        {

            public static void Postfix(GeoEventChoiceOutcome __instance, string eventID, ref GeoFactionReward __result)
            {
                try
                {
                    GeoLevelController level = GameUtl.CurrentLevel().GetComponent<GeoLevelController>();

                    if (eventID == "PROG_PU4_WIN" && __result.Units.Count > 0)
                    {
                        __result.Units.Clear();
                        GeoFaction faction2 = level.AnuFaction;
                        GeoUnitDescriptor geoUnitDescriptor = level.CharacterGenerator.GenerateUnit(faction2, __instance.Units[0]);
                        level.CharacterGenerator.ApplyRecruitDifficultyParameters(geoUnitDescriptor);
                        GeoCharacter item2 = geoUnitDescriptor.SpawnAsCharacter();
                        __result.Units.Add(item2);

                    }
                    else if (_eventsRewardingNJCharacters.Contains(eventID) && __result.Units.Count > 0)
                    {
                        __result.Units.Clear();
                        GeoFaction faction2 = level.NewJerichoFaction;
                        GeoUnitDescriptor geoUnitDescriptor = level.CharacterGenerator.GenerateUnit(faction2, __instance.Units[0]);
                        level.CharacterGenerator.ApplyRecruitDifficultyParameters(geoUnitDescriptor);
                        GeoCharacter item2 = geoUnitDescriptor.SpawnAsCharacter();
                        __result.Units.Add(item2);
                    }

                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    throw;
                }
            }
        }


        /// <summary>
        /// Fix to prevent last item being removed in Marketplace when number of offers > 7 
        /// No try/catch because harmless error on buying item
        /// </summary>

        [HarmonyPatch(typeof(UIModuleTheMarketplace), "UpdateList")]
        public static class UIModuleTheMarketplace_UpdateList_patch
        {
            public static bool Prefix(UIModuleTheMarketplace __instance, GeoscapeEvent geoEvent, bool ____isInit,
                List<TheMarketplaceChoiceButton> ____marketplaceChoiceButtons, GeoMarketplace ____geoMarketplace)
            {
                //  try
                //  {
                MethodInfo setChoiceMethod = typeof(TheMarketplaceChoicesController).GetMethod("SetChoice", BindingFlags.NonPublic | BindingFlags.Instance);

                if (____isInit)
                {
                    __instance.ListScrollRect.Scroll.verticalNormalizedPosition = 1f;
                }

                ____marketplaceChoiceButtons.Clear();

                //    TFTVLogger.Always($"____geoMarketplace.MarketplaceChoices.Count {____geoMarketplace.MarketplaceChoices.Count}");

                int count = ____geoMarketplace.MarketplaceChoices.Count;

                if (____geoMarketplace.MarketplaceChoices.Count > 7) //&& !TFTVChangesToDLC5.TFTVMarketPlaceUI.MarketplaceOfferListAdjustedOnce)
                {
                    count = ____geoMarketplace.MarketplaceChoices.Count + 1;
                }



                __instance.ListScrollRect.InitVertical(__instance.MarketplaceChoiceButtonPrefab.GetComponent<TheMarketplaceChoiceButton>(), count, delegate (int index, Component element)
                {
                    TheMarketplaceChoiceButton component = element.GetComponent<TheMarketplaceChoiceButton>();
                    setChoiceMethod.Invoke(__instance.TheMarketplaceChoicesController, new object[] { __instance.Context.ViewerFaction, ____geoMarketplace.MarketplaceChoices[index], component, geoEvent.Context });
                    ____marketplaceChoiceButtons.Add(component);
                });

                // TFTVLogger.Always($"____marketplaceChoiceButtons.Count {____marketplaceChoiceButtons.Count}");

                return false;
                //  }
                /*  catch (Exception e)
                  {
                      TFTVLogger.Error(e);
                      throw;
                  }*/
            }
        }


        /// <summary>
        /// Not strictly a bug, but once partial magazines become visible, without this patch they can be scrapped for the same price as a full one.
        /// This patch ensures that scrapping ammo is never profitable.
        /// </summary>

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

                        float costMultiplier = Math.Max(__instance.ChargesMax / Math.Max(weaponDef.DamagePayload.AutoFireShotCount, weaponDef.DamagePayload.ProjectilesPerShot), 2);




                        __result = new ResourcePack(new ResourceUnit[]
                             {
                        new ResourceUnit(ResourceType.Tech, Mathf.Max(Mathf.FloorToInt(__instance.ManufactureTech / costMultiplier), Mathf.FloorToInt(__instance.ManufactureTech/10))),
                        new ResourceUnit(ResourceType.Materials, Mathf.Max(Mathf.CeilToInt(__instance.ManufactureMaterials / costMultiplier), Mathf.CeilToInt(__instance.ManufactureMaterials/10))),
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




        /// <summary>
        /// Fixes money spent no purchase made at Marketplace if 2 or more aircraft at Marketplace
        /// </summary>
        [HarmonyPatch(typeof(GeoscapeEvent), "CompleteMarketplaceEvent")]
        public static class GeoscapeEvent_CompleteMarketplaceEvent_patch
        {

            public static bool Prefix(GeoscapeEvent __instance, GeoEventChoice choice, GeoFaction faction)
            {
                try
                {
                    // TFTVLogger.Always($"CompleteMarketplaceEvent triggered for choice {choice.Outcome.Items[0].ItemDef?.name}");

                    if (__instance.Context.Site.Vehicles.Count() > 1)
                    {
                        TFTVLogger.Always($"There is a more than one vehicle at {__instance.Context.Site.LocalizedSiteName}! Need to execute alternative code");

                        PropertyInfo propertyInfo = typeof(GeoscapeEvent).GetProperty("ChoiceReward", BindingFlags.Instance | BindingFlags.Public);




                        GeoLevelController component = GameUtl.CurrentLevel().GetComponent<GeoLevelController>();
                        GeoscapeEventContext geoscapeEventContext = new GeoscapeEventContext(component.PhoenixFaction.StartingBase, component.PhoenixFaction, __instance.Context.Site.Vehicles.First());
                        // TFTVLogger.Always($"geoscapeEventContext is null? {geoscapeEventContext==null} is faction null? {faction==null}");

                        propertyInfo?.SetValue(__instance, choice.Outcome.GenerateFactionReward(faction, geoscapeEventContext, __instance.EventID));

                        // TFTVLogger.Always($". is propertyInfo null? {propertyInfo==null} is choiceReward null? {__instance.ChoiceReward==null}");

                        // __instance.ChoiceReward = choice.Outcome.GenerateFactionReward(faction, geoscapeEventContext, __instance.EventID);
                        __instance.ChoiceReward.Apply(faction, geoscapeEventContext.Site, geoscapeEventContext.Vehicle);
                        // TFTVLogger.Always($"2");

                        if (choice.Outcome.ReEneableEvent)
                        {
                            //   TFTVLogger.Always($"");
                            GameUtl.CurrentLevel().GetComponent<GeoscapeEventSystem>().EnableGeoscapeEvent(__instance.EventID);
                        }


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



        //Factions attacking Phoenix bases fix
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


        //Strates fix for bloodlust
        [HarmonyPatch(typeof(DamageAccumulation), "GenerateStandardDamageTargetData")]
        class DamageAccumulation_GenerateStandardDamageTargetData_VanillaBugFix
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


        //Reduce population by 1 when recruiting at havens

        [HarmonyPatch(typeof(GeoHaven), "TakeRecruit")]

        public static class TFTV_GeoHaven_TakeRecruit_VanillaBugBix_patch
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

        //Remove negative damage notices with very large numbers when character with elemental immunity hit by elemental damage
        [HarmonyPatch(typeof(HealthbarUIActorElement), "AddNotificationMessage")]
        public class HealthbarUIActorElement_AddNotificationMessage_VanillaBugFix_Patch
        {
            static bool Prefix(int? val = null)
            {
                try
                {
                    // Check if val is outside the specified range
                    if (val.HasValue && (val.Value > 1000000 || val.Value < -1000000))
                    {
                        //TFTVLogger.Always("it worked");
                        // Return false to cancel the original method call
                        return false;
                    }

                    // Return true to allow the original method call
                    return true;
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    throw;
                }

            }
        }


        //Ensure facilities are working after repairing Power Generator
        [HarmonyPatch(typeof(GeoPhoenixFacility), "SetFacilityFunctioning")]
        public static class GeoPhoenixFacility_SetFacilityFunctioning_AfterGenRepairedVanillaBugFix_Patch
        {
            public static void Postfix(GeoPhoenixFacility __instance)
            {
                try
                {

                    //  TFTVLogger.Always($"SetFacilityFunctioning {__instance.ViewElementDef.name}");

                    if (__instance.GetComponent<PowerFacilityComponent>() != null)
                    {
                        CheckFacilitesNotWorking(__instance.PxBase);
                        //  __instance.PxBase.RoutePower();
                    }

                    //

                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    throw;
                }
            }
        }


        public static void CheckFacilitesNotWorking(GeoPhoenixBase phoenixBase)
        {
            try
            {
                foreach (GeoPhoenixFacility baseFacility in phoenixBase.Layout.Facilities)
                {

                    if (baseFacility.IsPowered && baseFacility.GetComponent<PrisonFacilityComponent>() == null)
                    {
                        baseFacility.SetPowered(false);
                        baseFacility.SetPowered(true);
                    }
                    // TFTVLogger.Always($"{baseFacility.ViewElementDef.name} at {phoenixBase.name} is working? {baseFacility.IsWorking}. is it powered? {baseFacility.IsPowered} ");
                }
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
                throw;
            }
        }


    }
}
