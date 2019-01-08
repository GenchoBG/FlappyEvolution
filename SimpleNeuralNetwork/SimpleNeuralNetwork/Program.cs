using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleNeuralNetwork
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var test = new Network(2, new[] { 5 }, 1);

            var result = test.FeedForward()
        }
    }
}
