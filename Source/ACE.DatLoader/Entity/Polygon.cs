using System.Collections.Generic;
using System.IO;
using ACE.Entity.Enum;
using Melt;

namespace ACE.DatLoader.Entity
{
    public class Polygon : IUnpackable, IPackable
    {
        public byte NumPts { get; private set; }
        public StipplingType Stippling { get; private set; } // Whether it has that textured/bumpiness to it

        public CullMode SidesType { get; private set; }
        public short PosSurface { get; private set; }
        public short NegSurface { get; private set; }

        public List<short> VertexIds { get; } = new List<short>();

        public List<byte> PosUVIndices { get; } = new List<byte>();
        public List<byte> NegUVIndices { get; private set; } = new List<byte>();

        public List<SWVertex> Vertices;

        public void Unpack(BinaryReader reader, bool isToD = true)
        {
            NumPts      = reader.ReadByte();
            Stippling   = (StipplingType)reader.ReadByte();

            SidesType   = (CullMode)reader.ReadInt32();
            PosSurface  = reader.ReadInt16();
            NegSurface  = reader.ReadInt16();

            for (short i = 0; i < NumPts; i++)
                VertexIds.Add(reader.ReadInt16());

            if (!Stippling.HasFlag(StipplingType.NoPos))
            {
                for (short i = 0; i < NumPts; i++)
                    PosUVIndices.Add(reader.ReadByte());
            }

            if (SidesType == CullMode.Clockwise && !Stippling.HasFlag(StipplingType.NoNeg))
            {
                for (short i = 0; i < NumPts; i++)
                    NegUVIndices.Add(reader.ReadByte());
            }

            if (SidesType == CullMode.None)
            {
                NegSurface = PosSurface;
                NegUVIndices = PosUVIndices;
            }

            if (!isToD)
                reader.AlignBoundary();
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeByte(NumPts, output);

            Utils.writeByte((byte)Stippling, output);

            Utils.writeInt32((int)SidesType, output);
            Utils.writeInt16(PosSurface, output);
            Utils.writeInt16(NegSurface, output);

            foreach(var entry in VertexIds)
            {
                Utils.writeInt16(entry, output);
            }

            if (!Stippling.HasFlag(StipplingType.NoPos))
            {
                foreach (var entry in PosUVIndices)
                {
                    Utils.writeByte(entry, output);
                }
            }

            if (SidesType == CullMode.Clockwise && !Stippling.HasFlag(StipplingType.NoNeg))
            {
                foreach (var entry in NegUVIndices)
                {
                    Utils.writeByte(entry, output);
                }
            }
        }

        public void LoadVertices(CVertexArray vertexArray)
        {
            Vertices = new List<SWVertex>();

            foreach (var id in VertexIds)
                Vertices.Add(vertexArray.Vertices[(ushort)id]);
        }
    }
}
