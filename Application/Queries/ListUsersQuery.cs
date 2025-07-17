using MediatR;
using System.Collections.Generic;
using Domain.Entities;

namespace Application.Queries
{
    public class ListUsersQuery : IRequest<IEnumerable<ApplicationUser>>
    {
    }
} 