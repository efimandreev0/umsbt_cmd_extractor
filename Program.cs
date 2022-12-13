using System;
using System.IO;
using archive_nintendo.UMSBT;

namespace umsbt
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                FileAttributes attr = File.GetAttributes(args[0]);
                bool isDir = false;
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    isDir = true;

                if (!isDir) 
                {
                    Directory.CreateDirectory(Path.GetFileNameWithoutExtension(args[0]));
                    UMSBT msbt = new UMSBT(File.OpenRead(args[0]));
                    foreach (var file in msbt.Files)
                    {
                        byte[] buf = new byte[(int)file.FileSize];
                        using (Stream str = file.FileData)
                        {
                            str.Read(buf, 0, buf.Length);
                        }
                        File.WriteAllBytes(Path.GetFileNameWithoutExtension(args[0]) + "\\" + file.FileName, buf);
                    }
                }
                else
                {
                    UMSBT umsbt = new UMSBT();
                    string[] files = Directory.GetFiles(args[0], "*.msbt", SearchOption.TopDirectoryOnly);
                    foreach(var file in files)
                    {
                        byte[] data = File.ReadAllBytes(file);
                        umsbt.Files.Add(new UmsbtFileInfo
                        {
                            FileData = new MemoryStream(data)
                        });
                    }
                    umsbt.Save(File.OpenWrite(args[0] + ".umsbt"));
                }
            }
            
        }
    }
}
