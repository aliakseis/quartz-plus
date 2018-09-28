using System.Configuration;

namespace LineCheckerSrv.LoginCheckersConfig
{
    internal class CheckerElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)(base["name"]); }
        }

        [ConfigurationProperty("module", IsRequired = true)]
        public string Module
        {
            get { return (string)(base["module"]); }
        }

        [ConfigurationProperty("class", IsRequired = true)]
        public string Class
        {
            get { return (string)(base["class"]); }
        }

        [ConfigurationProperty("method", IsRequired = true)]
        public string Method
        {
            get { return (string)(base["method"]); }
        }  

    }
}
