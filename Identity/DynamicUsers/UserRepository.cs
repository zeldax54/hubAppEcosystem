using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Identity.DynamicUsers
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<IdentityUser> userManager;
        // some dummy data. Replce this with your user persistence. 

        public UserRepository(UserManager<IdentityUser> userManager) 
        {
            this.userManager = userManager;        
        }

        public ValidateReturn ValidateCredentials(string username, string password)
        {
            var user = userManager.Users.Where(user => user.NormalizedUserName == username.ToUpper() || user.NormalizedEmail == username.ToUpper()).FirstOrDefault();           
            return new ValidateReturn() 
            {            
            Result = user != null && userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Failed,
            User = user
            };         
        }

        public IdentityUser FindBySubjectId(string subjectId)
        {
            return userManager.Users.FirstOrDefault(x => x.Id == subjectId);
        }

        public IdentityUser FindByUsername(string username)
        {
            return userManager.Users.FirstOrDefault(x => x.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public List<string> GetRoles(IdentityUser user)
        {
             return userManager.GetRolesAsync(user).ConfigureAwait(false).GetAwaiter().GetResult().ToList();
        }
    }
}
