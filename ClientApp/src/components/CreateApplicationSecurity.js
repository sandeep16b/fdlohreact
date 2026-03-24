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
  Alert
} from 'reactstrap';
import { Link, useParams, useNavigate } from 'react-router-dom';
import '@fortawesome/fontawesome-free/css/all.min.css';
import { toast } from 'react-toastify';
import applicationSecurityService from '../services/applicationSecurityService';

// Wrapper component to handle route parameters
function CreateApplicationSecurityWrapper() {
  const params = useParams();
  const navigate = useNavigate();
  return <CreateApplicationSecurityClass {...params} navigate={navigate} />;
}

export { CreateApplicationSecurityWrapper as CreateApplicationSecurity };

class CreateApplicationSecurityClass extends Component {
  static displayName = CreateApplicationSecurityClass.name;

  constructor(props) {
    super(props);
    
    // Check if we're in edit mode
    const isEditMode = this.props.id !== undefined;
    
    this.state = {
      isEditMode: isEditMode,
      id: this.props.id ? parseInt(this.props.id) : null,
      
      // Form data
      username: '',
      applicationRoleId: null,
      isActive: true,
      createdBy: '',
      updatedBy: '',
      
      // Lookup data
      applicationRoles: [],
      
      // Loading states
      isLoading: false,
      isSaving: false,
      isLoadingRoles: true,
      
      // Validation errors
      validationErrors: {},
      
      // Success message
      showSuccess: false
    };
  }

  async componentDidMount() {
    try {
      await this.loadApplicationRoles();
      
      // If edit mode, load the existing record
      if (this.state.isEditMode && this.state.id) {
        await this.loadApplicationSecurity();
      }
    } catch (error) {
      console.error('Error in componentDidMount:', error);
      toast.error('Failed to load data. Please try again.');
    }
  }

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
      this.setState({ isLoadingRoles: false });
    }
  };

  loadApplicationSecurity = async () => {
    try {
      this.setState({ isLoading: true });
      const record = await applicationSecurityService.getApplicationSecurityById(this.state.id);
      
      if (record) {
        this.setState({
          username: record.username || '',
          applicationRoleId: record.applicationRoleId || null,
          isActive: record.isActive !== undefined ? record.isActive : true,
          createdBy: record.createdBy || '',
          updatedBy: record.updatedBy || ''
        });
      }
    } catch (error) {
      console.error('Error loading application security record:', error);
      toast.error('Failed to load application security record.');
      // Navigate back to search if record not found
      if (error.message.includes('not found')) {
        this.props.navigate('/application-security/search');
      }
    } finally {
      this.setState({ isLoading: false });
    }
  };

  handleInputChange = (field, value) => {
    this.setState({
      [field]: value,
      validationErrors: {
        ...this.state.validationErrors,
        [field]: null // Clear validation error for this field
      }
    });
  };

  validateForm = () => {
    const errors = {};
    
    // Validate username (email)
    if (!this.state.username || this.state.username.trim() === '') {
      errors.username = 'Username (email) is required';
    } else {
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (!emailRegex.test(this.state.username.trim())) {
        errors.username = 'Username must be a valid email address';
      }
    }
    
    // Validate application role
    if (!this.state.applicationRoleId) {
      errors.applicationRoleId = 'Application Role is required';
    }
    
    this.setState({ validationErrors: errors });
    return Object.keys(errors).length === 0;
  };

  handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!this.validateForm()) {
      toast.error('Please fix validation errors before submitting.');
      return;
    }
    
    try {
      this.setState({ isSaving: true, showSuccess: false });
      
      const data = {
        username: this.state.username.trim(),
        applicationRoleId: this.state.applicationRoleId,
        isActive: this.state.isActive,
        createdBy: this.state.createdBy || 'System',
        updatedBy: this.state.updatedBy || 'System'
      };
      
      let result;
      if (this.state.isEditMode) {
        result = await applicationSecurityService.updateApplicationSecurity(this.state.id, data);
        toast.success('Application security record updated successfully!');
      } else {
        result = await applicationSecurityService.createApplicationSecurity(data);
        toast.success('Application security record created successfully!');
      }
      
      this.setState({ showSuccess: true });
      
      // Navigate back to search after a short delay
      setTimeout(() => {
        this.props.navigate('/application-security/search');
      }, 1500);
      
    } catch (error) {
      console.error('Error saving application security record:', error);
      const errorMessage = error.message || 'Failed to save application security record. Please try again.';
      toast.error(errorMessage);
    } finally {
      this.setState({ isSaving: false });
    }
  };

  handleCancel = () => {
    this.props.navigate('/application-security/search');
  };

  render() {
    const { 
      isEditMode,
      username,
      applicationRoleId,
      isActive,
      createdBy,
      updatedBy,
      applicationRoles,
      isLoading,
      isSaving,
      isLoadingRoles,
      validationErrors,
      showSuccess
    } = this.state;

    if (isLoading) {
      return (
        <Container fluid>
          <Row>
            <Col>
              <div className="text-center py-5">
                <i className="fas fa-spinner fa-spin fa-2x"></i>
                <p className="mt-2">Loading...</p>
              </div>
            </Col>
          </Row>
        </Container>
      );
    }

    return (
      <Container fluid>
        <Row>
          <Col>
            <div className="d-flex justify-content-between align-items-center mb-3">
              <h2>{isEditMode ? 'Edit' : 'Create'} Application Security</h2>
              <Link to="/application-security/search" className="btn btn-secondary">
                <i className="fas fa-arrow-left me-2"></i>Back to Search
              </Link>
            </div>
          </Col>
        </Row>

        {showSuccess && (
          <Row>
            <Col>
              <Alert color="success">
                <i className="fas fa-check-circle me-2"></i>
                Application security record {isEditMode ? 'updated' : 'created'} successfully! Redirecting...
              </Alert>
            </Col>
          </Row>
        )}

        <Row>
          <Col md={8} lg={6}>
            <Card>
              <CardHeader>
                <h5>Application Security Details</h5>
              </CardHeader>
              <CardBody>
                <Form onSubmit={this.handleSubmit}>
                  <FormGroup>
                    <Label for="username">
                      Username (Email) <span className="text-danger">*</span>
                    </Label>
                    <Input
                      type="email"
                      id="username"
                      value={username}
                      onChange={(e) => this.handleInputChange('username', e.target.value)}
                      placeholder="Enter email address"
                      invalid={!!validationErrors.username}
                      disabled={isSaving}
                    />
                    {validationErrors.username && (
                      <div className="text-danger small mt-1">{validationErrors.username}</div>
                    )}
                  </FormGroup>

                  <FormGroup>
                    <Label for="applicationRoleId">
                      Application Role <span className="text-danger">*</span>
                    </Label>
                    <Input
                      type="select"
                      id="applicationRoleId"
                      value={applicationRoleId || ''}
                      onChange={(e) => this.handleInputChange('applicationRoleId', e.target.value ? parseInt(e.target.value) : null)}
                      invalid={!!validationErrors.applicationRoleId}
                      disabled={isSaving || isLoadingRoles}
                    >
                      <option value="">Select Application Role</option>
                      {applicationRoles.map(role => (
                        <option key={role.id} value={role.id}>
                          {role.name}
                          {role.description ? ` - ${role.description}` : ''}
                        </option>
                      ))}
                    </Input>
                    {validationErrors.applicationRoleId && (
                      <div className="text-danger small mt-1">{validationErrors.applicationRoleId}</div>
                    )}
                    {isLoadingRoles && (
                      <div className="text-muted small mt-1">
                        <i className="fas fa-spinner fa-spin me-1"></i>Loading roles...
                      </div>
                    )}
                  </FormGroup>

                  <FormGroup check>
                    <Label check>
                      <Input
                        type="checkbox"
                        checked={isActive}
                        onChange={(e) => this.handleInputChange('isActive', e.target.checked)}
                        disabled={isSaving}
                      />
                      {' '}Active
                    </Label>
                  </FormGroup>

                  {isEditMode && (
                    <>
                      <FormGroup>
                        <Label for="createdBy">Created By</Label>
                        <Input
                          type="text"
                          id="createdBy"
                          value={createdBy}
                          disabled
                          readOnly
                        />
                      </FormGroup>

                      <FormGroup>
                        <Label for="updatedBy">Updated By</Label>
                        <Input
                          type="text"
                          id="updatedBy"
                          value={updatedBy}
                          onChange={(e) => this.handleInputChange('updatedBy', e.target.value)}
                          placeholder="Enter your name"
                          disabled={isSaving}
                        />
                      </FormGroup>
                    </>
                  )}

                  {!isEditMode && (
                    <FormGroup>
                      <Label for="createdBy">Created By</Label>
                      <Input
                        type="text"
                        id="createdBy"
                        value={createdBy}
                        onChange={(e) => this.handleInputChange('createdBy', e.target.value)}
                        placeholder="Enter your name"
                        disabled={isSaving}
                      />
                    </FormGroup>
                  )}

                  <div className="d-flex justify-content-end gap-2 mt-4">
                    <Button
                      type="button"
                      color="secondary"
                      onClick={this.handleCancel}
                      disabled={isSaving}
                    >
                      <i className="fas fa-times me-2"></i>Cancel
                    </Button>
                    <Button
                      type="submit"
                      color="primary"
                      disabled={isSaving || isLoadingRoles}
                    >
                      {isSaving ? (
                        <>
                          <i className="fas fa-spinner fa-spin me-2"></i>
                          {isEditMode ? 'Updating...' : 'Creating...'}
                        </>
                      ) : (
                        <>
                          <i className="fas fa-save me-2"></i>
                          {isEditMode ? 'Update' : 'Create'}
                        </>
                      )}
                    </Button>
                  </div>
                </Form>
              </CardBody>
            </Card>
          </Col>
        </Row>
      </Container>
    );
  }
}

