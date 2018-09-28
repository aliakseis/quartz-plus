using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

/// <summary>
/// See http://www.codeproject.com/KB/files/FastDirectoryEnumerator.aspx
/// </summary>
namespace ILC_ControlPanel.Utils
{
    /// <summary>
    /// Contains information about a file returned by the 
    /// <see cref="FastDirectoryEnumerator"/> class.
    /// </summary>
    [Serializable]
    public class FileData
    {
        /// <summary>
        /// Attributes of the file.
        /// </summary>
        public readonly FileAttributes Attributes;

        /// <summary>
        /// File creation time in UTC
        /// </summary>
        public readonly DateTime CreationTimeUtc;

        /// <summary>
        /// File last access time in UTC
        /// </summary>
        public readonly DateTime LastAccessTimeUtc;

        /// <summary>
        /// File last write time in UTC
        /// </summary>
        public readonly DateTime LastWriteTimeUtc;

        /// <summary>
        /// Name of the file
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Full path to the file.
        /// </summary>
        public readonly string Path;

        /// <summary>
        /// Size of the file in bytes
        /// </summary>
        public readonly long Size;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileData"/> class.
        /// </summary>
        /// <param name="dir">The directory that the file is stored at</param>
        /// <param name="findData">WIN32_FIND_DATA structure that this
        /// object wraps.</param>
        internal FileData(string dir, WIN32_FIND_DATA findData)
        {
            Attributes = findData.dwFileAttributes;


            CreationTimeUtc = ConvertDateTime(findData.ftCreationTime_dwHighDateTime,
                                              findData.ftCreationTime_dwLowDateTime);

            LastAccessTimeUtc = ConvertDateTime(findData.ftLastAccessTime_dwHighDateTime,
                                                findData.ftLastAccessTime_dwLowDateTime);

            LastWriteTimeUtc = ConvertDateTime(findData.ftLastWriteTime_dwHighDateTime,
                                               findData.ftLastWriteTime_dwLowDateTime);

            Size = CombineHighLowInts(findData.nFileSizeHigh, findData.nFileSizeLow);

            Name = findData.cFileName;
            Path = System.IO.Path.Combine(dir, findData.cFileName);
        }

        public DateTime CreationTime
        {
            get { return CreationTimeUtc.ToLocalTime(); }
        }

        /// <summary>
        /// Gets the last access time in local time.
        /// </summary>
        public DateTime LastAccesTime
        {
            get { return LastAccessTimeUtc.ToLocalTime(); }
        }

        /// <summary>
        /// Gets the last access time in local time.
        /// </summary>
        public DateTime LastWriteTime
        {
            get { return LastWriteTimeUtc.ToLocalTime(); }
        }

        private static long CombineHighLowInts(uint high, uint low)
        {
            return (((long) high) << 0x20) | low;
        }

        private static DateTime ConvertDateTime(uint high, uint low)
        {
            long fileTime = CombineHighLowInts(high, low);
            return DateTime.FromFileTimeUtc(fileTime);
        }
    }

    /// <summary>
    /// Contains information about the file that is found 
    /// by the FindFirstFile or FindNextFile functions.
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto), BestFitMapping(false)]
    internal class WIN32_FIND_DATA
    {
        public FileAttributes dwFileAttributes;
        public uint ftCreationTime_dwLowDateTime;
        public uint ftCreationTime_dwHighDateTime;
        public uint ftLastAccessTime_dwLowDateTime;
        public uint ftLastAccessTime_dwHighDateTime;
        public uint ftLastWriteTime_dwLowDateTime;
        public uint ftLastWriteTime_dwHighDateTime;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
        public int dwReserved0;
        public int dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)] public string cAlternateFileName;

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "File name=" + cFileName;
        }
    }

    /// <summary>
    /// A fast enumerator of files in a directory.  Use this if you need to get attributes for 
    /// all files in a directory.
    /// </summary>
    /// <remarks>
    /// This enumerator is substantially faster than using <see cref="Directory.GetFiles(string)"/>
    /// and then creating a new FileInfo object for each path.  Use this version when you 
    /// will need to look at the attibutes of each file returned (for example, you need
    /// to check each file in a directory to see if it was modified after a specific date).
    /// </remarks>
    public static class FastDirectoryEnumerator
    {
        /// <summary>
        /// Gets <see cref="FileData"/> for all the files in a directory.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <returns>An object that implements <see cref="IEnumerable{T}"/> and 
        /// allows you to enumerate the files in the given directory.</returns>
        public static IEnumerable<FileData> EnumerateFiles(string path)
        {
            return new FileEnumerable(path, "*", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Gets <see cref="FileData"/> for all the files in a directory that match a 
        /// specific filter.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="filter">The search string to match against files in the path.</param>
        /// <returns>An object that implements <see cref="IEnumerable{FileData}"/> and 
        /// allows you to enumerate the files in the given directory.</returns>
        public static IEnumerable<FileData> EnumerateFiles(string path, string filter)
        {
            return new FileEnumerable(path, filter, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Gets <see cref="FileData"/> for all the files in a directory that 
        /// match a specific filter, optionally including all sub directories.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="filter">The search string to match against files in the path.</param>
        /// <param name="searchOption">
        /// One of the SearchOption values that specifies whether the search 
        /// operation should include all subdirectories or only the current directory.
        /// </param>
        /// <returns>An object that implements <see cref="IEnumerable{FileData}"/> and 
        /// allows you to enumerate the files in the given directory.</returns>
        public static IEnumerable<FileData> EnumerateFiles(string path, string filter, SearchOption searchOption)
        {
            return new FileEnumerable(path, filter, searchOption);
        }

        #region Nested type: FileEnumerable

        /// <summary>
        /// Provides the implementation of the 
        /// <see cref="T:System.Collections.Generic.IEnumerable`1"/> interface
        /// </summary>
        private class FileEnumerable : IEnumerable<FileData>
        {
            private readonly string m_filter;
            private readonly string m_path;
            private readonly SearchOption m_searchOption;

            /// <summary>
            /// Initializes a new instance of the <see cref="FileEnumerable"/> class.
            /// </summary>
            /// <param name="path">The path to search.</param>
            /// <param name="filter">The search string to match against files in the path.</param>
            /// <param name="searchOption">
            /// One of the SearchOption values that specifies whether the search 
            /// operation should include all subdirectories or only the current directory.
            /// </param>
            public FileEnumerable(string path, string filter, SearchOption searchOption)
            {
                m_path = path;
                m_filter = filter;
                m_searchOption = searchOption;
            }

            #region IEnumerable<FileData> Members

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can 
            /// be used to iterate through the collection.
            /// </returns>
            public IEnumerator<FileData> GetEnumerator()
            {
                return new FileEnumerator(m_path, m_filter, m_searchOption);
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator"/> object that can be 
            /// used to iterate through the collection.
            /// </returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return new FileEnumerator(m_path, m_filter, m_searchOption);
            }

            #endregion
        }

        #endregion

        #region Nested type: FileEnumerator

        /// <summary>
        /// Provides the implementation of the 
        /// <see cref="T:System.Collections.Generic.IEnumerator`1"/> interface
        /// </summary>
        [SuppressUnmanagedCodeSecurity]
        private class FileEnumerator : IEnumerator<FileData>
        {
            private readonly string m_filter;
            private readonly Stack<SearchContext> m_handles;
            private readonly SearchOption m_searchOption;

            private readonly WIN32_FIND_DATA m_win_find_data = new WIN32_FIND_DATA();
            private SafeFindHandle m_hndFindFile;
            private string m_path;

            /// <summary>
            /// Initializes a new instance of the <see cref="FileEnumerator"/> class.
            /// </summary>
            /// <param name="path">The path to search.</param>
            /// <param name="filter">The search string to match against files in the path.</param>
            /// <param name="searchOption">
            /// One of the SearchOption values that specifies whether the search 
            /// operation should include all subdirectories or only the current directory.
            /// </param>
            public FileEnumerator(string path, string filter, SearchOption searchOption)
            {
                m_path = path;
                m_filter = filter;
                m_searchOption = searchOption;

                if (m_searchOption == SearchOption.AllDirectories)
                {
                    m_handles = new Stack<SearchContext>();
                }
            }

            #region IEnumerator<FileData> Members

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <value></value>
            /// <returns>
            /// The element in the collection at the current position of the enumerator.
            /// </returns>
            public FileData Current
            {
                get { return new FileData(m_path, m_win_find_data); }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, 
            /// or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                if (!m_hndFindFile.IsClosed)
                {
                    m_hndFindFile.Dispose();
                }
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <value></value>
            /// <returns>
            /// The element in the collection at the current position of the enumerator.
            /// </returns>
            object IEnumerator.Current
            {
                get { return new FileData(m_path, m_win_find_data); }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; 
            /// false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public bool MoveNext()
            {
                bool retval;

                //If the handle is null, this is first call to MoveNext in the current 
                // directory.  In that case, start a new search.
                if (m_hndFindFile == null)
                {
                    string searchPath = Path.Combine(m_path, m_filter);
                    m_hndFindFile = FindFirstFile(searchPath, m_win_find_data);
                    retval = !m_hndFindFile.IsInvalid;
                }
                else
                {
                    //Otherwise, find the next item.
                    retval = FindNextFile(m_hndFindFile, m_win_find_data);
                }

                //If the call to FindNextFile or FindFirstFile succeeded.
                if (retval)
                {
                    if (m_win_find_data.cFileName == "." ||
                        m_win_find_data.cFileName == "..")
                    {
                        //Ignore the special "." and ".." folders.   We 
                        //call MoveNext recursively here to move to the next item 
                        //that FindNextFile will return.
                        return MoveNext();
                    }

                    //If we are now on a directory...
                    if (m_win_find_data.dwFileAttributes == FileAttributes.Directory)
                    {
                        //If we are searching directories, push the current directory onto the 
                        // context stack, then restart the search in the sub-directory. 
                        //Otherwise, skip the directory and move on to the next file in the current 
                        // directory.
                        if (m_searchOption == SearchOption.AllDirectories)
                        {
                            SearchContext context = new SearchContext(m_hndFindFile, m_path);
                            m_handles.Push(context);

                            m_path = Path.Combine(m_path, m_win_find_data.cFileName);
                            m_hndFindFile = null;
                        }

                        return MoveNext();
                    }
                }
                else if (m_searchOption == SearchOption.AllDirectories)
                {
                    //If there are no more files in this directory and we are 
                    // in a sub directory, pop back up to the parent directory and
                    // continue the search from there.
                    if (m_handles.Count > 0)
                    {
                        SearchContext context = m_handles.Pop();
                        m_path = context.Path;
                        m_hndFindFile = context.Handle;

                        return MoveNext();
                    }
                }

                return retval;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public void Reset()
            {
                m_hndFindFile = null;
            }

            #endregion

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern SafeFindHandle FindFirstFile(string fileName,
                                                               [In, Out] WIN32_FIND_DATA data);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern bool FindNextFile(SafeFindHandle hndFindFile,
                                                    [In, Out, MarshalAs(UnmanagedType.LPStruct)] WIN32_FIND_DATA
                                                        lpFindFileData);

            #region Nested type: SearchContext

            /// <summary>
            /// Hold context information about where we current are in the directory search.
            /// </summary>
            private class SearchContext
            {
                public readonly SafeFindHandle Handle;
                public readonly string Path;

                public SearchContext(SafeFindHandle handle, string path)
                {
                    Handle = handle;
                    Path = path;
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: SafeFindHandle

        /// <summary>
        /// Wraps a FindFirstFile handle.
        /// </summary>
        private sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SafeFindHandle"/> class.
            /// </summary>
            [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
            internal SafeFindHandle()
                : base(true)
            {
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [DllImport("kernel32.dll")]
            private static extern bool FindClose(IntPtr handle);

            /// <summary>
            /// When overridden in a derived class, executes the code required to free the handle.
            /// </summary>
            /// <returns>
            /// true if the handle is released successfully; otherwise, in the 
            /// event of a catastrophic failure, false. In this case, it 
            /// generates a releaseHandleFailed MDA Managed Debugging Assistant.
            /// </returns>
            protected override bool ReleaseHandle()
            {
                return FindClose(handle);
            }
        }

        #endregion
    }
}