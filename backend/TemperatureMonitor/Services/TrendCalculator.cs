namespace TemperatureMonitor.Services;

public static class TrendCalculator
{
    public static double[] CalculateLinearTrend(double[] y)
    {
        var n = y.Length;
        var x = Enumerable.Range(0, n).Select(i => (double)i).ToArray();

        var sumX = x.Sum();
        var sumY = y.Sum();
        var sumXy = x.Zip(y, (xi, yi) => xi * yi).Sum();
        var sumXx = x.Sum(xi => xi * xi);

        var a = (n * sumXy - sumX * sumY) / (n * sumXx - sumX * sumX);
        var b = (sumY - a * sumX) / n;

        return x.Select(xi => a * xi + b).ToArray();
    }
}
