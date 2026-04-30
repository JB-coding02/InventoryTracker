using InventoryTracker.Models;
using Xunit;

namespace InventoryTracker.Tests;

public class ErrorViewModelTests
{
    [Fact]
    public void ErrorViewModelWithRequestId_ShowRequestIdReturnsTrue()
    {
        ErrorViewModel model = new ErrorViewModel
        {
            RequestId = "test-request-id-123"
        };

        Assert.True(model.ShowRequestId);
    }

    [Fact]
    public void ErrorViewModelWithNullRequestId_ShowRequestIdReturnsFalse()
    {
        ErrorViewModel model = new ErrorViewModel
        {
            RequestId = null
        };

        Assert.False(model.ShowRequestId);
    }

    [Fact]
    public void ErrorViewModelWithEmptyRequestId_ShowRequestIdReturnsFalse()
    {
        ErrorViewModel model = new ErrorViewModel
        {
            RequestId = string.Empty
        };

        Assert.False(model.ShowRequestId);
    }

    [Fact]
    public void ErrorViewModelWithWhitespaceRequestId_ShowRequestIdReturnsTrue()
    {
        ErrorViewModel model = new ErrorViewModel
        {
            RequestId = "   "
        };

        Assert.True(model.ShowRequestId);
    }

    [Fact]
    public void ErrorViewModelDefaultConstructor_CreatesValidInstance()
    {
        ErrorViewModel model = new ErrorViewModel();

        Assert.Null(model.RequestId);
        Assert.False(model.ShowRequestId);
    }

    [Fact]
    public void ErrorViewModelRequestIdCanBeSet()
    {
        ErrorViewModel model = new ErrorViewModel();

        model.RequestId = "new-request-id";

        Assert.Equal("new-request-id", model.RequestId);
        Assert.True(model.ShowRequestId);
    }
}
