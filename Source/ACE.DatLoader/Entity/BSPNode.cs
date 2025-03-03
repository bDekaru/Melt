using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ACE.Entity.Enum;
using Melt;

namespace ACE.DatLoader.Entity
{
    public class BSPNode : IUnpackable
    {
        // These constants are actually strings in the dat file
        private const uint PORT = 1347375700; // 0x504F5254
        private const uint LEAF = 1279607110; // 0x4C454146
        private const uint BPnn = 1112567406; // 0x42506E6E
        private const uint BPIn = 1112557934; // 0x4250496E
        private const uint BpIN = 1114655054; // 0x4270494E
        private const uint BpnN = 1114664526; // 0x42706E4E
        private const uint BPIN = 1112557902; // 0x4250494E
        private const uint BPnN = 1112567374; // 0x42506E4E

        public string Type { get; protected set; }

        public Plane SplittingPlane { get; protected set; }

        public BSPNode PosNode { get; protected set; }
        public BSPNode NegNode { get; protected set; }

        public Sphere Sphere { get; protected set; }

        public List<ushort> InPolys { get; protected set; } // List of PolygonIds

        /// <summary>
        /// You must use the Unpack(BinaryReader reader, BSPType treeType) method.
        /// </summary>
        /// <exception cref="NotSupportedException">You must use the Unpack(BinaryReader reader, BSPType treeType) method.</exception>
        public virtual void Unpack(BinaryReader reader, bool isToD = true)
        {
            throw new NotSupportedException();
        }

        public virtual void Unpack(BinaryReader reader, BSPType treeType, bool isToD = true)
        {
            Type = Encoding.ASCII.GetString(reader.ReadBytes(4)).Reverse();

            switch (Type)
            {
                // These types will unpack the data completely, in their own classes
                case "PORT":
                case "LEAF":
                    throw new Exception();
            }

            SplittingPlane = new Plane();
            SplittingPlane.Unpack(reader);

            switch (Type)
            {
                case "BPnn":
                case "BPIn":
                    PosNode = BSPNode.ReadNode(reader, treeType, isToD);
                    break;
                case "BpIN":
                case "BpnN":
                    NegNode = BSPNode.ReadNode(reader, treeType, isToD);
                    break;
                case "BPIN":
                case "BPnN":
                    PosNode = BSPNode.ReadNode(reader, treeType, isToD);
                    NegNode = BSPNode.ReadNode(reader, treeType, isToD);
                    break;
            }

            if (treeType == BSPType.Cell)
                return;

            Sphere = new Sphere();
            Sphere.Unpack(reader);

            if (treeType == BSPType.Physics)
                return;

            InPolys = new List<ushort>();
            uint numPolys = reader.ReadUInt32();
            for (uint i = 0; i < numPolys; i++)
                InPolys.Add(reader.ReadUInt16());

            if (!isToD)
                reader.AlignBoundary();
        }

        public virtual void Pack(StreamWriter output, BSPType treeType)
        {
            for (int i = 3; i >= 0; i--)
            {
                Utils.writeByte((byte)Type[i],output);
            }

            switch (Type)
            {
                // These types will pack the data completely, in their own classes
                case "PORT":
                case "LEAF":
                    throw new Exception();
            }

            SplittingPlane.Pack(output);

            switch (Type)
            {
                case "BPnn":
                case "BPIn":
                    PosNode.Pack(output, treeType);
                    break;
                case "BpIN":
                case "BpnN":
                    NegNode.Pack(output, treeType);
                    break;
                case "BPIN":
                case "BPnN":
                    PosNode.Pack(output, treeType);
                    NegNode.Pack(output, treeType);
                    break;
            }

            if (treeType == BSPType.Cell)
                return;

            Sphere.Pack(output);

            if (treeType == BSPType.Physics)
                return;

            Utils.writeUInt32((uint)InPolys.Count, output);
            foreach (var entry in InPolys)
            {
                Utils.writeUInt16(entry, output);
            }
        }

        public static BSPNode ReadNode(BinaryReader reader, BSPType treeType, bool isToD = true)
        {
            // We peek forward to get the type, then revert our position.
            var type = Encoding.ASCII.GetString(reader.ReadBytes(4)).Reverse();
            reader.BaseStream.Position -= 4;

            BSPNode node;

            switch (type)
            {
                case "PORT":
                    node = new BSPPortal();
                    break;

                case "LEAF":
                    node = new BSPLeaf();
                    break;

                case "BPnn":
                case "BPIn":
                case "BpIN":
                case "BpnN":
                case "BPIN":
                case "BPnN":
                default:
                    node = new BSPNode();
                    break;
            }

            node.Unpack(reader, treeType, isToD);

            return node;
        }
    }
}
