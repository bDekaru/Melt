using System.Collections.Generic;
using System.IO;

namespace ACE.DatLoader.Entity
{
    public class SceneType : IUnpackable
    {
        public uint StbIndex { get; private set; }
        public List<uint> Scenes { get; } = new List<uint>();

        public void Unpack(BinaryReader reader, bool isToD = true)
        {
            StbIndex = reader.ReadUInt32();

            Scenes.Unpack(reader);
        }
    }
}
