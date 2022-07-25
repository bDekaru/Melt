using System.Collections.Generic;
using System.IO;

using ACE.DatLoader.FileTypes;
using ACE.Entity.Enum;

namespace ACE.DatLoader.Entity
{
    public class SpellBase : IUnpackable
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public MagicSchool School { get; set; }
        public uint Icon { get; set; }
        public SpellCategory Category { get; set; } // All related levels of the same spell. Same category spells will not stack. (Strength Self I & Strength Self II)
        public uint Bitfield { get; set; }
        public uint BaseMana { get; set; } // Mana Cost
        public float BaseRangeConstant { get; set; }
        public float BaseRangeMod { get; set; }
        public uint Power { get; set; } // Used to determine which spell in the catgory is the strongest.
        public float SpellEconomyMod { get; set; } // A legacy of a bygone era
        public uint FormulaVersion { get; set; }
        public float ComponentLoss { get; set; } // Burn rate
        public SpellType MetaSpellType { get; set; }
        public uint MetaSpellId { get; set; } // Just the spell id again

        // Only on EnchantmentSpell/FellowshipEnchantmentSpells
        public double Duration { get; set; }
        public float DegradeModifier { get; set; } // Unknown what this does
        public float DegradeLimit { get; set; }  // Unknown what this does

        public double PortalLifetime { get; set; } // Only for PortalSummon_SpellType

        public List<uint> Formula { get; set; } // UInt Values correspond to the SpellComponentsTable

        public uint CasterEffect { get; set; }  // effect that playes on the caster of the casted spell (e.g. for buffs, protects, etc)
        public uint TargetEffect { get; set; } // effect that playes on the target of the casted spell (e.g. for debuffs, vulns, etc)
        public uint FizzleEffect { get; set; } // is always zero. All spells have the same fizzle effect.
        public double RecoveryInterval { get; set; } // is always zero
        public float RecoveryAmount { get; set; } // is always zero
        public uint DisplayOrder { get; set; } // for soring in the spell list in the client UI
        public uint NonComponentTargetType { get; set; } // Unknown what this does
        public uint ManaMod { get; set; } // Additional mana cost per target (e.g. "Incantation of Acid Bane" Mana Cost = 80 + 14 per target)

        public SpellBase()
        {
        }

        public SpellBase(uint power, double duration, float degradeModifier, float degradeLimit)
        {
            Power = power;

            Duration = duration;
            DegradeModifier = degradeModifier;
            DegradeLimit = degradeLimit;
        }

        public SpellBase(SpellBase other)
        {
            Name = other.Name;
            Desc = other.Desc;
            School = other.School;
            Icon = other.Icon;
            Category = other.Category;
            Bitfield = other.Bitfield;
            BaseMana = other.BaseMana;
            BaseRangeConstant = other.BaseRangeConstant;
            BaseRangeMod = other.BaseRangeMod;
            Power = other.Power;
            SpellEconomyMod = other.SpellEconomyMod;
            FormulaVersion = other.FormulaVersion;
            ComponentLoss = other.ComponentLoss;
            MetaSpellType = other.MetaSpellType;
            MetaSpellId = other.MetaSpellId;

            switch (MetaSpellType)
            {
                case SpellType.Enchantment:
                case SpellType.FellowEnchantment:
                    Duration = other.Duration;
                    DegradeModifier = other.DegradeModifier;
                    DegradeLimit = other.DegradeLimit;
                    break;
                case SpellType.PortalSummon:
                    PortalLifetime = other.PortalLifetime;
                    break;
            }

            Formula = new List<uint>(other.Formula);

            CasterEffect = other.CasterEffect;
            TargetEffect = other.TargetEffect;
            FizzleEffect = other.FizzleEffect;
            RecoveryInterval = other.RecoveryInterval;
            RecoveryAmount = other.RecoveryAmount;
            DisplayOrder = other.DisplayOrder;
            NonComponentTargetType = other.NonComponentTargetType;
            ManaMod = other.ManaMod;
        }

        public void Unpack(BinaryReader reader)
        {
            Name = reader.ReadObfuscatedString();
            reader.AlignBoundary();
            Desc = reader.ReadObfuscatedString();
            reader.AlignBoundary();
            School = (MagicSchool)reader.ReadUInt32();
            Icon = reader.ReadUInt32();
            Category = (SpellCategory)reader.ReadUInt32();
            Bitfield = reader.ReadUInt32();
            BaseMana = reader.ReadUInt32();
            BaseRangeConstant = reader.ReadSingle();
            BaseRangeMod = reader.ReadSingle();
            Power = reader.ReadUInt32();
            SpellEconomyMod = reader.ReadSingle();
            FormulaVersion = reader.ReadUInt32();
            ComponentLoss = reader.ReadSingle();
            MetaSpellType = (SpellType)reader.ReadUInt32();
            MetaSpellId = reader.ReadUInt32();

            switch (MetaSpellType)
            {
                case SpellType.Enchantment:
                case SpellType.FellowEnchantment:
                    Duration = reader.ReadDouble();
                    DegradeModifier = reader.ReadSingle();
                    DegradeLimit = reader.ReadSingle();
                    break;
                case SpellType.PortalSummon:
                    PortalLifetime = reader.ReadDouble();
                    break;
            }

            // Components : Load them first, then decrypt them. More efficient to hash all at once.
            List<uint> rawComps = new List<uint>();

            for (uint j = 0; j < 8; j++)
            {
                uint comp = reader.ReadUInt32();

                // We will only add the comp if it is valid
                if (comp > 0)
                    rawComps.Add(comp);
            }

            if (MetaSpellId != 3452
                && MetaSpellId != 3455
                && MetaSpellId != 3457
                && MetaSpellId != 3458
                && MetaSpellId != 3459
                && MetaSpellId != 3810
                && MetaSpellId != 3811
                && MetaSpellId != 3818
                && MetaSpellId != 3953
                && MetaSpellId != 3966
                && MetaSpellId != 4024
                ) // Odd uncastable spells - Let's move these values along without modifications.
            {
                // Get the decryped component values
                Formula = DecryptFormula(rawComps, Name, Desc);
            }
            else
                Formula = rawComps;

            CasterEffect = reader.ReadUInt32();
            TargetEffect = reader.ReadUInt32();
            FizzleEffect = reader.ReadUInt32();
            RecoveryInterval = reader.ReadDouble();
            RecoveryAmount = reader.ReadSingle();
            DisplayOrder = reader.ReadUInt32();
            NonComponentTargetType = reader.ReadUInt32();
            ManaMod = reader.ReadUInt32();
        }

        private const uint HIGHEST_COMP_ID = 198; // "Essence of Kemeroi", for Void Spells -- not actually ever in game!

        /// <summary>
        /// Does the math based on the crypto keys (name and description) for the spell formula.
        /// </summary>
        private static List<uint> DecryptFormula(List<uint> rawComps, string name, string desc)
        {
            List<uint> comps = new List<uint>();

            // uint testDescHash = ComputeHash(" – 200");
            uint nameHash = SpellTable.ComputeHash(name);
            uint descHash = SpellTable.ComputeHash(desc);

            uint key = (nameHash % 0x12107680) + (descHash % 0xBEADCF45);

            for (int i = 0; i < rawComps.Count; i++)
            {
                uint comp = (rawComps[i] - key);

                // This seems to correct issues with certain spells with extended characters.
                if (comp > HIGHEST_COMP_ID) // highest comp ID is 198 - "Essence of Kemeroi", for Void Spells
                    comp = comp & 0xFF;

                comps.Add(comp);
            }

            return comps;
        }

        private string spellWords;

        /// <summary>
        /// Not technically part of this function, but saves numerous looks later.
        /// </summary>
        public string GetSpellWords(SpellComponentsTable comps)
        {
            if (spellWords != null)
                return spellWords;

            spellWords = SpellComponentsTable.GetSpellWords(comps, Formula);

            return spellWords;
        }
    }
}
