import React, { useState, useEffect } from 'react'
import Chart from "chart.js";

export default function EnergyLogDashboard(props) {
  const date = new Date();
  const [year, month, day] = [date.getFullYear(), date.getMonth() + 1, date.getDate()]
  const [state, setState] = useState({
    sensors: [],
    loading: true,
    selectedSensor: null,
    data: { xy: [], logsRecent: [] },
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
    setState({ ...state, data: data, loading: false });
  }

  useEffect(() => { populateSensorsList(); }, [props.token]);
  useEffect(() => { populateDashboardData(); }, [state.dataRefresh]);
  useEffect(() => {
    var [x, y] = [state.data.xy.map(xy => xy.x), state.data.xy.map(xy => xy.y)]
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
  });

  const onSelectedSensor = (sensor) => {
    setState({ ...state, selectedSensor: sensor, loading: true, dataRefresh: state.dataRefresh + 1 });
  }

  const onRefresh = () => {
    if (state.loading) return;
    setState({ ...state, loading: true, dataRefresh: state.dataRefresh + 1 });
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
        <div className="btn-toolbar mb-2 mb-md-0">
          <div className="btn-group mr-2">
            <button type="button" className="btn btn-sm btn-outline-secondary" onClick={(e) => onRefresh()} >Refresh</button>
            <button type="button" className="btn btn-sm btn-outline-secondary">Share</button>
            <button type="button" className="btn btn-sm btn-outline-secondary">Export</button>
          </div>
          <button type="button" className="btn btn-sm btn-outline-secondary dropdown-toggle">
            <span data-feather="calendar"></span>
            This week
          </button>
        </div>
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
    </main>
  )
}