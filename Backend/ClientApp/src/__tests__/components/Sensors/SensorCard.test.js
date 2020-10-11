import React from "react";
import { render, unmountComponentAtNode } from "react-dom";
import { act } from "react-dom/test-utils";
import SensorCard from "../../../components/Sensors/SensorCard";
import pretty from "pretty";

let container = null;
beforeEach(() => {
  // configurar o elemento do DOM como o alvo da renderização
  container = document.createElement("div");
  document.body.appendChild(container);
});

afterEach(() => {
  // limpar na saída
  unmountComponentAtNode(container);
  container.remove();
  container = null;
});

test("render sensor card", async () => {
  let sensor = {
    name: "Sensor 01",
    sensorType: 0,
    sensorTypeName: "EnergyLog",
    logDurationMode: 14,
    logLastRecorded: "2020/10/11 12:52:59",
  };

  await act(async () => {
    render(<SensorCard sensor={sensor} />, container);
  });
  expect(pretty(container.innerHTML)).toMatchInlineSnapshot(`
    "<div class=\\"card\\" style=\\"width: 18rem;\\">
      <div class=\\"card-body\\">
        <h5 class=\\"card-title\\">Sensor 01</h5>
        <h6 class=\\"card-subtitle mb-2 text-muted\\">EnergyLog</h6>
        <p class=\\"card-text\\">Last log recorded 2020/10/11 12:52:59 with modal duration of 14 seconds.</p><a href=\\"#\\" class=\\"card-link\\">Dashboard</a>
      </div>
    </div>"
  `);
});
