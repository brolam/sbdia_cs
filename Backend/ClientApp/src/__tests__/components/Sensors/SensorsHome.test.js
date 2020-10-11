import React from "react";
import { render, unmountComponentAtNode } from "react-dom";
import { act } from "react-dom/test-utils";
import SensorsHome from "../../../components/Sensors/SensorsHome";
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

test("render empty sensors cards", async () => {
  const fakeSensorsEmptyList = [];
  jest.spyOn(global, "fetch").mockImplementation(() =>
    Promise.resolve({
      json: () => Promise.resolve(fakeSensorsEmptyList),
    })
  );

  await act(async () => {
    render(<SensorsHome token={""} />, container);
  });
  expect(pretty(container.innerHTML)).toMatchInlineSnapshot(`
    "<div>
      <h1 id=\\"tabelLabel\\">Sensors</h1>
      <div class=\\"container-fluid\\"></div>
    </div>"
  `);
  global.fetch.mockRestore();
});

test("render one sensor card", async () => {
  const fakeSensorsEmptyList = [
    {
      name: "Sensor 01",
      sensorType: 0,
      sensorTypeName: "EnergyLog",
      logDurationMode: 14,
      logLastRecorded: "2020/10/11 12:52:59",
    },
  ];
  jest.spyOn(global, "fetch").mockImplementation(() =>
    Promise.resolve({
      json: () => Promise.resolve(fakeSensorsEmptyList),
    })
  );

  await act(async () => {
    render(<SensorsHome token={""} />, container);
  });
  expect(pretty(container.innerHTML)).toMatchInlineSnapshot(`
    "<div>
      <h1 id=\\"tabelLabel\\">Sensors</h1>
      <div class=\\"container-fluid\\">
        <div class=\\"card\\" style=\\"width: 18rem;\\">
          <div class=\\"card-body\\">
            <h5 class=\\"card-title\\">Sensor 01</h5>
            <h6 class=\\"card-subtitle mb-2 text-muted\\">EnergyLog</h6>
            <p class=\\"card-text\\">Last log recorded 2020/10/11 12:52:59 with modal duration of 14 seconds.</p><a href=\\"#\\" class=\\"card-link\\">Dashboard</a>
          </div>
        </div>
      </div>
    </div>"
  `);
  global.fetch.mockRestore();
});
