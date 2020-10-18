import React from "react"
import { Link } from 'react-router-dom';

export default function SensorMenu(props) {
  let { isAuthenticated } = props;

  let authenticatedView = () => {
    return (
      <li className="nav-item dropdown">
        <Link className="nav-link dropdown-toggle" to="#" id="navbarDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
          Sensores
        </Link>
        <div className="dropdown-menu" aria-labelledby="navbarDropdown">
          <Link className="dropdown-item" to="/sensors">List</Link>
          <Link className="dropdown-item" to="/sensors">New</Link>
        </div>
      </li>
    );
  }

  let anonymousView = () => {
    return (
      <li className="nav-item" >
        <Link className="nav-link" to="/sensors">Sensors</Link>
      </li>
    );
  }

  return (
    isAuthenticated ? authenticatedView() : anonymousView()
  )
}