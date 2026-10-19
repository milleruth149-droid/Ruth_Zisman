using MuniServices.Core.Interfaces;
using MuniServices.Core.Models;

namespace MuniServices.Infrastructure.Clients;


// נתוני דמה במקום קריאה למערכת טפסים חיצונית.

public sealed class MockFormsClient : IFormsClient
{
    private readonly List<FormRequest> _requests;

    public MockFormsClient()
    {
        var now = DateTimeOffset.UtcNow;

        _requests =
        [

            new FormRequest
{
    RequestId = "F-88217",
    FormCode = "ARNONA_DISCOUNT",
    AuthorityId = "6500",
    ApplicantId = "012345678",
    StatusCode = "1",
    SubmittedAt = now.AddDays(-7),
    LastUpdate = now.AddDays(-5)
},
new FormRequest
{
    RequestId = "F-88218",
    FormCode = "PARKING_PERMIT",
    AuthorityId = "6500",
    ApplicantId = "012345678",
    StatusCode = "3",
    SubmittedAt = now.AddDays(-9),
    LastUpdate = now.AddDays(-2)
},
new FormRequest
{
    RequestId = "F-88219",
    FormCode = "HAZARD_REPORT",
    AuthorityId = "6500",
    ApplicantId = "012345678",
    StatusCode = "2",
    SubmittedAt = now.AddDays(-20),
    LastUpdate = now.AddDays(-10)
},
new FormRequest
{
    RequestId = "F-88220",
    FormCode = "HAZARD_REPORT",
    AuthorityId = "6500",
    ApplicantId = "012345678",
    StatusCode = "4",
    SubmittedAt = now.AddDays(-60),
    LastUpdate = now.AddDays(-45)
},


            new FormRequest
            {
                RequestId = "F-88213",
                FormCode = "ARNONA_DISCOUNT",
                AuthorityId = "6500",
                ApplicantId = "012345678",
                StatusCode = "3",
                SubmittedAt = now.AddDays(-12),
                LastUpdate = now.AddDays(-1)
            },
            new FormRequest
            {
                RequestId = "F-88214",
                FormCode = "PARKING_PERMIT",
                AuthorityId = "6500",
                ApplicantId = "012345678",
                StatusCode = "2",
                SubmittedAt = now.AddDays(-40),
                LastUpdate = now.AddDays(-3)
            },
            new FormRequest
            {
                RequestId = "F-88215",
                FormCode = "ARNONA_DISCOUNT",
                AuthorityId = "6500",
                ApplicantId = "012345678",
                StatusCode = "99",
                SubmittedAt = now.AddDays(-60),
                LastUpdate = now.AddDays(-20)
            },
            new FormRequest
            {
                RequestId = "F-88216",
                FormCode = "UNKNOWN_FORM",
                AuthorityId = "6500",
                ApplicantId = "012345678",
                StatusCode = "1",
                SubmittedAt = now.AddDays(-5),
                LastUpdate = now.AddDays(-2)
            },
            new FormRequest
            {
                RequestId = "F-99001",
                FormCode = "ARNONA_DISCOUNT",
                AuthorityId = "6500",
                ApplicantId = "987654321",
                StatusCode = "2",
                SubmittedAt = now.AddDays(-8),
                LastUpdate = now.AddDays(-2)
            }
        ];
    }


//סעיף 2
// מחזיר רק את בקשות הדמה שמספר הזהות שלהן תואם למשתמש המבוקש
    public Task<IReadOnlyList<FormRequest>> GetRequestsByApplicantIdAsync(
        string idNumber)
    {
        IReadOnlyList<FormRequest> result = _requests
            .Where(request => request.ApplicantId == idNumber)
            .ToList();

        return Task.FromResult(result);
    }
}