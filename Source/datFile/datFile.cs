using BTreeNamespace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Melt
{
    public enum eDatFormat
    {
        invalid,
        retail,
        ToD 
    }

    public partial class cDatFile
    {
        public eDatFormat fileFormat;

        public byte[] acTransactionRecord;

        public uint fileType;
        public int blockSize;
        public uint fileSize;
        public uint dataSet;
        public uint dataSubset;

        public uint freeHead;
        public uint freeTail;
        public uint freeCount;
        public uint rootDirectoryOffset;

        public uint youngLRU;
        public uint oldLRU;
        public uint useLRU;

        public uint masterMapId;

        public uint enginePackVersion;
        public uint gamePackVersion;
        public byte[] versionMajor;
        public uint versionMinor;

        public cDatFileNode rootDirectory;

        public cDatFileBlockCache inputBlockCache;
        public cDatFileBlockCache outputBlockCache;
        public SortedDictionary<uint, cDatFileEntry> fileCache;

        public cBTree bTree;

        public cDatFile()
        {
            fileCache = new SortedDictionary<uint, cDatFileEntry>();
        }

        public void loadFromDat(string filename)
        {
            byte[] buffer = new byte[1024];

            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile.BaseStream.Length < 1024)
            {
                Console.WriteLine("{0} is too small to be a valid dat file", filename);
                return;
            }

            Console.WriteLine("Reading data from {0}...", filename);
            Stopwatch timer = new Stopwatch();
            timer.Start();

            inputFile.BaseStream.Seek(257, SeekOrigin.Begin);
            int format = Utils.ReadInt32(buffer, inputFile);
            if (format == 0x4C50)
            {
                inputFile.BaseStream.Seek(256, SeekOrigin.Begin); //skip acVersionStr which is empty
                acTransactionRecord = Utils.ReadBytes(buffer, inputFile, 64);
                for (int i = 4; i < 64; i++)
                {
                    acTransactionRecord[i] = 0;
                }

                fileType = Utils.ReadUInt32(buffer, inputFile);
                if (fileType == 0x5442)
                {
                    fileFormat = eDatFormat.ToD;
                }
            }
            else
            {
                acTransactionRecord = new byte[64];
                acTransactionRecord[0] = 0x00;
                acTransactionRecord[1] = 0x50;
                acTransactionRecord[2] = 0x4C;
                acTransactionRecord[3] = 0x00;

                inputFile.BaseStream.Seek(300, SeekOrigin.Begin);
                fileType = Utils.ReadUInt32(buffer, inputFile);
                if (fileType == 0x5442)
                    fileFormat = eDatFormat.retail;
            }

            if (fileFormat == eDatFormat.invalid)
            {
                Console.WriteLine("{0} is not a valid dat file.", filename);
                return;
            }

            blockSize = Utils.ReadInt32(buffer, inputFile);
            fileSize = Utils.ReadUInt32(buffer, inputFile);
            dataSet = Utils.ReadUInt32(buffer, inputFile);
            if (fileFormat == eDatFormat.ToD)
                dataSubset = Utils.ReadUInt32(buffer, inputFile);
            else
            {
                //dataSet = 0x00000043 for old cell.dat
                //dataSet = 0x0000082f for old portal.dat
                if (dataSet == 0x00000043)
                {
                    dataSet = 0x00000002;
                    dataSubset = 0x00000001;
                }
                else
                {
                    dataSet = 0x00000001;
                    dataSubset = 0x00000000;
                }
            }
            //dataSet = 0x00000001 - dataSubset = 0x00000000 for client_portal.dat
            //dataSet = 0x00000001 - dataSubset = 0x69466948 for client_highres.dat
            //dataSet = 0x00000002 - dataSubset = 0x00000001 for client_cell_1.dat
            //dataSet = 0x00000003 - dataSubset = 0x00000001 for client_local_English.dat

            freeHead = Utils.ReadUInt32(buffer, inputFile);
            freeTail = Utils.ReadUInt32(buffer, inputFile);
            freeCount = Utils.ReadUInt32(buffer, inputFile);
            rootDirectoryOffset = Utils.ReadUInt32(buffer, inputFile);

            if (fileFormat == eDatFormat.ToD)
            {
                youngLRU = Utils.ReadUInt32(buffer, inputFile);
                oldLRU = Utils.ReadUInt32(buffer, inputFile);
                useLRU = Utils.ReadUInt32(buffer, inputFile);

                masterMapId = Utils.ReadUInt32(buffer, inputFile);

                enginePackVersion = Utils.ReadUInt32(buffer, inputFile);
                gamePackVersion = Utils.ReadUInt32(buffer, inputFile);
                versionMajor = Utils.ReadBytes(buffer, inputFile, 16);//int data1, short data2, short data3, int64 data4
                versionMinor = Utils.ReadUInt32(buffer, inputFile);
            }
            else
            {
                youngLRU = 0x00000000;
                oldLRU = 0x00000000;
                useLRU = 0xcdcdcd00;

                masterMapId = 0x00000000;

                enginePackVersion = 0x00000016;
                gamePackVersion = 0x00000000;
                versionMajor = new byte[] { 0xD2, 0xD7, 0xA7, 0x34, 0x2F, 0x72, 0x46, 0x4C, 0x8A, 0xB4, 0xEF, 0x51, 0x4F, 0x85, 0x6F, 0xFD };

                versionMinor = 0x000000de;
            }

            inputBlockCache = new cDatFileBlockCache(fileSize, blockSize, freeHead, freeTail, freeCount);

            cDatFileBlock.loadBlocksAndAddToDictionary(inputBlockCache, inputFile, blockSize);

            rootDirectory = new cDatFileNode(buffer, inputFile, inputBlockCache, rootDirectoryOffset, blockSize, fileFormat);
            rootDirectory.loadFilesAndAddToCache(fileCache, inputBlockCache, inputFile, blockSize);

            timer.Stop();
            Console.WriteLine("{0} blocks read in {1} seconds.", inputBlockCache.blocks.Count, timer.ElapsedMilliseconds / 1000f);
        }

        //we only write to the ToD data format.
        public void writeToDat(string filename)
        {
            fileSize = inputBlockCache.fileSize;
            freeHead = inputBlockCache.freeHead;
            freeTail = inputBlockCache.freeTail;
            freeCount = inputBlockCache.freeCount;

            byte[] buffer = new byte[1024];

            StreamWriter outputFile = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write));

            Console.WriteLine("Writing data to {0}...", filename);
            Stopwatch timer = new Stopwatch();
            timer.Start();

            ////keep some free blocks
            //if (inputBlockCache.freeCount < 1000)
            //    inputBlockCache.addNewFreeBlocks(1000 - (int)inputBlockCache.freeCount);

            //rootDirectory.updateBlockData(this, inputBlockCache);

            //foreach (KeyValuePair<uint, cDatFileBlock> entry in inputBlockCache.blocks)
            //{
            //    outputFile.BaseStream.Seek(entry.Key, SeekOrigin.Begin);
            //    entry.Value.writeToDat(outputFile, blockSize);
            //}

            //fileSize = inputBlockCache.fileSize;
            //freeHead = inputBlockCache.fileSize;
            //freeTail = inputBlockCache.fileSize;
            //freeCount = inputBlockCache.fileSize;

            bTree = new cBTree(31);

            foreach (KeyValuePair<uint, cDatFileEntry> entry in fileCache)
            {
                bTree.Insert(entry.Key);
            }

            outputBlockCache = new cDatFileBlockCache(blockSize, 1024);
            rootDirectory = buildBtreeStructure(bTree.Root, outputBlockCache);
            rootDirectory.updateBlockData(this, outputBlockCache);

            foreach (KeyValuePair<uint, cDatFileBlock> entry in outputBlockCache.blocks)
            {
                outputFile.BaseStream.Seek(entry.Key, SeekOrigin.Begin);
                entry.Value.writeToDat(outputFile, blockSize);
            }

            if (outputBlockCache.freeCount < 1000)
                outputBlockCache.addNewFreeBlocks(1000 - (int)outputBlockCache.freeCount);

            rootDirectoryOffset = rootDirectory.startBlockOffset;
            fileSize = outputBlockCache.fileSize;
            freeHead = outputBlockCache.fileSize;
            freeTail = outputBlockCache.fileSize;
            freeCount = outputBlockCache.fileSize;

            outputFile.BaseStream.Seek(256, SeekOrigin.Begin); //skip acVersionStr which is empty
            Utils.writeBytes(acTransactionRecord, outputFile);

            Utils.writeUInt32(fileType, outputFile);

            Utils.writeInt32(blockSize, outputFile);
            Utils.writeUInt32(fileSize, outputFile);
            Utils.writeUInt32(dataSet, outputFile);
            Utils.writeUInt32(dataSubset, outputFile);

            Utils.writeUInt32(freeHead, outputFile);
            Utils.writeUInt32(freeTail, outputFile);
            Utils.writeUInt32(freeCount, outputFile);
            Utils.writeUInt32(rootDirectoryOffset, outputFile);

            Utils.writeUInt32(youngLRU, outputFile);
            Utils.writeUInt32(oldLRU, outputFile);
            Utils.writeUInt32(useLRU, outputFile);

            Utils.writeUInt32(masterMapId, outputFile);

            Utils.writeUInt32(enginePackVersion, outputFile);
            Utils.writeUInt32(gamePackVersion, outputFile);
            Utils.writeBytes(versionMajor, outputFile);
            Utils.writeUInt32(versionMinor, outputFile);

            outputFile.Close();
            timer.Stop();
            Console.WriteLine("{0} blocks written in {1} seconds.", outputBlockCache.blocks.Count, timer.ElapsedMilliseconds / 1000f);

            //exportDirTree(rootDirectory);
        }

        public cDatFileNode buildBtreeStructure(Node node, cDatFileBlockCache blockCache)
        {
            cDatFileNode newFolder = new cDatFileNode(this, blockCache);
            foreach (uint entry in node.Entries)
            {
                cDatFileEntry file = fileCache[entry];
                file.startBlockOffset = 0;
                file.listOfBlocks = new List<cDatFileBlock>();
                //blockCache.dataToBlocks(file.listOfBlocks, file.fileContent);
                //file.startBlockOffset = file.listOfBlocks[0].blockOffset;
                newFolder.files.Add(file.fileId, file);
            }

            if (!node.IsLeaf)
            {
                foreach (Node child in node.Children)
                {
                    cDatFileNode newSubFolder = buildBtreeStructure(child, blockCache);
                    newFolder.subFolders.Add(newSubFolder);
                }
            }

            //newFolder.updateBlockData(this, blockCache);

            return newFolder;
        }

        //public cDatFileNode getWorkFolder(cDatFileNode folder)
        //{
        //    if (folder.files.Count < 30)
        //        return folder;

        //    foreach (cDatFileNode subFolder in folder.subFolders)
        //    {
        //        if (subFolder.files.Count < 30)
        //            return subFolder;

        //        foreach (cDatFileNode subFolder2 in subFolder.subFolders)
        //        {
        //            if (subFolder2.files.Count < 30)
        //                return subFolder2;
        //        }
        //    }

        //    if (folder.subFolders.Count <= folder.files.Count)
        //    {
        //        cDatFileNode newFolder = new cDatFileNode(this, inputBlockCache);
        //        folder.subFolders.Add(newFolder);
        //        return newFolder;
        //    }

        //    foreach (cDatFileNode subFolder in folder.subFolders)
        //    {
        //        if (subFolder.subFolders.Count <= subFolder.files.Count)
        //        {
        //            cDatFileNode newFolder = new cDatFileNode(this, inputBlockCache);
        //            subFolder.subFolders.Add(newFolder);
        //            return newFolder;
        //        }
        //    }

        //    throw new ArgumentException();
        //}

        //public cDatFileFolder findValidSpotForNewFile(cDatFileFolder directory)
        //{
        //    for (int i = 0; i < directory.files.Count; i++)
        //    {
        //        if (i < directory.subFolders.Count)
        //        {
        //            if (directory.files.Count < 30)
        //            {
        //                return directory;
        //            }
        //        }
        //    }
        //    return null;
        //}

        //public cDatFileFolder findValidSpotForNewDirectory(cDatFileFolder directory)
        //{
        //    for (int i = 0; i < directory.files.Count + 1; i++)
        //    {
        //        if (i < directory.subFolders.Count)
        //        {
        //            if (directory.files.Count < 30)
        //            {
        //                return directory;
        //            }
        //        }
        //    }
        //    return null;
        //}

        //public cDatFileEntry createNewFile(uint fileId)
        //{
        //    if (inputFileCache.ContainsKey(fileId))
        //        throw new ArgumentException();

        //    cDatFileNode currentWorkDirectory = getWorkFolder(rootDirectory);
        //    cDatFileEntry newFile = new cDatFileEntry(fileId, fileFormat);

        //    currentWorkDirectory.files.Add(newFile.fileId, newFile);

        //    inputFileCache.Add(fileId, newFile);

        //    return newFile;
        //}
    }
}