using Melt;
using System.Collections.Generic;
using System.IO;

namespace ACE.DatLoader.Entity
{
    public class PhysicsScriptTableData : IUnpackable, IPackable
    {
        public List<ScriptAndModData> Scripts { get; } = new List<ScriptAndModData>();

        public void Unpack(BinaryReader reader, bool isToD = true)
        {
            Scripts.Unpack(reader);
        }

        public void Pack(StreamWriter output)
        {
            Scripts.Pack(output);
        }
    }
}
