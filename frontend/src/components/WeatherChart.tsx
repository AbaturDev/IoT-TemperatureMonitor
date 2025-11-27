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
  ErrorBar,
} from "recharts";
import type { Measurement } from "../types/types";
import CustomTooltip from "./Tooltip";

type WeatherChartProps = {
  data: Measurement[];
};

const WeatherChart: React.FC<WeatherChartProps> = ({ data }) => {
  return (
    <div
      style={{
        width: "100%",
        minWidth: "200px",
        minHeight: "200px",
      }}
    >
      <ResponsiveContainer width="100%" aspect={3}>
        <LineChart data={[...data].reverse()}>
          <CartesianGrid strokeDasharray="1 25" />
          <XAxis
            dataKey="timestamp"
            tickFormatter={(t) =>
              new Date(t).toLocaleString("pl-PL", {
                day: "2-digit",
                month: "2-digit",
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
          <Legend />
          <Tooltip
            content={<CustomTooltip active={false} payload={[]} label={""} />}
          />

          <Line
            yAxisId="left"
            type="monotone"
            dataKey="temperatureAvg"
            stroke="#ff7300"
            dot={false}
            name="Temperatura"
          >
            <ErrorBar
              dataKey="temperatureStdDev"
              width={4}
              strokeWidth={1}
              direction="y"
              stroke="#ffffff6b"
            />
          </Line>
          <Line
            yAxisId="right"
            type="monotone"
            dataKey="humidity"
            stroke="#00ff7f"
            dot={false}
            name="Wilgotność"
          />
          <Line
            yAxisId="left"
            type="monotone"
            dataKey="temperatureTrend"
            stroke="#00ffff"
            dot={false}
            strokeDasharray="5 5"
            name="Trend"
          />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
};

export default WeatherChart;
