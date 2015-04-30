using OlnlineBanking.Infrastructure.Config;

namespace OlnlineBanking.Infrastructure.Abstract
{
    public interface IConfig
    {
        MailSetting MailSetting { get; }
        PassportSetting PassportSetting { get;  }
    }
}
