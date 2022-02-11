using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace FileConvert.Common
{
    public class FileUpdate
    {
        public static void DeleteAllFile(string Directorypath)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Directorypath);
            if (di.Exists)
            {
                FileInfo[] ff = di.GetFiles();
                foreach (FileInfo temp in ff)
                {
                    File.Delete(Directorypath + "\\" + temp.Name);
                }
                Directory.Delete(Directorypath);
            }
        }

    }
}
