import React from 'react'

export default function ToolbarButtonIcon(props) {
  const { icon, onClick } = props;
  return (
    <li className="nav-item">
      <a className="nav-link" to="#" role="button" onClick={(e) => onClick()}>
        {icon()}
      </a>
    </li>
  )
}