using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace FrontendWebRole
{
    public class QueueConnector
    {
        /// <summary>
        /// Thread safe. Recommended to cache 
        /// rather than recreating it on every 
        /// request.
        /// </summary>
        public static QueueClient OrdersQueueClient;


        /// <summary>
        /// These values are defined in the Azure
        /// Management Portal.
        /// </summary>
        public const string Namespace = "SBInterRoleSample";
        public const string IssuerName = "owner";
        public const string IssuerKey = "2LjZKry7d2+MAKQ7ipdKXB7Rr/CgjaGMyHQyZ9S83Ao=";

        public const string QueueName = "OrdersQueue";

        /// <summary>
        /// Create the namespace manager which gives
        /// you access to management operations.
        /// </summary>
        /// <returns></returns>
        public static NamespaceManager CreateNamespaceManager()
        {
            var uri = ServiceBusEnvironment.CreateServiceUri("sb", Namespace, string.Empty);
            var tP = TokenProvider.CreateSharedSecretTokenProvider(IssuerName, IssuerKey);
            return new NamespaceManager(uri, tP);
        }

        public static void Initialize()
        {
            // Use Http to be friendly with outbound firewalls.
            ServiceBusEnvironment.SystemConnectivity.Mode =
                ConnectivityMode.Http;

            // Get an instance of the namespace manager
            // to get access to management operations.
            var namespaceManager = CreateNamespaceManager();

            // Create the queue if it does not exist already.
            if (!namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.CreateQueue(QueueName);
            }

            // Get a client to the queue.
            var messagingFactory = MessagingFactory.Create(
                namespaceManager.Address,
                namespaceManager.Settings.TokenProvider);
            OrdersQueueClient = messagingFactory.CreateQueueClient(QueueName);
        }
    }
}