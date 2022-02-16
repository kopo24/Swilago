import { HttpHandler } from "@angular/common/http";
import { HttpRequest } from "@angular/common/http";
import { HttpInterceptor } from "@angular/common/http";

export class AuthInterceptorService implements HttpInterceptor {
  //모든 헤더에 Bearer 토큰 붙여넣는 인터셉터 
  intercept(req: HttpRequest<any>, next: HttpHandler) {
    let _token = null;

    for (let i = 0; i < localStorage.length; i++) {
      if (localStorage.key(i)?.indexOf('access') !== -1) {
        const _key = localStorage.key(i);
        _token = localStorage.getItem(_key!);
      }
    }

    const secret = JSON.parse(_token!)['secret'];
    //const secret = JSON.parse(_token!).secret;

    const modifiedRequest = req.clone({ setHeaders: { authorization: `Bearer ${secret}` } });

    return next.handle(modifiedRequest);
  }

}
