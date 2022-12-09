using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using ACE.DatLoader;
using ACE.DatLoader.FileTypes;
using System.Linq;

namespace Melt
{
    class Program
    {
        static void Main(string[] args)
        {
            //GeneratePhatACLootFiles(args);

            //MapManipulationForEoR(args);
            //PortalManipulationForEoR(args);

            //MapManipulationForCustomDM(args);
            //PortalManipulationForCustomDM(args);
            //LanguageManipulationForCustomDM(args);

            //MapManipulationForInfiltration(args);
            //PortalManipulationForInfiltration(args);
            //LanguageManipulationForInfiltration(args);

            //MapManipulationForInfiltrationAceClassicCustom(args);
            //PortalManipulationForInfiltrationAceClassicCustom(args);

            //MapManipulationForEvensong(args);
            //PortalManipulationForEvensong(args);

            Workbench(args);

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        //public static cCache4Converter cache4Converter = new cCache4Converter();
        public static cCache6Converter cache6Converter = new cCache6Converter();
        public static cCache8Converter cache8Converter = new cCache8Converter();
        public static cCache9Converter cache9Converter = new cCache9Converter();
        static void Workbench(string[] args)
        {
            //AceDatabaseUtilities.CreateFoodValueList();

            //var di = new DirectoryInfo("./Scene/DM");
            //var files = di.GetFiles("*.bin");
            //var output = new List<string>();

            //foreach(var file in files)
            //{
            //    output.Add($"Export.ExportPortalFile(0x{file.Name.Replace(".bin", "")}, path);");
            //}

            //File.WriteAllLines("./scenes.txt", output);

            //PlayScriptTools playScript = new PlayScriptTools();
            //playScript.LoadTableFromBin("./34000004 Original.bin");
            ////playScript.LoadScriptFromBin("./33000233 Original.bin");
            //playScript.AddSneakScripts();

            //playScript.SaveTableToBin("./34000004.bin");
            //playScript.SaveScriptToBin("./330013A0.bin");
            //playScript.SaveScript2ToBin("./330013A1.bin");

            //playScript.LoadScriptFromBin("./330013A0.bin");

            //cDatFile portalDatFile = new cDatFile();
            //portalDatFile.loadFromDat("./input/client_portal - EoR.dat");
            ////portalDatFile.copyFile(0x06006d3c, 0x06006d32);
            ////portalDatFile.copyFile(0x33000233, 0x330013A0);
            ////portalDatFile.copyFile(0x33000233, 0x330013A1);
            //portalDatFile.addFile("./330013A0.bin");
            //portalDatFile.writeToDat("./client_portal.dat");

            //GfxObjTools.BuildTranslationTable("./input/client_portal - EoR.dat", "./input/Surface Infiltration/");
            //GfxObjTools.FindUsedBy("./input/client_portal - EoR.dat", "./080008ed.bin");
            //GfxObjTools.FindTranslation("./input/client_portal - EoR.dat", "./08000aa2.bin");
            //GfxObjTools.FindTranslation("./input/client_portal - EoR.dat", "./08000aa3.bin");
            //GfxObjTools.FindTranslation("./input/client_portal - EoR.dat", "./080008ed.bin");

            //SetupModelTools.CompareObjects("./Scene/Objects/DM");

            //GfxObjTools gfxObj = new GfxObjTools("./input/01000CFA.bin", false);
            //gfxObj.SaveToBin("./01000CFA.bin");
            //gfxObj = new GfxObjTools("./input/01000D00.bin", false);
            //gfxObj.SaveToBin("./01000D00.bin");
            //gfxObj = new GfxObjTools("./input/01000D02.bin", false);
            //gfxObj.SaveToBin("./01000D02.bin");
            //gfxObj = new GfxObjTools("./input/01000D06.bin", false);
            //gfxObj.SaveToBin("./01000D06.bin");

            //cache9Converter.loadWeeniesRaw("./input/0009.raw");
            //cache9Converter.generateChestTreasureList();
            //cache9Converter.generateTreasureList();

            //Armor Buff
            //TextureConverter.toPNG("06001385.bin");
            //TextureConverter.toBin("06001385.png", 0x06020001, 21);
            //TextureConverter.toPNG("06007105.bin");

            //Ingame Map
            //TextureConverter.toPNG("0600127D ToD.bin");
            //TextureConverter.toBin("0600127D.png", 0x0600127D, 20);
            //TextureConverter.toBin("./input/0600127D.png", 0x0600127D, 20);
            //TextureConverter.toBin("./input/0600127D - Ingame Map ClassicAce.png", 0x0600127D, 20);

            //CharGen Map
            //TextureConverter.toPNG("06004D5F Latest.bin");
            //TextureConverter.toPNG("06004D5F.bin");
            //TextureConverter.toBin("06004d5f.png", 0x06004d5f, 500);
            //TextureConverter.toBin("./input/06004D5F - Chargen Map.png", 0x06004d5f, 500);

            //LanguageFileTools language = new LanguageFileTools("./input/client_local_English.dat");
            ////language.DumpStrings();
            ////language.ModifyForInfiltration();
            //language.ModifyForCustomDM();

            //TextureConverter.toPNG("./0600377F.bin");
            //TextureConverter.toBin("./input/06020000 - Magnifying Glass.png", 0x06020000, 21);
            //TextureConverter.darkMajestyToPNG("./0600015E - Arms and Armor.bin");
            //TextureConverter.darkMajestyToPNG("./06000166 - Awareness.bin");
            //TextureConverter.darkMajestyToPNG("./06000174 - Armor.bin");
            //TextureConverter.darkMajestyToPNG("./06000176 - Sneak.bin");
            //TextureConverter.darkMajestyToPNG("./06000177 - Spellcraft.bin");
            //TextureConverter.darkMajestyToPNG("./06000172 - Appraise.bin");
            //TextureConverter.toBin("./input/0600015e - Arms and Armor.png", 0x0600015e, 20);
            //TextureConverter.toBin("./input/06000166 - Awareness.png", 0x06000166, 20);
            //TextureConverter.toBin("./input/06000174 - Armor.png", 0x06000174, 20);
            //TextureConverter.toBin("./input/06000172 - Appraise.png", 0x06000172, 20);
            //TextureConverter.toBin("./input/06000176 - Sneak.png", 0x06000176, 20);
            //TextureConverter.toBin("./input/06000177 - Spellcraft.png", 0x06000177, 20);

            //SkillTable skillTableLatest = new SkillTable("./Skill Tables/0E000004 - Skills Table - Original.bin");
            //SkillTable skillTableClassicWeaponSkills = new SkillTable("./Skill Tables/0E000004 - Skills Table - Classic weapon skills.bin");
            ////SkillTable skillTableAssessSkills = new SkillTable("./Skill Tables/0E000004 - Skills Table - Classic weapon skills with assess.bin");
            ////SkillTable skillTableRelease = new SkillTable("./Skill Tables/0E000004 - Skills Table - Release.bin");
            //skillTableClassicWeaponSkills.modifyForCustomDM(skillTableLatest);
            //skillTableClassicWeaponSkills.save("./0E000004 - Skills Table - CustomDM.bin");

            //CharGen charGen = new CharGen("./input/0E000002.bin");
            ////charGen.modifyForDM();
            ////charGen.save("./0E000002 - CharGen - Infiltration.bin");
            //charGen.modifyForCustomDM();
            //charGen.save("./0E000002 - CharGen - CustomDM.bin");

            //AceDatabaseUtilities.AddSpellTransferScrollsToVendors();
            //AceDatabaseUtilities.AddSalvageBarrelToVendors();
            //AceDatabaseUtilities.ChangeArmorBurdens();
            //AceDatabaseUtilities.ChangeShieldBurdens();
            //AceDatabaseUtilities.AllowTeakEbonyPorcelainSilkOnGems();
            //AceDatabaseUtilities.AddCooldownToCasters();
            //AceDatabaseUtilities.ChangeArrows();
            //AceDatabaseUtilities.ChangeCompositeWeapons();
            //AceDatabaseUtilities.CreateSpellDamageListForPortalDat();
            //AceDatabaseUtilities.ModifySpellDamage();
            //AceDatabaseUtilities.AddSpellConduitToVendors();
            //AceDatabaseUtilities.SoCSDisassemble();
            //AceDatabaseUtilities.ConvertSomeSoCStoTwoHanded();
            //AceDatabaseUtilities.CreateFoodList();
            //AceDatabaseUtilities.RedistributeSpellServicesToVendors();
            //AceDatabaseUtilities.AddTethersToVendors();
            //AceDatabaseUtilities.AddMagnifyingGlassToVendors();
            //AceDatabaseUtilities.AddCombatTacticsAndTechniquesToVendors();
            //AceDatabaseUtilities.AddAlchemySuppliesToVendors();
            //AceDatabaseUtilities.AddLeyLineAmuletsToVendors();
            //AceDatabaseUtilities.AddCombatManualToVendors();
            //AceDatabaseUtilities.RemovePortalGemsFromAllSpellComponentVendorsAddToJewelers();
            //AceDatabaseUtilities.RedistributeFoodIngredientsToGrocersAndFarmers();
            //AceDatabaseUtilities.RemoveSalvageMerchandiseTypeFromVendors();
            //AceDatabaseUtilities.BuildVendorSellList();
            //AceDatabaseUtilities.RemoveNonMutatedItemsFromVendors();
            //AceDatabaseUtilities.RemoveAmmoFromBlacksmithsAndWeaponsmiths();
            //AceDatabaseUtilities.RemoveAmmoFromSpecificIDs();
            //AceDatabaseUtilities.RedistributeVendorMerchandiseTypes();
            //AceDatabaseUtilities.ChangeSpellScrollPrices();
            //AceDatabaseUtilities.AddRumorColorCodesToVendors();
            //AceDatabaseUtilities.RedistributeTradeNotesToVendors();
            //AceDatabaseUtilities.FindScrollVendors();
            //AceDatabaseUtilities.RemoveCowlsFromShopkeepers();

            //XPTable xpConverter = new XPTable("./input/0E000018 latest.bin");
            //xpConverter.capAtlLevel(126);
            //xpConverter.save("0E000018 edited.bin");

            //AceDatabaseUtilities.AddSkillReqToAtlanWeapons();
            //AceDatabaseUtilities.AddLevelReqToShadowArmor();

            //AceDatabaseUtilities.CreateCreatureXPList();
            //AceDatabaseUtilities.UpdateXPRewardsFromList("./input/listOfXpRewards.txt");

            //AceDatabaseUtilities.CreateXPRewardsList();

            //AceDatabaseUtilities.AddTethersToVendors();
            //AceDatabaseUtilities.IncreaseThrownWeaponsStackSizeTo250();
            //AceDatabaseUtilities.AddSpellComponentPouchToVendors();

            //LandscapeSpawnMap spawnMap = new LandscapeSpawnMap();
            //spawnMap.ExportSpawnMapToAceDatabase("./input/spawnMap/region.json", "./input/spawnMap/regionEncounterMap.png");
            //spawnMap.LoadRegionFromJson("./input/spawnMap/region.json");
            //spawnMap.SaveRegionToPng("./regionEncounterMap.png");
            //spawnMap.LoadRegionFromPng("./regionEncounterMap.png");
            //spawnMap.LoadRegionFromPng("./input/spawnMap/regionEncounterMap.png");
            //spawnMap.SaveRegionToJson("./region.json");

            //GoArrowUtilities goArrow = new GoArrowUtilities("./input/CustomDM-GoArrow-Locations - No APN.xml");
            //goArrow.AddApartmentHallConnections();
            //goArrow.ReIndex();
            //goArrow.Save("./CustomDM-GoArrow-Locations.xml");

            //AceDatabaseUtilities.RemoveAllNonApartmentHouses();
            //AceDatabaseUtilities.AddPortalGemsToAllSpellComponentVendors();

            //AceMutationScripts aceMutationScripts = new AceMutationScripts();
            //aceMutationScripts.BuildScripts();

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

            //SpellsConverter.MergeWeaponsSkillsForCustomDM("Spells/0E00000E - Reversed plus removed auras and converted description from cache file.txt", "./0E00000E - CustomDM.txt");
            //SpellsConverter.toBin("Spells/0E00000E - CustomDM.txt", "./0E00000E - Spells Table - CustomDM.bin");
            //SpellsConverter.toBin("Spells/0E00000E - Reversed plus removed auras and converted description from cache file.txt", "./0E00000E - Reversed plus removed auras and converted description from cache file.bin");

            ////TextureIdDictionary.folderExtractTextureFromHeader("./input/textureHeaders/");
            ////return;

            //RegionManipulation regionLatest = new RegionManipulation("Region/13000000 - Latest.bin", eDatFormat.Latest);
            ////RegionManipulation regionToD = new RegionManipulation("Region/13000000 - ToD - 2005-06-02 (Iteration 4).bin", eDatFormat.ToD);
            //RegionManipulation regionDM = new RegionManipulation("Region/130F0000 - 2005-02-XX (Admin) (Iteration 2112).bin", eDatFormat.retail);

            ////regionLatest.ExportSceneIds("sceneIdsLatest.txt");
            ////regionDM.ExportSceneIds("sceneIdsDM.txt");

            //regionDM.GenerateExtractAndReplaceBats();

            //List<uint> ListEoR = regionLatest.GetSceneIds();
            //List<uint> ListDM = regionDM.GetSceneIds();
            ////SceneUtilities.CompareObjectLists(ListEoR, ListDM);
            //SceneUtilities.CompareObjects(ListEoR, ListDM);

            //List<uint> ListEoR = regionLatest.GetObjects("./input/client_portal - EoR.dat");
            //List<uint> ListDM = regionLatest.GetObjects("./input/portal - 2005-02-XX (Admin) (Iteration 2112).dat");
            //List<uint> ListDM = regionLatest.GetObjects("./input/portal - 2000-08-22 (Ice Island, Shadow Spires) (Iteration 124).dat");
            //regionLatest.CompareObjects("./input/client_portal - EoR.dat", "./input/client_portal - Infiltration.dat");

            //RegionConverter regionInf = new RegionConverter("Region/13000000 - 2005-02-XX (Admin) (Iteration 2112).bin");
            //regionConverter.WriteToBin("./13000000.bin");
            //RegionConverter.convert("Region/13000000.bin");
            //RegionConverterDM.convert("Region/130F0000 Bael.bin");
            //RegionConverterDM.convert("Region/130F0000 test.bin");
            //RegionComparer.compare("Region/130F0000 Ice.bin", false, "Region/13000000 - EoR.bin", true);

            //cDatFile datFile = new cDatFile();
            ////datFile.loadFromDat("./input/client_cell_1 - EoR.dat");
            ////datFile.loadFromDat("./input/client_cell_1 - Infiltration.dat");
            ////datFile.loadFromDat("./input/client_cell_1 - Release.dat");
            //datFile.loadFromDat("./input/client_cell_1 - DM.dat");
            ////datFile.loadFromDat("./client_cell_1.dat");
            //cCellDat cellDat = new cCellDat();
            //cellDat.loadFromDat(datFile);
            //cMapDrawer mapDrawer = new cMapDrawer(cellDat);
            //mapDrawer.draw(false, 50);

            //testRandomValueGenerator();
            //Console.ReadLine();
            //return;

            //cache4Converter.loadFromRaw("./input/0004.raw");
            ////cache8Converter.loadFromJson("./output/questFlags.json");
            ////cache8Converter.writeRaw("./output/");
            //cache4Converter.writeJson("./output/");
            //Console.ReadLine();
            //return;

            //Patcher.patch();
            //return;

            //SpellsConverter.toTxt("Spells/0E00000E - 2010.bin");
            //SpellsConverter.toTxt("Spells/0E00000E - latest.bin");
            //SpellsConverter.raw0002toTxt("intermediate/0002.raw");
            //return;
            //SpellsConverter.to0002raw("Spells/0E00000E - latest.txt");
            //SpellsConverter.toBin("Spells/0E00000E - Reversed.txt");
            //SpellsConverter.toTxt("Spells/0E00000E - Reversed.bin");
            //SpellsConverter.revertWeaponMasteriesAndAuras("Spells/0E00000E - Latest.txt", "Spells/0E00000E - 2010.txt");
            //SpellsConverter.toBin("0E00000E.txt");
            //SpellsConverter.toBin("Spells/0E00000E - Reversed plus removed auras.txt");
            //SpellsConverter.toJson("Spells/0E00000E - Reversed plus removed auras.bin");
            //SpellsConverter.toJson("Spells/0E00000E - Latest.bin");
            //SpellsConverter.toJson("Spells/0002.raw", "Spells/0E00000E - Reversed plus removed auras.bin");
            //SpellsConverter.transferSpellDescriptiuonsFromJsonToTxt("Spells/spells.json", "Spells/0E00000E - Reversed plus removed auras.txt");

            //SpellManipulationTools oldSpells = new SpellManipulationTools("Spells/0E00000E - DM.bin", false);
            //oldSpells.SpellTableToTxt();
            //SpellsConverter.toTxt("Spells/0E00000E - DM.bin");

            //SpellManipulationTools spells = new SpellManipulationTools("Spells/0E00000E - latest.bin");
            //SpellManipulationTools oldSpells = new SpellManipulationTools("Spells/0E00000E - 2010.bin");
            //spells.LoadCache2Raw("Spells/0002.raw");
            //spells.RevertWeaponMasteriesAndAuras(oldSpells);
            //spells.ApplyCache2DataFixes();
            //spells.TransferSpellDataFromCacheToSpellBase();
            //spells.ModifyForCustomDM();
            //spells.UpdateDamageFromServerData("./spells/ServerSpellDamageList.txt");
            //spells.SpellTableToTxt();
            //spells.SpellTableToBin();

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
            //RegionComparer.compare("Region/130F0000 DM.bin", false, "Region/130F0000 - 2005-02-XX (Admin) (Iteration 2112).bin", false);
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
        }

        static void LanguageManipulationForCustomDM(string[] args)
        {
            //TODO: these files cause the client to crash, for now using Kenneth Gober's dat writing tools for these
            cDatFile languageDatFile = new cDatFile();
            languageDatFile.loadFromDat("./input/client_local_English - EoR.dat");
            languageDatFile.SetFileIteration(20004);
            languageDatFile.addFilesFromFolder("./Dat Builder/Language/CustomDM/");
            languageDatFile.writeToDat("./client_local_English.dat");
        }

        static void LanguageManipulationForInfiltration(string[] args)
        {
            //TODO: these files cause the client to crash, for now using Kenneth Gober's dat writing tools for these
            cDatFile languageDatFile = new cDatFile();
            languageDatFile.loadFromDat("./input/client_local_English - EoR.dat");
            languageDatFile.SetFileIteration(10002);
            languageDatFile.addFilesFromFolder("./Dat Builder/Language/Infiltration/");
            languageDatFile.writeToDat("./client_local_English.dat");
        }

        static void PortalManipulationForEoR(string[] args)
        {
            cDatFile portalDatFile = new cDatFile();
            portalDatFile.loadFromDat("./input/client_portal - EoR.dat");
            portalDatFile.SetFileIteration(4016);
            portalDatFile.addFilesFromFolder("./Dat Builder/Portal/EoR/");
            portalDatFile.writeToDat("./client_portal.dat");
        }

        static void MapManipulationForEoR(string[] args)
        {
            cDatFile datFile = new cDatFile();
            datFile.loadFromDat("./input/client_cell_1 - EoR.dat");
            datFile.SetFileIteration(4016);

            cDatFile datFileRelease = new cDatFile(); // 1999-10-09
            datFileRelease.loadFromDat("./input/cell - Release.dat");
            cDatFile datFileIceIsland = new cDatFile(); // 2000-08-22
            datFileIceIsland.loadFromDat("./input/cell - 2000-08-22 (58368kb) (Complete,Ice Island, Shadow Spires) (Iteration 108 - Complete).dat");
            cDatFile datFile279 = new cDatFile(); // 2000-12-31
            datFile279.loadFromDat("./input/cell - 2000-12-31 (Iteration 279 - Complete).dat");
            cDatFile datFile266 = new cDatFile(); // 2001-02-11
            datFile266.loadFromDat("./input/cell - 2001-02-11 (Iteraton 266 - Kelderam's Ward - 14.49%).dat");
            cDatFile datFileDM = new cDatFile(); // 2001-09-12
            datFileDM.loadFromDat("./input/cell - DM - 2001-09-12.dat");
            cDatFile datFile1593 = new cDatFile(); // 2005-02-XX
            datFile1593.loadFromDat("./input/cell - 2005-02-XX (202752kb) (Admin) (Iteration 1593 - Complete).dat");
            cDatFile datFile369 = new cDatFile(); // 2010-06-09
            datFile369.loadFromDat("./input/client_cell_1 - 2010-06-09 (Iteration 369 - Complete).dat");

            datFile.replaceLandblockRect(0xD0F9, 0xDEEE, datFileRelease); // Northeast Mud Island (leftmost of trio)

            datFile.replaceLandblockRect(0xE109, 0xEA05, datFileRelease); // One Mile Island

            datFile.replaceLandblockRect(0x6F0D, 0x7906, datFileRelease, 0x7B0D); // Ulgrim's Island Original

            var iceIsland = new List<uint>();
            iceIsland.AddRange(datFileIceIsland.getListOfLandblocksInRect(0x54FB, 0x61EB));
            iceIsland.AddRange(datFileIceIsland.getListOfLandblocksInRect(0x61FB, 0x6CEE));
            iceIsland.AddRange(datFileIceIsland.getListOfLandblocksInRect(0x6CFB, 0x72F2));
            datFile.replaceLandblockList(iceIsland, datFileIceIsland); // Ice Island

            var easternIsland = new List<uint>();
            easternIsland.AddRange(datFileIceIsland.getListOfLandblocksInRect(0xE2C6, 0xFD6D));
            easternIsland.AddRange(datFileIceIsland.getListOfLandblocksInRect(0xE96D, 0xF061));
            easternIsland.AddRange(datFileIceIsland.getListOfLandblocksInRect(0xDBBB, 0xE29E));
            easternIsland = easternIsland.Except(datFileIceIsland.getListOfLandblocksInRect(0xFAC7, 0xFEC3)).ToList();
            //datFile.landblockBucketFill(easternIsland, 0x001B);
            datFile.replaceLandblockList(0x4791, easternIsland, datFileIceIsland); // Eastern Island

            datFile.replaceLandblockRect(0x5C80, 0x6F57, datFileIceIsland, 0x6E53); // ! Island

            datFile.replaceLandblockRect(0x653E, 0x7B28, datFileIceIsland, 0x0AFD); // White Island

            var oldMarae = new List<uint>();
            oldMarae.AddRange(datFileIceIsland.getListOfLandblocksInRect(0x0EB3, 0x259F));
            oldMarae.AddRange(datFileIceIsland.getListOfLandblocksInRect(0x119F, 0x229F));
            oldMarae.AddRange(datFileIceIsland.getListOfLandblocksInRect(0x119E, 0x219E));
            oldMarae.AddRange(datFileIceIsland.getListOfLandblocksInRect(0x119D, 0x1F9B));
            oldMarae.AddRange(datFileIceIsland.getListOfLandblocksInRect(0x139A, 0x1E99));
            oldMarae.AddRange(datFileIceIsland.getListOfLandblocksInRect(0x1798, 0x1C97));
            oldMarae.AddRange(datFileIceIsland.getListOfLandblocksInRect(0x1896, 0x1A96));
            oldMarae = oldMarae.Except(datFileIceIsland.getListOfLandblocksInRect(0x1EB4, 0x26B1)).ToList();
            oldMarae = oldMarae.Except(datFileIceIsland.getListOfLandblocksInRect(0x23B1, 0x38AB)).ToList();

            // Add scene objects to old Marae, this was extrapolated from what is used in similar biomes.
            datFileIceIsland.replaceLandblocksSpecificTexture(oldMarae, 0x0000, 0x5000);
            datFileIceIsland.replaceLandblocksSpecificTexture(oldMarae, 0x0024, 0x5024);
            datFileIceIsland.replaceLandblocksSpecificTexture(oldMarae, 0x000C, 0x100C);
            datFileIceIsland.replaceLandblocksSpecificTexture(oldMarae, 0x0034, 0x5034);
            datFileIceIsland.replaceLandblocksSpecificTexture(oldMarae, 0x003C, 0x603C);
            datFileIceIsland.replaceLandblocksSpecificTexture(oldMarae, 0x0038, 0x5038);
            datFileIceIsland.replaceLandblocksSpecificTexture(oldMarae, 0x0040, 0x4840);
            datFileIceIsland.replaceLandblocksSpecificTexture(oldMarae, 0x0054, 0x6054);

            //datFile.landblockBucketFill(oldMarae, 0x001B);
            datFile.replaceLandblockList(0x03B2, oldMarae, datFileIceIsland); // Old Marae Lassel

            datFile.replaceLandblockRect(0x0011, 0x1100, datFileIceIsland, 0xEBC7); // Shadow Gauntlet (Caul) (Feb 2000)

            datFile.replaceLandblockRect(0x0011, 0x1100, datFileDM, 0xEBB2); // Singularity Caul Original (July 2001)

            var oldAsheronsIsland = new List<uint>();
            oldAsheronsIsland.AddRange(datFileIceIsland.getListOfLandblocksInRect(0xD29E, 0xDD97));
            oldAsheronsIsland.AddRange(datFileIceIsland.getListOfLandblocksInRect(0xD697, 0xDD91));
            oldAsheronsIsland.AddRange(datFileIceIsland.getListOfLandblocksInRect(0xD497, 0xD694));
            //datFile.landblockBucketFill(oldAsheronsIsland, 0x001B);
            datFile.replaceLandblockList(0xEE9C, oldAsheronsIsland, datFileIceIsland); // Asheron's Island Original

            cDatFile datFileEoR = new cDatFile();
            datFileEoR.loadFromDat("./input/client_cell_1 - EoR.dat");

            var innerSeaDungeons = new List<uint>();
            innerSeaDungeons.AddRange(datFileEoR.getListOfLandblocksInRect(0x507B, 0x6843));
            //datFile.landblockBucketFill(innerSeaDungeons, 0x001B);
            datFile.replaceLandblockList(innerSeaDungeons, datFileEoR, false, false, true, true, true);

            var dungeonsUnderOldMarae = new List<uint>();
            dungeonsUnderOldMarae.AddRange(datFileEoR.getListOfLandblocksInRect(0x03B3, 0x039E));
            //datFile.landblockBucketFill(dungeonsUnderOldMarae, 0x001B);
            datFile.replaceLandblockList(dungeonsUnderOldMarae, datFileEoR, false, false, true, true, true);

            var dungeonsUnderMovedCauls = new List<uint>();
            dungeonsUnderMovedCauls.AddRange(datFileEoR.getListOfLandblocksInRect(0xECC8, 0xEDA1));
            datFile.clearLandblockCellsList(dungeonsUnderMovedCauls);

            datFile.replaceLandblock(0x01D1, datFileRelease, 0x2c00); // Mayoi Shrine - 1999-10-09 - @teleloc 0x2C000247 [40.299999 -19.900000 6.000000] 0.009599 0.000000 0.000000 -0.999954

            datFile.replaceLandblock(0x0160, datFileIceIsland, 0x2d00); // Fort Tununska - 2000-08-22 - @teleloc 0x2D000143 [40.000000 -100.000000 0.000000] 1.000000 0.000000 0.000000 0.000000
            datFile.replaceLandblock(0x016A, datFileIceIsland, 0x2e00); // Guardian Crypt - 2000-08-22 - @teleloc 0x2E00010D [30.000000 -20.000000 -6.000000] 0.000000 0.000000 0.000000 -1.000000
            datFile.replaceLandblock(0x016B, datFileIceIsland, 0x2f00); // Creepy Chambers - 2000-08-22 - @teleloc 0x2F0001F3 [40.000000 -50.000000 6.000000] 0.000000 0.000000 0.000000 -1.000000
            datFile.replaceLandblock(0x02EA, datFileIceIsland, 0x3000); // Domino's Lodge - 2000-08-22 - @teleloc 0x30000133 [40.000000 0.000000 0.000000] 1.000000 0.000000 0.000000 0.000000
            datFile.replaceLandblock(0x02EB, datFileIceIsland, 0x3100); // Domino's Lodge - 2000-08-22 - @teleloc 0x31000133 [40.000000 0.000000 0.000000] 1.000000 0.000000 0.000000 0.000000
            datFile.replaceLandblock(0x02EC, datFileIceIsland, 0x3200); // Domino's Lodge - 2000-08-22 - @teleloc 0x32000133 [40.000000 0.000000 0.000000] 1.000000 0.000000 0.000000 0.000000
            datFile.replaceLandblock(0x0174, datFileIceIsland, 0x3300); // Training Hall Release - 2000-08-22 - @teleloc 0x3300016E [30.000000 -130.000000 0.000000] -1.000000 0.000000 0.000000 0.000000

            datFile.replaceLandblock(0x02B0, datFile279, 0x3400); // Sepulcher of the Hopeslayer - 2000-12-31 - @teleloc 0x340002A8 [276.622009 -67.712997 72.004997] -0.707107 0.000000 0.000000 -0.707107
            datFile.replaceLandblock(0x02B9, datFile279, 0x3500); // Shard of the Herald BOX Dungeon - 2000-12-31 - @teleloc 0x35000100 [0.000000 -5.000000 0.000000] 1.000000 0.000000 0.000000 0.000000

            datFile.replaceLandblock(0x02B7, datFile266, 0x3600); // Catacombs of Ithaenc Original - 2001-02-11 - @teleloc 0x360002F1 [70.000000 -60.000000 0.000000] 1.000000 0.000000 0.000000 0.000000
            datFile.replaceLandblock(0x02B9, datFile266, 0x3700); // Shard of the Herald Dungeon - 2001-02-11 - @teleloc 0x3700018A [80.000000 -20.000000 -12.000000] -0.707107 0.000000 0.000000 -0.707107

            datFile.replaceLandblock(0x018A, datFileDM, 0x3800); // Advocate Dungeon (Includes Swank v1) - 2001-09-12 - @teleloc 0x3800017F [180.000000 -90.000000 0.000000] 1.000000 0.000000 0.000000 0.000000
                                                                                                              // Swank: @teleloc 0x38000141 [70.000000 -110.000000 0.005000] -0.716461 0.000000 0.000000 0.697627
            datFile.replaceLandblock(0x0272, datFileDM, 0x3900); // Ogham Dungeon - 2001-09-12 - @teleloc 0x3900029A [160.000000 -60.000000 -6.000000] -0.707107 0.000000 0.000000 -0.707107
            datFile.replaceLandblock(0x03A5, datFileDM, 0x3a00); // Weeping Pit - 2001-09-12 - @teleloc 0x3A00010A [35.000000 -10.000000 0.005000] 0.708498 0.000000 0.000000 0.705713
            datFile.replaceLandblock(0x0114, datFileDM, 0x3b00); // Sub-Terranean Vault - 2001-09-12 - @teleloc 0x3B000104 [0.000000 -60.000000 0.000000] 0.707107 0.000000 0.000000 -0.707107

            datFile.replaceLandblock(0x0363, datFile1593, 0x3c00); // Training Academy Original (Dec 2001) - 2005-02-XX - @teleloc 0x3C00012F [2.500000 -29.000000 0.000000] -0.369747 0.000000 0.000000 -0.929133
                                                                                                              // Part 2 - @teleloc 0x3C0002EF [100.000000 -190.000000 0.005000] 0.902585 0.000000 0.000000 -0.430511

            datFile.replaceLandblock(0x8A04, datFile369, 0x3d00); // Town Network Original - 2010-06-09 - @teleloc 0x3D000145 [70.000000 -80.000000 0.005000] 1.000000 0.000000 0.000000 0.000000

            cCellDat cellDat = new cCellDat();
            cellDat.loadFromDat(datFile);
            cMapDrawer mapDrawer = new cMapDrawer(cellDat);
            mapDrawer.draw(true, 0);

            datFile.writeToDat("./client_cell_1.dat");
        }

        static void PortalManipulationForCustomDM(string[] args)
        {
            cDatFile portalDatFile = new cDatFile();
            portalDatFile.loadFromDat("./input/client_portal - EoR.dat");
            portalDatFile.SetFileIteration(20021);
            portalDatFile.addFilesFromFolder("./Dat Builder/Portal/CustomDM/");
            portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Shared/Textures");
            //portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Shared/Resized EoR Trees");
            portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Shared/Trees");
            portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Shared/Frozen Fields");
            portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Shared/Police Box");
            portalDatFile.writeToDat("./client_portal.dat");
        }

        static void MapManipulationForCustomDM(string[] args)
        {
            cDatFile datFile = new cDatFile();
            //int retailIteration = datFile.loadFromDat("./input/cell - 2005-02-XX (202752kb) (Admin) (Iteration 1593 - Complete).dat");
            //datFile.convertRetailToToD(retailIteration);
            datFile.loadFromDat("./input/client_cell_1 - Infiltration - Converted to ToD format (Iteration 1593).dat"); // Same as above but already converted to ToD format to speed thing up.
            datFile.SetFileIteration(20005);

            cDatFile datFileObsidianSpan = new cDatFile();
            datFileObsidianSpan.loadFromDat("./input/cell - 2000-12-31 - Obsidian Span.dat");

            cDatFile datFileRelease = new cDatFile();
            datFileRelease.loadFromDat("./input/cell - Release.dat");

            cDatFile datFileArwic = new cDatFile();
            datFileArwic.loadFromDat("./input/cell - 2000-08-22 (58368kb) (Complete,Ice Island, Shadow Spires) (Iteration 108 - Complete).dat");

            cDatFile datFileTufa = new cDatFile();
            datFileTufa.loadFromDat("./input/cell - 2000-07-26 (Iteration 91 - 98.57%).dat");

            cDatFile datFileToD = new cDatFile();
            datFileToD.loadFromDat("./input/client_cell_1 - ToD.dat");

            cDatFile datFileEoR = new cDatFile();
            datFileEoR.loadFromDat("./input/client_cell_1 - EoR.dat");

            //Arwic
            datFile.replaceLandblock(0xC6A90013, datFileRelease);
            datFile.replaceLandblock(0xC6A90008, datFileRelease);
            datFile.replaceLandblock(0xC6A80038, datFileRelease);
            datFile.replaceLandblock(0xC6AA0011, datFileRelease);
            datFile.replaceLandblock(0xC5AA0039, datFileRelease);
            datFile.replaceLandblock(0xC5A80028, datFileRelease);

            datFile.replaceLandblock(0xC5A90037, datFileArwic); // Meeting Hall with the correct fallen pillars around the road.
            datFile.replaceLandblock(0xC6A90013, datFileArwic, false, false, true, false); // use the surface objects from this file(lamp posts!) but not the buildings as they have fire lighting effects that bleed all over thru the walls on the new client.

            datFile.replaceLandblock(0xC5A6003A, datFileRelease); // Get rid of Newic.

            //Yanshi
            datFile.replaceLandblock(0xB86E0038, datFileArwic);
            datFile.replaceLandblock(0xB86F0032, datFileArwic);
            datFile.replaceLandblock(0xB8700029, datFileArwic);
            datFile.replaceLandblock(0xB9700024, datFileArwic);
            datFile.replaceLandblock(0xB96F0040, datFileArwic);
            datFile.replaceLandblock(0xB96E0018, datFileArwic);
            datFile.replaceLandblock(0xBA6F0004, datFileArwic);
            datFile.replaceLandblock(0xBA700001, datFileArwic);

            //Get rid of New Yanshi
            datFile.replaceLandblock(0xB9720022, datFileArwic);
            datFile.replaceLandblock(0xBA720005, datFileArwic);
            datFile.replaceLandblock(0xB9710030, datFileArwic);            

            datFile.replaceLandblock(0xB76F0034, datFileArwic); //Yanshi Meeting Hall

            //Tufa
            datFile.replaceLandblock(0x866D000B, datFileTufa);
            datFile.replaceLandblock(0x866E0011, datFileTufa);
            datFile.replaceLandblock(0x856D003C, datFileTufa);
            datFile.replaceLandblock(0x856E0031, datFileTufa);
            datFile.replaceLandblock(0x856C001C, datFileTufa);
            datFile.replaceLandblock(0x876E0032, datFileTufa);
            datFile.replaceLandblock(0x856F0015, datFileTufa);
            datFile.replaceLandblock(0x846E0012, datFileRelease);
            datFile.replaceLandblock(0x846D0018, datFileRelease);
            datFile.replaceLandblock(0x836D0040, datFileRelease);
            datFile.replaceLandblock(0x836E0039, datFileRelease);

            datFile.addBuildingFrom(0x856E010E, datFileToD); //Undead Hunter Tent

            datFile.replaceLandblock(0x846D003F, datFileArwic, false, false, true, true); //Tufa Meeting Hall, buildings only, terrain includes crater.

            //Eastham Shadow Spire Crater
            datFile.replaceLandblock(0xCD95002F, datFileRelease);
            datFile.replaceLandblock(0xCE950006, datFileRelease, true, true, false, false);
            datFile.replaceLandblock(0xCE960001, datFileRelease);
            datFile.replaceLandblock(0xCD960039, datFileRelease);
            datFile.removeBuilding(0xCE95016C);

            //datFile.replaceLandblockArea(0x2B110028, datFileToD); // Candeth Keep crash
            datFile.fixLandblockCells(0x2B110000, datFileToD); // Candeth Keep crash
            datFile.fixLandblockCells(0x2B120000, datFileToD); // Candeth Keep crash
            datFile.fixLandblockCells(0x2D5B002B, datFileToD); // Renegade Fortress crash
            //datFile.fixAllLandblockCells(datFileToD);

            datFile.replaceLandblock(0xEC0E0110, datFileToD); // Xi Ru's Chapel crash
            //datFile.findEnvironmentIdInCells();

            datFile.removeHouseSettlements(datFileObsidianSpan);

            datFile.removeNoobFence();

            datFile.replaceLandblock(0x7D8F0013, datFileObsidianSpan); // The Zaikhal settlement portals pillar is baked into this landblock for some reason, remove it.

            List<uint> LandblocksToReplace = new List<uint>()
            {
                0x905C003B, // North Al-Arqas Outpost
                0x915C0003, // North Al-Arqas Outpost part 2
                0x8C58002B, // West Al-Arqas Outpost
                0xC380000C, // East Lytelthorpe Outpost
                0xBB80002E, // West Lytelthorpe Outpost
                0xEA3D000E, // East Nanto Outpost
                0xE542002B, // North Nanto Outpost
                0xCC8C0014, // East Rithwic Outpost
                0xC8880016, // South Rithwic Outpost
                0x9B7B0015, // East Samsur Outpost
                0x937F002D, // Northwest Samsur Outpost
                0xB9730032, // North Yanshi Outpost
                0xB96B001E, // South Yanshi Outpost
            };
            datFile.replaceLandblocksSpecialForStarterOutposts(LandblocksToReplace, datFileObsidianSpan);

            List<uint> buildingsToRemove = new List<uint>()
            {
                0xA9B30114, // Holtburg->Neydisa Castle Portal Bunker
                0xA9B40183, // Holtburg->Shoushi Portal Bunker
                0xA9B40188, // Holtburg->Rithwic Portal Bunker
                0xA9B4017E, // Holtburg->Cragstone Portal Bunker
                0xAAB40108, // Holtburg->Arwic Portal Bunker
                0xAAB40103, // Holtburg->Dryreach Portal Bunker

                0x7D64018B, // Yaraq->Holtburg Portal Bunker
                0x7D64017C, // Yaraq->Al-Arqas Portal Bunker
                0x7D640181, // Yaraq->Samsur Portal Bunker
                0x7E640122, // Yaraq->Zaikhal Portal Bunker
                0x7E640127, // Yaraq->Linvak Tukal Portal Bunker
                0x7D640177, // Yaraq->Khayyaban Portal Bunker
                0x7D640186, // Yaraq->Xarabydum Portal Bunker

                0xDA560116, // Shoushi->Kryst Portal Bunker
                0xDA5501F9, // Shoushi->Nanto Portal Bunker
                0xDA56011B, // Shoushi->Kara Portal Bunker
                0xDA5501F4, // Shoushi->Yanshi Portal Bunker
                0xDA540103, // Shoushi->Tou-Tou Portal Bunker
                0xDA5501EF, // Shoushi->Yaraq Portal Bunker
                0xD955010D, // Shoushi->Hebian-To Portal Bunker

                //Statues
                0x90580141, // Al-Arqas
                0xA9B40177, // Holtburg
                0xBF80017D, // Lytelthorpe
                0xE63D0154, // Nanto
                0xC88C0171, // Rithwic
                0x977B015F, // Samsur
                0xDA5501E8, // Shoushi
                0x7D640151, // Yaraq
                0xBC9F0166, // Cragstone
                0x85880129, // Al-Jalima
                0xCE95016A, // Eastham
                0xA1A40162, // Glenden Wood
                0xE74E01A7, // Hebian-To
                0x9E430132, // Khayyaban
                0xE9220141, // Kryst
                0xDA3B013C, // Lin
                0xA2600122, // Uziz
                0x80900139, // Zhaikal
                0xF75C0155, // Tou-Tou
                0xCD41018B, // Baishi
                0xF2220122, // MacNiall's Freehold
                0xE5320136, // Mayoi
                0x49B6012D, // Plateau Village
                0x9722015F, // Qalaba'r
                0xC95B0169, // Sawato
                0x64D50131, // Stonehold
                0x866C010B, // Tufa
                0x1134014A, // Ayan Baqur
                0xBA170143, // Kara
                0x3F310101, // Wai Jhou
                0x2581016B, // Fort Tethana

                //Arcanum Buildings
                0x90570102, // Al-Arqas
                0xA9B40171, // Holtburg
                0xBE800109, // Lytelthorpe
                0xE63D014E, // Nanto
                0xC88B0102, // Rithwic
                0x977B0159, // Samsur
                0xDA56010E, // Shoushi
                0x7D64014B, // Yaraq
                0xBA9E0105, // Cragstone
                0x85880123, // Al-Jalima
                0xCE950163, // Eastham
                0xA2A40100, // Glenden Wood
                0xE64E0104, // Hebian-To
                0x9E43012D, // Khayyaban
                0xE8210104, // Kryst
                0xDB3B011D, // Lin
                0xA260011C, // Uziz
                0x7F8F011C, // Zhaikal
                0xF75C014F, // Tou-Tou
                0xCD410183, // Baishi
                0xF2230104, // MacNiall's Freehold
                0xE532012E, // Mayoi
                0x49B60126, // Plateau Village
                0x97220159, // Qalaba'r
                0xC95B0161, // Sawato
                0x64D50129, // Stonehold
                0x11340144, // Ayan Baqur
                0xBA17013B, // Kara
            };
            datFile.removeBuildings(buildingsToRemove);
            //Since we're removing the bunkers might as well roll back Holtburg's heightmap that was flattened in areas to fit the bunkers.
            datFile.replaceLandblockTerrain(0xA9B40024, datFileObsidianSpan, true, true);
            datFile.replaceLandblockTerrain(0xAAB4000A, datFileObsidianSpan, true, true);
            datFile.replaceLandblockTerrain(0xA8B4003B, datFileObsidianSpan, true, true);

            datFile.replaceLandblockRect(0x0011, 0x1100, datFileArwic, 0x0423); // Old Caul
            datFile.replaceLandblockRect(0x653E, 0x7B28, datFileArwic); // White Island
            datFile.replaceLandblockRect(0x5C80, 0x6F57, datFileArwic); // ! Island

            datFile.replaceLandblockRect(0x54FB, 0x61EB, datFileArwic); // Ice Island
            datFile.replaceLandblockRect(0x61FB, 0x6CEE, datFileArwic); // Ice Island
            datFile.replaceLandblockRect(0x6CFB, 0x72F2, datFileArwic); // Ice Island

            datFile.replaceLandblockRect(0xD1ED, 0xE2E2, datFileArwic, 0xD1F9); // Eastern Island
            datFile.replaceLandblockRect(0xE2C6, 0xFD6D, datFileArwic, 0xE2FD); // Eastern Island
            datFile.replaceLandblockRect(0xE96D, 0xF061, datFileArwic, 0xE9A4); // Eastern Island
            datFile.replaceLandblockRect(0xDBBB, 0xE29E, datFileArwic, 0xDBF2); // Eastern Island
            datFile.replaceLandblockRect(0xD6E8, 0xDADF, datFileArwic, 0xD6DF); // Eastern Island
            //datFile.replaceLandblockList(0xDBFD, easternIsland, datFileArwic); // Eastern Island

            var oldMarae = new List<uint>();
            oldMarae.AddRange(datFileArwic.getListOfLandblocksInRect(0x0EB3, 0x259F));
            oldMarae.AddRange(datFileArwic.getListOfLandblocksInRect(0x119F, 0x229F));
            oldMarae.AddRange(datFileArwic.getListOfLandblocksInRect(0x119E, 0x219E));
            oldMarae.AddRange(datFileArwic.getListOfLandblocksInRect(0x119D, 0x1F9B));
            oldMarae.AddRange(datFileArwic.getListOfLandblocksInRect(0x139A, 0x1E99));
            oldMarae.AddRange(datFileArwic.getListOfLandblocksInRect(0x1798, 0x1C97));
            oldMarae.AddRange(datFileArwic.getListOfLandblocksInRect(0x1896, 0x1A96));

            datFileArwic.replaceLandblocksSpecificTexture(oldMarae, 0x0000, 0x5000);
            datFileArwic.replaceLandblocksSpecificTexture(oldMarae, 0x0024, 0x5024);
            datFileArwic.replaceLandblocksSpecificTexture(oldMarae, 0x000C, 0x100C);
            datFileArwic.replaceLandblocksSpecificTexture(oldMarae, 0x0034, 0x5034);
            datFileArwic.replaceLandblocksSpecificTexture(oldMarae, 0x003C, 0x603C);
            datFileArwic.replaceLandblocksSpecificTexture(oldMarae, 0x0038, 0x5038);
            datFileArwic.replaceLandblocksSpecificTexture(oldMarae, 0x0040, 0x4840);
            datFileArwic.replaceLandblocksSpecificTexture(oldMarae, 0x0054, 0x6054);

            datFile.replaceLandblockRect(0x0EB3, 0x259F, datFileArwic, 0x05E5); // Old Marae Lassel
            datFile.replaceLandblockRect(0x119F, 0x229F, datFileArwic, 0x08D1); // Old Marae Lassel
            datFile.replaceLandblockRect(0x119E, 0x219E, datFileArwic, 0x08D0); // Old Marae Lassel
            datFile.replaceLandblockRect(0x119D, 0x1F9B, datFileArwic, 0x08CF); // Old Marae Lassel
            datFile.replaceLandblockRect(0x139A, 0x1E99, datFileArwic, 0x0ACC); // Old Marae Lassel
            datFile.replaceLandblockRect(0x1798, 0x1C97, datFileArwic, 0x0ECA); // Old Marae Lassel
            datFile.replaceLandblockRect(0x1896, 0x1A96, datFileArwic, 0x0FC8); // Old Marae Lassel

            datFile.replaceLandblockRect(0x3B0F, 0x400D, datFileEoR); // Moarsman Island - Nyr'leha
            datFile.replaceLandblockRect(0x370D, 0x4409, datFileEoR); // Moarsman Island - Nyr'leha

            datFile.replaceLandblockRect(0xC0F9, 0xD9E0, datFileEoR); // Dark Isle and Vissidal
            datFile.replaceLandblockRect(0xCBE0, 0xD9DD, datFileEoR); // Dark Isle and Vissidal
            datFile.replaceLandblockRect(0xD2DD, 0xD9D9, datFileEoR); // Dark Isle and Vissidal

            datFile.replaceLandblockRect(0xE2D8, 0xEBCC, datFileEoR, 0xCEF3); // Olthoi Island

            datFile.replaceLandblockRect(0xF43F, 0xFD2C, datFileEoR); // Freebooter Isle

            datFile.replaceLandblockRect(0x1EF9, 0x3DD5, datFileEoR); // Halaetan Isles
            datFile.replaceLandblockRect(0x3DFA, 0x45E6, datFileEoR); // Halaetan Isles
            datFile.replaceLandblockRect(0x45FA, 0x51EC, datFileEoR); // Halaetan Isles

            //datFile.addGridToAllLandblocks(); // Add grid for highlighting landblocks, for dev purposes. Disable for general use.

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

            //datFile.replaceLandblockRect(0x0011, 0x1100, datFileArwic, true, 0x3456);

            cCellDat cellDat = new cCellDat();
            cellDat.loadFromDat(datFile);
            cMapDrawer mapDrawer = new cMapDrawer(cellDat);
            mapDrawer.draw(false, 50);

            datFile.writeToDat("./client_cell_1.dat");
        }

        static void PortalManipulationForInfiltration(string[] args)
        {
            cDatFile portalDatFile = new cDatFile();
            portalDatFile.loadFromDat("./input/client_portal - EoR.dat");
            portalDatFile.SetFileIteration(10006);
            portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Infiltration/");
            portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Shared/Textures");
            //portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Shared/Resized EoR Trees");
            portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Shared/Trees");
            portalDatFile.writeToDat("./client_portal.dat");
        }

        static void MapManipulationForInfiltration(string[] args)
        {
            cDatFile datFile = new cDatFile();
            //int retailIteration = datFile.loadFromDat("./input/cell - 2005-02-XX (202752kb) (Admin) (Iteration 1593 - Complete).dat");
            //datFile.convertRetailToToD(retailIteration);
            datFile.loadFromDat("./input/client_cell_1 - Infiltration - Converted to ToD format (Iteration 1593).dat"); // Same as above but already converted to ToD format to speed thing up.
            datFile.SetFileIteration(10001);

            cDatFile datFileToD = new cDatFile();
            datFileToD.loadFromDat("./input/client_cell_1 - ToD.dat");

            //datFile.replaceLandblockArea(0x2B110028, datFileToD); // Candeth Keep crash
            datFile.fixLandblockCells(0x2B110000, datFileToD); // Candeth Keep crash
            datFile.fixLandblockCells(0x2B120000, datFileToD); // Candeth Keep crash
            datFile.fixLandblockCells(0x2D5B002B, datFileToD); // Renegade Fortress crash
            //datFile.fixAllLandblockCells(datFileToD);

            datFile.replaceLandblock(0xEC0E0110, datFileToD); // Xi Ru's Chapel crash
            //datFile.findEnvironmentIdInCells();

            cCellDat cellDat = new cCellDat();
            cellDat.loadFromDat(datFile);
            cMapDrawer mapDrawer = new cMapDrawer(cellDat);
            mapDrawer.draw(false, 50);

            datFile.writeToDat("./client_cell_1.dat");
        }

        static void PortalManipulationForInfiltrationAceClassicCustom(string[] args)
        {
            cDatFile portalDatFile = new cDatFile();
            portalDatFile.loadFromDat("./input/client_portal - EoR.dat");
            portalDatFile.SetFileIteration(40000);
            portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Infiltration/");
            portalDatFile.addFilesFromFolder("./Dat Builder/Portal/AceClassic Custom/");
            portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Shared/Textures");
            portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Shared/Resized EoR Trees");
            //portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Shared/Trees");
            portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Shared/Frozen Fields");
            portalDatFile.writeToDat("./client_portal.dat");
        }

        static void MapManipulationForInfiltrationAceClassicCustom(string[] args)
        {
            cDatFile datFile = new cDatFile();
            //int retailIteration = datFile.loadFromDat("./input/cell - 2005-02-XX (202752kb) (Admin) (Iteration 1593 - Complete).dat");
            //datFile.convertRetailToToD(retailIteration);
            datFile.loadFromDat("./input/client_cell_1 - Infiltration - Converted to ToD format (Iteration 1593).dat"); // Same as above but already converted to ToD format to speed thing up.
            datFile.SetFileIteration(40001);

            cDatFile datFileToD = new cDatFile();
            datFileToD.loadFromDat("./input/client_cell_1 - ToD.dat");

            //datFile.replaceLandblockArea(0x2B110028, datFileToD); // Candeth Keep crash
            datFile.fixLandblockCells(0x2B110000, datFileToD); // Candeth Keep crash
            datFile.fixLandblockCells(0x2B120000, datFileToD); // Candeth Keep crash
            datFile.fixLandblockCells(0x2D5B002B, datFileToD); // Renegade Fortress crash
            //datFile.fixAllLandblockCells(datFileToD);

            datFile.replaceLandblock(0xEC0E0110, datFileToD); // Xi Ru's Chapel crash
            //datFile.findEnvironmentIdInCells();

            cDatFile datFileIceIsland = new cDatFile();
            datFileIceIsland.loadFromDat("./input/cell - 2000-08-22 (58368kb) (Complete,Ice Island, Shadow Spires) (Iteration 108 - Complete).dat");

            var iceIsland = new List<uint>();
            iceIsland.AddRange(datFileIceIsland.getListOfLandblocksInRect(0x54FB, 0x73EC));
            iceIsland = iceIsland.Except(datFileIceIsland.getListOfLandblocksInRect(0x6BF2, 0x74EC)).ToList();
            iceIsland = iceIsland.Except(datFileIceIsland.getListOfLandblocksInRect(0x62F0, 0x74EC)).ToList();
            datFile.replaceLandblockList(0xC711, iceIsland, datFileIceIsland); // Ice Island

            cDatFile datFileEoR = new cDatFile();
            datFileEoR.loadFromDat("./input/client_cell_1 - EoR.dat");

            datFile.replaceLandblock(0x0066, datFileEoR, 0xC700); // PK Arena 1: @teleloc 0xC7000115 [33.252853 -25.985340 0.005000] 0.983027 0.000000 0.000000 -0.183464
            datFile.replaceLandblock(0x0066, datFileEoR, 0xC800); // PK Arena 2: @teleloc 0xC8000115 [33.252853 -25.985340 0.005000] 0.983027 0.000000 0.000000 -0.183464
            datFile.replaceLandblock(0x0066, datFileEoR, 0xC900); // PK Arena 3: @teleloc 0xC9000115 [33.252853 -25.985340 0.005000] 0.983027 0.000000 0.000000 -0.183464
            datFile.replaceLandblock(0x0066, datFileEoR, 0xCA00); // PK Arena 4: @teleloc 0xCA000115 [33.252853 -25.985340 0.005000] 0.983027 0.000000 0.000000 -0.183464

            cCellDat cellDat = new cCellDat();
            cellDat.loadFromDat(datFile);
            cMapDrawer mapDrawer = new cMapDrawer(cellDat);
            mapDrawer.draw(true, 50);

            datFile.writeToDat("./client_cell_1.dat");
        }
        static void PortalManipulationForEvensong(string[] args)
        {
            cDatFile portalDatFile = new cDatFile();
            //portalDatFile.loadFromDat("./input/client_portal - EoR.dat");
            portalDatFile.loadFromDat("./input/client_portal - Evensong.dat");
            portalDatFile.SetFileIteration(30001);
            portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Infiltration/");
            portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Shared/Textures");
            portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Evensong/");
            portalDatFile.addFile("./Dat Builder/Portal/CustomDM/01000CFA - Lifestone.bin");
            portalDatFile.addFile("./Dat Builder/Portal/CustomDM/01000D00 - Lifestone.bin");
            portalDatFile.addFile("./Dat Builder/Portal/CustomDM/01000D02 - Lifestone.bin");
            portalDatFile.addFile("./Dat Builder/Portal/CustomDM/01000D06 - Lifestone.bin");
            //portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Shared/Resized EoR Trees");
            portalDatFile.addFilesFromFolder("./Dat Builder/Portal/Shared/Trees");
            portalDatFile.writeToDat("./client_portal.dat");
        }

        static void MapManipulationForEvensong(string[] args)
        {
            cDatFile datFile = new cDatFile();
            //int retailIteration = datFile.loadFromDat("./input/cell - 2005-02-XX (202752kb) (Admin) (Iteration 1593 - Complete).dat");
            //datFile.convertRetailToToD(retailIteration);
            //datFile.loadFromDat("./input/client_cell_1 - Infiltration - Converted to ToD format (Iteration 1593).dat"); // Same as above but already converted to ToD format to speed thing up.
            datFile.loadFromDat("./input/client_cell_1 - Evensong.dat");
            datFile.SetFileIteration(30001);

            cDatFile datFileRelease = new cDatFile();
            datFileRelease.loadFromDat("./input/cell - Release.dat");

            cDatFile datFileArwic = new cDatFile();
            datFileArwic.loadFromDat("./input/cell - 2000-08-22 (58368kb) (Complete,Ice Island, Shadow Spires) (Iteration 108 - Complete).dat");

            cDatFile datFileObsidianSpan = new cDatFile();
            datFileObsidianSpan.loadFromDat("./input/cell - 2000-12-31 - Obsidian Span.dat");

            cDatFile datFileTufa = new cDatFile();
            datFileTufa.loadFromDat("./input/cell - 2000-07-26 (Iteration 91 - 98.57%).dat");

            cDatFile datFileToD = new cDatFile();
            datFileToD.loadFromDat("./input/client_cell_1 - ToD.dat");

            //Arwic
            datFile.replaceLandblock(0xC6A90013, datFileRelease);
            datFile.replaceLandblock(0xC6A90008, datFileRelease);
            datFile.replaceLandblock(0xC6A80038, datFileRelease);
            datFile.replaceLandblock(0xC6AA0011, datFileRelease);
            datFile.replaceLandblock(0xC5AA0039, datFileRelease);
            datFile.replaceLandblock(0xC5A80028, datFileRelease);

            datFile.replaceLandblock(0xC5A90037, datFileArwic); // Meeting Hall with the correct fallen pillars around the road.
            datFile.replaceLandblock(0xC6A90013, datFileArwic, false, false, true, false); // use the surface objects from this file(lamp posts!) but not the buildings as they have fire lighting effects that bleed all over thru the walls on the new client.

            datFile.replaceLandblock(0xC5A6003A, datFileRelease); // Get rid of Newic.

            //Yanshi
            datFile.replaceLandblock(0xB86E0038, datFileRelease);
            datFile.replaceLandblock(0xB86F0032, datFileRelease);
            datFile.replaceLandblock(0xB8700029, datFileRelease);
            datFile.replaceLandblock(0xB9700024, datFileRelease);
            datFile.replaceLandblock(0xB96F0040, datFileRelease);
            datFile.replaceLandblock(0xB96E0018, datFileRelease);
            datFile.replaceLandblock(0xBA6F0004, datFileRelease);
            datFile.replaceLandblock(0xBA700001, datFileRelease);

            //Get rid of New Yanshi
            datFile.replaceLandblock(0xB9720022, datFileRelease);
            datFile.replaceLandblock(0xBA720005, datFileRelease);
            datFile.replaceLandblock(0xB9710030, datFileRelease);

            datFile.replaceLandblock(0xB76F0034, datFileArwic); //Yanshi Meeting Hall

            //Tufa
            datFile.replaceLandblock(0x866D000B, datFileTufa);
            datFile.replaceLandblock(0x866E0011, datFileTufa);
            datFile.replaceLandblock(0x856D003C, datFileTufa);
            datFile.replaceLandblock(0x856E0031, datFileTufa);
            datFile.replaceLandblock(0x856C001C, datFileTufa);
            datFile.replaceLandblock(0x876E0032, datFileTufa);
            datFile.replaceLandblock(0x856F0015, datFileTufa);
            datFile.replaceLandblock(0x846E0012, datFileRelease);
            datFile.replaceLandblock(0x846D0018, datFileRelease);
            datFile.replaceLandblock(0x836D0040, datFileRelease);
            datFile.replaceLandblock(0x836E0039, datFileRelease);

            datFile.addBuildingFrom(0x856E010E, datFileToD); //Undead Hunter Tent

            datFile.replaceLandblock(0x846D003F, datFileArwic, false, false, true, true); //Tufa Meeting Hall, buildings only, terrain includes crater.

            ////Eastham Shadow Spire Crater
            //datFile.replaceLandblock(0xCD95002F, datFileRelease);
            //datFile.replaceLandblock(0xCE950006, datFileRelease, true, true, false, false);
            //datFile.replaceLandblock(0xCE960001, datFileRelease);
            //datFile.replaceLandblock(0xCD960039, datFileRelease);
            //datFile.removeBuilding(0xCE95016C);

            //datFile.replaceLandblockArea(0x2B110028, datFileToD); // Candeth Keep crash
            datFile.fixLandblockCells(0x2B110000, datFileToD); // Candeth Keep crash
            datFile.fixLandblockCells(0x2B120000, datFileToD); // Candeth Keep crash
            datFile.fixLandblockCells(0x2D5B002B, datFileToD); // Renegade Fortress crash
            //datFile.fixAllLandblockCells(datFileToD);

            datFile.replaceLandblock(0xEC0E0110, datFileToD); // Xi Ru's Chapel crash

            datFile.removeNoobFence();

            List<uint> LandblocksToReplace = new List<uint>()
            {
                0x905C003B, // North Al-Arqas Outpost
                0x915C0003, // North Al-Arqas Outpost part 2
                0x8C58002B, // West Al-Arqas Outpost
                0xC380000C, // East Lytelthorpe Outpost
                0xBB80002E, // West Lytelthorpe Outpost
                0xEA3D000E, // East Nanto Outpost
                0xE542002B, // North Nanto Outpost
                0xCC8C0014, // East Rithwic Outpost
                0xC8880016, // South Rithwic Outpost
                0x9B7B0015, // East Samsur Outpost
                0x937F002D, // Northwest Samsur Outpost
                0xB9730032, // North Yanshi Outpost
                0xB96B001E, // South Yanshi Outpost
            };
            datFile.replaceLandblocksSpecialForStarterOutposts(LandblocksToReplace, datFileObsidianSpan);

            List<uint> buildingsToRemove = new List<uint>()
            {
                0xA9B30114, // Holtburg->Neydisa Castle Portal Bunker
                0xA9B40183, // Holtburg->Shoushi Portal Bunker
                0xA9B40188, // Holtburg->Rithwic Portal Bunker
                0xA9B4017E, // Holtburg->Cragstone Portal Bunker
                0xAAB40108, // Holtburg->Arwic Portal Bunker
                0xAAB40103, // Holtburg->Dryreach Portal Bunker

                0x7D64018B, // Yaraq->Holtburg Portal Bunker
                0x7D64017C, // Yaraq->Al-Arqas Portal Bunker
                0x7D640181, // Yaraq->Samsur Portal Bunker
                0x7E640122, // Yaraq->Zaikhal Portal Bunker
                0x7E640127, // Yaraq->Linvak Tukal Portal Bunker
                0x7D640177, // Yaraq->Khayyaban Portal Bunker
                0x7D640186, // Yaraq->Xarabydum Portal Bunker

                0xDA560116, // Shoushi->Kryst Portal Bunker
                0xDA5501F9, // Shoushi->Nanto Portal Bunker
                0xDA56011B, // Shoushi->Kara Portal Bunker
                0xDA5501F4, // Shoushi->Yanshi Portal Bunker
                0xDA540103, // Shoushi->Tou-Tou Portal Bunker
                0xDA5501EF, // Shoushi->Yaraq Portal Bunker
                0xD955010D, // Shoushi->Hebian-To Portal Bunker

                ////Statues
                //0x90580141, // Al-Arqas
                //0xA9B40177, // Holtburg
                //0xBF80017D, // Lytelthorpe
                //0xE63D0154, // Nanto
                //0xC88C0171, // Rithwic
                //0x977B015F, // Samsur
                //0xDA5501E8, // Shoushi
                //0x7D640151, // Yaraq
                //0xBC9F0166, // Cragstone
                //0x85880129, // Al-Jalima
                //0xCE95016A, // Eastham
                //0xA1A40162, // Glenden Wood
                //0xE74E01A7, // Hebian-To
                //0x9E430132, // Khayyaban
                //0xE9220141, // Kryst
                //0xDA3B013C, // Lin
                //0xA2600122, // Uziz
                //0x80900139, // Zhaikal
                //0xF75C0155, // Tou-Tou
                //0xCD41018B, // Baishi
                //0xF2220122, // MacNiall's Freehold
                //0xE5320136, // Mayoi
                //0x49B6012D, // Plateau Village
                //0x9722015F, // Qalaba'r
                //0xC95B0169, // Sawato
                //0x64D50131, // Stonehold
                //0x866C010B, // Tufa
                //0x1134014A, // Ayan Baqur
                //0xBA170143, // Kara
                //0x3F310101, // Wai Jhou
                //0x2581016B, // Fort Tethana

                ////Arcanum Buildings
                //0x90570102, // Al-Arqas
                //0xA9B40171, // Holtburg
                //0xBE800109, // Lytelthorpe
                //0xE63D014E, // Nanto
                //0xC88B0102, // Rithwic
                //0x977B0159, // Samsur
                //0xDA56010E, // Shoushi
                //0x7D64014B, // Yaraq
                //0xBA9E0105, // Cragstone
                //0x85880123, // Al-Jalima
                //0xCE950163, // Eastham
                //0xA2A40100, // Glenden Wood
                //0xE64E0104, // Hebian-To
                //0x9E43012D, // Khayyaban
                //0xE8210104, // Kryst
                //0xDB3B011D, // Lin
                //0xA260011C, // Uziz
                //0x7F8F011C, // Zhaikal
                //0xF75C014F, // Tou-Tou
                //0xCD410183, // Baishi
                //0xF2230104, // MacNiall's Freehold
                //0xE532012E, // Mayoi
                //0x49B60126, // Plateau Village
                //0x97220159, // Qalaba'r
                //0xC95B0161, // Sawato
                //0x64D50129, // Stonehold
                //0x11340144, // Ayan Baqur
                //0xBA17013B, // Kara
            };
            datFile.removeBuildings(buildingsToRemove);
            //Since we're removing the bunkers might as well roll back Holtburg's heightmap that was flattened in areas to fit the bunkers.
            datFile.replaceLandblockTerrain(0xA9B40024, datFileObsidianSpan, true, true);
            datFile.replaceLandblockTerrain(0xAAB4000A, datFileObsidianSpan, true, true);
            datFile.replaceLandblockTerrain(0xA8B4003B, datFileObsidianSpan, true, true);

            cCellDat cellDat = new cCellDat();
            cellDat.loadFromDat(datFile);
            cMapDrawer mapDrawer = new cMapDrawer(cellDat);
            mapDrawer.draw(false, 50);

            datFile.writeToDat("./client_cell_1.dat");
        }

        static void GeneratePhatACLootFiles(string[] args)
        {
            bool invalidArgs = false;

            if (args.Length == 0)
                invalidArgs = true;

            if (!invalidArgs)
            {
                switch (args[0].ToLower())
                {
                    case "cached":
                        {
                            string fileLootProfile;
                            string file0002;
                            string file0009;

                            if (args.Length >= 3)
                            {
                                file0002 = Path.Combine(args[1], "0002.raw");
                                file0009 = Path.Combine(args[1], "0009.raw");
                                fileLootProfile = args[2];

                                if (!File.Exists(fileLootProfile))
                                {
                                    Console.WriteLine("Invalid file: {0}", fileLootProfile);
                                    Console.ReadLine();
                                    return;
                                }
                                else if (!File.Exists(file0009))
                                {
                                    Console.WriteLine("Invalid file: {0}", file0009);
                                    Console.ReadLine();
                                    return;
                                }
                                else if (!File.Exists(file0002))
                                {
                                    Console.WriteLine("Invalid file: {0}", file0002);
                                    Console.ReadLine();
                                    return;
                                }
                            }
                            else
                            {
                                invalidArgs = true;
                                break;
                            }

                            sLootProfile lootProfile = cache9Converter.generateRandomLoot(file0009, file0002, fileLootProfile, "./output/cached/", true, true, false, true);//write everything to raw

                            Console.WriteLine("Waiting while CachePwn builds cache.bin...");
                            Stopwatch timer = new Stopwatch();
                            timer.Start();
                            Process cachePwnProcess = new Process();
                            string path = AppDomain.CurrentDomain.BaseDirectory;
                            cachePwnProcess.StartInfo.FileName = $"{path}CachePwn.exe";
                            cachePwnProcess.StartInfo.Arguments = "2 .\\intermediate .\\output\\cached";
                            cachePwnProcess.Start();
                            cachePwnProcess.WaitForExit();
                            timer.Stop();
                            Console.WriteLine("Finished in {0} seconds.", timer.ElapsedMilliseconds / 1000f);

                            if (lootProfile.otherOptions.copyOutputToFolder != "")
                            {
                                if (!Directory.Exists(lootProfile.otherOptions.copyOutputToFolder))
                                    Console.WriteLine("Invalid copyOutputToFolder: {0}", lootProfile.otherOptions.copyOutputToFolder);
                                else
                                {
                                    Console.WriteLine("Copying output to \"{0}\"...", lootProfile.otherOptions.copyOutputToFolder);
                                    timer.Reset();
                                    timer.Start();

                                    Utils.copyDirectory(".\\output\\cached", lootProfile.otherOptions.copyOutputToFolder, true, true);

                                    timer.Stop();
                                    Console.WriteLine("Finished in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
                                }
                            }
                            return;
                        }
                    case "split":
                        {
                            string fileLootProfile;
                            string file0002;
                            string file0009;

                            if (args.Length >= 3)
                            {
                                file0002 = Path.Combine(args[1], "0002.raw");
                                file0009 = Path.Combine(args[1], "0009.raw");
                                fileLootProfile = args[2];

                                if (!File.Exists(fileLootProfile))
                                {
                                    Console.WriteLine("Invalid file: {0}", fileLootProfile);
                                    Console.ReadLine();
                                    return;
                                }
                                else if (!File.Exists(file0009))
                                {
                                    Console.WriteLine("Invalid file: {0}", file0009);
                                    Console.ReadLine();
                                    return;
                                }
                                else if (!File.Exists(file0002))
                                {
                                    Console.WriteLine("Invalid file: {0}", file0002);
                                    Console.ReadLine();
                                    return;
                                }
                            }
                            else
                            {
                                invalidArgs = true;
                                break;
                            }
                            sLootProfile lootProfile = cache9Converter.generateRandomLoot(file0009, file0002, fileLootProfile, "./output/split/", false, true, false, true);//write creature entries to raw
                            cache9Converter.generateRandomLoot(file0009, "", fileLootProfile, "./output/split/json/weenies/", true, false, true, false);//write items to json

                            Console.WriteLine("Waiting while CachePwn builds cache.bin...");
                            Stopwatch timer = new Stopwatch();
                            timer.Start();
                            Process cachePwnProcess = new Process();
                            string path = AppDomain.CurrentDomain.BaseDirectory;
                            cachePwnProcess.StartInfo.FileName = $"{path}CachePwn.exe";
                            cachePwnProcess.StartInfo.Arguments = "2 .\\intermediate .\\output\\split";
                            cachePwnProcess.Start();
                            cachePwnProcess.WaitForExit();
                            timer.Stop();
                            Console.WriteLine("Finished in {0} seconds.", timer.ElapsedMilliseconds / 1000f);

                            if (lootProfile.otherOptions.copyOutputToFolder != "")
                            {
                                if (!Directory.Exists(lootProfile.otherOptions.copyOutputToFolder))
                                    Console.WriteLine("Invalid copyOutputToFolder: {0}", lootProfile.otherOptions.copyOutputToFolder);
                                else
                                {
                                    Console.WriteLine("Copying output to \"{0}\"...", lootProfile.otherOptions.copyOutputToFolder);
                                    timer.Reset();
                                    timer.Start();

                                    Utils.copyDirectory(".\\output\\split", lootProfile.otherOptions.copyOutputToFolder, true, true);

                                    timer.Stop();
                                    Console.WriteLine("Finished in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
                                }
                            }
                            return;
                        }
                    case "json":
                        {
                            string fileLootProfile;
                            string file0002;
                            string file0009;

                            if (args.Length >= 3)
                            {
                                file0002 = Path.Combine(args[1], "0002.raw");
                                file0009 = Path.Combine(args[1], "0009.raw");
                                fileLootProfile = args[2];

                                if (!File.Exists(fileLootProfile))
                                {
                                    Console.WriteLine("Invalid file: {0}", fileLootProfile);
                                    Console.ReadLine();
                                    return;
                                }
                                else if (!File.Exists(file0009))
                                {
                                    Console.WriteLine("Invalid file: {0}", file0009);
                                    Console.ReadLine();
                                    return;
                                }
                                else if (!File.Exists(file0002))
                                {
                                    Console.WriteLine("Invalid file: {0}", file0002);
                                    Console.ReadLine();
                                    return;
                                }
                            }
                            else
                            {
                                invalidArgs = true;
                                break;
                            }
                            cache9Converter.generateRandomLoot(file0009, file0002, fileLootProfile, "./output/json/", true, true, true, false);//write everything to json
                            return;
                        }
                    case "loottablesjson":
                        {
                            string fileLootProfile;
                            string file0002;
                            string file0009;

                            if (args.Length >= 3)
                            {
                                file0002 = Path.Combine(args[1], "0002.raw");
                                file0009 = Path.Combine(args[1], "0009.raw");
                                fileLootProfile = args[2];

                                if (!File.Exists(fileLootProfile))
                                {
                                    Console.WriteLine("Invalid file: {0}", fileLootProfile);
                                    Console.ReadLine();
                                    return;
                                }
                                else if (!File.Exists(file0009))
                                {
                                    Console.WriteLine("Invalid file: {0}", file0009);
                                    Console.ReadLine();
                                    return;
                                }
                                else if (!File.Exists(file0002))
                                {
                                    Console.WriteLine("Invalid file: {0}", file0002);
                                    Console.ReadLine();
                                    return;
                                }
                            }
                            else
                            {
                                invalidArgs = true;
                                break;
                            }
                            cache9Converter.generateRandomLoot(file0009, file0002, fileLootProfile, "./output/json/", false, true, true, false);//write loot tables to json
                            return;
                        }
                    case "loottables":
                        {
                            string fileLootProfile;
                            string file0002;
                            string file0009;

                            if (args.Length >= 3)
                            {
                                file0002 = Path.Combine(args[1], "0002.raw");
                                file0009 = Path.Combine(args[1], "0009.raw");
                                fileLootProfile = args[2];

                                if (!File.Exists(fileLootProfile))
                                {
                                    Console.WriteLine("Invalid file: {0}", fileLootProfile);
                                    Console.ReadLine();
                                    return;
                                }
                                else if (!File.Exists(file0009))
                                {
                                    Console.WriteLine("Invalid file: {0}", file0009);
                                    Console.ReadLine();
                                    return;
                                }
                                else if (!File.Exists(file0002))
                                {
                                    Console.WriteLine("Invalid file: {0}", file0002);
                                    Console.ReadLine();
                                    return;
                                }
                            }
                            else
                            {
                                invalidArgs = true;
                                break;
                            }
                            cache9Converter.generateRandomLoot(file0009, file0002, fileLootProfile, "./output/split/", false, true, false, true);//write loot tables to raw
                            return;
                        }
                    case "weenies":
                        {
                            string file0009;

                            if (args.Length >= 2)
                            {
                                file0009 = Path.Combine(args[1], "0009.raw");

                                if (!File.Exists(file0009))
                                {
                                    Console.WriteLine("Invalid file: {0}", file0009);
                                    Console.ReadLine();
                                    return;
                                }
                            }
                            else
                            {
                                invalidArgs = true;
                                break;
                            }
                            cache9Converter.writeJson(file0009, "./output/weenies/", false);
                            return;
                        }
                    case "landblocks":
                        {
                            string file0006;
                            string file0009;

                            if (args.Length >= 2)
                            {
                                file0006 = Path.Combine(args[1], "0006.raw");
                                file0009 = Path.Combine(args[1], "0009.raw");

                                if (!File.Exists(file0006))
                                {
                                    Console.WriteLine("Invalid file: {0}", file0006);
                                    Console.ReadLine();
                                    return;
                                }
                                else if (!File.Exists(file0009))
                                {
                                    Console.WriteLine("Invalid file: {0}", file0009);
                                    Console.ReadLine();
                                    return;
                                }
                            }
                            else
                            {
                                invalidArgs = true;
                                break;
                            }
                            cache9Converter.loadWeeniesRaw(file0009);
                            cache6Converter.loadFromRaw(file0006);
                            cache6Converter.writeJson("./output/landblocks");
                            return;
                        }
                    case "questflags":
                        {
                            string file0008;

                            if (args.Length >= 2)
                            {
                                file0008 = Path.Combine(args[1], "0008.raw");

                                if (!File.Exists(file0008))
                                {
                                    Console.WriteLine("Invalid file: {0}", file0008);
                                    Console.ReadLine();
                                    return;
                                }
                            }
                            else
                            {
                                invalidArgs = true;
                                break;
                            }
                            cache8Converter.loadFromRaw(file0008);
                            cache8Converter.writeJson("./output");
                            return;
                        }
                    case "raw2json":
                        {
                            string file0009;

                            if (args.Length >= 2)
                            {
                                file0009 = Path.Combine(args[1], "0009.raw");

                                if (!File.Exists(file0009))
                                {
                                    Console.WriteLine("Invalid file: {0}", file0009);
                                    Console.ReadLine();
                                    return;
                                }
                            }
                            else
                            {
                                invalidArgs = true;
                                break;
                            }
                            cache9Converter.writeExtendedJson(file0009, "./output/extended weenies/", false);
                            return;
                        }
                    case "json2raw":
                        {
                            string folderExtendedWeenies;

                            if (args.Length >= 2)
                            {
                                folderExtendedWeenies = args[1];

                                if (!Directory.Exists(folderExtendedWeenies))
                                {
                                    Console.WriteLine("Invalid folder: {0}", folderExtendedWeenies);
                                    Console.ReadLine();
                                    return;
                                }
                            }
                            else
                            {
                                invalidArgs = true;
                                break;
                            }

                            cache9Converter.writeRawFromExtendedJson(folderExtendedWeenies);

                            Console.WriteLine("Waiting while CachePwn builds cache.bin...");
                            Stopwatch timer = new Stopwatch();
                            timer.Start();
                            Process cachePwnProcess = new Process();
                            string path = AppDomain.CurrentDomain.BaseDirectory;
                            cachePwnProcess.StartInfo.FileName = $"{path}CachePwn.exe";
                            cachePwnProcess.StartInfo.Arguments = "2 .\\intermediate \".\\output\\extended weenies\"";
                            cachePwnProcess.Start();
                            cachePwnProcess.WaitForExit();
                            timer.Stop();
                            Console.WriteLine("Finished in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
                            return;
                        }
                }
            }

            if (invalidArgs)
            {
                Console.WriteLine("Invalid arguments.");
                Console.WriteLine("Valid Modes:");
                Console.WriteLine("    Cached: produces a cache.bin file that contains both the generated loot and modified creatures and chests entries. Due to a limitation this file can only store up to 26550 new entries.");
                Console.WriteLine("        Usage: (mode) (inputFolder) (profileFile)");
                Console.WriteLine("    Split: produces a cache.bin file that contains the modified creatures and chests entries, and json files for the generated loot. Thus avoiding the limitation mentioned above but increasing server starting times considerably.");
                Console.WriteLine("        Usage: (mode) (inputFolder) (profileFile)");
                Console.WriteLine("    lootTables");
                Console.WriteLine("        Usage: (mode) (inputFolder) (profileFile)");
                Console.WriteLine("    Json: no cache.bin is produced, instead all modified entries are created as json.");
                Console.WriteLine("        Usage: (mode) (inputFolder) (profileFile)");
                Console.WriteLine("    lootTablesJson");
                Console.WriteLine("        Usage: (mode) (inputFolder) (profileFile)");
                Console.WriteLine("    raw2json: generate extended weenies that can later be reconverted into cached files.");
                Console.WriteLine("        Usage: (mode) (inputFolder)");
                Console.WriteLine("    json2raw: generate cache.bin file from extended weenies");
                Console.WriteLine("        Usage: (mode) (inputFolder)");
                Console.WriteLine("    Weenies: generate commented weenies files.");
                Console.WriteLine("        Usage: (mode) (inputFolder)");
                Console.WriteLine("    Landblocks: generate landblock files.");
                Console.WriteLine("        Usage: (mode) (inputFolder)");
                Console.ReadLine();
            }
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
            SortedDictionary<int, int> valueDistributionRounded = Utils.distributionRounding(valueDistribution);
            foreach (KeyValuePair<int, int> entry in valueDistributionRounded)
            {
                Console.WriteLine($"value: {entry.Key} amount: {entry.Value} percent: {entry.Value * 100d / testRolls}%");
            }
            Console.WriteLine("---");
        }
    }
}
