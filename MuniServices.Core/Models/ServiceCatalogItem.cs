namespace MuniServices.Core.Models;
// מסמך אחד בקטלוג השירותים, לפי קוד טופס.
public sealed class ServiceCatalogItem
{
    public required string FormCode { get; init; }
    public required string ServiceName { get; init; }
    // הגדרות לכל רשות שתומכת בטופס. המפתח הוא AuthorityId.
    public required Dictionary<string, AuthorityCatalogSettings> Authorities { get; init; }
}


// הגדרות של שירות כפי שרשות מסוימת הגדירה אותן.

public sealed class AuthorityCatalogSettings
{
    public required string AuthorityName { get; init; }

   
    // תרגום מקוד סטטוס לשם הסטטוס לתצוגה.
   
    public required Dictionary<string, string> StatusMap { get; init; }

//קודי סטטוס שבהם נדרשת פעולה מצד התושב.
   public required List<string> ActionRequiredStatuses { get; init; }

//קודי ססטוס שמסמנים שהטיפול בבקשה הסתיים 
    public required List<string> FinalStatuses { get; init; }
    
    //מספר הימים שהרשות הקצתה לטיפול בבקשה
    public required int SlaDays { get; init; }
}