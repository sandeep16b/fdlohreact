import React, { Component } from 'react';
import { 
  Container, 
  Row, 
  Col, 
  Card, 
  CardBody, 
  CardHeader, 
  FormGroup, 
  Label, 
  Input, 
  Button, 
  Table,
  Collapse,
  Badge,
  Spinner
} from 'reactstrap';
import { Link, useParams } from 'react-router-dom';
import { toast } from 'react-toastify';
import receivableReportService from '../services/receivableReportService';

// Wrapper component to handle route parameters
function ViewReceivableReportWrapper() {
  const params = useParams();
  return <ViewReceivableReportClass {...params} />;
}

export { ViewReceivableReportWrapper as ViewReceivableReport };

class ViewReceivableReportClass extends Component {
  static displayName = ViewReceivableReportClass.name;

  constructor(props) {
    super(props);
    this.state = {
      // Collapsible sections state
      orgDetailsCollapsed: false,
      procurementCollapsed: false,
      assetsCollapsed: false,
      
      // Report data loaded from API
      reportData: null,
      
      // Loading states
      isLoading: false,
      
      // Data loaded flag
      dataLoaded: false
    };
  }

  async componentDidMount() {
    try {
      if (this.props.id) {
        await this.loadReceivableReport(this.props.id);
      } else {
        this.setState({ 
          dataLoaded: true 
        });
      }
    } catch (error) {
      console.error('Error in componentDidMount:', error);
      toast.error('Failed to load report data. Please refresh the page.');
      this.setState({ 
        dataLoaded: true
      });
    }
  }

  // Load receivable report data
  loadReceivableReport = async (id) => {
    try {
      this.setState({ isLoading: true });
      const report = await receivableReportService.getReceivableReportById(id);
      
      // Validate report object
      if (!report || typeof report !== 'object') {
        throw new Error('Invalid report data received from API');
      }

      // Ensure assets array is valid
      const validatedReport = {
        ...report,
        assets: Array.isArray(report.assets) ? report.assets.map(asset => ({
          ...asset,
          assetValue: parseFloat(asset.assetValue) || 0
        })) : []
      };
      
      this.setState({
        reportData: validatedReport,
        isLoading: false,
        dataLoaded: true
      });
    } catch (error) {
      console.error('Error loading receivable report:', error);
      toast.error('Failed to load receivable report data.');
      this.setState({ 
        isLoading: false,
        dataLoaded: true
      });
    }
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
    const { 
      orgDetailsCollapsed, 
      procurementCollapsed, 
      assetsCollapsed, 
      reportData,
      isLoading,
      dataLoaded
    } = this.state;

    // Show loading spinner while data is loading
    if (!dataLoaded || isLoading) {
      return (
        <Container fluid className="d-flex justify-content-center align-items-center" style={{ minHeight: '400px' }}>
          <div className="text-center">
            <Spinner color="primary" style={{ width: '3rem', height: '3rem' }} />
            <div className="mt-2">Loading...</div>
          </div>
        </Container>
      );
    }

    // Show message if no report data
    if (!reportData) {
      return (
        <Container fluid>
          <Row>
            <Col>
              <Button color="secondary" tag={Link} to="/receivable-report/search">
                Back to Search
              </Button>
            </Col>
          </Row>
        </Container>
      );
    }

    return (
      <Container fluid>
        <Row>
          <Col>
            <div className="d-flex justify-content-between align-items-center mb-4">
              <div>
                <h2>View Receivable Report</h2>
                <p className="text-muted mb-0">
                  Request ID: <strong>{reportData.id}</strong> | 
                  RR Status: {this.getRRStatusBadge(reportData.rrStatus || 'Draft')} | 
                  Order Status: {this.getOrderStatusBadge(reportData.orderStatus)}
                </p>
              </div>
              <div>
                <Button 
                  color="primary" 
                  tag={Link} 
                  to={`/receivable-report/edit/${reportData.id}`}
                  className="me-2"
                >
                  Edit Report
                </Button>
                <Button color="secondary" tag={Link} to="/receivable-report/search">
                  Back to Search
                </Button>
              </div>
            </div>
          </Col>
        </Row>

        {/* Report Summary */}
        <Row className="mb-3">
          <Col>
            <Card>
              <CardHeader>
                <h5>Report Summary</h5>
              </CardHeader>
              <CardBody>
                <Row>
                  <Col md={3}>
                    <FormGroup>
                      <Label><strong>Requested By:</strong></Label>
                      <div>{reportData.requestedBy}</div>
                    </FormGroup>
                  </Col>
                  <Col md={3}>
                    <FormGroup>
                      <Label><strong>Purchase Order:</strong></Label>
                      <div>{reportData.purchaseOrderNumber}</div>
                    </FormGroup>
                  </Col>
                  <Col md={3}>
                    <FormGroup>
                      <Label><strong>Contract Number:</strong></Label>
                      <div>{reportData.contractNumber}</div>
                    </FormGroup>
                  </Col>
                  <Col md={3}>
                    <FormGroup>
                      <Label><strong>P-Card:</strong></Label>
                      <div>{reportData.pCard}</div>
                    </FormGroup>
                  </Col>
                </Row>
                <Row>
                  <Col md={3}>
                    <FormGroup>
                      <Label><strong>Created Date:</strong></Label>
                      <div>{new Date(reportData.rrCreatedDate).toLocaleDateString()}</div>
                    </FormGroup>
                  </Col>
                  <Col md={3}>
                    <FormGroup>
                      <Label><strong>Total Asset Value:</strong></Label>
                      <div>
                        <strong>
                          ${reportData.assets.reduce((sum, asset) => sum + (parseFloat(asset.assetValue) || 0), 0).toFixed(2)}
                        </strong>
                      </div>
                    </FormGroup>
                  </Col>
                  <Col md={3}>
                    <FormGroup>
                      <Label><strong>Total Assets:</strong></Label>
                      <div><strong>{reportData.assets.length}</strong></div>
                    </FormGroup>
                  </Col>
                </Row>
              </CardBody>
            </Card>
          </Col>
        </Row>

        {/* RR Status Information */}
        <Row className="mb-3">
          <Col>
            <Card>
              <CardHeader>
                <h5 className="mb-0">RR Status Information</h5>
              </CardHeader>
              <CardBody>
                <Row>
                  <Col md={6}>
                    <div>
                      <strong>Current Status:</strong>
                      <span className="ms-2">
                        {this.getRRStatusBadge(reportData.rrStatus || 'Draft')}
                      </span>
                    </div>
                  </Col>
                  <Col md={6}>
                    <div className="text-muted">
                      <small>
                        {(reportData.rrStatus || 'Draft') === 'Draft' && 'Report is in draft status and can be edited'}
                        {reportData.rrStatus === 'Submitted' && 'Report has been submitted and is awaiting asset tag attestation'}
                        {reportData.rrStatus === 'Complete' && 'All assets have been attested and report is complete'}
                      </small>
                    </div>
                  </Col>
                </Row>
              </CardBody>
            </Card>
          </Col>
        </Row>

        {/* Organization Details Section */}
        <Row className="mb-3">
          <Col>
            <Card>
              <CardHeader 
                onClick={() => this.setState({ orgDetailsCollapsed: !orgDetailsCollapsed })}
                style={{ cursor: 'pointer' }}
                className="d-flex justify-content-between align-items-center"
              >
                <h5 className="mb-0">1. Organization Details</h5>
                <span>{orgDetailsCollapsed ? '▼' : '▲'}</span>
              </CardHeader>
              <Collapse isOpen={!orgDetailsCollapsed}>
                <CardBody>
                  <Row>
                    <Col md={6}>
                      <FormGroup>
                        <Label><strong>Organization Code:</strong></Label>
                        <Input
                          type="text"
                          value={reportData.organizationCode}
                          readOnly
                        />
                      </FormGroup>
                    </Col>
                    <Col md={6}>
                      <FormGroup>
                        <Label><strong>Organization Name:</strong></Label>
                        <Input
                          type="text"
                          value={reportData.organizationName}
                          readOnly
                        />
                      </FormGroup>
                    </Col>
                  </Row>
                  <Row>
                    <Col md={6}>
                      <FormGroup>
                        <Label><strong>Location Code:</strong></Label>
                        <Input
                          type="text"
                          value={reportData.locationCode}
                          readOnly
                        />
                      </FormGroup>
                    </Col>
                    <Col md={6}>
                      <FormGroup>
                        <Label><strong>Order Status:</strong></Label>
                        <div className="pt-2">
                          {this.getOrderStatusBadge(reportData.orderStatus)}
                        </div>
                      </FormGroup>
                    </Col>
                  </Row>
                  <Row>
                    <Col md={6}>
                      <FormGroup>
                        <Label><strong>Address Line 1:</strong></Label>
                        <Input
                          type="text"
                          value={reportData.addressLine1}
                          readOnly
                        />
                      </FormGroup>
                    </Col>
                    <Col md={6}>
                      <FormGroup>
                        <Label><strong>City:</strong></Label>
                        <Input
                          type="text"
                          value={reportData.city}
                          readOnly
                        />
                      </FormGroup>
                    </Col>
                  </Row>
                  <Row>
                    <Col md={4}>
                      <FormGroup>
                        <Label><strong>County:</strong></Label>
                        <Input
                          type="text"
                          value={reportData.county}
                          readOnly
                        />
                      </FormGroup>
                    </Col>
                    <Col md={4}>
                      <FormGroup>
                        <Label><strong>State:</strong></Label>
                        <Input
                          type="text"
                          value={reportData.state}
                          readOnly
                        />
                      </FormGroup>
                    </Col>
                    <Col md={4}>
                      <FormGroup>
                        <Label><strong>Postal Code:</strong></Label>
                        <Input
                          type="text"
                          value={reportData.postalCode}
                          readOnly
                        />
                      </FormGroup>
                    </Col>
                  </Row>
                </CardBody>
              </Collapse>
            </Card>
          </Col>
        </Row>

        {/* Procurement Method Section */}
        <Row className="mb-3">
          <Col>
            <Card>
              <CardHeader 
                onClick={() => this.setState({ procurementCollapsed: !procurementCollapsed })}
                style={{ cursor: 'pointer' }}
                className="d-flex justify-content-between align-items-center"
              >
                <h5 className="mb-0">2. Procurement Method</h5>
                <span>{procurementCollapsed ? '▼' : '▲'}</span>
              </CardHeader>
              <Collapse isOpen={!procurementCollapsed}>
                <CardBody>
                  <Row>
                    <Col md={6}>
                      <FormGroup>
                        <Label><strong>Procurement Type:</strong></Label>
                        <Input
                          type="text"
                          value={reportData.procurementType}
                          readOnly
                        />
                      </FormGroup>
                    </Col>
                  </Row>
                  
                  {/* P-Card specific fields */}
                  {reportData.procurementType === 'P Card' && (
                    <>
                      <Row>
                        <Col md={6}>
                          <FormGroup>
                            <Label><strong>Charge Date:</strong></Label>
                            <Input
                              type="text"
                              value={reportData.chargeDate ? new Date(reportData.chargeDate).toLocaleDateString() : 'N/A'}
                              readOnly
                            />
                          </FormGroup>
                        </Col>
                        <Col md={6}>
                          <FormGroup>
                            <Label><strong>Group ID:</strong></Label>
                            <Input
                              type="text"
                              value={reportData.groupId}
                              readOnly
                            />
                          </FormGroup>
                        </Col>
                      </Row>
                      <Row>
                        <Col md={6}>
                          <FormGroup>
                            <Label><strong>P-Card Holder First Name:</strong></Label>
                            <Input
                              type="text"
                              value={reportData.pcardHolderFirstName}
                              readOnly
                            />
                          </FormGroup>
                        </Col>
                        <Col md={6}>
                          <FormGroup>
                            <Label><strong>P-Card Holder Last Name:</strong></Label>
                            <Input
                              type="text"
                              value={reportData.pcardHolderLastName}
                              readOnly
                            />
                          </FormGroup>
                        </Col>
                      </Row>
                    </>
                  )}
                  
                  {/* Purchase Order specific field */}
                  {reportData.procurementType === 'Purchase Order' && (
                    <Row>
                      <Col md={6}>
                        <FormGroup>
                          <Label><strong>Purchase Order Number:</strong></Label>
                          <Input
                            type="text"
                            value={reportData.purchaseOrderNumber}
                            readOnly
                          />
                        </FormGroup>
                      </Col>
                    </Row>
                  )}
                  
                  {/* Contract Asset specific field */}
                  {reportData.procurementType === 'Contact Asset' && (
                    <Row>
                      <Col md={6}>
                        <FormGroup>
                          <Label><strong>Contract Number:</strong></Label>
                          <Input
                            type="text"
                            value={reportData.contractNumber}
                            readOnly
                          />
                        </FormGroup>
                      </Col>
                    </Row>
                  )}
                </CardBody>
              </Collapse>
            </Card>
          </Col>
        </Row>

        {/* Assets Section */}
        <Row className="mb-3">
          <Col>
            <Card>
              <CardHeader 
                onClick={() => this.setState({ assetsCollapsed: !assetsCollapsed })}
                style={{ cursor: 'pointer' }}
                className="d-flex justify-content-between align-items-center"
              >
                <h5 className="mb-0">3. Assets ({reportData.assets.length} items)</h5>
                <span>{assetsCollapsed ? '▼' : '▲'}</span>
              </CardHeader>
              <Collapse isOpen={!assetsCollapsed}>
                <CardBody>
                  <Table responsive striped hover>
                    <thead>
                      <tr>
                        <th>Brand</th>
                        <th>Make</th>
                        <th>Model</th>
                        <th>Asset Value</th>
                        <th>Serial Number</th>
                        <th>Object Code</th>
                      </tr>
                    </thead>
                    <tbody>
                      {reportData.assets.map(asset => (
                        <tr key={asset.id}>
                          <td>{asset.brand}</td>
                          <td>{asset.make}</td>
                          <td>{asset.model}</td>
                          <td>
                            <strong>${(parseFloat(asset.assetValue) || 0).toFixed(2)}</strong>
                          </td>
                          <td>
                            <code>{asset.serialNumber}</code>
                          </td>
                          <td>{asset.objectCode || 'N/A'}</td>
                        </tr>
                      ))}
                    </tbody>
                    <tfoot>
                      <tr className="table-info">
                        <th colSpan="3">Total Value:</th>
                        <th>
                          <strong>
                            ${reportData.assets.reduce((sum, asset) => sum + (parseFloat(asset.assetValue) || 0), 0).toFixed(2)}
                          </strong>
                        </th>
                        <th colSpan="3"></th>
                      </tr>
                    </tfoot>
                  </Table>
                </CardBody>
              </Collapse>
            </Card>
          </Col>
        </Row>
      </Container>
    );
  }
}


