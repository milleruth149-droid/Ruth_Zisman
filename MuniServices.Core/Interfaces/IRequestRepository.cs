namespace MuniServices.Core.Interfaces;
// חוזה לשליפת בקשות לפי מספר זהות אינו חלק מהזרימה הנוכחית של ה-API.
public interface IRequestRepository
{
    Task<IEnumerable<object>> GetByCitizenIdAsync(string idNumber);
}