namespace RabbitOperations.Domain
{
    public enum AdditionalErrorStatus
    {
        NotAnError,
        Unresolved,
        RetryPending,
        Resolved,
        Closed
    }
}