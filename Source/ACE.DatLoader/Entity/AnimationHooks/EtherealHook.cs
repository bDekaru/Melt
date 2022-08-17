using System.IO;

namespace ACE.DatLoader.Entity.AnimationHooks
{
    public class EtherealHook : AnimationHook
    {
        public int Ethereal { get; private set; }

        public override void Unpack(BinaryReader reader, bool isToD = true)
        {
            base.Unpack(reader);

            Ethereal = reader.ReadInt32();
        }
    }
}
