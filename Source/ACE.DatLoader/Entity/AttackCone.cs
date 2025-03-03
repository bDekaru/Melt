using Melt;
using System.IO;

namespace ACE.DatLoader.Entity
{
    public class AttackCone : IUnpackable, IPackable
    {
        public uint PartIndex { get; set; }
        
        // these Left and Right are technically Vec2D types
        public float LeftX { get; private set; }
        public float LeftY { get; private set; }

        public float RightX { get; private set; }
        public float RightY { get; private set; }

        public float Radius { get; private set; }
        public float Height { get; private set; }

        public void Unpack(BinaryReader reader, bool isToD = true)
        {
            PartIndex   = reader.ReadUInt32();

            LeftX       = reader.ReadSingle();
            LeftY       = reader.ReadSingle();

            RightX      = reader.ReadSingle();
            RightY      = reader.ReadSingle();
            
            Radius      = reader.ReadSingle();
            Height      = reader.ReadSingle();
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeUInt32(PartIndex, output);

            Utils.writeSingle(LeftX, output);
            Utils.writeSingle(LeftY, output);

            Utils.writeSingle(RightX, output);
            Utils.writeSingle(RightY, output);

            Utils.writeSingle(Radius, output);
            Utils.writeSingle(Height, output);
        }
    }
}
