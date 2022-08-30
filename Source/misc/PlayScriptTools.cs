using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Instrumentation;
using ACE.DatLoader;
using ACE.DatLoader.Entity;
using ACE.DatLoader.Entity.AnimationHooks;
using ACE.DatLoader.FileTypes;

namespace Melt
{
    class PlayScriptTools
    {
        public PhysicsScriptTable table = null;
        public PhysicsScript script = null;
        public PhysicsScript script2 = null;

        public PlayScriptTools()
        {

        }

        public void AddSneakScripts()
        {
            if (table == null)
            {
                Console.WriteLine("PhysicsScriptTable is empty");
                return;
            }

            //SneakBegin
            script = new PhysicsScript();
            script.Id = 0x330013A0;

            TransparentHook hook = new TransparentHook();
            hook.HookType = ACE.Entity.Enum.AnimationHookType.Transparent;
            hook.Start = 0.0f;
            hook.End = 0.5f;
            hook.Time = 1.0f;
            hook.Direction = ACE.Entity.Enum.AnimationHookDir.Both;

            PhysicsScriptData data = new PhysicsScriptData();
            data.StartTime = 0;
            data.Hook = hook;
            script.ScriptData.Add(data);

            ScriptAndModData scriptAndMod = new ScriptAndModData();
            scriptAndMod.Mod = 1.0f;
            scriptAndMod.ScriptId = script.Id;

            PhysicsScriptTableData tableData = new PhysicsScriptTableData();
            tableData.Scripts.Add(scriptAndMod);

            table.ScriptTable.Add(0xAE, tableData);

            //SneakEnd
            script2 = new PhysicsScript();
            script2.Id = 0x330013A1;

            hook = new TransparentHook();
            hook.HookType = ACE.Entity.Enum.AnimationHookType.Transparent;
            hook.Start = 0.5f;
            hook.End = 0.0f;
            hook.Time = 1.0f;
            hook.Direction = ACE.Entity.Enum.AnimationHookDir.Both;

            data = new PhysicsScriptData();
            data.StartTime = 0;
            data.Hook = hook;
            script2.ScriptData.Add(data);

            scriptAndMod = new ScriptAndModData();
            scriptAndMod.Mod = 1.0f;
            scriptAndMod.ScriptId = script2.Id;

            tableData = new PhysicsScriptTableData();
            tableData.Scripts.Add(scriptAndMod);

            table.ScriptTable.Add(0xAF, tableData);
        }

        public void LoadScriptFromBin(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename);
                return;
            }

            using (var reader = new BinaryReader(inputFile.BaseStream))
            {
                Console.WriteLine("Loading PhysicsScript from binary...");
                script = new PhysicsScript();
                script.Unpack(reader);

                Console.WriteLine("Done");
                return;
            }
        }

        public void SaveScriptToBin(string outputFilename)
        {
            if (script == null)
            {
                Console.WriteLine("PhysicsScript is empty");
                return;
            }

            StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open {0}", outputFilename);
                return;
            }

            Console.WriteLine("Saving PhysicsScript to binary...");

            script.Pack(outputFile);

            outputFile.Close();
            Console.WriteLine("Done");
        }

        public void SaveScript2ToBin(string outputFilename)
        {
            if (script2 == null)
            {
                Console.WriteLine("PhysicsScript is empty");
                return;
            }

            StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open {0}", outputFilename);
                return;
            }

            Console.WriteLine("Saving PhysicsScript to binary...");

            script2.Pack(outputFile);

            outputFile.Close();
            Console.WriteLine("Done");
        }

        public void LoadTableFromBin(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename);
                return;
            }

            using (var reader = new BinaryReader(inputFile.BaseStream))
            {
                Console.WriteLine("Loading PhysicsScriptTable from binary...");
                table = new PhysicsScriptTable();
                table.Unpack(reader);

                Console.WriteLine("Done");
                return;
            }
        }

        public void SaveTableToBin(string outputFilename)
        {
            if (table == null)
            {
                Console.WriteLine("PhysicsScriptTable is empty");
                return;
            }

            StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open {0}", outputFilename);
                return;
            }

            Console.WriteLine("Saving PhysicsScriptTable to binary...");

            table.Pack(outputFile);

            outputFile.Close();
            Console.WriteLine("Done");
        }
    }
}
