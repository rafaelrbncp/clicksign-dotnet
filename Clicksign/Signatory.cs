namespace Clicksign
{
    /// <summary>
    /// Signatory, more information visit <see cref="http://clicksign.github.io/rest-api-v2">Clicksign Rest API</see>
    /// </summary>
    public class Signatory
    {
        /// <summary>
        /// Get or set <see cref="SignatoryAction"/>
        /// </summary>
        public SignatoryAction Action { get; set; }

        /// <summary>
        /// Get or set E-mail
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Get or set <see cref="SignatoryAllowMethod"/>
        /// </summary>
        public SignatoryAllowMethod AllowMethod { get; set; }

        /// <summary>
        /// Get or set PhoneNumber
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Get or set DisplayName
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Get or set Documentation
        /// </summary>
        public string Documentation { get; set; }

        /// <summary>
        /// Get or set Birthday
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// Get or set SkipDocumentation
        /// </summary>
        public string SkipDocumentation { get; set; }
    }
}