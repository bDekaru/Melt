using System.Collections.Generic;
using System.IO;

using ACE.DatLoader.Entity;
using Melt;

namespace ACE.DatLoader.FileTypes
{
    [DatFileType(DatFileType.StringTable)]
    public class StringTable : FileType
    {
        public static uint CharacterTitle_FileID = 0x2300000E;

        public uint Language { get; private set; } // This should always be 1 for English

        public byte Unknown { get; private set; }

        public List<StringTableData> StringTableData { get; } = new List<StringTableData>();

        public override void Unpack(BinaryReader reader, bool isToD = true)
        {
            Id = reader.ReadUInt32();

            Language = reader.ReadUInt32();

            Unknown = reader.ReadByte();

            StringTableData.UnpackSmartArray(reader);
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeUInt32(Id, output);
            Utils.writeUInt32(Language, output);
            Utils.writeByte(Unknown, output);

            Utils.writeCompressedUInt32((uint)StringTableData.Count, output);
            foreach (var entry in StringTableData)
                entry.Pack(output);
        }
    }
}
