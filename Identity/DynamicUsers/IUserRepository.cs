using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.DynamicUsers
{
    public interface IUserRepository
    {
        ValidateReturn ValidateCredentials(string username, string password);

        IdentityUser FindBySubjectId(string subjectId);

        IdentityUser FindByUsername(string username);

        List<string> GetRoles(IdentityUser user);
    }

    public class ValidateReturn{

        public bool Result { get; set; }
        public IdentityUser User { get; set; }

    }
}
