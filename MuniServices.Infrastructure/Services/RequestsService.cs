using System.Globalization;
using MuniServices.Core.Interfaces;
using MuniServices.Core.Models;
using Microsoft.Extensions.Logging;

namespace MuniServices.Infrastructure.Services;


// מביא בקשות ממערכת הטפסים המדומה, מחפש את הגדרותיהן 
// ומחזיר תשובה מנורמלת ללקוח.

public sealed class RequestService : IRequestService
{
    private readonly IFormsClient _formsClient;
    private readonly IServiceCatalogRepository _catalogRepository;
    private readonly ILogger<RequestService> _logger;

    public RequestService(
        IFormsClient formsClient,
        IServiceCatalogRepository catalogRepository,
        ILogger<RequestService> logger)
    {
        _formsClient = formsClient;
        _catalogRepository = catalogRepository;
        _logger = logger;
    }

    public async Task<MyRequestsResponse> GetRequestsAsync(string idNumber)
    {
         // מביא ממערכת הטפסים את הבקשות של המשתמש.
        var formRequests =
            await _formsClient.GetRequestsByApplicantIdAsync(idNumber);


        var normalizedRequests =
            new List<(NormalizedRequest Request, DateTimeOffset LastUpdate)>();

//משתמשים באותה נקודת זמן לכל הבקשות, כדי שחישוב האיחור יהיה עקבי.
        var now = DateTimeOffset.UtcNow;


        foreach (var formRequest in formRequests)
        {
            // עבור כל בקשה ממערכת הטפסים, מתבצע חיפוש בקטלוג לפי 
            // FormCode  
            var catalogItem = await _catalogRepository.GetByFormCodeAsync(
                formRequest.FormCode);


//4.5
            if (catalogItem is null)
            {
                _logger.LogWarning(
                    "Ignoring request {RequestId}: form code {FormCode} was not found in ServiceCatalog.",
                    formRequest.RequestId,
                    formRequest.FormCode);


      continue;//  מדלג רק על הבקשה הנוכחית
              // וממשיך לעבור על היתר 
             
            }

            if (!catalogItem.Authorities.TryGetValue(
                    formRequest.AuthorityId,
                    out var authoritySettings))
            {

                //כתיבה ללוג
                _logger.LogWarning(
                    "Ignoring request {RequestId}: authority {AuthorityId} was not found in ServiceCatalog for form {FormCode}.",
                    formRequest.RequestId,
                    formRequest.AuthorityId,
                    formRequest.FormCode);

                continue;
            }

            if (!authoritySettings.StatusMap.TryGetValue(
                    formRequest.StatusCode,
                    out var status))
            {
//4.2
                // סטטוס לא מוכר מוצג כברירת מחדל כ"בטיפול"
                //  ובמקביל נרשם בלוג.
                status = "בטיפול";

                _logger.LogWarning(
                    "Unknown status code {StatusCode} for request {RequestId}; using fallback status {FallbackStatus}.",
                    formRequest.StatusCode,
                    formRequest.RequestId,
                    status);
            }

            var actionRequired =
                authoritySettings.ActionRequiredStatuses.Contains(
                    formRequest.StatusCode);

            var isFinalStatus =
                authoritySettings.FinalStatuses.Contains(
                    formRequest.StatusCode);

            var deadline = formRequest.SubmittedAt.AddDays(
                authoritySettings.SlaDays);

            var isOverdue =
                !isFinalStatus && now > deadline;



            var normalizedRequest = new NormalizedRequest
            {
                RequestId = formRequest.RequestId,
                //קוד הטופס בשם השירות 4.1
                ServiceName = catalogItem.ServiceName,
              //  ואת קוד הרשות בשם הרשות. 
                AuthorityName = authoritySettings.AuthorityName,
                Status = status,
                ActionRequired = actionRequired,
                IsOverdue = isOverdue,

                //  התאריכים מוחזרים כמחרוזות בפורמט אחיד, ללא תלות בהגדרות  של השרת.
                SubmittedAt = formRequest.SubmittedAt.ToString(
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture),
                LastUpdate = formRequest.LastUpdate.ToString(
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture)
            };

            normalizedRequests.Add((normalizedRequest, formRequest.LastUpdate));
        }

// 4.6
        var sortedRequests = normalizedRequests
            .OrderByDescending(item => item.Request.ActionRequired)
            .ThenByDescending(item => item.LastUpdate)
            .Select(item => item.Request)
            .ToList();



        return new MyRequestsResponse
        {
            Requests = sortedRequests
        };
    }
}