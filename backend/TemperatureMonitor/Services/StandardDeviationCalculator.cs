namespace TemperatureMonitor.Services;

public static class StandardDeviationCalculator
{
    public static double StandardDeviation(IEnumerable<double?> values)
    {
        var data = values
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .ToList();

        if (data.Count == 0)
            return 0;

        double mean = data.Average();

        double sumOfSquares = data.Sum(x => Math.Pow(x - mean, 2));

        double result = Math.Sqrt(sumOfSquares / data.Count);

        if (double.IsNaN(result) || double.IsInfinity(result))
            return 0;

        return result;
    }
}