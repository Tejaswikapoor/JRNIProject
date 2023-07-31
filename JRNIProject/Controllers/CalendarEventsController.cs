using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Mvc;

namespace CalendarApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalendarEventsController : ControllerBase
    {
        private static readonly string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        private static readonly string ApplicationName = "JRNI APP";
        private static readonly string ClientId = "449374205863-to5bs1kp4mvhckppispeuom2ntu2j3h8.apps.googleusercontent.com";
        private static readonly string ClientSecret = "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";

        public class EventData
        {
           
            public DateTimeOffset StartTime { get; set; }
            public DateTimeOffset EndTime { get; set; }
            public string Summary { get; set; }
        }

        public class EventStatus
        {
            public string emailId { get; set; }
            public int BusyCount { get; set; }
            public int OutOfOfficeCount { get; set; }
            public List<EventData> Events { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult<EventStatus>> GetBusyAndOutOfOfficeEvents([FromQuery] string email)
        {
            try
            {
                var clientSecrets = new ClientSecrets
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret
                };

                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets,
                    Scopes,
                    email,
                    CancellationToken.None
                );

                var service = new CalendarService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName
                });

                // Call the API to retrieve the user's events
                var events = await service.Events.List("primary")
                    .ExecuteAsync();

                // Filter and extract busy and out-of-office events from the response
                var busyEvents = new List<EventData>();
                var outOfOfficeEvents = new List<EventData>();

                foreach (var e in events.Items)
                {
                    if (!string.IsNullOrEmpty(e.Summary) )
                    {
                        var eventData = new EventData
                        {
                            StartTime = e.Start.DateTime ?? DateTimeOffset.Parse(e.Start.Date),
                            EndTime = e.End.DateTime ?? DateTimeOffset.Parse(e.End.Date),
                            Summary = e.Summary
                        };

                        if (e.Transparency == "opaque"|| (e.Summary.ToLower().Contains("busy")))
                        {
                            busyEvents.Add(eventData);
                        }
                        else if (e.Transparency == "outofoffice" || e.Summary.ToLower().Contains("ooo")|| e.Summary.ToLower().Contains("OOO"))
                        { 
                            outOfOfficeEvents.Add(eventData);
                        }
                    }
                }

                var eventStatus = new EventStatus
                {
                    emailId = email,
                    BusyCount = busyEvents.Count,
                    OutOfOfficeCount = outOfOfficeEvents.Count,
                    Events = busyEvents.Concat(outOfOfficeEvents).ToList()
                };

                return Ok(eventStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
