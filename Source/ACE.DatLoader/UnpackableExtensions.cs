using ACE.DatLoader.Entity;
using Melt;
using System.Collections.Generic;
using System.IO;

namespace ACE.DatLoader
{
    static class UnpackableExtensions
    {
        /// <summary>
        /// A SmartArray uses a Compressed UInt32 for the length.
        /// </summary>
        public static void UnpackSmartArray(this List<int> value, BinaryReader reader)
        {
            var totalObjects = reader.ReadCompressedUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var item = reader.ReadInt32();
                value.Add(item);
            }
        }

        /// <summary>
        /// A SmartArray uses a Compressed UInt32 for the length.
        /// </summary>
        public static void UnpackSmartArray(this List<uint> value, BinaryReader reader)
        {
            var totalObjects = reader.ReadCompressedUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var item = reader.ReadUInt32();
                value.Add(item);
            }
        }

        /// <summary>
        /// A SmartArray uses a Compressed UInt32 for the length.
        /// </summary>
        public static void UnpackSmartArray<T>(this List<T> value, BinaryReader reader, bool isToD = true) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadCompressedUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var item = new T();
                item.Unpack(reader, isToD);
                value.Add(item);
            }
        }

        /// <summary>
        /// A SmartArray uses a Compressed UInt32 for the length.
        /// </summary>
        public static void UnpackSmartArray<T>(this Dictionary<ushort, T> value, BinaryReader reader, bool isToD = true) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadCompressedUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadUInt16();

                var item = new T();
                item.Unpack(reader, isToD);
                value.Add(key, item);
            }
        }

        /// <summary>
        /// A SmartArray uses a Compressed UInt32 for the length.
        /// </summary>
        public static void UnpackSmartArray<T>(this Dictionary<int, T> value, BinaryReader reader, bool isToD = true) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadCompressedUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadInt32();

                var item = new T();
                item.Unpack(reader, isToD);
                value.Add(key, item);
            }
        }

        /// <summary>
        /// A SmartArray uses a Compressed UInt32 for the length.
        /// </summary>
        public static void UnpackSmartArray<T>(this Dictionary<uint, T> value, BinaryReader reader, bool isToD = true) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadCompressedUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadUInt32();

                var item = new T();
                item.Unpack(reader, isToD);
                value.Add(key, item);
            }
        }


        /// <summary>
        /// A PackedHashTable uses a UInt16 for length, and a UInt16 for bucket size.
        /// We don't need to worry about the bucket size with C#.
        /// </summary>
        public static void UnpackPackedHashTable(this Dictionary<uint, uint> value, BinaryReader reader)
        {
            var totalObjects = reader.ReadUInt16();
            var bucketSize = reader.ReadUInt16();

            for (int i = 0; i < totalObjects; i++)
                value.Add(reader.ReadUInt32(), reader.ReadUInt32());
        }

        /// <summary>
        /// A PackedHashTable uses a UInt16 for length, and a UInt16 for bucket size.
        /// We don't need to worry about the bucket size with C#.
        /// </summary>
        public static ushort UnpackPackedHashTable<T>(this Dictionary<uint, T> value, BinaryReader reader, bool isToD = true) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadUInt16();
            var bucketSize = reader.ReadUInt16();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadUInt32();

                var item = new T();
                item.Unpack(reader, isToD);
                value.Add(key, item);
            }

            return bucketSize;
        }

        /// <summary>
        /// A PackedHashTable uses a byte for length, and a byte for bucket size.
        /// We don't need to worry about the bucket size with C#.
        /// </summary>
        public static byte UnpackBytePackedHashTable<T>(this Dictionary<uint, T> value, BinaryReader reader, bool isToD = true) where T : IUnpackable, new()
        {
            var bucketSize = reader.ReadByte();
            var totalObjects = reader.ReadByte();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadUInt32();

                var item = new T();
                item.Unpack(reader, isToD);
                value.Add(key, item);
            }

            return bucketSize;
        }

        public static byte UnpackBytePackedHashTable(this Dictionary<uint, UiHashProperty> value, BinaryReader reader, bool isToD = true)
        {
            var bucketSize = reader.ReadByte();
            var totalObjects = reader.ReadByte();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadUInt32();

                var item = new UiHashProperty();
                item.Unpack(reader, isToD);
                value.Add(key, item);

                if (item.ValueArrayCount != 0)
                    i += (int)item.ValueArrayCount;
            }

            return bucketSize;
        }

        public static void PackBytePackedHashTable<T>(this Dictionary<uint, T> value, byte bucketSize, StreamWriter output) where T : IPackable, new()
        {
            Utils.writeByte(bucketSize, output);
            Utils.writeByte((byte)value.Count, output);

            foreach(var entry in value)
            {
                Utils.writeUInt32(entry.Key, output);

                entry.Value.Pack(output);
            }
        }

        /// <summary>
        /// A PackedHashTable uses a byte for length, and a byte for bucket size.
        /// We don't need to worry about the bucket size with C#.
        /// </summary>
        public static void UnpackBytePackedHashTable<T>(this Dictionary<byte, T> value, BinaryReader reader, bool isToD = true) where T : IUnpackable, new()
        {
            var bucketSize = reader.ReadByte();
            var totalObjects = reader.ReadByte();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadByte();

                var item = new T();
                item.Unpack(reader, isToD);
                value.Add(key, item);
            }
        }

        /// <summary>
        /// A PackedHashTable uses a UInt16 for length, and a UInt16 for bucket size.
        /// We don't need to worry about the bucket size with C#.
        /// </summary>
        public static void UnpackPackedHashTable<T>(this SortedDictionary<uint, T> value, BinaryReader reader, bool isToD = true) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadUInt16();
            var bucketSize = reader.ReadUInt16();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadUInt32();

                var item = new T();
                item.Unpack(reader, isToD);
                value.Add(key, item);
            }
        }

        /// <summary>
        /// A list that uses a Int32 for the length.
        /// </summary>
        public static void Unpack(this List<uint> value, BinaryReader reader)
        {
            var totalObjects = reader.ReadInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var item = reader.ReadUInt32();
                value.Add(item);
            }
        }

        /// <summary>
        /// A list that uses a UInt32 for the length.
        /// </summary>
        public static void Unpack<T>(this List<T> value, BinaryReader reader, bool isToD = true) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var item = new T();
                item.Unpack(reader, isToD);
                value.Add(item);
            }
        }

        public static void Unpack2<T>(this List<T> value, BinaryReader reader, bool isToD = true) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadByte();

            for (int i = 0; i < totalObjects; i++)
            {
                var item = new T();
                item.Unpack(reader, isToD);
                value.Add(item);
            }
        }

        /// <summary>
        /// A list that uses a Byte for the length.
        /// </summary>
        public static void UnpackByte<T>(this List<T> value, BinaryReader reader, bool isToD = true) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadByte();

            for (int i = 0; i < totalObjects; i++)
            {
                var item = new T();
                item.Unpack(reader, isToD);
                value.Add(item);
            }
        }

        public static void Unpack(this List<uint> value, BinaryReader reader, uint fixedQuantity)
        {
            for (int i = 0; i < fixedQuantity; i++)
            {
                var item = reader.ReadUInt32();
                value.Add(item);
            }
        }

        public static void Unpack<T>(this List<T> value, BinaryReader reader, uint fixedQuantity, bool isToD = true) where T : IUnpackable, new()
        {
            for (int i = 0; i < fixedQuantity; i++)
            {
                var item = new T();
                item.Unpack(reader, isToD);
                value.Add(item);
            }
        }

        public static void Unpack<T>(this Dictionary<ushort, T> value, BinaryReader reader, uint fixedQuantity, bool isToD = true) where T : IUnpackable, new()
        {
            for (int i = 0; i < fixedQuantity; i++)
            {
                var key = reader.ReadUInt16();

                var item = new T();
                item.Unpack(reader, isToD);
                value.Add(key, item);
            }
        }

        public static void Unpack<T>(this Dictionary<ushort, T> value, BinaryReader reader, bool isToD = true) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadUInt16();

                var item = new T();
                item.Unpack(reader, isToD);
                value.Add(key, item);
            }
        }

        /// <summary>
        /// A Dictionary that uses a Int32 for the length.
        /// </summary>
        public static void Unpack<T>(this Dictionary<int, T> value, BinaryReader reader, bool isToD = true) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadInt32();

                var item = new T();
                item.Unpack(reader, isToD);
                value.Add(key, item);
            }
        }

        /// <summary>
        /// A Dictionary that uses a Int32 for the length.
        /// </summary>
        public static void Unpack<T>(this Dictionary<uint, T> value, BinaryReader reader, bool isToD = true) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadUInt32();

                var item = new T();
                item.Unpack(reader, isToD);
                value.Add(key, item);
            }
        }

        public static void Unpack<T>(this Dictionary<uint, T> value, BinaryReader reader, uint fixedQuantity, bool isToD = true) where T : IUnpackable, new()
        {
            for (int i = 0; i < fixedQuantity; i++)
            {
                var key = reader.ReadUInt32();

                var item = new T();
                item.Unpack(reader, isToD);
                value.Add(key, item);
            }
        }

        /// <summary>
        /// A Dictionary that uses a Int32 for the length.
        /// </summary>
        public static void Unpack<T>(this Dictionary<uint, Dictionary<uint, T>> value, BinaryReader reader, bool isToD = true) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadUInt32();

                var values = new Dictionary<uint, T>();
                values.Unpack(reader, isToD);

                value.Add(key, values);
            }
        }

        public static void Pack(this List<uint> value, StreamWriter output)
        {
            foreach (var entry in value)
            {
                Utils.writeUInt32(entry, output);
            }
        }

        public static void Pack<T>(this List<T> value, StreamWriter output) where T : IPackable, new()
        {
            Utils.writeUInt32((uint)value.Count, output);

            foreach (var entry in value)
            {
                entry.Pack(output);
            }
        }

        public static void Pack<T>(this Dictionary<ushort, T> value, StreamWriter output) where T : IPackable, new()
        {
            foreach (var entry in value)
            {
                Utils.writeUInt16(entry.Key, output);
                entry.Value.Pack(output);
            }
        }

        public static void Pack<T>(this Dictionary<uint, T> value, StreamWriter output) where T : IPackable, new()
        {
            Utils.writeUInt32((uint)value.Count, output);

            foreach (var entry in value)
            {
                Utils.writeUInt32(entry.Key, output);
                entry.Value.Pack(output);
            }
        }

        public static void PackSmartArray(this List<uint> value, StreamWriter output)
        {
            Utils.writeCompressedUInt32((uint)value.Count, output);

            foreach (var entry in value)
            {
                Utils.writeUInt32(entry, output);
            }
        }

        public static void PackSmartArray<T>(this List<T> value, StreamWriter output) where T : IPackable, new()
        {
            Utils.writeCompressedUInt32((uint)value.Count, output);

            foreach (var entry in value)
            {
                entry.Pack(output);
            }
        }

        public static void PackSmartArray<T>(this Dictionary<ushort, T> value, StreamWriter output) where T : IPackable, new()
        {
            Utils.writeCompressedUInt32((uint)value.Count, output);

            foreach (var entry in value)
            {
                Utils.writeUInt16(entry.Key, output);
                entry.Value.Pack(output);
            }
        }
    }
}
