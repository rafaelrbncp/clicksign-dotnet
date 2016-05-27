using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using log4net;
using RestSharp;

namespace Clicksign
{
    /// <summary>
    /// Clicksign API, more information visit <see cref="http://clicksign.github.io/rest-api-v2">Clicksign Rest API</see>
    /// </summary>
    public class Clicksign
    {
        private readonly ClicksignService _clicksignService;

        /// <summary>
        /// Initialize new instance of class <see cref="Clicksign"/>
        /// </summary>
        public Clicksign()
        {
            Host = string.IsNullOrEmpty(Host) ? ConfigurationManager.AppSettings["Clicksign-Host"] : Host;
            Token = string.IsNullOrEmpty(Token) ? ConfigurationManager.AppSettings["Clicksign-Token"] : Token;
            Log = LogManager.GetLogger("Clicksign");

            _clicksignService = new ClicksignService();
        }

        private ILog Log { get; set; }

        /// <summary>
        /// Initialize new instance of class <see cref="Clicksign"/>
        /// </summary>
        /// <param name="token">Token</param>
        public Clicksign(string token)
            : this()
        {
            Token = token;
        }

        /// <summary>
        /// Initialize new instance of class <see cref="Clicksign"/>
        /// </summary>
        /// <param name="host">Host</param>
        /// <param name="token">Token</param>
        public Clicksign(string host, string token)
            : this(token)
        {
            Host = host;
        }

        /// <summary>
        /// Get Clicksign host
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// Get Token
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        /// Get <see cref="Document"/>
        /// </summary>
        public Document Document { get; private set; }

        /// <summary>
        /// Upload file, more information visit <see cref="http://clicksign.github.io/rest-api-v2/#upload-de-documentos">Clicksign Rest API</see>
        /// </summary>
        /// <param name="file">File</param>
        /// <returns><see cref="Clicksign"/></returns>
        public Clicksign Upload(string file)
        {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentNullException(nameof(file), "File path is empty.");

            if (!File.Exists(file))
                throw new FileNotFoundException($"File {file} not found.");

            return Upload(File.ReadAllBytes(file), Path.GetFileName(file));
        }

        /// <summary>
        /// Upload file, more information visit <see cref="http://clicksign.github.io/rest-api-v2/#upload-de-documentos">Clicksign Rest API</see>
        /// </summary>
        /// <param name="file">Bytes of file</param>
        /// <param name="fileName">File name</param>
        /// <returns><see cref="Clicksign"/></returns>
        public Clicksign Upload(byte[] file, string fileName)
        {
            if (file.Length.Equals(0))
                throw new ArgumentNullException(nameof(file), "File is empty.");

            if (string.IsNullOrEmpty(fileName))
                throw new FileNotFoundException("File name is null or empty.");

            var client = new RestClient(Host);
            var request = new RestRequest("v1/documents", Method.POST);

            request.AddParameter("access_token", Token);
            request.AddHeader("Content-Type", "multipart/mixed; boundary=frontier");
            request.AddHeader("Accept", "application/json");
            request.AddFile("document[archive][original]", file, fileName);

            Log.Info($"Send file {fileName} with token {Token}");

            var response = _clicksignService.RetrieveResult(client, request).Data;

            Document = response.Document;

            return this;
        }

        /// <summary>
        /// Create list of <see cref="Signatory"/>
        /// </summary>
        /// <example>
        ///     <see cref="http://clicksign.github.io/rest-api-v2/#criacao-de-lista-de-assinatura">Sample and documentation</see>
        /// </example>
        /// <param name="signatory"><see cref="Signatory"/></param>
        /// <returns><see cref="Clicksign"/></returns>
        public Clicksign Signatories(Signatory signatory)
        {
            return Signatories(Document, signatory);
        }

        /// <summary>
        /// Create list of <see cref="Signatory"/>
        /// </summary>
        /// <example>
        ///     <see cref="http://clicksign.github.io/rest-api-v2/#criacao-de-lista-de-assinatura">Sample and documentation</see>
        /// </example>        
        /// <param name="signatories">List of <see cref="Signatory"/></param>
        /// <returns><see cref="Clicksign"/></returns>
        public Clicksign Signatories(IList<Signatory> signatories)
        {
            return Signatories(Document, signatories);
        }

        /// <summary>
        /// Create list of <see cref="Signatory"/>
        /// </summary>
        /// <example>
        ///     <see cref="http://clicksign.github.io/rest-api-v2/#criacao-de-lista-de-assinatura">Sample and documentation</see>
        /// </example>  
        /// <param name="document"><see cref="Document"/></param>
        /// <param name="signatory"><see cref="Signatory"/></param>
        /// <returns><see cref="Clicksign"/></returns>
        public Clicksign Signatories(Document document, Signatory signatory)
        {
            return Signatories(document, new List<Signatory> { signatory });
        }

        /// <summary>
        /// Create list of <see cref="Signatory"/>
        /// </summary>
        /// <example>
        ///     <see cref="http://clicksign.github.io/rest-api-v2/#criacao-de-lista-de-assinatura">Sample and documentation</see>
        /// </example>  
        /// <param name="document"><see cref="Document"/></param>
        /// <param name="signatories">List of <see cref="Signatory"/></param>
        /// <returns><see cref="Clicksign"/></returns>
        public Clicksign Signatories(Document document, IList<Signatory> signatories)
        {
            if (string.IsNullOrEmpty(document?.Key))
                throw new ArgumentNullException(nameof(document), "Document not informed or empty key");

            if (!signatories.Any())
                throw new ArgumentNullException(nameof(signatories), "Signatories is empty");

            var client = new RestClient(Host);
            var request = new RestRequest($"v1/documents/{document.Key}/list", Method.POST);

            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("access_token", Token);
            if (document.List.SkipEmail)
                request.AddParameter("skip_email", "true");
            request.AddParameter("message", document.List.Message);

            Log.Info(
                $"Send list of Signatories with Token {Token}, SkipEmail {document.List.SkipEmail}, Message {document.List.Message} and {signatories.Count} signatories");

            foreach (var signatory in signatories)
            {
                var action = signatory.Action.ToString().ToLower();
                var allowMethod = signatory.AllowMethod.ToString().ToLower();

                request.AddParameter("signers[][email]", signatory.Email);
                request.AddParameter("signers[][act]", action);
                request.AddParameter("signers[][allow_method]", allowMethod);
                if (!string.IsNullOrWhiteSpace(signatory.PhoneNumber))
                    request.AddParameter("signers[][phone_number]", signatory.PhoneNumber);
                if (!string.IsNullOrWhiteSpace(signatory.DisplayName))
                    request.AddParameter("signers[][display_name]", signatory.DisplayName);
                if (!string.IsNullOrWhiteSpace(signatory.Documentation))
                    request.AddParameter("signers[][documentation]", signatory.Documentation);
                if (!string.IsNullOrWhiteSpace(signatory.Birthday))
                    request.AddParameter("signers[][birthday]", signatory.Birthday);
                if (!string.IsNullOrWhiteSpace(signatory.SkipDocumentation))
                    request.AddParameter("signers[][skip_documentation]", signatory.SkipDocumentation);

                Log.Info($"Send Signatory Email {signatory.Email} and Action {action} to list");
            }

            var response = _clicksignService.RetrieveResult(client, request);

            if (_clicksignService.Errors.Any())
                return this;

            Document = response.Data.Document;
            return this;
        }

        /// <summary>
        /// List of <see cref="Document"/>, more information visit <see cref="http://clicksign.github.io/rest-api-v2/#listagem-de-documentos">Clicksign Rest API</see>
        /// </summary>
        /// <returns>List of <see cref="Document"/></returns>
        public List<Document> List()
        {
            var client = new RestClient(Host);
            var request = new RestRequest("v1/documents", Method.GET);

            request.AddParameter("access_token", Token);
            request.AddHeader("Accept", "application/json");

            Log.Info($"Get list of document with Token {Token}");

            var response = _clicksignService.RetrieveMultipleResult(client, request);
            var documents = response.Data.Select(result => result.Document).ToList();

            Log.Info($"Get {documents.Count} documents of list");

            return documents;
        }

        /// <summary>
        /// Resend an email with a message about a document
        /// </summary>
        /// <param name="documentKey">document key</param>
        /// <param name="email">email to send</param>
        /// <param name="message">message to send</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Clicksign Resend(string documentKey, string email, string message)
        {
            if (string.IsNullOrEmpty(documentKey))
                throw new ArgumentNullException(nameof(documentKey), "documentKey is empty.");

            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email), "email is empty.");

            var client = new RestClient(Host);
            var request = new RestRequest($"v1/documents/{documentKey}/resend", Method.POST);

            request.AddParameter("access_token", Token);
            request.AddHeader("Accept", "application/json");

            request.AddParameter("email", email);
            request.AddParameter("message", message);

            Log.Info($"Resending document {documentKey} to an email with token {Token} ");

            var response = _clicksignService.RetrieveMultipleResult(client, request);

            if (response.StatusCode == HttpStatusCode.OK)
                Log.Info("Resend Success");

            return this;
        }


        /// <summary>
        /// Create hook, more information visit <see cref="http://clicksign.github.io/rest-api-v2/#hooks">Clicksign Rest API</see>
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns><see cref="HookResult"/></returns>
        public HookResult CreateHook(string url)
        {
            return CreateHook(Document, url);
        }

        /// <summary>
        /// Create hook, more information visit <see cref="http://clicksign.github.io/rest-api-v2/#hooks">Clicksign Rest API</see>
        /// </summary>
        /// <param name="document"><see cref="Document"/></param>
        /// <param name="url">Url</param>
        /// <returns><see cref="HookResult"/></returns>
        public HookResult CreateHook(Document document, string url)
        {
            if (string.IsNullOrEmpty(document?.Key))
                throw new ArgumentNullException(nameof(document), "Document not informed or empty key");

            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url), "Url is null or empty");

            var client = new RestClient(Host);
            var request = new RestRequest($"v1/documents/{document.Key}/hooks", Method.POST);

            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("access_token", Token);
            request.AddParameter("url", url);

            Log.Info($"Create hook of document with Token {Token}, Document {document.Key} and Url {url}");

            return _clicksignService.RetrieveHookResult(client, request).Data;
        }

        /// <summary>
        /// Cancel <see cref="Document"/>, more information visit <see cref="http://clicksign.github.io/rest-api/#visualizacao-de-documento">Clicksign Rest API</see>
        /// </summary>
        /// <returns><see cref="Document"/></returns>
        public Document Cancel(string key)
        {
            var client = new RestClient(Host);
            var request = new RestRequest($"v1/documents/{key}/cancel", Method.POST);

            request.AddParameter("access_token", Token);
            request.AddHeader("Accept", "application/json");

            Log.Info($"Cancel document with Token {Token}");

            var response = _clicksignService.RetrieveResult(client, request);
            var document = response.Data.Document;

            if (document == null) Log.Debug("Document not found with key " + key);

            return document;
        }

        /// <summary>
        /// Get <see cref="Document"/>, more information visit <see cref="http://clicksign.github.io/rest-api/#visualizacao-de-documento">Clicksign Rest API</see>
        /// </summary>
        /// <returns><see cref="Document"/></returns>
        public Document Get(string key)
        {
            var client = new RestClient(Host);
            var request = new RestRequest($"v1/documents/{key}", Method.GET);

            request.AddParameter("access_token", Token);
            request.AddHeader("Accept", "application/json");

            Log.Info($"Get document with Token {Token}");

            var response = _clicksignService.RetrieveResult(client, request);
            var document = response.Data.Document;

            if (document == null) Log.Debug("Document not found with key " + key);

            return document;
        }

        /// <summary>
        ///     Download <see cref="Document" />, more information visit
        ///     <see cref="http://github.com/clicksign/rest-api#download-de-documento">Clicksign Rest API</see>
        /// </summary>
        public DownloadResponse Download(string key)
        {
            var downloadResponse = new DownloadResponse();

            try
            {
                var client = new RestClient(Host);
                var request = new RestRequest($"v1/documents/{key}/download", Method.GET);
                request.AddParameter("access_token", Token);
                request.AddHeader("Accept", "application/json");

                Log.Info($"Download document with Token {Token}");

                var response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    downloadResponse.binaryFile = response.RawBytes;
                }
                else if (response.StatusCode != HttpStatusCode.Accepted)
                {
                    downloadResponse.AddDownloadError("StatusCode unexpected -" + response.StatusCode);
                    Log.Debug($"Error - Download document with Token {Token}");
                }
            }
            catch (Exception e)
            {
                downloadResponse.AddDownloadError(e.Message, e);
            }

            return downloadResponse;
        }
    }
}
