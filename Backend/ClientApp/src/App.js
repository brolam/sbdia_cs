import React from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import SensorsHome from './components/Sensors/SensorsHome';
import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';
import SensorNew from './components/Sensors/SensorNew'
import Dashboards from './components/Dashboards'

import './custom.css'

export default function App(props) {
  return (
    <Layout>
      <Route exact path='/' component={Home} />
      <AuthorizeRoute path='/dashboards' component={Dashboards} />
      <AuthorizeRoute exact path='/sensors' component={SensorsHome} />
      <AuthorizeRoute exact path='/sensors/new' component={SensorNew} />
      <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />
    </Layout>
  );
}
