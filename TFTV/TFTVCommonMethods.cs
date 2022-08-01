﻿using Base.Defs;
using Base.Eventus.Filters;
using Base.UI;
using HarmonyLib;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Entities.Research;
using PhoenixPoint.Geoscape.Events;
using PhoenixPoint.Geoscape.Events.Eventus;
using PhoenixPoint.Geoscape.Events.Eventus.Filters;
using PhoenixPoint.Geoscape.Levels;
using System;
using System.Linq;
using System.Reflection;

namespace TFTV
{
    internal class TFTVCommonMethods
    {
        private static readonly DefRepository Repo = TFTVMain.Repo;



        public static void SetStaminaToZero(GeoCharacter __instance)
        {
            try
            {
                if (__instance.Fatigue != null && __instance.Fatigue.Stamina > 0 && __instance.TemplateDef.IsHuman && __instance.TemplateDef.IsMutoid)
                {
                    __instance.Fatigue.Stamina.SetToMin();
                }
            }

            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }
        public static void GenerateGeoEventChoice(GeoscapeEventDef geoEvent, string choice, string outcome)
        {
            try
            {
                geoEvent.GeoscapeEventData.Choices.Add(new GeoEventChoice()

                {
                    Text = new LocalizedTextBind(choice),
                    Outcome = new GeoEventChoiceOutcome()
                    {
                        OutcomeText = new EventTextVariation()
                        {
                            General = new LocalizedTextBind(outcome)
                        }
                    }
                });
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

        public static OutcomeDiplomacyChange GenerateDiplomacyOutcome(GeoFactionDef partyFaction, GeoFactionDef targetFaction, int value)
        {
            try
            {
                return new OutcomeDiplomacyChange()
                {
                    PartyFaction = partyFaction,
                    TargetFaction = targetFaction,
                    Value = value,
                    PartyType = (OutcomeDiplomacyChange.ChangeTarget)1,
                };
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
            throw new InvalidOperationException();
        }

        public static OutcomeVariableChange GenerateVariableChange(string variableName, int value, bool isSet)
        {
            try
            {
                return new OutcomeVariableChange()
                {
                    VariableName = variableName,
                    Value = { Min = value, Max = value },
                    IsSetOperation = isSet,
                };
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
            throw new InvalidOperationException();
        }

        public static GeoscapeEventDef CreateNewEvent(string name, string title, string description, string outcome)
        {
            try
            {

                string gUID = Guid.NewGuid().ToString();
                GeoscapeEventDef sourceLoseGeoEvent = Repo.GetAllDefs<GeoscapeEventDef>().FirstOrDefault(ged => ged.name.Equals("PROG_PU12_FAIL_GeoscapeEventDef"));
                GeoscapeEventDef newEvent = Helper.CreateDefFromClone(sourceLoseGeoEvent, gUID, name);
                newEvent.GeoscapeEventData.Choices[0].Outcome.ReEneableEvent = false;
                newEvent.GeoscapeEventData.Choices[0].Outcome.ReactiveEncounters.Clear();
                newEvent.GeoscapeEventData.EventID = name;
                newEvent.GeoscapeEventData.Title.LocalizationKey = title;
                newEvent.GeoscapeEventData.Description[0].General.LocalizationKey = description;
                if (outcome != null)
                {
                    newEvent.GeoscapeEventData.Choices[0].Outcome.OutcomeText.General.LocalizationKey = outcome;
                }
                return newEvent;
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
            throw new InvalidOperationException();
        }

        public static ResearchDef CreateNewPXResearch(string id, int cost, string gUID, ResearchViewElementDef researchViewElementDef)

        {
            try
            {
                ResearchDef sourceResearchDef = Repo.GetAllDefs<ResearchDef>().FirstOrDefault(ged => ged.name.Equals("PX_AtmosphericAnalysis_ResearchDef"));
                ResearchDef researchDef = Helper.CreateDefFromClone(sourceResearchDef, gUID, id);
                ResearchDef secondarySourceResearchDef = Repo.GetAllDefs<ResearchDef>().FirstOrDefault(ged => ged.name.Equals("PX_AlienGoo_ResearchDef"));


                researchDef.ResearchCost = cost;
                researchDef.ViewElementDef = researchViewElementDef;
                researchDef.Unlocks = secondarySourceResearchDef.Unlocks;
                researchDef.Tags = secondarySourceResearchDef.Tags;
                return researchDef;
            }

            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
            throw new InvalidOperationException();
        }

        public static ResearchViewElementDef CreateNewResearchViewElement(string def, string gUID, string name, string reveal, string unlock, string complete)

        {
            try
            {

                ResearchViewElementDef sourceResearchViewDef = Repo.GetAllDefs<ResearchViewElementDef>().FirstOrDefault(ged => ged.name.Equals("PX_Alien_CorruptionNode_ViewElementDef"));
                ResearchViewElementDef researchViewDef = Helper.CreateDefFromClone(sourceResearchViewDef, gUID, def);
                researchViewDef.DisplayName1.LocalizationKey = name;
                researchViewDef.RevealText.LocalizationKey = reveal;
                researchViewDef.UnlockText.LocalizationKey = unlock;
                researchViewDef.CompleteText.LocalizationKey = complete;
                return researchViewDef; 
            }

            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
            throw new InvalidOperationException();
        }

        [HarmonyPatch(typeof(GeoSite), "CreateHavenDefenseMission")]
        public static class GeoSite_CreateHavenDefenseMission_RevealHD_Patch
        {
            public static void Postfix(GeoSite __instance)
            {
                try 
                { 
                    __instance.RevealSite(__instance.GeoLevel.PhoenixFaction);
                    GeoscapeLogEntry entry = new GeoscapeLogEntry
                    {
                        Text = new LocalizedTextBind(__instance.Owner + " " + __instance.LocalizedSiteName + " is broadcasting an SOS, they are under attack!", true)
                    };
                    typeof(GeoscapeLog).GetMethod("AddEntry", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance.GeoLevel.Log, new object[] { entry, null });

                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }
        }

    }
}

