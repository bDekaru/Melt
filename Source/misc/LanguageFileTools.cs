using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ACE.DatLoader;
using ACE.DatLoader.FileTypes;

namespace Melt
{
    class LanguageFileTools
    {
        LanguageDatDatabase LanguageDat;
        public LanguageFileTools(string filename)
        {
            LanguageDat = new LanguageDatDatabase(filename);
        }

        public void ModifyForInfiltration()
        {
            Console.WriteLine("Modifying CharGen text to Infiltration...");

            StringTable stringTable = LanguageDat.ReadFromDat<StringTable>(0x23000002);

            foreach (var stringTableEntry in stringTable.StringTableData)
            {
                switch (stringTableEntry.Id)
                {
                    case 0x0F9C8FC2: // Trained Starting Skills:\n
                        stringTableEntry.Strings[0] = "";
                        break;
                    case 0x07C77F43: // Jump, Loyalty, Magic Defense, Run and Salvaging.\n\n
                        stringTableEntry.Strings[0] = "";
                        break;
                    case 0x004B7EE2: // Bonus Racial Skills:\n
                        stringTableEntry.Strings[0] = "Trained Starting Skills:\n";
                        break;
                    case 0x0206BBD4: // You gain mastery in Dagger and Bow which gives you a damage rating bonus of 5 when using those types of weapons.\n\nYou start with the 'Jack of All Trades' augmentation which raises all of your skills by 5.\n\n
                        stringTableEntry.Strings[0] = "Assess Person and Dagger\n";
                        break;
                    case 0x0630F8A4: // You gain mastery in Staff and Magic spells which gives you a damage rating bonus of 5 when using those types of attacks.\n\nYou start with the 'Jack of All Trades' augmentation which raises all of your skills by 5.\n\n
                        stringTableEntry.Strings[0] = "Item Tinkering and Staff\n";
                        break;
                    case 0x06166A24: // You gain mastery in Unarmed weaponry and Bow which gives you a damage rating bonus of 5 when using those types of weapons.\n\nYou start with the 'Jack of All Trades' augmentation which raises all of your skills by 5.\n\n
                        stringTableEntry.Strings[0] = "Unarmed Combat\n";
                        break;
                    case 0x00166F04: // You gain mastery in Sword and Crossbow which gives you a damage rating bonus of 5 when using those types of weapons.\n\nYou start with the 'Jack of All Trades' augmentation which raises all of your skills by 5.\n\n
                    case 0x0335BA44: // You gain mastery in Unarmed weaponry and Crossbow which gives you a damage rating bonus of 5 when using those types of weapons.\n\nYou start with the 'Eye of the Remorseless' augmentation which gives 1% increased chance of critical hits.\n\n
                    case 0x0D30FF94: // You gain mastery in Mace and Crossbow which gives you a damage rating bonus of 5 when using those types of weapons.\n\nYou start with the 'Iron Skin of the Invincible' augmentation which gives a Damage Resistance Rating of 3.\n\n
                    case 0x0D166794: // You gain mastery in Axe and Thrown Weapons which gives you a damage rating bonus of 5 when using those types of weapons.\n\nYou start with the 'Critical Protection' augmentation which reduces the chance that attacks score a critical hit against you.\n\n
                    case 0x01163794: // You gain mastery in Sword and Magic which gives you a damage rating bonus of 5 when using those types of weapons.\n\nYou start with the 'Infused Life Magic' augmentation and no longer need foci to cast Life Magic spells.\n\n
                    case 0x0330CE04: // You gain mastery in Spear and Thrown Weapons which gives you a damage rating bonus of 5 when using those types of weapons.\n\nYou start with the 'Hand of the Remorseless' augmentation which gives a Critical Damage Rating of 3.\n\n
                    case 0x0E162C44: // You gain mastery in Axe and Thrown Weapons which gives you a damage rating bonus of 5 when using those types of weapons.\n\nYou start with the 'Might of the Seventh Mule' augmentation which increases your burden limit.\n\n
                    case 0x032337A4: // Olthoi Soldiers fight with natural claws and pincers.\n\n
                    case 0x006E3E64: // Olthoi Spitters fight by projecting acidic spit from natural glands in their bodies.\n\n
                        stringTableEntry.Strings[0] = "None\n\nTHIS HERITAGE IS NOT AVAILABLE ON THIS SERVER.\n";
                        break;
                    case 0x0041BFF4: // Viamontian
                    case 0x0A5504D7: // Umbraen
                    case 0x0383269E: // Penumbraen
                    case 0x02B9ED04: // Gear Knight
                    case 0x0A8FD2C4: // Undead
                    case 0x0F1936FE: // Empyrean
                    case 0x021E8A1B: // Aun Tumerok
                    case 0x0EF0A6CE: // Lugian
                    case 0x0EA8D8E9: // Olthoi Soldier
                    case 0x08E3D124: // Olthoi Spitter
                        stringTableEntry.Strings[0] = "";
                        break;
                    case 0x0C618F12: // Bow Hunter
                        break;
                    case 0x05DB3724: // BOW HUNTERS are specialists in the use of bows. Few characters can shoot arrows with such lethal precision as a highly trained hunter.\n\nThe Bow Hunter profession contains the Arcane Lore, Finesse Weapons, Item Enchantment, Melee Defense, Missile Weapons, and Shield skills.
                        stringTableEntry.Strings[0] = "BOW HUNTERS are specialists in the use of bows. Few characters can shoot arrows with such lethal precision as a highly trained hunter.\n\nThe Bow Hunter profession contains the Arcane Lore, Bow, Item Enchantment and Melee Defense skills";
                        break;
                    case 0x00264E72: // Life Caster
                        break;
                    case 0x04B31214: // LIFE CASTERS are experts in all matters of vital essence.They can both give life and take it away.\n\nThe Life Caster profession contains the Arcane Lore, Life Magic, Creature Enchantment, Mana Conversion and War Magic skills.
                        stringTableEntry.Strings[0] = "LIFE CASTERS are experts in all matters of vital essence.They can both give life and take it away.\n\nThe Life Caster profession contains the Creature Enchantment, Life Magic, Mana Conversion and War Magic skills.";
                        break;
                    case 0x0132AC75: // War Mage
                        break;
                    case 0x01303754: // WAR MAGES are characters skilled in the arts of battle magics.Few can match their destructive power.\n\nThe War Mage profession contains the War Magic, Mana Conversion and Life Magic skills.
                        stringTableEntry.Strings[0] = "WAR MAGES are characters skilled in the arts of battle magics.Few can match their destructive power.\n\nThe War Mage profession contains the Arcane Lore, Healing, Mana Conversion and War Magic skills.";
                        break;
                    case 0x038532D2: // Wayfarer
                        break;
                    case 0x01393754: // WAYFARERS are versatile adventurers, fighting their way through life with stealth and skill.\n\nThe Wayfarer gets the Dirty Fighting, Dual Wield, Finesse Weapons, Healing, Item Enchantment, Lockpick, Melee Defense, Healing and Missile Weapons skills.
                        stringTableEntry.Strings[0] = "WAYFARERS are versatile adventurers, fighting their way through life with stealth and skill.\n\nThe Wayfarer gets the Arcane Lore, Dagger, Crossbow, Healing, Item Enchantment, Lockpick and Melee Defense skills.";
                        break;
                    case 0x06F53462: // Soldier
                        break;
                    case 0x046159C4: // SOLDIERS are the essential shield fighter, hewing hordes of enemies with a large weapon.\n\nThe Soldier profession contains the Dirty Fighting, Heavy Weapons, Melee Defense, Shield, Healing and Missile Weapons skills.
                        stringTableEntry.Strings[0] = "SOLDIERS are the essential shield fighter, hewing hordes of enemies with a large weapon.\n\nThe Soldier profession contains the Arcane Lore, Axe, Crossbow, Healing and Melee Defense skills.";
                        break;
                    case 0x03E0F292: // Swashbuckler
                        break;
                    case 0x0F060A94: // SWASHBUCKLERS are specialists in the art of swordfighting.  Few compare to their daring or their skill.\n\nThe Swashbuckler profession contains the Arcane Lore, Dual Wield, Heavy Weapons, Melee Defense, Healing and Item Enchantment skills.
                        stringTableEntry.Strings[0] = "SWASHBUCKLERS are specialists in the art of swordfighting.  Few compare to their daring or their skill.\n\nThe Swashbuckler profession contains the Arcane Lore, Item Enchantment, Melee Defense and Sword skills.";
                        break;
                    case 0x05D3F822: // Sanamar
                        stringTableEntry.Strings[0] = "Random";
                        break;
                    case 0x08227F04: // Sanamar is the royal town of the Viamontian domain.It was founded by King Varicci II and his followers, who pursued the rebellious Duke of Bellenesse through a portal from their homeland in Ispar.This proud, walled city is built on a seaside bluff on the southernmost of the Halaetan Islands.  From its walls and towers, Viamontians can gaze southeast over the land their King intends to conquer.  Busy as he is with war planning, Varicci himself is rarely seen in the town's ostentatious Royal Hall.  Newcomers who start in Sanamar may wish to speak with one of the Portal Guardians on the road just outside of town.
                        stringTableEntry.Strings[0] = "";
                        break;
                }
            }

            StreamWriter outputFile = new StreamWriter(new FileStream("23000002.bin", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open 23000002.bin");
                return;
            }

            stringTable.Pack(outputFile);

            outputFile.Flush();
            outputFile.Close();
            Console.WriteLine("Done");
        }

        public void ModifyForCustomDM()
        {
            Console.WriteLine("Modifying chargen text to CustomDM...");

            StringTable stringTable = LanguageDat.ReadFromDat<StringTable>(0x23000002);         

            foreach (var stringTableEntry in stringTable.StringTableData)
            {
                switch (stringTableEntry.Id)
                {
                    case 0x0F9C8FC2: // Trained Starting Skills:\n
                        stringTableEntry.Strings[0] = "";
                        break;
                    case 0x07C77F43: // Jump, Loyalty, Magic Defense, Run and Salvaging.\n\n
                        stringTableEntry.Strings[0] = "";
                        break;
                    case 0x004B7EE2: // Bonus Racial Skills:\n
                        stringTableEntry.Strings[0] = "Trained Starting Skills:\n";
                        break;
                    case 0x0206BBD4: // You gain mastery in Dagger and Bow which gives you a damage rating bonus of 5 when using those types of weapons.\n\nYou start with the 'Jack of All Trades' augmentation which raises all of your skills by 5.\n\n
                        stringTableEntry.Strings[0] = "Shield\n";
                        break;
                    case 0x0630F8A4: // You gain mastery in Staff and Magic spells which gives you a damage rating bonus of 5 when using those types of attacks.\n\nYou start with the 'Jack of All Trades' augmentation which raises all of your skills by 5.\n\n
                        stringTableEntry.Strings[0] = "Salvaging\n";
                        break;
                    case 0x06166A24: // You gain mastery in Unarmed weaponry and Bow which gives you a damage rating bonus of 5 when using those types of weapons.\n\nYou start with the 'Jack of All Trades' augmentation which raises all of your skills by 5.\n\n
                        stringTableEntry.Strings[0] = "Assess Person\n";
                        break;
                    case 0x00166F04: // You gain mastery in Sword and Crossbow which gives you a damage rating bonus of 5 when using those types of weapons.\n\nYou start with the 'Jack of All Trades' augmentation which raises all of your skills by 5.\n\n
                    case 0x0335BA44: // You gain mastery in Unarmed weaponry and Crossbow which gives you a damage rating bonus of 5 when using those types of weapons.\n\nYou start with the 'Eye of the Remorseless' augmentation which gives 1% increased chance of critical hits.\n\n
                    case 0x0D30FF94: // You gain mastery in Mace and Crossbow which gives you a damage rating bonus of 5 when using those types of weapons.\n\nYou start with the 'Iron Skin of the Invincible' augmentation which gives a Damage Resistance Rating of 3.\n\n
                    case 0x0D166794: // You gain mastery in Axe and Thrown Weapons which gives you a damage rating bonus of 5 when using those types of weapons.\n\nYou start with the 'Critical Protection' augmentation which reduces the chance that attacks score a critical hit against you.\n\n
                    case 0x01163794: // You gain mastery in Sword and Magic which gives you a damage rating bonus of 5 when using those types of weapons.\n\nYou start with the 'Infused Life Magic' augmentation and no longer need foci to cast Life Magic spells.\n\n
                    case 0x0330CE04: // You gain mastery in Spear and Thrown Weapons which gives you a damage rating bonus of 5 when using those types of weapons.\n\nYou start with the 'Hand of the Remorseless' augmentation which gives a Critical Damage Rating of 3.\n\n
                    case 0x0E162C44: // You gain mastery in Axe and Thrown Weapons which gives you a damage rating bonus of 5 when using those types of weapons.\n\nYou start with the 'Might of the Seventh Mule' augmentation which increases your burden limit.\n\n
                    case 0x032337A4: // Olthoi Soldiers fight with natural claws and pincers.\n\n
                    case 0x006E3E64: // Olthoi Spitters fight by projecting acidic spit from natural glands in their bodies.\n\n
                        stringTableEntry.Strings[0] = "None\n\nTHIS HERITAGE IS NOT AVAILABLE ON THIS SERVER.\n";
                        break;
                    case 0x0041BFF4: // Viamontian
                    case 0x0A5504D7: // Umbraen
                    case 0x0383269E: // Penumbraen
                    case 0x02B9ED04: // Gear Knight
                    case 0x0A8FD2C4: // Undead
                    case 0x0F1936FE: // Empyrean
                    case 0x021E8A1B: // Aun Tumerok
                    case 0x0EF0A6CE: // Lugian
                    case 0x0EA8D8E9: // Olthoi Soldier
                    case 0x08E3D124: // Olthoi Spitter
                        stringTableEntry.Strings[0] = "";
                        break;
                    case 0x0C618F12: // Bow Hunter
                        break;
                    case 0x05DB3724: // BOW HUNTERS are specialists in the use of bows. Few characters can shoot arrows with such lethal precision as a highly trained hunter.\n\nThe Bow Hunter profession contains the Arcane Lore, Finesse Weapons, Item Enchantment, Melee Defense, Missile Weapons, and Shield skills.
                        stringTableEntry.Strings[0] = "BOW HUNTERS are specialists in the use of bows. Few characters can shoot arrows with such lethal precision as a highly trained hunter.";
                        break;
                    case 0x00264E72: // Life Caster
                        break;
                    case 0x04B31214: // LIFE CASTERS are experts in all matters of vital essence.They can both give life and take it away.\n\nThe Life Caster profession contains the Arcane Lore, Life Magic, Creature Enchantment, Mana Conversion and War Magic skills.
                        stringTableEntry.Strings[0] = "LIFE CASTERS are experts in all matters of vital essence.They can both give life and take it away.";
                        break;
                    case 0x0132AC75: // War Mage
                        break;
                    case 0x01303754: // WAR MAGES are characters skilled in the arts of battle magics.Few can match their destructive power.\n\nThe War Mage profession contains the War Magic, Mana Conversion and Life Magic skills.
                        stringTableEntry.Strings[0] = "WAR MAGES are characters skilled in the arts of battle magics.Few can match their destructive power.";
                        break;
                    case 0x038532D2: // Wayfarer
                        break;
                    case 0x01393754: // WAYFARERS are versatile adventurers, fighting their way through life with stealth and skill.\n\nThe Wayfarer gets the Dirty Fighting, Dual Wield, Finesse Weapons, Healing, Item Enchantment, Lockpick, Melee Defense, Healing and Missile Weapons skills.
                        stringTableEntry.Strings[0] = "WAYFARERS are versatile adventurers, fighting their way through life with stealth and skill.";
                        break;
                    case 0x06F53462: // Soldier
                        break;
                    case 0x046159C4: // SOLDIERS are the essential shield fighter, hewing hordes of enemies with a large weapon.\n\nThe Soldier profession contains the Dirty Fighting, Heavy Weapons, Melee Defense, Shield, Healing and Missile Weapons skills.
                        stringTableEntry.Strings[0] = "SOLDIERS are the essential shield fighter, hewing hordes of enemies with a large weapon.";
                        break;
                    case 0x03E0F292: // Swashbuckler
                        break;
                    case 0x0F060A94: // SWASHBUCKLERS are specialists in the art of swordfighting.  Few compare to their daring or their skill.\n\nThe Swashbuckler profession contains the Arcane Lore, Dual Wield, Heavy Weapons, Melee Defense, Healing and Item Enchantment skills.
                        stringTableEntry.Strings[0] = "SWASHBUCKLERS are specialists in the art of swordfighting.  Few compare to their daring or their skill.";
                        break;
                    case 0x05D3F822: // Sanamar
                        stringTableEntry.Strings[0] = "Random";
                        break;
                    case 0x08227F04: // Sanamar is the royal town of the Viamontian domain.It was founded by King Varicci II and his followers, who pursued the rebellious Duke of Bellenesse through a portal from their homeland in Ispar.This proud, walled city is built on a seaside bluff on the southernmost of the Halaetan Islands.  From its walls and towers, Viamontians can gaze southeast over the land their King intends to conquer.  Busy as he is with war planning, Varicci himself is rarely seen in the town's ostentatious Royal Hall.  Newcomers who start in Sanamar may wish to speak with one of the Portal Guardians on the road just outside of town.
                        stringTableEntry.Strings[0] = "";
                        break;
                }
            }

            StreamWriter outputFile = new StreamWriter(new FileStream("23000002.bin", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open 23000002.bin");
                return;
            }

            stringTable.Pack(outputFile);

            outputFile.Flush();
            outputFile.Close();

            //UiLayout layout = LanguageDat.ReadFromDat<UiLayout>(0x21000038);
            //UiLayout layout = LanguageDat.ReadFromDat<UiLayout>(0x21000046);

            //outputFile = new StreamWriter(new FileStream("21000046.bin", FileMode.Create, FileAccess.Write));
            //if (outputFile == null)
            //{
            //    Console.WriteLine("Unable to open 21000046.bin");
            //    return;
            //}

            //layout.Pack(outputFile);

            //outputFile.Flush();
            //outputFile.Close();

            Console.WriteLine("Done");
        }

        public void DumpStrings()
        {
            foreach (var file in LanguageDat.AllFiles)
            {
                switch (file.Value.GetFileType(DatDatabaseType.Language))
                {
                    case DatFileType.UiLayout:
                        break;
                    case DatFileType.StringTable:
                        StreamWriter outputFile = new StreamWriter(new FileStream($"./Language/{file.Value.ObjectId.ToString("X8")}.txt", FileMode.Create, FileAccess.Write));
                        StringTable stringTable = LanguageDat.ReadFromDat<StringTable>(file.Value.ObjectId);
                        foreach (var stringTableEntry in stringTable.StringTableData)
                        {
                            outputFile.WriteLine($"--- 0x{stringTableEntry.Id}");
                            foreach (var stringEntry in stringTableEntry.Strings)
                            {
                                outputFile.WriteLine(stringEntry);
                                outputFile.Flush();
                            }
                        }
                        outputFile.Close();
                        break;
                    case DatFileType.StringState:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}