using Base.Serialization.General;
using PhoenixPoint.Modding;
using PhoenixPoint.Tactical.Levels;
using System;
using System.Collections.Generic;

namespace TFTV
{
    
    /// <summary>
    /// Mod's custom save data for tactical.
    /// </summary>
    [SerializeType(SerializeMembersByDefault = SerializeMembersType.SerializeAll)]
    public class TFTVTacInstanceData
    {
        // public int ExampleData;
        // Dictionary to transfer the characters geoscape stamina to tactical level by actor ID
        public List<int> charactersWithBrokenLimbs; // = TFTVStamina.charactersWithBrokenLimbs;
        public int TBTVVariable;
        public bool UmbraResearched;
        public Dictionary<int, int> DeadSoldiersDelirium;// = TFTVRevenant.DeadSoldiersDelirium;
                                                         //    public TimeUnit timeRevenantLastSeenSaveData = TFTVRevenant.timeRevenantLastSeen;
                                                         //    public TimeSpan timeLastRevenantSpawned = TFTVRevenant.timeLastRevenantSpawned;
        public List<string> revenantSpecialResistance;// = TFTVRevenant.revenantSpecialResistance;
        public bool revenantSpawned;// = TFTVRevenant.revenantSpawned;
        public bool revenantCanSpawnSaveDate;// = TFTVRevenant.revenantCanSpawn;
        public Dictionary<string, int> humanEnemiesLeaderTacticsSaveData;// = TFTVHumanEnemies.HumanEnemiesAndTactics;
        public int difficultyLevelForTacticalSaveData;// = TFTVHumanEnemies.difficultyLevel;
        public int infestedHavenPopulationSaveData;// = TFTVInfestationStory.HavenPopulation;
        public string infestedHavenOriginalOwnerSaveData;// = TFTVInfestationStory.OriginalOwner;
        public Dictionary<int, int[]> ProjectOsirisStatsTacticalSaveData;// = TFTVRevenantResearch.ProjectOsirisStats;
        public bool ProjectOrisisCompletedSaveData;// = TFTVRevenantResearch.ProjectOsiris;
        public int RevenantId;
        public bool[] VoidOmensCheck = TFTVVoidOmens.VoidOmensCheck;
        //Commented out for release #12
        public int HoplitesKilledOnMission;
        public bool LOTAReworkActiveInTactical;
        public bool TurnZeroMethodsExecuted;

    }

    /// <summary>
    /// Represents a mod instance specific for Tactical game.
    /// Each time Tactical level is loaded, new mod's ModTactical is created. 
    /// </summary>
    public class TFTVTactical : ModTactical
    {
        public static bool TurnZeroMethodsExecuted = false;
        /// <summary>
        /// Called when Tactical starts.
        /// </summary>
        public override void OnTacticalStart()
        {

            /// Tactical level controller is accessible at any time.
            TacticalLevelController tacController = Controller;

            /// ModMain is accesible at any time

            TFTVLogger.Always("Tactical Started");
            TFTVLogger.Always("The count of tactics in play is " + TFTVHumanEnemies.HumanEnemiesAndTactics.Count);
            TFTVLogger.Always("VO3 Active " + TFTVVoidOmens.VoidOmensCheck[3]);
            TFTVLogger.Always("VO5 Active " + TFTVVoidOmens.VoidOmensCheck[5]);
            TFTVLogger.Always("VO7 Active " + TFTVVoidOmens.VoidOmensCheck[7]);
            TFTVLogger.Always("VO10 Active " + TFTVVoidOmens.VoidOmensCheck[10]);
            TFTVLogger.Always("VO15 Active " + TFTVVoidOmens.VoidOmensCheck[15]);
            TFTVLogger.Always("VO16 Active " + TFTVVoidOmens.VoidOmensCheck[16]);
            TFTVLogger.Always("VO19 Active " + TFTVVoidOmens.VoidOmensCheck[19]);
            TFTVLogger.Always("Project Osiris researched " + TFTVRevenantResearch.ProjectOsiris);

            TFTVHumanEnemies.RollCount = 0;
            TFTVAncients.CheckCyclopsDefense();
            TFTVLogger.Always("Tactical start completed");
            TFTVLogger.Always("Difficulty level is " + tacController.Difficulty.Order);
            TFTVLogger.Always("LOTA rework active in tactical is " + TFTVAncients.LOTAReworkActive);
            TFTVAncients.CheckResearchStateOnTacticalStart();

        }

        /// <summary>
        /// Called when Tactical ends.
        /// </summary>
        public override void OnTacticalEnd()
        {

            TFTVLogger.Always("OnTacticalEnd check");
            TFTVRevenant.revenantCanSpawn = false;

            TFTVRevenantResearch.CheckRevenantCapturedOrKilled(Controller);

            base.OnTacticalEnd();

        }

        /// <summary>
        /// Called when Tactical save is being process. At this point level is already created, but TacticalStart is not called.
        /// </summary>
        /// <param name="data">Instance data serialized for this mod. Cannot be null.</param>
        public override void ProcessTacticalInstanceData(object instanceData)
        {
            try
            {
                TFTVLogger.Always("Tactical save is being processed");
                TFTVCommonMethods.ClearInternalVariables();
                TFTVTacInstanceData data = (TFTVTacInstanceData)instanceData;
                TFTVStamina.charactersWithBrokenLimbs = data.charactersWithBrokenLimbs;
                TFTVVoidOmens.VoidOmensCheck = data.VoidOmensCheck;
                TFTVUmbra.TBTVVariable = data.TBTVVariable;
                TFTVUmbra.UmbraResearched = data.UmbraResearched;
                TFTVRevenant.DeadSoldiersDelirium = data.DeadSoldiersDelirium;

                TFTVRevenant.revenantSpawned = data.revenantSpawned;
                TFTVRevenant.revenantSpecialResistance = data.revenantSpecialResistance;
                TFTVRevenant.revenantCanSpawn = data.revenantCanSpawnSaveDate;
                TFTVRevenantResearch.ProjectOsirisStats = data.ProjectOsirisStatsTacticalSaveData;
                TFTVRevenantResearch.ProjectOsiris = data.ProjectOrisisCompletedSaveData;

                TFTVHumanEnemies.HumanEnemiesAndTactics = data.humanEnemiesLeaderTacticsSaveData;
                TFTVInfestationStory.HavenPopulation = data.infestedHavenPopulationSaveData;
                TFTVInfestationStory.OriginalOwner = data.infestedHavenOriginalOwnerSaveData;
                TFTVRevenant.revenantID = data.RevenantId;
                //Commented out for release #13
                TFTVAncients.HoplitesKilled = data.HoplitesKilledOnMission;
                TFTVAncients.LOTAReworkActive = data.LOTAReworkActiveInTactical;
                TurnZeroMethodsExecuted = data.TurnZeroMethodsExecuted;
                TFTVBetaSaveGamesFixes.CheckNewLOTASavegame();
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }
        /// <summary>
        /// Called when Tactical save is going to be generated, giving mod option for custom save data.
        /// </summary>
        /// <returns>Object to serialize or null if not used.</returns>
        public override object RecordTacticalInstanceData()
        {
            TFTVLogger.Always("Tactical data will be saved");
            return new TFTVTacInstanceData()
            {
                charactersWithBrokenLimbs = TFTVStamina.charactersWithBrokenLimbs,
                VoidOmensCheck = TFTVVoidOmens.VoidOmensCheck,
                TBTVVariable = TFTVUmbra.TBTVVariable,
                UmbraResearched = TFTVUmbra.UmbraResearched,
                DeadSoldiersDelirium = TFTVRevenant.DeadSoldiersDelirium,
                revenantSpawned = TFTVRevenant.revenantSpawned,
                revenantSpecialResistance = TFTVRevenant.revenantSpecialResistance,
                revenantCanSpawnSaveDate = TFTVRevenant.revenantCanSpawn,
                ProjectOsirisStatsTacticalSaveData = TFTVRevenantResearch.ProjectOsirisStats,
                ProjectOrisisCompletedSaveData = TFTVRevenantResearch.ProjectOsiris,
                humanEnemiesLeaderTacticsSaveData = TFTVHumanEnemies.HumanEnemiesAndTactics,
                infestedHavenPopulationSaveData = TFTVInfestationStory.HavenPopulation,
                infestedHavenOriginalOwnerSaveData = TFTVInfestationStory.OriginalOwner,
                RevenantId = TFTVRevenant.revenantID,
                HoplitesKilledOnMission = TFTVAncients.HoplitesKilled,
                LOTAReworkActiveInTactical = TFTVAncients.LOTAReworkActive,
                TurnZeroMethodsExecuted = TurnZeroMethodsExecuted


            };

        }
        /// <summary>
        /// Called when new turn starts in tactical. At this point all factions must play in their order.
        /// </summary>
        /// <param name="turnNumber">Current turn number</param>
        public override void OnNewTurn(int turnNumber)
        {
            try
            {
                TFTVLogger.Always("The turn is " + turnNumber);

                if (!Controller.TacMission.MissionData.MissionType.name.Contains("Tutorial"))
                {

                    if (turnNumber == 0 && TFTVHumanEnemies.HumanEnemiesAndTactics.Count == 0)
                    {
                        TFTVHumanEnemies.CheckMissionType(Controller);
                    }

                    if (turnNumber == 0 && TFTVAncients.CheckIfAncientsPresent(Controller))
                    {
                        TFTVAncients.AdjustAncientsOnDeployment(Controller);
                    }

                    if (turnNumber == 0 && !TurnZeroMethodsExecuted)
                    {          
                            TFTVLogger.Always("Turn 0 check");
                            TFTVRevenant.ModifyRevenantResistanceAbility(Controller);
                            TFTVRevenant.CheckForNotDeadSoldiers(Controller);
                            TFTVRevenant.RevenantCheckAndSpawn(Controller);
                            TFTVRevenant.ImplementVO19(Controller);
                            TFTVVoidOmens.VO5TurnHostileCivviesFriendly(Controller);
                            TurnZeroMethodsExecuted = true;
                    }

                    TFTVRevenant.revenantSpecialResistance.Clear();
                    TFTVUmbra.SpawnUmbra(Controller);
                    TFTVHumanEnemies.ChampRecoverWPAura(Controller);
                    TFTVHumanEnemies.ApplyTactic(Controller);
                }

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);

            }

        }
    }
}