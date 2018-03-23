using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventWebApp.Models
{
    public class PageResponse<T>
    {
        public List<T> List { get; set; }
        public int Count { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }

        public PageResponse(List<T> list, int count, int page, int totalPages)
        {
            List = list;
            Count = count;
            Page = page;
            TotalPages = totalPages;
        }
    }
}
