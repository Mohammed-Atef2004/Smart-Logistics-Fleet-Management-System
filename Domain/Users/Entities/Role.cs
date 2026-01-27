using Domain.Common;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Users.Entities
{
    public class Role:IdentityRole<Guid>, IAudiatable
    {
        public string Description { get; set; }

       public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? CreatedBy { get; set; }

       public  string? UpdatedBy { get; set; }

        public Role(string name,string description):base(name)
        {
            Description= description;
        }
        public void SetCreated(string user)
        {
            CreatedAt= DateTime.UtcNow;
            CreatedBy= user;
        }

        public void SetUpdated(string UpdatedBy)
        {
            UpdatedAt= DateTime.UtcNow;
            this.UpdatedBy= UpdatedBy;
        }
        private Role() : base() { }// For EF Core
    }
}
