import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { NormalizedRequest } from '../models/normalized-request.model';
import { MyReqrestesService } from '../services/MyReqrestes.service';
import { HttpErrorResponse } from '@angular/common/http';




// רכיב רשימת הבקשות:


@Component({
  selector: 'app-my-requests',
  standalone: true,
  templateUrl: './MyReqrestes.html',
  styleUrl: './MyReqrestes.css'
})
export class MyReqrestes implements OnInit {

  private readonly myRequestsService = inject(MyReqrestesService);

  private readonly changeDetector = inject(ChangeDetectorRef);
// מערך הנתונים מהשרת יכנסו למשתנה הזה 
  requests: NormalizedRequest[] = [];
  selectedFilesByRequest: Record<string, string[]> = {};
  loading = false;
  errorMessage = '';

  ngOnInit(): void {
    //  טוען את הנתונים בעת יצירת הרכיב,
    this.loadRequest();
  }


  //פונקציה שמביאה את הבקשות מהשרת  מנהלת מצבי טעינה ושגיאה
  loadRequest(): void {
    this.loading = true;
    this.errorMessage = '';

    this.myRequestsService.getRequest().subscribe({
      next: (response) => {
        this.requests = response.requests;
        this.loading = false;
        this.changeDetector.markForCheck();
      },
      error: (error: HttpErrorResponse) => {
        this.loading = false;
        this.errorMessage =
          error.status === 401 || error.status === 403
            ? 'אין הרשאה לצפות בבקשות. יש לוודא שטוקן הבדיקה תקף.'
            : 'לא ניתן לטעון את רשימת הבקשות כרגע. אפשר לנסות שוב.';
        console.error('שגיאה בטעינת הבקשות:', error);
        this.changeDetector.markForCheck();
      }
    });
  }
// בבחירת קבצים הוא שומר בזיכרון רק את שמות הקבצים
// בלי להעלות אותם לשרת
  onFilesSelected(requestId: string, event: Event): void {
    const input = event.target;
    if (!(input instanceof HTMLInputElement)) {
      return;
    }


// שומר עבור הבקשה את שמות הקבצים שנבחרו בלבד הקבצים לא מועלים לשרת
    this.selectedFilesByRequest[requestId] = Array.from(
      input.files ?? [],
      (file) => file.name
    );
  }
}