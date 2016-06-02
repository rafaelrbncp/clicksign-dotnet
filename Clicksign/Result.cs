namespace Clicksign
{
    /// <summary>
    /// Result API
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Get or set <see cref="Document"/>
        /// </summary>
        public Document Document { get; set; }

        /// <summary>
        /// Get or set <see cref="Errors"/>
        /// </summary>
        public Errors Errors { get; set; }
    }
}