using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Melt
{
    public enum eRandomFormula
    {
        favorSpecificValue,
        favorLow,
        favorMid,
        favorHigh,
        equalDistribution,
    }

    class Utils
    {
        static public byte readByte(StreamReader data)
        {
            byte[] buffer = new byte[1];
            data.BaseStream.Read(buffer, 0, 1);
            return buffer[0];
        }

        static public byte[] readBytes(StreamReader data, int amount)
        {
            byte[] buffer = new byte[amount];
            data.BaseStream.Read(buffer, 0, amount);
            return buffer;
        }

        static public Int16 readInt16(StreamReader data)
        {
            byte[] buffer = new byte[2];
            data.BaseStream.Read(buffer, 0, 2);
            return BitConverter.ToInt16(buffer, 0);
        }

        static public Int32 readInt32(StreamReader data)
        {
            byte[] buffer = new byte[4];
            data.BaseStream.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        static public Int64 readInt64(StreamReader data)
        {
            byte[] buffer = new byte[8];
            data.BaseStream.Read(buffer, 0, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        static public bool readBool(StreamReader data)
        {
            byte[] buffer = new byte[1];
            data.BaseStream.Read(buffer, 0, 1);
            return BitConverter.ToBoolean(buffer, 0);
        }

        static public UInt16 readUInt16(StreamReader data)
        {
            byte[] buffer = new byte[2];
            data.BaseStream.Read(buffer, 0, 2);
            return BitConverter.ToUInt16(buffer, 0);
        }

        static public UInt32 readUInt32(StreamReader data)
        {
            byte[] buffer = new byte[4];
            data.BaseStream.Read(buffer, 0, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }

        static public UInt32 readCompressedUInt32(StreamReader data)
        {
            var b0 = readByte(data);
            if ((b0 & 0x80) == 0)
                return b0;

            var b1 = readByte(data);
            if ((b0 & 0x40) == 0)
                return (uint)(((b0 & 0x7F) << 8) | b1);

            var s = readUInt16(data);
            return (uint)(((((b0 & 0x3F) << 8) | b1) << 16) | s);
        }

        static public UInt32 readPackedUInt32(StreamReader data, uint typeSize = 0)
        {
            //uint testValue = 100000;
            //uint packedValue = (testValue << 16) | ((testValue >> 16) | 0x8000);
            //uint unpackedValue = (packedValue >> 16) | ((packedValue ^ 0x8000) << 16);

            ushort value = readUInt16(data);
            if (value >> 12 != 0x08)
            {
                return value + typeSize;
            }
            else
            {
                data.BaseStream.Seek(-2, SeekOrigin.Current);
                uint packedValue = readUInt32(data);
                uint unpackedValue = (packedValue >> 16) | ((packedValue ^ 0x8000) << 16);
                return unpackedValue + typeSize;
            }
        }

        static public UInt64 readUInt64(StreamReader data)
        {
            byte[] buffer = new byte[8];
            data.BaseStream.Read(buffer, 0, 8);
            return BitConverter.ToUInt64(buffer, 0);
        }

        static public float readSingle(StreamReader data)
        {
            byte[] buffer = new byte[4];
            data.BaseStream.Read(buffer, 0, 4);
            return BitConverter.ToSingle(buffer, 0);
        }

        static public double readDouble(StreamReader data)
        {
            byte[] buffer = new byte[8];
            data.BaseStream.Read(buffer, 0, 8);
            return BitConverter.ToDouble(buffer, 0);
        }

        static public void align(StreamWriter data)
        {
            int alignedPosition = (int)(data.BaseStream.Position);
            if (alignedPosition % 4 != 0)
            {
                alignedPosition = alignedPosition + (4 - alignedPosition % 4);

                int difference = alignedPosition - (int)(data.BaseStream.Position);

                for (int i = 0; i < difference; i++)
                {
                    data.BaseStream.WriteByte(0);
                }
            }
        }

        static public void align(StreamReader data)
        {
            int alignedPosition = (int)(data.BaseStream.Position);
            if (alignedPosition % 4 != 0)
            {
                alignedPosition = alignedPosition + (4 - alignedPosition % 4);
                data.BaseStream.Seek(alignedPosition, 0);
            }
        }

        static public int align4(int index)
        {
            return (index + 3) & 0xFFFFFC;
        }

        static public string replaceStringSpecialCharacters(string text)
        {
            text = text.Replace("\\n", "<tempLineBreak>");
            text = text.Replace("\\t", "<tempTabulation>");
            text = text.Replace("\\\"", "<tempQuote>");

            text = text.Replace("\n", "\\n");
            text = text.Replace("\t", "\\t");
            text = text.Replace("\"", "\\\"");

            text = text.Replace("<tempLineBreak>", "\\\\n");
            text = text.Replace("<tempTabulation>", "\\\\t");
            text = text.Replace("<tempQuote>", "\\\"");

            return text;
        }

        static public string restoreStringSpecialCharacters(string text)
        {
            text = text.Replace("\\\\n", "<tempLineBreak>");
            text = text.Replace("\\\\t", "<tempTabulation>");
            text = text.Replace("\\\\\"", "<tempQuote>");

            text = text.Replace("\\n", "\n");
            text = text.Replace("\\t", "\t");
            text = text.Replace("\\\"", "\"");

            text = text.Replace("<tempLineBreak>", "\\n");
            text = text.Replace("<tempTabulation>", "\\t");
            text = text.Replace("<tempQuote>", "\\\"");

            return text;
        }

        static public string readStringAndReplaceSpecialCharacters(StreamReader data)
        {
            int startIndex = (int)data.BaseStream.Position;
            string text = "";
            int letterCount = readInt16(data);

            byte[] buffer = new byte[letterCount];
            data.BaseStream.Read(buffer, 0, letterCount);
            for (int i = 0; i < letterCount; i++)
            {
                text += (char)buffer[i];
            }
            align(data);

            return replaceStringSpecialCharacters(text);
        }

        static public string readSerializedString(StreamReader data)
        {
            int startIndex = (int)data.BaseStream.Position;
            string text = "";
            uint letterCount = readCompressedUInt32(data);

            byte[] buffer = new byte[letterCount];
            data.BaseStream.Read(buffer, 0, (int)letterCount);
            for (int i = 0; i < letterCount; i++)
            {
                text += (char)buffer[i];
            }

            return text;
        }

        static public string readString(StreamReader data)
        {
            int startIndex = (int)data.BaseStream.Position;
            string text = "";
            int letterCount = readInt16(data);

            byte[] buffer = new byte[letterCount];
            data.BaseStream.Read(buffer, 0, letterCount);
            for (int i = 0; i < letterCount; i++)
            {
                text += (char)buffer[i];
            }
            align(data);

            return text;
        }

        static public string readStringNoAlign(StreamReader data)
        {
            int startIndex = (int)data.BaseStream.Position;
            string text = "";
            int letterCount = readInt16(data);

            byte[] buffer = new byte[letterCount];
            data.BaseStream.Read(buffer, 0, letterCount);
            for (int i = 0; i < letterCount; i++)
            {
                text += (char)buffer[i];
            }

            return text;
        }

        static public string readEncodedString(StreamReader data)
        {
            int startIndex = (int)data.BaseStream.Position;
            int letterCount = readInt16(data);

            byte[] buffer = new byte[letterCount];
            byte[] result = new byte[letterCount];
            data.BaseStream.Read(buffer, 0, letterCount);
            for (int i = 0; i < letterCount; i++)
            {
                result[i] = (byte)((buffer[i] >> 4) ^ (buffer[i] << 4));
            }

            align(data);
            return System.Text.Encoding.Default.GetString(result);
        }

        static public uint getHash(string value, uint seed)
        {
            uint r = 0;
            for (int i = 0; i < value.Length; i++)
            {
                int c = value[i];
                r = (uint)((r << 4) + c) & 0xFFFFFFFF;
                int t = (int)(r >> 28);
                if (t != 0)
                {
                    r = (uint)((r & 0xFFFFFFF) ^ (t << 4));
                }
            }
            return (uint)(r % seed);
        }

        public static void convertStringToEncodedByteArray(string text, ref byte[] byteArray, int startIndex, int length)
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

        public static void convertStringToByteArray(string text, ref byte[] byteArray, int startIndex, int length)
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

        static public byte readAndWriteByte(StreamReader data, StreamWriter outputData, bool write = true)
        {
            byte value = readByte(data);
            if (write)
                writeByte(value, outputData);
            return value;
        }

        static public short readAndWriteShort(StreamReader data, StreamWriter outputData, bool write = true)
        {
            short value = readInt16(data);
            if (write)
                writeInt16(value, outputData);
            return value;
        }

        static public Int32 readAndWriteInt32(StreamReader data, StreamWriter outputData, bool write = true)
        {
            Int32 value = readInt32(data);
            if (write)
                writeInt32(value, outputData);
            return value;
        }

        static public Int64 readAndWriteInt64(StreamReader data, StreamWriter outputData, bool write = true)
        {
            Int64 value = readInt64(data);
            if (write)
                writeInt64(value, outputData);
            return value;
        }

        static public UInt32 readAndWriteUInt32(StreamReader data, StreamWriter outputData, bool write = true)
        {
            UInt32 value = readUInt32(data);
            if (write)
                writeUInt32(value, outputData);
            return value;
        }

        static public UInt64 readAndWriteUInt64(StreamReader data, StreamWriter outputData, bool write = true)
        {
            UInt64 value = readUInt64(data);
            if (write)
                writeUInt64(value, outputData);
            return value;
        }

        static public float readAndWriteSingle(StreamReader data, StreamWriter outputData, bool write = true)
        {
            float value = readSingle(data);
            if (write)
                writeSingle(value, outputData);
            return value;
        }

        static public double readAndWriteDouble(StreamReader data, StreamWriter outputData, bool write = true)
        {
            double value = readDouble(data);
            if (write)
                writeDouble(value, outputData);
            return value;
        }


        static public string readAndWriteString(StreamReader data, StreamWriter outputData, bool write = true)
        {
            string value = readString(data);
            if (write)
                writeString(value, outputData);
            return value;
        }

        static public string readAndWriteEncodedString(StreamReader data, StreamWriter outputData, bool write = true)
        {
            string value = readEncodedString(data);
            if (write)
                writeEncodedString(value, outputData);
            return value;
        }

        static public void writeByte(byte value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 1);
        }

        static public void writeBytes(byte[] value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(value, 0, value.Length);
        }

        static public void writeBytes(byte[] value, int offset, int length, StreamWriter outputData)
        {
            outputData.BaseStream.Write(value, offset, length);
        }

        static public void writeInt16(Int16 value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 2);
        }

        static public void writeBool(bool value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 1);
        }

        static public void writeUInt16(UInt16 value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 2);
        }

        static public void writeInt32(Int32 value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 4);
        }

        static public void writeInt64(Int64 value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 8);
        }

        static public void writeUInt32(UInt32 value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 4);
        }

        static public void writeUInt64(UInt64 value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 8);
        }

        static public UInt16 lowWord(UInt32 number)
        {
            return (UInt16)(number & 0x0000FFFF);
        }

        static public UInt16 highWord(UInt32 number)
        {
            return (UInt16)(number & 0xFFFF0000);
        }

        static public void writeCompressedUInt32(UInt32 value, StreamWriter outputData)
        {
            byte[] byteArray = new byte[4];
            writeToByteArray(value, byteArray, 0);

            if (value > 0x7F)
            {
                if (value > 0x3FFF)
                {
                    writeByte((byte)(byteArray[3] | 0xC0), outputData);
                    writeByte(byteArray[2], outputData);
                    writeByte(byteArray[0], outputData);
                    writeByte(byteArray[1], outputData);
                }
                else
                {
                    writeByte((byte)(byteArray[1] | 0x80), outputData);
                    writeByte(byteArray[0], outputData);
                }
            }
            else
                writeByte(byteArray[0], outputData);
        }

        static public void writePackedUInt32(UInt32 value, StreamWriter outputData, UInt32 typeSize = 0)
        {
            value = value - typeSize;

            if (value <= 16383)
            {
                UInt16 shortPackedValue = Convert.ToUInt16(value);
                outputData.BaseStream.Write(BitConverter.GetBytes(shortPackedValue), 0, 2);
            }
            else
            {
                UInt32 packedValue = (value << 16) | ((value >> 16) | 0x8000);
                outputData.BaseStream.Write(BitConverter.GetBytes(packedValue), 0, 4);
            }
        }

        static public void writeSingle(Single value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 4);
        }

        static public void writeDouble(Double value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 8);
        }

        static public void writeSerializedString(string value, StreamWriter outputData)
        {
            writeCompressedUInt32((UInt32)value.Length, outputData);

            if (value.Length > 0)
            {
                byte[] buffer = new byte[value.Length + 4];
                convertStringToByteArray(value, ref buffer, 0, value.Length);
                outputData.BaseStream.Write(buffer, 0, value.Length);
            }
        }

        static public void writeString(string value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes((short)value.Length), 0, 2);

            if (value.Length > 0)
            {
                byte[] buffer = new byte[value.Length + 4];
                convertStringToByteArray(value, ref buffer, 0, value.Length);
                outputData.BaseStream.Write(buffer, 0, value.Length);
            }
            align(outputData);
        }

        public static void writeUnicodeString(string value, StreamWriter outputData)
        {
            writeCompressedUInt32((uint)value.Length, outputData);

            for(int i = 0; i < value.Length; i++)
            {
                ushort converted = Convert.ToUInt16(value[i]);
                writeUInt16(converted, outputData);
            }
        }

        static public void writeEncodedString(string value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes((short)value.Length), 0, 2);
            if (value.Length > 0)
            {
                byte[] buffer = new byte[value.Length + 4];
                convertStringToEncodedByteArray(value, ref buffer, 0, value.Length);
                outputData.BaseStream.Write(buffer, 0, value.Length);
            }
            align(outputData);
        }
        static public void writeVector3(Vector3 value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value.X), 0, 4);
            outputData.BaseStream.Write(BitConverter.GetBytes(value.Y), 0, 4);
            outputData.BaseStream.Write(BitConverter.GetBytes(value.Z), 0, 4);
        }


        static public void writeJson(StreamWriter outputStream, string key, string value, string tab = "", bool isFirst = false, bool lineBreak = true, int padAmount = 0)
        {
            string entryStarter = isFirst ? "" : ",";
            string newLine = lineBreak ? "\n" : "";

            string padding = "";
            //padding = padding.PadLeft(Math.Max(padAmount - (value.Length + 2), 0));
            padding = padding.PadLeft(Math.Max(padAmount, 0));

            string output = $"{entryStarter}{newLine}{tab}\"{key}\": {padding}\"{value}\"";
            outputStream.Write(output);
        }

        static public void writeJson(StreamWriter outputStream, string key, int value, string tab = "", bool isFirst = false, bool lineBreak = true, int padAmount = 0)
        {
            string entryStarter = isFirst ? "" : ",";
            string newLine = lineBreak ? "\n" : "";

            string paddedValue = value.ToString().PadLeft(padAmount);

            string output = $"{entryStarter}{newLine}{tab}\"{key}\": {paddedValue}";
            outputStream.Write(output);
        }

        static public void writeJson(StreamWriter outputStream, string key, uint value, string tab = "", bool isFirst = false, bool lineBreak = true, int padAmount = 0)
        {
            string entryStarter = isFirst ? "" : ",";
            string newLine = lineBreak ? "\n" : "";

            string paddedValue = value.ToString().PadLeft(padAmount);

            string output = $"{entryStarter}{newLine}{tab}\"{key}\": {paddedValue}";
            outputStream.Write(output);
        }

        static public void writeJson(StreamWriter outputStream, string key, Int64 value, string tab = "", bool isFirst = false, bool lineBreak = true, int padAmount = 0)
        {
            string entryStarter = isFirst ? "" : ",";
            string newLine = lineBreak ? "\n" : "";

            string paddedValue = value.ToString().PadLeft(padAmount);

            string output = $"{entryStarter}{newLine}{tab}\"{key}\": {paddedValue}";
            outputStream.Write(output);
        }

        static public void writeJson(StreamWriter outputStream, string key, bool value, string tab = "", bool isFirst = false, bool lineBreak = true, int padAmount = 0)
        {
            string entryStarter = isFirst ? "" : ",";
            string newLine = lineBreak ? "\n" : "";

            string paddedValue = value.ToString().PadLeft(padAmount);
            paddedValue = paddedValue.ToLower();

            string output = $"{entryStarter}{newLine}{tab}\"{key}\": {paddedValue}";
            outputStream.Write(output);
        }

        static public void writeJson(StreamWriter outputStream, string key, Single value, string tab = "", bool isFirst = false, bool lineBreak = true, int padAmount = 0, bool omitTrailingZeroes = false)
        {
            string entryStarter = isFirst ? "" : ",";
            string newLine = lineBreak ? "\n" : "";

            string formattedValue;

            if (!omitTrailingZeroes)
                formattedValue = value.ToString("0.0000");
            else
                formattedValue = value.ToString("0.0###");

            string paddedValue = formattedValue.PadLeft(padAmount);

            string output = $"{entryStarter}{newLine}{tab}\"{key}\": {paddedValue}";
            outputStream.Write(output);
        }

        static public void writeJson(StreamWriter outputStream, string key, Double value, string tab = "", bool isFirst = false, bool lineBreak = true, int padAmount = 0, bool omitTrailingZeroes = false)
        {
            string entryStarter = isFirst ? "" : ",";
            string newLine = lineBreak ? "\n" : "";

            string formattedValue;

            if (!omitTrailingZeroes)
                formattedValue = value.ToString("0.00000000");
            else
                formattedValue = value.ToString("0.0########");

            string paddedValue = formattedValue.PadLeft(padAmount);

            string output = $"{entryStarter}{newLine}{tab}\"{key}\": {paddedValue}";
            outputStream.Write(output);
        }

        static public string removeWcidNameRedundancy(string className, string name)
        {
            string result = "";
            if (className != "" && name != "")
            {
                if (className.ToLower() != name.ToLower())
                    result = $"{name}({className})";
                else
                    result = name;
            }
            else if (className == "" && name != "")
                result = name;
            else if (className != "" && name == "")
                result = className;
            else
                result = "";
            return result;
        }

        static int getRandomNumberWithFavoredValue(int minInclusive, int maxInclusive, double favorValue, double favorStrength, double favorModifier = 0)
        {
            int numValues = (maxInclusive - minInclusive) + 1;
            float maxWeight = (numValues) * 1000;

            IntRange[] range = new IntRange[numValues];

            int value = minInclusive;
            for (int i = 0; i < numValues; i++)
            {
                range[i].Min = value;
                range[i].Max = value;
                range[i].Weight = maxWeight / (float)Math.Pow(1 + ((Math.Pow(favorStrength, 2) / numValues)), Math.Abs(favorValue - value));
                value++;
            }

            return RandomRange.Range(RandomUtil.Standard, range);
        }

        public static int getRandomNumberExclusive(int maxExclusive)
        {
            return getRandomNumber(0, maxExclusive - 1, eRandomFormula.equalDistribution, 0);
        }

        public static int getRandomNumberExclusive(int maxExclusive, eRandomFormula formula, double favorStrength, double favorModifier = 0)
        {
            return getRandomNumber(0, maxExclusive - 1, formula, 0, favorStrength, favorModifier);
        }

        public static int getRandomNumber(int maxInclusive)
        {
            return getRandomNumber(0, maxInclusive, eRandomFormula.equalDistribution, 0);
        }

        public static int getRandomNumber(int maxInclusive, eRandomFormula formula, double favorStrength, double favorModifier = 0)
        {
            return getRandomNumber(0, maxInclusive, formula, 0, favorStrength, favorModifier);
        }

        public static int getRandomNumberExclusive(int maxExclusive, eRandomFormula formula, double favorValue, double favorStrength, double favorModifier = 0)
        {
            return getRandomNumber(0, maxExclusive - 1, formula, favorValue, favorStrength, favorModifier);
        }

        public static int getRandomNumber(int minInclusive, int maxInclusive)
        {
            return getRandomNumber(minInclusive, maxInclusive, eRandomFormula.equalDistribution, 0);
        }

        public static List<int> getRandomNumbersNoRepeat(int amount, int minInclusive, int maxInclusive)
        {
            List<int> numbers = new List<int>();
            for(int i = 0; i < amount; i++)
            {
                numbers.Add(getRandomNumberNoRepeat(minInclusive, maxInclusive, numbers));
            }
            return numbers;
        }

        public static int getRandomNumberNoRepeat(int minInclusive, int maxInclusive, List<int> notThese, int maxTries = 10)
        {
            int potentialValue = getRandomNumber(minInclusive, maxInclusive, eRandomFormula.equalDistribution, 0);
            for (int i = 0; i < maxTries; i++)
            {
                potentialValue = getRandomNumber(minInclusive, maxInclusive, eRandomFormula.equalDistribution, 0);
                if (!notThese.Contains(potentialValue))
                    break;
            }
            return potentialValue;
        }

        public static int getRandomNumber(int minInclusive, int maxInclusive, eRandomFormula formula, double favorStrength, double favorModifier = 0)
        {
            return getRandomNumber(minInclusive, maxInclusive, formula, 0, favorStrength, favorModifier);
        }

        public static int getRandomNumber(int minInclusive, int maxInclusive, eRandomFormula formula, double favorValue, double favorStrength, double favorModifier = 0)
        {
            int numbersAmount = maxInclusive - minInclusive;
            switch (formula)
            {
                case eRandomFormula.favorSpecificValue:
                    {
                        favorValue = favorValue + (numbersAmount * favorModifier);
                        favorValue = Math.Min(favorValue, maxInclusive);
                        favorValue = Math.Max(favorValue, minInclusive);
                        return getRandomNumberWithFavoredValue(minInclusive, maxInclusive, favorValue, favorStrength);
                    }
                case eRandomFormula.favorLow:
                    {
                        favorValue = minInclusive + (numbersAmount * favorModifier);
                        favorValue = Math.Min(favorValue, maxInclusive);
                        favorValue = Math.Max(favorValue, minInclusive);
                        return getRandomNumberWithFavoredValue(minInclusive, maxInclusive, favorValue, favorStrength);
                    }
                case eRandomFormula.favorMid:
                    {
                        int midValue = (int)Math.Round(((double)(maxInclusive - minInclusive) / 2)) + minInclusive;
                        favorValue = midValue + (numbersAmount * favorModifier);
                        favorValue = Math.Min(favorValue, maxInclusive);
                        favorValue = Math.Max(favorValue, minInclusive);
                        return getRandomNumberWithFavoredValue(minInclusive, maxInclusive, favorValue, favorStrength);
                    }
                case eRandomFormula.favorHigh:
                    {
                        favorValue = maxInclusive - (numbersAmount * favorModifier);
                        favorValue = Math.Min(favorValue, maxInclusive);
                        favorValue = Math.Max(favorValue, minInclusive);
                        return getRandomNumberWithFavoredValue(minInclusive, maxInclusive, favorValue, favorStrength);
                    }
                default:
                case eRandomFormula.equalDistribution:
                    {
                        return RandomUtil.Standard.Next(minInclusive, maxInclusive + 1);
                    }
            }
        }

        public static double getRandomDouble(double maxInclusive)
        {
            return getRandomDouble(0, maxInclusive, eRandomFormula.equalDistribution, 0);
        }

        public static double getRandomDouble(double maxInclusive, eRandomFormula formula, double favorStrength, double favorModifier = 0)
        {
            return getRandomDouble(0, maxInclusive, formula, favorStrength, favorModifier);
        }

        public static double getRandomDouble(double minInclusive, double maxInclusive)
        {
            return getRandomDouble(minInclusive, maxInclusive, eRandomFormula.equalDistribution, 0);
        }

        public static double getRandomDouble(double minInclusive, double maxInclusive, eRandomFormula formula, double favorStrength, double favorModifier = 0)
        {
            double decimalPlaces = 1000;
            int minInt = (int)Math.Round(minInclusive * decimalPlaces);
            int maxInt = (int)Math.Round(maxInclusive * decimalPlaces);

            int randomInt = getRandomNumber(minInt, maxInt, formula, favorStrength, favorModifier);
            double returnValue = randomInt / decimalPlaces;

            returnValue = Math.Min(returnValue, maxInclusive);
            returnValue = Math.Max(returnValue, minInclusive);

            return returnValue;
        }

        public static SortedDictionary<int, int> distributionRounding(SortedDictionary<int, int> numbers)
        {
            //Let's do some rounding so it looks better!

            SortedDictionary<int, int> workingNumbers = numbers.Copy();
            SortedDictionary<int, int> resultNumbers = new SortedDictionary<int, int>();

            int highestChanceKey = 0;
            int highestChanceValue = -1;
            int freeAmount = 0;
            float roundingBias = 0;
            for (int i = 0; i < 2; i++)
            {
                foreach (KeyValuePair<int, int> entry in workingNumbers)
                {
                    if (entry.Value > 2500)
                    {
                        //freeAmount += entry.Value % 1000;
                        //resultNumbers[entry.Key] = (int)Math.Floor(entry.Value / 1000.0f) * 1000;

                        int rounded = (int)(Math.Round((entry.Value / 1000.0f) - roundingBias, 0) * 1000);
                        int difference = -(entry.Value - rounded);

                        if (freeAmount >= difference)
                        {
                            freeAmount -= difference;
                            resultNumbers[entry.Key] = rounded;
                        }
                        else
                        {
                            rounded = (int)(Math.Round((entry.Value / 100.0f) - roundingBias, 0) * 100);
                            difference = -(entry.Value - rounded);
                            if (freeAmount >= difference)
                            {
                                freeAmount -= difference;
                                resultNumbers[entry.Key] = rounded;
                            }
                            else
                            {
                                resultNumbers[entry.Key] = entry.Value;
                            }
                        }
                    }
                    else if (entry.Value > 50)
                    {
                        int rounded = (int)(Math.Round((entry.Value / 100.0f) - roundingBias, 0) * 100);
                        int difference = -(entry.Value - rounded);

                        if (freeAmount >= difference)
                        {
                            freeAmount -= difference;
                            resultNumbers[entry.Key] = rounded;
                        }
                        else
                        {
                            rounded = (int)(Math.Round((entry.Value / 10.0f) - roundingBias, 0) * 10);
                            difference = -(entry.Value - rounded);

                            if (freeAmount >= difference)
                            {
                                freeAmount -= difference;
                                resultNumbers[entry.Key] = rounded;
                            }
                            else
                            {
                                freeAmount += entry.Value % 100;
                                resultNumbers[entry.Key] = (int)Math.Floor(entry.Value / 100.0f) * 100;
                            }
                        }
                    }
                    else if (entry.Value > 1)
                    {
                        int rounded = (int)(Math.Round((entry.Value / 10.0f) - roundingBias, 0) * 10);
                        int difference = -(entry.Value - rounded);

                        if (freeAmount >= difference)
                        {
                            freeAmount -= difference;
                            resultNumbers[entry.Key] = rounded;
                        }
                        else
                        {
                            freeAmount += entry.Value % 10;
                            resultNumbers[entry.Key] = (int)Math.Floor(entry.Value / 10.0f) * 10;
                        }
                    }
                    else
                    {
                        resultNumbers[entry.Key] = entry.Value;
                    }

                    if (entry.Value > highestChanceValue)
                    {
                        highestChanceKey = entry.Key;
                        highestChanceValue = entry.Value;
                    }
                }
                workingNumbers = resultNumbers.Copy();
                roundingBias += 0.1f;
            }

            while (freeAmount >= 1000)
            {
                int roll = Utils.getRandomNumber(0, resultNumbers.Count - 1, eRandomFormula.equalDistribution, 2, 0);

                int key = resultNumbers.ElementAt(roll).Key;
                resultNumbers[key] += 1000;
                freeAmount -= 1000;
            }

            int fails = 0;
            int fails2 = 0;
            int failThreshold = 10000;
            while (freeAmount >= 100)
            {
                int roll = Utils.getRandomNumber(0, resultNumbers.Count - 1, eRandomFormula.equalDistribution, 2, 0);
                int key = resultNumbers.ElementAt(roll).Key;

                if (resultNumbers[key] < 3000 || fails > failThreshold * 10)
                {
                    if ((resultNumbers[key] + 100) % 1000 == 0 || fails2 > failThreshold)
                    {
                        resultNumbers[key] += 100;
                        freeAmount -= 100;
                        fails = 0;
                        fails2 = 0;
                        continue;
                    }
                }

                fails++;
                fails2++;
            }

            resultNumbers[highestChanceKey] += freeAmount;
            freeAmount -= freeAmount;

            //while (freeAmount >= 10)
            //{
                //int roll = Utils.getRandomNumber(0, resultNumbers.Count - 1, eRandomFormula.equalDistribution, 2, 0);

                //int key = resultNumbers.ElementAt(roll).Key;
                //resultNumbers[key] += freeAmount;
                //freeAmount -= freeAmount;
            //}

            for (int i = 0; i < resultNumbers.Count; i++)
            {
                int key = resultNumbers.ElementAt(i).Key;
                if (resultNumbers[key] % 100 == 99) // Fixes 1/3 + 1/3 + 1/3 turning into 0.99999 instead of 1.0
                    resultNumbers[key]++;
            }

            if (freeAmount > 0)
                Console.WriteLine($"Unused freeAmount: {freeAmount}");
            return resultNumbers;
        }

        public static void copyDirectory(string sourceDirName, string destDirName, bool copySubDirs, bool replaceFiles)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, replaceFiles);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    copyDirectory(subdir.FullName, temppath, copySubDirs, replaceFiles);
                }
            }
        }

        public static void writeToByteArray(int source, byte[] destination, int offset)
        {
            if (destination == null)
                throw new ArgumentException("Destination array cannot be null");

            // check if there is enough space for all the 4 bytes we will copy
            if (destination.Length < offset + sizeof(int))
                throw new ArgumentException("Not enough room in the destination array");

            for (int i = 0; i < sizeof(int); i++)
            {
                destination[offset + i] = (byte)(source >> (8 * i));
            }
        }

        public static void writeToByteArray(uint source, byte[] destination, int offset)
        {
            if (destination == null)
                throw new ArgumentException("Destination array cannot be null");

            // check if there is enough space for all the 4 bytes we will copy
            if (destination.Length < offset + sizeof(uint))
                throw new ArgumentException("Not enough room in the destination array");

            for (int i = 0; i < sizeof(int); i++)
            {
                destination[offset + i] = (byte)(source >> (8 * i));
            }
        }
    }
}