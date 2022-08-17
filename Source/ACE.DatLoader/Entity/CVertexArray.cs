using Melt;
using System;
using System.Collections.Generic;
using System.IO;

namespace ACE.DatLoader.Entity
{
    /// <summary>
    /// A list of indexed vertices, and their associated type
    /// </summary>
    public class CVertexArray : IUnpackable
    {
        public int VertexType { get; private set; }
        public Dictionary<ushort, SWVertex> Vertices { get; } = new Dictionary<ushort, SWVertex>();

        public void Unpack(BinaryReader reader, bool isToD = true)
        {
            VertexType = reader.ReadInt32();

            var numVertices = reader.ReadUInt32();

            if (VertexType == 1)
                Vertices.Unpack(reader, numVertices, isToD);
            else
                throw new NotImplementedException();
        }

        public void Pack(StreamWriter output)
        {
            switch (VertexType)
            {
                case 1:
                    Utils.writeInt32(VertexType, output);
                    Utils.writeInt32(Vertices.Count, output);
                    Vertices.Pack(output);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
