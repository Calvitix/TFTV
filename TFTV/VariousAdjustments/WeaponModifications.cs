using Base.Core;
using Base.Defs;
using Base.UI;
using HarmonyLib;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Common.View.ViewControllers.Inventory;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Weapons;
using PhoenixPoint.Geoscape.Entities.Research; //Calvitix needed
using PhoenixPoint.Common.Entities; //Calvitix needed

using System;
using System.Collections.Generic;
using System.Reflection;
using TFTV;
using UnityEngine;

namespace PRMBetterClasses.VariousAdjustments
{
    internal class WeaponModifications
    {
        private static readonly DefRepository Repo = TFTVMain.Repo;
        private static readonly DefCache DefCache = TFTVMain.Main.DefCache;

        public static void ApplyChanges()
        {
            // Change Archangel RL to activate blast radius
            Change_Archangel();
            Change_Hera();
            Change_KaosWeapons();
            Change_Ragnarok();
            Change_Iconoclast();
            Change_NergalsWrath();
          //  Change_Crossbows();
            Change_PriestWeapons();
            if (TFTVMain.Main.Config.ApplyCalvitixChanges)

            {
            
	            Change_Phoenix_Autocanon();
	     	}
        }

        /// <summary>
        /// Harmony Patch to show the damage to body parts if the multiplier is not set to 1.0 (= same damage)
        /// 
        /// The original method is fully copied in here and will not be called unless an error occurs, i.e. this patch overwrites the original.
        /// </summary>
        [HarmonyPatch(typeof(UIItemTooltip), "SetWeaponStats")]
        internal static class UIItemTooltip_SetWeaponStats_Patch
        {
            // Create new static localized text binds for the inserted UI tooltip texts
            public static LocalizedTextBind bodyPartDamageText = new LocalizedTextBind("BC_KEY_ITEM_STAT_BODYPART_DAMAGE");

            public static bool Prefix(UIItemTooltip __instance, ItemDef item, bool secondObject, int subItemIndex = -1)
            {
                try
                {
                    WeaponDef weaponDef = item as WeaponDef;
                    if (weaponDef == null)
                    {
                        return false;
                    }

                    // Insert:
                    // Get access to the protected SetStat(..) method with defined parameter types
                    Type[] typeParameters = new Type[] { typeof(LocalizedTextBind), typeof(bool), typeof(object), typeof(object), typeof(Sprite), typeof(int) };
                    MethodInfo SetStat = AccessTools.Method(typeof(UIItemTooltip), "SetStat", typeParameters);
                    if (SetStat == null) // Reflection failed, return true to call vanilla method
                    {
                        return true;
                    }
                    // Variable for the parameter array
                    object[] parameters;
                    // Access the games shared damage keyword data
                    SharedDamageKeywordsDataDef sharedDamageKeywords = GameUtl.GameComponent<SharedData>().SharedDamageKeywords;
                    // End Insert

                    if (weaponDef.DamagePayload != null)
                    {
                        foreach (DamageKeywordPair damageKeywordPair in weaponDef.DamagePayload.DamageKeywords)
                        {
                            if (damageKeywordPair.DamageKeywordDef.ShowInItemTooltip)
                            {
                                parameters = new object[] { damageKeywordPair.DamageKeywordDef.Visuals.DisplayName1, secondObject, damageKeywordPair.Value, damageKeywordPair.Value, null, subItemIndex };
                                SetStat.Invoke(__instance, parameters);

                                // Insert:
                                // Show body part damage below normal damage if multiplier is not 1.0
                                if ((damageKeywordPair.DamageKeywordDef == sharedDamageKeywords.DamageKeyword || damageKeywordPair.DamageKeywordDef == sharedDamageKeywords.BlastKeyword) && weaponDef.DamagePayload.BodyPartMultiplier != 1.0)
                                {
                                    float bodypartDamage = damageKeywordPair.Value * weaponDef.DamagePayload.BodyPartMultiplier;
                                    string text = $"x{weaponDef.DamagePayload.BodyPartMultiplier}";
                                    parameters = new object[] { bodyPartDamageText, secondObject, text, text, null, subItemIndex };
                                    SetStat.Invoke(__instance, parameters);
                                }
                                // End Insert
                            }
                        }

                    }
                    bool flag = weaponDef.Tags.Contains(__instance.MeleeWeaponTag);
                    bool flag2 = !float.IsInfinity(weaponDef.AreaRadius);
                    ViewElementDef visuals = weaponDef.DamagePayload.GetTopPriorityDamageType().Visuals;
                    if (visuals != null)
                    {
                        Sprite smallIcon = visuals.SmallIcon;
                    }
                    if (weaponDef.DamagePayload.AutoFireShotCount * weaponDef.DamagePayload.ProjectilesPerShot > 1)
                    {
                        parameters = new object[] { __instance.RoundBurstStatName, secondObject, weaponDef.DamagePayload.AutoFireShotCount * weaponDef.DamagePayload.ProjectilesPerShot, null, null, subItemIndex };
                        SetStat.Invoke(__instance, parameters);
                    }
                    else if (flag)
                    {
                        parameters = new object[] { __instance.MeleeBurstStatName, secondObject, string.Empty, 1, null, subItemIndex };
                        SetStat.Invoke(__instance, parameters);
                    }
                    else
                    {
                        parameters = new object[] { __instance.SingleBurstStatName, secondObject, string.Empty, 1, null, subItemIndex };
                        SetStat.Invoke(__instance, parameters);
                    }
                    int num = 1;
                    if (!flag)
                    {
                        num = weaponDef.EffectiveRange;
                    }
                    parameters = new object[] { __instance.RangeStatName, secondObject, num, null, null, subItemIndex };
                    SetStat.Invoke(__instance, parameters);
                    if (flag2)
                    {
                        parameters = new object[] { __instance.BlastRadiusStatName, secondObject, weaponDef.AreaRadius, null, null, subItemIndex };
                        SetStat.Invoke(__instance, parameters);
                    }
                    parameters = new object[] { __instance.ActionCostStatName, secondObject, string.Format("{0}", (int)(weaponDef.FractActionPointCost * 4f)), -weaponDef.FractActionPointCost, null, subItemIndex };
                    SetStat.Invoke(__instance, parameters);
                    return false;
                }
                catch (Exception e)
                {
                    PRMLogger.Error(e);
                    return true;
                }
            }
        }

        private static void Change_Archangel()
        {
            SharedData shared = GameUtl.GameComponent<SharedData>();
            WeaponDef archangel = (WeaponDef)Repo.GetDef("2bdf3179-1fe7-6394-685e-4d77460c3a75"); // NJ_HeavyRocketLauncher_WeaponDef
            archangel.DamagePayload.DamageKeywords = new List<DamageKeywordPair>()
            {
                new DamageKeywordPair()
                {
                    DamageKeywordDef = shared.SharedDamageKeywords.DamageKeyword,
                    Value = 80
                },
                new DamageKeywordPair()
                {
                    DamageKeywordDef = shared.SharedDamageKeywords.PiercingKeyword,
                    Value = 50
                },
                new DamageKeywordPair()
                {
                    DamageKeywordDef = shared.SharedDamageKeywords.ShreddingKeyword,
                    Value = 10
                }
            };
            archangel.DamagePayload.BodyPartMultiplier = 3.0f;
            int expectedRange = 400;
            archangel.SpreadDegrees = 40.99f / expectedRange; // 40.99 to get the correct Expected Range, 41 can lead to rounding errors resulting in +1 ER

            // Change it to Sphere damage delivery type to activate blast radius with appropriate shoot ability --- does not work very good :-(
            //ShootAbilityDef launchRocket = (ShootAbilityDef)Repo.GetDef("0ea4735b-dde8-e904-180e-3e1faef16a56"); // LaunchRocket_ShootAbilityDef
            //ShootAbilityDef archangelShootAbility = (ShootAbilityDef)Repo.CreateDef("D27B9746-D6CC-43A7-8A7A-0B8E90BF6CC3", launchRocket);
            //archangelShootAbility.name = "ArchangelLaunchMissile_AbilityDef";
            //archangelShootAbility.UsesPerTurn = -1;
            //archangelShootAbility.SnapToBodyparts = true;
            //archangel.Abilities = new AbilityDef[]
            //{
            //    archangelShootAbility,
            //    (AbilityDef)Repo.GetDef("3d6a71c7-c27b-5374-5a51-0ba31db93d41"), // Reload_AbilityDef
            //    (AbilityDef)Repo.GetDef("075c7675-5b35-f524-7869-bc7e90601dbb")  // DropItem_AbilityDef
            //
            //};
            //archangel.DamagePayload.DamageDeliveryType = DamageDeliveryType.Sphere;
            //archangel.DamagePayload.AoeRadius = 1.5f;
        }

        private static void Change_Hera()
        {
            SharedDamageKeywordsDataDef damageKeywords = GameUtl.GameComponent<SharedData>().SharedDamageKeywords;
            WeaponDef Hera = DefCache.GetDef<WeaponDef>("SY_NeuralPistol_WeaponDef");
            Hera.ChargesMax = 5;
            Hera.DamagePayload.DamageKeywords.Find(dkp => dkp.DamageKeywordDef == damageKeywords.PiercingKeyword).Value = 20;
            ItemDef Hera_Ammo = DefCache.GetDef<ItemDef>("SY_NeuralPistol_AmmoClip_ItemDef");
            Hera_Ammo.ChargesMax = 5;
        }

        private static void Change_KaosWeapons()
        {
            SharedDamageKeywordsDataDef damageKeywords = GameUtl.GameComponent<SharedData>().SharedDamageKeywords;
            DefRepository Repo = TFTVMain.Repo;
            // GUIDS of all Kaos Weapons
            const string DevastatorGUID = "ec50afab-a51f-8f54-d909-441c7ebf8804";  // KS_Devastator_WeaponDef
            const string ObliteratorGUID = "7e7ea9c9-e939-dc14-8a23-3a749e76cd98"; // KS_Obliterator_WeaponDef
            const string RedemptorGUID = "5542b92d-054d-c774-c984-650f8eacca11";   // KS_Redemptor_WeaponDef
            const string SubjectorGUID = "29998034-c30c-dc14-4a17-7841405b5eed";   // KS_Subjector_WeaponDef
            const string TormentorGUID = "7fdd9090-5f6f-a114-1b96-058537bce8bd";   // KS_Tormentor_WeaponDef
            // Get all Kaos Weapons safely from Repo
            List<WeaponDef> kaosWeapons = new List<WeaponDef>()
            {
                (WeaponDef)Repo.GetDef(DevastatorGUID), // KS_Devastator_WeaponDef
                (WeaponDef)Repo.GetDef(ObliteratorGUID), // KS_Obliterator_WeaponDef
                (WeaponDef)Repo.GetDef(RedemptorGUID), // KS_Redemptor_WeaponDef
                (WeaponDef)Repo.GetDef(SubjectorGUID), // KS_Subjector_WeaponDef
                (WeaponDef)Repo.GetDef(TormentorGUID)  // KS_Tormentor_WeaponDef
            };
            // Loop over all weapons for several changes
            foreach (WeaponDef kaosWepaon in kaosWeapons)
            {
                // Reduce base malfunction chance for all to -30% (from -20%)
               /// kaosWepaon.WeaponMalfunction.BaseMalfunctionPercent = -30;
                // Different changes based on weapon GUID
                switch (kaosWepaon.Guid)
                {
                    case DevastatorGUID: // KS_Devastator_WeaponDef -> no changes yet
                        break;
                    case ObliteratorGUID: // KS_Obliterator_WeaponDef -> Damage 30
                        kaosWepaon.DamagePayload.DamageKeywords.Find(dkp => dkp.DamageKeywordDef == damageKeywords.DamageKeyword).Value = 30;
                        break;
                    case RedemptorGUID: // KS_Redemptor_WeaponDef -> ER 13, damage 40
                        kaosWepaon.SpreadDegrees = 40.99f / 13;
                        kaosWepaon.DamagePayload.DamageKeywords.Find(dkp => dkp.DamageKeywordDef == damageKeywords.DamageKeyword).Value = 40;
                        kaosWepaon.DamagePayload.ArmourShred = 0.0f;
                        kaosWepaon.DamagePayload.ArmourPiercing = 0.0f;
                        break;
                    case SubjectorGUID: // KS_Subjector_WeaponDef -> no changes yet
                        break;
                    case TormentorGUID: // KS_Tormentor_WeaponDef -> Damage 40
                        kaosWepaon.DamagePayload.DamageKeywords.Find(dkp => dkp.DamageKeywordDef == damageKeywords.DamageKeyword).Value = 40;
                        break;
                    default:
                        break;
                }
            }
        }

        private static void Change_Ragnarok()
        {
            WeaponDef Ragnarok = DefCache.GetDef<WeaponDef>("PX_ShredingMissileLauncherPack_WeaponDef");
            Ragnarok.DamagePayload.Range = 35.0f;
            Ragnarok.DamagePayload.AoeRadius = 5.5f;
            SharedData Shared = GameUtl.GameComponent<SharedData>();

            // Easter egg for all testes :-)
            Ragnarok.DamagePayload.DamageKeywords = new List<DamageKeywordPair>()
            {
                new DamageKeywordPair()
                {
                    DamageKeywordDef = Shared.SharedDamageKeywords.BlastKeyword,
                    Value = 40
                },
                new DamageKeywordPair()
                {
                    DamageKeywordDef = Shared.SharedDamageKeywords.ShreddingKeyword,
                    Value = 20
                }
            };
            Ragnarok.DamagePayload.ProjectilesPerShot = 4;
            Ragnarok.SpreadRadius = 5.5f;
            Ragnarok.ChargesMax = 8;

            ItemDef RagnarokAmmo = DefCache.GetDef<ItemDef>("PX_ShredingMissileLauncher_AmmoClip_ItemDef");
            RagnarokAmmo.ChargesMax = 8;
            RagnarokAmmo.ManufactureMaterials = 24;
            RagnarokAmmo.ManufactureTech = 62;
        }

        private static void Change_Iconoclast()
        {
            WeaponDef Iconoclast = DefCache.GetDef<WeaponDef>("AN_Shotgun_WeaponDef");
            SharedData Shared = GameUtl.GameComponent<SharedData>();
            Iconoclast.DamagePayload.DamageKeywords = new List<DamageKeywordPair>()
            {
                new DamageKeywordPair()
                {
                    DamageKeywordDef = Shared.SharedDamageKeywords.DamageKeyword,
                    Value = 30
                }
            };
            Iconoclast.SpreadDegrees = 40.99f / 13;
        }

        private static void Change_NergalsWrath()
        {
            WeaponDef NergalsWrath = DefCache.GetDef<WeaponDef>("AN_HandCannon_WeaponDef");
            NergalsWrath.APToUsePerc = 25;
            SharedData Shared = GameUtl.GameComponent<SharedData>();
            NergalsWrath.DamagePayload.DamageKeywords = new List<DamageKeywordPair>()
            {
                new DamageKeywordPair()
                {
                    DamageKeywordDef = Shared.SharedDamageKeywords.DamageKeyword,
                    Value = 50
                },
                new DamageKeywordPair()
                {
                    DamageKeywordDef = Shared.SharedDamageKeywords.ShreddingKeyword,
                    Value = 5
                }
            };
        }

        public static void Change_Crossbows()
        {
            WeaponDef ErosCrb = DefCache.GetDef<WeaponDef>("SY_Crossbow_WeaponDef");
            WeaponDef BonusErosCrb = DefCache.GetDef<WeaponDef>("SY_Crossbow_Bonus_WeaponDef");
            ItemDef ErosCrb_Ammo = DefCache.GetDef<ItemDef>("SY_Crossbow_AmmoClip_ItemDef");
            WeaponDef PsycheCrb = DefCache.GetDef<WeaponDef>("SY_Venombolt_WeaponDef");
            ItemDef PsycheCrb_Ammo = DefCache.GetDef<ItemDef>("SY_Venombolt_AmmoClip_ItemDef");
            ErosCrb.ChargesMax = 5;
            BonusErosCrb.ChargesMax = 5;
            ErosCrb_Ammo.ChargesMax = 5;
            PsycheCrb.ChargesMax = 4;
            PsycheCrb_Ammo.ChargesMax = 4;
        }

        public static void Change_PriestWeapons()
        {
            int redeemerViral = 4;
            int subjectorViral = 8;

            WeaponDef redeemer = DefCache.GetDef<WeaponDef>("AN_Redemptor_WeaponDef");
            WeaponDef subjector = DefCache.GetDef<WeaponDef>("AN_Subjector_WeaponDef");

            redeemer.DamagePayload.DamageKeywords[2].Value = redeemerViral;
            subjector.DamagePayload.DamageKeywords[2].Value = subjectorViral;
        }

        //Calvitix Weapon changes 
        public static void Change_Phoenix_Autocanon()
        {
            if (TFTVMain.Main.Config.ApplyCalvitixChanges)

            {

                TFTVLogger.Debug("Calvitix_Change on FS_Autocannon_WeaponDef...");
                WeaponDef PhoenixAutocanon = DefCache.GetDef<WeaponDef>("FS_Autocannon_WeaponDef");
                //ItemDef ErosCrb_Ammo = DefCache.GetDef<ItemDef>("SY_Crossbow_AmmoClip_ItemDef");
                PhoenixAutocanon.APToUsePerc = 25;
                PhoenixAutocanon.DamagePayload.AutoFireShotCount = 1;
                PhoenixAutocanon.DamagePayload.RageBurstsWhenInfiniteCharges = 12;
                PhoenixAutocanon.DamagePayload.DamageKeywords[0].Value = 90.0f;

                TFTVLogger.Info("Calvitix_Change on NE_MachineGun_WeaponDef...");

                WeaponDef MachineGun = DefCache.GetDef<WeaponDef>("NE_MachineGun_WeaponDef");
                MachineGun.APToUsePerc = 75;
                MachineGun.DamagePayload.AutoFireShotCount = 25;
                MachineGun.ChargesMax = 100;
                MachineGun.DamagePayload.DamageKeywords[0].Value = 30.0f;
                MachineGun.DamagePayload.DamageKeywords[1].Value = 1.0f;
                MachineGun.SpreadDegrees = 2.5f;


                TFTVLogger.Info("Calvitix_Change on NE_MachineGun_AmmoClip_ItemDef...");

                ItemDef MachineGun_Ammo = DefCache.GetDef<ItemDef>("NE_MachineGun_AmmoClip_ItemDef");
                MachineGun_Ammo.ChargesMax = 100;

                TFTVLogger.Info("Calvitix_Change on NJ_Gauss_MachineGun_WeaponDef...");

                WeaponDef NJ_MachineGun = DefCache.GetDef<WeaponDef>("NJ_Gauss_MachineGun_WeaponDef");
                NJ_MachineGun.APToUsePerc = 75;
                NJ_MachineGun.DamagePayload.AutoFireShotCount = 15;
                NJ_MachineGun.ChargesMax = 90;
                NJ_MachineGun.DamagePayload.DamageKeywords[0].Value = 40;
                NJ_MachineGun.DamagePayload.DamageKeywords[1].Value = 2;
                NJ_MachineGun.SpreadDegrees = 2;


                TFTVLogger.Info("Calvitix_Change on NJ_Gauss_MachineGun_AmmoClip_ItemDef...");

                ItemDef NJ_MachineGun_Ammo = DefCache.GetDef<ItemDef>("NJ_Gauss_MachineGun_AmmoClip_ItemDef");
                NJ_MachineGun_Ammo.ChargesMax = 90;

                TFTVLogger.Debug("Calvitix_Change on PX_SniperRifle_WeaponDef...");

                WeaponDef PX_SniperRifle = DefCache.GetDef<WeaponDef>("PX_SniperRifle_WeaponDef");
                PX_SniperRifle.DamagePayload.DamageKeywords[0].Value = 150;

                WeaponDef NJ_APC_MachineGun = DefCache.GetDef<WeaponDef>("NJ_Armadillo_Gauss_Turret_GroundVehicleWeaponDef");
                NJ_APC_MachineGun.APToUsePerc = 50;
                NJ_APC_MachineGun.DamagePayload.AutoFireShotCount = 15;
                NJ_APC_MachineGun.ChargesMax = 120;
                NJ_APC_MachineGun.DamagePayload.DamageKeywords[0].Value = 40;
                NJ_APC_MachineGun.DamagePayload.DamageKeywords[1].Value = 5;


                WeaponDef PX_Scarab_Missile = DefCache.GetDef<WeaponDef>("PX_Scarab_Missile_Turret_GroundVehicleWeaponDef");
                PX_Scarab_Missile.DamagePayload.AutoFireShotCount = 4;
                PX_Scarab_Missile.ChargesMax = 20;
                PX_Scarab_Missile.DamagePayload.DamageKeywords[0].Value = 60;

                WeaponDef PX_Scarab_Missile2 = DefCache.GetDef<WeaponDef>("PX_Scarab_Scorpio_GroundVehicleWeaponDef");
                PX_Scarab_Missile2.DamagePayload.AutoFireShotCount = 4;
                PX_Scarab_Missile2.ChargesMax = 20;
                PX_Scarab_Missile2.DamagePayload.DamageKeywords[0].Value = 100;

                WeaponDef PX_Scarab_Missile3 = DefCache.GetDef<WeaponDef>("PX_Scarab_Taurus_GroundVehicleWeaponDef");
                PX_Scarab_Missile3.ChargesMax = 8;

                WeaponDef PX_Armadillo_Grenade = DefCache.GetDef<WeaponDef>("NJ_Armadillo_Purgatory_GroundVehicleWeaponDef");
                PX_Armadillo_Grenade.ChargesMax = 8;

                WeaponDef PX_Armadillo_Grenade2 = DefCache.GetDef<WeaponDef>("NJ_Armadillo_Mephistopheles_GroundVehicleWeaponDef");
                PX_Armadillo_Grenade2.ChargesMax = 8;


                WeaponDef PX_Grenade_Launcher = DefCache.GetDef<WeaponDef>("PX_GrenadeLauncher_WeaponDef");
                PX_Grenade_Launcher.APToUsePerc = 50;

                WeaponDef PX_HealGrenade = DefCache.GetDef<WeaponDef>("PX_HealGrenade_WeaponDef");
                PX_HealGrenade.DamagePayload.DamageKeywords[0].Value = 120;


                //AmmoCost: divided by 2
                ////Calvitix Other Changes 
                Single multipleCostMaterial = 3;
                Single multipleCostTech = 2;

                List<String> PX_ammoList = new List<String>
                {
                    "AN_AcidHandGun_AmmoClip_ItemDef",
                    "AN_HandCannon_AmmoClip_ItemDef",
                    "AN_Redemptor_AmmoClip_ItemDef",
                    "AN_Shotgun_AmmoClip_ItemDef",
                    "AN_ShreddingShotgun_AmmoClip_ItemDef",
                    "AN_Subjector_AmmoClip_ItemDef",
                    "FS_AssaultGrenadeLauncher_AmmoClip_ItemDef",
                    "FS_Autocannon_AmmoClip_ItemDef",
                    "FS_BiogasLauncher_AmmoClip_ItemDef",
                    "FS_LightSniperRifle_AmmoClip_ItemDef",
                    "FS_SlamstrikeShotgun_AmmoClip_ItemDef",
                    "KS_AssaultRifle_AmmoClip_ItemDef",
                    "KS_Autocannon_AmmoClip_ItemDef",
                    "KS_Gauss_HandGun_AmmoClip_ItemDef",
                    "KS_ShreddingShotgun_AmmoClip_ItemDef",
                    "KS_SniperRifle_AmmoClip_ItemDef",
                    "MechArms_AmmoClip_ItemDef",
                    "NE_AssaultRifle_AmmoClip_ItemDef",
                    "NE_MachineGun_AmmoClip_ItemDef",
                    "NE_Pistol_AmmoClip_ItemDef",
                    "NE_SniperRifle_AmmoClip_ItemDef",
                    "NJ_Flamethrower_AmmoClip_ItemDef",
                    "NJ_Gauss_AssaultRifle_AmmoClip_ItemDef",
                    "NJ_Gauss_HandGun_AmmoClip_ItemDef",
                    "NJ_Gauss_MachineGun_AmmoClip_ItemDef",
                    "NJ_Gauss_PDW_AmmoClip_ItemDef",
                    "NJ_Gauss_SniperRifle_AmmoClip_ItemDef",
                    "NJ_GuidedMissileLauncher_AmmoClip_ItemDef",
                    "NJ_HeavyRocketLauncher_AmmoClip_ItemDef",
                    "NJ_PRCRTechTurretGun_AmmoClip_ItemDef",
                    "NJ_PRCR_AssaultRifle_AmmoClip_ItemDef",
                    "NJ_PRCR_PDW_AmmoClip_ItemDef",
                    "NJ_PRCR_SniperRifle_AmmoClip_ItemDef",
                    "NJ_RocketLauncher_AmmoClip_ItemDef",
                    "NJ_TechTurretGun_AmmoClip_ItemDef",
                    "PX_AcidCannon_AmmoClip_ItemDef",
                    "PX_AssaultRifle_AmmoClip_ItemDef",
                    "PX_GrenadeLauncher_AmmoClip_ItemDef",
                    "PX_HeavyCannon_AmmoClip_ItemDef",
                    "PX_LaserArray_AmmoClip_ItemDef",
                    "PX_LaserPDW_AmmoClip_ItemDef",
                    "PX_LaserTechTurretGun_AmmoClip_ItemDef",
                    "PX_PDW_AmmoClip_ItemDef",
                    "PX_Pistol_AmmoClip_ItemDef",
                    "PX_ShotgunRifle_AmmoClip_ItemDef",
                    "PX_ShredingMissileLauncher_AmmoClip_ItemDef",
                    "PX_SniperRifle_AmmoClip_ItemDef",
                    "PX_VirophageSniperRifle_AmmoClip_ItemDef",
                    "SY_Crossbow_AmmoClip_ItemDef",
                    "SY_LaserAssaultRifle_AmmoClip_ItemDef",
                    "SY_LaserPistol_AmmoClip_ItemDef",
                    "SY_LaserSniperRifle_AmmoClip_ItemDef",
                    "SY_NeuralPistol_AmmoClip_ItemDef",
                    "SY_NeuralSniperRifle_AmmoClip_ItemDef",
                    "SY_SpiderDroneLauncher_AmmoClip_ItemDef",
                    "SY_Venombolt_AmmoClip_ItemDef"


         };

                foreach (String PX_AmmoStr in PX_ammoList)
                {

                    ////Plants Cost
                    ItemDef PX_Ammo = DefCache.GetDef<ItemDef>(PX_AmmoStr);
                    //TFTVLogger.Info("Item " & PX_AmmoStr & "Manuf : " & PX_Ammo.ManufactureMaterials.ToString & ", Tech : " & PX_Ammo.ManufactureTech.ToString);

                    PX_Ammo.ManufactureMaterials = (Single)Math.Max(1,(Math.Round(PX_Ammo.ManufactureMaterials / multipleCostMaterial, 0)));
                    PX_Ammo.ManufactureTech = (Single)Math.Max(0,(Math.Round(PX_Ammo.ManufactureTech / multipleCostTech,0)));
                    TFTVLogger.Debug("Calvitix_Change all done...");


                }





                TFTVLogger.Debug("Calvitix_Change all done...");
            }

        }

    }
}
