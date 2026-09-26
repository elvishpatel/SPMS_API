using FluentValidation;
using SPMS_API.Data;
using SPMS_API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SPMS_API.Validators
{
    public class ProjectMasterValidator : AbstractValidator<CreateProjectMaster>
    {
        public readonly AppDbContext _context;

        public ProjectMasterValidator(AppDbContext context)
        {
            _context = context;

            RuleFor(p => p.ProjectTitle)
                .NotEmpty().WithMessage("Project title is required.")
                .MaximumLength(100).WithMessage("Project title cannot exceed 100 characters.")
                .MustAsync(async (projectTitle, cancellation) =>
                 {
                     return !await _context.ProjectMaster
                         .AnyAsync(x => x.ProjectTitle == projectTitle, cancellation);
                 })
                .WithMessage("Project title already exists.");

            RuleFor(p => p.Description)
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters.");

        }
    }

    public class ProjectUpdateDTOValidator : AbstractValidator<UpdateProjectMaster>
    {
        public ProjectUpdateDTOValidator()
        {
            RuleFor(p => p.ProjectTitle)
                .NotEmpty().WithMessage("Project title is required.")
                .MaximumLength(100).WithMessage("Project title cannot exceed 100 characters.");

            RuleFor(p => p.Description)
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters.");
        }
    }
}
