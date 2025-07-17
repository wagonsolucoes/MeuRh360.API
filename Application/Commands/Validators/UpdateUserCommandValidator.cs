using FluentValidation;

namespace Application.Commands.Validators
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id é obrigatório.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email inválido.");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Telefone é obrigatório.");
            RuleFor(x => x.Role).NotEmpty().When(x => !string.IsNullOrEmpty(x.Role)).WithMessage("Role não pode ser vazio se informado.");
        }
    }
} 