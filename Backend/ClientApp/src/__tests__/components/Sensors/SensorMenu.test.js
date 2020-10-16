import React from "react";
import { render, unmountComponentAtNode } from "react-dom";
import { act } from "react-dom/test-utils";
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
    render(<SensorMenu isAuthenticated={false} />, container);
  });

  expect(pretty(container.innerHTML)).toMatchInlineSnapshot(
    `"<li class=\\"nav-item\\"><a to=\\"/sensors\\" class=\\"text-dark nav-link\\">Sensors</a></li>"`
  );
});

test("render sensor menu user authenticated", async () => {
  await act(async () => {
    render(<SensorMenu isAuthenticated={true} />, container);
  });

  expect(pretty(container.innerHTML)).toMatchInlineSnapshot(`
    "<li class=\\"nav-item\\">
    <li class=\\"dropdown nav-item\\"><a aria-haspopup=\\"true\\" href=\\"#\\" class=\\"dropdown-toggle nav-link\\" aria-expanded=\\"false\\">Sensors</a>
      <div tabindex=\\"-1\\" role=\\"menu\\" aria-hidden=\\"true\\" class=\\"dropdown-menu dropdown-menu-right\\"><button type=\\"button\\" to=\\"/sensors\\" tabindex=\\"0\\" class=\\"dropdown-item active\\">Consultar</button><button type=\\"button\\" tabindex=\\"0\\" class=\\"dropdown-item\\">New</button></div>
    </li>
    </li>"
  `);
});
