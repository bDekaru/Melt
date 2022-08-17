using System;
using System.IO;
using System.Collections.Generic;
using Melt;

namespace ACE.DatLoader.Entity
{
    public class UiState : IUnpackable, IPackable
    {
        public uint StateId;
        public byte PassToChildren;
        public uint UiIncorporationFlags;
        public byte HashPropertiesBucketSize;
        public Dictionary<uint, UiHashProperty> HashProperties;
        public List<UiMedia> Media;

        public void Unpack(BinaryReader reader, bool isToD = true)
        {
            StateId = reader.ReadUInt32();
            PassToChildren = reader.ReadByte();
            UiIncorporationFlags = reader.ReadUInt32();

            HashProperties = new Dictionary<uint, UiHashProperty>();
            HashPropertiesBucketSize = HashProperties.UnpackBytePackedHashTable(reader);
            if (StateId == 0x00004b00)
            {
                Media = new List<UiMedia>();
                Media.Unpack2(reader);
            }
            else
            {
                Media = new List<UiMedia>();
                Media.UnpackSmartArray(reader);
            }
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeUInt32(StateId, output);
            Utils.writeByte(PassToChildren, output);
            Utils.writeUInt32(UiIncorporationFlags, output);

            HashProperties.PackBytePackedHashTable(HashPropertiesBucketSize, output);

            Media.PackSmartArray(output);
        }
    }
}