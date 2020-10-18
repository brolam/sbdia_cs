import React from 'react'
import { Component } from 'react'
import { Route, Redirect } from 'react-router-dom'
import { ApplicationPaths, QueryParameterNames } from './ApiAuthorizationConstants'
import authService from './AuthorizeService'

export default class AuthorizeRoute extends Component {
    constructor(props) {
        super(props);

        this.state = {
            ready: false,
            authenticated: false,
            token: ""
        };
    }

    componentDidMount() {
        this._subscription = authService.subscribe(() => this.authenticationChanged());
        this.populateAuthenticationState();
    }

    componentWillUnmount() {
        authService.unsubscribe(this._subscription);
    }

    render() {
        const { ready, authenticated, token } = this.state;
        const redirectUrl = `${ApplicationPaths.Login}?${QueryParameterNames.ReturnUrl}=${encodeURI(window.location.href)}`
        if (!ready) {
            return <div></div>;
        } else {
            const { component: ComponentAuthenticated, ...rest } = this.props;
            return <Route {...rest}
                render={(props) => {
                    props.token = token;
                    if (authenticated) {
                        return <ComponentAuthenticated {...props} />
                    } else {
                        return <Redirect to={redirectUrl} />
                    }
                }} />
        }
    }

    async populateAuthenticationState() {
        const authenticated = await authService.isAuthenticated();
        const token = authenticated ? await authService.getAccessToken() : "";
        this.setState({ ready: true, authenticated, token: token });
    }

    async authenticationChanged() {
        this.setState({ ready: false, authenticated: false, token: "" });
        await this.populateAuthenticationState();
    }
}
