using Melt;
using System.IO;

namespace ACE.DatLoader.Entity.AnimationHooks
{
    public class SoundHook : AnimationHook
    {
        public uint Id { get; private set; }

        public override void Unpack(BinaryReader reader, bool isToD = true)
        {
            base.Unpack(reader);

            Id = reader.ReadUInt32();
        }

        public override void Pack(StreamWriter output)
        {
            base.Pack(output);

            Utils.writeUInt32(Id, output);
        }
    }
}
