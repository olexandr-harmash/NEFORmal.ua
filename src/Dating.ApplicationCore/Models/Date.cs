namespace NEFORmal.ua.Dating.ApplicationCore.Models;

public class Date
{
    // Sender's unique identifier (must be a positive number).
    private int _senderId;

    /// <summary>
    /// Gets the sender's ID.
    /// Sender ID must be a positive number.
    /// </summary>
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

    // Receiver's unique identifier (must be a positive number).
    private int _receiverId;

    /// <summary>
    /// Gets the receiver's ID.
    /// Receiver ID must be a positive number.
    /// </summary>
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

    // Indicates whether the date request has been approved.
    private bool _isApproved;

    /// <summary>
    /// Gets or sets whether the date request has been approved.
    /// Sender and receiver cannot be the same person.
    /// </summary>
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

    // The message sent with the date request (optional).
    private string? _message;

    /// <summary>
    /// Gets or sets the message associated with the date request.
    /// Message cannot exceed 500 characters.
    /// </summary>
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

    // Navigation properties for the sender and receiver profiles.
    public Profile? Sender { get; set; }
    public Profile? Receiver { get; set; }

    /// <summary>
    /// Constructor to initialize a new date request between a sender and a receiver.
    /// </summary>
    /// <param name="senderId">ID of the sender.</param>
    /// <param name="receiverId">ID of the receiver.</param>
    /// <param name="isApproved">Approval status of the date request (default is false).</param>
    /// <param name="message">Optional message accompanying the date request.</param>
    public Date(int senderId, int receiverId, bool isApproved = false, string? message = "")
    {
        SenderId = senderId;
        ReceiverId = receiverId;
        IsApproved = isApproved;
        Message = message;
    }

    /// <summary>
    /// Approves the date request, indicating mutual agreement.
    /// </summary>
    public void Approve()
    {
        if (SenderId == ReceiverId)
            throw new InvalidOperationException("Sender and receiver cannot be the same.");
        
        IsApproved = true;
    }

    /// <summary>
    /// Rejects the date request, indicating disapproval.
    /// </summary>
    public void Reject()
    {
        IsApproved = false;
    }

    /// <summary>
    /// Updates the message associated with the date request.
    /// </summary>
    /// <param name="newMessage">New message to set.</param>
    public void UpdateMessage(string? newMessage)
    {
        Message = newMessage;
    }
}
