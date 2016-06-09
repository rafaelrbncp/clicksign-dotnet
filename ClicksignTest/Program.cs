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
            var document = response1.Document;

            var response2 = clicksign
                .Signatories(document,
                new List<Signatory>
                {
                    new Signatory { Email = "rafaelrbncp@gmail.com", Action = SignatoryAction.Sign },
                    new Signatory { Email = "rafael.pena@animaeducacao.com.br", Action = SignatoryAction.Sign}
                });

            var response3 = clicksign.CreateHook(document, "http://requestb.in/1ma7ggj1");

            Console.WriteLine($"{response1.HasErrors} {response1.Document.Key}");
            Console.WriteLine($"{response2.HasErrors} {response2.Document.Key}");
            Console.WriteLine($"{response3.HasErrors} {response3.Id}");
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