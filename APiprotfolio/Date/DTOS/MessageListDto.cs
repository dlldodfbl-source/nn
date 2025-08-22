namespace PROtofile.Date.DTOS
{
  
    public class MessageListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string MessageBody { get; set; } = default!;
        public string? LocationText { get; set; } // يظهر "مدينة, دولة"
        public DateTime CreatedAt { get; set; }
    }
}

