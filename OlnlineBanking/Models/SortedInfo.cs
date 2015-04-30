namespace OlnlineBanking.Models
{
    public enum Ordering
    {
        Asc,
        Desc
    }; 

    public class SortedInfo
    {
        public string SortedField { get; set; }
        public Ordering SortedOrder { get; set; }
    }
}