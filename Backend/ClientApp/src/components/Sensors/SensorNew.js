import React, { useState, useEffect } from 'react'

export default function SensorNew(props) {
  const [state, setState] = useState({
    timeZones: [],
    loading: true
  });

  const onSubmit = (e) => {
    e.preventDefault();
    var sensorName = e.target.elements.name.value;
    var sensorTimeZone = e.target.elements.timeZone.value;
    fetch('api/sensor', {
      method: 'POST',
      headers: {
        'Accept': 'application/json, text/plain, */*',
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${props.token}`
      },
      body: JSON.stringify({ "Name": sensorName, "SensorType": 0, "TimeZone": sensorTimeZone }),
      redirect: 'follow'
    }).then(response => {
      if (response.ok) props.history.push("/sensors")
    });
    return false;
  }

  useEffect(() => {
    async function populateTimeZones() {
      const response = await fetch('api/sensor/timeZones', {
        headers: !props.token ? {} : { 'Authorization': `Bearer ${props.token}` }
      });
      const timeZones = await response.json();
      setState({ ...state, timeZones: timeZones, loading: false });
    }
    populateTimeZones();
  }, [props.token, state]);

  return (
    <form onSubmit={onSubmit}  >
      <div className="form-group">
        <label for="sensorNameInput">Name</label>
        <input name="name" type="text" className="form-control" id="sensorNameInput" aria-describedby="sensorNameHelp" placeholder="Enter sensor name" />
        <small id="sensorNameHelp" className="form-text text-muted">Sensor name is required.</small>
      </div>
      <div className="form-group">
        <label for="sensorTimeZone">Select a Time Zone</label>
        <select name="timeZone" className="form-control" id="sensorTimeZone">
          {
            state.timeZones.map(timeZone =>
              <option key={timeZone} value={timeZone}>{timeZone}</option>
            )
          }
        </select>
      </div>
      {!state.loading && <button className="btn btn-primary" >Submit</button>}
    </form>
  )
} 