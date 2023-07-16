using Base;
using Base.Core;
using Base.Levels;
using Base.UI;
using HarmonyLib;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities.GameTagsTypes;
using PhoenixPoint.Common.View.ViewControllers;
using PhoenixPoint.Geoscape.Core;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Entities.Missions;
using PhoenixPoint.Geoscape.Events;
using PhoenixPoint.Geoscape.Events.Eventus;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Geoscape.Levels.Objectives;
using PhoenixPoint.Geoscape.View.ViewControllers.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace TFTV
{
    internal class TFTVInfestation
    {
        public static SharedData sharedData = GameUtl.GameComponent<SharedData>();
        //  private static readonly DefRepository Repo = TFTVMain.Repo;
        private static readonly DefCache DefCache = TFTVMain.Main.DefCache;

        internal static GeoHavenDefenseMission DefenseMission = null;
        internal static GeoSite GeoSiteForInfestation = null;
        internal static GeoSite GeoSiteForScavenging = null;
        public static string InfestedHavensVariable = "Number_of_Infested_Havens";
        public static string LivingWeaponsAcquired = "Living_Weapons_Acquired";
        public static int roll = 0;

        public static bool InfestationMissionWon = false;


        
        //force Corruption of the Mind to spawn in a haven covered in Mist

        [HarmonyPatch(typeof(GeoEventChoiceOutcome), "GetRandomSiteForEncounter")]
        public static class GeoEventChoiceOutcome_GetRandomSiteForEncounter_CorruptionOfMind_Patch
        {

            public static void Postfix(ref GeoSite __result, GeoLevelController level, string encounterID, GeoSite contextSite, EarthUnits range, GeoActor rangeReference, bool nearestSite = false, bool removeCurrent = true)
            {
                try 
                {
                    if (encounterID == "PROG_FS3_MISS") 
                    {
                        GeoSite anuHaven = __result;
                        __result = null;

                        if (!anuHaven.IsInMist) 
                        {

                            List<GeoSite> list = level.EventSystem.GetValidSitesForEvent(encounterID).Where(gs=>gs.IsInMist).ToList();
                            if (removeCurrent)
                            {
                                list.Remove(contextSite);
                            }

                            if (list.Count > 0)
                            {
                                anuHaven = list.GetRandomElement();
                            }
                        }

                        level.EventSystem.SetVariable("TrappedInTheMistTriggered", 1);
                        level.EventSystem.SetVariable("Number_of_Infested_Havens", level.EventSystem.GetVariable(InfestedHavensVariable) + 1);
                        level.AlienFaction.InfestHaven(anuHaven);
                        anuHaven.RevealSite(level.PhoenixFaction);
                        anuHaven.RefreshVisuals();

                        /* DiplomaticGeoFactionObjective cyclopsObjective = new DiplomaticGeoFactionObjective(controller.PhoenixFaction, controller.PhoenixFaction)
                  {
                      Title = new LocalizedTextBind("BUILD_CYCLOPS_OBJECTIVE"),
                      Description = new LocalizedTextBind("BUILD_CYCLOPS_OBJECTIVE"),
                  };
                  cyclopsObjective.IsCriticalPath = true;
                  controller.PhoenixFaction.AddObjective(cyclopsObjective);*/


                    }
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                   // return true;
                }
            }

        }



        // Copied and adapted from Mad´s Assorted Adjustments


        // Store mission for other patches
        [HarmonyPatch(typeof(GeoHavenDefenseMission), "UpdateGeoscapeMissionState")]
        public static class GeoHavenDefenseMission_UpdateGeoscapeMissionState_StoreMission_Patch
        {


            public static void Prefix(GeoHavenDefenseMission __instance)
            {
                DefenseMission = __instance;

            }

            public static void Postfix()
            {
                DefenseMission = null;
                roll = 0;

            }
        }

        [HarmonyPatch(typeof(GeoSite), "DestroySite")]
        public static class GeoSite_DestroySite_Patch_ConvertDestructionToInfestation
        {


            public static bool Prefix(GeoSite __instance)
            {
                try
                {
                    if (__instance.Type == GeoSiteType.Haven)
                    {
                        // TFTVLogger.Always("DestroySite method called");
                        TFTVLogger.Always("infestation variable is " + __instance.GeoLevel.EventSystem.GetVariable("Infestation_Encounter_Variable"));

                        if (TFTVInfestationStory.HavenPopulation != 0)
                        {
                            InfestationMissionWon = true;
                        }
                        string faction = __instance.Owner.GetPPName();
                        //  TFTVLogger.Always(faction);
                        if (DefenseMission == null)
                        {

                            return true;
                        }

                        IGeoFactionMissionParticipant attacker = DefenseMission.GetEnemyFaction();
                        if (roll == 0)
                        {
                            roll = UnityEngine.Random.Range(1, 7 + __instance.GeoLevel.CurrentDifficultyLevel.Order);
                            TFTVLogger.Always("Infestation roll is " + roll);
                        }

                        int[] rolledVoidOmens = TFTVVoidOmens.CheckFordVoidOmensInPlay(__instance.GeoLevel);
                        if (attacker.PPFactionDef == sharedData.AlienFactionDef && __instance.IsInMist && __instance.GeoLevel.EventSystem.GetVariable("Infestation_Encounter_Variable") > 0
                         && (roll >= 6 || rolledVoidOmens.Contains(17) || __instance.GeoLevel.EventSystem.GetVariable("TrappedInTheMistTriggered") != 1))//to make infestation more likely
                        {
                            GeoSiteForInfestation = __instance;
                            __instance.ActiveMission = null;
                            __instance.ActiveMission?.Cancel();

                            TFTVLogger.Always("We got to here, defense mission should be successful and haven should look infested");
                            __instance.GeoLevel.EventSystem.SetVariable("Number_of_Infested_Havens", __instance.GeoLevel.EventSystem.GetVariable(InfestedHavensVariable) + 1);

                            __instance.RefreshVisuals();

                            if (__instance.GeoLevel.EventSystem.GetVariable("TrappedInTheMistTriggered") != 1)
                            {
                                GeoscapeEventContext geoscapeEventContext = new GeoscapeEventContext(__instance.GeoLevel.AlienFaction, __instance.GeoLevel.PhoenixFaction);
                                __instance.GeoLevel.EventSystem.SetVariable("TrappedInTheMistTriggered", 1);
                                __instance.GeoLevel.EventSystem.TriggerGeoscapeEvent("OlenaOnHavenInfested", geoscapeEventContext);
                            }


                            return false;
                        }
                        //   int roll2 = UnityEngine.Random.Range(0, 10);
                        /*   if (!__instance.IsInMist && rolledVoidOmens.Contains(12) && roll2 > 5)
                           {
                               GeoSiteForScavenging = __instance;
                               TFTVLogger.Always(__instance.SiteName.LocalizeEnglish() + "ready to spawn a scavenging site");
                           }*/

                        TFTVLogger.Always("Defense mission is not null, the conditions for infestation were not fulfilled, so return true");
                        return true;
                    }
                    return true;
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    return true;
                }
            }
        }


        [HarmonyPatch(typeof(GeoscapeLog), "AddEntry")]
        public static class GeoscapeLog_AddEntry_Patch_ConvertDestructionToInfestation
        {
            public static void Prefix(GeoscapeLogEntry entry)
            {
                try
                {
                    //TFTVLogger.Always("AddEntry method invoked");

                    if (GeoSiteForInfestation != null && GeoSiteForInfestation.SiteName != null && entry != null && entry.Parameters != null && entry.Parameters.Contains(GeoSiteForInfestation.SiteName))
                    {
                        //  TFTVLogger.Always("Attempting to add infestation entry to Log");
                        if (entry == null)
                        {
                            //    TFTVLogger.Always("Failed because entry is null");
                        }
                        else
                        {
                            if (entry.Text == null)
                            {
                                //    TFTVLogger.Always("Failed because entry.text is null");

                            }
                            else
                            {
                                // TFTVLogger.Always("Entry.text is not null");
                                entry.Text = new LocalizedTextBind(GeoSiteForInfestation.Owner + " " + DefenseMission.Haven.Site.Name + " a succombé à l'infestation Pandorienne !", true);  //Calvitix Trad " has succumbed to Pandoran infestation!"
                                // TFTVLogger.Always("The following entry to Log was added" + GeoSiteForInfestation.Owner + " " + DefenseMission.Haven.Site.Name + " has succumbed to Pandoran infestation!");

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

        [HarmonyPatch(typeof(GeoscapeLog), "Map_SiteMissionEnded")]
        public static class GeoscapeLog_Map_SiteMissionEnded_Patch_ConvertDestructionToInfestation
        {

            public static void Postfix(GeoSite site, GeoMission mission)
            {
                try
                {

                    if (GeoSiteForInfestation != null && site == GeoSiteForInfestation && mission is GeoHavenDefenseMission)
                    {
                        TFTVLogger.Always("GeoSiteForInfestation is " + GeoSiteForInfestation.name);
                        site.GeoLevel.AlienFaction.InfestHaven(GeoSiteForInfestation);
                        TFTVLogger.Always("We got to here, haven should be infested!");
                        GeoSiteForInfestation = null;
                    }
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }
        }


        [HarmonyPatch(typeof(InfestedHavenOutcomeDataBind), "ModalShowHandler")]

        public static class InfestedHavenOutcomeDataBind_Patch_ConvertDestructionToInfestation
        {
            // [Obsolete]
            public static bool Prefix(InfestedHavenOutcomeDataBind __instance, UIModal modal, bool ____shown, UIModal ____modal)
            {

                try
                {
                    GeoLevelController controller = GameUtl.CurrentLevel().GetComponent<GeoLevelController>();

                    TFTVLogger.Always("InfestationMissionWon is " + InfestationMissionWon);
                    if (InfestationMissionWon)
                    {
                        if (!____shown)
                        {
                            ____shown = true;
                            if (____modal == null)
                            {
                                ____modal = modal;
                                ____modal.OnModalHide += __instance.ModalHideHandler;
                            }

                            GeoInfestationCleanseMission geoInfestationCleanseMission = (GeoInfestationCleanseMission)modal.Data;
                            GeoSite site = geoInfestationCleanseMission.Site;

                            GeoFaction originalOwner = null;
                            foreach (GeoFaction faction in site.GeoLevel.Factions)
                            {
                                if (faction.PPFactionDef.ShortName == TFTVInfestationStory.OriginalOwner)
                                {
                                    originalOwner = faction;
                                }

                            }

                            List<GeoHaven> geoHavens = originalOwner.Havens.ToList();

                            int populationSaved = (int)(TFTVInfestationStory.HavenPopulation * 0.2);
                            List<GeoHaven> havenToReceiveRefugees = new List<GeoHaven>();

                            foreach (GeoHaven haven in geoHavens)
                            {
                                if (Vector3.Distance(haven.Site.WorldPosition, site.WorldPosition) <= 1.5
                                    && haven.isActiveAndEnabled && !haven.IsInfested && haven.Site != site)
                                {
                                    havenToReceiveRefugees.Add(haven);
                                }
                            }


                            string dynamicDescription = "";

                            if (havenToReceiveRefugees.Count > 0)
                            {
                                dynamicDescription = " Around " + (Mathf.RoundToInt(populationSaved / 100)) * 100 + " of them were fortunate enough to survive and have relocated to friendly nearby havens.";

                            }


                            Text description = __instance.GetComponentInChildren<DescriptionController>().Description;
                            description.GetComponent<I2.Loc.Localize>().enabled = false;
                            description.text = "We destroyed the monstrosity that had taken over the inhabitants of the haven, delivering them from a fate worse than death." + dynamicDescription;
                            Text title = __instance.TopBar.Title;
                            title.GetComponent<I2.Loc.Localize>().enabled = false;
                            title.text = "NODE DESTOYED";


                            __instance.Background.sprite = Helper.CreateSpriteFromImageFile("NodeAlt.jpg");
                            Sprite icon = __instance.CommonResources.GetFactionInfo(site.Owner).Icon;
                            __instance.TopBar.Icon.sprite = icon;
                            __instance.TopBar.Subtitle.text = site.LocalizedSiteName;
                            GeoFactionRewardApplyResult applyResult = geoInfestationCleanseMission.Reward.ApplyResult;
                            __instance.AttitudeChange.SetAttitudes(applyResult.Diplomacy);
                            //  __instance.Rewards.SetItems(InfestationRewardGenerator(1));
                            __instance.Rewards.SetReward(geoInfestationCleanseMission.Reward);

                            TFTVLogger.Always("InfestedHavensVariable before method is " + site.GeoLevel.EventSystem.GetVariable(InfestedHavensVariable));
                            site.GeoLevel.EventSystem.SetVariable(InfestedHavensVariable, site.GeoLevel.EventSystem.GetVariable(InfestedHavensVariable) - 1);
                            TFTVLogger.Always("InfestedHavensVariable is " + site.GeoLevel.EventSystem.GetVariable(InfestedHavensVariable));
                            //  site.GeoLevel.EventSystem.SetVariable(LivingWeaponsAcquired, site.GeoLevel.EventSystem.GetVariable(LivingWeaponsAcquired) + 1);                       

                            if (havenToReceiveRefugees.Count > 0)
                            {
                                TFTVLogger.Always("There are havens that can receive refugees");

                                foreach (GeoHaven haven in havenToReceiveRefugees)
                                {
                                    int refugeesToHaven = populationSaved / havenToReceiveRefugees.Count() - UnityEngine.Random.Range(0, 50);
                                    if (refugeesToHaven > 0)
                                    {
                                        haven.Population += refugeesToHaven;
                                        GeoscapeLogEntry entry = new GeoscapeLogEntry
                                        {
                                            Text = new LocalizedTextBind("Les survivants de '" + refugeesToHaven + "' du refuge " + site.LocalizedSiteName + " ont fuit vers " + haven.Site.LocalizedSiteName, true)  //Calvitix Trad
                                        };
                                        typeof(GeoscapeLog).GetMethod("AddEntry", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(site.GeoLevel.Log, new object[] { entry, null });

                                    }

                                }
                            }

                            TFTVInfestationStory.HavenPopulation = 0;
                            TFTVInfestationStory.OriginalOwner = "";
                            TFTVLogger.Always("LivingWeaponsAcquired variables is " + site.GeoLevel.EventSystem.GetVariable(LivingWeaponsAcquired));

                            string pronoun = "they";
                            string nameMainCharacter = "Phoenix operatives";
                            string plural = "";
                            string possesivePronoun = "them";
                            string havenName = site.LocalizedSiteName;

                            if (FindCharactersOnSite(site).Count() > 0)
                            {
                                if (FindCharactersOnSite(site).First().Identity.Sex == GeoCharacterSex.Female)
                                {
                                    pronoun = "she";
                                    possesivePronoun = "her";
                                }
                                else
                                {
                                    pronoun = "he";
                                    possesivePronoun = "him";
                                }
                                nameMainCharacter = FindCharactersOnSite(site).First().DisplayName;
                                plural = "s";
                            }


                            GeoscapeEventDef LW1Miss = DefCache.GetDef<GeoscapeEventDef>("PROG_LW1_WIN_GeoscapeEventDef");
                            GeoscapeEventDef LW2Miss = DefCache.GetDef<GeoscapeEventDef>("PROG_LW2_WIN_GeoscapeEventDef");
                            GeoscapeEventDef LW3Miss = DefCache.GetDef<GeoscapeEventDef>("PROG_LW3_WIN_GeoscapeEventDef");

                            LocalizedTextBind lWDescription1 = new LocalizedTextBind(
                                    "Alors que votre équipe était à la recherche de survivants et d'objets pouvant être récupérés dans le Refuge de " + havenName + ", ils sont tombés sur une ancienne clinique. " +
                                    "Elle est pleine d'horreurs : des corps mutilés jonchent le sol et les murs sont couverts de symboles incompréhensibles griffonnés dans un sang épais et sombre. " +
                                    "\r\n\r\nL'une des tables d'examen a attiré l'attention de " + nameMainCharacter + ": \r\n\"On dirait un autel dans un temple... Est-ce une offrande ? Whoa, c'est vivant !\"\r\n\r\n" +
                                    "Il s'agit en fait d'une remarquable combinaison blindée : un organisme vivant muté, fabriqué par bio-ingénierie à l'aide d'une technologie inconnue. \r\n", true);

                            LocalizedTextBind lWDescription2 = new LocalizedTextBind("Comme dans le premier refuge infesté, vos agents rencontrent un autre temple des horreurs installé " +
                                "par les infectés; cette fois dans un entrepôt souterrain.\r\n\r\nDans un coin du sol, un homme est assis, apparemment perdu dans ses pensées, " +
                                "une arme étrange est posée devant lui. Lorsque " + nameMainCharacter + " se rapproche" + plural + " plus près, " + pronoun + " remarque" + plural + " que l'homme est couvert de brûlures à l'acide et qu'il est complètement catatonique. " +
                                "D'après les haillons de son uniforme, il a dû être concierge à une époque.\r\n", true);

                            LocalizedTextBind lWDescription3text = new LocalizedTextBind("Un homme arrive en courant, en trébuchant et en tombant à travers les ruines de " + havenName + ", s'approchant de votre escouade. " +
                                "Tout d'abord " + nameMainCharacter + " lui prête" + plural + " des intentions hostiles, provoquées par la folie, et envisage" + plural + " de lui tirer dessus. " +
                                "\r\n\r\n“J'ai quelque chose pour vous, j'ai quelque chose pour vous ! Vous <i>devez</i> la prendre ! Venez avec moi!”\r\n", true);

                            LocalizedTextBind lWDescription3outcome = new LocalizedTextBind("Alors que vos agents le poursuivent, il continue à courir devant lui, puis à revenir, tout en répétant " +
                                "“Vous devez le prendre ! Vous devez le prendre ! Oui ! Prends-le, et Constant Malachi sera libre ! Libre !”" +
                                "\r\n\r\nMalachi emmène votre équipe dans un atelier situé derrière le générateur principal du Refuge. " +
                                "Sur un établi se trouve un autre appareil mystérieux qui ressemble à une arme lourde à projectiles multiples. " +
                                "Il jette un regard suppliant à " + nameMainCharacter + ", "+ possesivePronoun + " priant de prendre l'arme." +
                                "\r\n\r\nLorsqu'" + pronoun + " le fait, Malachi éclate d'une joie sans limites. “Ils m'ont choisi pour te le donner ! Quelqu'un là-haut m'aime vraiment !”" +
                                "\r\n\r\nMalheureusement, cet effort est trop important pour lui. Il s'effondre, mort, mais avec un sourire de béatitude sur le visage. \r\n", true);


                            if (site.GeoLevel.EventSystem.GetVariable(LivingWeaponsAcquired) == 0)
                            {
                                LW1Miss.GeoscapeEventData.Description[0].General = lWDescription1;
                                GeoscapeEventContext context = new GeoscapeEventContext(site, site.GeoLevel.PhoenixFaction);
                                site.GeoLevel.EventSystem.TriggerGeoscapeEvent("PROG_LW1_WIN", context);
                                site.GeoLevel.EventSystem.SetVariable(LivingWeaponsAcquired, 1);
                            }
                            else if (site.GeoLevel.EventSystem.GetVariable(LivingWeaponsAcquired) == 1)
                            {
                                LW2Miss.GeoscapeEventData.Description[0].General = lWDescription2;
                                GeoscapeEventContext context = new GeoscapeEventContext(site, site.GeoLevel.PhoenixFaction);
                                site.GeoLevel.EventSystem.TriggerGeoscapeEvent("PROG_LW2_WIN", context);
                                site.GeoLevel.EventSystem.SetVariable(LivingWeaponsAcquired, 2);

                            }
                            else if (site.GeoLevel.EventSystem.GetVariable(LivingWeaponsAcquired) == 2)
                            {
                                LW3Miss.GeoscapeEventData.Description[0].General = lWDescription3text;
                                LW3Miss.GeoscapeEventData.Choices[0].Outcome.OutcomeText.General = lWDescription3outcome;
                                GeoscapeEventContext context = new GeoscapeEventContext(site, site.GeoLevel.PhoenixFaction);
                                site.GeoLevel.EventSystem.TriggerGeoscapeEvent("PROG_LW3_WIN", context);
                                site.GeoLevel.EventSystem.SetVariable(LivingWeaponsAcquired, 3);
                            }
                        }
                        InfestationMissionWon = false;


                        return false;
                    }
                    else
                    {
                        Text description = __instance.GetComponentInChildren<DescriptionController>().Description;
                        description.GetComponent<I2.Loc.Localize>().enabled = false;
                        description.text = "We failed to destroy the monstrosity that has taken over the inhabitants of the haven.";
                        Text title = __instance.TopBar.Title;
                        title.GetComponent<I2.Loc.Localize>().enabled = false;
                        title.text = "MISSION FAILED";


                        __instance.Background.sprite = Helper.CreateSpriteFromImageFile("Node.jpg");



                        return true;

                    }
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                    return true;
                }

            }
        }

        public static List<GeoCharacter> FindCharactersOnSite(GeoSite site)
        {
            try
            {

                List<GeoCharacter> eligibleCharacters = new List<GeoCharacter>();
                List<GeoCharacter> orderedOperatives = new List<GeoCharacter>();

                //  TFTVLogger.Always("There are " + site.GetPlayerVehiclesOnSite().Count() + " player vehicles on site");

                foreach (GeoVehicle geoVehicle in site.GetPlayerVehiclesOnSite())
                {
                    foreach (GeoCharacter geoCharacter in geoVehicle.Soldiers)
                    {
                        //  TFTVLogger.Always("There are " + geoVehicle.Soldiers.Count() + " soldiers in vehicle " + geoVehicle.Name);


                        if (geoCharacter.TemplateDef.IsHuman)
                        {

                            eligibleCharacters.Add(geoCharacter);

                            // TFTVLogger.Always("Character " + geoCharacter.DisplayName + " added to list");
                        }

                    }


                }


                if (eligibleCharacters.Count > 0)
                {
                    orderedOperatives = eligibleCharacters.OrderByDescending(e => e.LevelProgression.Experience).ToList();

                }

                // TFTVLogger.Always("ordeded operatives counts " + orderedOperatives.Count);

                return orderedOperatives;

            }

            catch (Exception e)
            {
                TFTVLogger.Error(e);

            }
            throw new InvalidOperationException();
        }
    }


}

