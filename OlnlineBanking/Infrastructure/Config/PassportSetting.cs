using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

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