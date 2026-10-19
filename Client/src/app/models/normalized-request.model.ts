// מגדיר את צורת הנתונים שהלקוח מצפה לקבל מהשרת, כולל 
// actionRequired ו־isOverdue.
export interface NormalizedRequest {
  requestId: string;
  serviceName: string;
  authorityName: string;
  status: string;
  actionRequired: boolean;
  isOverdue: boolean;
  submittedAt: string;
  lastUpdate: string;
}

// מבנה התשובה שמכילה את רשימת הבקשות
export interface MyRequestsResponse {
  requests: NormalizedRequest[];
}
