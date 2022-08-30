using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Melt
{
    public static class AceDatabaseUtilities
    {
        public static void FindScrollVendors()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = "SELECT object_Id, weenie_Class_Id FROM weenie_properties_create_list WHERE destination_Type = 4";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            Dictionary<int, List<int>> npcItemMap = new Dictionary<int, List<int>>();

            while (reader.Read())
            {
                int npcId = reader.GetInt32(0);
                int itemId = reader.GetInt32(1);
                List<int> npcItemList;
                if(!npcItemMap.TryGetValue(npcId, out npcItemList))
                    npcItemMap[npcId] = new List<int>();

                if(!npcItemMap[npcId].Contains(itemId))
                    npcItemMap[npcId].Add(itemId);
            }
            reader.Close();

            int count = 0;
            List<int> scrollVendorList = new List<int>();
            foreach (var npc in npcItemMap)
            {
                foreach (var itemId in npc.Value)
                {
                    sql = $"SELECT type FROM weenie WHERE class_Id = {itemId}";
                    command = new MySqlCommand(sql, connection);
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int type = reader.GetInt32(0);
                        if (type == 34 && !scrollVendorList.Contains(npc.Key))
                            scrollVendorList.Add(npc.Key);
                    }
                    reader.Close();
                }
            }

            List<string> locList = new List<string>();
            foreach (var npcId in scrollVendorList)
            {
                sql = $"SELECT`obj_Cell_Id`,`origin_X`,`origin_Y`,`origin_Z`,`angles_W`,`angles_X`,`angles_Y`,`angles_Z`FROM`landblock_instance`WHERE`weenie_Class_Id` = {npcId}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string obj_Cell_Id = reader.GetInt32(0).ToString("x8");
                    string origin_X = reader.GetFloat(1).ToString();
                    string origin_Y = reader.GetFloat(2).ToString();
                    string origin_Z = reader.GetFloat(3).ToString();
                    string angles_W = reader.GetFloat(4).ToString();
                    string angles_X = reader.GetFloat(5).ToString();
                    string angles_Y = reader.GetFloat(6).ToString();
                    string angles_Z = reader.GetFloat(7).ToString();
                    locList.Add($"{npcId} at {obj_Cell_Id} {origin_X} {origin_Z} {angles_W} {angles_X} {angles_Y} {angles_Z}");
                    count++;
                }
                reader.Close();

                sql = $"SELECT`object_Id`FROM`weenie_properties_create_list`WHERE`weenie_Class_Id` = {npcId}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string object_Id = reader.GetInt32(0).ToString();
                    locList.Add($"Create_List of weenie {object_Id}");
                    count++;
                }
                reader.Close();

                sql = $"SELECT`object_Id`FROM`weenie_properties_generator`WHERE`weenie_Class_Id`= {npcId}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string object_Id = reader.GetInt32(0).ToString();
                    locList.Add($"Generator of weenie {object_Id}");
                    count++;
                }
                reader.Close();
            }

            connection.Close();

            Console.WriteLine($"Found {count} entries.");
        }

        public static void BuildVendorSellList()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = $"SELECT `weenie_Class_Id`,COUNT(*) FROM `weenie_properties_create_list` WHERE `destination_Type`= 4 GROUP BY `weenie_Class_Id`";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            Dictionary<int, int> vendorItemsMap = new Dictionary<int, int>();

            while (reader.Read())
            {
                vendorItemsMap.Add(reader.GetInt32(0), reader.GetInt32(1));
            }
            reader.Close();

            int count = 0;
            StreamWriter outputFile = new StreamWriter(new FileStream("./vendorItemList.txt", FileMode.Create, FileAccess.Write));
            outputFile.WriteLine($"WeenieId\tWeenieClassName\tCount");
            foreach (var entry in vendorItemsMap)
            {
                 sql = $"SELECT `class_Name` FROM `weenie` WHERE `class_Id` = {entry.Key}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string weenieClassName = reader.GetString(0);
                    outputFile.WriteLine($"{entry.Key}\t{weenieClassName}\t{entry.Value}");
                    outputFile.Flush();
                    count++;
                }
                reader.Close();
            }
            connection.Close();
            outputFile.Close();

            Console.WriteLine($"Exported {count} entries.");
        }

        public static void RemoveAmmoFromSpecificIDs()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql;
            MySqlCommand command;

            List<int> vendors = new List<int>(){ 4693, 12718, 2317, 1045, 993, 1822, 4685, 1057, 982, 4702, 737, 1833, 718, 820, 676, 4444, 702, 2300, 2259, 656, 842, 2232, 4555, 24219, 2534 };
            int count = 0;
            foreach (var vendor in vendors)
            {
                sql = $"DELETE FROM weenie_properties_create_list WHERE object_Id = {vendor} AND destination_Type = 4 AND weenie_Class_Id IN (316,304,310,315,320,343,317,300,305,12464,3598,3599,3600,3601,3602,3603,3604,3605)";
                command = new MySqlCommand(sql, connection);
                count += command.ExecuteNonQuery();
            }
            connection.Close();

            Console.WriteLine($"Removed {count} entries.");
        }

        public static void RemoveAmmoFromBlacksmithsAndWeaponsmiths()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = "SELECT object_Id FROM weenie_properties_string WHERE `type` = 5 AND `value` IN (\"Blacksmith\",\"Weaponsmith\")";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<int> vendors = new List<int>();

            while (reader.Read())
            {
                vendors.Add(reader.GetInt32(0));
            }
            reader.Close();

            int count = 0;
            foreach (var vendor in vendors)
            {
                // Remove all existing ones so we can readd them in the correct order.
                sql = $"DELETE FROM weenie_properties_create_list WHERE object_Id = {vendor} AND destination_Type = 4 AND weenie_Class_Id IN (316,304,310,315,320,343,317,300,305,12464,3598,3599,3600,3601,3602,3603,3604,3605)";
                command = new MySqlCommand(sql, connection);
                count += command.ExecuteNonQuery();
            }
            connection.Close();

            Console.WriteLine($"Removed {count} entries.");
        }

        public static void UpdateRumorDescriptions()
        {
            //NOT COMPLETE - Must find a way to differentiate rumors from other items.
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = "SELECT `object_Id` FROM `weenie_properties_d_i_d` WHERE `type` = 8 AND `value` = 0x60030BA"; // Light Blue
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();
            List<int> rumorsLvl01_10 = new List<int>();
            while (reader.Read())
            {
                rumorsLvl01_10.Add(reader.GetInt32(0));
            }
            reader.Close();

            sql = "SELECT `object_Id` FROM `weenie_properties_d_i_d` WHERE `type` = 8 AND `value` = 0x60030A4"; // Blue
            command = new MySqlCommand(sql, connection);
            reader = command.ExecuteReader();
            List<int> rumorsLvl10_20 = new List<int>();
            while (reader.Read())
            {
                rumorsLvl10_20.Add(reader.GetInt32(0));
            }
            reader.Close();

            sql = "SELECT `object_Id` FROM `weenie_properties_d_i_d` WHERE `type` = 8 AND `value` = 0x60030A3"; // Green
            command = new MySqlCommand(sql, connection);
            reader = command.ExecuteReader();
            List<int> rumorsLvl20_40 = new List<int>();
            while (reader.Read())
            {
                rumorsLvl20_40.Add(reader.GetInt32(0));
            }
            reader.Close();

            sql = "SELECT `object_Id` FROM `weenie_properties_d_i_d` WHERE `type` = 8 AND `value` = 0x60030A7"; // Orange
            command = new MySqlCommand(sql, connection);
            reader = command.ExecuteReader();
            List<int> rumorsLvl60_80 = new List<int>();
            while (reader.Read())
            {
                rumorsLvl60_80.Add(reader.GetInt32(0));
            }
            reader.Close();

            sql = "SELECT `object_Id` FROM `weenie_properties_d_i_d` WHERE `type` = 8 AND `value` = 0x60030A2"; // Red
            command = new MySqlCommand(sql, connection);
            reader = command.ExecuteReader();
            List<int> rumorsLvl80_126 = new List<int>();
            while (reader.Read())
            {
                rumorsLvl80_126.Add(reader.GetInt32(0));
            }
            reader.Close();

            int count = 0;
            foreach (var entry in rumorsLvl01_10)
            {
                sql = $"SELECT `value` FROM `weenie_properties_string` WHERE `type` = 16 AND `object_Id` = {entry}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string currentText = reader.GetString(0);
                    if (currentText == "This is a good adventure for someone who is newly arrived in Dereth.")
                        currentText = "This is a good adventure for levels 1-10.";
                    else
                        currentText = currentText.Replace("This is a good adventure for someone who is newly arrived in Dereth.", "\n\nThis is a good adventure for levels 1-10.");

                    //sql = $"UPDATE `weenie_properties_string` SET `value` = {currentText} WHERE `type` = 16 AND `object_id` = {entry}";
                    //command = new MySqlCommand(sql, connection);
                    //count += command.ExecuteNonQuery();
                }
                else
                {
                    //sql = $"INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`) VALUES ({entry},   16, 'This is a good adventure for levels 1-10.')";
                    //command = new MySqlCommand(sql, connection);
                    //count += command.ExecuteNonQuery();
                }

                reader.Close();
            }

            foreach (var entry in rumorsLvl10_20)
            {
                sql = $"SELECT `value` FROM `weenie_properties_string` WHERE `type` = 16 AND `object_Id` = {entry}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string currentText = reader.GetString(0);
                    if (currentText == "This is a good adventure for someone who is newly arrived in Dereth.")
                        currentText = "This is a good adventure for levels 10-20.";
                    else
                        currentText = currentText.Replace("This is a good adventure for someone who is newly arrived in Dereth.", "\n\nThis is a good adventure for levels 10-20.");

                    //sql = $"UPDATE `weenie_properties_string` SET `value` = {currentText} WHERE `type` = 16 AND `object_id` = {entry}";
                    //command = new MySqlCommand(sql, connection);
                    //count += command.ExecuteNonQuery();
                }
                else
                {
                    //sql = $"INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`) VALUES ({entry},   16, 'This is a good adventure for levels 10-20.')";
                    //command = new MySqlCommand(sql, connection);
                    //count += command.ExecuteNonQuery();
                }

                reader.Close();
            }

            connection.Close();

            Console.WriteLine($"Updated {count} entries.");
        }

        public static void ChangeSpellScrollPrices()
        {
            StreamReader inputFile = new StreamReader(new FileStream("./input/ListOfSpellScrollIds.txt", FileMode.Open, FileAccess.Read));

            Dictionary<int, string> scrolls = new Dictionary<int, string>();

            while (!inputFile.EndOfStream)
            {
                string line = inputFile.ReadLine();
                string[] splitLine = line.Split('\t');

                int newItem;
                if (int.TryParse(splitLine[0], out newItem))
                    scrolls.Add(newItem, splitLine[1]);
            }
            inputFile.Close();

            List<int> scrolls1 = new List<int>();
            List<int> scrolls2 = new List<int>();
            List<int> scrolls3 = new List<int>();
            List<int> scrolls4 = new List<int>();
            List<int> scrolls5 = new List<int>();
            List<int> scrolls6 = new List<int>();
            List<int> scrolls7 = new List<int>();

            foreach(var entry in scrolls)
            {
                if (entry.Value.Contains("7"))
                    scrolls7.Add(entry.Key);
                else if (entry.Value.Contains("6"))
                    scrolls6.Add(entry.Key);
                else if (entry.Value.Contains("5"))
                    scrolls5.Add(entry.Key);
                else if (entry.Value.Contains("4"))
                    scrolls4.Add(entry.Key);
                else if (entry.Value.Contains("3"))
                    scrolls3.Add(entry.Key);
                else if (entry.Value.Contains("2"))
                    scrolls2.Add(entry.Key);
                else
                    scrolls1.Add(entry.Key);
            }

            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = $"UPDATE `weenie_properties_int` SET `value` = 1000 WHERE `type` = 19 AND `object_Id` IN ({string.Join(",", scrolls1)})";
            MySqlCommand command = new MySqlCommand(sql, connection);
            int count = command.ExecuteNonQuery();

            sql = $"UPDATE `weenie_properties_int` SET `value` = 1500 WHERE `type` = 19 AND `object_Id` IN ({string.Join(",", scrolls2)})";
            command = new MySqlCommand(sql, connection);
            count += command.ExecuteNonQuery();

            sql = $"UPDATE `weenie_properties_int` SET `value` = 2000 WHERE `type` = 19 AND `object_Id` IN ({string.Join(",", scrolls3)})";
            command = new MySqlCommand(sql, connection);
            count += command.ExecuteNonQuery();

            sql = $"UPDATE `weenie_properties_int` SET `value` = 2500 WHERE `type` = 19 AND `object_Id` IN ({string.Join(",", scrolls4)})";
            command = new MySqlCommand(sql, connection);
            count += command.ExecuteNonQuery();

            sql = $"UPDATE `weenie_properties_int` SET `value` = 5000 WHERE `type` = 19 AND `object_Id` IN ({string.Join(",", scrolls5)})";
            command = new MySqlCommand(sql, connection);
            count += command.ExecuteNonQuery();

            sql = $"UPDATE `weenie_properties_int` SET `value` = 10000 WHERE `type` = 19 AND `object_Id` IN ({string.Join(",", scrolls6)})";
            command = new MySqlCommand(sql, connection);
            count += command.ExecuteNonQuery();

            sql = $"UPDATE `weenie_properties_int` SET `value` = 30000 WHERE `type` = 19 AND `object_Id` IN ({string.Join(",", scrolls7)})";
            command = new MySqlCommand(sql, connection);
            count += command.ExecuteNonQuery();

            sql = $"DELETE from `weenie_properties_bool` WHERE `type` = 23 AND `object_Id` IN ({string.Join(",", scrolls1)})";
            command = new MySqlCommand(sql, connection);
            count = command.ExecuteNonQuery();

            sql = $"DELETE from `weenie_properties_bool` WHERE `type` = 23 AND `object_Id` IN ({string.Join(",", scrolls2)})";
            command = new MySqlCommand(sql, connection);
            count = command.ExecuteNonQuery();

            sql = $"DELETE from `weenie_properties_bool` WHERE `type` = 23 AND `object_Id` IN ({string.Join(",", scrolls3)})";
            command = new MySqlCommand(sql, connection);
            count = command.ExecuteNonQuery();

            sql = $"DELETE from `weenie_properties_bool` WHERE `type` = 23 AND `object_Id` IN ({string.Join(",", scrolls4)})";
            command = new MySqlCommand(sql, connection);
            count = command.ExecuteNonQuery();

            sql = $"DELETE from `weenie_properties_bool` WHERE `type` = 23 AND `object_Id` IN ({string.Join(",", scrolls5)})";
            command = new MySqlCommand(sql, connection);
            count = command.ExecuteNonQuery();

            sql = $"DELETE from `weenie_properties_bool` WHERE `type` = 23 AND `object_Id` IN ({string.Join(",", scrolls6)})";
            command = new MySqlCommand(sql, connection);
            count = command.ExecuteNonQuery();

            sql = $"DELETE from `weenie_properties_bool` WHERE `type` = 23 AND `object_Id` IN ({string.Join(",", scrolls7)})";
            command = new MySqlCommand(sql, connection);
            count = command.ExecuteNonQuery();

            connection.Close();

            Console.WriteLine($"Updated {count} entries.");
        }

        public static void RemoveCowlsFromShopkeepers()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = "SELECT object_Id FROM weenie_properties_string WHERE `type` = 5 AND `value` IN (\"Shopkeeper\")";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<int> npcs = new List<int>();
            List<int> vendors = new List<int>();

            while (reader.Read())
            {
                npcs.Add(reader.GetInt32(0));
            }
            reader.Close();

            foreach (var npc in npcs)
            {
                sql = $"SELECT value FROM weenie_properties_int WHERE `type` = 76 and object_id = {npc}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int value = reader.GetInt32(0);
                    if (value != 0)
                        vendors.Add(npc);
                }
                reader.Close();
            }

            int count = 0;
            foreach (var vendor in vendors)
            {
                sql = $"DELETE FROM weenie_properties_create_list WHERE object_Id = {vendor} AND destination_Type = 4 AND weenie_Class_Id IN (119)";
                command = new MySqlCommand(sql, connection);
                count += command.ExecuteNonQuery();
            }

            connection.Close();

            Console.WriteLine($"Removed {count} entries.");
        }

        public static void RemoveNonMutatedItemsFromVendors()
        {
            StreamReader inputFile = new StreamReader(new FileStream("./input/ListOfItemsToRemoveFromVendors.txt", FileMode.Open, FileAccess.Read));

            List <int> itemsToRemove = new List<int>();

            while (!inputFile.EndOfStream)
            {
                string line = inputFile.ReadLine();
                string[] splitLine = line.Split('\t');

                int newItem;
                if (int.TryParse(splitLine[0], out newItem))
                    itemsToRemove.Add(newItem);
            }
            inputFile.Close();

            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = $"DELETE FROM `weenie_properties_create_list` WHERE `destination_Type`= 4 and `weenie_Class_Id` in ({string.Join(",", itemsToRemove)})";
            MySqlCommand command = new MySqlCommand(sql, connection);
            int count = command.ExecuteNonQuery();

            connection.Close();

            Console.WriteLine($"Removed {count} entries.");
        }

        private class MerchandiseItemTypeData
        {
            public int MerchandiseItemTypes;
            public string Template;

            public MerchandiseItemTypeData(int merchandiseItemTypes, string template)
            {
                MerchandiseItemTypes = merchandiseItemTypes;
                Template = template;
            }
        }

        public static void RedistributeVendorMerchandiseTypes()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = "SELECT object_Id, value FROM weenie_properties_string WHERE `type` = 5 AND `value` IN (\"Blacksmith\",\"Bowyer\",\"Weaponsmith\",\"Armorer\",\"Peddler\",\"Shopkeeper\",\"Barkeeper\")";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            Dictionary<int, MerchandiseItemTypeData> vendors = new Dictionary<int, MerchandiseItemTypeData>();

            while (reader.Read())
            {
                vendors.Add(reader.GetInt32(0), new MerchandiseItemTypeData(0, reader.GetString(1)));
            }
            reader.Close();

            sql = $"SELECT `object_Id`,`value` FROM `weenie_properties_int` WHERE `type` = 74 and object_Id IN({string.Join(",", vendors.Keys)})";
            command = new MySqlCommand(sql, connection);
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                if (vendors.TryGetValue(id, out _))
                {
                    vendors[id].MerchandiseItemTypes = reader.GetInt32(1);
                }
            }
            reader.Close();

            int count = 0;
            foreach (var entry in vendors)
            {
                if (entry.Value.MerchandiseItemTypes == 0)
                    continue; // We're not a vendor.

                int newValue = 0;
                switch (entry.Value.Template)
                {
                    case "Armorer":
                        newValue = entry.Value.MerchandiseItemTypes & ~(int)(eItemType.Type_Clothing | eItemType.Type_Weapon);
                        break;
                    case "Blacksmith":
                        newValue = entry.Value.MerchandiseItemTypes & ~(int)(eItemType.Type_Clothing | eItemType.Type_Missile_Weapon);
                        break;
                    case "Bowyer":
                        newValue = entry.Value.MerchandiseItemTypes & ~(int)eItemType.Type_Melee_Weapon;
                        break;
                    case "Peddler":
                        newValue = entry.Value.MerchandiseItemTypes | (int)eItemType.Type_Vestements | (int)eItemType.Type_Missile_Weapon;
                        break;
                    case "Weaponsmith":
                        newValue = entry.Value.MerchandiseItemTypes & ~(int)(eItemType.Type_Vestements | eItemType.Type_Missile_Weapon);
                        break;
                    case "Barkeeper":
                        newValue = entry.Value.MerchandiseItemTypes & ~(int)(eItemType.Type_Weapon | eItemType.Type_Vestements | eItemType.Type_Jewelry | eItemType.Type_Gem | eItemType.Type_Caster);
                        break;
                    case "Shopkeeper":
                        switch (entry.Key)
                        {
                            case 4693:
                                newValue = entry.Value.MerchandiseItemTypes & ~(int)(eItemType.Type_Weapon | eItemType.Type_Vestements | eItemType.Type_Caster);
                                break;
                            case 2534:
                            case 24219:
                                newValue = entry.Value.MerchandiseItemTypes & ~(int)eItemType.Type_Missile_Weapon;
                                break;
                            case 12718:
                            case 2317:
                            case 1045:
                            case 993:
                            case 1822:
                            case 4685:
                            case 1057:
                            case 982:
                            case 4702:
                            case 737:
                            case 1833:
                            case 718:
                            case 820:
                            case 676:
                            case 4444:
                            case 702:
                            case 2300:
                            case 2259:
                            case 656:
                            case 842:
                            case 2232:
                            case 4555:
                                newValue = entry.Value.MerchandiseItemTypes & ~(int)(eItemType.Type_Weapon | eItemType.Type_Vestements | eItemType.Type_Jewelry | eItemType.Type_Gem | eItemType.Type_Caster);
                                break;
                        }
                        newValue = newValue | (int)eItemType.Type_Craft_Alchemy_Base | (int)eItemType.Type_Craft_Alchemy_Intermediate | (int)eItemType.Type_Craft_Fletching_Base | (int)eItemType.Type_Craft_Fletching_Intermediate | (int)eItemType.Type_Craft_Cooking_Base;
                        break;
                    default:
                        newValue = -1;
                        break;
                }

                if (newValue != -1 && entry.Value.MerchandiseItemTypes != newValue)
                {
                    sql = $"UPDATE `weenie_properties_int` SET `value` = {newValue} WHERE `type` = 74 AND `object_Id` = {entry.Key}; ";
                    command = new MySqlCommand(sql, connection);
                    count += command.ExecuteNonQuery();
                }
            }
            connection.Close();

            Console.WriteLine($"Updated {count} entries.");
        }

        public static void RemoveSalvageMerchandiseTypeFromVendors()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = $"SELECT `object_Id`,`value` FROM `weenie_properties_int` WHERE `type` = 74";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            Dictionary<int, int> MerchandiseItemTypesMap = new Dictionary<int, int>();

            while (reader.Read())
            {
                MerchandiseItemTypesMap.Add(reader.GetInt32(0), reader.GetInt32(1));
            }
            reader.Close();

            int count = 0;
            foreach (var entry in MerchandiseItemTypesMap)
            {
                int oldValue = entry.Value;
                int newValue = entry.Value & ~0x40000000;
                if (oldValue != newValue)
                {
                    sql = $"UPDATE `weenie_properties_int` SET `value` = {newValue} WHERE `type` = 74 AND `object_Id` = {entry.Key}; ";
                    command = new MySqlCommand(sql, connection);
                    count += command.ExecuteNonQuery();
                }
            }
            connection.Close();

            Console.WriteLine($"Updated {count} entries.");
        }

        public static void RedistributeTradeNotesToVendors()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = "SELECT object_Id FROM weenie_properties_create_list WHERE weenie_Class_Id IN(2621,2622,2623,2624,2625,2626,2627,20628,20629,20630) AND destination_Type = 4";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<int> npcs = new List<int>();
            List<int> vendors = new List<int>();

            while (reader.Read())
            {
                npcs.Add(reader.GetInt32(0));
            }
            reader.Close();

            foreach (var npc in npcs)
            {
                sql = $"SELECT value FROM weenie_properties_int WHERE `type` = 76 and object_id = {npc}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int value = reader.GetInt32(0);
                    if (value != 0 && !vendors.Contains(npc))
                        vendors.Add(npc);
                }
                reader.Close();
            }

            int count = 0;
            foreach (var vendor in vendors)
            {
                // Remove all existing ones so we can readd them in the correct order.
                sql = $"DELETE FROM weenie_properties_create_list WHERE object_Id = {vendor} AND destination_Type = 4 AND weenie_Class_Id IN (2621,2622,2623,2624,2625,2626,2627,20628,20629,20630)";
                command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                sql = $"INSERT INTO weenie_properties_create_list (object_Id, destination_Type, weenie_Class_Id, stack_Size, palette, shade, try_To_Bond)" +
                      $"VALUES ({vendor}, 4,  2621, -1, 0, 0, 0)" + // I
                            $",({vendor}, 4,  2622, -1, 0, 0, 0)" + // V
                            $",({vendor}, 4,  2623, -1, 0, 0, 0)" + // X
                            $",({vendor}, 4,  2624, -1, 0, 0, 0)" + // L
                            $",({vendor}, 4,  2625, -1, 0, 0, 0)" + // C
                            $",({vendor}, 4,  2626, -1, 0, 0, 0)" + // D
                            $",({vendor}, 4,  2627, -1, 0, 0, 0)" + // M
                            $",({vendor}, 4, 20628, -1, 0, 0, 0)" + // MD
                            $",({vendor}, 4, 20629, -1, 0, 0, 0)" + // MM
                            $",({vendor}, 4, 20630, -1, 0, 0, 0)";  // MMD
                command = new MySqlCommand(sql, connection);
                count += command.ExecuteNonQuery();
            }
            connection.Close();

            Console.WriteLine($"Added {count} entries.");
        }

        public static void AddRumorColorCodesToVendors()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = "SELECT object_Id FROM weenie_properties_string WHERE `type` = 5 AND `value` IN (\"Barkeeper\")";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<int> npcs = new List<int>();
            List<int> vendors = new List<int>();

            while (reader.Read())
            {
                npcs.Add(reader.GetInt32(0));
            }
            reader.Close();

            foreach (var npc in npcs)
            {
                sql = $"SELECT value FROM weenie_properties_int WHERE `type` = 76 and object_id = {npc}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int value = reader.GetInt32(0);
                    if (value != 0)
                        vendors.Add(npc);
                }
                reader.Close();
            }

            int count = 0;
            foreach (var vendor in vendors)
            {
                // Remove all existing ones so we can readd them in the correct order.
                sql = $"DELETE FROM weenie_properties_create_list WHERE object_Id = {vendor} AND destination_Type = 4 AND weenie_Class_Id IN (50054)";
                command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                sql = $"INSERT INTO weenie_properties_create_list (object_Id, destination_Type, weenie_Class_Id, stack_Size, palette, shade, try_To_Bond)" +
                      $"VALUES ({vendor}, 4, 50054, -1, 0, 0.0, 0)";
                command = new MySqlCommand(sql, connection);
                count += command.ExecuteNonQuery();
            }
            connection.Close();

            Console.WriteLine($"Added {count} entries.");
        }

        public static void AddLeyLineAmuletsToVendors()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = "SELECT object_Id FROM weenie_properties_string WHERE `type` = 5 AND `value` IN (\"Archmage\",\"Master Archmage\",\"Wandering Archmage\")";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<int> npcs = new List<int>();
            List<int> vendors = new List<int>();

            while (reader.Read())
            {
                npcs.Add(reader.GetInt32(0));
            }
            reader.Close();

            foreach (var npc in npcs)
            {
                sql = $"SELECT value FROM weenie_properties_int WHERE `type` = 76 and object_id = {npc}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int value = reader.GetInt32(0);
                    if (value != 0)
                        vendors.Add(npc);
                }
                reader.Close();
            }

            int count = 0;
            foreach (var vendor in vendors)
            {
                // Remove all existing ones so we can readd them in the correct order.
                sql = $"DELETE FROM weenie_properties_create_list WHERE object_Id = {vendor} AND destination_Type = 4 AND weenie_Class_Id IN (50056,50057)";
                command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                sql = $"INSERT INTO weenie_properties_create_list (object_Id, destination_Type, weenie_Class_Id, stack_Size, palette, shade, try_To_Bond)" +
                      $"VALUES ({vendor}, 4, 50056, -1, 0, 0.0, 0)" +
                      $"     , ({vendor}, 4, 50057, -1, 0, 0.0, 0)";
                command = new MySqlCommand(sql, connection);
                count += command.ExecuteNonQuery();
            }
            connection.Close();

            Console.WriteLine($"Added {count} entries.");
        }

        public static void AddCombatManualToVendors()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = "SELECT object_Id FROM weenie_properties_string WHERE `type` = 5 AND `value` IN (\"Scribe\")";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<int> npcs = new List<int>();
            List<int> vendors = new List<int>();

            while (reader.Read())
            {
                npcs.Add(reader.GetInt32(0));
            }
            reader.Close();

            foreach (var npc in npcs)
            {
                sql = $"SELECT value FROM weenie_properties_int WHERE `type` = 76 and object_id = {npc}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int value = reader.GetInt32(0);
                    if (value != 0)
                        vendors.Add(npc);
                }
                reader.Close();
            }

            int count = 0;
            foreach (var vendor in vendors)
            {
                // Remove all existing ones so we can readd them in the correct order.
                sql = $"DELETE FROM weenie_properties_create_list WHERE object_Id = {vendor} AND destination_Type = 4 AND weenie_Class_Id IN (50045)";
                command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                sql = $"INSERT INTO weenie_properties_create_list (object_Id, destination_Type, weenie_Class_Id, stack_Size, palette, shade, try_To_Bond)" +
                      $"VALUES ({vendor}, 4, 50045, -1, 0, 0.0, 0)";
                command = new MySqlCommand(sql, connection);
                count += command.ExecuteNonQuery();
            }
            connection.Close();

            Console.WriteLine($"Added {count} entries.");
        }

        public static void AddMagnifyingGlassToVendors()
        {
            //Add magnifying tools to everyone that sells usts.
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = "SELECT object_Id FROM weenie_properties_create_list WHERE weenie_Class_Id = 20646 AND destination_Type = 4"; // Pack
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<int> vendors = new List<int>();

            while (reader.Read())
            {
                vendors.Add(reader.GetInt32(0));
            }
            reader.Close();

            int count = 0;
            foreach (var vendor in vendors)
            {
                sql = $"INSERT INTO weenie_properties_create_list (object_Id, destination_Type, weenie_Class_Id, stack_Size, palette, shade, try_To_Bond)" +
                      $"VALUES ({vendor}, 4, 50077, -1, 0, 0.0, 0)";
                command = new MySqlCommand(sql, connection);
                count += command.ExecuteNonQuery();
            }
            connection.Close();

            Console.WriteLine($"Added {count} entries.");
        }

        public static void AddCombatTacticsAndTechniquesToVendors()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = "SELECT object_Id FROM weenie_properties_string WHERE `type` = 5 AND `value` IN (\"Blacksmith\",\"Weaponsmith\",\"Armorer\")";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<int> npcs = new List<int>();
            List<int> vendors = new List<int>();

            while (reader.Read())
            {
                npcs.Add(reader.GetInt32(0));
            }
            reader.Close();

            foreach (var npc in npcs)
            {
                sql = $"SELECT value FROM weenie_properties_int WHERE `type` = 76 and object_id = {npc}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int value = reader.GetInt32(0);
                    if (value != 0)
                        vendors.Add(npc);
                }
                reader.Close();
            }

            int count = 0;
            foreach (var vendor in vendors)
            {
                // Remove all existing ones so we can readd them in the correct order.
                sql = $"DELETE FROM weenie_properties_create_list WHERE object_Id = {vendor} AND destination_Type = 4 AND weenie_Class_Id IN (50045,50078,50046,50047,50048,50049,50050)";
                command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                sql = $"INSERT INTO weenie_properties_create_list (object_Id, destination_Type, weenie_Class_Id, stack_Size, palette, shade, try_To_Bond)" +
                      $"VALUES ({vendor}, 4, 50046, -1, 0, 0.0, 0)" +
                      $"     , ({vendor}, 4, 50078, -1, 0, 0.0, 0)" +
                      $"     , ({vendor}, 4, 50047, -1, 0, 0.0, 0)" +
                      $"     , ({vendor}, 4, 50048, -1, 0, 0.0, 0)" +
                      $"     , ({vendor}, 4, 50049, -1, 0, 0.0, 0)" +
                      $"     , ({vendor}, 4, 50050, -1, 0, 0.0, 0)";
                command = new MySqlCommand(sql, connection);
                count += command.ExecuteNonQuery();
            }
            connection.Close();

            Console.WriteLine($"Added {count} entries.");
        }

        public static void AddTethersToVendors()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = "SELECT object_Id FROM weenie_properties_string WHERE `type` = 5 AND `value` IN (\"Blacksmith\",\"Weaponsmith\",\"Armor Smith\",\"Armorsmith\")";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<int> npcs = new List<int>();
            List<int> vendors = new List<int>();

            while (reader.Read())
            {
                npcs.Add(reader.GetInt32(0));
            }
            reader.Close();

            foreach (var npc in npcs)
            {
                sql = $"SELECT value FROM weenie_properties_int WHERE `type` = 76 and object_id = {npc}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if(reader.Read())
                {
                    int value = reader.GetInt32(0);
                    if (value != 0)
                        vendors.Add(npc);
                }
                reader.Close();
            }

            int count = 0;
            foreach (var vendor in vendors)
            {
                // Remove all existing ones so we can readd them in the correct order.
                sql = $"DELETE FROM weenie_properties_create_list WHERE object_Id = {vendor} AND destination_Type = 4 AND weenie_Class_Id IN (45683, 45684)";
                command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                sql = $"INSERT INTO weenie_properties_create_list (object_Id, destination_Type, weenie_Class_Id, stack_Size, palette, shade, try_To_Bond)" +
                      $"VALUES ({vendor}, 4, 45683, -1, 0, 0.0, 0)" +
                      $"     , ({vendor}, 4, 45684, -1, 0, 0.0, 0)";
                command = new MySqlCommand(sql, connection);
                count += command.ExecuteNonQuery();
            }
            connection.Close();

            Console.WriteLine($"Added {count} entries.");
        }

        public static void AddSkillReqToAtlanWeapons()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=root;password=;DefaultCommandTimeout=120;database=ace_world_test");
            connection.Open();

            List<int> Axe = new List<int>()
            {
                6162,6164,6166,6168,6170,6163,6165,6167,6169,6153,6155,6157,6159,6161,6154,6156,6158,6160,6144,6146,6148,6150,6152,6145,6147,6149,6151
            };

            List<int> AxeBlackfire = new List<int>()
            {
                7450,7449,7448
            };

            List<int> Dagger = new List<int>()
            {
                6217,6219,6221,6223,6225,6218,6220,6222,6224,6208,6210,6212,6214,6216,6209,6211,6213,6215,6199,6201,6203,6205,6207,6200,6202,6204,6206
            };

            List<int> DaggerBlackfire = new List<int>()
            {
                7456,7455,7454
            };

            List<int> Mace = new List<int>()
            {
                6244,6246,6248,6250,6252,6245,6247,6249,6251,6235,6237,6239,6241,6243,6236,6238,6240,6242,6226,6228,6230,6232,6234,6227,6229,6231,6233
            };

            List<int> MaceBlackfire = new List<int>()
            {
                7459,7458,7457
            };

            List<int> Spear = new List<int>()
            {
                6271,6273,6275,6277,6279,6272,6274,6276,6278,6262,6264,6266,6268,6270,6263,6265,6267,6269,6253,6255,6257,6259,6261,6254,6256,6258,6260
            };

            List<int> SpearBlackfire = new List<int>()
            {
                7462,7461,7460
            };

            List<int> Staff = new List<int>()
            {
                6142,6198,6288,6289,6290,6138,6139,6140,6141,6137,6284,6285,6286,6287,6133,6134,6135,6136,6132,6280,6281,6282,6283,6128,6129,6130,6131
            };

            List<int> StaffBlackfire = new List<int>()
            {
                7465,7464,7463
            };

            List<int> Sword = new List<int>()
            {
                6309,6311,6313,6315,6317,6310,6312,6314,6316,6300,6302,6304,6306,6308,6301,6303,6305,6307,6291,6293,6295,6297,6299,6292,6294,6296,6298
            };

            List<int> SwordBlackfire = new List<int>()
            {
                7468,7467,7466
            };

            List<int> Unarmed = new List<int>()
            {
                6189,6191,6193,6195,6197,6190,6192,6194,6196,6180,6182,6184,6186,6188,6181,6183,6185,6187,6171,6173,6175,6177,6179,6172,6174,6176,6178
            };

            List<int> UnarmedBlackfire = new List<int>()
            {
                7453,7452,7451
            };

            string prefix = "";
            string sql = "INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)\nVALUES";
            foreach (var entry in Axe)
            {
                sql += $"{prefix} ({entry}, 158, 2)\n";
                prefix = ",";
                sql += $"{prefix} ({entry}, 159, 1)\n";
                sql += $"{prefix} ({entry}, 160, 250)\n";
            }
            foreach (var entry in AxeBlackfire)
            {
                sql += $"{prefix} ({entry}, 158, 2)\n";
                prefix = ",";
                sql += $"{prefix} ({entry}, 159, 1)\n";
                sql += $"{prefix} ({entry}, 160, 400)\n";
            }
            foreach (var entry in Dagger)
            {
                sql += $"{prefix} ({entry}, 158, 2)\n";
                prefix = ",";
                sql += $"{prefix} ({entry}, 159, 4)\n";
                sql += $"{prefix} ({entry}, 160, 250)\n";
            }
            foreach (var entry in DaggerBlackfire)
            {
                sql += $"{prefix} ({entry}, 158, 2)\n";
                prefix = ",";
                sql += $"{prefix} ({entry}, 159, 4)\n";
                sql += $"{prefix} ({entry}, 160, 400)\n";
            }
            foreach (var entry in Mace)
            {
                sql += $"{prefix} ({entry}, 158, 2)\n";
                prefix = ",";
                sql += $"{prefix} ({entry}, 159, 5)\n";
                sql += $"{prefix} ({entry}, 160, 250)\n";
            }
            foreach (var entry in MaceBlackfire)
            {
                sql += $"{prefix} ({entry}, 158, 2)\n";
                prefix = ",";
                sql += $"{prefix} ({entry}, 159, 5)\n";
                sql += $"{prefix} ({entry}, 160, 400)\n";
            }
            foreach (var entry in Spear)
            {
                sql += $"{prefix} ({entry}, 158, 2)\n";
                prefix = ",";
                sql += $"{prefix} ({entry}, 159, 9)\n";
                sql += $"{prefix} ({entry}, 160, 250)\n";
            }
            foreach (var entry in SpearBlackfire)
            {
                sql += $"{prefix} ({entry}, 158, 2)\n";
                prefix = ",";
                sql += $"{prefix} ({entry}, 159, 9)\n";
                sql += $"{prefix} ({entry}, 160, 400)\n";
            }
            foreach (var entry in Staff)
            {
                sql += $"{prefix} ({entry}, 158, 2)\n";
                prefix = ",";
                sql += $"{prefix} ({entry}, 159, 10)\n";
                sql += $"{prefix} ({entry}, 160, 250)\n";
            }
            foreach (var entry in StaffBlackfire)
            {
                sql += $"{prefix} ({entry}, 158, 2)\n";
                prefix = ",";
                sql += $"{prefix} ({entry}, 159, 10)\n";
                sql += $"{prefix} ({entry}, 160, 400)\n";
            }
            foreach (var entry in Sword)
            {
                sql += $"{prefix} ({entry}, 158, 2)\n";
                prefix = ",";
                sql += $"{prefix} ({entry}, 159, 11)\n";
                sql += $"{prefix} ({entry}, 160, 250)\n";
            }
            foreach (var entry in SwordBlackfire)
            {
                sql += $"{prefix} ({entry}, 158, 2)\n";
                prefix = ",";
                sql += $"{prefix} ({entry}, 159, 11)\n";
                sql += $"{prefix} ({entry}, 160, 400)\n";
            }
            foreach (var entry in Unarmed)
            {
                sql += $"{prefix} ({entry}, 158, 2)\n";
                prefix = ",";
                sql += $"{prefix} ({entry}, 159, 13)\n";
                sql += $"{prefix} ({entry}, 160, 250)\n";
            }
            foreach (var entry in UnarmedBlackfire)
            {
                sql += $"{prefix} ({entry}, 158, 2)\n";
                prefix = ",";
                sql += $"{prefix} ({entry}, 159, 13)\n";
                sql += $"{prefix} ({entry}, 160, 400)\n";
            }

            MySqlCommand command = new MySqlCommand(sql, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }


        public static void AddLevelReqToShadowArmor()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=root;password=;DefaultCommandTimeout=120;database=ace_world_test");
            connection.Open();

            int lesserLevel = 25;
            List<int> lesser = new List<int>()
            {
                7663,7664,7665,7666,7667,7694,7695,7696,7697,7698,
                7648,7649,7650,7651,7652,7725,7726,7727,7728,7729,7755,7756,7757,7758,7759,
                7633,7634,7635,7636,7637,7679,7680,7681,7682,7683,7710,7711,7712,7713,7714,7740,7741,7742,7743,7744
            };

            int regularLevel = 45;
            List<int> regular = new List<int>()
            {
                7668,7669,7670,7671,7672,7699,7700,7701,7702,7703,
                7653,7654,7655,7656,7657,7730,7731,7732,7733,7734,7760,7761,7762,7763,7764,
                7638,7639,7640,7641,7642,7684,7685,7686,7687,7688,7715,7716,7717,7718,7719,7745,7746,7747,7748,7749
            };

            int greaterLevel = 65;
            List<int> greater = new List<int>()
            {
                7658,7659,7660,7661,7662,7689,7690,7691,7692,7693,
                7643,7644,7645,7646,7647,7720,7721,7722,7723,7724,7750,7751,7752,7753,7754,
                7628,7629,7630,7631,7632,7674,7675,7676,7677,7678,7705,7706,7707,7708,7709,7735,7736,7737,7738,7739
            };

            string prefix = "";
            string sql = "INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)\nVALUES";
            foreach (var entry in lesser)
            {
                sql += $"{prefix} ({entry}, 158, 7)\n";
                prefix = ",";
                sql += $"{prefix} ({entry}, 159, 1)\n";
                sql += $"{prefix} ({entry}, 160, {lesserLevel})\n";
            }
            foreach (var entry in regular)
            {
                sql += $"{prefix} ({entry}, 158, 7)\n";
                prefix = ",";
                sql += $"{prefix} ({entry}, 159, 1)\n";
                sql += $"{prefix} ({entry}, 160, {regularLevel})\n";
            }
            foreach (var entry in greater)
            {
                sql += $"{prefix} ({entry}, 158, 7)\n";
                prefix = ",";
                sql += $"{prefix} ({entry}, 159, 1)\n";
                sql += $"{prefix} ({entry}, 160, {greaterLevel})\n";
            }

            MySqlCommand command = new MySqlCommand(sql, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }

        public class FoodEntry
        {
            public int WeenieId;
            public string WeenieClassName;
            public string Name;
            public int Type;
            public int BoosterEnum;
            public int BoostValue;
            public int NewBoostValue;

            public FoodEntry(int weenieId, string weenieClassName)
            {
                WeenieId = weenieId;
                WeenieClassName = weenieClassName;
            }
        }

        public static void CreateFoodList()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=root;password=;DefaultCommandTimeout=120;database=ace_world_test");
            connection.Open();

            string sql = "SELECT `class_Id`,`class_Name` FROM `weenie` WHERE `type` = 18";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<FoodEntry> foodEntries = new List<FoodEntry>();

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                if (id != 3722 && (id < 29104 || id > 29216))
                    foodEntries.Add(new FoodEntry(id, reader.GetString(1)));
            }
            reader.Close();

            foreach (var entry in foodEntries)
            {
                sql = $"SELECT `value` FROM `weenie_properties_string` WHERE `type` = 1 AND `object_Id` = {entry.WeenieId}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                    entry.Name = reader.GetString(0);
                reader.Close();

                sql = $"SELECT `value` FROM `weenie_properties_int` WHERE `type` = 1 AND `object_Id` = {entry.WeenieId}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                    entry.Type = reader.GetInt32(0);
                else
                    entry.Type = 0;
                reader.Close();

                sql = $"SELECT `value` FROM `weenie_properties_int` WHERE `type` = 89 AND `object_Id` = {entry.WeenieId}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                    entry.BoosterEnum = reader.GetInt32(0);
                reader.Close();

                sql = $"SELECT `value` FROM `weenie_properties_int` WHERE `type` = 90 AND `object_Id` = {entry.WeenieId}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                    entry.BoostValue = reader.GetInt32(0);
                reader.Close();
            }

            StreamWriter outputFile = new StreamWriter(new FileStream("./foodList.txt", FileMode.Create, FileAccess.Write));
            outputFile.WriteLine($"WeenieId\tName(WeenieClassName)\tBoosterEnum\tBoostValue\tNewBoostValue");
            foreach (var entry in foodEntries)
            {
                if (entry.Type != 128)
                {
                    if (entry.BoosterEnum != 4) // Mana and HP
                    {
                        if (entry.WeenieId == 14864) // Peppermint Chocolate Cookie(cookiechocolatepeppermint)
                            entry.NewBoostValue = 40;
                        else if (entry.BoostValue < 0)
                            entry.NewBoostValue = entry.BoostValue;
                        else if (entry.BoostValue <= 15)
                            entry.NewBoostValue = 20;
                        else if (entry.BoostValue <= 30)
                            entry.NewBoostValue = 40;
                        else if (entry.BoostValue <= 50)
                            entry.NewBoostValue = 50;
                        else if (entry.BoostValue <= 55)
                            entry.NewBoostValue = 60;
                        else
                            entry.NewBoostValue = entry.BoostValue;
                    }
                    else // Stamina
                    {
                        if (entry.BoostValue < 0)
                            entry.NewBoostValue = entry.BoostValue;
                        else if (entry.BoostValue <= 5)
                            entry.NewBoostValue = 20;
                        else if (entry.BoostValue <= 20)
                            entry.NewBoostValue = 30;
                        else if (entry.BoostValue <= 50)
                            entry.NewBoostValue = 50;
                        else if (entry.BoostValue <= 55)
                            entry.NewBoostValue = 60;
                        else
                            entry.NewBoostValue = entry.BoostValue;
                    }
                    outputFile.WriteLine($"{entry.WeenieId}\t{entry.Name}({entry.WeenieClassName})\t{entry.BoosterEnum}\t{entry.BoostValue}\t{entry.NewBoostValue}");
                }
                outputFile.Flush();
            }
            outputFile.Close();

            outputFile = new StreamWriter(new FileStream("./foodList.sql", FileMode.Create, FileAccess.Write));
            foreach (var entry in foodEntries)
            {
                outputFile.WriteLine($"UPDATE `weenie_properties_int`");
                outputFile.WriteLine($"SET");
                outputFile.WriteLine($"`value` = {entry.NewBoostValue}");
                outputFile.WriteLine($"WHERE `type` = 90 AND `object_Id` = {entry.WeenieId};");
                outputFile.WriteLine($"");
                outputFile.Flush();
            }
            outputFile.Close();
        }

        public class CreatureXpEntry: IComparable<CreatureXpEntry>
        {
            public int WeenieId;
            public string WeenieClassName;
            public string Name;
            public int XpAmount;
            public int Level;
            public int Hitpoints;
            public int NumOfSpells;
            public bool IsAttackable;

            public int NewXpAmount;

            public CreatureXpEntry(int weenieId, int xpAmount)
            {
                WeenieId = weenieId;
                XpAmount = xpAmount;
            }

            public int CompareTo(CreatureXpEntry other)
            {
                if(Level != other.Level)
                    return Level.CompareTo(other.Level);
                return XpAmount.CompareTo(other.XpAmount);
            }
        }

        public class LevelXpEntry
        {
            public int Level;
            public int LowestXp = int.MaxValue;
            public int HighestXp = 0;
            public int LowestHp = int.MaxValue;
            public int HighestHp = 0;

            public int CreatureCount;
            public int TotalXp;
            public int AverageXp;

            public int TotalHp;
            public int AverageHp;

            public Dictionary<int, List<CreatureXpEntry>> XPs = new Dictionary<int, List<CreatureXpEntry>>();
        }

        public static void CreateCreatureXPList()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = "SELECT `object_Id`,`value` FROM`weenie_properties_int` WHERE `type` = 146";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<CreatureXpEntry> creatures = new List<CreatureXpEntry>();

            while (reader.Read())
            {
                creatures.Add(new CreatureXpEntry(reader.GetInt32(0), reader.GetInt32(1)));
            }
            reader.Close();

            foreach (var creature in creatures)
            {
                sql = $"SELECT `value` FROM`weenie_properties_bool` WHERE `type` = 19 AND `object_id` = {creature.WeenieId}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                    creature.IsAttackable = reader.GetBoolean(0);
                else
                    creature.IsAttackable = true;

                reader.Close();
            }

            foreach (var creature in creatures)
            {
                sql = $"SELECT `value` FROM`weenie_properties_string` WHERE `type` = 1 AND `object_id` = {creature.WeenieId}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                    creature.Name = reader.GetString(0);

                reader.Close();
            }

            foreach (var creature in creatures)
            {
                sql = $"SELECT class_Name FROM weenie WHERE class_Id = {creature.WeenieId}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                    creature.WeenieClassName = reader.GetString(0);
                reader.Close();
            }

            foreach (var creature in creatures)
            {
                sql = $"SELECT `value` FROM`weenie_properties_int` WHERE `type` = 25 AND `object_id` = {creature.WeenieId}" ;
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                    creature.Level = reader.GetInt32(0);
                reader.Close();
            }

            foreach (var creature in creatures)
            {
                sql = $"SELECT `current_level` FROM `weenie_properties_attribute_2nd` WHERE `type` = 1 AND `object_Id` = {creature.WeenieId}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                    creature.Hitpoints = reader.GetInt32(0);
                reader.Close();
            }

            foreach (var creature in creatures)
            {
                sql = $"SELECT * FROM `weenie_properties_spell_book` WHERE `object_Id` = {creature.WeenieId}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                while(reader.Read())
                    creature.NumOfSpells++;
                reader.Close();
            }

            creatures.Sort();

            List<string> excludedEntries = new List<string>()
            {
                "knightlord-nofall","knightmage_nofall","fiunmaddenedcannibal","knighthand-nofall","knightmagecounselor_nofall","knighttribune_nofall","knightmagewarwizard_nofall","knightmagewarcaster_nofall","knightmanatarms_nofall","knightmercenary-nofall",
                "knighttorturer-nofall","knightexecutioner-nofall","knightcommander_nofall","knightviamontian_nofall","knightroyalguardmelee","knightmageroyalthaumaturge_nofall","knightroyalguardmagic","knightcastellansilver","ace31040-tursh","eaterswordswallower",
                "knightcastellanplatinum","knightcastellangold","knightcastellancopper","fiunmaddenedatavistic","knightroyalprisonwarden","knightquartermastergold","penguinuberlow","penguinubermid","penguin","penguinarrogant","penguinaugmented","penguinrebellious",
                "ruschkslayer","eaterabhorrent","drudgeprowler","ruschkvile","ruschkwarlord","ruschkbarbaric","ruschkfiend","knighttorturer","knightexecutioner","penguinuberhigh","penguinsycophantic","knightmanatarms","knightviamontian","knightcommander","knightbodyguardgold",
                "fiun","fiunraving","knightquartermasterplatinum","fiunmaddened","knightbodyguardcopper","fiunmaniacal","knightquartermastersilver","knighthand","fiunfrenzied","knightmagewarcaster","drudgescrawled","drudgeprowlerdancer","fiunderanged","eaterengorged","knightlord",
                "fiunhighmage","knighttribune","ruschksadist","ruschklaktar","knightmagecounselor","knightmagewarwizard","penguincavegreat","eaterravenousjawdropper","eaterinsatiablejawdropper","eaterengorgedjawdropper","eatervoraciousjawdropper","eaterabhorrentjawdropper",
                "ruschkfledgemaster","drudgeprowlermosswartexodus","ruschkfledge","thrungusporcini","thrungustruffle","thrungusmorel","eaterravenous","eaterrabid","eaterinsatiable","penguincave","eatermarauder","ruschkkartak","thrungusthievingnewbieacademy","eaterlola","knightmage",
                "knightmageroyalthaumaturge","ruschkshatterer","thrungusbutton","thrungusdeathcap","ruschkdraktehn","thrungusenoki","thrungusshiitake","thrungus","thrungusbeefsteak","eater","eatervoracious","knightsirbellas","eaterrepugnant","thrungusportobello","fiuncrazed","thrunguscrimini",
                "knightcaptainramelle2","drudgerobber","knightinvadergold","knightinvadercopper","knightgeneralcorcima2","knightgeneralcorcima1","knightbodyguardplatinum","knightdoorkeepersilver","knightcaptainbalanchi2","knightcaptainbalanchi1","knightinvaderplatinum","knightcaptainramelle1",
                "knightinvadersilver","knightdoorkeeperplatinum","knightdoorkeepergold","knightcaptainaurachon2","knightbodyguardsilver","knightcaptainaurachon1","knightcastleguardcopper","knightcastleguardgold","knightcaptainargenne1","knightcastleguardplatinum","knightcastleguardsilver",
                "knightmercenary","knightquartermastercopper","knightcaptainargenne2","knightdoorkeepercopper","fiundemented","fiunmaddenedabayar","fiunmaddenedreasearchassistant",

                "human","wallburunfortress","hollowminiongenericnewbieacademy","hollowminionnewbieacademy","teststarteventdrudge","teststopeventdrudge","emotetestdrudge","emotetestdrudge2","emotetestnpc","testeventnpc","fellowemotetestnpc",
                "cowmad","easterbunny","mosswartsmall","golemwoodsmall","golemtestisland", "olthoismall", "mysterioussarcophagus","eggsburun","eggsburunmorgluuk","isindulebeta","virindiinquisitorevent", "bossdeedultra","asheron","asheronlo",
                "golemdiamondbadtrip-xp","eaterliveopspreactd","golemsapphire","skeletongreatgeneral","skeletonadvocatedungeon","boygrubinfestedpraetorian_xp","boygrubinfestedpraetorian_nofall_xp","Viamontian Tribune",
                "knightmercenaryliveopspreactd","knightviamontianliveopspreactd","knighttribuneliveopspreactd","eatervoraciousliveopspreactd","eaterrepugnantliveopspreactd"
            };

            List<string> bossExplicitEntries = new List<string>()
            {
                "soulcrystalfenmalain","soulcrystalcaulnalain","soulcrystalshendolain", "phyntoswaspbossmonster", "golemmegamagma",
                "crystalshardsentinel","golemnepholmed","golemnepholmed_nostone","revenanttremblant","shadowspirekhayyaban", "shadowspiretufa","shadowcyst",
                "mosswartswamplordmartine","darkrevenantrytheran","rabbitbabywhite"
            };

            List<string> notBossExplicitEntries = new List<string>()
            {
                "rabbitgardenpink","rabbitgardengreen","rabbitgardenpurple","golemcoral","golemcoralceremonydisrupted","golemcoralgreen",
                "olthoimutated-xp",""
            };

            foreach (var creature in creatures)
            {
                creature.NewXpAmount = GetCreatureXP(creature.Level, creature.Hitpoints, creature.NumOfSpells);
            }

            Dictionary<int, LevelXpEntry> levelGrouped = new Dictionary<int, LevelXpEntry>();
            foreach (var creature in creatures)
            {
                if (!creature.IsAttackable)
                    continue;

                if (creature.XpAmount == 0)
                    continue;

                if (excludedEntries.Contains(creature.WeenieClassName))
                    continue;

                bool isBoss = false;
                if (bossExplicitEntries.Contains(creature.WeenieClassName))
                    isBoss = true;
                else if (creature.WeenieClassName.Contains("boss"))
                    isBoss = true;
                else if (creature.Level <= 30 && creature.Hitpoints > 150)
                    isBoss = true;
                else if (creature.Level <= 50 && creature.Hitpoints > 240)
                    isBoss = true;
                else if (creature.Level < 79 && creature.Hitpoints >= 400)
                    isBoss = true;
                else if (creature.Level < 100 && creature.Hitpoints > 650)
                    isBoss = true;
                else if (creature.Hitpoints > 800)
                    isBoss = true;

                if (notBossExplicitEntries.Contains(creature.WeenieClassName))
                    isBoss = false;

                if (isBoss)
                    continue;

                if (!levelGrouped.TryGetValue(creature.Level, out var levelEntry))
                {
                    levelEntry = new LevelXpEntry();
                    levelEntry.Level = creature.Level;
                    levelGrouped.Add(creature.Level, levelEntry);
                }

                if (creature.XpAmount < levelEntry.LowestXp)
                    levelEntry.LowestXp = creature.XpAmount;
                if (creature.XpAmount > levelEntry.HighestXp)
                    levelEntry.HighestXp = creature.XpAmount;

                if (creature.Hitpoints < levelEntry.LowestHp)
                    levelEntry.LowestHp = creature.Hitpoints;
                if (creature.Hitpoints > levelEntry.HighestHp)
                    levelEntry.HighestHp = creature.Hitpoints;

                levelEntry.TotalXp += creature.XpAmount;
                levelEntry.TotalHp += creature.Hitpoints;
                levelEntry.CreatureCount++;

                if (!levelEntry.XPs.TryGetValue(creature.XpAmount, out var creatureXPEntry))
                {
                    creatureXPEntry = new List<CreatureXpEntry>();
                    levelEntry.XPs.Add(creature.XpAmount, creatureXPEntry);
                }

                creatureXPEntry.Add(creature);
            }

            foreach (var levelEntry in levelGrouped)
            {
                levelEntry.Value.AverageXp = levelEntry.Value.TotalXp / levelEntry.Value.CreatureCount;
                levelEntry.Value.AverageHp = levelEntry.Value.TotalHp / levelEntry.Value.CreatureCount;
            }

            StreamWriter outputFile = new StreamWriter(new FileStream("./listOfCreatureXp.txt", FileMode.Create, FileAccess.Write));
            outputFile.WriteLine($"level\txpAmount\tnewXpAmount\tName");
            foreach (var creature in creatures)
            {
                if (!creature.IsAttackable)
                    continue;

                if (creature.XpAmount == 0)
                    continue;

                if (excludedEntries.Contains(creature.WeenieClassName))
                    continue;

                outputFile.WriteLine($"{creature.Level}\t{creature.XpAmount}\t{creature.NewXpAmount}\t{creature.Name}({creature.WeenieClassName})");
                outputFile.Flush();
            }
            outputFile.Close();

            outputFile = new StreamWriter(new FileStream("./listOfCreatureXp - Stats.txt", FileMode.Create, FileAccess.Write));
            outputFile.WriteLine($"level\tAmountOfEntries\tLowestXp\tHighestXp\tDifference\tAverageXp\tAverageHp\tLowestHp\tHightestHp");
            foreach (var levelEntry in levelGrouped)
            {
                outputFile.WriteLine($"{levelEntry.Value.Level}\t{levelEntry.Value.XPs.Count}\t{levelEntry.Value.LowestXp}\t{levelEntry.Value.HighestXp}\t{(levelEntry.Value.HighestXp * 100 / levelEntry.Value.LowestXp) - 100}%\t{levelEntry.Value.AverageXp}\t{levelEntry.Value.AverageHp}\t{levelEntry.Value.LowestHp}\t{levelEntry.Value.HighestHp}");
                outputFile.Flush();
            }
            outputFile.Close();
            connection.Close();
        }

        public static int GetCreatureXP(int level, int hitpoints, int numSpellInSpellbook)
        {
            double baseXp = Math.Min((1.75 * Math.Pow(level, 2)) + (20 * level), 30000);

            double hitpointsXp = hitpoints / 10 * baseXp / 35;

            double casterXp = baseXp * ((float)numSpellInSpellbook / 20);

            return (int)Math.Round(baseXp + hitpointsXp + casterXp);
        }

        public class DroppedByEntry
        {
            public int WeenieId;
            public int Level;
            public string WeenieClassName;
            public string Name;

            public DroppedByEntry(int weenieId)
            {
                WeenieId = weenieId;
            }
        }

        public class XpRewardEntry
        {
            public int SourceWeenieId;
            public string SourceWeenieClassName;
            public int EmoteId;
            public int EmoteCategory;
            public int WeenieClassId;
            public string WeenieClassNameOrQuestName;
            public int XpAmount;
            public string QuestFlag;
            public int QuestMaxRepeat;
            public int QuestTimer;
            public int NewXpAmount;

            public string SourceWeenieIdList;
            public List<DroppedByEntry> DroppedByList = new List<DroppedByEntry>();
            public int DroppedByLowestLevel = int.MaxValue;

            public XpRewardEntry()
            {
            }

            public XpRewardEntry(int emoteId, int xpAmount)
            {
                EmoteId = emoteId;
                XpAmount = xpAmount;
            }
        }

        public static void UpdateXPRewardsFromList(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));

            List<XpRewardEntry> rewards = new List<XpRewardEntry>();

            inputFile.ReadLine();
            while (!inputFile.EndOfStream)
            {
                string line = inputFile.ReadLine();
                if (line.Length <= 17 || line.Contains("xpAmount"))
                    continue;
                string[] splitLine = line.Split('\t');

                XpRewardEntry newEntry = new XpRewardEntry();
                newEntry.XpAmount = int.Parse(splitLine[0], NumberStyles.AllowThousands);
                newEntry.NewXpAmount = int.Parse(splitLine[2]);
                newEntry.WeenieClassId = int.Parse(splitLine[4]);
                newEntry.EmoteCategory = int.Parse(splitLine[5]);
                newEntry.SourceWeenieIdList = splitLine[6];

                if(newEntry.EmoteCategory != 6)
                    newEntry.QuestFlag = splitLine[3];

                rewards.Add(newEntry);
            }

            inputFile.Close();

            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql;
            MySqlCommand command;
            MySqlDataReader reader;

            foreach (var reward in rewards)
            {
                if (reward.NewXpAmount == 0)
                    continue; // 0 means do not change.

                if (reward.EmoteCategory == 6)
                {
                    sql = $"SELECT id FROM weenie_properties_emote WHERE `category` = 6 AND `object_Id` IN ({reward.SourceWeenieIdList}) AND `weenie_Class_Id` = {reward.WeenieClassId}";
                    command = new MySqlCommand(sql, connection);
                    reader = command.ExecuteReader();

                    List<int> emoteIdList = new List<int>();
                    while (reader.Read())
                        emoteIdList.Add(reader.GetInt32(0));
                    reader.Close();

                    foreach (var emoteId in emoteIdList)
                    {
                        sql = $"SELECT amount_64 FROM `weenie_properties_emote_action` WHERE TYPE = 2 AND `emote_Id`= {emoteId}";
                        command = new MySqlCommand(sql, connection);
                        reader = command.ExecuteReader();

                        int currentValue = 0;
                        if (reader.Read())
                            currentValue = reader.GetInt32(0);
                        reader.Close();

                        Debug.Assert(currentValue == reward.XpAmount);

                        sql = $"UPDATE `weenie_properties_emote_action` SET amount_64 = {reward.NewXpAmount} WHERE TYPE = 2 AND `emote_Id`= {emoteId}";
                        command = new MySqlCommand(sql, connection);
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    if (reward.QuestFlag.Length == 0)
                        continue;

                    if(reward.SourceWeenieIdList == "12050, 12050, 24577, 24577")//fix mistake while generating file of not considering the possibility the same item would be on 2 categories at once, thankfully there's only this instance.
                        sql = $"SELECT id FROM weenie_properties_emote WHERE `category` IN (12,13) AND `object_Id` IN ({reward.SourceWeenieIdList}) AND `quest`= \"{reward.QuestFlag}\"";
                    else
                        sql = $"SELECT id FROM weenie_properties_emote WHERE `category` = {reward.EmoteCategory} AND `object_Id` IN ({reward.SourceWeenieIdList}) AND `quest`= \"{reward.QuestFlag}\"";
                    command = new MySqlCommand(sql, connection);
                    reader = command.ExecuteReader();

                    List<int> emoteIdList = new List<int>();
                    while (reader.Read())
                        emoteIdList.Add(reader.GetInt32(0));
                    reader.Close();

                    foreach (var emoteId in emoteIdList)
                    {
                        sql = $"SELECT amount_64 FROM `weenie_properties_emote_action` WHERE TYPE = 2 AND `emote_Id`= {emoteId}";
                        command = new MySqlCommand(sql, connection);
                        reader = command.ExecuteReader();

                        int currentValue = 0;
                        if (reader.Read())
                            currentValue = reader.GetInt32(0);
                        reader.Close();

                        Debug.Assert(currentValue == reward.XpAmount);

                        sql = $"UPDATE `weenie_properties_emote_action` SET amount_64 = {reward.NewXpAmount} WHERE TYPE = 2 AND `emote_Id`= {emoteId}";
                        command = new MySqlCommand(sql, connection);
                        command.ExecuteNonQuery();
                    }
                }
            }

            connection.Close();
        }

        public static void CreateXPRewardsList()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=root;password=;DefaultCommandTimeout=120;database=ace_world_test");
            connection.Open();

            string sql = "SELECT emote_id, amount_64 FROM weenie_properties_emote_action WHERE type = 2";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<XpRewardEntry> rewards = new List<XpRewardEntry>();

            while (reader.Read())
                rewards.Add(new XpRewardEntry(reader.GetInt32(0), reader.GetInt32(1)));
            reader.Close();

            foreach (var reward in rewards)
            {
                sql = $"SELECT object_Id, category, weenie_Class_Id, quest FROM weenie_properties_emote WHERE id = {reward.EmoteId}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    reward.SourceWeenieId = reader.GetInt32(0);
                    reward.EmoteCategory = reader.GetInt32(1);
                    if (reward.EmoteCategory == 12 || reward.EmoteCategory == 13 || reward.EmoteCategory == 22 || reward.EmoteCategory == 23 || reward.EmoteCategory == 32)
                    {
                        reward.WeenieClassId = 0;
                        reward.WeenieClassNameOrQuestName = reader.GetString(3);
                        reward.QuestFlag = reward.WeenieClassNameOrQuestName;
                    }
                    else if (reward.EmoteCategory == 8)
                    {
                        reward.WeenieClassId = 0;
                        reward.WeenieClassNameOrQuestName = "";
                    }
                    else
                        reward.WeenieClassId = reader.GetInt32(2);

                }
                reader.Close();

                if (reward.EmoteCategory == 6)
                {
                    sql = $"SELECT class_Name FROM weenie WHERE class_Id = {reward.WeenieClassId}";
                    command = new MySqlCommand(sql, connection);
                    reader = command.ExecuteReader();

                    if (reader.Read())
                        reward.WeenieClassNameOrQuestName = reader.GetString(0);
                    reader.Close();
                }

                sql = $"SELECT class_Name FROM weenie WHERE class_Id = {reward.SourceWeenieId}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if (reader.Read())
                    reward.SourceWeenieClassName = reader.GetString(0);
                reader.Close();

                if (reward.QuestFlag == null && reward.WeenieClassId != 0)
                {
                    sql = $"SELECT value FROM `weenie_properties_string` WHERE `type` = 33 AND `object_Id` = {reward.WeenieClassId}";
                    command = new MySqlCommand(sql, connection);
                    reader = command.ExecuteReader();

                    if (reader.Read())
                        reward.QuestFlag = reader.GetString(0);
                    reader.Close();
                }

                if (reward.QuestFlag != null)
                {
                    sql = $"SELECT min_delta, max_solves FROM quest WHERE name = \"{reward.QuestFlag}\"";
                    command = new MySqlCommand(sql, connection);
                    reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        reward.QuestTimer = reader.GetInt32(0);
                        reward.QuestMaxRepeat = reader.GetInt32(1);
                    }
                    reader.Close();

                    int modifier;
                    if (reward.QuestMaxRepeat == 1)
                        modifier = 5000; // once per character
                    else if (reward.QuestTimer >= 60 * 60 * 24 * 7 * 21)
                        modifier = 4000; // once every 3 weeks or more
                    else if (reward.QuestTimer >= 60 * 60 * 24 * 7)
                        modifier = 3000; // once a week or more
                    else if (reward.QuestTimer >= 60 * 60 * 24)
                        modifier = 2000; // once a day or more
                    else
                        modifier = 1000; // more than once per day

                    reward.NewXpAmount = -modifier;
                }

                if (reward.WeenieClassId == 0)
                    continue;

                sql = $"SELECT object_id FROM `weenie_properties_create_list` WHERE `destination_Type` IN (1, 8, 9) AND `weenie_Class_Id`= {reward.WeenieClassId}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                    reward.DroppedByList.Add(new DroppedByEntry(reader.GetInt32(0)));
                reader.Close();

                List<string> excludedEntries = new List<string>()
                {
                    "easterbunny"
                };

                foreach (var entry in reward.DroppedByList)
                {
                    sql = $"SELECT value FROM `weenie_properties_int` WHERE `type` = 25 AND `object_Id` = {entry.WeenieId}";
                    command = new MySqlCommand(sql, connection);
                    reader = command.ExecuteReader();

                    if (reader.Read())
                        entry.Level = reader.GetInt32(0);
                    reader.Close();

                    sql = $"SELECT value FROM `weenie_properties_string` WHERE `type` = 1 AND `object_Id` = {entry.WeenieId}";
                    command = new MySqlCommand(sql, connection);
                    reader = command.ExecuteReader();

                    if (reader.Read())
                        entry.Name = reader.GetString(0);
                    reader.Close();

                    sql = $"SELECT class_Name FROM `weenie` WHERE `class_Id` = {entry.WeenieId}";
                    command = new MySqlCommand(sql, connection);
                    reader = command.ExecuteReader();

                    if (reader.Read())
                        entry.WeenieClassName = reader.GetString(0);
                    reader.Close();

                    if (excludedEntries.Contains(entry.WeenieClassName))
                        continue;

                    if (entry.Level > 0 && entry.Level < reward.DroppedByLowestLevel)
                    {
                        reward.DroppedByLowestLevel = entry.Level;
                    }
                }

                if (reward.DroppedByLowestLevel != int.MaxValue)
                {
                    if (reward.QuestFlag != null)
                    {
                        int modifier;
                        if (reward.QuestMaxRepeat == 1)
                            modifier = 5000; // once per character
                        else if (reward.QuestTimer >= 60 * 60 * 24 * 7 * 21)
                            modifier = 4000; // once every 3 weeks or more
                        else if (reward.QuestTimer >= 60 * 60 * 24 * 7)
                            modifier = 3000; // once a week or more
                        else if (reward.QuestTimer >= 60 * 60 * 24)
                            modifier = 2000; // once a day or more
                        else
                            modifier = 1000; // more than once per day

                        reward.NewXpAmount = -(modifier + reward.DroppedByLowestLevel);
                    }
                    else
                        reward.NewXpAmount = -reward.DroppedByLowestLevel;
                }
            }

            Dictionary<string, XpRewardEntry> rewardsGrouped = new Dictionary<string, XpRewardEntry>();

            foreach (var reward in rewards)
            {
                string key = $"{reward.WeenieClassNameOrQuestName}-{reward.XpAmount}";
                XpRewardEntry entry;
                if (rewardsGrouped.TryGetValue(key, out entry))
                {
                    entry.SourceWeenieIdList += $", {reward.SourceWeenieId}";
                    entry.SourceWeenieClassName += $", {reward.SourceWeenieClassName}";
                }
                else
                {
                    reward.SourceWeenieIdList = reward.SourceWeenieId.ToString();
                    rewardsGrouped.Add(key, reward);
                }
            }

            StreamWriter outputFile = new StreamWriter(new FileStream("./listOfXpRewards.txt", FileMode.Create, FileAccess.Write));
            outputFile.WriteLine($"xpAmount\tnewXpAmount\tWeenieClassNameOrQuestName\tWeenieClassId\tEmoteCategory\tSourceWeenieId\tSourceWeenieClassName\tDroppedBy");
            foreach (var reward in rewardsGrouped)
            {
                outputFile.Write($"{reward.Value.XpAmount}\t{reward.Value.NewXpAmount}\t{reward.Value.WeenieClassNameOrQuestName}\t{reward.Value.WeenieClassId}\t{reward.Value.EmoteCategory}\t{reward.Value.SourceWeenieIdList}\t{reward.Value.SourceWeenieClassName}\t");
                foreach (var entry in reward.Value.DroppedByList)
                {
                    outputFile.Write($"{entry.Name}({entry.WeenieClassName} - level: {entry.Level}),");
                }
                outputFile.WriteLine();
                outputFile.Flush();
            }

            connection.Close();
        }

        public static void AddThrownWeaponsToVendors()
        {
            //Add thrown weapons to everyone that sells arrows. Add quarrels and atlatl darts to those that are missing them as well.
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=root;password=;DefaultCommandTimeout=120;database=ace_world_test");
            connection.Open();

            string sql = "SELECT object_Id FROM weenie_properties_create_list WHERE weenie_Class_Id = 300 AND destination_Type = 4";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<int> vendors = new List<int>();

            while (reader.Read())
            {
                vendors.Add(reader.GetInt32(0));
            }
            reader.Close();

            int count = 0;
            foreach (var vendor in vendors)
            {
                sql = $"DELETE FROM weenie_properties_create_list WHERE weenie_Class_Id = 316 AND destination_Type = 4 AND object_id = {vendor}";
                command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                sql = $"DELETE FROM weenie_properties_create_list WHERE weenie_Class_Id = 304 AND destination_Type = 4 AND object_id = {vendor}";
                command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                sql = $"DELETE FROM weenie_properties_create_list WHERE weenie_Class_Id = 310 AND destination_Type = 4 AND object_id = {vendor}";
                command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                sql = $"DELETE FROM weenie_properties_create_list WHERE weenie_Class_Id = 315 AND destination_Type = 4 AND object_id = {vendor}";
                command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                sql = $"DELETE FROM weenie_properties_create_list WHERE weenie_Class_Id = 320 AND destination_Type = 4 AND object_id = {vendor}";
                command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                sql = $"DELETE FROM weenie_properties_create_list WHERE weenie_Class_Id = 343 AND destination_Type = 4 AND object_id = {vendor}";
                command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                sql = $"DELETE FROM weenie_properties_create_list WHERE weenie_Class_Id = 317 AND destination_Type = 4 AND object_id = {vendor}";
                command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                bool sellsAtlatlDart = false;
                sql = $"SELECT id FROM weenie_properties_create_list WHERE weenie_Class_Id = 12464 AND destination_Type = 4 AND object_id = {vendor}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();
                if (reader.Read())
                    sellsAtlatlDart = true;
                reader.Close();

                bool sellsBow = false;
                sql = $"SELECT id FROM weenie_properties_create_list WHERE weenie_Class_Id IN (307,341,360,141,363,334) AND destination_Type = 4 AND object_id = {vendor}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();
                if (reader.Read())
                    sellsBow = true;
                reader.Close();

                bool sellsAtlatl = false;
                sql = $"SELECT id FROM weenie_properties_create_list WHERE weenie_Class_Id = 12463 AND destination_Type = 4 AND object_id = {vendor}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();
                if (reader.Read())
                    sellsAtlatl = true;
                reader.Close();

                bool sellsQuarrel = false;
                sql = $"SELECT id FROM weenie_properties_create_list WHERE weenie_Class_Id = 305 AND destination_Type = 4 AND object_id = {vendor}";
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();
                if (reader.Read())
                    sellsQuarrel = true;
                reader.Close();

                sql = $"INSERT INTO weenie_properties_create_list (object_Id, destination_Type, weenie_Class_Id, stack_Size, palette, shade, try_To_Bond) VALUES";

                string prefix = "";
                if (!sellsQuarrel)
                {
                    sql += $"{prefix}({vendor}, 4, 305, -1, 0, 0, 0)";
                    prefix = ",";
                }
                if (sellsBow && !sellsAtlatl)
                {
                    sql += $"{prefix}({vendor}, 4, 12463, -1, 0, 0, 0)";
                    prefix = ",";
                }
                if (!sellsAtlatlDart)
                {
                    sql += $"{prefix}({vendor}, 4, 12464, -1, 0, 0, 0)";
                    prefix = ",";
                }
                sql += $"{prefix}({vendor}, 4, 316, -1, 0, 0, 0)";
                prefix = ",";
                sql += $"{prefix}({vendor}, 4, 315, -1, 0, 0, 0)";
                sql += $"{prefix}({vendor}, 4, 320, -1, 0, 0, 0)";
                sql += $"{prefix}({vendor}, 4, 304, -1, 0, 0, 0)";
                sql += $"{prefix}({vendor}, 4, 310, -1, 0, 0, 0)";
                sql += $"{prefix}({vendor}, 4, 317, -1, 0, 0, 0)";
                sql += $"{prefix}({vendor}, 4, 343, -1, 0, 0, 0)";
                command = new MySqlCommand(sql, connection);
                count += command.ExecuteNonQuery();
            }
            connection.Close();

            Console.WriteLine($"Added {count} entries.");
        }

        public static void ConvertSomeSoCStoTwoHanded()
        {
            Console.WriteLine($"Converting some Silifi of Crimson Stars to Two Handed...");

            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = "";
            MySqlCommand command;
            MySqlDataReader reader;

            List<int> socsList = new List<int>() { 6665, 6666, 6667, 6668, 6676, 6677, 6678, 6679, 6680, 6681, 6682, 6691, 6692, 6693, 6694, 6702, 6703, 6704, 6705, 6706, 6707, 6708, 6717, 6718, 6719, 6720, 6728, 6729, 6730, 6731, 6732, 6733, 6734, 6743, 6744, 6745, 6746, 6754, 6755, 6756, 6757, 6758, 6759, 6760, 22953, 22957, 22961, 22962, 22963, 22967, 22968, 22969, 22973, 22974, 22975, 22979, 22983, 22987, 22988, 22989, 22993, 22994, 22995, 22999, 23000, 23001, 23005, 23009, 23013, 23014, 23015, 23019, 23020, 23021, 23025, 23026, 23027 };

            int count = 0;
            sql = $"UPDATE weenie_properties_int SET value = 33554432 WHERE type = 9 AND object_Id IN ({string.Join(",", socsList)})";
            command = new MySqlCommand(sql, connection); /* ValidLocations - TwoHanded */
            count += command.ExecuteNonQuery();

            sql = $"UPDATE weenie_properties_int SET value = 8 WHERE type = 46 AND object_Id IN ({string.Join(",", socsList)})";
            command = new MySqlCommand(sql, connection);  /* DefaultCombatStyle - TwoHanded */
            count += command.ExecuteNonQuery();

            sql = $"UPDATE weenie_properties_int SET value = 5 WHERE type = 51 AND object_Id IN ({string.Join(",", socsList)})";
            command = new MySqlCommand(sql, connection);  /* CombatUse - TwoHanded */
            count += command.ExecuteNonQuery();

            foreach (var entry in socsList)
            {
                sql = $"INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)" +
                      $"VALUES({entry}, 292, 2) /* Cleaving */" +
                      $"    , ({entry}, 353, 11) /* WeaponType - TwoHanded */;";
                command = new MySqlCommand(sql, connection);
                count += command.ExecuteNonQuery();

                sql = $"DELETE FROM weenie_properties_spell_book WHERE object_Id = {entry} AND spell IN (35,1612,1613,1614,1615,1616,2096)";
                command = new MySqlCommand(sql, connection);  // Remove Blood Drinker
                count += command.ExecuteNonQuery();

                sql = $"SELECT value FROM weenie_properties_int WHERE object_Id = {entry} AND type = 44"; /* Damage */
                command = new MySqlCommand(sql, connection);
                reader = command.ExecuteReader();

                if(reader.Read())
                {
                    int currentDamage = reader.GetInt32(0);
                    int newDamage = (int)Math.Round(currentDamage / 3.0f);

                    reader.Close();
                    sql = $"UPDATE weenie_properties_int SET value = {newDamage} WHERE object_Id = {entry} AND type = 44";
                    command = new MySqlCommand(sql, connection); /* Damage */
                    count += command.ExecuteNonQuery();
                }
                else
                    reader.Close();
            }

            Console.WriteLine($"Updated {count} entries.");
        }

        public static void IncreaseThrownWeaponsStackSizeTo250()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=root;password=;DefaultCommandTimeout=120;database=ace_world_test");
            connection.Open();

            string sql = "SELECT class_Id FROM weenie WHERE type = 4"; // Missile
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<int> missiles = new List<int>();

            while (reader.Read())
            {
                missiles.Add(reader.GetInt32(0));
            }
            reader.Close();

            int count = 0;
            foreach (var missile in missiles)
            {
                sql = $"UPDATE weenie_properties_int SET value = 250 WHERE type = 11 AND value = 100 AND object_Id = {missile}";
                command = new MySqlCommand(sql, connection);
                count += command.ExecuteNonQuery();
            }
            connection.Close();

            Console.WriteLine($"Updated {count} entries.");
        }

        public static void AddSpellComponentPouchToVendors()
        {
            //Add portal gems to everyone that sells lead scarabs.
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=root;password=;DefaultCommandTimeout=120;database=ace_world_test");
            connection.Open();

            string sql = "SELECT object_Id FROM weenie_properties_create_list WHERE weenie_Class_Id = 136 AND destination_Type = 4"; // Pack
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<int> vendors = new List<int>();

            while (reader.Read())
            {
                vendors.Add(reader.GetInt32(0));
            }
            reader.Close();

            List<int> palettes = new List<int>() { 0, 84, 85, 86, 88, 89, 90, 91, 92, 93 };

            int count = 0;
            foreach (var vendor in vendors)
            {
                sql = $"INSERT INTO weenie_properties_create_list (object_Id, destination_Type, weenie_Class_Id, stack_Size, palette, shade, try_To_Bond)" +
                      $"VALUES ({vendor}, 4, 50009, -1, {palettes[Utils.getRandomNumber(0, palettes.Count - 1)]}, 1.0, 0)";
                command = new MySqlCommand(sql, connection);
                count += command.ExecuteNonQuery();
            }
            connection.Close();

            Console.WriteLine($"Added {count} entries.");
        }

        public static void RedistributeFoodIngredientsToGrocersAndFarmers()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = "SELECT object_Id FROM weenie_properties_string WHERE `type` = 5 AND `value` IN (\"Grocer\",\"Farmer\")";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<int> vendors = new List<int>();

            while (reader.Read())
            {
                vendors.Add(reader.GetInt32(0));
            }
            reader.Close();

            int count = 0;
            foreach (var vendor in vendors)
            {
                // Remove all existing ones so we can readd them in the correct order.
                sql = $"DELETE FROM weenie_properties_create_list WHERE object_Id = {vendor} AND destination_Type = 4 AND weenie_Class_Id IN (23327,23326,258,264,22578,5758,260,8232,547,4746,2463,546,4761,4768,262,263,265,4753,5633,4766,4755,5780,4763,5803,5794,14795,14789,7825,13222)";
                command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                sql = $"INSERT INTO weenie_properties_create_list (object_Id, destination_Type, weenie_Class_Id, stack_Size, palette, shade, try_To_Bond)" +
                      $"VALUES ({vendor}, 4, 23327, -1, 0, 0, 0)" + // Simple Dried Rations
                            $",({vendor}, 4, 23326, -1, 0, 0, 0)" + // Elaborate Dried Rations

                            $",({vendor}, 4, 258, -1, 0, 0, 0)" + // Apple
                            $",({vendor}, 4, 264, -1, 0, 0, 0)" + // Grapes
                            $",({vendor}, 4, 22578, -1, 0, 0, 0)" + // Bunch of Nanners
                            $",({vendor}, 4, 5758, -1, 0, 0, 0)" + // Carrot
                            $",({vendor}, 4, 260, -1, 0, 0, 0)" + // Cabbage
                            $",({vendor}, 4, 8232, -1, 0, 0, 0)" + // Pumpkin
                            $",({vendor}, 4, 547, -1, 0, 0, 0)" + // Brimstone-cap Mushroom

                            $",({vendor}, 4, 4746, -1, 0, 0, 0)" + // Water
                            $",({vendor}, 4, 2463, -1, 0, 0, 0)" + // Milk
                            $",({vendor}, 4, 546, -1, 0, 0, 0)" + // Egg
                            $",({vendor}, 4, 4761, -1, 0, 0, 0)" + // Flour
                            $",({vendor}, 4, 4768, -1, 0, 0, 0)" + // Uncooked Rice

                            $",({vendor}, 4, 262, -1, 0, 0, 0)" + // Chicken
                            $",({vendor}, 4, 263, -1, 0, 0, 0)" + // Fish
                            $",({vendor}, 4, 265, -1, 0, 0, 0)" + // Meat
                            $",({vendor}, 4, 4753, -1, 0, 0, 0)" + // Side of Beef
                            $",({vendor}, 4, 5633, -1, 0, 0, 0)" + // Rabbit Carcass

                            $",({vendor}, 4, 4766, -1, 0, 0, 0)" + // Rennet
                            $",({vendor}, 4, 4755, -1, 0, 0, 0)" + // Brine
                            $",({vendor}, 4, 5780, -1, 0, 0, 0)" + // Cinnamon Bark
                            $",({vendor}, 4, 4763, -1, 0, 0, 0)" + // Honey
                            $",({vendor}, 4, 5803, -1, 0, 0, 0)" + // Oregano
                            $",({vendor}, 4, 5794, -1, 0, 0, 0)" + // Hot Pepper
                            $",({vendor}, 4, 14795, -1, 0, 0, 0)" + // Nutmeg
                            $",({vendor}, 4, 14789, -1, 0, 0, 0)" + // Ginger
                            $",({vendor}, 4, 7825, -1, 0, 0, 0)" + // Brown Beans
                            $",({vendor}, 4, 13222, -1, 0, 0, 0)"; // Peppermint Stick 
                command = new MySqlCommand(sql, connection);
                count += command.ExecuteNonQuery();
            }
            connection.Close();

            Console.WriteLine($"Added {count} entries.");
        }

        public static void RemovePortalGemsFromAllSpellComponentVendorsAddToJewelers()
        {
            // Remove portal gems from everyone that sells lead scarabs.
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            string sql = "SELECT object_Id FROM weenie_properties_create_list WHERE weenie_Class_Id = 691 AND destination_Type = 4";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<int> vendors = new List<int>();

            while (reader.Read())
            {
                vendors.Add(reader.GetInt32(0));
            }
            reader.Close();

            int count = 0;
            foreach (var vendor in vendors)
            {
                sql = $"DELETE FROM weenie_properties_create_list WHERE object_Id = {vendor} AND destination_Type = 4 AND weenie_Class_Id IN (26639,8973,8984,8980,8983,8981,8978,8976,8977,8979,50000,50001,50002,50003,50004,50005,50006,50007,50008)";
                command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }

            // Add them to jewelers.
            sql = "SELECT object_Id FROM weenie_properties_string WHERE `type` = 5 AND `value` IN (\"Jeweler\")"; 
            command = new MySqlCommand(sql, connection);
            reader = command.ExecuteReader();

            vendors.Clear();

            while (reader.Read())
            {
                vendors.Add(reader.GetInt32(0));
            }
            reader.Close();

            foreach(var vendor in vendors)
            {
                // Remove all existing ones so we can readd them in the correct order.
                sql = $"DELETE FROM weenie_properties_create_list WHERE object_Id = {vendor} AND destination_Type = 4 AND weenie_Class_Id IN (26639,8973,8984,8980,8983,8981,8978,8976,8977,8979,50000,50001,50002,50003,50004,50005,50006,50007,50008)";
                command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                sql = $"INSERT INTO weenie_properties_create_list (object_Id, destination_Type, weenie_Class_Id, stack_Size, palette, shade, try_To_Bond)" +
                      $"VALUES ({vendor}, 4, 50000, -1, 0, 0, 0)" + // primary portal recall gem
                            $",({vendor}, 4, 50001, -1, 0, 0, 0)" + // primary portal recall gem
                            $",({vendor}, 4, 50002, -1, 0, 0, 0)" + // secondary portal recall gem
                            $",({vendor}, 4, 50007, -1, 0, 0, 0)" + // lifestone recall gem
                            $",({vendor}, 4, 50005, -1, 0, 0, 0)" + // primary portal summon gem
                            $",({vendor}, 4, 50006, -1, 0, 0, 0)" + // secondary portal summon gem
                            $",({vendor}, 4, 50003, -1, 0, 0, 0)" + // primary portal tie gem
                            $",({vendor}, 4, 50004, -1, 0, 0, 0)" + // secondary portal tie gem
                            $",({vendor}, 4, 50008, -1, 0, 0, 0)" + // lifestone tie gem
                            $",({vendor}, 4, 26639, -1, 0, 0, 0)" + // Xarabydun Portal Summoning Gem
                            $",({vendor}, 4,  8973, -1, 0, 0, 0)" + // Al-Arqas Portal Gem
                            $",({vendor}, 4,  8984, -1, 0, 0, 0)" + // Yaraq Portal Gem
                            $",({vendor}, 4,  8980, -1, 0, 0, 0)" + // Samsur Portal Gem
                            $",({vendor}, 4,  8983, -1, 0, 0, 0)" + // Yanshi Portal Gem
                            $",({vendor}, 4,  8981, -1, 0, 0, 0)" + // Shoushi Portal Gem
                            $",({vendor}, 4,  8978, -1, 0, 0, 0)" + // Nanto Portal Gem
                            $",({vendor}, 4,  8976, -1, 0, 0, 0)" + // Holtburg Portal Gem
                            $",({vendor}, 4,  8977, -1, 0, 0, 0)" + // Lytelthorpe Portal Gem
                            $",({vendor}, 4,  8979, -1, 0, 0, 0)";  // Rithwic Portal Gem
                command = new MySqlCommand(sql, connection);
                count += command.ExecuteNonQuery();
            }
            connection.Close();

            Console.WriteLine($"Added {count} entries.");
        }

        public static void RedistributeSpellServicesToVendors()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=ACEmulator;password=password;DefaultCommandTimeout=120;database=ace_world_customDM");
            connection.Open();

            //Add creature buffs to everyone that sells lead scarabs.
            string sql = "SELECT object_Id FROM weenie_properties_create_list WHERE weenie_Class_Id = 691 AND destination_Type = 4";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<int> vendors = new List<int>();

            while (reader.Read())
            {
                vendors.Add(reader.GetInt32(0));
            }
            reader.Close();

            int count = 0;
            foreach (var vendor in vendors)
            {
                // Remove all existing ones so we can readd them in the correct order.
                sql = $"DELETE FROM weenie_properties_create_list WHERE object_Id = {vendor} AND destination_Type = 4 AND weenie_Class_Id IN (4384,4601,30664,30665,4602,4603,30670,30671,4604,4605,30668,30669,4606,4607,30674,30663,4608,4609,30672,30673,4610,4611,30666,30667,8180,8181,8182,8183,8184,8185,4450,4587,4588,4589,4590,4591,4592,4593,4594,4595,4596,4597,4598,4599,4600,50051,50052,50053)";
                command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                sql = $"INSERT INTO weenie_properties_create_list (object_Id, destination_Type, weenie_Class_Id, stack_Size, palette, shade, try_To_Bond)" +
                $"VALUES ({vendor}, 4, 30664, -1, 0, 0, 0)" +  //servicestrengthother3
                      $",({vendor}, 4, 30670, -1, 0, 0, 0)" +  //serviceenduranceother3
                      $",({vendor}, 4, 30668, -1, 0, 0, 0)" +  //servicecoordinationother3
                      $",({vendor}, 4, 30674, -1, 0, 0, 0)" +  //servicequicknessother3
                      $",({vendor}, 4, 30672, -1, 0, 0, 0)" +  //servicefocusother3
                      $",({vendor}, 4, 30666, -1, 0, 0, 0)";  //servicewillpowerother3

                command = new MySqlCommand(sql, connection);
                count += command.ExecuteNonQuery();
            }

            //Add heals, regens and dispels toto everyone that sells health potions.
            sql = "SELECT object_Id FROM weenie_properties_create_list WHERE weenie_Class_Id = 377 AND destination_Type = 4";
            command = new MySqlCommand(sql, connection);
            reader = command.ExecuteReader();

            vendors.Clear();
            while (reader.Read())
            {
                vendors.Add(reader.GetInt32(0));
            }
            reader.Close();

            foreach (var vendor in vendors)
            {
                // Remove all existing ones so we can readd them in the correct order.
                sql = $"DELETE FROM weenie_properties_create_list WHERE object_Id = {vendor} AND destination_Type = 4 AND weenie_Class_Id IN (4384,4601,30664,30665,4602,4603,30670,30671,4604,4605,30668,30669,4606,4607,30674,30663,4608,4609,30672,30673,4610,4611,30666,30667,8180,8181,8182,8183,8184,8185,4450,4587,4588,4589,4590,4591,4592,4593,4594,4595,4596,4597,4598,4599,4600,50051,50052,50053)";
                command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                sql = $"INSERT INTO weenie_properties_create_list (object_Id, destination_Type, weenie_Class_Id, stack_Size, palette, shade, try_To_Bond)" +
                $"VALUES ({vendor}, 4,  4588, -1, 0, 0, 0)" +  //servicehealother3
                      $",({vendor}, 4,  4591, -1, 0, 0, 0)" +  //servicerevitalizeother3
                      $",({vendor}, 4,  4594, -1, 0, 0, 0)" +  //servicemanaboost3
                      $",({vendor}, 4, 50051, -1, 0, 0, 0)" +  //serviceregenerateother3
                      $",({vendor}, 4, 50052, -1, 0, 0, 0)" +  //servicerejuvenationother3
                      $",({vendor}, 4, 50053, -1, 0, 0, 0)" +  //servicemanarenewal3
                      $",({vendor}, 4,  8182, -1, 0, 0, 0)" +  //servicedispelother3
                      $",({vendor}, 4,  8185, -1, 0, 0, 0)";   //servicedispelother6

                command = new MySqlCommand(sql, connection);
                count += command.ExecuteNonQuery();
            }
            connection.Close();

            Console.WriteLine($"Added {count} entries.");
        }

        public static void RemoveAllNonApartmentHouses()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=root;password=;DefaultCommandTimeout=120;database=ace_world_test");
            connection.Open();

            string sql = "SELECT * FROM weenie WHERE type = 53";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<int> allHousesList = new List<int>();

            while (reader.Read())
            {
                allHousesList.Add(int.Parse(reader[0].ToString()));
            }
            reader.Close();

            List<int> housesList = new List<int>();
            sql = $"SELECT * FROM weenie_properties_string WHERE object_Id IN ({string.Join(",", allHousesList)})";
            command = new MySqlCommand(sql, connection);
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (reader[3].ToString() != "Apartment")
                    housesList.Add(int.Parse(reader[1].ToString()));
            }
            reader.Close();

            // Clear their instances
            List<int> parentGUIDsToClear = new List<int>();
            List<int> landblocksList = new List<int>();
            sql = $"SELECT * FROM landblock_instance WHERE weenie_Class_Id IN ({string.Join(",", housesList)})";
            command = new MySqlCommand(sql, connection);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                parentGUIDsToClear.Add(int.Parse(reader[0].ToString()));

                int landblockId = int.Parse(reader[1].ToString());
                if (!landblocksList.Contains(landblockId))
                    landblocksList.Add(landblockId);
            }
            reader.Close();

            List<int> GUIDsToClear = new List<int>();
            sql = $"SELECT * FROM landblock_instance_link WHERE parent_GUID IN ({string.Join(",", parentGUIDsToClear)})";
            command = new MySqlCommand(sql, connection);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                GUIDsToClear.Add(int.Parse(reader[2].ToString()));
            }
            reader.Close();

            int removed;
            sql = $"DELETE FROM weenie WHERE class_Id IN ({string.Join(",", housesList)})";
            command = new MySqlCommand(sql, connection);
            removed = command.ExecuteNonQuery();
            Console.WriteLine($"Removed {removed} entries from weenies.");

            sql = $"DELETE FROM landblock_instance WHERE weenie_Class_Id IN ({string.Join(",", housesList)})";
            command = new MySqlCommand(sql, connection);
            removed = command.ExecuteNonQuery();
            Console.WriteLine($"Removed {removed} entries from landblock_instance.");

            sql = $"DELETE FROM landblock_instance WHERE guid IN ({string.Join(",", GUIDsToClear)})";
            command = new MySqlCommand(sql, connection);
            removed = command.ExecuteNonQuery();
            Console.WriteLine($"Removed {removed} child entries from landblock_instance.");

            List<int> listOfRemainingObjects = new List<int>();
            sql = $"SELECT weenie_Class_Id FROM landblock_instance WHERE landblock IN ({string.Join(",", landblocksList)})";
            command = new MySqlCommand(sql, connection);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                listOfRemainingObjects.Add(int.Parse(reader[0].ToString()));
            }
            reader.Close();

            sql = $"DELETE FROM landblock_instance WHERE landblock IN ({string.Join(",", landblocksList)})";
            command = new MySqlCommand(sql, connection);
            removed = command.ExecuteNonQuery();
            Console.WriteLine($"Removed {removed} entries from landblock_instance.");

            StreamWriter outputFile = new StreamWriter(new FileStream("./ListOfSettlementLandblocks.txt", FileMode.Create, FileAccess.Write)); // Let's save this so we can remove them from the cell map as well.
            foreach (var entry in landblocksList)
            {
                outputFile.WriteLine($"{(entry << 16).ToString("x8")}");
                outputFile.Flush();
            }

            connection.Close();
        }
    }
}