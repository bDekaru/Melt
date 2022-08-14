using System.IO;

namespace ACE.DatLoader
{
    public interface IUnpackable
    {
        void Unpack(BinaryReader reader);
    }

    public interface IPackable
    {
        void Pack(StreamWriter output);
    }
}
