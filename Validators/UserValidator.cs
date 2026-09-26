using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SPMS_API.Data;
using SPMS_API.DTOs;

namespace SPMS_API.Validators
{
    public class UserCreateDTOValidator : AbstractValidator<CreateUser>
    {
        private readonly AppDbContext _context;

        public UserCreateDTOValidator(AppDbContext context)
        {
            _context = context;

            RuleFor(u => u.UserTypeId)
                .GreaterThan(0).WithMessage("User type ID is required.");

            RuleFor(u => u.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(250).WithMessage("Full name cannot exceed 250 characters.");

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(150).WithMessage("Email cannot exceed 150 characters.")
                .MustAsync(async (email, cancellation) =>
                {
                    return !await _context.User
                        .AnyAsync(x => x.Email == email, cancellation);
                })
                .WithMessage("Email already exists.");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                .MaximumLength(255).WithMessage("Password cannot exceed 255 characters.");

            RuleFor(u => u.MobileNumber)
                .MaximumLength(15).WithMessage("Mobile number cannot exceed 15 characters.");
        }
    }

    public class UserUpdateDTOValidator : AbstractValidator<UpdateUser>
    {
        public UserUpdateDTOValidator()
        {
            RuleFor(u => u.UserTypeId)
                .GreaterThan(0).WithMessage("User type ID is required.");

            RuleFor(u => u.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(250).WithMessage("Full name cannot exceed 250 characters.");

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(150).WithMessage("Email cannot exceed 150 characters.");

            RuleFor(u => u.MobileNumber)
                .MaximumLength(15).WithMessage("Mobile number cannot exceed 15 characters.");
        }
    }
}
