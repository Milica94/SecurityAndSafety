using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contracts
{
   public class Baza
    {
     
       private string region;
       private string grad;
       private int godina;
       private List<double> potrosnja;       
       private int id;

       

 
       
       public int Id
       {
           get { return id; }
           set { id = value; }
       }

       public string Region
       {
           get { return region; }
           set { region = value; }
       }

       public string Grad
       {
           get { return grad; }
           set { grad = value; }
       }

       public int Godina
       {
           get { return godina; }
           set { godina = value; }
       }

       public List<double> Potrosnja
       {
           get { return potrosnja; }
           set { potrosnja = value; }
       }
    }
}
