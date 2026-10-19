using MuniServices.Core.Models;
namespace MuniServices.Core.Interfaces;
// ממשק לסרביס 
public interface IRequestService
{
Task<MyRequestsResponse> GetRequestsAsync(string idNumber);
   

}