using IdentityModel;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.DynamicUsers
{
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserRepository _userRepository;
 
        public CustomResourceOwnerPasswordValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
 
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var validate = _userRepository.ValidateCredentials(context.UserName, context.Password);
            if (validate.Result)
            {               
                context.Result = new GrantValidationResult(validate.User.Id, OidcConstants.AuthenticationMethods.Password);
            }
 
            return Task.FromResult(0);
        }
    }
}
