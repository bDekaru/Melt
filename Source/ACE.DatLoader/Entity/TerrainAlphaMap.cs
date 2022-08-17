using System.IO;

namespace ACE.DatLoader.Entity
{
    public class TerrainAlphaMap : IUnpackable
    {
        public uint TCode { get; private set; }
        public uint TexGID { get; private set; }

        public void Unpack(BinaryReader reader, bool isToD = true)
        {
            TCode   = reader.ReadUInt32();
            TexGID  = reader.ReadUInt32();
        }
    }
}
