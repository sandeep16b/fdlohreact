import React, { Component } from 'react';
import { 
  Container, 
  Row, 
  Col, 
  Card, 
  CardBody, 
  CardHeader, 
  Form, 
  FormGroup, 
  Label, 
  Input, 
  Button, 
  Table,
  Badge,
  Modal,
  ModalHeader,
  ModalBody,
  ModalFooter
} from 'reactstrap';
import { Link, useNavigate } from 'react-router-dom';
import '@fortawesome/fontawesome-free/css/all.min.css';
import { toast } from 'react-toastify';
import applicationSecurityService from '../services/applicationSecurityService';


// Wrapper component to handle navigation
function SearchApplicationSecurityWrapper() {
  const navigate = useNavigate();
  return <SearchApplicationSecurityClass navigate={navigate} />;
}

export { SearchApplicationSecurityWrapper as SearchApplicationSecurity };

class SearchApplicationSecurityClass extends Component {
  static displayName = SearchApplicationSecurityClass.name

  constructor(props) {
    super(props);
    this.state = {
      searchCriteria: {
        username: '',
        applicationRoleId: null,
        applicationRoleName: ''
      },
      searchResults: [],
      
      // Pagination
      currentPage: 1,
      pageSize: 10,
      totalCount: 0,
      totalPages: 0,
      
      // Loading states
      isLoading: false,
      isLoadingRoles: true,
      
      // Lookup data
      applicationRoles: [],
      
      // UI state
      showError: false,
      errorMessage: '',
      hasSearched: false,
      
      // Delete confirmation modal
      showDeleteModal: false,
      itemToDelete: null
    };
  }

  async componentDidMount() {
    try {
      await this.loadApplicationRoles();
      // Perform initial search to show all records
      await this.performSearch();
    } catch (error) {
      console.error('Error in componentDidMount:', error);
      toast.error('Failed to load data. Please refresh the page.');
      this.setState({ 
        isLoadingRoles: false
      });
    }
  }

  // Load application roles from API
  loadApplicationRoles = async () => {
    try {
      this.setState({ isLoadingRoles: true });
      const roles = await applicationSecurityService.getApplicationRoles();
      
      this.setState({
        applicationRoles: Array.isArray(roles) ? roles : [],
        isLoadingRoles: false
      });
    } catch (error) {
      console.error('Error loading application roles:', error);
      toast.error('Failed to load application roles.');
      this.setState({ 
        isLoadingRoles: false
      });
    }
  };

  // Perform search using API
  performSearch = async (page = 1) => {
    try {
      this.setState({ isLoading: true, showError: false });
      
      const searchCriteria = applicationSecurityService.buildSearchCriteria({
        username: this.state.searchCriteria.username || null,
        applicationRoleId: this.state.searchCriteria.applicationRoleId || null,
        applicationRoleName: this.state.searchCriteria.applicationRoleName || null,
        pageNumber: page,
        pageSize: this.state.pageSize
      });

      const result = await applicationSecurityService.searchApplicationSecurity(searchCriteria);
      
      // Validate search results
      const validResult = result && typeof result === 'object' ? result : {};
      const searchResults = Array.isArray(validResult.items) ? validResult.items : [];
      
      this.setState({
        searchResults: searchResults,
        totalCount: parseInt(validResult.totalCount) || 0,
        totalPages: parseInt(validResult.totalPages) || 0,
        currentPage: parseInt(validResult.pageNumber) || 1,
        hasSearched: true,
        isLoading: false
      });
      
    } catch (error) {
      console.error('Error performing search:', error);
      toast.error('Failed to search application security records. Please try again.');
      this.setState({ 
        isLoading: false
      });
    }
  };

  // Delete application security record
  deleteApplicationSecurity = async (id) => {
    try {
      this.setState({ isLoading: true });
      await applicationSecurityService.deleteApplicationSecurity(id);
      
      toast.success('Application security record deleted successfully.');
      this.setState({ 
        showDeleteModal: false,
        itemToDelete: null,
        isLoading: false
      });
      
      // Refresh the search results
      await this.performSearch(this.state.currentPage);
      
    } catch (error) {
      console.error('Error deleting application security record:', error);
      const errorMessage = error.message || 'Failed to delete application security record. Please try again.';
      toast.error(errorMessage);
      this.setState({ 
        showDeleteModal: false,
        itemToDelete: null,
        isLoading: false
      });
    }
  };

  handleInputChange = (field, value) => {
    this.setState({
      searchCriteria: {
        ...this.state.searchCriteria,
        [field]: value
      }
    });
  };

  handleSearch = async () => {
    await this.performSearch(1);
  };

  handleClear = () => {
    this.setState({
      searchCriteria: {
        username: '',
        applicationRoleId: null,
        applicationRoleName: ''
      },
      searchResults: [],
      hasSearched: false,
      currentPage: 1,
      totalCount: 0,
      totalPages: 0
    });
  };

  handlePageChange = async (page) => {
    await this.performSearch(page);
  };

  handleDeleteClick = (item) => {
    this.setState({
      showDeleteModal: true,
      itemToDelete: item
    });
  };

  handleDeleteConfirm = async () => {
    if (this.state.itemToDelete) {
      await this.deleteApplicationSecurity(this.state.itemToDelete.id);
    }
  };

  handleDeleteCancel = () => {
    this.setState({
      showDeleteModal: false,
      itemToDelete: null
    });
  };

  getRoleBadge = (roleName) => {
    if (!roleName) return null;
    
    const roleColors = {
      'Initiator': 'primary',
      'Facility Admin': 'success',
      'Custodian': 'info',
      'Delegation': 'warning'
    };
    
    return (
      <Badge color={roleColors[roleName] || 'secondary'}>
        {roleName}
      </Badge>
    );
  };

  render() {
    const { searchCriteria, searchResults, hasSearched, isLoading, applicationRoles, showDeleteModal, itemToDelete, currentPage, totalPages, totalCount } = this.state;

    return (
      <Container fluid>
        <Row>
          <Col>
            <div className="d-flex justify-content-between align-items-center">
              <h2>Application Security Maintenance</h2>
              <Button
                color="primary"
                onClick={() => this.props.navigate('/application-security/create')}
              >
                <i className="fas fa-plus me-2"></i>Add New
              </Button>
            </div>
          </Col>
        </Row>

        {/* Search Form */}
        <Row className="mb-4">
          <Col>
            <Card>
              <CardHeader>
                <h5>Search Criteria</h5>
              </CardHeader>
              <CardBody>
                <Form>
                  <Row>
                    <Col md={6}>
                      <FormGroup>
                        <Label for="username">Username (Email)</Label>
                        <Input
                          type="email"
                          id="username"
                          value={searchCriteria.username}
                          onChange={(e) => this.handleInputChange('username', e.target.value)}
                          placeholder="Enter email address"
                        />
                      </FormGroup>
                    </Col>
                    <Col md={6}>
                      <FormGroup>
                        <Label for="applicationRoleId">Application Role</Label>
                        <Input
                          type="select"
                          id="applicationRoleId"
                          value={searchCriteria.applicationRoleId || ''}
                          onChange={(e) => this.handleInputChange('applicationRoleId', e.target.value ? parseInt(e.target.value) : null)}
                          disabled={this.state.isLoadingRoles}
                        >
                          <option value="">All Roles</option>
                          {applicationRoles.map(role => (
                            <option key={role.id} value={role.id}>{role.name}</option>
                          ))}
                        </Input>
                      </FormGroup>
                    </Col>
                  </Row>
                  <Row>
                    <Col>
                      <Button color="primary" onClick={this.handleSearch} className="me-2" disabled={isLoading}>
                        <i className="fas fa-search me-2"></i>{isLoading ? 'Searching...' : 'Search'}
                      </Button>
                      <Button color="secondary" onClick={this.handleClear} className="me-2">
                        <i className="fas fa-eraser me-2"></i>Clear
                      </Button>
                    </Col>
                  </Row>
                </Form>
              </CardBody>
            </Card>
          </Col>
        </Row>

        {/* Search Results */}
        {hasSearched && (
          <Row>
            <Col>
              <Card>
                <CardHeader>
                  <h5>Search Results ({totalCount} records found)</h5>
                </CardHeader>
                <CardBody>
                  {isLoading ? (
                    <div className="text-center py-4">
                      <i className="fas fa-spinner fa-spin fa-2x"></i>
                      <p className="mt-2">Loading...</p>
                    </div>
                  ) : (
                    <>
                      <div className="table-responsive">
                        <Table responsive striped hover>
                          <thead>
                            <tr>
                              <th>ID</th>
                              <th>Username (Email)</th>
                              <th>Application Role</th>
                              <th>Created Date</th>
                              <th>Created By</th>
                              <th>Updated Date</th>
                              <th>Updated By</th>
                              <th>Actions</th>
                            </tr>
                          </thead>
                          <tbody>
                            {searchResults.length === 0 ? (
                              <tr>
                                <td colSpan="8" className="text-center py-4">
                                  <em>No records found</em>
                                </td>
                              </tr>
                            ) : (
                              searchResults.map((result) => (
                                <tr key={result.id}>
                                  <td><strong>{result.id}</strong></td>
                                  <td>{result.username}</td>
                                  <td>{this.getRoleBadge(result.applicationRoleName)}</td>
                                  <td>{result.createdDate ? new Date(result.createdDate).toLocaleDateString() : 'N/A'}</td>
                                  <td>{result.createdBy || 'N/A'}</td>
                                  <td>{result.updatedDate ? new Date(result.updatedDate).toLocaleDateString() : 'N/A'}</td>
                                  <td>{result.updatedBy || 'N/A'}</td>
                                  <td>
                                    <Button
                                      size="sm"
                                      color="primary"
                                      onClick={() => this.props.navigate(`/application-security/edit/${result.id}`)}
                                      className="me-1"
                                      title="Edit"
                                    >
                                      <i className="fas fa-edit"></i>
                                    </Button>
                                    <Button
                                      size="sm"
                                      color="danger"
                                      onClick={() => this.handleDeleteClick(result)}
                                      title="Delete"
                                    >
                                      <i className="fas fa-trash"></i>
                                    </Button>
                                  </td>
                                </tr>
                              ))
                            )}
                          </tbody>
                        </Table>
                      </div>
                      
                      {/* Pagination */}
                      {totalPages > 1 && (
                        <div className="d-flex justify-content-between align-items-center mt-3">
                          <div>
                            Showing page {currentPage} of {totalPages} (Total: {totalCount} records)
                          </div>
                          <div>
                            <Button
                              color="secondary"
                              size="sm"
                              onClick={() => this.handlePageChange(currentPage - 1)}
                              disabled={currentPage === 1}
                              className="me-2"
                            >
                              <i className="fas fa-chevron-left"></i> Previous
                            </Button>
                            <Button
                              color="secondary"
                              size="sm"
                              onClick={() => this.handlePageChange(currentPage + 1)}
                              disabled={currentPage === totalPages}
                            >
                              Next <i className="fas fa-chevron-right"></i>
                            </Button>
                          </div>
                        </div>
                      )}
                    </>
                  )}
                </CardBody>
              </Card>
            </Col>
          </Row>
        )}

        {/* Delete Confirmation Modal */}
        <Modal isOpen={showDeleteModal} toggle={this.handleDeleteCancel}>
          <ModalHeader toggle={this.handleDeleteCancel}>Confirm Delete</ModalHeader>
          <ModalBody>
            Are you sure you want to delete the application security record for <strong>{itemToDelete?.username}</strong>?
            <br />
            <small className="text-muted">This action cannot be undone.</small>
          </ModalBody>
          <ModalFooter>
            <Button color="secondary" onClick={this.handleDeleteCancel} disabled={isLoading}>
              Cancel
            </Button>
            <Button color="danger" onClick={this.handleDeleteConfirm} disabled={isLoading}>
              {isLoading ? (
                <>
                  <i className="fas fa-spinner fa-spin me-2"></i>Deleting...
                </>
              ) : (
                'Delete'
              )}
            </Button>
          </ModalFooter>
        </Modal>
      </Container>
    );
  }
}

