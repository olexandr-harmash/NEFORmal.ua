namespace NEFORmal.ua.Dating.ApplicationCore.Models;

public class Date
{
    private int _senderId;
    public int SenderId
    {
        get => _senderId;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("SenderId must be a positive number.", nameof(SenderId));
            _senderId = value;
        }
    }

    private int _receiverId;
    public int ReceiverId
    {
        get => _receiverId;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("ReceiverId must be a positive number.", nameof(ReceiverId));
            _receiverId = value;
        }
    }

    private bool _isApproved;
    public bool IsApproved
    {
        get => _isApproved;
        private set
        {
            if (SenderId == ReceiverId)
                throw new InvalidOperationException("Sender and receiver cannot be the same.");
            _isApproved = value;
        }
    }

    private string? _message;
    public string? Message
    {
        get => _message;
        private set
        {
            if (value != null && value.Length > 500)
                throw new ArgumentException("Message cannot exceed 500 characters.", nameof(Message));
            _message = value;
        }
    }

    
    public Profile? Sender   { get; set; }
    public Profile? Receiver { get; set; }

    public Date(int senderId, int receiverId, bool isApproved = false, string? message = "", Profile? sender = null, Profile? receiver = null)
    {
        SenderId   = senderId;
        ReceiverId = receiverId;
        IsApproved = isApproved;
        Message    = message;
        Receiver   = receiver;
        Sender     = sender;
    }

    public void Approve()
    {
        if (SenderId == ReceiverId)
            throw new InvalidOperationException("Sender and receiver cannot be the same.");

        IsApproved = true;
    }

    public void Reject()
    {
        IsApproved = false;
    }

    public void UpdateMessage(string? newMessage)
    {
        Message = newMessage;
    }
}
