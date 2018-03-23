using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventWebApp.Models.Filter;
using Microsoft.Extensions.Primitives;

namespace EventWebApp.Models
{
    public class EventRequestFilter : IRequestFilter
    {
        public String sortAttribute { get; set; }
        public Boolean sortDirection { get; set; }
        public String currentOrFuture { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }

        public static implicit operator EventRequestFilter(StringValues v)
        {
            throw new NotImplementedException();
        }
    }
}
