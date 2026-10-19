using MuniServices.Core.Models;

namespace MuniServices.Core.Interfaces;


// ממשק לשליפת הגדרות שירות מהקטלוג לפי קוד טופס.

public interface IServiceCatalogRepository
{
    Task<ServiceCatalogItem?> GetByFormCodeAsync(string formCode);
}