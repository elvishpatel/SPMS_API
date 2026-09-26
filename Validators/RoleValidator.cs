using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SPMS_API.Data;
using SPMS_API.DTOs;

namespace SPMS_API.Validators
{
    public class RoleCreateDTOValidator : AbstractValidator<CreateRole>
    {
        public readonly AppDbContext _context;

        public RoleCreateDTOValidator(AppDbContext context)
        {
            _context = context;

            RuleFor(role => role.RoleName)
                .NotEmpty()
                .WithMessage("Role name is required.")
                .MaximumLength(50)
                .WithMessage("Role name cannot exceed 50 characters.")
                .MustAsync(async (roleName, cancellation) =>
                 {
                     return !await _context.Role
                         .AnyAsync(x => x.RoleName == roleName ,cancellation);
                 })
                .WithMessage("Role name already exists.");

            RuleFor(role => role.Description)
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters.");
        }
    }

    public class RoleUpdateDTOValidator : AbstractValidator<UpdateRole>
    {
        public RoleUpdateDTOValidator()
        {
            RuleFor(role => role.RoleName)
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(50).WithMessage("Role name cannot exceed 50 characters.");

            RuleFor(role => role.Description)
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters.");
        }
    }
}
