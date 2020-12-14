import React, { useState, useEffect } from 'react'
import Chart from "chart.js";

export default function EnergyLogDashboard(props) {
  const [state, setState] = useState({ sensors: [], loading: true, selectedSensor: null });
  var canvaChartRef = React.createRef();

  async function populateSensorsList() {
    const response = await fetch('api/sensor', {
      headers: !props.token ? {} : { 'Authorization': `Bearer ${props.token}` }
    });
    const data = await response.json();
    setState({ ...state, sensors: data, loading: false });
  }

  useEffect(() => { populateSensorsList(); }, [props.token]);

  useEffect(() => {
    console.log('ctx', canvaChartRef.current)
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
  })

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
      <h2>Section title</h2>
      <div className="table-responsive">
        <table className="table table-striped table-sm">
          <thead>
            <tr>
              <th>#</th>
              <th>Header</th>
              <th>Header</th>
              <th>Header</th>
              <th>Header</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>1,001</td>
              <td>Lorem</td>
              <td>ipsum</td>
              <td>dolor</td>
              <td>sit</td>
            </tr>
            <tr>
              <td>1,002</td>
              <td>amet</td>
              <td>consectetur</td>
              <td>adipiscing</td>
              <td>elit</td>
            </tr>
            <tr>
              <td>1,003</td>
              <td>Integer</td>
              <td>nec</td>
              <td>odio</td>
              <td>Praesent</td>
            </tr>
            <tr>
              <td>1,003</td>
              <td>libero</td>
              <td>Sed</td>
              <td>cursus</td>
              <td>ante</td>
            </tr>
            <tr>
              <td>1,004</td>
              <td>dapibus</td>
              <td>diam</td>
              <td>Sed</td>
              <td>nisi</td>
            </tr>
            <tr>
              <td>1,005</td>
              <td>Nulla</td>
              <td>quis</td>
              <td>sem</td>
              <td>at</td>
            </tr>
            <tr>
              <td>1,006</td>
              <td>nibh</td>
              <td>elementum</td>
              <td>imperdiet</td>
              <td>Duis</td>
            </tr>
            <tr>
              <td>1,007</td>
              <td>sagittis</td>
              <td>ipsum</td>
              <td>Praesent</td>
              <td>mauris</td>
            </tr>
            <tr>
              <td>1,008</td>
              <td>Fusce</td>
              <td>nec</td>
              <td>tellus</td>
              <td>sed</td>
            </tr>
            <tr>
              <td>1,009</td>
              <td>augue</td>
              <td>semper</td>
              <td>porta</td>
              <td>Mauris</td>
            </tr>
            <tr>
              <td>1,010</td>
              <td>massa</td>
              <td>Vestibulum</td>
              <td>lacinia</td>
              <td>arcu</td>
            </tr>
            <tr>
              <td>1,011</td>
              <td>eget</td>
              <td>nulla</td>
              <td>Class</td>
              <td>aptent</td>
            </tr>
            <tr>
              <td>1,012</td>
              <td>taciti</td>
              <td>sociosqu</td>
              <td>ad</td>
              <td>litora</td>
            </tr>
            <tr>
              <td>1,013</td>
              <td>torquent</td>
              <td>per</td>
              <td>conubia</td>
              <td>nostra</td>
            </tr>
            <tr>
              <td>1,014</td>
              <td>per</td>
              <td>inceptos</td>
              <td>himenaeos</td>
              <td>Curabitur</td>
            </tr>
            <tr>
              <td>1,015</td>
              <td>sodales</td>
              <td>ligula</td>
              <td>in</td>
              <td>libero</td>
            </tr>
          </tbody>
        </table>
      </div>
    </main>
  )
}