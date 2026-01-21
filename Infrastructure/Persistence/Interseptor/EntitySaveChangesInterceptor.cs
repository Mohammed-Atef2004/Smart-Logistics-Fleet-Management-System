using Domain.Common;
using Domain.Users.Interfaces.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors
{
    public class EntitySaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IPublisher _publisher;
        private readonly ICurrentUserService _currentUserService;

        public EntitySaveChangesInterceptor(IPublisher publisher, ICurrentUserService currentUserService)
        {
            _publisher = publisher;
            _currentUserService = currentUserService;
        }

        // للهندلة في حالة الـ Synchronous (نادراً ما تُستخدم لكن للأمان)
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        // للهندلة في حالة الـ Async (وهي الأساس في شغلنا)
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);
            await DispatchDomainEvents(eventData.Context);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateEntities(DbContext? context)
        {
            if (context == null) return;

            var currentUserId = _currentUserService.UserId ?? "System";

            foreach (var entry in context.ChangeTracker.Entries<IAudiatable>())
            {
                if (entry.State == EntityState.Added)
                {
                    // استخدمنا الـ ID الحقيقي بدل الكلمة الثابتة
                    entry.Entity.SetCreated(currentUserId);
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.SetUpdated(currentUserId);
                }
            }

            // التعامل مع الـ Soft Delete
            foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified; 
                    entry.Entity.Delete();
                }
            }
        }

        private async Task DispatchDomainEvents(DbContext? context)
        {
            if (context == null) return;

            var entities = context.ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            foreach (var entity in entities)
            {
                var events = entity.DomainEvents.ToList();
                entity.ClearDomainEvents(); 

                foreach (var domainEvent in events)
                {
                    await _publisher.Publish(domainEvent); 
                }
            }
        }
    }
}