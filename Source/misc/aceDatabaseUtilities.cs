using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;

namespace Melt
{
    public static class AceDatabaseUtilities
    {
        public static void RemoveAllNonApartmentHouses()
        {
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=root;password=;DefaultCommandTimeout=120;database=ace_world");
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
            }

            connection.Close();
        }
    }
}