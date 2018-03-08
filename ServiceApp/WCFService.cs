using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;
using SecurityManager;
using System.Threading;
using System.Security.Permissions;
using System.ServiceModel;

namespace ServiceApp
{
	public class WCFService : IWCFService
	{
        
        
        public List<Baza> Read()
		{
            /// audit successfull authorization checks


            string name = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            string role = Manager.CertManager.GetRole(name);

            string ime_klijenta = Manager.CertManager.GetName(name);
            
          
                if (role=="Reader")
                {

                    Console.WriteLine("Read() passed for user {0}.", ime_klijenta);
                    return ServiceApp.Program.Baza_podataka;

                }
                else
                {

                    Console.WriteLine(" SrednjaVrednost() failed for user {0}.", ime_klijenta);
                    return null;
                }	

            


        }

        public double SrednjaVrednost(string grad)
		{
            double srednja=0;

			

            string name = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            string role = Manager.CertManager.GetRole(name);
            string ime_klijenta=Manager.CertManager.GetName(name);
           
           

            if (role == "Reader")
			{

                Console.WriteLine(" SrednjaVrednost() passed for user {0}.", ime_klijenta);
                
                List<Baza> podaci = new List<Baza>();
                podaci = ServiceApp.Program.Baza_podataka;
                for (int i = 0; i < podaci.Count; i++)
                {
                    if (podaci[i].Grad == grad)
                    {
                        srednja = podaci[i].Potrosnja.Average();
                    }
                }


                return srednja;
			}
			else
			{

                Console.WriteLine(" SrednjaVrednost() failed for user {0}.", ime_klijenta);
                srednja = 0;
                return srednja;
			}	
			
		}

        public bool Azuriranje(List<double> lista,int id)
		{
			bool allowed = false;


            string serviceName = OperationContext.Current.IncomingMessageHeaders.Action;
            string name = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            string role = Manager.CertManager.GetRole(name);
            string ime_klijenta = Manager.CertManager.GetName(name);


            if (role == "Editor")
			{
                Audit.AuthorizationSuccess(ime_klijenta, serviceName);//dodato
                Console.WriteLine("Azuriranje() passed for user {0}.", ime_klijenta);
				allowed = true;
              
                for (int i = 0; i < ServiceApp.Program.Baza_podataka.Count; i++)
                {
                    if (ServiceApp.Program.Baza_podataka[i].Id == id)
                    {
                        
                        ServiceApp.Program.Baza_podataka[i].Potrosnja = lista;
                        ServiceApp.Program.update.Enqueue(ServiceApp.Program.Baza_podataka[i]);
                        ServiceApp.Program.provera.Enqueue(true);
                    }

                }
                Contracts.Serijalizacija ser = new Contracts.Serijalizacija();
                ser.SerializeObject<List<Baza>>(ServiceApp.Program.Baza_podataka, "../../bin/data.txt");
                return allowed;

			}
			else
			{
                
                Audit.AuthorizationFailed(ime_klijenta, serviceName, "azuriranje");
                Console.WriteLine("Azuriranje() failed for user {0}.", ime_klijenta);
                return allowed;
			}	

		
		}


        public bool Modify(Baza b)
        {
            bool allowed = false;

          

            

            string serviceName = OperationContext.Current.IncomingMessageHeaders.Action;
            string name = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            string role = Manager.CertManager.GetRole(name);
            string ime_klijenta = Manager.CertManager.GetName(name);


            if (role == "Admini")
            {
              
                
                ServiceApp.Program.Baza_podataka.Add(b);
                ServiceApp.Program.update.Enqueue(b);
                ServiceApp.Program.provera.Enqueue(false);

                Contracts.Serijalizacija ser = new Contracts.Serijalizacija();
                ser.SerializeObject<List<Baza>>(ServiceApp.Program.Baza_podataka, "../../bin/data.txt");

                Audit.AuthorizationSuccess(ime_klijenta, serviceName);
                Console.WriteLine("Modify() passed for user {0}.", ime_klijenta);
                allowed = true;
                return allowed;


            }
            else
            {
               
                Audit.AuthorizationFailed(ime_klijenta, serviceName, "modify");
                Console.WriteLine("Modify() failed for user {0}.", ime_klijenta);
                return allowed;
            }

            
        }
		
	}
}
