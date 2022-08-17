using Melt;
using System.IO;

namespace ACE.DatLoader.Entity
{
    public class PortalPoly : IUnpackable, IPackable
    {
        public short PortalIndex { get; set; }
        public short PolygonId { get; set; }

        public void Unpack(BinaryReader reader, bool isToD = true)
        {
            PortalIndex = reader.ReadInt16();
            PolygonId   = reader.ReadInt16();
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeInt16(PortalIndex, output);
            Utils.writeInt16(PolygonId, output);
        }
    }
}
