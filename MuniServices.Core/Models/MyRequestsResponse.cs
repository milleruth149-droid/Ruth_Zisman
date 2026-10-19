namespace MuniServices.Core.Models;

/// <summary>
/// מעטפת התשובה של נקודת הקצה.
/// </summary>
public sealed class MyRequestsResponse
{
    public required IReadOnlyList<NormalizedRequest> Requests { get; init; }
}