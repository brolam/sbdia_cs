import React from 'react'

export default function ToolbarButtonChartMetric(props) {
  const { title, selected, value, onClick } = props;
  return (
    <li className="nav-item">
      <div
        className={selected ? "nav-link active" : "nav-link"}
        role="button"
        onClick={(e) => onClick()}>{title} <span className="badge bg-secondary text-white">{value}</span>
      </div>
    </li>
  )
}