using System.Configuration;

namespace LineCheckerSrv.LoginCheckersConfig
{
    /// <summary>
    /// Represents a "checkers" section within a configuration file.
    /// </summary>
    public sealed class ProjectConfigurationSection : 
        ConfigurationSection
    {
        [ConfigurationProperty("checkers", 
            IsDefaultCollection = true, IsRequired = true)]
        internal UserElementCollection Checkers
        {
            get { return (UserElementCollection)this["checkers"]; }
        }
    }
}
