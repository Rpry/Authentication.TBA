export interface UserInfo {
  claims: {type: string, value: string}[];

  scheme: string;

  isAuthenticated: boolean;
}
