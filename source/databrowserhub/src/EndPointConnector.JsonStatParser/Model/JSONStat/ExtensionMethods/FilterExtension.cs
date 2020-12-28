using System.Collections.Generic;
using EndPointConnector.Models;

namespace EndPointConnector.JsonStatParser.Model.JsonStat.ExtensionMethods
{
    public static class FilterExtension
    {

        public static void Filter(this JsonStatDataset jsonDataset, List<FilterCriteria> filters,
            List<Criteria> notDisplayed = null)
        {
            var filter = new JsonStatFilter(jsonDataset, filters, notDisplayed);
            filter.Filter();
        }

    }
}