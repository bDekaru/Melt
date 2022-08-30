using Melt;
using System.IO;

namespace ACE.DatLoader.Entity
{
    public class PhysicsScriptData : IUnpackable, IPackable
    {
        public double StartTime { get; set; }
        public AnimationHook Hook { get; set; } = new AnimationHook();

        public void Unpack(BinaryReader reader, bool isToD = true)
        {
            StartTime = reader.ReadDouble();

            Hook = AnimationHook.ReadHook(reader);
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeDouble(StartTime, output);

            Hook.Pack(output);
        }
    }
}
