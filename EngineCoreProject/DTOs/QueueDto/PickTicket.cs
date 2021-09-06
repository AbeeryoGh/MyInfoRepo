using System;
using System.Collections.Generic;


namespace EngineCoreProject.DTOs.QueueDto
{
    public class PickTicket
    {
        public string TicketId { get; set; }
        public DateTime ExpectDateTime { get; set; }

        public Dictionary<int, int> WorkingHours { get; set; }
        public PickTicket()
        {
            ExpectDateTime = DateTime.Now;
            WorkingHours = new Dictionary<int, int>();
        }

    }
}
