using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<Sensor> Sensors { get; set; }
    }
}
