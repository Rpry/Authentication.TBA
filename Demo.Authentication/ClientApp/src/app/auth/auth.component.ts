import {ChangeDetectionStrategy, Component, Inject, OnInit} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserInfo } from './user-info';
import * as moment from "moment";
import {TokenDto} from "./tokenDto";
import {User} from "oidc-client";

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css']
})
export class AuthComponent implements OnInit {

  constructor(private readonly httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.login = "";
    this.password = "";
    this.baseUrl = baseUrl;
  }

  public userInfo: UserInfo = {
    claims: [],
    isAuthenticated: false,
    scheme: ''
  };

  public login: string;
  public password: string;
  public baseUrl: string;
  ngOnInit() {
    this.loadUserInfo();
  }

  public loadUserInfo(): void {
    this
      .httpClient
      .post<UserInfo>(`${this.baseUrl}/protected/getUserInfo`, null)
      .subscribe((data) =>  {
        this.userInfo = data
      });
  }
  public logout(): void {
    this.userInfo.isAuthenticated = false;
    this.setSession("");
  }
  public authenticate(login: string, password: string): void {
    this
      .httpClient
      .post<TokenDto>(`${this.baseUrl}/token`, { login: login, password: password })
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
      .post(`${this.baseUrl}/protected/methodRequiringAuthorization`, null)
      .subscribe(result => {
          alert(result);
        },
          error => alert(error.status));
  }
  public checkAnonymousRoute(): void {
    this
      .httpClient
      .post(`${this.baseUrl}/protected/method`, null)
      .subscribe(result => alert(result), error => alert(error.status));
  }
  private setSession(authResult: string) {
    localStorage.setItem('id_token', authResult);
    if(authResult == null){
      return;
    }

    //const expiresAt = moment().add(authResult.expiresIn,'second');
    //localStorage.setItem("expires_at", JSON.stringify(expiresAt.valueOf()) );
  }
}
