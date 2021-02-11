import React from 'react';

export default function DropdownSensors(props) {
  const { loading, selectedSensor, sensors, onSelectedSensor } = props;
  return (
    <li className="nav-item dropdown">
      <a className="nav-link">
        <h4 className="dropdown-toggle" to="#" id="dropdownSensors" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
          {loading ?
            "Dashboard loading" :
            selectedSensor ?
              selectedSensor.name :
              "Select a Sensor"}
        </h4>
        <ul className="dropdown-menu" aria-labelledby="dropdownSensors">
          {!loading && sensors.map(sensor => <li key={sensor.id} className="dropdown-item" onClick={(e) => { onSelectedSensor(sensor); }}>{sensor.name}</li>
          )}
        </ul>
      </a>
    </li>
  )
}
