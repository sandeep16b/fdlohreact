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
  InputGroup,
  InputGroupText
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
      searchCriteria: {
        requestedBy: '',
        organizationCode: '',
        locationCode: '',
        orderStatus: '',
        rrStatus: '',
        organizationName: '',
        createdDateFrom: '',
        createdDateTo: '',
        createdBy: '',
        priceRangeStart: '',
        priceRangeEnd: ''
      },
      searchResults: [],
      
      // Pagination
      currentPage: 1,
      pageSize: 10,
      totalCount: 0,
      totalPages: 0,
      
      // Loading states
      isLoading: false,
      isLoadingLookupData: true,
      
      // Lookup data
      organizations: [],
      locations: {},
      
      // UI state
      showError: false,
      errorMessage: '',
      showSuccess: false,
      successMessage: '',
      
      // Delete confirmation modal
      showDeleteModal: false,
      itemToDelete: null,
      hasSearched: false
    };
  }

  async componentDidMount() {
    try {
      await this.loadLookupData();
      // Perform initial search to show all reports
      await this.performSearch();
    } catch (error) {
      console.error('Error in componentDidMount:', error);
      toast.error('Failed to load data. Please refresh the page.');
      this.setState({ 
        isLoadingLookupData: false
      });
    }
  }

  // Load lookup data from API
  loadLookupData = async () => {
    try {
      this.setState({ isLoadingLookupData: true });
      const lookupData = await receivableReportService.getLookupData();
      
      // Validate API response
      const validLookupData = lookupData && typeof lookupData === 'object' ? lookupData : {};
      const organizations = Array.isArray(validLookupData.organizations) ? validLookupData.organizations : [];
      const locations = {};
      
      // Transform locations to the expected format
      if (validLookupData.locations && Array.isArray(validLookupData.locations)) {
        validLookupData.locations.forEach(loc => {
          if (loc && loc.code) {
            locations[loc.code] = {
              addressLine1: loc.addressLine1 || '',
              city: loc.city || '',
              county: loc.county || '',
              state: loc.state || '',
              postalCode: loc.postalCode || ''
            };
          }
        });
      }

      this.setState({
        organizations,
        locations,
        isLoadingLookupData: false
      });
    } catch (error) {
      console.error('Error loading lookup data:', error);
      toast.error('Failed to load lookup data. Using default values.');
      this.setState({ 
        isLoadingLookupData: false
      });
    }
  };

  // Perform search using API
  performSearch = async (page = 1) => {
    try {
      this.setState({ isLoading: true, showError: false });
      
      const searchCriteria = receivableReportService.buildSearchCriteria({
        organizationCode: this.state.searchCriteria.organizationCode || null,
        locationCode: this.state.searchCriteria.locationCode || null,
        orderStatus: this.state.searchCriteria.orderStatus || null,
        rrStatus: this.state.searchCriteria.rrStatus || null,
        createdDateFrom: this.state.searchCriteria.createdDateFrom || null,
        createdDateTo: this.state.searchCriteria.createdDateTo || null,
        createdBy: this.state.searchCriteria.createdBy || null,
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
        hasSearched: true,
        isLoading: false
      });
      
    } catch (error) {
      console.error('Error performing search:', error);
      toast.error('Failed to search receivable reports. Please try again.');
      this.setState({ 
        isLoading: false
      });
    }
  };

  // Delete receivable report
  deleteReceivableReport = async (id) => {
    try {
      await receivableReportService.deleteReceivableReport(id);
      
      toast.success('Receivable report deleted successfully.');
      this.setState({ 
        showDeleteModal: false,
        itemToDelete: null
      });
      
      // Refresh the search results
      await this.performSearch(this.state.currentPage);
      
    } catch (error) {
      console.error('Error deleting receivable report:', error);
      toast.error('Failed to delete receivable report. Please try again.');
      this.setState({ 
        showDeleteModal: false,
        itemToDelete: null
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
      await this.deleteReceivableReport(this.state.itemToDelete.id);
    }
  };

  handleDeleteCancel = () => {
    this.setState({
      showDeleteModal: false,
      itemToDelete: null
    });
  };

  handleClear = () => {
    this.setState({
      searchCriteria: {
        requestedBy: '',
        organizationCode: '',
        locationCode: '',
        orderStatus: '',
        rrStatus: '',
        organizationName: '',
        createdDateFrom: '',
        createdDateTo: '',
        createdBy: '',
        priceRangeStart: '',
        priceRangeEnd: ''
      },
      hasSearched: false
    });
  };

  getStatusBadge = (status) => {
    const color = status === 'Active' ? 'success' : status === 'Closed' ? 'secondary' : 'warning';
    return <Badge color={color}>{status}</Badge>;
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
    const { searchCriteria, searchResults, hasSearched } = this.state;

    return (
      <Container fluid>
        <Row>
          <Col>
            <h2>Search Receivable Reports</h2>
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
                        <Label for="requestedBy">Requested By</Label>
                        <Input
                          type="text"
                          id="requestedBy"
                          value={searchCriteria.requestedBy}
                          onChange={(e) => this.handleInputChange('requestedBy', e.target.value)}
                          placeholder="Enter requester name"
                        />
                      </FormGroup>
                    </Col>
                    <Col md={6}>
                      <FormGroup>
                        <Label for="organizationCode">Organization Code</Label>
                        <Input
                          type="text"
                          id="organizationCode"
                          value={searchCriteria.organizationCode}
                          onChange={(e) => this.handleInputChange('organizationCode', e.target.value)}
                          placeholder="Enter organization code"
                        />
                      </FormGroup>
                    </Col>
                  </Row>
                  <Row>
                    <Col md={6}>
                      <FormGroup>
                        <Label for="organizationName">Organization Name</Label>
                        <Input
                          type="text"
                          id="organizationName"
                          value={searchCriteria.organizationName}
                          onChange={(e) => this.handleInputChange('organizationName', e.target.value)}
                          placeholder="Enter organization name"
                        />
                      </FormGroup>
                    </Col>
                    <Col md={6}>
                      <FormGroup>
                        <Label for="orderStatus">Order Status</Label>
                        <Input
                          type="select"
                          id="orderStatus"
                          value={searchCriteria.orderStatus}
                          onChange={(e) => this.handleInputChange('orderStatus', e.target.value)}
                        >
                          <option value="">Select Status</option>
                          <option value="Partial">Partial</option>
                          <option value="Complete">Complete</option>
                        </Input>
                      </FormGroup>
                    </Col>
                  </Row>
                  <Row>
                    <Col md={6}>
                      <FormGroup>
                        <Label for="rrStatus">RR Status</Label>
                        <Input
                          type="select"
                          id="rrStatus"
                          value={searchCriteria.rrStatus}
                          onChange={(e) => this.handleInputChange('rrStatus', e.target.value)}
                        >
                          <option value="">Select RR Status</option>
                          <option value="Draft">Draft</option>
                          <option value="Submitted">Submitted</option>
                          <option value="Complete">Complete</option>
                        </Input>
                      </FormGroup>
                    </Col>
                    <Col md={6}>
                      {/* Empty column for layout balance */}
                    </Col>
                  </Row>
                  <Row>
                    <Col md={6}>
                      <FormGroup>
                        <Label for="priceRangeStart">Price Range Start</Label>
                        <InputGroup>
                          <InputGroupText>$</InputGroupText>
                          <Input
                            type="number"
                            id="priceRangeStart"
                            value={searchCriteria.priceRangeStart}
                            onChange={(e) => this.handleInputChange('priceRangeStart', e.target.value)}
                            placeholder="0.00"
                            min="0"
                            step="0.01"
                          />
                        </InputGroup>
                      </FormGroup>
                    </Col>
                    <Col md={6}>
                      <FormGroup>
                        <Label for="priceRangeEnd">Price Range Up To</Label>
                        <InputGroup>
                          <InputGroupText>$</InputGroupText>
                          <Input
                            type="number"
                            id="priceRangeEnd"
                            value={searchCriteria.priceRangeEnd}
                            onChange={(e) => this.handleInputChange('priceRangeEnd', e.target.value)}
                            placeholder="0.00"
                            min="0"
                            step="0.01"
                          />
                        </InputGroup>
                      </FormGroup>
                    </Col>
                  </Row>
                  <Row>
                    <Col>
                      <Button color="primary" onClick={this.handleSearch} className="me-2">
                        <i className="fas fa-search me-2"></i>Search
                      </Button>
                      <Button color="secondary" onClick={this.handleClear} className="me-2">
                        <i className="fas fa-eraser me-2"></i>Clear
                      </Button>
                      <Button 
                        color="success" 
                        tag={Link} 
                        to="/receivable-report/create"
                        className="float-end"
                      >
                        <i className="fas fa-plus me-2"></i>Create Receivable Report
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
                  <h5>Search Results ({searchResults.length} records found)</h5>
                </CardHeader>
                <CardBody>
                  <Table responsive striped hover>
                    <thead>
                      <tr>
                        <th>ID</th>
                        <th>Organization</th>
                        <th>Procurement Type</th>
                        <th>Purchase Order Number</th>
                        <th>Contract Number</th>
                        <th>Created Date</th>
                        <th>Created By</th>
                        <th>Order Status</th>
                        <th>RR Status</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {searchResults.map((result) => (
                        <tr key={result.id}>
                          <td><strong>{result.id}</strong></td>
                          <td>{result.organizationName || result.organizationCode || 'N/A'}</td>
                          <td>{result.procurementType || 'N/A'}</td>
                          <td>{result.purchaseOrderNumber || 'N/A'}</td>
                          <td>{result.contractNumber || 'N/A'}</td>
                          <td>{result.createdDate ? new Date(result.createdDate).toLocaleDateString() : 'N/A'}</td>
                          <td>{result.createdBy || 'N/A'}</td>
                          <td>{this.getOrderStatusBadge(result.orderStatus)}</td>
                          <td>{this.getRRStatusBadge(result.rrStatus || 'Draft')}</td>
                          <td>
                            <Button
                              size="sm"
                              color="info"
                              tag={Link}
                              to={`/receivable-report/edit/${result.id}`}
                              className="me-1"
                              title="Edit Receivable Report"
                            >
                              <i className="fas fa-edit"></i>
                            </Button>
                            <Button
                              size="sm"
                              color="outline-secondary"
                              tag={Link}
                              to={`/receivable-report/view/${result.id}`}
                              title="View Receivable Report"
                            >
                              <i className="fas fa-eye"></i>
                            </Button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
                </CardBody>
              </Card>
            </Col>
          </Row>
        )}
      </Container>
    );
  }
}



