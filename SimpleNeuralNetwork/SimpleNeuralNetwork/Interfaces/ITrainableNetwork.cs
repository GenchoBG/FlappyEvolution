namespace SimpleNeuralNetwork.Interfaces
{
    public interface ITrainableNetwork<T> : INetwork
    {
        void Mutate(double rate);
        T Crossover(T other);
    }
}
