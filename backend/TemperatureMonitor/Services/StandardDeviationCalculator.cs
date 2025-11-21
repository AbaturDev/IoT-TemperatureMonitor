namespace TemperatureMonitor.Services;

public static class StandardDeviationCalculator
{
    public static double StandardDeviation(int count, double? max, double? min)
    {
        if (!max.HasValue || !min.HasValue || count <= 0)
            return 0;

        var range = max.Value - min.Value;

        if (range < 0)
            return 0;

        var result = range / Math.Sqrt(count);
        if (double.IsNaN(result) || double.IsInfinity(result))
            return 0;

        return result;
    }
}
