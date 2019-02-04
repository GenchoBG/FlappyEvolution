using System;
using System.Collections.Generic;
using SimpleNeuralNetwork.Helpers;
using SimpleNeuralNetwork.Interfaces;

namespace SimpleNeuralNetwork
{
    public class TrainableNetwork : Network, ITrainableNetwork<TrainableNetwork>
    {
        public TrainableNetwork(int inputsCount, ICollection<int> hiddenLayersCounts, int outputsCount, Func<double, double> activationFuction = null) : base(inputsCount, hiddenLayersCounts, outputsCount, activationFuction)
        {
        }

        public void Mutate(double rate)
        {
            var rateInPercent = rate * 100;

            foreach (var layerWeights in this.Weights)
            {
                foreach (var neuronWeights in layerWeights)
                {
                    for (int i = 0; i < neuronWeights.Count; i++)
                    {
                        var chance = this.Random.NextDouble(0, 100);
                        if (chance < rateInPercent)
                        {
                            neuronWeights[i] += this.Random.NextDouble(-1, 1);
                        }
                    }
                }
            }

            foreach (var layerBiases in this.Biases)
            {
                for (int i = 0; i < layerBiases.Count; i++)
                {
                    var chance = this.Random.NextDouble(0, 100);
                    if (chance < rateInPercent)
                    {
                        layerBiases[i] += this.Random.NextDouble(-1, 1);
                    }
                }
            }
        }

        //for the sake of simplicity we crossover two NNs with identical structure and only adjust the weights and biases
        public TrainableNetwork Crossover(TrainableNetwork other)
        {
            var network = new TrainableNetwork(this.InputsCount, this.HiddenLayersCounts, this.OutputsCount);

            for (int layerIndex = 0; layerIndex < this.HiddenLayersCount + 1; layerIndex++)
            {
                for (int neuronIndex = 0; neuronIndex < this.Weights[layerIndex].Count; neuronIndex++)
                {
                    for (int weightIndex = 0; weightIndex < this.Weights[layerIndex][neuronIndex].Count; weightIndex++)
                    {
                        var copyWeights = this.Random.NextBool() ? this.Weights : other.Weights;

                        network.Weights[layerIndex][neuronIndex][weightIndex] = copyWeights[layerIndex][neuronIndex][weightIndex];
                    }

                    var copyBiases = this.Random.NextBool() ? this.Biases : other.Biases;

                    network.Biases[layerIndex][neuronIndex] = copyBiases[layerIndex][neuronIndex];
                }
            }

            return network;
        }
    }
}
