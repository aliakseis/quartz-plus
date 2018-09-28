using System;
using System.IO;

namespace ILC_ControlPanel.Utils
{
    /// <summary>
    /// Class implements functionality for getting disk information
    /// </summary>
    static class DiskInfo
    {
        private const int PERCENTAGE = 10;
        /// <summary>
        /// Convert bytes to appropriate values(KB, MB, GB)
        /// </summary>
        /// <param name="bytes">bytes value</param>
        /// <returns></returns>
        public static string ConvertBytes(long bytes)
        {
            if (bytes >= 1073741824)
            {
                Decimal size = Decimal.Divide(bytes, 1073741824);
                return String.Format("{0:###.#} GB", size);
            }
            if (bytes >= 1048576)
            {
                Decimal size = Decimal.Divide(bytes, 1048576);
                return String.Format("{0:###.#} MB", size);
            }
            if (bytes >= 1024)
            {
                Decimal size = Decimal.Divide(bytes, 1024);
                return String.Format("{0:###.#} KB", size);
            }
            if (bytes > 0)
            {
                Decimal size = bytes;
                return String.Format("{0:###.#} bytes", size);
            }
            return "0 bytes";
        }


        /// <summary>
        /// Gets directory size
        /// </summary>
        /// <param name="path">directory path</param>
        /// <returns>directory size in bytes</returns>
        public static long GetDirSize(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            long size = 0;
            // Add file sizes.
            FileInfo[] filesInfo = dirInfo.GetFiles();
            foreach (FileInfo fileInfo in filesInfo)
            {
                size += fileInfo.Length;
            }
            
            return size;
        }

        /// <summary>
        /// Gets directory size
        /// </summary>
        /// <param name="path">directory path</param>
        /// <returns>directory size in bytes</returns>
        public static long GetDirSizeFast(string path)
        {
            long size = 0;
            foreach (FileData file in FastDirectoryEnumerator.EnumerateFiles(path))
            {
                size += file.Size;
            }
            return size;
        }


        /// <summary>
        /// Get directory size using Monte Carlo method
        /// </summary>
        /// <param name="path">directory path</param>
        /// <returns>directory size in bytes</returns>
        public static long GetDirSizeByMonteCarlo(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            long size = 0;

            FileInfo[] files = di.GetFiles();
            // if files amount less than 1000 we return real directory size
            if (files.Length < 1000)
            {
                foreach (FileInfo file in files)
                {
                    size += file.Length;
                }
                return size;
            }
            
            int count = files.Length * PERCENTAGE / 100;
            
            // we need same random numbers every time to avoid rolling of results if amount of files is not changed
            Random rnd = new Random(files.Length);
            
            for (int i = 0; i < count; ++i)
            {
                size += files[rnd.Next(0, files.Length - 1)].Length;
            }
            
            return size * (100 / PERCENTAGE); 
        }

        /// <summary>
        /// Gets size of disk that contains pointed directory
        /// </summary>
        /// <param name="path">directory path</param>
        /// <param name="size">size in bytes</param>
        /// <param name="convertedSize">converted size</param>
        public static void GetDiskSize(string path, out string size, out string convertedSize)
        {
            size = "0 bytes";
            convertedSize = "0 bytes";
            
            if (string.IsNullOrEmpty(path))
                return;
            
            DriveInfo di = null;
            try
            {
                di = new DriveInfo(Path.GetPathRoot(path));
            }
            catch (System.Exception)
            {
                size = " bytes";
                convertedSize = " bytes";
                return;
            }

            size = di.TotalFreeSpace.ToString("###,###,###,###,###,###") + " bytes";
            convertedSize = ConvertBytes(di.TotalFreeSpace);
        }

    }
}
