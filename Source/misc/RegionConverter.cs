using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ACE.DatLoader.FileTypes;
using ACE.DatLoader;
using ACE.DatLoader.Entity;

namespace Melt
{
    class RegionManipulation
    {
        public class RegionMisc
        {
            public uint Version { get; set; }
            public uint GameMapID { get; set; }
            public uint AutotestMapId { get; set; }
            public uint AutotestMapSize { get; set; }
            public uint ClearCellId { get; set; }
            public uint ClearMonsterId { get; set; }

            public static RegionMisc Read(StreamReader data)
            {
                RegionMisc obj = new RegionMisc();

                obj.Version = Utils.readUInt32(data);
                obj.GameMapID = Utils.readUInt32(data);
                obj.AutotestMapId = Utils.readUInt32(data);
                obj.AutotestMapSize = Utils.readUInt32(data);
                obj.ClearCellId = Utils.readUInt32(data);
                obj.ClearMonsterId = Utils.readUInt32(data);

                return obj;
            }

            public void Write(StreamWriter output)
            {
                Utils.writeUInt32(Version, output);
                Utils.writeUInt32(GameMapID, output);
                Utils.writeUInt32(AutotestMapId, output);
                Utils.writeUInt32(AutotestMapSize, output);
                Utils.writeUInt32(ClearCellId, output);
                Utils.writeUInt32(ClearMonsterId, output);
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

            public static TerrainTex Read(StreamReader data)
            {
                TerrainTex obj = new TerrainTex();

                obj.TexGID = Utils.readUInt32(data);
                obj.TexTiling = Utils.readUInt32(data);
                obj.MaxVertBright = Utils.readUInt32(data);
                obj.MinVertBright = Utils.readUInt32(data);
                obj.MaxVertSaturate = Utils.readUInt32(data);
                obj.MinVertSaturate = Utils.readUInt32(data);
                obj.MaxVertHue = Utils.readUInt32(data);
                obj.MinVertHue = Utils.readUInt32(data);
                obj.DetailTexTiling = Utils.readUInt32(data);
                obj.DetailTexGID = Utils.readUInt32(data);

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

            public static TMTerrainDesc Read(StreamReader data)
            {
                TMTerrainDesc obj = new TMTerrainDesc();

                obj.terrainType = Utils.readUInt32(data);
                obj.terrainTex = TerrainTex.Read(data);

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

            public static RoadAlphaMap Read(StreamReader data)
            {
                RoadAlphaMap obj = new RoadAlphaMap();
                obj.RCode = Utils.readUInt32(data);
                obj.RoadTexGID = Utils.readUInt32(data);
                return obj;
            }

            public void Write(StreamWriter output)
            {
                Utils.writeUInt32(RCode, output);
                Utils.writeUInt32(RoadTexGID, output);
            }
        }

        public class TerrainAlphaMap
        {
            public uint TCode { get; set; }
            public uint TexGID { get; set; }

            public static TerrainAlphaMap Read(StreamReader data)
            {
                TerrainAlphaMap obj = new TerrainAlphaMap();
                obj.TCode = Utils.readUInt32(data);
                obj.TexGID = Utils.readUInt32(data);
                return obj;
            }

            public void Write(StreamWriter output)
            {
                Utils.writeUInt32(TCode, output);
                Utils.writeUInt32(TexGID, output);
            }
        }

        public class TexMerge
        {
            public uint BaseTexSize { get; set; }
            public List<TerrainAlphaMap> CornerTerrainMaps { get; set; }
            public List<TerrainAlphaMap> SideTerrainMaps { get; set; }
            public List<RoadAlphaMap> RoadMaps { get; set; }
            public List<TMTerrainDesc> TerrainDescription { get; set; }

            public static TexMerge Read(StreamReader data)
            {
                TexMerge obj = new TexMerge();

                obj.CornerTerrainMaps = new List<TerrainAlphaMap>();
                obj.SideTerrainMaps = new List<TerrainAlphaMap>();
                obj.RoadMaps = new List<RoadAlphaMap>();
                obj.TerrainDescription = new List<TMTerrainDesc>();

                //obj.BaseTexSize = Utils.readUInt32(data);
                obj.BaseTexSize = Utils.readUInt32(data);
                //outputData.BaseStream.Write(BitConverter.GetBytes((uint)256), 0, 4);

                uint num_corner_terrain_maps = Utils.readUInt32(data);
                for (uint i = 0; i < num_corner_terrain_maps; i++)
                    obj.CornerTerrainMaps.Add(TerrainAlphaMap.Read(data));

                uint num_side_terrain_maps = Utils.readUInt32(data);
                for (uint i = 0; i < num_side_terrain_maps; i++)
                    obj.SideTerrainMaps.Add(TerrainAlphaMap.Read(data));

                uint num_road_maps = Utils.readUInt32(data);
                for (uint i = 0; i < num_road_maps; i++)
                    obj.RoadMaps.Add(RoadAlphaMap.Read(data));

                uint num_terrain_desc = Utils.readUInt32(data);

                for (uint i = 0; i < num_terrain_desc; i++)
                    obj.TerrainDescription.Add(TMTerrainDesc.Read(data));

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

            public void Write(StreamWriter output)
            {
                Utils.writeUInt32(BaseTexSize, output);

                Utils.writeUInt32((uint)CornerTerrainMaps.Count, output);
                foreach (var entry in CornerTerrainMaps)
                    entry.Write(output);

                Utils.writeUInt32((uint)SideTerrainMaps.Count, output);
                foreach (var entry in SideTerrainMaps)
                    entry.Write(output);

                Utils.writeUInt32((uint)RoadMaps.Count, output);
                foreach (var entry in RoadMaps)
                    entry.Write(output);

                Utils.writeUInt32((uint)TerrainDescription.Count, output);
                foreach (var entry in TerrainDescription)
                    entry.Write(output);
            }
        }

        public class LandSurf
        {
            public uint HasPalShift { get; set; }
            public TexMerge texMerge { get; set; }

            public static LandSurf Read(StreamReader data)
            {
                LandSurf obj = new LandSurf();

                obj.HasPalShift = Utils.readUInt32(data); // This is always 0

                if (obj.HasPalShift == 1)
                {
                    // PalShift.Read would go here, if it ever actually existed...which it doesn't.
                }
                else
                    obj.texMerge = TexMerge.Read(data);

                return obj;
            }

            public void Write(StreamWriter output)
            {
                Utils.writeUInt32(HasPalShift, output);

                if (HasPalShift == 1)
                {
                    // PalShift.write would go here, if it ever actually existed...which it only existed at the very eary stages when AC supported software rendering.
                }
                else
                    texMerge.Write(output);
            }
        }

        public class TerrainType
        {
            public string TerrainName { get; set; }
            public uint TerrainColor { get; set; }
            public List<uint> SceneTypes { get; set; }

            public static TerrainType Read(StreamReader data)
            {
                TerrainType obj = new TerrainType();

                obj.TerrainName = Utils.readString(data);

                obj.TerrainColor = Utils.readUInt32(data);

                obj.SceneTypes = new List<uint>();
                uint num_stypes = Utils.readUInt32(data);
                for (uint i = 0; i < num_stypes; i++)
                    obj.SceneTypes.Add(Utils.readUInt32(data));

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

            public static TerrainDesc Read(StreamReader data)
            {
                TerrainDesc obj = new TerrainDesc();

                uint num_terrain_types = Utils.readUInt32(data);

                obj.TerrainTypes = new List<TerrainType>();

                for (uint i = 0; i < num_terrain_types; i++)
                    obj.TerrainTypes.Add(TerrainType.Read(data));

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

                obj.LandSurfaces = LandSurf.Read(data);

                return obj;
            }

            public void Write(StreamWriter output)
            {
                Utils.writeUInt32((uint)TerrainTypes.Count, output);
                foreach (var entry in TerrainTypes)
                    entry.Write(output);

                LandSurfaces.Write(output);
            }
        }

        public class SceneType
        {
            public uint StbIndex;
            public List<uint> Scenes { get; set; }

            public static SceneType Read(StreamReader data)
            {
                SceneType obj = new SceneType();

                obj.StbIndex = Utils.readUInt32(data);

                uint num_scenes = Utils.readUInt32(data);
                obj.Scenes = new List<uint>();
                for (uint i = 0; i < num_scenes; i++)
                    obj.Scenes.Add(Utils.readUInt32(data));

                return obj;
            }

            public void Write(StreamWriter output)
            {
                Utils.writeUInt32(StbIndex, output);

                Utils.writeUInt32((uint)Scenes.Count, output);
                foreach (var entry in Scenes)
                    Utils.writeUInt32(entry, output);
            }
        }

        public class SceneDesc
        {
            public List<SceneType> SceneTypes { get; set; }

            public static SceneDesc Read(StreamReader data)
            {
                SceneDesc obj = new SceneDesc();

                uint num_scene_types = Utils.readUInt32(data);
                obj.SceneTypes = new List<SceneType>();
                for (uint i = 0; i < num_scene_types; i++)
                    obj.SceneTypes.Add(SceneType.Read(data));

                return obj;
            }

            public void Write(StreamWriter output)
            {
                Utils.writeUInt32((uint)SceneTypes.Count, output);
                foreach (var entry in SceneTypes)
                    entry.Write(output);
            }
        }
        public class AmbientSoundDesc
        {
            public uint SType { get; set; }
            public float Volume { get; set; }
            public float BaseChance { get; set; }
            public float MinRate { get; set; }
            public float MaxRate { get; set; }

            public static AmbientSoundDesc Read(StreamReader data)
            {
                AmbientSoundDesc obj = new AmbientSoundDesc();
                obj.SType = Utils.readUInt32(data);
                obj.Volume = Utils.readSingle(data);
                obj.BaseChance = Utils.readSingle(data);
                obj.MinRate = Utils.readSingle(data);
                obj.MaxRate = Utils.readSingle(data);
                return obj;
            }

            public void Write(StreamWriter output)
            {
                Utils.writeUInt32(SType, output);
                Utils.writeSingle(Volume, output);
                Utils.writeSingle(BaseChance, output);
                Utils.writeSingle(MinRate, output);
                Utils.writeSingle(MaxRate, output);
            }
        }
        public class AmbientSTBDesc
        {
            public uint STBId { get; set; }
            public List<AmbientSoundDesc> AmbientSounds { get; set; }

            public static AmbientSTBDesc Read(StreamReader data)
            {
                AmbientSTBDesc obj = new AmbientSTBDesc();
                obj.STBId = Utils.readUInt32(data);

                uint num_ambient_sounds = Utils.readUInt32(data);
                obj.AmbientSounds = new List<AmbientSoundDesc>();
                for (uint i = 0; i < num_ambient_sounds; i++)
                    obj.AmbientSounds.Add(AmbientSoundDesc.Read(data));

                return obj;
            }

            public void Write(StreamWriter output)
            {
                Utils.writeUInt32(STBId, output);

                Utils.writeUInt32((uint)AmbientSounds.Count, output);
                foreach (var entry in AmbientSounds)
                    entry.Write(output);
            }
        }

        public class SoundDesc
        {
            public List<AmbientSTBDesc> STBDesc { get; set; }

            public static SoundDesc Read(StreamReader data)
            {
                SoundDesc obj = new SoundDesc();

                uint num_stb_desc = Utils.readUInt32(data);
                obj.STBDesc = new List<AmbientSTBDesc>();
                for (uint i = 0; i < num_stb_desc; i++)
                    obj.STBDesc.Add(AmbientSTBDesc.Read(data));

                return obj;
            }

            public void Write(StreamWriter output)
            {
                Utils.writeUInt32((uint)STBDesc.Count, output);
                foreach (var entry in STBDesc)
                    entry.Write(output);
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

            public static SkyObjectReplace Read(StreamReader data)
            {
                SkyObjectReplace obj = new SkyObjectReplace();
                obj.ObjectIndex = Utils.readUInt32(data);
                obj.GFXObjId = Utils.readUInt32(data);
                obj.Rotate = Utils.readSingle(data);
                obj.Transparent = Utils.readSingle(data);
                obj.Luminosity = Utils.readSingle(data);
                obj.MaxBright = Utils.readSingle(data);
                return obj;
            }

            public void Write(StreamWriter output)
            {
                Utils.writeUInt32(ObjectIndex, output);
                Utils.writeUInt32(GFXObjId, output);
                Utils.writeSingle(Rotate, output);
                Utils.writeSingle(Transparent, output);
                Utils.writeSingle(Luminosity, output);
                Utils.writeSingle(MaxBright, output);
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

            public static SkyTimeOfDay Read(StreamReader data)
            {
                SkyTimeOfDay obj = new SkyTimeOfDay();
                obj.Begin = Utils.readSingle(data);
                obj.DirBright = Utils.readSingle(data);
                obj.DirHeading = Utils.readSingle(data);
                obj.DirPitch = Utils.readSingle(data);
                obj.DirColor = Utils.readUInt32(data);

                obj.AmbBright = Utils.readSingle(data);
                obj.AmbColor = Utils.readUInt32(data);

                obj.MinWorldFog = Utils.readSingle(data);
                obj.MaxWorldFog = Utils.readSingle(data);
                obj.WorldFogColor = Utils.readUInt32(data);
                obj.WorldFog = Utils.readUInt32(data);

                uint num_sky_obj_replace = Utils.readUInt32(data);
                obj.SkyObjReplace = new List<SkyObjectReplace>();
                for (uint i = 0; i < num_sky_obj_replace; i++)
                    obj.SkyObjReplace.Add(SkyObjectReplace.Read(data));

                return obj;
            }

            public void Write(StreamWriter output)
            {
                Utils.writeSingle(Begin, output);
                Utils.writeSingle(DirBright, output);
                Utils.writeSingle(DirHeading, output);
                Utils.writeSingle(DirPitch, output);
                Utils.writeUInt32(DirColor, output);

                Utils.writeSingle(AmbBright, output);
                Utils.writeUInt32(AmbColor, output);

                Utils.writeSingle(MinWorldFog, output);
                Utils.writeSingle(MaxWorldFog, output);
                Utils.writeUInt32(WorldFogColor, output);
                Utils.writeUInt32(WorldFog, output);

                Utils.writeUInt32((uint)SkyObjReplace.Count, output);
                foreach (var entry in SkyObjReplace)
                {
                    entry.Write(output);
                }
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

            public static SkyObject Read(StreamReader data, eDatFormat format)
            {
                SkyObject obj = new SkyObject();
                obj.BeginTime = Utils.readSingle(data);
                obj.EndTime = Utils.readSingle(data);
                obj.BeginAngle = Utils.readSingle(data);
                obj.EndAngle = Utils.readSingle(data);
                obj.TexVelocityX = Utils.readSingle(data);
                obj.TexVelocityY = Utils.readSingle(data);
                obj.TexVelocityZ = 0;
                obj.DefaultGFXObjectId = Utils.readUInt32(data);
                obj.DefaultPESObjectId = Utils.readUInt32(data);

                if(format == eDatFormat.Latest)
                    obj.Properties = Utils.readUInt32(data);
                //Utils.align(data);

                return obj;
            }

            public void Write(StreamWriter output)
            {
                Utils.writeSingle(BeginTime, output);
                Utils.writeSingle(EndTime, output);
                Utils.writeSingle(BeginAngle, output);
                Utils.writeSingle(EndAngle, output);
                Utils.writeSingle(TexVelocityX, output);
                Utils.writeSingle(TexVelocityY, output);                
                Utils.writeUInt32(DefaultGFXObjectId, output);
                Utils.writeUInt32(DefaultPESObjectId, output);
                Utils.writeUInt32(Properties, output);
            }
        }

        public class DayGroup
        {
            public float ChanceOfOccur { get; set; }
            public string DayName { get; set; }
            public List<SkyObject> SkyObjects { get; set; }
            public List<SkyTimeOfDay> SkyTimes { get; set; }

            public static DayGroup Read(StreamReader data, eDatFormat format)
            {
                DayGroup obj = new DayGroup();
                obj.ChanceOfOccur = Utils.readSingle(data);
                obj.DayName = Utils.readString(data);

                uint num_sky_objects = Utils.readUInt32(data);
                obj.SkyObjects = new List<SkyObject>();
                for (uint i = 0; i < num_sky_objects; i++)
                    obj.SkyObjects.Add(SkyObject.Read(data, format));

                uint num_sky_times = Utils.readUInt32(data);
                obj.SkyTimes = new List<SkyTimeOfDay>();
                for (uint i = 0; i < num_sky_times; i++)
                    obj.SkyTimes.Add(SkyTimeOfDay.Read(data));

                return obj;
            }

            public void Write(StreamWriter output)
            {
                Utils.writeSingle(ChanceOfOccur, output);
                Utils.writeString(DayName, output);

                Utils.writeUInt32((uint)SkyObjects.Count, output);
                foreach(var entry in SkyObjects)
                    entry.Write(output);

                Utils.writeUInt32((uint)SkyTimes.Count, output);
                foreach (var entry in SkyTimes)
                    entry.Write(output);
            }
        }
        public class SkyDesc
        {
            public UInt64 TickSize { get; set; }
            public UInt64 LightTickSize { get; set; }
            public List<DayGroup> DayGroups { get; set; }

            public static SkyDesc Read(StreamReader data, eDatFormat format)
            {
                SkyDesc obj = new SkyDesc();
                obj.TickSize = Utils.readUInt64(data);
                obj.LightTickSize = Utils.readUInt64(data);

                uint numDayGroups = Utils.readUInt32(data);
                obj.DayGroups = new List<DayGroup>();
                for (uint i = 0; i < numDayGroups; i++)
                    obj.DayGroups.Add(DayGroup.Read(data, format));

                return obj;
            }

            public void Write(StreamWriter output)
            {
                Utils.writeUInt64(TickSize, output);
                Utils.writeUInt64(LightTickSize, output);

                Utils.writeUInt32((uint)DayGroups.Count, output);
                foreach (var entry in DayGroups)
                    entry.Write(output);
            }
        }

        public class LandDefs
        {
            public List<float> LandHeightTable { get; set; }

            public static LandDefs Read(StreamReader data)
            {
                LandDefs obj = new LandDefs();
                obj.LandHeightTable = new List<float>();
                for (int i = 0; i < 256; i++)
                {
                    obj.LandHeightTable.Add(Utils.readSingle(data));
                }
                return obj;
            }

            public void Write(StreamWriter output)
            {
                foreach(var entry in LandHeightTable)
                {
                    Utils.writeSingle(entry, output);
                }
            }
        }

        public class Season
        {
            public uint StartDate { get; set; }
            public string Name { get; set; }

            public static Season Read(StreamReader data)
            {
                Season obj = new Season();
                obj.StartDate = Utils.readUInt32(data);
                obj.Name = Utils.readString(data);
                return obj;
            }

            public void Write(StreamWriter output)
            {
                Utils.writeUInt32(StartDate, output);
                Utils.writeString(Name, output);
            }
        }

        public class TimeOfDay
        {
            public uint Start { get; set; }
            public uint IsNight { get; set; }
            public string Name { get; set; }

            public static TimeOfDay Read(StreamReader data)
            {
                TimeOfDay obj = new TimeOfDay();
                obj.Start = Utils.readUInt32(data);
                obj.IsNight = Utils.readUInt32(data);
                obj.Name = Utils.readString(data);
                return obj;
            }

            public void Write(StreamWriter output)
            {
                Utils.writeUInt32(Start, output);
                Utils.writeUInt32(IsNight, output);
                Utils.writeString(Name, output);
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

            public static GameTime Read(StreamReader data)
            {
                GameTime obj = new GameTime();
                obj.ZeroTimeOfYear = Utils.readUInt64(data);
                obj.ZeroYear = Utils.readUInt32(data);
                obj.DayLength = Utils.readUInt32(data);
                obj.DaysPerYear = Utils.readUInt32(data);
                obj.YearSpec = Utils.readString(data);

                uint numTimesOfDay = Utils.readUInt32(data);
                obj.TimesOfDay = new List<TimeOfDay>();
                for (uint i = 0; i < numTimesOfDay; i++)
                {
                    obj.TimesOfDay.Add(TimeOfDay.Read(data));
                }

                uint numDaysOfTheWeek = Utils.readUInt32(data);
                obj.DaysOfTheWeek = new List<string>();
                for (uint i = 0; i < numDaysOfTheWeek; i++)
                {
                    obj.DaysOfTheWeek.Add(Utils.readString(data));
                }

                uint numSeasons = Utils.readUInt32(data);
                obj.Seasons = new List<Season>();
                for (uint i = 0; i < numSeasons; i++)
                {
                    obj.Seasons.Add(Season.Read(data));
                }

                return obj;
            }

            public void Write(StreamWriter output)
            {
                Utils.writeUInt64(ZeroTimeOfYear, output);
                Utils.writeUInt32(ZeroYear, output);
                Utils.writeUInt32(DayLength, output);
                Utils.writeUInt32(DaysPerYear, output);
                Utils.writeString(YearSpec, output);

                Utils.writeUInt32((uint)TimesOfDay.Count, output);                
                foreach(var entry in TimesOfDay)
                {
                    entry.Write(output);
                }

                Utils.writeUInt32((uint)DaysOfTheWeek.Count, output);
                foreach (var entry in DaysOfTheWeek)
                {
                    Utils.writeString(entry, output);
                }

                Utils.writeUInt32((uint)Seasons.Count, output);
                foreach (var entry in Seasons)
                {
                    entry.Write(output);
                }
            }
        }

        //        static public void convert(string filename)
        //        {
        //            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
        //            if (inputFile == null)
        //            {
        //                Console.WriteLine("Unable to open {0}", filename);
        //                return;
        //            }
        //            StreamWriter outputFile = new StreamWriter(new FileStream("./Region/13000000 - World Info - Winter.bin", FileMode.Create, FileAccess.Write));
        //            if (outputFile == null)
        //            {
        //                Console.WriteLine("Unable to open 13000000 - World Info - Winter.bin");
        //                return;
        //            }

        //            Console.WriteLine("Converting region file to winter...");

        //            uint fileHeader;
        //            uint loaded;
        //            uint timeStamp;
        //            string regionName;
        //            uint partsMask;
        //            uint unknown1;
        //            uint unknown2;
        //            uint unknown3;
        //            uint unknown4;
        //            uint unknown5;
        //            uint unknown6;
        //            uint unknown7;

        //            LandDefs landDef;
        //            GameTime gameTime;
        //            uint next;

        //            SkyDesc skyInfo;
        //            SoundDesc soundInfo;
        //            SceneDesc sceneInfo;
        //            TerrainDesc terrainInfo;
        //            RegionMisc regionMisc;

        //            fileHeader = Utils.readUInt32(inputFile);
        //            if (fileHeader != 0x13000000)
        //            {
        //                Console.WriteLine("Invalid header, aborting.");
        //                return;
        //            }

        //            loaded = Utils.readUInt32(inputFile);
        //            timeStamp = Utils.readUInt32(inputFile);
        //            regionName = Utils.readString(inputFile);
        //            partsMask = Utils.readUInt32(inputFile);

        //            unknown1 = Utils.readUInt32(inputFile);
        //            unknown2 = Utils.readUInt32(inputFile);
        //            unknown3 = Utils.readUInt32(inputFile);
        //            unknown4 = Utils.readUInt32(inputFile);
        //            unknown5 = Utils.readUInt32(inputFile);
        //            unknown6 = Utils.readUInt32(inputFile);
        //            unknown7 = Utils.readUInt32(inputFile);

        //            landDef = LandDefs.Read(inputFile);
        //            gameTime = GameTime.Read(inputFile);

        //            next = Utils.readUInt32(inputFile);

        //            if ((next & 0x10) > 0)
        //                skyInfo = SkyDesc.Read(inputFile);

        //            if ((next & 0x01) > 0)
        //                soundInfo = SoundDesc.Read(inputFile);

        //            if ((next & 0x02) > 0)
        //                sceneInfo = SceneDesc.Read(inputFile);

        //            terrainInfo = TerrainDesc.Read(inputFile);

        //            if ((next & 0x0200) > 0)
        //                regionMisc = RegionMisc.Read(inputFile);

        //            //StreamWriter outputFile2 = new StreamWriter(new FileStream("./13000000 - Terrain Textures -ToD.txt", FileMode.Create, FileAccess.Write));
        //            //if (outputFile2 == null)
        //            //{
        //            //    Console.WriteLine("Unable to open 13000000 - Terrain Textures - ToD.txt");
        //            //    return;
        //            //}

        //            //for (int i = 0; i < terrainInfo.LandSurfaces.texMerge.CornerTerrainMaps.Count; i++)
        //            //{
        //            //    TerrainAlphaMap cornerTerrainMap = terrainInfo.LandSurfaces.texMerge.CornerTerrainMaps[i];

        //            //    outputFile2.WriteLine("{0}", cornerTerrainMap.TexGID.ToString("x8"));
        //            //    outputFile2.Flush();
        //            //}

        //            //for (int i = 0; i < terrainInfo.LandSurfaces.texMerge.SideTerrainMaps.Count; i++)
        //            //{
        //            //    TerrainAlphaMap sideTerrainMap = terrainInfo.LandSurfaces.texMerge.SideTerrainMaps[i];

        //            //    outputFile2.WriteLine("{0}", sideTerrainMap.TexGID.ToString("x8"));
        //            //    outputFile2.Flush();
        //            //}

        //            //for (int i = 0; i < terrainInfo.LandSurfaces.texMerge.RoadMaps.Count; i++)
        //            //{
        //            //    RoadAlphaMap roadMap = terrainInfo.LandSurfaces.texMerge.RoadMaps[i];

        //            //    outputFile2.WriteLine("{0}", roadMap.RoadTexGID.ToString("x8"));
        //            //    outputFile2.Flush();
        //            //}

        //            //for (int i = 0; i < terrainInfo.LandSurfaces.texMerge.TerrainDescription.Count; i++)//ignore first entry as it's a repeat of the second
        //            //{

        //            //    TMTerrainDesc desc = terrainInfo.LandSurfaces.texMerge.TerrainDescription[i];
        //            //    string terrainName = "Unknown";
        //            //    uint terrainColor = 0;
        //            //    if (i < terrainInfo.TerrainTypes.Count)
        //            //    {
        //            //        terrainName = terrainInfo.TerrainTypes[(int)desc.terrainType].TerrainName;
        //            //        terrainColor = terrainInfo.TerrainTypes[(int)desc.terrainType].TerrainColor;
        //            //    }
        //            //    else if (i == 32)
        //            //    {
        //            //        terrainName = "Road";
        //            //        terrainColor = 0;
        //            //    }

        //            //    terrainName = terrainName.PadLeft(20);

        //            //    outputFile2.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}", desc.terrainType, terrainName, desc.terrainTex.TexGID.ToString("x8"), desc.terrainTex.TexTiling, desc.terrainTex.DetailTexGID.ToString("x8"),
        //            //        desc.terrainTex.DetailTexTiling, desc.terrainTex.MaxVertBright, desc.terrainTex.MaxVertHue, desc.terrainTex.MaxVertSaturate, desc.terrainTex.MinVertBright,
        //            //        desc.terrainTex.MinVertHue, desc.terrainTex.MinVertSaturate);

        //            //    //outputFile2.WriteLine("TextureConverter.toBin(\"Landscape Texture Conversion/DM/Textures/Upscaled/xxx.png\", 0x{0}, 21);//{1}",desc.terrainTex.TexGID.ToString("x8"),terrainName);

        //            //    //outputFile2.WriteLine("writedat client_portal.dat -f {0}={0}.bin", desc.terrainTex.TexGID.ToString("x8"));
        //            //    outputFile2.Flush();
        //            //}
        //            //outputFile2.Close();

        //            inputFile.Close();

        //            outputFile.Flush();
        //            outputFile.Close();
        //            Console.WriteLine("Done");
        //        }

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

        LandDefs landDefs;
        GameTime gameTime;
        uint next;

        SkyDesc skyDesc;
        SoundDesc soundDesc;
        SceneDesc sceneDesc;
        TerrainDesc terrainDesc;
        RegionMisc regionMisc;

        eDatFormat format;

        public RegionManipulation(string filename, eDatFormat format)
        {
            LoadFromBin(filename, format);
        }

        public void LoadFromBin(string filename, eDatFormat format)
        {
            this.format = format;

            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename);
                return;
            }

            Console.WriteLine("Loading region file from bin...");

            fileHeader = Utils.readUInt32(inputFile);
            if (fileHeader != 0x13000000 && fileHeader != 0x130F0000)
            {
                Console.WriteLine("Invalid header, aborting.");
                return;
            }

            loaded = Utils.readUInt32(inputFile);
            timeStamp = Utils.readUInt32(inputFile);
            regionName = Utils.readString(inputFile);
            partsMask = Utils.readUInt32(inputFile);

            unknown1 = Utils.readUInt32(inputFile);
            unknown2 = Utils.readUInt32(inputFile);
            unknown3 = Utils.readUInt32(inputFile);
            unknown4 = Utils.readUInt32(inputFile);
            unknown5 = Utils.readUInt32(inputFile);
            unknown6 = Utils.readUInt32(inputFile);
            unknown7 = Utils.readUInt32(inputFile);

            landDefs = LandDefs.Read(inputFile);
            gameTime = GameTime.Read(inputFile);

            next = Utils.readUInt32(inputFile);

            if ((next & 0x10) > 0)
                skyDesc = SkyDesc.Read(inputFile, format);

            if ((next & 0x01) > 0)
                soundDesc = SoundDesc.Read(inputFile);

            if ((next & 0x02) > 0)
                sceneDesc = SceneDesc.Read(inputFile);

            terrainDesc = TerrainDesc.Read(inputFile);

            if ((next & 0x0200) > 0)
                regionMisc = RegionMisc.Read(inputFile);

            inputFile.Close();

            Console.WriteLine("Done");
        }

        public void WriteToBin(string filename)
        {
            StreamWriter outputFile = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename);
                return;
            }

            Console.WriteLine("Writing region file to bin...");


            if (fileHeader != 0x13000000)
            {
                Console.WriteLine("Invalid header, aborting.");
                return;
            }

            Utils.writeUInt32(fileHeader, outputFile);

            Utils.writeUInt32(loaded, outputFile);
            Utils.writeUInt32(timeStamp, outputFile);
            Utils.writeString(regionName, outputFile);
            Utils.writeUInt32(partsMask, outputFile);

            Utils.writeUInt32(unknown1, outputFile);
            Utils.writeUInt32(unknown2, outputFile);
            Utils.writeUInt32(unknown3, outputFile);
            Utils.writeUInt32(unknown4, outputFile);
            Utils.writeUInt32(unknown5, outputFile);
            Utils.writeUInt32(unknown6, outputFile);
            Utils.writeUInt32(unknown7, outputFile);

            landDefs.Write(outputFile);
            gameTime.Write(outputFile);

            Utils.writeUInt32(next, outputFile);

            if ((next & 0x10) > 0)
                skyDesc.Write(outputFile);

            if ((next & 0x01) > 0)
                soundDesc.Write(outputFile);

            if ((next & 0x02) > 0)
                sceneDesc.Write(outputFile);

            terrainDesc.Write(outputFile);

            if ((next & 0x0200) > 0)
                regionMisc.Write(outputFile);

            outputFile.Close();

            Console.WriteLine("Done");
        }

        public void CompareObjects(string filename, string otherFilename)
        {
            StreamWriter outputFile = new StreamWriter(new FileStream("SceneObjectsTranslationTable.txt", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open SceneObjectsTranslationTable.txt");
                return;
            }

            Console.WriteLine("Comparing Scene Objects...");

            PortalDatDatabase PortalDat = new PortalDatDatabase(filename);
            PortalDatDatabase PortalDatOther = new PortalDatDatabase(otherFilename);

            SortedDictionary<uint, uint> translationMap = new SortedDictionary<uint, uint>();

            List<uint> sceneIds = GetSceneIds();
            foreach (var sceneIdEntry in sceneIds)
            {
                var scene = PortalDat.ReadFromDat<Scene>(sceneIdEntry);
                var sceneOther = PortalDatOther.ReadFromDat<Scene>(sceneIdEntry);

                if(scene.Objects.Count != sceneOther.Objects.Count)
                {
                    Console.WriteLine($"SceneId: {scene.Id}: Object amount mismatch.");
                    continue;
                }

                for(int i = 0; i < scene.Objects.Count; i++)
                {
                    ObjectDesc obj = scene.Objects[i];
                    ObjectDesc objOther = sceneOther.Objects[i];
                    if (!translationMap.ContainsKey(obj.ObjId))
                    {
                        translationMap.Add(obj.ObjId, objOther.ObjId);
                    }
                }
            }

            foreach(var entry in translationMap)
            {
                outputFile.WriteLine($"{entry.Key.ToString("x8")}\t{entry.Value.ToString("x8")}");
                outputFile.Flush();
            }

            outputFile.Close();

            Console.WriteLine("Done");
        }

        public List<uint> GetObjects(string filename)
        {
            PortalDatDatabase PortalDat = new PortalDatDatabase(filename, true);

            List<uint> list = new List<uint>();

            List<uint> sceneIds = GetSceneIds();
            foreach (var sceneIdEntry in sceneIds)
            {
                var scene = PortalDat.ReadFromDat<Scene>(sceneIdEntry);

                foreach (var sceneObject in scene.Objects)
                {
                    if(!list.Contains(sceneObject.ObjId))
                        list.Add(sceneObject.ObjId);
                }
            }

            return list;
        }

        public List<uint> GetSceneIds()
        {
            List<uint> list = new List<uint>();
            foreach (var sceneEntry in sceneDesc.SceneTypes)
            {
                foreach (var entry in sceneEntry.Scenes)
                {
                    if (!list.Contains(entry))
                    {
                        list.Add(entry);
                    }
                }
            }

            return list;
        }

        public void ExportSceneIds(string filename)
        {
            StreamWriter outputFile = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename);
                return;
            }

            List<uint> list = new List<uint>();
            foreach(var sceneEntry in sceneDesc.SceneTypes)
            {
                foreach (var entry in sceneEntry.Scenes)
                {
                    if (!list.Contains(entry))
                    {
                        list.Add(entry);
                        outputFile.WriteLine(entry.ToString("x8"));
                        //outputFile.WriteLine($"readdat -f \"portal.dat\" {entry.ToString("x8").Replace("0x","")}");                        
                        outputFile.Flush();
                    }
                }
            }

            outputFile.Close();
        }

        public void GenerateExtractAndReplaceBats()
        {
            StreamWriter extractFile = new StreamWriter(new FileStream("Extract Scene Files.bat", FileMode.Create, FileAccess.Write));
            if (extractFile == null)
            {
                Console.WriteLine("Unable to open Extract Scene Files.bat");
                return;
            }

            StreamWriter replaceFile = new StreamWriter(new FileStream("Replace Scene Files.bat", FileMode.Create, FileAccess.Write));
            if (replaceFile == null)
            {
                Console.WriteLine("Unable to open Replace Scene Files.bat");
                return;
            }

            List<uint> list = new List<uint>();
            foreach (var sceneEntry in sceneDesc.SceneTypes)
            {
                foreach (var entry in sceneEntry.Scenes)
                {
                    if (!list.Contains(entry))
                    {
                        string file = entry.ToString("x8").Replace("0x", "");
                        list.Add(entry);
                        extractFile.WriteLine($"readdat -f \"client_portal.dat\" {file}");
                        extractFile.Flush();
                        replaceFile.WriteLine($"writedat client_portal.dat -f {file}=\"..\\Shared\\Textures\\{file}.bin\"");
                        replaceFile.Flush();
                    }
                }
            }

            extractFile.Close();
            replaceFile.Close();
        }
    }
}