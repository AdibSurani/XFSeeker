using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using static System.Linq.Enumerable;
using System.Xml.Serialization;
using System.Xml;
using System.Text;

namespace XFSeeker
{
    class Program
    {
        static List<string> types = new List<string> { "fsm", "xfsc" };

        static void PrintUsage()
        {
            Console.WriteLine("Usage: XFSeeker.exe \"<type>\" <path>\n\nPossible types:\n   fms\n   xfsc");
        }

        static void Main(string[] args)
        {
            if (args.Count() != 2)
            {
                PrintUsage();
                Environment.Exit(0);
            }
            else
            {
                if (!types.Contains(args[0]))
                {
                    Console.WriteLine("Unsupported XFS type!\n");
                    PrintUsage();
                    Environment.Exit(0);
                }

                if (!File.Exists(args[1]))
                {
                    Console.WriteLine("File not found!");
                    Environment.Exit(0);
                }
            }

            var type = args[0];
            var filename = args[1];

            if (type == "fsm")
            {
                var root = (FSM.rAIFSM)ReadXFS(File.OpenRead(filename));

                // Do something arbitrary, such as printing out all conditions
                foreach (var cond in root.mpConditionTree.mpTreeList)
                    Console.WriteLine($"[{cond.mName.mId}] {cond.mpRootNode}");
            }
            else if (type == "xfsc")
            {
                var root = (XFSC.rCharacter)ReadXFS(File.OpenRead(filename));

                // writing to xml will not work directly this way
                //File.WriteAllText(filename + ".xml", ToXmlString<XFSC.rCharacter>(root));

                // Do something arbitrary, such as printing out all rotConstraints
                foreach (var item in root.mRotConstraints)
                    Console.WriteLine($"[{item.mBaseNo}, {item.mNo}] {item.mRotRatio}");
            }
        }

        static object ReadXFS(Stream stream)
        {
            // This is just the standard crc32 algorithm
            uint Crc32(string s) => ~new BitArray(s.Select(x => (byte)x).ToArray()).Cast<bool>()
                                    .Aggregate(~0u, (h, i) => h / 2 ^ (i ^ h % 2 != 0 ? 0xEDB88320 : 0));
            // Get all types in assembly and calculate their crc32-based hashes
            var dicTypes = (from type in Assembly.GetExecutingAssembly().GetTypes()
                            let classname = type.FullName.Split('.').Last().Replace("+", "::") // C# -> C++
                            let hash = ~Crc32(classname) & 0x7FFFFFFF
                            select (hash, type))
                            .ToDictionary(pair => pair.hash, pair => pair.type);

            using (var br = new BinaryReader(stream))
            {
                br.ReadBytes(16);
                int count = br.ReadInt32();
                int infoSize = br.ReadInt32();
                var offsets = Range(0, count).Select(_ => br.ReadInt32() + 0x18).ToList();

                var structs = (from offset in offsets
                               let type = dicTypes[br.ReadUInt32()]
                               let members = (from _ in Range(0, br.ReadInt32())
                                              let fieldInfo = type.GetField(ReadStringAt(br.ReadInt32() + 0x18))
                                              let typeNo = br.ReadInt16()
                                              let typeLength = br.ReadInt16() // ignore length
                                              let bytes = br.ReadBytes(32) // ignore bytes
                                              select (fieldInfo, typeNo))
                                              .ToList()
                               select (type, members))
                               .ToList();

                br.BaseStream.Position = infoSize + 0x18;
                var retval = ReadClass();
                if (br.BaseStream.Position != br.BaseStream.Length) throw new Exception("Unused bytes at end of stream.");
                return retval;

                #region BinaryReader Helpers
                // helpers for reading strings
                string ReadString() => string.Concat(Range(0, 999).Select(_ => (char)br.ReadByte()).TakeWhile(c => c != 0));
                string ReadStringAt(int offset)
                {
                    var tmp = stream.Position;
                    stream.Position = offset;
                    var str = ReadString();
                    stream.Position = tmp;
                    return str;
                }

                // helper for reading a struct or class
                object ReadClass()
                {
                    var (structId, objectId) = (br.ReadInt16(), br.ReadInt16());
                    if (structId == -2) return default;
                    if (structId % 2 == 0) throw new Exception($"Error parsing structId = {structId}");

                    var (size, str) = (br.ReadInt32(), structs[structId / 2]);
                    var item = Activator.CreateInstance(str.type);

                    foreach (var (fieldInfo, type) in str.members)
                    {
                        int lstCount = br.ReadInt32();
                        if ((type & 0xFF00) != 0) // probably means it has to be a list? needs further investigation
                        {
                            var lst = (IList)Activator.CreateInstance(fieldInfo.FieldType);
                            for (int i = 0; i < lstCount; i++)
                                lst.Add(ReadObject(type));
                            fieldInfo.SetValue(item, lst);
                        }
                        else if (lstCount == 1)
                            fieldInfo.SetValue(item, ReadObject(type));
                        else
                            throw new Exception($"Error parsing type = {type}");
                    }
                    return item;
                }

                //helper for reading a TypePath
                object ReadTypePath()
                {
                    var stringCount = br.ReadByte() - 1;
                    var typeName = ReadString();
                    var pathList = (from i in Range(0, stringCount)
                                    select ReadString());
                    return new XFSC.TypePath { typeName = typeName, paths = pathList.ToList() };
                }

                // helper for reading an object by type
                object ReadObject(int type)
                {
                    switch ((byte)type)
                    {
                        case 1: // fallthrough: ReadStruct() == ReadClass()
                        case 2: return ReadClass();
                        case 3: return br.ReadByte() != 0;
                        case 4: return br.ReadSByte();
                        case 6: return br.ReadUInt32();
                        case 10: return br.ReadInt32();
                        case 12: return br.ReadSingle();
                        case 14: return ReadString();
                        case 20: return (br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                        case 21: return (br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32());
                        case 0x80: return ReadTypePath();
                        default: throw new NotSupportedException($"Unknown type {type}");
                    }
                }
                #endregion
            }
        }

        public static string ToXmlString<T>(object obj)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var xmlSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                NewLineOnAttributes = false,
                IndentChars = "\t",
                CheckCharacters = false,
                OmitXmlDeclaration = true
            };
            using (var sw = new StringWriter())
            {
                new XmlSerializer(typeof(T)).Serialize(XmlWriter.Create(sw, xmlSettings), obj, ns);
                return sw.ToString();
            }
        }
    }
}
