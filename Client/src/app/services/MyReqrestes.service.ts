import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MyRequestsResponse } from '../models/normalized-request.model';

@Injectable({
  providedIn: 'root'
})
export class MyReqrestesService {

  // URL לשרת
  private readonly apiUrl =
    'https://ominous-capybara-4j57rq97xjg4fw9g-5225.app.github.dev/api';

  constructor(private readonly http: HttpClient) {}
//  פונקציה שניגשת לשרת ומקבלת   את רשימת הבקשות של המשתמש 
  getRequest(): Observable<MyRequestsResponse> {
    return this.http.get<MyRequestsResponse>(`${this.apiUrl}/my-requests`);
  }
}