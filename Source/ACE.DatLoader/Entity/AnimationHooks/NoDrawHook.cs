using System.IO;

namespace ACE.DatLoader.Entity.AnimationHooks
{
    public class NoDrawHook : AnimationHook
    {
        public uint NoDraw { get; private set; }

        public override void Unpack(BinaryReader reader, bool isToD = true)
        {
            base.Unpack(reader);

            NoDraw = reader.ReadUInt32();
        }
    }
}
