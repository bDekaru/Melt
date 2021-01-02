using Newtonsoft.Json;
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
    public class cDatFileEntry //BTEntry
    {
        public eDatFormat fileFormat;
        public uint bitFlags;
        public uint fileId;
        public int fileSize;
        public uint startBlockOffset;
        public uint timeStamp;
        public uint version;

        public MemoryStream fileContent;
        public List<cDatFileBlock> listOfBlocks;

        //public static cDatFileEntry newFrom(cDatFileEntry other)
        //{
        //    return newFrom(other, other.fileId);
        //}

        public static cDatFileEntry newFrom(cDatFileEntry other, uint newFileId)
        {
            cDatFileEntry newFile = other.Copy();
            newFile.fileId = newFileId;
            return newFile;
        }

        public void copyFrom(cDatFileEntry other)
        {
            copyFrom(other, other.fileId);
        }

        public void copyFrom(cDatFileEntry other, uint newFileId)
        {
            fileFormat = other.fileFormat;
            bitFlags = other.bitFlags;
            fileSize = other.fileSize;
            startBlockOffset = other.startBlockOffset;
            timeStamp = other.timeStamp;
            version = other.version;

            fileId = newFileId;
            fileContent = other.fileContent;
            listOfBlocks = new List<cDatFileBlock>();
        }

        public cDatFileEntry(uint fileId, eDatFormat fileFormat)
        {
            this.fileId = fileId;
            this.fileFormat = fileFormat;

            startBlockOffset = 0;
            fileSize = 0;
            bitFlags = 0x00020000;
            timeStamp = (uint)DateTimeOffset.Now.ToUnixTimeSeconds();
            version = 1;

            fileContent = new MemoryStream();
            listOfBlocks = new List<cDatFileBlock>();
        }

        public cDatFileEntry(StreamReader inputFile, eDatFormat fileFormat)
        {
            this.fileFormat = fileFormat;

            if (fileFormat == eDatFormat.ToD)
            {
                bitFlags = Utils.readUInt32(inputFile);//0x10000 = empty 0x20000 = has data
                fileId = Utils.readUInt32(inputFile);
                startBlockOffset = Utils.readUInt32(inputFile);
                fileSize = Utils.readInt32(inputFile);
                timeStamp = Utils.readUInt32(inputFile);
                version = Utils.readUInt32(inputFile);

                //DateTimeOffset convertedTimeStamp = DateTimeOffset.FromUnixTimeSeconds(timeStamp);
            }
            else
            {
                fileId = Utils.readUInt32(inputFile);
                startBlockOffset = Utils.readUInt32(inputFile);
                fileSize = Utils.readInt32(inputFile);
                bitFlags = 0x00020000;
                timeStamp = (uint)DateTimeOffset.Now.ToUnixTimeSeconds();
                version = 1;
            }

            fileContent = new MemoryStream();
        }

        public void writeHeader(StreamWriter outputFile)
        {
            Utils.writeUInt32(bitFlags, outputFile);
            Utils.writeUInt32(fileId, outputFile);
            Utils.writeUInt32(startBlockOffset, outputFile);
            Utils.writeInt32(fileSize, outputFile);
            Utils.writeUInt32(timeStamp, outputFile);
            Utils.writeUInt32(version, outputFile);
        }
    }
}