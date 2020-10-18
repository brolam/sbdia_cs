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
        <nav class=\\"navbar navbar-expand-lg navbar-dark bg-primary\\"><a class=\\"navbar-brand\\" href=\\"/\\">SBDIA</a><button class=\\"navbar-toggler\\" type=\\"button\\" data-toggle=\\"collapse\\" data-target=\\"#navbarSupportedContent\\" aria-controls=\\"navbarSupportedContent\\" aria-expanded=\\"true\\" aria-label=\\"Toggle navigation\\"><span class=\\"navbar-toggler-icon\\"></span></button>
          <div class=\\"collapse navbar-collapse\\" id=\\"navbarSupportedContent\\">
            <ul class=\\"navbar-nav mr-auto\\">
              <li class=\\"nav-item active\\"><a class=\\"nav-link\\" href=\\"/\\">Home <span class=\\"sr-only\\">(current)</span></a></li>
              <li class=\\"nav-item\\"><a class=\\"nav-link\\" href=\\"/counter\\">Counter</a></li>
              <li class=\\"nav-item\\"><a class=\\"nav-link\\" href=\\"/fetch-data\\">Fetch data</a></li>
              <li class=\\"nav-item\\"><a class=\\"nav-link\\" href=\\"/sensors\\">Sensors</a></li>
              <li class=\\"nav-item active\\"><a class=\\"nav-link\\" href=\\"/authentication/register\\">Register</a></li>
              <li class=\\"nav-item active\\"><a class=\\"nav-link\\" href=\\"/authentication/login\\">Login</a></li>
            </ul>
            <form class=\\"form-inline my-2 my-lg-0\\"><input class=\\"form-control mr-sm-2\\" type=\\"search\\" placeholder=\\"Search\\" aria-label=\\"Search\\"><button class=\\"btn btn-outline-light my-2 my-sm-0\\" type=\\"submit\\">Search</button></form>
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
