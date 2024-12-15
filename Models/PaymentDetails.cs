using Microsoft.EntityFrameworkCore;
using System;

namespace BDwAS_projekt.Models
{
    [Owned]
    public class PaymentDetails
    {
        public DateTime PaymentDate { get; set; }
        public int AddedDays { get; set; }
        public double FullPrice { get; set; }
        public double PaidPrice { get; set; }
        public double Discount { get; set; }
    }
}
