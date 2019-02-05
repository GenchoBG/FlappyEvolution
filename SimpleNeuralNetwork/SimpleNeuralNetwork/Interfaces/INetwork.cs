using System.Collections.Generic;

namespace SimpleNeuralNetwork.Interfaces
{
    public interface INetwork
    {
        int InputsCount { get; }
        int HiddenLayersCount { get; }
        IList<int> HiddenLayersCounts { get; }
        int OutputsCount { get; }

        //i don't like these being public at all >:(
        IList<IList<IList<double>>> Weights { get; }
        IList<IList<double>> Biases { get; }

        IList<double> FeedForward(ICollection<double> input);
    }
}
