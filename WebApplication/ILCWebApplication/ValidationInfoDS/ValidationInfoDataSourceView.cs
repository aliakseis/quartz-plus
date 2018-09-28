using System;
using System.Collections.Generic;
using System.Web.UI;

namespace ILCWebApplication.ValidationInfoDS
{
    /// <summary>
    /// Represents a data view on collection of nodes in a hierarchical data structure
    /// </summary>
    public class ValidationInfoDataSourceView : HierarchicalDataSourceView
    {
        private readonly string viewPath;
        private ValidationInfoHierarchicalEnumerable fshe;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="viewPath">view path</param>
        public ValidationInfoDataSourceView(string viewPath)
        {
            this.viewPath = viewPath;
        }

        public override IHierarchicalEnumerable Select()
        {
            if (fshe == null)
            {
                fshe = GetHierarchicalEnumerable(viewPath);
            }
            return fshe;
        }

        private static ValidationInfoHierarchicalEnumerable GetHierarchicalEnumerable(string viewPath)
        {
            ValidationInfoHierarchicalEnumerable fshe = new ValidationInfoHierarchicalEnumerable();

            PathAnalyzer.ItemType type = PathAnalyzer.GetItemType(viewPath);
            switch (type)
            {
                case PathAnalyzer.ItemType.Root:
                    fshe.Add(
                        new RootValidationInfoHierarchyData("Root", "Servers", viewPath));
                    break;
                case PathAnalyzer.ItemType.Server:
                    {
                        IList<KeyValuePair<string, string>> list = IlcWebDao.GetServersList();
                        foreach (KeyValuePair<string, string> kvp in list)
                        {
                            fshe.Add(
                                new ServerValidationInfoHierarchyData(kvp.Key, kvp.Value, viewPath));
                        }
                    }
                    break;
                case PathAnalyzer.ItemType.Project:
                    {
                        IList<KeyValuePair<string, string>> list =
                            IlcWebDao.GetProjectsList(PathAnalyzer.GetServerId(viewPath));
                        foreach (KeyValuePair<string, string> kvp in list)
                        {
                            fshe.Add(new ProjectValidationInfoHierarchyData(kvp.Key, kvp.Value, viewPath));
                        }
                    }
                    break;
                default:
                    throw new ApplicationException("Unexpected type of tree node");
            }
            return fshe;
        }
    }
}