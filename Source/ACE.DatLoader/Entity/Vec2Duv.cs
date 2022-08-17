using Melt;
using System.IO;

namespace ACE.DatLoader.Entity
{
    /// <summary>
    /// Info on texture UV mapping
    /// </summary>
    public class Vec2Duv : IUnpackable, IPackable
    {
        public float U { get; private set; }
        public float V { get; private set; }

        public void Unpack(BinaryReader reader, bool isToD = true)
        {
            U = reader.ReadSingle();
            V = reader.ReadSingle();
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeSingle(U, output);
            Utils.writeSingle(V, output);
        }
    }
}
