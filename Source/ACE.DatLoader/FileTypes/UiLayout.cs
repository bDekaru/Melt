using System.Collections.Generic;
using System.IO;

using ACE.DatLoader.Entity;
using Melt;

namespace ACE.DatLoader.FileTypes
{
    [DatFileType(DatFileType.UiLayout)]
    public class UiLayout : FileType
    {
        public int DisplayWidth;
        public int DisplayHeight;
        public byte ElementsBucketSize;
        public Dictionary<uint, UiElement> Elements;

        public override void Unpack(BinaryReader reader, bool isToD = true)
        {
            Id = reader.ReadUInt32();
            DisplayWidth = reader.ReadInt32();
            DisplayHeight = reader.ReadInt32();

            Elements = new Dictionary<uint, UiElement>();
            ElementsBucketSize = Elements.UnpackBytePackedHashTable(reader);
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeUInt32(Id, output);
            Utils.writeInt32(DisplayWidth, output);
            Utils.writeInt32(DisplayHeight, output);
            Elements.PackBytePackedHashTable(ElementsBucketSize, output);
        }
    }
}
