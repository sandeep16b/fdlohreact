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
      errorMessage: ''
    };
  }

  async componentDidMount() {
    try {
      // Load all reports on page mount
      await this.loadAllReports();
    } catch (error) {
      console.error('Error in componentDidMount:', error);
      toast.error('Failed to load data. Please refresh the page.');
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
    const { searchResults, isLoading, totalCount } = this.state;

    return (
      <Container fluid>
        {/* Data Grid Card - VHCPP Style */}
        <Row style={{ marginTop: '2rem' }}>
          <Col>
            <Card>
              {/* Card Header with Title - Blue Background */}
              <CardHeader style={{ background: 'linear-gradient(to bottom, #0C7FA5 0%, #E8F7FB 100%)', color: 'white', padding: '1rem' }}>
                <h5 style={{ margin: 0 }}>Receivable Reports</h5>
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
                    color="primary"
                    onClick={() => this.loadAllReports(1)}
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
                      </thead>
                      <tbody>
                        {searchResults.length === 0 ? (
                          <tr>
                            <td colSpan="10" className="text-center py-4">
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



