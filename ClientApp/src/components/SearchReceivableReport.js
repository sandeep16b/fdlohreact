import React, { Component } from 'react';
import { AgGridReact } from 'ag-grid-react';
import { ModuleRegistry, AllCommunityModule } from 'ag-grid-community';
import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-quartz.css';
import {
  Container, Row, Col, Card, CardBody, CardHeader,
  Form, FormGroup, Label, Input, Button,
  InputGroup, InputGroupText, Modal, ModalHeader, ModalBody, ModalFooter
} from 'reactstrap';
import { Link } from 'react-router-dom';
import '@fortawesome/fontawesome-free/css/all.min.css';
import { toast } from 'react-toastify';
import receivableReportService from '../services/receivableReportService';

ModuleRegistry.registerModules([AllCommunityModule]);

const RRStatusCellRenderer = ({ value }) => {
  const status = value || 'Draft';
  const colorMap = { Draft: 'secondary', Submitted: 'warning', Complete: 'success' };
  const bg = colorMap[status] || 'secondary';
  return <span className={`badge bg-${bg}`}>{status}</span>;
};

const OrderStatusCellRenderer = ({ value }) => {
  if (!value) return <span>—</span>;
  const bg = value === 'Complete' ? 'success' : 'warning';
  return <span className={`badge bg-${bg}`}>{value}</span>;
};

const ActionsCellRenderer = ({ data }) => {
  if (!data?.id) return null;
  return (
    <div style={{ display: 'flex', alignItems: 'center', height: '100%' }}>
      <Link
        to={`/receivable-report/edit/${data.id}`}
        className="grid-action-btn"
        title="Edit"
      >
        <i className="fa fa-pencil-alt"></i>
      </Link>
      <Link
        to={`/receivable-report/view/${data.id}`}
        className="grid-action-btn view-btn"
        title="View"
      >
        <i className="fa fa-eye"></i>
      </Link>
    </div>
  );
};

export class SearchReceivableReport extends Component {
  static displayName = SearchReceivableReport.name;

  constructor(props) {
    super(props);

    this.gridRef = React.createRef();

    this.columnDefs = [
      {
        headerName: 'Actions',
        colId: 'Actions',
        field: 'id',
        cellRenderer: ActionsCellRenderer,
        width: 110,
        sortable: false,
        filter: false,
        pinned: 'left',
      },
      { field: 'id', colId: 'Id', headerName: 'ID', width: 80, filter: 'agNumberColumnFilter' },
      {
        field: 'organizationName',
        colId: 'OrganizationName',
        headerName: 'Organization',
        filter: 'agTextColumnFilter',
        valueFormatter: (p) => p.value || p.data?.organizationCode || 'N/A',
      },
      { field: 'procurementType', colId: 'ProcurementType', headerName: 'Procurement Type', filter: 'agTextColumnFilter' },
      { field: 'purchaseOrderNumber', colId: 'PurchaseOrderNumber', headerName: 'PO Number', filter: 'agTextColumnFilter' },
      { field: 'contractNumber', colId: 'ContractNumber', headerName: 'Contract Number', filter: 'agTextColumnFilter' },
      {
        field: 'createdDate',
        colId: 'CreatedDate',
        headerName: 'Created Date',
        filter: 'agDateColumnFilter',
        valueFormatter: (p) => p.value ? new Date(p.value).toLocaleDateString() : 'N/A',
      },
      { field: 'createdBy', colId: 'CreatedBy', headerName: 'Created By', filter: 'agTextColumnFilter' },
      {
        field: 'orderStatus',
        colId: 'OrderStatus',
        headerName: 'Order Status',
        cellRenderer: OrderStatusCellRenderer,
        filter: 'agTextColumnFilter',
      },
      {
        field: 'rrStatus',
        colId: 'RRStatus',
        headerName: 'RR Status',
        cellRenderer: RRStatusCellRenderer,
        filter: 'agTextColumnFilter',
      },
    ];

    this.defaultColDef = {
      sortable: true,
      filter: true,
      resizable: true,
      minWidth: 120,
      filterParams: { maxNumConditions: 1 },
    };

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
        priceRangeEnd: '',
      },
      rowData: [],
      isLoading: false,
      isLoadingLookupData: true,
      organizations: [],
      locations: {},
      showDeleteModal: false,
      itemToDelete: null,
    };
  }

  async componentDidMount() {
    try {
      await this.loadLookupData();
      await this.performSearch();
    } catch (error) {
      console.error('Error in componentDidMount:', error);
      toast.error('Failed to load data. Please refresh the page.');
      this.setState({ isLoadingLookupData: false });
    }
  }

  loadLookupData = async () => {
    try {
      this.setState({ isLoadingLookupData: true });
      const lookupData = await receivableReportService.getLookupData();
      const validLookupData = lookupData && typeof lookupData === 'object' ? lookupData : {};
      const organizations = Array.isArray(validLookupData.organizations) ? validLookupData.organizations : [];
      const locations = {};
      if (validLookupData.locations && Array.isArray(validLookupData.locations)) {
        validLookupData.locations.forEach(loc => {
          if (loc && loc.code) {
            locations[loc.code] = {
              addressLine1: loc.addressLine1 || '',
              city: loc.city || '',
              county: loc.county || '',
              state: loc.state || '',
              postalCode: loc.postalCode || '',
            };
          }
        });
      }
      this.setState({ organizations, locations, isLoadingLookupData: false });
    } catch (error) {
      console.error('Error loading lookup data:', error);
      toast.error('Failed to load lookup data.');
      this.setState({ isLoadingLookupData: false });
    }
  };

  performSearch = async () => {
    try {
      this.setState({ isLoading: true });
      const searchCriteria = receivableReportService.buildSearchCriteria({
        organizationCode: this.state.searchCriteria.organizationCode || null,
        locationCode: this.state.searchCriteria.locationCode || null,
        orderStatus: this.state.searchCriteria.orderStatus || null,
        rrStatus: this.state.searchCriteria.rrStatus || null,
        createdDateFrom: this.state.searchCriteria.createdDateFrom || null,
        createdDateTo: this.state.searchCriteria.createdDateTo || null,
        createdBy: this.state.searchCriteria.createdBy || null,
        pageNumber: 1,
        pageSize: 1000,
      });
      const result = await receivableReportService.searchReceivableReports(searchCriteria);
      const rowData = Array.isArray(result?.items) ? result.items : [];
      this.setState({ rowData, isLoading: false });
    } catch (error) {
      console.error('Error performing search:', error);
      toast.error('Failed to search receivable reports.');
      this.setState({ isLoading: false });
    }
  };

  deleteReceivableReport = async (id) => {
    try {
      await receivableReportService.deleteReceivableReport(id);
      toast.success('Receivable report deleted successfully.');
      this.setState({ showDeleteModal: false, itemToDelete: null });
      await this.performSearch();
    } catch (error) {
      console.error('Error deleting:', error);
      toast.error('Failed to delete receivable report.');
      this.setState({ showDeleteModal: false, itemToDelete: null });
    }
  };

  handleInputChange = (field, value) => {
    this.setState({
      searchCriteria: { ...this.state.searchCriteria, [field]: value },
    });
  };

  handleSearch = async () => {
    await this.performSearch();
  };

  handleClear = () => {
    this.setState({
      searchCriteria: {
        requestedBy: '', organizationCode: '', locationCode: '',
        orderStatus: '', rrStatus: '', organizationName: '',
        createdDateFrom: '', createdDateTo: '', createdBy: '',
        priceRangeStart: '', priceRangeEnd: '',
      },
    });
  };

  handleClearFilters = () => {
    if (this.gridRef.current?.api) {
      this.gridRef.current.api.setFilterModel(null);
    }
  };

  handleDeleteClick = (item) => {
    this.setState({ showDeleteModal: true, itemToDelete: item });
  };

  handleDeleteConfirm = async () => {
    if (this.state.itemToDelete) {
      await this.deleteReceivableReport(this.state.itemToDelete.id);
    }
  };

  handleDeleteCancel = () => {
    this.setState({ showDeleteModal: false, itemToDelete: null });
  };

  render() {
    const { searchCriteria, rowData, isLoading, showDeleteModal, itemToDelete } = this.state;

    return (
      <Container fluid>
        <Row>
          <Col>
            <h2>Receivable Reports</h2>
          </Col>
        </Row>

        {/* Search Form */}
        <Row className="mb-4">
          <Col>
            <Card className="modern-card">
              <CardHeader><h5>Search Criteria</h5></CardHeader>
              <CardBody>
                <Form>
                  <Row>
                    <Col md={6}>
                      <FormGroup>
                        <Label for="organizationCode">Organization Code</Label>
                        <Input type="text" id="organizationCode"
                          value={searchCriteria.organizationCode}
                          onChange={(e) => this.handleInputChange('organizationCode', e.target.value)}
                          placeholder="Enter organization code" />
                      </FormGroup>
                    </Col>
                    <Col md={6}>
                      <FormGroup>
                        <Label for="organizationName">Organization Name</Label>
                        <Input type="text" id="organizationName"
                          value={searchCriteria.organizationName}
                          onChange={(e) => this.handleInputChange('organizationName', e.target.value)}
                          placeholder="Enter organization name" />
                      </FormGroup>
                    </Col>
                  </Row>
                  <Row>
                    <Col md={6}>
                      <FormGroup>
                        <Label for="orderStatus">Order Status</Label>
                        <Input type="select" id="orderStatus"
                          value={searchCriteria.orderStatus}
                          onChange={(e) => this.handleInputChange('orderStatus', e.target.value)}>
                          <option value="">Select Status</option>
                          <option value="Partial">Partial</option>
                          <option value="Complete">Complete</option>
                        </Input>
                      </FormGroup>
                    </Col>
                    <Col md={6}>
                      <FormGroup>
                        <Label for="rrStatus">RR Status</Label>
                        <Input type="select" id="rrStatus"
                          value={searchCriteria.rrStatus}
                          onChange={(e) => this.handleInputChange('rrStatus', e.target.value)}>
                          <option value="">Select RR Status</option>
                          <option value="Draft">Draft</option>
                          <option value="Submitted">Submitted</option>
                          <option value="Complete">Complete</option>
                        </Input>
                      </FormGroup>
                    </Col>
                  </Row>
                  <Row>
                    <Col md={6}>
                      <FormGroup>
                        <Label for="priceRangeStart">Price Range Start</Label>
                        <InputGroup>
                          <InputGroupText>$</InputGroupText>
                          <Input type="number" id="priceRangeStart"
                            value={searchCriteria.priceRangeStart}
                            onChange={(e) => this.handleInputChange('priceRangeStart', e.target.value)}
                            placeholder="0.00" min="0" step="0.01" />
                        </InputGroup>
                      </FormGroup>
                    </Col>
                    <Col md={6}>
                      <FormGroup>
                        <Label for="priceRangeEnd">Price Range Up To</Label>
                        <InputGroup>
                          <InputGroupText>$</InputGroupText>
                          <Input type="number" id="priceRangeEnd"
                            value={searchCriteria.priceRangeEnd}
                            onChange={(e) => this.handleInputChange('priceRangeEnd', e.target.value)}
                            placeholder="0.00" min="0" step="0.01" />
                        </InputGroup>
                      </FormGroup>
                    </Col>
                  </Row>
                  <Row>
                    <Col>
                      <Button color="primary" onClick={this.handleSearch} className="me-2" disabled={isLoading}>
                        <i className="fas fa-search me-2"></i>Search
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

        {/* AG-Grid Results */}
        <Row>
          <Col>
            <Card className="modern-card results-card">
              <CardHeader>
                <div className="d-flex justify-content-between align-items-center">
                  <h5 className="mb-0">Search Results</h5>
                  <div>
                    <button
                      className="btn btn-sm btn-outline-secondary me-2"
                      title="Clear Grid Filters"
                      onClick={this.handleClearFilters}
                    >
                      <i className="fas fa-filter me-1"></i>Clear Filters
                    </button>
                    <Link to="/receivable-report/create" className="btn btn-success btn-sm">
                      <i className="fas fa-plus me-1"></i>Create Receivable Report
                    </Link>
                  </div>
                </div>
              </CardHeader>
              <CardBody className="p-0">
                {isLoading && (
                  <div className="text-center py-4">
                    <div className="spinner-border text-primary" role="status">
                      <span className="visually-hidden">Loading...</span>
                    </div>
                  </div>
                )}
                <div className="ag-theme-quartz agGrid" style={{ width: '100%', height: '600px' }}>
                  <AgGridReact
                    ref={this.gridRef}
                    columnDefs={this.columnDefs}
                    defaultColDef={this.defaultColDef}
                    rowData={rowData}
                    pagination={true}
                    paginationPageSize={25}
                    paginationPageSizeSelector={[25, 50, 100]}
                    suppressCellFocus={true}
                    animateRows={true}
                    rowHeight={56}
                    headerHeight={44}
                  />
                </div>
              </CardBody>
            </Card>
          </Col>
        </Row>

        {/* Delete Confirmation Modal */}
        <Modal isOpen={showDeleteModal} toggle={this.handleDeleteCancel}>
          <ModalHeader toggle={this.handleDeleteCancel}>Confirm Delete</ModalHeader>
          <ModalBody>
            Are you sure you want to delete receivable report{' '}
            <strong>#{itemToDelete?.id}</strong>? This action cannot be undone.
          </ModalBody>
          <ModalFooter>
            <Button color="danger" onClick={this.handleDeleteConfirm}>Delete</Button>
            <Button color="secondary" onClick={this.handleDeleteCancel}>Cancel</Button>
          </ModalFooter>
        </Modal>
      </Container>
    );
  }
}
