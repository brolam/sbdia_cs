import React, { Component, Fragment } from 'react';
import { Link } from 'react-router-dom';
import authService from './AuthorizeService';
import { ApplicationPaths } from './ApiAuthorizationConstants';

export class LoginMenu extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isAuthenticated: false,
            userName: null
        };
    }

    componentDidMount() {
        this._subscription = authService.subscribe(() => this.populateState());
        this.populateState();
    }

    componentWillUnmount() {
        authService.unsubscribe(this._subscription);
    }

    async populateState() {
        const [isAuthenticated, user] = await Promise.all([authService.isAuthenticated(), authService.getUser()])
        this.setState({
            isAuthenticated,
            userName: user && user.name
        });
    }

    render() {
        const { isAuthenticated, userName } = this.state;
        if (!isAuthenticated) {
            const registerPath = `${ApplicationPaths.Register}`;
            const loginPath = `${ApplicationPaths.Login}`;
            return this.anonymousView(registerPath, loginPath);
        } else {
            const profilePath = `${ApplicationPaths.Profile}`;
            const logoutPath = { pathname: `${ApplicationPaths.LogOut}`, state: { local: true } };
            return this.authenticatedView(userName, profilePath, logoutPath);
        }
    }

    authenticatedView(userName, profilePath, logoutPath) {
        return (
            <Fragment>
                <li className="nav-item active">
                    <Link className="nav-link" to={profilePath} >Hello {userName}</Link>
                </li>
                <li className="nav-item active">
                    <Link className="nav-link" to={logoutPath}>Logout</Link>
                </li>
            </Fragment>);

    }

    anonymousView(registerPath, loginPath) {
        return (
            <Fragment>
                <li className="nav-item active">
                    <Link className="nav-link" to={registerPath}>Register</Link>
                </li>
                <li className="nav-item active">
                    <Link className="nav-link" to={loginPath}>Login</Link>
                </li>
            </Fragment>);
    }
}
