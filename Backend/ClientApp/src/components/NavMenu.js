import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import SensorMenu from './Sensors/SensorMenu'
import { LoginMenu } from './api-authorization/LoginMenu';
import authService from './api-authorization/AuthorizeService';
import './NavMenu.css';


export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor(props) {
    super(props);
    this.state = {
      isAuthenticated: false
    };
  }

  componentDidMount() {
    this.setAuthenticated()
  }

  async setAuthenticated() {
    let isAuthenticated = await authService.isAuthenticated();
    this.setState({ ...this.state, isAuthenticated })
  }

  render() {
    return (
      <header>
        <nav className="navbar navbar-expand-lg navbar-dark bg-primary">
          <Link className="navbar-brand" to="/">SBDIA</Link>
          <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="true" aria-label="Toggle navigation">
            <span className="navbar-toggler-icon"></span>
          </button>
          <div className="collapse navbar-collapse" id="navbarSupportedContent">
            <ul className="navbar-nav mr-auto">
              <li className="nav-item active">
                <Link className="nav-link" to="/dashboards">Dashboard<span className="sr-only">(current)</span></Link>
              </li>
              <SensorMenu isAuthenticated={this.state.isAuthenticated} />
              <LoginMenu />
            </ul>
            <form className="form-inline my-2 my-lg-0">
              <input className="form-control mr-sm-2" type="search" placeholder="Search" aria-label="Search" />
              <button className="btn btn-outline-light my-2 my-sm-0" type="submit">Search</button>
            </form>
          </div>
        </nav>
      </header>
    );
  }
}