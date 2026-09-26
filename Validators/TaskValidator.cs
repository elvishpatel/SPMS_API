using FluentValidation;
using SPMS_API.DTOs;

namespace SPMS_API.Validators
{
    public class TaskCreateDTOValidator : AbstractValidator<CreateTask>
    {
        public TaskCreateDTOValidator()
        {
            RuleFor(t => t.ProjectAllocationID)
                .GreaterThan(0).WithMessage("Project allocation ID is required.");

            RuleFor(t => t.TaskTitle)
                .NotEmpty().WithMessage("Task title is required.")
                .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters.");

            RuleFor(t => t.TaskStatusID)
                .GreaterThan(0).WithMessage("Task status ID is required.");

            RuleFor(t => t.TaskPriorityID)
                .GreaterThan(0).WithMessage("Task priority ID is required.");

            RuleFor(t => t.AssignedScore)
                .GreaterThanOrEqualTo(0).WithMessage("Assigned score must be non-negative.");

            RuleFor(t => t.ProgressPercentage)
                .InclusiveBetween(0, 100).WithMessage("Progress percentage must be between 0 and 100.");

            RuleFor(t => t.FacultyRemarks)
                .MaximumLength(500).WithMessage("Faculty remarks cannot exceed 500 characters.");

            RuleFor(t => t.StudentRemarks)
                .MaximumLength(500).WithMessage("Student remarks cannot exceed 500 characters.");
        }
    }

    public class TaskUpdateDTOValidator : AbstractValidator<UpdateTask>
    {
        public TaskUpdateDTOValidator()
        {
            RuleFor(t => t.ProjectAllocationID)
                .GreaterThan(0).WithMessage("Project allocation ID is required.");

            RuleFor(t => t.TaskTitle)
                .NotEmpty().WithMessage("Task title is required.")
                .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters.");

            RuleFor(t => t.TaskStatusID)
                .GreaterThan(0).WithMessage("Task status ID is required.");

            RuleFor(t => t.TaskPriorityID)
                .GreaterThan(0).WithMessage("Task priority ID is required.");

            RuleFor(t => t.AssignedScore)
                .GreaterThanOrEqualTo(0).WithMessage("Assigned score must be non-negative.");

            RuleFor(t => t.ProgressPercentage)
                .InclusiveBetween(0, 100).WithMessage("Progress percentage must be between 0 and 100.");

            RuleFor(t => t.FacultyRemarks)
                .MaximumLength(500).WithMessage("Faculty remarks cannot exceed 500 characters.");

            RuleFor(t => t.StudentRemarks)
                .MaximumLength(500).WithMessage("Student remarks cannot exceed 500 characters.");
        }
    }
}
