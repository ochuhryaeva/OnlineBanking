using System.Configuration;
using OlnlineBanking.Infrastructure.Abstract;
using OlnlineBanking.Infrastructure.Config;

namespace OlnlineBanking.Infrastructure.Concrete
{
    public class ConfigManager: IConfig
    {

        public MailSetting MailSetting
        {
            get
            {
                return ConfigurationManager.GetSection("mailConfig") as MailSetting;
            }
        }

        public PassportSetting PassportSetting
        {
            get
            {
                return ConfigurationManager.GetSection("passportConfig") as PassportSetting;
            }
        }
    }
}