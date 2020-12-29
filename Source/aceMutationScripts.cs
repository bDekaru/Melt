using System;
using System.Collections.Generic;
using System.IO;

namespace Melt
{
    public enum Tier
    {
        Invalid = -1,
        Tier1,
        Tier2,
        Tier3,
        Tier4,
        Tier5,
        Tier6,
        Tier7,
        Tier8,
    }

    public enum MinDamage
    {
        Invalid = -1,
        Zero = 0,
        Tier1,
        Tier2,
        Tier3,
        Tier4,
        Tier5,
        Tier6,
        Tier7,
        Tier8,
    }

    public enum MaxDamage
    {
        Invalid = -1,
        Zero = 0,
        Tier1,
        Tier2,
        Tier3,
        Tier4,
        Tier5,
        Tier6,
        Tier7,
        Tier8,
    }

    public struct ChanceEntry
    {
        public float Chance;
        public int Value;
    }

    public class TierData
    {
        public string WieldRequirements;
        public string WieldSkillType;
        public ChanceEntry[] Bonus;
        public int[] wieldDifficulty;

        public TierData(string wieldRequirements, string wieldSkillType)
        {
            WieldRequirements = wieldRequirements;
            WieldSkillType = wieldSkillType;
        }
    }

    public class WeaponProfile
    {
        public string WeaponName;
        public float BestDamageVariance; // MinDamage = MaxDamage * (1.0f - Variance)
        public float VarianceSpread; // The worst variance will be this much worse than the best.
        public ChanceEntry[] Variances;
        public int[] DamageTier;
        public TierData[] Tiers =
        {
            new TierData("RawSkill", "WeaponSkill"),
            new TierData("RawSkill", "WeaponSkill"),
            new TierData("RawSkill", "WeaponSkill"),
            new TierData("RawSkill", "WeaponSkill"),
            new TierData("RawSkill", "WeaponSkill"),
            new TierData("RawSkill", "WeaponSkill"),
            new TierData("RawSkill", "WeaponSkill"),
            new TierData("RawSkill", "WeaponSkill"),
         };

        public WeaponProfile(string weaponName, float bestDamageVariance, float varianceSpread)
        {
            WeaponName = weaponName;
            BestDamageVariance = bestDamageVariance;
            VarianceSpread = varianceSpread;
        }

        public void SetDamageTiers(int tier1, int tier2, int tier3, int tier4, int tier5, int tier6, int tier7, int tier8)
        {
            DamageTier = new int[8];
            DamageTier[0] = tier1;
            DamageTier[1] = tier2;
            DamageTier[2] = tier3;
            DamageTier[3] = tier4;
            DamageTier[4] = tier5;
            DamageTier[5] = tier6;
            DamageTier[6] = tier7;
            DamageTier[7] = tier8;
        }
    }

    public class ArmorProfile
    {
        public string ArmorName;
        public ChanceEntry[] ArmorBonus;
        public int[] ArmorTier;
        public TierData[] Tiers =
        {
            new TierData("Level", "1"),
            new TierData("Level", "1"),
            new TierData("Level", "1"),
            new TierData("Level", "1"),
            new TierData("Level", "1"),
            new TierData("Level", "1"),
            new TierData("Level", "1"),
            new TierData("Level", "1"),
         };

        public ArmorProfile(string armorName)
        {
            ArmorName = armorName;
        }

        public void SetArmorTiers(int tier1, int tier2, int tier3, int tier4, int tier5, int tier6, int tier7, int tier8)
        {
            ArmorTier = new int[8];
            ArmorTier[0] = tier1;
            ArmorTier[1] = tier2;
            ArmorTier[2] = tier3;
            ArmorTier[3] = tier4;
            ArmorTier[4] = tier5;
            ArmorTier[5] = tier6;
            ArmorTier[6] = tier7;
            ArmorTier[7] = tier8;
        }
    }

    public class AceMutationScripts
    {
        public int DebugLevel = 1;

        WeaponProfile Axe;
        WeaponProfile Dagger;
        WeaponProfile DaggerMS;
        WeaponProfile Mace;
        WeaponProfile MaceJitte;
        WeaponProfile Spear;
        WeaponProfile Staff;
        WeaponProfile Sword;
        WeaponProfile SwordMS;
        WeaponProfile Unarmed;
        float GenericVarianceSpread = 0.25f; // Using a made up value as I couldn't find data.

        ArmorProfile Non_Extremity;
        ArmorProfile Extremity;
        ArmorProfile Shield;

        public AceMutationScripts()
        {
            Console.WriteLine($"Creating ACEmulator Mutation Scripts...");

            Non_Extremity = new ArmorProfile("non_extremity");
            Extremity = new ArmorProfile("extremity");
            Shield = new ArmorProfile("shield");

            Non_Extremity.SetArmorTiers( 25, 50, 75, 100, 125, 150, 160, 170);
            Extremity.SetArmorTiers(     25, 50, 75, 100, 125, 150, 160, 170);
            Shield.SetArmorTiers(        20, 40, 60,  80, 100, 120, 130, 140);

            BuildArmorTier(Non_Extremity, Tier.Tier1, MinDamage.Zero,  MaxDamage.Tier1,   0, eRandomFormula.favorMid);
            BuildArmorTier(Non_Extremity, Tier.Tier2, MinDamage.Tier1, MaxDamage.Tier2,   0, eRandomFormula.favorMid);
            BuildArmorTier(Non_Extremity, Tier.Tier3, MinDamage.Tier2, MaxDamage.Tier3,   0, eRandomFormula.favorMid);
            BuildArmorTier(Non_Extremity, Tier.Tier4, MinDamage.Tier3, MaxDamage.Tier4,   0, eRandomFormula.favorMid);
            BuildArmorTier(Non_Extremity, Tier.Tier5, MinDamage.Tier4, MaxDamage.Tier5,   0, eRandomFormula.favorMid);
            BuildArmorTier(Non_Extremity, Tier.Tier6, MinDamage.Tier5, MaxDamage.Tier6,   0, eRandomFormula.favorMid);
            BuildArmorTier(Non_Extremity, Tier.Tier7, MinDamage.Tier6, MaxDamage.Tier7, 150, eRandomFormula.favorLow);
            BuildArmorTier(Non_Extremity, Tier.Tier8, MinDamage.Tier6, MaxDamage.Tier8, 180, eRandomFormula.favorLow);

            BuildArmorTier(Extremity, Tier.Tier1, MinDamage.Zero,  MaxDamage.Tier1,   0, eRandomFormula.favorMid);
            BuildArmorTier(Extremity, Tier.Tier2, MinDamage.Tier1, MaxDamage.Tier2,   0, eRandomFormula.favorMid);
            BuildArmorTier(Extremity, Tier.Tier3, MinDamage.Tier2, MaxDamage.Tier3,   0, eRandomFormula.favorMid);
            BuildArmorTier(Extremity, Tier.Tier4, MinDamage.Tier3, MaxDamage.Tier4,   0, eRandomFormula.favorMid);
            BuildArmorTier(Extremity, Tier.Tier5, MinDamage.Tier4, MaxDamage.Tier5,   0, eRandomFormula.favorMid);
            BuildArmorTier(Extremity, Tier.Tier6, MinDamage.Tier5, MaxDamage.Tier6,   0, eRandomFormula.favorMid);
            BuildArmorTier(Extremity, Tier.Tier7, MinDamage.Tier6, MaxDamage.Tier7, 150, eRandomFormula.favorLow);
            BuildArmorTier(Extremity, Tier.Tier8, MinDamage.Tier6, MaxDamage.Tier8, 180, eRandomFormula.favorLow);

            BuildArmorTier(Shield, Tier.Tier1, MinDamage.Zero,  MaxDamage.Tier1,   0, eRandomFormula.favorMid);
            BuildArmorTier(Shield, Tier.Tier2, MinDamage.Tier1, MaxDamage.Tier2,   0, eRandomFormula.favorMid);
            BuildArmorTier(Shield, Tier.Tier3, MinDamage.Tier2, MaxDamage.Tier3,   0, eRandomFormula.favorMid);
            BuildArmorTier(Shield, Tier.Tier4, MinDamage.Tier3, MaxDamage.Tier4,   0, eRandomFormula.favorMid);
            BuildArmorTier(Shield, Tier.Tier5, MinDamage.Tier4, MaxDamage.Tier5,   0, eRandomFormula.favorMid);
            BuildArmorTier(Shield, Tier.Tier6, MinDamage.Tier5, MaxDamage.Tier6,   0, eRandomFormula.favorMid);
            BuildArmorTier(Shield, Tier.Tier7, MinDamage.Tier6, MaxDamage.Tier7, 150, eRandomFormula.favorLow);
            BuildArmorTier(Shield, Tier.Tier8, MinDamage.Tier6, MaxDamage.Tier8, 180, eRandomFormula.favorLow);

            WriteFile(Non_Extremity);
            WriteFile(Extremity);
            WriteFile(Shield);

            //Axe = new WeaponProfile(             "Axe", 0.40f, GenericVarianceSpread);
            //Dagger = new WeaponProfile(       "Dagger", 0.70f, GenericVarianceSpread);
            //DaggerMS = new WeaponProfile(   "DaggerMS", 0.71f, GenericVarianceSpread);
            //Mace = new WeaponProfile(           "Mace", 0.25f, GenericVarianceSpread);
            //MaceJitte = new WeaponProfile( "MaceJitte", 0.25f, GenericVarianceSpread);
            //Spear = new WeaponProfile(         "Spear", 0.45f, GenericVarianceSpread);
            //Staff = new WeaponProfile(         "Staff", 0.25f, GenericVarianceSpread);
            //Sword = new WeaponProfile(         "Sword", 0.40f, GenericVarianceSpread);
            //SwordMS = new WeaponProfile(     "SwordMS", 0.50f, GenericVarianceSpread);
            //Unarmed = new WeaponProfile(     "Unarmed", 0.50f, GenericVarianceSpread);

            //Axe.SetDamageTiers      ( 8, 17, 21, 25, 27, 31, 35, 39);
            //Dagger.SetDamageTiers   ( 5,  7,  9, 11, 13, 17, 19, 21);
            //DaggerMS.SetDamageTiers ( 3,  0,  0,  0,  0,  0,  0,  0);
            //Mace.SetDamageTiers     ( 8, 16, 20, 24, 26, 28, 32, 36);
            //MaceJitte.SetDamageTiers( 8, 16, 20, 24, 26, 28, 32, 36);
            //Spear.SetDamageTiers    ( 8, 15, 17, 19, 23, 27, 31, 35);
            //Staff.SetDamageTiers    ( 5,  7,  9, 11, 13, 17, 19, 21);
            //Sword.SetDamageTiers    (10, 20, 25, 30, 35, 40, 45, 50);
            //SwordMS.SetDamageTiers  ( 5,  0,  0,  0,  0,  6, 16, 17);
            //Unarmed.SetDamageTiers  ( 4,  7,  9, 12, 16, 18, 22, 24);

            //BuildTier(Axe, Tier.Tier1, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid);
            //BuildTier(Axe, Tier.Tier2, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid, MinDamage.Tier1, MaxDamage.Tier2, 250, eRandomFormula.favorMid);
            //BuildTier(Axe, Tier.Tier3, MinDamage.Tier1, MaxDamage.Tier2, 250, eRandomFormula.favorHigh, MinDamage.Tier2, MaxDamage.Tier3, 300, eRandomFormula.favorLow);
            //BuildTier(Axe, Tier.Tier4, MinDamage.Tier2, MaxDamage.Tier3, 300, eRandomFormula.favorLow, MinDamage.Tier3, MaxDamage.Tier4, 325, eRandomFormula.favorLow);
            //BuildTier(Axe, Tier.Tier5, MinDamage.Tier3, MaxDamage.Tier4, 325, eRandomFormula.favorLow, MinDamage.Tier4, MaxDamage.Tier5, 350, eRandomFormula.favorLow);
            //BuildTier(Axe, Tier.Tier6, MinDamage.Tier4, MaxDamage.Tier5, 350, eRandomFormula.favorLow, MinDamage.Tier5, MaxDamage.Tier6, 370, eRandomFormula.favorLow, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorLow);
            //BuildTier(Axe, Tier.Tier7, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorMid, MinDamage.Tier7, MaxDamage.Tier8, 420, eRandomFormula.favorLow);
            //BuildTier(Axe, Tier.Tier8, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorHigh, MinDamage.Tier7, MaxDamage.Tier8, 420, eRandomFormula.favorLow);
            //BuildVariances(Axe);
            //WriteFile(Axe);

            //BuildTier(Dagger, Tier.Tier1, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid);
            //BuildTier(Dagger, Tier.Tier2, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid, MinDamage.Tier1, MaxDamage.Tier2, 250, eRandomFormula.favorMid);
            //BuildTier(Dagger, Tier.Tier3, MinDamage.Tier1, MaxDamage.Tier2, 250, eRandomFormula.favorHigh, MinDamage.Tier2, MaxDamage.Tier3, 300, eRandomFormula.favorLow);
            //BuildTier(Dagger, Tier.Tier4, MinDamage.Tier2, MaxDamage.Tier3, 300, eRandomFormula.favorLow, MinDamage.Tier3, MaxDamage.Tier4, 325, eRandomFormula.favorLow);
            //BuildTier(Dagger, Tier.Tier5, MinDamage.Tier3, MaxDamage.Tier4, 325, eRandomFormula.favorLow, MinDamage.Tier4, MaxDamage.Tier5, 350, eRandomFormula.favorLow);
            //BuildTier(Dagger, Tier.Tier6, MinDamage.Tier4, MaxDamage.Tier5, 350, eRandomFormula.favorLow, MinDamage.Tier5, MaxDamage.Tier6, 370, eRandomFormula.favorLow, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorLow);
            //BuildTier(Dagger, Tier.Tier7, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorMid, MinDamage.Tier7, MaxDamage.Tier8, 420, eRandomFormula.favorLow);
            //BuildTier(Dagger, Tier.Tier8, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorHigh, MinDamage.Tier7, MaxDamage.Tier8, 420, eRandomFormula.favorLow);
            //BuildVariances(Dagger);
            //WriteFile(Dagger);

            //BuildTier(DaggerMS, Tier.Tier1, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid);
            //BuildTier(DaggerMS, Tier.Tier2, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid);
            //BuildTier(DaggerMS, Tier.Tier3, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorHigh);
            //BuildTier(DaggerMS, Tier.Tier4, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorHigh);
            //BuildTier(DaggerMS, Tier.Tier5, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorHigh);
            //BuildTier(DaggerMS, Tier.Tier6, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorHigh);
            //BuildTier(DaggerMS, Tier.Tier7, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorHigh);
            //BuildTier(DaggerMS, Tier.Tier8, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorHigh);
            //BuildVariances(DaggerMS);
            //WriteFile(DaggerMS);

            //BuildTier(Mace, Tier.Tier1, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid);
            //BuildTier(Mace, Tier.Tier2, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid, MinDamage.Tier1, MaxDamage.Tier2, 250, eRandomFormula.favorMid);
            //BuildTier(Mace, Tier.Tier3, MinDamage.Tier1, MaxDamage.Tier2, 250, eRandomFormula.favorHigh, MinDamage.Tier2, MaxDamage.Tier3, 300, eRandomFormula.favorLow);
            //BuildTier(Mace, Tier.Tier4, MinDamage.Tier2, MaxDamage.Tier3, 300, eRandomFormula.favorLow, MinDamage.Tier3, MaxDamage.Tier4, 325, eRandomFormula.favorLow);
            //BuildTier(Mace, Tier.Tier5, MinDamage.Tier3, MaxDamage.Tier4, 325, eRandomFormula.favorLow, MinDamage.Tier4, MaxDamage.Tier5, 350, eRandomFormula.favorLow);
            //BuildTier(Mace, Tier.Tier6, MinDamage.Tier4, MaxDamage.Tier5, 350, eRandomFormula.favorLow, MinDamage.Tier5, MaxDamage.Tier6, 370, eRandomFormula.favorLow, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorLow);
            //BuildTier(Mace, Tier.Tier7, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorMid, MinDamage.Tier7, MaxDamage.Tier8, 420, eRandomFormula.favorLow);
            //BuildTier(Mace, Tier.Tier8, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorHigh, MinDamage.Tier7, MaxDamage.Tier8, 420, eRandomFormula.favorLow);
            //BuildVariances(Mace);
            //WriteFile(Mace);

            //BuildTier(MaceJitte, Tier.Tier1, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid);
            //BuildTier(MaceJitte, Tier.Tier2, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid, MinDamage.Tier1, MaxDamage.Tier2, 250, eRandomFormula.favorMid);
            //BuildTier(MaceJitte, Tier.Tier3, MinDamage.Tier1, MaxDamage.Tier2, 250, eRandomFormula.favorHigh, MinDamage.Tier2, MaxDamage.Tier3, 300, eRandomFormula.favorLow);
            //BuildTier(MaceJitte, Tier.Tier4, MinDamage.Tier2, MaxDamage.Tier3, 300, eRandomFormula.favorLow, MinDamage.Tier3, MaxDamage.Tier4, 325, eRandomFormula.favorLow);
            //BuildTier(MaceJitte, Tier.Tier5, MinDamage.Tier3, MaxDamage.Tier4, 325, eRandomFormula.favorLow, MinDamage.Tier4, MaxDamage.Tier5, 350, eRandomFormula.favorLow);
            //BuildTier(MaceJitte, Tier.Tier6, MinDamage.Tier4, MaxDamage.Tier5, 350, eRandomFormula.favorLow, MinDamage.Tier5, MaxDamage.Tier6, 370, eRandomFormula.favorLow, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorLow);
            //BuildTier(MaceJitte, Tier.Tier7, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorMid, MinDamage.Tier7, MaxDamage.Tier8, 420, eRandomFormula.favorLow);
            //BuildTier(MaceJitte, Tier.Tier8, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorHigh, MinDamage.Tier7, MaxDamage.Tier8, 420, eRandomFormula.favorLow);
            //BuildVariances(MaceJitte);
            //WriteFile(MaceJitte);

            //BuildTier(Spear, Tier.Tier1, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid);
            //BuildTier(Spear, Tier.Tier2, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid, MinDamage.Tier1, MaxDamage.Tier2, 250, eRandomFormula.favorMid);
            //BuildTier(Spear, Tier.Tier3, MinDamage.Tier1, MaxDamage.Tier2, 250, eRandomFormula.favorHigh, MinDamage.Tier2, MaxDamage.Tier3, 300, eRandomFormula.favorLow);
            //BuildTier(Spear, Tier.Tier4, MinDamage.Tier2, MaxDamage.Tier3, 300, eRandomFormula.favorLow, MinDamage.Tier3, MaxDamage.Tier4, 325, eRandomFormula.favorLow);
            //BuildTier(Spear, Tier.Tier5, MinDamage.Tier3, MaxDamage.Tier4, 325, eRandomFormula.favorLow, MinDamage.Tier4, MaxDamage.Tier5, 350, eRandomFormula.favorLow);
            //BuildTier(Spear, Tier.Tier6, MinDamage.Tier4, MaxDamage.Tier5, 350, eRandomFormula.favorLow, MinDamage.Tier5, MaxDamage.Tier6, 370, eRandomFormula.favorLow, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorLow);
            //BuildTier(Spear, Tier.Tier7, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorMid, MinDamage.Tier7, MaxDamage.Tier8, 420, eRandomFormula.favorLow);
            //BuildTier(Spear, Tier.Tier8, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorHigh, MinDamage.Tier7, MaxDamage.Tier8, 420, eRandomFormula.favorLow);
            //BuildVariances(Spear);
            //WriteFile(Spear);

            //BuildTier(Staff, Tier.Tier1, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid);
            //BuildTier(Staff, Tier.Tier2, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid, MinDamage.Tier1, MaxDamage.Tier2, 250, eRandomFormula.favorMid);
            //BuildTier(Staff, Tier.Tier3, MinDamage.Tier1, MaxDamage.Tier2, 250, eRandomFormula.favorHigh, MinDamage.Tier2, MaxDamage.Tier3, 300, eRandomFormula.favorLow);
            //BuildTier(Staff, Tier.Tier4, MinDamage.Tier2, MaxDamage.Tier3, 300, eRandomFormula.favorLow, MinDamage.Tier3, MaxDamage.Tier4, 325, eRandomFormula.favorLow);
            //BuildTier(Staff, Tier.Tier5, MinDamage.Tier3, MaxDamage.Tier4, 325, eRandomFormula.favorLow, MinDamage.Tier4, MaxDamage.Tier5, 350, eRandomFormula.favorLow);
            //BuildTier(Staff, Tier.Tier6, MinDamage.Tier4, MaxDamage.Tier5, 350, eRandomFormula.favorLow, MinDamage.Tier5, MaxDamage.Tier6, 370, eRandomFormula.favorLow, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorLow);
            //BuildTier(Staff, Tier.Tier7, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorMid, MinDamage.Tier7, MaxDamage.Tier8, 420, eRandomFormula.favorLow);
            //BuildTier(Staff, Tier.Tier8, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorHigh, MinDamage.Tier7, MaxDamage.Tier8, 420, eRandomFormula.favorLow);
            //BuildVariances(Staff);
            //WriteFile(Staff);

            //BuildTier(Sword, Tier.Tier1, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid);
            //BuildTier(Sword, Tier.Tier2, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid, MinDamage.Tier1, MaxDamage.Tier2, 250, eRandomFormula.favorMid);
            //BuildTier(Sword, Tier.Tier3, MinDamage.Tier1, MaxDamage.Tier2, 250, eRandomFormula.favorHigh, MinDamage.Tier2, MaxDamage.Tier3, 300, eRandomFormula.favorLow);
            //BuildTier(Sword, Tier.Tier4, MinDamage.Tier2, MaxDamage.Tier3, 300, eRandomFormula.favorLow, MinDamage.Tier3, MaxDamage.Tier4, 325, eRandomFormula.favorLow);
            //BuildTier(Sword, Tier.Tier5, MinDamage.Tier3, MaxDamage.Tier4, 325, eRandomFormula.favorLow, MinDamage.Tier4, MaxDamage.Tier5, 350, eRandomFormula.favorLow);
            //BuildTier(Sword, Tier.Tier6, MinDamage.Tier4, MaxDamage.Tier5, 350, eRandomFormula.favorLow, MinDamage.Tier5, MaxDamage.Tier6, 370, eRandomFormula.favorLow, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorLow);
            //BuildTier(Sword, Tier.Tier7, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorMid, MinDamage.Tier7, MaxDamage.Tier8, 420, eRandomFormula.favorLow);
            //BuildTier(Sword, Tier.Tier8, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorHigh, MinDamage.Tier7, MaxDamage.Tier8, 420, eRandomFormula.favorLow);
            //BuildVariances(Sword);
            //WriteFile(Sword);

            //BuildTier(SwordMS, Tier.Tier1, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid);
            //BuildTier(SwordMS, Tier.Tier2, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid);
            //BuildTier(SwordMS, Tier.Tier3, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorHigh);
            //BuildTier(SwordMS, Tier.Tier4, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorHigh);
            //BuildTier(SwordMS, Tier.Tier5, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorHigh);
            //BuildTier(SwordMS, Tier.Tier6, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorHigh, MinDamage.Tier5, MaxDamage.Tier6, 370, eRandomFormula.favorLow, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorLow);
            //BuildTier(SwordMS, Tier.Tier7, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorMid, MinDamage.Tier7, MaxDamage.Tier8, 420, eRandomFormula.favorLow);
            //BuildTier(SwordMS, Tier.Tier8, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorHigh, MinDamage.Tier7, MaxDamage.Tier8, 420, eRandomFormula.favorLow);
            //BuildVariances(SwordMS);
            //WriteFile(SwordMS);

            //BuildTier(Unarmed, Tier.Tier1, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid);
            //BuildTier(Unarmed, Tier.Tier2, MinDamage.Zero, MaxDamage.Tier1, 0, eRandomFormula.favorMid, MinDamage.Tier1, MaxDamage.Tier2, 250, eRandomFormula.favorMid);
            //BuildTier(Unarmed, Tier.Tier3, MinDamage.Tier1, MaxDamage.Tier2, 250, eRandomFormula.favorHigh, MinDamage.Tier2, MaxDamage.Tier3, 300, eRandomFormula.favorLow);
            //BuildTier(Unarmed, Tier.Tier4, MinDamage.Tier2, MaxDamage.Tier3, 300, eRandomFormula.favorLow, MinDamage.Tier3, MaxDamage.Tier4, 325, eRandomFormula.favorLow);
            //BuildTier(Unarmed, Tier.Tier5, MinDamage.Tier3, MaxDamage.Tier4, 325, eRandomFormula.favorLow, MinDamage.Tier4, MaxDamage.Tier5, 350, eRandomFormula.favorLow);
            //BuildTier(Unarmed, Tier.Tier6, MinDamage.Tier4, MaxDamage.Tier5, 350, eRandomFormula.favorLow, MinDamage.Tier5, MaxDamage.Tier6, 370, eRandomFormula.favorLow, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorLow);
            //BuildTier(Unarmed, Tier.Tier7, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorMid, MinDamage.Tier7, MaxDamage.Tier8, 420, eRandomFormula.favorLow);
            //BuildTier(Unarmed, Tier.Tier8, MinDamage.Tier6, MaxDamage.Tier7, 400, eRandomFormula.favorHigh, MinDamage.Tier7, MaxDamage.Tier8, 420, eRandomFormula.favorLow);
            //BuildVariances(Unarmed);
            //WriteFile(Unarmed);

            Console.WriteLine($"Done");
        }

        public void BuildVariances(WeaponProfile weapon)
        {
            int bestVariance = (int)Math.Round(weapon.BestDamageVariance * 100);
            int worstVariance = bestVariance + (int)Math.Round(weapon.BestDamageVariance * weapon.VarianceSpread * 100);
            int numberOfEntries = DetermineNumberOfEntries(bestVariance, worstVariance);

            weapon.Variances = new ChanceEntry[numberOfEntries];

            int numOfRolls = 100000;
            DistributeVarianceEntries(weapon, 0, numberOfEntries, numOfRolls, eRandomFormula.favorMid);
            VarianceDistributionRounding(weapon, numberOfEntries);

            ProcesVarianceEntries(weapon, bestVariance, worstVariance, 0, numberOfEntries, numOfRolls);

            int entryCounter = 0;
            float totalChance = 0;
            for (int i = 0; i < numberOfEntries; i++)
            {
                entryCounter++;
                totalChance += weapon.Variances[i].Chance;
                if(DebugLevel > 1)
                    Console.WriteLine($"Entry {entryCounter} Chance: {weapon.Variances[i].Chance}% - Variance = {weapon.Variances[i].Value / 100.0f} - Example max damage 325 skill {weapon.WeaponName.ToLower()}: {weapon.DamageTier[4]*(1.0f-(weapon.Variances[i].Value / 100.0f))}-{weapon.DamageTier[4]}");
            }

            if (DebugLevel > 1)
                Console.WriteLine($"Total Chance: {totalChance}%");

            //if (totalChance != 100.0f)
            //{
            //    if (DebugLevel > 0)
            //        Console.WriteLine($"Retrying due to total chance: {totalChance}%");
            //    BuildVariances(weapon);
            //    return;
            //}
            //if (totalChance < 100.0f)
            //{
            //    float difference = 100.0f - totalChance;
            //    weapon.Variances[0].Chance += difference;
            //    if (DebugLevel > 0)
            //        Console.WriteLine($"Added {difference.ToString("0.000000")}% to Entry 1 to make the total chance 100%.");
            //    totalChance = 0;
            //    for (int i = 0; i < numberOfEntries; i++)
            //    {
            //        totalChance += weapon.Variances[i].Chance;
            //    }
            //    if (DebugLevel > 1)
            //        Console.WriteLine($"New Total Chance: {totalChance}%");
            //}
        }

        public void VarianceDistributionRounding(WeaponProfile weapon, int totalNumberOfEntries)
        {
            //Let's do some rounding so it looks better!
            float freeAmount = 0;
            for (int i = 0; i < totalNumberOfEntries; i++)
            {
                if (weapon.Variances[i].Chance > 10000)
                {
                    freeAmount += weapon.Variances[i].Chance % 1000;
                    weapon.Variances[i].Chance = (float)Math.Floor(weapon.Variances[i].Chance / 1000) * 1000;
                }
                else
                {
                    freeAmount += weapon.Variances[i].Chance % 100;
                    weapon.Variances[i].Chance = (float)Math.Floor(weapon.Variances[i].Chance / 100) * 100;

                    if ((weapon.Variances[i].Chance - 100) % 1000 == 0)
                    {
                        weapon.Variances[i].Chance -= 100;
                        freeAmount += 100;
                    }
                }
            }

            while (freeAmount >= 1000)
            {
                int roll = Utils.getRandomNumber(0, totalNumberOfEntries - 1, eRandomFormula.equalDistribution, 2, 0);

                weapon.Variances[roll].Chance += 1000;
                freeAmount -= 1000;
            }

            int fails = 0;
            int fails2 = 0;
            int failThreshold = 10000;
            while (freeAmount >= 100)
            {
                int roll = Utils.getRandomNumber(0, totalNumberOfEntries - 1, eRandomFormula.equalDistribution, 2, 0);

                if (weapon.Variances[roll].Chance < 3000 || fails > failThreshold * 10)
                {
                    if ((weapon.Variances[roll].Chance + 100) % 1000 == 0 || fails2 > failThreshold)
                    {
                        weapon.Variances[roll].Chance += 100;
                        freeAmount -= 100;
                        fails = 0;
                        fails2 = 0;
                        continue;
                    }
                }

                fails++;
                fails2++;
            }

            while (freeAmount >= 10)
            {
                int roll = Utils.getRandomNumber(0, totalNumberOfEntries - 1, eRandomFormula.equalDistribution, 2, 0);

                weapon.Variances[roll].Chance += freeAmount;
                freeAmount -= freeAmount;
            }

            for (int i = 0; i < totalNumberOfEntries; i++)
            {
                if (weapon.Variances[i].Chance % 100 == 99) // Fixes 1/3 + 1/3 + 1/3 turning into 0.99999 instead of 1.0
                    weapon.Variances[i].Chance++;
            }

            if (DebugLevel > 0 && freeAmount > 0)
                Console.WriteLine($"Unused freeAmount: {freeAmount}");
        }

        public void DistributeVarianceEntries(WeaponProfile weapon, int startEntry, int numberOfEntriesToProcess, int numOfRolls, eRandomFormula distribution)
        {
            for (int i = 0; i < numOfRolls; i++)
            {
                int roll = Utils.getRandomNumber(startEntry, startEntry + numberOfEntriesToProcess - 1, distribution, 2, 0);

                weapon.Variances[roll].Chance++;
            }
        }

        public void ProcesVarianceEntries(WeaponProfile weapon, int bestVariance, int worstVariance, int startEntry, int numberOfEntriesToProcess, int numOfRolls)
        {
            for (int i = startEntry; i < startEntry + numberOfEntriesToProcess; i++)
            {
                weapon.Variances[i].Chance /= (numOfRolls / 100);
                if (i == startEntry)
                    weapon.Variances[i].Value = worstVariance;
                else if (i == numberOfEntriesToProcess - 1)
                    weapon.Variances[i].Value = bestVariance;
                else
                    weapon.Variances[i].Value = (int)Math.Round((((bestVariance - worstVariance) / ((float)numberOfEntriesToProcess - 1)) * (i - startEntry)) + worstVariance);
            }
        }

        public void BuildTier(WeaponProfile weapon, Tier tier, MinDamage minDamageTierA, MaxDamage maxDamageTierA, int wieldDifficultyA, eRandomFormula distributionA, MinDamage minDamageTierB = MinDamage.Invalid, MaxDamage maxDamageTierB = MaxDamage.Invalid, int wieldDifficultyB = -1, eRandomFormula distributionB = eRandomFormula.favorMid, MinDamage minDamageTierC = MinDamage.Invalid, MaxDamage maxDamageTierC = MaxDamage.Invalid, int wieldDifficultyC = -1, eRandomFormula distributionC = eRandomFormula.favorMid)
        {
            int totalNumberOfEntries = 0;
            int numOfRollsDivider = 1;

            int minDamageA = minDamageTierA == MinDamage.Zero ? 0 : weapon.DamageTier[(int)minDamageTierA - 1];
            int maxDamageA = maxDamageTierA == MaxDamage.Zero ? 0 : weapon.DamageTier[(int)maxDamageTierA - 1];
            int minDamageB = 0;
            int maxDamageB = 0;
            int minDamageC = 0;
            int maxDamageC = 0;
            int numberOfEntriesA = DetermineNumberOfEntries(minDamageA, maxDamageA);

            int numberOfEntriesB = 0;
            if (minDamageTierB != MinDamage.Invalid && maxDamageTierB != MaxDamage.Invalid)
            {
                minDamageB = minDamageTierB == MinDamage.Zero ? 0 : weapon.DamageTier[(int)minDamageTierB - 1];
                maxDamageB = maxDamageTierB == MaxDamage.Zero ? 0 : weapon.DamageTier[(int)maxDamageTierB - 1];
                numberOfEntriesB = DetermineNumberOfEntries(minDamageB, maxDamageB);
                numOfRollsDivider++;
            }

            int numberOfEntriesC = 0;
            if (minDamageTierC != MinDamage.Invalid && maxDamageTierC != MaxDamage.Invalid)
            {
                minDamageC = minDamageTierC == MinDamage.Zero ? 0 : weapon.DamageTier[(int)minDamageTierC - 1];
                maxDamageC = maxDamageTierC == MaxDamage.Zero ? 0 : weapon.DamageTier[(int)maxDamageTierC - 1];
                numberOfEntriesC = DetermineNumberOfEntries(minDamageC, maxDamageC);
                numOfRollsDivider++;
            }

            totalNumberOfEntries = numberOfEntriesA + numberOfEntriesB + numberOfEntriesC;

            weapon.Tiers[(int)tier].Bonus = new ChanceEntry[totalNumberOfEntries];
            weapon.Tiers[(int)tier].wieldDifficulty = new int[totalNumberOfEntries];

            int numOfRolls = 100000;

            DistributeDamageBonusEntries((int)tier, weapon, 0, numberOfEntriesA, numOfRolls / numOfRollsDivider, distributionA);
            if (numberOfEntriesB > 0)
                DistributeDamageBonusEntries((int)tier, weapon, numberOfEntriesA, numberOfEntriesB, numOfRolls / numOfRollsDivider, distributionB);
            if (numberOfEntriesC > 0)
                DistributeDamageBonusEntries((int)tier, weapon, numberOfEntriesA + numberOfEntriesB, numberOfEntriesC, numOfRolls / numOfRollsDivider, distributionC);

            DamageBonusDistributionRounding((int)tier, weapon, totalNumberOfEntries);

            ProcessDamageBonusEntries(weapon, (int)tier, minDamageA, maxDamageA, wieldDifficultyA, 0, numberOfEntriesA, numOfRolls);
            if (numberOfEntriesB > 0)
                ProcessDamageBonusEntries(weapon, (int)tier, minDamageB, maxDamageB, wieldDifficultyB, numberOfEntriesA, numberOfEntriesB, numOfRolls);
            if (numberOfEntriesC > 0)
                ProcessDamageBonusEntries(weapon, (int)tier, minDamageC, maxDamageC, wieldDifficultyC, numberOfEntriesA + numberOfEntriesB, numberOfEntriesC, numOfRolls);

            float totalChance = 0;
            int entryCounter = 0;
            for (int i = 0; i < totalNumberOfEntries; i++)
            {
                entryCounter++;
                totalChance += weapon.Tiers[(int)tier].Bonus[i].Chance;
                if (DebugLevel > 1)
                    Console.WriteLine($"Entry {entryCounter} Chance: {weapon.Tiers[(int)tier].Bonus[i].Chance}% - Damage += {weapon.Tiers[(int)tier].Bonus[i].Value} WieldDifficulty = {weapon.Tiers[(int)tier].wieldDifficulty[i]}");
            }

            if (DebugLevel > 1)
                Console.WriteLine($"Total Chance: {totalChance}%");

            //if (totalChance != 100.0f)
            //{
            //    if (DebugLevel > 0)
            //        Console.WriteLine($"Retrying due to total chance: {totalChance}%");
            //    BuildTier(weapon, tier, minDamageTierA, maxDamageTierA, wieldDifficultyA, distributionA, minDamageTierB, maxDamageTierB, wieldDifficultyB, distributionB, minDamageTierC, maxDamageTierC, wieldDifficultyC, distributionC);
            //    return;
            //}
            //if (totalChance < 100.0f)
            //{
            //    float difference = 100.0f - totalChance;
            //    weapon.Tiers[(int)tier].damageBonus[0].Chance += difference;
            //    if (DebugLevel > 0)
            //        Console.WriteLine($"Added {difference.ToString("0.000000")}% to Entry 1 to make the total chance 100%.");
            //    totalChance = 0;
            //    for (int i = 0; i < totalNumberOfEntries; i++)
            //    {
            //        totalChance += weapon.Tiers[(int)tier].damageBonus[i].Chance;
            //    }
            //    if (DebugLevel > 1)
            //        Console.WriteLine($"New Total Chance: {totalChance}%");
            //}
        }

        public void BuildArmorTier(ArmorProfile armor, Tier tier, MinDamage minArmorBonus, MaxDamage maxArmorBonus, int wieldDifficulty, eRandomFormula distribution)
        {
            int minArmor = minArmorBonus == MinDamage.Zero ? 0 : armor.ArmorTier[(int)minArmorBonus - 1];
            int maxArmor = maxArmorBonus == MaxDamage.Zero ? 0 : armor.ArmorTier[(int)maxArmorBonus - 1];

            int numberOfEntries = DetermineNumberOfEntries(minArmor, maxArmor);

            armor.Tiers[(int)tier].Bonus = new ChanceEntry[numberOfEntries];
            armor.Tiers[(int)tier].wieldDifficulty = new int[numberOfEntries];

            int numOfRolls = 100000;

            DistributeArmorBonusEntries((int)tier, armor, 0, numberOfEntries, numOfRolls, distribution);

            ArmorBonusDistributionRounding((int)tier, armor, numberOfEntries);

            ProcessArmorBonusEntries(armor, (int)tier, minArmor, maxArmor, wieldDifficulty, 0, numberOfEntries, numOfRolls);

            float totalChance = 0;
            int entryCounter = 0;
            for (int i = 0; i < numberOfEntries; i++)
            {
                entryCounter++;
                totalChance += armor.Tiers[(int)tier].Bonus[i].Chance;
                if (DebugLevel > 1)
                    Console.WriteLine($"Entry {entryCounter} Chance: {armor.Tiers[(int)tier].Bonus[i].Chance}% - ArmorBonus += {armor.Tiers[(int)tier].Bonus[i].Value}");
            }

            if (DebugLevel > 1)
                Console.WriteLine($"Total Chance: {totalChance}%");
        }

        public void DistributeArmorBonusEntries(int tier, ArmorProfile armor, int startEntry, int numberOfEntriesToProcess, int numOfRolls, eRandomFormula distribution)
        {
            for (int i = 0; i < numOfRolls; i++)
            {
                int roll = Utils.getRandomNumber(startEntry, startEntry + numberOfEntriesToProcess - 1, distribution, 2, 0);

                armor.Tiers[tier].Bonus[roll].Chance++;
            }
        }

        public void ArmorBonusDistributionRounding(int tier, ArmorProfile armor, int totalNumberOfEntries)
        {
            //Let's do some rounding so it looks better!
            float freeAmount = 0;
            for (int i = 0; i < totalNumberOfEntries; i++)
            {
                if (armor.Tiers[tier].Bonus[i].Chance > 10000)
                {
                    freeAmount += armor.Tiers[tier].Bonus[i].Chance % 1000;
                    armor.Tiers[tier].Bonus[i].Chance = (float)Math.Floor(armor.Tiers[tier].Bonus[i].Chance / 1000) * 1000;
                }
                else
                {
                    freeAmount += armor.Tiers[tier].Bonus[i].Chance % 100;
                    armor.Tiers[tier].Bonus[i].Chance = (float)Math.Floor(armor.Tiers[tier].Bonus[i].Chance / 100) * 100;

                    if ((armor.Tiers[tier].Bonus[i].Chance - 100) % 1000 == 0)
                    {
                        armor.Tiers[tier].Bonus[i].Chance -= 100;
                        freeAmount += 100;
                    }
                }
            }

            while (freeAmount >= 1000)
            {
                int roll = Utils.getRandomNumber(0, totalNumberOfEntries - 1, eRandomFormula.equalDistribution, 2, 0);

                armor.Tiers[tier].Bonus[roll].Chance += 1000;
                freeAmount -= 1000;
            }

            int fails = 0;
            int fails2 = 0;
            int failThreshold = 10000;
            while (freeAmount >= 100)
            {
                int roll = Utils.getRandomNumber(0, totalNumberOfEntries - 1, eRandomFormula.equalDistribution, 2, 0);

                if (armor.Tiers[tier].Bonus[roll].Chance < 3000 || fails > failThreshold * 10)
                {
                    if ((armor.Tiers[tier].Bonus[roll].Chance + 100) % 1000 == 0 || fails2 > failThreshold)
                    {
                        armor.Tiers[tier].Bonus[roll].Chance += 100;
                        freeAmount -= 100;
                        fails = 0;
                        fails2 = 0;
                        continue;
                    }
                }

                fails++;
                fails2++;
            }

            while (freeAmount >= 10)
            {
                int roll = Utils.getRandomNumber(0, totalNumberOfEntries - 1, eRandomFormula.equalDistribution, 2, 0);

                armor.Tiers[tier].Bonus[roll].Chance += freeAmount;
                freeAmount -= freeAmount;
            }

            for (int i = 0; i < totalNumberOfEntries; i++)
            {
                if (armor.Tiers[(int)tier].Bonus[i].Chance % 100 == 99) // Fixes 1/3 + 1/3 + 1/3 turning into 0.99999 instead of 1.0
                    armor.Tiers[(int)tier].Bonus[i].Chance++;
            }

            if (DebugLevel > 0 && freeAmount > 0)
                Console.WriteLine($"Unused freeAmount: {freeAmount}");
        }

        public void ProcessArmorBonusEntries(ArmorProfile armor, int tier, int minBonus, int maxBonus, int wieldDificulty, int startEntry, int numberOfEntriesToProcess, int numOfRolls)
        {
            for (int i = startEntry; i < startEntry + numberOfEntriesToProcess; i++)
            {
                armor.Tiers[tier].Bonus[i].Chance /= (numOfRolls / 100);
                if (i == startEntry)
                    armor.Tiers[tier].Bonus[i].Value = minBonus;
                else if (i == numberOfEntriesToProcess - 1)
                    armor.Tiers[tier].Bonus[i].Value = maxBonus;
                else
                    armor.Tiers[tier].Bonus[i].Value = (int)Math.Round((((maxBonus - minBonus) / ((float)numberOfEntriesToProcess - 1)) * (i - startEntry)) + minBonus);

                armor.Tiers[tier].wieldDifficulty[i] = wieldDificulty;
            }
        }

        public void ProcessDamageBonusEntries(WeaponProfile weapon, int tier, int minBonus, int maxBonus, int wieldDificulty, int startEntry, int numberOfEntriesToProcess, int numOfRolls)
        {
            for (int i = startEntry; i < startEntry + numberOfEntriesToProcess; i++)
            {
                weapon.Tiers[tier].Bonus[i].Chance /= (numOfRolls / 100);
                if (i == startEntry)
                    weapon.Tiers[tier].Bonus[i].Value = minBonus;
                else if (i == numberOfEntriesToProcess - 1)
                    weapon.Tiers[tier].Bonus[i].Value = maxBonus;
                else
                    weapon.Tiers[tier].Bonus[i].Value = (int)Math.Round((((maxBonus - minBonus) / ((float)numberOfEntriesToProcess - 1)) * (i - startEntry)) + minBonus);

                weapon.Tiers[tier].wieldDifficulty[i] = wieldDificulty;
            }
        }

        public void DamageBonusDistributionRounding(int tier, WeaponProfile weapon, int totalNumberOfEntries)
        {
            //Let's do some rounding so it looks better!
            float freeAmount = 0;
            for (int i = 0; i < totalNumberOfEntries; i++)
            {
                if (weapon.Tiers[tier].Bonus[i].Chance > 10000)
                {
                    freeAmount += weapon.Tiers[tier].Bonus[i].Chance % 1000;
                    weapon.Tiers[tier].Bonus[i].Chance = (float)Math.Floor(weapon.Tiers[tier].Bonus[i].Chance / 1000) * 1000;
                }
                else
                {
                    freeAmount += weapon.Tiers[tier].Bonus[i].Chance % 100;
                    weapon.Tiers[tier].Bonus[i].Chance = (float)Math.Floor(weapon.Tiers[tier].Bonus[i].Chance / 100) * 100;

                    if ((weapon.Tiers[tier].Bonus[i].Chance - 100) % 1000 == 0)
                    {
                        weapon.Tiers[tier].Bonus[i].Chance -= 100;
                        freeAmount += 100;
                    }
                }
            }

            while (freeAmount >= 1000)
            {
                int roll = Utils.getRandomNumber(0, totalNumberOfEntries - 1, eRandomFormula.equalDistribution, 2, 0);

                weapon.Tiers[tier].Bonus[roll].Chance += 1000;
                freeAmount -= 1000;
            }

            int fails = 0;
            int fails2 = 0;
            int failThreshold = 10000;
            while (freeAmount >= 100)
            {
                int roll = Utils.getRandomNumber(0, totalNumberOfEntries - 1, eRandomFormula.equalDistribution, 2, 0);

                if (weapon.Tiers[tier].Bonus[roll].Chance < 3000 || fails > failThreshold * 10)
                {
                    if ((weapon.Tiers[tier].Bonus[roll].Chance + 100) % 1000 == 0 || fails2 > failThreshold)
                    {
                        weapon.Tiers[tier].Bonus[roll].Chance += 100;
                        freeAmount -= 100;
                        fails = 0;
                        fails2 = 0;
                        continue;
                    }
                }

                fails++;
                fails2++;
            }

            while (freeAmount >= 10)
            {
                int roll = Utils.getRandomNumber(0, totalNumberOfEntries - 1, eRandomFormula.equalDistribution, 2, 0);

                weapon.Tiers[tier].Bonus[roll].Chance += freeAmount;
                freeAmount -= freeAmount;
            }

            for (int i = 0; i < totalNumberOfEntries; i++)
            {
                if (weapon.Tiers[(int)tier].Bonus[i].Chance % 100 == 99) // Fixes 1/3 + 1/3 + 1/3 turning into 0.99999 instead of 1.0
                    weapon.Tiers[(int)tier].Bonus[i].Chance++;
            }

            if (DebugLevel > 0 && freeAmount > 0)
                Console.WriteLine($"Unused freeAmount: {freeAmount}");
        }

        public void DistributeDamageBonusEntries(int tier, WeaponProfile weapon, int startEntry, int numberOfEntriesToProcess, int numOfRolls, eRandomFormula distribution)
        {
            for (int i = 0; i < numOfRolls; i++)
            {
                int roll = Utils.getRandomNumber(startEntry, startEntry + numberOfEntriesToProcess - 1, distribution, 2, 0);

                weapon.Tiers[tier].Bonus[roll].Chance++;
            }
        }

        public int DetermineNumberOfEntries(int minValue, int maxValue)
        {
            int numberOfEntries = maxValue - minValue + 1;

            if (numberOfEntries > 10)
                numberOfEntries /= 2;

            //if (numberOfEntries % 2 != 0) //is odd
            //    numberOfEntries += 1;

            return numberOfEntries;
        }

        public void WriteFile(WeaponProfile weapon)
        {
            string convertedWeaponName = weapon.WeaponName.ToLower();
            if (convertedWeaponName == "daggerms")
                convertedWeaponName = "dagger_ms";
            else if (convertedWeaponName == "macejitte")
                convertedWeaponName = "mace_jitte";
            else if (convertedWeaponName == "swordms")
                convertedWeaponName = "sword_ms";

            string filename = $".\\ACEmulator Mutations\\MeleeWeapons\\Damage_WieldDifficulty_DamageVariance\\Infiltration\\{convertedWeaponName}.txt";

            StreamWriter outputFile = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine($"Unable to open {filename}.");
                return;
            }

            Console.WriteLine($"Writing {filename}.");

            int mutationCounter = 0;

            // Damage and wield requirements
            for (int i = 0; i < weapon.Tiers.Length; i++)
            {
                mutationCounter++;
                outputFile.WriteLine($"{weapon.WeaponName} Mutation #{mutationCounter}:");
                outputFile.WriteLine();
                outputFile.WriteLine($"Tier Chances: {(i == 0 ? 1 : 0)}, {(i == 1 ? 1 : 0)}, {(i == 2 ? 1 : 0)}, {(i == 3 ? 1 : 0)}, {(i == 4 ? 1 : 0)}, {(i == 5 ? 1 : 0)}, {(i == 6 ? 1 : 0)}, {(i == 7 ? 1 : 0)}");
                outputFile.WriteLine();

                for (int j = 0; j < weapon.Tiers[i].Bonus.Length; j++)
                {
                    outputFile.WriteLine($"    - Chance: {weapon.Tiers[i].Bonus[j].Chance}%:");
                    outputFile.WriteLine($"        Damage += {weapon.Tiers[i].Bonus[j].Value}");
                    if (weapon.Tiers[i].wieldDifficulty[j] > 0)
                    {
                        outputFile.WriteLine($"        WieldRequirements = {weapon.Tiers[i].WieldRequirements}");
                        outputFile.WriteLine($"        WieldSkillType = { weapon.Tiers[i].WieldSkillType}");
                        outputFile.WriteLine($"        WieldDifficulty = { weapon.Tiers[i].wieldDifficulty[j]}");
                    }
                    //if (j < weapon.Tiers[i].damageBonus.Length - 1 || i < weapon.Tiers.Length - 1)
                        outputFile.WriteLine();
                }

                outputFile.Flush();
            }

            // Variance
            mutationCounter++;
            outputFile.WriteLine($"{weapon.WeaponName} Mutation #{mutationCounter}:");
            outputFile.WriteLine();
            outputFile.WriteLine($"Tier chances: 1, 1, 1, 1, 1, 1, 1, 1");
            outputFile.WriteLine();

            for (int i = 0; i < weapon.Variances.Length; i++)
            {
                outputFile.WriteLine($"    - Chance: {weapon.Variances[i].Chance}%:");
                outputFile.WriteLine($"        DamageVariance = {weapon.Variances[i].Value / 100.0f}");

                if (i < weapon.Variances.Length - 1)
                    outputFile.WriteLine();
            }

            outputFile.Close();
        }

        public void WriteFile(ArmorProfile armor)
        {
            string armorString = armor.ArmorName.ToLower();
            string convertedArmorName = armorString;
            if (armorString == "non_extremity")
            {
                armorString = "Armor";
                convertedArmorName = "armor_level_extremity";
            }
            else if (armorString == "extremity")
            {
                armorString = "Armor";
                convertedArmorName = "armor_level_non_extremity";
            }
            else if (armorString == "shield")
            {
                armorString = "Shield";
                convertedArmorName = "shield_level";
            }

            string filename = $".\\ACEmulator Mutations\\ArmorLevel\\Infiltration\\{convertedArmorName}.txt";

            StreamWriter outputFile = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine($"Unable to open {filename}.");
                return;
            }

            Console.WriteLine($"Writing {filename}.");

            int mutationCounter = 0;

            for (int i = 0; i < armor.Tiers.Length; i++)
            {
                mutationCounter++;
                outputFile.WriteLine($"{armorString} Mutation #{mutationCounter}:");
                outputFile.WriteLine();
                outputFile.WriteLine($"Tier Chances: {(i == 0 ? 1 : 0)}, {(i == 1 ? 1 : 0)}, {(i == 2 ? 1 : 0)}, {(i == 3 ? 1 : 0)}, {(i == 4 ? 1 : 0)}, {(i == 5 ? 1 : 0)}, {(i == 6 ? 1 : 0)}, {(i == 7 ? 1 : 0)}");
                outputFile.WriteLine();

                for (int j = 0; j < armor.Tiers[i].Bonus.Length; j++)
                {
                    outputFile.WriteLine($"    - Chance: {armor.Tiers[i].Bonus[j].Chance}%:");
                    outputFile.WriteLine($"        ArmorLevel += {armor.Tiers[i].Bonus[j].Value}");
                    if (armor.Tiers[i].wieldDifficulty[j] > 0)
                    {
                        outputFile.WriteLine($"        WieldRequirements = {armor.Tiers[i].WieldRequirements}");
                        outputFile.WriteLine($"        WieldSkillType = { armor.Tiers[i].WieldSkillType}");
                        outputFile.WriteLine($"        WieldDifficulty = { armor.Tiers[i].wieldDifficulty[j]}");
                    }
                    outputFile.WriteLine();
                }

                outputFile.Flush();
            }

            // Extra armor roll
            mutationCounter++;
            outputFile.WriteLine($"{armorString} Mutation #{mutationCounter}:");
            outputFile.WriteLine();
            outputFile.WriteLine($"Tier chances: 1, 1, 1, 1, 1, 1, 1, 1");
            outputFile.WriteLine();

            outputFile.WriteLine($"    - Chance: 100%:");
            outputFile.WriteLine($"        ArmorLevel += Random(-5, 5)");

            outputFile.Close();
        }
    }
}