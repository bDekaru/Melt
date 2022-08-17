using System;
using System.IO;
using System.Collections.Generic;
using Melt;

namespace ACE.DatLoader.Entity
{
    public class StringTableData : IUnpackable
    {
        public uint Id { get; private set; }
        public List<string> VarNames { get; } = new List<string>();
        public List<string> Vars { get; } = new List<string>();
        public List<string> Strings { get; } = new List<string>();
        public List<uint> Comments { get; } = new List<uint>();

        public byte Unknown { get; private set; }

        public void Unpack(BinaryReader reader, bool isToD = true)
        {
            Id = reader.ReadUInt32();

            var num_varnames = reader.ReadUInt16();
            for (uint i = 0; i < num_varnames; i++)
                VarNames.Add(reader.ReadUnicodeString());

            var num_vars = reader.ReadUInt16();
            for (uint i = 0; i < num_vars; i++)
                Vars.Add(reader.ReadUnicodeString());

            var num_strings = reader.ReadUInt32();
            for (uint i = 0; i < num_strings; i++)
                Strings.Add(reader.ReadUnicodeString());

            var num_comments = reader.ReadUInt32();
            for (uint i = 0; i < num_comments; i++)
                Comments.Add(reader.ReadUInt32());

            Unknown = reader.ReadByte();
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeUInt32(Id, output);

            Utils.writeUInt16((ushort)VarNames.Count, output);
            foreach (var entry in VarNames)
                Utils.writeUnicodeString(entry, output);

            Utils.writeUInt16((ushort)Vars.Count, output);
            foreach (var entry in Vars)
                Utils.writeUnicodeString(entry, output);

            Utils.writeUInt32((uint)Strings.Count, output);
            foreach (var entry in Strings)
                Utils.writeUnicodeString(entry, output);

            Utils.writeUInt32((uint)Comments.Count, output);
            foreach (var entry in Comments)
                Utils.writeUInt32(entry, output);

            Utils.writeByte(Unknown, output);
        }
    }
}
