using ACE.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;

namespace Melt
{
    public partial class cDatFile
    {
        public int GetFileIteration()
        {
            cDatFileEntry iterationFile;
            if (fileCache.TryGetValue(0xFFFF0001, out iterationFile))
            {
                StreamReader reader = new StreamReader(iterationFile.fileContent);

                List<int> ints = new List<int>();
                bool sorted;

                ints.Add(Utils.readInt32(reader));
                ints.Add(Utils.readInt32(reader));

                sorted = Utils.readBool(reader);

                Utils.align(reader);

                return ints[0];
            }
            return 0;
        }

        public void SetFileIteration(int iteration)
        {
            Console.WriteLine($"Setting file iteration to {iteration}...");
            cDatFileEntry iterationFile = new cDatFileEntry(0xFFFF0001, eDatFormat.ToD);
            iterationFile.bitFlags = 0x00010000;

            StreamWriter outputStream = new StreamWriter(iterationFile.fileContent);
            Utils.writeInt32(iteration, outputStream);
            if (masterMapId == 0x25000000) //portal
                Utils.writeInt32(-iteration, outputStream);
            else
                Utils.writeInt32((int)(0xffffffff - iteration + 1), outputStream);
            Utils.writeBool(true, outputStream);
            Utils.align(outputStream);
            outputStream.Flush();

            iterationFile.fileContent.Seek(0, SeekOrigin.Begin);
            iterationFile.fileSize = (int)iterationFile.fileContent.Length;

            fileCache[0xFFFF0001] = iterationFile;
        }

        public void removeNoobFence(int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine("Removing noob fence...");

            Stopwatch timer = new Stopwatch();
            timer.Start();

            cDatFile toDat = this; //just to make things less confusing

            List<uint> landblockIds = new List<uint>();
            foreach (var file in fileCache)
            {
                if ((file.Key & 0x0000FFFF) == 0x0000FFFF)
                    landblockIds.Add(file.Key);
            }

            foreach (var landblockId in landblockIds)
            {
                cDatFileEntry landblockFile;
                if (fileCache.TryGetValue(landblockId, out landblockFile))
                {
                    cCellLandblock landblock = new cCellLandblock(landblockFile);

                    for (int i = 0; i < landblock.Terrain.Count; i++)
                    {
                        if (landblock.Terrain[i] == 59416)
                        {
                            if (i > 0 && landblock.Terrain[i - 1] != 59416)
                                landblock.Terrain[i] = landblock.Terrain[i - 1];
                            else if (i < 80 && landblock.Terrain[i + 1] != 59416)
                                landblock.Terrain[i] = landblock.Terrain[i + 1];
                            else
                                landblock.Terrain[i] = 0;
                        }
                    }

                    landblock.updateFileContent(landblockFile);
                }
            }

            timer.Stop();
            if (verboseLevel > 5)
                Console.WriteLine("Removed noob fence in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
        }

        public bool convertLandblock(uint cellIdInLandblock, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine("Converting landblock...");

            Stopwatch timer = new Stopwatch();
            timer.Start();

            cDatFile toDat = this; //just to make things less confusing

            uint landblockId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFF;

            cDatFileEntry landblockFile;
            if (fileCache.TryGetValue(landblockId, out landblockFile))
            {
                cCellLandblock landblock = new cCellLandblock(landblockFile);

                landblock.updateFileContent(landblockFile);
            }

            uint landblockInfoId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFE;
            cDatFileEntry landblockInfoFile;
            if (fileCache.TryGetValue(landblockInfoId, out landblockInfoFile))
            {
                cLandblockInfo landblockInfo = new cLandblockInfo(landblockInfoFile);

                int cellsWritten = 0;
                uint startCellId;
                List<uint> replacedList = new List<uint>();
                foreach (var newBuilding in landblockInfo.buildings)
                {
                    if (newBuilding.Portals.Count > 0 && newBuilding.Portals[0].visibleCells.Count > 0)
                    {
                        startCellId = newBuilding.Portals[0].visibleCells[0] | (landblockInfoId & 0xFFFF0000);
                        cellsWritten += replaceCell(startCellId, startCellId, this, true, false, newBuilding, replacedList, landblockId, verboseLevel);
                    }
                }

                //if (landblockInfo.NumCells > 0)
                //{
                //    startCellId = 0x0100 | (landblockInfoId & 0xFFFF0000);
                //    if (!replacedList.Contains(startCellId))
                //        cellsWritten += replaceCell(startCellId, this, true, false, null, replacedList, landblockId, verboseLevel);
                //}

                List<cDatFileEntry> cells = new List<cDatFileEntry>();
                for (uint i = 0; i < 0xFFFE; i++)
                {
                    uint id = i | (landblockId & 0xFFFF0000);

                    cDatFileEntry entry;
                    if (fileCache.TryGetValue(id, out entry))
                        cells.Add(entry);
                }

                List<uint> missingList = new List<uint>();
                foreach (var entry in cells)
                {
                    if (!replacedList.Contains(entry.fileId))
                    {
                        //ideally I would like to get at these cells from a reference and not by searching for them.
                        missingList.Add(entry.fileId);
                        cellsWritten += replaceCell(entry.fileId, entry.fileId, this, true, false, null, replacedList, landblockId, verboseLevel);
                    }
                }

                landblockInfoFile.fileFormat = eDatFormat.ToD;
                landblockInfo.updateFileContent(landblockInfoFile);
            }

            timer.Stop();
            if (verboseLevel > 5)
                Console.WriteLine("Landblock rewritten in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            return true;
        }

        public uint getNextAvailableCellIdBlock(uint startSearchId, int minBlockLength)
        {
            int blockLength = 0;
            uint currentSearchId = startSearchId;
            uint currentBlockStartId = startSearchId;
            while (true)
            {
                if (!fileCache.ContainsKey(currentSearchId))
                {
                    blockLength++;
                    currentSearchId++;
                    if (blockLength >= minBlockLength)
                        break;
                }
                else
                {
                    blockLength = 0;
                    currentSearchId++;
                    currentBlockStartId = currentSearchId;
                }
            }

            return currentBlockStartId;
        }

        public bool replaceLandblockSpecialForStarterOutposts(uint cellIdInLandblock, cDatFile fromDat, bool createOnly = false, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine("Replacing landblock surface and objects...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            cDatFile toDat = this; //just to make things less confusing

            uint landblockId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFF;

            cDatFileEntry toLandBlockFile;
            cDatFileEntry fromLandblockFile;
            if (fromDat.fileCache.TryGetValue(landblockId, out fromLandblockFile))
            {
                cCellLandblock landblockFrom = new cCellLandblock(fromLandblockFile);
                cCellLandblock landblockTo;

                bool existsOnDestination = toDat.fileCache.TryGetValue(landblockId, out toLandBlockFile);

                if (!existsOnDestination || !createOnly)
                {
                    if (existsOnDestination)
                    {
                        //cCellLandblock exists on origin and destination, replace.
                        landblockTo = new cCellLandblock(toLandBlockFile);

                        landblockTo.Terrain = landblockFrom.Terrain;
                        landblockTo.HasObjects = landblockFrom.HasObjects;
                    }
                    else
                    {
                        toLandBlockFile = new cDatFileEntry(landblockId, eDatFormat.ToD);
                        landblockTo = landblockFrom;
                    }

                    landblockTo.updateFileContent(toLandBlockFile);

                    if (!existsOnDestination)
                    {
                        //cCellLandblock exists on origin but not on destination, add.
                        toDat.fileCache.Add(landblockId, toLandBlockFile);
                    }
                }
            }
            else
            {
                if (toDat.fileCache.TryGetValue(landblockId, out toLandBlockFile))
                {
                    //cCellLandblock exists on destination but not on origin, delete.
                    if (!createOnly)
                        toDat.fileCache.Remove(landblockId);
                }
                else
                {
                    //no cCellLandblock on origin or destination. nothing to do.
                }
            }

            uint landblockInfoId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFE;
            cDatFileEntry toLandblockInfoFile;
            cDatFileEntry fromLandblockInfoFile;
            if (fromDat.fileCache.TryGetValue(landblockInfoId, out fromLandblockInfoFile))
            {
                cLandblockInfo landblockInfoFrom = new cLandblockInfo(fromLandblockInfoFile);
                cLandblockInfo landblockInfoTo;


                bool existsOnDestination = toDat.fileCache.TryGetValue(landblockInfoId, out toLandblockInfoFile);
                if (!existsOnDestination || !createOnly)
                {
                    if (existsOnDestination)
                    {
                        //cLandblockInfo exists on origin and destination, replace.

                        List<uint> stabIdsToKeep = new List<uint> //DM portal stands
                        {
                            0x020007c5,
                            0x02000b6f,
                            0x02000b61,
                        };

                        landblockInfoTo = new cLandblockInfo(toLandblockInfoFile);
                        foreach (var building in landblockInfoTo.buildings)
                        {
                            if (building.ModelId == 0x01002866) //the ruined spire
                            {
                                removeBuilding(building.Portals[0].OtherCellId | (landblockId & 0xFFFF0000), true, verboseLevel);
                                break;
                            }
                        }
                        landblockInfoTo = new cLandblockInfo(toLandblockInfoFile); //refetch the file now that we removed the building.

                        List<cStab> tempList = new List<cStab>();
                        tempList = landblockInfoTo.objects.Copy();
                        landblockInfoTo.objects.Clear();
                        foreach(var entry in tempList)
                        {
                            if(stabIdsToKeep.Contains(entry.id))
                                landblockInfoTo.objects.Add(entry);
                        }
                        landblockInfoTo.objects.AddRange(landblockInfoFrom.objects);

                        List<uint> copiedList = new List<uint>();

                        cBuildInfo newBuilding = null;
                        foreach (var building in landblockInfoFrom.buildings)
                        {
                            if (building.ModelId == 0x010014c3) //the pristine spire
                            {
                                newBuilding = building;
                                landblockInfoTo.buildings.Add(building);
                                uint cellInBuilding = building.Portals[0].OtherCellId | (landblockId & 0xFFFF0000);
                                uint lowestCellId;
                                int cellsNeeded = countCell(cellInBuilding, fromDat, out lowestCellId);
                                uint newCellId = getNextAvailableCellIdBlock(0x0100 | (landblockId & 0xFFFF0000), cellsNeeded);

                                //replaceCellNewId(newCellId, fromDat, lowestCellId, true, false, building, replacedList, landblockId, verboseLevel);
                                copyBuildingCellsNewId(landblockId, building, fromDat, newCellId, copiedList, verboseLevel);
                                break;
                            }
                        }
                    }
                    else
                    {
                        landblockInfoTo = landblockInfoFrom;
                        landblockInfoTo.numCells = 0;
                        landblockInfoTo.buildings.Clear();
                        toLandblockInfoFile = new cDatFileEntry(landblockInfoId, eDatFormat.ToD);
                    }

                    landblockInfoTo.updateFileContent(toLandblockInfoFile);

                    if (!existsOnDestination)
                    {
                        //cCellLandblock exists on origin but not on destination, add.
                        toDat.fileCache.Add(landblockInfoId, toLandblockInfoFile);
                    }
                }
            }

            timer.Stop();
            if (verboseLevel > 5)
                Console.WriteLine("Landblock replaced in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            return true;
        }

        public bool removeNoobPillarFromLandblock(uint cellIdInLandblock, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine($"Removing noob pillar from landblock...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            cDatFile toDat = this; //just to make things less confusing

            uint landblockId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFF;

            cDatFileEntry landBlockFile;
            cCellLandblock landblock;

            bool existsOnDestination = toDat.fileCache.TryGetValue(landblockId, out landBlockFile);

            if (existsOnDestination)
            {
                //cCellLandblock exists on origin and destination, replace.
                landblock = new cCellLandblock(landBlockFile);

                for (int i = 0; i < landblock.Terrain.Count; i++)
                {
                    if ((landblock.Terrain[i] & 0x0800) != 0x0000)
                        landblock.Terrain[i] = (ushort)(landblock.Terrain[i] & ~0x0800);
                }

                landblock.updateFileContent(landBlockFile);
            }

            timer.Stop();
            if (verboseLevel > 5)
                Console.WriteLine("Landblock noob pillars removed in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            return true;
        }

        public bool removeRoadFromLandblock(uint cellIdInLandblock, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine($"Removing roads from landblock...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            cDatFile toDat = this; //just to make things less confusing

            uint landblockId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFF;

            cDatFileEntry landBlockFile;
            cCellLandblock landblock;

            bool existsOnDestination = toDat.fileCache.TryGetValue(landblockId, out landBlockFile);

            if (existsOnDestination)
            {
                //cCellLandblock exists on origin and destination, replace.
                landblock = new cCellLandblock(landBlockFile);

                for (int i = 0; i < landblock.Terrain.Count; i++)
                {
                    if ((landblock.Terrain[i] & 0x0003) != 0x0000)
                        landblock.Terrain[i] = (ushort)(landblock.Terrain[i] & ~0x0003);
                }

                landblock.updateFileContent(landBlockFile);
            }

            timer.Stop();
            if (verboseLevel > 5)
                Console.WriteLine("Landblock roads removed in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            return true;
        }

        public bool replaceLandblockSpecificTexture(uint cellIdInLandblock, ushort texId, ushort replacementTexId, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine($"Replacing landblock TextureId from 0x{texId.ToString("x4")} to 0x{replacementTexId.ToString("x4")}...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            cDatFile toDat = this; //just to make things less confusing

            uint landblockId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFF;

            cDatFileEntry landBlockFile;
            cCellLandblock landblock;

            bool existsOnDestination = toDat.fileCache.TryGetValue(landblockId, out landBlockFile);

            if (existsOnDestination)
            {
                //cCellLandblock exists on origin and destination, replace.
                landblock = new cCellLandblock(landBlockFile);

                for (int i = 0; i < landblock.Terrain.Count; i++)
                {
                    if (landblock.Terrain[i] == texId)
                        landblock.Terrain[i] = replacementTexId;
                }

                landblock.updateFileContent(landBlockFile);
            }

            timer.Stop();
            if (verboseLevel > 5)
                Console.WriteLine("Landblock heightmap replaced in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            return true;
        }

        public bool landblockBucketFill(uint cellIdInLandblock, ushort texId, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine($"Replacing landblock TextureId to 0x{texId.ToString("x4")}...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            cDatFile toDat = this; //just to make things less confusing

            uint landblockId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFF;

            cDatFileEntry landBlockFile;
            cCellLandblock landblock;

            bool existsOnDestination = toDat.fileCache.TryGetValue(landblockId, out landBlockFile);

            if (existsOnDestination)
            {
                //cCellLandblock exists on origin and destination, replace.
                landblock = new cCellLandblock(landBlockFile);

                for (int i = 0; i < landblock.Terrain.Count; i++)
                {
                    landblock.Terrain[i] = texId;
                }

                landblock.updateFileContent(landBlockFile);
            }

            timer.Stop();
            if (verboseLevel > 5)
                Console.WriteLine("Landblock heightmap replaced in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            return true;
        }

        public bool replaceLandblockTerrain(uint cellIdInLandblock, cDatFile fromDat, bool heightmap = true, bool textures = true, bool createOnly = false, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine("Replacing landblock heightmap...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            cDatFile toDat = this; //just to make things less confusing

            uint landblockId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFF;

            cDatFileEntry toLandBlockFile;
            cDatFileEntry fromLandblockFile;
            if (fromDat.fileCache.TryGetValue(landblockId, out fromLandblockFile))
            {
                cCellLandblock landblockFrom = new cCellLandblock(fromLandblockFile);
                cCellLandblock landblockTo;

                bool existsOnDestination = toDat.fileCache.TryGetValue(landblockId, out toLandBlockFile);

                if (!existsOnDestination || !createOnly)
                {
                    if (existsOnDestination)
                    {
                        //cCellLandblock exists on origin and destination, replace.
                        landblockTo = new cCellLandblock(toLandBlockFile);

                        if (heightmap)
                            landblockTo.Height = landblockFrom.Height;

                        if (textures)
                            landblockTo.Terrain = landblockFrom.Terrain;
                    }
                    else
                    {
                        toLandBlockFile = new cDatFileEntry(landblockId, eDatFormat.ToD);
                        landblockTo = landblockFrom;
                        landblockTo.HasObjects = false; //we're not importing them here so might as well set this.
                    }

                    landblockTo.updateFileContent(toLandBlockFile);

                    if (!existsOnDestination)
                    {
                        //cCellLandblock exists on origin but not on destination, add.
                        toDat.fileCache.Add(landblockId, toLandBlockFile);
                    }
                }
            }
            else
            {
                if (toDat.fileCache.TryGetValue(landblockId, out toLandBlockFile))
                {
                    //cCellLandblock exists on destination but not on origin, delete.
                    if (!createOnly)
                        toDat.fileCache.Remove(landblockId);
                }
                else
                {
                    //no cCellLandblock on origin or destination. nothing to do.
                }
            }

            timer.Stop();
            if (verboseLevel > 5)
                Console.WriteLine("Landblock heightmap replaced in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            return true;
        }

        public bool compareLandblockInfo(uint cellIdInLandblock, cDatFile otherDat, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine("Comparing landblock...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            cDatFile ourDat = this; //just to make things less confusing

            uint landblockId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFF;
            uint landblockInfoId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFE;
            cDatFileEntry ourLandblockInfoFile;
            cDatFileEntry otherLandblockInfoFile;

            if (otherDat.fileCache.TryGetValue(landblockInfoId, out otherLandblockInfoFile))
            {
                cLandblockInfo landblockInfoOther = new cLandblockInfo(otherLandblockInfoFile);
                cLandblockInfo landblockInfoOurs;

                if (ourDat.fileCache.TryGetValue(landblockInfoId, out ourLandblockInfoFile))
                {
                    landblockInfoOurs = new cLandblockInfo(ourLandblockInfoFile);

                    return compareLandblockInfo(landblockInfoOurs, landblockInfoOther);
                }
            }

            return false;
        }

        public bool compareLandblockCells(uint cellIdInLandblock, cDatFile otherDat, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine("Comparing landblock...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            cDatFile ourDat = this; //just to make things less confusing

            uint landblockId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFF;
            uint landblockInfoId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFE;
            cDatFileEntry ourLandblockInfoFile;
            cDatFileEntry otherLandblockInfoFile;

            if (otherDat.fileCache.TryGetValue(landblockInfoId, out otherLandblockInfoFile))
            {
                cLandblockInfo landblockInfoOther = new cLandblockInfo(otherLandblockInfoFile);
                cLandblockInfo landblockInfoOurs;

                if (ourDat.fileCache.TryGetValue(landblockInfoId, out ourLandblockInfoFile))
                {
                    landblockInfoOurs = new cLandblockInfo(ourLandblockInfoFile);

                    List<cEnvCell> ourEnvCells = new List<cEnvCell>();
                    List<cEnvCell> otherEnvCells = new List<cEnvCell>();

                    for (uint i = 0; i < 0xFFFE; i++)
                    {
                        uint id = i | (landblockId & 0xFFFF0000);
                        cDatFileEntry envCellFile;
                        if (ourDat.fileCache.TryGetValue(id, out envCellFile))
                            ourEnvCells.Add(new cEnvCell(envCellFile));
                        if (otherDat.fileCache.TryGetValue(id, out envCellFile))
                            otherEnvCells.Add(new cEnvCell(envCellFile));
                    }

                    if (ourEnvCells.Count != otherEnvCells.Count)
                        Console.WriteLine($"Cells count mismatch: {ourEnvCells.Count} {otherEnvCells.Count}");

                    for (int i = 0; i < ourEnvCells.Count; i++)
                    {
                        cEnvCell ourEnvCell = ourEnvCells[i];
                        cEnvCell otherEnvCell = otherEnvCells[i];

                        int counter = 0;

                        if (!compareEnvCells(ourEnvCell, otherEnvCell))
                            counter++;
                    }

                }
            }

            timer.Stop();
            if (verboseLevel > 5)
                Console.WriteLine("Landblock compared in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            return true;
        }

        public void findEnvironmentIdInCells(int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine("Searching all landblocks...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landblocksCounter = 0;

            List<uint> landblockIds = new List<uint>();
            foreach (var file in fileCache)
            {
                if ((file.Key & 0x0000FFFF) == 0x0000FFFF)
                    landblockIds.Add(file.Key);
            }

            foreach (var landblockId in landblockIds)
            {
                if (findEnvironmentIdInCells(landblockId, verboseLevel))
                    landblocksCounter++;
            }

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("Found {0} landblocks with matches in {1} seconds.", landblocksCounter, timer.ElapsedMilliseconds / 1000f);
        }

        public bool findEnvironmentIdInCells(uint cellIdInLandblock, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine("Searching landblock...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            cDatFile ourDat = this; //just to make things less confusing

            uint landblockId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFF;
            uint landblockInfoId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFE;
            cDatFileEntry ourLandblockInfoFile;
            cLandblockInfo landblockInfoOurs;

            if (ourDat.fileCache.TryGetValue(landblockInfoId, out ourLandblockInfoFile))
            {
                landblockInfoOurs = new cLandblockInfo(ourLandblockInfoFile);

                List<cEnvCell> ourEnvCells = new List<cEnvCell>();

                for (uint i = 0; i < 0xFFFE; i++)
                {
                    uint id = i | (landblockId & 0xFFFF0000);
                    cDatFileEntry envCellFile;
                    if (ourDat.fileCache.TryGetValue(id, out envCellFile))
                        ourEnvCells.Add(new cEnvCell(envCellFile));
                }

                for (int i = 0; i < ourEnvCells.Count; i++)
                {
                    cEnvCell ourEnvCell = ourEnvCells[i];

                    if(ourEnvCell.EnvironmentId == 0x05bc || ourEnvCell.EnvironmentId == 0x05bd || ourEnvCell.EnvironmentId == 0x05be || ourEnvCell.EnvironmentId == 0x05bf || ourEnvCell.EnvironmentId == 0x05c1 || ourEnvCell.EnvironmentId == 0x05c2)
                    {
                        Console.WriteLine("Found match in Landblock {0}", landblockId);
                        return true;
                    }
                }

            }

            timer.Stop();
            if (verboseLevel > 5)
                Console.WriteLine("Landblock compared in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            return false;
        }

        public void fixAllLandblockCells(cDatFile todDat, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine("Fixing Cells from all landblocks...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landblocksCounter = 0;

            List<uint> landblockIds = new List<uint>();
            foreach (var file in fileCache)
            {
                if ((file.Key & 0x0000FFFF) == 0x0000FFFF)
                    landblockIds.Add(file.Key);
            }

            foreach (var landblockId in landblockIds)
            {
                if (fixLandblockCells(landblockId, todDat, verboseLevel))
                    landblocksCounter++;
            }

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("{0} landblocks fixed in {1} seconds.", landblocksCounter, timer.ElapsedMilliseconds / 1000f);
        }

        public bool fixLandblockCells(uint cellIdInLandblock, cDatFile todDat, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine("Fixing landblock cells...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            cDatFile ourDat = this; //just to make things less confusing

            uint landblockId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFF;
            uint landblockInfoId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFE;
            cDatFileEntry ourLandblockInfoFile;
            cDatFileEntry otherLandblockInfoFile;

            int counter = 0;
            if (todDat.fileCache.TryGetValue(landblockInfoId, out otherLandblockInfoFile))
            {
                cLandblockInfo landblockInfoOther = new cLandblockInfo(otherLandblockInfoFile);
                cLandblockInfo landblockInfoOurs;

                if (ourDat.fileCache.TryGetValue(landblockInfoId, out ourLandblockInfoFile))
                {
                    landblockInfoOurs = new cLandblockInfo(ourLandblockInfoFile);

                    if (landblockInfoOurs.buildings.Count > 0) // We're only fixing buildings cells atm so skip if there are no buildings.
                    {
                        List<cDatFileEntry> ourEnvCells = new List<cDatFileEntry>();
                        List<cDatFileEntry> otherEnvCells = new List<cDatFileEntry>();

                        for (uint i = 0; i < 0xFFFE; i++)
                        {
                            uint id = i | (landblockId & 0xFFFF0000);
                            cDatFileEntry envCellFile;
                            if (ourDat.fileCache.TryGetValue(id, out envCellFile))
                                ourEnvCells.Add(envCellFile);
                            if (todDat.fileCache.TryGetValue(id, out envCellFile))
                                otherEnvCells.Add(envCellFile);
                        }

                        if (ourEnvCells.Count == otherEnvCells.Count) // We can only fix if we're the same between versions.
                        {
                            for (int i = 0; i < ourEnvCells.Count; i++)
                            {
                                cEnvCell ourEnvCell = new cEnvCell(ourEnvCells[i]);
                                cEnvCell otherEnvCell = new cEnvCell(otherEnvCells[i]);
                                if (ourEnvCell.EnvironmentId == 0x0574 || ourEnvCell.EnvironmentId == 0x0575)
                                {
                                    ourEnvCell.Portals = otherEnvCell.Portals;
                                    ourEnvCell.updateFileContent(ourEnvCells[i]);
                                    counter++;
                                }
                            }
                        }
                    }
                }
            }

            timer.Stop();
            if (verboseLevel > 5 || (verboseLevel >= 4 && counter > 0))
                Console.WriteLine("Fixed {0} entries in {1} seconds.", counter, timer.ElapsedMilliseconds / 1000f);
            return true;
        }

        public bool replaceLandblock(uint cellIdInLandblock, cDatFile fromDat, bool heightmap = true, bool textures = true, bool objects = true, bool cells = true, bool createOnly = false, uint newCellIdInLandblock = 0, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine("Replacing landblock...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            cDatFile toDat = this; //just to make things less confusing

            uint landblockId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFF;
            uint newLandblockId = landblockId;
            if (newCellIdInLandblock != 0)
                newLandblockId = newCellIdInLandblock & 0xFFFF0000 | 0x0000FFFF;                

            cDatFileEntry toLandBlockFile;
            cDatFileEntry fromLandblockFile;
            if (fromDat.fileCache.TryGetValue(landblockId, out fromLandblockFile))
            {
                cCellLandblock landblockFrom = new cCellLandblock(fromLandblockFile);
                cCellLandblock landblockTo;

                bool existsOnDestination = toDat.fileCache.TryGetValue(newLandblockId, out toLandBlockFile);

                if (!existsOnDestination || !createOnly)
                {
                    if (existsOnDestination)
                    {
                        //cCellLandblock exists on origin and destination, replace.
                        landblockTo = new cCellLandblock(toLandBlockFile);

                        if (objects)
                            landblockTo.HasObjects = landblockFrom.HasObjects;
                        if (heightmap)
                            landblockTo.Height = landblockFrom.Height;
                        if (textures)
                            landblockTo.Terrain = landblockFrom.Terrain;
                    }
                    else
                    {
                        toLandBlockFile = new cDatFileEntry(newLandblockId, eDatFormat.ToD);
                        landblockTo = landblockFrom;
                        landblockTo.Id = newLandblockId;
                    }

                    landblockTo.updateFileContent(toLandBlockFile);

                    if (!existsOnDestination)
                    {
                        //cCellLandblock exists on origin but not on destination, add.
                        toDat.fileCache.Add(newLandblockId, toLandBlockFile);
                    }
                }
            }
            else
            {
                if (toDat.fileCache.TryGetValue(newLandblockId, out toLandBlockFile))
                {
                    //cCellLandblock exists on destination but not on origin, delete.
                    if (!createOnly)
                        toDat.fileCache.Remove(newLandblockId);
                }
                else
                {
                    //no cCellLandblock on origin or destination. nothing to do.
                }
            }

            uint landblockInfoId = landblockId & 0xFFFF0000 | 0x0000FFFE;
            uint newLandblockInfoId = newLandblockId & 0xFFFF0000 | 0x0000FFFE;
            cDatFileEntry toLandblockInfoFile;
            cDatFileEntry fromLandblockInfoFile;
            if (fromDat.fileCache.TryGetValue(landblockInfoId, out fromLandblockInfoFile))
            {
                cLandblockInfo landblockInfoFrom = new cLandblockInfo(fromLandblockInfoFile);
                cLandblockInfo landblockInfoTo;

                bool existsOnDestination = toDat.fileCache.TryGetValue(newLandblockInfoId, out toLandblockInfoFile);
                if (!existsOnDestination || !createOnly)
                {
                    if (existsOnDestination)
                    {
                        //cLandblockInfo exists on origin and destination, replace.
                        landblockInfoTo = new cLandblockInfo(toLandblockInfoFile);

                        if (cells)
                        {
                            for (uint i = 0; i < 0xFFFE; i++)
                            {
                                uint id = i | (newLandblockInfoId & 0xFFFF0000);
                                toDat.fileCache.Remove(id);
                            }

                            landblockInfoTo.numCells = 0;
                        }
                    }
                    else
                    {
                        landblockInfoTo = landblockInfoFrom;
                        landblockInfoTo.Id = newLandblockInfoId;
                        if (!cells)
                            landblockInfoTo.numCells = 0;
                        toLandblockInfoFile = new cDatFileEntry(newLandblockInfoId, eDatFormat.ToD);
                    }

                    int cellsAdded = 0;
                    uint startCellId;
                    uint startDestinationCellId;
                    List<uint> replacedList = new List<uint>();
                    if (cells)
                    {
                        foreach (var newBuilding in landblockInfoFrom.buildings)
                        {
                            if (newBuilding.Portals.Count > 0 && newBuilding.Portals[0].visibleCells.Count > 0)
                            {
                                startCellId = newBuilding.Portals[0].visibleCells[0] | (landblockInfoId & 0xFFFF0000);
                                startDestinationCellId = newBuilding.Portals[0].visibleCells[0] | (newLandblockInfoId & 0xFFFF0000);
                                cellsAdded += replaceCell(startCellId, startDestinationCellId, fromDat, true, false, newBuilding, replacedList, newLandblockId, verboseLevel);
                            }
                        }

                        //if (landblockInfoFrom.NumCells > 0)
                        //{
                        //    startCellId = 0x0100 | (landblockInfoId & 0xFFFF0000);
                        //    if (!replacedList.Contains(startCellId))
                        //        cellsAdded += replaceCell(startCellId, fromDat, true, false, null, replacedList, landblockId, verboseLevel);
                        //}

                        List<cDatFileEntry> cellList = new List<cDatFileEntry>();
                        for (uint i = 0; i < 0xFFFE; i++)
                        {
                            uint id = i | (landblockId & 0xFFFF0000);

                            cDatFileEntry entry;
                            if (fromDat.fileCache.TryGetValue(id, out entry))
                                cellList.Add(entry);
                        }

                        List<uint> missingList = new List<uint>();
                        foreach (var entry in cellList)
                        {
                            if (!replacedList.Contains(entry.fileId))
                            {
                                uint id = entry.fileId;
                                uint newId = (id & 0x0000FFFF) | (newLandblockId & 0xFFFF0000);
                                //ideally I would like to get at these cells from a reference and not by searching for them.
                                missingList.Add(entry.fileId);
                                cellsAdded += replaceCell(id, newId, fromDat, true, false, null, replacedList, newLandblockId, verboseLevel);
                            }
                        }

                        if (cellsAdded > 0)
                        {
                            cCellLandblock landblockFrom = new cCellLandblock(fromLandblockFile);
                            cCellLandblock landblockTo;
                            toDat.fileCache.TryGetValue(newLandblockId, out toLandBlockFile);

                            landblockTo = new cCellLandblock(toLandBlockFile);

                            landblockTo.HasObjects = landblockFrom.HasObjects;
                            landblockTo.updateFileContent(toLandBlockFile);
                        }
                    }

                    landblockInfoTo.Id = newLandblockInfoId;
                    if (cells)
                    {
                        landblockInfoTo.numCells = landblockInfoFrom.numCells;
                        landblockInfoTo.buildingFlags = landblockInfoFrom.buildingFlags;
                        landblockInfoTo.buildings = landblockInfoFrom.buildings;
                    }
                    landblockInfoTo.restrictionTableCount = landblockInfoFrom.restrictionTableCount;
                    landblockInfoTo.restrictionTables = landblockInfoFrom.restrictionTables;
                    landblockInfoTo.restrictionTableBucketSize = landblockInfoFrom.restrictionTableBucketSize;
                    if (objects)
                    {
                        landblockInfoTo.objects = landblockInfoFrom.objects;
                    }
                    landblockInfoTo.updateFileContent(toLandblockInfoFile);

                    if (!existsOnDestination)
                    {
                        //cCellLandblock exists on origin but not on destination, add.
                        toDat.fileCache.Add(newLandblockInfoId, toLandblockInfoFile);
                    }
                }
            }
            else
            {
                if (toDat.fileCache.TryGetValue(newLandblockInfoId, out toLandblockInfoFile))
                {
                    //cLandblockInfo exists on destination but not on origin, delete.
                    if (!createOnly)
                    {
                        cLandblockInfo landblockInfoTo = new cLandblockInfo(toLandblockInfoFile);

                        //if(objects)
                        //    toDat.fileCache.Remove(newLandblockInfoId);
                        //else
                        //{
                        //    //Clear everything but the objects.
                        //    landblockInfoTo.NumCells = 0;
                        //    landblockInfoTo.buildingFlags = 0;
                        //    List<cBuildInfo> buildingsToKeep = new List<cBuildInfo>(); // Let's keep buildings without interions as they are pretty much objects.
                        //    foreach(var building in landblockInfoTo.Buildings)
                        //    {
                        //        if (building.Portals.Count == 0)
                        //            buildingsToKeep.Add(building);
                        //    }
                        //    landblockInfoTo.Buildings = buildingsToKeep;
                        //    landblockInfoTo.restrictionTables.Clear();
                        //    landblockInfoTo.updateFileContent(toLandblockInfoFile);
                        //}

                        if (objects && cells)
                            toDat.fileCache.Remove(newLandblockInfoId);
                        else if (!objects)
                        {
                            //Clear everything but the objects.
                            landblockInfoTo.numCells = 0;
                            landblockInfoTo.buildingFlags = 0;
                            List<cBuildInfo> buildingsToKeep = new List<cBuildInfo>(); // Let's keep buildings without interions as they are pretty much objects.
                            foreach (var building in landblockInfoTo.buildings)
                            {
                                if (building.Portals.Count == 0)
                                    buildingsToKeep.Add(building);
                            }
                            landblockInfoTo.buildings = buildingsToKeep;
                            landblockInfoTo.restrictionTables.Clear();
                            landblockInfoTo.updateFileContent(toLandblockInfoFile);

                            for (uint i = 0; i < 0xFFFE; i++)
                            {
                                uint id = i | (newLandblockInfoId & 0xFFFF0000);
                                toDat.fileCache.Remove(id);
                            }
                        }
                        else if (!cells)
                        {
                            //Clear objects.
                            landblockInfoTo.objects.Clear();
                            landblockInfoTo.updateFileContent(toLandblockInfoFile);
                        }
                    }
                }
                else
                {
                    //no cLandblockInfo on origin or destination. nothing to do.
                }
            }

            timer.Stop();
            if (verboseLevel > 5)
                Console.WriteLine("Landblock replaced in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            return true;
        }

        public int countCell(uint cellId, cDatFile fromDat, out uint lowestCellId)
        {
            int cellsCounter = 0;
            lowestCellId = uint.MaxValue;

            List<uint> countedList = new List<uint>();
            countCellRecursive(cellId, fromDat, ref cellsCounter, countedList, ref lowestCellId);

            return cellsCounter;
        }

        public void countCellRecursive(uint cellId, cDatFile fromDat, ref int cellsCounter, List<uint> countedList, ref uint lowestCellId)
        {
            cDatFileEntry file;
            if (fromDat.fileCache.TryGetValue(cellId, out file))
            {
                if (cellId < lowestCellId)
                    lowestCellId = cellId;
                cEnvCell eEnvCell = new cEnvCell(file);
                countedList.Add(cellId);
                cellsCounter++;

                foreach (ushort connectedCell in eEnvCell.Cells)
                {
                    uint connectedCellId = connectedCell | (cellId & 0xFFFF0000);

                    if (countedList.Contains(connectedCellId))
                        continue;
                    else
                        countCellRecursive(connectedCellId, fromDat, ref cellsCounter, countedList, ref lowestCellId);
                }
            }
        }

        public int replaceCell(uint cellId, uint destinationCellId, cDatFile fromDat, bool recurse, bool createOnly, cBuildInfo building, List<uint> replacedList, uint parentCellOrLandblock, int verboseLevel = 7)
        {
            int cellsReplacedCounter = 0;
            int cellsNotFoundCounter = 0;
            //List<uint> replacedList = new List<uint>();

            replaceCellRecursive(cellId, destinationCellId, fromDat, recurse, createOnly, ref cellsReplacedCounter, ref cellsNotFoundCounter, replacedList, building, parentCellOrLandblock, verboseLevel);

            if (verboseLevel > 6)
                Console.WriteLine("{0} cell(s) copied, {1} cell(s) not found.", cellsReplacedCounter, cellsNotFoundCounter);

            return cellsReplacedCounter;
        }

        public Dictionary<uint, List<uint>> CellsNotFound = new Dictionary<uint, List<uint>>();
        public void replaceCellRecursive(uint cellId, uint destinationCellId, cDatFile fromDat, bool recurse, bool createOnly, ref int cellsReplacedCounter, ref int cellsNotFoundCounter, List<uint> copiedList, cBuildInfo building, uint parentCellOrLandblock, int verboseLevel = 7)
        {
            if (copiedList.Contains(cellId))
                return;

            if (createOnly && fileCache.ContainsKey(cellId))
                return;

            cDatFileEntry file;
            if (fromDat.fileCache.TryGetValue(cellId, out file))
            {
                cEnvCell eEnvCell = new cEnvCell(file);

                if (file.fileFormat == eDatFormat.retail && building != null)
                {
                    foreach (var portal in eEnvCell.Portals)
                    {
                        ushort id = validPortalDatEntries.translateBuildingPortalEnvId(building.ModelId, portal.EnvironmentId);
                        if (id != portal.EnvironmentId)
                        {
                            if (verboseLevel > 6)
                                Console.WriteLine($"INFO: 0x{cellId.ToString("x8")}: BuildInfo.ModelId: 0x{building.ModelId.ToString("x8")}: Translated EnvironmentId in child portal from 0x{portal.EnvironmentId.ToString("x4")} to 0x{id.ToString("x4")}.");
                            portal.EnvironmentId = id;
                        }
                    }
                }

                cDatFileEntry fileNew = new cDatFileEntry(destinationCellId, eDatFormat.ToD);
                eEnvCell.Id = destinationCellId;
                eEnvCell.updateFileContent(fileNew);

                copiedList.Add(cellId);
                //fileCache.Add(cellId, fileNew);
                fileCache[destinationCellId] = fileNew;
                cellsReplacedCounter++;

                if (recurse)
                {
                    foreach (ushort connectedCell in eEnvCell.Cells)
                    {
                        uint connectedCellId = connectedCell | (cellId & 0xFFFF0000);
                        uint connectedDestinationCellId = connectedCell | (destinationCellId & 0xFFFF0000);

                        if (copiedList.Contains(connectedCellId))
                            continue;
                        else
                            replaceCellRecursive(connectedCellId, connectedDestinationCellId, fromDat, recurse, createOnly, ref cellsReplacedCounter, ref cellsNotFoundCounter, copiedList, building, cellId, verboseLevel);
                    }
                }
                return;
            }

            if (verboseLevel > 0)
                Console.WriteLine($"Couldn't find cell id: 0x{cellId.ToString("x8")}. Parent cell: 0x{parentCellOrLandblock.ToString("x8")}.");

            cellsNotFoundCounter++;

            List<uint> parentCellList;
            if (!CellsNotFound.TryGetValue(parentCellOrLandblock, out parentCellList))
            {
                parentCellList = new List<uint>();
                CellsNotFound[parentCellOrLandblock] = parentCellList;
            }
            if (!parentCellList.Contains(cellId))
                parentCellList.Add(cellId);
        }

        public int copyBuildingCellsNewId(uint fromLandblockId, cBuildInfo building, cDatFile fromDat, uint newLowestCellId, List<uint> copiedList, int verboseLevel = 7)
        {
            int cellsCopiedCounter = 0;
            int cellsNotFoundCounter = 0;

            uint lowestCellId = uint.MaxValue;
            foreach (var portal in building.Portals)
            {
                foreach (var cell in portal.visibleCells)
                {
                    if (cell < lowestCellId)
                        lowestCellId = cell;
                }
            }

            lowestCellId = lowestCellId | (fromLandblockId & 0xFFFF0000);

            int offset = (int)(newLowestCellId - lowestCellId);

            foreach (var portal in building.Portals)
            {
                for (int i = 0; i < portal.visibleCells.Count; i++)
                {
                    uint fromCellId = portal.visibleCells[i] | (fromLandblockId & 0xFFFF0000);
                    uint toCellId = (uint)(fromCellId + offset);
                    copyCell(fromCellId, fromDat, toCellId, building, copiedList, ref cellsCopiedCounter, ref cellsNotFoundCounter, verboseLevel);
                    portal.visibleCells[i] += (ushort)offset;
                }
                portal.OtherCellId += (ushort)offset;
            }

            if (verboseLevel > 6)
                Console.WriteLine("{0} cell(s) copied, {1} cell(s) not found.", cellsCopiedCounter, cellsNotFoundCounter);

            return cellsCopiedCounter;
        }

        public void copyCell(uint fromCellId, cDatFile fromDat, uint toCellId, cBuildInfo building, List<uint> copiedList, ref int cellsCopiedCounter, ref int cellsNotFoundCounter, int verboseLevel = 7)
        {
            if (copiedList.Contains(toCellId))
                return;

            cDatFileEntry file;
            if (fromDat.fileCache.TryGetValue(fromCellId, out file))
            {
                cEnvCell envCell = new cEnvCell(file);

                envCell.Id = toCellId;

                int offset = (int)(toCellId - fromCellId);

                for (int i = 0; i < envCell.Cells.Count; i++)
                {
                    envCell.Cells[i] = (ushort)(envCell.Cells[i] + offset);
                }
                foreach (var portal in envCell.Portals)
                {
                    if(portal.OtherCellId != 0xffff)
                        portal.OtherCellId = (ushort)(portal.OtherCellId + offset);
                }

                if (file.fileFormat == eDatFormat.retail && building != null)
                {
                    foreach (var portal in envCell.Portals)
                    {
                        ushort id = validPortalDatEntries.translateBuildingPortalEnvId(building.ModelId, portal.EnvironmentId);
                        if (id != portal.EnvironmentId)
                        {
                            if (verboseLevel > 6)
                                Console.WriteLine($"INFO: 0x{toCellId.ToString("x8")}: BuildInfo.ModelId: 0x{building.ModelId.ToString("x8")}: Translated EnvironmentId in child portal from 0x{portal.EnvironmentId.ToString("x4")} to 0x{id.ToString("x4")}.");
                            portal.EnvironmentId = id;
                        }
                    }
                }

                cDatFileEntry fileNew = new cDatFileEntry(toCellId, eDatFormat.ToD);
                envCell.updateFileContent(fileNew);

                copiedList.Add(toCellId);
                //fileCache.Add(cellId, fileNew);
                fileCache[toCellId] = fileNew;
                cellsCopiedCounter++;
            }
            else
                cellsNotFoundCounter++;
        }

        public int replaceCellNewId(uint toCellId, cDatFile fromDat, uint fromCellIdLowest, bool recurse, bool createOnly, cBuildInfo building, List<uint> replacedList, uint parentCellOrLandblock, int verboseLevel = 7)
        {
            int cellsReplacedCounter = 0;
            int cellsNotFoundCounter = 0;

            int offset = (int)(toCellId - fromCellIdLowest);
            replaceCellNewIdRecursive(fromCellIdLowest, offset, fromDat, recurse, createOnly, ref cellsReplacedCounter, ref cellsNotFoundCounter, replacedList, building, parentCellOrLandblock, verboseLevel);

            if (building != null)
            {
                foreach (var portal in building.Portals)
                {
                    portal.OtherCellId = (ushort)(portal.OtherCellId + offset);
                    for (int i = 0; i < portal.visibleCells.Count; i++)
                    {
                        portal.visibleCells[i] = (ushort)(portal.visibleCells[i] + offset);
                    }
                }
            }

            foreach (var entry in replacedList)
            {
                cDatFileEntry file;
                if (fileCache.TryGetValue(entry, out file))
                {
                    cEnvCell envCell = new cEnvCell(file);
                    envCell.Id = entry;
                    for (int i = 0; i < envCell.Cells.Count; i++)
                    {
                        envCell.Cells[i] = (ushort)(envCell.Cells[i] + offset);
                    }
                    foreach (var portal in envCell.Portals)
                    {
                        if (portal.OtherCellId != 0xffff)
                            portal.OtherCellId = (ushort)(portal.OtherCellId + offset);
                    }

                    envCell.updateFileContent(file);
                }
            }

            if (verboseLevel > 6)
                Console.WriteLine("{0} cell(s) copied, {1} cell(s) not found.", cellsReplacedCounter, cellsNotFoundCounter);

            return cellsReplacedCounter;
        }

        public void replaceCellNewIdRecursive(uint fromCellId, int cellIdOffset, cDatFile fromDat, bool recurse, bool createOnly, ref int cellsReplacedCounter, ref int cellsNotFoundCounter, List<uint> copiedList, cBuildInfo building, uint parentCellOrLandblock, int verboseLevel = 7)
        {
            uint cellId = (uint)(fromCellId + cellIdOffset);

            if (copiedList.Contains(cellId))
                return;

            if (createOnly && fileCache.ContainsKey(cellId))
                return;

            cDatFileEntry file;
            if (fromDat.fileCache.TryGetValue(fromCellId, out file))
            {
                cEnvCell eEnvCell = new cEnvCell(file);

                if (file.fileFormat == eDatFormat.retail && building != null)
                {
                    foreach (var portal in eEnvCell.Portals)
                    {
                        ushort id = validPortalDatEntries.translateBuildingPortalEnvId(building.ModelId, portal.EnvironmentId);
                        if (id != portal.EnvironmentId)
                        {
                            if (verboseLevel > 6)
                                Console.WriteLine($"INFO: 0x{cellId.ToString("x8")}: BuildInfo.ModelId: 0x{building.ModelId.ToString("x8")}: Translated EnvironmentId in child portal from 0x{portal.EnvironmentId.ToString("x4")} to 0x{id.ToString("x4")}.");
                            portal.EnvironmentId = id;
                        }
                    }
                }

                cDatFileEntry fileNew = new cDatFileEntry(cellId, eDatFormat.ToD);
                eEnvCell.updateFileContent(fileNew);

                copiedList.Add(cellId);
                //fileCache.Add(cellId, fileNew);
                fileCache[cellId] = fileNew;
                cellsReplacedCounter++;

                if (recurse)
                {
                    foreach (var portal in eEnvCell.Portals)
                    {
                        if (portal.OtherCellId == 0xffff)
                            continue;
                        uint connectedCellId = portal.OtherCellId | (fromCellId & 0xFFFF0000);

                        if (copiedList.Contains(connectedCellId))
                            continue;
                        else
                            replaceCellNewIdRecursive(connectedCellId, cellIdOffset, fromDat, recurse, createOnly, ref cellsReplacedCounter, ref cellsNotFoundCounter, copiedList, building, fromCellId, verboseLevel);
                    }

                    foreach (ushort connectedCell in eEnvCell.Cells)
                    {
                        uint connectedCellId = connectedCell | (fromCellId & 0xFFFF0000);

                        if (copiedList.Contains(connectedCellId))
                            continue;
                        else
                            replaceCellNewIdRecursive(connectedCellId, cellIdOffset, fromDat, recurse, createOnly, ref cellsReplacedCounter, ref cellsNotFoundCounter, copiedList, building, fromCellId, verboseLevel);
                    }
                }
                return;
            }

            if (verboseLevel > 0)
                Console.WriteLine($"Couldn't find cell id: 0x{fromCellId.ToString("x8")}. Parent cell: 0x{parentCellOrLandblock.ToString("x8")}.");

            cellsNotFoundCounter++;

            List<uint> parentCellList;
            if (!CellsNotFound.TryGetValue(parentCellOrLandblock, out parentCellList))
            {
                parentCellList = new List<uint>();
                CellsNotFound[parentCellOrLandblock] = parentCellList;
            }
            if (!parentCellList.Contains(fromCellId))
                parentCellList.Add(fromCellId);
        }

        private int removeCell(uint cellId, bool recurse, int verboseLevel = 7)
        {
            int cellsRemovedCounter = 0;
            int cellsNotFoundCounter = 0;
            List<uint> removedList = new List<uint>();

            removeCellRecursive(cellId, recurse, ref cellsRemovedCounter, ref cellsNotFoundCounter, removedList);

            if (verboseLevel > 4)
                Console.WriteLine("{0} cell(s) removed, {1} cell(s) not found.", cellsRemovedCounter, cellsNotFoundCounter);

            return cellsRemovedCounter;
        }

        private bool removeCellRecursive(uint cellId, bool recurse, ref int cellsRemovedCounter, ref int cellsNotFoundCounter, List<uint> removedList, int verboseLevel = 7)
        {
            cDatFileEntry file;
            if (fileCache.TryGetValue(cellId, out file))
            {
                removedList.Add(cellId);
                fileCache.Remove(cellId);
                cellsRemovedCounter++;

                if (recurse)
                {
                    cEnvCell eEnvCell = new cEnvCell(file);

                    foreach (ushort connectedCell in eEnvCell.Cells)
                    {
                        uint connectedCellId = connectedCell | (cellId & 0xFFFF0000);

                        if (removedList.Contains(connectedCellId))
                            continue;
                        else
                            removeCellRecursive(connectedCellId, recurse, ref cellsRemovedCounter, ref cellsNotFoundCounter, removedList);
                    }
                }
                return true;
            }

            if (verboseLevel > 0)
                Console.WriteLine("Couldn't find cell id: {0:x}.", cellId);
            cellsNotFoundCounter++;
            return false;
        }

        public void removeBuildings(uint cellIdInLandblock, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine("Removing all buildings from landblock...");

            uint landblockId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFE;
            cDatFileEntry file;
            if (fileCache.TryGetValue(landblockId, out file))
            {
                cLandblockInfo landblockInfo = new cLandblockInfo(file);

                foreach (var building in landblockInfo.buildings)
                {
                    if (building.Portals.Count > 0 && building.Portals[0].visibleCells.Count > 0)
                    {
                        uint startCellId = building.Portals[0].visibleCells[0] | (landblockId & 0xFFFF0000);
                        removeCell(startCellId, true, verboseLevel);
                    }
                }
                landblockInfo.buildings.Clear();
                landblockInfo.updateFileContent(file);
            }
        }

        public bool addBuildingFrom(uint idToCellInBuilding, cDatFile fromDat, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine("Adding Building...");

            uint landblockId = idToCellInBuilding & 0xFFFF0000 | 0x0000FFFE;
            uint cellId = idToCellInBuilding & 0x0000FFFF;

            cDatFileEntry fromFile;
            if (fromDat.fileCache.TryGetValue(landblockId, out fromFile))
            {
                cLandblockInfo landblockInfoFrom = new cLandblockInfo(fromFile);

                for (int i = 0; i < landblockInfoFrom.buildings.Count; i++)
                {
                    cBuildInfo building = landblockInfoFrom.buildings[i];
                    foreach (cCBldPortal portal in building.Portals)
                    {
                        if (portal.visibleCells.Contains((ushort)cellId))
                        {
                            // We found our building.

                            cDatFileEntry toFile;
                            if (fileCache.TryGetValue(landblockId, out toFile))
                            {
                                cLandblockInfo landblockInfoTo = new cLandblockInfo(toFile);

                                uint lowestCellId;
                                uint cellInBuilding = building.Portals[0].OtherCellId | (landblockId & 0xFFFF0000);
                                int cellsNeeded = countCell(cellInBuilding, fromDat, out lowestCellId);
                                uint newCellId = getNextAvailableCellIdBlock(0x0100 | (landblockId & 0xFFFF0000), cellsNeeded);

                                List<uint> copiedList = new List<uint>();
                                //replaceCellNewId(newCellId, fromDat, lowestCellId, true, false, building, replacedList, landblockId, verboseLevel);
                                copyBuildingCellsNewId(landblockId, building, fromDat, newCellId, copiedList);

                                landblockInfoTo.numCells += (uint)copiedList.Count;
                                landblockInfoTo.buildings.Add(building);

                                landblockInfoTo.updateFileContent(toFile);
                            }

                            return true;
                        }
                    }
                }
            }
            if (verboseLevel > 1)
                Console.WriteLine("Couldn't find building containing cell id: {0:x}.", idToCellInBuilding);
            return false;
        }

        public bool removeBuilding(uint idToCellInBuilding, bool removeCells = false, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine("Removing Building...");

            uint landblockId = idToCellInBuilding & 0xFFFF0000 | 0x0000FFFE;
            uint cellId = idToCellInBuilding & 0x0000FFFF;

            cDatFileEntry file;
            if (fileCache.TryGetValue(landblockId, out file))
            {
                cLandblockInfo landblockInfo = new cLandblockInfo(file);

                for (int i = 0; i < landblockInfo.buildings.Count; i++)
                {
                    cBuildInfo building = landblockInfo.buildings[i];
                    foreach (cCBldPortal portal in building.Portals)
                    {
                        if (portal.visibleCells.Contains((ushort)cellId))
                        {
                            landblockInfo.buildings.RemoveAt(i);

                            if (removeCells) //removing the cells will cause problems if the cells in the landblock become non-continuous(when the building's cells aren't the last ones in the list). todo: a way to resort the remaining cells, but this would also require updating the server database to redirect NPCs and items to the new cell Ids.
                            {
                                uint startCellId = cellId | (landblockId & 0xFFFF0000);
                                removeCell(startCellId, true, verboseLevel);
                            }

                            landblockInfo.updateFileContent(file);

                            return true;
                        }
                    }
                }
            }
            if (verboseLevel > 1)
                Console.WriteLine("Couldn't find building containing cell id: {0:x}.", idToCellInBuilding);
            return false;
        }

        public void removeBuildings(List<uint> listOfBuildingsIds, bool removeCells = false, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine($"Removing {listOfBuildingsIds.Count} Buildings...");

            Stopwatch timer = new Stopwatch();
            timer.Start();

            int buildingsRemovedCounter = 0;
            int buildingsNotFoundCounter = 0;
            foreach (uint buildingId in listOfBuildingsIds)
            {
                if (removeBuilding(buildingId, removeCells, verboseLevel))
                    buildingsRemovedCounter++;
                else
                    buildingsNotFoundCounter++;
            }

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("{0} building(s) removed in {1} seconds. {2} building(s) not found.", buildingsRemovedCounter, timer.ElapsedMilliseconds / 1000f, buildingsNotFoundCounter);
        }

        public void replaceDungeon(ushort dungeonId, cDatFile fromDat, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine($"Replacing dungeon 0x{dungeonId.ToString("x4")}...");
            uint dungeonLandblock = (uint)(dungeonId << 16 | 0x0000FFFF);
            replaceLandblock(dungeonLandblock, fromDat, true, true, true, true, false, 0, verboseLevel);
        }

        public void replaceDungeonList(List<ushort> listOfDungeonIds, cDatFile fromDat, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine($"Replacing {listOfDungeonIds.Count} dungeons...");
            foreach (var dungeonId in listOfDungeonIds)
            {
                replaceDungeon(dungeonId, fromDat, verboseLevel);
            }
        }

        public void replaceLandblockList(List<uint> listOfLandblockIds, cDatFile fromDat, bool heightmap = true, bool textures = true, bool objects = true, bool cells = true, bool createOnly = false, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine($"Replacing {listOfLandblockIds.Count} landblocks...");
            foreach (var landblockId in listOfLandblockIds)
            {
                replaceLandblock(landblockId, fromDat, heightmap, textures, objects, cells, createOnly, 0, verboseLevel);
            }
        }

        public void clearLandblockCellsList(List<uint> listOfLandblockIds, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine($"Removing cells from {listOfLandblockIds.Count} landblocks...");
            foreach (var landblockId in listOfLandblockIds)
            {
                clearLandblockCells(landblockId, verboseLevel);
            }
        }

        public bool clearLandblockCells(uint cellIdInLandblock, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine($"Removing cells from landblock...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            uint landblockId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFF;
            uint landblockInfoId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFE;
            cDatFileEntry landblockInfoFile;

            int counter = 0;
            if (fileCache.TryGetValue(landblockInfoId, out landblockInfoFile))
            {
                cLandblockInfo landblockInfo = new cLandblockInfo(landblockInfoFile);
                landblockInfo.numCells = 0;

                landblockInfo.updateFileContent(landblockInfoFile);

                for (uint i = 0; i < 0xFFFE; i++)
                {
                    uint id = i | (landblockId & 0xFFFF0000);
                    if (fileCache.ContainsKey(id))
                    {
                        fileCache.Remove(id);
                        counter++;
                    }
                }
            }

            timer.Stop();
            if (verboseLevel > 5)
                Console.WriteLine("{0} cells removed in {1} seconds.", counter, timer.ElapsedMilliseconds / 1000f);
            return true;
        }

        //public void replaceCellList(List<uint> listOfCellIds, cDatFile fromDat, int verboseLevel = 5)
        //{
        //    if (verboseLevel > 1)
        //        Console.WriteLine($"Replacing {listOfCellIds.Count} cells...");

        //    List<uint> replacedList = new List<uint>();
        //    int cellsReplaced = 0;
        //    foreach (var cellId in listOfCellIds)
        //    {
        //        cellsReplaced += replaceCell(cellId, fromDat, false, null, replacedList, landblockId, verboseLevel);
        //    }

        //    if (verboseLevel > 1)
        //        Console.WriteLine($"Replaced {cellsReplaced} of {listOfCellIds.Count} cells.");
        //}

        public void replaceCellList(Dictionary<uint, List<uint>> listOfParentCellIds, cDatFile fromDat, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine($"Replacing cells...");

            List<uint> replacedList = new List<uint>();
            int cellsReplaced = 0;
            foreach (var listOfCellIds in listOfParentCellIds)
            {
                foreach (var cellId in listOfCellIds.Value)
                {
                    cellsReplaced += replaceCell(cellId, cellId, fromDat, true, true, null, replacedList, listOfCellIds.Key, verboseLevel);
                }
            }

            List<uint> emptyKeys = new List<uint>();
            int replacedCount = 0;
            bool found = false;
            foreach (var replacedEntry in replacedList)
            {
                foreach (var entry in CellsNotFound)
                {
                    if (entry.Value.Remove(replacedEntry))
                        found = true;

                    if (entry.Value.Count == 0)
                        emptyKeys.Add(entry.Key);
                }
                if (found)
                    replacedCount++;
                found = false;
            }

            foreach (var emptyEntry in emptyKeys)
            {
                CellsNotFound.Remove(emptyEntry);
            }

            if (verboseLevel > 1)
                Console.WriteLine($"Replaced {replacedCount} cells and added an extra {cellsReplaced - replacedCount} connected ones.");
        }

        public bool isMissingCells
        {
            get { return CellsNotFound.Count > 0; }
        }

        public List<uint> getMissingCellsList()
        {
            List<uint> cellsList = new List<uint>();
            foreach (var entry in CellsNotFound)
            {
                foreach (var cell in entry.Value)
                {
                    if (!cellsList.Contains(cell))
                        cellsList.Add(cell);
                }
            }

            cellsList.Sort();
            return cellsList;
        }

        public bool isMissingLandblocks(out int missingLandblockAmount)
        {
            missingLandblockAmount = 65025 - countLandblocks();
            return countLandblocks() < 65025;
        }

        public int countLandblocks()
        {
            int landblocksCounter = 0;
            foreach (var file in fileCache)
            {
                if ((file.Key & 0x0000FFFF) == 0x0000FFFF)
                    landblocksCounter++;
            }
            return landblocksCounter;
        }

        public void completeLandblocksFrom(cDatFile fromDat, int verboseLevel = 5)
        {
            int totalCount = countLandblocks();
            if (countLandblocks() >= 65025)
            {
                if (verboseLevel > 1)
                    Console.WriteLine($"We have all {totalCount} landblocks.");
                return;
            }

            if (verboseLevel > 1)
                Console.WriteLine("Looking for missing landblocks...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landblocksCounter = 0;

            List<uint> landblockIds = new List<uint>();
            foreach (var file in fromDat.fileCache)
            {
                if ((file.Key & 0x0000FFFF) == 0x0000FFFF)
                    landblockIds.Add(file.Key);
            }

            foreach (var landblockId in landblockIds)
            {
                if (!fileCache.ContainsKey(landblockId))
                {
                    if (replaceLandblock(landblockId, fromDat, true, true, true, true, true, 0, verboseLevel))
                        landblocksCounter++;
                }
            }

            timer.Stop();
            if (verboseLevel > 1)
            {
                totalCount += landblocksCounter;
                if (totalCount >= 65025)
                {
                    if (verboseLevel > 1)
                        Console.WriteLine("{0} landblocks added in {1} seconds. We now have all {2} landblocks.", landblocksCounter, timer.ElapsedMilliseconds / 1000f, totalCount);
                }
                else
                {
                    if (verboseLevel > 1)
                        Console.WriteLine("{0} landblocks added in {1} seconds. Still missing {2} landblocks.", landblocksCounter, timer.ElapsedMilliseconds / 1000f, 65025 - totalCount);
                }
            }
        }

        public void completeCellsFrom(cDatFile fromDat, bool truncateNotFound = false, int verboseLevel = 5)
        {
            if (CellsNotFound.Count > 0)
                replaceCellList(CellsNotFound, fromDat, verboseLevel);

            if (truncateNotFound && isMissingCells)
            {
                if (verboseLevel > 1)
                    Console.WriteLine($"Still missing {getMissingCellsList().Count} cells, truncating...");
                truncateMissingCells(verboseLevel);
            }
            else if (isMissingCells)
            {
                if (verboseLevel > 1)
                    Console.WriteLine($"Still missing {getMissingCellsList().Count} cells.");
            }
            else
            {
                if (verboseLevel > 1)
                    Console.WriteLine($"All missing cells found!");
            }
        }

        public void truncateMissingCells(int verboseLevel = 5)
        {
            int removedCounter = 0;
            int totalAmount = 0;
            if (CellsNotFound.Count > 0)
            {
                List<uint> cellsList = new List<uint>();
                foreach (var entry in CellsNotFound)
                {
                    cellsList.AddRange(entry.Value);
                }

                totalAmount = cellsList.Count;

                foreach (var entry in CellsNotFound)
                {
                    if (entry.Key == 0x00000000)
                    {
                        //we have no parent cell!
                    }
                    else
                    {
                        cDatFileEntry file;
                        if (fileCache.TryGetValue(entry.Key, out file))
                        {
                            cEnvCell parentCell = new cEnvCell(file);
                            foreach (var missingCell in entry.Value)
                            {
                                ushort localId = (ushort)missingCell;
                                parentCell.Cells.Remove(localId);

                                List<int> portalsToRemove = new List<int>();
                                for (int i = 0; i < parentCell.Portals.Count; i++)
                                {
                                    if (parentCell.Portals[i].OtherCellId == localId)
                                        portalsToRemove.Add(i);
                                }
                                foreach (var portalId in portalsToRemove)
                                {
                                    parentCell.Portals.RemoveAt(portalId);
                                }
                                removedCounter++;

                                parentCell.updateFileContent(file);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Parent cell not found: 0x{entry.Key.ToString("x8")}.");
                        }
                    }
                }
                CellsNotFound.Clear();
            }
            Console.WriteLine($"Removed {removedCounter} references to these entries, {totalAmount - removedCounter} missing references remain.");
        }

        public void convertRetailToToD(int iteration = 4, int verboseLevel = 5)
        {
            if (fileFormat == eDatFormat.ToD)
            {
                Console.WriteLine("File is already in the Throne of Destiny format. No conversion necessary.");
                return;
            }

            SetFileIteration(iteration); //add iteration file that retail does not have.
            if (verboseLevel > 1)
                Console.WriteLine("Converting files to Throne of Destiny format...");
            convertAllLandblocks(verboseLevel);
        }

        public void convertAllLandblocks(int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine("Rewriting all landblocks...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landblocksCounter = 0;

            List<uint> landblockIds = new List<uint>();
            foreach (var file in fileCache)
            {
                if ((file.Key & 0x0000FFFF) == 0x0000FFFF)
                    landblockIds.Add(file.Key);
            }

            foreach (var landblockId in landblockIds)
            {
                if (convertLandblock(landblockId, verboseLevel))
                    landblocksCounter++;
            }

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("{0} landblocks converted in {1} seconds.", landblocksCounter, timer.ElapsedMilliseconds / 1000f);
        }

        public void replaceAllLandblocks(cDatFile fromDat, bool heightmap = true, bool textures = true, bool objects = true, bool cells = true, bool createOnly = false, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine("Replacing all landblocks...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landblocksCounter = 0;

            for (uint landblockId = 0x00000000; landblockId < 0xFFFF0000; landblockId += 0x00010000)
            {
                if (replaceLandblock(landblockId, fromDat, heightmap, textures, objects, cells, createOnly, 0, verboseLevel))
                    landblocksCounter++;
            }

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("{0} landblocks replaced in {1} seconds.", landblocksCounter, timer.ElapsedMilliseconds / 1000f);
        }

        public void replaceLandblockList(ushort newTopLeft, List<uint> listOfLandblocks, cDatFile fromDat, bool heightmap = true, bool textures = true, bool cells = true, bool objects = true, int verboseLevel = 5)
        {
            byte newTopLeftX = (byte)((newTopLeft & 0xFF00) >> 8);
            byte newTopLeftY = (byte)(newTopLeft & 0x00FF);

            byte topLeftmostX = byte.MaxValue;
            byte topLeftmostY = 0;
            foreach (var entry in listOfLandblocks)
            {
                byte entryX = (byte)(entry >> 24);
                byte entryY = (byte)(entry >> 16);

                if (entryX < topLeftmostX)
                    topLeftmostX = entryX;

                if (entryY > topLeftmostY)
                    topLeftmostY = entryY;
            }

            int movementOffsetX = newTopLeftX - topLeftmostX;
            int movementOffsetY = newTopLeftY - topLeftmostY;

            if (verboseLevel > 1)
                Console.WriteLine($"Replacing {listOfLandblocks.Count} landblocks starting at {newTopLeft.ToString("x4")}.");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landblocksAddedCounter = 0;
            int landblocksNotFoundCounter = 0;

            uint landblockId;
            uint newlandblockId;
            foreach (var entry in listOfLandblocks)
            {
                byte entryX = (byte)(entry >> 24);
                byte entryY = (byte)(entry >> 16);
                landblockId = (uint)(entryX << 24 | entryY << 16 | 0x0000FFFF);

                newlandblockId = (uint)((entryX + movementOffsetX) << 24 | (entryY + movementOffsetY) << 16 | 0x0000FFFF);

                if (replaceLandblock(landblockId, fromDat, heightmap, textures, objects, cells, false, newTopLeft == 0 ? 0 : newlandblockId, verboseLevel - 1))
                    landblocksAddedCounter++;
                else
                    landblocksNotFoundCounter++;
            }

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("{0} landblocks replaced in {1} seconds. {2} landblocks unchanged", landblocksAddedCounter, timer.ElapsedMilliseconds / 1000f, landblocksNotFoundCounter);
        }

        public void replaceLandblock(ushort landblockId, cDatFile fromDat, ushort newLandblockId = 0, bool heightmap = true, bool textures = true, bool cells = true, bool objects = true, int verboseLevel = 5)
        {
            var fullLandblockId = (uint)(landblockId << 16 | 0x0000FFFF);
            var fullNewLandblockId = (uint)(newLandblockId << 16 | 0x0000FFFF);
            replaceLandblock(fullLandblockId, fromDat, heightmap, textures, objects, cells, false, newLandblockId == 0 ? 0 : fullNewLandblockId, verboseLevel);
        }

        public void replaceLandblockRect(ushort topLeftLandblock, ushort bottomRightLandblock, cDatFile fromDat, ushort newTopLeft = 0, bool heightmap = true, bool textures = true, bool cells = true, bool objects = true, int verboseLevel = 5)
        {
            byte topLeftX = (byte)((topLeftLandblock & 0xFF00) >> 8);
            byte topLeftY = (byte)(topLeftLandblock & 0x00FF);

            byte bottomRightX = (byte)((bottomRightLandblock & 0xFF00) >> 8);
            byte bottomRightY = (byte)(bottomRightLandblock & 0x00FF);

            byte newTopLeftX = (byte)((newTopLeft & 0xFF00) >> 8);
            byte newTopLeftY = (byte)(newTopLeft & 0x00FF);

            int topLeftOffsetX = newTopLeftX - topLeftX;
            int topLeftOffsetY = newTopLeftY - topLeftY;

            if (verboseLevel > 1)
                Console.WriteLine($"Replacing landblocks in a rectangle from {topLeftLandblock.ToString("x4")} to {bottomRightLandblock.ToString("x4")}.");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landblocksAddedCounter = 0;
            int landblocksNotFoundCounter = 0;

            uint landblockId;
            uint newlandblockId;
            for (byte x = topLeftX; x <= bottomRightX; x++)
            {
                for (byte y = bottomRightY; y <= topLeftY; y++)
                {
                    landblockId = (uint)(x << 24 | y << 16 | 0x0000FFFF);
                    newlandblockId = (uint)((x + topLeftOffsetX) << 24 | (y + topLeftOffsetY) << 16 | 0x0000FFFF);
                    if (replaceLandblock(landblockId, fromDat, heightmap, textures, objects, cells, false, newTopLeft == 0 ? 0 : newlandblockId, verboseLevel))
                        landblocksAddedCounter++;
                    else
                        landblocksNotFoundCounter++;
                }
            }

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("{0} landblocks replaced in {1} seconds. {2} landblocks unchanged", landblocksAddedCounter, timer.ElapsedMilliseconds / 1000f, landblocksNotFoundCounter);
        }

        public void replaceLandblockArea(uint baseLandblock, cDatFile fromDat, bool heightmap = true, bool textures = true, bool cells = true, bool objects = true, int verboseLevel = 5)
        {
            uint startLandblockId = baseLandblock & 0xFF000000 | 0x0000FFFF;
            int gridSize = 9;

            byte startX = (byte)((baseLandblock & 0xFF000000) >> 24);
            byte startY = (byte)((baseLandblock & 0x00FF0000) >> 16);

            float radius = gridSize / 2.0f;

            startX -= (byte)radius;
            startY -= (byte)radius;
            byte endX = (byte)(startX + Math.Ceiling(radius));
            byte endY = (byte)(startY + Math.Ceiling(radius));

            if (verboseLevel > 1)
                Console.WriteLine($"Replacing landblocks in a {gridSize}x{gridSize} grid centered around 0x{startLandblockId.ToString("x8")}.");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landblocksAddedCounter = 0;
            int landblocksNotFoundCounter = 0;

            uint landblockId;
            for (byte x = startX; x <= endX; x++)
            {
                for (byte y = startY; y <= endY; y++)
                {
                    landblockId = (uint)(x << 24 | y << 16 | 0x0000FFFF);
                    if (replaceLandblock(landblockId, fromDat, heightmap, textures, objects, cells, false, 0, verboseLevel))
                        landblocksAddedCounter++;
                    else
                        landblocksNotFoundCounter++;
                }
            }

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("{0} landblocks replaced in {1} seconds. {2} landblocks unchanged", landblocksAddedCounter, timer.ElapsedMilliseconds / 1000f, landblocksNotFoundCounter);
        }

        public void replaceLandblocksTerrain(List<uint> listOfLandblocks, cDatFile fromFile, bool heightmap = true, bool textures = true, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine($"Replacing {listOfLandblocks.Count} landblocks terrain...");

            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landblockReplacedCounter = 0;
            int landblockNotFoundCounter = 0;
            foreach (uint landblockId in listOfLandblocks)
            {
                if (replaceLandblockTerrain(landblockId, fromFile, heightmap, textures, false, verboseLevel))
                    landblockReplacedCounter++;
                else
                    landblockNotFoundCounter++;
            }

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("{0} landblock(s) terrain replaced in {1} seconds. {2} landblock(s) not found.", landblockReplacedCounter, timer.ElapsedMilliseconds / 1000f, landblockNotFoundCounter);
        }

        public void removeNoobPillarFromLandblocks(List<uint> listOfLandblocks, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine($"Removing noob pillars from {listOfLandblocks.Count} landblocks...");

            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landblockReplacedCounter = 0;
            int landblockNotFoundCounter = 0;
            foreach (uint landblockId in listOfLandblocks)
            {
                if (removeNoobPillarFromLandblock(landblockId, verboseLevel))
                    landblockReplacedCounter++;
                else
                    landblockNotFoundCounter++;
            }

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("Removed noob pillars from {0} landblock(s) in {1} seconds. {2} landblock(s) not found.", landblockReplacedCounter, timer.ElapsedMilliseconds / 1000f, landblockNotFoundCounter);
        }

        public void removeRoadsFromLandblocks(List<uint> listOfLandblocks, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine($"Removing roads from {listOfLandblocks.Count} landblocks...");

            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landblockReplacedCounter = 0;
            int landblockNotFoundCounter = 0;
            foreach (uint landblockId in listOfLandblocks)
            {
                if (removeRoadFromLandblock(landblockId, verboseLevel))
                    landblockReplacedCounter++;
                else
                    landblockNotFoundCounter++;
            }

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("Removed roads from {0} landblock(s) in {1} seconds. {2} landblock(s) not found.", landblockReplacedCounter, timer.ElapsedMilliseconds / 1000f, landblockNotFoundCounter);
        }

        public List<uint> getListOfLandblocksInRect(ushort topLeftLandblock, ushort bottomRightLandblock)
        {
            byte topLeftX = (byte)((topLeftLandblock & 0xFF00) >> 8);
            byte topLeftY = (byte)(topLeftLandblock & 0x00FF);

            byte bottomRightX = (byte)((bottomRightLandblock & 0xFF00) >> 8);
            byte bottomRightY = (byte)(bottomRightLandblock & 0x00FF);

            var list = new List<uint>();
            for (byte x = topLeftX; x <= bottomRightX; x++)
            {
                for (byte y = bottomRightY; y <= topLeftY; y++)
                {
                    var landblockId = (uint)(x << 24 | y << 16 | 0x0000FFFF);
                    if (fileCache.TryGetValue(landblockId, out _))
                        list.Add(landblockId);
                }
            }
            return list;
        }

        public void replaceLandblocksSpecificTexture(List<uint> listOfLandblocks, ushort texId, ushort replacementTexId, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine($"Modifying {listOfLandblocks.Count}: TextureId from 0x{texId.ToString("x4")} to 0x{replacementTexId.ToString("x4")}...");

            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landblockReplacedCounter = 0;
            int landblockNotFoundCounter = 0;
            foreach (uint landblockId in listOfLandblocks)
            {
                if (replaceLandblockSpecificTexture(landblockId, texId, replacementTexId, verboseLevel))
                    landblockReplacedCounter++;
                else
                    landblockNotFoundCounter++;
            }

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("Modified {0} landblock(s) in {1} seconds. {2} landblock(s) not found.", landblockReplacedCounter, timer.ElapsedMilliseconds / 1000f, landblockNotFoundCounter);
        }

        public void landblockBucketFill(List<uint> listOfLandblocks, ushort texId, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine($"Modifying {listOfLandblocks.Count}: TextureId to 0x{texId.ToString("x4")}...");

            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landblockReplacedCounter = 0;
            int landblockNotFoundCounter = 0;
            foreach (uint landblockId in listOfLandblocks)
            {
                if (landblockBucketFill(landblockId, texId, verboseLevel))
                    landblockReplacedCounter++;
                else
                    landblockNotFoundCounter++;
            }

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("Modified {0} landblock(s) in {1} seconds. {2} landblock(s) not found.", landblockReplacedCounter, timer.ElapsedMilliseconds / 1000f, landblockNotFoundCounter);
        }

        public void replaceLandblocks(List<uint> listOfLandblocks, cDatFile fromFile, bool heightmap = true, bool textures = true, bool objects = true, bool cells = true, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine($"Replacing {listOfLandblocks.Count} landblocks...");

            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landblockReplacedCounter = 0;
            int landblockNotFoundCounter = 0;
            foreach (uint landblockId in listOfLandblocks)
            {
                if (replaceLandblock(landblockId, fromFile, heightmap, textures, objects, cells, false, 0, verboseLevel))
                    landblockReplacedCounter++;
                else
                    landblockNotFoundCounter++;
            }

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("{0} landblock(s) replaced in {1} seconds. {2} landblock(s) not found.", landblockReplacedCounter, timer.ElapsedMilliseconds / 1000f, landblockNotFoundCounter);
        }

        public void replaceLandblocksSpecialForStarterOutposts(List<uint> listOfLandblocks, cDatFile fromFile, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine($"Replacing {listOfLandblocks.Count} landblocks...");

            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landblockReplacedCounter = 0;
            int landblockNotFoundCounter = 0;
            foreach (uint landblockId in listOfLandblocks)
            {
                if (replaceLandblockSpecialForStarterOutposts(landblockId, fromFile, false, verboseLevel))
                    landblockReplacedCounter++;
                else
                    landblockNotFoundCounter++;
            }

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("{0} landblock(s) replaced in {1} seconds. {2} landblock(s) not found.", landblockReplacedCounter, timer.ElapsedMilliseconds / 1000f, landblockNotFoundCounter);
        }

        public void removeHouseSettlements(cDatFile datFileWithoutSettlements) // The ideal cell without settlements is one from after the obsidian span was added and before the first housing settlements were added. I used one from 2000-12-31.
        {
            List<uint> listOfSettlements = new List<uint>();
            List<uint> listOfSettlementsPreserveTexture = new List<uint>();
            List<uint> listOfSettlementsPreserveSurface = new List<uint>();
            loadSettlementListFromFile("./input/ListOfSettlementLandblocks.txt", listOfSettlements, listOfSettlementsPreserveTexture, listOfSettlementsPreserveSurface);

            replaceLandblocks(listOfSettlements, datFileWithoutSettlements);
            removeNoobPillarFromLandblocks(listOfSettlements); // Some landblocks have noob pillars under the houses!

            replaceLandblocks(listOfSettlementsPreserveTexture, datFileWithoutSettlements, true, false, true);
            removeRoadsFromLandblocks(listOfSettlementsPreserveTexture);

            replaceLandblocks(listOfSettlementsPreserveSurface, datFileWithoutSettlements, false, false, false);
        }

        public void addGridToAllLandblocks(int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine("Adding grid to all landblocks...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landblocksCounter = 0;

            List<uint> landblockIds = new List<uint>();
            foreach (var file in fileCache)
            {
                if ((file.Key & 0x0000FFFF) == 0x0000FFFF)
                    landblockIds.Add(file.Key);
            }

            foreach (var landblockId in landblockIds)
            {
                if (addGridToLandblock(landblockId, verboseLevel))
                    landblocksCounter++;
            }

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("Added grid to {0} landblocks in {1} seconds.", landblocksCounter, timer.ElapsedMilliseconds / 1000f);
        }

        public bool addGridToLandblock(uint cellIdInLandblock,int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine($"Adding grid to landblock...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            cDatFile toDat = this; //just to make things less confusing

            uint landblockId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFF;

            cDatFileEntry landBlockFile;
            cCellLandblock landblock;

            bool existsOnDestination = toDat.fileCache.TryGetValue(landblockId, out landBlockFile);

            if (existsOnDestination)
            {
                landblock = new cCellLandblock(landBlockFile);

                for (int i = 0; i < landblock.Terrain.Count; i++)
                {
                    if (i < 9 || i % 9 == 0)
                        landblock.Terrain[i] = 27;
                }

                landblock.updateFileContent(landBlockFile);
            }

            timer.Stop();
            if (verboseLevel > 5)
                Console.WriteLine("Added grid to landblock in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            return true;
        }

        public cDatFileEntry getFile(uint fileId)
        {
            cDatFileEntry file;
            if (fileCache.TryGetValue(fileId, out file))
                return file;
            return null;
        }

        public bool copyFile(uint fileId, uint destinationFileId, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine("Copying File...");
            Stopwatch timer = new Stopwatch();
            timer.Start();
            cDatFileEntry file = getFile(fileId);

            cDatFileEntry newFile = new cDatFileEntry(destinationFileId, eDatFormat.ToD);

            newFile.copyFrom(file);
            newFile.fileId = destinationFileId;

            if (fileCache.ContainsKey(destinationFileId))
                fileCache.Remove(destinationFileId);
            fileCache.Add(destinationFileId, newFile);

            timer.Stop();
            if (verboseLevel > 5)
                Console.WriteLine("Copied file in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            return true;
        }

        public bool addFilesFromFolder(string folderName, eDatFormat fileFormat = eDatFormat.ToD, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine($"Adding files from folder: {folderName}...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            var di = new DirectoryInfo(folderName);

            if (di == null)
            {
                Console.WriteLine($"Unable to open {folderName}");
                return false;
            }

            var fileList = di.GetFiles("*.bin", SearchOption.AllDirectories);

            var successCount = 0;
            foreach(var entry in fileList)
            {
                if (addFile(entry.FullName, fileFormat, verboseLevel))
                    successCount++;
            }

            timer.Stop();
            if (verboseLevel > 5)
                Console.WriteLine($"Added {successCount}/{fileList.Length} files in {timer.ElapsedMilliseconds / 1000f} seconds.");

            return successCount == fileList.Length;
        }

        public bool addFile(string filename, eDatFormat fileFormat = eDatFormat.ToD, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine($"Adding file {filename}...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine($"Unable to open {filename}");
                return false;
            }

            var fileId = Utils.readUInt32(inputFile);
            inputFile.BaseStream.Position = 0;

            var filenameWithoutPath = Path.GetFileName(filename);
            if (filenameWithoutPath.StartsWith("08")) //Surface files have no id in them, get it from filename
            {
                if (!uint.TryParse(filenameWithoutPath.Remove(8), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out fileId))
                    throw new Exception("Couldn't find the id of this file.");
            }

            cDatFileEntry file = new cDatFileEntry(fileId, fileFormat);
            file.updateFileContentFromStream(inputFile);

            if (fileCache.ContainsKey(file.fileId))
                fileCache.Remove(file.fileId);
            fileCache.Add(file.fileId, file);

            timer.Stop();
            if (verboseLevel > 5)
                Console.WriteLine($"Added file in {timer.ElapsedMilliseconds / 1000f} seconds.");
            return true;
        }

        public void UpdateIterationForChangedFiles(int iteration, uint timestamp)
        {
            Console.WriteLine($"Updating file iteration to {iteration} for files with timestamp equal to or newer than {timestamp}...");

            var counter = 0;

            foreach (var file in fileCache.Values)
            {
                if (file.fileId == 0xFFFF0001) continue;
                
                if (file.timeStamp >= timestamp)
                {
                    //Console.WriteLine($"0x{file.fileId:X8}.version == {file.version} | now {iteration}");
                    file.version = (uint)iteration;
                    counter++;
                }
                //else
                    //Console.WriteLine($"0x{file.fileId:X8}.version == {file.version}");
            }

            Console.WriteLine($"{counter} files updated.");
        }
    }
}