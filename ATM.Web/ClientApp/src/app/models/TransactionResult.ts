import {TransactionStatus} from "./TransactionStatus";
import {Bill} from "./Bill";

export class TransactionResult {
  public status: TransactionStatus = TransactionStatus.UnavailableFunds;
  public amount: Bill[] = [];
  public message: string = '';
}

