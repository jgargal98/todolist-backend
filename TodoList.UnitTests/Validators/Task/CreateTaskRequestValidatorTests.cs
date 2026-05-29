using TodoList.API.Validation.Task;

namespace TodoList.UnitTests.Validators.Task;

public class CreateTaskRequestValidatorTests
{
    private readonly CreateTaskRequestValidator _sut = new();

    private static CreateTaskRequest ValidRequest => new()
    {
        Title = "Valid Task",
        Description = "Description",
        DueDate = DateTime.UtcNow.AddDays(1),
        Status = 1,
        SubTasks = new List<CreateSubTaskRequest>
        {
            new() { Title = "Subtask" }
        }
    };

    [Fact]
    public void ValidTask_PassesValidation()
    {
        var result = _sut.TestValidate(ValidRequest);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyTitle_FailsValidation()
    {
        var request = ValidRequest;
        request.Title = "";
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void TitleExceedsMaxLength_FailsValidation()
    {
        var request = ValidRequest;
        request.Title = new string('a', 201);
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void TitleAtMaxLength_PassesValidation()
    {
        var request = ValidRequest;
        request.Title = new string('a', 200);
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void DescriptionExceedsMaxLength_FailsValidation()
    {
        var request = ValidRequest;
        request.Description = new string('a', 1001);
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void DescriptionAtMaxLength_PassesValidation()
    {
        var request = ValidRequest;
        request.Description = new string('a', 1000);
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void NullDescription_PassesValidation()
    {
        var request = ValidRequest;
        request.Description = null;
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void StatusBelowRange_FailsValidation()
    {
        var request = ValidRequest;
        request.Status = 0;
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void StatusAboveRange_FailsValidation()
    {
        var request = ValidRequest;
        request.Status = 6;
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void StatusAtUpperBoundary_PassesValidation()
    {
        var request = ValidRequest;
        request.Status = 5;
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void StatusAtLowerBoundary_PassesValidation()
    {
        var request = ValidRequest;
        request.Status = 1;
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void PastDueDate_FailsValidation()
    {
        var request = ValidRequest;
        request.DueDate = DateTime.UtcNow.AddDays(-1);
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.DueDate);
    }

    [Fact]
    public void NullDueDate_PassesValidation()
    {
        var request = ValidRequest;
        request.DueDate = null;
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptySubTaskTitle_FailsValidation()
    {
        var request = ValidRequest;
        request.SubTasks = new List<CreateSubTaskRequest>
        {
            new() { Title = "" }
        };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor("SubTasks[0].Title");
    }

    [Fact]
    public void EmptySubTasksList_PassesValidation()
    {
        var request = ValidRequest;
        request.SubTasks = new List<CreateSubTaskRequest>();
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
