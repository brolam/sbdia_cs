import React, { useState, useEffect } from "react";
import SensorCard from './SensorCard'

export default function SensorsHome(props) {
  const [state, setState] = useState({ sensors: [], loading: true, token: props.token });

  useEffect(() => {
    async function populateSensors() {
      const token = state.token;
      const response = await fetch('api/sensor', {
        headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
      });
      const data = await response.json();
      setState({ ...state, sensors: data, loading: false });
    }
    populateSensors();
  }, [state, props.token]);

  let renderSensorsTable = (sensors) => {
    return (
      sensors.map(sensor => <SensorCard key={sensor.id} sensor={sensor} />)
    );
  }

  let contents = state.loading
    ? <p><em>Loading...</em></p>
    : renderSensorsTable(state.sensors);

  return (
    <div>
      <h1 id="tabelLabel" >Sensors</h1>
      <div className="container-fluid">{contents}</div>
    </div>
  );

}
