
using Dawn;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        private ApplicationUser() {}
        public ApplicationUser(string firstName, string lastName, string email)
        {
            FirstName = Guard.Argument(firstName, nameof(firstName))
                .NotNull()
                .NotWhiteSpace();
            
            LastName = Guard.Argument(lastName, nameof(lastName))
                .NotNull()
                .NotWhiteSpace();
            
            Email = Guard.Argument(email, nameof(email))
                .NotNull()
                .NotWhiteSpace();
                
            UserName = email;
        }

    }
}