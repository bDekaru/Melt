using System.IO;

namespace ACE.DatLoader.Entity.AnimationHooks
{
    public class DefaultScriptPartHook : AnimationHook
    {
        public uint PartIndex { get; private set; }

        public override void Unpack(BinaryReader reader, bool isToD = true)
        {
            base.Unpack(reader);

            PartIndex = reader.ReadUInt32();
        }
    }
}
