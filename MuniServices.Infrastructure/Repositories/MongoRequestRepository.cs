using MuniServices.Core.Interfaces;

namespace MuniServices.Infrastructure.Repositories;


// מימוש דמה בלבד:
//  למרות שם המחלקה, הקוד אינו מתחבר ל-MongoDB ומשתמש באובייקטים בזיכרון.
public class MongoRequestRepository : IRequestRepository
{
    public async Task<IEnumerable<object>> GetByCitizenIdAsync(string idNumber)
    {
        // הנתונים הקשיחים יושבים פה בשכבת התשתית
        var mockRequests = new List<object>
        {
            new { Id = 1, RequestType = "ארנונה", Status = "בטיפול", CitizenId = idNumber },
            new { Id = 2, RequestType = "חינוך", Status = "אושר", CitizenId = idNumber }
        };

        await Task.CompletedTask;
        return mockRequests;
    }
}