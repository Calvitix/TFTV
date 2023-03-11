using HarmonyLib;
using PhoenixPoint.Common.ContextHelp;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.Entities.GameTagsTypes;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Levels;
using PhoenixPoint.Tactical.View.ViewStates;
using System;
using System.Collections.Generic;
using System.Linq;


namespace TFTV
{
    internal class TFTVInfestationStory
    {
        //  private static readonly DefRepository Repo = TFTVMain.Repo;
        private static readonly DefCache DefCache = TFTVMain.Main.DefCache;

        private static readonly GameTagDef mutoidTag = DefCache.GetDef<GameTagDef>("Mutoid_TagDef");
        private static readonly GameTagDef nodeTag = DefCache.GetDef<GameTagDef>("CorruptionNode_ClassTagDef");

        private static readonly MissionTypeTagDef infestationMissionTagDef = DefCache.GetDef<MissionTypeTagDef>("HavenInfestation_MissionTypeTagDef");
       
        public static int HavenPopulation = 0;
        public static string OriginalOwner = "";
      

        [HarmonyPatch(typeof(TacticalLevelController), "ActorDied")]
        public static class TacticalLevelController_ActorDied_InfestationOutro_Patch
        {
            public static void Prefix(DeathReport deathReport, TacticalLevelController __instance)
            {
                try
                {
                    if (deathReport.Actor.HasGameTag(nodeTag))
                    {
                        CreateOutroInfestation(__instance);   
                                       
                    }

                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }

            public static void Postfix(DeathReport deathReport)
            {
                try
                {
                    if (deathReport.Actor.HasGameTag(nodeTag))
                    {
                       // CreateOutroInfestation(__instance);
                       
                    }

                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(GeoMission), "Launch")]
        public static class GeoMission_Launch_InfestationStory_Patch
        {
            public static void Postfix(GeoMission __instance, GeoSquad squad)
            {
                try
                {
                    if (__instance.MissionDef.Tags.Contains(infestationMissionTagDef))
                    {

                        List<GeoHaven> geoHavens = __instance.Site.GeoLevel.AlienFaction.Havens.ToList();
                        GeoHaven geoHaven = new GeoHaven();

                        foreach (GeoHaven haven in geoHavens)
                        {
                            if (haven.Site.SiteId == __instance.Site.SiteId)
                            {
                                geoHaven = haven;

                            }
                        }
                        TFTVLogger.Always("The haven is " + geoHaven.Site.LocalizedSiteName + " and its population is " + geoHaven.Population);

                        HavenPopulation = geoHaven.Population;
                        OriginalOwner = geoHaven.OriginalOwner.PPFactionDef.ShortName;

                        List<GeoCharacter> operatives = new List<GeoCharacter>();

                        foreach (GeoCharacter geoCharacter in squad.Soldiers)
                        {
                            if (!geoCharacter.IsMutoid)
                            {
                                operatives.Add(geoCharacter);
                            }
                        }

                        if (operatives.Count < 2)
                        {

                        }
                        else
                        {
                           
                            TFTVLogger.Always("There are " + operatives.Count() + " phoenix operatives");
                            List<GeoCharacter> orderedOperatives = operatives.OrderByDescending(e => e.LevelProgression.Experience).ToList();
                            string characterName = "";
                            
                            for (int i = 0; i < operatives.Count; i++)
                            {
                                TFTVLogger.Always("Phoenix operative is " + orderedOperatives[i].DisplayName + " with XP " + orderedOperatives[i].LevelProgression.Experience);
                                TFTVLogger.Always("The count is " + orderedOperatives[i].DisplayName.Split().Count());
                                if (orderedOperatives[i].DisplayName.Split().Count()> 1)
                                {
                                    TFTVLogger.Always("The first name of the operative is " + orderedOperatives[i].DisplayName.Split()[1]);
                                    characterName = orderedOperatives[i].DisplayName.Split()[1];
                                }
                                else 
                                {
                                    TFTVLogger.Always("The operative " + orderedOperatives[i].DisplayName + " doesn't have a first or last name");
                                    characterName = orderedOperatives[i].DisplayName;
                                }
                            }

                            string name = "InfestationMissionIntro";
                            string title = "Recherche et sauvetage"; // "Search and Rescue"; 
                            string text = "Directeur, " + characterName + " au rapport. Nous sommes à " + __instance.Site.LocalizedSiteName + ". Vous voyez ça ? Ce vert scintillant... Je... J'ai l'impression d'être déjà venu ici...";



                            string reply = characterName + " ne vous laissez pas abattre ! " +
                                "Nous sommes toujours des agents de Phoenix et nous avons un travail à faire. " +
                                "Les scanners montrent qu'il y a des survivants. Gardez votre sang froid et soyez prêts à tout. " + orderedOperatives[0].DisplayName + " terminé.";

                            ContextHelpHintDef infestationIntro2 = DefCache.GetDef<ContextHelpHintDef>(name + "2");
                            ContextHelpHintDef infestationIntro = DefCache.GetDef<ContextHelpHintDef>(name);
                            infestationIntro.Trigger = HintTrigger.MissionStart;
                            infestationIntro.NextHint = infestationIntro2;


                            infestationIntro.Text = new Base.UI.LocalizedTextBind(text, true);
                            infestationIntro.Title = new Base.UI.LocalizedTextBind(title, true);
                            infestationIntro2.Text = new Base.UI.LocalizedTextBind(reply, true);
                            infestationIntro2.Title = new Base.UI.LocalizedTextBind(title, true);
                        }
                    }
                }
                catch (Exception e)
                {
                    TFTVLogger.Error(e);
                }
            }


        }

        public static void CreateOutroInfestation(TacticalLevelController level)
        {
            try
            {
                if (GetTacticalActorsPhoenix(level).Count >= 1)
                {
                    string nameOfOperative = GetTacticalActorsPhoenix(level)[0].DisplayName;
                    string title = "Le Réveil";
                    string text = " <i>”Il y avait quelques survivants ici et là. Les quelques soldats qui ont eu la chance d'être contrôlés lors de l'attaque initiale; " +
                        "mais aussi des civils, qui n'ont pas été totalement saisis par la créature. Ils s'en sont sortis, comme s'ils se réveillaient d'un cauchemar. " +
                        "C'est alors que j'ai compris ce qu'Alistair voulait dire lorsqu'il disait en plaisantant que nous étions en train de mener une guerre pour notre place dans la nouvelle chaîne alimentaire : " +
                        "Les Pandoriens ne veulent pas nous exterminer. Cela aurait été trop clément. Ils nous veulent pour autre chose.”</i>\n\n" + nameOfOperative
                        + ", Projet Phoenix";

                    ContextHelpHintDef infestationOutro = DefCache.GetDef<ContextHelpHintDef>("InfestationMissionEnd");
                    infestationOutro.Trigger = HintTrigger.MissionOver;

                    infestationOutro.Text = new Base.UI.LocalizedTextBind(text, true);
                    infestationOutro.Title = new Base.UI.LocalizedTextBind(title, true);



                }
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }



        public static List<TacticalActor> GetTacticalActorsPhoenix(TacticalLevelController level)
        {
            try
            {
                TacticalFaction phoenix = level.GetFactionByCommandName("PX");

                List<TacticalActor> operatives = new List<TacticalActor>();

                foreach (TacticalActorBase tacticalActorBase in phoenix.Actors)
                {
                    TacticalActor tacticalActor = tacticalActorBase as TacticalActor;

                    if (tacticalActorBase.BaseDef.name == "Soldier_ActorDef" && tacticalActorBase.InPlay && !tacticalActorBase.HasGameTag(mutoidTag)
                        && tacticalActorBase.IsAlive && level.TacticalGameParams.Statistics.LivingSoldiers.ContainsKey(tacticalActor.GeoUnitId))
                    {

                        operatives.Add(tacticalActor);
                    }
                }

                if (operatives.Count == 0)
                {
                    return null;
                }

                TFTVLogger.Always("There are " + operatives.Count() + " phoenix operatives");
                List<TacticalActor> orderedOperatives = operatives.OrderByDescending(e => GetNumberOfMissions(e)).ToList();
                for (int i = 0; i < operatives.Count; i++)
                {
                    TFTVLogger.Always("TacticalActor is " + orderedOperatives[i].DisplayName + " and # of missions " + GetNumberOfMissions(orderedOperatives[i]));
                }
                return orderedOperatives;

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
            throw new InvalidOperationException();

        }

        public static int GetNumberOfMissions(TacticalActor tacticalActor)
        {
            try
            {
                TacticalLevelController level = tacticalActor.TacticalFaction.TacticalLevel;

                int numberOfMission = level.TacticalGameParams.Statistics.LivingSoldiers[tacticalActor.GeoUnitId].MissionsParticipated;

                return numberOfMission;


            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
            throw new InvalidOperationException();

        }



    }


}
