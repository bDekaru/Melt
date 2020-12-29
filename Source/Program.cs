using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Melt
{
    class Program
    {
        //public static cCache4Converter cache4Converter = new cCache4Converter();
        public static cCache6Converter cache6Converter = new cCache6Converter();
        public static cCache8Converter cache8Converter = new cCache8Converter();
        public static cCache9Converter cache9Converter = new cCache9Converter();

        static void Main(string[] args)
        {
            //AceMutationScripts aceMutationScripts = new AceMutationScripts();

            //cache9Converter.loadWeeniesRaw("./input/0009.raw");
            //cache9Converter.performCacheFixes();
            //cache9Converter.writeRaw();
            //Stopwatch timer = new Stopwatch();
            //timer.Start();
            //Process cachePwnProcess = new Process();
            //string path = AppDomain.CurrentDomain.BaseDirectory;
            //cachePwnProcess.StartInfo.FileName = $"{path}CachePwn.exe";
            //cachePwnProcess.StartInfo.Arguments = "2 .\\intermediate .\\output\\";
            //cachePwnProcess.Start();
            //cachePwnProcess.WaitForExit();
            //timer.Stop();
            //Console.WriteLine("Finished in {0} seconds.", timer.ElapsedMilliseconds / 1000f);

            //    cache6Converter.loadFromRaw("./input/0006.raw");
            //    cache6Converter.writeJson("./output/landblocks");
            //    return;

            //SkillTable skillTable = new SkillTable("./Skill Tables/0E000004 - Skills Table - Original.bin");
            //SkillTable skillTable2 = new SkillTable("./Skill Tables/0E000004 - Skills Table - Classic weapon skills.bin");
            //SkillTable skillTable3 = new SkillTable("./Skill Tables/0E000004 - Skills Table - Release.bin");
            //skillTable2.copySkill("Salvaging", skillTable);
            //skillTable2.save("./0E000004 - Skills Table - Classic weapon skills with salvaging.bin");

            //CharGen charGen = new CharGen("./input/0E000002.bin");
            //charGen.modify();
            //charGen.save("./0E000002 - CharGen - Classic.bin");
            //Console.ReadLine();
            //return;

            ////TextureConverter.folderToPNG("textures ToD");
            ////TextureConverter.folderBMPToPNG("C:/Users/Dekaru/Desktop/Research/Textures Retail");
            ////TextureConverter.toPNG("textures ToD/06003afb.bin");
            ////Console.ReadLine();
            ////return;

            ////TextureIdDictionary.folderExtractTextureFromHeader("./input/textureHeaders/");
            ////return;

            //RegionConverter.convert("Region/13000000.bin");
            //RegionConverterDM.convert("Region/130F0000 Bael.bin");
            //RegionConverterDM.convert("Region/130F0000 test.bin");

            //cDatFile portalDatFile = new cDatFile();
            //portalDatFile.loadFromDat("./input/client_portal.dat");
            //int iteration = portalDatFile.GetFileIteration();
            //portalDatFile.SetFileIteration(10000);
            //portalDatFile.writeToDat("client_portal.dat");





            //cDatFile datFile = new cDatFile();
            //datFile.loadFromDat("./input/client_cell_1.dat");
            //datFile.loadFromDat("./input/cell - Late DM - 2004-09-01.dat");
            //datFile.loadFromDat("./client_cell_1.dat");

            cDatFile datFileOld = new cDatFile();
            //datFileOld.loadFromDat("./input/cell - Release.dat");
            //datFileOld.loadFromDat("./input/client_cell_1.dat");
            //datFileOld.loadFromDat("./input/cell - End of Beta Event.dat");
            //datFileOld.loadFromDat("./input/cell - 2005-01-05 (198656kb) (Admin) (Iteration 1583 - Complete).dat");
            datFileOld.loadFromDat("./input/cell - 2005-02-XX (202752kb) (Admin) (Iteration 1593 - Complete).dat");

            datFileOld.convertRetailToToD(10000);

            cDatFile replacementDataFile = null;
            if (datFileOld.isMissingCells)
            {
                Console.WriteLine($"Missing {datFileOld.getMissingCellsList().Count} cells...");
                replacementDataFile = new cDatFile();
                //replacementDataFile.loadFromDat("./input/cell - Release.dat");
                replacementDataFile.loadFromDat("./input/client_cell_1.dat");
                datFileOld.completeCellsFrom(replacementDataFile, true);
                //datFileOld.truncateMissingCells();
            }

            int missingCount;
            if (datFileOld.isMissingLandblocks(out missingCount))
            {
                Console.WriteLine($"Missing {missingCount} landblocks...");
                if (replacementDataFile == null)
                {
                    replacementDataFile = new cDatFile();
                    //replacementDataFile.loadFromDat("./input/cell - Release.dat");
                    replacementDataFile.loadFromDat("./input/client_cell_1.dat");
                }
                datFileOld.completeLandblocksFrom(replacementDataFile);
            }

            datFileOld.writeToDat("./client_cell_1.dat");

            //datFile.replaceDungeon(0x017d, datFileOld, 0);

            //datFile.replaceLandblockArea(0xC98C0028, datFileOld); //Rithwic
            //datFile.replaceLandblockArea(0xC6A90013, datFileOld); //Arwic
            //datFile.replaceLandblockArea(0x856D0040, datFileOld); //Tufa

            //datFile.replaceLandblock(0xC6A90013, datFileOld); //Arwic

            //datFile.removeBuildings(0xC6A90013);
            //datFile.removeBuilding(0xC6A9011A);

            //List<uint> buildingsToRemove = new List<uint>()
            //{
            //    0xA9B30114, //Holtburg->Neydisa Castle Portal Bunker
            //    0xAAB40103, //Holtburg->Dryreach Portal Bunker
            //    0xAAB40108, //Holtburg->Arwic Portal Bunker
            //    0xA9B40188, //Holtburg->Rithwic Portal Bunker
            //    0xA9B4017E, //Holtburg->Cragstone Portal Bunker
            //    0xA9B40183, //Holtburg->Shoushi Portal Bunker
            //};
            //datFile.removeBuildings(buildingsToRemove);

            //datFile.migrateDungeon(datFileOld, 0x017D, 0x017D); //original training academy

            //List<ushort> trainingAcademies = new List<ushort>()
            //{
            //    0x0363,
            //    0x0364,
            //    0x0365,
            //    0x0366,
            //    0x0367,
            //    0x0368
            //};
            //datFile.replaceDungeonList(trainingAcademies, datFileOld);

            //datFile.writeToDat("./client_cell_1.dat");


            //Console.ReadLine();
            //return;

            //cCellDat cellDat = new cCellDat();
            //cellDat.loadFromDat(datFileOld);
            //cMapDrawer mapDrawer = new cMapDrawer(cellDat);
            //mapDrawer.draw();

            //testRandomValueGenerator();
            //Console.ReadLine();
            //return;

            //cache4Converter.loadFromRaw("./input/0004.raw");
            ////cache8Converter.loadFromJson("./output/questFlags.json");
            ////cache8Converter.writeRaw("./output/");
            //cache4Converter.writeJson("./output/");
            //Console.ReadLine();
            //return;

            //testing
            //args = new string[3];
            //args[0] = "cached";
            //args[1] = "./input/";
            //args[2] = "./input/lootProfile.json";
            //testing

            //Patcher.patch();
            //return;

            //SpellsConverter.toTxt("Spells/0E00000E - 2010.bin");
            //SpellsConverter.toTxt("Spells/0E00000E - latest.bin");
            //SpellsConverter.raw0002toTxt("intermediate/0002.raw");
            //return;
            //SpellsConverter.to0002raw("Spells/0E00000E - latest.txt");
            //SpellsConverter.toBin("Spells/0E00000E - Reversed.txt");
            //SpellsConverter.toTxt("Spells/0E00000E - Reversed.bin");
            //SpellsConverter.revertWeaponMasteries("Spells/0E00000E - Latest.txt", "Spells/0E00000E - 2010.txt");
            //SpellsConverter.toBin("0E00000E.txt");
            //SpellsConverter.toBin("Spells/0E00000E - Reversed plus removed auras.txt");
            //SpellsConverter.toJson("Spells/0E00000E - Reversed plus removed auras.bin");
            //SpellsConverter.toJson("Spells/0E00000E - Latest.bin");
            //SpellsConverter.toJson("Spells/0002.raw", "Spells/0E00000E - Reversed plus removed auras.bin");
            //Diff.FolderDiff("E:\\Downloads\\Asheron's Call\\AC Utilities\\Dat Patcher\\All\\Summer", "E:\\Downloads\\Asheron's Call\\AC Utilities\\Dat Patcher\\All\\Winter");
            //TextureConverter.toPNG("06006D40.bin");
            //TextureConverter.darkMajestyfolderToPNG("Landscape Texture Conversion/DM/Textures");
            //TextureConverter.darkMajestyfolderToPNG("Landscape Texture Conversion/DM/Detail Textures");
            //TextureConverter.darkMajestyfolderToPNG("Landscape Texture Conversion/DM/Alpha Maps");
            //TextureConverter.darkMajestyToPNG("Landscape Texture Conversion/DM/Textures/05001c3c.bin");
            //TextureConverter.folderToPNG("Landscape Texture Conversion/ToD/Detail Textures");
            //TextureConverter.folderToPNG("Landscape Texture Conversion/ToD/Textures");
            //TextureConverter.folderToPNG("Landscape Texture Conversion/ToD/Alpha Maps");
            //RegionConverter.convert("Region/13000000.bin");
            //RegionConverterDM.convert("Region/130F0000 Bael.bin");
            //RegionComparer.compare("Region/130F0000 DM.bin", false, "Region/130F0000 Bael.bin", false);
            //TextureHeader.folderExtractTextureFromHeader("Landscape Texture Conversion/ToD/Alpha Maps/Headers");
            //TextureConverter.toBin("Landscape Texture Conversion/ToD/Alpha Maps/06006d6b.png", 0x06006d6b, 244);
            //DMtoToDTexture.convert();
            //cCache9Converter.writeJson("./PhatAC Cache/0009.raw", true);
            //cCache9Converter.writeJson("./input/0009.raw", "./output/weenies/", false);
            //cCache9Converter.writeJson("./PhatAC Cache/0009.raw", true, eWeenieTypes.Creature);
            //ClassNameConverter.convert();

            //cCache9Converter.generateRandomLoot("./PhatAC Cache/0009.raw", "./input/lootProfile2.json", false, true, false, true);//write creature entries to raw
            //cCache9Converter.generateRandomLoot("./PhatAC Cache/0009.raw", "./input/lootProfile2.json", true, false, true, false);//write items to json

            //cCache9Converter.generateRandomLoot("./PhatAC Cache/0009.raw", "./input/lootProfile.json", false, true, false, true);//write creature entries to raw
            //cCache9Converter.generateRandomLoot("./PhatAC Cache/0009.raw", "./input/lootProfile.json", true, false, true, false);//write items to json
            //cCache9Converter.generateRandomLoot("./PhatAC Cache/0009.raw", "./input/lootProfile.json", true, true, true, false);//write everything to json

            //cCache9Converter.generateRandomLoot("../input/0009.raw", "../input/lootProfile.json", "../output/cached/", true, true, false, true);//write everything to raw

            //cCache9Converter.writeExtendedJson("./PhatAC Cache/0009.raw", false);
            //cCache9Converter.writeRawFromExtendedJson("./output/extended weenies/");

            //cCache9Converter.writeRawFromRaw("./PhatAC Cache/0009.raw");

            //cCache9Converter.writeJson("./input/0009.raw", false);
            //cCache9Converter.writeExtendedJson("./input/0009.raw", false);
            //cCache9Converter.writeRawFromExtendedJson("./input/extended weenies/");

            Console.WriteLine("Done");
            Console.ReadLine();
            return;

            //bool invalidArgs = false;

            //if (args.Length == 0)
            //    invalidArgs = true;

            //if (!invalidArgs)
            //{
            //    switch (args[0].ToLower())
            //    {
            //        case "cached":
            //            {
            //                string fileLootProfile;
            //                string file0002;
            //                string file0009;

            //                if (args.Length >= 3)
            //                {
            //                    file0002 = Path.Combine(args[1], "0002.raw");
            //                    file0009 = Path.Combine(args[1], "0009.raw");
            //                    fileLootProfile = args[2];

            //                    if (!File.Exists(fileLootProfile))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", fileLootProfile);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0009))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0009);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0002))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0002);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }

            //                sLootProfile lootProfile = cache9Converter.generateRandomLoot(file0009, file0002, fileLootProfile, "./output/cached/", true, true, false, true);//write everything to raw

            //                Console.WriteLine("Waiting while CachePwn builds cache.bin...");
            //                Stopwatch timer = new Stopwatch();
            //                timer.Start();
            //                Process cachePwnProcess = new Process();
            //                string path = AppDomain.CurrentDomain.BaseDirectory;
            //                cachePwnProcess.StartInfo.FileName = $"{path}CachePwn.exe";
            //                cachePwnProcess.StartInfo.Arguments = "2 .\\intermediate .\\output\\cached";
            //                cachePwnProcess.Start();
            //                cachePwnProcess.WaitForExit();
            //                timer.Stop();
            //                Console.WriteLine("Finished in {0} seconds.", timer.ElapsedMilliseconds / 1000f);

            //                if (lootProfile.otherOptions.copyOutputToFolder != "")
            //                {
            //                    if (!Directory.Exists(lootProfile.otherOptions.copyOutputToFolder))
            //                        Console.WriteLine("Invalid copyOutputToFolder: {0}", lootProfile.otherOptions.copyOutputToFolder);
            //                    else
            //                    {
            //                        Console.WriteLine("Copying output to \"{0}\"...", lootProfile.otherOptions.copyOutputToFolder);
            //                        timer.Reset();
            //                        timer.Start();

            //                        Utils.copyDirectory(".\\output\\cached", lootProfile.otherOptions.copyOutputToFolder, true, true);

            //                        timer.Stop();
            //                        Console.WriteLine("Finished in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            //                    }
            //                }
            //                return;
            //            }
            //        case "split":
            //            {
            //                string fileLootProfile;
            //                string file0002;
            //                string file0009;

            //                if (args.Length >= 3)
            //                {
            //                    file0002 = Path.Combine(args[1], "0002.raw");
            //                    file0009 = Path.Combine(args[1], "0009.raw");
            //                    fileLootProfile = args[2];

            //                    if (!File.Exists(fileLootProfile))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", fileLootProfile);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0009))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0009);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0002))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0002);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }
            //                sLootProfile lootProfile = cache9Converter.generateRandomLoot(file0009, file0002, fileLootProfile, "./output/split/", false, true, false, true);//write creature entries to raw
            //                cache9Converter.generateRandomLoot(file0009, "", fileLootProfile, "./output/split/json/weenies/", true, false, true, false);//write items to json

            //                Console.WriteLine("Waiting while CachePwn builds cache.bin...");
            //                Stopwatch timer = new Stopwatch();
            //                timer.Start();
            //                Process cachePwnProcess = new Process();
            //                string path = AppDomain.CurrentDomain.BaseDirectory;
            //                cachePwnProcess.StartInfo.FileName = $"{path}CachePwn.exe";
            //                cachePwnProcess.StartInfo.Arguments = "2 .\\intermediate .\\output\\split";
            //                cachePwnProcess.Start();
            //                cachePwnProcess.WaitForExit();
            //                timer.Stop();
            //                Console.WriteLine("Finished in {0} seconds.", timer.ElapsedMilliseconds / 1000f);

            //                if (lootProfile.otherOptions.copyOutputToFolder != "")
            //                {
            //                    if (!Directory.Exists(lootProfile.otherOptions.copyOutputToFolder))
            //                        Console.WriteLine("Invalid copyOutputToFolder: {0}", lootProfile.otherOptions.copyOutputToFolder);
            //                    else
            //                    {
            //                        Console.WriteLine("Copying output to \"{0}\"...", lootProfile.otherOptions.copyOutputToFolder);
            //                        timer.Reset();
            //                        timer.Start();

            //                        Utils.copyDirectory(".\\output\\split", lootProfile.otherOptions.copyOutputToFolder, true, true);

            //                        timer.Stop();
            //                        Console.WriteLine("Finished in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            //                    }
            //                }
            //                return;
            //            }
            //        case "json":
            //            {
            //                string fileLootProfile;
            //                string file0002;
            //                string file0009;

            //                if (args.Length >= 3)
            //                {
            //                    file0002 = Path.Combine(args[1], "0002.raw");
            //                    file0009 = Path.Combine(args[1], "0009.raw");
            //                    fileLootProfile = args[2];

            //                    if (!File.Exists(fileLootProfile))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", fileLootProfile);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0009))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0009);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0002))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0002);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }
            //                cache9Converter.generateRandomLoot(file0009, file0002, fileLootProfile, "./output/json/", true, true, true, false);//write everything to json
            //                return;
            //            }
            //        case "loottablesjson":
            //            {
            //                string fileLootProfile;
            //                string file0002;
            //                string file0009;

            //                if (args.Length >= 3)
            //                {
            //                    file0002 = Path.Combine(args[1], "0002.raw");
            //                    file0009 = Path.Combine(args[1], "0009.raw");
            //                    fileLootProfile = args[2];

            //                    if (!File.Exists(fileLootProfile))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", fileLootProfile);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0009))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0009);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0002))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0002);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }
            //                cache9Converter.generateRandomLoot(file0009, file0002, fileLootProfile, "./output/json/", false, true, true, false);//write loot tables to json
            //                return;
            //            }
            //        case "loottables":
            //            {
            //                string fileLootProfile;
            //                string file0002;
            //                string file0009;

            //                if (args.Length >= 3)
            //                {
            //                    file0002 = Path.Combine(args[1], "0002.raw");
            //                    file0009 = Path.Combine(args[1], "0009.raw");
            //                    fileLootProfile = args[2];

            //                    if (!File.Exists(fileLootProfile))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", fileLootProfile);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0009))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0009);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0002))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0002);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }
            //                cache9Converter.generateRandomLoot(file0009, file0002, fileLootProfile, "./output/split/", false, true, false, true);//write loot tables to raw
            //                return;
            //            }
            //        case "weenies":
            //            {
            //                string file0009;

            //                if (args.Length >= 2)
            //                {
            //                    file0009 = Path.Combine(args[1], "0009.raw");

            //                    if (!File.Exists(file0009))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0009);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }
            //                cache9Converter.writeJson(file0009, "./output/weenies/", false);
            //                return;
            //            }
            //        case "landblocks":
            //            {
            //                string file0006;
            //                string file0009;

            //                if (args.Length >= 2)
            //                {
            //                    file0006 = Path.Combine(args[1], "0006.raw");
            //                    file0009 = Path.Combine(args[1], "0009.raw");

            //                    if (!File.Exists(file0006))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0006);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0009))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0009);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }
            //                cache9Converter.loadWeeniesRaw(file0009);
            //                cache6Converter.loadFromRaw(file0006);
            //                cache6Converter.writeJson("./output/landblocks");
            //                return;
            //            }
            //        case "questflags":
            //            {
            //                string file0008;

            //                if (args.Length >= 2)
            //                {
            //                    file0008 = Path.Combine(args[1], "0008.raw");

            //                    if (!File.Exists(file0008))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0008);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }
            //                cache8Converter.loadFromRaw(file0008);
            //                cache8Converter.writeJson("./output");
            //                return;
            //            }
            //        case "raw2json":
            //            {
            //                string file0009;

            //                if (args.Length >= 2)
            //                {
            //                    file0009 = Path.Combine(args[1], "0009.raw");

            //                    if (!File.Exists(file0009))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0009);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }
            //                cache9Converter.writeExtendedJson(file0009, "./output/extended weenies/", false);
            //                return;
            //            }
            //        case "json2raw":
            //            {
            //                string folderExtendedWeenies;

            //                if (args.Length >= 2)
            //                {
            //                    folderExtendedWeenies = args[1];

            //                    if (!Directory.Exists(folderExtendedWeenies))
            //                    {
            //                        Console.WriteLine("Invalid folder: {0}", folderExtendedWeenies);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }

            //                cache9Converter.writeRawFromExtendedJson(folderExtendedWeenies);

            //                Console.WriteLine("Waiting while CachePwn builds cache.bin...");
            //                Stopwatch timer = new Stopwatch();
            //                timer.Start();
            //                Process cachePwnProcess = new Process();
            //                string path = AppDomain.CurrentDomain.BaseDirectory;
            //                cachePwnProcess.StartInfo.FileName = $"{path}CachePwn.exe";
            //                cachePwnProcess.StartInfo.Arguments = "2 .\\intermediate \".\\output\\extended weenies\"";
            //                cachePwnProcess.Start();
            //                cachePwnProcess.WaitForExit();
            //                timer.Stop();
            //                Console.WriteLine("Finished in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            //                return;
            //            }
            //    }
            //}

            //if (invalidArgs)
            //{
            //    Console.WriteLine("Invalid arguments.");
            //    Console.WriteLine("Valid Modes:");
            //    Console.WriteLine("    Cached: produces a cache.bin file that contains both the generated loot and modified creatures and chests entries. Due to a limitation this file can only store up to 26550 new entries.");
            //    Console.WriteLine("        Usage: (mode) (inputFolder) (profileFile)");
            //    Console.WriteLine("    Split: produces a cache.bin file that contains the modified creatures and chests entries, and json files for the generated loot. Thus avoiding the limitation mentioned above but increasing server starting times considerably.");
            //    Console.WriteLine("        Usage: (mode) (inputFolder) (profileFile)");
            //    Console.WriteLine("    lootTables");
            //    Console.WriteLine("        Usage: (mode) (inputFolder) (profileFile)");
            //    Console.WriteLine("    Json: no cache.bin is produced, instead all modified entries are created as json.");
            //    Console.WriteLine("        Usage: (mode) (inputFolder) (profileFile)");
            //    Console.WriteLine("    lootTablesJson");
            //    Console.WriteLine("        Usage: (mode) (inputFolder) (profileFile)");
            //    Console.WriteLine("    raw2json: generate extended weenies that can later be reconverted into cached files.");
            //    Console.WriteLine("        Usage: (mode) (inputFolder)");
            //    Console.WriteLine("    json2raw: generate cache.bin file from extended weenies");
            //    Console.WriteLine("        Usage: (mode) (inputFolder)");
            //    Console.WriteLine("    Weenies: generate commented weenies files.");
            //    Console.WriteLine("        Usage: (mode) (inputFolder)");
            //    Console.WriteLine("    Landblocks: generate landblock files.");
            //    Console.WriteLine("        Usage: (mode) (inputFolder)");
            //    Console.ReadLine();
            //}
        }

        static void testRandomValueGenerator()
        {
            int testRolls = 100000;

            //double[] values =
            //{
            //    0.4423077,
            //    0.3846154,
            //    0.0384615,
            //    0.0192308,
            //    0.0769231,
            //    0.0384615,
            // };

            //SortedDictionary<int, int> valueDistribution = new SortedDictionary<int, int>();

            //valueDistribution.Add(1, (int)(values[0] * 1000));
            //valueDistribution.Add(2, (int)(values[1] * 1000));
            //valueDistribution.Add(3, (int)(values[2] * 1000));
            //valueDistribution.Add(4, (int)(values[3] * 1000));
            //valueDistribution.Add(5, (int)(values[4] * 1000));
            //valueDistribution.Add(6, (int)(values[5] * 1000));

            SortedDictionary<int, int> valueDistribution = new SortedDictionary<int, int>();
            for (int i = 0; i < testRolls; i++)
            {
                int maxRank = 10;
                float favoredRank = (float)Math.Ceiling(maxRank * 0.4f);
                favoredRank = Math.Max(favoredRank, 1);
                int test = Utils.getRandomNumber(1, maxRank, eRandomFormula.favorSpecificValue, favoredRank, 3, 0);

                //double preferredValue = 0;
                //int test = Utils.getRandomNumberExclusive(100);
                //int test = (int)Math.Floor(Utils.getRandomDouble(0.5, 4.0, eRandomFormula.favorLow, 1.5) * 10);
                //int test = Utils.getRandomNumber(5, 6, eRandomFormula.favorSpecificValue, 5, 4);
                //int test = Utils.getRandomNumber(5, 6, eRandomFormula.favorSpecificValue, preferredValue, 1.8d);
                //int test = Utils.getRandomNumber(1, 10, eRandomFormula.favorSpecificValue, 4, 3);
                //int test = Utils.getRandomNumber(1, eRandomFormula.favorLow, 2d);
                //int test = Utils.getRandomNumber(1, eRandomFormula.favorLow, 2d);
                //int test = Utils.getRandomNumber(1, 2, eRandomFormula.favorSpecificValue, 1.2, 1.8d);
                //int test = Utils.getRandomNumberExclusive(10, eRandomFormula.equalDistribution);
                //int test = (int)Math.Round(Utils.getRandomDouble(0.7d, 1.0d, eRandomFormula.favorMid, 1.4d)*100);
                if (valueDistribution.ContainsKey(test))
                    valueDistribution[test]++;
                else
                    valueDistribution.Add(test, 1);
            }

            Console.WriteLine("Raw:");
            foreach (KeyValuePair<int, int> entry in valueDistribution)
            {
                Console.WriteLine($"value: {entry.Key} amount: {entry.Value} percent: {entry.Value * 100d / testRolls}%");
            }
            Console.WriteLine("---");
            Console.WriteLine("Rounded:");
            SortedDictionary<int, int> valueDistributionRounded = Utils.DistributionRounding(valueDistribution);
            foreach (KeyValuePair<int, int> entry in valueDistributionRounded)
            {
                Console.WriteLine($"value: {entry.Key} amount: {entry.Value} percent: {entry.Value * 100d / testRolls}%");
            }
            Console.WriteLine("---");
        }
    }
}
