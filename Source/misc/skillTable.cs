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

        public void copySkill(string skillName, SkillTable from)
        {
            foreach (KeyValuePair<uint, Skill> entry in from.Skills)
            {
                Skill skill = entry.Value;
                if (skill.Name == skillName)
                {
                    Skills.Add(entry.Key, skill);
                    Console.WriteLine("Done");
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
