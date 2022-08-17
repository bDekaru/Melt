using System.IO;

namespace ACE.DatLoader.FileTypes
{
    public abstract class FileType : IUnpackable
    {
        public uint Id { get; set; }

        public abstract void Unpack(BinaryReader reader, bool isToD = true);
    }
}
