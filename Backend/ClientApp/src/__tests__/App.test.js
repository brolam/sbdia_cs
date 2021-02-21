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
              <li class=\\"nav-item active\\"><a class=\\"nav-link\\" href=\\"/dashboards\\">Dashboard<span class=\\"sr-only\\">(current)</span></a></li>
              <li class=\\"nav-item\\"><a class=\\"nav-link\\" href=\\"/sensors\\">Sensors</a></li>
              <li class=\\"nav-item active\\"><a class=\\"nav-link\\" href=\\"/authentication/register\\">Register</a></li>
              <li class=\\"nav-item active\\"><a class=\\"nav-link\\" href=\\"/authentication/login\\">Login</a></li>
            </ul>
            <form class=\\"form-inline my-2 my-lg-0\\"><input class=\\"form-control mr-sm-2\\" type=\\"search\\" placeholder=\\"Search\\" aria-label=\\"Search\\"><button class=\\"btn btn-outline-light my-2 my-sm-0\\" type=\\"submit\\">Search</button></form>
          </div>
        </nav>
      </header>
      <div class=\\"container\\">
        <div></div>
      </div>
    </div>"
  `);
});
