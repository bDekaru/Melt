using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Melt
{
    class RegionComparer
    {
        public class RegionMisc
        {
            public uint Version { get; set; }
            public uint GameMapID { get; set; }
            public uint AutotestMapId { get; set; }
            public uint AutotestMapSize { get; set; }
            public uint ClearCellId { get; set; }
            public uint ClearMonsterId { get; set; }

            public static RegionMisc Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                RegionMisc obj = new RegionMisc();

                obj.Version = Utils.readAndWriteUInt32(data, outputData, write);
                obj.GameMapID = Utils.readAndWriteUInt32(data, outputData, write);
                obj.AutotestMapId = Utils.readAndWriteUInt32(data, outputData, write);
                obj.AutotestMapSize = Utils.readAndWriteUInt32(data, outputData, write);
                obj.ClearCellId = Utils.readAndWriteUInt32(data, outputData, write);
                obj.ClearMonsterId = Utils.readAndWriteUInt32(data, outputData, write);

                return obj;
            }

            public bool Compare(RegionMisc other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (Version != other.Version)
                {
                    outputFile.WriteLine("\tVersion is different: {0} {1}", Version, other.Version);
                    comparisonResult = false;
                }

                if (GameMapID != other.GameMapID)
                {
                    outputFile.WriteLine("\tGameMapID is different: {0} {1}", GameMapID, other.GameMapID);
                    comparisonResult = false;
                }

                if (AutotestMapId != other.AutotestMapId)
                {
                    outputFile.WriteLine("\tAutotestMapId is different: {0} {1}", AutotestMapId, other.AutotestMapId);
                    comparisonResult = false;
                }

                if (AutotestMapSize != other.AutotestMapSize)
                {
                    outputFile.WriteLine("\tAutotestMapSize is different: {0} {1}", AutotestMapSize, other.AutotestMapSize);
                    comparisonResult = false;
                }

                if (ClearCellId != other.ClearCellId)
                {
                    outputFile.WriteLine("\tClearCellId is different: {0} {1}", ClearCellId, other.ClearCellId);
                    comparisonResult = false;
                }

                if (ClearMonsterId != other.ClearMonsterId)
                {
                    outputFile.WriteLine("\tClearMonsterId is different: {0} {1}", ClearMonsterId, other.ClearMonsterId);
                    comparisonResult = false;
                }

                return comparisonResult;
            }
        }

        public class TerrainTex
        {
            public uint TexGID { get; set; }
            public uint TexTiling { get; set; }
            public uint MaxVertBright { get; set; }
            public uint MinVertBright { get; set; }
            public uint MaxVertSaturate { get; set; }
            public uint MinVertSaturate { get; set; }
            public uint MaxVertHue { get; set; }
            public uint MinVertHue { get; set; }
            public uint DetailTexTiling { get; set; }
            public uint DetailTexGID { get; set; }

            public static TerrainTex Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                TerrainTex obj = new TerrainTex();

                obj.TexGID = Utils.readAndWriteUInt32(data, outputData, write);
                obj.TexTiling = Utils.readAndWriteUInt32(data, outputData, write);
                obj.MaxVertBright = Utils.readAndWriteUInt32(data, outputData, write);
                obj.MinVertBright = Utils.readAndWriteUInt32(data, outputData, write);
                obj.MaxVertSaturate = Utils.readAndWriteUInt32(data, outputData, write);
                obj.MinVertSaturate = Utils.readAndWriteUInt32(data, outputData, write);
                obj.MaxVertHue = Utils.readAndWriteUInt32(data, outputData, write);
                obj.MinVertHue = Utils.readAndWriteUInt32(data, outputData, write);
                obj.DetailTexTiling = Utils.readAndWriteUInt32(data, outputData, write);
                obj.DetailTexGID = Utils.readAndWriteUInt32(data, outputData, write);

                return obj;
            }
            public void Write(StreamWriter outputData)
            {
                outputData.BaseStream.Write(BitConverter.GetBytes(TexGID), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(TexTiling), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(MaxVertBright), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(MinVertBright), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(MaxVertSaturate), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(MinVertSaturate), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(MaxVertHue), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(MinVertHue), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(DetailTexTiling), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(DetailTexGID), 0, 4);
            }

            public bool Compare(TerrainTex other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (TexGID != other.TexGID)
                {
                    outputFile.WriteLine("\tTexGID is different: {0} {1}", TexGID, other.TexGID);
                    comparisonResult = false;
                }

                if (MaxVertBright != other.MaxVertBright)
                {
                    outputFile.WriteLine("\tMaxVertBright is different: {0} {1}", MaxVertBright, other.MaxVertBright);
                    comparisonResult = false;
                }

                if (MinVertBright != other.MinVertBright)
                {
                    outputFile.WriteLine("\tMinVertBright is different: {0} {1}", MinVertBright, other.MinVertBright);
                    comparisonResult = false;
                }

                if (MaxVertSaturate != other.MaxVertSaturate)
                {
                    outputFile.WriteLine("\tMaxVertSaturate is different: {0} {1}", MaxVertSaturate, other.MaxVertSaturate);
                    comparisonResult = false;
                }

                if (MinVertSaturate != other.MinVertSaturate)
                {
                    outputFile.WriteLine("\tMinVertSaturate is different: {0} {1}", MinVertSaturate, other.MinVertSaturate);
                    comparisonResult = false;
                }

                if (MaxVertHue != other.MaxVertHue)
                {
                    outputFile.WriteLine("\tMaxVertHue is different: {0} {1}", MaxVertHue, other.MaxVertHue);
                    comparisonResult = false;
                }

                if (MinVertHue != other.MinVertHue)
                {
                    outputFile.WriteLine("MinVertHue is different: {0} {1}", MinVertHue, other.MinVertHue);
                    comparisonResult = false;
                }

                if (DetailTexTiling != other.DetailTexTiling)
                {
                    outputFile.WriteLine("\tDetailTexTiling is different: {0} {1}", DetailTexTiling, other.DetailTexTiling);
                    comparisonResult = false;
                }

                if (DetailTexGID != other.DetailTexGID)
                {
                    outputFile.WriteLine("\tDetailTexGID is different: {0} {1}", DetailTexGID, other.DetailTexGID);
                    comparisonResult = false;
                }

                return comparisonResult;
            }
        }
        public class TMTerrainDesc
        {
            public uint terrainType { get; set; }
            public TerrainTex terrainTex { get; set; }

            public static TMTerrainDesc Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                TMTerrainDesc obj = new TMTerrainDesc();

                obj.terrainType = Utils.readAndWriteUInt32(data, outputData, write);
                obj.terrainTex = TerrainTex.Read(data, outputData, isToD, write);

                return obj;
            }

            public void Write(StreamWriter outputData)
            {
                outputData.BaseStream.Write(BitConverter.GetBytes(terrainType), 0, 4);
                terrainTex.Write(outputData);
            }

            public bool Compare(TMTerrainDesc other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (terrainType != other.terrainType)
                    comparisonResult = false;

                if (!terrainTex.Compare(terrainTex, outputFile))
                    comparisonResult = false;

                return comparisonResult;
            }
        }

        public class RoadAlphaMap
        {
            public uint RCode { get; set; }
            public uint RoadTexGID { get; set; }

            public static RoadAlphaMap Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                RoadAlphaMap obj = new RoadAlphaMap();
                obj.RCode = Utils.readAndWriteUInt32(data, outputData, write);
                obj.RoadTexGID = Utils.readAndWriteUInt32(data, outputData, write);
                return obj;
            }

            public bool Compare(RoadAlphaMap other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (RCode != other.RCode)
                {
                    outputFile.WriteLine("\tRCode is different: {0} {1}", RCode, other.RCode);
                    comparisonResult = false;
                }

                if (RoadTexGID != other.RoadTexGID)
                {
                    outputFile.WriteLine("\tRoadTexGID is different: {0} {1}", RoadTexGID, other.RoadTexGID);
                    comparisonResult = false;
                }

                return comparisonResult;
            }
        }

        public class TerrainAlphaMap
        {
            public uint TCode { get; set; }
            public uint TexGID { get; set; }

            public static TerrainAlphaMap Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                TerrainAlphaMap obj = new TerrainAlphaMap();
                obj.TCode = Utils.readAndWriteUInt32(data, outputData, write);
                obj.TexGID = Utils.readAndWriteUInt32(data, outputData, write);
                return obj;
            }

            public bool Compare(TerrainAlphaMap other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (TCode != other.TCode)
                {
                    outputFile.WriteLine("\tTCode is different: {0} {1}", TCode, other.TCode);
                    comparisonResult = false;
                }

                if (TexGID != other.TexGID)
                {
                    outputFile.WriteLine("\tTexGID is different: {0} {1}", TexGID, other.TexGID);
                    comparisonResult = false;
                }

                return comparisonResult;
            }
        }

        public class TexMerge
        {
            public uint BaseTexSize { get; set; }
            public List<TerrainAlphaMap> CornerTerrainMaps { get; set; }
            public List<TerrainAlphaMap> SideTerrainMaps { get; set; }
            public List<RoadAlphaMap> RoadMaps { get; set; }
            public List<TMTerrainDesc> TerrainDescription { get; set; }

            public static TexMerge Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                TexMerge obj = new TexMerge();

                obj.CornerTerrainMaps = new List<TerrainAlphaMap>();
                obj.SideTerrainMaps = new List<TerrainAlphaMap>();
                obj.RoadMaps = new List<RoadAlphaMap>();
                obj.TerrainDescription = new List<TMTerrainDesc>();

                obj.BaseTexSize = Utils.readAndWriteUInt32(data, outputData, write);

                uint num_corner_terrain_maps = Utils.readAndWriteUInt32(data, outputData, write);
                for (uint i = 0; i < num_corner_terrain_maps; i++)
                    obj.CornerTerrainMaps.Add(TerrainAlphaMap.Read(data, outputData, isToD, write));

                uint num_side_terrain_maps = Utils.readAndWriteUInt32(data, outputData, write);
                for (uint i = 0; i < num_side_terrain_maps; i++)
                    obj.SideTerrainMaps.Add(TerrainAlphaMap.Read(data, outputData, isToD, write));

                uint num_road_maps = Utils.readAndWriteUInt32(data, outputData, write);
                for (uint i = 0; i < num_road_maps; i++)
                    obj.RoadMaps.Add(RoadAlphaMap.Read(data, outputData, isToD, write));

                uint num_terrain_desc = Utils.readAndWriteUInt32(data, outputData, write);

                for (uint i = 0; i < num_terrain_desc; i++)
                    obj.TerrainDescription.Add(TMTerrainDesc.Read(data, outputData, isToD, write));

                //TMTerrainDesc BarrenRock = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc Grassland = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc Ice = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc LushGrass = TMTerrainDesc.Read(data, outputData, false);

                //TMTerrainDesc MarshSparseSwamp = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc MudRichDirt = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc ObsidianPlain = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc PackedDirt = TMTerrainDesc.Read(data, outputData, false);

                //TMTerrainDesc PatchyDirt = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc PatchyGrassland = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc SandYellow = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc SandGrey = TMTerrainDesc.Read(data, outputData, false);

                //TMTerrainDesc SandRockStrewn = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc SedimentaryRock = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc SemiBarrenRock = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc Snow = TMTerrainDesc.Read(data, outputData, false);

                //TMTerrainDesc WaterRunning = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc WaterStandingFresh = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc WaterShallowSea = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc WaterShallowStillSea = TMTerrainDesc.Read(data, outputData, false);

                //TMTerrainDesc WaterDeepSea = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc Forestfloor = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc FauxWaterRunning = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc SeaSlime = TMTerrainDesc.Read(data, outputData, false);

                //TMTerrainDesc Argila = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc Volcano1 = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc Volcano2 = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc BlueIce = TMTerrainDesc.Read(data, outputData, false);

                //TMTerrainDesc Moss = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc DarkMoss = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc olthoi = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc DesolateLands = TMTerrainDesc.Read(data, outputData, false);
                //TMTerrainDesc Road = TMTerrainDesc.Read(data, outputData, false);

                //BarrenRock.terrainTex = Snow.terrainTex;
                //Grassland.terrainTex.TexGID = 0x0500146B;
                ////LushGrass.terrainTex = Snow.terrainTex;
                ////PatchyDirt.terrainTex = Snow.terrainTex;
                //PatchyGrassland.terrainTex = Snow.terrainTex;
                ////Forestfloor.terrainTex = Ice.terrainTex;
                //WaterRunning.terrainTex = BlueIce.terrainTex;
                //WaterStandingFresh.terrainTex = BlueIce.terrainTex;

                //BarrenRock.Write(outputData);
                //Grassland.Write(outputData);
                //Ice.Write(outputData);
                //LushGrass.Write(outputData);

                //MarshSparseSwamp.Write(outputData);
                //MudRichDirt.Write(outputData);
                //ObsidianPlain.Write(outputData);
                //PackedDirt.Write(outputData);

                //PatchyDirt.Write(outputData);
                //PatchyGrassland.Write(outputData);
                //SandYellow.Write(outputData);
                //SandGrey.Write(outputData);

                //SandRockStrewn.Write(outputData);
                //SedimentaryRock.Write(outputData);
                //SemiBarrenRock.Write(outputData);
                //Snow.Write(outputData);

                //WaterRunning.Write(outputData);
                //WaterStandingFresh.Write(outputData);
                //WaterShallowSea.Write(outputData);
                //WaterShallowStillSea.Write(outputData);

                //WaterDeepSea.Write(outputData);
                //Forestfloor.Write(outputData);
                //FauxWaterRunning.Write(outputData);
                //SeaSlime.Write(outputData);

                //Argila.Write(outputData);
                //Volcano1.Write(outputData);
                //Volcano2.Write(outputData);
                //BlueIce.Write(outputData);

                //Moss.Write(outputData);
                //DarkMoss.Write(outputData);
                //olthoi.Write(outputData);
                //DesolateLands.Write(outputData);
                //Road.Write(outputData);

                return obj;
            }

            public bool Compare(TexMerge other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (BaseTexSize != other.BaseTexSize)
                {
                    outputFile.WriteLine("\tBaseTexSize is different: {0} {1}", BaseTexSize, other.BaseTexSize);
                    comparisonResult = false;
                }

                if (CornerTerrainMaps.Count != other.CornerTerrainMaps.Count)
                {
                    outputFile.WriteLine("\tCornerTerrainMaps.Count is different: {0} {1}", CornerTerrainMaps.Count, other.CornerTerrainMaps.Count);
                    comparisonResult = false;
                }
                for (int i = 0; i < CornerTerrainMaps.Count && i < other.CornerTerrainMaps.Count; i++)
                {
                    if (!CornerTerrainMaps[i].Compare(other.CornerTerrainMaps[i], outputFile))
                    {
                        outputFile.WriteLine("\tCornerTerrainMaps[{0}] is different", i);
                        comparisonResult = false;
                    }
                }

                if (SideTerrainMaps.Count != other.SideTerrainMaps.Count)
                {
                    outputFile.WriteLine("\tSideTerrainMaps.Count is different: {0} {1}", SideTerrainMaps.Count, other.SideTerrainMaps.Count);
                    comparisonResult = false;
                }
                for (int i = 0; i < SideTerrainMaps.Count && i < other.SideTerrainMaps.Count; i++)
                {
                    if (!SideTerrainMaps[i].Compare(other.SideTerrainMaps[i], outputFile))
                    {
                        outputFile.WriteLine("\tSideTerrainMaps[{0}] is different", i, SideTerrainMaps[i], other.SideTerrainMaps[i]);
                        comparisonResult = false;
                    }
                }

                if (RoadMaps.Count != other.RoadMaps.Count)
                {
                    outputFile.WriteLine("\tRoadMaps.Count is different: {0} {1}", RoadMaps.Count, other.RoadMaps.Count);
                    comparisonResult = false;
                }
                for (int i = 0; i < RoadMaps.Count && i < other.RoadMaps.Count; i++)
                {
                    if (!RoadMaps[i].Compare(other.RoadMaps[i],outputFile))
                    {
                        outputFile.WriteLine("\tRoadMaps[{0}] is different", i, RoadMaps[i], other.RoadMaps[i]);
                        comparisonResult = false;
                    }
                }

                if (TerrainDescription.Count != other.TerrainDescription.Count)
                {
                    outputFile.WriteLine("\tTerrainDescription.Count is different: {0} {1}", TerrainDescription.Count, other.TerrainDescription.Count);
                    comparisonResult = false;
                }
                for (int i = 0; i < TerrainDescription.Count && i < other.TerrainDescription.Count; i++)
                {
                    if (!TerrainDescription[i].Compare(other.TerrainDescription[i],outputFile))
                    {
                        outputFile.WriteLine("\tTerrainDescription[{0}] is different", i, TerrainDescription[i], other.TerrainDescription[i]);
                        comparisonResult = false;
                    }
                }

                return comparisonResult;
            }
        }

        public class LandSurf
        {
            public uint HasPalShift { get; set; }
            public TexMerge texMerge { get; set; }

            public static LandSurf Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                LandSurf obj = new LandSurf();

                obj.HasPalShift = Utils.readAndWriteUInt32(data, outputData, write); // This is always 0

                if (obj.HasPalShift == 1)
                {
                    // PalShift.Read would go here, if it ever actually existed...which it doesn't.
                    // PalShift is used in software rendering in pre-ToD versions
                }
                else
                    obj.texMerge = TexMerge.Read(data, outputData, isToD, write);

                return obj;
            }

            public bool Compare(LandSurf other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (HasPalShift != other.HasPalShift)
                {
                    outputFile.WriteLine("\tHasPalShift is different: {0} {1}", HasPalShift, other.HasPalShift);
                    comparisonResult = false;
                }
                if (!texMerge.Compare(other.texMerge, outputFile))
                {
                    outputFile.WriteLine("\ttexMerge is different: {0} {1}", texMerge, other.texMerge);
                    comparisonResult = false;
                }

                return comparisonResult;
            }
        }

        public class TerrainType
        {
            public string TerrainName { get; set; }
            public uint TerrainColor { get; set; }
            public List<uint> SceneTypes { get; set; }

            public static TerrainType Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                TerrainType obj = new TerrainType();

                obj.TerrainName = Utils.readAndWriteString(data, outputData, write);

                obj.TerrainColor = Utils.readAndWriteUInt32(data, outputData, write);

                obj.SceneTypes = new List<uint>();
                uint num_stypes = Utils.readAndWriteUInt32(data, outputData, write);
                for (uint i = 0; i < num_stypes; i++)
                    obj.SceneTypes.Add(Utils.readAndWriteUInt32(data, outputData, write));

                return obj;
            }

            public void Write(StreamWriter outputData)
            {
                byte[] buffer = new byte[1024];
                outputData.BaseStream.Write(BitConverter.GetBytes((short)TerrainName.Length), 0, 2);
                Utils.convertStringToByteArray(TerrainName, ref buffer, 0, TerrainName.Length);
                int startIndex = (int)outputData.BaseStream.Position;
                int endIndex = (int)outputData.BaseStream.Position + TerrainName.Length + 2;
                int alignedIndex = Utils.align4(endIndex - startIndex);
                int newIndex = startIndex + alignedIndex;
                int bytesNeededToReachAlignment = newIndex - endIndex;
                outputData.BaseStream.Write(buffer, 0, TerrainName.Length + bytesNeededToReachAlignment);

                outputData.BaseStream.Write(BitConverter.GetBytes(TerrainColor), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(SceneTypes.Count), 0, 4);
                foreach (uint stype in SceneTypes)
                    outputData.BaseStream.Write(BitConverter.GetBytes(stype), 0, 4);
            }

            public bool Compare(TerrainType other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (TerrainName.Replace("\0","") != other.TerrainName)
                {
                    outputFile.WriteLine("\tTerrainName is different: {0} {1}", TerrainName, other.TerrainName);
                    comparisonResult = false;
                }
                if (TerrainColor != other.TerrainColor)
                {
                    outputFile.WriteLine("\tTerrainColor is different: {0} {1}", TerrainColor, other.TerrainColor);
                    comparisonResult = false;
                }

                if (SceneTypes.Count != other.SceneTypes.Count)
                {
                    outputFile.WriteLine("\tSceneTypes.Count is different: {0} {1}", SceneTypes.Count, other.SceneTypes.Count);
                    comparisonResult = false;
                }
                for (int i = 0; i < SceneTypes.Count && i < other.SceneTypes.Count; i++)
                {
                    if (SceneTypes[i] != other.SceneTypes[i])
                    {
                        outputFile.WriteLine("\tSceneTypes[{0}] is different", i, SceneTypes[i], other.SceneTypes[i]);
                        comparisonResult = false;
                    }
                }

                return comparisonResult;
            }
        }

        public class TerrainDesc
        {
            public List<TerrainType> TerrainTypes { get; set; }
            public LandSurf LandSurfaces { get; set; }

            public static TerrainDesc Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                TerrainDesc obj = new TerrainDesc();

                uint num_terrain_types = Utils.readAndWriteUInt32(data, outputData, write);

                obj.TerrainTypes = new List<TerrainType>();

                for (uint i = 0; i < num_terrain_types; i++)
                    obj.TerrainTypes.Add(TerrainType.Read(data, outputData, isToD, write));

                //TerrainType BarrenRock = TerrainType.Read(data, outputData, false);
                //TerrainType Grassland = TerrainType.Read(data, outputData, false);
                //TerrainType Ice = TerrainType.Read(data, outputData, false);
                //TerrainType LushGrass = TerrainType.Read(data, outputData, false);

                //TerrainType MarshSparseSwamp = TerrainType.Read(data, outputData, false);
                //TerrainType MudRichDirt = TerrainType.Read(data, outputData, false);
                //TerrainType ObsidianPlain = TerrainType.Read(data, outputData, false);
                //TerrainType PackedDirt = TerrainType.Read(data, outputData, false);

                //TerrainType PatchyDirt = TerrainType.Read(data, outputData, false);
                //TerrainType PatchyGrassland = TerrainType.Read(data, outputData, false);
                //TerrainType SandYellow = TerrainType.Read(data, outputData, false);
                //TerrainType SandGrey = TerrainType.Read(data, outputData, false);

                //TerrainType SandRockStrewn = TerrainType.Read(data, outputData, false);
                //TerrainType SedimentaryRock = TerrainType.Read(data, outputData, false);
                //TerrainType SemiBarrenRock = TerrainType.Read(data, outputData, false);
                //TerrainType Snow = TerrainType.Read(data, outputData, false);

                //TerrainType WaterRunning = TerrainType.Read(data, outputData, false);
                //TerrainType WaterStandingFresh = TerrainType.Read(data, outputData, false);
                //TerrainType WaterShallowSea = TerrainType.Read(data, outputData, false);
                //TerrainType WaterShallowStillSea = TerrainType.Read(data, outputData, false);

                //TerrainType WaterDeepSea = TerrainType.Read(data, outputData, false);
                //TerrainType Forestfloor = TerrainType.Read(data, outputData, false);
                //TerrainType FauxWaterRunning = TerrainType.Read(data, outputData, false);
                //TerrainType SeaSlime = TerrainType.Read(data, outputData, false);

                //TerrainType Argila = TerrainType.Read(data, outputData, false);
                //TerrainType Volcano1 = TerrainType.Read(data, outputData, false);
                //TerrainType Volcano2 = TerrainType.Read(data, outputData, false);
                //TerrainType BlueIce = TerrainType.Read(data, outputData, false);

                //TerrainType Moss = TerrainType.Read(data, outputData, false);
                //TerrainType DarkMoss = TerrainType.Read(data, outputData, false);
                //TerrainType olthoi = TerrainType.Read(data, outputData, false);
                //TerrainType DesolateLands = TerrainType.Read(data, outputData, false);

                ////BarrenRock = Snow;
                ////Grassland = Snow;
                ////LushGrass = Snow;
                ////PatchyDirt = Snow;
                ////PatchyGrassland = Snow;
                ////Forestfloor = Snow;

                //BarrenRock.Write(outputData);
                //Grassland.Write(outputData);
                //Ice.Write(outputData);
                //LushGrass.Write(outputData);

                //MarshSparseSwamp.Write(outputData);
                //MudRichDirt.Write(outputData);
                //ObsidianPlain.Write(outputData);
                //PackedDirt.Write(outputData);

                //PatchyDirt.Write(outputData);
                //PatchyGrassland.Write(outputData);
                //SandYellow.Write(outputData);
                //SandGrey.Write(outputData);

                //SandRockStrewn.Write(outputData);
                //SedimentaryRock.Write(outputData);
                //SemiBarrenRock.Write(outputData);
                //Snow.Write(outputData);

                //WaterRunning.Write(outputData);
                //WaterStandingFresh.Write(outputData);
                //WaterShallowSea.Write(outputData);
                //WaterShallowStillSea.Write(outputData);

                //WaterDeepSea.Write(outputData);
                //Forestfloor.Write(outputData);
                //FauxWaterRunning.Write(outputData);
                //SeaSlime.Write(outputData);

                //Argila.Write(outputData);
                //Volcano1.Write(outputData);
                //Volcano2.Write(outputData);
                //BlueIce.Write(outputData);

                //Moss.Write(outputData);
                //DarkMoss.Write(outputData);
                //olthoi.Write(outputData);
                //DesolateLands.Write(outputData);

                obj.LandSurfaces = LandSurf.Read(data, outputData, isToD, write);

                return obj;
            }

            public bool Compare(TerrainDesc other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (TerrainTypes.Count != other.TerrainTypes.Count)
                {
                    outputFile.WriteLine("\tTerrainTypes.Count is different: {0} {1}", TerrainTypes.Count, other.TerrainTypes.Count);
                    comparisonResult = false;
                }
                for (int i = 0; i < TerrainTypes.Count && i < other.TerrainTypes.Count; i++)
                {
                    if (!TerrainTypes[i].Compare(other.TerrainTypes[i], outputFile))
                    {
                        outputFile.WriteLine("\tTerrainTypes[{0}] is different", i, TerrainTypes[i], other.TerrainTypes[i]);
                        comparisonResult = false;
                    }
                }

                if (!LandSurfaces.Compare(other.LandSurfaces, outputFile))
                    comparisonResult = false;

                return comparisonResult;
            }
        }

        public class SceneType
        {
            public List<uint> Scenes { get; set; }

            public static SceneType Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                SceneType obj = new SceneType();

                // Not sure what this is...
                uint unknown = Utils.readAndWriteUInt32(data, outputData, write);

                uint num_scenes = Utils.readAndWriteUInt32(data, outputData, write);
                obj.Scenes = new List<uint>();
                for (uint i = 0; i < num_scenes; i++)
                    obj.Scenes.Add(Utils.readAndWriteUInt32(data, outputData, write));

                return obj;
            }

            public bool Compare(SceneType other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (Scenes.Count != other.Scenes.Count)
                {
                    outputFile.WriteLine("\tScenes.Count is different: {0} {1}", Scenes.Count, other.Scenes.Count);
                    comparisonResult = false;
                }
                for (int i = 0; i < Scenes.Count && i < other.Scenes.Count; i++)
                {
                    if (Scenes[i] != other.Scenes[i])
                    {
                        outputFile.WriteLine("\tScenes[{0}] is different: {1} {2}", i, Scenes[i], other.Scenes[i]);
                        comparisonResult = false;
                    }
                }

                return comparisonResult;
            }
        }
        public class SceneDesc
        {
            public List<SceneType> SceneTypes { get; set; }

            public static SceneDesc Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                SceneDesc obj = new SceneDesc();

                uint num_scene_types = Utils.readAndWriteUInt32(data, outputData, write);
                obj.SceneTypes = new List<SceneType>();
                for (uint i = 0; i < num_scene_types; i++)
                    obj.SceneTypes.Add(SceneType.Read(data, outputData, isToD, write));

                return obj;
            }

            public bool Compare(SceneDesc other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (SceneTypes.Count != other.SceneTypes.Count)
                {
                    outputFile.WriteLine("\tSceneTypes.Count is different: {0} {1}", SceneTypes.Count, other.SceneTypes.Count);
                    comparisonResult = false;
                }
                for (int i = 0; i < SceneTypes.Count && i < other.SceneTypes.Count; i++)
                {
                    if (!SceneTypes[i].Compare(other.SceneTypes[i], outputFile))
                    {
                        outputFile.WriteLine("\tSceneTypes[{0}] is different: {1} {2}", i, SceneTypes[i], other.SceneTypes[i]);
                        comparisonResult = false;
                    }
                }

                return comparisonResult;
            }
        }
        public class AmbientSoundDesc
        {
            public uint SType { get; set; }
            public float Volume { get; set; }
            public float BaseChance { get; set; }
            public float MinRate { get; set; }
            public float MaxRate { get; set; }

            public static AmbientSoundDesc Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                AmbientSoundDesc obj = new AmbientSoundDesc();
                obj.SType = Utils.readAndWriteUInt32(data, outputData, write);
                obj.Volume = Utils.readAndWriteSingle(data, outputData, write);
                obj.BaseChance = Utils.readAndWriteSingle(data, outputData, write);
                obj.MinRate = Utils.readAndWriteSingle(data, outputData, write);
                obj.MaxRate = Utils.readAndWriteSingle(data, outputData, write);
                return obj;
            }

            public bool Compare(AmbientSoundDesc other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (SType != other.SType)
                {
                    outputFile.WriteLine("\tSType is different: {0} {1}", SType, other.SType);
                    comparisonResult = false;
                }
                if (Volume != other.Volume)
                {
                    outputFile.WriteLine("\tVolume is different: {0} {1}", Volume, other.Volume);
                    comparisonResult = false;
                }
                if (BaseChance != other.BaseChance)
                {
                    outputFile.WriteLine("\tBaseChance is different: {0} {1}", BaseChance, other.BaseChance);
                    comparisonResult = false;
                }
                if (MinRate != other.MinRate)
                {
                    outputFile.WriteLine("\tMinRate is different: {0} {1}", MinRate, other.MinRate);
                    comparisonResult = false;
                }
                if (MaxRate != other.MaxRate)
                {
                    outputFile.WriteLine("\tMaxRate is different: {0} {1}", MaxRate, other.MaxRate);
                    comparisonResult = false;
                }

                return comparisonResult;
            }
        }
        public class AmbientSTBDesc
        {
            public uint STBId { get; set; }
            public List<AmbientSoundDesc> AmbientSounds { get; set; }

            public static AmbientSTBDesc Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                AmbientSTBDesc obj = new AmbientSTBDesc();
                obj.STBId = Utils.readAndWriteUInt32(data, outputData, write);

                uint num_ambient_sounds = Utils.readAndWriteUInt32(data, outputData, write);
                obj.AmbientSounds = new List<AmbientSoundDesc>();
                for (uint i = 0; i < num_ambient_sounds; i++)
                    obj.AmbientSounds.Add(AmbientSoundDesc.Read(data, outputData, isToD, write));

                return obj;
            }

            public bool Compare(AmbientSTBDesc other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (STBId != other.STBId)
                {
                    outputFile.WriteLine("\tSTBId is different: {0} {1}", STBId, other.STBId);
                    comparisonResult = false;
                }

                if (AmbientSounds.Count != other.AmbientSounds.Count)
                {
                    outputFile.WriteLine("\tAmbientSounds.Count is different: {0} {1}", AmbientSounds.Count, other.AmbientSounds.Count);
                    comparisonResult = false;
                }
                for (int i = 0; i < AmbientSounds.Count && i < other.AmbientSounds.Count; i++)
                {
                    if (!AmbientSounds[i].Compare(other.AmbientSounds[i], outputFile))
                    {
                        outputFile.WriteLine("\tAmbientSounds[{0}] is different", i, AmbientSounds[i], other.AmbientSounds[i]);
                        comparisonResult = false;
                    }
                }

                return comparisonResult;
            }
        }
        public class SoundDesc
        {
            public List<AmbientSTBDesc> STBDesc { get; set; }

            public static SoundDesc Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                SoundDesc obj = new SoundDesc();

                uint num_stb_desc = Utils.readAndWriteUInt32(data, outputData, write);
                obj.STBDesc = new List<AmbientSTBDesc>();
                for (uint i = 0; i < num_stb_desc; i++)
                    obj.STBDesc.Add(AmbientSTBDesc.Read(data, outputData, isToD, write));

                return obj;
            }

            public bool Compare(SoundDesc other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (STBDesc.Count != other.STBDesc.Count)
                {
                    outputFile.WriteLine("\tSTBDesc is different: {0} {1}", STBDesc, other.STBDesc);
                    comparisonResult = false;
                }
                for (int i = 0; i < STBDesc.Count && i < other.STBDesc.Count; i++)
                {
                    if (!STBDesc[i].Compare(other.STBDesc[i],outputFile))
                    {
                        outputFile.WriteLine("\tSTBDesc[{0}] is different: {1} {2}", i, STBDesc[i], other.STBDesc[i]);
                        comparisonResult = false;
                    }
                }

                return comparisonResult;
            }
        }

        public class SkyObjectReplace
        {
            public uint ObjectIndex { get; set; }
            public uint GFXObjId { get; set; }
            public float Rotate { get; set; }
            public float Transparent { get; set; }
            public float Luminosity { get; set; }
            public float MaxBright { get; set; }

            public static SkyObjectReplace Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                SkyObjectReplace obj = new SkyObjectReplace();
                obj.ObjectIndex = Utils.readAndWriteUInt32(data, outputData, write);
                obj.GFXObjId = Utils.readAndWriteUInt32(data, outputData, write);
                obj.Rotate = Utils.readAndWriteSingle(data, outputData, write);
                obj.Transparent = Utils.readAndWriteSingle(data, outputData, write);
                obj.Luminosity = Utils.readAndWriteSingle(data, outputData, write);
                obj.MaxBright = Utils.readAndWriteSingle(data, outputData, write);
                return obj;
            }

            public bool Compare(SkyObjectReplace other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (ObjectIndex != other.ObjectIndex)
                {
                    outputFile.WriteLine("\tObjectIndex is different: {0} {1}", ObjectIndex, other.ObjectIndex);
                    comparisonResult = false;
                }

                if (GFXObjId != other.GFXObjId)
                {
                    outputFile.WriteLine("\tGFXObjId is different: {0} {1}", GFXObjId, other.GFXObjId);
                    comparisonResult = false;
                }

                if (Rotate != other.Rotate)
                {
                    outputFile.WriteLine("\tRotate is different: {0} {1}", Rotate, other.Rotate);
                    comparisonResult = false;
                }

                if (Transparent != other.Transparent)
                {
                    outputFile.WriteLine("\tTransparent is different: {0} {1}", Transparent, other.Transparent);
                    comparisonResult = false;
                }

                if (Luminosity != other.Luminosity)
                {
                    outputFile.WriteLine("\tLuminosity is different: {0} {1}", Luminosity, other.Luminosity);
                    comparisonResult = false;
                }

                if (MaxBright != other.MaxBright)
                {
                    outputFile.WriteLine("\tMaxBright is different: {0} {1}", MaxBright, other.MaxBright);
                    comparisonResult = false;
                }

                return comparisonResult;
            }
        }
        public class SkyTimeOfDay
        {
            public float Begin { get; set; }
            public float DirBright { get; set; }
            public float DirHeading { get; set; }
            public float DirPitch { get; set; }
            public uint DirColor { get; set; }

            public float AmbBright { get; set; }
            public uint AmbColor { get; set; }
            public float MinWorldFog { get; set; }
            public float MaxWorldFog { get; set; }
            public uint WorldFogColor { get; set; }
            public uint WorldFog { get; set; }

            public List<SkyObjectReplace> SkyObjReplace { get; set; }

            public static SkyTimeOfDay Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                SkyTimeOfDay obj = new SkyTimeOfDay();
                obj.Begin = Utils.readAndWriteSingle(data, outputData, write);
                obj.DirBright = Utils.readAndWriteSingle(data, outputData, write);
                obj.DirHeading = Utils.readAndWriteSingle(data, outputData, write);
                obj.DirPitch = Utils.readAndWriteSingle(data, outputData, write);
                obj.DirColor = Utils.readAndWriteUInt32(data, outputData, write);

                obj.AmbBright = Utils.readAndWriteSingle(data, outputData, write);
                obj.AmbColor = Utils.readAndWriteUInt32(data, outputData, write);

                obj.MinWorldFog = Utils.readAndWriteSingle(data, outputData, write);
                obj.MaxWorldFog = Utils.readAndWriteSingle(data, outputData, write);
                obj.WorldFogColor = Utils.readAndWriteUInt32(data, outputData, write);
                obj.WorldFog = Utils.readAndWriteUInt32(data, outputData, write);

                uint num_sky_obj_replace = Utils.readAndWriteUInt32(data, outputData, write);
                obj.SkyObjReplace = new List<SkyObjectReplace>();
                for (uint i = 0; i < num_sky_obj_replace; i++)
                    obj.SkyObjReplace.Add(SkyObjectReplace.Read(data, outputData, isToD, write));

                return obj;
            }

            public bool Compare(SkyTimeOfDay other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (Begin != other.Begin)
                {
                    outputFile.WriteLine("\tBegin is different: {0} {1}", Begin, other.Begin);
                    comparisonResult = false;
                }
                if (DirBright != other.DirBright)
                {
                    outputFile.WriteLine("\tDirBright is different: {0} {1}", DirBright, other.DirBright);
                    comparisonResult = false;
                }
                if (DirHeading != other.DirHeading)
                {
                    outputFile.WriteLine("\tDirHeading is different: {0} {1}", DirHeading, other.DirHeading);
                    comparisonResult = false;
                }
                if (DirPitch != other.DirPitch)
                {
                    outputFile.WriteLine("\tDirPitch is different: {0} {1}", DirPitch, other.DirPitch);
                    comparisonResult = false;
                }
                if (DirColor != other.DirColor)
                {
                    outputFile.WriteLine("\tDirColor is different: {0} {1}", DirColor, other.DirColor);
                    comparisonResult = false;
                }
                if (AmbBright != other.AmbBright)
                {
                    outputFile.WriteLine("\tAmbBright is different: {0} {1}", AmbBright, other.AmbBright);
                    comparisonResult = false;
                }
                if (AmbColor != other.AmbColor)
                {
                    outputFile.WriteLine("\tAmbColor is different: {0} {1}", AmbColor, other.AmbColor);
                    comparisonResult = false;
                }
                if (MinWorldFog != other.MinWorldFog)
                {
                    outputFile.WriteLine("\tMinWorldFog is different: {0} {1}", MinWorldFog, other.MinWorldFog);
                    comparisonResult = false;
                }
                if (MaxWorldFog != other.MaxWorldFog)
                {
                    outputFile.WriteLine("\tMaxWorldFog is different: {0} {1}", MaxWorldFog, other.MaxWorldFog);
                    comparisonResult = false;
                }
                if (WorldFogColor != other.WorldFogColor)
                {
                    outputFile.WriteLine("\tWorldFogColor is different: {0} {1}", WorldFogColor, other.WorldFogColor);
                    comparisonResult = false;
                }
                if (WorldFog != other.WorldFog)
                {
                    outputFile.WriteLine("\tWorldFog is different: {0} {1}", WorldFog, other.WorldFog);
                    comparisonResult = false;
                }

                if (SkyObjReplace.Count != other.SkyObjReplace.Count)
                {
                    outputFile.WriteLine("\tSkyObjReplace.Count is different: {0} {1}", SkyObjReplace.Count, other.SkyObjReplace.Count);
                    comparisonResult = false;
                }
                for (int i = 0; i < SkyObjReplace.Count && i < other.SkyObjReplace.Count; i++)
                {
                    if (!SkyObjReplace[i].Compare(other.SkyObjReplace[i], outputFile))
                    {
                        outputFile.WriteLine("\tSkyObjReplace[{0}] is different", i, SkyObjReplace[i], other.SkyObjReplace[i]);
                        comparisonResult = false;
                    }
                }
                return comparisonResult;
            }
        }
        public class SkyObject
        {
            public float BeginTime { get; set; }
            public float EndTime { get; set; }
            public float BeginAngle { get; set; }
            public float EndAngle { get; set; }
            public float TexVelocityX { get; set; }
            public float TexVelocityY { get; set; }
            public float TexVelocityZ { get; set; }
            public uint DefaultGFXObjectId { get; set; }
            public uint DefaultPESObjectId { get; set; }
            public uint Properties { get; set; }

            public static SkyObject Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                SkyObject obj = new SkyObject();
                obj.BeginTime = Utils.readAndWriteSingle(data, outputData, write);
                obj.EndTime = Utils.readAndWriteSingle(data, outputData, write);
                obj.BeginAngle = Utils.readAndWriteSingle(data, outputData, write);
                obj.EndAngle = Utils.readAndWriteSingle(data, outputData, write);
                obj.TexVelocityX = Utils.readAndWriteSingle(data, outputData, write);
                obj.TexVelocityY = Utils.readAndWriteSingle(data, outputData, write);
                obj.TexVelocityZ = 0;
                obj.DefaultGFXObjectId = Utils.readAndWriteUInt32(data, outputData, write);
                obj.DefaultPESObjectId = Utils.readAndWriteUInt32(data, outputData, write);
                if (isToD)
                    obj.Properties = obj.Properties = Utils.readAndWriteUInt32(data, outputData, write);
                else
                    obj.Properties = 0;
                return obj;
            }

            public bool Compare(SkyObject other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (BeginTime != other.BeginTime)
                {
                    outputFile.WriteLine("\tBeginTime is different: {0} {1}", BeginTime, other.BeginTime);
                    comparisonResult = false;
                }
                if (EndTime != other.EndTime)
                {
                    outputFile.WriteLine("\tEndTime is different: {0} {1}", EndTime, other.EndTime);
                    comparisonResult = false;
                }
                if (BeginAngle != other.BeginAngle)
                {
                    outputFile.WriteLine("\tBeginAngle is different: {0} {1}", BeginAngle, other.BeginAngle);
                    comparisonResult = false;
                }
                if (TexVelocityX != other.TexVelocityX)
                {
                    outputFile.WriteLine("\tTexVelocityX is different: {0} {1}", TexVelocityX, other.TexVelocityX);
                    comparisonResult = false;
                }
                if (TexVelocityY != other.TexVelocityY)
                {
                    outputFile.WriteLine("\tTexVelocityY is different: {0} {1}", TexVelocityY, other.TexVelocityY);
                    comparisonResult = false;
                }
                if (TexVelocityZ != other.TexVelocityZ)
                {
                    outputFile.WriteLine("\tTexVelocityZ is different: {0} {1}", TexVelocityZ, other.TexVelocityZ);
                    comparisonResult = false;
                }
                if (TexVelocityZ != other.TexVelocityZ)
                {
                    outputFile.WriteLine("\tTexVelocityZ is different: {0} {1}", TexVelocityZ, other.TexVelocityZ);
                    comparisonResult = false;
                }
                if (DefaultGFXObjectId != other.DefaultGFXObjectId)
                {
                    outputFile.WriteLine("\tDefaultGFXObjectId is different: {0} {1}", DefaultGFXObjectId.ToString("x8"), other.DefaultGFXObjectId.ToString("x8"));
                    comparisonResult = false;
                }
                if (DefaultPESObjectId != other.DefaultPESObjectId)
                {
                    outputFile.WriteLine("\tDefaultPESObjectId is different: {0} {1}", DefaultPESObjectId.ToString("x8"), other.DefaultPESObjectId.ToString("x8"));
                    comparisonResult = false;
                }
                if (Properties != other.Properties)
                {
                    outputFile.WriteLine("\tProperties is different: {0} {1}", Properties, other.Properties);
                    comparisonResult = false;
                }
                return comparisonResult;
            }
        }

        public class DayGroup
        {
            public float ChanceOfOccur { get; set; }
            public string DayName { get; set; }
            public List<SkyObject> SkyObjects { get; set; }
            public List<SkyTimeOfDay> SkyTime { get; set; }

            public static DayGroup Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                DayGroup obj = new DayGroup();
                obj.ChanceOfOccur = Utils.readAndWriteSingle(data, outputData, write);
                obj.DayName = Utils.readAndWriteString(data, outputData, write);

                uint num_sky_objects = Utils.readAndWriteUInt32(data, outputData, write);
                obj.SkyObjects = new List<SkyObject>();
                for (uint i = 0; i < num_sky_objects; i++)
                    obj.SkyObjects.Add(SkyObject.Read(data, outputData, isToD, write));

                uint num_sky_times = Utils.readAndWriteUInt32(data, outputData, write);
                obj.SkyTime = new List<SkyTimeOfDay>();
                for (uint i = 0; i < num_sky_times; i++)
                    obj.SkyTime.Add(SkyTimeOfDay.Read(data, outputData, isToD, write));

                return obj;
            }

            public bool Compare(DayGroup other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (ChanceOfOccur != other.ChanceOfOccur)
                {
                    outputFile.WriteLine("\tChanceOfOccur is different: {0} {1}", ChanceOfOccur, other.ChanceOfOccur);
                    comparisonResult = false;
                }

                if (DayName != other.DayName)
                {
                    outputFile.WriteLine("\tDayName is different: {0} {1}", DayName, other.DayName);
                    comparisonResult = false;
                }

                if (SkyObjects.Count != other.SkyObjects.Count)
                {
                    outputFile.WriteLine("\tSkyObjects.Count is different: {0} {1}", SkyObjects.Count, other.SkyObjects.Count);
                    comparisonResult = false;
                }
                for (int i = 0; i < SkyObjects.Count && i < other.SkyObjects.Count; i++)
                {
                    if (!SkyObjects[i].Compare(other.SkyObjects[i], outputFile))
                    {
                        outputFile.WriteLine("\tSkyObjects[{0}] is different", i, SkyObjects[i], other.SkyObjects[i]);
                        comparisonResult = false;
                    }
                }

                if (SkyTime.Count != other.SkyTime.Count)
                {
                    outputFile.WriteLine("\tSkyTime.Count is different: {0} {1}", SkyTime.Count, other.SkyTime.Count);
                    comparisonResult = false;
                }
                for (int i = 0; i < SkyTime.Count && i < other.SkyTime.Count; i++)
                {
                    if (!SkyTime[i].Compare(other.SkyTime[i], outputFile))
                    {
                        outputFile.WriteLine("\tSkyTime[{0}] is different", i, SkyTime[i], other.SkyTime[i]);
                        comparisonResult = false;
                    }
                }

                return comparisonResult;
            }
        }
        public class SkyDesc
        {
            public UInt64 TickSize { get; set; }
            public UInt64 LightTickSize { get; set; }
            public List<DayGroup> DayGroups { get; set; }

            public static SkyDesc Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                SkyDesc obj = new SkyDesc();
                obj.TickSize = Utils.readAndWriteUInt64(data, outputData, write);
                obj.LightTickSize = Utils.readAndWriteUInt64(data, outputData, write);

                uint numDayGroups = Utils.readAndWriteUInt32(data, outputData, write);
                obj.DayGroups = new List<DayGroup>();
                for (uint i = 0; i < numDayGroups; i++)
                    obj.DayGroups.Add(DayGroup.Read(data, outputData, isToD, write));

                return obj;
            }

            public bool Compare(SkyDesc other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (TickSize != other.TickSize)
                {
                    outputFile.WriteLine("\tTickSize is different: {0} {1}", TickSize, other.TickSize);
                    comparisonResult = false;
                }

                if (LightTickSize != other.LightTickSize)
                {
                    outputFile.WriteLine("\tLightTickSize is different: {0} {1}", LightTickSize, other.LightTickSize);
                    comparisonResult = false;
                }

                if (DayGroups.Count != other.DayGroups.Count)
                {
                    outputFile.WriteLine("\tDayGroups.Count is different: {0} {1}", DayGroups.Count, other.DayGroups.Count);
                    comparisonResult = false;
                }

                for (int i = 0; i < DayGroups.Count && i < other.DayGroups.Count; i++)
                {
                    if (!DayGroups[i].Compare(other.DayGroups[i], outputFile))
                    {
                        outputFile.WriteLine("\tDayGroups[{0}] is different", i, DayGroups[i], other.DayGroups[i]);
                        comparisonResult = false;
                    }
                }

                return comparisonResult;
            }
        }

        public class LandDefs
        {
            public List<float> LandHeightTable { get; set; }

            public static LandDefs Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                LandDefs obj = new LandDefs();
                obj.LandHeightTable = new List<float>();
                for (int i = 0; i < 256; i++)
                {
                    obj.LandHeightTable.Add(Utils.readAndWriteSingle(data, outputData, write));
                }
                return obj;
            }

            public bool Compare(LandDefs other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (LandHeightTable.Count != other.LandHeightTable.Count)
                {
                    outputFile.WriteLine("\tLandHeightTable.Count is different: {0} {1}", LandHeightTable.Count, other.LandHeightTable.Count);
                    comparisonResult = false;
                }

                for (int i = 0; i < LandHeightTable.Count && i < other.LandHeightTable.Count; i++)
                {
                    if (LandHeightTable[i] != other.LandHeightTable[i])
                    {
                        outputFile.WriteLine("\tLandHeightTable[{0}] is different", i, LandHeightTable[i], other.LandHeightTable[i]);
                        comparisonResult = false;
                    }
                }

                return comparisonResult;
            }
        }

        public class Season
        {
            public uint StartDate { get; set; }
            public string Name { get; set; }

            public static Season Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                Season obj = new Season();
                obj.StartDate = Utils.readAndWriteUInt32(data, outputData, write);
                obj.Name = Utils.readAndWriteString(data, outputData, write);
                return obj;
            }

            public bool Compare(Season other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (StartDate != other.StartDate)
                {
                    outputFile.WriteLine("\tStartDate is different: {0} {1}", StartDate, other.StartDate);
                    comparisonResult = false;
                }
                if (Name != other.Name)
                {
                    outputFile.WriteLine("\tStartDate is different: {0} {1}", StartDate, other.StartDate);
                    comparisonResult = false;
                }
                return comparisonResult;
            }
        }

        public class TimeOfDay
        {
            public uint Start { get; set; }
            public uint IsNight { get; set; }
            public string Name { get; set; }

            public static TimeOfDay Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                TimeOfDay obj = new TimeOfDay();
                obj.Start = Utils.readAndWriteUInt32(data, outputData, write);
                obj.IsNight = Utils.readAndWriteUInt32(data, outputData, write);
                obj.Name = Utils.readAndWriteString(data, outputData, write);
                return obj;
            }

            public bool Compare(TimeOfDay other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (Start != other.Start)
                {
                    outputFile.WriteLine("\tStart is different: {0} {1}", Start, other.Start);
                    comparisonResult = false;
                }
                if (IsNight != other.IsNight)
                {
                    outputFile.WriteLine("\tIsNight is different: {0} {1}", IsNight, other.IsNight);
                    comparisonResult = false;
                }
                if (Name != other.Name)
                {
                    outputFile.WriteLine("\tName is different: {0} {1}", Name, other.Name);
                    comparisonResult = false;
                }
                return comparisonResult;
            }
        }

        public class GameTime
        {
            public UInt64 ZeroTimeOfYear { get; set; }
            public uint ZeroYear { get; set; } // Year "0" is really "P.Y. 10" in the calendar.
            public uint DayLength { get; set; }
            public uint DaysPerYear { get; set; } // 360. Likely for easier math so each month is same length
            public string YearSpec { get; set; } // "P.Y."
            public List<TimeOfDay> TimesOfDay { get; set; }
            public List<string> DaysOfTheWeek { get; set; }
            public List<Season> Seasons { get; set; }

            public static GameTime Read(StreamReader data, StreamWriter outputData, bool isToD, bool write)
            {
                GameTime obj = new GameTime();
                obj.ZeroTimeOfYear = Utils.readAndWriteUInt64(data, outputData, write);
                obj.ZeroYear = Utils.readAndWriteUInt32(data, outputData, write);
                obj.DayLength = Utils.readAndWriteUInt32(data, outputData, write);
                obj.DaysPerYear = Utils.readAndWriteUInt32(data, outputData, write);
                obj.YearSpec = Utils.readAndWriteString(data, outputData, write);

                uint numTimesOfDay = Utils.readAndWriteUInt32(data, outputData, write);
                obj.TimesOfDay = new List<TimeOfDay>();
                for (uint i = 0; i < numTimesOfDay; i++)
                {
                    obj.TimesOfDay.Add(TimeOfDay.Read(data, outputData, isToD, write));
                }

                uint numDaysOfTheWeek = Utils.readAndWriteUInt32(data, outputData, write);
                obj.DaysOfTheWeek = new List<string>();
                for (uint i = 0; i < numDaysOfTheWeek; i++)
                {
                    obj.DaysOfTheWeek.Add(Utils.readAndWriteString(data, outputData, write));
                }

                uint numSeasons = Utils.readAndWriteUInt32(data, outputData, write);
                obj.Seasons = new List<Season>();
                for (uint i = 0; i < numSeasons; i++)
                {
                    obj.Seasons.Add(Season.Read(data, outputData, isToD, write));
                }

                return obj;
            }

            public bool Compare(GameTime other, StreamWriter outputFile)
            {
                if (this == null && other == null)
                    return true;
                else if (this == null || other == null)
                {
                    outputFile.WriteLine("\tError comparing, one of the values is NULL");
                    return false;
                }

                bool comparisonResult = true;

                if (ZeroTimeOfYear != other.ZeroTimeOfYear)
                {
                    outputFile.WriteLine("\tZeroTimeOfYear is different: {0} {1}", ZeroTimeOfYear, other.ZeroTimeOfYear);
                    comparisonResult = false;
                }
                if (ZeroYear != other.ZeroYear)
                {
                    outputFile.WriteLine("\tZeroYear is different: {0} {1}", ZeroYear, other.ZeroYear);
                    comparisonResult = false;
                }
                if (DayLength != other.DayLength)
                {
                    outputFile.WriteLine("\tDayLength is different: {0} {1}", DayLength, other.DayLength);
                    comparisonResult = false;
                }
                if (DaysPerYear != other.DaysPerYear)
                {
                    outputFile.WriteLine("\tDaysPerYear is different: {0} {1}", DaysPerYear, other.DaysPerYear);
                    comparisonResult = false;
                }
                if (YearSpec != other.YearSpec)
                {
                    outputFile.WriteLine("\tYearSpec is different: {0} {1}", YearSpec, other.YearSpec);
                    comparisonResult = false;
                }

                if (TimesOfDay.Count != other.TimesOfDay.Count)
                {
                    outputFile.WriteLine("\tTimesOfDay.Count is different: {0} {1}", TimesOfDay.Count, other.TimesOfDay.Count);
                    comparisonResult = false;
                }
                for (int i = 0; i < TimesOfDay.Count && i < other.TimesOfDay.Count; i++)
                {
                    if (!TimesOfDay[i].Compare(other.TimesOfDay[i], outputFile))
                    {
                        outputFile.WriteLine("\tTimesOfDay[{0}] is different", i, TimesOfDay[i], other.TimesOfDay[i]);
                        comparisonResult = false;
                    }
                }

                if (DaysOfTheWeek.Count != other.DaysOfTheWeek.Count)
                {
                    outputFile.WriteLine("\tDaysOfTheWeek.Count is different: {0} {1}", DaysOfTheWeek.Count, other.DaysOfTheWeek.Count);
                    comparisonResult = false;
                }
                for (int i = 0; i < DaysOfTheWeek.Count && i < other.DaysOfTheWeek.Count; i++)
                {
                    if (DaysOfTheWeek[i] != other.DaysOfTheWeek[i])
                    {
                        outputFile.WriteLine("\tDaysOfTheWeek[{0}] is different: {1} {2}", i, DaysOfTheWeek[i], other.DaysOfTheWeek[i]);
                        comparisonResult = false;
                    }
                }

                if (Seasons.Count != other.Seasons.Count)
                {
                    outputFile.WriteLine("\tSeasons.Count is different: {0} {1}", Seasons.Count, other.Seasons.Count);
                    comparisonResult = false;
                }
                for (int i = 0; i < Seasons.Count && i < other.Seasons.Count; i++)
                {
                    if (!Seasons[i].Compare(other.Seasons[i], outputFile))
                    {
                        outputFile.WriteLine("\tSeasons[{0}] is different", i, Seasons[i], other.Seasons[i]);
                        comparisonResult = false;
                    }
                }

                return comparisonResult;
            }
        }

        //In DM the 130F0000 is the equivalent of ToD 13000000 and the DM's 13000000 is possibly used for software rendering
        static public void compare(string filename, bool isToD, string filename2, bool isToD2)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename);
                return;
            }
            StreamReader inputFile2 = new StreamReader(new FileStream(filename2, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename2);
                return;
            }

            StreamWriter outputFile = null;
            //StreamWriter outputFile = new StreamWriter(new FileStream("./region/130F0000 - Comparison Result - DM.bin", FileMode.Create, FileAccess.Write));
            //if (outputFile == null)
            //{
            //    Console.WriteLine("Unable to open 130F0000 - World Info - Winter.bin");
            //    return;
            //}

            Console.WriteLine("Comparing {0} and {1}", filename, filename2);

            byte[] buffer = new byte[1024];

            uint fileHeader;
            uint loaded;
            uint timeStamp;
            string regionName;
            uint partsMask;
            uint unknown1;
            uint unknown2;
            uint unknown3;
            uint unknown4;
            uint unknown5;
            uint unknown6;
            uint unknown7;

            LandDefs landDef = null;
            GameTime gameTime = null;
            uint next;

            SkyDesc skyInfo = null;
            SoundDesc soundInfo = null;
            SceneDesc sceneInfo = null;
            TerrainDesc terrainInfo = null;
            RegionMisc regionMisc = null;

            uint file2fileHeader;
            uint file2loaded;
            uint file2timeStamp;
            string file2regionName;
            uint file2partsMask;
            uint file2unknown1;
            uint file2unknown2;
            uint file2unknown3;
            uint file2unknown4;
            uint file2unknown5;
            uint file2unknown6;
            uint file2unknown7;

            LandDefs file2landDef = null;
            GameTime file2gameTime = null;
            uint file2next;

            SkyDesc file2skyInfo = null;
            SoundDesc file2soundInfo = null;
            SceneDesc file2sceneInfo = null;
            TerrainDesc file2terrainInfo = null;
            RegionMisc file2regionMisc = null;

            fileHeader = Utils.readAndWriteUInt32(inputFile, outputFile, false);
            if (fileHeader != 0x13000000 && fileHeader != 0x130F0000)
            {
                Console.WriteLine("Invalid header, aborting.");
                return;
            }

            loaded = Utils.readAndWriteUInt32(inputFile, outputFile, false);
            timeStamp = Utils.readAndWriteUInt32(inputFile, outputFile, false);
            regionName = Utils.readAndWriteString(inputFile, outputFile, false);
            partsMask = Utils.readAndWriteUInt32(inputFile, outputFile, false);

            unknown1 = Utils.readAndWriteUInt32(inputFile, outputFile, false);
            unknown2 = Utils.readAndWriteUInt32(inputFile, outputFile, false);
            unknown3 = Utils.readAndWriteUInt32(inputFile, outputFile, false);
            unknown4 = Utils.readAndWriteUInt32(inputFile, outputFile, false);
            unknown5 = Utils.readAndWriteUInt32(inputFile, outputFile, false);
            unknown6 = Utils.readAndWriteUInt32(inputFile, outputFile, false);
            unknown7 = Utils.readAndWriteUInt32(inputFile, outputFile, false);

            landDef = LandDefs.Read(inputFile, outputFile, isToD, false);
            gameTime = GameTime.Read(inputFile, outputFile, isToD, false);

            next = Utils.readAndWriteUInt32(inputFile, outputFile, false);

            if ((next & 0x10) > 0)
                skyInfo = SkyDesc.Read(inputFile, outputFile, isToD, false);

            if ((next & 0x01) > 0)
                soundInfo = SoundDesc.Read(inputFile, outputFile, isToD, false);

            if ((next & 0x02) > 0)
                sceneInfo = SceneDesc.Read(inputFile, outputFile, isToD, false);

            terrainInfo = TerrainDesc.Read(inputFile, outputFile, isToD, false);

            if ((next & 0x0200) > 0)
                regionMisc = RegionMisc.Read(inputFile, outputFile, isToD, false);





            file2fileHeader = Utils.readAndWriteUInt32(inputFile2, outputFile, false);
            if (file2fileHeader != 0x13000000 && file2fileHeader != 0x130F0000)
            {
                Console.WriteLine("Invalid header, aborting.");
                return;
            }

            file2loaded = Utils.readAndWriteUInt32(inputFile2, outputFile, false);
            file2timeStamp = Utils.readAndWriteUInt32(inputFile2, outputFile, false);
            file2regionName = Utils.readAndWriteString(inputFile2, outputFile, false);
            file2partsMask = Utils.readAndWriteUInt32(inputFile2, outputFile, false);

            file2unknown1 = Utils.readAndWriteUInt32(inputFile2, outputFile, false);
            file2unknown2 = Utils.readAndWriteUInt32(inputFile2, outputFile, false);
            file2unknown3 = Utils.readAndWriteUInt32(inputFile2, outputFile, false);
            file2unknown4 = Utils.readAndWriteUInt32(inputFile2, outputFile, false);
            file2unknown5 = Utils.readAndWriteUInt32(inputFile2, outputFile, false);
            file2unknown6 = Utils.readAndWriteUInt32(inputFile2, outputFile, false);
            file2unknown7 = Utils.readAndWriteUInt32(inputFile2, outputFile, false);

            file2landDef = LandDefs.Read(inputFile2, outputFile, isToD2, false);
            file2gameTime = GameTime.Read(inputFile2, outputFile, isToD2, false);

            file2next = Utils.readAndWriteUInt32(inputFile2, outputFile, false);

            if ((file2next & 0x10) > 0)
                file2skyInfo = SkyDesc.Read(inputFile2, outputFile, isToD2, false);

            if ((file2next & 0x01) > 0)
                file2soundInfo = SoundDesc.Read(inputFile2, outputFile, isToD2, false);

            if ((file2next & 0x02) > 0)
                file2sceneInfo = SceneDesc.Read(inputFile2, outputFile, isToD2, false);

            file2terrainInfo = TerrainDesc.Read(inputFile2, outputFile, isToD2, false);

            if ((file2next & 0x0200) > 0)
                file2regionMisc = RegionMisc.Read(inputFile2, outputFile, isToD2, false);

            inputFile.Close();
            inputFile2.Close();

            StreamWriter outputFile2 = new StreamWriter(new FileStream("./130F0000 - File Comparison.txt", FileMode.Create, FileAccess.Write));
            if (outputFile2 == null)
            {
                Console.WriteLine("130F0000 - File Comparison.txt");
                return;
            }

            outputFile2.WriteLine("Comparing {0} and {1}", filename, filename2);

            if (fileHeader != file2fileHeader)
                outputFile2.WriteLine("FileHeader is different: {0} {1}", fileHeader, file2fileHeader);
            if (loaded != file2loaded)
                outputFile2.WriteLine("loaded is different: {0} {1}",loaded,file2loaded);
            if (timeStamp != file2timeStamp)
                outputFile2.WriteLine("timeStamp is different: {0} {1}",timeStamp,file2timeStamp);
            if (regionName != file2regionName)
                outputFile2.WriteLine("regionName is different: {0} {1}",regionName,file2regionName);
            if (partsMask != file2partsMask)
                outputFile2.WriteLine("partsMask is different: {0} {1}",partsMask,file2partsMask);
            if (unknown1 != file2unknown1)
                outputFile2.WriteLine("unknown1 is different: {0} {1}",unknown1,file2unknown1);
            if (unknown2 != file2unknown2)
                outputFile2.WriteLine("unknown2 is different: {0} {1}",unknown2,file2unknown2);
            if (unknown3 != file2unknown3)
                outputFile2.WriteLine("unknown3 is different: {0} {1}",unknown3,file2unknown3);
            if (unknown4 != file2unknown4)
                outputFile2.WriteLine("unknown4 is different: {0} {1}",unknown4,file2unknown4);
            if (unknown5 != file2unknown5)
                outputFile2.WriteLine("unknown5 is different: {0} {1}",unknown5,file2unknown5);
            if (unknown6 != file2unknown6)
                outputFile2.WriteLine("unknown6 is different: {0} {1}",unknown6,file2unknown6);
            if (unknown7 != file2unknown7)
                outputFile2.WriteLine("unknown7 is different: {0} {1}", unknown7, file2unknown7);

            if (!landDef.Compare(file2landDef, outputFile2))
                outputFile2.WriteLine("landDef is different");
            //if (!gameTime.Compare(file2gameTime, outputFile2))
            //    outputFile2.WriteLine("gameTime is different");
            if (next != file2next)
                outputFile2.WriteLine("next is different");

            if (!skyInfo.Compare(file2skyInfo, outputFile2))
                outputFile2.WriteLine("skyInfo is different");
            if (!soundInfo.Compare(file2soundInfo, outputFile2))
                outputFile2.WriteLine("soundInfo is different");
            if (!sceneInfo.Compare(file2sceneInfo, outputFile2))
                outputFile2.WriteLine("sceneInfo is different");
            if (!terrainInfo.Compare(file2terrainInfo, outputFile2))
                outputFile2.WriteLine("terrainInfo is different");
            if (!regionMisc.Compare(file2regionMisc, outputFile2))
                outputFile2.WriteLine("regionMisc is different");

            outputFile2.Flush();
            outputFile2.Close();

            //outputFile.Flush();
            //outputFile.Close();
            Console.WriteLine("Done");
        }
    }
}