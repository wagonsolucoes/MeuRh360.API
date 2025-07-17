using MediatR;

namespace Application.Commands
{
    public class UpdateUserCommand : IRequest<bool>
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
    }
} 