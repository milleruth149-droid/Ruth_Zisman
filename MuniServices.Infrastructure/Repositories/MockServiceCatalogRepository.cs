using MuniServices.Core.Interfaces;
using MuniServices.Core.Models;

namespace MuniServices.Infrastructure.Repositories;
// קטלוג דמה שמוגדר בזיכרון
//  הוא מדמה את המבנה של 
// ServiceCatalog אך אינו מתחבר ל-MongoDB.
public sealed class MockServiceCatalogRepository : IServiceCatalogRepository
{
    private readonly List<ServiceCatalogItem> _items =
    [
        new ServiceCatalogItem
        {
            FormCode = "ARNONA_DISCOUNT",
            ServiceName = "הנחה בארנונה",
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
                    SlaDays = 30
                }
            }
        },
        new ServiceCatalogItem
{
    FormCode = "HAZARD_REPORT",
    ServiceName = "דיווח על מפגע",
    Authorities = new Dictionary<string, AuthorityCatalogSettings>
    {
        ["6500"] = new AuthorityCatalogSettings
        {
            AuthorityName = "עיריית חדרה",
            StatusMap = new Dictionary<string, string>
            {
                ["1"] = "התקבל דיווח",
                ["2"] = "בטיפול",
                ["3"] = "נדרש מידע נוסף",
                ["4"] = "טופל",
                ["5"] = "נסגר"
            },
            ActionRequiredStatuses = ["3"],
            FinalStatuses = ["4", "5"],
            SlaDays = 7
        }
    }
},
        new ServiceCatalogItem
        {
            FormCode = "PARKING_PERMIT",
            ServiceName = "תו חניה",
            Authorities = new Dictionary<string, AuthorityCatalogSettings>
            {
                ["6500"] = new AuthorityCatalogSettings
                {
                    AuthorityName = "עיריית חדרה",
                    StatusMap = new Dictionary<string, string>
                    {
                        ["1"] = "התקבלה",
                        ["2"] = "בטיפול",
                        ["3"] = "נדרשים מסמכים",
                        ["4"] = "אושרה",
                        ["5"] = "נדחתה"
                    },
                    ActionRequiredStatuses = ["3"],
                    FinalStatuses = ["4", "5"],
                    SlaDays = 14
                }
            }
        }
    ];

//מחפש ברשימה  
// את המסמך שקוד הטופס שלו תואם לקוד שהתקבל עבור משימה 3 חלק א
// מחפש בזיכרון שירות לפי קוד טופס ומחזיר null אם הקוד אינו מוגדר בקטלוג.
    public Task<ServiceCatalogItem?> GetByFormCodeAsync(string formCode)
    {
        var item = _items.FirstOrDefault(
            catalogItem => catalogItem.FormCode == formCode);

        return Task.FromResult(item);
    }
}