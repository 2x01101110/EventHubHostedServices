using Azure.Messaging.EventHubs.Processor;
using EventHubHostedServices.BuildingBlocks.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventHubHostedServices.Host.Extensions
{
    public static class ProcessEventArgsExtension
    {
        public static Message GetSerializedMessage(this ProcessEventArgs args)
        {
            try
            {
                var message = Utf8Json.JsonSerializer.Deserialize<Message>(args.Data.BodyAsStream);
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
