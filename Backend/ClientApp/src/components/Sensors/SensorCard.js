import React from "react";

export default function SensorCard(props) {
    const sensor = props.sensor
    const idSecretTokenModal = `id_secretToken_Model${sensor.id}`;
    return (
        <div className="card" style={{ width: "18rem" }}>
            <div className="card-body">
                <h5 className="card-title">{sensor.name}</h5>
                <h6 className="card-subtitle mb-2 text-muted">{sensor.sensorTypeName}</h6>
                <p className="card-text">Last log recorded {sensor.logLastRecorded} with modal duration of {sensor.logDurationMode} seconds.</p>
                <a href="window.location" className="card-link">Dashboard</a>
                <a href="window.location" className="card-link" data-toggle="modal" data-target={"#" + idSecretTokenModal} >Secret Token</a>
                <div className="modal fade" id={idSecretTokenModal} tabIndex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
                    <div className="modal-dialog modal-dialog-centered">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title" id="exampleModalLabel">{sensor.name}</h5>
                                <button type="button" className="close" data-dismiss="modal" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div className="modal-body">
                                <h6>Sensor ID: {sensor.id}</h6>
                                <h6>Secret Token: {sensor.secretApiToken}</h6>
                            </div>
                            <div className="modal-footer">
                                <button type="button" className="btn btn-secondary" data-dismiss="modal">Close</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}

