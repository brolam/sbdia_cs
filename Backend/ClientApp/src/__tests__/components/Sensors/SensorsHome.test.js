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
      id: 1,
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
            <p class=\\"card-text\\">Last log recorded 2020/10/11 12:52:59 with modal duration of 14 seconds.</p><a href=\\"window.location\\" class=\\"card-link\\">Dashboard</a><a href=\\"window.location\\" class=\\"card-link\\" data-toggle=\\"modal\\" data-target=\\"#id_secretToken_Model1\\">Secret Token</a>
            <div class=\\"modal fade\\" id=\\"id_secretToken_Model1\\" tabindex=\\"-1\\" aria-labelledby=\\"exampleModalLabel\\" aria-hidden=\\"true\\">
              <div class=\\"modal-dialog modal-dialog-centered\\">
                <div class=\\"modal-content\\">
                  <div class=\\"modal-header\\">
                    <h5 class=\\"modal-title\\" id=\\"exampleModalLabel\\">Sensor 01</h5><button type=\\"button\\" class=\\"close\\" data-dismiss=\\"modal\\" aria-label=\\"Close\\"><span aria-hidden=\\"true\\">×</span></button>
                  </div>
                  <div class=\\"modal-body\\">
                    <h6>Sensor ID: 1</h6>
                    <h6>Secret Token: </h6>
                  </div>
                  <div class=\\"modal-footer\\"><button type=\\"button\\" class=\\"btn btn-secondary\\" data-dismiss=\\"modal\\">Close</button></div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>"
  `);
  global.fetch.mockRestore();
});
