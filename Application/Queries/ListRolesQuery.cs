using MediatR;
using System.Collections.Generic;

namespace Application.Queries
{
    public class ListRolesQuery : IRequest<IEnumerable<string>>
    {
    }
} 