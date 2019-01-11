using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleNeuralNetwork
{
    public class Network : INetwork
    {
        private readonly int inputsCount;
        private readonly int hiddenLayersCount;
        private readonly IList<int> hiddenLayersCounts;
        private readonly int outputsCount;
        private readonly IList<IList<IList<double>>> weights;
        private readonly IList<IList<double>> biases;
        private readonly Random random;

        public Network(int inputsCount, ICollection<int> hiddenLayersCounts, int outputsCount)
        {
            this.inputsCount = inputsCount;
            this.outputsCount = outputsCount;
            this.hiddenLayersCount = hiddenLayersCounts.Count;
            this.hiddenLayersCounts = hiddenLayersCounts.ToList();
            this.weights = new List<IList<IList<double>>>();
            this.biases = new List<IList<double>>();
            this.random = new Random();

            this.InitializeWeights();
            this.InitializeBiases();
        }

        public IList<double> FeedForward(ICollection<double> input)
        {
            if (input.Count != this.inputsCount)
            {
                throw new ArgumentException("Invalid number of input arguments passed");
            }

            var inputs = input.ToList();
            for (int layer = 0; layer < this.hiddenLayersCount + 1; layer++)
            {
                var layerWeights = this.weights[layer];
                var layerBiases = this.biases[layer];
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


                    layerOutput.Add(neuronOutput);
                }

                inputs = layerOutput;
            }

            return inputs;
        }

        private void InitializeBiases()
        {
            for (int weightsIndex = 0; weightsIndex < this.hiddenLayersCount + 1; weightsIndex++)
            {
                var currentLayerBiases = new List<double>();

                var currentLayerNeurons = this.GetLayerNeuronsCount(weightsIndex);

                for (int neuronIndex = 0; neuronIndex < currentLayerNeurons; neuronIndex++)
                {
                    currentLayerBiases.Add(this.random.NextDouble(-2, 2));
                }

                this.biases.Add(currentLayerBiases);
            }
        }

        private void InitializeWeights()
        {
            for (int weightsIndex = 0; weightsIndex < this.hiddenLayersCount + 1; weightsIndex++)
            {
                var currentLayerWeights = new List<IList<double>>();

                var currentLayerNeurons = this.GetLayerNeuronsCount(weightsIndex);
                var prevLayerNeurons = this.GetPrevLayerNeuronsCount(weightsIndex);

                for (int neuronIndex = 0; neuronIndex < currentLayerNeurons; neuronIndex++)
                {
                    currentLayerWeights.Add(this.GenerateRandomWeights(prevLayerNeurons));
                }

                this.weights.Add(currentLayerWeights);
            }
        }

        private IList<double> GenerateRandomWeights(int count)
        {
            var result = new List<double>(count);

            for (int i = 0; i < count; i++)
            {
                result.Add(this.random.NextDouble(-2, 2));
            }

            return result;
        }

        private int GetPrevLayerNeuronsCount(int weightsIndex)
        {
            if (weightsIndex == 0)
            {
                return this.inputsCount;
            }

            return this.hiddenLayersCounts[weightsIndex - 1];
        }

        private int GetLayerNeuronsCount(int weightsIndex)
        {
            if (weightsIndex == this.hiddenLayersCount)
            {
                return this.outputsCount;
            }

            return this.hiddenLayersCounts[weightsIndex];
        }
    }
}
