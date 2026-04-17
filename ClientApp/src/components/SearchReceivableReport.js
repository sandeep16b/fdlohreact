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
  Badge
} from 'reactstrap';
import { Link } from 'react-router-dom';
import '@fortawesome/fontawesome-free/css/all.min.css';
import { toast } from 'react-toastify';
import receivableReportService from '../services/receivableReportService';

export class SearchReceivableReport extends Component {
  static displayName = SearchReceivableReport.name;

  constructor(props) {
    super(props);
    this.state = {
      isAuthenticated: true,      searchResults: [],
      filteredResults: [],

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

      // Filter state
      filters: {
        id: '',
        organization: '',
        procurementType: '',
        purchaseOrderNumber: '',
        contractNumber: '',
        createdDate: '',
        createdBy: '',
        orderStatus: '',
        rrStatus: ''
      }
    };
  }

  async componentDidMount() {
    // Check if user is authenticated before loading data
    const authResponse = await fetch("/api/user", { credentials: "include" });
    const authData = await authResponse.json();
    if (!authData.isAuthenticated) {
      this.setState({ isAuthenticated: false });
      return;
    }
    try {
      // Load all reports on page mount
      await this.loadAllReports();
    } catch (error) {
      console.error('Error in componentDidMount:', error);
      this.setState({ isAuthenticated: false });
      toast.error('Please log in to access the application.');
      this.setState({ isLoading: false });
    }
  }

  // Load all receivable reports
  loadAllReports = async (page = 1) => {
    try {
      this.setState({ isLoading: true, showError: false });

      // Get all reports without specific search criteria
      const searchCriteria = receivableReportService.buildSearchCriteria({
        pageNumber: page,
        pageSize: this.state.pageSize
      });

      const result = await receivableReportService.searchReceivableReports(searchCriteria);

      // Validate search results
      const validResult = result && typeof result === 'object' ? result : {};
      const searchResults = Array.isArray(validResult.items) ? validResult.items : [];

      this.setState({
        searchResults: searchResults,
        filteredResults: searchResults,
        totalCount: parseInt(validResult.totalCount) || 0,
        totalPages: parseInt(validResult.totalPages) || 0,
        currentPage: parseInt(validResult.pageNumber) || 1,
        isLoading: false
      });

    } catch (error) {
      console.error('Error loading receivable reports:', error);
      toast.error('Failed to load receivable reports. Please try again.');
      this.setState({
        isLoading: false
      });
    }
  };

  handlePageChange = async (page) => {
    await this.loadAllReports(page);
  };

  handleFilterChange = (e) => {
    const { name, value } = e.target;
    const filters = { ...this.state.filters, [name]: value };

    // Filter the search results based on all filter values
    const filtered = this.state.searchResults.filter(result => {
      return (
        (result.id?.toString().toLowerCase().includes(filters.id.toLowerCase()) || filters.id === '') &&
        ((result.organizationName || result.organizationCode || 'N/A').toLowerCase().includes(filters.organization.toLowerCase()) || filters.organization === '') &&
        ((result.procurementType || 'N/A').toLowerCase().includes(filters.procurementType.toLowerCase()) || filters.procurementType === '') &&
        ((result.purchaseOrderNumber || 'N/A').toLowerCase().includes(filters.purchaseOrderNumber.toLowerCase()) || filters.purchaseOrderNumber === '') &&
        ((result.contractNumber || 'N/A').toLowerCase().includes(filters.contractNumber.toLowerCase()) || filters.contractNumber === '') &&
        ((result.createdDate ? new Date(result.createdDate).toLocaleDateString() : 'N/A').includes(filters.createdDate) || filters.createdDate === '') &&
        ((result.createdBy || 'N/A').toLowerCase().includes(filters.createdBy.toLowerCase()) || filters.createdBy === '') &&
        ((result.orderStatus || 'N/A').toLowerCase().includes(filters.orderStatus.toLowerCase()) || filters.orderStatus === '') &&
        ((result.rrStatus || 'Draft').toLowerCase().includes(filters.rrStatus.toLowerCase()) || filters.rrStatus === '')
      );
    });

    this.setState({ filters, filteredResults: filtered });
  };

  getRRStatusBadge = (status) => {
    let color;
    switch (status) {
      case 'Draft':
        color = 'secondary';
        break;
      case 'Submitted':
        color = 'warning';
        break;
      case 'Complete':
        color = 'success';
        break;
      default:
        color = 'secondary';
    }
    return <Badge color={color}>{status}</Badge>;
  };

  getOrderStatusBadge = (status) => {
    const color = status === 'Complete' ? 'success' : 'warning';
    return <Badge color={color}>{status}</Badge>;
  };

  render() {
    const { searchResults, filteredResults, isLoading, totalCount, filters } = this.state;
    if (!this.state.isAuthenticated) {
      return <div style={{ padding: "2rem" }}>Please log in to view data.</div>;
    }

    return (
      <Container fluid>
        {/* Data Grid Card - VHCPP Style */}
        <Row style={{ marginTop: '2rem' }}>
          <Col>
            <Card>
              {/* Card Header with Title - Blue Background */}
              <CardHeader style={{ background: 'linear-gradient(to bottom, #0C7FA5 0%, #E8F7FB 100%)', padding: '1rem' }}>
                <h5 style={{ margin: 0, color: '#1A3A3A' }}>Receivable Reports</h5>
              </CardHeader>

              {/* Card Body */}
              <CardBody>
                {/* Action Buttons - LEFT Aligned */}
                <div style={{ marginBottom: '1.5rem', display: 'flex', gap: '0.5rem', alignItems: 'center' }}>
                  <Button
                    color="success"
                    tag={Link}
                    to="/receivable-report/create"
                    title="Create Receivable Report"
                    style={{ padding: '0.5rem 0.75rem' }}
                  >
                    <i className="fas fa-plus"></i>
                  </Button>
                  <Button
                    outline
                    color="secondary"
                    onClick={() => window.location.reload()}
                    title="Refresh"
                    style={{ padding: '0.5rem 0.75rem' }}
                  >
                    <i className="fas fa-sync"></i>
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
                    <Table responsive striped hover>
                      <thead>
                        <tr>
                          <th>Actions</th>
                          <th>ID</th>
                          <th>Organization</th>
                          <th>Procurement Type</th>
                          <th>Purchase Order Number</th>
                          <th>Contract Number</th>
                          <th>Created Date</th>
                          <th>Created By</th>
                          <th>Order Status</th>
                          <th>RR Status</th>
                        </tr>
                        <tr style={{ backgroundColor: '#f0f0f0' }}>
                          <th style={{ padding: '0.5rem' }}></th>
                          <th style={{ padding: '0.5rem' }}>
                            <input
                              type="text"
                              name="id"
                              value={filters.id}
                              onChange={this.handleFilterChange}
                              placeholder="Filter ID"
                              style={{ width: '100%', padding: '0.3rem', fontSize: '0.85rem' }}
                            />
                          </th>
                          <th style={{ padding: '0.5rem' }}>
                            <input
                              type="text"
                              name="organization"
                              value={filters.organization}
                              onChange={this.handleFilterChange}
                              placeholder="Filter Org"
                              style={{ width: '100%', padding: '0.3rem', fontSize: '0.85rem' }}
                            />
                          </th>
                          <th style={{ padding: '0.5rem' }}>
                            <input
                              type="text"
                              name="procurementType"
                              value={filters.procurementType}
                              onChange={this.handleFilterChange}
                              placeholder="Filter Type"
                              style={{ width: '100%', padding: '0.3rem', fontSize: '0.85rem' }}
                            />
                          </th>
                          <th style={{ padding: '0.5rem' }}>
                            <input
                              type="text"
                              name="purchaseOrderNumber"
                              value={filters.purchaseOrderNumber}
                              onChange={this.handleFilterChange}
                              placeholder="Filter PO"
                              style={{ width: '100%', padding: '0.3rem', fontSize: '0.85rem' }}
                            />
                          </th>
                          <th style={{ padding: '0.5rem' }}>
                            <input
                              type="text"
                              name="contractNumber"
                              value={filters.contractNumber}
                              onChange={this.handleFilterChange}
                              placeholder="Filter Contract"
                              style={{ width: '100%', padding: '0.3rem', fontSize: '0.85rem' }}
                            />
                          </th>
                          <th style={{ padding: '0.5rem' }}>
                            <input
                              type="text"
                              name="createdDate"
                              value={filters.createdDate}
                              onChange={this.handleFilterChange}
                              placeholder="Filter Date"
                              style={{ width: '100%', padding: '0.3rem', fontSize: '0.85rem' }}
                            />
                          </th>
                          <th style={{ padding: '0.5rem' }}>
                            <input
                              type="text"
                              name="createdBy"
                              value={filters.createdBy}
                              onChange={this.handleFilterChange}
                              placeholder="Filter By"
                              style={{ width: '100%', padding: '0.3rem', fontSize: '0.85rem' }}
                            />
                          </th>
                          <th style={{ padding: '0.5rem' }}>
                            <input
                              type="text"
                              name="orderStatus"
                              value={filters.orderStatus}
                              onChange={this.handleFilterChange}
                              placeholder="Filter Status"
                              style={{ width: '100%', padding: '0.3rem', fontSize: '0.85rem' }}
                            />
                          </th>
                          <th style={{ padding: '0.5rem' }}>
                            <input
                              type="text"
                              name="rrStatus"
                              value={filters.rrStatus}
                              onChange={this.handleFilterChange}
                              placeholder="Filter RR"
                              style={{ width: '100%', padding: '0.3rem', fontSize: '0.85rem' }}
                            />
                          </th>
                        </tr>
                      </thead>
                      <tbody>
                        {filteredResults.length === 0 ? (
                          <tr>
                            <td colSpan="10" className="text-center py-4">
                              <em>No records found</em>
                            </td>
                          </tr>
                        ) : (
                          filteredResults.map((result) => (
                            <tr key={result.id}>
                              <td>
                                <div style={{ display: 'flex', gap: '0.5rem' }}>
                                  <Button
                                    size="sm"
                                    outline
                                    color="primary"
                                    tag={Link}
                                    to={`/receivable-report/edit/${result.id}`}
                                    title="Edit"
                                    style={{ padding: '0.25rem 0.5rem' }}
                                  >
                                    <i className="fas fa-edit"></i>
                                  </Button>
                                  <Button
                                    size="sm"
                                    color="outline-secondary"
                                    tag={Link}
                                    to={`/receivable-report/view/${result.id}`}
                                    title="View"
                                    style={{ padding: '0.25rem 0.5rem' }}
                                  >
                                    <i className="fas fa-eye"></i>
                                  </Button>
                                </div>
                              </td>
                              <td><strong>{result.id}</strong></td>
                              <td>{result.organizationName || result.organizationCode || 'N/A'}</td>
                              <td>{result.procurementType || 'N/A'}</td>
                              <td>{result.purchaseOrderNumber || 'N/A'}</td>
                              <td>{result.contractNumber || 'N/A'}</td>
                              <td>{result.createdDate ? new Date(result.createdDate).toLocaleDateString() : 'N/A'}</td>
                              <td>{result.createdBy || 'N/A'}</td>
                              <td>{this.getOrderStatusBadge(result.orderStatus)}</td>
                              <td>{this.getRRStatusBadge(result.rrStatus || 'Draft')}</td>
                            </tr>
                          ))
                        )}
                      </tbody>
                    </Table>
                  </>
                )}
              </CardBody>
            </Card>
          </Col>
        </Row>
      </Container>
    );
  }
}



