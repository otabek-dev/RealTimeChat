namespace RealTimeMail.Model
{
    public class Message
    {
        public Guid Id { get; set; }
        public string SenderName { get; set; } = null!;
        public string ReceiverName { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime Timestamp { get; set; }
    }
}
