using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OlnlineBanking.Infrastructure.Config;

namespace OlnlineBanking.Infrastructure.Abstract
{
    public interface IConfig
    {
        MailSetting MailSetting { get; }
        PassportSetting PassportSetting { get;  }
    }
}
