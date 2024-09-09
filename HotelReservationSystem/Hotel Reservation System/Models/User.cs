using System;
using System.Collections.Generic;

namespace Hotel_Reservation_System.Models;

public partial class User
{
    public decimal UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string UserRole { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
