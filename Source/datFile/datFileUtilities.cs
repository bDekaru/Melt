using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Melt
{
    public partial class cDatFile
    {
        class cAmbiguousValues
        {
            public ushort newId;
            public List<ushort> alternateNewIds;
        }

        class cAmbiguousUintValues
        {
            public uint newId;
            public List<uint> alternateNewIds;
        }

        public void buildTextureIdMigrationTable(cDatFile otherDat)
        {
            Dictionary<uint, cEnvCell> thisEnvCells = new Dictionary<uint, cEnvCell>(); ;
            Dictionary<uint, cEnvCell> otherEnvCells = new Dictionary<uint, cEnvCell>(); ;
            SortedDictionary<ushort, ushort> idMigrationTable = new SortedDictionary<ushort, ushort>(); ;

            SortedDictionary<ushort, cAmbiguousValues> ambiguousList = new SortedDictionary<ushort, cAmbiguousValues>();

            List<ushort> allOldIds = new List<ushort>();
            SortedDictionary<ushort, ushort> missingList = new SortedDictionary<ushort, ushort>();

            foreach (KeyValuePair<uint, cDatFileEntry> entry in fileCache)
            {
                if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF) //surface
                {
                }
                else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE) //surface objects
                {
                }
                else //dungeons and interiors
                {
                    cEnvCell thisEnvCell = new cEnvCell(entry.Value);

                    thisEnvCells.Add(entry.Value.fileId, thisEnvCell);
                }
            }

            foreach (KeyValuePair<uint, cDatFileEntry> entry in otherDat.fileCache)
            {
                if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF) //surface
                {
                }
                else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE) //surface objects
                {
                }
                else //dungeons and interiors
                {
                    cEnvCell otherEnvCell = new cEnvCell(entry.Value, false);

                    otherEnvCells.Add(entry.Value.fileId, otherEnvCell);
                }
            }

            foreach (KeyValuePair<uint, cEnvCell> entry in otherEnvCells)
            {
                cEnvCell thisEnvCell;
                cEnvCell otherEnvCell = entry.Value;

                if (!thisEnvCells.TryGetValue(otherEnvCell.Id, out thisEnvCell))
                    continue;

                for (int i = 0; i < otherEnvCell.Textures.Count; i++)
                {
                    ushort oldId = otherEnvCell.Textures[i];
                    if (!allOldIds.Contains(oldId))
                        allOldIds.Add(oldId);
                }

                if (compareEnvCells(thisEnvCell, otherEnvCell))
                {
                    for (int i = 0; i < otherEnvCell.Textures.Count; i++)
                    {
                        ushort oldId = otherEnvCell.Textures[i];
                        ushort newId = thisEnvCell.Textures[i];

                        ushort currentId = 0;
                        idMigrationTable.TryGetValue(oldId, out currentId);

                        if (currentId == 0)
                            idMigrationTable.Add(oldId, newId);
                        else
                        {
                            if (newId != currentId)
                            {
                                cAmbiguousValues ambiguousId;
                                if (!ambiguousList.TryGetValue(oldId, out ambiguousId))
                                {
                                    ambiguousId = new cAmbiguousValues();
                                    ambiguousId.newId = currentId;
                                    ambiguousId.alternateNewIds = new List<ushort>();
                                    ambiguousList.Add(oldId, ambiguousId);
                                    ambiguousId.alternateNewIds.Add(newId);
                                }
                                else
                                {
                                    if (!ambiguousId.alternateNewIds.Contains(newId))
                                        ambiguousId.alternateNewIds.Add(newId);
                                }
                                //throw new Exception("ambiguous texture id migration");
                            }
                        }
                    }
                }
            }

            foreach (ushort entry in allOldIds)
            {
                if (!idMigrationTable.ContainsKey(entry))
                {
                    missingList.Add(entry, entry);
                }
            }

            StreamWriter outputFile = new StreamWriter(new FileStream("textureIdMigrationTable.txt", FileMode.Create, FileAccess.Write));
            foreach (KeyValuePair<ushort, ushort> entry in idMigrationTable)
            {
                outputFile.WriteLine($"{entry.Key.ToString("x4")} {entry.Value.ToString("x4")}");
                outputFile.Flush();
            }
            outputFile.Close();

            outputFile = new StreamWriter(new FileStream("textureIdMigrationMissingConversions.txt", FileMode.Create, FileAccess.Write));
            foreach (KeyValuePair<ushort, ushort> entry in missingList)
            {
                outputFile.WriteLine(entry.Key.ToString("x4"));
                outputFile.Flush();
            }
            outputFile.Close();

            outputFile = new StreamWriter(new FileStream("textureIdMigrationTableAmbiguous.txt", FileMode.Create, FileAccess.Write));
            foreach (KeyValuePair<ushort, cAmbiguousValues> entry in ambiguousList)
            {
                outputFile.Write($"{entry.Key.ToString("x4")} {entry.Value.newId.ToString("x4")}");
                bool first = true;
                foreach (ushort value in entry.Value.alternateNewIds)
                {
                    if (first)
                    {
                        outputFile.Write("(");
                        first = false;
                    }
                    else
                        outputFile.Write(", ");
                    outputFile.Write(value.ToString("x4"));
                    outputFile.Flush();
                }
                outputFile.WriteLine(")");

                outputFile.Flush();
            }
            outputFile.Close();
        }

        public void buildObjectIdMigrationTable(cDatFile otherDat)
        {
            Dictionary<uint, cLandblockInfo> thisLandblockInfoMap = new Dictionary<uint, cLandblockInfo>();
            Dictionary<uint, cLandblockInfo> otherLandblockInfoMap = new Dictionary<uint, cLandblockInfo>();

            Dictionary<uint, cEnvCell> thisEnvCells = new Dictionary<uint, cEnvCell>();
            Dictionary<uint, cEnvCell> otherEnvCells = new Dictionary<uint, cEnvCell>();
            SortedDictionary<uint, uint> idMigrationTable = new SortedDictionary<uint, uint>();

            SortedDictionary<uint, cAmbiguousUintValues> ambiguousList = new SortedDictionary<uint, cAmbiguousUintValues>();

            List<uint> allOldIds = new List<uint>();
            SortedDictionary<uint, uint> missingList = new SortedDictionary<uint, uint>();

            foreach (KeyValuePair<uint, cDatFileEntry> entry in fileCache)
            {
                if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF) //surface
                {
                }
                else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE) //surface objects
                {
                    cLandblockInfo thisLandblockInfo = new cLandblockInfo(entry.Value);

                    thisLandblockInfoMap.Add(entry.Value.fileId, thisLandblockInfo);
                }
                else //dungeons and interiors
                {
                    cEnvCell thisEnvCell = new cEnvCell(entry.Value);

                    thisEnvCells.Add(entry.Value.fileId, thisEnvCell);
                }
            }

            foreach (KeyValuePair<uint, cDatFileEntry> entry in otherDat.fileCache)
            {
                if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF) //surface
                {
                }
                else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE) //surface objects
                {
                    cLandblockInfo otherLandlockInfo = new cLandblockInfo(entry.Value);

                    otherLandblockInfoMap.Add(entry.Value.fileId, otherLandlockInfo);
                }
                else //dungeons and interiors
                {
                    cEnvCell otherEnvCell = new cEnvCell(entry.Value, false);

                    otherEnvCells.Add(entry.Value.fileId, otherEnvCell);
                }
            }

            foreach (KeyValuePair<uint, cLandblockInfo> entry in otherLandblockInfoMap)
            {
                cLandblockInfo thisLandblockInfo;
                cLandblockInfo otherLandblockInfo = entry.Value;

                if (!thisLandblockInfoMap.TryGetValue(otherLandblockInfo.Id, out thisLandblockInfo))
                    continue;

                for (int i = 0; i < otherLandblockInfo.Objects.Count; i++)
                {
                    uint oldId = otherLandblockInfo.Objects[i].id;
                    if (!allOldIds.Contains(oldId))
                        allOldIds.Add(oldId);
                }

                if (compareLandblockInfo(thisLandblockInfo, otherLandblockInfo))
                {
                    for (int i = 0; i < otherLandblockInfo.Objects.Count; i++)
                    {
                        uint oldId = otherLandblockInfo.Objects[i].id;
                        uint newId = thisLandblockInfo.Objects[i].id;

                        uint currentId = 0;

                        if (!idMigrationTable.TryGetValue(oldId, out currentId))
                            idMigrationTable.Add(oldId, newId);
                        else
                        {
                            if (newId != currentId)
                            {
                                cAmbiguousUintValues ambiguousId;
                                if (!ambiguousList.TryGetValue(oldId, out ambiguousId))
                                {
                                    ambiguousId = new cAmbiguousUintValues();
                                    ambiguousId.newId = currentId;
                                    ambiguousId.alternateNewIds = new List<uint>();
                                    ambiguousList.Add(oldId, ambiguousId);
                                    ambiguousId.alternateNewIds.Add(newId);
                                }
                                else
                                {
                                    if (!ambiguousId.alternateNewIds.Contains(newId))
                                        ambiguousId.alternateNewIds.Add(newId);
                                }
                                //throw new Exception("ambiguous texture id migration");
                            }
                        }
                    }
                }
            }

            foreach (KeyValuePair<uint, cEnvCell> entry in otherEnvCells)
            {
                cEnvCell thisEnvCell;
                cEnvCell otherEnvCell = entry.Value;

                if (!thisEnvCells.TryGetValue(otherEnvCell.Id, out thisEnvCell))
                    continue;

                for (int i = 0; i < otherEnvCell.Stabs.Count; i++)
                {
                    uint oldId = otherEnvCell.Stabs[i].id;
                    if (!allOldIds.Contains(oldId))
                        allOldIds.Add(oldId);
                }

                if (compareEnvCells(thisEnvCell, otherEnvCell, true))
                {
                    for (int i = 0; i < otherEnvCell.Stabs.Count; i++)
                    {
                        uint oldId = otherEnvCell.Stabs[i].id;
                        uint newId = thisEnvCell.Stabs[i].id;

                        uint currentId = 0;

                        if (!idMigrationTable.TryGetValue(oldId, out currentId))
                            idMigrationTable.Add(oldId, newId);
                        else
                        {
                            if (newId != currentId)
                            {
                                cAmbiguousUintValues ambiguousId;
                                if (!ambiguousList.TryGetValue(oldId, out ambiguousId))
                                {
                                    ambiguousId = new cAmbiguousUintValues();
                                    ambiguousId.newId = currentId;
                                    ambiguousId.alternateNewIds = new List<uint>();
                                    ambiguousList.Add(oldId, ambiguousId);
                                    ambiguousId.alternateNewIds.Add(newId);
                                }
                                else
                                {
                                    if (!ambiguousId.alternateNewIds.Contains(newId))
                                        ambiguousId.alternateNewIds.Add(newId);
                                }
                                //throw new Exception("ambiguous texture id migration");
                            }
                        }
                    }
                }
            }

            foreach (uint entry in allOldIds)
            {
                if (!idMigrationTable.ContainsKey(entry))
                {
                    missingList.Add(entry, entry);
                }
            }

            StreamWriter outputFile = new StreamWriter(new FileStream("objectIdMigrationTable.txt", FileMode.Create, FileAccess.Write));
            foreach (KeyValuePair<uint, uint> entry in idMigrationTable)
            {
                if (entry.Key == entry.Value)
                    continue;
                outputFile.WriteLine($"{entry.Key.ToString("x8")} {entry.Value.ToString("x8")}");
                outputFile.Flush();
            }
            outputFile.Close();

            outputFile = new StreamWriter(new FileStream("objectIdMigrationMissingConversions.txt", FileMode.Create, FileAccess.Write));
            foreach (KeyValuePair<uint, uint> entry in missingList)
            {
                outputFile.WriteLine(entry.Key.ToString("x8"));
                outputFile.Flush();
            }
            outputFile.Close();

            outputFile = new StreamWriter(new FileStream("objectIdMigrationTableAmbiguous.txt", FileMode.Create, FileAccess.Write));
            foreach (KeyValuePair<uint, cAmbiguousUintValues> entry in ambiguousList)
            {
                outputFile.Write($"{entry.Key.ToString("x8")} {entry.Value.newId.ToString("x8")}");
                bool first = true;
                foreach (uint value in entry.Value.alternateNewIds)
                {
                    if (first)
                    {
                        outputFile.Write("(");
                        first = false;
                    }
                    else
                        outputFile.Write(", ");
                    outputFile.Write(value.ToString("x8"));
                    outputFile.Flush();
                }
                outputFile.WriteLine(")");

                outputFile.Flush();
            }
            outputFile.Close();
        }

        public void buildUsedEnvironmentIdList()
        {
            List<ushort> usedIds = new List<ushort>();

            foreach (KeyValuePair<uint, cDatFileEntry> entry in fileCache)
            {
                if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF) //surface
                {
                }
                else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE) //surface objects
                {
                }
                else //dungeons and interiors
                {
                    cEnvCell thisEnvCell = new cEnvCell(entry.Value);

                    foreach (var portal in thisEnvCell.Portals)
                    {
                        if (!usedIds.Contains(portal.EnvironmentId))
                            usedIds.Add(portal.EnvironmentId);
                    }
                }
            }

            usedIds.Sort();
            StreamWriter outputFile = new StreamWriter(new FileStream("usedEnvironmentIdList.txt", FileMode.Create, FileAccess.Write));
            foreach (var entry in usedIds)
            {
                outputFile.WriteLine(entry.ToString("x4"));
                outputFile.Flush();
            }
            outputFile.Close();
        }

        private bool compareLandblockInfo(cLandblockInfo thisLandblockInfo, cLandblockInfo otherLandblockInfo)
        {
            if (thisLandblockInfo.Id != otherLandblockInfo.Id)
                return false;
            if (thisLandblockInfo.NumCells != otherLandblockInfo.NumCells)
                return false;

            if (thisLandblockInfo.Objects.Count != otherLandblockInfo.Objects.Count)
                return false;
            for (int i = 0; i < thisLandblockInfo.Objects.Count; i++)
            {
                cStab thisStab = thisLandblockInfo.Objects[i];
                cStab otherStab = otherLandblockInfo.Objects[i];
                //if (thisStab.id == otherStab.id) // this is what we're migrating
                //    return false;

                if (thisStab.frame.angles.x != otherStab.frame.angles.x)
                    return false;
                if (thisStab.frame.angles.y != otherStab.frame.angles.y)
                    return false;
                if (thisStab.frame.angles.z != otherStab.frame.angles.z)
                    return false;
                if (thisStab.frame.angles.w != otherStab.frame.angles.w)
                    return false;

                if (thisStab.frame.origin.x != otherStab.frame.origin.x)
                    return false;
                if (thisStab.frame.origin.y != otherStab.frame.origin.y)
                    return false;
                if (thisStab.frame.origin.z != otherStab.frame.origin.z)
                    return false;
            }
            if (thisLandblockInfo.buildingFlags != otherLandblockInfo.buildingFlags)
                return false;

            if (thisLandblockInfo.Buildings.Count != otherLandblockInfo.Buildings.Count)
                return false;
            for (int i = 0; i < thisLandblockInfo.Buildings.Count; i++)
            {
                //ignoring for now
            }

            if (thisLandblockInfo.RestrictionTables.Count != otherLandblockInfo.RestrictionTables.Count)
                return false;
            for (int i = 0; i < thisLandblockInfo.RestrictionTables.Count; i++)
            {
                ///ignoring for now
            }
            if (thisLandblockInfo.totalObjects != otherLandblockInfo.totalObjects)
                return false;
            if (thisLandblockInfo.bucketSize != otherLandblockInfo.bucketSize)
                return false;
            return true;
        }

        private bool compareEnvCells(cEnvCell thisEnvCell, cEnvCell otherEnvCell, bool isObjectComparison = false)
        {
            if (thisEnvCell.Id != otherEnvCell.Id)
                return false;
            if (thisEnvCell.Bitfield != otherEnvCell.Bitfield)
                return false;
            if (thisEnvCell.EnvironmentId != otherEnvCell.EnvironmentId)
                return false;
            if (thisEnvCell.StructId != otherEnvCell.StructId)
                return false;
            if (thisEnvCell.RestrictionObj != otherEnvCell.RestrictionObj)
                return false;

            if (thisEnvCell.Position.angles.x != otherEnvCell.Position.angles.x)
                return false;
            if (thisEnvCell.Position.angles.y != otherEnvCell.Position.angles.y)
                return false;
            if (thisEnvCell.Position.angles.z != otherEnvCell.Position.angles.z)
                return false;
            if (thisEnvCell.Position.angles.w != otherEnvCell.Position.angles.w)
                return false;

            if (thisEnvCell.Position.origin.x != otherEnvCell.Position.origin.x)
                return false;
            if (thisEnvCell.Position.origin.y != otherEnvCell.Position.origin.y)
                return false;
            if (thisEnvCell.Position.origin.z != otherEnvCell.Position.origin.z)
                return false;

            if (thisEnvCell.Portals.Count != otherEnvCell.Portals.Count)
                return false;
            for (int i = 0; i < thisEnvCell.Portals.Count; i++)
            {
                cCellPortal thisPortal = thisEnvCell.Portals[i];
                cCellPortal otherPortal = otherEnvCell.Portals[i];

                if (thisPortal.EnvironmentId != otherPortal.EnvironmentId)
                    return false;

                if (thisPortal.Bitfield != otherPortal.Bitfield ||
                    thisPortal.OtherCellId != otherPortal.OtherCellId ||
                    thisPortal.OtherPortalId != otherPortal.OtherPortalId)
                    return false;
            }

            if (thisEnvCell.Cells.Count != otherEnvCell.Cells.Count)
                return false;
            for (int i = 0; i < thisEnvCell.Cells.Count; i++)
            {
                if (thisEnvCell.Cells[i] != otherEnvCell.Cells[i])
                    return false;
            }

            if (thisEnvCell.Stabs.Count != otherEnvCell.Stabs.Count)
                return false;
            for (int i = 0; i < thisEnvCell.Stabs.Count; i++)
            {
                cStab thisStab = thisEnvCell.Stabs[i];
                cStab otherStab = otherEnvCell.Stabs[i];

                if (!isObjectComparison && thisStab.id != otherStab.id) //do not check thisStab.id if that's what we're converting
                    return false;

                if (thisStab.frame.angles.x != otherStab.frame.angles.x)
                    return false;
                if (thisStab.frame.angles.y != otherStab.frame.angles.y)
                    return false;
                if (thisStab.frame.angles.z != otherStab.frame.angles.z)
                    return false;
                if (thisStab.frame.angles.w != otherStab.frame.angles.w)
                    return false;

                if (thisStab.frame.origin.x != otherStab.frame.origin.x)
                    return false;
                if (thisStab.frame.origin.y != otherStab.frame.origin.y)
                    return false;
                if (thisStab.frame.origin.z != otherStab.frame.origin.z)
                    return false;
            }

            if (thisEnvCell.Textures.Count != otherEnvCell.Textures.Count)
                return false;

            if (isObjectComparison) //do not check textureIds if that's what we're converting
            {
                for (int i = 0; i < thisEnvCell.Textures.Count; i++)
                {
                    if (thisEnvCell.Textures[i] != validPortalDatEntries.translateTextureId(otherEnvCell.Textures[i]))
                        return false;
                }
            }

            return true;
        }

        public void exportDirTree(cDatFileNode directory)
        {
            StreamWriter outputFile = new StreamWriter(new FileStream(".\\dirTree.json", FileMode.Create, FileAccess.Write));
            outputFile.WriteLine($"-- root directory({directory.files.Count} files, {directory.subFolders.Count} subDirectories)-- ");
            exportSubDirTrees(directory, 0, outputFile);
        }

        private void exportSubDirTrees(cDatFileNode directory, int tabCount, StreamWriter outputFile)
        {
            string tab = "";
            for (int i = 0; i < tabCount; i++)
            {
                tab += "    ";
            }

            foreach (KeyValuePair<uint, cDatFileEntry> entry in directory.files)
            {
                cDatFileEntry file = entry.Value;
                //if ((file.fileId & 0x0000FFFF) == 0x0000FFFF)
                //{
                //    uint x = (uint)file.fileId >> 24;
                //    uint y = (uint)(file.fileId & 0x00FF0000) >> 16;

                //    outputFile.WriteLine($"{tab}file: {file.fileId.ToString("x8")} = cellLandblock {x},{y}");
                //}
                //else if((file.fileId & 0x0000FFFE) == 0x0000FFFE)
                //{
                //    uint x = (uint)file.fileId >> 24;
                //    uint y = (uint)(file.fileId & 0x00FF0000) >> 16;

                //    outputFile.WriteLine($"{tab}file: {file.fileId.ToString("x8")} = landblockInfo {x},{y}");
                //}
                //else
                //outputFile.WriteLine($"{tab}file: {file.fileId.ToString("x8")} bitFlags:{file.bitFlags.ToString("x8")}");
                outputFile.WriteLine($"{tab}file: {file.fileId.ToString("x8")}");
            }

            outputFile.Flush();

            int subDirCount = 0;
            foreach (cDatFileNode subDirectory in directory.subFolders)
            {
                outputFile.WriteLine($"{tab}-- {tabCount} subDirectory {subDirCount} ({subDirectory.files.Count} files, {subDirectory.subFolders.Count} subDirectories)-- ");
                exportSubDirTrees(subDirectory, tabCount + 1, outputFile);
                subDirCount++;
            }

            outputFile.Flush();
        }

        List<cCellLandblock> cellLandblockList;
        List<cLandblockInfo> landblockInfoList;
        List<cEnvCell> envCellList;
        public void exportCellJson(string outputPath)
        {
            Console.WriteLine("Writing cell.dat to json files...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            cellLandblockList = new List<cCellLandblock>();
            landblockInfoList = new List<cLandblockInfo>();
            envCellList = new List<cEnvCell>();

            Console.WriteLine("Preparing files...");
            foreach (KeyValuePair<uint, cDatFileEntry> entry in fileCache)
            {
                if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF) //surface
                {
                    //StreamReader reader = new StreamReader(entry.Value.fileContent);
                    //cCellLandblock thisLandblock = new cCellLandblock(reader);

                    //cellLandblockList.Add(thisLandblock);
                }
                else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE) //surface objects
                {
                    if ((entry.Value.fileId >> 16) != 0xC6A9)//arwic
                        continue;

                    cLandblockInfo thisLandblockInfo = new cLandblockInfo(entry.Value);

                    landblockInfoList.Add(thisLandblockInfo);
                }
                else //dungeons and interiors
                {
                    if ((entry.Value.fileId >> 16) != 0xC6A9)//arwic
                        continue;

                    cEnvCell thisEnvCell = new cEnvCell(entry.Value);

                    envCellList.Add(thisEnvCell);
                }
            }

            JsonSerializerSettings settings = new JsonSerializerSettings();
            //settings.TypeNameHandling = TypeNameHandling.Auto;
            //settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            //settings.DefaultValueHandling = DefaultValueHandling.Ignore;

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            //Console.WriteLine("Exporting landblocks...");
            //foreach (cCellLandblock entry in cellLandblockList)
            //{
            //    string outputFilename = Path.Combine(outputPath, (entry.Id >> 16).ToString("x4"));
            //    if (!Directory.Exists(outputFilename))
            //        Directory.CreateDirectory(outputFilename);
            //    outputFilename = Path.Combine(outputFilename, $"{entry.Id.ToString("x8")}.json");

            //    StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));

            //    string jsonString = JsonConvert.SerializeObject(entry, Formatting.Indented, settings);
            //    outputFile.Write(jsonString);
            //    outputFile.Close();
            //}

            Console.WriteLine("Exporting landblock info...");
            foreach (cLandblockInfo entry in landblockInfoList)
            {
                string outputFilename = Path.Combine(outputPath, (entry.Id >> 16).ToString("x4"));
                if (!Directory.Exists(outputFilename))
                    Directory.CreateDirectory(outputFilename);
                outputFilename = Path.Combine(outputFilename, $"{entry.Id.ToString("x8")}.json");

                StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));

                string jsonString = JsonConvert.SerializeObject(entry, Formatting.Indented, settings);
                outputFile.Write(jsonString);
                outputFile.Close();
            }

            Console.WriteLine("Exporting envCells...");
            foreach (cEnvCell entry in envCellList)
            {
                string outputFilename = Path.Combine(outputPath, (entry.Id >> 16).ToString("x4"));
                if (!Directory.Exists(outputFilename))
                    Directory.CreateDirectory(outputFilename);
                outputFilename = Path.Combine(outputFilename, $"{entry.Id.ToString("x8")}.json");

                StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));

                string jsonString = JsonConvert.SerializeObject(entry, Formatting.Indented, settings);
                outputFile.Write(jsonString);
                outputFile.Close();
            }

            timer.Stop();
            Console.WriteLine("Finished in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
        }

        static public List<uint> loadSettlementListFromFile(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));

            List<uint> list = new List<uint>();

            string line = inputFile.ReadLine();
            string[] splitLine;
            while (!inputFile.EndOfStream)
            {
                splitLine = line.Split('\t');
                uint cellId = Convert.ToUInt32(splitLine[0], 16);
                list.Add(cellId);

                line = inputFile.ReadLine();
            }

            return list;
        }

        static public void buildSettlementFileFromGoArrowLocationFile(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            StreamWriter outputFile = new StreamWriter(new FileStream("./ListOfSettlements.txt", FileMode.Create, FileAccess.Write));

            string line = inputFile.ReadLine();
            string substring;
            while (!inputFile.EndOfStream)
            {
                if (line.Contains("type=\"Village\""))
                {
                    int nameStartIndex = line.IndexOf("name=") + 6;
                    int nameEndIndex = line.IndexOf("\"", nameStartIndex);

                    string name = line.Substring(nameStartIndex, nameEndIndex - nameStartIndex);

                    int nsStartIndex = line.IndexOf("NS=") + 4;
                    int nsEndIndex = line.IndexOf("\"", nsStartIndex);

                    substring = line.Substring(nsStartIndex, nsEndIndex - nsStartIndex);
                    float ns = float.Parse(substring, CultureInfo.InvariantCulture.NumberFormat);

                    int ewStartIndex = line.IndexOf("EW=") + 4;
                    int ewEndIndex = line.IndexOf("\"", ewStartIndex);

                    substring = line.Substring(ewStartIndex, ewEndIndex - ewStartIndex);
                    float ew = float.Parse(substring, CultureInfo.InvariantCulture.NumberFormat);

                    uint cellId = coordsToCellId(ns, ew);

                    outputFile.WriteLine($"{cellId.ToString("x8")}\t{name}");
                    outputFile.Flush();
                }

                line = inputFile.ReadLine();
            }

            inputFile.Close();
            outputFile.Close();
        }

        static public uint coordsToCellId(float northSouth, float eastWest)
        {
            northSouth = (northSouth - 0.5f) * 10.0f;
            eastWest = (eastWest - 0.5f) * 10.0f;

            var baseX = (uint)(eastWest + 0x400);
            var baseY = (uint)(northSouth + 0x400);

            byte blockX = (byte)(baseX >> 3);
            byte blockY = (byte)(baseY >> 3);
            byte cellX = (byte)(baseX & 7);
            byte cellY = (byte)(baseY & 7);

            uint block = (uint)((blockX << 8) | blockY);
            uint cell = (uint)((cellX << 3) | cellY);

            uint cellId = (block << 16) | (cell + 1);

            return cellId;
        }
    }
}