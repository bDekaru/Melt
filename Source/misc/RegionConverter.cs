using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Melt
{
    class RegionConverter
    {
        public class RegionMisc
        {
            public uint Version { get; set; }
            public uint GameMapID { get; set; }
            public uint AutotestMapId { get; set; }
            public uint AutotestMapSize { get; set; }
            public uint ClearCellId { get; set; }
            public uint ClearMonsterId { get; set; }

            public static RegionMisc Read(StreamReader data, StreamWriter outputData, bool write = true)
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

            public static TerrainTex Read(StreamReader data, StreamWriter outputData, bool write = true)
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
        }
        public class TMTerrainDesc
        {
            public uint terrainType { get; set; }
            public TerrainTex terrainTex { get; set; }

            public static TMTerrainDesc Read(StreamReader data, StreamWriter outputData, bool write = true)
            {
                TMTerrainDesc obj = new TMTerrainDesc();

                obj.terrainType = Utils.readAndWriteUInt32(data, outputData, write);
                obj.terrainTex = TerrainTex.Read(data, outputData, write);

                return obj;
            }

            public void Write(StreamWriter outputData)
            {
                outputData.BaseStream.Write(BitConverter.GetBytes(terrainType), 0, 4);
                terrainTex.Write(outputData);
            }
        }

        public class RoadAlphaMap
        {
            public uint RCode { get; set; }
            public uint RoadTexGID { get; set; }

            public static RoadAlphaMap Read(StreamReader data, StreamWriter outputData, bool write = true)
            {
                RoadAlphaMap obj = new RoadAlphaMap();
                obj.RCode = Utils.readAndWriteUInt32(data, outputData, write);
                obj.RoadTexGID = Utils.readAndWriteUInt32(data, outputData, write);
                return obj;
            }
        }

        public class TerrainAlphaMap
        {
            public uint TCode { get; set; }
            public uint TexGID { get; set; }

            public static TerrainAlphaMap Read(StreamReader data, StreamWriter outputData, bool write = true)
            {
                TerrainAlphaMap obj = new TerrainAlphaMap();
                obj.TCode = Utils.readAndWriteUInt32(data, outputData, write);
                obj.TexGID = Utils.readAndWriteUInt32(data, outputData, write);
                return obj;
            }
        }

        public class TexMerge
        {
            public uint BaseTexSize { get; set; }
            public List<TerrainAlphaMap> CornerTerrainMaps { get; set; }
            public List<TerrainAlphaMap> SideTerrainMaps { get; set; }
            public List<RoadAlphaMap> RoadMaps { get; set; }
            public List<TMTerrainDesc> TerrainDescription { get; set; }

            public static TexMerge Read(StreamReader data, StreamWriter outputData, bool write = true)
            {
                TexMerge obj = new TexMerge();

                obj.CornerTerrainMaps = new List<TerrainAlphaMap>();
                obj.SideTerrainMaps = new List<TerrainAlphaMap>();
                obj.RoadMaps = new List<RoadAlphaMap>();
                obj.TerrainDescription = new List<TMTerrainDesc>();

                //obj.BaseTexSize = Utils.readAndWriteUInt32(data, outputData, write);
                obj.BaseTexSize = Utils.readAndWriteUInt32(data, outputData, false);
                outputData.BaseStream.Write(BitConverter.GetBytes((uint)256), 0, 4);

                uint num_corner_terrain_maps = Utils.readAndWriteUInt32(data, outputData, write);
                for (uint i = 0; i < num_corner_terrain_maps; i++)
                    obj.CornerTerrainMaps.Add(TerrainAlphaMap.Read(data, outputData, write));

                uint num_side_terrain_maps = Utils.readAndWriteUInt32(data, outputData, write);
                for (uint i = 0; i < num_side_terrain_maps; i++)
                    obj.SideTerrainMaps.Add(TerrainAlphaMap.Read(data, outputData, write));

                uint num_road_maps = Utils.readAndWriteUInt32(data, outputData, write);
                for (uint i = 0; i < num_road_maps; i++)
                    obj.RoadMaps.Add(RoadAlphaMap.Read(data, outputData, write));

                uint num_terrain_desc = Utils.readAndWriteUInt32(data, outputData, write);

                for (uint i = 0; i < num_terrain_desc; i++)
                    obj.TerrainDescription.Add(TMTerrainDesc.Read(data, outputData, write));

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
        }

        public class LandSurf
        {
            public uint HasPalShift { get; set; }
            public TexMerge texMerge { get; set; }

            public static LandSurf Read(StreamReader data, StreamWriter outputData, bool write = true)
            {
                LandSurf obj = new LandSurf();

                obj.HasPalShift = Utils.readAndWriteUInt32(data, outputData, write); // This is always 0

                if (obj.HasPalShift == 1)
                {
                    // PalShift.Read would go here, if it ever actually existed...which it doesn't.
                }
                else
                    obj.texMerge = TexMerge.Read(data, outputData, write);

                return obj;
            }
        }

        public class TerrainType
        {
            public string TerrainName { get; set; }
            public uint TerrainColor { get; set; }
            public List<uint> SceneTypes { get; set; }

            public static TerrainType Read(StreamReader data, StreamWriter outputData, bool write = true)
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
        }

        public class TerrainDesc
        {
            public List<TerrainType> TerrainTypes { get; set; }
            public LandSurf LandSurfaces { get; set; }

            public static TerrainDesc Read(StreamReader data, StreamWriter outputData, bool write = true)
            {
                TerrainDesc obj = new TerrainDesc();

                uint num_terrain_types = Utils.readAndWriteUInt32(data, outputData, write);

                obj.TerrainTypes = new List<TerrainType>();

                for (uint i = 0; i < num_terrain_types; i++)
                    obj.TerrainTypes.Add(TerrainType.Read(data, outputData, write));

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

                obj.LandSurfaces = LandSurf.Read(data, outputData, write);

                return obj;
            }
        }

        public class SceneType
        {
            public List<uint> Scenes { get; set; }

            public static SceneType Read(StreamReader data, StreamWriter outputData, bool write = true)
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
        }
        public class SceneDesc
        {
            public List<SceneType> SceneTypes { get; set; }

            public static SceneDesc Read(StreamReader data, StreamWriter outputData, bool write = true)
            {
                SceneDesc obj = new SceneDesc();

                uint num_scene_types = Utils.readAndWriteUInt32(data, outputData, write);
                obj.SceneTypes = new List<SceneType>();
                for (uint i = 0; i < num_scene_types; i++)
                    obj.SceneTypes.Add(SceneType.Read(data, outputData, write));

                return obj;
            }
        }
        public class AmbientSoundDesc
        {
            public uint SType { get; set; }
            public float Volume { get; set; }
            public float BaseChance { get; set; }
            public float MinRate { get; set; }
            public float MaxRate { get; set; }

            public static AmbientSoundDesc Read(StreamReader data, StreamWriter outputData, bool write = true)
            {
                AmbientSoundDesc obj = new AmbientSoundDesc();
                obj.SType = Utils.readAndWriteUInt32(data, outputData, write);
                obj.Volume = Utils.readAndWriteSingle(data, outputData, write);
                obj.BaseChance = Utils.readAndWriteSingle(data, outputData, write);
                obj.MinRate = Utils.readAndWriteSingle(data, outputData, write);
                obj.MaxRate = Utils.readAndWriteSingle(data, outputData, write);
                return obj;
            }
        }
        public class AmbientSTBDesc
        {
            public uint STBId { get; set; }
            public List<AmbientSoundDesc> AmbientSounds { get; set; }

            public static AmbientSTBDesc Read(StreamReader data, StreamWriter outputData, bool write = true)
            {
                AmbientSTBDesc obj = new AmbientSTBDesc();
                obj.STBId = Utils.readAndWriteUInt32(data, outputData, write);

                uint num_ambient_sounds = Utils.readAndWriteUInt32(data, outputData, write);
                obj.AmbientSounds = new List<AmbientSoundDesc>();
                for (uint i = 0; i < num_ambient_sounds; i++)
                    obj.AmbientSounds.Add(AmbientSoundDesc.Read(data, outputData, write));

                return obj;
            }
        }
        public class SoundDesc
        {
            public List<AmbientSTBDesc> STBDesc { get; set; }

            public static SoundDesc Read(StreamReader data, StreamWriter outputData, bool write = true)
            {
                SoundDesc obj = new SoundDesc();

                uint num_stb_desc = Utils.readAndWriteUInt32(data, outputData, write);
                obj.STBDesc = new List<AmbientSTBDesc>();
                for (uint i = 0; i < num_stb_desc; i++)
                    obj.STBDesc.Add(AmbientSTBDesc.Read(data, outputData, write));

                return obj;
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

            public static SkyObjectReplace Read(StreamReader data, StreamWriter outputData, bool write = true)
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

            public static SkyTimeOfDay Read(StreamReader data, StreamWriter outputData, bool write = true)
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
                    obj.SkyObjReplace.Add(SkyObjectReplace.Read(data, outputData, write));

                return obj;
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

            public static SkyObject Read(StreamReader data, StreamWriter outputData, bool write = true)
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
                obj.Properties = Utils.readAndWriteUInt32(data, outputData, write);
                return obj;
            }
        }

        public class DayGroup
        {
            public float ChanceOfOccur { get; set; }
            public string DayName { get; set; }
            public List<SkyObject> SkyObjects { get; set; }
            public List<SkyTimeOfDay> SkyTime { get; set; }

            public static DayGroup Read(StreamReader data, StreamWriter outputData, bool write = true)
            {
                DayGroup obj = new DayGroup();
                obj.ChanceOfOccur = Utils.readAndWriteSingle(data, outputData, write);
                obj.DayName = Utils.readAndWriteString(data, outputData, write);

                uint num_sky_objects = Utils.readAndWriteUInt32(data, outputData, write);
                obj.SkyObjects = new List<SkyObject>();
                for (uint i = 0; i < num_sky_objects; i++)
                    obj.SkyObjects.Add(SkyObject.Read(data, outputData, write));

                uint num_sky_times = Utils.readAndWriteUInt32(data, outputData, write);
                obj.SkyTime = new List<SkyTimeOfDay>();
                for (uint i = 0; i < num_sky_times; i++)
                    obj.SkyTime.Add(SkyTimeOfDay.Read(data, outputData, write));

                return obj;
            }
        }
        public class SkyDesc
        {
            public UInt64 TickSize { get; set; }
            public UInt64 LightTickSize { get; set; }
            public List<DayGroup> DayGroups { get; set; }

            public static SkyDesc Read(StreamReader data, StreamWriter outputData, bool write = true)
            {
                SkyDesc obj = new SkyDesc();
                obj.TickSize = Utils.readAndWriteUInt64(data, outputData, write);
                obj.LightTickSize = Utils.readAndWriteUInt64(data, outputData, write);

                uint numDayGroups = Utils.readAndWriteUInt32(data, outputData, write);
                obj.DayGroups = new List<DayGroup>();
                for (uint i = 0; i < numDayGroups; i++)
                    obj.DayGroups.Add(DayGroup.Read(data, outputData, write));

                return obj;
            }
        }

        public class LandDefs
        {
            public List<float> LandHeightTable { get; set; }

            public static LandDefs Read(StreamReader data, StreamWriter outputData, bool write = true)
            {
                LandDefs obj = new LandDefs();
                obj.LandHeightTable = new List<float>();
                for (int i = 0; i < 256; i++)
                {
                    obj.LandHeightTable.Add(Utils.readAndWriteSingle(data, outputData, write));
                }
                return obj;
            }
        }

        public class Season
        {
            public uint StartDate { get; set; }
            public string Name { get; set; }

            public static Season Read(StreamReader data, StreamWriter outputData, bool write = true)
            {
                Season obj = new Season();
                obj.StartDate = Utils.readAndWriteUInt32(data, outputData, write);
                obj.Name = Utils.readAndWriteString(data, outputData, write);
                return obj;
            }
        }

        public class TimeOfDay
        {
            public uint Start { get; set; }
            public uint IsNight { get; set; }
            public string Name { get; set; }

            public static TimeOfDay Read(StreamReader data, StreamWriter outputData, bool write = true)
            {
                TimeOfDay obj = new TimeOfDay();
                obj.Start = Utils.readAndWriteUInt32(data, outputData, write);
                obj.IsNight = Utils.readAndWriteUInt32(data, outputData, write);
                obj.Name = Utils.readAndWriteString(data, outputData, write);
                return obj;
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

            public static GameTime Read(StreamReader data, StreamWriter outputData, bool write = true)
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
                    obj.TimesOfDay.Add(TimeOfDay.Read(data, outputData, write));
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
                    obj.Seasons.Add(Season.Read(data, outputData, write));
                }

                return obj;
            }
        }

        static public void convert(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename);
                return;
            }
            StreamWriter outputFile = new StreamWriter(new FileStream("./Region/13000000 - World Info - Winter.bin", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open 13000000 - World Info - Winter.bin");
                return;
            }

            Console.WriteLine("Converting region file to winter...");

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

            LandDefs landDef;
            GameTime gameTime;
            uint next;

            SkyDesc skyInfo;
            SoundDesc soundInfo;
            SceneDesc sceneInfo;
            TerrainDesc terrainInfo;
            RegionMisc regionMisc;

            fileHeader = Utils.readAndWriteUInt32(inputFile, outputFile);
            if (fileHeader != 0x13000000)
            {
                Console.WriteLine("Invalid header, aborting.");
                return;
            }

            loaded = Utils.readAndWriteUInt32(inputFile, outputFile);
            timeStamp = Utils.readAndWriteUInt32(inputFile, outputFile);
            regionName = Utils.readAndWriteString(inputFile, outputFile);
            partsMask = Utils.readAndWriteUInt32(inputFile, outputFile);

            unknown1 = Utils.readAndWriteUInt32(inputFile, outputFile);
            unknown2 = Utils.readAndWriteUInt32(inputFile, outputFile);
            unknown3 = Utils.readAndWriteUInt32(inputFile, outputFile);
            unknown4 = Utils.readAndWriteUInt32(inputFile, outputFile);
            unknown5 = Utils.readAndWriteUInt32(inputFile, outputFile);
            unknown6 = Utils.readAndWriteUInt32(inputFile, outputFile);
            unknown7 = Utils.readAndWriteUInt32(inputFile, outputFile);

            landDef = LandDefs.Read(inputFile, outputFile);
            gameTime = GameTime.Read(inputFile, outputFile);

            next = Utils.readAndWriteUInt32(inputFile, outputFile);

            if ((next & 0x10) > 0)
                skyInfo = SkyDesc.Read(inputFile, outputFile);

            if ((next & 0x01) > 0)
                soundInfo = SoundDesc.Read(inputFile, outputFile);

            if ((next & 0x02) > 0)
                sceneInfo = SceneDesc.Read(inputFile, outputFile);

            terrainInfo = TerrainDesc.Read(inputFile, outputFile);

            if ((next & 0x0200) > 0)
                regionMisc = RegionMisc.Read(inputFile, outputFile);

            //StreamWriter outputFile2 = new StreamWriter(new FileStream("./13000000 - Terrain Textures -ToD.txt", FileMode.Create, FileAccess.Write));
            //if (outputFile2 == null)
            //{
            //    Console.WriteLine("Unable to open 13000000 - Terrain Textures - ToD.txt");
            //    return;
            //}

            //for (int i = 0; i < terrainInfo.LandSurfaces.texMerge.CornerTerrainMaps.Count; i++)
            //{
            //    TerrainAlphaMap cornerTerrainMap = terrainInfo.LandSurfaces.texMerge.CornerTerrainMaps[i];

            //    outputFile2.WriteLine("{0}", cornerTerrainMap.TexGID.ToString("x8"));
            //    outputFile2.Flush();
            //}

            //for (int i = 0; i < terrainInfo.LandSurfaces.texMerge.SideTerrainMaps.Count; i++)
            //{
            //    TerrainAlphaMap sideTerrainMap = terrainInfo.LandSurfaces.texMerge.SideTerrainMaps[i];

            //    outputFile2.WriteLine("{0}", sideTerrainMap.TexGID.ToString("x8"));
            //    outputFile2.Flush();
            //}

            //for (int i = 0; i < terrainInfo.LandSurfaces.texMerge.RoadMaps.Count; i++)
            //{
            //    RoadAlphaMap roadMap = terrainInfo.LandSurfaces.texMerge.RoadMaps[i];

            //    outputFile2.WriteLine("{0}", roadMap.RoadTexGID.ToString("x8"));
            //    outputFile2.Flush();
            //}

            //for (int i = 0; i < terrainInfo.LandSurfaces.texMerge.TerrainDescription.Count; i++)//ignore first entry as it's a repeat of the second
            //{

            //    TMTerrainDesc desc = terrainInfo.LandSurfaces.texMerge.TerrainDescription[i];
            //    string terrainName = "Unknown";
            //    uint terrainColor = 0;
            //    if (i < terrainInfo.TerrainTypes.Count)
            //    {
            //        terrainName = terrainInfo.TerrainTypes[(int)desc.terrainType].TerrainName;
            //        terrainColor = terrainInfo.TerrainTypes[(int)desc.terrainType].TerrainColor;
            //    }
            //    else if (i == 32)
            //    {
            //        terrainName = "Road";
            //        terrainColor = 0;
            //    }

            //    terrainName = terrainName.PadLeft(20);

            //    outputFile2.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}", desc.terrainType, terrainName, desc.terrainTex.TexGID.ToString("x8"), desc.terrainTex.TexTiling, desc.terrainTex.DetailTexGID.ToString("x8"),
            //        desc.terrainTex.DetailTexTiling, desc.terrainTex.MaxVertBright, desc.terrainTex.MaxVertHue, desc.terrainTex.MaxVertSaturate, desc.terrainTex.MinVertBright,
            //        desc.terrainTex.MinVertHue, desc.terrainTex.MinVertSaturate);

            //    //outputFile2.WriteLine("TextureConverter.toBin(\"Landscape Texture Conversion/DM/Textures/Upscaled/xxx.png\", 0x{0}, 21);//{1}",desc.terrainTex.TexGID.ToString("x8"),terrainName);

            //    //outputFile2.WriteLine("writedat client_portal.dat -f {0}={0}.bin", desc.terrainTex.TexGID.ToString("x8"));
            //    outputFile2.Flush();
            //}
            //outputFile2.Close();

            inputFile.Close();

            outputFile.Flush();
            outputFile.Close();
            Console.WriteLine("Done");
        }
    }
}