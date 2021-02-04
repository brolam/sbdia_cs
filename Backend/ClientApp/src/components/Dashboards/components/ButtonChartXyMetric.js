import React from 'react'

export default function ButtonChartXyMetric(props) {
  const { title, selected, value, onClick } = props;
  return (
    <li class="nav-item">
      <a
        className={selected ? "nav-link active" : "nav-link"}
        onClick={(e) => onClick()}>{title} <span className="badge bg-secondary text-white">{value}</span>
      </a>
    </li>
  )
}