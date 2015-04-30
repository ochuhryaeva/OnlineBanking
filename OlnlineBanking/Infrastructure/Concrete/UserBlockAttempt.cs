using System.Collections.ObjectModel;

namespace OlnlineBanking.Infrastructure.Concrete
{
    public class UserBlockAttempt
    {
        public string Login { get; set; }
        public int LoginAttemptsCount { get; set; }
    }

    public class UserBlockAttemptCollection : Collection<UserBlockAttempt>
    {
        
    }
}