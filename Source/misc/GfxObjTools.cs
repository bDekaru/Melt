using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Instrumentation;
using ACE.DatLoader;
using ACE.DatLoader.FileTypes;

namespace Melt
{
    class GfxObjTools
    {
        public static Dictionary<uint, uint> textureIdMigrationTable;
        public static uint TranslateTextureId(uint id)
        {
            if (textureIdMigrationTable == null)
            {
                textureIdMigrationTable = new Dictionary<uint, uint>();

                var inputFile = new StreamReader(new FileStream("./input/gfxObjTextureIdMigrationTable.txt", FileMode.Open, FileAccess.Read));

                while (!inputFile.EndOfStream)
                {
                    string line = inputFile.ReadLine();
                    string[] splitLine = line.Split('\t');

                    if (splitLine.Length >= 2)
                    {
                        uint a = uint.Parse(splitLine[0], System.Globalization.NumberStyles.HexNumber);
                        uint b = uint.Parse(splitLine[1], System.Globalization.NumberStyles.HexNumber);
                        textureIdMigrationTable.Add(a, b);
                    }
                }

                inputFile.Close();
            }

            if (textureIdMigrationTable.ContainsKey(id))
                return textureIdMigrationTable[id];
            else
                return 0;
        }

        public GfxObj Obj = null;

        public GfxObjTools(string filename, bool isToD)
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
                Console.WriteLine("Loading GfxObj from binary...");
                Obj = new GfxObj();
                Obj.Unpack(reader, isToD);

                if (!isToD)
                {
                    List<uint> newSurfaces = new List<uint>();
                    foreach (var entry in Obj.Surfaces)
                    {
                        var newEntry = TranslateTextureId(entry);
                        if (newEntry != 0)
                            newSurfaces.Add(newEntry);
                        else
                            throw new Exception("Missing surface in ToD dat file.");
                    }
                    Obj.Surfaces = newSurfaces;
                }

                Console.WriteLine("Done");
                return;
            }
        }

        public void SaveToBin(string outputFilename)
        {
            if (Obj == null)
            {
                Console.WriteLine("GfxObj is empty");
                return;
            }

            StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open {0}", outputFilename);
                return;
            }

            Console.WriteLine("Saving GfxObj to binary...");

            Obj.Pack(outputFile);

            outputFile.Close();
            Console.WriteLine("Done");
        }

        private static PortalDatDatabase PortalDat = null;
        private static string PortalDatString = "";

        public static void BuildTranslationTable(string portalDatEoR, string SurfaceFilesFolderInfiltration)
        {
            Console.WriteLine("Building gfxObj Translation Table...");

            StreamWriter oututFile = new StreamWriter(new FileStream("gfxObjTextureIdMigrationTable.txt", FileMode.Create, FileAccess.Write));
            if (oututFile == null)
            {
                Console.WriteLine("Unable to open gfxObjTextureIdMigrationTable.txt");
                return;
            }

            for (uint fileIdInf = 0x08000000; fileIdInf < 0x0800FFFF; fileIdInf++)
            {
                var filename = $"{SurfaceFilesFolderInfiltration}/{fileIdInf.ToString("X8")}.bin";

                var fileIdToD = FindTranslation(portalDatEoR, filename);

                if (fileIdToD != 0)
                {
                    oututFile.WriteLine($"{fileIdInf.ToString("X8")}\t{fileIdToD.ToString("X8")}");
                    oututFile.Flush();
                }
            }
            oututFile.Close();
            Console.WriteLine("Done");
        }

        public static uint FindTranslation(string portalDatEoR, string surfaceFilename)
        {
            if (!File.Exists(surfaceFilename))
            {
                //Console.WriteLine($"Failed to find {fileIdInf.ToString("X8")}");
                return 0;
            }

            StreamReader inputFile = new StreamReader(new FileStream(surfaceFilename, FileMode.Open, FileAccess.Read));

            var id = Utils.readUInt32(inputFile); // Pre-ToD Surfaces have the fileId built-in

            if (inputFile == null)
            {
                Console.WriteLine($"Failed to read {surfaceFilename}");
                return 0;
            }

            Surface surfaceEntryInf = null;
            using (var reader = new BinaryReader(inputFile.BaseStream))
            {
                surfaceEntryInf = new Surface();
                surfaceEntryInf.Unpack(reader, false);
                surfaceEntryInf.Id = id;
            }

            if (surfaceEntryInf == null)
            {
                Console.WriteLine($"Failed to load {surfaceFilename}");
                return 0;
            }

            return FindTranslation(portalDatEoR, surfaceEntryInf);
        }

        public static uint FindTranslation(string portalDatEoR, Surface surfaceEntryInf)
        {
            if (surfaceEntryInf == null)
                return 0;

            if (PortalDat == null || PortalDatString != portalDatEoR)
            {
                PortalDat = new PortalDatDatabase(portalDatEoR);
                PortalDatString = portalDatEoR;
            }

            for (uint fileIdTod = 0x08000000; fileIdTod < 0x0800FFFF; fileIdTod++)
            {
                var surfaceEntryToD = PortalDat.ReadFromDat<Surface>(fileIdTod);

                if (surfaceEntryToD == null)
                    continue;

                if (surfaceEntryToD.Type == surfaceEntryInf.Type &&
                    surfaceEntryToD.OrigTextureId == surfaceEntryInf.OrigTextureId &&
                    //surfaceEntryToD.OrigPaletteId == surfaceEntryInf.OrigPaletteId && // Always 0 in ToD
                    surfaceEntryToD.ColorValue == surfaceEntryInf.ColorValue &&
                    surfaceEntryToD.Translucency == surfaceEntryInf.Translucency &&
                    surfaceEntryToD.Luminosity == surfaceEntryInf.Luminosity &&
                    surfaceEntryToD.Diffuse == surfaceEntryInf.Diffuse)
                {
                    return fileIdTod;
                }
            }

            Console.WriteLine($"Unable to find translation for {surfaceEntryInf.Id.ToString("X8")}");
            return 0;
        }

        public static List<uint> FindUsedBy(string portalDatEoR, string surfaceFilename, bool isTod = false)
        {
            if (!File.Exists(surfaceFilename))
            {
                //Console.WriteLine($"Failed to find {fileIdInf.ToString("X8")}");
                return null;
            }

            StreamReader inputFile = new StreamReader(new FileStream(surfaceFilename, FileMode.Open, FileAccess.Read));

            var id = Utils.readUInt32(inputFile); // Pre-ToD Surfaces have the fileId built-in

            if (inputFile == null)
            {
                Console.WriteLine($"Failed to read {surfaceFilename}");
                return null;
            }

            Surface surfaceEntryInf = null;
            using (var reader = new BinaryReader(inputFile.BaseStream))
            {
                surfaceEntryInf = new Surface();
                surfaceEntryInf.Unpack(reader, isTod);
                surfaceEntryInf.Id = id;
            }

            if (surfaceEntryInf == null)
            {
                Console.WriteLine($"Failed to load {surfaceFilename}");
                return null;
            }

            if (!isTod)
            {
                var translatedId = FindTranslation(portalDatEoR, surfaceEntryInf);
                surfaceEntryInf.Id = translatedId;
            }

            return FindUsedBy(portalDatEoR, surfaceEntryInf);
        }

        public static List<uint> FindUsedBy(string portalDatEoR, Surface surfaceEntryInf)
        {
            List<uint> returnList = new List<uint>();
            if (surfaceEntryInf == null)
                return returnList;

            if (PortalDat == null || PortalDatString != portalDatEoR)
            {
                PortalDat = new PortalDatDatabase(portalDatEoR);
                PortalDatString = portalDatEoR;
            }

            for (uint fileIdTod = 0x01000000; fileIdTod < 0x0100FFFF; fileIdTod++)
            {
                var gfxEntry = PortalDat.ReadFromDat<GfxObj>(fileIdTod);

                if (gfxEntry == null)
                    continue;

                foreach (var surfaceEntry in gfxEntry.Surfaces)
                {
                    if (surfaceEntry == surfaceEntryInf.Id)
                    {
                        returnList.Add(gfxEntry.Id);
                    }
                }
            }

            return returnList;
        }
    }
}
