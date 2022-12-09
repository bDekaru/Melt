using System;
using System.Collections.Generic;
using System.Xml;

namespace Melt
{
    public class GoArrowUtilities
    {
        public class GoArrowLocation
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

        List<GoArrowLocation> Locations = new List<GoArrowLocation>();

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

                Locations.Add(newLoc);
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

                idNode.InnerText = entry.id.ToString();
                latitudeNode.InnerText = entry.latitude;
                logitudeNode.InnerText = entry.longitude;
                nameNode.InnerText = entry.name;
                typeNode.InnerText = entry.type;
                arrivalLatitudeNode.InnerText = entry.arrival_latitude;
                arrivalLogitudeNode.InnerText = entry.arrival_longitude;
                descriptionNode.InnerText = entry.description;
                date_addedNode.InnerText = entry.date_added;
                last_updateNode.InnerText = entry.last_update;
                dungeon_idNode.InnerText = entry.dungeon_id;
                retiredNode.InnerText = entry.retired;
                tod_reqNode.InnerText = entry.tod_req;
            }

            xmlDoc.Save(filename);
        }

        public void ReIndex()
        {
            int newKey = 1;
            for(int i = 0; i < Locations.Count; i++)
            {
                Locations[i].id = newKey++;
            }
        }

        public enum Town
        {
            Holtburg,
            Cragstone,
            Arwic,
            Dryreach,
            Eastham,
            FortTethana,
            GlendenWood,
            Lytelthorpe,
            PlateauVillage,
            Rithwic,
            Stonehold,
            UndergroundCity,
            Shoushi,
            HebianTo,
            Baishi,
            Kara,
            Kryst,
            Lin,
            Mayoi,
            Nanto,
            Sawato,
            WaiJhou,
            Yanshi,
            TouTou,
            Yaraq,
            Zaikhal,
            AlArqas,
            AlJalima,
            AyanBaqur,
            Khayyaban,
            Qalabar,
            Samsur,
            Tufa,
            Uziz,
            Xarabydun,
            Greenspire,
            Redspire,
            Bluespire,
            CandethKeep,
            MacNiallsFreehold,
            NeydisaCastle,
            BanditCastle,
            DanbysOutpost
        }

        public class Coords
        {
            public string latitude;
            public string longitude;

            public Coords(double NS, double WE)
            {
                latitude = NS.ToString();
                longitude = WE.ToString();
            }
        }

        public Dictionary<Town, Coords> TownDepartureCoords = new Dictionary<Town, Coords>()
        {
            { Town.Holtburg, new Coords(-42.1, 33.9) },
            { Town.Cragstone, new Coords(-25.5, 48.7) },
            { Town.Arwic, new Coords(-33.8, 56.7) },
            { Town.Dryreach, new Coords(8.2, 72.9) },
            { Town.Eastham, new Coords(-17.6, 63.2) },
            { Town.FortTethana, new Coords(-1.7, -72.1) },
            { Town.GlendenWood, new Coords(-29.7, 27.0) },
            { Town.Lytelthorpe, new Coords(-0.6, 51.6) },
            { Town.PlateauVillage, new Coords(-44.0, -43.1) },
            { Town.Rithwic, new Coords(-10.8, 58.1) },
            { Town.Stonehold, new Coords(-69.0, -21.7) },
            { Town.UndergroundCity, new Coords(-21.3, 53.9) },
            { Town.Shoushi, new Coords(33.9, 72.8) },
            { Town.HebianTo, new Coords(39.6, 83.5) },
            { Town.Baishi, new Coords(49.3, 62.4) },
            { Town.Kara, new Coords(83.2, 47.1) },
            { Town.Kryst, new Coords(74.3, 84.2) },
            { Town.Lin, new Coords(54.3, 73.2) },
            { Town.Mayoi, new Coords(61.8, 81.6) },
            { Town.Nanto, new Coords(52.3, 82.0) },
            { Town.Sawato, new Coords(28.8, 59.5) },
            { Town.WaiJhou, new Coords(61.9, -51.5) },
            { Town.Yanshi, new Coords(12.7, 46.4) },
            { Town.TouTou, new Coords(28.2, 95.6) },
            { Town.Yaraq, new Coords(21.5, -1.3) },
            { Town.Zaikhal, new Coords(-13.4, 0.6) },
            { Town.AlArqas, new Coords(31.1, 14.0) },
            { Town.AlJalima, new Coords(-7.3, 4.6) },
            { Town.AyanBaqur, new Coords(59.9, -88.2) },
            { Town.Khayyaban, new Coords(47.4, 25.1) },
            { Town.Qalabar, new Coords(74.1, 19.2) },
            { Town.Samsur, new Coords(3.2, 19.1) },
            { Town.Tufa, new Coords(14.2, 5.5) },
            { Town.Uziz, new Coords(24.9, 28.3) },
            { Town.Xarabydun, new Coords(41.9, 15.9) },
            { Town.Greenspire, new Coords(-43.3, -66.8) },
            { Town.Redspire, new Coords(-40.6, -82.9) },
            { Town.Bluespire, new Coords(-39.4, -75.1) },
            { Town.CandethKeep, new Coords(87.4, -66.9) },
            { Town.MacNiallsFreehold, new Coords(74.1, 92.3) },
            { Town.NeydisaCastle, new Coords(-69.5, 17.6) },
            { Town.BanditCastle, new Coords(-66.4, 50.0) },
            { Town.DanbysOutpost, new Coords(-23.5, -28.6) },
        };

        public Dictionary<Town, Coords> TownArrivalCoords = new Dictionary<Town, Coords>()
        {
            { Town.Holtburg, new Coords(-41.6, 33.7) },
            { Town.Cragstone, new Coords(-26.1, 48.1) },
            { Town.Arwic, new Coords(-33.6, 56.8) },
            { Town.Dryreach, new Coords(8.1, 72.9) },
            { Town.Eastham, new Coords(-16.9, 63.5) },
            { Town.FortTethana, new Coords(-1.5, -72.1) },
            { Town.GlendenWood, new Coords(-29.7, 26.5) },
            { Town.Lytelthorpe, new Coords(-1.9, 51.9) },
            { Town.PlateauVillage, new Coords(-44.5, -43.1) },
            { Town.Rithwic, new Coords(-10.8, 59.3) },
            { Town.Stonehold, new Coords(-68.8, -21.6) },
            { Town.UndergroundCity, new Coords(-21.3, 53.9) },
            { Town.Shoushi, new Coords(33.5, 72.8) },
            { Town.HebianTo, new Coords(38.9, 82.6) },
            { Town.Baishi, new Coords(49.3, 62.9) },
            { Town.Kara, new Coords(83.3, 47.1) },
            { Town.Kryst, new Coords(74.6, 84.2) },
            { Town.Lin, new Coords(54.1, 73.3) },
            { Town.Mayoi, new Coords(61.9, 82.5) },
            { Town.Nanto, new Coords(52.2, 82.5) },
            { Town.Sawato, new Coords(29.1, 58.9) },
            { Town.WaiJhou, new Coords(62.1, -51.5) },
            { Town.Yanshi, new Coords(12.5, 46.6) },
            { Town.TouTou, new Coords(30.4, 94.7) },
            { Town.Yaraq, new Coords(21.5, -1.8) },
            { Town.Zaikhal, new Coords(-13.5, 0.7) },
            { Town.AlArqas, new Coords(31.3, 13.1) },
            { Town.AlJalima, new Coords(-6.4, 6.3) },
            { Town.AyanBaqur, new Coords(60.5, -88.0) },
            { Town.Khayyaban, new Coords(47.4, 25.6) },
            { Town.Qalabar, new Coords(73.9, 18.9) },
            { Town.Samsur, new Coords(2.7, 18.9) },
            { Town.Tufa, new Coords(13.9, 5.4) },
            { Town.Uziz, new Coords(24.8, 28.4) },
            { Town.Xarabydun, new Coords(41.9, 16.2) },
            { Town.Greenspire, new Coords(-43.2, -66.8) },
            { Town.Redspire, new Coords(-40.7, -82.5) },
            { Town.Bluespire, new Coords(-39.5, -75.3) },
            { Town.CandethKeep, new Coords(87.5, -67.0) },
            { Town.MacNiallsFreehold, new Coords(74, 92.3) },
            { Town.NeydisaCastle, new Coords(-69.5, 17.9) },
            { Town.BanditCastle, new Coords(-66.5, 50.0) },
            { Town.DanbysOutpost, new Coords(-23.4, -28.8) },
        };

        public enum Halls
        {
            Atrium,
            Haven,
            Oriel,
            Sanctum,
            Victory
        }

        public Dictionary<Town, Halls> HallEntrances = new Dictionary<Town, Halls>()
        {
            { Town.Holtburg, Halls.Oriel },
            { Town.Cragstone, Halls.Sanctum },
            { Town.Arwic, Halls.Haven },
            { Town.Dryreach, Halls.Sanctum },
            { Town.Eastham, Halls.Oriel },
            { Town.FortTethana, Halls.Victory },
            { Town.GlendenWood, Halls.Atrium },
            { Town.Lytelthorpe, Halls.Haven },
            { Town.PlateauVillage, Halls.Oriel },
            { Town.Rithwic, Halls.Haven },
            { Town.Stonehold, Halls.Victory },
            { Town.UndergroundCity, Halls.Haven },
            { Town.Shoushi, Halls.Haven },
            { Town.HebianTo, Halls.Sanctum },
            { Town.Baishi, Halls.Atrium },
            { Town.Kara, Halls.Haven },
            { Town.Kryst, Halls.Sanctum },
            { Town.Lin, Halls.Atrium },
            { Town.Mayoi, Halls.Oriel },
            { Town.Nanto, Halls.Sanctum },
            { Town.Sawato, Halls.Haven },
            { Town.WaiJhou, Halls.Victory },
            { Town.Yanshi, Halls.Oriel },
            { Town.TouTou, Halls.Haven },
            { Town.Yaraq, Halls.Atrium },
            { Town.Zaikhal, Halls.Sanctum },
            { Town.AlArqas, Halls.Haven },
            { Town.AlJalima, Halls.Atrium },
            { Town.AyanBaqur, Halls.Victory },
            { Town.Khayyaban, Halls.Oriel },
            { Town.Qalabar, Halls.Atrium },
            { Town.Samsur, Halls.Oriel },
            { Town.Tufa, Halls.Oriel },
            { Town.Uziz, Halls.Haven },
            { Town.Xarabydun, Halls.Oriel },
            { Town.Greenspire, Halls.Sanctum },
            { Town.Redspire, Halls.Atrium },
            { Town.Bluespire, Halls.Sanctum },
            { Town.CandethKeep, Halls.Victory },
            { Town.MacNiallsFreehold, Halls.Sanctum },
            { Town.NeydisaCastle, Halls.Atrium },
            { Town.BanditCastle, Halls.Sanctum },
            { Town.DanbysOutpost, Halls.Oriel },
        };

        public void AddApartmentHallConnections()
        {
            Dictionary<Town, string> Atrium = new Dictionary<Town, string>();
            Atrium.Add(Town.GlendenWood, "Alphus Court"); // main hall portal
            Atrium.Add(Town.Baishi, "Gajin Dwellings"); // main hall portal
            Atrium.Add(Town.Yaraq, "Hasina Gardens");
            Atrium.Add(Town.Lin, "Heartland Yard");
            Atrium.Add(Town.Qalabar, "Ivory Gate"); // main hall portal
            //Atrium.Add(Town., "Larkspur Gardens"); // not used
            //Atrium.Add(Town., "Mellas Court"); // not used
            Atrium.Add(Town.AlJalima, "Valorya Gate");
            Atrium.Add(Town.Redspire, "Vesper Gate");
            Atrium.Add(Town.NeydisaCastle, "Winthur Gate");

            Dictionary<Town, string> Haven = new Dictionary<Town, string>();
            Haven.Add(Town.Kara, "Ben Ten Lodge");
            Haven.Add(Town.Lytelthorpe, "Cedraic Court"); // main hall portal
            Haven.Add(Town.Rithwic, "Celcynd Grotto");
            Haven.Add(Town.Uziz, "Crescent Moon Veranda"); // main hall portal
            Haven.Add(Town.AlArqas, "Dulok Court");
            Haven.Add(Town.Arwic, "Ispar Yard");
            Haven.Add(Town.Sawato, "Jade Gate");
            Haven.Add(Town.Shoushi, "Jojii Gardens");
            Haven.Add(Town.UndergroundCity, "Trothyr Hollow");
            Haven.Add(Town.TouTou, "Xao Wu Gardens"); // main hall portal

            Dictionary<Town, string> Oriel = new Dictionary<Town, string>();
            Oriel.Add(Town.PlateauVillage, "Allain Court"); // main hall portal
            Oriel.Add(Town.Eastham, "Autumn Moon Gardens");
            Oriel.Add(Town.Xarabydun, "Endara Gate");
            Oriel.Add(Town.Samsur, "Forsythian Gardens");
            Oriel.Add(Town.Tufa, "Maru Veranda");
            Oriel.Add(Town.Khayyaban, "Sorac Gate"); // main hall portal
            Oriel.Add(Town.Mayoi, "Syrah Dwellings"); // main hall portal
            Oriel.Add(Town.Holtburg, "Trellyn Gardens");
            Oriel.Add(Town.DanbysOutpost, "Vindalan Dwellings");
            Oriel.Add(Town.Yanshi, "White Lotus Gate");

            Dictionary<Town, string> Sanctum = new Dictionary<Town, string>();
            Sanctum.Add(Town.Cragstone, "Alvan Court"); // main hall portal
            Sanctum.Add(Town.Greenspire, "Caerna Dwellings");
            Sanctum.Add(Town.BanditCastle, "Illsin Veranda");
            Sanctum.Add(Town.HebianTo, "Marin Court"); // main hall portal
            Sanctum.Add(Town.Zaikhal, "Ruadnar Court"); // main hall portal
            Sanctum.Add(Town.Kryst, "Senmai Court");
            Sanctum.Add(Town.Dryreach, "Sigil Veranda");
            Sanctum.Add(Town.Bluespire, "Sorveya Court");
            Sanctum.Add(Town.MacNiallsFreehold, "Sylvan Dwellings");
            Sanctum.Add(Town.Nanto, "Treyval Veranda");

            Dictionary<Town, string> Victory = new Dictionary<Town, string>();
            Victory.Add(Town.Stonehold, "Accord Veranda");
            Victory.Add(Town.CandethKeep, "Candeth Court");
            Victory.Add(Town.FortTethana, "Celdiseth Court");
            //Victory.Add(Town., "Festivus Court"); // not used
            //Victory.Add(Town., "Hibiscus Gardens"); // not used
            //Victory.Add(Town., "Meditation Gardens"); // not used
            //Victory.Add(Town., "Setera Gardens"); // not used
            Victory.Add(Town.WaiJhou, "Spirit Gate");
            Victory.Add(Town.AyanBaqur, "Triumphal Gardens");
            //Victory.Add(Town., "Wilamil Court"); // not used

            foreach (var entry in HallEntrances)
            {
                switch(entry.Value)
                {
                    case Halls.Atrium:
                        AddPortals(entry.Value, entry.Key, Atrium);
                        break;
                    case Halls.Haven:
                        AddPortals(entry.Value, entry.Key, Haven);
                        break;
                    case Halls.Oriel:
                        AddPortals(entry.Value, entry.Key, Oriel);
                        break;
                    case Halls.Sanctum:
                        AddPortals(entry.Value, entry.Key, Sanctum);
                        break;
                    case Halls.Victory:
                        AddPortals(entry.Value, entry.Key, Victory);
                        break;
                }
            }
        }

        private void AddPortals(Halls hall, Town town, Dictionary<Town, string> portals)
        {
            foreach (var portal in portals)
            {
                if (town != portal.Key)
                {
                    GoArrowLocation newLocation = new GoArrowLocation();
                    newLocation.id = 0; // we will fill this in later with a re-index.
                    newLocation.name = $"{hall} Halls - {portal.Value} - {GetFriendlyTownName(portal.Key)}";
                    newLocation.type = "Portal";
                    newLocation.latitude = TownDepartureCoords[town].latitude;
                    newLocation.longitude = TownDepartureCoords[town].longitude;
                    newLocation.arrival_latitude = TownArrivalCoords[portal.Key].latitude;
                    newLocation.arrival_longitude = TownArrivalCoords[portal.Key].longitude;
                    newLocation.date_added = "2021-01-31T19:00:00.0000000-03:00";
                    newLocation.last_update = "2021-01-31T19:00:00.0000000-03:00";
                    newLocation.retired = "N";
                    newLocation.tod_req = "0";

                    Locations.Add(newLocation);
                }
            }
        }

        private string GetFriendlyTownName(Town town)
        {
            string friendlyTownName;
            switch (town)
            {
                case Town.AlArqas:
                    friendlyTownName = "Al-Arqas";
                    break;
                case Town.AlJalima:
                    friendlyTownName = "Al-Jalima";
                    break;
                case Town.AyanBaqur:
                    friendlyTownName = "Ayan Baqur";
                    break;
                case Town.BanditCastle:
                    friendlyTownName = "Bandit Castle";
                    break;
                case Town.CandethKeep:
                    friendlyTownName = "Candeth Keep";
                    break;
                case Town.DanbysOutpost:
                    friendlyTownName = "Danby's Outpost";
                    break;
                case Town.FortTethana:
                    friendlyTownName = "Fort Tethana";
                    break;
                case Town.GlendenWood:
                    friendlyTownName = "Glenden Wood";
                    break;
                case Town.MacNiallsFreehold:
                    friendlyTownName = "MacNiall's Freehold";
                    break;
                case Town.NeydisaCastle:
                    friendlyTownName = "Neydisa Castle";
                    break;
                case Town.PlateauVillage:
                    friendlyTownName = "Plateau Village";
                    break;
                case Town.TouTou:
                    friendlyTownName = "Tou-Tou";
                    break;
                case Town.UndergroundCity:
                    friendlyTownName = "Underground City";
                    break;
                default:
                    friendlyTownName = town.ToString();
                    break;
            }
            return friendlyTownName;
        }
    }
}