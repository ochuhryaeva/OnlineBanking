using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OlnlineBanking.Infrastructure.Concrete
{
    public enum LoginResult
    {
        LrSuccess,
        LrUserNotExist,
        LrWrongPassword,
        LrUserNotActivated,
        LrUserIsBlocked,
        LrError
    }

    public enum RegisterResult
    {
        RrSuccess,
        RrUserAlreadyExist,
        RrError
    }
}