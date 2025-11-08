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
  temperature: number;
  humidity: number;
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
          <Line
            yAxisId="left"
            type="monotone"
            dataKey="temperature"
            stroke="#ff7300"
            dot={false}
            name="Temperatura"
          />
          <Line
            yAxisId="right"
            type="monotone"
            dataKey="humidity"
            stroke="#387908"
            dot={false}
            name="Wilgotność"
          />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
};

export default WeatherChart;
