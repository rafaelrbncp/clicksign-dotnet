using RestSharp;
using System;
using System.Linq;
using System.Net;

namespace Clicksign.Extensions
{
    public static class RestResponseExtensions
    {
        public static void EnsureSuccess(this IRestResponse restResponse)
        {
            var expected = new[]
            {
                HttpStatusCode.OK,
                HttpStatusCode.Created,
                HttpStatusCode.Accepted,
                HttpStatusCode.Unauthorized,
                HttpStatusCode.NoContent,
                HttpStatusCode.ResetContent,
                HttpStatusCode.PartialContent,
                HttpStatusCode.MultipleChoices
            };

            if (!expected.Contains(restResponse.StatusCode))
                throw new Exception("The HttpStatusCode returned is outside of the expected range: 2xx.");
        }
    }
}