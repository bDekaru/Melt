using System.Collections.Generic;
using System.Xml;

namespace Melt
{
    public class GoArrowUtilities
    {
        public struct GoArrowLocation
        {
            public int id;
            public string latitude;
            public string longitude;
            public string name;
            public string type;
            public string arrival_latitude;
            public string arrival_longitude;
            public string description;
            public string date_added;
            public string last_update;
            public string dungeon_id;
            public string retired;
            public string tod_req;
        }

        Dictionary<int, GoArrowLocation> Locations = new Dictionary<int, GoArrowLocation>();

        public GoArrowUtilities(string filename)
        {
            XmlDocument file = new XmlDocument();
            file.Load(filename);

            foreach (XmlNode node in file.DocumentElement.ChildNodes)
            {
                GoArrowLocation newLoc = new GoArrowLocation();
                foreach (XmlNode locNode in node)
                {
                    if (locNode.Name == "id")
                        newLoc.id = int.Parse(locNode.InnerText);
                    else if (locNode.Name == "latitude")
                        newLoc.latitude = locNode.InnerText;
                    else if (locNode.Name == "longitude")
                        newLoc.longitude = locNode.InnerText;
                    else if (locNode.Name == "name")
                        newLoc.name = locNode.InnerText;
                    else if (locNode.Name == "type")
                        newLoc.type = locNode.InnerText;
                    else if (locNode.Name == "arrival_latitude")
                        newLoc.arrival_latitude = locNode.InnerText;
                    else if (locNode.Name == "arrival_longitude")
                        newLoc.arrival_longitude = locNode.InnerText;
                    else if (locNode.Name == "description")
                        newLoc.description = locNode.InnerText;
                    else if (locNode.Name == "date_added")
                        newLoc.date_added = locNode.InnerText;
                    else if (locNode.Name == "last_update")
                        newLoc.last_update = locNode.InnerText;
                    else if (locNode.Name == "dungeon_id")
                        newLoc.dungeon_id = locNode.InnerText;
                    else if (locNode.Name == "retired")
                        newLoc.retired = locNode.InnerText;
                    else if (locNode.Name == "tod_req")
                        newLoc.tod_req = locNode.InnerText;
                }
                Locations.Add(newLoc.id, newLoc);
            }
        }

        public void Save(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode docNode = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            xmlDoc.AppendChild(docNode);

            XmlNode rootNode = xmlDoc.CreateElement("atlas");
            xmlDoc.AppendChild(rootNode);

            XmlAttribute attribute = xmlDoc.CreateAttribute("xmlns:xsi");
            attribute.Value = "http://www.w3.org/2001/XMLSchema-instance";
            rootNode.Attributes.Append(attribute);

            foreach (var entry in Locations)
            {
                XmlNode locationNode = xmlDoc.CreateElement("location");
                XmlNode idNode = xmlDoc.CreateElement("id");
                XmlNode latitudeNode = xmlDoc.CreateElement("latitude");
                XmlNode logitudeNode = xmlDoc.CreateElement("longitude");
                XmlNode nameNode = xmlDoc.CreateElement("name");
                XmlNode typeNode = xmlDoc.CreateElement("type");
                XmlNode arrivalLatitudeNode = xmlDoc.CreateElement("arrival_latitude");
                XmlNode arrivalLogitudeNode = xmlDoc.CreateElement("arrival_longitude");
                XmlNode descriptionNode = xmlDoc.CreateElement("description");
                XmlNode date_addedNode = xmlDoc.CreateElement("date_added");
                XmlNode last_updateNode = xmlDoc.CreateElement("last_update");
                XmlNode dungeon_idNode = xmlDoc.CreateElement("dungeon_id");
                XmlNode retiredNode = xmlDoc.CreateElement("retired");
                XmlNode tod_reqNode = xmlDoc.CreateElement("tod_req");
                rootNode.AppendChild(locationNode);
                locationNode.AppendChild(idNode);
                locationNode.AppendChild(latitudeNode);
                locationNode.AppendChild(logitudeNode);
                locationNode.AppendChild(nameNode);
                locationNode.AppendChild(typeNode);
                locationNode.AppendChild(arrivalLatitudeNode);
                locationNode.AppendChild(arrivalLogitudeNode);
                locationNode.AppendChild(descriptionNode);
                locationNode.AppendChild(date_addedNode);
                locationNode.AppendChild(last_updateNode);
                locationNode.AppendChild(dungeon_idNode);
                locationNode.AppendChild(retiredNode);
                locationNode.AppendChild(tod_reqNode);

                idNode.InnerText = entry.Key.ToString();
                latitudeNode.InnerText = entry.Value.latitude;
                logitudeNode.InnerText = entry.Value.longitude;
                nameNode.InnerText = entry.Value.name;
                typeNode.InnerText = entry.Value.type;
                arrivalLatitudeNode.InnerText = entry.Value.arrival_latitude;
                arrivalLogitudeNode.InnerText = entry.Value.arrival_longitude;
                descriptionNode.InnerText = entry.Value.description;
                date_addedNode.InnerText = entry.Value.date_added;
                last_updateNode.InnerText = entry.Value.last_update;
                dungeon_idNode.InnerText = entry.Value.dungeon_id;
                retiredNode.InnerText = entry.Value.retired;
                tod_reqNode.InnerText = entry.Value.tod_req;
            }

            xmlDoc.Save(filename);
        }

        public string[] settlementPortals =
        {
            "Adam's Beach",
            "Adept's Domain",
            "Adventurer's Haven Cottages",
            "Ahr-Zona",
            "Aimaru Plains Cottages",
            "Al-Arqis Cottages",
            "Al-Hatar Settlement",
            "Al-Kasan Settlement",
            "Al-Mar Oasis",
            "Al-Nosaj",
            "Alfreth Ridge Cottages",
            "Aloria",
            "Alvador",
            "Amarand Villas",
            "Anadil",
            "Aqalah",
            "Ardent Realm",
            "Ariake",
            "Arida Butte",
            "Arqasanti",
            "Arrak",
            "Arrowdale Cottages",
            "Artifice Cottages",
            "Asandra Cottages",
            "Asbel Domain",
            "Auralla Settlement",
            "Auroch Pasture Estates",
            "Avatania Cottages",
            "Axefall Glen",
            "Ayn Tayan",
            "Azaroth Cottages",
            "Bandit Road Villas",
            "Baron's Domain",
            "Bay of Sands",
            "Beach Pass Villas",
            "Bellig Mesa Cottages",
            "Bhah Dhah Villas",
            "Blackmire Edge Cottages",
            "Blazing Wand Villas",
            "Bleached Skull Wastes Settlement",
            "Bleak Valley",
            "Bluewater Cottages",
            "Brazenn Domain",
            "Bretslef Cottages",
            "Brigand Sands Cottages",
            "Brigands Bay Settlement",
            "Bright Blade Cottages",
            "Broadacre Cottages",
            "Broken Haft Vale",
            "Broken Sword Bethel Settlement",
            "Bucolic Villas",
            "Cactus Acres",
            "Caerlin Cottages",
            "Cape Feirgard Cottages",
            "Celcynd Cottages",
            "Celdiseth's Beach Settlement",
            "Charbone Ridge",
            "Charnhold",
            "Chi Zou Cottages",
            "Colier View Hill",
            "Cragstone Farms",
            "Crescent Lane Cottages",
            "Dagger Vale",
            "Dame Tolani Villas",
            "Darasa Villas",
            "Darawyll Village",
            "Deepvale Cottages",
            "Defiant Prey Cottages",
            "Demonsbane Cottages",
            "Desert Boundary Cottages",
            "Desert Mirage Cottages",
            "Desert Vanguard Cottages",
            "Desolation Beach",
            "Devana",
            "Dillo Butte Settlement",
            "Dire's Edge",
            "Dires' Door Estates",
            "Direvale Villas",
            "Djebel al-Nar Cottages",
            "Djinaya Wind Cottages",
            "Dovetail Valley Villas",
            "Dry Bone Manors",
            "Dryreach Beach Cottages",
            "Dryreach Beach Manors",
            "Drytree Settlement",
            "Dunes End Cottages",
            "Durglen",
            "East Al-Jalima Villas",
            "East Danby",
            "East Esper Valley",
            "East Lytelthorpe Settlement",
            "East Morntide Settlement",
            "East Morntide Villas",
            "East Rithwic Estates",
            "East Sawato Settlement",
            "East Span Way Settlement",
            "Eaves of Tiofor Settlement",
            "Ebbing Tide Villas",
            "Embara",
            "Empyrean Fields Cottages",
            "Empyrean Shore Villas",
            "Enchanter's Meadow",
            "Eotensfang Cottages",
            "Erevana Villas",
            "Evensong Settlement",
            "Explorer's Villas",
            "Fadsahil Settlement",
            "Far Claw Villas",
            "Far Horizon Cottages",
            "Faranar Foothills",
            "Fearnot Valley Cottages",
            "Filos' Font Cottages",
            "Firedew",
            "Firesong Cottages",
            "Font Alpa",
            "Forgotten Hills",
            "Four Towers Settlement",
            "Frosty Dale Cottages",
            "Gaerwel Edge Settlement",
            "Genem Causland",
            "Gharu'n Victory Villas",
            "Glenden Hills East Settlement",
            "Glenden Hills North Settlement",
            "Glystaene Cottages",
            "Gredaline Villas",
            "Greenswath",
            "Greenvale Settlement",
            "Haliana",
            "Hamud Cottages",
            "Hand-on-Sword Cottages",
            "Harmonious Blade Cottages",
            "Helms Villas",
            "Hermit Hill Cottages",
            "Hero's Vale",
            "Hidden Valley",
            "Highland Manors",
            "Holtburg Wilderness Settlement",
            "Hopevale",
            "Howling Wolf Villas",
            "Ianna",
            "Ice's Edge Cottages",
            "Iceea Hills Estates",
            "Ijaniya",
            "Ikama Cottages",
            "Imuth Maer Cottages",
            "Inquisitor's Dale Cottages",
            "Ishilai Inlet Settlement",
            "Ishilai Inlet Villas",
            "Ispan Hill",
            "Isparian Flame Estates",
            "Jackcat Canyon",
            "Jai-Tan Dale",
            "Janaa Ridge Settlement",
            "Jasmine Meadow",
            "Jenshi Cottages",
            "Jeweled Thorn Estates",
            "Jin-Lai Stronghold",
            "Jinianshi",
            "Kanasa",
            "Kelnen Village",
            "King Pwyll Square",
            "Kuyiza",
            "Lady Maila Estates",
            "Laiti's Villa",
            "Lake Blessed Cottages",
            "Lake Nemuel Settlement",
            "Lake Thrasyl Cottages",
            "Lanadryll Cottages",
            "Land Bridge Villas",
            "Last-Stop-Before-Dires Villas",
            "Leafdawning Settlement",
            "Li-Po Cottages",
            "Liang Chi Settlement",
            "Lightbringer Dale Cottages",
            "Lilyglen Cottages",
            "Lin Kiln Park",
            "Linvak Tukal Foothills Settlement",
            "Lithaenean Cottages",
            "Lo-Han",
            "Lord Cambarth Villas",
            "Loredane Villas",
            "Lost Realm Cottages",
            "Lost Wish Cottages",
            "Lugian Meadows Settlement",
            "Maedew",
            "Mage's Pass",
            "Mahara Cottages",
            "Maitland",
            "Majestic Hill Cottages",
            "Majestic Saddle Cottages",
            "Mattekar Slopes Cottages",
            "Maythen Geroyu Villas",
            "Meditation Meadow",
            "Meerthus Square",
            "Merak",
            "Meridian Cottages",
            "Midhill Cottages",
            "Midsong Cottages",
            "Mimiana Villas",
            "Mire Hill",
            "Mirthless Dale",
            "Mistdweller Villas",
            "Monument Sands Settlement",
            "Morntide Ascent Cottages",
            "Mosswart Place Cottages",
            "Mountain Keep Cottages",
            "Mountain Retreat Cottages",
            "Mountain Ridge Abodes",
            "Musansayn",
            "Nal Wadi Cottages",
            "Nalib Cavana Settlement",
            "Nan-Zari",
            "Naqut Dreams Villas",
            "Narsys",
            "Narziz Cottages",
            "Neu Gerz Villas",
            "New Cannthalo",
            "New Colier",
            "New Nesortania",
            "New Suntik",
            "Neydisa Village",
            "Nidal-Taraq Villas",
            "Norstead",
            "North Adjamaer Cottages",
            "North Baishi Cottages",
            "North Eastham Meadow",
            "North Lytelthorpe Villas",
            "North Pass Hollow Cottages",
            "North Sawato Villas",
            "North Uziz Settlement",
            "North Yanshi Plains Settlement",
            "North Yaraq Villas",
            "Northfire Estates",
            "Northreach",
            "Northwater Cottages",
            "Norvale",
            "Oboro",
            "Ong-Hau Village",
            "Osric Cottages",
            "P'rnelle Acres",
            "Palm-of-Stone Villas",
            "Patron's Honor Cottages",
            "Pavanne Vale Freehold",
            "Peril's Edge Cottages",
            "Pillars-on-the-Sea Cottages",
            "Pine Deep",
            "Pine Hillock Settlement",
            "Plainsview Cottages",
            "Plateau Hollow Settlement",
            "Point Tremblant",
            "Prosper River Headwaters",
            "Qalaba'r Seaside Villas",
            "Qalabar Oasis Settlement",
            "Rahvard Square",
            "Redrock Cottages",
            "Regina Cottages",
            "Rending Talon Cottages",
            "Rethux Vale",
            "Return of Mumiyah Cottages",
            "Reviled Maw Cottages",
            "Rhynntal Cottages",
            "Ring of Crystals Estates",
            "Riverbend Cottages",
            "Rivermouth Villas",
            "Ro-Nan",
            "Rytheran Dale",
            "Sai-Nan",
            "Samsur Butte Cottages",
            "San-Chin",
            "Sanai",
            "Sanam Batal Villas",
            "Sand Kings Cottages",
            "Sand Shallow Cottages",
            "Sand's Edge",
            "Sands-of-the-Skull Cottages",
            "Sawato Foothills Settlement",
            "Scimitar Lake Cottages",
            "Sclavavania",
            "Seaview Ridge Cottages",
            "Secluded Valley Cottages",
            "Sennon Valley Retreat",
            "Serpent Hills Settlement",
            "Shaky Ledge Cottages",
            "Shara",
            "Sharvale",
            "Shian-To Cottages",
            "Shield of Ispar Villas",
            "Shield of Valor Cottages",
            "Shore Vista Cottages",
            "Shou-Zin",
            "Siege Road Settlement",
            "Siege Road Villas",
            "Simda'r Villas",
            "Slinker Meadows",
            "Sliver-of-BlueCottages",
            "Snakehead",
            "Snowy Valley",
            "Solstice Hill",
            "Soltan Villas",
            "Sonel",
            "Songview",
            "Sonpay",
            "South Adjamaer Cottages",
            "South Beach Pass Villas",
            "South Hebian-To Cottages",
            "South Hebian-To Estates",
            "South Khayyaban Cottages",
            "South Lytelthorpe",
            "South Shoushi Villas",
            "South Siege Villas",
            "South Uziz Villas",
            "South Victory Harbor",
            "South Yaraq Cottages",
            "Southeast Arwic Settlement",
            "Southern Park",
            "Southwest Hebian-To Settlement",
            "Spire Hills Settlement",
            "Stone Face Oasis Villas",
            "Stone Scar Settlement",
            "Stone Scythe Stronghold",
            "Stone Triad Dell",
            "Stonebend Cottages",
            "Stoneport Villas",
            "Stonerune Cottages",
            "Stormbrow",
            "Stormtree Villas",
            "Strathelar's Watch Cottages",
            "Swamp Temple Place",
            "Sweet Maple Cottages",
            "Taklihuan Settlement",
            "Tanshi",
            "Taralla",
            "Tarn Vinara Villas",
            "Tattered Ridge",
            "Tharesun",
            "Thasali Cottages",
            "Thyrinn Cant Cottages",
            "Tia-Leh Homestead",
            "Tinkelo Hold Villas",
            "Tiofor Deeps",
            "Tou-Tou Penninsula Cottages",
            "Tou-Tou Road Villas",
            "Tusker Notch",
            "Two Hills Cottages",
            "Tyrrin Cottages",
            "Unified Heart Villas",
            "Verena",
            "Vesayan Overlook",
            "Village Quan",
            "Villalabar",
            "Vulture's Eye Villas",
            "West Baishi Settlement",
            "West Holtburg Villas",
            "West Mayoi Mountain Villas",
            "West Norstead",
            "West Rithwic Estates",
            "West Sawato Cottages",
            "West Uziz Settlement",
            "West Yanshi Namoon",
            "West Zaikhal Freehold",
            "Westshore Cottages",
            "Whispering Pines Cottages",
            "Wi Badlands Settlement",
            "Wilomine Villas",
            "Windrune Cottages",
            "Wisp Lake Cottages",
            "Wolfenvale",
            "Woodsbane Cottages",
            "Woodshore Cottages",
            "Xinh",
            "Yanshi Namoon North",
            "Yee Villas",
            "Yinar",
            "Yukikaze",
            "Yushad Ridge Cottages",
            "Zabool Overlook Settlement",
            "Zatara",
            "Zin-Dai",
            "N Adjamaer Cottages",
            "S Adjamaer Cottages",
            "S Khayyaban Cottages",
            "SE Arwic Settlement",
            "Stone Face Villas Portal",
            "Stonebend Settlement Portal",
            "Tia-Leh Portal",
            "Wisp Lake Portal",
            "Woodsbane Settlement",
        };

        public void RemoveSettlements(bool removePortals = true)
        {
            Dictionary<int, GoArrowLocation> tempLocations = new Dictionary<int, GoArrowLocation>();
            foreach (var entry in Locations)
            {
                if (entry.Value.type != "Village")
                    tempLocations.Add(entry.Key, entry.Value);
            }

            List<string> names = new List<string>();
            Dictionary<int, GoArrowLocation> tempLocations2 = new Dictionary<int, GoArrowLocation>();
            foreach (var name in settlementPortals)
            {
                bool found = false;
                foreach (var entry in tempLocations)
                {
                    if (entry.Value.type == "Portal" && entry.Value.name.Contains(name))
                    {
                        if (!tempLocations2.ContainsKey(entry.Key))
                        {
                            tempLocations2.Add(entry.Key, entry.Value);
                            names.Add(entry.Value.name);
                            found = true;
                        }
                    }
                }
                if (!found)
                    found = true;
            }

            Locations.Clear();
            int newKey = 1;
            foreach (var entry in tempLocations)
            {
                if(!tempLocations2.ContainsKey(entry.Key))
                    Locations.Add(newKey++, entry.Value);
            }
        }

        public void ReIndex()
        {
            int newKey = 1;
            Dictionary<int, GoArrowLocation> tempLocations = new Dictionary<int, GoArrowLocation>();
            foreach (var entry in Locations)
            {
                tempLocations.Add(newKey++, entry.Value);
            }
            Locations = tempLocations;
        }
    }
}