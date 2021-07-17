import React from 'react';
import { DatePicker } from 'antd';
import moment from 'moment';

export default function ToolbarDropdownDays(props) {
  const dateFormat = 'YYYY/MM/DD';
  const { selectedDay, onSelectedDay } = props;
  const strDate = moment(selectedDay, dateFormat);
  const onChange = (date, dateString) => {
    onSelectedDay(dateString)
  }

  return (
    <li className="nav-item" >
      <div className="nav-link">
        <DatePicker
          defaultValue={strDate}
          format={dateFormat}
          onChange={onChange}
          allowClear={false}
        />
      </div>
    </li >
  )
}
