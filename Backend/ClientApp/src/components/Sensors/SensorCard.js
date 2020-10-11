import React from "react";

export default function SensorCard(props) {
    const sensor = props.sensor
    return (
        <div className="card" style={{width:"18rem"}}>
            <div className="card-body">
                <h5 className="card-title">{sensor.name}</h5>
                <h6 className="card-subtitle mb-2 text-muted">{sensor.sensorTypeName}</h6>
                <p className="card-text">Last log recorded {sensor.logLastRecorded} with modal duration of {sensor.logDurationMode} seconds.</p>
                <a href="#" className="card-link">Dashboard</a>
            </div>
        </div>
    )
}

