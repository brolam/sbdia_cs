import React from "react"
import { NavItem, UncontrolledDropdown, NavLink, DropdownItem, DropdownMenu, DropdownToggle } from 'reactstrap';


export default function SensorMenu(props) {
  let { tag, isAuthenticated } = props;

  return (
    <NavItem >
      {
        isAuthenticated ?
          <UncontrolledDropdown nav inNavbar>
            <DropdownToggle nav caret>Sensors</DropdownToggle>
            <DropdownMenu right>
              <DropdownItem tag={tag} to="/sensors" active>Consultar</DropdownItem>
              <DropdownItem>New</DropdownItem>
            </DropdownMenu>
          </UncontrolledDropdown>
          :
          <NavLink tag={tag} className="text-dark" to="/sensors">
            Sensors
        </NavLink>
      }
    </NavItem>
  )
}