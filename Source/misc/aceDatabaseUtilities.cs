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
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=root;password=;DefaultCommandTimeout=120;database=ace_world_test");
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

            StreamWriter outputFile = new StreamWriter(new FileStream("./listOfCreatureXp - Comparison.txt", FileMode.Create, FileAccess.Write));
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

            outputFile = new StreamWriter(new FileStream("./listOfCreatureXp.txt", FileMode.Create, FileAccess.Write));
            outputFile.WriteLine($"level\txpAmount\tWeenieClassIds\tNames");
            foreach (var level in levelGrouped)
            {
                foreach (var xpEntry in level.Value.XPs)
                {
                    outputFile.Write(level.Value.Level);
                    outputFile.Write("\t");
                    outputFile.Write(xpEntry.Key);
                    outputFile.Write("\t");
                    foreach (var entry in xpEntry.Value)
                    {
                        outputFile.Write(entry.WeenieId);
                        outputFile.Write(",");
                    }
                    outputFile.Write("\t");
                    foreach (var entry in xpEntry.Value)
                    {
                        outputFile.Write($"{entry.Name}({entry.WeenieClassName} - hp: {entry.Hitpoints})");
                        outputFile.Write(",");
                    }
                    outputFile.WriteLine();
                }
                outputFile.Flush();
            }
            outputFile.Close();

            connection.Close();
        }

        public static int GetCreatureXP(int level, int hitpoints, int numSpellInSpellbook)
        {
            double baseXp = Math.Min((1.75 * Math.Pow(level, 2)) + (20 * level), 30000);

            double hitpointsXp = hitpoints / 10 * baseXp / 35;

            double casterXp = baseXp * (numSpellInSpellbook / 20);

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

            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=root;password=;DefaultCommandTimeout=120;database=ace_world_test");
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

        public static void AddPortalGemsToAllSpellComponentVendors()
        {
            //Add portal gems to everyone that sells lead scarabs.
            var connection = new MySqlConnection($"server=127.0.0.1;port=3306;user=root;password=;DefaultCommandTimeout=120;database=ace_world_test");
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
            foreach(var vendor in vendors)
            {
                sql = $"INSERT INTO weenie_properties_create_list (object_Id, destination_Type, weenie_Class_Id, stack_Size, palette, shade, try_To_Bond)" +
                      $"VALUES ({vendor}, 4, 50000, -1, 0, 0, 0)" + // portal recall gem
                            $",({vendor}, 4, 50001, -1, 0, 0, 0)" + // primary portal recall gem
                            $",({vendor}, 4, 50002, -1, 0, 0, 0)" + // secondary portal recall gem
                            $",({vendor}, 4, 50007, -1, 0, 0, 0)" + // lifestone recall gem
                            $",({vendor}, 4, 50005, -1, 0, 0, 0)" + // primary portal summon gem
                            $",({vendor}, 4, 50006, -1, 0, 0, 0)" + // secondary portal summon gem
                            $",({vendor}, 4, 50003, -1, 0, 0, 0)" + // primary portal tie gem
                            $",({vendor}, 4, 50004, -1, 0, 0, 0)" + // secondary portal tie gem
                            $",({vendor}, 4, 50008, -1, 0, 0, 0)";  // lifestone tie gem
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