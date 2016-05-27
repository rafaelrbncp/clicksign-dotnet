using System;
using System.Collections.Generic;
using System.Linq;
using Clicksign.Extensions;
using log4net;
using RestSharp;

namespace Clicksign
{
    public class ClicksignService
    {
        public readonly List<string> Errors = new List<string>();
        public ILog Log { get; set; }

        public ClicksignService()
        {
            Log = LogManager.GetLogger("ClicksignService");
        }

        public IRestResponse<HookResult> RetrieveHookResult(IRestClient client, IRestRequest request)
        {
            var result = Execute<HookResult>(client, request);
            return result;
        }

        public IRestResponse<List<Result>> RetrieveMultipleResult(IRestClient client, IRestRequest request)
        {
            var result = Execute<List<Result>>(client, request);
            Errors.AddRange(result.Data.Where(x => x.Errors != null).SelectMany(x => x.Errors.List));
            return result;
        }

        public IRestResponse<Result> RetrieveResult(IRestClient client, IRestRequest request)
        {
            var result = Execute<Result>(client, request);
            if (result?.Data?.Errors != null)
                Errors.AddRange(result.Data.Errors.List);
            return result;
        }

        private IRestResponse<T> Execute<T>(IRestClient client, IRestRequest request) where T : new()
        {
            IRestResponse<T> response = null;
            try
            {
                response = client.Execute<T>(request);

                Log.Info($"Status Code {response.StatusCode}, Status Description {(string.IsNullOrEmpty(response.StatusDescription) ? "is empty" : response.StatusDescription)} and Content {(string.IsNullOrEmpty(response.Content) ? "is empty" : response.Content)}");

                response.EnsureSuccess();

                if (response.ErrorException != null)
                    throw response.ErrorException;

                if (!string.IsNullOrEmpty(response.ErrorMessage))
                    throw new Exception(response.ErrorMessage);

                return response;
            }
            catch (Exception ex)
            {
                Log.Error("Erro of execute ClickSign API", ex);
                Errors.Add(ex.Message);
            }

            return response;
        }
    }
}