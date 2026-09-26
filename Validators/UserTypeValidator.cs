using FluentValidation;
using SPMS_API.DTOs;

namespace SPMS_API.Validators
{
    public class UserTypeValidator : AbstractValidator<CreateUserType>
    {
        public UserTypeValidator()
        {
            RuleFor(userType => userType.UserTypeName)
                .NotEmpty().WithMessage("User type name is required.")
                .MaximumLength(50).WithMessage("User type name cannot exceed 50 characters.");

            RuleFor(userType => userType.Description)
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters.");
        }
    }

    public class UserTypeUpdateValidator : AbstractValidator<UpdateUserType>
    {
        public UserTypeUpdateValidator()
        {
            RuleFor(userType => userType.UserTypeName)
                .NotEmpty().WithMessage("User type name is required.")
                .MaximumLength(50).WithMessage("User type name cannot exceed 50 characters.");

            RuleFor(userType => userType.Description)
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters.");
        }
    }
}
