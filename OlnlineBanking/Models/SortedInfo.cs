using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OlnlineBanking.Models
{
    public enum Orderring
    {
        Asc,
        Desc
    }; 

    public class SortedInfo
    {
        public string SortedField { get; set; }
        public Orderring SortedOrder { get; set; }
    }
}