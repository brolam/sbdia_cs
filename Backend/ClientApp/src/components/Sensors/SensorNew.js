import React from 'react'

export default function SensorNew(props) {

  const onSubmit = (e) => {
    e.preventDefault();
    var sensorName = e.target.elements.name.value
    fetch('api/sensor', {
      method: 'POST',
      headers: {
        'Accept': 'application/json, text/plain, */*',
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${props.token}`
      },
      body: JSON.stringify({ "Name": sensorName, "SensorType": 0 }),
      redirect: 'follow'
    }).then(response => {
      if (response.ok) props.history.push("/sensors")
    });
    return false;
  }

  return (
    <form onSubmit={onSubmit}  >
      <div className="form-group">
        <label for="sensorNameInput">Name</label>
        <input name="name" type="text" className="form-control" id="sensorNameInput" aria-describedby="sensorNameHelp" placeholder="Enter sensor name" />
        <small id="sensorNameHelp" className="form-text text-muted">Sensor name is required.</small>
      </div>
      <button className="btn btn-primary" >Submit</button>
    </form>
  )
} 