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

            public SkillFormula(byte[] buffer, StreamReader inputFile)
            {
                W = Utils.ReadUInt32(buffer, inputFile);
                X = Utils.ReadUInt32(buffer, inputFile);
                Y = Utils.ReadUInt32(buffer, inputFile);
                Z = Utils.ReadUInt32(buffer, inputFile);
                Attr1 = Utils.ReadUInt32(buffer, inputFile);
                Attr2 = Utils.ReadUInt32(buffer, inputFile);
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

            public Skill(byte[] buffer, StreamReader inputFile)
            {
                Description = Utils.ReadString(buffer, inputFile);
                Name = Utils.ReadString(buffer, inputFile);
                IconId = Utils.ReadUInt32(buffer, inputFile);
                TrainedCost = Utils.ReadUInt32(buffer, inputFile);
                SpecializedCost = Utils.ReadUInt32(buffer, inputFile);
                Category = Utils.ReadInt32(buffer, inputFile);
                CharGenUse = Utils.ReadInt32(buffer, inputFile);
                MinLevel = Utils.ReadInt32(buffer, inputFile);
                Formula = new SkillFormula(buffer, inputFile);
                UpperBound = Utils.ReadDouble(buffer, inputFile);
                LowerBound = Utils.ReadDouble(buffer, inputFile);
                LearnMod = Utils.ReadDouble(buffer, inputFile);
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

            byte[] buffer = new byte[1024];

            FileId = Utils.ReadUInt32(buffer, inputFile);

            if (FileId != 0x0E000004)
            {
                Console.WriteLine("Invalid header, aborting.");
                return;
            }

            ushort count = Utils.ReadUInt16(buffer, inputFile);
            BucketSize = Utils.ReadUInt16(buffer, inputFile);

            Skills = new Dictionary<uint, Skill>();
            uint key = 0;
            for (int i = 0; i < count; i++)
            {
                key = Utils.ReadUInt32(buffer, inputFile);
                Skills.Add(key, new Skill(buffer, inputFile));
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
