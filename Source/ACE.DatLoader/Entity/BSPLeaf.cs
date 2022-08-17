using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ACE.Entity.Enum;
using Melt;

namespace ACE.DatLoader.Entity
{
    public class BSPLeaf : BSPNode
    {
        public int LeafIndex { get; private set; }
        public int Solid { get; private set; }

        /// <summary>
        /// You must use the Unpack(BinaryReader reader, BSPType treeType) method.
        /// </summary>
        /// <exception cref="NotSupportedException">You must use the Unpack(BinaryReader reader, BSPType treeType) method.</exception>
        public override void Unpack(BinaryReader reader, bool isToD = true)
        {
            throw new NotSupportedException();
        }

        public override void Unpack(BinaryReader reader, BSPType treeType, bool isToD = true)
        {
            Type = Encoding.ASCII.GetString(reader.ReadBytes(4)).Reverse();

            LeafIndex = reader.ReadInt32();

            if (treeType == BSPType.Physics)
            {
                Solid = reader.ReadInt32();

                // Note that if Solid is equal to 0, these values will basically be null. Still read them, but they don't mean anything.
                Sphere = new Sphere();
                Sphere.Unpack(reader);

                InPolys = new List<ushort>();
                uint numPolys = reader.ReadUInt32();
                for (uint i = 0; i < numPolys; i++)
                    InPolys.Add(reader.ReadUInt16());
            }

            if (!isToD)
                reader.AlignBoundary();
        }

        public override void Pack(StreamWriter output, BSPType treeType)
        {
            for (int i = 3; i >= 0; i--)
            {
                Utils.writeByte((byte)Type[i], output);
            }

            Utils.writeInt32(LeafIndex, output);

            if (treeType == BSPType.Physics)
            {
                Utils.writeInt32(Solid, output);

                Sphere.Pack(output);

                Utils.writeUInt32((uint)InPolys.Count, output);
                foreach(var entry in InPolys)
                {
                    Utils.writeUInt16(entry, output);
                }
            }
        }
    }
}
