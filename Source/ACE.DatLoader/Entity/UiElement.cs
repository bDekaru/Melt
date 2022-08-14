using System;
using System.IO;
using System.Collections.Generic;
using Melt;

namespace ACE.DatLoader.Entity
{
    public class UiElement : IUnpackable, IPackable
    {
        public uint StateId;
        public byte PassToChildren;
        public uint UiIncorporationFlags;
        public byte HashPropertiesBucketSize;
        public Dictionary<uint, UiHashProperty> HashProperties;
        public List<UiMedia> Media;
        public uint UiReadOrder;
        public uint ElementID;
        public uint Type;
        public uint BaseElement;
        public uint BaseLayout;
        public uint DefaultState;
        public int X;
        public int Y;
        public int Z_Level;
        public int Width;
        public int Height;
        public int LeftEdge;
        public int TopEdge;
        public int RightEdge;
        public int BottomEdge;
        public byte StatesBucketSize;
        public Dictionary<uint, UiState> States;
        public byte ChildrenBucketSize;
        public Dictionary<uint, UiElement> Children;

        public void Unpack(BinaryReader reader)
        {
            StateId = reader.ReadUInt32();
            PassToChildren = reader.ReadByte();
            UiIncorporationFlags = reader.ReadUInt32();

            HashProperties = new Dictionary<uint, UiHashProperty>();
            HashPropertiesBucketSize = HashProperties.UnpackBytePackedHashTable(reader);

            Media = new List<UiMedia>();
            Media.UnpackSmartArray(reader);

            UiReadOrder = reader.ReadUInt32();
            ElementID = reader.ReadUInt32();
            Type = reader.ReadUInt32();
            BaseElement = reader.ReadUInt32();
            BaseLayout = reader.ReadUInt32();
            DefaultState = reader.ReadUInt32();
            X = reader.ReadInt32();
            Y = reader.ReadInt32();
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            if(ElementID == 0x100003be ||
                ElementID == 0x100003bf ||
                ElementID == 0x00000000 ||
                ElementID == 0x00000005)
                Z_Level = reader.ReadInt32();
            LeftEdge = reader.ReadInt32();
            TopEdge = reader.ReadInt32();
            RightEdge = reader.ReadInt32();
            BottomEdge = reader.ReadInt32();
            States = new Dictionary<uint, UiState>();
            StatesBucketSize = States.UnpackBytePackedHashTable(reader);
            Children = new Dictionary<uint, UiElement>();
            ChildrenBucketSize = Children.UnpackBytePackedHashTable(reader);
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeUInt32(StateId, output);
            Utils.writeByte(PassToChildren, output);
            Utils.writeUInt32(UiIncorporationFlags, output);

            HashProperties.PackBytePackedHashTable(HashPropertiesBucketSize, output);

            Media.PackSmartArray(output);

            Utils.writeUInt32(UiReadOrder, output);
            Utils.writeUInt32(ElementID, output);
            Utils.writeUInt32(Type, output);
            Utils.writeUInt32(BaseElement, output);
            Utils.writeUInt32(BaseLayout, output);
            Utils.writeUInt32(DefaultState, output);
            Utils.writeInt32(X, output);
            Utils.writeInt32(Y, output);
            Utils.writeInt32(Width, output);
            Utils.writeInt32(Height, output);
            Utils.writeInt32(LeftEdge, output);
            Utils.writeInt32(TopEdge, output);
            Utils.writeInt32(RightEdge, output);
            Utils.writeInt32(BottomEdge, output);

            States.PackBytePackedHashTable(StatesBucketSize, output);
            Children.PackBytePackedHashTable(ChildrenBucketSize, output);
        }
    }

    //public interface UiElementType
    //{
    //    void Unpack(BinaryReader reader);
    //    void Pack(StreamWriter output);
    //}

    //public class UiElementBase : UiElementType
    //{
    //    public uint BaseElement;
    //    public uint BaseLayout;
    //    public uint DefaultState;

    //    public void Unpack(BinaryReader reader)
    //    {
    //        BaseElement = reader.ReadUInt32();
    //        BaseLayout = reader.ReadUInt32();
    //        DefaultState = reader.ReadUInt32();
    //    }

    //    public void Pack(StreamWriter output)
    //    {
    //        Utils.writeUInt32(BaseElement, output);
    //        Utils.writeUInt32(BaseLayout, output);
    //        Utils.writeUInt32(DefaultState, output);
    //    }
    //}

    //public class UiElementHeritageField : UiElementType
    //{
    //    public uint BaseElement;
    //    public uint BaseLayout;
    //    public uint DefaultState;

    //    public void Unpack(BinaryReader reader)
    //    {
    //        BaseElement = reader.ReadUInt32();
    //        BaseLayout = reader.ReadUInt32();
    //        DefaultState = reader.ReadUInt32();
    //    }

    //    public void Pack(StreamWriter output)
    //    {
    //        Utils.writeUInt32(BaseElement, output);
    //        Utils.writeUInt32(BaseLayout, output);
    //        Utils.writeUInt32(DefaultState, output);
    //    }
    //}
}