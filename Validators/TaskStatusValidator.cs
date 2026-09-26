using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SPMS_API.Data;
using SPMS_API.DTOs;

namespace SPMS_API.Validators
{
    public class TaskStatusCreateDTOValidator : AbstractValidator<CreateTaskStatus>
    {
        private readonly AppDbContext _context;

        public TaskStatusCreateDTOValidator(AppDbContext context)
        {
            _context = context;

            RuleFor(ts => ts.TaskStatusName)
                .NotEmpty().WithMessage("Task status name is required.")
                .MaximumLength(20).WithMessage("Task status name cannot exceed 20 characters.")
                .MustAsync(async (name, cancellation) =>
                {
                    return !await _context.TaskStatus
                        .AnyAsync(x => x.TaskStatusName == name, cancellation);
                })
                .WithMessage("Task status name already exists.");

            RuleFor(ts => ts.TaskStatusCssClass)
                .NotEmpty().WithMessage("Task status CSS class is required.")
                .MaximumLength(250).WithMessage("Task status CSS class cannot exceed 250 characters.");
        }
    }

    public class TaskStatusUpdateDTOValidator : AbstractValidator<UpdateTaskStatus>
    {
        public TaskStatusUpdateDTOValidator()
        {
            RuleFor(ts => ts.TaskStatusName)
                .NotEmpty().WithMessage("Task status name is required.")
                .MaximumLength(20).WithMessage("Task status name cannot exceed 20 characters.");

            RuleFor(ts => ts.TaskStatusCssClass)
                .NotEmpty().WithMessage("Task status CSS class is required.")
                .MaximumLength(250).WithMessage("Task status CSS class cannot exceed 250 characters.");
        }
    }
}
