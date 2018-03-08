using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Contracts;
using System.Security.Cryptography.X509Certificates;
using Manager;
using SecurityManager;
using System.Security.Principal;

namespace ClientApp
{
	public class Program
	{

        public static int A;
		static void Main(string[] args)
		{
            //string srvCertCN = "server1";
            string srvCertCN = args[0];


            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
      
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
         
          
            EndpointAddress address = new EndpointAddress(new Uri(args[1]),
                                      new X509CertificateEndpointIdentity(srvCert));

       
            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

          


            using (WCFClient proxy = new WCFClient(binding, address))
            {
              

                proxy.Read();
				proxy.SrednjaVrednost("Novi Sad");
                proxy.SrednjaVrednost("Subotica");

               
                Random rand = new Random();
                List<double> novaPotrosnja= new List<double>();
                for (int i = 0; i < 12; i++)
                {
                    novaPotrosnja.Add(rand.Next(100));
                }

				proxy.Azuriranje(novaPotrosnja, 2);

                Baza novaBaza = new Baza();
                novaBaza.Godina = 1223;
                novaBaza.Grad = "Beograd";
                novaBaza.Id = 3;
                novaBaza.Potrosnja = new List<double>();
                for (int i = 0; i < 12; i++)
                {
                    novaBaza.Potrosnja.Add(rand.Next(100));
                }
                novaBaza.Region = "Sumadija";

				proxy.Modify(novaBaza);
			}

			Console.ReadLine();
		}
	}
}
