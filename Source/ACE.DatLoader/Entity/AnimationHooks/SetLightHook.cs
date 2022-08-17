using System.IO;

namespace ACE.DatLoader.Entity.AnimationHooks
{
    public class SetLightHook : AnimationHook
    {
        public int LightsOn { get; private set; }

        public override void Unpack(BinaryReader reader, bool isToD = true)
        {
            base.Unpack(reader);

            LightsOn = reader.ReadInt32();
        }
    }
}
