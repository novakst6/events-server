using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventWebApp.Models
{
    public class LoggingEvents
    {
        public const int GetPage = 1000;
        public const int ListItems = 1001;
        public const int GetItem = 1002;
        public const int InsertItem = 1003;
        public const int UpdateItem = 1004;
        public const int DeleteItem = 1005;

        public const int GetItemNotFound = 4000;
        public const int UpdateItemNotFound = 4001;

        public const int ItemNotValid = 5001;
        public const int ItemTimeIntervalNotValid = 5002;

        //Debug
        public const int Filtering = 100001;
        public const int Sorting = 100002;
        public const int PageResult = 100003;

    }
}
