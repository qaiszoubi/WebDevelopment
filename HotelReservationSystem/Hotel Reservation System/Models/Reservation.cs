using System;
using System.Collections.Generic;

namespace Hotel_Reservation_System.Models;

public partial class Reservation
{
    public decimal Reservationid { get; set; }

    public decimal? UserId { get; set; }

    public decimal? Roomid { get; set; }

    public DateTime Checkindate { get; set; }

    public DateTime Checkoutdate { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Room? Room { get; set; }

    public virtual User? User { get; set; }
}
