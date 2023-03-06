using Base.Defs;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Weapons;
using System.Linq;
using PhoenixPoint.Geoscape.Entities.Research.Reward;
using PhoenixPoint.Geoscape.Entities.Research;
using PhoenixPoint.Geoscape.Entities.PhoenixBases.FacilityComponents;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using System.Collections.Generic;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Common.Entities.Addons;
using Base.Core;
using PhoenixPoint.Common.Entities.GameTagsTypes;
using Code.PhoenixPoint.Tactical.Entities.Equipments;
using Base.Entities.Abilities;
using Base.Entities.Effects;
using Base.Entities.Statuses;
using Base.UI;
using PhoenixPoint.Tactical.Entities.Animations;
using PhoenixPoint.Tactical.Entities.Effects.ApplicationConditions;
using PhoenixPoint.Tactical.Entities.Statuses;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Tactical.Entities.Effects.DamageTypes;
using HarmonyLib;
using I2.Loc;
using System;
using PhoenixPoint.Geoscape.Levels.Factions;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical.Entities.Effects;

namespace TFTV.FlashGrenade
{
    internal class FlashGrenade
    {
        private static readonly DefRepository Repo = TFTVMain.Repo;
        private static  readonly SharedData Shared = TFTVMain.Shared;
        public static void Apply()
        {
            Clone_BlindStatus();
            Clone_SonicDamageKeyWord();
            Clone_SonicGrenade();                     
            AddToStartingManufactureItems();
            AddToEquipmentAnimation();
            
        }

        public static void Clone_SonicGrenade()
        {
            string skillName3 = "SY_FlashGrenade_WeaponDef";
            WeaponDef pxGrenade = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("PX_HandGrenade_WeaponDef"));
            WeaponDef source3 = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("SY_SonicGrenade_WeaponDef"));
            WeaponDef FlashBang = Helper.CreateDefFromClone(
                source3,
                "966A5726-8862-46E4-B7D1-D839E9026338",
                skillName3);
            FlashBang.ViewElementDef = Helper.CreateDefFromClone(
                source3.ViewElementDef,
                "EF112ECF-8431-4F2B-A6D0-CBFFFCC91222",
                "E_View [SY_FlashGrenade_WeaponDef]");
            //FlashBang.DamagePayload.ProjectileVisuals = Helper.CreateDefFromClone(
            //    source3.DamagePayload.ProjectileVisuals,
            //    "CAE30D7E-C5D2-4FC0-911D-2C64870A167D",
            //    "E_Projectile [SY_FlashGrenade_WeaponDef]");
            //FlashBang.DamagePayload.DamageType = Helper.CreateDefFromClone(
            //    source3.DamagePayload.DamageType,
            //    "3F1605FD-810A-4F6D-8CAC-20D992B58501",
            //    "Electroshock_AttenuatingDamageTypeEffectDef");
            FlashBang.DamagePayload.DamageKeywords[0].DamageKeywordDef = Helper.CreateDefFromClone(
                source3.DamagePayload.DamageKeywords[0].DamageKeywordDef,
                "2E6047C3-525B-4530-B84F-49DE1C33E0C3",
                "Flash_DamageKeywordDataDef");
            //FlashBang.DamagePayload.ProjectileVisuals = Helper.CreateDefFromClone(
            //    source3.DamagePayload.ProjectileVisuals,
            //    "14BBC013-2AA8-4527-9EBF-71CCFDC08CA3",
            //    skillName3);
            //FlashBang.ShootingEvent = Helper.CreateDefFromClone(
            //    source3.ShootingEvent,
            //    "52184BB3-FF17-4DBE-857E-462D35297AD4",
            //    "ShootShot_EventDef");
            //FlashBang.HolsterSlotDef = Helper.CreateDefFromClone(
            //    source3.HolsterSlotDef,
            //    "EE4EA3E3-0975-4399-85EB-BC62FBBFC559",
            //    "Human_Holster_SlotDef");
            //FlashBang.HolsterSlotDef = Helper.CreateDefFromClone(
            //    source3.HolsterSlotDef,
            //    "3BB04A0C-D2B3-4648-B957-A5BD9FD734CD",
            //    skillName3);
            //FlashBang.VisualEffects = Helper.CreateDefFromClone(
            //    source3.VisualEffects,
            //    "CC56DD40-33A3-40D1-8435-C297131B03E8",
            //    "E_VisualEffects [SY_FlashGrenade_WeaponDef]");
            //FlashBang.OnDisabledEvent = Helper.CreateDefFromClone(
            //    source3.OnDisabledEvent,
            //    "A0340F3E-C615-48F1-8553-7144BCE056CE",
            //    "BrokenEquipment_EventDef");
            //FlashBang.RequiredSlotBinds[0].RequiredSlot = Helper.CreateDefFromClone(
            //    source3.RequiredSlotBinds[0].RequiredSlot,
            //    "D0DAAEC3-F229-4AF9-BA41-545D8F3008E2",
            //    skillName3);
            //FlashBang.SkinData = Helper.CreateDefFromClone(
            //    source3.SkinData,
            //    "F7753625-818A-4B67-9B58-56F078698E1C",
            //    skillName3);


           

            //FlashBangGrenadeConfig Config = (FlashBangGrenadeConfig)FlashBangGrenadeMain.Main.Config;
            //if (Config.FixText == true)
            {
                string text8 = "Grenade Flash";// "Flash Bang";
                string text9 = "Aveugle les ennemis";// "Blinds Enemies";
                FlashBang.ViewElementDef.DisplayName1 = new LocalizedTextBind(text8, true);
                FlashBang.ViewElementDef.Description = new LocalizedTextBind(text9, true);
                //FlashBangGrenadeMain.ModifiedLocalizationTerms.Add(text8);
                //FlashBangGrenadeMain.ModifiedLocalizationTerms.Add(text9);
            }         
            FlashBang.DamagePayload.DamageKeywords[0].DamageKeywordDef = Repo.GetAllDefs<StunDamageKeywordDataDef>().FirstOrDefault(p => p.name.Equals("Flash_DamageKeywordDataDef"));
            FlashBang.DamagePayload.DamageKeywords[0].Value = 500;

           // StunDamageKeywordDataDef FlashKey = Repo.GetAllDefs<StunDamageKeywordDataDef>().FirstOrDefault(p => p.name.Equals("Flash_DamageKeywordDataDef"));
           //
           // FlashKey.DamageTypeDef
        }

        public static void Clone_SonicDamageKeyWord()
        {
            string skillName3 = "Flash_DamageKeywordDataDef";
            StunDamageEffectDef source4 = Repo.GetAllDefs<StunDamageEffectDef>().FirstOrDefault(p => p.name.Equals("E_FormulaEffect [Sonic_AttenuatingDamageTypeEffectDef]"));
            StatMultiplierStatusDef blindStatus = Repo.GetAllDefs<StatMultiplierStatusDef>().FirstOrDefault(p => p.name.Equals("FlashBlinded_StatusDef"));
            StunDamageKeywordDataDef source3 = Repo.GetAllDefs<StunDamageKeywordDataDef>().FirstOrDefault(p => p.name.Equals("Sonic_DamageKeywordDataDef"));
            StunDamageKeywordDataDef FlashKeyWord = Helper.CreateDefFromClone(
                source3,
                "5A2C45CA-4DF3-44DE-A0D0-AB538AE15D14",
                skillName3);           
            FlashKeyWord.Visuals = Helper.CreateDefFromClone(
                source3.Visuals,
                "B1E48EA0-F4FB-470B-A60B-CAD957410CF3",
                "E_Visuals [Sonic2_AttenuatingDamageTypeEffectDef]");
            FlashKeyWord.StatusDef = Helper.CreateDefFromClone(
                source3.StatusDef,
                "3202BE34-2942-491B-B11A-075E39E3ECDC",
                blindStatus.name);
            FlashKeyWord.DamageTypeDef = Helper.CreateDefFromClone(
                source3.DamageTypeDef,
                "274E0326-06E1-4CBB-AE39-D43D63BE6F8A",
                source3.DamageTypeDef.name);
            //FlashKeyWord.DamageTypeDef.FormulaEffect = Helper.CreateDefFromClone(
            //    source3.DamageTypeDef.FormulaEffect,
            //    "943AFA05-4070-4D6C-B314-ADA45CF54406",
            //    skillName3);
            FlashKeyWord.DamageTypeDef.Visuals = Helper.CreateDefFromClone(
               source3.DamageTypeDef.Visuals,
               "9BFA2B30-BC5F-479F-9600-6AC700B6E4E7",
               source3.DamageTypeDef.Visuals.name);
            //
            //StunDamageEffectDef fview = (StunDamageEffectDef)FlashKeyWord.DamageTypeDef.FormulaEffect;
            //fview.StunStatusDef = Helper.CreateDefFromClone(
            //    source4.StunStatusDef,
            //    "2AE663A7-4DD1-49D1-AEB5-74FC6781E53D",
            //    skillName3);           
            //fview.DamageTypeDef = Helper.CreateDefFromClone(
             //   source4.DamageTypeDef,
             //   "6B6A1E68-C57C-4678-AB6C-6C8D98B3CFCC",
             //   skillName3);

            //FlashBangGrenadeConfig Config = (FlashBangGrenadeConfig)FlashBangGrenadeMain.Main.Config;
            //if(Config.FixText == true)
            {
                string text10 = "Flash";
                string text11 = "Réduit la perception à 0, et la précision de -50%";// "Reduces Perception To 0 And -50% Accuracy";
                string text12 = "Flashé";
                FlashKeyWord.Visuals.DisplayName1 = new LocalizedTextBind(text10, true);
                FlashKeyWord.Visuals.Description = new LocalizedTextBind(text11, true);
                FlashKeyWord.DamageTypeDef.Visuals.DisplayName1 = new LocalizedTextBind(text12, true);
                //FlashBangGrenadeMain.ModifiedLocalizationTerms.Add(text10);
                //FlashBangGrenadeMain.ModifiedLocalizationTerms.Add(text11);
            }
                      
            FlashKeyWord.StatusDef = blindStatus;
            

        }

        public static void Clone_BlindStatus()
        {
            string skillName3 = "FlashBlinded_StatusDef";
            StatMultiplierStatusDef source3 = Repo.GetAllDefs<StatMultiplierStatusDef>().FirstOrDefault(p => p.name.Equals("Blinded_StatusDef"));
            StatMultiplierStatusDef FlashStatus = Helper.CreateDefFromClone(
                source3,
                "EA58E7CA-2119-4FC2-9DF8-703A50BAAC5D",
                skillName3);
            FlashStatus.Visuals = Helper.CreateDefFromClone(
                source3.Visuals,
                "71826D2A-0888-4C7F-9EA6-95D6A90479F1",
                "E_Visuals [FlashBlinded_StatusDef]");

            FlashStatus.StatsMultipliers = new StatMultiplierStatusDef.StatMultiplier[]
            {
                new StatMultiplierStatusDef.StatMultiplier()
                {
                    StatName = "Perception",
                    Multiplier = 0.01f,
                },
                new StatMultiplierStatusDef.StatMultiplier()
                {
                    StatName = "Accuracy",
                    Multiplier = 0.5f,
                },
            };
        }
        public static void AddToStartingManufactureItems()
        {
            GeoPhoenixFactionDef Phoenix = Repo.GetAllDefs<GeoPhoenixFactionDef>().FirstOrDefault(p => p.name.Equals("Phoenix_GeoPhoenixFactionDef"));
            WeaponDef FlashBang = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("SY_FlashGrenade_WeaponDef"));
            Phoenix.StartingManufacturableItems = new ItemDef[]
            {
                Phoenix.StartingManufacturableItems[0],
                Phoenix.StartingManufacturableItems[1],
                Phoenix.StartingManufacturableItems[2],
                Phoenix.StartingManufacturableItems[3],
                Phoenix.StartingManufacturableItems[4],
                Phoenix.StartingManufacturableItems[5],
                Phoenix.StartingManufacturableItems[6],
                Phoenix.StartingManufacturableItems[7],
                Phoenix.StartingManufacturableItems[8],
                Phoenix.StartingManufacturableItems[9],
                Phoenix.StartingManufacturableItems[10],
                Phoenix.StartingManufacturableItems[11],
                Phoenix.StartingManufacturableItems[12],
                Phoenix.StartingManufacturableItems[13],
                Phoenix.StartingManufacturableItems[14],
                Phoenix.StartingManufacturableItems[15],
                Phoenix.StartingManufacturableItems[16],
                Phoenix.StartingManufacturableItems[17],
                Phoenix.StartingManufacturableItems[18],
                Phoenix.StartingManufacturableItems[19],
                Phoenix.StartingManufacturableItems[20],
                Phoenix.StartingManufacturableItems[21],
                Phoenix.StartingManufacturableItems[22],
                Phoenix.StartingManufacturableItems[23],
                Phoenix.StartingManufacturableItems[24],
                Phoenix.StartingManufacturableItems[25],
                Phoenix.StartingManufacturableItems[26],
                Phoenix.StartingManufacturableItems[27],
                Phoenix.StartingManufacturableItems[28],
                Phoenix.StartingManufacturableItems[29],
                Phoenix.StartingManufacturableItems[30],
                Phoenix.StartingManufacturableItems[31],
                Phoenix.StartingManufacturableItems[32],
                Phoenix.StartingManufacturableItems[33],
                Phoenix.StartingManufacturableItems[34],
                Phoenix.StartingManufacturableItems[35],
                Phoenix.StartingManufacturableItems[36],
                Phoenix.StartingManufacturableItems[37],
                FlashBang,
            };
        }
        public static void AddToEquipmentAnimation()
        {
            WeaponDef FlashBang = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("SY_FlashGrenade_WeaponDef"));
            TacActorShootAnimActionDef pxGrenade = Repo.GetAllDefs<TacActorShootAnimActionDef>().FirstOrDefault(p => p.name.Equals("E_GrenadeThrowing [Soldier_Utka_AnimActionsDef]"));
            pxGrenade.EquipmentList.Equipments = new EquipmentDef[]
            {
                pxGrenade.EquipmentList.Equipments[0],
                pxGrenade.EquipmentList.Equipments[1],
                pxGrenade.EquipmentList.Equipments[2],
                pxGrenade.EquipmentList.Equipments[3],
                pxGrenade.EquipmentList.Equipments[4],
                pxGrenade.EquipmentList.Equipments[5],
                pxGrenade.EquipmentList.Equipments[6],
                pxGrenade.EquipmentList.Equipments[7],
                pxGrenade.EquipmentList.Equipments[8],
                pxGrenade.EquipmentList.Equipments[9],
                pxGrenade.EquipmentList.Equipments[10],
                FlashBang,
            };
            pxGrenade.Equipments = new EquipmentDef[]
            {
                pxGrenade.Equipments[0],
                pxGrenade.Equipments[1],
                pxGrenade.Equipments[2],
                pxGrenade.Equipments[3],
                pxGrenade.Equipments[4],
                pxGrenade.Equipments[5],
                pxGrenade.Equipments[6],
                pxGrenade.Equipments[7],
                pxGrenade.Equipments[8],
                FlashBang,
            };

            foreach (TacActorShootAnimActionDef animActionDef in Repo.GetAllDefs<TacActorShootAnimActionDef>().Where(aad => aad.name.Contains("E_GrenadeThrowing [Soldier_Utka_AnimActionsDef]")))
            {
                if (animActionDef.EquipmentList.Equipments != null && !animActionDef.EquipmentList.Equipments.Contains(FlashBang))
                {
                    animActionDef.EquipmentList.Equipments = animActionDef.EquipmentList.Equipments.Append(FlashBang).ToArray();
                }
                if (animActionDef.Equipments != null && !animActionDef.Equipments.Contains(FlashBang))
                {
                    animActionDef.Equipments = animActionDef.Equipments.Append(FlashBang).ToArray();
                }
            }
            foreach (TacActorShootAnimActionDef animActionDef in Repo.GetAllDefs<TacActorShootAnimActionDef>().Where(aad => aad.name.Contains("E_MutoidThrow [Soldier_Utka_AnimActionsDef]")))
            {
                if (animActionDef.Equipments != null && !animActionDef.Equipments.Contains(FlashBang))
                {
                    animActionDef.Equipments = animActionDef.Equipments.Append(FlashBang).ToArray();
                }
            }
        }
        [HarmonyPatch(typeof(LocalizationManager), "TryGetTranslation")]
        public static class Locali2zationManager2_TryGetTranslation_Patch
        {
            // Token: 0x060000CA RID: 202 RVA: 0x0000ABF2 File Offset: 0x00008DF2
            public static bool Prepare()
            {
                //FlashBangGrenadeConfig Config = (FlashBangGrenadeConfig)FlashBangGrenadeMain.Main.Config;
                return true;// Config.FixText;
            }

            // Token: 0x060000CB RID: 203 RVA: 0x0000B0B4 File Offset: 0x000092B4
            public static void Postfix(bool __result, string Term, ref string Translation)
            {
                try
                {
                    if (!__result)
                    {
                        if (!string.IsNullOrEmpty(Term))// && FlashBangGrenadeMain.ModifiedLocalizationTerms.Contains(Term))
                        {
                            Translation = Term;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
