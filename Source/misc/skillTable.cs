using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ACE.DatLoader;
using ACE.DatLoader.FileTypes;
using System.Security.Cryptography;
using ACE.DatLoader.Entity;
using ACE.Entity.Enum;

namespace Melt
{
    class SkillTable
    {
        public class SkillFormula
        {
            public uint TotalAddition;
            public uint Attribute1Multiplier;
            public uint Attribute2Multiplier;
            public uint TotalDivisor;
            public uint Attribute1;
            public uint Attribute2;

            public SkillFormula(eAttributes attribute1, uint attribute1Multiplier, eAttributes attribute2, uint attribute2Multiplier, uint totalDivisor, uint totalAddition)
            {
                TotalAddition = totalAddition;
                Attribute1Multiplier = attribute1Multiplier;
                Attribute2Multiplier = attribute2Multiplier;
                TotalDivisor = totalDivisor;
                Attribute1 = (uint)attribute1;
                Attribute2 = (uint)attribute2;
            }

            public SkillFormula(StreamReader inputFile)
            {
                TotalAddition = Utils.readUInt32(inputFile);
                Attribute1Multiplier = Utils.readUInt32(inputFile);
                Attribute2Multiplier = Utils.readUInt32(inputFile);
                TotalDivisor = Utils.readUInt32(inputFile);
                Attribute1 = Utils.readUInt32(inputFile);
                Attribute2 = Utils.readUInt32(inputFile);
            }

            public void writeRaw(StreamWriter outputStream)
            {
                Utils.writeUInt32(TotalAddition, outputStream);
                Utils.writeUInt32(Attribute1Multiplier, outputStream);
                Utils.writeUInt32(Attribute2Multiplier, outputStream);
                Utils.writeUInt32(TotalDivisor, outputStream);
                Utils.writeUInt32(Attribute1, outputStream);
                Utils.writeUInt32(Attribute2, outputStream);
            }
        }

        public class Skill
        {
            public string Description;
            public string Name;
            public uint IconId;
            public uint TrainedCost;
            public uint SpecializedCost;
            public int Category;
            public int CharGenUse;
            public int MinLevel;
            public SkillFormula Formula;
            public Double UpperBound;
            public Double LowerBound;
            public Double LearnMod;

            public Skill()
            {

            }

            public Skill(StreamReader inputFile)
            {
                Description = Utils.readString(inputFile);
                Name = Utils.readString(inputFile);
                IconId = Utils.readUInt32(inputFile);
                TrainedCost = Utils.readUInt32(inputFile);
                SpecializedCost = Utils.readUInt32(inputFile);
                Category = Utils.readInt32(inputFile);
                CharGenUse = Utils.readInt32(inputFile);
                MinLevel = Utils.readInt32(inputFile);
                Formula = new SkillFormula(inputFile);
                UpperBound = Utils.readDouble(inputFile);
                LowerBound = Utils.readDouble(inputFile);
                LearnMod = Utils.readDouble(inputFile);
            }

            public void writeRaw(StreamWriter outputStream)
            {
                Utils.writeString(Description, outputStream);
                Utils.writeString(Name, outputStream);
                Utils.writeUInt32(IconId,outputStream);
                Utils.writeUInt32(TrainedCost, outputStream);
                Utils.writeUInt32(SpecializedCost, outputStream);
                Utils.writeInt32(Category, outputStream);
                Utils.writeInt32(CharGenUse, outputStream);
                Utils.writeInt32(MinLevel, outputStream);
                Formula.writeRaw(outputStream);
                Utils.writeDouble(UpperBound, outputStream);
                Utils.writeDouble(LowerBound, outputStream);
                Utils.writeDouble(LearnMod, outputStream);
            }
        }

        public uint FileId;
        public ushort BucketSize;
        public Dictionary<uint, Skill> Skills;

        public SkillTable(string filename)
        {
            Console.WriteLine("Reading skillTable from {0}...", filename);

            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));

            FileId = Utils.readUInt32(inputFile);

            if (FileId != 0x0E000004)
            {
                Console.WriteLine("Invalid header, aborting.");
                return;
            }

            ushort count = Utils.readUInt16(inputFile);
            BucketSize = Utils.readUInt16(inputFile);

            Skills = new Dictionary<uint, Skill>();
            uint key = 0;
            for (int i = 0; i < count; i++)
            {
                key = Utils.readUInt32(inputFile);
                Skills.Add(key, new Skill(inputFile));
            }

            inputFile.Close();
            Console.WriteLine("Done");
        }

        public void removeSkill(string skillName)
        {
            bool found = false;
            uint key = 0;
            foreach (KeyValuePair<uint, Skill> entry in Skills)
            {
                Skill skill = entry.Value;
                if (skill.Name == skillName)
                {
                    key = entry.Key;
                    found = true;
                    break;
                }
            }
            if (found)
            {
                Skills.Remove(key);
                Console.WriteLine($"Skill {skillName} removed.");
            }
            else
                Console.WriteLine($"Couldn't find {skillName} skill to remove.");
        }

        public void copySkill(string skillName, SkillTable from, uint newId = 0)
        {
            foreach (KeyValuePair<uint, Skill> entry in from.Skills)
            {
                Skill skill = entry.Value;
                if (skill.Name.Replace("\0", "") == skillName)
                {
                    skill.Name = skill.Name.Replace("\0", "");
                    skill.Description = skill.Description.Replace("\0", "");
                    if (newId == 0)
                        Skills.Add(entry.Key, skill);
                    else
                        Skills.Add(newId, skill);
                    Console.WriteLine($"Skill {skillName} copied.");
                    return;
                }
            }
            Console.WriteLine($"Couldn't find {skillName} skill to copy.");
        }

        public bool addSkill(uint id, uint iconId, string name, string description, eSkillCategory category, eSkillMinLevel minLevel, uint trainedCost, uint specializedCost, SkillFormula formula)
        {
            if (Skills.ContainsKey(id))
            {
                Console.WriteLine($"Skill id {id} already exists.");
                return false;
            }

            var newSkill = new Skill();
            newSkill.IconId = iconId;
            newSkill.Name = name;
            newSkill.Description = description;
            newSkill.Category = (int)category;
            newSkill.MinLevel = (int)minLevel;
            newSkill.TrainedCost = trainedCost;
            newSkill.SpecializedCost = specializedCost;
            newSkill.Formula = formula;

            newSkill.CharGenUse = 1;
            newSkill.LearnMod = 1;
            newSkill.LowerBound = 120;
            newSkill.UpperBound = 900;

            Skills.Add(id, newSkill);
            Console.WriteLine($"Skill {name} added.");
            return true;
        }

        public void modifyForCustomDM(SkillTable skillTableLatest, SkillTable skillTableAssess)
        {
            copySkill("Salvaging", skillTableLatest);
            copySkill("Shield", skillTableLatest);
            copySkill("Dual Wield", skillTableLatest);

            //copySkill("Appraise Item", skillTableAssess, 55);
            //copySkill("Appraise Weapon", skillTableAssess, 56);
            //copySkill("Appraise Armor", skillTableAssess, 57);
            //copySkill("Appraise Magic Item", skillTableAssess, 58);

            removeSkill("Item Enchantment");
            removeSkill("Creature Enchantment");
            removeSkill("Mace");
            removeSkill("Staff");
            removeSkill("Crossbow");

            //addSkill((uint)eSkills.SpellCraft, 0x060004db, "Spellcraft", "Lets you transfer spells between items.", eSkillCategory.Magic, eSkillMinLevel.Unusable, 4, 8, new SkillFormula(eAttributes.Coordination, 1, eAttributes.Focus, 1, 3, 0));
            addSkill((uint)eSkills.SpellCraft, 0x060004db, "Spellcraft", "Lets you transfer spells between items.", eSkillCategory.Magic, eSkillMinLevel.Unusable, 999, 999, new SkillFormula(eAttributes.Coordination, 1, eAttributes.Focus, 1, 3, 0));
            addSkill((uint)eSkills.Awareness, 0x060004c8, "Awareness", "Helps you see traps and other hidden objects.", eSkillCategory.NonWeapon, eSkillMinLevel.Untrained, 4, 8, new SkillFormula(eAttributes.Coordination, 1, eAttributes.Focus, 1, 3, 0));
            //addSkill((uint)eSkills.ArmsAndArmorRepair, 0x060004c6, "Arms and Armor Repair", "Lets you improve arms and armor condition.", eSkillCategory.NonWeapon, eSkillMinLevel.Unusable, 4, 8, new SkillFormula(eAttributes.Coordination, 1, eAttributes.Focus, 1, 3, 0));
            addSkill((uint)eSkills.ArmsAndArmorRepair, 0x060004c6, "Arms and Armor Repair", "Lets you improve arms and armor condition.", eSkillCategory.NonWeapon, eSkillMinLevel.Unusable, 999, 999, new SkillFormula(eAttributes.Coordination, 1, eAttributes.Focus, 1, 3, 0));
            addSkill(55, 0x0600209b, "Appraise Missile Item", "Helps you evaluate the value of ranged weapons.", eSkillCategory.NonWeapon, eSkillMinLevel.Unusable, 2, 4, new SkillFormula(eAttributes.Focus, 1, eAttributes.None, 0, 1, 0));
            addSkill(56, 0x0600209d, "Appraise Melee Item", "Helps you evaluate the value of close-quarters weapons.", eSkillCategory.NonWeapon, eSkillMinLevel.Unusable, 2, 4, new SkillFormula(eAttributes.Focus, 1, eAttributes.None, 0, 1, 0));
            addSkill(57, 0x0600209a, "Appraise Armor", "Helps you evaluate the value of armor and clothing.", eSkillCategory.NonWeapon, eSkillMinLevel.Unusable, 2, 4, new SkillFormula(eAttributes.Focus, 1, eAttributes.None, 0, 1, 0));
            addSkill(58, 0x0600209c, "Appraise Caster Item", "Helps you evaluate the value of casting implements, gems and jewelry.", eSkillCategory.NonWeapon, eSkillMinLevel.Unusable, 2, 4, new SkillFormula(eAttributes.Focus, 1, eAttributes.None, 0, 1, 0));
            addSkill(59, 0x060004ca, "Armor", "Helps you wield armor.", eSkillCategory.NonWeapon, eSkillMinLevel.Untrained, 2, 4, new SkillFormula(eAttributes.Strength, 1, eAttributes.Coordination, 1, 2, 0));
            addSkill(60, 0x060004da, "Sneaking", "Helps you walk past enemies without being noticed and past traps without triggering them.", eSkillCategory.NonWeapon, eSkillMinLevel.Untrained, 4, 8, new SkillFormula(eAttributes.Coordination, 1, eAttributes.Focus, 1, 3, 0));

            Skills[(uint)eSkills.Shield].MinLevel = (int)eSkillMinLevel.Unusable;
            Skills[(uint)eSkills.DualWield].MinLevel = (int)eSkillMinLevel.Unusable;

            Skills[(uint)eSkills.Salvaging].TrainedCost = 2;
            Skills[(uint)eSkills.Salvaging].SpecializedCost = 1001;
            Skills[(uint)eSkills.Salvaging].Formula = Skills[(uint)eSkills.ItemAppraisal].Formula;

            Skills[(uint)eSkills.Sword].TrainedCost = 6;
            Skills[(uint)eSkills.Sword].SpecializedCost = 12;

            Skills[(uint)eSkills.Axe].Name = "Axe and Mace";
            Skills[(uint)eSkills.Axe].Formula = new SkillFormula(eAttributes.Strength, 4, eAttributes.Coordination, 2, 9, 0);

            Skills[(uint)eSkills.Spear].Name = "Spear and Staff";
            Skills[(uint)eSkills.Spear].TrainedCost = 6;
            Skills[(uint)eSkills.Spear].SpecializedCost = 12;
            Skills[(uint)eSkills.Spear].Formula = new SkillFormula(eAttributes.Coordination, 4, eAttributes.Quickness, 2, 9, 0);

            Skills[(uint)eSkills.UnarmedCombat].TrainedCost = 4;
            Skills[(uint)eSkills.UnarmedCombat].SpecializedCost = 8;
            Skills[(uint)eSkills.UnarmedCombat].Formula = new SkillFormula(eAttributes.Coordination, 1, eAttributes.Focus, 1, 3, 0);

            Skills[(uint)eSkills.Bow].Name = "Bow and Crossbow";
            Skills[(uint)eSkills.Bow].TrainedCost = 6;
            Skills[(uint)eSkills.Bow].SpecializedCost = 12;

            Skills[(uint)eSkills.ThrownWeapon].Formula = new SkillFormula(eAttributes.Strength, 1, eAttributes.Coordination, 1, 4, 0);

            Skills[(uint)eSkills.Alchemy].TrainedCost = 8;
            Skills[(uint)eSkills.Alchemy].SpecializedCost = 16;

            Skills[(uint)eSkills.CreatureAppraisal].SpecializedCost = 8;

            Skills[(uint)eSkills.Axe                ].Description = "Helps you wield axes, hammers, maces, clubs and similar weapons.";
            Skills[(uint)eSkills.Bow                ].Description = "Helps you fire bows, crossbows and similar weapons.";
            Skills[(uint)eSkills.CreatureAppraisal  ].Description = "Helps you figure out creatures' attributes and vulnerabilities.";
            Skills[(uint)eSkills.Deception          ].Description = "Helps prevent others from seeing your attributes and vulnerabilities and to taunt opponents.";
            Skills[(uint)eSkills.PersonalAppraisal  ].Description = "Helps you figure out humans' attributes and vulnerabilities.";
            Skills[(uint)eSkills.Spear              ].Description = "Helps you wield spears, staffs and similar weapons.";

            foreach(var entry in Skills)
            {
                entry.Value.Description += $"\n\nFormula : {buildFormulaString(entry.Value.Formula)}";
            }

            Skills[(uint)eSkills.Axe            ].Description += "\nDamage : Strength";
            Skills[(uint)eSkills.Bow            ].Description += "\nDamage : Coordination";
            Skills[(uint)eSkills.Dagger         ].Description += "\nDamage : Coordination";
            Skills[(uint)eSkills.Spear          ].Description += "\nDamage : Coordination";
            Skills[(uint)eSkills.Sword          ].Description += "\nDamage : Strength";
            Skills[(uint)eSkills.ThrownWeapon   ].Description += "\nDamage : Strength";
            Skills[(uint)eSkills.UnarmedCombat  ].Description += "\nDamage : Unarmed Combat";
        }

        public string buildFormulaString(SkillFormula formula)
        {
            string formulaString = "";
            var attribute1Present = formula.Attribute1 != 0;
            var attribute2Present = formula.Attribute2 != 0;

            if (!attribute1Present && !attribute2Present)
                return formulaString;

            string attribute1String = "";
            if (attribute1Present)
            {
                if (formula.Attribute1Multiplier > 1)
                {
                    attribute1String = $"{formula.Attribute1Multiplier} x {(eAttributes)formula.Attribute1}";
                    if (attribute2Present)
                        attribute1String = $"({attribute1String})";
                }
                else
                    attribute1String = $"{(eAttributes)formula.Attribute1}";
            }

            string attribute2String = "";
            if (attribute2Present)
            {
                if (formula.Attribute2Multiplier > 1)
                {
                    attribute2String = $"{formula.Attribute2Multiplier} x {(eAttributes)formula.Attribute2}";
                    if (attribute1Present)
                        attribute2String = $"({attribute2String})";
                }
                else
                    attribute2String = $"{(eAttributes)formula.Attribute2}";
            }

            if (attribute1Present && attribute2Present)
                formulaString = $"{attribute1String} + {attribute2String}";
            else if (attribute1Present)
                formulaString = attribute1String;
            else
                formulaString = attribute2String;

            if (formula.TotalDivisor > 1)
            {
                if(attribute1Present && attribute2Present)
                    formulaString = $"({formulaString})";
                formulaString = $"{formulaString} / {formula.TotalDivisor}";
            }

            if (formula.TotalAddition > 0)
                formulaString = $"({formulaString}) + {formula.TotalAddition}";

            return formulaString;
        }

        public void save(string filename)
        {
            Console.WriteLine("Saving skillTable to {0}...", filename);

            StreamWriter outputFile = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write));

            Utils.writeUInt32(FileId, outputFile);
            Utils.writeUInt16((ushort)Skills.Count, outputFile);
            Utils.writeUInt16(BucketSize, outputFile);

            foreach (KeyValuePair<uint, Skill> entry in Skills)
            {
                Utils.writeUInt32(entry.Key, outputFile);
                entry.Value.writeRaw(outputFile);
            }

            outputFile.Close();
            Console.WriteLine("Done");
        }
    }
}
