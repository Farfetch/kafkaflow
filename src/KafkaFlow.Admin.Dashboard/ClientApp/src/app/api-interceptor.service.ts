import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpErrorResponse
} from '@angular/common/http';
import {Observable, throwError} from 'rxjs';
import {retry, catchError} from 'rxjs/operators';
import {CookieService} from 'ngx-cookie-service';
import {Injectable} from '@angular/core';

@Injectable()
export class ApiInterceptor implements HttpInterceptor {
  constructor( private cookieService: CookieService ) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    request = request.clone({
      setHeaders: this.cookieService.getAll()
    });
    return next.handle(request)
      .pipe(
        retry(1),
        catchError((error: HttpErrorResponse) => {
          let errorMessage = '';
          if (error.error instanceof ErrorEvent) {
            // client-side error
            errorMessage = `Error: ${error.error.message}`;
          } else {
            // server-side error
            errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
          }
          // window.alert(errorMessage);
          console.error(errorMessage);
          return throwError(errorMessage);
        })
      );
  }
}
