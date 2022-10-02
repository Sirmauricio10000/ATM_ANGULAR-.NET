import {HttpClient} from "@angular/common/http";
import {Inject, Injectable} from "@angular/core";
import {BASE_API} from "../app.module";
import {Bill, WithdrawalOption} from "../models/Bill";
import {TransactionResult} from "../models/TransactionResult";
import {TransactionData} from "../models/TransactionData";
import {map} from "rxjs";

@Injectable()
export class TransactionService {

  constructor(private http: HttpClient, @Inject(BASE_API) private baseApi: string) {
  }

  getAmountAvailable() {
    return this.http.get<Bill[]>(`${this.baseApi}/Transaction/GetAmountAvailable`);
  }

  withdraw(amount: number) {
    return this.http.post<TransactionResult>(`${this.baseApi}/Transaction/Withdraw`, { amount });
  }

  getTransactions() {
    return this.http.get<TransactionData[]>(`${this.baseApi}/Transaction/GetTransactions`)
      .pipe(map(data => {
        return data.map(x => {
          let t = new TransactionData();
          t.id = x.id;
          t.type = x.type;
          t.amount = x.amount.map(b => new Bill(b.denomination, b.quantity));
          t.dateTime = new Date(x.dateTime);
          return t;
        })
      }));
  }

  getOptions(amount: number) {
    return this.http.get<WithdrawalOption[]>(`${this.baseApi}/Transaction/GetOptionForAmount`, {
      params: {amount}
    })
      .pipe(map(x => {
        return x.map(b => {
          let option = new WithdrawalOption();
          option.bills = b.bills.map(b => new Bill(b.denomination, b.quantity));
          option.score = b.score;
          return b;
        });
      }));
  }
}
