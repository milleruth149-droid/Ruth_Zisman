
using MuniServices.Core.Models;

namespace MuniServices.Core.Interfaces;


// ממשק לתקשורת עם מערכת הטפסים.
// בפרויקט הזה המימוש יהיה מדומה, ובהמשך ניתן להחליפו בלקוח API אמיתי.

public interface IFormsClient
{
    Task<IReadOnlyList<FormRequest>> GetRequestsByApplicantIdAsync(
        string idNumber);
}