﻿using System.Collections.Generic;

namespace SimpleNeuralNetwork.Interfaces
{
    public interface INetwork
    {
        IList<double> FeedForward(ICollection<double> input);
    }
}