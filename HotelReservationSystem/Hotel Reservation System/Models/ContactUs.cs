namespace Hotel_Reservation_System.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }
        public DateTime SentDate { get; set; } = DateTime.Now;
    }
}
