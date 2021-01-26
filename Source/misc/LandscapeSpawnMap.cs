using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace Melt
{
    public class LandscapeSpawnMap
    {
        public class Encounter
        {
            public uint key;
            public List<uint> value;
        }

        public class Region
        {
            public List<ushort> encounterMap;
            public Encounter[] encounters;
            public int tableCount;
            public int tableSize;
        }

        Region LandscapeSpawnRegions;

        Dictionary<int, Color> IdToColor = new Dictionary<int, Color>();
        Dictionary<Color, int> ColorToId = new Dictionary<Color, int>();

        public LandscapeSpawnMap()
        {
            IdToColor.Add(0, Color.FromArgb(0, 0, 0));
            IdToColor.Add(8, Color.FromArgb(255, 128, 128));
            IdToColor.Add(16, Color.FromArgb(255, 255, 128));
            IdToColor.Add(23, Color.FromArgb(128, 255, 128));
            IdToColor.Add(24, Color.FromArgb(0, 255, 128));
            IdToColor.Add(32, Color.FromArgb(128, 255, 255));
            IdToColor.Add(40, Color.FromArgb(0, 128, 255));
            IdToColor.Add(48, Color.FromArgb(255, 128, 192));
            IdToColor.Add(56, Color.FromArgb(255, 128, 255));
            IdToColor.Add(64, Color.FromArgb(255, 0, 0));
            IdToColor.Add(72, Color.FromArgb(255, 255, 0));
            IdToColor.Add(80, Color.FromArgb(128, 255, 0));
            IdToColor.Add(88, Color.FromArgb(0, 255, 64));
            IdToColor.Add(96, Color.FromArgb(0, 255, 255));
            IdToColor.Add(104, Color.FromArgb(0, 128, 192));
            IdToColor.Add(112, Color.FromArgb(128, 128, 192));
            IdToColor.Add(160, Color.FromArgb(255, 0, 255));
            IdToColor.Add(184, Color.FromArgb(128, 64, 64));
            IdToColor.Add(192, Color.FromArgb(255, 128, 64));
            IdToColor.Add(200, Color.FromArgb(0, 255, 0));
            IdToColor.Add(207, Color.FromArgb(0, 128, 128));
            IdToColor.Add(208, Color.FromArgb(0, 64, 128));
            IdToColor.Add(215, Color.FromArgb(128, 128, 255));
            IdToColor.Add(216, Color.FromArgb(128, 0, 64));
            IdToColor.Add(223, Color.FromArgb(255, 0, 128));
            IdToColor.Add(224, Color.FromArgb(128, 0, 0));
            IdToColor.Add(232, Color.FromArgb(255, 128, 0));
            IdToColor.Add(240, Color.FromArgb(0, 128, 0));

            foreach (var entry in IdToColor)
            {
                ColorToId.Add(entry.Value, entry.Key);
            }
        }

        public void LoadRegionFromJson(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));

            string jsonData = inputFile.ReadToEnd();
            inputFile.Close();

            LandscapeSpawnRegions = JsonConvert.DeserializeObject<Region>(jsonData);
        }

        public void SaveRegionToJson(string filename)
        {
            string jsonData = JsonConvert.SerializeObject(LandscapeSpawnRegions);

            StreamWriter outputFile = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write));
            outputFile.Write(jsonData);
            outputFile.Close();
        }

        public void LoadRegionFromPng(string filename)
        {
            LandscapeSpawnRegions.encounterMap.Clear();
            int sizeMultiplier = 8;
            Bitmap bmp = new Bitmap(filename);

            for (int x = 0; x < LandscapeSpawnRegions.tableSize - 1; x++)
            {
                for (int y = LandscapeSpawnRegions.tableSize - 2; y >= 0; y--)
                {
                    Color color = bmp.GetPixel(x * sizeMultiplier, y * sizeMultiplier);
                    int id;
                    if (ColorToId.TryGetValue(color, out id))
                        LandscapeSpawnRegions.encounterMap.Add((ushort)id);
                    else
                    {
                        Console.WriteLine($"Invalid color found on pixel {x} {y}! Defaulting to 0.");
                        LandscapeSpawnRegions.encounterMap.Add(0);
                    }
                }
            }
        }

        public void SaveRegionToPng(string filename)
        {
            int sizeMultiplier = 8;
            Bitmap bmp = new Bitmap((LandscapeSpawnRegions.tableSize - 1) * sizeMultiplier, (LandscapeSpawnRegions.tableSize - 1) * sizeMultiplier);

            int i = 0;
            for(int x = 0; x < LandscapeSpawnRegions.tableSize - 1; x++)
            {
                for (int y = LandscapeSpawnRegions.tableSize - 2; y >= 0 ; y--)
                {
                    int value = LandscapeSpawnRegions.encounterMap[i];
                    for (int offsetX = 0; offsetX < sizeMultiplier; offsetX++)
                        for (int offsetY = 0; offsetY < sizeMultiplier; offsetY++)
                            bmp.SetPixel((x * sizeMultiplier)+ offsetX, (y * sizeMultiplier)+offsetY, IdToColor[value]);
                    i++;
                }
            }

            bmp.Save(filename, ImageFormat.Png);
        }

        public void ExportSpawnMapToAceDatabase(string originalEncounterMapJsonFilename, string modifiedEncounterMapPngFilename)
        {
            LoadRegionFromJson(originalEncounterMapJsonFilename);

            List<ushort> newEncounterMap = new List<ushort>();

            int sizeMultiplier = 8;
            Bitmap bmp = new Bitmap(modifiedEncounterMapPngFilename);

            for (int x = 0; x < LandscapeSpawnRegions.tableSize - 1; x++)
            {
                for (int y = LandscapeSpawnRegions.tableSize - 2; y >= 0; y--)
                {
                    Color color = bmp.GetPixel(x * sizeMultiplier, y * sizeMultiplier);
                    int id;
                    if (ColorToId.TryGetValue(color, out id))
                        newEncounterMap.Add((ushort)id);
                    else
                    {
                        Console.WriteLine($"Invalid color found on pixel {x} {y}! Defaulting to 0.");
                        newEncounterMap.Add(0);
                    }
                }
            }

            UpdateDatabaseEncounters(newEncounterMap);
        }

        Dictionary<ushort, Dictionary<int, int>> ReplacementMap = new Dictionary<ushort, Dictionary<int, int>>();

        public bool BuildReplacementMap(string filename)
        {
            ReplacementMap.Clear();
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));

            string line;
            string[] splitLine;

            ushort currentEntry = 0;
            while (!inputFile.EndOfStream)
            {
                line = inputFile.ReadLine();
                splitLine = line.Split('\t');
                if (splitLine.Length == 3)
                {
                    int from;
                    int to;
                    if (int.TryParse(splitLine[1], out from) && int.TryParse(splitLine[2], out to))
                    {
                        currentEntry = (ushort)((from << 8) | to);
                    }
                    else
                    {
                        Console.WriteLine("Failed to parse ReplacementMap!");
                        return false;
                    }
                }
                else if (splitLine.Length == 2)
                {
                    int from;
                    int to;
                    if (int.TryParse(splitLine[0], out from) && int.TryParse(splitLine[1], out to))
                    {
                        if (!ReplacementMap.ContainsKey(currentEntry))
                            ReplacementMap[currentEntry] = new Dictionary<int, int>();
                        ReplacementMap[currentEntry][from] = to;
                    }
                    else
                    {
                        Console.WriteLine("Failed to parse ReplacementMap!");
                        return false;
                    }
                }
            }

            return true;
        }

        public class dbEncounter
        {
            public int id;
            public int landblock;
            public int weenie_class_id;
            public int cell_X;
            public int cell_Y;
            public DateTime last_Modified;
        }

        public int ConvertId(int id, ushort fromEncounterMap, ushort toEncounterMap)
        {
            ushort key = (ushort)((fromEncounterMap << 8) | toEncounterMap);
            Debug.Assert(ReplacementMap.ContainsKey(key), $"Missing conversion for {fromEncounterMap}->{toEncounterMap}");
            Debug.Assert(ReplacementMap[key].ContainsKey(id), $"Missing entry in {fromEncounterMap}->{toEncounterMap} map: {id}");
            return ReplacementMap[key][id];

        }

        public void UpdateDatabaseEncounters(List<ushort> newEncounterMap)
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=root;password=;DefaultCommandTimeout=120;database=ace_world_test");
            connection.Open();

            string sql;
            MySqlCommand command;
            MySqlDataReader reader;

            if (BuildReplacementMap("./input/spawnMap/replacementMap.txt"))
            {
                int i = 0;
                int count = 0;
                Console.WriteLine($"Updating database encounters...");
                for (int x = 0; x < LandscapeSpawnRegions.tableSize - 1; x++)
                {
                    for (int y = 0; y < LandscapeSpawnRegions.tableSize - 1; y++)
                    {
                        int landblockId = (x * LandscapeSpawnRegions.tableSize) + y;

                        if (newEncounterMap[i] != LandscapeSpawnRegions.encounterMap[i])
                        {
                            sql = $"SELECT * FROM encounter WHERE landblock = {landblockId}";
                            command = new MySqlCommand(sql, connection);
                            reader = command.ExecuteReader();

                            List<dbEncounter> encounters = new List<dbEncounter>();
                            while (reader.Read())
                            {
                                dbEncounter entry = new dbEncounter();
                                entry.id = reader.GetInt32(0);
                                entry.landblock = reader.GetInt32(1);
                                entry.weenie_class_id = reader.GetInt32(2);
                                entry.cell_X = reader.GetInt32(3);
                                entry.cell_Y = reader.GetInt32(4);
                                entry.last_Modified = reader.GetDateTime(5);

                                encounters.Add(entry);
                            }
                            reader.Close();

                            for (int j = 0; j < encounters.Count; j++)
                            {
                                encounters[j].weenie_class_id = ConvertId(encounters[j].weenie_class_id, LandscapeSpawnRegions.encounterMap[i], newEncounterMap[i]);
                                sql = $"UPDATE encounter SET weenie_class_id = {encounters[j].weenie_class_id} WHERE id = {encounters[j].id}";
                                command = new MySqlCommand(sql, connection);
                                count += command.ExecuteNonQuery();
                            }
                        }
                        i++;
                    }
                }
                Console.WriteLine($"Updated {count} entries.");
            }
            connection.Close();
        }
    }
}