using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using ACE.DatLoader.FileTypes;
using ACE.Entity.Enum;
using ACE.DatLoader.Entity;

namespace Melt
{
    class SpellsConverter
    {
        static public void toTxt(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename);
                return;
            }
            StreamWriter outputFile = new StreamWriter(new FileStream(".\\0E00000E.txt", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open 0E00000E.txt");
                return;
            }

            Console.WriteLine("Converting spells from binary to txt...");

            byte[] buffer = new byte[1024];

            int fileHeader;
            short spellCount;
            short unknown1;

            fileHeader = Utils.readInt32(inputFile);
            if (fileHeader != 0x0E00000E)
            {
                Console.WriteLine("Invalid header, aborting.");
                return;
            }

            spellCount = Utils.readInt16(inputFile);
            unknown1 = Utils.readInt16(inputFile);

            outputFile.WriteLine("{0}", spellCount);
            outputFile.WriteLine("{0}", unknown1);
            outputFile.Flush();

            for (int entry = 0; entry < spellCount; entry++)
            {
                int spellId;
                string spellName;
                string spellDescription;
                uint hash;
                int schoolId;
                int iconId;
                int familyId;
                int flags;
                int manaCost;
                float unknown2; //min range?
                float unknown3; //extra range per level?
                int difficulty;
                float economy;
                int generation;
                float speed;
                int spellType;
                int unknown4; //Id?
                double duration = 0d; //if(spellType = 1/7/12)
                int unknown5a = 0; //if(type = 1/12) // = 0xc4268000 for buffs/debuffs, = 0 for wars/portals -- related to duration?
                int unknown5b = 0; //if(type = 1/12) // = 0xc4268000 for buffs/debuffs, = 0 for wars/portals -- related to duration?
                int[] component = new int[8];
                int casterEffect;
                int targetEffect;
                int unknown6;
                int unknown7;
                int unknown8;
                int unknown9;
                int sortOrder;
                int targetMask;
                int unknown10; //something for fellowship spells

                spellId = Utils.readInt32(inputFile);

                spellName = Utils.readEncodedString(inputFile);
                spellDescription = Utils.readEncodedString(inputFile);
                hash = Utils.getHash(spellDescription, 0xBEADCF45) + Utils.getHash(spellName, 0x12107680);
                schoolId = Utils.readInt32(inputFile);
                iconId = Utils.readInt32(inputFile);
                familyId = Utils.readInt32(inputFile);
                flags = Utils.readInt32(inputFile);
                manaCost = Utils.readInt32(inputFile);
                unknown2 = Utils.readSingle(inputFile);
                unknown3 = Utils.readSingle(inputFile);
                difficulty = Utils.readInt32(inputFile);
                economy = Utils.readSingle(inputFile);
                generation = Utils.readInt32(inputFile);
                speed = Utils.readSingle(inputFile);
                spellType = Utils.readInt32(inputFile);
                unknown4 = Utils.readInt32(inputFile);

                switch (spellType)
                {
                    case 1:
                        duration = Utils.readDouble(inputFile);
                        unknown5a = Utils.readInt32(inputFile);
                        unknown5b = Utils.readInt32(inputFile);
                        break;
                    case 7:
                        duration = Utils.readDouble(inputFile);
                        break;
                    case 12:
                        duration = Utils.readDouble(inputFile);
                        unknown5a = Utils.readInt32(inputFile);
                        unknown5b = Utils.readInt32(inputFile);
                        break;
                    default:
                        break;
                }

                for (int i = 0; i < 8; i++)
                {
                    int hashedComponent = Utils.readInt32(inputFile);
                    if (hashedComponent != 0)
                        component[i] = (int)(hashedComponent - hash);
                    else
                        component[i] = 0;
                }

                casterEffect = Utils.readInt32(inputFile);
                targetEffect = Utils.readInt32(inputFile);

                unknown6 = Utils.readInt32(inputFile);
                unknown7 = Utils.readInt32(inputFile);
                unknown8 = Utils.readInt32(inputFile);
                unknown9 = Utils.readInt32(inputFile);

                //These 3 fields are only present on post-ToD files.
                //sortOrder = Utils.readInt32(inputFile);
                //targetMask = Utils.readInt32(inputFile);
                //unknown10 = Utils.readInt32(inputFile);
                sortOrder = 0;
                targetMask = 0;
                unknown10 = 0;

                outputFile.WriteLine("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}|{20}|{21}|{22}|{23}|{24}|{25}|{26}|{27}|{28}|{29}|{30}|{31}|{32}|{33}|{34}|{35}",
                    spellId, spellName, spellDescription, schoolId, iconId, familyId, flags,
                    manaCost, unknown2, unknown3, difficulty, economy, generation, speed, spellType,
                    unknown4, duration, unknown5a, unknown5b, component[0], component[1], component[2], component[3],
                    component[4], component[5], component[6], component[7], casterEffect, targetEffect,
                    unknown6, unknown7, unknown8, unknown9, sortOrder, targetMask, unknown10);

                //if (difficulty < 1)
                //    difficulty = 10;
                //if (spellName.Contains("Minor ") && difficulty < 150)
                //    difficulty = 150;
                //if (spellName.Contains("Moderate ") && difficulty < 175)
                //    difficulty = 175;
                //else if (spellName.Contains("Major ") && difficulty < 200)
                //    difficulty = 200;
                //else if (spellName.Contains("Epic ") && difficulty < 250)
                //    difficulty = 250;
                //else if (spellName.Contains("Legendary ") && difficulty < 300)
                //    difficulty = 300;
                //outputFile.WriteLine("map.Add({0}, new sSpellInfo({0},\"{1}\",{2},{3}));", spellId, spellName, manaCost, difficulty);

                //outputFile.WriteLine("map.Add({0}, new sSpellInfo({0},\"{1}\"));", spellId, spellName);

                //if(spellType == 2)
                //{
                //    if(spellDescription == "CREATURE MAGIC ONLY!")
                //        outputFile.WriteLine("creatureOnlySpellList.Add({0}); //{1}", spellId, spellName);
                //    else
                //        outputFile.WriteLine("replaceSpellList.Add({0}); //{1}", spellId, spellName);
                //}

                outputFile.Flush();
            }

            //unknown data
            while (true)
            {
                int bytesRead = inputFile.BaseStream.Read(buffer, 0, 4);
                if (bytesRead != 4)
                    break;
                int unknown11 = BitConverter.ToInt32(buffer, 0);
                outputFile.WriteLine("{0}", unknown11);
                outputFile.Flush();
            }

            inputFile.Close();
            outputFile.Close();
            Console.WriteLine("Done");
        }

        struct SpellInfo
        {
            public int Key;
            public string Name;
            public string Description;
        }

        static public void transferSpellDescriptiuonsFromJsonToTxt(string jsonFileName, string txtFilename)
        {
            StreamReader jsonFile = new StreamReader(new FileStream(jsonFileName, FileMode.Open, FileAccess.Read));
            if (jsonFile == null)
            {
                Console.WriteLine("Unable to open {0}", jsonFileName);
                return;
            }

            StreamReader inputFile = new StreamReader(new FileStream(txtFilename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", txtFilename);
                return;
            }

            StreamWriter outputFile = new StreamWriter(new FileStream(".\\0E00000E.txt", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open 0E00000E.txt");
                return;
            }

            string line;
            bool foundDesc = false;
            bool foundKey = false;
            Dictionary<int, SpellInfo> spellList = new Dictionary<int, SpellInfo>();

            SpellInfo newSpell = new SpellInfo();
            while (!jsonFile.EndOfStream)
            {
                line = jsonFile.ReadLine();

                if (!foundDesc && line.Contains("\"desc\""))
                {
                    foundDesc = true;
                    newSpell.Description = line.Replace("                    \"desc\": \"", "").Replace("\",", "");
                }
                else if (!foundKey && line.Contains("\"key\":") && !line.Contains("                                "))
                {
                    foundKey = true;
                    newSpell.Key = int.Parse(line.Replace("                \"key\": ", "").Replace(",", ""));
                }
                else if (line.Contains("\"name\":"))
                {
                    newSpell.Name = line.Replace("                    \"name\": \"", "").Replace("\",", "");

                    spellList.Add(newSpell.Key, newSpell);

                    foundDesc = false;
                    foundKey = false;
                    newSpell = new SpellInfo();
                }
            }
            jsonFile.Close();

            string[] spell;         
            line = inputFile.ReadLine();
            short spellCount = Convert.ToInt16(line);
            line = inputFile.ReadLine();

            for (int entry = 0; entry < spellCount; entry++)
            {
                line = inputFile.ReadLine();
                spell = line.Split('|');

                int spellId = Convert.ToInt32(spell[0]);
                string spellName = spell[1];
                string spellDescription = spell[2];

                SpellInfo spellInfo;
                if(spellList.TryGetValue(spellId, out spellInfo))
                {
                    if (spellInfo.Name != spellName)
                        Console.WriteLine("Spell with same id but different name found: {0} -> {1}", spellName, newSpell.Name);
                    else if(spellInfo.Description != spellDescription)
                        line = line.Replace(spellDescription, spellInfo.Description);

                    outputFile.WriteLine(line);
                    outputFile.Flush();
                }
                else
                    Console.WriteLine("Spell not found in json file: {0}", newSpell.Name);
            }

            inputFile.Close();
            outputFile.Close();
        }

        static public void toJson(string cache2rawFilename, string client0e00000eFilename)
        {
            StreamReader cache2rawFile = new StreamReader(new FileStream(cache2rawFilename, FileMode.Open, FileAccess.Read));
            if (cache2rawFile == null)
            {
                Console.WriteLine("Unable to open {0}", cache2rawFilename);
                return;
            }
            StreamReader client0e00000eFile = new StreamReader(new FileStream(client0e00000eFilename, FileMode.Open, FileAccess.Read));
            if (client0e00000eFile == null)
            {
                Console.WriteLine("Unable to open {0}", client0e00000eFilename);
                return;
            }
            JsonSerializerSettings settings = new JsonSerializerSettings();
            StreamWriter outputStream = new StreamWriter(new FileStream(".\\spells.json", FileMode.Create, FileAccess.Write));

            Console.WriteLine("Converting spells from binary to json...");

            int fileHeader;
            short spellCount;
            short spellCount2;
            short unknown1;

            fileHeader = Utils.readInt32(client0e00000eFile);
            if (fileHeader != 0x0E00000E)
            {
                Console.WriteLine("Invalid header, aborting.");
                return;
            }

            spellCount = Utils.readInt16(cache2rawFile); spellCount2 = Utils.readInt16(client0e00000eFile);

            //if (spellCount != spellCount2)
            //{
            //    Console.WriteLine($"File mismatch! Different number of spells found: {spellCount} and {spellCount2}");
            //    return;
            //}

            unknown1 = Utils.readInt16(cache2rawFile); Utils.readInt16(client0e00000eFile);

            string tabChar = "    "; //"\t"
            string newLine = "\n";
            string bracket = "{";
            string closeBracket = "}";
            string tab = tabChar;

            outputStream.Write("{");
            outputStream.Write($"{newLine}{tab}\"table\": {bracket}");
            tab += tabChar;
            outputStream.Write($"{newLine}{tab}\"spellBaseHash\": [");
            tab += tabChar;
            outputStream.Write($"{newLine}{tab}{bracket}");
            tab += tabChar;

            for (int entry = 0; entry < spellCount; entry++)
            {
                int _spellId;
                int _spellId2 = 0;
                string _name; string _name_client;
                string _desc;
                //uint _hash;

                uint _school;
                uint _iconID;
                uint _category;
                uint _bitfield;
                int _base_mana;
                float _base_range_constant;
                float _base_range_mod;
                int _power;
                float _spell_economy_mod ;
                uint _formula_version;
                float _component_loss;
                uint _sp_type;

                //EnchantmentSpellEx = 1
                uint _elementalDamageType = 0;
                double _duration = -1.0;
                float _degrade_modifier = 0.0f;
                float _degrade_limit = -666.0f;
                int _spellCategory = 0;
                uint _smod_type = 0;
                uint _smod_key = 0;
                float _smod_val = 0.0f;

                //FellowshipEnchantmentSpellEx = 12
                //+EnchantmentSpellEx variables

                //ProjectileSpellEx = 2
                uint _etype = 0;
                int _baseIntensity = 0;
                int _variance = 0;
                uint _wcid = 0;
                int _numProjectiles = 0;
                int _numProjectilesVariance = 0;
                float _spreadAngle = 0.0f;
                float _verticalAngle = 0.0f;
                float _defaultLaunchAngle = 0.0f;
                int _bNonTracking = 0;
                float[] _createOffset = { 0.0f, 0.0f, 0.0f };
                float[] _padding = { 0.0f, 0.0f, 0.0f };
                float[] _dims = { 0.0f, 0.0f, 0.0f };
                float[] _peturbation = { 0.0f, 0.0f, 0.0f };

                uint _imbuedEffect = 0;
                int _slayerCreatureType = 0;
                float _slayerDamageBonus = 0.0f;
                double _critFreq = 0.0;
                double _critMultiplier = 0.0;
                int _ignoreMagicResist = 0;
                double _elementalModifier = 0.0;

                //ProjectileLifeSpellEx = 10
                //+ProjectileSpellEx variables
                float _drain_percentage = 0.0f;
                float _damage_ratio = 0.0f;

                //ProjectileEnchantmentSpellEx = 15
                //+ProjectileSpellEx variables
                //+EnchantmentSpellEx variables minus _elementalDamageType

                //BoostSpellEx = 3
                int _boost = 0; // e.g. 4-6 would be _boost=4 _boostVariance=2, and -4 to -6 would be _boost=-4 _boostVariance=-2
                int _boostVariance = 0; // boost+boost variance = max change

                //FellowshipBoostSpellEx = 11
                //+BoostSpellEx variables

                //TransferSpellEx = 4
                uint _src = 0;
                uint _dest = 0;
                float _proportion = 0f;
                float _lossPercent = 0f;
                int _sourceLoss = 0;
                int _transferCap = 0;
                int _maxBoostAllowed = 0;
                uint _transferBitfield = 0; // 1 = source self, 2 = source other, 4 = destination self, 8 = destination other

                //PortalLinkSpellEx = 5
                int _index = -1;

                //PortalRecallSpellEx = 6
                //+PortalLinkSpellEx variables

                //PortalSendingSpellEx = 8
                sPosition _pos = new sPosition();

                //FellowshipPortalSendingSpellEx = 13
                //+PortalSendingSpellEx variables

                //DispelSpellEx = 9
                int _min_power = 0;
                int _max_power = 0;
                float _power_variance = 0.0f;
                int _align = 0;
                int _dispelSchool = 0;
                int _number = 0;
                float _number_variance = 0.0f;

                //FellowshipDispelSpellEx = 14
                //+DispelSpellEx variables

                //PortalSummonSpellEx = 7
                double _portal_lifetime = 0.0;
                int _link = 0;

                //all
                int[] _component = new int[8];
                int _caster_effect;
                int _target_effect; int _target_effect_client;
                int _fizzle_effect;
                double _recovery_interval;
                float _recovery_amount;
                int _display_order;
                uint _non_component_target_type;
                int _mana_mod;

                ///starting at _spellId 3679 the server files contains placeholder data for ToD spells
                _spellId = Utils.readInt32(cache2rawFile); Utils.readInt32(client0e00000eFile);
                _spellId2 = _spellId;
                _name = Utils.readString(cache2rawFile); _name_client = Utils.readEncodedString(client0e00000eFile);
                _desc = Utils.readString(cache2rawFile); Utils.readEncodedString(client0e00000eFile);
                //_hash = Utils.GetHash(_desc, 0xBEADCF45) + Utils.GetHash(_name, 0x12107680);
                _school = Utils.readUInt32(cache2rawFile); Utils.readUInt32(client0e00000eFile);
                _iconID = Utils.readUInt32(cache2rawFile); Utils.readUInt32(client0e00000eFile);
                _category = Utils.readUInt32(cache2rawFile); Utils.readUInt32(client0e00000eFile);
                _bitfield = Utils.readUInt32(cache2rawFile); Utils.readUInt32(client0e00000eFile);
                _base_mana = Utils.readInt32(cache2rawFile); Utils.readInt32(client0e00000eFile);
                _base_range_constant = Utils.readSingle(cache2rawFile); Utils.readSingle(client0e00000eFile);
                _base_range_mod = Utils.readSingle(cache2rawFile); Utils.readSingle(client0e00000eFile);
                _power = Utils.readInt32(cache2rawFile); Utils.readInt32(client0e00000eFile);
                _spell_economy_mod = Utils.readSingle(cache2rawFile); Utils.readSingle(client0e00000eFile);
                _formula_version = Utils.readUInt32(cache2rawFile); Utils.readUInt32(client0e00000eFile);
                _component_loss = Utils.readSingle(cache2rawFile); Utils.readSingle(client0e00000eFile);
                _sp_type = Utils.readUInt32(cache2rawFile); Utils.readUInt32(client0e00000eFile);

                switch (_sp_type)
                {
                    case 1: //Enchantment_SpellType
                    case 12: //FellowEnchantment_SpellType
                        _elementalDamageType = Utils.readUInt32(cache2rawFile); Utils.readUInt32(client0e00000eFile);
                        _duration = Utils.readDouble(cache2rawFile); Utils.readDouble(client0e00000eFile);
                        _degrade_modifier = Utils.readSingle(cache2rawFile); Utils.readSingle(client0e00000eFile);
                        _degrade_limit = Utils.readSingle(cache2rawFile); Utils.readSingle(client0e00000eFile);
                        _spellCategory = Utils.readInt32(cache2rawFile);
                        _smod_type = Utils.readUInt32(cache2rawFile);
                        _smod_key = Utils.readUInt32(cache2rawFile);
                        _smod_val = Utils.readSingle(cache2rawFile);
                        break;
                    case 2: //Projectile_SpellType
                        _spellId2 = Utils.readInt32(cache2rawFile); Utils.readInt32(client0e00000eFile);
                        _etype = Utils.readUInt32(cache2rawFile);
                        _baseIntensity = Utils.readInt32(cache2rawFile);
                        _variance = Utils.readInt32(cache2rawFile);
                        _wcid = Utils.readUInt32(cache2rawFile);
                        _numProjectiles = Utils.readInt32(cache2rawFile);
                        _numProjectilesVariance = Utils.readInt32(cache2rawFile);
                        _spreadAngle = Utils.readSingle(cache2rawFile);
                        _verticalAngle = Utils.readSingle(cache2rawFile);
                        _defaultLaunchAngle = Utils.readSingle(cache2rawFile);
                        _bNonTracking = Utils.readInt32(cache2rawFile);
                        _createOffset[0] = Utils.readSingle(cache2rawFile);
                        _createOffset[1] = Utils.readSingle(cache2rawFile);
                        _createOffset[2] = Utils.readSingle(cache2rawFile);
                        _padding[0] = Utils.readSingle(cache2rawFile);
                        _padding[1] = Utils.readSingle(cache2rawFile);
                        _padding[2] = Utils.readSingle(cache2rawFile);
                        _dims[0] = Utils.readSingle(cache2rawFile);
                        _dims[1] = Utils.readSingle(cache2rawFile);
                        _dims[2] = Utils.readSingle(cache2rawFile);
                        _peturbation[0] = Utils.readSingle(cache2rawFile);
                        _peturbation[1] = Utils.readSingle(cache2rawFile);
                        _peturbation[2] = Utils.readSingle(cache2rawFile);
                        _imbuedEffect = Utils.readUInt32(cache2rawFile);
                        _slayerCreatureType = Utils.readInt32(cache2rawFile);
                        _slayerDamageBonus = Utils.readSingle(cache2rawFile);
                        _critFreq = Utils.readDouble(cache2rawFile);
                        _critMultiplier = Utils.readDouble(cache2rawFile);
                        _ignoreMagicResist = Utils.readInt32(cache2rawFile);
                        _elementalModifier = Utils.readDouble(cache2rawFile);
                        break;
                    case 3: //Boost_SpellType
                    case 11: //FellowBoost_SpellType
                        _spellId2 = Utils.readInt32(cache2rawFile); Utils.readInt32(client0e00000eFile);
                        _elementalDamageType = Utils.readUInt32(cache2rawFile);
                        _boost = Utils.readInt32(cache2rawFile);
                        _boostVariance = Utils.readInt32(cache2rawFile);
                        break;
                    case 4: //Transfer_SpellType
                        _spellId2 = Utils.readInt32(cache2rawFile); Utils.readInt32(client0e00000eFile);
                        _src = Utils.readUInt32(cache2rawFile);
                        _dest = Utils.readUInt32(cache2rawFile);
                        _proportion = Utils.readSingle(cache2rawFile);
                        _lossPercent = Utils.readSingle(cache2rawFile);
                        _sourceLoss = Utils.readInt32(cache2rawFile);
                        _transferCap = Utils.readInt32(cache2rawFile);
                        _maxBoostAllowed = Utils.readInt32(cache2rawFile);
                        _transferBitfield = Utils.readUInt32(cache2rawFile);
                        break;
                    case 5: //PortalLink_SpellType
                    case 6: //PortalRecall_SpellType
                        _spellId2 = Utils.readInt32(cache2rawFile); Utils.readInt32(client0e00000eFile);
                        _index = Utils.readInt32(cache2rawFile);
                        break;
                    case 7: //PortalSummon_SpellType
                        _spellId2 = Utils.readInt32(cache2rawFile); Utils.readInt32(client0e00000eFile);
                        _portal_lifetime = Utils.readDouble(cache2rawFile); Utils.readDouble(client0e00000eFile);
                        _link = Utils.readInt32(cache2rawFile);
                        break;
                    case 8: //PortalSending_SpellType
                    case 13: //FellowPortalSending_SpellType
                        _spellId2 = Utils.readInt32(cache2rawFile); Utils.readInt32(client0e00000eFile);
                        _pos = new sPosition(cache2rawFile);
                        break;
                    case 9: //Dispel_SpellType
                    case 14: //FellowDispel_SpellType
                        _spellId2 = Utils.readInt32(cache2rawFile); Utils.readInt32(client0e00000eFile);
                        _min_power = Utils.readInt32(cache2rawFile);
                        _max_power = Utils.readInt32(cache2rawFile);
                        _power_variance = Utils.readSingle(cache2rawFile);
                        _dispelSchool = Utils.readInt32(cache2rawFile);
                        _align = Utils.readInt32(cache2rawFile);
                        _number = Utils.readInt32(cache2rawFile);
                        _number_variance = Utils.readSingle(cache2rawFile);
                        break;
                    case 10: //LifeProjectile_SpellType
                        _spellId2 = Utils.readInt32(cache2rawFile); Utils.readInt32(client0e00000eFile);
                        _etype = Utils.readUInt32(cache2rawFile);
                        _baseIntensity = Utils.readInt32(cache2rawFile);
                        _variance = Utils.readInt32(cache2rawFile);
                        _wcid = Utils.readUInt32(cache2rawFile);
                        _numProjectiles = Utils.readInt32(cache2rawFile);
                        _numProjectilesVariance = Utils.readInt32(cache2rawFile);
                        _spreadAngle = Utils.readSingle(cache2rawFile);
                        _verticalAngle = Utils.readSingle(cache2rawFile);
                        _defaultLaunchAngle = Utils.readSingle(cache2rawFile);
                        _bNonTracking = Utils.readInt32(cache2rawFile);
                        _createOffset[0] = Utils.readSingle(cache2rawFile);
                        _createOffset[1] = Utils.readSingle(cache2rawFile);
                        _createOffset[2] = Utils.readSingle(cache2rawFile);
                        _padding[0] = Utils.readSingle(cache2rawFile);
                        _padding[1] = Utils.readSingle(cache2rawFile);
                        _padding[2] = Utils.readSingle(cache2rawFile);
                        _dims[0] = Utils.readSingle(cache2rawFile);
                        _dims[1] = Utils.readSingle(cache2rawFile);
                        _dims[2] = Utils.readSingle(cache2rawFile);
                        _peturbation[0] = Utils.readSingle(cache2rawFile);
                        _peturbation[1] = Utils.readSingle(cache2rawFile);
                        _peturbation[2] = Utils.readSingle(cache2rawFile);
                        _imbuedEffect = Utils.readUInt32(cache2rawFile);
                        _slayerCreatureType = Utils.readInt32(cache2rawFile);
                        _slayerDamageBonus = Utils.readSingle(cache2rawFile);
                        _critFreq = Utils.readDouble(cache2rawFile);
                        _critMultiplier = Utils.readDouble(cache2rawFile);
                        _ignoreMagicResist = Utils.readInt32(cache2rawFile);
                        _elementalModifier = Utils.readDouble(cache2rawFile);
                        _drain_percentage = Utils.readSingle(cache2rawFile);
                        _damage_ratio = Utils.readSingle(cache2rawFile);
                        break;
                    case 15: //EnchantmentProjectile_SpellType
                        _spellId2 = Utils.readInt32(cache2rawFile); Utils.readInt32(client0e00000eFile);
                        _etype = Utils.readUInt32(cache2rawFile);
                        _baseIntensity = Utils.readInt32(cache2rawFile);
                        _variance = Utils.readInt32(cache2rawFile);
                        _wcid = Utils.readUInt32(cache2rawFile);
                        _numProjectiles = Utils.readInt32(cache2rawFile);
                        _numProjectilesVariance = Utils.readInt32(cache2rawFile);
                        _spreadAngle = Utils.readSingle(cache2rawFile);
                        _verticalAngle = Utils.readSingle(cache2rawFile);
                        _defaultLaunchAngle = Utils.readSingle(cache2rawFile);
                        _bNonTracking = Utils.readInt32(cache2rawFile);
                        _createOffset[0] = Utils.readSingle(cache2rawFile);
                        _createOffset[1] = Utils.readSingle(cache2rawFile);
                        _createOffset[2] = Utils.readSingle(cache2rawFile);
                        _padding[0] = Utils.readSingle(cache2rawFile);
                        _padding[1] = Utils.readSingle(cache2rawFile);
                        _padding[2] = Utils.readSingle(cache2rawFile);
                        _dims[0] = Utils.readSingle(cache2rawFile);
                        _dims[1] = Utils.readSingle(cache2rawFile);
                        _dims[2] = Utils.readSingle(cache2rawFile);
                        _peturbation[0] = Utils.readSingle(cache2rawFile);
                        _peturbation[1] = Utils.readSingle(cache2rawFile);
                        _peturbation[2] = Utils.readSingle(cache2rawFile);
                        _imbuedEffect = Utils.readUInt32(cache2rawFile);
                        _slayerCreatureType = Utils.readInt32(cache2rawFile);
                        _slayerDamageBonus = Utils.readSingle(cache2rawFile);
                        _critFreq = Utils.readDouble(cache2rawFile);
                        _critMultiplier = Utils.readDouble(cache2rawFile);
                        _ignoreMagicResist = Utils.readInt32(cache2rawFile);
                        _elementalModifier = Utils.readDouble(cache2rawFile);
                        _elementalDamageType = Utils.readUInt32(cache2rawFile); Utils.readUInt32(client0e00000eFile);
                        _duration = Utils.readDouble(cache2rawFile); Utils.readDouble(client0e00000eFile);
                        _degrade_modifier = Utils.readSingle(cache2rawFile); Utils.readSingle(client0e00000eFile);
                        _degrade_limit = Utils.readSingle(cache2rawFile); Utils.readSingle(client0e00000eFile);
                        _spellCategory = Utils.readInt32(cache2rawFile);
                        _smod_type = Utils.readUInt32(cache2rawFile);
                        _smod_key = Utils.readUInt32(cache2rawFile);
                        _smod_val = Utils.readSingle(cache2rawFile);
                        break;
                    default:
                        break;
                }

                for (int i = 0; i < 8; i++)
                {
                    _component[i] = Utils.readInt32(cache2rawFile); Utils.readInt32(client0e00000eFile);
                    //int hashedComponent = Utils.readInt32(inputFile);
                    //if (hashedComponent != 0)
                    //    _component[i] = (int)(hashedComponent - _hash);
                    //else
                    //    _component[i] = 0;
                }

                _caster_effect = Utils.readInt32(cache2rawFile); Utils.readInt32(client0e00000eFile);
                _target_effect = Utils.readInt32(cache2rawFile); _target_effect_client = Utils.readInt32(client0e00000eFile);
                _fizzle_effect = Utils.readInt32(cache2rawFile); Utils.readInt32(client0e00000eFile);
                _recovery_interval = Utils.readDouble(cache2rawFile); Utils.readDouble(client0e00000eFile);
                _recovery_amount = Utils.readSingle(cache2rawFile); Utils.readSingle(client0e00000eFile);

                //there's a divergence here that must have been added by the devs at some point
                if (_caster_effect >= 30)
                    _caster_effect += 1;
                if (_target_effect >= 30)
                    _target_effect += 1;
                if (_fizzle_effect >= 30)
                    _fizzle_effect += 1;

                Utils.readInt32(cache2rawFile); _display_order = Utils.readInt32(client0e00000eFile); //get the display order from the client file
                _non_component_target_type = Utils.readUInt32(cache2rawFile); Utils.readUInt32(client0e00000eFile);
                _mana_mod = Utils.readInt32(cache2rawFile); Utils.readInt32(client0e00000eFile);

                Utils.writeJson(outputStream, "key", _spellId, tab, true);
                outputStream.Write($",{newLine}{tab}\"value\": {bracket}");
                tab += tabChar;
                Utils.writeJson(outputStream, "base_mana", _base_mana, tab, true, true);
                Utils.writeJson(outputStream, "base_range_constant", _base_range_constant, tab, false, true, 0, true);
                Utils.writeJson(outputStream, "base_range_mod", _base_range_mod, tab, false, true, 0, true);
                Utils.writeJson(outputStream, "bitfield", _bitfield, tab, false, true);
                Utils.writeJson(outputStream, "caster_effect", _caster_effect, tab, false, true);
                Utils.writeJson(outputStream, "category", _category, tab, false, true);
                Utils.writeJson(outputStream, "component_loss", _component_loss, tab, false, true, 0, true);
                Utils.writeJson(outputStream, "desc", _desc, tab, false, true);
                Utils.writeJson(outputStream, "display_order", _display_order, tab, false, true);
                Utils.writeJson(outputStream, "fizzle_effect", _fizzle_effect, tab, false, true);
                outputStream.Write($",{newLine}{tab}\"formula\": [ {_component[0]}, {_component[1]}, {_component[2]}, {_component[3]}, {_component[4]}, {_component[5]}, {_component[6]}, {_component[7]} ]");
                Utils.writeJson(outputStream, "formula_version", _formula_version, tab, false, true);
                Utils.writeJson(outputStream, "iconID", _iconID, tab, false, true);
                Utils.writeJson(outputStream, "mana_mod", _mana_mod, tab, false, true);
                Utils.writeJson(outputStream, "name", _name, tab, false, true);
                Utils.writeJson(outputStream, "non_component_target_type", _non_component_target_type, tab, false, true);
                Utils.writeJson(outputStream, "power", _power, tab, false, true);
                Utils.writeJson(outputStream, "recovery_amount", _recovery_amount, tab, false, true, 0, true);
                Utils.writeJson(outputStream, "recovery_interval", _recovery_interval, tab, false, true, 0, true);
                Utils.writeJson(outputStream, "school", _school, tab, false, true);
                Utils.writeJson(outputStream, "spell_economy_mod", _spell_economy_mod, tab, false, true,0, true);
                Utils.writeJson(outputStream, "target_effect", _target_effect, tab, false, true);

                outputStream.Write($",{newLine}{tab}\"meta_spell\": {bracket}");
                tab += tabChar;
                Utils.writeJson(outputStream, "sp_type", _sp_type, tab, true, true);
                outputStream.Write($",{newLine}{tab}\"spell\": {bracket}");
                tab += tabChar;
                Utils.writeJson(outputStream, "spell_id", _spellId2, tab, true, true);

                switch (_sp_type)
                {
                    case 1: //Enchantment_SpellType
                    case 12: //FellowEnchantment_SpellType
                        Utils.writeJson(outputStream, "degrade_limit", _degrade_limit, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "degrade_modifier", _degrade_modifier, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "duration", _duration, tab, false, true, 0, true);
                        outputStream.Write($",{newLine}{tab}\"smod\": {bracket}");
                        tab += tabChar;
                        Utils.writeJson(outputStream, "key", _smod_key, tab, true, true);
                        Utils.writeJson(outputStream, "type", _smod_type, tab, false, true);
                        Utils.writeJson(outputStream, "val", _smod_val, tab, false, true, 0, true);
                        tab = tab.Remove(tab.Length - 4);
                        outputStream.Write($"{newLine}{tab}{closeBracket}");
                        Utils.writeJson(outputStream, "spellCategory", _spellCategory, tab, false, true);
                        break;
                    case 2: //Projectile_SpellType
                        Utils.writeJson(outputStream, "etype", _etype, tab, false, true);
                        Utils.writeJson(outputStream, "baseIntensity", _baseIntensity, tab, false, true);
                        Utils.writeJson(outputStream, "variance", _variance, tab, false, true);
                        Utils.writeJson(outputStream, "wcid", _wcid, tab, false, true);
                        Utils.writeJson(outputStream, "numProjectiles", _numProjectiles, tab, false, true);
                        Utils.writeJson(outputStream, "numProjectilesVariance", _numProjectilesVariance, tab, false, true);
                        Utils.writeJson(outputStream, "spreadAngle", _spreadAngle, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "verticalAngle", _verticalAngle, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "defaultLaunchAngle", _defaultLaunchAngle, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "bNonTracking", _bNonTracking, tab, false, true);
                        outputStream.Write($",{newLine}{tab}\"createOffset\": {bracket}");
                        tab += tabChar;
                        Utils.writeJson(outputStream, "x", _createOffset[0], tab, true, true, 0, true);
                        Utils.writeJson(outputStream, "y", _createOffset[1], tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "z", _createOffset[2], tab, false, true, 0, true);
                        tab = tab.Remove(tab.Length - 4);
                        outputStream.Write($"{newLine}{tab}{closeBracket}");
                        outputStream.Write($",{newLine}{tab}\"padding\": {bracket}");
                        tab += tabChar;
                        Utils.writeJson(outputStream, "x", _padding[0], tab, true, true, 0, true);
                        Utils.writeJson(outputStream, "y", _padding[1], tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "z", _padding[2], tab, false, true, 0, true);
                        tab = tab.Remove(tab.Length - 4);
                        outputStream.Write($"{newLine}{tab}{closeBracket}");
                        outputStream.Write($",{newLine}{tab}\"dims\": {bracket}");
                        tab += tabChar;
                        Utils.writeJson(outputStream, "x", _dims[0], tab, true, true, 0, true);
                        Utils.writeJson(outputStream, "y", _dims[1], tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "z", _dims[2], tab, false, true, 0, true);
                        tab = tab.Remove(tab.Length - 4);
                        outputStream.Write($"{newLine}{tab}{closeBracket}");
                        outputStream.Write($",{newLine}{tab}\"peturbation\": {bracket}");
                        tab += tabChar;
                        Utils.writeJson(outputStream, "x", _peturbation[0], tab, true, true, 0, true);
                        Utils.writeJson(outputStream, "y", _peturbation[1], tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "z", _peturbation[2], tab, false, true, 0, true);
                        tab = tab.Remove(tab.Length - 4);
                        outputStream.Write($"{newLine}{tab}{closeBracket}");
                        Utils.writeJson(outputStream, "imbuedEffect", _imbuedEffect, tab, false, true);
                        Utils.writeJson(outputStream, "slayerCreatureType", _slayerCreatureType, tab, false, true);
                        Utils.writeJson(outputStream, "slayerDamageBonus", _slayerDamageBonus, tab, false, true, 0 , true);
                        Utils.writeJson(outputStream, "critFreq", _critFreq, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "critMultiplier", _critMultiplier, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "ignoreMagicResist", _ignoreMagicResist, tab, false, true);
                        Utils.writeJson(outputStream, "elementalModifier", _elementalModifier, tab, false, true, 0, true);
                        break;
                    case 3: //Boost_SpellType
                    case 11: //FellowBoost_SpellType
                        Utils.writeJson(outputStream, "dt", _elementalDamageType, tab, false, true);
                        Utils.writeJson(outputStream, "boost", _boost, tab, false, true);
                        Utils.writeJson(outputStream, "boostVariance", _boostVariance, tab, false, true);
                        break;
                    case 4: //Transfer_SpellType
                        Utils.writeJson(outputStream, "src", _src, tab, false, true);
                        Utils.writeJson(outputStream, "dest", _dest, tab, false, true);
                        Utils.writeJson(outputStream, "proportion", _proportion, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "lossPercent", _lossPercent, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "sourceLoss", _sourceLoss, tab, false, true);
                        Utils.writeJson(outputStream, "transferCap", _transferCap, tab, false, true);
                        Utils.writeJson(outputStream, "maxBoostAllowed", _maxBoostAllowed, tab, false, true);
                        Utils.writeJson(outputStream, "bitfield", _transferBitfield, tab, false, true);
                        break;
                    case 5: //PortalLink_SpellType
                    case 6: //PortalRecall_SpellType
                        Utils.writeJson(outputStream, "index", _index, tab, false, true);
                        break;
                    case 7: //PortalSummon_SpellType
                        Utils.writeJson(outputStream, "portal_lifetime", _portal_lifetime, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "link", _link, tab, false, true);
                        break;
                    case 8: //PortalSending_SpellType
                    case 13: //FellowPortalSending_SpellType
                        outputStream.Write($",{newLine}{tab}\"pos\": {bracket}");
                        tab += tabChar;
                        outputStream.Write($"{newLine}{tab}\"frame\": {bracket}");
                        tab += tabChar;
                        outputStream.Write($"{newLine}{tab}\"origin\": {bracket}");
                        tab += tabChar;
                        Utils.writeJson(outputStream, "x", _pos.frame.origin.x, tab, true, true, 0, true);
                        Utils.writeJson(outputStream, "y", _pos.frame.origin.y, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "z", _pos.frame.origin.z, tab, false, true, 0, true);
                        tab = tab.Remove(tab.Length - 4);
                        outputStream.Write($"{newLine}{tab}{closeBracket}");
                        outputStream.Write($",{newLine}{tab}\"angles\": {bracket}");
                        tab += tabChar;
                        Utils.writeJson(outputStream, "w", (double)_pos.frame.angles.w, tab, true, true, 0, true);
                        Utils.writeJson(outputStream, "x", (double)_pos.frame.angles.x, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "y", (double)_pos.frame.angles.y, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "z", (double)_pos.frame.angles.z, tab, false, true, 0, true);
                        tab = tab.Remove(tab.Length - 4);
                        outputStream.Write($"{newLine}{tab}{closeBracket}");
                        tab = tab.Remove(tab.Length - 4);
                        outputStream.Write($"{newLine}{tab}{closeBracket}");
                        Utils.writeJson(outputStream, "objcell_id", _pos.objcell_id, tab, false, true);
                        tab = tab.Remove(tab.Length - 4);
                        outputStream.Write($"{newLine}{tab}{closeBracket}");
                        break;
                    case 9: //Dispel_SpellType
                    case 14: //FellowDispel_SpellType
                        Utils.writeJson(outputStream, "min_power", _min_power, tab, false, true);
                        Utils.writeJson(outputStream, "max_power", _max_power, tab, false, true);
                        Utils.writeJson(outputStream, "power_variance", _power_variance, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "school", _dispelSchool, tab, false, true);
                        Utils.writeJson(outputStream, "align", _align, tab, false, true);
                        Utils.writeJson(outputStream, "number", _number, tab, false, true);
                        Utils.writeJson(outputStream, "number_variance", _number_variance, tab, false, true, 0, true);
                        break;
                    case 10: //LifeProjectile_SpellType
                        Utils.writeJson(outputStream, "etype", _etype, tab, false, true);
                        Utils.writeJson(outputStream, "baseIntensity", _baseIntensity, tab, false, true);
                        Utils.writeJson(outputStream, "variance", _variance, tab, false, true);
                        Utils.writeJson(outputStream, "wcid", _wcid, tab, false, true);
                        Utils.writeJson(outputStream, "numProjectiles", _numProjectiles, tab, false, true);
                        Utils.writeJson(outputStream, "numProjectilesVariance", _numProjectilesVariance, tab, false, true);
                        Utils.writeJson(outputStream, "spreadAngle", _spreadAngle, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "verticalAngle", _verticalAngle, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "defaultLaunchAngle", _defaultLaunchAngle, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "bNonTracking", _bNonTracking, tab, false, true);
                        outputStream.Write($",{newLine}{tab}\"createOffset\": {bracket}");
                        tab += tabChar;
                        Utils.writeJson(outputStream, "x", _createOffset[0], tab, true, true, 0, true);
                        Utils.writeJson(outputStream, "y", _createOffset[1], tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "z", _createOffset[2], tab, false, true, 0, true);
                        tab = tab.Remove(tab.Length - 4);
                        outputStream.Write($"{newLine}{tab}{closeBracket}");
                        outputStream.Write($",{newLine}{tab}\"padding\": {bracket}");
                        tab += tabChar;
                        Utils.writeJson(outputStream, "x", _padding[0], tab, true, true, 0, true);
                        Utils.writeJson(outputStream, "y", _padding[1], tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "z", _padding[2], tab, false, true, 0, true);
                        tab = tab.Remove(tab.Length - 4);
                        outputStream.Write($"{newLine}{tab}{closeBracket}");
                        outputStream.Write($",{newLine}{tab}\"dims\": {bracket}");
                        tab += tabChar;
                        Utils.writeJson(outputStream, "x", _dims[0], tab, true, true, 0, true);
                        Utils.writeJson(outputStream, "y", _dims[1], tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "z", _dims[2], tab, false, true, 0, true);
                        tab = tab.Remove(tab.Length - 4);
                        outputStream.Write($"{newLine}{tab}{closeBracket}");
                        outputStream.Write($",{newLine}{tab}\"peturbation\": {bracket}");
                        tab += tabChar;
                        Utils.writeJson(outputStream, "x", _peturbation[0], tab, true, true, 0, true);
                        Utils.writeJson(outputStream, "y", _peturbation[1], tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "z", _peturbation[2], tab, false, true, 0, true);
                        tab = tab.Remove(tab.Length - 4);
                        outputStream.Write($"{newLine}{tab}{closeBracket}");
                        Utils.writeJson(outputStream, "imbuedEffect", _imbuedEffect, tab, false, true);
                        Utils.writeJson(outputStream, "slayerCreatureType", _slayerCreatureType, tab, false, true);
                        Utils.writeJson(outputStream, "slayerDamageBonus", _slayerDamageBonus, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "critFreq", _critFreq, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "critMultiplier", _critMultiplier, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "ignoreMagicResist", _ignoreMagicResist, tab, false, true);
                        Utils.writeJson(outputStream, "elementalModifier", _elementalModifier, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "drain_percentage", _drain_percentage, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "damage_ratio", _damage_ratio, tab, false, true, 0, true);
                        break;
                    case 15: //EnchantmentProjectile_SpellType
                        Utils.writeJson(outputStream, "degrade_limit", _degrade_limit, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "degrade_modifier", _degrade_modifier, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "duration", _duration, tab, false, true, 0, true);
                        outputStream.Write($",{newLine}{tab}\"smod\": {bracket}");
                        tab += tabChar;
                        Utils.writeJson(outputStream, "key", _smod_key, tab, true, true);
                        Utils.writeJson(outputStream, "type", _smod_type, tab, false, true);
                        Utils.writeJson(outputStream, "val", _smod_val, tab, false, true, 0, true);
                        tab = tab.Remove(tab.Length - 4);
                        outputStream.Write($"{newLine}{tab}{closeBracket}");
                        Utils.writeJson(outputStream, "spellCategory", _spellCategory, tab, false, true);
                        Utils.writeJson(outputStream, "etype", _etype, tab, true, true);
                        Utils.writeJson(outputStream, "baseIntensity", _baseIntensity, tab, true, true);
                        Utils.writeJson(outputStream, "variance", _variance, tab, true, true);
                        Utils.writeJson(outputStream, "wcid", _wcid, tab, true, true);
                        Utils.writeJson(outputStream, "numProjectiles", _numProjectiles, tab, true, true);
                        Utils.writeJson(outputStream, "numProjectilesVariance", _numProjectilesVariance, tab, true, true, 0);
                        Utils.writeJson(outputStream, "spreadAngle", _spreadAngle, tab, true, true, 0, true);
                        Utils.writeJson(outputStream, "verticalAngle", _verticalAngle, tab, true, true, 0, true);
                        Utils.writeJson(outputStream, "defaultLaunchAngle", _defaultLaunchAngle, tab, true, true, 0, true);
                        Utils.writeJson(outputStream, "bNonTracking", _bNonTracking, tab, true, true);
                        outputStream.Write($",{newLine}{tab}\"createOffset\": {bracket}");
                        tab += tabChar;
                        Utils.writeJson(outputStream, "x", _createOffset[0], tab, true, true, 0, true);
                        Utils.writeJson(outputStream, "y", _createOffset[1], tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "z", _createOffset[2], tab, false, true, 0, true);
                        tab = tab.Remove(tab.Length - 4);
                        outputStream.Write($"{newLine}{tab}{closeBracket}");
                        outputStream.Write($",{newLine}{tab}\"padding\": {bracket}");
                        tab += tabChar;
                        Utils.writeJson(outputStream, "x", _createOffset[0], tab, true, true, 0, true);
                        Utils.writeJson(outputStream, "y", _createOffset[1], tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "z", _createOffset[2], tab, false, true, 0, true);
                        tab = tab.Remove(tab.Length - 4);
                        outputStream.Write($"{newLine}{tab}{closeBracket}");
                        outputStream.Write($",{newLine}{tab}\"dims\": {bracket}");
                        tab += tabChar;
                        Utils.writeJson(outputStream, "x", _createOffset[0], tab, true, true, 0, true);
                        Utils.writeJson(outputStream, "y", _createOffset[1], tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "z", _createOffset[2], tab, false, true, 0, true);
                        tab = tab.Remove(tab.Length - 4);
                        outputStream.Write($"{newLine}{tab}{closeBracket}");
                        outputStream.Write($",{newLine}{tab}\"peturbation\": {bracket}");
                        tab += tabChar;
                        Utils.writeJson(outputStream, "x", _createOffset[0], tab, true, true, 0, true);
                        Utils.writeJson(outputStream, "y", _createOffset[1], tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "z", _createOffset[2], tab, false, true, 0, true);
                        tab = tab.Remove(tab.Length - 4);
                        outputStream.Write($"{newLine}{tab}{closeBracket}");
                        Utils.writeJson(outputStream, "imbuedEffect", _imbuedEffect, tab, false, true);
                        Utils.writeJson(outputStream, "slayerCreatureType", _slayerCreatureType, tab, false, true);
                        Utils.writeJson(outputStream, "slayerDamageBonus", _slayerDamageBonus, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "critFreq", _critFreq, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "critMultiplier", _critMultiplier, tab, false, true, 0, true);
                        Utils.writeJson(outputStream, "ignoreMagicResist", _ignoreMagicResist, tab, false, true);
                        Utils.writeJson(outputStream, "elementalModifier", _elementalModifier, tab, false, true, 0, true);
                        break;
                    default:
                        break;
                }
                tab = tab.Remove(tab.Length - 4);
                outputStream.Write($"{newLine}{tab}{closeBracket}");
                tab = tab.Remove(tab.Length - 4);
                outputStream.Write($"{newLine}{tab}{closeBracket}");
                tab = tab.Remove(tab.Length - 4);
                outputStream.Write($"{newLine}{tab}{closeBracket}");

                Utils.writeJson(outputStream, "lastModified", "2020-12-10T08:30:00.0000000-03:00", tab, false, true);
                Utils.writeJson(outputStream, "modifiedBy", "Dekaru", tab, false, true);
                outputStream.Write($",{newLine}{tab}\"changelog\": []");
                outputStream.Write($",{newLine}{tab}\"isDone\": false");
                tab = tab.Remove(tab.Length - 4);
                outputStream.Write($"{newLine}{tab}{closeBracket}");
                if (entry < spellCount - 1)
                {
                    outputStream.Write($",\n{tab}{bracket}");
                    tab += tabChar;
                }
                else
                {
                    tab = tab.Remove(tab.Length - 4);
                    outputStream.Write($"{newLine}{tab}]");
                    tab = tab.Remove(tab.Length - 4);
                    outputStream.Write($"{newLine}{tab}{closeBracket}");
                    tab = tab.Remove(tab.Length - 4);
                    outputStream.Write($"{newLine}{tab}{closeBracket}");
                }

                outputStream.Flush();
            }

            cache2rawFile.Close();
            outputStream.Close();
            Console.WriteLine("Done");
        }

        public static void convertStringToByteArrayNoEncode(string text, ref byte[] byteArray, int startIndex, int length)
        {
            for (int i = startIndex; i < startIndex + length; i++)
            {
                byte nextByte = (byte)text[i];
                byteArray[i] = nextByte;
            }

            byte fillerByte = 0x00;
            for (int i = startIndex + length; i < startIndex + length + 4; i++)
            {

                byteArray[i] = fillerByte;
            }
        }

        public static void convertStringToByteArray(string text, ref byte[] byteArray, int startIndex, int length)
        {
            for (int i = startIndex; i < startIndex + length; i++)
            {
                byte nextByte = (byte)((text[i] << 4) ^ (text[i] >> 4));
                byteArray[i] = nextByte;
            }

            byte fillerByte = (byte)((0 << 4) ^ (0 >> 4));
            for (int i = startIndex + length; i < startIndex + length + 4; i++)
            {

                byteArray[i] = fillerByte;
            }
        }

        static public void MergeWeaponsSkillsForCustomDM(string inputFilename, string outputFilename = "./0E00000E - CustomDM.txt")
        {
            StreamReader inputFile = new StreamReader(new FileStream(inputFilename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", inputFilename);
                return;
            }
            StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open {0}", outputFilename);
                return;
            }

            Console.WriteLine("Merging weapon skills spells...");

            string line;
            string[] spell;
            byte[] buffer = new byte[1024];
            char[] textArray = new char[1024];

            short spellCount;
            short unknown1;

            line = inputFile.ReadLine();
            spellCount = Convert.ToInt16(line);
            outputFile.WriteLine(line);

            line = inputFile.ReadLine();
            unknown1 = Convert.ToInt16(line);
            outputFile.WriteLine(line);

            outputFile.Flush();

            for (int entry = 0; entry < spellCount; entry++)
            {
                line = inputFile.ReadLine();
                spell = line.Split('|');

                int spellId = Convert.ToInt32(spell[0]);
                string spellName = spell[1];
                string spellDescription = spell[2];
                int schoolId = Convert.ToInt32(spell[3]);
                int iconId = Convert.ToInt32(spell[4]);
                int familyId = Convert.ToInt32(spell[5]);
                int flags = Convert.ToInt32(spell[6]);
                int manaCost = Convert.ToInt32(spell[7]);
                float unknown2 = Convert.ToSingle(spell[8]);
                float unknown3 = Convert.ToSingle(spell[9]);
                int difficulty = Convert.ToInt32(spell[10]);
                float economy = Convert.ToSingle(spell[11]);
                int generation = Convert.ToInt32(spell[12]);
                float speed = Convert.ToSingle(spell[13]);
                int spellType = Convert.ToInt32(spell[14]);
                int unknown4 = Convert.ToInt32(spell[15]);

                double duration = 0d; //if(spellType = 1/7/12)
                int unknown5a = 0; //if(type = 1/12) // = 0xc4268000 for buffs/debuffs, = 0 for wars/portals -- related to duration?
                int unknown5b = 0; //if(type = 1/12) // = 0xc4268000 for buffs/debuffs, = 0 for wars/portals -- related to duration?
                switch (spellType)
                {
                    case 1:
                        duration = Convert.ToDouble(spell[16]);
                        unknown5a = Convert.ToInt32(spell[17]);
                        unknown5b = Convert.ToInt32(spell[18]);
                        break;
                    case 7:
                        duration = Convert.ToDouble(spell[16]);
                        break;
                    case 12:
                        duration = Convert.ToDouble(spell[16]);
                        unknown5a = Convert.ToInt32(spell[17]);
                        unknown5b = Convert.ToInt32(spell[18]);
                        break;
                    default:
                        break;
                }

                int[] component = new int[8];
                for (int i = 0; i < 8; i++)
                {
                    component[i] = Convert.ToInt32(spell[19 + i]);
                }

                int casterEffect = Convert.ToInt32(spell[27]);
                int targetEffect = Convert.ToInt32(spell[28]);
                int unknown6 = Convert.ToInt32(spell[29]);
                int unknown7 = Convert.ToInt32(spell[30]);
                int unknown8 = Convert.ToInt32(spell[31]);
                int unknown9 = Convert.ToInt32(spell[32]);
                int sortOrder = Convert.ToInt32(spell[33]);
                int targetMask = Convert.ToInt32(spell[34]);
                int unknown10 = Convert.ToInt32(spell[35]);

                switch(familyId)
                {
                    case 17: // Axe mastery
                    case 438: // Axe boon
                        if (!spellName.Contains("Boon"))
                            spellName = spellName.Replace("Axe", "Axe and Mace");
                        spellDescription = spellDescription.Replace("Axe", "Axe and Mace");
                        break;
                    case 18: // Axe ineptude
                        spellName = spellName.Replace("Axe", "Axe and Mace");
                        spellDescription = spellDescription.Replace("Axe", "Axe and Mace");
                        break;
                    case 25: // Mace mastery
                        spellName = spellName.Replace("Mace", "Axe and Mace");
                        spellDescription = spellDescription.Replace("Mace", "Axe and Mace");
                        familyId = 17;
                        break;
                    case 443: // Mace boon
                        if (!spellName.Contains("Boon"))
                            spellName = spellName.Replace("Mace", "Axe and Mace");
                        spellDescription = spellDescription.Replace("Mace", "Axe and Mace");
                        familyId = 438;
                        break;
                    case 26: // Mace ineptude
                        spellName = spellName.Replace("Mace", "Axe and Mace");
                        spellDescription = spellDescription.Replace("Mace", "Axe and Mace");
                        familyId = 18;
                        break;
                    case 27: // Spear mastery
                    case 445: // Spear boon
                        if (!spellName.Contains("Boon"))
                            spellName = spellName.Replace("Spear", "Spear and Staff");
                        spellDescription = spellDescription.Replace("Spear", "Spear and Staff");
                        break;
                    case 28: // Spear ineptude
                        spellName = spellName.Replace("Spear", "Spear and Staff");
                        spellDescription = spellDescription.Replace("Spear", "Spear and Staff");
                        break;
                    case 29: // Staff mastery
                        spellName = spellName.Replace("Staff", "Spear and Staff");
                        spellDescription = spellDescription.Replace("Staff", "Spear and Staff");
                        familyId = 27;
                        break;
                    case 446: // Staff boon
                        if (!spellName.Contains("Boon"))
                            spellName = spellName.Replace("Staff", "Spear and Staff");
                        spellDescription = spellDescription.Replace("Staff", "Spear and Staff");
                        familyId = 445;
                        break;
                    case 30: // Staff ineptude
                        spellName = spellName.Replace("Staff", "Spear and Staff");
                        spellDescription = spellDescription.Replace("Staff", "Spear and Staff");
                        familyId = 28;
                        break;
                    case 19: // Bow mastery
                    case 439: // Bow boon
                        if (!spellName.Contains("Boon"))
                            spellName = spellName.Replace("Bow", "Bow and Crossbow");
                        spellDescription = spellDescription.Replace("Bow", "Bow and Crossbow");
                        break;
                    case 20: // Bow ineptude
                        spellName = spellName.Replace("Bow", "Bow and Crossbow");
                        spellDescription = spellDescription.Replace("Bow", "Bow and Crossbow");
                        break;
                    case 21: // Crossbow mastery
                        if (!spellName.Contains("Boon"))
                            spellName = spellName.Replace("Crossbow", "Bow and Crossbow");
                        spellDescription = spellDescription.Replace("Crossbow", "Bow and Crossbow");
                        familyId = 19;
                        break;
                    case 441: // Crossbow boon
                        if (!spellName.Contains("Boon"))
                            spellName = spellName.Replace("Crossbow", "Bow and Crossbow");
                        spellDescription = spellDescription.Replace("Crossbow", "Bow and Crossbow");
                        familyId = 439;
                        break;
                    case 22: // Crossbow ineptude
                        spellName = spellName.Replace("Crossbow", "Bow and Crossbow");
                        spellDescription = spellDescription.Replace("Crossbow", "Bow and Crossbow");
                        familyId = 20;
                        break;
                }

                outputFile.WriteLine("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}|{20}|{21}|{22}|{23}|{24}|{25}|{26}|{27}|{28}|{29}|{30}|{31}|{32}|{33}|{34}|{35}",
                    spellId, spellName, spellDescription, schoolId, iconId, familyId, flags,
                    manaCost, unknown2, unknown3, difficulty, economy, generation, speed, spellType,
                    unknown4, duration, unknown5a, unknown5b, component[0], component[1], component[2], component[3],
                    component[4], component[5], component[6], component[7], casterEffect, targetEffect,
                    unknown6, unknown7, unknown8, unknown9, sortOrder, targetMask, unknown10);

                outputFile.Flush();
            }

            //unknown data
            while (!inputFile.EndOfStream)
            {
                line = inputFile.ReadLine();
                int unknown11 = Convert.ToInt32(line);
                outputFile.WriteLine(line);
                outputFile.Flush();
            }

            inputFile.Close();
            outputFile.Close();
            Console.WriteLine("Done");
        }

        static public void toBin(string inputFilename, string outputFilename = "./0E00000E.bin")
        {
            StreamReader inputFile = new StreamReader(new FileStream(inputFilename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", inputFilename);
                return;
            }
            StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open {0}", outputFilename);
                return;
            }

            Console.WriteLine("Converting spells from txt to binary...");

            string line;
            string[] spell;
            byte[] buffer = new byte[1024];
            char[] textArray = new char[1024];

            int fileHeader = 0x0E00000E;
            short spellCount;
            short unknown1;

            line = inputFile.ReadLine();
            spellCount = Convert.ToInt16(line);

            line = inputFile.ReadLine();
            unknown1 = Convert.ToInt16(line);

            outputFile.BaseStream.Write(BitConverter.GetBytes(fileHeader), 0, 4);
            outputFile.BaseStream.Write(BitConverter.GetBytes(spellCount), 0, 2);
            outputFile.BaseStream.Write(BitConverter.GetBytes(unknown1), 0, 2);
            outputFile.Flush();

            for (int entry = 0; entry < spellCount; entry++)
            {
                line = inputFile.ReadLine();
                spell = line.Split('|');

                int spellId = Convert.ToInt32(spell[0]);
                string spellName = spell[1];
                string spellDescription = spell[2];
                uint hash;
                int schoolId = Convert.ToInt32(spell[3]);
                int iconId = Convert.ToInt32(spell[4]);
                int familyId = Convert.ToInt32(spell[5]);
                int flags = Convert.ToInt32(spell[6]);
                int manaCost = Convert.ToInt32(spell[7]);
                float unknown2 = Convert.ToSingle(spell[8]);
                float unknown3 = Convert.ToSingle(spell[9]);
                int difficulty = Convert.ToInt32(spell[10]);
                float economy = Convert.ToSingle(spell[11]);
                int generation = Convert.ToInt32(spell[12]);
                float speed = Convert.ToSingle(spell[13]);
                int spellType = Convert.ToInt32(spell[14]);
                int unknown4 = Convert.ToInt32(spell[15]);

                double duration = 0d; //if(spellType = 1/7/12)
                int unknown5a = 0; //if(type = 1/12) // = 0xc4268000 for buffs/debuffs, = 0 for wars/portals -- related to duration?
                int unknown5b = 0; //if(type = 1/12) // = 0xc4268000 for buffs/debuffs, = 0 for wars/portals -- related to duration?
                switch (spellType)
                {
                    case 1:
                        duration = Convert.ToDouble(spell[16]);
                        unknown5a = Convert.ToInt32(spell[17]);
                        unknown5b = Convert.ToInt32(spell[18]);
                        break;
                    case 7:
                        duration = Convert.ToDouble(spell[16]);
                        break;
                    case 12:
                        duration = Convert.ToDouble(spell[16]);
                        unknown5a = Convert.ToInt32(spell[17]);
                        unknown5b = Convert.ToInt32(spell[18]);
                        break;
                    default:
                        break;
                }

                hash = Utils.getHash(spellDescription, 0xBEADCF45) + Utils.getHash(spellName, 0x12107680);

                int[] component = new int[8];
                for (int i = 0; i < 8; i++)
                {
                    int hashedComponent = Convert.ToInt32(spell[19 + i]);
                    if (hashedComponent != 0)
                        component[i] = (int)(hashedComponent + hash);
                    else
                        component[i] = 0;
                }

                int casterEffect = Convert.ToInt32(spell[27]);
                int targetEffect = Convert.ToInt32(spell[28]);
                int unknown6 = Convert.ToInt32(spell[29]);
                int unknown7 = Convert.ToInt32(spell[30]);
                int unknown8 = Convert.ToInt32(spell[31]);
                int unknown9 = Convert.ToInt32(spell[32]);
                int sortOrder = Convert.ToInt32(spell[33]);
                int targetMask = Convert.ToInt32(spell[34]);
                int unknown10 = Convert.ToInt32(spell[35]);

                outputFile.BaseStream.Write(BitConverter.GetBytes(spellId), 0, 4);

                outputFile.BaseStream.Write(BitConverter.GetBytes((short)spellName.Length), 0, 2);
                convertStringToByteArray(spellName, ref buffer, 0, spellName.Length);
                int startIndex = (int)outputFile.BaseStream.Position;
                int endIndex = (int)outputFile.BaseStream.Position + spellName.Length + 2;
                int alignedIndex = Utils.align4(endIndex - startIndex);
                int newIndex = startIndex + alignedIndex;
                int bytesNeededToReachAlignment = newIndex - endIndex;
                outputFile.BaseStream.Write(buffer, 0, spellName.Length + bytesNeededToReachAlignment);

                outputFile.BaseStream.Write(BitConverter.GetBytes((short)spellDescription.Length), 0, 2);
                convertStringToByteArray(spellDescription, ref buffer, 0, spellDescription.Length);
                startIndex = (int)outputFile.BaseStream.Position;
                endIndex = (int)outputFile.BaseStream.Position + spellDescription.Length + 2;
                alignedIndex = Utils.align4(endIndex - startIndex);
                newIndex = startIndex + alignedIndex;
                bytesNeededToReachAlignment = newIndex - endIndex;
                outputFile.BaseStream.Write(buffer, 0, spellDescription.Length + bytesNeededToReachAlignment);

                outputFile.BaseStream.Write(BitConverter.GetBytes(schoolId), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(iconId), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(familyId), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(flags), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(manaCost), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(unknown2), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(unknown3), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(difficulty), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(economy), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(generation), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(speed), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spellType), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(unknown4), 0, 4);

                switch (spellType)
                {
                    case 1:
                        outputFile.BaseStream.Write(BitConverter.GetBytes(duration), 0, 8);
                        outputFile.BaseStream.Write(BitConverter.GetBytes(unknown5a), 0, 4);
                        outputFile.BaseStream.Write(BitConverter.GetBytes(unknown5b), 0, 4);
                        break;
                    case 7:
                        outputFile.BaseStream.Write(BitConverter.GetBytes(duration), 0, 8);
                        break;
                    case 12:
                        outputFile.BaseStream.Write(BitConverter.GetBytes(duration), 0, 8);
                        outputFile.BaseStream.Write(BitConverter.GetBytes(unknown5a), 0, 4);
                        outputFile.BaseStream.Write(BitConverter.GetBytes(unknown5b), 0, 4);
                        break;
                    default:
                        break;
                }

                for (int i = 0; i < 8; i++)
                {
                    outputFile.BaseStream.Write(BitConverter.GetBytes(component[i]), 0, 4);
                }

                outputFile.BaseStream.Write(BitConverter.GetBytes(casterEffect), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(targetEffect), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(unknown6), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(unknown7), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(unknown8), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(unknown9), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(sortOrder), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(targetMask), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(unknown10), 0, 4);

                outputFile.Flush();
            }

            //unknown data
            while (!inputFile.EndOfStream)
            {
                line = inputFile.ReadLine();
                int unknown11 = Convert.ToInt32(line);
                outputFile.BaseStream.Write(BitConverter.GetBytes(unknown11), 0, 4);
                outputFile.Flush();
            }

            inputFile.Close();
            outputFile.Close();
            Console.WriteLine("Done");
        }

        //static public void toJson(string filename)
        //{
        //    StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
        //    if (inputFile == null)
        //    {
        //        Console.WriteLine("Unable to open {0}", filename);
        //        return;
        //    }
        //    //StreamWriter outputFile = new StreamWriter(new FileStream(".\\0E00000E.json", FileMode.Create, FileAccess.Write));
        //    //if (outputFile == null)
        //    //{
        //    //    Console.WriteLine("Unable to open 0E00000E.json");
        //    //    return;
        //    //}

        //    Console.WriteLine("Converting spells from txt to json...");

        //    JsonSerializerSettings settings = new JsonSerializerSettings();
        //    StreamWriter outputStream = new StreamWriter(new FileStream(".\\spells.json", FileMode.Create, FileAccess.Write));

        //    string line;
        //    string[] spell;
        //    byte[] buffer = new byte[1024];
        //    char[] textArray = new char[1024];

        //    short spellCount;
        //    short unknown1;

        //    line = inputFile.ReadLine();
        //    spellCount = Convert.ToInt16(line);

        //    line = inputFile.ReadLine();
        //    unknown1 = Convert.ToInt16(line);

        //    outputStream.Write("{");
        //    outputStream.Write("\n\t\"table\": {");
        //    outputStream.Write("\n\t\t\"spellBaseHash\": [");
        //    outputStream.Write("\n\t\t\t{");

        //    for (int entry = 0; entry < spellCount; entry++)
        //    {
        //        line = inputFile.ReadLine();
        //        spell = line.Split('|');

        //        int spellId = Convert.ToInt32(spell[0]);
        //        string spellName = spell[1];
        //        string spellDescription = spell[2];
        //        uint hash;
        //        int schoolId = Convert.ToInt32(spell[3]);
        //        int iconId = Convert.ToInt32(spell[4]);
        //        int category = Convert.ToInt32(spell[5]);
        //        int bitfield = Convert.ToInt32(spell[6]);
        //        int base_mana = Convert.ToInt32(spell[7]);
        //        float base_range_constant = Convert.ToSingle(spell[8]);
        //        float base_range_mod = Convert.ToSingle(spell[9]);
        //        int power = Convert.ToInt32(spell[10]);
        //        float economyMod = Convert.ToSingle(spell[11]);
        //        int formulaVersion = Convert.ToInt32(spell[12]);
        //        float component_loss = Convert.ToSingle(spell[13]);
        //        int spellType = Convert.ToInt32(spell[14]);
        //        int unknown4 = Convert.ToInt32(spell[15]);

        //        double duration = 0d;
        //        float degrade_mod = 0f;
        //        float degrade_limit = 0f;
        //        int spell_category = 0;

        //        switch (spellType)
        //        {
        //            case 1://Enchantment_SpellType
        //                duration = Convert.ToDouble(spell[16]);
        //                degrade_mod = Convert.ToSingle(spell[17]);
        //                degrade_limit = Convert.ToSingle(spell[18]);
        //                spell_category = Convert.ToInt32(spell[19]);
        //                break;
        //            case 2://Projectile_SpellType
        //                break;
        //            case 3://Boost_SpellType
        //                break;
        //            case 4://Transfer_SpellType
        //                break;
        //            case 5://PortalLink_SpellType
        //                break;
        //            case 6://PortalRecall_SpellType
        //                break;
        //            case 7://PortalSummon_SpellType
        //                duration = Convert.ToDouble(spell[16]);
        //                break;
        //            case 8://PortalSending_SpellType
        //                break;
        //            case 9://Dispel_SpellType
        //                break;
        //            case 10://LifeProjectile_SpellType
        //                break;
        //            case 15://EnchantmentProjectile_SpellType
        //                break;
        //            case 11://FellowBoost_SpellType
        //                break;
        //            case 12://FellowEnchantment_SpellType
        //                duration = Convert.ToDouble(spell[16]);
        //                unknown5a = Convert.ToInt32(spell[17]);
        //                unknown5b = Convert.ToInt32(spell[18]);
        //                break;
        //            case 13://FellowPortalSending_SpellType
        //                break;
        //            case 14://FellowDispel_SpellType
        //                break;
        //            default:
        //                break;
        //        }

        //        hash = Utils.GetHash(spellDescription, 0xBEADCF45) + Utils.GetHash(spellName, 0x12107680);

        //        int[] component = new int[8];
        //        for (int i = 0; i < 8; i++)
        //        {
        //            int hashedComponent = Convert.ToInt32(spell[19 + i]);
        //            if (hashedComponent != 0)
        //                component[i] = (int)(hashedComponent + hash);
        //            else
        //                component[i] = 0;
        //        }

        //        int caster_effect = Convert.ToInt32(spell[27]);
        //        int targetEffect = Convert.ToInt32(spell[28]);
        //        int fizzleEffect = Convert.ToInt32(spell[29]);
        //        double recoveryInterval = Convert.ToDouble(spell[30]);///
        //        float recoveryAmount = Convert.ToSingle(spell[31]);///
        //        int display_order = Convert.ToInt32(spell[33]);
        //        int non_component_target_type = Convert.ToInt32(spell[34]);
        //        int mana_mod = Convert.ToInt32(spell[35]);
        //        //0 | 1               | 2                                         |3|4        |5  |6 |7 |8 |9  |10 |11|12|13  |14|15  |16  |17|18         |19|20|21|22|23|24|25|26|27|27|29|30|31|32|33   |34   |35
        //        //1 | Strength Other I| Increases the target's Strength by 10 poin|4|100668300|1  |6 |10|5 |1  |1  |1 |1 |0   |1 |1   |1800|0 |-1004109824|1 |7 |33|44|49|0 |0 |0 |0 |6 |0 |0 |0 |0 |10412|16   |0
        //        //3 | Weakness Other I| Decreases the target's Strength by 10 pos.|4|100668300|2  |19|10|5 |1  |1  |1 |1 |0.01|1 |3   |60  |0 |-1004109824|1 |8 |33|44|50|0 |0 |0 |0 |7 |0 |0 |0 |0 |11024|16   |0
        //        //47|Primary Portal Tie|Links the caster to a targeted portal.    |3|100672865|200|0 |50|30|0  |150|1 |1 |0.25|5 |47  |0   |0 |0          |3 |73|21|66|32|42|59|0 |16|17|0 |0 |0 |0 |8870 |65536|0
        //        //2760|Martyr's Hecatomb I|text                                   |2|100673244|80 |3 |10|30|0.7|1  |1 |1 |0   |10|2760|0   |0 |0          |1 |8 |26|41|55|0 |0 |0 |32|0 |0 |0 |0 |0 |7346 |16   |0

        //        Utils.writeJson(outputStream, "key", spellId, "\t\t\t", entry == 0);
        //        outputStream.Write(",\n\t\t\t\"value\": {");
        //        Utils.writeJson(outputStream, "base_mana", base_mana, "\t\t\t\t", true, true);
        //        Utils.writeJson(outputStream, "base_range_constant", base_range_constant, "\t\t\t\t", true, true);
        //        Utils.writeJson(outputStream, "base_range_mod", base_range_mod, "\t\t\t\t", true, true);
        //        Utils.writeJson(outputStream, "bitfield", bitfield, "\t\t\t\t", true, true);
        //        Utils.writeJson(outputStream, "caster_effect", caster_effect, "\t\t\t\t", true, true);
        //        Utils.writeJson(outputStream, "category", category, "\t\t\t\t", true, true);
        //        Utils.writeJson(outputStream, "component_loss", component_loss, "\t\t\t\t", true, true);
        //        Utils.writeJson(outputStream, "desc", spellDescription, "\t\t\t\t", true, true);
        //        Utils.writeJson(outputStream, "display_order", display_order, "\t\t\t\t", true, true);
        //        Utils.writeJson(outputStream, "fizzle_effect", display_order, "\t\t\t\t", true, true);

        //        //    "fizzle_effect": 0,
        //        //    "formula": [ 1, 7, 33, 44, 49, 0, 0, 0 ],
        //        //    "formula_version": 1,
        //        //    "iconID": 100668300,
        //        //    "mana_mod": 0,
        //        //    "name": "Strength Other I",
        //        //    "non_component_target_type": 16,
        //        //    "power": 1,
        //        //    "recovery_amount": 0.0,
        //        //    "recovery_interval": 0.0,
        //        //    "school": 4,
        //        //    "spell_economy_mod": 1.0,
        //        //    "target_effect": 6,

        //        //outputFile.BaseStream.Write(buffer, 0, spellDescription.Length + bytesNeededToReachAlignment);

        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(schoolId), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(iconId), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(familyId), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(flags), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(manaCost), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(unknown2), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(unknown3), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(difficulty), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(economy), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(generation), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(speed), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(spellType), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(unknown4), 0, 4);

        //        //switch (spellType)
        //        //{
        //        //    case 1:
        //        //        outputFile.BaseStream.Write(BitConverter.GetBytes(duration), 0, 8);
        //        //        outputFile.BaseStream.Write(BitConverter.GetBytes(unknown5a), 0, 4);
        //        //        outputFile.BaseStream.Write(BitConverter.GetBytes(unknown5b), 0, 4);
        //        //        break;
        //        //    case 7:
        //        //        outputFile.BaseStream.Write(BitConverter.GetBytes(duration), 0, 8);
        //        //        break;
        //        //    case 12:
        //        //        outputFile.BaseStream.Write(BitConverter.GetBytes(duration), 0, 8);
        //        //        outputFile.BaseStream.Write(BitConverter.GetBytes(unknown5a), 0, 4);
        //        //        outputFile.BaseStream.Write(BitConverter.GetBytes(unknown5b), 0, 4);
        //        //        break;
        //        //    default:
        //        //        break;
        //        //}

        //        //for (int i = 0; i < 8; i++)
        //        //{
        //        //    outputFile.BaseStream.Write(BitConverter.GetBytes(component[i]), 0, 4);
        //        //}

        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(casterEffect), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(targetEffect), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(unknown6), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(unknown7), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(unknown8), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(unknown9), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(sortOrder), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(targetMask), 0, 4);
        //        //outputFile.BaseStream.Write(BitConverter.GetBytes(unknown10), 0, 4);

        //        outputStream.Write("\n\t\t\t}");
        //    }

        //    inputFile.Close();

        //    outputStream.Close();
        //    Console.WriteLine("Done");
        //}

        static public void revertWeaponMasteriesAndAuras(string filename1, string originalSpellsFilename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename1, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename1);
                return;
            }
            StreamReader inputFile2 = new StreamReader(new FileStream(originalSpellsFilename, FileMode.Open, FileAccess.Read));
            if (inputFile2 == null)
            {
                Console.WriteLine("Unable to open {0}", originalSpellsFilename);
                return;
            }

            StreamWriter outputFile = new StreamWriter(new FileStream(".\\0E00000E.txt", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open 0E00000E.txt");
                return;
            }

            Console.WriteLine("Reverting weapon masteries spells...");

            string line;
            string line2;
            string[] spell;
            string[] spellOld;

            byte[] buffer = new byte[1024];
            char[] textArray = new char[1024];

            //int fileHeader = 0x0E00000E;
            short spellCount;
            short unknown1;

            short spellCount2;
            short unknown1_2;

            line = inputFile.ReadLine();
            spellCount = Convert.ToInt16(line);
            outputFile.WriteLine(line);

            line = inputFile.ReadLine();
            unknown1 = Convert.ToInt16(line);
            outputFile.WriteLine(line);

            outputFile.Flush();

            line2 = inputFile2.ReadLine();
            spellCount2 = Convert.ToInt16(line2);

            line2 = inputFile2.ReadLine();
            unknown1_2 = Convert.ToInt16(line2);

            for (int entry = 0; entry < spellCount; entry++)
            {
                line = inputFile.ReadLine();
                spell = line.Split('|');

                int spellId = Convert.ToInt32(spell[0]);
                string spellName = spell[1];
                string spellDescription = spell[2];
                int schoolId = Convert.ToInt32(spell[3]);
                int iconId = Convert.ToInt32(spell[4]);
                int familyId = Convert.ToInt32(spell[5]);
                eSpellIndex flags = (eSpellIndex)Convert.ToInt32(spell[6]);
                int baseManaCost = Convert.ToInt32(spell[7]);
                float unknown2 = Convert.ToSingle(spell[8]);
                float unknown3 = Convert.ToSingle(spell[9]);
                int power = Convert.ToInt32(spell[10]);
                float economy = Convert.ToSingle(spell[11]);
                int generation = Convert.ToInt32(spell[12]);
                float speed = Convert.ToSingle(spell[13]);
                eSpellType spellType = (eSpellType)Convert.ToInt32(spell[14]);
                int unknown4 = Convert.ToInt32(spell[15]);

                double duration = 0d; //if(spellType = 1/7/12)
                int unknown5a = 0; //if(type = 1/12) // = 0xc4268000 for buffs/debuffs, = 0 for wars/portals -- related to duration?
                int unknown5b = 0; //if(type = 1/12) // = 0xc4268000 for buffs/debuffs, = 0 for wars/portals -- related to duration?
                switch (spellType)
                {
                    case eSpellType.Enchantment_SpellType:
                        duration = Convert.ToDouble(spell[16]);
                        unknown5a = Convert.ToInt32(spell[17]);
                        unknown5b = Convert.ToInt32(spell[18]);
                        break;
                    case eSpellType.PortalSummon_SpellType:
                        duration = Convert.ToDouble(spell[16]);
                        break;
                    case eSpellType.FellowEnchantment_SpellType:
                        duration = Convert.ToDouble(spell[16]);
                        unknown5a = Convert.ToInt32(spell[17]);
                        unknown5b = Convert.ToInt32(spell[18]);
                        break;
                    default:
                        break;
                }

                int[] component = new int[8];
                for (int i = 0; i < 8; i++)
                {
                    component[i] = Convert.ToInt32(spell[19 + i]);
                }

                int casterEffect = Convert.ToInt32(spell[27]);
                int targetEffect = Convert.ToInt32(spell[28]);
                int fizzleEffect = Convert.ToInt32(spell[29]);
                int unknown7 = Convert.ToInt32(spell[30]);
                int unknown8 = Convert.ToInt32(spell[31]);
                int unknown9 = Convert.ToInt32(spell[32]);
                int sortOrder = Convert.ToInt32(spell[33]);
                eItemType targetMask = (eItemType)Convert.ToInt32(spell[34]);
                int unknown10 = Convert.ToInt32(spell[35]);

                if (entry < spellCount2)
                {
                    line2 = inputFile2.ReadLine();
                    spellOld = line2.Split('|');

                    int spellIdOld = Convert.ToInt32(spellOld[0]);
                    string spellNameOld = spellOld[1];
                    string spellDescriptionOld = spellOld[2];
                    int schoolIdOld = Convert.ToInt32(spellOld[3]);
                    int iconIdOld = Convert.ToInt32(spellOld[4]);
                    int familyIdOld = Convert.ToInt32(spellOld[5]);
                    int casterEffectOld = Convert.ToInt32(spellOld[27]);
                    int targetEffectOld = Convert.ToInt32(spellOld[28]);
                    int fizzleEffectOld = Convert.ToInt32(spellOld[29]);

                    eItemType targetMaskOld = (eItemType)Convert.ToInt32(spellOld[34]);
                    eSpellIndex flagsOld = (eSpellIndex)Convert.ToInt32(spell[6]);

                    //Revert Item Enchantment
                    if (targetMaskOld != targetMask)
                    {
                        flags &= ~(eSpellIndex.SelfTargeted_SpellIndex);

                        for (int i = 0; i < 8; i++)
                        {
                            //60 = rowan talisman(creature enchantment self)
                            //49 = poplar talisman(creature enchantment other)
                            if (component[i] == 60 || component[i] == 49)
                                component[i] = 57; //change rowan and poplar talisman to ashwood talisman
                        }                        

                        spellName = spellNameOld;
                        spellDescription = spellDescriptionOld;
                        iconId = iconIdOld;
                        casterEffect = casterEffectOld;
                        targetEffect = targetEffectOld;
                        fizzleEffect = fizzleEffectOld;
                        targetMask = targetMaskOld;
                    }

                    if (spellName.Contains("Light Weapon") ||
                        spellName.Contains("Finesse Weapon") ||
                        spellName.Contains("Heavy Weapon") ||
                        spellName.Contains("Light Weapon") ||
                        spellName.Contains("Missile Weapon") ||
                        spellName.Contains("Kern's Boon") ||
                        spellName.Contains("Ranger's Boon") ||
                        spellName.Contains("Fencer's Boon") ||
                        spellName.Contains("Soldier's Boon") ||
                        spellName.Contains("Cascade"))
                    {
                        spellName = spellNameOld;
                        spellDescription = spellDescriptionOld;
                        iconId = iconIdOld;
                        familyId = familyIdOld;
                        casterEffect = casterEffectOld;
                        targetEffect = targetEffectOld;
                        fizzleEffect = fizzleEffectOld;
                    }
                }

                outputFile.WriteLine("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}|{20}|{21}|{22}|{23}|{24}|{25}|{26}|{27}|{28}|{29}|{30}|{31}|{32}|{33}|{34}|{35}",
                    spellId, spellName, spellDescription, schoolId, iconId, familyId, (int)flags,
                    baseManaCost, unknown2, unknown3, power, economy, generation, speed, (int)spellType,
                    unknown4, duration, unknown5a, unknown5b, component[0], component[1], component[2], component[3],
                    component[4], component[5], component[6], component[7], casterEffect, targetEffect,
                    fizzleEffect, unknown7, unknown8, unknown9, sortOrder, (int)targetMask, unknown10);
                outputFile.Flush();
            }

            //unknown data
            while (!inputFile.EndOfStream)
            {
                line = inputFile.ReadLine();
                outputFile.WriteLine(line);
                outputFile.Flush();
            }

            inputFile.Close();
            outputFile.Close();
            Console.WriteLine("Done");
        }
    }
}
