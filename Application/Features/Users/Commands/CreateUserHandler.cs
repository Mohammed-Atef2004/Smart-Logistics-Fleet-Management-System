//using Application.Interfaces;
//using Domain.Interfaces.Repositories;
//using Domain.Users;
//using MediatR;

//namespace Application.Features.Users.Commands
//{
//    public class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
//    {
//        private readonly IUserRepository _userRepository;

//        public CreateUserHandler(IUserRepository userRepository)
//        {
//            _userRepository = userRepository;
//        }

//        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
//        {
//            // 1. إنشاء المستخدم
//            var user = new ApplicationUser(request.FullName, request.Email, request.Password);

//            // 2. إضافة أي roles إذا موجودة
//            if (request.RoleIds != null)
//            {
//                foreach (var roleId in request.RoleIds)
//                {
//                    var role = await _userRepository.GetRoleByIdAsync(roleId, cancellationToken);
//                    user.AssignRole(role);
//                }
//            }

//            await _userRepository.AddAsync(user, cancellationToken);

//            return user.Id;
//        }
//    }
//}
