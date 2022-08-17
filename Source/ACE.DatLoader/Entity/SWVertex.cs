using ACE.Entity.Enum;
using Melt;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace ACE.DatLoader.Entity
{
    /// <summary>
    /// A vertex position, normal, and texture coords
    /// </summary>
    public class SWVertex : IUnpackable, IPackable
    {
        public Vector3 Origin { get; private set; }
        public Vector3 Normal { get; private set; }

        public List<Vec2Duv> UVs { get; private set; }

        public void Unpack(BinaryReader reader, bool isToD = true)
        {
            var numUVs = reader.ReadUInt16();
            UVs = new List<Vec2Duv>(numUVs);

            Origin = reader.ReadVector3();
            Normal = reader.ReadVector3();

            UVs.Unpack(reader, numUVs);

            if (!isToD)
                reader.AlignBoundary();
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeUInt16((ushort)UVs.Count, output);

            Utils.writeVector3(Origin, output);
            Utils.writeVector3(Normal, output);

            UVs.Pack(output);
        }
    }
}
