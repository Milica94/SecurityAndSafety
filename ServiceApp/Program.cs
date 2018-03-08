using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using Contracts;
using System.IdentityModel.Policy;
using SecurityManager;
using System.Threading;
using Manager;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;

namespace ServiceApp
{
	public class Program
    {
        public static List<string> serveri = new List<string>();

        public static string trenutna_adresa;
        public static List<Baza> Baza_podataka = new List<Contracts.Baza>();
        public static Queue<Baza> update = new Queue<Baza>();
        public static Queue<bool> provera = new Queue<bool>();

		static void Main(string[] args)
		{
           

            Baza_podataka = napraviBazu();

            serveri.Add(  "net.tcp://localhost:8888/WCFService");
            serveri.Add( "net.tcp://localhost:8887/WCFService");
            serveri.Add("net.tcp://localhost:8886/WCFService");
     
            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            string address = args[0];

			ServiceHost host = new ServiceHost(typeof(WCFService));
			host.AddServiceEndpoint(typeof(IWCFService), binding, address);
            host.Credentials.ServiceCertificate.Certificate = Manager.CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);
            Console.WriteLine(Manager.CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN));
            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            ///If CA doesn't have a CRL associated, WCF blocks every client because it cannot be validated
            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

          
            
            NetTcpBinding binding1 = new NetTcpBinding();
            binding1.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            trenutna_adresa = args[1];
            ServiceHost host1 = new ServiceHost(typeof(Replicator));
            host1.AddServiceEndpoint(typeof(IReplicator), binding1, trenutna_adresa);
            host1.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host1.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            


            Thread t1 = new Thread(new ThreadStart(Replicator));
            t1.Start();


            
        
           
           
			host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
			host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

             ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation =  AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;
            newAudit.SuppressAuditFailure = true;
            

            host.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            host.Description.Behaviors.Add(newAudit);
            host1.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            host1.Description.Behaviors.Add(newAudit);



			host.Open();
            host1.Open();
			Console.WriteLine("WCFService is opened. Press <enter> to finish...");
			Console.ReadLine();

			host.Close();
		}

        private static void Replicator()
        {

            Thread.Sleep(50000);

            serveri.Remove(trenutna_adresa);
            do
            {
                
               
                    
                        NetTcpBinding binding = new NetTcpBinding();
                        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

                        EndpointAddress address = new EndpointAddress(new Uri(serveri[0]));

                        using (Klijent proxy = new Klijent(binding, address))
                        {



                            proxy.Update(update, provera);
                            
                        }
                            NetTcpBinding binding1 = new NetTcpBinding();
                            binding1.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

                            EndpointAddress address1 = new EndpointAddress(new Uri(serveri[1]));
                        
                        using (Klijent proxy1 = new Klijent(binding1, address1))
                        {



                            proxy1.Update(update, provera);
                            
                            update.Clear();
                            provera.Clear();




                        

                    

                     }
                Thread.Sleep(10000);
            } while (true);
         
            
             
            }

        

   


        private static List<Baza> napraviBazu()
        {

           

             List< Baza> baze = new List<Contracts.Baza>();

            /////
            Contracts.Baza baza1 = new Contracts.Baza();
            baza1.Id = 1;
            baza1.Godina = 2016;
            baza1.Grad = "Novi Sad";
            baza1.Region = "juzni";
            baza1.Potrosnja = new List<double>();
            for (int i = 0; i < 12; i++)
            {
                baza1.Potrosnja.Add(i+2);
            }


            Contracts.Baza baza2 = new Contracts.Baza();
            baza2.Id = 2;
            baza2.Godina = 2016;
            baza2.Grad = "Subotica";
            baza2.Region = "severni";
            baza2.Potrosnja = new List<double>();
            for (int i = 0; i < 12; i++)
            {
                baza2.Potrosnja.Add(i+1);
            }
            baze.Add(baza1); 
            baze.Add(baza2);


            Contracts.Serijalizacija ser = new Contracts.Serijalizacija();

            ser.SerializeObject<List<Baza>>(baze, "../../bin/data.txt");


            return baze;

           


        }


	}


}

   

