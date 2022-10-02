import {TransactionType} from "./TransactionType";
import {Bill} from "./Bill";

export class TransactionData {
  public id: string = '';
  public dateTime: Date = new Date();
  public type: TransactionType = TransactionType.Withdrawal;
  public amount: Bill[] = [];


  public getQuantity(denomination: number) {
    let bill = this.amount.filter(x => x.denomination == denomination);
    if(bill.length > 0) {
      return bill[0].quantity;
    }
    return 0;
  }
}
