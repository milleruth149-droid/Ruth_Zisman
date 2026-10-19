
namespace MuniServices.Core.Models;
public sealed class FormRequest
{
public required string RequestId { get; init; }
    public required string FormCode { get; init; }
    public required string AuthorityId { get; init; }
    public required string ApplicantId { get; init; }
    public required string StatusCode { get; init; }
    public required DateTimeOffset SubmittedAt { get; init; }
    public required DateTimeOffset LastUpdate { get; init; }
    
    }


