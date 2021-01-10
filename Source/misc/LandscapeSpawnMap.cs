using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Melt
{
    public class LandscapeSpawnMap
    {
        public class Encounter
        {
            public uint key;
            public List<uint> value;
        }

        public class RegionFile
        {
            public int tableCount;
            public int tableSize;
            public List<ushort> encounterMap;
            public Encounter[] encounters;
        }

        Dictionary<int, Color> Colors = new Dictionary<int, Color>();

        public LandscapeSpawnMap(string filename)
        {
            Colors.Add(0, Color.FromArgb(0, 0, 0));
            Colors.Add(8, Color.FromArgb(255, 128, 128));
            Colors.Add(16, Color.FromArgb(255, 255, 128));
            Colors.Add(23, Color.FromArgb(128, 255, 128));
            Colors.Add(24, Color.FromArgb(0, 255, 128));
            Colors.Add(32, Color.FromArgb(128, 255, 255));
            Colors.Add(40, Color.FromArgb(0, 128, 255));
            Colors.Add(48, Color.FromArgb(255, 128, 192));
            Colors.Add(56, Color.FromArgb(255, 128, 255));
            Colors.Add(64, Color.FromArgb(255, 0, 0));
            Colors.Add(72, Color.FromArgb(255, 255, 0));
            Colors.Add(80, Color.FromArgb(128, 255, 0));
            Colors.Add(88, Color.FromArgb(0, 255, 64));
            Colors.Add(96, Color.FromArgb(0, 255, 255));
            Colors.Add(104, Color.FromArgb(0, 128, 192));
            Colors.Add(112, Color.FromArgb(128, 128, 192));
            Colors.Add(160, Color.FromArgb(255, 0, 255));
            Colors.Add(184, Color.FromArgb(128, 64, 64));
            Colors.Add(192, Color.FromArgb(255, 128, 64));
            Colors.Add(200, Color.FromArgb(0, 255, 0));
            Colors.Add(207, Color.FromArgb(0, 128, 128));
            Colors.Add(208, Color.FromArgb(0, 64, 128));
            Colors.Add(215, Color.FromArgb(128, 128, 255));
            Colors.Add(216, Color.FromArgb(128, 0, 64));
            Colors.Add(223, Color.FromArgb(255, 0, 128));
            Colors.Add(224, Color.FromArgb(128, 0, 0));
            Colors.Add(232, Color.FromArgb(255, 128, 0));
            Colors.Add(240, Color.FromArgb(0, 128, 0));

            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));

            string jsonData = inputFile.ReadToEnd();
            inputFile.Close();

            RegionFile regionFile;
            regionFile = JsonConvert.DeserializeObject<RegionFile>(jsonData);

            int sizeMultiplier = 8;
            Bitmap bmp = new Bitmap((regionFile.tableSize - 1) * sizeMultiplier, (regionFile.tableSize - 1) * sizeMultiplier);

            int i = 0;
            for(int y = 0; y < regionFile.tableSize - 1; y++)
            {
                for (int x = regionFile.tableSize - 2; x >= 0 ; x--)
                {
                    int value = regionFile.encounterMap[i];
                    for(int offsetX = 0; offsetX < sizeMultiplier; offsetX++)
                        for (int offsetY = 0; offsetY < sizeMultiplier; offsetY++)
                            bmp.SetPixel((y * sizeMultiplier)+ offsetY, (x * sizeMultiplier)+offsetX, Colors[value]);
                    i++;
                }
            }

            bmp.Save("Landscape Spawn Map.png", ImageFormat.Png);
        }
    }
}