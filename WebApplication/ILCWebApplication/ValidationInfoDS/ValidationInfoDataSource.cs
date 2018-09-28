using System.Web.UI;

namespace ILCWebApplication.ValidationInfoDS
{
    /// <summary>
    /// Class for data source controls that represent validation hierarchical data
    /// </summary>
    public class ValidationInfoDataSource : HierarchicalDataSourceControl
    {
        protected override HierarchicalDataSourceView GetHierarchicalView(string viewPath)
        {
            return new ValidationInfoDataSourceView(viewPath);
        }

        protected override ControlCollection CreateControlCollection()
        {
            return new ControlCollection(this);
        }
    }
}