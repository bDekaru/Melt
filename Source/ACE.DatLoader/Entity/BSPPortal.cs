using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ACE.Entity.Enum;
using Melt;

namespace ACE.DatLoader.Entity
{
    public class BSPPortal : BSPNode
    {
        public List<PortalPoly> InPortals { get; } = new List<PortalPoly>();

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

            SplittingPlane = new Plane();
            SplittingPlane.Unpack(reader);

            PosNode = BSPNode.ReadNode(reader, treeType);
            NegNode = BSPNode.ReadNode(reader, treeType);

            if (treeType == BSPType.Drawing)
            {
                Sphere = new Sphere();
                Sphere.Unpack(reader);

                var numPolys = reader.ReadUInt32();
                var numPortals = reader.ReadUInt32();

                InPolys = new List<ushort>();
                for (uint i = 0; i < numPolys; i++)
                    InPolys.Add(reader.ReadUInt16());

                InPortals.Unpack(reader, numPortals);
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

            SplittingPlane.Pack(output);

            PosNode.Pack(output, treeType);
            NegNode.Pack(output, treeType);

            if (treeType == BSPType.Drawing)
            {
                Sphere.Pack(output);

                Utils.writeUInt32((uint)InPolys.Count, output);
                Utils.writeUInt32((uint)InPortals.Count, output);

                foreach (var entry in InPolys)
                {
                    Utils.writeUInt16(entry, output);
                }

                InPortals.Pack(output);
            }
        }
    }
}
