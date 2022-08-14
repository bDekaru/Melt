using System;
using System.IO;
using System.Collections.Generic;
using Melt;

namespace ACE.DatLoader.Entity
{
    public class UiMedia : IUnpackable, IPackable
    {
        public uint Type;
        public UiMediaType Data;

        public void Unpack(BinaryReader reader)
        {
            Type = reader.ReadUInt32();

            switch(Type)
            {
                case 0x00000005:
                    Data = new UiMediaImage();
                    Data.Unpack(reader);
                    break;
                case 0x00000009:
                    Data = new UiMediaSound();
                    Data.Unpack(reader);
                    break;
                case 0x00000001:
                    Data = new UiMediaVideo();
                    Data.Unpack(reader);
                    break;
                case 0x00000007:
                    Data = new UiMediaMessage();
                    Data.Unpack(reader);
                    break;
                //default:
                //    throw new NotImplementedException();
            }
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeUInt32(Type, output);

            switch (Type)
            {
                case 0x00000005:
                    Data.Pack(output);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public interface UiMediaType
    {
        void Unpack(BinaryReader reader);
        void Pack(StreamWriter output);
    }

    public class UiMediaImage : UiMediaType
    {
        public uint Type;
        public uint File;
        public uint DrawMode;

        public void Unpack(BinaryReader reader)
        {
            Type = reader.ReadUInt32();
            File = reader.ReadUInt32();
            DrawMode = reader.ReadUInt32();
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeUInt32(Type, output);
            Utils.writeUInt32(File, output);
            Utils.writeUInt32(DrawMode, output);
        }
    }

    public class UiMediaSound : UiMediaType
    {
        public uint Type;
        public uint File;
        public uint SType;

        public void Unpack(BinaryReader reader)
        {
            Type = reader.ReadUInt32();
            File = reader.ReadUInt32();
            SType = reader.ReadUInt32();
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeUInt32(Type, output);
            Utils.writeUInt32(File, output);
            Utils.writeUInt32(SType, output);
        }
    }

    public class UiMediaVideo : UiMediaType
    {
        public uint Type;
        public string Filename;
        public bool Stretch;

        public void Unpack(BinaryReader reader)
        {
            Type = reader.ReadUInt32();
            Filename = reader.ReadString();
            Stretch = reader.ReadBoolean();
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeUInt32(Type, output);
            Utils.writeString(Filename, output);
            Utils.writeBool(Stretch, output);
        }
    }

    public class UiMediaMessage : UiMediaType
    {
        public uint Type;
        public uint MessageId;
        public float Probability;

        public void Unpack(BinaryReader reader)
        {
            Type = reader.ReadUInt32();
            MessageId = reader.ReadUInt32();
            Probability = reader.ReadSingle();
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeUInt32(Type, output);
            Utils.writeUInt32(MessageId, output);
            Utils.writeSingle(Probability, output);
        }
    }
}