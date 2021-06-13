import React from 'react';

export default function ToolbarDropdownDays(props) {
  const { loading, selectedDay, days, onSelectedDay } = props;
  return (
    <li className="nav-item dropdown">
      <div className="nav-link">
        <h4 className="dropdown-toggle" to="#" id="dropdownDays" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
          {selectedDay.year}/{selectedDay.month}/{selectedDay.day}
        </h4>
        <ul className="dropdown-menu" aria-labelledby="dropdownDays">
          {
            !loading && days.reverse().map(day =>
              <li key={day} className="dropdown-item" onClick={e => onSelectedDay(day)} >{day}</li>
            )
          }
        </ul>
      </div>
    </li>
  )
}
