using System;
using System.IO;
using System.Collections.Generic;
using Melt;

namespace ACE.DatLoader.Entity
{
    public enum UiHashType
    {
        Bool,
        StringInfo,
        Enum,
        Data,
        Array,
    }

    public class UiHashProperty : IUnpackable, IPackable
    {
        public uint Key;
        public bool ValueBool;
        public List<uint> ValueStringInfo;
        public uint ValueEnum;
        public UiHashType HashType;
        public uint ValueData;
        public Dictionary<uint, uint> ValueArray;
        public uint ValueArrayCount;

        public void Unpack(BinaryReader reader)
        {
            Key = reader.ReadUInt32();
            switch (Key)
            {
                case 26:
                    HashType = UiHashType.Array;
                    break;
                case 23:
                case 73:
                    HashType = UiHashType.StringInfo;
                    break;
                case 33:
                case 51:
                case 59:
                case 75:
                    HashType = UiHashType.Bool;
                    break;
                case 71:
                    HashType = UiHashType.Enum;
                    break;
                case 72:
                    HashType = UiHashType.Data;
                    break;
                default:
                    throw new NotImplementedException();
            }
            switch (HashType)
            {
                case UiHashType.Bool:
                    ValueBool = reader.ReadBoolean();
                    break;
                case UiHashType.Enum:
                    ValueEnum = reader.ReadUInt32();
                    break;
                case UiHashType.Data:
                    ValueData = reader.ReadUInt32();
                    break;
                case UiHashType.StringInfo:
                    ValueStringInfo = new List<uint>();
                    ValueStringInfo.Unpack(reader, 3);
                    break;
                case UiHashType.Array:
                    ValueArrayCount = reader.ReadUInt32();
                    ValueArray = new Dictionary<uint, uint>();
                    for(int i = 0; i < ValueArrayCount; i++)
                    {
                        ValueArray.Add(reader.ReadUInt32(), reader.ReadUInt32());
                    }
                    break;
            }
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeUInt32(Key, output);
            switch (HashType)
            {
                case UiHashType.Bool:
                    Utils.writeBool(ValueBool, output);
                    break;
                case UiHashType.Enum:
                    Utils.writeUInt32(ValueEnum, output);
                    break;
                case UiHashType.Data:
                    Utils.writeUInt32(ValueData, output);
                    break;
                case UiHashType.StringInfo:
                    ValueStringInfo.Pack(output, 3);
                    break;
                case UiHashType.Array:
                    ValueArrayCount = (uint)ValueArray.Count;
                    Utils.writeUInt32(ValueArrayCount, output);
                    foreach(var entry in ValueArray)
                    {
                        Utils.writeUInt32(entry.Key, output);
                        Utils.writeUInt32(entry.Value, output);
                    }
                    break;
            }
        }
    }
}