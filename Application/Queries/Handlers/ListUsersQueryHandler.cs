using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.Handlers
{
    public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, IEnumerable<ApplicationUser>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ListUsersQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<ApplicationUser>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
        {
            // Retorna todos os usuários
            return _userManager.Users;
        }
    }
} 