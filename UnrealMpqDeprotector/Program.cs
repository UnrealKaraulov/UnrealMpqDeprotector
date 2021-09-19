using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UnrealMpqDeprotector
{
    class Program
    {
        static void Main(string[] args)
        {


            string MapPath = string.Empty;
            if (args.Length == 1)
            {
                MapPath = args[0].Replace("\"", "");
            }
            else
            {
                Console.WriteLine("Enter path to map:");
                MapPath = Console.ReadLine().Replace("\"", "");
            }
            bool BadMapHeader = false;
            byte[] MapData = File.ReadAllBytes(MapPath);
            List<byte> OutMapData = new List<byte>();
            List<byte> OutMapHeader = new List<byte>();
            MemoryStream MapDataStream = new MemoryStream(MapData);
            BinaryReader MapDataReader = new BinaryReader(MapDataStream);

            Console.WriteLine("Test W3X/W3M header and restore if possibled.");
            string War3Header = new string(MapDataReader.ReadChars(4));

            if (War3Header != "HM3W")
            {
                Console.WriteLine("Bad map header.");
                Console.ReadKey();
                return;
            }

            OutMapHeader.AddRange(BitConverter.GetBytes(1462979912));


            Console.WriteLine("Warcraft III map detected.");

            int UnknownInt = MapDataReader.ReadInt32();

            if (UnknownInt != 0)
            {
                BadMapHeader = true;
                Console.WriteLine("Possible bad map header!");
            }

            OutMapHeader.AddRange(BitConverter.GetBytes(0));

            List<byte> MapName = new List<byte>();
            while (true)
            {
                byte readedbyte = MapDataReader.ReadByte();
                if (readedbyte == 0)
                    break;
                MapName.Add(readedbyte);
            }

            OutMapHeader.AddRange(MapName.ToArray());



            int MapFlags = MapDataReader.ReadInt32();
            int Players = MapDataReader.ReadInt32();
            if (Players > 15)
            {
                BadMapHeader = true;
                Console.WriteLine("Bad player count, map - protected.");
            }

            OutMapHeader.AddRange(BitConverter.GetBytes(MapFlags));
            OutMapHeader.AddRange(BitConverter.GetBytes(10));


            while (MapDataReader.ReadByte() == 0)
            {

            }

            if (MapDataReader.BaseStream.Position != 513)
            {
                BadMapHeader = true;
                Console.WriteLine("Bad map heaeder, map - protected.");
            }
            else
            {
                MapDataReader.BaseStream.Seek(-1, SeekOrigin.Current);
            }

            for (int i = OutMapHeader.Count; i < 512; i++)
            {
                OutMapHeader.Add(0);
            }

            Console.WriteLine("Map heaeder deprotected!");
            
            Console.WriteLine("Search MPQ header and restore if possibled.");

            Console.WriteLine("the end.");
            Console.ReadKey();


        }
    }
}
