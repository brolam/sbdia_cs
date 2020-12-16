import React, { useState, useEffect } from 'react'
import Chart from "chart.js";

export default function EnergyLogDashboard(props) {
  const date = new Date();
  const [year, month, day] = [date.getFullYear(), date.getMonth() + 1, date.getDate()]
  const [state, setState] = useState({
    sensors: [],
    loading: true,
    selectedSensor: null,
    data: [],
    seconds: 0
  });
  var canvaChartRef = React.createRef();

  async function populateSensorsList() {
    const response = await fetch('api/sensor', {
      headers: !props.token ? {} : { 'Authorization': `Bearer ${props.token}` }
    });
    const sensors = await response.json();
    setState({ ...state, sensors: sensors, loading: false });
    console.log(state.sensors, props.token);
  }

  async function populateDashboardData() {
    if (!state.selectedSensor) return;
    const sensorId = state.selectedSensor.id;
    const response = await fetch(`api/sensor/${sensorId}/dashboard/${year}/${month}/${day}`, {
      headers: !props.token ? {} : { 'Authorization': `Bearer ${props.token}` }
    });
    const data = await response.json();
    setState({ ...state, data: data });
    console.log(state.data, props.token);
  }

  //setTimeout(() => setState({ ...state, seconds: state.seconds + 1 }), 15000);

  useEffect(() => { populateSensorsList(); }, [props.token]);

  useEffect(() => { populateDashboardData(); }, [props.token, state.selectedSensor, state.seconds]);

  useEffect(() => {
    new Chart(canvaChartRef.current, {
      type: 'line',
      data: {
        labels: [
          'Sunday',
          'Monday',
          'Tuesday',
          'Wednesday',
          'Thursday',
          'Friday',
          'Saturday'
        ],
        datasets: [{
          data: [
            15339,
            21345,
            18483,
            24003,
            23489,
            24092,
            12034
          ],
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
  }, [state.data])

  const onSelectedSensor = (sensor) => {
    setState({ ...state, selectedSensor: sensor });
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
            <div className="dropdown-menu" aria-labelledby="navbarDropdown">
              {
                state.sensors.map(sensor =>
                  <a key={sensor.id} className="dropdown-item" onClick={(e) => { onSelectedSensor(sensor) }}>{sensor.name}</a>
                )
              }
            </div>
          </a>
        </div>
        <div className="btn-toolbar mb-2 mb-md-0">
          <div className="btn-group mr-2">
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
              <tr>
                <td>{Date(log.unixDate).substring(0, 21)}</td>
                <td>{log.duration}</td>
                <td>{log.watts1}</td>
                <td>{log.watts2}</td>
                <td>{log.watts3}</td>
                <td>{log.wattsTotal}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </main>
  )
}