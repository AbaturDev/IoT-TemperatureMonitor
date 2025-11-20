import React from "react";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from "recharts";

export type Measurement = {
  timestamp: string;
  temperatureAvg: number;
  temperatureMin: number;
  temperatureMax: number;
  humidity: number;
  count: number;
};

export type MeasurementWithTrend = {
  timestamp: string;
  temperatureAvg: number;
  temperatureMin: number;
  temperatureMax: number;
  humidity: number;
  count: number;
  temperatureTrend: number;
};

type WeatherChartProps = {
  data: Measurement[];
};

const WeatherChart: React.FC<WeatherChartProps> = ({ data }) => {
  return (
    <div style={{ width: "100%", height: "400px" }}>
      <ResponsiveContainer width="100%" height="100%">
        <LineChart data={[...data].reverse()}>
          <CartesianGrid strokeDasharray="1 25" />
          <XAxis
            dataKey="timestamp"
            tickFormatter={(t) =>
              new Date(t).toLocaleTimeString([], {
                hour: "2-digit",
                minute: "2-digit",
              })
            }
          />
          <YAxis
            yAxisId="left"
            label={{ value: "°C", position: "insideLeft" }}
          />
          <YAxis
            yAxisId="right"
            orientation="right"
            label={{ value: "%", position: "insideRight" }}
          />
          <Tooltip
            labelFormatter={(t) => new Date(t).toLocaleString()}
            formatter={(value: number) => value.toFixed(1)}
            contentStyle={{
              backgroundColor: "#19222e",
              color: "white",
              border: "2px solid #2f4158ff",
              borderRadius: "12px",
            }}
            itemStyle={{ color: "white" }}
          />
          <Legend />

          {/* linie */}
          <Line
            yAxisId="left"
            type="monotone"
            dataKey="temperatureMin" // temperatureAvg
            stroke="#ff7300"
            dot={false}
            name="Temperatura"
          />
          <Line
            yAxisId="right"
            type="monotone"
            dataKey="temperatureMax" // humidity
            stroke="#387908"
            dot={false}
            name="Wilgotność"
          />
          <Line
            yAxisId="left"
            type="monotone"
            dataKey="temperatureTrend"
            stroke="#346dadff"
            dot={false}
            strokeDasharray="5 5" // <-- to robi przerywaną linię
            name="Trend"
          />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
};

export default WeatherChart;
