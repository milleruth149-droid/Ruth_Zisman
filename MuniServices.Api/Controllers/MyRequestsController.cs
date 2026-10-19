using Microsoft.AspNetCore.Mvc;
using MuniServices.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MuniServices.Core.Models;

namespace MuniServices.Api.Controllers;
//חלק א' סעיף 1
// כל הפעולות בקונטרולר דורשות משתמש עם טוקן שעבר אימות
[Authorize]

[ApiController]

// קונטרולר יחיד לכל הפרויקט 
[Route("api/my-requests")]
public class MyRequestsController : ControllerBase
{
    private readonly IRequestService _service;

    public MyRequestsController(IRequestService service)
    {
        _service = service;
    }

// מחזיר את הבקשות של המשתמש המחובר.
    [HttpGet]
    public async Task<ActionResult<MyRequestsResponse>> GetRequests()
    {

// מוציא את מספר הזהות מתוך הטוקן של המשתמש.
var idNumber = User.FindFirst("idNumber")?.Value;
if (string.IsNullOrWhiteSpace(idNumber))
// בלי claim של מספר זהות אין אפשרות לשייך את הבקשות למשתמש, 
// ולכן הבקשה נדחית.
            return Forbid();

// מבקש מהסרביס את הבקשות ששייכות למספר הזהות.
        var requests = await _service.GetRequestsAsync(idNumber);
        return Ok(requests);
        }
}