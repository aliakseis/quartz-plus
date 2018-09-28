using System.Collections;
using System.Web.UI;

namespace ILCWebApplication.ValidationInfoDS
{
    /// <summary>
    /// Represents a hierarchical collection that can be enumerated with an IEnumerator interface
    /// </summary>
    public class ValidationInfoHierarchicalEnumerable : ArrayList, IHierarchicalEnumerable
    {
        #region IHierarchicalEnumerable Members

        public IHierarchyData GetHierarchyData(object enumeratedItem)
        {
            return enumeratedItem as IHierarchyData;
        }

        #endregion
    }
}