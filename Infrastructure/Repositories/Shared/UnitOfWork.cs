//using Domain.Driver;
//using Domain.Interfaces.Repositories;
//using Domain.Shipment;
//using Domain.Vehicles.Events;
//using Infrastructure.Persistence.Data;
//using Infrastructure.Repositories;


//namespace Infrastructure.Repositories.Shared
//{
//    public class UnitOfWork : Domain.Interfaces.Repositories.IUnitOfWork
//    {
//        private readonly ApplicationDbContext _context;

//        // بنعرف الحقول كـ private عشان نشيل فيها النسخة لما تتعمل
   
       

//        public UnitOfWork(ApplicationDbContext context)
//        {
//            _context = context ?? throw new ArgumentNullException(nameof(context));
//        }

      

//        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
//        {
           
//            return await _context.SaveChangesAsync(cancellationToken);
//        }

//        public void Dispose()
//        {
//            _context.Dispose();
//            GC.SuppressFinalize(this);
//        }
//    }
//}