using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQService.Abstract
{
    public interface IRabbitMQService
    {
        void SendMessage(string exchangeName, string routingKey, string message);
        void ReceiveMessage(string exchangeName, string queueName, string routingKey, Action<string> messageHandler);
    }
}
