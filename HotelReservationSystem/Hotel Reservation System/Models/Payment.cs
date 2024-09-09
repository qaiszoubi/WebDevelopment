using System;
using System.Collections.Generic;

namespace Hotel_Reservation_System.Models;

public partial class Payment
{
    public decimal Paymentid { get; set; }

    public decimal? Reservationid { get; set; }

    public decimal Amount { get; set; }

    public DateTime Paymentdate { get; set; }

    public string Paymentmethod { get; set; } = null!;

    public string? Cardnumber { get; set; }

    public virtual Reservation? Reservation { get; set; }
}
