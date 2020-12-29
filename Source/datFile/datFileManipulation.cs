using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Melt
{
    public partial class cDatFile
    {
        public int GetFileIteration()
        {
            cDatFileEntry iterationFile;
            if (fileCache.TryGetValue(0xFFFF0001, out iterationFile))
            {
                byte[] buffer = new byte[1024];
                StreamReader reader = new StreamReader(iterationFile.fileContent);

                List<int> ints = new List<int>();
                bool sorted;

                ints.Add(Utils.ReadInt32(buffer, reader));
                ints.Add(Utils.ReadInt32(buffer, reader));

                sorted = Utils.ReadBool(buffer, reader);

                Utils.Align(reader);

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
            if (dataSet == 0x00000001) //portal
                Utils.writeInt32(-iteration, outputStream);
            else
                Utils.writeInt32((int)(0xffffffff - iteration + 1), outputStream);
            Utils.writeBool(true, outputStream);
            Utils.Align(outputStream);
            outputStream.Flush();

            iterationFile.fileContent.Seek(0, SeekOrigin.Begin);
            iterationFile.fileSize = (int)iterationFile.fileContent.Length;

            fileCache[0xFFFF0001] = iterationFile;
        }

        public bool clearLandblock(uint cellIdInLandblock, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine("Clearing landblock...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            uint landblockId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFF;

            int removedCounter = 0;
            for (uint i = 0; i <= 0xFFFF; i++)
            {
                uint id = i | (landblockId & 0xFFFF0000);

                cDatFileEntry entry;
                if (fileCache.TryGetValue(id, out entry))
                {
                    fileCache.Remove(id);
                    removedCounter++;
                }
            }

            timer.Stop();
            if (verboseLevel > 5)
                Console.WriteLine("Landblock cleared in {0} seconds. {1} files deleted.", timer.ElapsedMilliseconds / 1000f, removedCounter);
            return true;
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
                byte[] buffer = new byte[1024];
                StreamReader reader = new StreamReader(landblockFile.fileContent);
                cCellLandblock landblock = new cCellLandblock(buffer, reader);

                landblock.updateFileContent(landblockFile);
            }

            uint landblockInfoId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFE;
            cDatFileEntry landblockInfoFile;
            if (fileCache.TryGetValue(landblockInfoId, out landblockInfoFile))
            {
                byte[] buffer = new byte[1024];
                StreamReader readerFrom = new StreamReader(landblockInfoFile.fileContent);
                cLandblockInfo landblockInfo = new cLandblockInfo(buffer, readerFrom, landblockInfoFile.fileFormat);

                int cellsWritten = 0;
                uint startCellId;
                List<uint> replacedList = new List<uint>();
                foreach (var newBuilding in landblockInfo.Buildings)
                {
                    if (newBuilding.Portals.Count > 0 && newBuilding.Portals[0].visibleCells.Count > 0)
                    {
                        startCellId = newBuilding.Portals[0].visibleCells[0] | (landblockInfoId & 0xFFFF0000);
                        cellsWritten += replaceCell(startCellId, this, true, false, newBuilding, replacedList, landblockId, verboseLevel);
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
                        cellsWritten += replaceCell(entry.fileId, this, true, false, null, replacedList, landblockId, verboseLevel);
                    }
                }

                //int a = 0;
                //if (landblockInfoFrom.NumCells > cellsAdded || cells.Count > 0)
                //    a = 1;

                landblockInfoFile.fileFormat = eDatFormat.ToD;
                landblockInfo.updateFileContent(landblockInfoFile);
            }

            timer.Stop();
            if (verboseLevel > 5)
                Console.WriteLine("Landblock rewritten in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            return true;
        }

        public bool replaceLandblock(uint cellIdInLandblock, cDatFile fromDat, bool createOnly = false, int verboseLevel = 6)
        {
            if (verboseLevel > 5)
                Console.WriteLine("Replacing landblock...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            cDatFile toDat = this; //just to make things less confusing

            uint landblockId = cellIdInLandblock & 0xFFFF0000 | 0x0000FFFF;

            cDatFileEntry toLandBlockFile;
            cDatFileEntry fromLandblockFile;
            if (fromDat.fileCache.TryGetValue(landblockId, out fromLandblockFile))
            {
                byte[] bufferFrom = new byte[1024];
                StreamReader readerFrom = new StreamReader(fromLandblockFile.fileContent);
                cCellLandblock landblockFrom = new cCellLandblock(bufferFrom, readerFrom);
                cCellLandblock landblockInfoTo;

                bool existsOnDestination = toDat.fileCache.TryGetValue(landblockId, out toLandBlockFile);

                if (!existsOnDestination || !createOnly)
                {
                    if (existsOnDestination)
                    {
                        //cCellLandblock exists on origin and destination, replace.
                        //byte[] bufferTo = new byte[1024];
                        //StreamReader readerTo = new StreamReader(toLandBlockFile.fileContent);
                        //landblockInfoTo = new cCellLandblock(bufferTo, readerTo);

                        // Here is where we would manipulate the origin files before copying them over, nothing for now.

                        landblockInfoTo = landblockFrom;
                    }
                    else
                    {
                        toLandBlockFile = new cDatFileEntry(landblockId, eDatFormat.ToD);
                        landblockInfoTo = landblockFrom;
                    }

                    landblockInfoTo.updateFileContent(toLandBlockFile);

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
                byte[] bufferFrom = new byte[1024];
                StreamReader readerFrom = new StreamReader(fromLandblockInfoFile.fileContent);
                cLandblockInfo landblockInfoFrom = new cLandblockInfo(bufferFrom, readerFrom, fromLandblockInfoFile.fileFormat);

                byte[] bufferTo = new byte[1024];
                StreamReader readerTo;
                cLandblockInfo landblockInfoTo;

                bool existsOnDestination = toDat.fileCache.TryGetValue(landblockInfoId, out toLandblockInfoFile);
                if (!existsOnDestination || !createOnly)
                {
                    if (existsOnDestination)
                    {
                        //cLandblockInfo exists on origin and destination, replace.
                        readerTo = new StreamReader(toLandblockInfoFile.fileContent);
                        landblockInfoTo = new cLandblockInfo(bufferTo, readerTo, toLandblockInfoFile.fileFormat);

                        for (uint i = 0; i < 0xFFFE; i++)
                        {
                            uint id = i | (landblockId & 0xFFFF0000);
                            toDat.fileCache.Remove(id);
                        }
                    }
                    else
                    {
                        landblockInfoTo = landblockInfoFrom;
                        toLandblockInfoFile = new cDatFileEntry(landblockInfoId, eDatFormat.ToD);
                    }

                    int cellsAdded = 0;
                    uint startCellId;
                    List<uint> replacedList = new List<uint>();
                    foreach (var newBuilding in landblockInfoFrom.Buildings)
                    {
                        if (newBuilding.Portals.Count > 0 && newBuilding.Portals[0].visibleCells.Count > 0)
                        {
                            startCellId = newBuilding.Portals[0].visibleCells[0] | (landblockInfoId & 0xFFFF0000);
                            cellsAdded += replaceCell(startCellId, fromDat, true, false, newBuilding, replacedList, landblockId, verboseLevel);
                        }
                    }

                    //if (landblockInfoFrom.NumCells > 0)
                    //{
                    //    startCellId = 0x0100 | (landblockInfoId & 0xFFFF0000);
                    //    if (!replacedList.Contains(startCellId))
                    //        cellsAdded += replaceCell(startCellId, fromDat, true, false, null, replacedList, landblockId, verboseLevel);
                    //}

                    List<cDatFileEntry> cells = new List<cDatFileEntry>();
                    for (uint i = 0; i < 0xFFFE; i++)
                    {
                        uint id = i | (landblockId & 0xFFFF0000);

                        cDatFileEntry entry;
                        if (fromDat.fileCache.TryGetValue(id, out entry))
                            cells.Add(entry);
                    }

                    List<uint> missingList = new List<uint>();
                    foreach (var entry in cells)
                    {
                        if (!replacedList.Contains(entry.fileId))
                        {
                            //ideally I would like to get at these cells from a reference and not by searching for them.
                            missingList.Add(entry.fileId);
                            cellsAdded += replaceCell(entry.fileId, fromDat, true, false, null, replacedList, landblockId, verboseLevel);
                        }
                    }

                    //int a = 0;
                    //if (landblockInfoFrom.NumCells > cellsAdded || cells.Count > 0)
                    //    a = 1;

                    landblockInfoTo = landblockInfoFrom;
                    landblockInfoTo.updateFileContent(toLandblockInfoFile);

                    if (!existsOnDestination)
                    {
                        //cCellLandblock exists on origin but not on destination, add.
                        toDat.fileCache.Add(landblockInfoId, fromLandblockInfoFile);
                    }
                }
            }
            else
            {
                if (toDat.fileCache.TryGetValue(landblockInfoId, out toLandblockInfoFile))
                {
                    //cLandblockInfo exists on destination but not on origin, delete.
                    if (!createOnly)
                    {
                        //byte[] bufferTo = new byte[1024];
                        //StreamReader readerTo = new StreamReader(toLandblockInfoFile.fileContent);
                        //cLandblockInfo landblockInfoTo = new cLandblockInfo(bufferTo, readerTo, toLandblockInfoFile.fileFormat);

                        toDat.fileCache.Remove(landblockInfoId);

                        for (uint i = 0; i < 0xFFFE; i++)
                        {
                            uint id = i | (landblockId & 0xFFFF0000);
                            toDat.fileCache.Remove(id);
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

        public int replaceCell(uint cellId, cDatFile fromDat, bool recurse, bool createOnly, cBuildInfo building, List<uint> replacedList, uint parentCellOrLandblock, int verboseLevel = 7)
        {
            int cellsReplacedCounter = 0;
            int cellsNotFoundCounter = 0;
            //List<uint> replacedList = new List<uint>();

            replaceCellRecursive(cellId, fromDat, recurse, createOnly, ref cellsReplacedCounter, ref cellsNotFoundCounter, replacedList, building, parentCellOrLandblock, verboseLevel);

            if (verboseLevel > 6)
                Console.WriteLine("{0} cell(s) copied, {1} cell(s) not found.", cellsReplacedCounter, cellsNotFoundCounter);

            return cellsReplacedCounter;
        }

        public Dictionary<uint, List<uint>> CellsNotFound = new Dictionary<uint, List<uint>>();
        public void replaceCellRecursive(uint cellId, cDatFile fromDat, bool recurse, bool createOnly, ref int cellsReplacedCounter, ref int cellsNotFoundCounter, List<uint> copiedList, cBuildInfo building, uint parentCellOrLandblock, int verboseLevel = 7)
        {
            if (copiedList.Contains(cellId))
                return;

            if (createOnly && fileCache.ContainsKey(cellId))
                return;

            cDatFileEntry file;
            if (fromDat.fileCache.TryGetValue(cellId, out file))
            {
                StreamReader reader = new StreamReader(file.fileContent);
                cEnvCell eEnvCell = new cEnvCell(new byte[1024], reader, file.fileFormat);

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
                    foreach (ushort connectedCell in eEnvCell.Cells)
                    {
                        uint connectedCellId = connectedCell | (cellId & 0xFFFF0000);

                        if (copiedList.Contains(connectedCellId))
                            continue;
                        else
                            replaceCellRecursive(connectedCellId, fromDat, recurse, createOnly, ref cellsReplacedCounter, ref cellsNotFoundCounter, copiedList, building, cellId, verboseLevel);
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
                    StreamReader reader = new StreamReader(file.fileContent);
                    cEnvCell eEnvCell = new cEnvCell(new byte[1024], reader, file.fileFormat);

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
                byte[] buffer = new byte[1024];
                StreamReader reader = new StreamReader(file.fileContent);
                cLandblockInfo landblockInfo = new cLandblockInfo(buffer, reader, file.fileFormat);

                foreach (var building in landblockInfo.Buildings)
                {
                    if (building.Portals.Count > 0 && building.Portals[0].visibleCells.Count > 0)
                    {
                        uint startCellId = building.Portals[0].visibleCells[0] | (landblockId & 0xFFFF0000);
                        removeCell(startCellId, true, verboseLevel);
                    }
                }
                landblockInfo.Buildings.Clear();
                landblockInfo.updateFileContent(file);
            }
        }

        public bool removeBuilding(uint idToCellInBuilding, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine("Removing Building...");

            uint landblockId = idToCellInBuilding & 0xFFFF0000 | 0x0000FFFE;
            uint cellId = idToCellInBuilding & 0x0000FFFF;

            cDatFileEntry file;
            if (fileCache.TryGetValue(landblockId, out file))
            {
                byte[] buffer = new byte[1024];
                StreamReader reader = new StreamReader(file.fileContent);
                cLandblockInfo landblockInfo = new cLandblockInfo(buffer, reader, file.fileFormat);

                for (int i = 0; i < landblockInfo.Buildings.Count; i++)
                {
                    cBuildInfo building = landblockInfo.Buildings[i];
                    foreach (cCBldPortal portal in building.Portals)
                    {
                        if (portal.visibleCells.Contains((ushort)cellId))
                        {
                            landblockInfo.Buildings.RemoveAt(i);

                            uint startCellId = cellId | (landblockId & 0xFFFF0000);
                            removeCell(startCellId, true, verboseLevel);

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

        public void removeBuildings(List<uint> listOfBuildingsIds, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine($"Removing {listOfBuildingsIds.Count} Buildings...");

            Stopwatch timer = new Stopwatch();
            timer.Start();

            int buildingsRemovedCounter = 0;
            int buildingsNotFoundCounter = 0;
            foreach (uint buildingId in listOfBuildingsIds)
            {
                if (removeBuilding(buildingId, verboseLevel))
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
            replaceLandblock(dungeonLandblock, fromDat, false, verboseLevel);
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

        public void replaceLandblockList(List<uint> listOfLandblockIds, cDatFile fromDat, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine($"Replacing {listOfLandblockIds.Count} landblocks...");
            foreach (var landblockId in listOfLandblockIds)
            {
                replaceLandblock(landblockId, fromDat, false, verboseLevel);
            }
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
                    cellsReplaced += replaceCell(cellId, fromDat, true, true, null, replacedList, listOfCellIds.Key, verboseLevel);
                }
            }

            List<uint> emptyKeys = new List<uint>();
            int replacedCount = 0;
            bool found = false;
            foreach (var replacedEntry in replacedList)
            {
                foreach (var entry in CellsNotFound)
                {
                    if(entry.Value.Remove(replacedEntry))
                        found = true;

                    if (entry.Value.Count == 0)
                        emptyKeys.Add(entry.Key);
                }
                if(found)
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
                    if (replaceLandblock(landblockId, fromDat, true, verboseLevel))
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
                            byte[] buffer = new byte[1024];
                            StreamReader reader = new StreamReader(file.fileContent);
                            cEnvCell parentCell = new cEnvCell(buffer, reader, file.fileFormat);
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

        public void clearAllLandblocks(int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine("Clearing all landblocks...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landblocksCounter = 0;

            List<uint> landblockIds = new List<uint>();
            foreach(var file in fileCache)
            {
                if ((file.Key & 0x0000FFFF) == 0x0000FFFF)
                    landblockIds.Add(file.Key);
            }

            foreach(var landblockId in landblockIds)
            {
                if (clearLandblock(landblockId, verboseLevel))
                    landblocksCounter++;
            }

            //for (uint landblockId = 0x00000000; landblockId < 0xFFFF0000; landblockId += 0x00010000)
            //{
            //    if (clearLandblock(landblockId, verboseLevel))
            //        landblocksCounter++;
            //}

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("{0} landblocks cleared in {1} seconds.", landblocksCounter, timer.ElapsedMilliseconds / 1000f);
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

            //List<uint> landblockIds = new List<uint>();
            //foreach (var file in fileCache)
            //{
            //    if ((file.Key & 0x0000FFFF) == 0x0000FFFF)
            //        landblockIds.Add(file.Key);
            //}

            //foreach (var landblockId in landblockIds)
            //{
            //    if (convertLandblock(landblockId, verboseLevel))
            //        landblocksCounter++;
            //}

            for (uint landblockId = 0x00000000; landblockId < 0xFFFF0000; landblockId += 0x00010000)
            {
                if (convertLandblock(landblockId, verboseLevel))
                    landblocksCounter++;
            }

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("{0} landblocks converted in {1} seconds.", landblocksCounter, timer.ElapsedMilliseconds / 1000f);
        }

        public void replaceAllLandblocks(cDatFile fromDat, bool createOnly = false, int verboseLevel = 5)
        {
            if (verboseLevel > 1)
                Console.WriteLine("Replacing all landblocks...");
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
                if (replaceLandblock(landblockId, fromDat, createOnly, verboseLevel))
                    landblocksCounter++;
            }

            //for (uint landblockId = 0x00000000; landblockId < 0xFFFF0000; landblockId+= 0x00010000)
            //{
            //    if (replaceLandblock(landblockId, fromDat, createOnly, verboseLevel))
            //        landblocksCounter++;
            //}

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("{0} landblocks replaced in {1} seconds.", landblocksCounter, timer.ElapsedMilliseconds / 1000f);
        }

        public void replaceLandblockArea(uint baseLandblock, cDatFile fromDat, int verboseLevel = 5)
        {
            uint startLandblockId = baseLandblock & 0xFF000000 | 0x0000FFFF;
            int gridSize = 3;

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
                    if (replaceLandblock(landblockId, fromDat, false, verboseLevel))
                        landblocksAddedCounter++;
                    else
                        landblocksNotFoundCounter++;
                }
            }

            timer.Stop();
            if (verboseLevel > 1)
                Console.WriteLine("{0} landblocks replaced in {1} seconds. {2} landblocks unchanged", landblocksAddedCounter, timer.ElapsedMilliseconds / 1000f, landblocksNotFoundCounter);
        }
    }
}