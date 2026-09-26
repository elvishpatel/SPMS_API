using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SPMS_API.Data;
using SPMS_API.DTOs;

namespace SPMS_API.Validators
{
    public class TaskPriorityCreateDTOValidator : AbstractValidator<CreateTaskPriority>
    {
        private readonly AppDbContext _context;

        public TaskPriorityCreateDTOValidator(AppDbContext context)
        {
            _context = context;

            RuleFor(tp => tp.TaskPriorityName)
                .NotEmpty().WithMessage("Task priority name is required.")
                .MaximumLength(20).WithMessage("Task priority name cannot exceed 20 characters.")
                .MustAsync(async (name, cancellation) =>
                {
                    return !await _context.TaskPriority
                        .AnyAsync(x => x.TaskPriorityName == name, cancellation);
                })
                .WithMessage("Task priority name already exists.");

            RuleFor(tp => tp.TaskPriorityCssClass)
                .NotEmpty().WithMessage("Task priority CSS class is required.")
                .MaximumLength(20).WithMessage("Task priority CSS class cannot exceed 20 characters.");
        }
    }

    public class TaskPriorityUpdateDTOValidator : AbstractValidator<UpdateTaskPriority>
    {
        public TaskPriorityUpdateDTOValidator()
        {
            RuleFor(tp => tp.TaskPriorityName)
                .NotEmpty().WithMessage("Task priority name is required.")
                .MaximumLength(20).WithMessage("Task priority name cannot exceed 20 characters.");

            RuleFor(tp => tp.TaskPriorityCssClass)
                .NotEmpty().WithMessage("Task priority CSS class is required.")
                .MaximumLength(20).WithMessage("Task priority CSS class cannot exceed 20 characters.");
        }
    }
}
