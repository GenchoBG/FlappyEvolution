using System.Collections.Generic;

namespace SimpleNeuralNetwork
{
    public interface INetwork
    {
        IList<double> FeedForward(ICollection<double> input);
    }
}
