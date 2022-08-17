using System;
using System.IO;

using ACE.Entity.Enum;

namespace ACE.DatLoader.Entity
{
    public class BSPTree : IUnpackable, IPackable
    {
        public BSPNode RootNode { get; private set; } = new BSPNode();

        /// <summary>
        /// You must use the Unpack(BinaryReader reader, BSPType treeType) method.
        /// </summary>
        /// <exception cref="NotSupportedException">You must use the Unpack(BinaryReader reader, BSPType treeType) method.</exception>
        public void Unpack(BinaryReader reader, bool isToD = true)
        {
            throw new NotSupportedException();
        }

        public void Pack(StreamWriter output)
        {
            throw new NotImplementedException();
        }

        public void Unpack(BinaryReader reader, BSPType treeType, bool isToD = true)
        {
            RootNode = BSPNode.ReadNode(reader, treeType, isToD);
        }

        public void Pack(StreamWriter output, BSPType treeType)
        {
            RootNode.Pack(output, treeType);
        }
    }
}
