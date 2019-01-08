using System.Collections.Generic;

namespace SimpleNeuralNetwork
{
    public interface INetwork
    {
        int FeedForward(ICollection<double> input);
    }
}
