﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melt
{
    public class cCellDat
    {
        int landSize = 2045;

        public Dictionary<int, Dictionary<int, cCellLandblock>> surfaceLandblocks;
        public Dictionary<int, Dictionary<int, cLandblockInfo>> landblockInfo;

        public cCellDat()
        {
            surfaceLandblocks = new Dictionary<int, Dictionary<int, cCellLandblock>>(landSize);
            for (int x = 0; x < landSize; x++)
            {
                surfaceLandblocks[x] = new Dictionary<int, cCellLandblock>(landSize);
            }

            landblockInfo = new Dictionary<int, Dictionary<int, cLandblockInfo>>(landSize);
            for (int x = 0; x < landSize; x++)
            {
                landblockInfo[x] = new Dictionary<int, cLandblockInfo>(landSize);
            }
        }

        public int countDirs(cDatFileNode directory)
        {
            int count = 1;
            foreach (cDatFileNode subDirectory in directory.subFolders)
            {
                count += countDirs(subDirectory);
            }
            return count;
        }

        public void loadFromDat(cDatFile datFile)
        {
            Console.WriteLine("Reading map from dat file...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landBlockCounter = 0;
            int landBlockInfoCounter = 0;
            foreach (KeyValuePair<uint, cDatFileEntry> entry in datFile.fileCache)
            {
                if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF)
                {
                    uint x = (uint)entry.Value.fileId >> 24;
                    uint y = (uint)(entry.Value.fileId & 0x00FF0000) >> 16;
                    cCellLandblock newLandblock = new cCellLandblock(entry.Value);
                    surfaceLandblocks[(int)x].Add((int)y, newLandblock);
                    landBlockCounter++;
                }
                else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE)
                {
                    uint x = (uint)entry.Value.fileId >> 24;
                    uint y = (uint)(entry.Value.fileId & 0x00FF0000) >> 16;
                    cLandblockInfo newLandblockInfo = new cLandblockInfo(entry.Value);
                    landblockInfo[(int)x].Add((int)y, newLandblockInfo);
                    landBlockInfoCounter++;
                }
            }

            timer.Stop();
            Console.WriteLine("{0} landblocks and {1} landblockInfos read in {2} seconds.", landBlockCounter, landBlockInfoCounter, timer.ElapsedMilliseconds / 1000f);
        }
    }
}