using System.IO;

namespace ACE.DatLoader.Entity
{
    public class ScriptAndModData : IUnpackable
    {
        public float Mod { get; private set; }
        public uint ScriptId { get; private set; }

        public void Unpack(BinaryReader reader, bool isToD = true)
        {
            Mod         = reader.ReadSingle();
            ScriptId    = reader.ReadUInt32();
        }
    }
}
