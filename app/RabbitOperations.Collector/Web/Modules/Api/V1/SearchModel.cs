namespace RabbitOperations.Collector.Web.Modules.Api.V1
{
    public class SearchModel
    {
        public SearchModel()
        {
            SearchString = "";
            Page = 0;
            Take = 20;
        }

        public string SearchString { get; set; }
        public int Page { get; set; }
        public int Take { get; set; }
    }
}