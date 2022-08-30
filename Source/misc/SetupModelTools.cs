using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Instrumentation;
using ACE.DatLoader;
using ACE.DatLoader.FileTypes;

namespace Melt
{
    class SetupModelTools
    {
        public static void CompareObjects(string DMPath)
        {
            StreamWriter outputFile = new StreamWriter(new FileStream("SceneSetupAndGfxObjectsList.txt", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open SceneSetupAndGfxObjectsList.txt");
                return;
            }

            var files = Directory.GetFiles(DMPath);

            List<uint> list = new List<uint>();
            foreach (var file in files)
            {
                if (file.Contains("01000a6f.bin"))
                    continue;

                var eorFile = file.Replace("DM", "EoR");
                StreamReader inputFileEoR = new StreamReader(new FileStream(eorFile, FileMode.Open, FileAccess.Read));
                if (inputFileEoR == null)
                {
                    Console.WriteLine("Unable to open {0}", eorFile);
                    continue;
                }

                StreamReader inputFileDM = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read));
                if (inputFileDM == null)
                {
                    Console.WriteLine("Unable to open {0}", file);
                    continue;
                }

                SetupModel objEoR = null;
                SetupModel objDM = null;

                using (var reader = new BinaryReader(inputFileEoR.BaseStream))
                {
                    objEoR = new SetupModel();
                    objEoR.Unpack(reader);
                }

                using (var reader = new BinaryReader(inputFileDM.BaseStream))
                {
                    objDM = new SetupModel();
                    objDM.Unpack(reader);
                }

                if (objDM.Parts.Count != objEoR.Parts.Count)
                {
                    outputFile.WriteLine($"{objDM.Id.ToString("x8")}");
                    //outputFile.WriteLine($"Export.ExportPortalFile(0x{objDM.Id.ToString("x8")}, path);");
                    foreach (var entry in objDM.Parts)
                    {
                        if (!list.Contains(entry))
                            list.Add(entry);
                        outputFile.WriteLine($"\t{entry.ToString("x8")}");
                    }
                    outputFile.Flush();
                }
                else
                {
                    outputFile.WriteLine($"{objDM.Id.ToString("x8")}");
                    for (int i = 0; i < objDM.Parts.Count; i++)
                    {
                        var entryDM = objDM.Parts[i];
                        var entryEoR = objEoR.Parts[i];

                        if (entryDM != entryEoR)
                        {
                            outputFile.WriteLine($"\tParts mismatch DM: {entryDM.ToString("x8")} EoR: {entryEoR.ToString("x8")}");
                        }
                    }
                }
            }

            outputFile.WriteLine($"---");
            foreach (var entry in list)
            {
                outputFile.WriteLine($"Export.ExportPortalFile(0x{entry.ToString("x8")}, path);");
                //outputFile.WriteLine($"readdat -f \"portal.dat\" {entry.ToString("x8")}");
            }

            outputFile.Close();
            Console.WriteLine("Done");
        }

        public SetupModel Obj = null;

        public SetupModelTools(string filename, bool isToD)
        {
            LoadFromBin(filename, isToD);
        }

        public void LoadFromBin(string filename, bool isToD)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename);
                return;
            }

            using (var reader = new BinaryReader(inputFile.BaseStream))
            {
                Console.WriteLine("Loading SetupModel from binary...");
                Obj = new SetupModel();
                Obj.Unpack(reader, isToD);
                Console.WriteLine("Done");
                return;
            }
        }

        public void SaveToBin(string outputFilename)
        {
            if (Obj == null)
            {
                Console.WriteLine("SetupModel is empty");
                return;
            }

            StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open {0}", outputFilename);
                return;
            }

            Console.WriteLine("Saving SetupModel to binary...");

            //Obj.Pack(outputFile);

            outputFile.Close();
            Console.WriteLine("Done");
        }
    }
}
