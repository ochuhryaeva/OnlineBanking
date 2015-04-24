using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using OlnlineBanking.Infrastructure.Abstract;
using OlnlineBanking.Infrastructure.Config;

namespace OlnlineBanking.Infrastructure.Concrete
{
    public class ConfigManager: IConfig
    {

        public Config.MailSetting MailSetting
        {
            get
            {
                return ConfigurationManager.GetSection("mailConfig") as MailSetting;
            }
        }

        public Config.PassportSetting PassportSetting
        {
            get
            {
                return ConfigurationManager.GetSection("passportConfig") as PassportSetting;
            }
        }
    }
}