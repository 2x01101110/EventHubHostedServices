using System;
using System.Collections.Generic;
using System.Text;

namespace EventHubHostedServices.BuildingBlocks.Models
{
    public class Condition
    {
        public string Operator { get; }
        public string Aggregate { get; }
        public double Value { get; }

        public Condition(
            string @operator, 
            string aggregate, 
            double value)
        {
            this.Operator = @operator;
            this.Aggregate = aggregate;
            this.Value = value;
        }

        public override string ToString() => $"{this.Operator} {this.Value}";
    }
}
