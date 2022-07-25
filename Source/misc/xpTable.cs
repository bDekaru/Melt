using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Melt
{
    class XPTable
    {
        public uint FileId;

        public List<uint> AttributeXpList;
        public List<uint> VitalXpList;
        public List<uint> TrainedSkillXpList;
        public List<uint> SpecializedSkillXpList;
        public List<ulong> CharacterLevelXPList;
        public List<uint> CharacterLevelSkillCreditList;

        public XPTable(string filename)
        {
            Console.WriteLine("Reading XPTable from {0}...", filename);

            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));

            FileId = Utils.readUInt32(inputFile);

            if (FileId != 0x0E000018)
            {
                Console.WriteLine("Invalid header, aborting.");
                return;
            }

            int attributeCount = Utils.readInt32(inputFile);
            int vitalCount = Utils.readInt32(inputFile);
            int trainedSkillCount = Utils.readInt32(inputFile);
            int specializedSkillCount = Utils.readInt32(inputFile);
            uint levelCount = Utils.readUInt32(inputFile);

            AttributeXpList = new List<uint>();
            for (int i = 0; i <= attributeCount; i++)
                AttributeXpList.Add(Utils.readUInt32(inputFile));

            VitalXpList = new List<uint>();
            for (int i = 0; i <= vitalCount; i++)
                VitalXpList.Add(Utils.readUInt32(inputFile));

            TrainedSkillXpList = new List<uint>();
            for (int i = 0; i <= trainedSkillCount; i++)
                TrainedSkillXpList.Add(Utils.readUInt32(inputFile));

            SpecializedSkillXpList = new List<uint>();
            for (int i = 0; i <= specializedSkillCount; i++)
                SpecializedSkillXpList.Add(Utils.readUInt32(inputFile));

            CharacterLevelXPList = new List<ulong>();
            for (int i = 0; i <= levelCount; i++)
                CharacterLevelXPList.Add(Utils.readUInt64(inputFile));

            CharacterLevelSkillCreditList = new List<uint>();
            for (int i = 0; i <= levelCount; i++)
                CharacterLevelSkillCreditList.Add(Utils.readUInt32(inputFile));

            inputFile.Close();
            Console.WriteLine("Done");
        }

        public void capAtlLevel(int level)
        {
            CharacterLevelXPList.RemoveRange(level + 1, CharacterLevelXPList.Count - level - 1);
            CharacterLevelSkillCreditList.RemoveRange(level + 1, CharacterLevelXPList.Count - level - 1);
        }

        public void save(string filename)
        {
            Console.WriteLine("Saving XPTable to {0}...", filename);

            StreamWriter outputFile = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write));

            Utils.writeUInt32(FileId, outputFile);
            Utils.writeInt32(AttributeXpList.Count - 1, outputFile);
            Utils.writeInt32(VitalXpList.Count - 1, outputFile);
            Utils.writeInt32(TrainedSkillXpList.Count - 1, outputFile);
            Utils.writeInt32(SpecializedSkillXpList.Count - 1, outputFile);
            Utils.writeInt32(CharacterLevelXPList.Count - 1, outputFile);

            foreach (uint entry in AttributeXpList)
                Utils.writeUInt32(entry, outputFile);
            foreach (uint entry in VitalXpList)
                Utils.writeUInt32(entry, outputFile);
            foreach (uint entry in TrainedSkillXpList)
                Utils.writeUInt32(entry, outputFile);
            foreach (uint entry in SpecializedSkillXpList)
                Utils.writeUInt32(entry, outputFile);
            foreach (ulong entry in CharacterLevelXPList)
                Utils.writeUInt64(entry, outputFile);
            foreach (uint entry in CharacterLevelSkillCreditList)
                Utils.writeUInt32(entry, outputFile);

            outputFile.Close();
            Console.WriteLine("Done");
        }
    }
}
