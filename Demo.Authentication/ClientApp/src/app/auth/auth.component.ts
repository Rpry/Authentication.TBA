import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserInfo } from './user-info';
import * as moment from "moment";
import {TokenDto} from "./tokenDto";

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css']
})
export class AuthComponent implements OnInit {

  constructor(private readonly httpClient: HttpClient) { }

  public userInfo: UserInfo = {
    claims: [],
    isAuthenticated: false,
    scheme: ''
  };

  public login: string;
  public password: string;

  ngOnInit() {
    this.loadUserInfo();
  }

  public loadUserInfo(): void {
    this
      .httpClient
      .post('protected/getUserInfo', null)
      .subscribe((userInfo: UserInfo) => this.userInfo = userInfo);
  }

  public logout(): void {
    this.userInfo.isAuthenticated = false;
    this.setSession(null);
  }

  public authenticate(login: string, password: string): void {
    this
      .httpClient
      .post<TokenDto>(`token`, { login: login, password: password })
      .subscribe(result  => {
        if (result) {
          this.setSession(result.idToken);
          alert('Успех');
          this.loadUserInfo();
        } else {
          alert('Ошибка');
        }
      });
  }

  public checkJwtProtectedRoute(): void {
    this
      .httpClient
      .post('protected/methodRequiringAuthorization', null)
      .subscribe(result => {
          alert(result);
        },
          error => alert(error.status));
  }

  public checkAnonymousRoute(): void {
    this
      .httpClient
      .post('protected/method', null)
      .subscribe(result => alert(result), error => alert(error.status));
  }

  private setSession(authResult) {
    localStorage.setItem('id_token', authResult);
    if(authResult == null){
      return;
    }

    const expiresAt = moment().add(authResult.expiresIn,'second');
    localStorage.setItem("expires_at", JSON.stringify(expiresAt.valueOf()) );
  }
}
