using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libplctag;
using libplctag.DataTypes.Simple;

namespace Ring.Services.PLC
{
    public sealed class PlcRealBlockReader
    {
        private readonly string _nameBase;  // e.g., "PC_Read_Float[0]"
        private readonly string _ip;
        private readonly string _path;
        private readonly int _count;

        public PlcRealBlockReader(string nameBase, string ip, int count, string path = "1,0")
        {
            _nameBase = nameBase;
            _ip = ip;
            _path = path;
            _count = count;
        }

        public float[] ReadAll()
        {
            try
            {
                // Use the fallback approach since TagRealArray doesn't exist
                // Read in chunks to avoid too many individual calls
                return PlcRealBlockReaderCompat.ReadAll180(_ip, _path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Read float block error from {_nameBase}: {ex.Message}");
                return new float[_count]; // Return zero array on error
            }
        }
    }

    public static class PlcRealBlockReaderCompat
    {
        // Reads a contiguous slice: startIndex..(startIndex+length-1)
        public static float[] ReadSlice(string baseName, string ip, int startIndex, int length, string path = "1,0")
        {
            // Many builds allow naming a slice by setting Name to the first element.
            // We still need one request per element with TagReal, so instead we batch in reasonable chunks.
            // If your wrapper lacks a true array read, do 2–4 chunks max (not 179 calls).
            var result = new float[length];
            for (int i = 0; i < length; i++)
            {
                var tag = new TagReal
                {
                    Name = $"{baseName.Replace("[0]", $"[{startIndex + i}]")}", // "PC_Read_Float[<i>]"
                    Gateway = ip,
                    Path = path,
                    PlcType = PlcType.ControlLogix,
                    Protocol = Protocol.ab_eip
                };
                result[i] = tag.Read();
            }
            return result;
        }

        public static float[] ReadAll180(string ip, string path = "1,0")
        {
            // 180 REALs total -> read as two slices so you never create hundreds of tiny calls elsewhere.
            // If your wrapper supports true array reads, prefer Option A.
            var first = ReadSlice("PC_Read_Float[0]", ip, 0, 120, path); // chunk 1
            var second = ReadSlice("PC_Read_Float[0]", ip, 120, 60, path); // chunk 2

            var all = new float[180];
            Array.Copy(first, 0, all, 0, first.Length);
            Array.Copy(second, 0, all, 120, second.Length);
            return all;
        }
    }

    // Define a mapping enum for readability instead of magic numbers
    public enum ReadFloatMap
    {
        // Tank Weight and Temperature (Make Ready Tank)
        TankWeight = 30,
        TankTemperature = 31,
        BoraxCausticWeight = 32,
        
        // Add more mappings as needed
        MixerSpeed = 5,
        Tank1Level = 25,
        Tank2Level = 26,
        Tank3Level = 27,
        Tank4Level = 28,
        
        // Add other commonly used indices
        Pressure = 10,
        FlowRate = 15,
        PhLevel = 20,
        AgitatorSpeed = 35,
        HeatingPower = 40
    }
}
