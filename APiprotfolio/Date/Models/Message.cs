using System.ComponentModel.DataAnnotations;

namespace PROtofile.Date.Models
{


    public class Message
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string FullName { get; set; } = default!;

        [Required, EmailAddress, MaxLength(160)]
        public string Email { get; set; } = default!;

        [Required, MaxLength(4000)]
        public string MessageBody { get; set; } = default!; // النص اللي بعته

        // موقع مختصر يظهر في الداشبورد
        [MaxLength(120)] public string? City { get; set; }
        [MaxLength(120)] public string? Country { get; set; }
        [MaxLength(200)] public string? LocationText { get; set; } // مثال: "القاهرة, مصر"

     

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

