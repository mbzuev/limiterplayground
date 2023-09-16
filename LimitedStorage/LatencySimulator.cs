namespace LimitedStorage;

public class LatencySimulator
{
    // Constructor to set latency parameters
    private readonly Random _random = new Random();
    private double _mean;        // Mean of the distribution
    private double _stdDev;      // Standard deviation of the distribution
    // Constructor to set the distribution parameters
    public LatencySimulator(double mean, double stdDev)
    {
        _mean = mean;
        _stdDev = stdDev;
    }

    public void UpdateValues(double mean, double stdDev)
    {
        _mean = mean;
        _stdDev = stdDev;
    }

    public double GetLatency()
    {
        return _random.NextDouble();
        // // Generate a random number from a normal distribution
        // double randomNumber = _random.NextDouble();
        // double simulatedLatency = _mean + _stdDev * Math.Sqrt(-2.0 * Math.Log(randomNumber)) * Math.Cos(2.0 * Math.PI * _random.NextDouble());
        //
        // return (int)Math.Round(simulatedLatency);
    }
}