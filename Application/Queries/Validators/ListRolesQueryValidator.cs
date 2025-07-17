using FluentValidation;

namespace Application.Queries.Validators
{
    public class ListRolesQueryValidator : AbstractValidator<ListRolesQuery>
    {
        public ListRolesQueryValidator()
        {
            // Nenhuma regra, mas estrutura pronta para futuras validações
        }
    }
} 