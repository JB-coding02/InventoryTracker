namespace InventoryTracker.Models
{
    /// <summary>
    /// Represents data displayed in the application error page.
    /// Provides diagnostic information including the request ID and trace information for troubleshooting.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// The unique request ID that identifies the HTTP request that encountered an error.
        /// Used for diagnostic and logging purposes.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Indicates whether the request ID should be displayed on the error page.
        /// Returns true if a RequestId has been provided; otherwise false.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}

