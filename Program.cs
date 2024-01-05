using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace UMSBT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FileAttributes attr = File.GetAttributes(args[0]);
            //detect whether its a directory or file
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                Build(args[0]);
            else
                Extract(args[0]);
        }
        public static void Extract(string game)
        {
            var reader = new BinaryReader(File.OpenRead(game));
            int[] fileSize = new int[5];
            int[] filePointers = new int[5];
            for (int i = 0; i < 5; i++)
            {
                filePointers[i] = reader.ReadInt32();
                fileSize[i] = reader.ReadInt32();
            }
            string outPath = Path.GetFileNameWithoutExtension(game) + "\\";
            Directory.CreateDirectory(Path.GetFileNameWithoutExtension(game));
            for (int i = 0; i < 5; i++)
            {
                reader.BaseStream.Position = filePointers[i];
                byte[] text = reader.ReadBytes(fileSize[i]);
                File.WriteAllBytes(outPath + i + ".msbt", text);
                Array.Clear(text, 0, fileSize[i]);
            }
        }
        public static void Build(string inputDirectory)
        {
            int[] fileSize = new int[5];
            int[] fileOffset = new int[5];
            using (BinaryWriter arcWriter = new BinaryWriter(File.Create(inputDirectory + ".umsbt")))
            {
                for (int i = 0; i < 48; i++)
                {
                    arcWriter.Write(new byte());
                }
                for (int i = 0; i < 5; i++)
                {
                    byte[] idk = File.ReadAllBytes(inputDirectory + "\\" + i + ".msbt");
                    fileOffset[i] = (int)arcWriter.BaseStream.Position;
                    fileSize[i] = idk.Length;
                    arcWriter.Write(idk);
                }
                arcWriter.BaseStream.Position = 0;
                for (int i = 0; i < 5; i++)
                {
                    arcWriter.Write(fileOffset[i]);
                    arcWriter.Write(fileSize[i]);
                }
            }
        }
    }
}
