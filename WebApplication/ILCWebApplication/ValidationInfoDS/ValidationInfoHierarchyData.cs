using System;
using System.Web;
using System.Web.UI;

namespace ILCWebApplication.ValidationInfoDS
{
    /// <summary>
    /// Exposes a node of a hierarchical data structure, including the node object
    /// and some properties that describe characteristics of the node
    /// </summary>
    public class ValidationInfoHierarchyData : IHierarchyData, INavigateUIData
    {
        private readonly ValidationInfoHierarchicalEnumerable children;

        private readonly string name;
        private readonly string path;
        protected bool hasChildren;
        protected string id;
        protected string navigatePage;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="id">node id</param>
        /// <param name="name">node name</param>
        /// <param name="path">node path</param>
        public ValidationInfoHierarchyData(string id, string name, string path)
        {
            hasChildren = true;
            navigatePage = "";

            this.id = id;
            this.name = name;
            this.path = path + PathAnalyzer.SPLITTER + id;

            children = new ValidationInfoHierarchicalEnumerable();
        }

        #region IHierarchyData Members

        public bool HasChildren
        {
            get
            {
                return hasChildren;
            }
        }


        public string Path
        {
            get { return path; }
        }


        public object Item
        {
            get { return this; }
        }


        public string Type
        {
            get { return "ValidationInfoHierarchyData"; }
        }


        public IHierarchicalEnumerable GetChildren()
        {
            return children;
        }

        public IHierarchyData GetParent()
        {
            throw new ApplicationException("Unimplemented method was called: GetParent()");
        }

        #endregion

        #region INavigateUIData Members

        public string Description
        {
            get { return ""; }
        }

        public string Name
        {
            get { return name; }
        }

        public string NavigateUrl
        {
            get
            {
                return "javascript:LoadGridView('" + navigatePage + "');";
            }
        }

        public string Value
        {
            get { return id; }
        }

        #endregion

        public override string ToString()
        {
            return name; 
        }
    }

    /// <summary>
    /// Exposes a root node of a hierarchical data structure
    /// </summary>
    public class RootValidationInfoHierarchyData : ValidationInfoHierarchyData
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="id">node id</param>
        /// <param name="name">node name</param>
        /// <param name="path">node path</param>
        public RootValidationInfoHierarchyData(string id, string name, string path)
            : base(id, name, path)
        {
            navigatePage = "Grids/ServersGrid.aspx";
        }
    }

    /// <summary>
    /// Exposes a server node of a hierarchical data structure
    /// </summary>
    public class ServerValidationInfoHierarchyData : ValidationInfoHierarchyData
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="id">node id</param>
        /// <param name="name">node name</param>
        /// <param name="path">node path</param>
        public ServerValidationInfoHierarchyData(string id, string name, string path)
            : base(id, name, path)
        {
            navigatePage = "Grids/ProjectsGrid.aspx?server=" + id;
        }
    }

    /// <summary>
    /// Exposes a project node of a hierarchical data structure
    /// </summary>
    public class ProjectValidationInfoHierarchyData : ValidationInfoHierarchyData
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="id">node id</param>
        /// <param name="name">node name</param>
        /// <param name="path">node path</param>
        public ProjectValidationInfoHierarchyData(string id, string name, string path)
            : base(id, name, path)
        {
            hasChildren = false;
            navigatePage = "Grids/ItemsGrid.aspx?project=" + id;
        }
    }
}