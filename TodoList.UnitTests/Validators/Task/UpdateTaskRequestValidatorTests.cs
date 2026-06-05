using TodoList.API.Validation.Task;

namespace TodoList.UnitTests.Validators.Task;

public class UpdateTaskRequestValidatorTests
{
    private readonly UpdateTaskRequestValidator _sut = new();

    private static UpdateTaskRequest ValidRequest => new(
        "Updated Title",
        "Description",
        DateTime.UtcNow.AddDays(1),
        2,
        null,
        new List<UpdateSubTaskRequest>(),
        new List<Guid>());

    [Fact]
    public void ValidTask_PassesValidation()
    {
        var result = _sut.TestValidate(ValidRequest);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyTitle_FailsValidation()
    {
        var request = ValidRequest with { Title = "" };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void TitleExceedsMaxLength_FailsValidation()
    {
        var request = ValidRequest with { Title = new string('a', 201) };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void TitleAtMaxLength_PassesValidation()
    {
        var request = ValidRequest with { Title = new string('a', 200) };
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void DescriptionExceedsMaxLength_FailsValidation()
    {
        var request = ValidRequest with { Description = new string('a', 1001) };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void DescriptionAtMaxLength_PassesValidation()
    {
        var request = ValidRequest with { Description = new string('a', 1000) };
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void NullDescription_PassesValidation()
    {
        var request = ValidRequest with { Description = null };
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void StatusBelowRange_FailsValidation()
    {
        var request = ValidRequest with { Status = 0 };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void StatusAboveRange_FailsValidation()
    {
        var request = ValidRequest with { Status = 6 };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void StatusAtBoundaries_PassesValidation()
    {
        Assert.Multiple(() =>
        {
            var r1 = _sut.TestValidate(ValidRequest with { Status = 1 });
            r1.ShouldNotHaveAnyValidationErrors();

            var r5 = _sut.TestValidate(ValidRequest with { Status = 5 });
            r5.ShouldNotHaveAnyValidationErrors();
        });
    }

    [Fact]
    public void EmptySubTaskTitle_FailsValidation()
    {
        var request = ValidRequest with
        {
            SubTasks = new List<UpdateSubTaskRequest>
            {
                new() { Title = "" }
            }
        };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor("SubTasks[0].Title");
    }

    [Fact]
    public void ValidSubTaskTitle_PassesValidation()
    {
        var request = ValidRequest with
        {
            SubTasks = new List<UpdateSubTaskRequest>
            {
                new() { Title = "Valid subtask" }
            }
        };
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptySubTasksList_PassesValidation()
    {
        var request = ValidRequest with
        {
            SubTasks = new List<UpdateSubTaskRequest>()
        };
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void PastDueDate_FailsValidation()
    {
        var request = ValidRequest with { DueDate = DateTime.UtcNow.AddDays(-1) };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.DueDate);
    }

    [Fact]
    public void NullDueDate_PassesValidation()
    {
        var request = ValidRequest with { DueDate = null };
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void FutureDueDate_PassesValidation()
    {
        var request = ValidRequest with { DueDate = DateTime.UtcNow.AddDays(365) };
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void WhitespaceTitle_FailsValidation()
    {
        var result = _sut.TestValidate(ValidRequest with { Title = "   " });
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void MultipleSubTasks_WithOneInvalid_FailsOnInvalidOnly()
    {
        var request = ValidRequest with
        {
            SubTasks = new List<UpdateSubTaskRequest>
            {
                new() { Title = "Valid" },
                new() { Title = "" },
                new() { Title = "Also valid" }
            }
        };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor("SubTasks[1].Title");
        result.ShouldNotHaveValidationErrorFor("SubTasks[0].Title");
        result.ShouldNotHaveValidationErrorFor("SubTasks[2].Title");
    }

    [Fact]
    public void CategoryId_Null_PassesValidation()
    {
        var result = _sut.TestValidate(ValidRequest with { CategoryId = null });
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CategoryId_ValidGuid_PassesValidation()
    {
        var result = _sut.TestValidate(ValidRequest with { CategoryId = Guid.NewGuid() });
        result.ShouldNotHaveAnyValidationErrors();
    }
}
