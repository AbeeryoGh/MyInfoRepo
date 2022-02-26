using System;
using System.Collections.Generic;

#nullable disable

namespace ChatService.Models
{
    public partial class User
    {
        public User()
        {
            UserRooms = new HashSet<UserRoom>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Pass { get; set; }
        public string CurrentRoom { get; set; }

        public virtual ICollection<UserRoom> UserRooms { get; set; }
    }
}
