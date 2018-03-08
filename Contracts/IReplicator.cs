using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Contracts
{
    [ServiceContract]
    public interface IReplicator
    {
        [OperationContract]
        void Update(Queue<Baza> b, Queue<bool> a);
    }
}
