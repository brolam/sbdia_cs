import React from "react";
import { render, unmountComponentAtNode } from "react-dom";
import { act } from "react-dom/test-utils";
import { MemoryRouter } from "react-router";
import SensorMenu from "../../../components/Sensors/SensorMenu";
import pretty from "pretty";

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

test("render sensor menu user not authenticated", async () => {
  await act(async () => {
    render(
      <MemoryRouter>
        <SensorMenu isAuthenticated={false} />
      </MemoryRouter>,
      container
    );
  });

  expect(pretty(container.innerHTML)).toMatchInlineSnapshot(
    `"<li class=\\"nav-item\\"><a class=\\"nav-link\\" href=\\"/sensors\\">Sensors</a></li>"`
  );
});

test("render sensor menu user authenticated", async () => {
  await act(async () => {
    render(
      <MemoryRouter>
        <SensorMenu isAuthenticated={true} />
      </MemoryRouter>,
      container
    );
  });

  expect(pretty(container.innerHTML)).toMatchInlineSnapshot(`
    "<li class=\\"nav-item dropdown\\"><a class=\\"nav-link dropdown-toggle\\" id=\\"navbarDropdown\\" role=\\"button\\" data-toggle=\\"dropdown\\" aria-haspopup=\\"true\\" aria-expanded=\\"false\\" href=\\"/\\">Sensores</a>
      <div class=\\"dropdown-menu\\" aria-labelledby=\\"navbarDropdown\\"><a class=\\"dropdown-item\\" href=\\"/sensors\\">List</a><a class=\\"dropdown-item\\" href=\\"/sensors\\">New</a></div>
    </li>"
  `);
});
