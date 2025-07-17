using MediatR;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.Handlers
{
    public class ListRolesQueryHandler : IRequestHandler<ListRolesQuery, IEnumerable<string>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public ListRolesQueryHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<string>> Handle(ListRolesQuery request, CancellationToken cancellationToken)
        {
            // Retorna todos os nomes dos roles
            return _roleManager.Roles.Select(r => r.Name).ToList();
        }
    }
} 