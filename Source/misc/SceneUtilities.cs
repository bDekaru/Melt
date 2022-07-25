using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ACE.DatLoader.FileTypes;
using ACE.DatLoader;
using ACE.DatLoader.Entity;

namespace Melt
{
    class SceneUtilities
    {
        public static void CompareObjectLists(List<uint> sceneIdsEoR, List<uint> sceneIdsDM)
        {
            StreamWriter outputFile = new StreamWriter(new FileStream("SceneObjectsTranslationTable.txt", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open SceneObjectsTranslationTable.txt");
                return;
            }

            SortedDictionary<uint, uint> translationMap = new SortedDictionary<uint, uint>();
            foreach (var sceneIdEntry in sceneIdsEoR)
            {
                string filenameEoR = $"./Scene/EoR/{sceneIdEntry.ToString("x8").Replace("0x","")}.bin";
                StreamReader inputFileEoR = new StreamReader(new FileStream(filenameEoR, FileMode.Open, FileAccess.Read));
                if (inputFileEoR == null)
                {
                    Console.WriteLine("Unable to open {0}", filenameEoR);
                    return;
                }
                string filenameDM = $"./Scene/DM/{sceneIdEntry.ToString("x8").Replace("0x", "")}.bin";
                StreamReader inputFileDM = new StreamReader(new FileStream(filenameEoR, FileMode.Open, FileAccess.Read));
                if (inputFileEoR == null)
                {
                    Console.WriteLine("Unable to open {0}", filenameDM);
                    return;
                }

                Scene sceneEoR = new Scene();
                using (var reader = new BinaryReader(inputFileEoR.BaseStream))
                    sceneEoR.Unpack(reader);
                inputFileEoR.Close();

                Scene sceneDM = new Scene();
                using (var reader = new BinaryReader(inputFileDM.BaseStream))
                    sceneDM.Unpack(reader);
                inputFileDM.Close();
                
                if (sceneEoR.Objects.Count != sceneDM.Objects.Count)
                {
                    Console.WriteLine($"SceneId: {sceneEoR.Id}: Object amount mismatch.");
                    continue;
                }
            }

            foreach (var entry in translationMap)
            {
                outputFile.WriteLine($"{entry.Key.ToString("x8")}\t{entry.Value.ToString("x8")}");
                outputFile.Flush();
            }

            outputFile.Close();

            Console.WriteLine("Done");
        }
    }
}