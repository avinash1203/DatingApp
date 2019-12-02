import { HttpClient } from '@angular/Common/http';
import { Injectable } from '@angular/core';
import {map} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

baseurl = 'http://localhost:5000/api/auth/'

constructor( private http: HttpClient) { }




login(model: any ){

  return this.http.post(this.baseurl + 'login',model)
         .pipe(
           map((reponse: any ) =>
           {
             const user = reponse;
             if (user)
             {
              localStorage.setItem('token', user.token);
             }
           }
           )
         )
}

}
