import React, { useRef } from 'react';
import moment from 'moment';

export default function ToolbarDropdownDays(props) {
  const modal = useRef();
  const { selectedDay, onSelectedDay } = props;
  const strDate = moment(
    new Date(selectedDay.year, selectedDay.month, selectedDay.day)
  ).format("YYYY-MM-DD");

  const show = () => {
    modal.current.style.display = "block";
  }

  const close = () => {
    modal.current.style.display = "none";
  }

  const onChangeDate = (value) => {
    onSelectedDay(value.target.value);
    close();
  }

  return (
    <li className="nav-item dropdown" onClick={show} >
      <div className="nav-link">
        <h4 className="dropdown-toggle" to="#" role="button" aria-haspopup="true" aria-expanded="false">
          {selectedDay.year}/{selectedDay.month}/{selectedDay.day}
        </h4>
      </div>
      <div ref={modal} className="modal fade show" aria-hidden="true" role="dialog" style={{ display: "none" }} >
        <div className="modal-dialog modal-sm">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title">Select Day</h5>
              <button type="button" className="close" aria-label="Close" onClick={close}>
                <span aria-hidden="true">&times;</span>
              </button>
            </div>
            <div className="modal-body">
              <input
                type="date"
                style={{ width: "100%" }}
                value={strDate}
                onChange={onChangeDate}
                onKeyPress={() => false}
              />
            </div>
            <div className="modal-footer">
              <button
                type="button"
                className="btn btn-secondary"
                onClick={close}
              >Close</button>
            </div>
          </div>
        </div>
      </div>
    </li >
  )
}
