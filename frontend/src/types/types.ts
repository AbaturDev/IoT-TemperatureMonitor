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
  temperatureStdDev: number;
};

export type SensorStatus = {
  timestamp: string;
  status: 0 | 1 | 2;
};
