
namespace MuniServices.Tests;
// פרויקט הבדיקות 
public sealed class RequestsServiceTests
{
    [Fact]
    public async Task Normalizes_service_authority_status_and_action()
    {
        var service = CreateService(Request("R1", "3", 2, 1));

        var result = await service.GetRequestsAsync("012345678");

        var request = Assert.Single(result.Requests);
        Assert.Equal("שירות בדיקה", request.ServiceName);
        Assert.Equal("עיריית חדרה", request.AuthorityName);
        Assert.Equal("מסמכים נדרשים", request.Status);
        Assert.True(request.ActionRequired);
    }

    [Fact]
    public async Task Unknown_status_uses_fallback_status()
    {
        var service = CreateService(Request("R2", "99", 2, 1));

        var result = await service.GetRequestsAsync("012345678");

        var request = Assert.Single(result.Requests);
        Assert.Equal("בטיפול", request.Status);
        Assert.False(request.ActionRequired);
    }

    [Fact]
    public async Task Open_request_past_sla_is_overdue()
    {
        var service = CreateService(Request("R3", "2", 30, 1));

        var result = await service.GetRequestsAsync("012345678");

        var request = Assert.Single(result.Requests);
        Assert.True(request.IsOverdue);
    }

    [Fact]
    public async Task Final_request_is_not_overdue_even_when_old()
    {
        var service = CreateService(Request("R4", "4", 60, 30));

        var result = await service.GetRequestsAsync("012345678");

        var request = Assert.Single(result.Requests);
        Assert.False(request.IsOverdue);
    }

    [Fact]
    public async Task Form_missing_from_catalog_is_ignored()
    {
        var service = CreateService(
            Request("R5", "1", 2, 1, formCode: "UNKNOWN_FORM"));

        var result = await service.GetRequestsAsync("012345678");

        Assert.Empty(result.Requests);
    }

    [Fact]
    public async Task Action_required_requests_are_sorted_before_other_requests()
    {
        var service = CreateService(
            Request("ACTION", "3", 2, 10),
            Request("NEWER", "1", 2, 1),
            Request("OLDER", "1", 2, 5));

        var result = await service.GetRequestsAsync("012345678");

        Assert.Collection(
            result.Requests,
            request => Assert.Equal("ACTION", request.RequestId),
            request => Assert.Equal("NEWER", request.RequestId),
            request => Assert.Equal("OLDER", request.RequestId));
    }

    private static RequestService CreateService(params FormRequest[] requests)
    {
        var catalogItem = new ServiceCatalogItem
        {
            FormCode = "TEST",
            ServiceName = "שירות בדיקה",
            Authorities = new Dictionary<string, AuthorityCatalogSettings>
            {
                ["6500"] = new AuthorityCatalogSettings
                {
                    AuthorityName = "עיריית חדרה",
                    StatusMap = new Dictionary<string, string>
                    {
                        ["1"] = "התקבלה",
                        ["2"] = "בטיפול",
                        ["3"] = "מסמכים נדרשים",
                        ["4"] = "אושרה",
                        ["5"] = "נדחתה"
                    },
                    ActionRequiredStatuses = ["3"],
                    FinalStatuses = ["4", "5"],
                    SlaDays = 14
                }
            }
        };

        return new RequestService(
            new StubFormsClient(requests),
            new StubCatalogRepository(catalogItem),
            NullLogger<RequestService>.Instance);
    }

    private static FormRequest Request(
        string requestId,
        string statusCode,
        int submittedDaysAgo,
        int lastUpdateDaysAgo,
        string formCode = "TEST")
    {
        var now = DateTimeOffset.UtcNow;

        return new FormRequest
        {
            RequestId = requestId,
            FormCode = formCode,
            AuthorityId = "6500",
            ApplicantId = "012345678",
            StatusCode = statusCode,
            SubmittedAt = now.AddDays(-submittedDaysAgo),
            LastUpdate = now.AddDays(-lastUpdateDaysAgo)
        };
    }

    private sealed class StubFormsClient(
        IReadOnlyList<FormRequest> requests) : IFormsClient
    {
        public Task<IReadOnlyList<FormRequest>> GetRequestsByApplicantIdAsync(
            string idNumber)
        {
            return Task.FromResult(requests);
        }
    }

    private sealed class StubCatalogRepository(
        ServiceCatalogItem catalogItem) : IServiceCatalogRepository
    {
        public Task<ServiceCatalogItem?> GetByFormCodeAsync(string formCode)
        {
            ServiceCatalogItem? result =
                formCode == catalogItem.FormCode ? catalogItem : null;

            return Task.FromResult(result);
        }
    }
}