using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using ACE.DatLoader.FileTypes;
using ACE.Entity.Enum;
using ACE.DatLoader.Entity;
using System.Threading;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Linq;

namespace Melt
{
    class SpellManipulationTools
    {
        public SpellTable SpellTable = null;
        public Dictionary<uint, Cache2Spell> Cache2SpellTable = new Dictionary<uint, Cache2Spell>();

        public SpellManipulationTools(string filename, bool isToD = true)
        {
            LoadSpellTableFromBin(filename, isToD);
        }

        public bool LoadSpellTableFromBin(string filename, bool isToD = true)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename);
                return false;
            }

            using (var reader = new BinaryReader(inputFile.BaseStream))
            {
                Console.WriteLine("Loading spells from binary...");
                SpellTable = new SpellTable();
                SpellTable.Unpack(reader, isToD);
                Console.WriteLine("Done");
                return true;
            }
        }

        public void SpellTableToTxt()
        {
            if (SpellTable == null)
            {
                Console.WriteLine("SpellTable is empty");
                return;
            }

            StreamWriter outputFile = new StreamWriter(new FileStream(".\\0E00000E.txt", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open 0E00000E.txt");
                return;
            }

            Console.WriteLine("Converting spells from binary to txt...");

            outputFile.WriteLine(SpellTable.Spells.Count);
            outputFile.WriteLine(8192);

            foreach (var spellEntry in SpellTable.Spells)
            {
                var spell = spellEntry.Value;
                outputFile.Write(spell.MetaSpellId); outputFile.Write("|");
                outputFile.Write(spell.Name); outputFile.Write("|");
                outputFile.Write(spell.Desc); outputFile.Write("|");
                outputFile.Write(spell.School); outputFile.Write("|");
                outputFile.Write(spell.Icon); outputFile.Write("|");
                outputFile.Write(spell.Category); outputFile.Write("|");
                outputFile.Write(spell.Bitfield); outputFile.Write("|");
                outputFile.Write(spell.BaseMana); outputFile.Write("|");
                outputFile.Write(spell.BaseRangeConstant); outputFile.Write("|");
                outputFile.Write(spell.BaseRangeMod); outputFile.Write("|");
                outputFile.Write(spell.Power); outputFile.Write("|");
                outputFile.Write(spell.SpellEconomyMod); outputFile.Write("|");
                outputFile.Write(spell.FormulaVersion); outputFile.Write("|");
                outputFile.Write(spell.ComponentLoss); outputFile.Write("|");
                outputFile.Write(spell.MetaSpellType); outputFile.Write("|");
                switch (spell.MetaSpellType)
                {
                    case SpellType.Enchantment:
                    case SpellType.FellowEnchantment:
                        outputFile.Write(spell.Duration); outputFile.Write("|");
                        outputFile.Write(spell.DegradeModifier); outputFile.Write("|");
                        outputFile.Write(spell.DegradeLimit); outputFile.Write("|");
                        break;
                    case SpellType.PortalSummon:
                        outputFile.Write(spell.PortalLifetime); outputFile.Write("|");
                        break;
                }

                for (int i = 0; i < 8; i++)
                {
                    if (spell.Formula.Count > i)
                    {
                        outputFile.Write(spell.Formula[i]); outputFile.Write("|");
                    }
                    else
                    {
                        outputFile.Write(0); outputFile.Write("|");
                    }
                }

                outputFile.Write(spell.CasterEffect); outputFile.Write("|");
                outputFile.Write(spell.TargetEffect); outputFile.Write("|");
                outputFile.Write(spell.FizzleEffect); outputFile.Write("|");
                outputFile.Write(spell.RecoveryInterval); outputFile.Write("|");
                outputFile.Write(spell.RecoveryAmount); outputFile.Write("|");
                outputFile.Write(spell.DisplayOrder); outputFile.Write("|");
                outputFile.Write(spell.NonComponentTargetType); outputFile.Write("|");
                outputFile.Write(spell.ManaMod);

                outputFile.Write("\n");
                outputFile.Flush();
            }

            foreach (var spellSet in SpellTable.SpellSet)
            {
                outputFile.Write("SpellSet\n");
                foreach (var spellSetTiers in spellSet.Value.SpellSetTiers)
                {
                    outputFile.Write("SpellSetTier|");
                    foreach (var spell in spellSetTiers.Value.Spells)
                    {
                        outputFile.Write(spell); outputFile.Write("|");
                    }
                    outputFile.Write("\n");
                }
                outputFile.Flush();
            }
        }

        public void SpellTableToBin(string outputFilename = "./0E00000E.bin")
        {
            if (SpellTable == null)
            {
                Console.WriteLine("SpellTable is empty");
                return;
            }

            StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open {0}", outputFilename);
                return;
            }

            Console.WriteLine("Converting spells to binary...");

            byte[] buffer = new byte[1024];
            outputFile.BaseStream.Write(BitConverter.GetBytes((uint)0x0E00000E), 0, 4);
            outputFile.BaseStream.Write(BitConverter.GetBytes(SpellTable.Spells.Count), 0, 2);
            outputFile.BaseStream.Write(BitConverter.GetBytes((short)8192), 0, 2);
            outputFile.Flush();

            foreach (var spellEntry in SpellTable.Spells)
            {
                var spell = spellEntry.Value;
                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.MetaSpellId), 0, 4);

                // Fix a problem with a character that was getting interpreted differently by our encoding and AC's
                string fixedName = spell.Name.Replace("’", ""); // The IDE may not show it but there's a character in there.

                outputFile.BaseStream.Write(BitConverter.GetBytes((short)fixedName.Length), 0, 2);
                ConvertStringToByteArray(fixedName, ref buffer, 0, fixedName.Length);
                int startIndex = (int)outputFile.BaseStream.Position;
                int endIndex = (int)outputFile.BaseStream.Position + fixedName.Length + 2;
                int alignedIndex = Utils.align4(endIndex - startIndex);
                int newIndex = startIndex + alignedIndex;
                int bytesNeededToReachAlignment = newIndex - endIndex;
                outputFile.BaseStream.Write(buffer, 0, fixedName.Length + bytesNeededToReachAlignment);

                string fixedDescription = spell.Desc.Replace("–", "").Replace("’", ""); // The IDE may not show it but there's a character in there.

                outputFile.BaseStream.Write(BitConverter.GetBytes((short)fixedDescription.Length), 0, 2);
                ConvertStringToByteArray(fixedDescription, ref buffer, 0, fixedDescription.Length);
                startIndex = (int)outputFile.BaseStream.Position;
                endIndex = (int)outputFile.BaseStream.Position + fixedDescription.Length + 2;
                alignedIndex = Utils.align4(endIndex - startIndex);
                newIndex = startIndex + alignedIndex;
                bytesNeededToReachAlignment = newIndex - endIndex;
                outputFile.BaseStream.Write(buffer, 0, fixedDescription.Length + bytesNeededToReachAlignment);

                outputFile.BaseStream.Write(BitConverter.GetBytes((int)spell.School), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.Icon), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes((int)spell.Category), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.Bitfield), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.BaseMana), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.BaseRangeConstant), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.BaseRangeMod), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.Power), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.SpellEconomyMod), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.FormulaVersion), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.ComponentLoss), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes((int)spell.MetaSpellType), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.MetaSpellId), 0, 4);

                switch (spell.MetaSpellType)
                {
                    case SpellType.Enchantment:
                    case SpellType.FellowEnchantment:
                        outputFile.BaseStream.Write(BitConverter.GetBytes(spell.Duration), 0, 8);
                        outputFile.BaseStream.Write(BitConverter.GetBytes(spell.DegradeModifier), 0, 4);
                        outputFile.BaseStream.Write(BitConverter.GetBytes(spell.DegradeLimit), 0, 4);
                        break;
                    case SpellType.PortalSummon:
                        outputFile.BaseStream.Write(BitConverter.GetBytes(spell.PortalLifetime), 0, 8);
                        break;
                }
                if (spell.MetaSpellId != 3452
                    && spell.MetaSpellId != 3455
                    && spell.MetaSpellId != 3457
                    && spell.MetaSpellId != 3458
                    && spell.MetaSpellId != 3459
                    && spell.MetaSpellId != 3810
                    && spell.MetaSpellId != 3811
                    && spell.MetaSpellId != 3818
                    && spell.MetaSpellId != 3953
                    && spell.MetaSpellId != 3966
                    && spell.MetaSpellId != 4024
                    ) // Odd uncastable spells - Let's move these values along without modifications.
                {
                    uint hash = Utils.getHash(spell.Desc, 0xBEADCF45) + Utils.getHash(spell.Name, 0x12107680);
                    for (int i = 0; i < 8; i++)
                    {
                        uint component = 0;
                        if (spell.Formula.Count > i)
                            component = spell.Formula[i] + hash;

                        outputFile.BaseStream.Write(BitConverter.GetBytes(component), 0, 4);
                    }
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                    {
                        uint component = 0;
                        if (spell.Formula.Count > i)
                            component = spell.Formula[i];

                        outputFile.BaseStream.Write(BitConverter.GetBytes(component), 0, 4);
                    }
                }

                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.CasterEffect), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.TargetEffect), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.FizzleEffect), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.RecoveryInterval), 0, 8);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.RecoveryAmount), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.DisplayOrder), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.NonComponentTargetType), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spell.ManaMod), 0, 4);

                outputFile.Flush();
            }

            PackPackedHashTable(SpellTable.SpellSet, outputFile);

            outputFile.Close();
            Console.WriteLine("Done");
        }

        public void PackPackedHashTable(Dictionary<uint, SpellSet> spellSet, StreamWriter outputFile)
        {
            outputFile.BaseStream.Write(BitConverter.GetBytes((short)spellSet.Count), 0, 2);
            outputFile.BaseStream.Write(BitConverter.GetBytes((short)256), 0, 2); //bucketSize

            foreach (var entry in spellSet)
            {
                outputFile.BaseStream.Write(BitConverter.GetBytes(entry.Key), 0, 4);
                PackPackedHashTable(entry.Value.SpellSetTiers, outputFile);
                outputFile.Flush();
            }
        }

        public void PackPackedHashTable(SortedDictionary<uint, SpellSetTiers> spellSetTiers, StreamWriter outputFile)
        {
            outputFile.BaseStream.Write(BitConverter.GetBytes((short)spellSetTiers.Count), 0, 2);
            outputFile.BaseStream.Write(BitConverter.GetBytes((short)0), 0, 2); //bucketSize

            foreach (var entry in spellSetTiers)
            {
                outputFile.BaseStream.Write(BitConverter.GetBytes(entry.Key), 0, 4);
                PackList(entry.Value.Spells, outputFile);
                outputFile.Flush();
            }
        }

        public void PackList(List<uint> list, StreamWriter outputFile)
        {
            outputFile.BaseStream.Write(BitConverter.GetBytes(list.Count), 0, 4);

            foreach (var item in list)
            {
                outputFile.BaseStream.Write(BitConverter.GetBytes(item), 0, 4);
                outputFile.Flush();
            }
        }


        public class Cache2Spell
        {
            public uint spellId;
            public uint spellId2;
            public string name;
            public string desc;

            public uint school;
            public uint iconID;
            public uint category;
            public uint bitfield;
            public int base_mana;
            public float base_range_constant;
            public float base_range_mod;
            public int power;
            public float spell_economy_mod;
            public uint formula_version;
            public float component_loss;
            public uint sp_type;

            //EnchantmentSpellEx = 1
            public uint elementalDamageType;
            public double duration;
            public float degrade_modifier;
            public float degrade_limit;
            public int spellCategory;
            public uint smod_type;
            public uint smod_key;
            public float smod_val;

            //FellowshipEnchantmentSpellEx = 12
            //+EnchantmentSpellEx variables

            //ProjectileSpellEx = 2
            public uint etype;
            public int baseIntensity;
            public int variance;
            public uint wcid;
            public int numProjectiles;
            public int numProjectilesVariance;
            public float spreadAngle;
            public float verticalAngle;
            public float defaultLaunchAngle;
            public int bNonTracking;
            public float[] createOffset;
            public float[] padding;
            public float[] dims;
            public float[] peturbation;

            public uint imbuedEffect;
            public int slayerCreatureType;
            public float slayerDamageBonus;
            public double critFreq;
            public double critMultiplier;
            public int ignoreMagicResist;
            public double elementalModifier;

            //ProjectileLifeSpellEx = 10
            //+ProjectileSpellEx variables
            public float drain_percentage;
            public float damage_ratio;

            //ProjectileEnchantmentSpellEx = 15
            //+ProjectileSpellEx variables
            //+EnchantmentSpellEx variables minus elementalDamageType

            //BoostSpellEx = 3
            public int boost; // e.g. 4-6 would be boost=4 boostVariance=2, and -4 to -6 would be boost=-4 boostVariance=-2
            public int boostVariance; // boost+boost variance = max change

            //FellowshipBoostSpellEx = 11
            //+BoostSpellEx variables

            //TransferSpellEx = 4
            public uint src;
            public uint dest;
            public float proportion;
            public float lossPercent;
            public int sourceLoss;
            public int transferCap;
            public int maxBoostAllowed;
            public uint transferBitfield; // 1 = source self, 2 = source other, 4 = destination self, 8 = destination other

            //PortalLinkSpellEx = 5
            public int index;

            //PortalRecallSpellEx = 6
            //+PortalLinkSpellEx variables

            //PortalSendingSpellEx = 8
            public sPosition pos;

            //FellowshipPortalSendingSpellEx = 13
            //+PortalSendingSpellEx variables

            //DispelSpellEx = 9
            public int min_power;
            public int max_power;
            public float power_variance;
            public int align;
            public int dispelSchool;
            public int number;
            public float number_variance;

            //FellowshipDispelSpellEx = 14
            //+DispelSpellEx variables

            //PortalSummonSpellEx = 7
            public double portal_lifetime;
            public int link;

            //all
            public uint[] component;
            public int caster_effect;
            public int target_effect;
            public int fizzle_effect;
            public double recovery_interval;
            public float recovery_amount;
            public int display_order;
            public uint non_component_target_type;
            public int mana_mod;

            public Cache2Spell()
            {
                component = new uint[8];
                createOffset = new float[3];
                padding = new float[3];
                dims = new float[3];
                peturbation = new float[3];
            }
        }

        public void LoadCache2Raw(string cache2Filename)
        {
            StreamReader cache2rawFile = new StreamReader(new FileStream(cache2Filename, FileMode.Open, FileAccess.Read));
            if (cache2rawFile == null)
            {
                Console.WriteLine("Unable to open {0}", cache2Filename);
                return;
            }

            Console.WriteLine("Loading spells from cache2 file...");

            Cache2SpellTable.Clear();

            short spellCount = Utils.readInt16(cache2rawFile);
            short unknown1 = Utils.readInt16(cache2rawFile);

            for (int entry = 0; entry < spellCount; entry++) // Starting at spellId 3679 the server files contains placeholder data for ToD spells
            {
                Cache2Spell spell = new Cache2Spell();

                spell.spellId = Utils.readUInt32(cache2rawFile);
                spell.name = Utils.readString(cache2rawFile);
                spell.desc = Utils.readString(cache2rawFile);
                spell.school = Utils.readUInt32(cache2rawFile);
                spell.iconID = Utils.readUInt32(cache2rawFile);
                spell.category = Utils.readUInt32(cache2rawFile);
                spell.bitfield = Utils.readUInt32(cache2rawFile);
                spell.base_mana = Utils.readInt32(cache2rawFile);
                spell.base_range_constant = Utils.readSingle(cache2rawFile);
                spell.base_range_mod = Utils.readSingle(cache2rawFile);
                spell.power = Utils.readInt32(cache2rawFile);
                spell.spell_economy_mod = Utils.readSingle(cache2rawFile);
                spell.formula_version = Utils.readUInt32(cache2rawFile);
                spell.component_loss = Utils.readSingle(cache2rawFile);
                spell.sp_type = Utils.readUInt32(cache2rawFile);

                switch (spell.sp_type)
                {
                    case 1: //Enchantment_SpellType
                    case 12: //FellowEnchantment_SpellType
                        spell.elementalDamageType = Utils.readUInt32(cache2rawFile);
                        spell.duration = Utils.readDouble(cache2rawFile);
                        spell.degrade_modifier = Utils.readSingle(cache2rawFile);
                        spell.degrade_limit = Utils.readSingle(cache2rawFile);
                        spell.spellCategory = Utils.readInt32(cache2rawFile);
                        spell.smod_type = Utils.readUInt32(cache2rawFile);
                        spell.smod_key = Utils.readUInt32(cache2rawFile);
                        spell.smod_val = Utils.readSingle(cache2rawFile);
                        break;
                    case 2: //Projectile_SpellType
                        spell.spellId2 = Utils.readUInt32(cache2rawFile);
                        spell.etype = Utils.readUInt32(cache2rawFile);
                        spell.baseIntensity = Utils.readInt32(cache2rawFile);
                        spell.variance = Utils.readInt32(cache2rawFile);
                        spell.wcid = Utils.readUInt32(cache2rawFile);
                        spell.numProjectiles = Utils.readInt32(cache2rawFile);
                        spell.numProjectilesVariance = Utils.readInt32(cache2rawFile);
                        spell.spreadAngle = Utils.readSingle(cache2rawFile);
                        spell.verticalAngle = Utils.readSingle(cache2rawFile);
                        spell.defaultLaunchAngle = Utils.readSingle(cache2rawFile);
                        spell.bNonTracking = Utils.readInt32(cache2rawFile);
                        spell.createOffset[0] = Utils.readSingle(cache2rawFile);
                        spell.createOffset[1] = Utils.readSingle(cache2rawFile);
                        spell.createOffset[2] = Utils.readSingle(cache2rawFile);
                        spell.padding[0] = Utils.readSingle(cache2rawFile);
                        spell.padding[1] = Utils.readSingle(cache2rawFile);
                        spell.padding[2] = Utils.readSingle(cache2rawFile);
                        spell.dims[0] = Utils.readSingle(cache2rawFile);
                        spell.dims[1] = Utils.readSingle(cache2rawFile);
                        spell.dims[2] = Utils.readSingle(cache2rawFile);
                        spell.peturbation[0] = Utils.readSingle(cache2rawFile);
                        spell.peturbation[1] = Utils.readSingle(cache2rawFile);
                        spell.peturbation[2] = Utils.readSingle(cache2rawFile);
                        spell.imbuedEffect = Utils.readUInt32(cache2rawFile);
                        spell.slayerCreatureType = Utils.readInt32(cache2rawFile);
                        spell.slayerDamageBonus = Utils.readSingle(cache2rawFile);
                        spell.critFreq = Utils.readDouble(cache2rawFile);
                        spell.critMultiplier = Utils.readDouble(cache2rawFile);
                        spell.ignoreMagicResist = Utils.readInt32(cache2rawFile);
                        spell.elementalModifier = Utils.readDouble(cache2rawFile);
                        break;
                    case 3: //Boost_SpellType
                    case 11: //FellowBoost_SpellType
                        spell.spellId2 = Utils.readUInt32(cache2rawFile);
                        spell.elementalDamageType = Utils.readUInt32(cache2rawFile);
                        spell.boost = Utils.readInt32(cache2rawFile);
                        spell.boostVariance = Utils.readInt32(cache2rawFile);
                        break;
                    case 4: //Transfer_SpellType
                        spell.spellId2 = Utils.readUInt32(cache2rawFile);
                        spell.src = Utils.readUInt32(cache2rawFile);
                        spell.dest = Utils.readUInt32(cache2rawFile);
                        spell.proportion = Utils.readSingle(cache2rawFile);
                        spell.lossPercent = Utils.readSingle(cache2rawFile);
                        spell.sourceLoss = Utils.readInt32(cache2rawFile);
                        spell.transferCap = Utils.readInt32(cache2rawFile);
                        spell.maxBoostAllowed = Utils.readInt32(cache2rawFile);
                        spell.transferBitfield = Utils.readUInt32(cache2rawFile);
                        break;
                    case 5: //PortalLink_SpellType
                    case 6: //PortalRecall_SpellType
                        spell.spellId2 = Utils.readUInt32(cache2rawFile);
                        spell.index = Utils.readInt32(cache2rawFile);
                        break;
                    case 7: //PortalSummon_SpellType
                        spell.spellId2 = Utils.readUInt32(cache2rawFile);
                        spell.portal_lifetime = Utils.readDouble(cache2rawFile);
                        spell.link = Utils.readInt32(cache2rawFile);
                        break;
                    case 8: //PortalSending_SpellType
                    case 13: //FellowPortalSending_SpellType
                        spell.spellId2 = Utils.readUInt32(cache2rawFile);
                        spell.pos = new sPosition(cache2rawFile);
                        break;
                    case 9: //Dispel_SpellType
                    case 14: //FellowDispel_SpellType
                        spell.spellId2 = Utils.readUInt32(cache2rawFile);
                        spell.min_power = Utils.readInt32(cache2rawFile);
                        spell.max_power = Utils.readInt32(cache2rawFile);
                        spell.power_variance = Utils.readSingle(cache2rawFile);
                        spell.dispelSchool = Utils.readInt32(cache2rawFile);
                        spell.align = Utils.readInt32(cache2rawFile);
                        spell.number = Utils.readInt32(cache2rawFile);
                        spell.number_variance = Utils.readSingle(cache2rawFile);
                        break;
                    case 10: //LifeProjectile_SpellType
                        spell.spellId2 = Utils.readUInt32(cache2rawFile);
                        spell.etype = Utils.readUInt32(cache2rawFile);
                        spell.baseIntensity = Utils.readInt32(cache2rawFile);
                        spell.variance = Utils.readInt32(cache2rawFile);
                        spell.wcid = Utils.readUInt32(cache2rawFile);
                        spell.numProjectiles = Utils.readInt32(cache2rawFile);
                        spell.numProjectilesVariance = Utils.readInt32(cache2rawFile);
                        spell.spreadAngle = Utils.readSingle(cache2rawFile);
                        spell.verticalAngle = Utils.readSingle(cache2rawFile);
                        spell.defaultLaunchAngle = Utils.readSingle(cache2rawFile);
                        spell.bNonTracking = Utils.readInt32(cache2rawFile);
                        spell.createOffset[0] = Utils.readSingle(cache2rawFile);
                        spell.createOffset[1] = Utils.readSingle(cache2rawFile);
                        spell.createOffset[2] = Utils.readSingle(cache2rawFile);
                        spell.padding[0] = Utils.readSingle(cache2rawFile);
                        spell.padding[1] = Utils.readSingle(cache2rawFile);
                        spell.padding[2] = Utils.readSingle(cache2rawFile);
                        spell.dims[0] = Utils.readSingle(cache2rawFile);
                        spell.dims[1] = Utils.readSingle(cache2rawFile);
                        spell.dims[2] = Utils.readSingle(cache2rawFile);
                        spell.peturbation[0] = Utils.readSingle(cache2rawFile);
                        spell.peturbation[1] = Utils.readSingle(cache2rawFile);
                        spell.peturbation[2] = Utils.readSingle(cache2rawFile);
                        spell.imbuedEffect = Utils.readUInt32(cache2rawFile);
                        spell.slayerCreatureType = Utils.readInt32(cache2rawFile);
                        spell.slayerDamageBonus = Utils.readSingle(cache2rawFile);
                        spell.critFreq = Utils.readDouble(cache2rawFile);
                        spell.critMultiplier = Utils.readDouble(cache2rawFile);
                        spell.ignoreMagicResist = Utils.readInt32(cache2rawFile);
                        spell.elementalModifier = Utils.readDouble(cache2rawFile);
                        spell.drain_percentage = Utils.readSingle(cache2rawFile);
                        spell.damage_ratio = Utils.readSingle(cache2rawFile);
                        break;
                    case 15: //EnchantmentProjectile_SpellType
                        spell.spellId2 = Utils.readUInt32(cache2rawFile);
                        spell.etype = Utils.readUInt32(cache2rawFile);
                        spell.baseIntensity = Utils.readInt32(cache2rawFile);
                        spell.variance = Utils.readInt32(cache2rawFile);
                        spell.wcid = Utils.readUInt32(cache2rawFile);
                        spell.numProjectiles = Utils.readInt32(cache2rawFile);
                        spell.numProjectilesVariance = Utils.readInt32(cache2rawFile);
                        spell.spreadAngle = Utils.readSingle(cache2rawFile);
                        spell.verticalAngle = Utils.readSingle(cache2rawFile);
                        spell.defaultLaunchAngle = Utils.readSingle(cache2rawFile);
                        spell.bNonTracking = Utils.readInt32(cache2rawFile);
                        spell.createOffset[0] = Utils.readSingle(cache2rawFile);
                        spell.createOffset[1] = Utils.readSingle(cache2rawFile);
                        spell.createOffset[2] = Utils.readSingle(cache2rawFile);
                        spell.padding[0] = Utils.readSingle(cache2rawFile);
                        spell.padding[1] = Utils.readSingle(cache2rawFile);
                        spell.padding[2] = Utils.readSingle(cache2rawFile);
                        spell.dims[0] = Utils.readSingle(cache2rawFile);
                        spell.dims[1] = Utils.readSingle(cache2rawFile);
                        spell.dims[2] = Utils.readSingle(cache2rawFile);
                        spell.peturbation[0] = Utils.readSingle(cache2rawFile);
                        spell.peturbation[1] = Utils.readSingle(cache2rawFile);
                        spell.peturbation[2] = Utils.readSingle(cache2rawFile);
                        spell.imbuedEffect = Utils.readUInt32(cache2rawFile);
                        spell.slayerCreatureType = Utils.readInt32(cache2rawFile);
                        spell.slayerDamageBonus = Utils.readSingle(cache2rawFile);
                        spell.critFreq = Utils.readDouble(cache2rawFile);
                        spell.critMultiplier = Utils.readDouble(cache2rawFile);
                        spell.ignoreMagicResist = Utils.readInt32(cache2rawFile);
                        spell.elementalModifier = Utils.readDouble(cache2rawFile);
                        spell.elementalDamageType = Utils.readUInt32(cache2rawFile);
                        spell.duration = Utils.readDouble(cache2rawFile);
                        spell.degrade_modifier = Utils.readSingle(cache2rawFile);
                        spell.degrade_limit = Utils.readSingle(cache2rawFile);
                        spell.spellCategory = Utils.readInt32(cache2rawFile);
                        spell.smod_type = Utils.readUInt32(cache2rawFile);
                        spell.smod_key = Utils.readUInt32(cache2rawFile);
                        spell.smod_val = Utils.readSingle(cache2rawFile);
                        break;
                }

                for (int i = 0; i < 8; i++)
                {
                    spell.component[i] = Utils.readUInt32(cache2rawFile);
                }

                spell.caster_effect = Utils.readInt32(cache2rawFile);
                spell.target_effect = Utils.readInt32(cache2rawFile);
                spell.fizzle_effect = Utils.readInt32(cache2rawFile);
                spell.recovery_interval = Utils.readDouble(cache2rawFile);
                spell.recovery_amount = Utils.readSingle(cache2rawFile);

                //there's a divergence here that must have been added by the devs at some point
                if (spell.caster_effect >= 30)
                    spell.caster_effect += 1;
                if (spell.target_effect >= 30)
                    spell.target_effect += 1;
                if (spell.fizzle_effect >= 30)
                    spell.fizzle_effect += 1;

                spell.display_order = Utils.readInt32(cache2rawFile);
                spell.non_component_target_type = Utils.readUInt32(cache2rawFile);
                spell.mana_mod = Utils.readInt32(cache2rawFile);

                Cache2SpellTable.Add(spell.spellId, spell);
            }

            cache2rawFile.Close();
            Console.WriteLine("Done");
        }

        public void ConvertStringToByteArrayNoEncode(string text, ref byte[] byteArray, int startIndex, int length)
        {
            for (int i = startIndex; i < startIndex + length; i++)
            {
                byte nextByte = (byte)text[i];
                byteArray[i] = nextByte;
            }

            byte fillerByte = 0x00;
            for (int i = startIndex + length; i < startIndex + length + 4; i++)
            {

                byteArray[i] = fillerByte;
            }
        }

        public void ConvertStringToByteArray(string text, ref byte[] byteArray, int startIndex, int length)
        {
            for (int i = startIndex; i < startIndex + length; i++)
            {
                byte nextByte = (byte)((text[i] << 4) ^ (text[i] >> 4));
                byteArray[i] = nextByte;
            }

            byte fillerByte = (byte)((0 << 4) ^ (0 >> 4));
            for (int i = startIndex + length; i < startIndex + length + 4; i++)
            {

                byteArray[i] = fillerByte;
            }
        }

        public void UpdateDamageFromServerData(string serverSpellDamageList)
        {
            var input = File.ReadAllLines(serverSpellDamageList);

            foreach (var entry in input)
            {
                var values = entry.Split('\t');
                var spellIdString = values[0];
                var minDamage = values[1];
                var maxDamage = values[2];

                if (uint.TryParse(spellIdString, out var spellId) && SpellTable.Spells.TryGetValue(spellId, out var spell))
                {
                    if (spell.Desc.Contains('-'))
                    {
                        var dashPos = spell.Desc.IndexOf('-');
                        var minDamageStartPos = spell.Desc.Substring(0, dashPos).LastIndexOf(' ') + 1;
                        var maxDamageEndPos = dashPos + spell.Desc.Substring(dashPos, spell.Desc.Length - dashPos).IndexOf(' ');

                        var preDamageString = spell.Desc.Substring(0, minDamageStartPos);
                        var postDamageString = spell.Desc.Substring(maxDamageEndPos, spell.Desc.Length - maxDamageEndPos);

                        var newDesc = $"{preDamageString}{minDamage}-{maxDamage}{postDamageString}";
                        spell.Desc = newDesc;
                    }
                }
                else
                    Debug.Assert(false);
            }
        }

        public uint GetHighestSpellId()
        {
            uint highestId = 0;
            foreach (var spellEntry in SpellTable.Spells)
            {
                if (spellEntry.Value.MetaSpellId > highestId)
                    highestId = spellEntry.Value.MetaSpellId;
            }

            return highestId;
        }

        public SpellBase NewSpell(SpellBase baseSpell, SpellId id, string name, string desc, uint icon, SpellCategory category)
        {
            var newSpell = new SpellBase(baseSpell);
            newSpell.MetaSpellId = (uint)id;
            newSpell.Name = name;
            newSpell.Desc = desc;
            newSpell.Icon = icon;
            newSpell.Category = category;

            return newSpell;
        }

        public void AddSpell(SpellId spellId, uint icon, SpellCategory category, SpellId baseSpellId, string replaceNameFrom, string replaceNameTo, uint replaceCompFrom = 0, uint replaceCompTo = 0)
        {
            if (SpellTable.Spells.TryGetValue((uint)baseSpellId, out var baseSpell))
            {
                var newSpell = NewSpell(baseSpell, spellId, baseSpell.Name.Replace(replaceNameFrom, replaceNameTo), baseSpell.Desc.Replace(replaceNameFrom, replaceNameTo), icon, category);
                if(replaceCompFrom != 0)
                    ReplaceComponent(newSpell.Formula, replaceCompFrom, replaceCompTo);
                SpellTable.Spells.Add((uint)spellId, newSpell);
            }
        }

        public void ReplaceComponent(List<uint> formula, uint from, uint to)
        {
            for (int i = 0; i < formula.Count; i++)
            {
                if (formula[i] == from)
                    formula[i] = to;
            }
        }

        public void ModifyForCustomDM()
        {
            if (SpellTable == null)
            {
                Console.WriteLine("SpellTable is empty");
                return;
            }

            Console.WriteLine("Modifying spells...");

            AddSpell(SpellId.ArmorMasterySelf1, 0x06020001, SpellCategory.ArmorSkillRaising, SpellId.ShieldMasterySelf1, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorMasterySelf2, 0x06020001, SpellCategory.ArmorSkillRaising, SpellId.ShieldMasterySelf2, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorMasterySelf3, 0x06020001, SpellCategory.ArmorSkillRaising, SpellId.ShieldMasterySelf3, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorMasterySelf4, 0x06020001, SpellCategory.ArmorSkillRaising, SpellId.ShieldMasterySelf4, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorMasterySelf5, 0x06020001, SpellCategory.ArmorSkillRaising, SpellId.ShieldMasterySelf5, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorMasterySelf6, 0x06020001, SpellCategory.ArmorSkillRaising, SpellId.ShieldMasterySelf6, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorMasterySelf7, 0x06020001, SpellCategory.ArmorSkillRaising, SpellId.ShieldMasterySelf7, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorMasterySelf8, 0x06020001, SpellCategory.ArmorSkillRaising, SpellId.ShieldMasterySelf8, "Shield", "Armor", 37, 40);

            AddSpell(SpellId.ArmorMasteryOther1, 0x06020001, SpellCategory.ArmorSkillRaising, SpellId.ShieldMasteryOther1, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorMasteryOther2, 0x06020001, SpellCategory.ArmorSkillRaising, SpellId.ShieldMasteryOther2, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorMasteryOther3, 0x06020001, SpellCategory.ArmorSkillRaising, SpellId.ShieldMasteryOther3, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorMasteryOther4, 0x06020001, SpellCategory.ArmorSkillRaising, SpellId.ShieldMasteryOther4, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorMasteryOther5, 0x06020001, SpellCategory.ArmorSkillRaising, SpellId.ShieldMasteryOther5, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorMasteryOther6, 0x06020001, SpellCategory.ArmorSkillRaising, SpellId.ShieldMasteryOther6, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorMasteryOther7, 0x06020001, SpellCategory.ArmorSkillRaising, SpellId.ShieldMasteryOther7, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorMasteryOther8, 0x06020001, SpellCategory.ArmorSkillRaising, SpellId.ShieldMasteryOther8, "Shield", "Armor", 37, 40);

            AddSpell(SpellId.ArmorIneptitudeOther1, 0x06020001, SpellCategory.ArmorSkillLowering, SpellId.ShieldIneptitudeOther1, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorIneptitudeOther2, 0x06020001, SpellCategory.ArmorSkillLowering, SpellId.ShieldIneptitudeOther2, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorIneptitudeOther3, 0x06020001, SpellCategory.ArmorSkillLowering, SpellId.ShieldIneptitudeOther3, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorIneptitudeOther4, 0x06020001, SpellCategory.ArmorSkillLowering, SpellId.ShieldIneptitudeOther4, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorIneptitudeOther5, 0x06020001, SpellCategory.ArmorSkillLowering, SpellId.ShieldIneptitudeOther5, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorIneptitudeOther6, 0x06020001, SpellCategory.ArmorSkillLowering, SpellId.ShieldIneptitudeOther6, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorIneptitudeOther7, 0x06020001, SpellCategory.ArmorSkillLowering, SpellId.ShieldIneptitudeOther7, "Shield", "Armor", 37, 40);
            AddSpell(SpellId.ArmorIneptitudeOther8, 0x06020001, SpellCategory.ArmorSkillLowering, SpellId.ShieldIneptitudeOther8, "Shield", "Armor", 37, 40);

            AddSpell(SpellId.CantripArmorAptitude1, 0x06020001, SpellCategory.ExtraArmorSkillRaising, SpellId.CantripShieldAptitude1, "Shield", "Armor");
            AddSpell(SpellId.CantripArmorAptitude2, 0x06020001, SpellCategory.ExtraArmorSkillRaising, SpellId.CantripShieldAptitude2, "Shield", "Armor");
            AddSpell(SpellId.CantripArmorAptitude3, 0x06020001, SpellCategory.ExtraArmorSkillRaising, SpellId.CantripShieldAptitude3, "Shield", "Armor");
            AddSpell(SpellId.CantripArmorAptitude4, 0x06020001, SpellCategory.ExtraArmorSkillRaising, SpellId.CantripShieldAptitude4, "Shield", "Armor");

            foreach (var spellEntry in SpellTable.Spells)
            {
                SpellBase spell = spellEntry.Value;

                if (spell.ComponentLoss == 0.0f && spell.Power == 1)
                    spell.ComponentLoss = 0.01f; // All level 1 spells now have a chance to burn components

                switch (spell.Category)
                {
                    case SpellCategory.ArmorRaising:
                    case SpellCategory.ArmorIncrease:
                        spell.Desc += " Natural armor does not stack with equipment armor. The highest of the two will be used.";
                        break;
                    case SpellCategory.AxeRaising: // Axe mastery
                    case SpellCategory.AxeRaisingRare:
                    case SpellCategory.CascadeAxeRaising: // Axe cantrip
                    case SpellCategory.CascadeAxeRaising2: // Axe boon
                        if (!spell.Name.Contains("Boon"))
                            spell.Name = spell.Name.Replace("Axe", "Axe and Mace");
                        spell.Desc = spell.Desc.Replace("Axe", "Axe and Mace");
                        break;
                    case SpellCategory.AxeLowering: // Axe ineptude
                        spell.Name = spell.Name.Replace("Axe", "Axe and Mace");
                        spell.Desc = spell.Desc.Replace("Axe", "Axe and Mace");
                        break;
                    case SpellCategory.MaceRaising: // Mace mastery
                    case SpellCategory.MaceRaisingRare:
                        spell.Name = spell.Name.Replace("Mace", "Axe and Mace");
                        spell.Desc = spell.Desc.Replace("Mace", "Axe and Mace");
                        spell.Category = SpellCategory.AxeRaising;
                        break;
                    case SpellCategory.CascadeMaceRaising: // Mace cantrip
                    case SpellCategory.CascadeMaceRaising2: // Mace boon
                        if (!spell.Name.Contains("Boon"))
                            spell.Name = spell.Name.Replace("Mace", "Axe and Mace");
                        spell.Desc = spell.Desc.Replace("Mace", "Axe and Mace");
                        spell.Category = SpellCategory.CascadeAxeRaising2;
                        break;
                    case SpellCategory.MaceLowering: // Mace ineptude
                        spell.Name = spell.Name.Replace("Mace", "Axe and Mace");
                        spell.Desc = spell.Desc.Replace("Mace", "Axe and Mace");
                        spell.Category = SpellCategory.AxeLowering;
                        break;
                    case SpellCategory.SpearRaising: // Spear mastery
                    case SpellCategory.SpearRaisingRare:
                    case SpellCategory.CascadeSpearRaising: // Spear cantrip
                    case SpellCategory.CascadeSpearRaising2: // Spear boon
                        if (!spell.Name.Contains("Boon"))
                            spell.Name = spell.Name.Replace("Spear", "Spear and Staff");
                        spell.Desc = spell.Desc.Replace("Spear", "Spear and Staff");
                        break;
                    case SpellCategory.SpearLowering: // Spear ineptude
                        spell.Name = spell.Name.Replace("Spear", "Spear and Staff");
                        spell.Desc = spell.Desc.Replace("Spear", "Spear and Staff");
                        break;
                    case SpellCategory.StaffRaising: // Staff mastery
                    case SpellCategory.StaffRaisingRare:
                        spell.Name = spell.Name.Replace("Staff", "Spear and Staff");
                        spell.Desc = spell.Desc.Replace("Staff", "Spear and Staff");
                        spell.Category = SpellCategory.SpearRaising;
                        break;
                    case SpellCategory.CascadeStaffRaising: // Staff cantrip
                    case SpellCategory.CascadeStaffRaising2: // Staff boon
                        if (!spell.Name.Contains("Boon"))
                            spell.Name = spell.Name.Replace("Staff", "Spear and Staff");
                        spell.Desc = spell.Desc.Replace("Staff", "Spear and Staff");
                        spell.Category = SpellCategory.CascadeSpearRaising2;
                        break;
                    case SpellCategory.StaffLowering: // Staff ineptude
                        spell.Name = spell.Name.Replace("Staff", "Spear and Staff");
                        spell.Desc = spell.Desc.Replace("Staff", "Spear and Staff");
                        spell.Category = SpellCategory.SpearLowering;
                        break;
                    case SpellCategory.BowRaising: // Bow mastery
                    case SpellCategory.BowRaisingRare:
                    case SpellCategory.ExtraBowSkillRaising: // Bow cantrip
                    case SpellCategory.ExtraBowSkillRaising2: // Bow boon
                        if (!spell.Name.Contains("Boon"))
                            spell.Name = spell.Name.Replace("Bow", "Bow and Crossbow");
                        spell.Desc = spell.Desc.Replace("Bow", "Bow and Crossbow");
                        break;
                    case SpellCategory.BowLowering: // Bow ineptude
                        spell.Name = spell.Name.Replace("Bow", "Bow and Crossbow");
                        spell.Desc = spell.Desc.Replace("Bow", "Bow and Crossbow");
                        break;
                    case SpellCategory.CrossbowRaising: // Crossbow mastery
                    case SpellCategory.CrossbowRaisingRare:
                        if (!spell.Name.Contains("Boon"))
                            spell.Name = spell.Name.Replace("Crossbow", "Bow and Crossbow");
                        spell.Desc = spell.Desc.Replace("Crossbow", "Bow and Crossbow");
                        spell.Category = SpellCategory.BowRaising;
                        break;
                    case SpellCategory.ExtraCrossbowSkillRaising: // Crossbow cantrip
                    case SpellCategory.ExtraCrossbowSkillRaising2: // Crossbow boon
                        if (!spell.Name.Contains("Boon"))
                            spell.Name = spell.Name.Replace("Crossbow", "Bow and Crossbow");
                        spell.Desc = spell.Desc.Replace("Crossbow", "Bow and Crossbow");
                        spell.Category = SpellCategory.ExtraBowSkillRaising2;
                        break;
                    case SpellCategory.CrossbowLowering: // Crossbow ineptude
                        spell.Name = spell.Name.Replace("Crossbow", "Bow and Crossbow");
                        spell.Desc = spell.Desc.Replace("Crossbow", "Bow and Crossbow");
                        spell.Category = SpellCategory.BowLowering;
                        break;
                }
            }
            Console.WriteLine("Done");
        }

        public void RevertWeaponMasteriesAndAuras(SpellManipulationTools oldSpellData)
        {
            if (SpellTable == null)
            {
                Console.WriteLine("SpellTable is empty");
                return;
            }

            if (oldSpellData == null || oldSpellData.SpellTable == null)
            {
                Console.WriteLine("Old spellTable is empty");
                return;
            }

            Dictionary<uint, SpellBase> Spells = new Dictionary<uint, SpellBase>();
            Dictionary<uint, SpellBase> OldSpells = oldSpellData.SpellTable.Spells;

            Console.WriteLine("Reverting weapon masteries spells...");

            foreach(var spellEntry in SpellTable.Spells)
            {
                SpellBase spell = spellEntry.Value;
                SpellBase oldSpell;

                if (spell.Name.Contains("Monster") || spell.Name.Contains("Topheron's") || spell.Name == "Ignorance's Bliss" || spell.Name == "Fauna Perlustration")
                {
                    spell.Name = spell.Name.Replace("Monster", "Assess");
                    spell.Desc = spell.Desc.Replace("Monster ", "");
                    spell.Desc = spell.Desc.Replace("Creature ", "");
                }

                // Revert Item Enchantment
                if (OldSpells.TryGetValue(spell.MetaSpellId, out oldSpell))
                {
                    if (spell.Name.Contains("Light Weapon") ||
                        spell.Name.Contains("Finesse Weapon") ||
                        spell.Name.Contains("Heavy Weapon") ||
                        spell.Name.Contains("Light Weapon") ||
                        spell.Name.Contains("Missile Weapon") ||
                        spell.Name.Contains("Kern's Boon") ||
                        spell.Name.Contains("Ranger's Boon") ||
                        spell.Name.Contains("Fencer's Boon") ||
                        spell.Name.Contains("Soldier's Boon") ||
                        spell.Name.Contains("Cascade"))
                    {
                        spell.Name = oldSpell.Name;
                        spell.Desc = oldSpell.Desc;
                        spell.Icon = oldSpell.Icon;
                        spell.Category = oldSpell.Category;
                        spell.CasterEffect = oldSpell.CasterEffect;
                        spell.TargetEffect = oldSpell.TargetEffect;
                        spell.FizzleEffect = oldSpell.FizzleEffect;
                    }

                    if (spell.NonComponentTargetType != oldSpell.NonComponentTargetType)
                    {
                        spell.Bitfield &= ~(uint)(eSpellIndex.SelfTargeted_SpellIndex);

                        for (int i = 0; i < 8; i++)
                        {
                            //60 = rowan talisman(creature enchantment self)
                            //49 = poplar talisman(creature enchantment other)
                            if (spell.Formula.Count > i && (spell.Formula[i] == 60 || spell.Formula[i] == 49))
                                spell.Formula[i] = 57; //change rowan and poplar talisman to ashwood talisman
                        }

                        spell.Name = oldSpell.Name;
                        spell.Desc = oldSpell.Desc;
                        spell.Icon = oldSpell.Icon;
                        spell.CasterEffect = oldSpell.CasterEffect;
                        spell.TargetEffect = oldSpell.TargetEffect;
                        spell.FizzleEffect = oldSpell.FizzleEffect;
                        spell.NonComponentTargetType = oldSpell.NonComponentTargetType;
                    }
                }
                else
                {
                    Console.WriteLine("Could not find spell in the old database: {0} - {1}", spell.MetaSpellId, spell.Name);
                }
            }

            Console.WriteLine("Done");
        }

        public void ApplyCache2DataFixes()
        {
            if (Cache2SpellTable == null || Cache2SpellTable.Count == 0)
            {
                Console.WriteLine("Cache2SpellTable is empty");
                return;
            }

            Cache2Spell cacheSpell;

            if (Cache2SpellTable.TryGetValue(7, out cacheSpell)) // Harm Other I
                cacheSpell.desc = "Drains 4-10 points of the target's Health.";

            if (Cache2SpellTable.TryGetValue(8, out cacheSpell)) // Harm Self I
                cacheSpell.desc = "Drains 4-10 points of the caster's Health.";

            if (Cache2SpellTable.TryGetValue(2549, out cacheSpell)) // Minor Impregnability
                cacheSpell.category = (int)SpellCategory.ExtraMissileDefenseSkillRaising;

            if (Cache2SpellTable.TryGetValue(2373, out cacheSpell)) // Enervation of the Heart
                cacheSpell.category = (int)SpellCategory.HealthLowering;

            if (Cache2SpellTable.TryGetValue(2374, out cacheSpell)) // Enervation of the Limb
                cacheSpell.category = (int)SpellCategory.StaminaLowering;

            if (Cache2SpellTable.TryGetValue(2417, out cacheSpell)) // Obedience
            {
                cacheSpell.desc = "Increases the target's Leadership skill by 10 points. This can be combined with other Leadership-enhancing spells.";
                cacheSpell.power = 10;
            }

            if (Cache2SpellTable.TryGetValue(2358, out cacheSpell)) // Lyceum Recall
                cacheSpell.desc = "Transports the caster to the Ishilai Lyceum.";

            if (Cache2SpellTable.TryGetValue(3060, out cacheSpell)) // Poison Blood
                cacheSpell.desc = "Lowers the total health of a target by 30% for 45 seconds.";

            if (Cache2SpellTable.TryGetValue(3064, out cacheSpell)) // Poison Blood
                cacheSpell.desc = "Lowers the total health of a target by 10% for 45 seconds.";

            if (Cache2SpellTable.TryGetValue(3069, out cacheSpell)) // Poison Blood
                cacheSpell.desc = "Lowers the total health of a target by 20% for 45 seconds.";

            if (Cache2SpellTable.TryGetValue(3242, out cacheSpell)) // Weave of Chorizite
                cacheSpell.desc = "Veins of chorizite serve to raise your magic defense. Magic Defense is raised by 2 points when this item is wielded. This is in addition to any spells and cantrips.";

            Console.WriteLine("Applying fixes to cache2 data...");
        }

        public void TransferSpellDataFromCacheToSpellBase()
        {
            if (SpellTable == null || SpellTable.Spells.Count == 0)
            {
                Console.WriteLine("SpellTable is empty");
                return;
            }

            if (Cache2SpellTable == null || Cache2SpellTable.Count == 0)
            {
                Console.WriteLine("Cache2SpellTable is empty");
                return;
            }

            Console.WriteLine("Transfering data from cache originated file to dat originated file...");

            List<uint> ignoreList = new List<uint>();
            ignoreList.Add(3250); // Major Spirit Thirst
            ignoreList.Add(3251); // Minor Spirit Thirst
            ignoreList.Add(3252); // Spirit Thirst

            ignoreList.Add(3253); // Spirit Drinker I
            ignoreList.Add(3254); // Spirit Drinker II
            ignoreList.Add(3255); // Spirit Drinker II
            ignoreList.Add(3256); // Spirit Drinker IV
            ignoreList.Add(3257); // Spirit Drinker V
            ignoreList.Add(3258); // Spirit Drinker VI

            ignoreList.Add(3260); // Spirit Loather I
            ignoreList.Add(3261); // Spirit Loather II
            ignoreList.Add(3262); // Spirit Loather II
            ignoreList.Add(3263); // Spirit Loather IV
            ignoreList.Add(3264); // Spirit Loather V
            ignoreList.Add(3265); // Spirit Loather VI

            ignoreList.Add(3452); // Concussive Wail
            ignoreList.Add(3455); // Koruu Cloud
            ignoreList.Add(3457); // Mana Bolt
            ignoreList.Add(3458); // Mana Purge
            ignoreList.Add(3459); // Mucor Cloud

            ignoreList.Add(3651); // Aerfalle's Gaze

            for(uint id = 2760; id <= 2780; id++) 
                ignoreList.Add(id); // Martyr's spells

            foreach (var spellEntry in SpellTable.Spells)
            {
                SpellBase spell = spellEntry.Value;
                Cache2Spell cacheSpell;

                if (ignoreList.Contains(spell.MetaSpellId))
                    continue;

                if (Cache2SpellTable.TryGetValue(spell.MetaSpellId, out cacheSpell))
                {
                    if (spell.Name == cacheSpell.name)
                    {
                        spell.Name = cacheSpell.name;
                        spell.Desc = cacheSpell.desc;
                        spell.School = (MagicSchool)cacheSpell.school;
                        spell.Icon = cacheSpell.iconID;
                        //spell.Category = (SpellCategory)cacheSpell.category; // Spell categories in the cache seem to have many errors, keep what we got.
                        if (spell.MetaSpellId == 2437 || spell.MetaSpellId == 2438 || spell.MetaSpellId == 2439) // Greater, Lesser and Regular Rockslide
                            spell.Category = (SpellCategory)cacheSpell.category;
                        //spell.Bitfield = cacheSpell.bitfield;
                        spell.BaseMana = (uint)cacheSpell.base_mana;
                        //spell.BaseRangeConstant = cacheSpell.base_range_constant;
                        //spell.BaseRangeMod = cacheSpell.base_range_mod;
                        //spell.Power = (uint)cacheSpell.power;
                        //spell.SpellEconomyMod = cacheSpell.spell_economy_mod;
                        //spell.FormulaVersion = cacheSpell.formula_version;
                        spell.ComponentLoss = cacheSpell.component_loss;
                        //spell.MetaSpellType = (SpellType)cacheSpell.sp_type;

                        switch (cacheSpell.sp_type)
                        {
                            case 1: //Enchantment_SpellType
                            case 12: //FellowEnchantment_SpellType
                                //spell.Duration = cacheSpell.duration;
                                //spell.DegradeModifier = cacheSpell.degrade_modifier;
                                //spell.DegradeLimit = cacheSpell.degrade_limit;
                                break;
                            case 7: //PortalSummon_SpellType
                                //spell.PortalLifetime = cacheSpell.portal_lifetime;
                                break;
                        }

                        //spell.Formula = new List<uint>(cacheSpell.component);

                        //spell.CasterEffect = (uint)cacheSpell.caster_effect;
                        //spell.TargetEffect = (uint)cacheSpell.target_effect;
                        //spell.FizzleEffect = (uint)cacheSpell.fizzle_effect;
                        spell.RecoveryInterval = cacheSpell.recovery_interval;
                        spell.RecoveryAmount = cacheSpell.recovery_amount;

                        //spell.DisplayOrder = cacheSpell.display_order; // Keep the client order as we have more spells there
                        spell.NonComponentTargetType = cacheSpell.non_component_target_type;
                        spell.ManaMod = (uint)cacheSpell.mana_mod;
                    }
                    else
                        Console.WriteLine("Spell with same id but different name found: {0} -> {1}", spell.Name, cacheSpell.name);
                }
                else
                    Console.WriteLine("Spell not found on the cache file: {0} - {1}", spell.MetaSpellId, spell.Name);
            }

            Console.WriteLine("Done");
        }
    }
}
