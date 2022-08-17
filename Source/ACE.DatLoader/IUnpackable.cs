using System.IO;

namespace ACE.DatLoader
{
    public interface IUnpackable
    {
        void Unpack(BinaryReader reader, bool isToD = true);
    }

    public interface IPackable
    {
        void Pack(StreamWriter output);
    }
}
