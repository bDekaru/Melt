using Melt;
using System.IO;

namespace ACE.DatLoader.Entity
{
    public class ScriptAndModData : IUnpackable, IPackable
    {
        public float Mod { get; set; }
        public uint ScriptId { get; set; }

        public void Unpack(BinaryReader reader, bool isToD = true)
        {
            Mod         = reader.ReadSingle();
            ScriptId    = reader.ReadUInt32();
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeSingle(Mod, output);
            Utils.writeUInt32(ScriptId, output);
        }
    }
}
