using Melt;
using System.IO;

namespace ACE.DatLoader.Entity.AnimationHooks
{
    public class AttackHook : AnimationHook
    {
        public AttackCone AttackCone { get; } = new AttackCone();

        public override void Unpack(BinaryReader reader, bool isToD = true)
        {
            base.Unpack(reader);

            AttackCone.Unpack(reader);
        }

        public override void Pack(StreamWriter output)
        {
            base.Pack(output);

            AttackCone.Pack(output);
        }
    }
}
