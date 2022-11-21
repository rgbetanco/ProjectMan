using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSHelper.Extensions;

namespace projectman.Models
{
    [BindProperties]
    public class QueryVM
    {
        public int? limit { get; set; }

        public int? offset { get; set; }
        public string search { get; set; }
        public string searchPhone { get; set; }
        public long? company_id { get; set; }
        public ProjectStatus? status { get; set; }
        public ProjectType? service_type { get; set; }

        public string sort { get; set; }

        public string order { get; set; }


        public IQueryable<T> Apply<T>( IQueryable<T> query, IDictionary<string,string> fieldMap = null )
        {

            // apply sorting / offsets / limits
            if (!String.IsNullOrWhiteSpace(sort))
            {
                string sort = this.sort;
                if (fieldMap != null)
                {
                    if (!fieldMap.TryGetValue(sort, out sort))
                        return null;
                }

                query = query.OrderBy(sort, order != null && order == "desc");
            }

            if (offset != null)
                query = query.Skip(offset.Value);

            if (limit != null)
                query = query.Take(limit.Value);

            return query;
        }
    }
}
