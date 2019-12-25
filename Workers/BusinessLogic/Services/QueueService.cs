﻿using BusinessLogic.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.RabbitMq.QueueInterfaces;
using Shared.RabbitMq.QueueModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class QueueService : IQueueService, IDisposable
    {
        private readonly IMessageConsumerScope _messageConsumerScope;
        private readonly IHabr habr;
        public QueueService(IMessageConsumerScopeFactory messageConsumerScopeFactory, IHabr habr)
        {
            this.habr = habr;
            this._messageConsumerScope = messageConsumerScopeFactory.Connect(new MessageScopeSettings
            {
                ExchangeName = "ServerExchange",
                ExchangeType = ExchangeType.Topic,
                QueueName = "SendValueQueue",
                RoutingKey = "topic.queue"
            });
            _messageConsumerScope.MessageConsumer.Received += MessageReceived;
        }

        private void MessageReceived(object sender, BasicDeliverEventArgs e)
        {
            var processed = false;
            try
            {
                Parse();
                var value = Encoding.UTF8.GetString(e.Body);
                Console.WriteLine($"Received {value}");
                processed = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                processed = false;
            }
            finally
            {
                _messageConsumerScope.MessageConsumer.SetAcknowledge(e.DeliveryTag, processed);
            }
        }
        public async Task Parse()
        {
            var pages = await habr.ParseMainPages(2);
            await habr.ParseConcretePages(pages);
        }
        public void Dispose()
        {
            _messageConsumerScope.Dispose();
        }
    }
}
