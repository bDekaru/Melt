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
    public class cCellPortal
    {
        public ushort Bitfield;
        public ushort EnvironmentId;
        public ushort OtherCellId;
        public ushort OtherPortalId;

        //public bool ExactMatch => (Bitfield & 1) != 0;
        //public bool PortalSide => (Bitfield & 2) == 0;

        public cCellPortal(ushort bitfield, ushort environmentId, ushort otherCellId, ushort otherPortalId)
        {
            Bitfield = bitfield;
            EnvironmentId = environmentId;
            OtherCellId = otherCellId;
            OtherPortalId = otherPortalId;
        }

        public cCellPortal(cDatFileEntry file) : this(new StreamReader(file.fileContent))
        {
        }

        public cCellPortal(StreamReader inputFile)
        {
            Bitfield = Utils.readUInt16(inputFile);
            EnvironmentId = Utils.readUInt16(inputFile);
            OtherCellId = Utils.readUInt16(inputFile);
            OtherPortalId = Utils.readUInt16(inputFile);
        }

        public void writeToDat(StreamWriter outputFile)
        {
            Utils.writeUInt16(Bitfield, outputFile);
            Utils.writeUInt16(EnvironmentId, outputFile);
            Utils.writeUInt16(OtherCellId, outputFile);
            Utils.writeUInt16(OtherPortalId, outputFile);
        }
    }

    public class cEnvCell
    {
        public uint Id;
        public UInt32 Bitfield { get; set; }
        // 0x08000000 surfaces (which contains degrade/quality info to reference the specific 0x06000000 graphics)
        public List<ushort> Textures = new List<ushort>();
        // the 0x0D000000 model of the pre-fab dungeon block
        public ushort EnvironmentId;
        public ushort StructId;
        public sFrame Position;
        public List<cCellPortal> Portals = new List<cCellPortal>();
        public List<ushort> Cells = new List<ushort>();
        public List<cStab> Stabs = new List<cStab>();
        public uint RestrictionObj;

        public bool SeenOutside => (Bitfield & 1) != 0;

        public cEnvCell()
        {
        }

        public cEnvCell(cDatFileEntry file, bool translateTextureIds = true) : this(new StreamReader(file.fileContent), file.fileFormat, translateTextureIds)
        {
            file.fileContent.Seek(0, SeekOrigin.Begin);
        }

        public cEnvCell(StreamReader inputFile, eDatFormat format, bool translateTextureIds = true)
        {
            if (format == eDatFormat.ToD)
            {
                Id = Utils.readUInt32(inputFile);

                Bitfield = Utils.readUInt32(inputFile);

                Utils.readUInt32(inputFile); //repeat id
            }
            else if(format == eDatFormat.retail)
            {
                Bitfield = Utils.readUInt32(inputFile);
                Id = Utils.readUInt32(inputFile);
            }

            byte numSurfaces = Utils.readByte(inputFile);
            byte numPortals = Utils.readByte(inputFile);    // Note that "portal" in this context does not refer to the swirly pink/purple thing, its basically connecting cells
            ushort numCells = Utils.readUInt16(inputFile);  // I believe this is what cells can be seen from this one. So the engine knows what else it needs to load/draw.

            Textures = new List<ushort>();
            // Read what surfaces are used in this cell
            for (uint i = 0; i < numSurfaces; i++)
            {
                ushort texture = Utils.readUInt16(inputFile);

                if (translateTextureIds)
                {
                    if (format == eDatFormat.ToD)
                        Textures.Add(texture);
                    else
                        Textures.Add(validPortalDatEntries.translateTextureId(texture));
                }
                else
                    Textures.Add(texture);
            }

            if (format == eDatFormat.retail)
                Utils.align(inputFile);

            EnvironmentId = Utils.readUInt16(inputFile);

            StructId = Utils.readUInt16(inputFile);

            Position = new sFrame(inputFile);

            Portals = new List<cCellPortal>();
            for (int i = 0; i < numPortals; i++)
            {
                cCellPortal newPortal = new cCellPortal(inputFile);
                Portals.Add(newPortal);
            }

            if(format == eDatFormat.retail)
                Utils.align(inputFile);

            Cells = new List<ushort>();
            for (uint i = 0; i < numCells; i++)
                Cells.Add(Utils.readUInt16(inputFile));

            if (format == eDatFormat.retail)
                Utils.align(inputFile);

            Stabs = new List<cStab>();
            if ((Bitfield & 2) != 0)
            {
                int numStabs = Utils.readInt32(inputFile);

                for (int i = 0; i < numStabs; i++)
                {
                    //cStab Size is 32
                    cStab newStab = new cStab(inputFile);
                    Stabs.Add(newStab);
                }
            }

            if (format == eDatFormat.retail)
                Utils.align(inputFile);

            if ((Bitfield & 8) != 0)
                RestrictionObj = Utils.readUInt32(inputFile);

            if (inputFile.BaseStream.Position != inputFile.BaseStream.Length)
                throw new Exception();
        }

        public void updateFileContent(cDatFileEntry file)
        {
            file.listOfBlocks.Clear();
            file.startBlockOffset = 0;
            file.timeStamp = (uint)DateTimeOffset.Now.ToUnixTimeSeconds();

            var baseStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(baseStream);
            writeToDat(writer);

            file.fileContent = new MemoryStream();
            baseStream.WriteTo(file.fileContent);
            writer.Close();

            file.fileContent.Seek(0, SeekOrigin.Begin);
            file.fileSize = (int)file.fileContent.Length;
            file.fileFormat = eDatFormat.ToD; //we only write int ToD format
        }

        public void writeToDat(StreamWriter outputFile)
        {
            Utils.writeUInt32(Id, outputFile);
            Utils.writeUInt32(Bitfield, outputFile);
            Utils.writeUInt32(Id, outputFile);

            Utils.writeByte((byte)Textures.Count, outputFile);
            Utils.writeByte((byte)Portals.Count, outputFile);
            Utils.writeUInt16((ushort)Cells.Count, outputFile);

            foreach (ushort texture in Textures)
            {
                Utils.writeUInt16(texture, outputFile);
            }

            Utils.writeUInt16(EnvironmentId, outputFile);
            Utils.writeUInt16(StructId, outputFile);
            Position.writeRaw(outputFile);

            foreach (cCellPortal cellPortal in Portals)
            {
                cellPortal.writeToDat(outputFile);
            }

            foreach (ushort light in Cells)
            {
                Utils.writeUInt16(light, outputFile);
            }

            if ((Bitfield & 2) != 0)
            {
                Utils.writeInt32(Stabs.Count, outputFile);
                foreach (cStab stab in Stabs)
                {
                    stab.writeToDat(outputFile);
                }
            }

            if ((Bitfield & 8) != 0)
                Utils.writeUInt32(RestrictionObj, outputFile);
        }
    }
}