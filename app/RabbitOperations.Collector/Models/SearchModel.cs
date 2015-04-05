namespace RabbitOperations.Collector.Models
{
    public class SearchModel
    {
        public SearchModel()
        {
            SearchString = "";
            Page = 0;
            Take = 10;
            SortField = "TimeSent";
            SortAscending = false;
        }

        public string SearchString { get; set; }
        public int Page { get; set; }
        public int Take { get; set; }
        public string SortField { get; set; }
        public bool SortAscending { get; set; }

        public string RavenSearchString
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SearchString) || SearchString == "undefined")
                {
                    return "";
                }

                return SearchString;
            }
        }

        public string RavenSort
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SortField))
                {
                    return string.Format("{0}{1}", SortAscending ? "+" : "-", SortField);
                }

                return "-TimeSent";
            }
        }
    }
}