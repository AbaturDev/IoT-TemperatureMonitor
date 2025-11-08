import { useEffect, useState } from "react";
import { API_URL } from "../config";
import WeatherChart from "./WeatherChart";
import type { Measurement } from "./WeatherChart";
import "./Dashboard.css";

type Status = "Online" | "Offline" | "Error";

export function Dashboard() {
  const [live, setLive] = useState<Measurement | null>(null);
  const [status, setStatus] = useState<Status>("Offline");
  const [measurements, setMeasurements] = useState<Measurement[]>([]);

  const handleSetStatus = (e: number) => {
    if (e == 0) setStatus("Online");
    else if (e == 1) setStatus("Offline");
    else if (e == 2) setStatus("Error");
  };

  const fetchMeasurements = async (
    startDate: Date,
    endDate: Date = new Date()
  ) => {
    try {
      const response = await fetch(
        `${API_URL}/api/measurements?startDate=${startDate.toISOString()}&endDate=${endDate.toISOString()}`
      );
      const data = await response.json();
      setMeasurements(data);
    } catch (err) {
      console.log(err);
    }
  };

  const fetchLatestMeasurement = async () => {
    try {
      const response = await fetch(`${API_URL}/api/measurements/latest`);
      const data: Measurement = await response.json();
      setLive(data);
    } catch (err) {
      console.log(err);
    }
  };

  const fetchLatestSensor = async () => {
    try {
      const response = await fetch(`${API_URL}/api/sensor/latest`);
      const data = await response.json();
      handleSetStatus(data.status);
    } catch (err) {
      console.log(err);
    }
  };

  const fetchEventSource = () => {
    try {
      const eventSource = new EventSource(`${API_URL}/api/live/measurements`);
      eventSource.onmessage = (e) => {
        const data = JSON.parse(e.data);
        setLive(data);
        console.log("live dat received: ", data);
      };
    } catch (err) {
      console.log(err);
    }
  };

  useEffect(() => {
    fetchEventSource();
    fetchLatestMeasurement();
    fetchLatestSensor();
    fetchMeasurements(new Date(new Date().getTime() - 24 * 60 * 60 * 1000));
  }, []);

  return (
    <div className="dashboard-container">
      <div className="sensor-status-container">
        Status:{" "}
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
      {live && (
        <div className="live-stats">
          {/* <div className="day">
            {new Date(live.timestamp).toLocaleDateString("pl-PL", {
              weekday: "long",
            })}
          </div>
          <div className="temperature-humidity">
            <div className="temperature">{live.temperature.toFixed(0)}C</div>
            <div className="humidity">{live.humidity.toFixed(0)}%</div>
          </div> */}
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
            <div className="temperature">{live.temperature.toFixed(0)}C</div>
            <div className="humidity">
              wilgotność: <span>{live.humidity.toFixed(0)}%</span>
            </div>
          </div>
        </div>
      )}
      <WeatherChart data={measurements} />
    </div>
  );
}
