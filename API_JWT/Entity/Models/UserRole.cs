using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models
{
    public class UserRole
    {
        public int Id { get; set; }

        //Relaciones
        public int UserId { get; set; } = default!;
        public User User { get; set; } = default!;

        public int RoleId { get; set; } = default!;
        public Role Role { get; set; } = default!;
    }
}
