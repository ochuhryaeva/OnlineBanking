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