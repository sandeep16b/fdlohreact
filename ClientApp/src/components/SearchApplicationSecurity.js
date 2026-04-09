import React, { Component } from 'react';
import {
  Container,
  Row,
  Col,
  Card,
  CardBody,
  CardHeader,
  Button,
  Table,
  Badge,
  Modal,
  ModalHeader,
  ModalBody,
  ModalFooter
} from 'reactstrap';
import { useNavigate } from 'react-router-dom';
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
  static displayName = SearchApplicationSecurityClass.name;

  constructor(props) {
    super(props);
    this.state = {
      searchResults: [],

      // Pagination
      currentPage: 1,
      pageSize: 10,
      totalCount: 0,
      totalPages: 0,

      // Loading states
      isLoading: true,

      // UI state
      showError: false,
      errorMessage: '',

      // Delete confirmation modal
      showDeleteModal: false,
      itemToDelete: null
    };
  }

  async componentDidMount() {
    try {
      // Load all records on page mount
      await this.loadAllRecords();
    } catch (error) {
      console.error('Error in componentDidMount:', error);
      toast.error('Failed to load data. Please refresh the page.');
      this.setState({ isLoading: false });
    }
  }

  // Load all application security records
  loadAllRecords = async (page = 1) => {
    try {
      this.setState({ isLoading: true, showError: false });

      // Get all records without specific search criteria
      const searchCriteria = applicationSecurityService.buildSearchCriteria({
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
        isLoading: false
      });

    } catch (error) {
      console.error('Error loading application security records:', error);
      toast.error('Failed to load application security records. Please try again.');
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
        itemToDelete: null
      });

      // Refresh the records
      await this.loadAllRecords(this.state.currentPage);

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

  handlePageChange = async (page) => {
    await this.loadAllRecords(page);
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
    const { searchResults, isLoading, showDeleteModal, itemToDelete, currentPage, totalPages, totalCount } = this.state;

    return (
      <Container fluid>
        {/* Data Grid Card - VHCPP Style */}
        <Row style={{ marginTop: '2rem' }}>
          <Col>
            <Card>
              {/* Card Header with Title - Blue Background */}
              <CardHeader style={{ background: 'linear-gradient(to bottom, #0C7FA5 0%, #E8F7FB 100%)', color: 'white', padding: '1rem' }}>
                <h5 style={{ margin: 0 }}>Application Security</h5>
              </CardHeader>

              {/* Card Body */}
              <CardBody>
                {/* Action Buttons - LEFT Aligned */}
                <div style={{ marginBottom: '1.5rem', display: 'flex', gap: '0.5rem', alignItems: 'center' }}>
                  <Button
                    color="success"
                    onClick={() => this.props.navigate('/application-security/create')}
                    title="Add New"
                    style={{ padding: '0.5rem 0.75rem' }}
                  >
                    <i className="fas fa-plus"></i>
                  </Button>
                  <Button
                    color="primary"
                    onClick={() => this.loadAllRecords(1)}
                    disabled={isLoading}
                    title="Search"
                    style={{ padding: '0.5rem 0.75rem' }}
                  >
                    <i className="fas fa-search"></i>
                  </Button>
                  <Button
                    outline
                    color="secondary"
                    onClick={() => window.location.reload()}
                    title="Clear Filters"
                    style={{ padding: '0.5rem 0.75rem' }}
                  >
                    <i className="fas fa-eraser"></i>
                  </Button>
                </div>

                {/* Data Grid Table */}
                {isLoading ? (
                  <div className="text-center py-4">
                    <i className="fas fa-spinner fa-spin fa-2x"></i>
                    <p className="mt-2">Loading...</p>
                  </div>
                ) : (
                  <>
                    <div style={{ marginBottom: '1rem', fontSize: '0.9rem', color: '#666' }}>
                      {totalCount} records found
                    </div>
                    <div className="table-responsive">
                      <Table responsive striped hover>
                        <thead>
                          <tr>
                            <th>Actions</th>
                            <th>ID</th>
                            <th>Username (Email)</th>
                            <th>Application Role</th>
                            <th>Created Date</th>
                            <th>Created By</th>
                            <th>Updated Date</th>
                            <th>Updated By</th>
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
                                <td>
                                  <div style={{ display: 'flex', gap: '0.5rem' }}>
                                    <Button
                                      size="sm"
                                      outline
                                      color="primary"
                                      onClick={() => this.props.navigate(`/application-security/edit/${result.id}`)}
                                      title="Edit"
                                      style={{ padding: '0.25rem 0.5rem' }}
                                    >
                                      <i className="fas fa-edit"></i>
                                    </Button>
                                    <Button
                                      size="sm"
                                      outline
                                      color="danger"
                                      onClick={() => this.handleDeleteClick(result)}
                                      title="Delete"
                                      style={{ padding: '0.25rem 0.5rem' }}
                                    >
                                      <i className="fas fa-trash"></i>
                                    </Button>
                                  </div>
                                </td>
                                <td><strong>{result.id}</strong></td>
                                <td>{result.username}</td>
                                <td>{this.getRoleBadge(result.applicationRoleName)}</td>
                                <td>{result.createdDate ? new Date(result.createdDate).toLocaleDateString() : 'N/A'}</td>
                                <td>{result.createdBy || 'N/A'}</td>
                                <td>{result.updatedDate ? new Date(result.updatedDate).toLocaleDateString() : 'N/A'}</td>
                                <td>{result.updatedBy || 'N/A'}</td>
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

