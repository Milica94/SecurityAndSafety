using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class CertManager
    {

        public static X509Certificate2 GetCertificateFromStorage(StoreName storeName, StoreLocation storeLocation, string subjectName)
        {
            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, false);

            /// Check whether the subjectName of the certificate is exactly the same as the given "subjectName"
            foreach (X509Certificate2 c in certCollection)
            {
                if (c.SubjectName.Name.Contains(string.Format("CN={0}", subjectName)))

                { 
                    
                    return c;
                }
            }

            return null;
        }
        public static string GetRole(string name)
        {
          
            string[] parts = new string[] { };
            parts = name.Split(',');
            string[] parts1 = new string[] { };
            parts1 = parts[1].Split('=');
            return parts1[1];
        }
        public static string GetName(string name)
        {

            string[] parts = new string[] { };
            parts = name.Split(',');
            string[] parts1 = new string[] { };
            parts1 = parts[0].Split('=');
            return parts1[1];
        }

    }
}
