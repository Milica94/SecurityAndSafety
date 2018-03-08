using Contracts;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;

namespace ServiceApp
{
    public class Replicator :  IReplicator

    {
       

    
        public void Update(Queue<Baza> upd, Queue<bool> b)
        {
            bool provera;
           
            if (upd.Count > 0)
            {
                Console.WriteLine("REPLICATION: {0} values replicated.", upd.Count);
            }

            if (upd.Count == 0)
            {
              
            }else
            {
                for (int i = 0; i < upd.Count; i++)
                {
                    provera = b.Dequeue();

                 if (provera == true)  
                 {
                    Baza trenutna = new Baza();

                    trenutna = upd.Dequeue();
                 
                    ServiceApp.Program.Baza_podataka.RemoveAt(trenutna.Id - 1);
                    ServiceApp.Program.Baza_podataka.Insert(trenutna.Id - 1, trenutna);
                    Contracts.Serijalizacija ser = new Contracts.Serijalizacija();
                    ser.SerializeObject<List<Baza>>(ServiceApp.Program.Baza_podataka, "../../bin/data.txt");
                    Console.WriteLine("Updated azur \n");

                    string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
                    Audit.AuthorizationSuccess(srvCertCN, "Database updated from other server");//dodato

                    }
                    else
                    {
                         Baza trenutna2 = new Baza();
                         trenutna2 = upd.Dequeue();

                         ServiceApp.Program.Baza_podataka.Add(trenutna2);
                         Contracts.Serijalizacija ser = new Contracts.Serijalizacija();
                         ser.SerializeObject<List<Baza>>(ServiceApp.Program.Baza_podataka, "../../bin/data.txt");
                         Console.WriteLine("Updated modify \n");

                       
                         string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
                         Audit.AuthorizationSuccess(srvCertCN, "Database updated from other server");//dodato



                    }
                }

            }
            
        }
    }
}
