using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventWebApp.Models;
using EventWebApp.Service;
using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging;

namespace EventWebApp.Controllers
{
    [Produces("application/json")]
    [ApiVersion("1")]
    [Route("api/v{ver:apiVersion}/Events")]
    public class EventsControllerV1 : Controller
    {
        private readonly IDaoService<Event, EventRequestFilter> _service;
        private readonly ILogger _logger;

        public EventsControllerV1(
            IDaoService<Event, EventRequestFilter> service,
            ILoggerFactory logger
        )
        {
            _logger = logger.CreateLogger("EventWebApp.Controllers.EventsController");
            _service = service;
        }

        // PAGE: api/Events/page?page=0&size=5
        [HttpGet]
        [Route("page")]
        public PageResponse<Event> GetEventPage([FromQuery] int page, [FromQuery] int size)
        {
            _logger.LogInformation(LoggingEvents.GetPage, "Getting page {PAGE}", page);

            string filterString = HttpContext.Request.Query["filter"];
            EventRequestFilter filter = JsonConvert.DeserializeObject<EventRequestFilter>(filterString);
            return _service.GetPage(page, size, filter);
        }

        // GET: api/Events
        [HttpGet]
        public IEnumerable<Event> GetEvent()
        {
            _logger.LogInformation(LoggingEvents.ListItems, "Getting list");

            return _service.GetAll();
        }

        // GET: api/Events/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation(LoggingEvents.ItemNotValid, "Item is not valid");

                return BadRequest(ModelState);
            }

            var @event = await _service.GetById(id);

            if (@event == null)
            {
                _logger.LogInformation(LoggingEvents.GetItemNotFound, "Item not exists");

                return NotFound();
            }

            return Ok(@event);
        }

        // PUT: api/Events/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent([FromRoute] string id, [FromBody] Event @event)
        {
            if (!ModelState.IsValid || !_service.Validate(@event))
            {
                _logger.LogInformation(LoggingEvents.ItemNotValid, "Item is not valid");

                return BadRequest(ModelState);
            }
            CorrectUserTime(@event);
            if (id != @event.Id)
            {
                _logger.LogInformation(LoggingEvents.ItemNotValid, "Item id mismatch");

                return BadRequest();
            }

            _service.Edit(@event);

            try
            {
                await _service.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_service.EventExists(id))
                {
                    _logger.LogInformation(LoggingEvents.UpdateItemNotFound, "Item id mismatch");

                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Events
        [HttpPost]
        public async Task<IActionResult> PostEvent([FromBody] Event @event)
        {
            if (!ModelState.IsValid || !_service.Validate(@event))
            {
                _logger.LogInformation(LoggingEvents.ItemNotValid, "Item is not valid");

                return BadRequest(ModelState);
            }
            CorrectUserTime(@event);
            _service.Create(@event);
            await _service.Save();

            return CreatedAtAction("GetEvent", new { id = @event.Id }, @event);
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation(LoggingEvents.ItemNotValid, "Item is not valid");

                return BadRequest(ModelState);
            }

            var @event = await _service.GetById(id);
            if (@event == null)
            {
                _logger.LogInformation(LoggingEvents.GetItemNotFound, "Item not exists");

                return NotFound();
            }

            _service.Delete(@event);
            await _service.Save();

            return Ok(@event);
        }

        private void CorrectUserTime(Event e)
        {
            var offset = 0;
            IHeaderDictionary dictionary = HttpContext.Request.Headers;
            foreach (KeyValuePair<string, StringValues> item in dictionary)
            {
                if (item.Key == "TimeZoneOffset") {
                    offset = int.Parse(item.Value);
                }
            }
            if (offset != 0) {
                TimeSpan tsOffset = TimeSpan.FromMinutes(-offset);
                //FromDate                
                e.FromDate = new DateTimeOffset(e.FromDate).ToOffset(tsOffset).DateTime;
                //ToDate
                e.FromDate = new DateTimeOffset(e.ToDate).ToOffset(tsOffset).DateTime;
            }
        }
    }

}