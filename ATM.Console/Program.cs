using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;

int retiro = 200;


var reserves = new Amount(new[] {
    //new Bill(5, 10),
    new Bill(10, 40),
    new Bill(20, 1),
    new Bill(50, 3),
    new Bill(100, 2),
});

var disponible = reserves.Total;

Console.WriteLine(disponible);


var resultTransaction = WithdrawalOptions(reserves, retiro);

//if (resultTransaction.Result == ResultTransaction.AvailableFunds)
//{
//    foreach (var option in resultTransaction.Options.OrderBy(x => x.Score).Take(10))
//    {
//        foreach (var bill in option.Bills)
//        {
//            Console.WriteLine($"d: {bill.Denomination}, q: {bill.Quantity}");
//        }

//        Console.WriteLine();
//    }

//}
