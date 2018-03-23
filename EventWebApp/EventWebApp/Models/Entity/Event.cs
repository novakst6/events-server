using System;
using System.Collections.Generic;

namespace EventWebApp.Models
{
    public partial class Event : IEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime Created { get; set; }
    }
}
