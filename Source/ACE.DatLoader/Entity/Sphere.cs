using Melt;
using System.IO;
using System.Numerics;

namespace ACE.DatLoader.Entity
{
    public class Sphere : IUnpackable, IPackable
    {
        public Vector3 Origin { get; private set; }
        public float Radius { get; private set; }

        public void Unpack(BinaryReader reader, bool isToD = true)
        {
            Origin = reader.ReadVector3();
            Radius = reader.ReadSingle();
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeVector3(Origin, output);
            Utils.writeSingle(Radius, output);
        }

        public static Sphere CreateDummySphere()
        {
            var sphere = new Sphere();
            sphere.Origin = Vector3.Zero;
            return sphere;
        }
    }
}
