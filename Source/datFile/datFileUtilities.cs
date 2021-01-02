using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

                        if(currentId == 0)
                            idMigrationTable.Add(oldId, newId);
                        else
                        {
                            if (newId != currentId)
                            {
                                cAmbiguousValues ambiguousId;
                                if(!ambiguousList.TryGetValue(oldId, out ambiguousId))
                                {
                                    ambiguousId = new cAmbiguousValues();
                                    ambiguousId.newId = currentId;
                                    ambiguousId.alternateNewIds = new List<ushort>();
                                    ambiguousList.Add(oldId, ambiguousId);
                                    ambiguousId.alternateNewIds.Add(newId);
                                }
                                else
                                {
                                    if(!ambiguousId.alternateNewIds.Contains(newId))
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
                if(!idMigrationTable.ContainsKey(entry))
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

        public void buildEnvironmentIdMigrationTable(cDatFile otherDat)
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

                for (int i = 0; i < otherEnvCell.Portals.Count; i++)
                {
                    ushort oldId = otherEnvCell.Portals[i].EnvironmentId;
                    if (!allOldIds.Contains(oldId))
                        allOldIds.Add(oldId);
                }

                if (compareEnvCells(thisEnvCell, otherEnvCell, true))
                {
                    for (int i = 0; i < otherEnvCell.Portals.Count; i++)
                    {
                        ushort oldId = otherEnvCell.Portals[i].EnvironmentId;
                        ushort newId = thisEnvCell.Portals[i].EnvironmentId;

                        ushort currentId = 0;

                        if (!idMigrationTable.TryGetValue(oldId, out currentId))
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

            StreamWriter outputFile = new StreamWriter(new FileStream("environmentIdMigrationTable.txt", FileMode.Create, FileAccess.Write));
            foreach (KeyValuePair<ushort, ushort> entry in idMigrationTable)
            {
                if (entry.Key == entry.Value)
                    continue;
                outputFile.WriteLine($"{entry.Key.ToString("x4")} {entry.Value.ToString("x4")}");
                outputFile.Flush();
            }
            outputFile.Close();

            outputFile = new StreamWriter(new FileStream("environmentIdMigrationMissingConversions.txt", FileMode.Create, FileAccess.Write));
            foreach (KeyValuePair<ushort, ushort> entry in missingList)
            {
                outputFile.WriteLine(entry.Key.ToString("x4"));
                outputFile.Flush();
            }
            outputFile.Close();

            outputFile = new StreamWriter(new FileStream("environmentIdMigrationTableAmbiguous.txt", FileMode.Create, FileAccess.Write));
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
                        if(!usedIds.Contains(portal.EnvironmentId))
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

        private bool compareEnvCells(cEnvCell thisEnvCell, cEnvCell otherEnvCell, bool isEnviromentComparison = false)
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

                if (!isEnviromentComparison && thisPortal.EnvironmentId != otherPortal.EnvironmentId)
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
                if(thisEnvCell.Cells[i] != otherEnvCell.Cells[i])
                    return false;
            }

            if (thisEnvCell.Stabs.Count != otherEnvCell.Stabs.Count)
                return false;
            for (int i = 0; i < thisEnvCell.Stabs.Count; i++)
            {
                cStab thisStab = thisEnvCell.Stabs[i];
                cStab otherStab = otherEnvCell.Stabs[i];

                if (thisStab.id != otherStab.id)
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

            if (isEnviromentComparison) //do not check textureIds if that's what we're converting
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
    }
}