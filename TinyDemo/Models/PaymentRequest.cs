using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyDemo.Models
{
    public class PaymentRequest
    {
        public int OrderId { get; set; }
        public string CreditCardNumber { get; set; }
    }
}