using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Application.Commands.Handlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);
            if (user == null)
                return false;

            user.Email = request.Email;
            user.UserName = request.Email;
            user.PhoneNumber = request.PhoneNumber;

            // Atualizar role se informado
            if (!string.IsNullOrEmpty(request.Role))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (!currentRoles.Contains(request.Role))
                {
                    // Remove todas as roles antigas e adiciona a nova
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!await _userManager.IsInRoleAsync(user, request.Role))
                    {
                        await _userManager.AddToRoleAsync(user, request.Role);
                    }
                }
            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
} 