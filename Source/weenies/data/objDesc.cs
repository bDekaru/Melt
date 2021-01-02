using System;
using System.Collections.Generic;
using System.IO;

namespace Melt
{
    public struct sSubPalette
    {
        public uint subPaletteId;
        public byte offset;
        public byte length;

        public sSubPalette(StreamReader inputFile)
        {
            subPaletteId = Utils.readPackedUInt32(inputFile, 0x4000000);
            offset = Utils.readByte(inputFile);
            length = Utils.readByte(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writePackedUInt32(subPaletteId, outputStream, 0x4000000);
            Utils.writeByte(offset, outputStream);
            Utils.writeByte(length, outputStream);
        }
    }

    public struct sPalette
    {
        public uint basePaletteId;
        public List<sSubPalette> subPalettes;

        public sPalette(StreamReader inputFile, byte subPaletteCount)
        {
            if (subPaletteCount > 0)
            {
                basePaletteId = Utils.readPackedUInt32(inputFile, 0x4000000);
            }
            else
                basePaletteId = 0;

            subPalettes = new List<sSubPalette>(subPaletteCount);
            for (byte i = 0; i < subPaletteCount; i++)
            {
                subPalettes.Add(new sSubPalette(inputFile));
            }
        }

        public void writeRaw(StreamWriter outputStream)
        {
            if (subPalettes.Count > 0)
            {
                Utils.writePackedUInt32(basePaletteId, outputStream, 0x4000000);

                foreach (sSubPalette entry in subPalettes)
                {
                    entry.writeRaw(outputStream);
                }
            }
        }
    }

    public struct sTextureMaps
    {
        public byte index;
        public uint oldTexture;
        public uint newTexture;

        public sTextureMaps(StreamReader inputFile)
        {
            index = Utils.readByte(inputFile);
            oldTexture = Utils.readPackedUInt32(inputFile, 0x5000000);
            newTexture = Utils.readPackedUInt32(inputFile, 0x5000000);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeByte(index, outputStream);
            Utils.writePackedUInt32(oldTexture, outputStream, 0x5000000);
            Utils.writePackedUInt32(newTexture, outputStream, 0x5000000);
        }
    }

    public struct sModels
    {
        public byte index;
        public uint modelId;

        public sModels(StreamReader inputFile)
        {
            index = Utils.readByte(inputFile);
            modelId = Utils.readPackedUInt32(inputFile, 0x1000000);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeByte(index, outputStream);
            Utils.writePackedUInt32(modelId, outputStream, 0x1000000);
        }
    }

    public struct sObjDesc
    {
        public sPalette palette;
        public List<sTextureMaps> textureMaps;
        public List<sModels> models;

        public sObjDesc(StreamReader inputFile)
        {
            byte sectionDelimiter = Utils.readByte(inputFile);

            if (sectionDelimiter != 0x11)
                Console.WriteLine("Error reading weenie at {0}", inputFile.BaseStream.Position);

            byte subPaletteCount = Utils.readByte(inputFile);
            byte textureMapCount = Utils.readByte(inputFile);
            byte modelsCount = Utils.readByte(inputFile);

            palette = new sPalette(inputFile, subPaletteCount);

            textureMaps = new List<sTextureMaps>(textureMapCount);
            for (byte i = 0; i < textureMapCount; i++)
            {
                textureMaps.Add(new sTextureMaps(inputFile));
            }

            models = new List<sModels>(modelsCount);
            for (byte i = 0; i < modelsCount; i++)
            {
                models.Add(new sModels(inputFile));
            }

            if(subPaletteCount + textureMapCount + modelsCount > 0)
                Utils.align(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeByte(0x11, outputStream);
            Utils.writeByte((byte)palette.subPalettes.Count, outputStream);
            Utils.writeByte((byte)textureMaps.Count, outputStream);
            Utils.writeByte((byte)models.Count, outputStream);

            palette.writeRaw(outputStream);

            foreach(sTextureMaps entry in textureMaps)
            {
                entry.writeRaw(outputStream);
            }

            foreach (sModels entry in models)
            {
                entry.writeRaw(outputStream);
            }

            if (palette.subPalettes.Count + textureMaps.Count + models.Count > 0)
                Utils.align(outputStream);
        }
    }
}