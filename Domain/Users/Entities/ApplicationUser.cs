using Domain.Common;
using Domain.Fleet;
using Domain.Users.Events;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Users.Entities
{

    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; private set; }

        private readonly List<DomainEvent> _domainEvents = new();
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        private ApplicationUser() { } // EF Core

        public ApplicationUser(string fullName, string email)
        {
            FullName = fullName;
            Email = email;

            AddDomainEvent(new UserCreatedEvent(Id));
        }

        //public void AssignRole(Role role)
        //{
        //    if (role == null) throw new ArgumentNullException(nameof(role));

        //    var userRole = new Role(role.Name, role.Description)
        //    {
        //        Description = role.Description
        //    };
        //    UserRoles.Add(userRole);

        //    AddDomainEvent(new UserRoleAssignedEvent(Id, role.Id));
        //}

        public void ChangePassword(string newPassword)
        {
            PasswordHash = newPassword; 
            AddDomainEvent(new UserPasswordChangedEvent(Id));
        }

        private void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents() => _domainEvents.Clear();
    }

}
