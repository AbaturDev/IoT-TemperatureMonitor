import { useEffect, useState } from "react";
import { API_URL } from "../config";
import WeatherChart from "./WeatherChart";
import "./Dashboard.css";
import type {
  Measurement,
  MeasurementWithTrend,
  SensorStatus,
  Status,
  Mode,
} from "../types/types";
import Loading from "./Loading";

export function Dashboard() {
  const [live, setLive] = useState<Measurement | null>(null);
  const [status, setStatus] = useState<Status>("Offline");
  const [measurements, setMeasurements] = useState<MeasurementWithTrend[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [mode, setMode] = useState<Mode>(0);

  const handleSetStatus = (e: number) => {
    if (e == 0) setStatus("Online");
    else if (e == 1) setStatus("Offline");
    else if (e == 2) setStatus("Server Error");
  };

  const refresh = async () => {
    fetchMeasurements();
    fetchSensorStatus();
    fetchLatestMeasurement();
  };

  const fetchMeasurements = async () => {
    const endDate: Date = new Date();
    let startDate: Date;
    switch (mode) {
      case 0:
        startDate = new Date(endDate.getTime() - 1 * 60 * 60 * 1000);
        break;

      case 1:
        startDate = new Date(endDate.getTime() - 4 * 60 * 60 * 1000);
        break;

      case 2:
        startDate = new Date(endDate.getTime() - 24 * 60 * 60 * 1000);
        break;

      case 3:
        startDate = new Date(endDate.getTime() - 7 * 24 * 60 * 60 * 1000);
        break;

      default:
        startDate = new Date(endDate.getTime() - 1 * 60 * 60 * 1000);
        break;
    }

    try {
      setLoading(true);
      const response = await fetch(
        `${API_URL}/api/measurements?startDate=${startDate.toISOString()}&endDate=${endDate.toISOString()}`
      );
      const data: MeasurementWithTrend[] = await response.json();
      setMeasurements(data);
    } catch (err) {
      console.log(err);
      handleSetStatus(2);
    } finally {
      setLoading(false);
    }
  };

  const fetchLatestMeasurement = async () => {
    try {
      setLoading(true);
      const response = await fetch(`${API_URL}/api/measurements/latest`);
      const data: Measurement = await response.json();
      setLive(data);
    } catch (err) {
      console.log(err);
      handleSetStatus(2);
    } finally {
      setLoading(false);
    }
  };

  const fetchSensorStatus = async () => {
    try {
      setLoading(true);
      const response = await fetch(`${API_URL}/api/sensor/latest`);
      const data: SensorStatus = await response.json();
      console.log(data);
      if (data.status != 0) {
        handleSetStatus(1);
      } else if (
        new Date(data.timestamp).getTime() <
        new Date().getTime() - 1 * 60 * 60 * 1000
      ) {
        handleSetStatus(1);
      } else {
        handleSetStatus(0);
      }
    } catch (err) {
      console.log(err);
      handleSetStatus(2);
    } finally {
      setLoading(false);
    }
  };

  const fetchEventSource = () => {
    try {
      const eventSource = new EventSource(`${API_URL}/api/live/measurements`);
      eventSource.onmessage = (e) => {
        const data = JSON.parse(e.data);
        setLive(data);
        fetchMeasurements();
      };
    } catch (err) {
      console.log(err);
      handleSetStatus(2);
    }
  };

  useEffect(() => {
    fetchEventSource();
    fetchLatestMeasurement();
    fetchSensorStatus();
    fetchMeasurements();
  }, []);

  useEffect(() => {
    fetchMeasurements();
  }, [mode]);

  useEffect(() => {
    const checkRateMs = 6 * 60 * 1000;

    const interval = setInterval(() => {
      if (!live) return;

      const liveTime = new Date(live.timestamp).getTime();
      const diff = Date.now() - liveTime;

      if (diff > checkRateMs) handleSetStatus(1);
    }, 30 * 1000);

    return () => clearInterval(interval);
  }, [live]);

  return (
    <div className="dashboard-container">
      <Loading isLoading={loading} />
      <div className="sensor-status-container">
        Sensor Status:{" "}
        <span
          className={
            status === "Online"
              ? "status online"
              : status === "Offline"
              ? "status offline"
              : "status error"
          }
        >
          {status}
        </span>
      </div>
      {live != null && (
        <>
          <div className="live-stats">
            <div className="date">
              <div className="day">
                {new Date(live.timestamp).toLocaleDateString("pl-PL", {
                  weekday: "long",
                })}
              </div>
              <div className="month">
                {new Date(live.timestamp).toLocaleDateString("pl-PL", {
                  day: "numeric",
                  month: "long",
                })}
              </div>
            </div>
            <div className="temperature-humidity">
              <div className="temperature">
                {live.temperatureAvg.toFixed(0)}°C
              </div>
              <div className="humidity">
                wilgotność: <span>{live.humidity.toFixed(0)}%</span>
              </div>
            </div>
          </div>
          <div className="live-last-update">
            Ostatnia aktualizacja:{" "}
            <span>
              {new Date(live.timestamp).toLocaleTimeString([], {
                hour: "2-digit",
                minute: "2-digit",
              })}
            </span>
          </div>
        </>
      )}
      <div>
        <div className="mode-switch">
          <button
            className={mode === 0 ? "enabled" : ""}
            onClick={() => setMode(0)}
          >
            1H
          </button>
          <button
            className={mode === 1 ? "enabled" : ""}
            onClick={() => setMode(1)}
          >
            4H
          </button>
          <button
            className={mode === 2 ? "enabled" : ""}
            onClick={() => setMode(2)}
          >
            1D
          </button>
          <button
            className={mode === 3 ? "enabled" : ""}
            onClick={() => setMode(3)}
          >
            1W
          </button>
        </div>
        <button className="refresh-button" onClick={() => refresh()}>
          Odśwież
        </button>
      </div>
      <div className="chart-container">
        {measurements.length > 0 ? (
          <WeatherChart data={measurements} />
        ) : (
          "Brak pomiarów przez ostatnie 12h"
        )}
      </div>
    </div>
  );
}
