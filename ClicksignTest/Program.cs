using System.IO;
using System.Linq;

namespace ClicksignTest
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            Get();
        }

        private static void Get()
        {
            var clicksign = new Clicksign.Clicksign();

            var list = clicksign.List();

            var document = clicksign.Get(list.First().Key);

            var document2 = clicksign.Download(list.First().Key);

            File.WriteAllBytes("Download-Clicksign.zip",document2);
        }

    }
}
