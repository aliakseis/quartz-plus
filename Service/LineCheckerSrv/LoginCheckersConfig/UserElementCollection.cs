using System.Configuration;

namespace LineCheckerSrv.LoginCheckersConfig
{
    [ConfigurationCollection(typeof(CheckerElement),
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    internal sealed class UserElementCollection : 
        ConfigurationElementCollection<CheckerElement>
    {
    } 
}
