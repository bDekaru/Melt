using Melt;
using System.IO;
using System.Numerics;

namespace ACE.DatLoader.Entity
{
    public class Plane : IUnpackable, IPackable
    {
        public Vector3 N { get; private set; }
        public float D { get; private set; }

        public void Unpack(BinaryReader reader, bool isToD = true)
        {
            N = reader.ReadVector3();
            D = reader.ReadSingle();
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeVector3(N, output);
            Utils.writeSingle(D, output);
        }

        public System.Numerics.Plane ToNumerics()
        {
            return new System.Numerics.Plane(N, D);
        }
    }
}
