namespace Pure.Data.Pooling
{
    internal static class ErrorMessages
    {
        public const string AsyncFactoryForSyncGetObject = "Async factories cannot be used for sync GetObject operations. Please use GetObjectAsync.";
        public const string NegativeOrZeroMaximumPoolSize = "Maximum pool size must be greater than zero.";
        public const string NegativeOrZeroTimeout = "Timeout must be greater than zero.";
        public const string NullResource = "Resource cannot be null.";
    }
}
