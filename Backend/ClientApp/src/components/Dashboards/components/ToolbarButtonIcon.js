import React from 'react'

export default function ToolbarButtonIcon(props) {
  const { icon, onClick } = props;
  return (
    <li className="nav-item">
      <div className="nav-link" to="#" role="button" onClick={(e) => onClick()}>
        {icon()}
      </div>
    </li>
  )
}