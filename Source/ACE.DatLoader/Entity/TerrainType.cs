using System.Collections.Generic;
using System.IO;

namespace ACE.DatLoader.Entity
{
    public class TerrainType : IUnpackable
    {
        public string TerrainName { get; private set; }
        public uint TerrainColor { get; private set; }
        public List<uint> SceneTypes { get; } = new List<uint>();

        public void Unpack(BinaryReader reader, bool isToD = true)
        {
            TerrainName = reader.ReadPString();
            reader.AlignBoundary();

            TerrainColor = reader.ReadUInt32();

            SceneTypes.Unpack(reader);
        }
    }
}
