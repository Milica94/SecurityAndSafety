using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Contracts
{
	[ServiceContract]
	public interface IWCFService
	{
		[OperationContract]
        List<Baza> Read();

		[OperationContract]
		double SrednjaVrednost(string grad);

		[OperationContract]
		bool Azuriranje(List<double> lista , int id); 

		[OperationContract]
		bool Modify(Baza b);
	}
}
