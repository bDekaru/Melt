using System.IO;

namespace ACE.DatLoader.Entity.AnimationHooks
{
    public class TransparentPartHook : AnimationHook
    {
        public uint Part { get; set; }
        public float Start { get; set; }
        public float End { get; set; }
        public float Time { get; set; }

        public override void Unpack(BinaryReader reader, bool isToD = true)
        {
            base.Unpack(reader);

            Part    = reader.ReadUInt32();
            Start   = reader.ReadSingle();
            End     = reader.ReadSingle();
            Time    = reader.ReadSingle();
        }
    }
}
