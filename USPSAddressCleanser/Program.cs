using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.IO;


namespace USPSAddressCleanser
{
    class Program
    {
        static void Main(string[] args)
        {
            AddressCleanser cleanser = new AddressCleanser();
            List<string> Address = new List<string>();

            Console.WriteLine("Enter address Line0");
            string AddressLine0 = Console.ReadLine().Replace(" ", "+");

            Console.WriteLine("Enter address Line1");
            string AddressLine1 = Console.ReadLine().Replace(" ", "+");

            Console.WriteLine("Enter address Line2");
            string AddressLine2 = Console.ReadLine().Replace(" ", "+");

            Console.WriteLine("Enter address city");
            string City = Console.ReadLine().Replace(" ", "+");

            Console.WriteLine("Enter address state");
            string State = Console.ReadLine().Replace(" ", "+");

            string[] tempaddr = { AddressLine0, AddressLine1, AddressLine2, City, State };

            Address.AddRange(tempaddr);
            List<string> CleansedAddress = cleanser.StandardizeAddress(Address);

            Console.WriteLine(Environment.NewLine + "USPS Standardized Address: " + Environment.NewLine);
            foreach(string element in CleansedAddress)
            {
                Console.WriteLine(element);
                Console.WriteLine(Environment.NewLine);
            }
            Console.WriteLine(Environment.NewLine + "Press any key to exit...");
            Console.ReadLine();

        }
    }


    class AddressCleanser
    {
        public List<string> StandardizeAddress(List<string> addr)
        {
            string baseurl = "http://tools.usps.com/go/ZipLookupResultsAction!input.action?resultMode=1&companyName=[ADDRESS_LINE_0]&address1=[ADDRESS_LINE_1]&address2=[ADDRESS_LINE_2]&city=[CITY]&state=[STATE]&urbanCode=&postalCode=&zip=";
            baseurl = baseurl.Replace("[ADDRESS_LINE_0]", addr[0]);
            baseurl = baseurl.Replace("[ADDRESS_LINE_1]", addr[1]);
            baseurl = baseurl.Replace("[ADDRESS_LINE_2]", addr[2]);
            baseurl = baseurl.Replace("[CITY]", addr[3]);
            baseurl = baseurl.Replace("[STATE]", addr[4]);

            List<string> cleansedAddress = new List<string>();
            try
            {
                using (WebClient wc = new WebClient())
                {
                    string uspshtml = wc.DownloadString(baseurl);
                    HtmlAgilityPack.HtmlDocument usps = new HtmlAgilityPack.HtmlDocument();
                    usps.LoadHtml(uspshtml);
                    string AddressLine1 = usps.DocumentNode.SelectSingleNode(".//*[@id='submitted']/p/span[1]/span/text()").InnerText;
                    string City = usps.DocumentNode.SelectSingleNode(".//*[@id='result-list']/ul/li/div/p[1]/span[2]/text()").InnerText;
                    string State = usps.DocumentNode.SelectSingleNode(".//*[@id='result-list']/ul/li/div/p[1]/span[3]").InnerText;
                    string Zip = usps.DocumentNode.SelectSingleNode(".//*[@id='result-list']/ul/li/div/p[1]/span[4]").InnerText;
                    string Zip4 = usps.DocumentNode.SelectSingleNode(".//*[@id='result-list']/ul/li/div/p[1]/span[6]").InnerText;

                    cleansedAddress.AddRange(new string[] { AddressLine1, City, State, Zip, Zip4 });

                }
            }

            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return cleansedAddress;
        }


    }
}
