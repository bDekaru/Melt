using System.Collections.Generic;
using System.IO;
using System.Numerics;

using ACE.DatLoader.Entity;
using ACE.Entity.Enum;
using Melt;

namespace ACE.DatLoader.FileTypes
{
    /// <summary>
    /// These are client_portal.dat files starting with 0x01. 
    /// These are used both on their own for some pre-populated structures in the world (trees, buildings, etc) or make up SetupModel (0x02) objects.
    /// </summary>
    [DatFileType(DatFileType.GraphicsObject)]
    public class GfxObj : FileType
    {
        public GfxObjFlags Flags { get; private set; }
        public List<uint> Surfaces { get; set; } = new List<uint>(); // also referred to as m_rgSurfaces in the client
        public CVertexArray VertexArray { get; } = new CVertexArray();

        public Dictionary<ushort, Polygon> PhysicsPolygons { get; } = new Dictionary<ushort, Polygon>();
        public BSPTree PhysicsBSP { get; } = new BSPTree();

        public Vector3 SortCenter { get; private set; }

        public Dictionary<ushort, Polygon> Polygons { get; } = new Dictionary<ushort, Polygon>();
        public BSPTree DrawingBSP { get; } = new BSPTree();

        public uint DIDDegrade { get; private set; }

        public override void Unpack(BinaryReader reader, bool isToD = true)
        {

            Id = reader.ReadUInt32();

            Flags = (GfxObjFlags)reader.ReadUInt32();

            if(isToD)
                Surfaces.UnpackSmartArray(reader);
            else
                Surfaces.Unpack(reader);

            VertexArray.Unpack(reader, isToD);

            // Has Physics 
            if ((Flags & GfxObjFlags.HasPhysics) != 0)
            {
                if (isToD)
                    PhysicsPolygons.UnpackSmartArray(reader, isToD);
                else
                    PhysicsPolygons.Unpack(reader, isToD);

                PhysicsBSP.Unpack(reader, BSPType.Physics, isToD);
            }

            SortCenter = reader.ReadVector3();

            // Has Drawing 
            if ((Flags & GfxObjFlags.HasDrawing) != 0)
            {
                if (isToD)
                    Polygons.UnpackSmartArray(reader, isToD);
                else
                    Polygons.Unpack(reader, isToD);

                DrawingBSP.Unpack(reader, BSPType.Drawing, isToD);
            }

            if ((Flags & GfxObjFlags.HasDIDDegrade) != 0)
                DIDDegrade = reader.ReadUInt32();
        }

        public void Pack(StreamWriter output)
        {
            Utils.writeUInt32(Id, output);

            Utils.writeUInt32((uint)Flags, output);

            Surfaces.PackSmartArray(output);

            VertexArray.Pack(output);

            // Has Physics 
            if ((Flags & GfxObjFlags.HasPhysics) != 0)
            {
                PhysicsPolygons.PackSmartArray(output);
                PhysicsBSP.Pack(output, BSPType.Physics);
            }

            Utils.writeVector3(SortCenter, output);

            output.Flush();

            // Has Drawing 
            if ((Flags & GfxObjFlags.HasDrawing) != 0)
            {
                Polygons.PackSmartArray(output);

                DrawingBSP.Pack(output, BSPType.Drawing);
            }

            if ((Flags & GfxObjFlags.HasDIDDegrade) != 0)
                Utils.writeUInt32(DIDDegrade, output);
        }
    }
}
