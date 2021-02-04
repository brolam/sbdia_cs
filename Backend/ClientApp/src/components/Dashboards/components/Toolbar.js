import React from 'react'
import ButtonChartXyMetric from './ButtonChartXyMetric'

export default function Toolbar(props) {
  const { loading, selectedSensor, sensors, onSelectedSensor } = props;
  return (
    <div className="pt-3 pb-2 mb-3 border-bottom">
      <ul className="nav nav-pills">
        <li className="nav-item dropdown">
          <a className="nav-link">
            <h4 className="dropdown-toggle" to="#" id="dropdownSensors" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
              {loading ?
                "Dashboard loading" :
                selectedSensor ?
                  selectedSensor.name :
                  "Select a Sensor"
              }
            </h4>
            <ul className="dropdown-menu" aria-labelledby="dropdownSensors">
              {
                !loading && sensors.map(sensor =>
                  <li key={sensor.id} className="dropdown-item" onClick={(e) => { onSelectedSensor(sensor) }}>{sensor.name}</li>
                )
              }
            </ul>
          </a>
        </li>
        <ButtonChartXyMetric title="Kwh" selected={true} value="0.00" onClick={() => { }} />
        <ButtonChartXyMetric title="Duration" selected={false} value="0.00" onClick={() => { }} />
      </ul>
    </div>
  )
}   