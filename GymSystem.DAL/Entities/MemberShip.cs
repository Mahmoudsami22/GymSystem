using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GymSystem.DAL.Entities
{
    public class MemberShip:BaseEntity
    {
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;

        public int PlanId { get; set; }
        public Plan Plan { get; set; } = null!;
        public DateTime EndDate { get; set; }

        [NotMapped]
        public string Status => ISActive ? "Active" : "Expired";

        [NotMapped]
        public bool ISActive => EndDate > DateTime.Now;
    }
}
