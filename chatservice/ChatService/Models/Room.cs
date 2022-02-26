using System;
using System.Collections.Generic;

#nullable disable

namespace ChatService.Models
{
    public partial class Room
    {
        public Room()
        {
            UserRooms = new HashSet<UserRoom>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UserRoom> UserRooms { get; set; }
    }
}
