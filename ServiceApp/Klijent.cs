using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ServiceApp
{
    public class Klijent : ChannelFactory<IReplicator>, IReplicator, IDisposable
    {
  
        IReplicator factory;

		public Klijent(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
        {

            factory = this.CreateChannel();
		}

        public void Update(Queue<Baza> upd, Queue<bool> b)
        {
            factory.Update(upd,b);
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
