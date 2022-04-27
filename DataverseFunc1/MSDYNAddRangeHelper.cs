using System;
using Microsoft.Xrm.Sdk.Query;

namespace DataverseFunc1
{
    public static partial class MSDYNAddRange
    {
        public static QueryExpression GetQuery(DateTime Start, DateTime End)
        {
            var condition1 = new ConditionExpression
            {
                AttributeName = "msdyn_start",
                Operator = ConditionOperator.GreaterEqual
            };
            condition1.Values.Add(Start);
            var filter1 = new FilterExpression();
            filter1.Conditions.Add(condition1);

            var condition2 = new ConditionExpression
            {
                AttributeName = "msdyn_start",
                Operator = ConditionOperator.LessEqual
            };
            condition2.Values.Add(End);
            var filter2 = new FilterExpression();
            filter2.Conditions.Add(condition2);

            var query = new QueryExpression("msdyn_timeentry");
            query.ColumnSet.AddColumns("msdyn_start", "msdyn_end");
            query.Criteria.AddFilter(filter1);
            query.Criteria.AddFilter(filter2);
            return query;
        }
    }
}
