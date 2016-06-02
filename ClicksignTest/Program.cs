using Clicksign;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClicksignTest
{
    internal class Program
    {
        private static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();

            Upload();
        }

        private static void Upload()
        {
            var clicksign = new Clicksign.Clicksign();
            const string filePath = "D:\\Documentos\\Projetos\\Oferta de disciplina\\Oferta de Disciplina ao Professor.pdf";

            var response1 = clicksign.Upload(filePath);

            var response2 = clicksign
                .Signatories(response1.Document,
                new List<Signatory>
                {
                    new Signatory { Email = "rafaelrbncp@gmail.com", Action = SignatoryAction.Sign },
                    new Signatory { Email = "rafael.pena@animaeducacao.com.br", Action = SignatoryAction.Sign, AllowMethod = SignatoryAllowMethod.Sms}
                });

            Console.WriteLine(response1.Document.Key);
            Console.WriteLine(response2.Document.Key);
        }

        private static void Get()
        {
            var clicksign = new Clicksign.Clicksign();

            var list = clicksign.List();

            // ReSharper disable once UnusedVariable
            var document = clicksign.Get(list.First().Key);

            DownloadResponse downloadResponse;
            do
            {
                downloadResponse = clicksign.Download(list.First().Key);
            } while (!downloadResponse.isActionFinished);

            File.WriteAllBytes("Download-Clicksign.zip", downloadResponse.binaryFile);
        }
    }
}