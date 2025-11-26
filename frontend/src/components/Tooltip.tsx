import type { MeasurementWithTrend } from "../types/types";

type TooltipData = {
  active: boolean;
  payload: Array<{
    value: number;
    name: string;
    payload: MeasurementWithTrend;
  }>;
  label: string | number;
};

const CustomTooltip: React.FC<TooltipData> = ({ active, payload, label }) => {
  if (!active || !payload || payload.length === 0) return null;

  const data = payload[0].payload;

  return (
    <div
      style={{
        backgroundColor: "#19222e",
        padding: "10px",
        borderRadius: "10px",
        border: "2px solid #2f4158",
        color: "white",
      }}
    >
      <div>
        <strong>{new Date(label!).toLocaleString()}</strong>
      </div>
      <div>Średnia: {data.temperatureAvg?.toFixed(1)}°C</div>
      <div>Min: {data.temperatureMin?.toFixed(1)}°C</div>
      <div>Max: {data.temperatureMax?.toFixed(1)}°C</div>
      <div>Wilgotność: {data.humidity?.toFixed(1)}%</div>
      <div>Trend: {data.temperatureTrend?.toFixed(2)}</div>
      <div>Odchylenie: {data.temperatureStdDev?.toFixed(2)}</div>
    </div>
  );
};

export default CustomTooltip;
