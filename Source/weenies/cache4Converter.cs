using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melt
{
    public struct sItemInteractionResult
    {
        public int resultId;
        public int unknown1;
        public eSkills skill;
        public int difficulty;
        public int unknown2;
        public int resultWcid;
        public int resultAmount;
        public string successMessage;
        public int unknown3;
        public int unknown4;
        public string failureMessage;

        public sItemInteractionResult(StreamReader inputFile)
        {
            resultId = Utils.readInt32(inputFile);
            unknown1 = Utils.readInt32(inputFile);
            skill = (eSkills)Utils.readInt32(inputFile);
            difficulty = Utils.readInt32(inputFile);
            unknown2 = Utils.readInt32(inputFile);
            resultWcid = Utils.readInt32(inputFile);
            resultAmount = Utils.readInt32(inputFile);
            successMessage = Utils.readString(inputFile);
            unknown3 = Utils.readInt32(inputFile);
            unknown4 = Utils.readInt32(inputFile);
            failureMessage = Utils.readString(inputFile);

            for (int i = 0; i < 163; i++)
            {
                int unknown = Utils.readInt32(inputFile);
            }
        }

        public void writeRaw(StreamWriter outputStream)
        {
            //Utils.writeString(name, outputStream);
            //Utils.writeInt32(repeatTimerSeconds, outputStream);
            //Utils.writeInt32(maxRepetitions, outputStream);
            //Utils.writeEncodedString(description, outputStream);
        }
    }

    public class cCache4Converter
    {
        List<sItemInteractionResult> itemInteractionResults;

        public cCache4Converter()
        {
        }

        public void loadFromRaw(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            Console.WriteLine("Reading item interactions from {0}...", filename);
            Stopwatch timer = new Stopwatch();
            timer.Start();

            itemInteractionResults = new List<sItemInteractionResult>();

            int header1 = Utils.readInt32(inputFile);

            short itemInteractionsCount = 0;
            for (itemInteractionsCount = 0; itemInteractionsCount < 292; itemInteractionsCount++)
            {
                sItemInteractionResult interactionResult = new sItemInteractionResult(inputFile);
                itemInteractionResults.Add(interactionResult);
            }

            int pos = (int)inputFile.BaseStream.Position;

            inputFile.Close();
            timer.Stop();
            Console.WriteLine("{0} item interactions read in {1} seconds.", itemInteractionsCount, timer.ElapsedMilliseconds / 1000f);
        }

        public void loadFromJson(string filename)
        {
            //StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            //Console.WriteLine("Reading timers from {0}...", filename);
            //Stopwatch timer = new Stopwatch();
            //timer.Start();

            //questFlags = new List<sQuestFlag>();

            //StreamReader reader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            //string jsonData = reader.ReadToEnd();

            //JsonSerializerSettings settings = new JsonSerializerSettings();
            //settings.TypeNameHandling = TypeNameHandling.Auto;
            //settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            ////settings.DefaultValueHandling = DefaultValueHandling.Ignore;

            //questFlags = JsonConvert.DeserializeObject<List<sQuestFlag>>(jsonData, settings);

            //inputFile.Close();
            //timer.Stop();
            //Console.WriteLine("{0} timers read in {1} seconds.", questFlags.Count, timer.ElapsedMilliseconds / 1000f);
        }

        public void writeJson(string outputPath)
        {
            //Console.WriteLine("Writing quest flags to json file..");
            //Stopwatch timer = new Stopwatch();
            //timer.Start();
            //JsonSerializerSettings settings = new JsonSerializerSettings();
            //settings.TypeNameHandling = TypeNameHandling.Auto;
            //settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            ////settings.DefaultValueHandling = DefaultValueHandling.Ignore;

            //if (!Directory.Exists(outputPath))
            //    Directory.CreateDirectory(outputPath);

            //string outputFilename = Path.Combine(outputPath, "questFlags.json");
            //StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));

            //string jsonString = JsonConvert.SerializeObject(questFlags, Formatting.Indented, settings);
            //outputFile.Write(jsonString);
            //outputFile.Close();

            //timer.Stop();
            //Console.WriteLine("{0} quest flags written in {1} seconds.", questFlags.Count, timer.ElapsedMilliseconds / 1000f);
        }

        public void writeRaw(string outputPath)
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();
            //Console.WriteLine("Writing \"0008.raw\"");
            //if (!Directory.Exists(outputPath))
            //    Directory.CreateDirectory(outputPath);
            //string outputFilename = Path.Combine(outputPath, "0008.raw");
            //StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));
            //Utils.writeInt16((short)questFlags.Count, outputFile);
            //Utils.writeInt16(32, outputFile); //unknown

            //foreach(sQuestFlag timerEntry in questFlags)
            //{
            //    timerEntry.writeRaw(outputFile);
            //}

            //outputFile.Close();
            //timer.Stop();
            //Console.WriteLine("{0} quest flags written in {1} seconds.", questFlags.Count, timer.ElapsedMilliseconds / 1000f);
        }
    }
}