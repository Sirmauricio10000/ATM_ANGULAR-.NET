import {Component, OnInit} from '@angular/core';
import {TransactionService} from "../services/TransactionService";
import {TransactionData} from "../models/TransactionData";
import {WithdrawalOption} from "../models/Bill";

@Component({
  selector: 'app-counter-component',
  templateUrl: './counter.component.html'
})
export class CounterComponent {

  public transactions: WithdrawalOption[] = [];
  public denominations: number[] = [];
  public headers: string[] = [];

  constructor(private service: TransactionService) {

    service.getOptions(20000).subscribe(x => console.log(x));
  }

}
