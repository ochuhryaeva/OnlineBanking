using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

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