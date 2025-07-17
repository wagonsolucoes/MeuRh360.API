using FluentValidation;

namespace Application.Queries.Validators
{
    public class ListUsersQueryValidator : AbstractValidator<ListUsersQuery>
    {
        public ListUsersQueryValidator()
        {
            // Nenhuma regra, mas estrutura pronta para futuras validações
        }
    }
} 