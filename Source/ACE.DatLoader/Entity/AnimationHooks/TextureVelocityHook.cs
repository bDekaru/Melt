using System.IO;

namespace ACE.DatLoader.Entity.AnimationHooks
{
    public class TextureVelocityHook : AnimationHook
    {
        public float USpeed { get; private set; }
        public float VSpeed { get; private set; }

        public override void Unpack(BinaryReader reader, bool isToD = true)
        {
            base.Unpack(reader);

            USpeed = reader.ReadSingle();
            VSpeed = reader.ReadSingle();
        }
    }
}
