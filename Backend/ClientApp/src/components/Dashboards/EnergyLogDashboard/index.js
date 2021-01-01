import React, { useState, useEffect } from 'react'
import Chart from "chart.js";
var chartXy = null;
export default function EnergyLogDashboard(props) {

  const [CHART_XY_KWH, CHART_XY_DURATION] = ["CHART_XY_KWH", "CHART_XY_DURATION"];
  const toDay = new Date();
  const [dafaultYear, dafaultMonth, defautlDay] = [toDay.getFullYear(), toDay.getMonth() + 1, toDay.getDate()]
  const [state, setState] = useState({
    sensors: [],
    loading: true,
    selectedSensor: null,
    selectedXyDay: { year: dafaultYear, month: dafaultMonth, day: defautlDay },
    data: {
      xyTotalKwh: [],
      xyTotalDuration: [],
      xyDays: [],
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

  async function populateDashboardData(selectedSensor) {
    if (!selectedSensor) return false;
    const sensorId = selectedSensor.id;
    const { year, month, day } = state.selectedXyDay;
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
  useEffect(() => { populateDashboardData(state.selectedSensor); }, [state.dataRefresh]);
  useEffect(() => {
    chartXy = new Chart(canvaChartRef.current, {
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
  }, []);
  useEffect(() => {
    var [x, y] = state.chartXyMetric == CHART_XY_KWH ?
      [state.data.xyTotalKwh.map(xy => xy.x), state.data.xyTotalKwh.map(xy => xy.y)]
      :
      [state.data.xyTotalDuration.map(xy => xy.x), state.data.xyTotalDuration.map(xy => xy.y)]
    chartXy.data.labels = x;
    chartXy.data.datasets[0].data = y;
    chartXy.update();
  }, [state.data, state.chartXyMetric]);

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

  const onSelectedXyDay = (selectedDay) => {
    if (state.loading) return;
    const [year, month, day] = selectedDay.split("/");
    setState({ ...state, loading: true, selectedXyDay: { year, month, day }, dataRefresh: state.dataRefresh + 1 });
  }

  const IconeReflesh = () => {
    return (
      <svg width="16" height="16" fill="currentColor" className="bi bi-arrow-clockwise" viewBox="0 0 16 16">
        <path fillRule="evenodd" d="M8 3a5 5 0 1 0 4.546 2.914.5.5 0 0 1 .908-.417A6 6 0 1 1 8 2v1z" />
        <path d="M8 4.466V.534a.25.25 0 0 1 .41-.192l2.36 1.966c.12.1.12.284 0 .384L8.41 4.658A.25.25 0 0 1 8 4.466z" />
      </svg>
    )
  }

  return (
    <main role="main" className="col-md-16 ml-sm-auto col-lg-12 px-md-4">
      <div className="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
        <a className="dropdown">
          <h2 className="dropdown-toggle" to="#" id="dropdownSensors" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
            {state.loading ?
              "Dashboard loading" :
              state.selectedSensor ?
                state.selectedSensor.name :
                "Select a Sensor"
            }
          </h2>
          <ul className="dropdown-menu" aria-labelledby="dropdownSensors">
            {
              !state.loading && state.sensors.map(sensor =>
                <li key={sensor.id} className="dropdown-item" onClick={(e) => { onSelectedSensor(sensor) }}>{sensor.name}</li>
              )
            }
          </ul>
        </a>
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
                <div className="dropdown">
                  <button type="button" id="dropdownDays" className="btn btn-sm btn-outline-secondary dropdown-toggle" data-toggle="dropdown" >
                    <span data-feather="calendar">
                      {state.selectedXyDay.year}/{state.selectedXyDay.month}/{state.selectedXyDay.day}
                    </span>
                  </button>
                  <ul className="dropdown-menu" aria-labelledby="dropdownDays">
                    {
                      !state.loading && state.data.xyDays.map(day =>
                        <li key={day} className="dropdown-item" onClick={e => onSelectedXyDay(day)} >{day}</li>
                      )
                    }
                  </ul>
                </div>
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
            {state.data.logsRecent && state.data.logsRecent.map((log) => (
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