using Domain.Common;
using Domain.Fleet;
using Domain.Users.Events;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Users
{

    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; private set; }

        public ICollection<Role> UserRoles { get; private set; } = new List<Role>();
        private readonly List<DomainEvent> _domainEvents = new();
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public Driver? DriverProfile { get; private set; }

        public ApplicationUser(string fullName, string email)
        {
            FullName = fullName;
            Email = email;

            AddDomainEvent(new UserCreatedEvent(this.Id));
        }

        public void AssignRole(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            var userRole = new Role(role.Name, role.Description)
            {
                Description = role.Description
            };
            UserRoles.Add(userRole);

            AddDomainEvent(new UserRoleAssignedEvent(this.Id, role.Id));
        }

        public void ChangePassword(string newPassword)
        {
            PasswordHash = newPassword; 
            AddDomainEvent(new UserPasswordChangedEvent(this.Id));
        }

        private void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents() => _domainEvents.Clear();
    }

}
