import React from "react";
import { render, unmountComponentAtNode } from "react-dom";
import { act } from "react-dom/test-utils";
import pretty from "pretty";
import { MemoryRouter } from "react-router-dom";
import App from "../App";

let container = null;
beforeEach(() => {
  container = document.createElement("div");
  document.body.appendChild(container);
});

afterEach(() => {
  unmountComponentAtNode(container);
  container.remove();
  container = null;
});

it("renders without crashing", async () => {

  await act(async () => {
    render(
      <MemoryRouter>
        <App />
      </MemoryRouter>,
      container
    );
  });
  await new Promise((resolve) => setTimeout(resolve, 1000));

  expect(pretty(container.innerHTML)).toMatchInlineSnapshot(`
    "<div>
      <header>
        <nav class=\\"navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3 navbar navbar-light\\">
          <div class=\\"container\\"><a class=\\"navbar-brand\\" href=\\"/\\">SBDIA</a><button type=\\"button\\" class=\\"mr-2 navbar-toggler\\"><span class=\\"navbar-toggler-icon\\"></span></button>
            <div class=\\"d-sm-inline-flex flex-sm-row-reverse collapse navbar-collapse\\">
              <ul class=\\"navbar-nav flex-grow\\">
                <li class=\\"nav-item\\"><a class=\\"text-dark nav-link\\" href=\\"/\\">Home</a></li>
                <li class=\\"nav-item\\"><a class=\\"text-dark nav-link\\" href=\\"/counter\\">Counter</a></li>
                <li class=\\"nav-item\\"><a class=\\"text-dark nav-link\\" href=\\"/fetch-data\\">Fetch data</a></li>
                <li class=\\"nav-item\\"><a class=\\"text-dark nav-link\\" href=\\"/sensors\\">Sensors</a></li>
                <li class=\\"nav-item\\"><a class=\\"text-dark nav-link\\" href=\\"/authentication/register\\">Register</a></li>
                <li class=\\"nav-item\\"><a class=\\"text-dark nav-link\\" href=\\"/authentication/login\\">Login</a></li>
              </ul>
            </div>
          </div>
        </nav>
      </header>
      <div class=\\"container\\">
        <div>
          <h1>Hello, world!</h1>
          <p>Welcome to your new single-page application, built with:</p>
          <ul>
            <li><a href=\\"https://get.asp.net/\\">ASP.NET Core</a> and <a href=\\"https://msdn.microsoft.com/en-us/library/67ef8sbd.aspx\\">C#</a> for cross-platform server-side code</li>
            <li><a href=\\"https://facebook.github.io/react/\\">React</a> for client-side code</li>
            <li><a href=\\"http://getbootstrap.com/\\">Bootstrap</a> for layout and styling</li>
          </ul>
          <p>To help you get started, we have also set up:</p>
          <ul>
            <li><strong>Client-side navigation</strong>. For example, click <em>Counter</em> then <em>Back</em> to return here.</li>
            <li><strong>Development server integration</strong>. In development mode, the development server from <code>create-react-app</code> runs in the background automatically, so your client-side resources are dynamically built on demand and the page refreshes when you modify any file.</li>
            <li><strong>Efficient production builds</strong>. In production mode, development-time features are disabled, and your <code>dotnet publish</code> configuration produces minified, efficiently bundled JavaScript files.</li>
          </ul>
          <p>The <code>ClientApp</code> subdirectory is a standard React application based on the <code>create-react-app</code> template. If you open a command prompt in that directory, you can run <code>npm</code> commands such as <code>npm test</code> or <code>npm install</code>.</p>
        </div>
      </div>
    </div>"
  `);
});
