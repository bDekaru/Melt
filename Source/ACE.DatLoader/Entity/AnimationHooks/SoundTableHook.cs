using Melt;
using System.IO;

namespace ACE.DatLoader.Entity.AnimationHooks
{
    public class SoundTableHook : AnimationHook
    {
        public uint SoundType { get; private set; }

        public override void Unpack(BinaryReader reader, bool isToD = true)
        {
            base.Unpack(reader);

            SoundType = reader.ReadUInt32();
        }

        public override void Pack(StreamWriter output)
        {
            base.Pack(output);

            Utils.writeUInt32(SoundType, output);
        }
    }
}
