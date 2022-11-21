using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectman.Models
{
    public class TableReplyVM<ListType>
    {
        public TableReplyVM( int total, int totalNotFiltered, IEnumerable<ListType> rows)
        {
            this.total = total;
            this.totalNotFiltered = totalNotFiltered;
            this.rows = rows;
        }

        public int total { get; set; }
        public int totalNotFiltered { get; set; }
        public IEnumerable<ListType> rows { get; set; }
    }
}
