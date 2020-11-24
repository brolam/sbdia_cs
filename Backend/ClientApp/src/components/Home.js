import React, { Component } from 'react';
import Dashboard from './Dashboards'

export class Home extends Component {
  static displayName = Home.name;

  render() {
    return (
      <Dashboard />
    );
  }
}
