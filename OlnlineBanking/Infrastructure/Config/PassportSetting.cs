using System.Configuration;

namespace OlnlineBanking.Infrastructure.Config
{
    public class PassportSetting: ConfigurationSection
    {
        [ConfigurationProperty("BlockAttempts", IsRequired = true, DefaultValue = 5)]
        public int BlockAttempts
        {
            get
            {
                return (int)this["BlockAttempts"];
            }
            set
            {
                this["BlockAttempts"] = value;
            }
        }
    }
}