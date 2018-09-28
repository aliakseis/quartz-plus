using System;

namespace ILCWebApplication.ValidationInfoDS
{
    /// <summary>
    /// Summary description for PathAnalyzer
    /// </summary>
    public static class PathAnalyzer
    {
        /// <summary>
        /// splitter character
        /// </summary>
        public const char SPLITTER = '/';

        /// <summary>
        /// Defines item type
        /// </summary>
        public enum ItemType { None = 0, Root = 1, Server = 2, Project = 3 };

        /// <summary>
        /// Gets item type
        /// </summary>
        /// <param name="viewPath">view path</param>
        /// <returns>item type</returns>
        public static ItemType GetItemType(string viewPath)
        {
            string[] split = viewPath.Split( SPLITTER );
            return (ItemType) split.Length;
        }

        /// <summary>
        /// Gets server id
        /// </summary>
        /// <param name="viewPath">view path</param>
        /// <returns>server id</returns>
        public static string GetServerId(string viewPath)
        {
            string[] split = viewPath.Split( SPLITTER );
            return split[2];
        }

        /// <summary>
        /// Gets project id
        /// </summary>
        /// <param name="viewPath">view path</param>
        /// <returns>project id</returns>
        public static string GetProjectId(string viewPath)
        {
            string[] split = viewPath.Split( SPLITTER );
            return split[3];
        }
    }
}