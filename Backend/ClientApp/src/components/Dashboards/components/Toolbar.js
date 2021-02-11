import React from 'react'
import ButtonChartXyMetric from './ButtonChartXyMetric'
import DropdownSensors from './DropdownSensors';
import DropdownDays from './DropdownDays'

export default function Toolbar(props) {
  const {
    loading,
    selectedSensor,
    sensors,
    onSelectedSensor,
    selectedDay,
    days,
    onSelectedDay
  } = props;
  return (
    <div className="pt-3 pb-2 mb-3 border-bottom">
      <ul className="nav nav-pills">
        <DropdownSensors
          loading={loading}
          selectedSensor={selectedSensor}
          sensors={sensors}
          onSelectedSensor={onSelectedSensor}
        />
        <ButtonChartXyMetric title="Kwh" selected={true} value="0.00" onClick={() => { }} />
        <ButtonChartXyMetric title="Duration" selected={false} value="0.00" onClick={() => { }} />
        <DropdownDays
          loading={loading}
          selectedDay={selectedDay}
          days={days}
          onSelectedDay={onSelectedDay}
        />
      </ul>
    </div>
  )
}   
