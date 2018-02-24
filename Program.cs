using System;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

using System.IO;
using Microsoft.Extensions.Configuration;

namespace BC2Flow
{
    class Program
    {
        private static MqttClient _mqttClient = new MqttClient("localhost");
        private static IConfiguration _configuration { get; set; }
        
        static void Main(string[] args)
        {
            /*
                connection.json

                {
                    "connectionString": ""
                }
             */
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("connection.json");
            _configuration = builder.Build();
            
            _mqttClient.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            _mqttClient.Connect("BC01");
            
            if (_mqttClient.IsConnected)
            {
                _mqttClient.Subscribe(new string[] { "#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
                Console.WriteLine("MQTT connected");
            }

            while (true) {}
        }

        private static void Client_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            string[] topicParts = e.Topic.Split('/');

            if (topicParts.Length != 5)
            {
                return;
            }

            string key = topicParts[4];
            string value = System.Text.Encoding.Default.GetString(e.Message);

            string data = "{'" + key + "':" + value + "}";            

            Console.WriteLine(data);            
        } 
    }
}
