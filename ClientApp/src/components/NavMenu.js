import React, { Component } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink, UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor(props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true,
      isAuthenticated: false,
      userName: ''
    };
  }

  componentDidMount() {
    this.checkAuthStatus();
  }

  checkAuthStatus = async () => {
    try {
      const response = await fetch('/api/user', {
        credentials: 'include'
      });
      const data = await response.json();
      this.setState({
        isAuthenticated: data.isAuthenticated,
        userName: data.name || ''
      });
    } catch (error) {
      console.error('Error checking auth status:', error);
    }
  }

  toggleNavbar() {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  handleLogout = () => {
    window.location.href = '/MicrosoftIdentity/Account/SignOut';
  }

  render() {
    const { isAuthenticated, userName } = this.state;

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
              {isAuthenticated && (
                <>
                  <NavItem>
                    <NavLink tag={Link} className="navbar-link-custom" to="/receivable-report/search">Receivable Reports</NavLink>
                  </NavItem>
                  <NavItem>
                    <NavLink tag={Link} className="navbar-link-custom" to="/application-security/search">Application Security</NavLink>
                  </NavItem>
                </>
              )}
            </ul>
            <ul className="navbar-nav" style={{ marginLeft: 'auto' }}>
              {isAuthenticated ? (
                <NavItem>
                  <UncontrolledDropdown nav inNavbar>
                    <DropdownToggle nav caret className="navbar-link-custom">
                      <i className="fas fa-user-circle"></i> {userName}
                    </DropdownToggle>
                    <DropdownMenu right>
                      <DropdownItem onClick={this.handleLogout}>
                        <i className="fas fa-sign-out-alt"></i> Logout
                      </DropdownItem>
                    </DropdownMenu>
                  </UncontrolledDropdown>
                </NavItem>
              ) : (
                <NavItem>
                  <NavLink href="/account/login" className="navbar-link-custom">
                    <i className="fas fa-sign-in-alt"></i> Login
                  </NavLink>
                </NavItem>
              )}
            </ul>
          </Collapse>
        </Navbar>
      </header>
    );
  }
}
