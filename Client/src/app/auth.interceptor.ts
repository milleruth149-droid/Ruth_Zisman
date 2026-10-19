// מתחילה בכתובת ה־API שהוגדרה, אם נמצא טוקן בשם access_token ב־sessionStorage. 
// אם אין טוקן, או שהבקשה אינה לכתובת ה־API הזו,
//  הבקשה נשלחת ללא שינוי. ה־interceptor אינו אוכף הרשאות בעצמו.

import { HttpInterceptorFn } from '@angular/common/http';

const apiBaseUrl =
  'https://ominous-capybara-4j57rq97xjg4fw9g-5225.app.github.dev/api/';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  if (!request.url.startsWith(apiBaseUrl)) {
    return next(request);
  }

  const token = sessionStorage.getItem('access_token');
  if (!token) {
    return next(request);
  }

  return next(
    request.clone({
      setHeaders: {
        Authorization: ['Bearer', token].join(' ')
      }
    })
  );
};
