namespace PROtofile.Date.DTOS
{
    public class ChatDto
    {
    
    public record ChatMessage(string role, string content); // roles: "system" | "user" | "assistant"

    public class ChatRequestDto
    {
        public List<ChatMessage> Messages { get; set; } = new(); // تاريخ المحادثة
        public bool Stream { get; set; } = false;                 // لِمّا نفعّل ستريم لاحقًا
    }

    public class ChatResponseDto
    {
        public string Reply { get; set; } = "";
    }
}
}
