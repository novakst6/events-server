using System;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventWebApp.Service
{
    public class EventDaoService : IDaoService<Event, EventRequestFilter>
    {
        private readonly EventDatabaseContext _context;
        private readonly ILogger _logger;

        public EventDaoService(
            EventDatabaseContext context,
            ILoggerFactory logger
        )
        {
            _logger = logger.CreateLogger("EventWebApp.Service.EventDaoService");
            _context = context;
        }

        public Event Create(Event e)
        {
            _context.Event.Add(e);
            return e;
        }

        public void Delete(Event e)
        {
            _context.Event.Remove(e);
        }

        public bool EventExists(string id)
        {
            return _context.Event.Any(e => e.Id == id);
        }

        public IEnumerable<Event> GetAll()
        {
            return _context.Event;
        }

        public Task<Event> GetById(string id)
        {
            return _context.Event.SingleOrDefaultAsync(m => m.Id == id);
        }

        public PageResponse<Event> GetPage(int page, int size, EventRequestFilter filter)
        {
            IQueryable<Event> events = from e in _context.Event select e;
            //Filtering
            if (filter.currentOrFuture != null && filter.currentOrFuture == "yes") {
                _logger.LogDebug(LoggingEvents.Filtering, "Filter current and future events");
                events = events.Where(e => e.FromDate >= System.DateTime.Now);
            }
            if (filter.fromDate != null && filter.toDate != null)
            {
                if (filter.fromDate > DateTime.MinValue && filter.toDate < DateTime.MaxValue)
                {
                    _logger.LogDebug(LoggingEvents.Filtering, "Filter interval from {IN1} to {IN2}", filter.fromDate, filter.toDate);
                    events = events.Where(e => e.FromDate >= filter.fromDate && e.ToDate <= filter.toDate);
                }
            }
            //Sorting
            if (filter.sortAttribute != null) {
                _logger.LogDebug(LoggingEvents.Sorting, "Try to sort results by {ATTR} ASC {ASC}", filter.sortAttribute, filter.sortDirection);
                var attr = typeof(Event).GetProperty(filter.sortAttribute);
                if (attr != null)
                {
                    if (filter.sortDirection)
                    {
                            events = events.OrderBy(x => attr.GetValue(x, null));
                    }
                    else
                    {
                            events = events.OrderByDescending(x => attr.GetValue(x, null));
                    }
                }
                else
                {
                    _logger.LogDebug(LoggingEvents.Sorting, "Sorting attribute {ATTR} not found", filter.sortAttribute);
                }
            }
            
            PaginatedList<Event> Events =  PaginatedList<Event>.Create(
                events.AsNoTracking(), page, size);
            PageResponse<Event> Response = new PageResponse<Event>(Events, Events.TotalCount,
                Events.PageIndex, Events.TotalPages);
            _logger.LogDebug(LoggingEvents.PageResult, "Return results total: {TOTAL} page: {PAGE}", Response.Count, Response.Page);
            return Response;
        }

        public Task<int> Save()
        {
           return _context.SaveChangesAsync();

        }

        public void Edit(Event e)
        {
            _context.Entry(e).State = EntityState.Modified;
        }

        public bool Validate(Event e)
        {
            //It must be interval
            if (e.FromDate == null && e.ToDate != null 
                || e.FromDate != null && e.ToDate == null) {
                _logger.LogInformation(LoggingEvents.ItemTimeIntervalNotValid, "Needed whole time interval");
                return false;
            }
            //It must start before end
            if (e.FromDate != null && e.ToDate != null) {
                if (e.FromDate > e.ToDate) {
                    _logger.LogInformation(LoggingEvents.ItemTimeIntervalNotValid, "'From' time > 'To' time");
                    return false;
                }
            }
            return true;
        }
    }
}
