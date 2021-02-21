import React from "react";
import { render, unmountComponentAtNode } from "react-dom";
import { act } from "react-dom/test-utils";
import pretty from "pretty";
import ToolbarButtonIcon from "../../../components/Dashboards/components/ToolbarButtonIcon";
import ToolbarButtonChartMetric from "../../../components/Dashboards/components/ToolbarButtonChartMetric";
import ToolbarDropdownDays from "../../../components/Dashboards/components/ToolbarDropdownDays";
import ToolbarDropdownSensors from "../../../components/Dashboards/components/ToolbarDropdownSensors";
import Toolbar from "../../../components/Dashboards/components/Toolbar";

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

test("render ToolbarButtonIcon", async () => {
  await act(async () => {
    render(<ToolbarButtonIcon icon={() => { }} onClick={() => { }} />, container);
  });
  expect(pretty(container.innerHTML)).toMatchInlineSnapshot(
    `"<li class=\\"nav-item\\"><a class=\\"nav-link\\" to=\\"#\\" role=\\"button\\"></a></li>"`
  );
});

test("render ToolbarButtonChartMetric", async () => {
  await act(async () => {
    render(
      <ToolbarButtonChartMetric
        title="Kwh"
        selected={true}
        value={0.0}
        onClick={() => { }}
      />,
      container
    );
  });
  expect(pretty(container.innerHTML)).toMatchInlineSnapshot(
    `"<li class=\\"nav-item\\"><a class=\\"nav-link active\\" role=\\"button\\">Kwh <span class=\\"badge bg-secondary text-white\\">0</span></a></li>"`
  );
});

test("render ToolbarDropdownDays loading", async () => {
  const selectedDay = { year: "2001", month: "01", day: "01" };
  let days = ["2001/01/01", "2001/01/02"];
  const onSelectedDay = (selectedDay) => { };
  await act(async () => {
    render(
      <ToolbarDropdownDays
        loading={true}
        selectedDay={selectedDay}
        days={days}
        onSelectedDay={onSelectedDay}
      />,
      container
    );
  });
  expect(pretty(container.innerHTML)).toMatchInlineSnapshot(`
    "<li class=\\"nav-item dropdown\\"><a class=\\"nav-link\\">
        <h4 class=\\"dropdown-toggle\\" to=\\"#\\" id=\\"dropdownDays\\" role=\\"button\\" data-toggle=\\"dropdown\\" aria-haspopup=\\"true\\" aria-expanded=\\"false\\">2001/01/01</h4>
        <ul class=\\"dropdown-menu\\" aria-labelledby=\\"dropdownDays\\"></ul>
      </a></li>"
  `);
});

test("render ToolbarDropdownDays not loading", async () => {
  const selectedDay = { year: "2001", month: "01", day: "01" };
  let days = ["2001/01/01", "2001/01/02"];
  const onSelectedDay = (selectedDay) => { };
  await act(async () => {
    render(
      <ToolbarDropdownDays
        loading={false}
        selectedDay={selectedDay}
        days={days}
        onSelectedDay={onSelectedDay}
      />,
      container
    );
  });
  expect(pretty(container.innerHTML)).toMatchInlineSnapshot(`
    "<li class=\\"nav-item dropdown\\"><a class=\\"nav-link\\">
        <h4 class=\\"dropdown-toggle\\" to=\\"#\\" id=\\"dropdownDays\\" role=\\"button\\" data-toggle=\\"dropdown\\" aria-haspopup=\\"true\\" aria-expanded=\\"false\\">2001/01/01</h4>
        <ul class=\\"dropdown-menu\\" aria-labelledby=\\"dropdownDays\\">
          <li class=\\"dropdown-item\\">2001/01/01</li>
          <li class=\\"dropdown-item\\">2001/01/02</li>
        </ul>
      </a></li>"
  `);
});

test("render ToolbarDropdownSensors loading", async () => {
  const loading = true;
  const selectedSensor = null;
  let sensors = [];
  const onSelectedSensor = (selectedSensor) => { };
  await act(async () => {
    render(
      <ToolbarDropdownSensors
        loading={loading}
        selectedSensor={selectedSensor}
        sensors={sensors}
        onSelectedSensor={onSelectedSensor}
      />,
      container
    );
  });
  expect(pretty(container.innerHTML)).toMatchInlineSnapshot(`
    "<li class=\\"nav-item dropdown\\"><a class=\\"nav-link\\">
        <h4 class=\\"dropdown-toggle\\" to=\\"#\\" id=\\"dropdownSensors\\" role=\\"button\\" data-toggle=\\"dropdown\\" aria-haspopup=\\"true\\" aria-expanded=\\"false\\">Dashboard loading</h4>
        <ul class=\\"dropdown-menu\\" aria-labelledby=\\"dropdownSensors\\"></ul>
      </a></li>"
  `);
});

test("render ToolbarDropdownSensors not loading", async () => {
  const loading = false;
  const selectedSensor = { id: 1, name: "OHA-1" };
  let sensors = [
    { id: 1, name: "OHA-1" },
    { id: 2, name: "OHA-2" },
  ];
  const onSelectedSensor = (selectedSensor) => { };
  await act(async () => {
    render(
      <ToolbarDropdownSensors
        loading={loading}
        selectedSensor={selectedSensor}
        sensors={sensors}
        onSelectedSensor={onSelectedSensor}
      />,
      container
    );
  });
  expect(pretty(container.innerHTML)).toMatchInlineSnapshot(`
    "<li class=\\"nav-item dropdown\\"><a class=\\"nav-link\\">
        <h4 class=\\"dropdown-toggle\\" to=\\"#\\" id=\\"dropdownSensors\\" role=\\"button\\" data-toggle=\\"dropdown\\" aria-haspopup=\\"true\\" aria-expanded=\\"false\\">OHA-1</h4>
        <ul class=\\"dropdown-menu\\" aria-labelledby=\\"dropdownSensors\\">
          <li class=\\"dropdown-item\\">OHA-1</li>
          <li class=\\"dropdown-item\\">OHA-2</li>
        </ul>
      </a></li>"
  `);
});

test("render Toolbar loading", async () => {
  const [CHART_XY_KWH, CHART_XY_DURATION] = [
    "CHART_XY_KWH",
    "CHART_XY_DURATION",
  ];
  const loading = true;
  const selectedSensor = null;
  let sensors = [];
  const selectedDay = { year: "2001", month: "01", day: "01" };
  let days = ["2001/01/01", "2001/01/02"];
  const onSelectedSensor = (selectedSensor) => { };
  const onSelectedDay = (seletedDay) => { };
  const onSelectedChartMetric = (seletedDay) => { };
  const onRefresh = () => { };
  await act(async () => {
    render(
      <Toolbar
        loading={loading}
        selectedSensor={selectedSensor}
        sensors={sensors}
        onSelectedSensor={onSelectedSensor}
        selectedDay={selectedDay}
        days={days}
        onSelectedDay={onSelectedDay}
        chartMetrics={[
          { key: CHART_XY_KWH, title: "Kwh", selected: true, value: 0.0 },
          {
            key: CHART_XY_DURATION,
            title: "Duration",
            selected: false,
            value: 0.0,
          },
        ]}
        onSelectedChartMetric={onSelectedChartMetric}
        onRefresh={onRefresh}
      />,
      container
    );
  });
  expect(pretty(container.innerHTML)).toMatchInlineSnapshot(`
    "<div class=\\"pt-3 pb-2 mb-3 border-bottom\\">
      <ul class=\\"nav nav-pills justify-content-center\\">
        <li class=\\"nav-item dropdown\\"><a class=\\"nav-link\\">
            <h4 class=\\"dropdown-toggle\\" to=\\"#\\" id=\\"dropdownSensors\\" role=\\"button\\" data-toggle=\\"dropdown\\" aria-haspopup=\\"true\\" aria-expanded=\\"false\\">Dashboard loading</h4>
            <ul class=\\"dropdown-menu\\" aria-labelledby=\\"dropdownSensors\\"></ul>
          </a></li>
        <li class=\\"nav-item dropdown\\"><a class=\\"nav-link\\">
            <h4 class=\\"dropdown-toggle\\" to=\\"#\\" id=\\"dropdownDays\\" role=\\"button\\" data-toggle=\\"dropdown\\" aria-haspopup=\\"true\\" aria-expanded=\\"false\\">2001/01/01</h4>
            <ul class=\\"dropdown-menu\\" aria-labelledby=\\"dropdownDays\\"></ul>
          </a></li>
        <li class=\\"nav-item\\"><a class=\\"nav-link active\\" role=\\"button\\">Kwh <span class=\\"badge bg-secondary text-white\\">0</span></a></li>
        <li class=\\"nav-item\\"><a class=\\"nav-link\\" role=\\"button\\">Duration <span class=\\"badge bg-secondary text-white\\">0</span></a></li>
        <li class=\\"nav-item\\"><a class=\\"nav-link\\" to=\\"#\\" role=\\"button\\"><svg width=\\"24\\" height=\\"24\\" fill=\\"currentColor\\" class=\\"bi bi-arrow-clockwise\\" viewBox=\\"0 0 16 16\\">
              <path fill-rule=\\"evenodd\\" d=\\"M8 3a5 5 0 1 0 4.546 2.914.5.5 0 0 1 .908-.417A6 6 0 1 1 8 2v1z\\"></path>
              <path d=\\"M8 4.466V.534a.25.25 0 0 1 .41-.192l2.36 1.966c.12.1.12.284 0 .384L8.41 4.658A.25.25 0 0 1 8 4.466z\\"></path>
            </svg></a></li>
        <li class=\\"nav-item\\"><a class=\\"nav-link\\" to=\\"#\\" role=\\"button\\"><svg width=\\"24\\" height=\\"24\\" fill=\\"currentColor\\" class=\\"bi bi-gear\\" viewBox=\\"0 0 16 16\\">
              <path d=\\"M8 4.754a3.246 3.246 0 1 0 0 6.492 3.246 3.246 0 0 0 0-6.492zM5.754 8a2.246 2.246 0 1 1 4.492 0 2.246 2.246 0 0 1-4.492 0z\\"></path>
              <path d=\\"M9.796 1.343c-.527-1.79-3.065-1.79-3.592 0l-.094.319a.873.873 0 0 1-1.255.52l-.292-.16c-1.64-.892-3.433.902-2.54 2.541l.159.292a.873.873 0 0 1-.52 1.255l-.319.094c-1.79.527-1.79 3.065 0 3.592l.319.094a.873.873 0 0 1 .52 1.255l-.16.292c-.892 1.64.901 3.434 2.541 2.54l.292-.159a.873.873 0 0 1 1.255.52l.094.319c.527 1.79 3.065 1.79 3.592 0l.094-.319a.873.873 0 0 1 1.255-.52l.292.16c1.64.893 3.434-.902 2.54-2.541l-.159-.292a.873.873 0 0 1 .52-1.255l.319-.094c1.79-.527 1.79-3.065 0-3.592l-.319-.094a.873.873 0 0 1-.52-1.255l.16-.292c.893-1.64-.902-3.433-2.541-2.54l-.292.159a.873.873 0 0 1-1.255-.52l-.094-.319zm-2.633.283c.246-.835 1.428-.835 1.674 0l.094.319a1.873 1.873 0 0 0 2.693 1.115l.291-.16c.764-.415 1.6.42 1.184 1.185l-.159.292a1.873 1.873 0 0 0 1.116 2.692l.318.094c.835.246.835 1.428 0 1.674l-.319.094a1.873 1.873 0 0 0-1.115 2.693l.16.291c.415.764-.42 1.6-1.185 1.184l-.291-.159a1.873 1.873 0 0 0-2.693 1.116l-.094.318c-.246.835-1.428.835-1.674 0l-.094-.319a1.873 1.873 0 0 0-2.692-1.115l-.292.16c-.764.415-1.6-.42-1.184-1.185l.159-.291A1.873 1.873 0 0 0 1.945 8.93l-.319-.094c-.835-.246-.835-1.428 0-1.674l.319-.094A1.873 1.873 0 0 0 3.06 4.377l-.16-.292c-.415-.764.42-1.6 1.185-1.184l.292.159a1.873 1.873 0 0 0 2.692-1.115l.094-.319z\\"></path>
            </svg></a></li>
      </ul>
    </div>"
  `);
});

test("render Toolbar not loading", async () => {
  const [CHART_XY_KWH, CHART_XY_DURATION] = [
    "CHART_XY_KWH",
    "CHART_XY_DURATION",
  ];
  const loading = false;
  const selectedSensor = { id: 1, name: "OHA-1" };
  let sensors = [
    { id: 1, name: "OHA-1" },
    { id: 2, name: "OHA-2" },
  ];
  const selectedDay = { year: "2001", month: "01", day: "01" };
  let days = ["2001/01/01", "2001/01/02"];
  const onSelectedSensor = (selectedSensor) => { };
  const onSelectedDay = (seletedDay) => { };
  const onSelectedChartMetric = (seletedDay) => { };
  const onRefresh = () => { };
  await act(async () => {
    render(
      <Toolbar
        loading={loading}
        selectedSensor={selectedSensor}
        sensors={sensors}
        onSelectedSensor={onSelectedSensor}
        selectedDay={selectedDay}
        days={days}
        onSelectedDay={onSelectedDay}
        chartMetrics={[
          { key: CHART_XY_KWH, title: "Kwh", selected: true, value: 0.0 },
          {
            key: CHART_XY_DURATION,
            title: "Duration",
            selected: false,
            value: 0.0,
          },
        ]}
        onSelectedChartMetric={onSelectedChartMetric}
        onRefresh={onRefresh}
      />,
      container
    );
  });
  expect(pretty(container.innerHTML)).toMatchInlineSnapshot(`
    "<div class=\\"pt-3 pb-2 mb-3 border-bottom\\">
      <ul class=\\"nav nav-pills justify-content-center\\">
        <li class=\\"nav-item dropdown\\"><a class=\\"nav-link\\">
            <h4 class=\\"dropdown-toggle\\" to=\\"#\\" id=\\"dropdownSensors\\" role=\\"button\\" data-toggle=\\"dropdown\\" aria-haspopup=\\"true\\" aria-expanded=\\"false\\">OHA-1</h4>
            <ul class=\\"dropdown-menu\\" aria-labelledby=\\"dropdownSensors\\">
              <li class=\\"dropdown-item\\">OHA-1</li>
              <li class=\\"dropdown-item\\">OHA-2</li>
            </ul>
          </a></li>
        <li class=\\"nav-item dropdown\\"><a class=\\"nav-link\\">
            <h4 class=\\"dropdown-toggle\\" to=\\"#\\" id=\\"dropdownDays\\" role=\\"button\\" data-toggle=\\"dropdown\\" aria-haspopup=\\"true\\" aria-expanded=\\"false\\">2001/01/01</h4>
            <ul class=\\"dropdown-menu\\" aria-labelledby=\\"dropdownDays\\">
              <li class=\\"dropdown-item\\">2001/01/01</li>
              <li class=\\"dropdown-item\\">2001/01/02</li>
            </ul>
          </a></li>
        <li class=\\"nav-item\\"><a class=\\"nav-link active\\" role=\\"button\\">Kwh <span class=\\"badge bg-secondary text-white\\">0</span></a></li>
        <li class=\\"nav-item\\"><a class=\\"nav-link\\" role=\\"button\\">Duration <span class=\\"badge bg-secondary text-white\\">0</span></a></li>
        <li class=\\"nav-item\\"><a class=\\"nav-link\\" to=\\"#\\" role=\\"button\\"><svg width=\\"24\\" height=\\"24\\" fill=\\"currentColor\\" class=\\"bi bi-arrow-clockwise\\" viewBox=\\"0 0 16 16\\">
              <path fill-rule=\\"evenodd\\" d=\\"M8 3a5 5 0 1 0 4.546 2.914.5.5 0 0 1 .908-.417A6 6 0 1 1 8 2v1z\\"></path>
              <path d=\\"M8 4.466V.534a.25.25 0 0 1 .41-.192l2.36 1.966c.12.1.12.284 0 .384L8.41 4.658A.25.25 0 0 1 8 4.466z\\"></path>
            </svg></a></li>
        <li class=\\"nav-item\\"><a class=\\"nav-link\\" to=\\"#\\" role=\\"button\\"><svg width=\\"24\\" height=\\"24\\" fill=\\"currentColor\\" class=\\"bi bi-gear\\" viewBox=\\"0 0 16 16\\">
              <path d=\\"M8 4.754a3.246 3.246 0 1 0 0 6.492 3.246 3.246 0 0 0 0-6.492zM5.754 8a2.246 2.246 0 1 1 4.492 0 2.246 2.246 0 0 1-4.492 0z\\"></path>
              <path d=\\"M9.796 1.343c-.527-1.79-3.065-1.79-3.592 0l-.094.319a.873.873 0 0 1-1.255.52l-.292-.16c-1.64-.892-3.433.902-2.54 2.541l.159.292a.873.873 0 0 1-.52 1.255l-.319.094c-1.79.527-1.79 3.065 0 3.592l.319.094a.873.873 0 0 1 .52 1.255l-.16.292c-.892 1.64.901 3.434 2.541 2.54l.292-.159a.873.873 0 0 1 1.255.52l.094.319c.527 1.79 3.065 1.79 3.592 0l.094-.319a.873.873 0 0 1 1.255-.52l.292.16c1.64.893 3.434-.902 2.54-2.541l-.159-.292a.873.873 0 0 1 .52-1.255l.319-.094c1.79-.527 1.79-3.065 0-3.592l-.319-.094a.873.873 0 0 1-.52-1.255l.16-.292c.893-1.64-.902-3.433-2.541-2.54l-.292.159a.873.873 0 0 1-1.255-.52l-.094-.319zm-2.633.283c.246-.835 1.428-.835 1.674 0l.094.319a1.873 1.873 0 0 0 2.693 1.115l.291-.16c.764-.415 1.6.42 1.184 1.185l-.159.292a1.873 1.873 0 0 0 1.116 2.692l.318.094c.835.246.835 1.428 0 1.674l-.319.094a1.873 1.873 0 0 0-1.115 2.693l.16.291c.415.764-.42 1.6-1.185 1.184l-.291-.159a1.873 1.873 0 0 0-2.693 1.116l-.094.318c-.246.835-1.428.835-1.674 0l-.094-.319a1.873 1.873 0 0 0-2.692-1.115l-.292.16c-.764.415-1.6-.42-1.184-1.185l.159-.291A1.873 1.873 0 0 0 1.945 8.93l-.319-.094c-.835-.246-.835-1.428 0-1.674l.319-.094A1.873 1.873 0 0 0 3.06 4.377l-.16-.292c-.415-.764.42-1.6 1.185-1.184l.292.159a1.873 1.873 0 0 0 2.692-1.115l.094-.319z\\"></path>
            </svg></a></li>
      </ul>
    </div>"
  `);
});
