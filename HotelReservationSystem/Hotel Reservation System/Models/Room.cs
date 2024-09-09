using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_Reservation_System.Models;

public partial class Room
{
    public decimal Roomid { get; set; }

    public string Roomnumber { get; set; } = null!;

    public string Roomtype { get; set; } = null!;
    

    public decimal Price { get; set; }

    public string Isavailable { get; set; } = null!;

    public decimal? Hotelid { get; set; }

    public string? Imagepath { get; set; }
    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    public virtual Hotel? Hotel { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
