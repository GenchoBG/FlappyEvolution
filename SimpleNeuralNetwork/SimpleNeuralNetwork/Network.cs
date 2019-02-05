using SimpleNeuralNetwork.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using SimpleNeuralNetwork.Interfaces;

namespace SimpleNeuralNetwork
{
    public class Network : INetwork
    {
        private readonly Func<double, double> activationFunction;

        public Network(int inputsCount, ICollection<int> hiddenLayersCounts, int outputsCount, Func<double, double> activationFuction = null)
        {
            this.InputsCount = inputsCount;
            this.OutputsCount = outputsCount;
            this.HiddenLayersCount = hiddenLayersCounts.Count;
            this.HiddenLayersCounts = hiddenLayersCounts.ToList();
            this.Weights = new List<IList<IList<double>>>();
            this.Biases = new List<IList<double>>();

            this.activationFunction = activationFuction ?? this.Sigmoid;

            this.InitializeWeights();
            this.InitializeBiases();
        }

        protected static Random Random { get; } = new Random();

        public int InputsCount { get; }
        public int HiddenLayersCount { get; }
        public IList<int> HiddenLayersCounts { get; }
        public int OutputsCount { get; }

        public IList<IList<IList<double>>> Weights { get; }
        public IList<IList<double>> Biases { get; }

        public IList<double> FeedForward(ICollection<double> input)
        {
            if (input.Count != this.InputsCount)
            {
                throw new ArgumentException("Invalid number of input arguments passed");
            }

            var inputs = input.ToList();
            for (int layer = 0; layer < this.HiddenLayersCount + 1; layer++)
            {
                var layerWeights = this.Weights[layer];
                var layerBiases = this.Biases[layer];
                var layerOutput = new List<double>();

                for (int neuronIndex = 0; neuronIndex < layerWeights.Count; neuronIndex++)
                {
                    var neuronWeights = layerWeights[neuronIndex];
                    var neuronBias = layerBiases[neuronIndex];

                    var neuronOutput = 0D;

                    for (int weightIndex = 0; weightIndex < neuronWeights.Count; weightIndex++)
                    {
                        neuronOutput += neuronWeights[weightIndex] * inputs[weightIndex];
                    }
                    neuronOutput += neuronBias;

                    layerOutput.Add(this.activationFunction(neuronOutput));
                }

                inputs = layerOutput;
            }

            return inputs;
        }

        private void InitializeBiases()
        {
            for (int weightsIndex = 0; weightsIndex < this.HiddenLayersCount + 1; weightsIndex++)
            {
                var currentLayerBiases = new List<double>();

                var currentLayerNeurons = this.GetLayerNeuronsCount(weightsIndex);

                for (int neuronIndex = 0; neuronIndex < currentLayerNeurons; neuronIndex++)
                {
                    currentLayerBiases.Add(Network.Random.NextDouble(-2, 2));
                }

                this.Biases.Add(currentLayerBiases);
            }
        }

        private void InitializeWeights()
        {
            for (int weightsIndex = 0; weightsIndex < this.HiddenLayersCount + 1; weightsIndex++)
            {
                var currentLayerWeights = new List<IList<double>>();

                var currentLayerNeurons = this.GetLayerNeuronsCount(weightsIndex);
                var prevLayerNeurons = this.GetPrevLayerNeuronsCount(weightsIndex);

                for (int neuronIndex = 0; neuronIndex < currentLayerNeurons; neuronIndex++)
                {
                    currentLayerWeights.Add(this.GenerateRandomWeights(prevLayerNeurons));
                }

                this.Weights.Add(currentLayerWeights);
            }
        }

        private IList<double> GenerateRandomWeights(int count)
        {
            var result = new List<double>(count);

            for (int i = 0; i < count; i++)
            {
                result.Add(Network.Random.NextDouble(-2, 2));
            }

            return result;
        }

        private int GetPrevLayerNeuronsCount(int weightsIndex)
        {
            if (weightsIndex == 0)
            {
                return this.InputsCount;
            }

            return this.HiddenLayersCounts[weightsIndex - 1];
        }

        private int GetLayerNeuronsCount(int weightsIndex)
        {
            if (weightsIndex == this.HiddenLayersCount)
            {
                return this.OutputsCount;
            }

            return this.HiddenLayersCounts[weightsIndex];
        }

        private double Sigmoid(double value)
        {
            return 1 / (1 + Math.Exp(-value));
        }
    }
}