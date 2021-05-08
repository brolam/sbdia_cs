import React, { useState, useEffect } from 'react'
import DashboardToolbar from '../components/Toolbar'
import Chart from "chart.js";
var chartXy = null;
export default function EnergyLogDashboard(props) {

  const [CHART_XY_KWH, CHART_XY_DURATION] = ["CHART_XY_KWH", "CHART_XY_DURATION"];
  const toDay = new Date();
  const [dafaultYear, dafaultMonth, defautlDay] = [toDay.getFullYear(), toDay.getMonth() + 1, toDay.getDate()];
  const [stateLoading, setStateLoading] = useState(true);
  const [stateSensors, setStateSensors] = useState({ sensors: [], selectedSensor: null });
  const [stateXy, setStateXy] = useState({
    xyTotalKwh: [],
    xyTotalDuration: [],
    xyDays: [],
    logsRecent: [],
    totalKwh: 0.00,
    totalDuration: 0.00
  });
  const [stateSelectedXyDay, setStateSelectedXyDay] = useState({ year: dafaultYear, month: dafaultMonth, day: defautlDay });
  const [stateChartXyMetric, setStateChartXyMetric] = useState(CHART_XY_KWH);
  const [stateRefresh, setStateRefresh] = useState(0);
  var canvaChartRef = React.createRef();

  function saveLastSensorIdSelected(sensorId) {
    window.localStorage.setItem("EnergyLogDashboard.LastSensorIdSelected", sensorId);
  }

  function getLastSensorSelected(sensors) {
    if (sensors.lengths === 0) return null;
    var lastSensorId = window.localStorage.getItem("EnergyLogDashboard.LastSensorIdSelected");
    if (!lastSensorId) return null;
    for (const index in sensors) {
      if (sensors[index].id === lastSensorId) return sensors[index];
    }
    return null;
  }

  useEffect(() => {
    async function populateSensorsList() {
      const response = await fetch('api/sensor', {
        headers: !props.token ? {} : { 'Authorization': `Bearer ${props.token}` }
      });
      const sensors = await response.json();
      const selectedSensor = getLastSensorSelected(sensors);
      setStateSensors({ sensors: sensors, selectedSensor: selectedSensor });
      setStateLoading(false);
    }
    populateSensorsList();
  }, [props.token]);

  useEffect(() => {
    async function populateDashboardData(selectedSensor) {
      if (!selectedSensor) return false;
      const sensorId = selectedSensor.id;
      const { year, month, day } = stateSelectedXyDay;
      const response = await fetch(`api/sensor/${sensorId}/dashboard/${year}/${month}/${day}`, {
        headers: !props.token ? {} : { 'Authorization': `Bearer ${props.token}` }
      });
      const data = await response.json();
      var totalKwh = 0.00, totalDuration = 0.00;
      data.xyTotalKwh.map(xy => (totalKwh += xy.y))
      data.xyTotalDuration.map(xy => (totalDuration += xy.y))
      setStateXy({ ...data, totalKwh, totalDuration });
      setStateLoading(false);
    }
    populateDashboardData(stateSensors.selectedSensor);
  }, [props.token, stateSensors, stateSelectedXyDay, stateRefresh]);

  useEffect(() => {
    if (chartXy) return;
    chartXy = new Chart(canvaChartRef.current.getContext('2d'), {
      type: 'line',
      data: {
        labels: [],
        datasets: [{
          data: [],
          lineTension: 0,
          backgroundColor: 'transparent',
          borderColor: '#007bff',
          borderWidth: 4,
          pointBackgroundColor: '#007bff'
        }]
      },
      options: {
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: false
            }
          }]
        },
        legend: {
          display: false
        }
      }
    })
  }, [canvaChartRef]);

  useEffect(() => {
    chartXy.clear();
    var [x, y] = stateChartXyMetric === CHART_XY_KWH ?
      [stateXy.xyTotalKwh.map(xy => xy.x), stateXy.xyTotalKwh.map(xy => xy.y)]
      :
      [stateXy.xyTotalDuration.map(xy => xy.x), stateXy.xyTotalDuration.map(xy => xy.y)]
    chartXy.data.labels = x;
    chartXy.data.datasets[0].data = y;
    chartXy.update();
  }, [stateXy, stateChartXyMetric, CHART_XY_KWH, stateLoading]);

  const onSelectedSensor = (sensor) => {
    setStateSensors({ ...stateSensors, selectedSensor: sensor });
    saveLastSensorIdSelected(sensor.id);
  }

  const onRefresh = (e) => {
    if (stateLoading) return;
    setStateRefresh(stateRefresh + 1);
    setStateLoading(true);
  }

  const onSelectedChartXyMetric = (metric) => {
    if (stateLoading) return;
    setStateChartXyMetric(metric);
  }

  const onSelectedXyDay = (selectedDay) => {
    if (stateLoading) return;
    const [year, month, day] = selectedDay.split("/");
    setStateSelectedXyDay({ year, month, day });
    setStateLoading(true);
  }

  return (
    <main role="main" className="col-md-16 ml-sm-auto col-lg-12 px-md-4">
      <DashboardToolbar
        loading={stateLoading}
        selectedSensor={stateSensors.selectedSensor}
        sensors={stateSensors.sensors}
        onSelectedSensor={onSelectedSensor}
        selectedDay={stateSelectedXyDay}
        days={stateXy.xyDays}
        onSelectedDay={onSelectedXyDay}
        chartMetrics={[
          { key: CHART_XY_KWH, title: "Kwh", selected: stateChartXyMetric === CHART_XY_KWH, value: stateXy.totalKwh.toFixed(2) },
          { key: CHART_XY_DURATION, title: "Duration", selected: stateChartXyMetric === CHART_XY_DURATION, value: stateXy.totalDuration.toFixed(2) }
        ]
        }
        onSelectedChartMetric={onSelectedChartXyMetric}
        onRefresh={onRefresh}
      />
      <canvas ref={canvaChartRef} className="my-4 w-100" id="myChart" width="1200" height="380"></canvas>
      <h2>Logs</h2>
      <div className="table-responsive">
        <table className="table">
          <thead>
            <tr>
              <th>When</th>
              <th className="text-center">Duration</th>
              <th className="text-right d-none d-lg-table-cell">Phase 01</th>
              <th className="text-right d-none d-lg-table-cell">Phase 02</th>
              <th className="text-right d-none d-lg-table-cell">Phase 03</th>
              <th className="text-right">Total Watts</th>
            </tr>
          </thead>
          <tbody>
            {stateXy.logsRecent && stateXy.logsRecent.map((log) => (
              <tr key={log.id}>
                <td>{(new Date(log.dateTime)).toLocaleString()}</td>
                <td className="text-center">{log.duration}</td>
                <td className="text-right d-none d-lg-table-cell">{log.watts1.toFixed(2)}</td>
                <td className="text-right d-none d-lg-table-cell">{log.watts2.toFixed(2)}</td>
                <td className="text-right d-none d-lg-table-cell">{log.watts3.toFixed(2)}</td>
                <td className="text-right">{log.wattsTotal.toFixed(2)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </main >
  )
}