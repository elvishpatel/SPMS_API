using FluentValidation;
using SPMS_API.DTOs;

namespace SPMS_API.Validators
{
    public class UserRoleCreateDTOValidator : AbstractValidator<CreateUserRole>
    {
        public UserRoleCreateDTOValidator()
        {
            RuleFor(ur => ur.RoleId)
                .GreaterThan(0).WithMessage("Role ID is required.");

            RuleFor(ur => ur.UserId)
                .GreaterThan(0).WithMessage("User ID is required.");
        }
    }

    public class UserRoleUpdateDTOValidator : AbstractValidator<UpdateUserRole>
    {
        public UserRoleUpdateDTOValidator()
        {
            RuleFor(ur => ur.RoleId)
                .GreaterThan(0).WithMessage("Role ID is required.");

            RuleFor(ur => ur.UserId)
                .GreaterThan(0).WithMessage("User ID is required.");
        }
    }
}
