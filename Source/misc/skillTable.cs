using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Melt
{
    class SkillTable
    {
        public class SkillFormula
        {
            public uint W;
            public uint X;
            public uint Y;
            public uint Z;
            public uint Attr1;
            public uint Attr2;

            public SkillFormula(StreamReader inputFile)
            {
                W = Utils.readUInt32(inputFile);
                X = Utils.readUInt32(inputFile);
                Y = Utils.readUInt32(inputFile);
                Z = Utils.readUInt32(inputFile);
                Attr1 = Utils.readUInt32(inputFile);
                Attr2 = Utils.readUInt32(inputFile);
            }

            public void writeRaw(StreamWriter outputStream)
            {
                Utils.writeUInt32(W, outputStream);
                Utils.writeUInt32(X, outputStream);
                Utils.writeUInt32(Y, outputStream);
                Utils.writeUInt32(Z, outputStream);
                Utils.writeUInt32(Attr1, outputStream);
                Utils.writeUInt32(Attr2, outputStream);
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
                }
            }
            if (found)
            {
                Skills.Remove(key);
                Console.WriteLine("Skill removed.");
            }
            else
                Console.WriteLine($"Couldn't find {skillName} skill to remove.");
        }

        public void modifyForCustomDM(SkillTable skillTableLatest)
        {
            copySkill("Salvaging", skillTableLatest);
            copySkill("Shield", skillTableLatest);
            removeSkill("Item Enchantment");
            removeSkill("Creature Enchantment");
            removeSkill("Mace");
            removeSkill("Staff");
            removeSkill("Crossbow");

            Skills[(uint)eSkills.Salvaging].TrainedCost = 2;
            Skills[(uint)eSkills.Salvaging].SpecializedCost = 1001;
            Skills[(uint)eSkills.Salvaging].Formula = Skills[(uint)eSkills.ItemAppraisal].Formula;

            Skills[(uint)eSkills.Sword].TrainedCost = 6;
            Skills[(uint)eSkills.Sword].SpecializedCost = 12;
            Skills[(uint)eSkills.Sword].Description = $"Bonus damage source: Coordination\n{Skills[(uint)eSkills.Sword].Description}";

            Skills[(uint)eSkills.Dagger].Description = $"Bonus damage source: Coordination\n{Skills[(uint)eSkills.Dagger].Description}";

            Skills[(uint)eSkills.Axe].Name = "Axe and Mace";
            Skills[(uint)eSkills.Axe].Description = "Bonus damage source: Strength\nHelps you wield axes, hammers, maces, clubs and similar weapons.";

            Skills[(uint)eSkills.Spear].Name = "Spear and Staff";
            Skills[(uint)eSkills.Spear].Description = "Bonus damage source: Coordination\nHelps you wield spears, staffs and similar weapons.";
            Skills[(uint)eSkills.Spear].TrainedCost = 6;
            Skills[(uint)eSkills.Spear].SpecializedCost = 12;
            Skills[(uint)eSkills.Spear].Formula = Skills[(uint)eSkills.Dagger].Formula;

            Skills[(uint)eSkills.UnarmedCombat].TrainedCost = 4;
            Skills[(uint)eSkills.UnarmedCombat].SpecializedCost = 8;
            Skills[(uint)eSkills.UnarmedCombat].Description = $"Bonus damage source: Unarmed Combat Skill\n{Skills[(uint)eSkills.UnarmedCombat].Description}";

            Skills[(uint)eSkills.Bow].Name = "Bow and Crossbow";
            Skills[(uint)eSkills.Bow].Description = "Bonus damage source: Coordination\nHelps you fire bows, crossbows and similar weapons.";
            Skills[(uint)eSkills.Bow].TrainedCost = 6;
            Skills[(uint)eSkills.Bow].SpecializedCost = 12;

            Skills[(uint)eSkills.ThrownWeapon].Description = $"Bonus damage source: Strength\n{Skills[(uint)eSkills.ThrownWeapon].Description}";

            Skills[(uint)eSkills.Alchemy].TrainedCost = 8;
            Skills[(uint)eSkills.Alchemy].SpecializedCost = 16;

            Skills[(uint)eSkills.CreatureAppraisal].SpecializedCost = 8;
        }

        public void copySkill(string skillName, SkillTable from)
        {
            foreach (KeyValuePair<uint, Skill> entry in from.Skills)
            {
                Skill skill = entry.Value;
                if (skill.Name == skillName)
                {
                    Skills.Add(entry.Key, skill);
                    Console.WriteLine("Skill copied.");
                    return;
                }
            }
            Console.WriteLine($"Couldn't find {skillName} skill to copy.");
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
