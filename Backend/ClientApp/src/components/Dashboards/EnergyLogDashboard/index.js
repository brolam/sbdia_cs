import React, { useState, useEffect } from 'react'
import Chart from "chart.js";

export default function EnergyLogDashboard(props) {
  const [CHART_XY_KWH, CHART_XY_DURATION] = ["CHART_XY_KWH", "CHART_XY_DURATION"];
  const date = new Date();
  const [year, month, day] = [date.getFullYear(), date.getMonth() + 1, date.getDate()]
  const [state, setState] = useState({
    sensors: [],
    loading: true,
    selectedSensor: null,
    data: {
      xyTotalKwh: [],
      xyTotalDuration: [],
      logsRecent: [],
      totalKwh: 0.00,
      totalDuration: 0.00
    },
    chartXyMetric: CHART_XY_KWH,
    dataRefresh: 0
  });
  var canvaChartRef = React.createRef();

  async function populateSensorsList() {
    const response = await fetch('api/sensor', {
      headers: !props.token ? {} : { 'Authorization': `Bearer ${props.token}` }
    });
    const sensors = await response.json();
    setState({ ...state, sensors: sensors, loading: false });
  }

  async function populateDashboardData() {
    if (!state.selectedSensor) return false;
    const sensorId = state.selectedSensor.id;
    const response = await fetch(`api/sensor/${sensorId}/dashboard/${year}/${month}/${day}`, {
      headers: !props.token ? {} : { 'Authorization': `Bearer ${props.token}` }
    });
    const data = await response.json();
    var totalKwh = 0.00, totalDuration = 0.00;
    data.xyTotalKwh.map(xy => { totalKwh += xy.y })
    data.xyTotalDuration.map(xy => { totalDuration += xy.y })
    setState({ ...state, data: { ...data, totalKwh, totalDuration }, loading: false });
  }

  useEffect(() => { populateSensorsList(); }, [props.token]);
  useEffect(() => { populateDashboardData(); }, [state.dataRefresh]);
  useEffect(() => {
    var [x, y] = state.chartXyMetric == CHART_XY_KWH ?
      [state.data.xyTotalKwh.map(xy => xy.x), state.data.xyTotalKwh.map(xy => xy.y)]
      :
      [state.data.xyTotalDuration.map(xy => xy.x), state.data.xyTotalDuration.map(xy => xy.y)]

    new Chart(canvaChartRef.current, {
      type: 'line',
      data: {
        labels: x,
        datasets: [{
          data: y,
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
  }, [state.data.xyTotalKwh, state.chartXyMetric]);

  const onSelectedSensor = (sensor) => {
    setState({ ...state, selectedSensor: sensor, loading: true, dataRefresh: state.dataRefresh + 1 });
  }

  const onRefresh = (e) => {
    if (state.loading) return;
    setState({ ...state, loading: true, dataRefresh: state.dataRefresh + 1 });
  }

  const onSelectedChartXyMetric = (metric) => {
    if (state.loading) return;
    setState({ ...state, chartXyMetric: metric });
  }

  const IconeReflesh = () => {
    return (
      <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-arrow-clockwise" viewBox="0 0 16 16">
        <path fillRule="evenodd" d="M8 3a5 5 0 1 0 4.546 2.914.5.5 0 0 1 .908-.417A6 6 0 1 1 8 2v1z" />
        <path d="M8 4.466V.534a.25.25 0 0 1 .41-.192l2.36 1.966c.12.1.12.284 0 .384L8.41 4.658A.25.25 0 0 1 8 4.466z" />
      </svg>
    )
  }

  return (
    <main role="main" className="col-md-16 ml-sm-auto col-lg-12 px-md-4">
      <div className="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
        <div className="dropdown">
          <a className="dropdown">
            <h2 className="dropdown-toggle" to="#" id="navbarDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
              {state.loading ?
                "Dashboard loading" :
                state.selectedSensor ?
                  state.selectedSensor.name :
                  "Select a Sensor"
              }
            </h2>
            <ul className="dropdown-menu" aria-labelledby="navbarDropdown">
              {
                !state.loading && state.sensors.map(sensor =>
                  <li key={sensor.id} className="dropdown-item" onClick={(e) => { onSelectedSensor(sensor) }}>{sensor.name}</li>
                )
              }
            </ul>
          </a>
        </div>
        {(!state.loading && state.selectedSensor) &&
          <div className="btn-toolbar mb-2 mb-md-0">
            <div className="btn-group mr-2">
              <button type="button" className={state.chartXyMetric == CHART_XY_KWH ? "btn btn-sm btn-outline-secondary active" : "btn btn-sm btn-outline-secondary"} onClick={(e) => onSelectedChartXyMetric(CHART_XY_KWH)}>
                Kwh <span className="badge bg-secondary text-white">{state.data.totalKwh.toFixed(2)}</span>
              </button>
              <button type="button" className={state.chartXyMetric == CHART_XY_DURATION ? "btn btn-sm btn-outline-secondary active" : "btn btn-sm btn-outline-secondary"} onClick={(e) => onSelectedChartXyMetric(CHART_XY_DURATION)}>
                Duration <span className="badge bg-secondary text-white">{state.data.totalDuration.toFixed(2)}</span>
              </button>
            </div>
            <div className="row">
              <div className="col">
                <button type="button" className="btn btn-sm btn-outline-secondary dropdown-toggle">
                  <span data-feather="calendar"></span>Day
            </button>
              </div>
              <div className="col">
                <button type="button" className="btn btn-sm btn-outline-secondary" onClick={(e) => onRefresh()}>
                  <IconeReflesh />
                </button>
              </div>
            </div>
          </div>
        }
      </div>
      <canvas ref={canvaChartRef} className="my-4 w-100" id="myChart" width="1200" height="380"></canvas>
      <h2>Logs</h2>
      <div className="table-responsive">
        <table className="table table-striped table-sm">
          <thead>
            <tr>
              <th>When</th>
              <th>Duration</th>
              <th>Phase 01</th>
              <th>Phase 02</th>
              <th>Phase 03</th>
              <th>Total Watts</th>
            </tr>
          </thead>
          <tbody>
            {state.data.logsRecent && state.data.logsRecent.map((log) => (
              <tr key={log.id}>
                <td>{log.dateTime}</td>
                <td>{log.duration}</td>
                <td>{log.watts1.toFixed(2)}</td>
                <td>{log.watts2.toFixed(2)}</td>
                <td>{log.watts3.toFixed(2)}</td>
                <td>{log.wattsTotal.toFixed(2)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </main >
  )
}