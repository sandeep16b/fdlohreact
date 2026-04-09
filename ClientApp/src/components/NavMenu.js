import React, { Component } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor(props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true
    };
  }

  toggleNavbar() {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  render() {
    return (
      <header>
        <Navbar className="navbar-custom navbar-expand-sm navbar-toggleable-sm" style={{ backgroundColor: '#0C7FA5', padding: '0' }}>
          <div className="navbar-brand-custom" tag={Link} to="/receivable-report/search">
            <img src="/fldoh-logo.jpeg" alt="FL DOH Logo" className="navbar-logo" />
            <span className="navbar-title">Asset Management System</span>
          </div>
          <NavbarToggler onClick={this.toggleNavbar} className="navbar-toggle-custom" />
          <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
            <ul className="navbar-nav flex-grow">
              <NavItem>
                <NavLink tag={Link} className="navbar-link-custom" to="/receivable-report/search">Receivable Reports</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} className="navbar-link-custom" to="/application-security/search">Application Security</NavLink>
              </NavItem>
            </ul>
          </Collapse>
        </Navbar>
      </header>
    );
  }
}
