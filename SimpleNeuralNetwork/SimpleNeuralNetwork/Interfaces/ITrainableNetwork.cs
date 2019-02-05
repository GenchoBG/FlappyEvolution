namespace SimpleNeuralNetwork.Interfaces
{
    public interface ITrainableNetwork : INetwork
    {
        void Mutate(double rate);
        ITrainableNetwork Crossover(ITrainableNetwork other);
    }
}
