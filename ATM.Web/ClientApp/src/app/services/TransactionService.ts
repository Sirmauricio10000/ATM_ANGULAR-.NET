import {HttpClient} from "@angular/common/http";
import {Inject, Injectable} from "@angular/core";
import {BASE_API} from "../app.module";
import {Bill} from "../models/Bill";

@Injectable()
export class TransactionService {

  constructor(private http: HttpClient, @Inject(BASE_API) private baseApi: string) {
  }

  getAmountAvailable() {
    return this.http.get<Bill[]>(`${this.baseApi}/Transaction/GetAmountAvailable`, {
      headers:{
        "Accept": "text/json"
      }
    });
  }
}
