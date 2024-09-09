using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_Reservation_System.Models;

public partial class Hotel
{
    public decimal Hotelid { get; set; }

    public string Name { get; set; } = null!;

    public string Location { get; set; } = null!;

    public decimal Numberofrooms { get; set; }

    public string? Description { get; set; }

    public string? Imagepath { get; set; }

    [NotMapped]
    public IFormFile ImageFile { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
