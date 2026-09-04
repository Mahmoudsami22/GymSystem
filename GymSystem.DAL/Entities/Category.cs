using System;
using System.Collections.Generic;
using System.Text;

namespace GymSystem.DAL.Entities
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; } = null!;
        public ICollection<Session> Sessions { get; set; }  = new HashSet<Session>();
    }
}
