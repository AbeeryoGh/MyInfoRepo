using System;
using System.Collections.Generic;

#nullable disable

namespace ChatService.Models
{
    public partial class UserRoom
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public long? RoomId { get; set; }

        public virtual Room Room { get; set; }
        public virtual User User { get; set; }
    }
}
