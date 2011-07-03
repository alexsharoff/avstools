using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Reflection;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace AVS.Tools
{
    public static class TcpServiceHelper
    {
        public static ServiceHost CreateHost<ContractType, ServiceType>(ServiceType instance, string hostname, int port, bool streamed = false)
        {
            ServiceHost host = new ServiceHost(instance);
            NetTcpBinding bindingTransport = CreateBinding(streamed);
            host.AddServiceEndpoint(typeof(ContractType), bindingTransport,
                                    string.Format("net.tcp://{2}:{0}/{1}", port, typeof(ContractType).Name, hostname));
            host.Open();
            SetupBehaviours(host.Description.Behaviors);
            return host;
        }

        public static ServiceHost CreateHost<ContractType, ServiceType>(string hostname, int port, bool streamed = false)
        {
            ServiceHost host = new ServiceHost(typeof(ServiceType));
            NetTcpBinding bindingTransport = CreateBinding(streamed);
            host.AddServiceEndpoint(typeof(ContractType), bindingTransport,
                                    string.Format("net.tcp://{2}:{0}/{1}", port, typeof(ContractType).Name, hostname));

            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            host.Open();
            SetupBehaviours(host.Description.Behaviors);
            return host;
        }

        public static ContractType CreateChannel<ContractType>(string host, int port, bool streamed = false)
        {
            NetTcpBinding binding = CreateBinding(streamed);
            ChannelFactory<ContractType> serviceFactory =
                new ChannelFactory<ContractType>(binding,
                                                 new EndpointAddress(
                                                    new Uri(string.Format("net.tcp://{0}:{1}/{2}", host, port,
                                                                          typeof(ContractType).Name))));
            ContractType serviceProxy = serviceFactory.CreateChannel();
            return serviceProxy;
        }

        public static ContractType CreateChannel<ContractType, CallBackType>(
            string host, int port, CallBackType callback, bool streamed = false)
        {
            NetTcpBinding binding = CreateBinding(streamed);
            DuplexChannelFactory<ContractType> serviceFactory =
                new DuplexChannelFactory<ContractType>(
                    callback, binding,
                    new EndpointAddress(
                        new Uri(string.Format("net.tcp://{0}:{1}/{2}", host, port,
                                              typeof(ContractType).Name))));
            ContractType serviceProxy = serviceFactory.CreateChannel();
            return serviceProxy;
        }

        static void SetupBehaviours(KeyedByTypeCollection<IServiceBehavior> behaviours)
        {
            behaviours.Remove(typeof(ServiceDebugBehavior));
            behaviours.Remove(typeof(ServiceThrottlingBehavior));
            behaviours.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            behaviours.Add(new ServiceThrottlingBehavior()
            {
                MaxConcurrentCalls = 100,
                MaxConcurrentSessions = 100,
                MaxConcurrentInstances = 100
            });
        }
        static NetTcpBinding CreateBinding(bool streamed)
        {
            NetTcpBinding binding = new NetTcpBinding()
            {
                OpenTimeout = TimeSpan.FromSeconds(5),
                CloseTimeout = TimeSpan.FromSeconds(5),
                MaxBufferPoolSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue,
                ReceiveTimeout = new TimeSpan(0, 0, 30),
                SendTimeout = new TimeSpan(0, 0, 30),
            };
            if (streamed)
                binding.TransferMode = TransferMode.Streamed;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.ReaderQuotas.MaxDepth = int.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            binding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
            binding.Security = new NetTcpSecurity()
            {
                Mode = SecurityMode.None
            };
            return binding;
        }
    }
}
