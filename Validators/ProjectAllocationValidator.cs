using FluentValidation;
using SPMS_API.DTOs;

namespace SPMS_API.Validators
{
    public class ProjectAllocationCreateDTOValidator : AbstractValidator<CreateProjectAllocation>
    {
        public ProjectAllocationCreateDTOValidator()
        {
            RuleFor(pa => pa.ProjectID)
                .GreaterThan(0).WithMessage("Project ID is required.");

            RuleFor(pa => pa.StudentID)
                .GreaterThan(0).WithMessage("Student ID is required.");

            RuleFor(pa => pa.FacultyID)
                .GreaterThan(0).WithMessage("Faculty ID is required.");

            RuleFor(pa => pa.ProjectStartDate)
                .NotEmpty().WithMessage("Project start date is required.");

            RuleFor(pa => pa.ProjectEndDate)
                .NotEmpty().WithMessage("Project end date is required.")
                .GreaterThanOrEqualTo(pa => pa.ProjectStartDate).WithMessage("Project end date must be on or after start date.");

            RuleFor(pa => pa.ProgressPercentage)
                .InclusiveBetween(0, 100).WithMessage("Progress percentage must be between 0 and 100.");

            RuleFor(pa => pa.OverAllGrade)
                .Matches("^[ABC]?$").WithMessage("Grade must be A, B, or C.");
        }
    }

    public class ProjectAllocationUpdateDTOValidator : AbstractValidator<UpdateProjectAllocation>
    {
        public ProjectAllocationUpdateDTOValidator()
        {
            RuleFor(pa => pa.ProjectID)
                .GreaterThan(0).WithMessage("Project ID is required.");

            RuleFor(pa => pa.StudentID)
                .GreaterThan(0).WithMessage("Student ID is required.");

            RuleFor(pa => pa.FacultyID)
                .GreaterThan(0).WithMessage("Faculty ID is required.");

            RuleFor(pa => pa.ProjectStartDate)
                .NotEmpty().WithMessage("Project start date is required.");

            RuleFor(pa => pa.ProjectEndDate)
                .NotEmpty().WithMessage("Project end date is required.")
                .GreaterThanOrEqualTo(pa => pa.ProjectStartDate).WithMessage("Project end date must be on or after start date.");

            RuleFor(pa => pa.ProgressPercentage)
                .InclusiveBetween(0, 100).WithMessage("Progress percentage must be between 0 and 100.");

            RuleFor(pa => pa.OverAllGrade)
                .Matches("^[ABC]?$").WithMessage("Grade must be A, B, or C.");
        }
    }
}
