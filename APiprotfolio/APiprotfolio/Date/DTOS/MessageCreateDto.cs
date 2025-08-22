using System.ComponentModel.DataAnnotations;

namespace PROtofile.Date.DTOS
{

    public class MessageCreateDto
    {
        [Required, MaxLength(120)]
        public string FullName { get; set; } = default!;

        [Required, EmailAddress, MaxLength(160)]
        public string Email { get; set; } = default!;

        [Required, MaxLength(4000)]
        public string MessageBody { get; set; } = default!;

        // اختياري من الفرونت لو متاح
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public string? LocationText { get; set; } // لو بعته جاهز
    }
}
