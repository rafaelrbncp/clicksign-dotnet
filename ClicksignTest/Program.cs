using System.IO;
using System.Linq;
using Clicksign;

namespace ClicksignTest
{
    class Program
    {
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();

            Get();
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

            File.WriteAllBytes("Download-Clicksign.zip",downloadResponse.binaryFile);
        }

    }
}
