using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Contracts;
using SecurityManager;
using Manager;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;

namespace ClientApp
{
	public class WCFClient : ChannelFactory<IWCFService>, IWCFService, IDisposable
	{
		IWCFService factory;

		public WCFClient(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
        {
            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);
            Console.WriteLine(CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN));
            

            factory = this.CreateChannel();
		}



        public List<Baza> Read()
		{
            List<Baza> lista = new List<Baza>();

			try
			{
				 lista= factory.Read();
                 if (lista == null)
                 {
                     Console.WriteLine("Read() DENIED}");
                 }
                 else
                 {
                     Console.WriteLine("Sadrzaj database:\n");
                     for (int i = 0; i < lista.Count; i++)
                     {
                         Console.WriteLine("ID: " + lista[i].Id + " Grad: " + lista[i].Grad + " Region: " + lista[i].Region + " Godina: " + lista[i].Godina);
                         Console.WriteLine("Potrosnja:\n Januar: " + lista[i].Potrosnja[0] + "\nFebruar: " + lista[i].Potrosnja[1] + "\nMart: " + lista[i].Potrosnja[2] + "\nApril: " + lista[i].Potrosnja[3] + "\nMaj: " + lista[i].Potrosnja[4] + "\nJun: " + lista[i].Potrosnja[5] + "\nJul: " + lista[i].Potrosnja[6] + "\nAvgust: " + lista[i].Potrosnja[7] + "\nSeptembar: " + lista[i].Potrosnja[8] + "\nOktobar: " + lista[i].Potrosnja[9] + "\nNovembar: " + lista[i].Potrosnja[10] + "\nDecembar: " + lista[i].Potrosnja[11] + "\n");


                     }
                 }
				
			}
            catch (FaultException e)
            {
                Console.WriteLine("Error while trying to Read(). {0}", e.Message);
            }
           

			return lista;
		}

		

        public double SrednjaVrednost(string grad)
		{
            double srednja;

			try
			{
                srednja = factory.SrednjaVrednost(grad);
                if(srednja==0)
                {
                    Console.WriteLine("srednja DENIED");
                }
                else
                {
                    Console.WriteLine("Prosecna godisnja potrosnja za grad: " + grad + " je " + srednja);

                }
                
			}
			catch (Exception e)
			{
				Console.WriteLine("Error while trying to srednja vr(). {0}", e.Message);
			}

            return 0;
		}

		

        public bool Azuriranje(List<double> lista, int id)
		{
			bool allowed = false;
			try
			{
                allowed = factory.Azuriranje(lista, id);
                Console.WriteLine("azur() >> {0}", allowed);
			}
			catch (FaultException e)
			{
				Console.WriteLine("Error while trying to azur(). {0}", e.Message);
			}

			return allowed;
		}

	
        public bool Modify(Baza b)
		{
			bool allowed = false;
			try
			{
                allowed = factory.Modify(b);
                Console.WriteLine("modify() >> {0}", allowed);
			}
			catch (FaultException e)
			{
				Console.WriteLine("Error while trying to modify(). {0}", e.Message);
			}

			return allowed;
		}

		

		public void Dispose()
		{
			if (factory != null)
			{
				factory = null;
			}

			this.Close();
		}
	}
}
