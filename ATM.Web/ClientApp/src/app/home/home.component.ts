import {Component, OnInit} from '@angular/core';
import {TransactionService} from "../services/TransactionService";
import {Bill} from "../models/Bill";
import {catchError} from "rxjs";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: [
    'home.component.sass'
  ]
})
export class HomeComponent implements OnInit{

  public amounts: Bill[] = [];
  public loadingAmount = false;

  constructor(public service: TransactionService) {
  }

  ngOnInit() {
    this.loadingAmount = true;
    this.service.getAmountAvailable()
      .subscribe(x => {
        this.amounts = x;
        this.loadingAmount = false;
      });
  }

  get totalAmount(){
    return this.amounts.reduce((p, x) => p + x.amount, 0);
  }
}
