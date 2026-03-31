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
  Collapse,
  Modal,
  ModalHeader,
  ModalBody,
  ModalFooter,
  Spinner
} from 'reactstrap';
import { Link, useParams } from 'react-router-dom';
import '@fortawesome/fontawesome-free/css/all.min.css';
import { toast } from 'react-toastify';
import receivableReportService from '../services/receivableReportService';
import * as assetService from '../services/assetService';

// Wrapper component to handle route parameters
function CreateReceivableReportWrapper() {
  const params = useParams();
  return <CreateReceivableReportClass {...params} />;
}

export { CreateReceivableReportWrapper as CreateReceivableReport };

class CreateReceivableReportClass extends Component {
  static displayName = CreateReceivableReportClass.name;

  constructor(props) {
    super(props);
    
    // Check if we're in edit mode
    const isEditMode = this.props.id !== undefined;
    
    this.state = {
      // Mode
      isEditMode: isEditMode,
      receivableReportId: this.props.id ? parseInt(this.props.id) : null,
      isReportSaved: isEditMode, // If editing, report is already saved
      
      // Collapsible sections state
      orgDetailsCollapsed: false,
      procurementCollapsed: false,
      assetsCollapsed: false,
      
      // Form data
      organizationId: null,
      organizationCode: '',
      organizationName: '',
      fundId: null,
      fundCode: '',
      fundName: '',
      oa1Id: null,
      oa1Code: '',
      oa1Name: '',
      locationId: null,
      locationCode: '',
      addressLine1: '',
      city: '',
      county: '',
      state: '',
      postalCode: '',
      orderStatus: '',
      rrStatus: 'Draft',
      
      // Procurement data
      procurementTypeId: null,
      procurementType: '',
      chargeDate: '',
      pcardHolderFirstName: '',
      pcardHolderLastName: '',
      groupId: '',
      purchaseOrderNumber: '',
      contractNumber: '',
      
      // Assets data (loaded from API after report is saved)
      assets: [],
      
      // Modal state
      assetModalOpen: false,
      editingAsset: null,
      
      // Organization search modal state
      orgSearchModalOpen: false,
      filteredOrganizations: [],
      orgSearchFilters: {
        organization: '',
        organizationDesc: '',
        organizationShortDesc: '',
        effectiveStatus: ''
      },
      
      // Typable dropdown state
      orgDropdownOpen: false,
      orgDropdownFilter: '',
      filteredOrgDropdown: [],
      
      // Fund search modal state
      fundSearchModalOpen: false,
      filteredFunds: [],
      fundSearchFilters: {
        fund: '',
        fundDesc: '',
        fundShortDesc: '',
        effectiveStatus: ''
      },
      
      // Fund typable dropdown state
      fundDropdownOpen: false,
      fundDropdownFilter: '',
      filteredFundDropdown: [],
      
      // OA1 search modal state
      oa1SearchModalOpen: false,
      filteredOa1s: [],
      oa1SearchFilters: {
        oa1: '',
        oa1Desc: '',
        oa1ShortDesc: '',
        effectiveStatus: ''
      },
      
      // OA1 typable dropdown state
      oa1DropdownOpen: false,
      oa1DropdownFilter: '',
      filteredOa1Dropdown: [],
      currentAsset: {
        brand: '',
        make: '',
        model: '',
        assetValue: '',
        assetTag: '',
        serialNumber: '',
        objectCodeId: null,
        assetGroupId: null,
        assetSubGroupId: null,
        assignedTo: '',
        isOwnedByCounty: false,
        countyId: null,
        floor: '',
        room: '',
        uniqueTagNumber: '',
        assetStatus: 'Open'
      },
      
      // FDW lookup data
      organizations: [],
      funds: [],
      oa1s: [],
      
      // Other lookup data
      locations: {},
      objectCodes: [],
      countyCodes: [],
      assetGroups: [],
      assetSubGroups: [],
      
      // Validation
      errors: {},
      procurementValidationErrors: {},
      
      // Loading states
      isLoading: false,
      isSaving: false,
      isLoadingLookupData: true,
      
      // API data loaded flag
      dataLoaded: false
    };
  }

  async componentDidMount() {
    try {
      await this.loadLookupData();
      
      // If in edit mode, load the receivable report data and assets
      if (this.state.isEditMode && this.props.id) {
        await this.loadReceivableReport(this.props.id);
        // Load assets separately using the new API
        await this.refreshAssetsGrid();
      }
      
      this.setState({ dataLoaded: true });
    } catch (error) {
      console.error('Error in componentDidMount:', error);
      toast.error('Failed to load data. Please refresh the page.');
      this.setState({ 
        isLoadingLookupData: false,
        dataLoaded: true
      });
    }
  }

  // Load lookup data from API
  loadLookupData = async () => {
    try {
      this.setState({ isLoadingLookupData: true });
      
      // Load FDW data in parallel
      const [organizations, funds, oa1s, lookupData] = await Promise.all([
        receivableReportService.getOrganizations(),
        receivableReportService.getFunds(),
        receivableReportService.getOA1s(),
        receivableReportService.getLookupData()
      ]);
      console.log('OA1: ', oa1s);
      // Validate API responses
      const validOrganizations = Array.isArray(organizations) ? organizations : [];
      const validFunds = Array.isArray(funds) ? funds : [];
      const validOA1s = Array.isArray(oa1s) ? oa1s : [];
      const validLookupData = lookupData && typeof lookupData === 'object' ? lookupData : {};
      
      // Transform locations to use ID as key (not code)
      const locations = {};
      if (validLookupData.locations && Array.isArray(validLookupData.locations)) {
        console.log('Raw location data from API:', validLookupData.locations);
        validLookupData.locations.forEach(loc => {
          if (loc && loc.id) {
            locations[loc.id] = {
              id: loc.id,
              code: loc.code,
              addressLine1: loc.addressLine1 || '',
              city: loc.city || '',
              county: loc.county || '',
              state: loc.state || '',
              postalCode: loc.postalCode || ''
            };
          }
        });
        console.log('Transformed locations (ID as key):', locations);
      } else {
        console.warn('Locations data is missing or not an array:', validLookupData.locations);
      }

      // Log procurement types for debugging
      const procurementTypesArray = Array.isArray(validLookupData.procurementTypes) ? validLookupData.procurementTypes : [];
      console.log('Procurement types from API:', procurementTypesArray);
      console.log('Procurement types count:', procurementTypesArray.length);

      this.setState({
        organizations: validOrganizations,
        funds: validFunds,
        oa1s: validOA1s,
        locations,
        objectCodes: Array.isArray(validLookupData.objectCodes) ? validLookupData.objectCodes : [],
        countyCodes: Array.isArray(validLookupData.counties) ? validLookupData.counties : [],
        procurementTypes: procurementTypesArray,
        assetGroups: Array.isArray(validLookupData.assetGroups) ? validLookupData.assetGroups : [],
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

  // Load receivable report for editing
  loadReceivableReport = async (id) => {
    try {
      this.setState({ isLoading: true });
        const report = await receivableReportService.getReceivableReportById(id);
        // Validate report object
      if (!report || typeof report !== 'object') {
        throw new Error('Invalid report data received from API');
      }

      // Safely parse numeric IDs
      const parseId = (value) => {
        if (value === null || value === undefined || value === '') return null;
        const parsed = parseInt(value, 10);
        return isNaN(parsed) ? null : parsed;
      };

      // Safely parse assets array
      const assets = Array.isArray(report.assets) ? report.assets.map(asset => ({
        ...asset,
        assetValue: parseFloat(asset.assetValue) || 0,
        uniqueTagNumber: asset.uniqueTagNumber || '',
        assetStatus: asset.assetStatus || 'Open'
      })) : [];
      
      this.setState({
        organizationId: parseId(report.organizationId),
        organizationCode: report.organizationCode || '',
        organizationName: report.organizationName || '',
        fundId: parseId(report.fundId),
        fundCode: report.fundCode || '',
        fundName: report.fundName || '',
        oa1Id: parseId(report.oA1Id) || parseId(report.oa1Id) || parseId(report.OA1Id),  // Try all variations
        oa1Code: report.oA1Code || report.oa1Code || report.OA1Code || '',
        oa1Name: report.oA1Name || report.oa1Name || report.OA1Name || '',
        locationId: report.locationId || null,
        locationCode: report.locationCode || '',
        addressLine1: report.addressLine1 || '',
        city: report.city || '',
        county: report.county || '',
        state: report.state || '',
        postalCode: report.postalCode || '',
        orderStatus: report.orderStatus || '',
        rrStatus: report.rrStatus || 'Draft',
        // Load procurement data if available
        procurementTypeId: parseId(report.procurementMethod?.procurementTypeId),
        procurementType: report.procurementMethod?.procurementTypeCode || '',
        chargeDate: report.procurementMethod?.chargeDate ? this.formatDateForInput(report.procurementMethod.chargeDate) : '',
        pcardHolderFirstName: report.procurementMethod?.pcardHolderFirstName || '',
        pcardHolderLastName: report.procurementMethod?.pcardHolderLastName || '',
        groupId: report.procurementMethod?.groupId || '',
        purchaseOrderNumber: report.procurementMethod?.purchaseOrderNumber || '',
        contractNumber: report.procurementMethod?.contractNumber || '',
        assets: assets,
        isLoading: false
      });
    } catch (error) {
      console.error('Error loading receivable report:', error);
      toast.error('Failed to load receivable report data.');
      this.setState({ 
        isLoading: false
      });
    }
  };

  // Helper method to format date for HTML date input (YYYY-MM-DD format)
  formatDateForInput = (dateString) => {
    try {
      if (!dateString) return '';
      const date = new Date(dateString);
      if (isNaN(date.getTime())) return '';
      return date.toISOString().split('T')[0]; // Returns YYYY-MM-DD format
    } catch (error) {
      console.warn('Error formatting date for input:', error);
      return '';
    }
  };

  // Organization autocomplete
  handleOrganizationCodeChange = (value) => {
    const org = this.state.organizations.find(o => o.code === value);
    this.setState({
      organizationCode: value,
      organizationName: org ? org.name : ''
    });
  };

  // Location change handler (now handles ID values)
  handleLocationChange = (locationId) => {
    // Handle empty selection
    if (!locationId || locationId === '') {
      this.setState({
        locationId: null,
        locationCode: '',
        addressLine1: '',
        city: '',
        county: '',
        state: '',
        postalCode: ''
      });
      return;
    }
    
    // Find location by ID directly from the locations object (now using ID as key)
    const location = this.state.locations[locationId];
    
    if (location) {
      const numericLocationId = parseInt(locationId);
      this.setState({
        locationId: numericLocationId, // Store as number
        locationCode: location.code,
        addressLine1: location.addressLine1,
        city: location.city,
        county: location.county,
        state: location.state,
        postalCode: location.postalCode
      });
    } else {
      this.setState({
        locationId: null,
        locationCode: '',
        addressLine1: '',
        city: '',
        county: '',
        state: '',
        postalCode: ''
      });
    }
  };

  // FDW dropdown change handlers
  handleOrganizationChange = (organizationId) => {
    const selectedOrg = this.state.organizations.find(org => org.id === parseInt(organizationId));
    this.setState({
      organizationId,
      organizationCode: selectedOrg?.code || '',
      organizationName: selectedOrg?.name || ''
    });
  };

  handleFundChange = (fundId) => {
    const selectedFund = this.state.funds.find(fund => fund.id === parseInt(fundId));
    this.setState({
      fundId,
      fundCode: selectedFund?.code || '',
      fundName: selectedFund?.name || ''
    });
  };

  handleOA1Change = (oa1Id) => {
    const selectedOA1 = this.state.oa1s.find(oa1 => oa1.id === parseInt(oa1Id));
    this.setState({
      oa1Id,
      oa1Code: selectedOA1?.code || '',
      oa1Name: selectedOA1?.name || ''
    });
  };

  // Asset modal handlers
  openAssetModal = (asset = null) => {
    if (asset) {
      this.setState({
        editingAsset: asset,
        currentAsset: { ...asset },
        assetModalOpen: true
      });
    } else {
      this.setState({
        editingAsset: null,
        currentAsset: {
          brand: '',
          make: '',
          model: '',
          assetValue: '',
          assetTag: '',
          serialNumber: '',
          objectCodeId: null,
          assignedTo: '',
          isOwnedByCounty: false,
          countyId: null,
          floor: '',
          room: '',
          uniqueTagNumber: '',
          assetStatus: 'Open'
        },
        assetModalOpen: true
      });
    }
  };

  closeAssetModal = async () => {
    // Refresh parent screen assets grid before closing
    await this.refreshAssetsGrid();
    
    this.setState({
      assetModalOpen: false,
      editingAsset: null,
      currentAsset: {
        brand: '',
        make: '',
        model: '',
        assetValue: '',
        assetTag: '',
        serialNumber: '',
        objectCodeId: null,
        assignedTo: '',
        isOwnedByCounty: false,
        countyId: null,
        floor: '',
        room: ''
      }
    });
  };

  // Organization search modal handlers
  openOrgSearchModal = () => {
    // Initialize filtered organizations with all organizations
    this.setState({
      orgSearchModalOpen: true,
      filteredOrganizations: this.state.organizations,
      orgSearchFilters: {
        organization: '',
        organizationDesc: '',
        organizationShortDesc: '',
        effectiveStatus: ''
      }
    });
  };

  closeOrgSearchModal = () => {
    this.setState({
      orgSearchModalOpen: false,
      filteredOrganizations: [],
      orgSearchFilters: {
        organization: '',
        organizationDesc: '',
        organizationShortDesc: '',
        effectiveStatus: ''
      }
    });
  };

  handleOrgSearchFilterChange = (field, value) => {
    const newFilters = { ...this.state.orgSearchFilters, [field]: value };
    this.setState({ orgSearchFilters: newFilters });
    
    // Apply filters to organizations
    this.applyOrgSearchFilters(newFilters);
  };

  applyOrgSearchFilters = (filters) => {
    const { organizations } = this.state;
    let filtered = [...organizations];

    // Apply each filter
    if (filters.organization) {
      filtered = filtered.filter(org => 
        org.code.toLowerCase().includes(filters.organization.toLowerCase())
      );
    }
    
    if (filters.organizationDesc) {
      filtered = filtered.filter(org => 
        (org.name || '').toLowerCase().includes(filters.organizationDesc.toLowerCase())
      );
    }
    
    if (filters.organizationShortDesc) {
      filtered = filtered.filter(org => 
        (org.shortDesc || '').toLowerCase().includes(filters.organizationShortDesc.toLowerCase())
      );
    }
    
    if (filters.effectiveStatus) {
      filtered = filtered.filter(org => 
        org.effectiveStatus === filters.effectiveStatus
      );
    }

    this.setState({ filteredOrganizations: filtered });
  };

  selectOrganization = (organization) => {
    this.handleOrganizationChange(organization.id);
    this.closeOrgSearchModal();
  };

  // Typable dropdown handlers
  handleOrgDropdownFilterChange = (value) => {
    this.setState({ orgDropdownFilter: value });
    
    if (value.trim() === '') {
      this.setState({ filteredOrgDropdown: this.state.organizations });
    } else {
      const filtered = this.state.organizations.filter(org => 
        org.code.toLowerCase().includes(value.toLowerCase()) ||
        (org.name || '').toLowerCase().includes(value.toLowerCase())
      );
      this.setState({ filteredOrgDropdown: filtered });
    }
  };

  toggleOrgDropdown = () => {
    const newOpenState = !this.state.orgDropdownOpen;
    this.setState({ 
      orgDropdownOpen: newOpenState,
      filteredOrgDropdown: newOpenState ? this.state.organizations : []
    });
  };

  selectOrgFromDropdown = (organization) => {
    this.handleOrganizationChange(organization.id);
    this.setState({
      orgDropdownOpen: false,
      orgDropdownFilter: `${organization.code} - ${organization.name}`
    });
  };

  closeOrgDropdown = () => {
    this.setState({ orgDropdownOpen: false });
  };

  // Fund search modal handlers
  openFundSearchModal = () => {
    this.setState({
      fundSearchModalOpen: true,
      filteredFunds: this.state.funds,
      fundSearchFilters: {
        fund: '',
        fundDesc: '',
        fundShortDesc: '',
        effectiveStatus: ''
      }
    });
  };

  closeFundSearchModal = () => {
    this.setState({
      fundSearchModalOpen: false,
      filteredFunds: [],
      fundSearchFilters: {
        fund: '',
        fundDesc: '',
        fundShortDesc: '',
        effectiveStatus: ''
      }
    });
  };

  handleFundSearchFilterChange = (field, value) => {
    const newFilters = { ...this.state.fundSearchFilters, [field]: value };
    this.setState({ fundSearchFilters: newFilters });
    this.applyFundSearchFilters(newFilters);
  };

  applyFundSearchFilters = (filters) => {
    const { funds } = this.state;
    let filtered = [...funds];

    if (filters.fund) {
      filtered = filtered.filter(fund => 
        fund.code.toLowerCase().includes(filters.fund.toLowerCase())
      );
    }
    
    if (filters.fundDesc) {
      filtered = filtered.filter(fund => 
        (fund.name || '').toLowerCase().includes(filters.fundDesc.toLowerCase())
      );
    }
    
    if (filters.fundShortDesc) {
      filtered = filtered.filter(fund => 
        (fund.shortDesc || '').toLowerCase().includes(filters.fundShortDesc.toLowerCase())
      );
    }
    
    if (filters.effectiveStatus) {
      filtered = filtered.filter(fund => 
        fund.effectiveStatus === filters.effectiveStatus
      );
    }

    this.setState({ filteredFunds: filtered });
  };

  selectFund = (fund) => {
    this.handleFundChange(fund.id);
    this.closeFundSearchModal();
  };

  // Fund typable dropdown handlers
  handleFundDropdownFilterChange = (value) => {
    this.setState({ fundDropdownFilter: value });
    
    if (value.trim() === '') {
      this.setState({ filteredFundDropdown: this.state.funds });
    } else {
      const filtered = this.state.funds.filter(fund => 
        fund.code.toLowerCase().includes(value.toLowerCase()) ||
        (fund.name || '').toLowerCase().includes(value.toLowerCase())
      );
      this.setState({ filteredFundDropdown: filtered });
    }
  };

  toggleFundDropdown = () => {
    const newOpenState = !this.state.fundDropdownOpen;
    this.setState({ 
      fundDropdownOpen: newOpenState,
      filteredFundDropdown: newOpenState ? this.state.funds : []
    });
  };

  selectFundFromDropdown = (fund) => {
    this.handleFundChange(fund.id);
    this.setState({
      fundDropdownOpen: false,
      fundDropdownFilter: `${fund.code} - ${fund.name}`
    });
  };

  closeFundDropdown = () => {
    this.setState({ fundDropdownOpen: false });
  };

  // OA1 search modal handlers
  openOa1SearchModal = () => {
    this.setState({
      oa1SearchModalOpen: true,
      filteredOa1s: this.state.oa1s,
      oa1SearchFilters: {
        oa1: '',
        oa1Desc: '',
        oa1ShortDesc: '',
        effectiveStatus: ''
      }
    });
  };

  closeOa1SearchModal = () => {
    this.setState({
      oa1SearchModalOpen: false,
      filteredOa1s: [],
      oa1SearchFilters: {
        oa1: '',
        oa1Desc: '',
        oa1ShortDesc: '',
        effectiveStatus: ''
      }
    });
  };

  handleOa1SearchFilterChange = (field, value) => {
    const newFilters = { ...this.state.oa1SearchFilters, [field]: value };
    this.setState({ oa1SearchFilters: newFilters });
    this.applyOa1SearchFilters(newFilters);
  };

  applyOa1SearchFilters = (filters) => {
    const { oa1s } = this.state;
    let filtered = [...oa1s];

    if (filters.oa1) {
      filtered = filtered.filter(oa1 => 
        oa1.code.toLowerCase().includes(filters.oa1.toLowerCase())
      );
    }
    
    if (filters.oa1Desc) {
      filtered = filtered.filter(oa1 => 
        (oa1.name || '').toLowerCase().includes(filters.oa1Desc.toLowerCase())
      );
    }
    
    if (filters.oa1ShortDesc) {
      filtered = filtered.filter(oa1 => 
        (oa1.shortDesc || '').toLowerCase().includes(filters.oa1ShortDesc.toLowerCase())
      );
    }
    
    if (filters.effectiveStatus) {
      filtered = filtered.filter(oa1 => 
        oa1.effectiveStatus === filters.effectiveStatus
      );
    }

    this.setState({ filteredOa1s: filtered });
  };

  selectOa1 = (oa1) => {
    this.handleOA1Change(oa1.id);
    this.closeOa1SearchModal();
  };

  // OA1 typable dropdown handlers
  handleOa1DropdownFilterChange = (value) => {
    this.setState({ oa1DropdownFilter: value });
    
    if (value.trim() === '') {
      this.setState({ filteredOa1Dropdown: this.state.oa1s });
    } else {
      const filtered = this.state.oa1s.filter(oa1 => 
        oa1.code.toLowerCase().includes(value.toLowerCase()) ||
        (oa1.name || '').toLowerCase().includes(value.toLowerCase())
      );
      this.setState({ filteredOa1Dropdown: filtered });
    }
  };

  toggleOa1Dropdown = () => {
    const newOpenState = !this.state.oa1DropdownOpen;
    this.setState({ 
      oa1DropdownOpen: newOpenState,
      filteredOa1Dropdown: newOpenState ? this.state.oa1s : []
    });
  };

  selectOa1FromDropdown = (oa1) => {
    this.handleOA1Change(oa1.id);
    this.setState({
      oa1DropdownOpen: false,
      oa1DropdownFilter: `${oa1.code} - ${oa1.name}`
    });
  };

  closeOa1Dropdown = () => {
    this.setState({ oa1DropdownOpen: false });
  };

  handleAssetChange = (field, value) => {
    const newAsset = { ...this.state.currentAsset, [field]: value };
    
    // Handle ID-based fields - store as integer or null (not empty string)
    if (field === 'objectCodeId') {
      newAsset.objectCodeId = value ? parseInt(value) : null;
    } else if (field === 'countyId') {
      newAsset.countyId = value ? parseInt(value) : null;
    } else if (field === 'assetGroupId') {
      newAsset.assetGroupId = value ? parseInt(value) : null;
    } else if (field === 'assetSubGroupId') {
      newAsset.assetSubGroupId = value ? parseInt(value) : null;
    }
    
    // If unchecking "Owned by County", clear the county field
    if (field === 'isOwnedByCounty' && !value) {
      newAsset.countyId = null;
    }
    
    this.setState({
      currentAsset: newAsset
    });
  };

  // Handle asset group selection and load subgroups
  handleAssetGroupChange = async (assetGroupId) => {
    const groupId = assetGroupId ? parseInt(assetGroupId) : null;
    
    // Update current asset with selected group and clear subgroup
    this.setState({
      currentAsset: {
        ...this.state.currentAsset,
        assetGroupId: groupId,
        assetSubGroupId: null  // Reset subgroup when parent changes
      },
      assetSubGroups: []  // Clear subgroups
    });
    
    // Load subgroups if a group is selected
    if (groupId) {
      try {
        const response = await fetch(`/api/AssetGroup/${groupId}/subgroups`);
        if (response.ok) {
          const subgroups = await response.json();
          this.setState({ assetSubGroups: Array.isArray(subgroups) ? subgroups : [] });
        }
      } catch (error) {
        console.error('Error loading asset subgroups:', error);
        this.setState({ assetSubGroups: [] });
      }
    }
  };

  // NOTE: Old saveAsset and deleteAsset methods removed - now using API-based methods:
  // - handleSaveAsset() - Saves asset to API immediately
  // - handleDeleteAsset() - Deletes asset via API immediately

  // Procurement field validation
  validateProcurementField = (fieldName, value, maxLength) => {
    const errors = { ...this.state.procurementValidationErrors };
    
    if (value && value.length > maxLength) {
      errors[fieldName] = `Maximum ${maxLength} characters allowed (current: ${value.length})`;
    } else {
      delete errors[fieldName];
    }
    
    this.setState({ procurementValidationErrors: errors });
    return !errors[fieldName];
  };

  // Helper method to get procurement type codes dynamically from backend data
  getProcurementTypeCode = (typeName) => {
    if (!this.state.procurementTypes || this.state.procurementTypes.length === 0) {
      return null;
    }
    
    const procurementType = this.state.procurementTypes.find(pt => 
      pt.name.toLowerCase().includes(typeName.toLowerCase())
    );
    return procurementType ? procurementType.code : null;
  };

  // Helper method to check if current procurement type matches a specific type
  isProcurementType = (typeName) => {
    if (!this.state.procurementTypeId || !this.state.procurementTypes) {
      return false;
    }
    
    const selectedProcurementType = this.state.procurementTypes.find(pt => 
      pt.id === parseInt(this.state.procurementTypeId)
    );
    
    if (!selectedProcurementType) {
      return false;
    }
    
    // Check by type name (more flexible)
    return selectedProcurementType.name.toLowerCase().includes(typeName.toLowerCase());
  };

  // Validate all procurement fields
  validateAllProcurementFields = () => {
    const { procurementType, pcardHolderFirstName, pcardHolderLastName, purchaseOrderNumber, contractNumber } = this.state;
    let isValid = true;

    // Validate ProcurementTypeCode (max 10 chars)
    if (!this.validateProcurementField('procurementType', procurementType, 10)) {
      isValid = false;
    }

    // Validate P-Card holder names (max 50 chars each)
    if (this.isProcurementType('card')) {
      if (!this.validateProcurementField('pcardHolderFirstName', pcardHolderFirstName, 50)) {
        isValid = false;
      }
      if (!this.validateProcurementField('pcardHolderLastName', pcardHolderLastName, 50)) {
        isValid = false;
      }
    }

    // Validate Purchase Order Number (max 50 chars)
    if (this.isProcurementType('order')) {
      if (!this.validateProcurementField('purchaseOrderNumber', purchaseOrderNumber, 50)) {
        isValid = false;
      }
    }

    // Validate Contract Number (max 50 chars)
    if (this.isProcurementType('contract')) {
      if (!this.validateProcurementField('contractNumber', contractNumber, 50)) {
        isValid = false;
      }
    }

    return isValid;
  };

  // Form submission (Save Receivable Report WITHOUT assets)
  handleSubmit = async (e) => {
    e.preventDefault();
    
    // Validate procurement fields before submission
    if (!this.validateAllProcurementFields()) {
      alert('Please fix the validation errors before submitting.');
      return;
    }
    
    try {
      this.setState({ isSaving: true });
      
      // Build procurement method data if provided
      let procurementMethod = null;
      if (this.state.procurementType) {
        procurementMethod = {
          procurementTypeId: this.state.procurementTypeId,
          procurementTypeCode: this.state.procurementType,
          chargeDate: this.state.chargeDate,
          pcardHolderFirstName: this.state.pcardHolderFirstName,
          pcardHolderLastName: this.state.pcardHolderLastName,
          groupId: this.state.groupId,
          purchaseOrderNumber: this.state.purchaseOrderNumber,
          contractNumber: this.state.contractNumber
        };
      }
      
      // Build the data object for API (WITHOUT assets - they are saved individually)
      const receivableReportData = receivableReportService.buildReceivableReportData({
        organizationId: this.state.organizationId,
        fundId: this.state.fundId,
        oa1Id: this.state.oa1Id,
        locationId: this.state.locationId,
        locationCode: this.state.locationCode,
        orderStatus: this.state.orderStatus,
        rrStatus: this.state.rrStatus,
        addressLine1: this.state.addressLine1,
        addressLine2: '', // Not used in current form
        city: this.state.city,
        county: this.state.county,
        state: this.state.state,
        postalCode: this.state.postalCode,
        procurementMethod: procurementMethod,
        assets: [] // Assets are now saved individually
      });

      let result;
      if (this.state.receivableReportId) {
        // Update existing receivable report
        result = await receivableReportService.updateReceivableReport(this.state.receivableReportId, receivableReportData);
        toast.success('Receivable Report updated successfully!');
      } else {
        // Create new receivable report
        result = await receivableReportService.createReceivableReport(receivableReportData);
        toast.success('Receivable Report created successfully! You can now add assets.');
        
        // Store the report ID and enable asset addition
        this.setState({ 
          receivableReportId: result.id,
          isReportSaved: true,
          isEditMode: true // Switch to edit mode after first save
        });
      }

      this.setState({ isSaving: false });
      
    } catch (error) {
      console.error('Error saving receivable report:', error);
      toast.error(error.message || 'Failed to save receivable report. Please try again.');
      this.setState({ isSaving: false });
    }
  };

  // Refresh assets grid from API
  refreshAssetsGrid = async () => {
    try {
      if (!this.state.receivableReportId) return;
      
      const assets = await assetService.getAssetsByReceivableReport(this.state.receivableReportId);
      this.setState({ assets });
    } catch (error) {
      console.error('Error refreshing assets:', error);
      toast.error('Error refreshing assets: ' + error.message);
    }
  };

  // Save individual asset (called from asset modal)
  handleSaveAsset = async () => {
    try {
      if (!this.state.receivableReportId) {
        toast.error('Please save the Receivable Report first before adding assets');
        return;
      }
      
      // Frontend validation
      const { currentAsset } = this.state;
      
      // Validate required fields
      if (!currentAsset.brand || !currentAsset.brand.trim()) {
        toast.error('Brand is required');
        return;
      }
      if (!currentAsset.make || !currentAsset.make.trim()) {
        toast.error('Make is required');
        return;
      }
      if (!currentAsset.model || !currentAsset.model.trim()) {
        toast.error('Model is required');
        return;
      }
      if (!currentAsset.serialNumber || !currentAsset.serialNumber.trim()) {
        toast.error('Serial Number is required');
        return;
      }
      if (!currentAsset.assetValue || parseFloat(currentAsset.assetValue) <= 0) {
        toast.error('Asset Value must be greater than 0');
        return;
      }
      
      // Validate ObjectCodeId (REQUIRED)
      if (!currentAsset.objectCodeId) {
        toast.error('Object Code is required');
        return;
      }
      
      // Validate CountyId (required only if IsOwnedByCounty is true)
      if (currentAsset.isOwnedByCounty && !currentAsset.countyId) {
        toast.error('County is required when "Owned by County" is selected');
        return;
      }
      
      this.setState({ isSaving: true });
      
      // Save asset via API
      await assetService.saveAsset(
        this.state.receivableReportId,
        this.state.currentAsset
      );
      
      // Refresh assets grid
      await this.refreshAssetsGrid();
      
      // Keep modal open and reset form for new asset
      this.setState({
        editingAsset: null,
        currentAsset: this.getEmptyAsset(),
        isSaving: false
      });
      
      toast.success('Asset saved successfully!');
    } catch (error) {
      console.error('Error saving asset:', error);
      toast.error('Error saving asset: ' + error.message);
      this.setState({ isSaving: false });
    }
  };

  // Delete asset
  handleDeleteAsset = async (assetId) => {
    if (!window.confirm('Are you sure you want to delete this asset?')) {
      return;
    }
    
    try {
      await assetService.deleteAsset(this.state.receivableReportId, assetId);
      await this.refreshAssetsGrid();
      toast.success('Asset deleted successfully!');
    } catch (error) {
      console.error('Error deleting asset:', error);
      toast.error('Error deleting asset: ' + error.message);
    }
  };

  handleGenerateAssetTag = async (assetId) => {
    const { receivableReportId } = this.state;
    
    if (!receivableReportId) {
      toast.error('Please save the receivable report before generating asset tags');
      return;
    }

    try {
      this.setState({ isSaving: true });

      // Use the new atomic endpoint that generates and assigns the tag in one operation
      // This prevents race conditions and duplicate tag errors
      const response = await fetch(`/api/receivablereport/${receivableReportId}/asset/${assetId}/generate-asset-tag`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        }
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText);
      }

      const updatedAsset = await response.json();

      // Update the asset in state
      this.setState(prevState => ({
        assets: prevState.assets.map(asset =>
          asset.id === assetId ? { ...asset, assetTag: updatedAsset.assetTag } : asset
        ),
        isSaving: false
      }));

      toast.success(`Asset Tag generated and saved: ${updatedAsset.assetTag}`);
    } catch (error) {
      console.error('Error generating asset tag:', error);
      toast.error('Error generating asset tag: ' + error.message);
      this.setState({ isSaving: false });
    }
  };

  handleResetAssetTag = async (assetId) => {
    const { receivableReportId } = this.state;
    
    if (!receivableReportId) {
      toast.error('Please save the receivable report first');
      return;
    }

    if (!window.confirm('Are you sure you want to reset the Asset Tag? This will:\n• Clear the asset tag\n• Reset status to "Open"\n• Allow you to generate a new tag')) {
      return;
    }

    try {
      this.setState({ isSaving: true });

      // Reset asset tag to NULL
      const response = await fetch(`/api/receivablereport/${receivableReportId}/asset/${assetId}/asset-tag`, {
        method: 'PATCH',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ assetTag: null })
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText);
      }

      const updatedAsset = await response.json();

      // Update the asset in state - clear tag and reset status to "Open"
      this.setState(prevState => ({
        assets: prevState.assets.map(asset =>
          asset.id === assetId ? { ...asset, assetTag: null, assetStatus: 'Open' } : asset
        ),
        isSaving: false
      }));

      toast.success('Asset Tag and status reset successfully');
    } catch (error) {
      console.error('Error resetting asset tag:', error);
      toast.error('Error resetting asset tag: ' + error.message);
      this.setState({ isSaving: false });
    }
  };

  // Helper method to get an empty asset object
  getEmptyAsset = () => ({
    brand: '',
    make: '',
    model: '',
    assetValue: '',
    // assetTag is NOT included - will be generated via grid button only
    serialNumber: '',
    objectCodeId: null,  // Use null instead of empty string for ID fields
    assetGroupId: null,
    assetSubGroupId: null,
    assignedTo: '',
    isOwnedByCounty: false,
    countyId: null,      // Use null instead of empty string for ID fields
    floor: '',
    room: '',
    uniqueTagNumber: '',
    assetStatus: 'Open'
  });

  // Button handlers for edit mode actions
  handleAttestTags = async () => {
    if (!this.state.receivableReportId) return;
    
    try {
      this.setState({ showError: false, showSuccess: false });
      const result = await receivableReportService.attestTags(this.props.id);
      
      this.setState({ 
        showSuccess: true,
        successMessage: `Tags attested successfully on ${new Date(result.attestedDate).toLocaleString()}`
      });
      
      // Reload the data to show updated status
      await this.loadReceivableReport(this.props.id);
      
    setTimeout(() => {
        this.setState({ showSuccess: false, successMessage: '' });
      }, 5000);
      
    } catch (error) {
      console.error('Error attesting tags:', error);
      this.setState({ 
        showError: true,
        errorMessage: 'Failed to attest tags. Please try again.'
      });
      
      setTimeout(() => {
        this.setState({ showError: false });
      }, 5000);
    }
  };

  handlePrintTags = async () => {
    if (!this.props.id) return;
    
    try {
      this.setState({ showError: false, showSuccess: false });
      const result = await receivableReportService.printTags(this.props.id);
      
      this.setState({ 
        showSuccess: true,
        successMessage: result.message || 'Tags printed successfully'
      });
      
      setTimeout(() => {
        this.setState({ showSuccess: false, successMessage: '' });
    }, 3000);
      
    } catch (error) {
      console.error('Error printing tags:', error);
      this.setState({ 
        showError: true,
        errorMessage: 'Failed to print tags. Please try again.'
      });
      
      setTimeout(() => {
        this.setState({ showError: false });
      }, 5000);
    }
  };


   // Asset-level operations
   handlePrintAssetTag = async (assetId) => {
     try {
       this.setState({ showError: false, showSuccess: false });
       const result = await receivableReportService.printAssetTag(assetId);
       
      // Extract the unique tag number from the response
      const uniqueTagNumber = result.uniqueTagNumber;
      
      // Update the asset in the state with the new status and unique tag number
      this.setState(prevState => ({
        assets: prevState.assets.map(asset =>
          asset.id === assetId
            ? { ...asset, assetStatus: 'Printed Tag', uniqueTagNumber: uniqueTagNumber }
            : asset
        )
      }));
      
      toast.success(`Asset tag printed successfully. Tag #: ${uniqueTagNumber}`);
      
      // Open print dialog with the print data
      const printData = result.printData;
      this.openPrintDialog(printData);
       
     } catch (error) {
       console.error('Error printing asset tag:', error);
       toast.error('Failed to print asset tag. Please try again.');
     }
   };

   handleAttestAssetTag = async (assetId) => {
     try {
       const result = await receivableReportService.attestAssetTag(assetId);
       
       // Update the asset in the state with the new status
       this.setState(prevState => ({
         assets: prevState.assets.map(asset =>
           asset.id === assetId
             ? { ...asset, assetStatus: 'Attested Tag' }
             : asset
         )
       }));
       
       toast.success(`Asset tag attested successfully on ${new Date(result.attestedDate).toLocaleString()}`);
       
     } catch (error) {
       console.error('Error attesting asset tag:', error);
       toast.error('Failed to attest asset tag. Please try again.');
     }
   };

   // Check if all assets are attested
   allAssetsAttested = () => {
     return this.state.assets.length > 0 && 
            this.state.assets.every(asset => asset.assetStatus === 'Attested Tag');
   };

   // Get badge class for asset status
   getAssetStatusBadgeClass = (status) => {
     switch (status) {
       case 'Open':
         return 'bg-secondary';
       case 'Printed Tag':
         return 'bg-warning';
       case 'Attested Tag':
         return 'bg-success';
       default:
         return 'bg-secondary';
     }
   };

  // Open print dialog for asset tag
  openPrintDialog = (printData) => {
    if (!printData) {
      console.error('Print data is undefined or null');
      alert('Error: Print data is not available');
      return;
    }

    // Extract data from response (backend returns camelCase)
    const data = {
      uniqueTagNumber: printData.uniqueTagNumber || 'N/A',
      organizationName: printData.organizationName || 'N/A',
      organizationCode: printData.organizationCode || 'N/A',
      brand: printData.brand || 'N/A',
      make: printData.make || 'N/A',
      model: printData.model || 'N/A',
      serialNumber: printData.serialNumber || 'N/A'
    };

    const printWindow = window.open('', '_blank', 'width=600,height=400');
    printWindow.document.write(`
      <html>
        <head>
          <title>Asset Tag - ${data.uniqueTagNumber}</title>
          <style>
            body { font-family: Arial, sans-serif; padding: 20px; }
            .tag-container { border: 2px solid #000; padding: 15px; width: 300px; margin: 0 auto; }
            .tag-header { text-align: center; font-weight: bold; font-size: 16px; margin-bottom: 10px; }
            .tag-content { font-size: 12px; line-height: 1.4; }
            .field { margin: 5px 0; }
            .label { font-weight: bold; }
            @media print { .no-print { display: none; } }
          </style>
        </head>
        <body>
          <div class="tag-container">
            <div class="tag-header">${data.organizationName}</div>
            <div class="tag-content">
              <div class="field"><span class="label">Org Code:</span> ${data.organizationCode}</div>
              <div class="field"><span class="label">Tag #:</span> ${data.uniqueTagNumber}</div>
              <div class="field"><span class="label">Brand:</span> ${data.brand}</div>
              <div class="field"><span class="label">Model:</span> ${data.make} ${data.model}</div>
              <div class="field"><span class="label">Serial:</span> ${data.serialNumber}</div>
            </div>
          </div>
           <div class="no-print" style="text-align: center; margin-top: 20px;">
             <button onclick="window.print()">Print Tag</button>
             <button onclick="window.close()">Close</button>
           </div>
         </body>
       </html>
     `);
     printWindow.document.close();
   };

  // Submit RR (change status from Draft to Submitted)
  handleSubmitRR = async () => {
    try {
      this.setState({ isSaving: true });
      const result = await receivableReportService.submitReceivableReport(this.state.receivableReportId || this.props.id);
      
      // Update local state
      this.setState({
        rrStatus: 'Submitted',
        isSaving: false
      });
      
      toast.success('Receivable Report submitted successfully!');
      console.log('RR submitted successfully:', result);
    } catch (error) {
      console.error('Error submitting RR:', error);
      toast.error(`Error submitting RR: ${error.message}`);
      this.setState({
        isSaving: false
      });
    }
  };

  // Complete RR (when all assets are attested)
  handleCompleteRR = async () => {
    try {
      this.setState({ isSaving: true });
      const result = await receivableReportService.completeReceivableReport(this.props.id);
      
      // Update local state
      this.setState({
        rrStatus: 'Complete',
        isSaving: false
      });
      
      toast.success('Receivable Report completed successfully!');
      console.log('RR completed successfully:', result);
    } catch (error) {
      console.error('Error completing RR:', error);
      toast.error(`Error completing RR: ${error.message}`);
      this.setState({
        isSaving: false
      });
    }
  };

  // Helper method to get RR status badge class
  getRRStatusBadgeClass = (status) => {
    switch (status) {
      case 'Draft':
        return 'bg-secondary';
      case 'Submitted':
        return 'bg-warning';
      case 'Complete':
        return 'bg-success';
      default:
        return 'bg-secondary';
    }
  };

  render() {
    const { 
      isEditMode,
      isReportSaved,
      orgDetailsCollapsed, 
      procurementCollapsed, 
      assetsCollapsed, 
      organizationId,
      organizationName,
      fundId,
      oa1Id,
      locationId,
      locationCode,
      addressLine1,
      city,
      county,
      state,
      postalCode,
      orderStatus,
      rrStatus,
      // Procurement data
      procurementTypeId,
      procurementType,
      chargeDate,
      pcardHolderFirstName,
      pcardHolderLastName,
      groupId,
      purchaseOrderNumber,
      contractNumber,
      assets,
      assetModalOpen,
      procurementValidationErrors,
      currentAsset,
      editingAsset,
      // Organization search modal state
      orgSearchModalOpen,
      filteredOrganizations,
      orgSearchFilters,
      // Typable dropdown state
      orgDropdownOpen,
      orgDropdownFilter,
      filteredOrgDropdown,
      // Fund search modal state
      fundSearchModalOpen,
      filteredFunds,
      fundSearchFilters,
      // Fund typable dropdown state
      fundDropdownOpen,
      fundDropdownFilter,
      filteredFundDropdown,
      // OA1 search modal state
      oa1SearchModalOpen,
      filteredOa1s,
      oa1SearchFilters,
      // OA1 typable dropdown state
      oa1DropdownOpen,
      oa1DropdownFilter,
      filteredOa1Dropdown,
      // FDW lookup data
      organizations,
      funds,
      oa1s,
      // Other lookup data
      locations,
      objectCodes,
      countyCodes,
      procurementTypes,
      assetGroups,
      assetSubGroups,
      isLoading,
      isSaving,
      isLoadingLookupData,
      dataLoaded
    } = this.state;

    // Show loading spinner while data is loading
    if (!dataLoaded || isLoadingLookupData || isLoading) {
      return (
        <Container fluid className="d-flex justify-content-center align-items-center" style={{ minHeight: '400px' }}>
          <div className="text-center">
            <Spinner color="primary" style={{ width: '3rem', height: '3rem' }} />
            <div className="mt-2">Loading...</div>
          </div>
        </Container>
      );
    }

    return (
      <Container fluid onClick={() => {
        this.closeOrgDropdown();
        this.closeFundDropdown();
        this.closeOa1Dropdown();
      }}>
        <Row>
          <Col>
            <div className="d-flex justify-content-between align-items-center mb-4">
              <h2>{isEditMode ? 'Edit Receivable Report' : 'Create Receivable Report'}</h2>
              <Button color="secondary" tag={Link} to="/receivable-report/search">
                <i className="fas fa-arrow-left me-2"></i>Back to Search
              </Button>
            </div>
          </Col>
        </Row>


        <Form onSubmit={this.handleSubmit}>
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
                      <Col md={4}>
                        <FormGroup>
                          <Label for="organizationId">Organization *</Label>
                          <div className="d-flex" onClick={(e) => e.stopPropagation()}>
                            <div className="position-relative flex-grow-1 me-2">
                              <Input
                                type="text"
                                id="organizationId"
                                value={orgDropdownFilter || (organizationId ? `${this.state.organizations.find(org => org.id === organizationId)?.code || ''} - ${this.state.organizations.find(org => org.id === organizationId)?.name || ''}` : '')}
                                onChange={(e) => this.handleOrgDropdownFilterChange(e.target.value)}
                                onFocus={this.toggleOrgDropdown}
                                placeholder="Type to search organizations..."
                                required
                                autoComplete="off"
                              />
                              {orgDropdownOpen && (
                                <div className="position-absolute w-100 bg-white border border-top-0 rounded-bottom shadow-lg" style={{ zIndex: 1000, maxHeight: '200px', overflowY: 'auto' }}>
                                  {filteredOrgDropdown.length > 0 ? (
                                    filteredOrgDropdown.map(org => (
                                      <div
                                        key={org.id}
                                        className="p-2 border-bottom cursor-pointer hover-bg-light"
                                        onClick={() => this.selectOrgFromDropdown(org)}
                                        style={{ cursor: 'pointer' }}
                                        onMouseEnter={(e) => e.target.style.backgroundColor = '#f8f9fa'}
                                        onMouseLeave={(e) => e.target.style.backgroundColor = 'white'}
                                      >
                                        <div className="fw-bold">{org.code}</div>
                                        <div className="text-muted small">{org.name}</div>
                                      </div>
                                    ))
                                  ) : (
                                    <div className="p-2 text-muted">No organizations found</div>
                                  )}
                                </div>
                              )}
                            </div>
                            <Button
                              color="info"
                              size="sm"
                              onClick={this.openOrgSearchModal}
                              title="Search Organizations"
                              className="d-flex align-items-center"
                            >
                              <i className="fas fa-search"></i>
                            </Button>
                          </div>
                        </FormGroup>
                      </Col>
                      <Col md={4}>
                        <FormGroup>
                          <Label for="fundId">Fund *</Label>
                          <div className="d-flex" onClick={(e) => e.stopPropagation()}>
                            <div className="position-relative flex-grow-1 me-2">
                              <Input
                                type="text"
                                id="fundId"
                                value={fundDropdownFilter || (fundId ? `${this.state.funds.find(fund => fund.id === fundId)?.code || ''} - ${this.state.funds.find(fund => fund.id === fundId)?.name || ''}` : '')}
                                onChange={(e) => this.handleFundDropdownFilterChange(e.target.value)}
                                onFocus={this.toggleFundDropdown}
                                placeholder="Type to search funds..."
                                required
                                autoComplete="off"
                              />
                              {fundDropdownOpen && (
                                <div className="position-absolute w-100 bg-white border border-top-0 rounded-bottom shadow-lg" style={{ zIndex: 1000, maxHeight: '200px', overflowY: 'auto' }}>
                                  {filteredFundDropdown.length > 0 ? (
                                    filteredFundDropdown.map(fund => (
                                      <div
                                        key={fund.id}
                                        className="p-2 border-bottom cursor-pointer hover-bg-light"
                                        onClick={() => this.selectFundFromDropdown(fund)}
                                        style={{ cursor: 'pointer' }}
                                        onMouseEnter={(e) => e.target.style.backgroundColor = '#f8f9fa'}
                                        onMouseLeave={(e) => e.target.style.backgroundColor = 'white'}
                                      >
                                        <div className="fw-bold">{fund.code}</div>
                                        <div className="text-muted small">{fund.name}</div>
                                      </div>
                                    ))
                                  ) : (
                                    <div className="p-2 text-muted">No funds found</div>
                                  )}
                                </div>
                              )}
                            </div>
                            <Button
                              color="info"
                              size="sm"
                              onClick={this.openFundSearchModal}
                              title="Search Funds"
                              className="d-flex align-items-center"
                            >
                              <i className="fas fa-search"></i>
                            </Button>
                          </div>
                        </FormGroup>
                      </Col>
                      <Col md={4}>
                        <FormGroup>
                          <Label for="oa1Id">OA1 *</Label>
                          <div className="d-flex" onClick={(e) => e.stopPropagation()}>
                            <div className="position-relative flex-grow-1 me-2">
                              <Input
                                type="text"
                                id="oa1Id"
                                value={oa1DropdownFilter || (oa1Id ? `${this.state.oa1s.find(oa1 => oa1.id === oa1Id)?.code || ''} - ${this.state.oa1s.find(oa1 => oa1.id === oa1Id)?.name || ''}` : '')}
                                onChange={(e) => this.handleOa1DropdownFilterChange(e.target.value)}
                                onFocus={this.toggleOa1Dropdown}
                                placeholder="Type to search OA1s..."
                                required
                                autoComplete="off"
                              />
                              {oa1DropdownOpen && (
                                <div className="position-absolute w-100 bg-white border border-top-0 rounded-bottom shadow-lg" style={{ zIndex: 1000, maxHeight: '200px', overflowY: 'auto' }}>
                                  {filteredOa1Dropdown.length > 0 ? (
                                    filteredOa1Dropdown.map(oa1 => (
                                      <div
                                        key={oa1.id}
                                        className="p-2 border-bottom cursor-pointer hover-bg-light"
                                        onClick={() => this.selectOa1FromDropdown(oa1)}
                                        style={{ cursor: 'pointer' }}
                                        onMouseEnter={(e) => e.target.style.backgroundColor = '#f8f9fa'}
                                        onMouseLeave={(e) => e.target.style.backgroundColor = 'white'}
                                      >
                                        <div className="fw-bold">{oa1.code}</div>
                                        <div className="text-muted small">{oa1.name}</div>
                                      </div>
                                    ))
                                  ) : (
                                    <div className="p-2 text-muted">No OA1s found</div>
                                  )}
                                </div>
                              )}
                            </div>
                            <Button
                              color="info"
                              size="sm"
                              onClick={this.openOa1SearchModal}
                              title="Search OA1s"
                              className="d-flex align-items-center"
                            >
                              <i className="fas fa-search"></i>
                            </Button>
                          </div>
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
                            value={organizationName}
                            readOnly
                            placeholder="Auto-populated based on selection"
                          />
                        </FormGroup>
                      </Col>
                      <Col md={6}>
                        <FormGroup>
                          <Label for="locationId">Location</Label>
                          
                          <Input
                            type="select"
                            id="locationId"
                            value={locationId ? String(locationId) : ''}
                            onChange={(e) => this.handleLocationChange(e.target.value)}
                          >
                            <option value="">Select Location</option>
                            {locations && Object.keys(locations).length > 0 ? (
                              Object.keys(locations).map(locId => (
                                <option key={locId} value={locId}>
                                  {locations[locId].code} - {locations[locId].city}
                                </option>
                              ))
                            ) : (
                              <option value="" disabled>No locations available</option>
                            )}
                          </Input>
                        </FormGroup>
                      </Col>
                    </Row>
                    <Row>
                      <Col md={6}>
                        <FormGroup>
                          <Label for="orderStatus">Order Status *</Label>
                          <Input
                            type="select"
                            id="orderStatus"
                            value={orderStatus}
                            onChange={(e) => this.setState({ orderStatus: e.target.value })}
                            required
                          >
                            <option value="">Select Order Status</option>
                            <option value="Partial">Partial</option>
                            <option value="Complete">Complete</option>
                          </Input>
                        </FormGroup>
                      </Col>
                    </Row>
                    <Row>
                      <Col md={6}>
                        <FormGroup>
                          <Label for="addressLine1">Address Line 1</Label>
                          <Input
                            type="text"
                            id="addressLine1"
                            value={addressLine1}
                            onChange={(e) => this.setState({ addressLine1: e.target.value })}
                            placeholder="Auto-populated based on Location Code"
                          />
                        </FormGroup>
                      </Col>
                      <Col md={6}>
                        <FormGroup>
                          <Label for="city">City</Label>
                          <Input
                            type="text"
                            id="city"
                            value={city}
                            onChange={(e) => this.setState({ city: e.target.value })}
                            placeholder="Auto-populated based on Location Code"
                          />
                        </FormGroup>
                      </Col>
                    </Row>
                    <Row>
                      <Col md={4}>
                        <FormGroup>
                          <Label for="county">County</Label>
                          <Input
                            type="text"
                            id="county"
                            value={county}
                            onChange={(e) => this.setState({ county: e.target.value })}
                            placeholder="Auto-populated based on Location Code"
                          />
                        </FormGroup>
                      </Col>
                      <Col md={4}>
                        <FormGroup>
                          <Label for="state">State</Label>
                          <Input
                            type="text"
                            id="state"
                            value={state}
                            onChange={(e) => this.setState({ state: e.target.value })}
                            placeholder="Auto-populated based on Location Code"
                          />
                        </FormGroup>
                      </Col>
                      <Col md={4}>
                        <FormGroup>
                          <Label for="postalCode">Postal Code</Label>
                          <Input
                            type="text"
                            id="postalCode"
                            value={postalCode}
                            onChange={(e) => this.setState({ postalCode: e.target.value })}
                            placeholder="Auto-populated based on Location Code"
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
                          <Label for="procurementType">Procurement Type *</Label>
                          <Input
                            type="select"
                            id="procurementType"
                            value={procurementTypeId ? String(procurementTypeId) : ''}
                            onChange={(e) => {
                              const newId = e.target.value;
                              const selectedProcurementType = this.state.procurementTypes.find(pt => pt.id === parseInt(newId));
                              
                              this.setState({ 
                                procurementTypeId: newId,
                                procurementType: selectedProcurementType ? selectedProcurementType.code : '',
                                // Clear P-Card fields if not P-Card type
                                chargeDate: selectedProcurementType?.name.toLowerCase().includes('card') ? chargeDate : '',
                                pcardHolderFirstName: selectedProcurementType?.name.toLowerCase().includes('card') ? pcardHolderFirstName : '',
                                pcardHolderLastName: selectedProcurementType?.name.toLowerCase().includes('card') ? pcardHolderLastName : '',
                                groupId: selectedProcurementType?.name.toLowerCase().includes('card') ? groupId : '',
                                // Clear Purchase Order field if not Purchase Order type
                                purchaseOrderNumber: selectedProcurementType?.name.toLowerCase().includes('order') ? purchaseOrderNumber : '',
                                // Clear Contract field if not Contract type
                                contractNumber: selectedProcurementType?.name.toLowerCase().includes('contract') ? contractNumber : ''
                              }, () => {
                                // Callback to ensure state is updated before validation
                                this.validateProcurementField('procurementType', selectedProcurementType?.code || '', 10);
                              });
                            }}
                            invalid={!!procurementValidationErrors.procurementType}
                            required
                          >
                            <option value="">Select Procurement Type</option>
                            {this.state.procurementTypes && Array.isArray(this.state.procurementTypes) && this.state.procurementTypes.length > 0 ? (
                              this.state.procurementTypes.map(pt => (
                                <option key={pt.id} value={String(pt.id)}>{pt.name || pt.code || 'Unknown'}</option>
                              ))
                            ) : (
                              <option value="" disabled>No procurement types available</option>
                            )}
                          </Input>
                          {procurementValidationErrors.procurementType && (
                            <div className="text-danger small mt-1">
                              {procurementValidationErrors.procurementType}
                            </div>
                          )}
                        </FormGroup>
                      </Col>
                    </Row>
                    
                    {/* P-Card specific fields */}
                    {this.isProcurementType('card') && (
                      <>
                        <Row>
                          <Col md={6}>
                            <FormGroup>
                              <Label for="chargeDate">Charge Date *</Label>
                              <Input
                                type="date"
                                id="chargeDate"
                                value={chargeDate}
                                onChange={(e) => this.setState({ chargeDate: e.target.value })}
                                required
                              />
                            </FormGroup>
                          </Col>
                          <Col md={6}>
                            <FormGroup>
                              <Label for="groupId">Group ID *</Label>
                              <Input
                                type="text"
                                id="groupId"
                                value={groupId}
                                onChange={(e) => this.setState({ groupId: e.target.value })}
                                placeholder="Enter Group ID"
                                required
                              />
                            </FormGroup>
                          </Col>
                        </Row>
                        <Row>
                          <Col md={6}>
                            <FormGroup>
                              <Label for="pcardHolderFirstName">P-Card Holder First Name * (Max 50 chars)</Label>
                              <Input
                                type="text"
                                id="pcardHolderFirstName"
                                value={pcardHolderFirstName}
                                onChange={(e) => {
                                  const newValue = e.target.value;
                                  this.setState({ pcardHolderFirstName: newValue });
                                  this.validateProcurementField('pcardHolderFirstName', newValue, 50);
                                }}
                                placeholder="Enter first name"
                                maxLength="50"
                                invalid={!!procurementValidationErrors.pcardHolderFirstName}
                                required
                              />
                              {procurementValidationErrors.pcardHolderFirstName && (
                                <div className="text-danger small mt-1">
                                  {procurementValidationErrors.pcardHolderFirstName}
                                </div>
                              )}
                            </FormGroup>
                          </Col>
                          <Col md={6}>
                            <FormGroup>
                              <Label for="pcardHolderLastName">P-Card Holder Last Name * (Max 50 chars)</Label>
                              <Input
                                type="text"
                                id="pcardHolderLastName"
                                value={pcardHolderLastName}
                                onChange={(e) => {
                                  const newValue = e.target.value;
                                  this.setState({ pcardHolderLastName: newValue });
                                  this.validateProcurementField('pcardHolderLastName', newValue, 50);
                                }}
                                placeholder="Enter last name"
                                maxLength="50"
                                invalid={!!procurementValidationErrors.pcardHolderLastName}
                                required
                              />
                              {procurementValidationErrors.pcardHolderLastName && (
                                <div className="text-danger small mt-1">
                                  {procurementValidationErrors.pcardHolderLastName}
                                </div>
                              )}
                            </FormGroup>
                          </Col>
                        </Row>
                      </>
                    )}
                    
                    {/* Purchase Order specific field */}
                    {this.isProcurementType('order') && (
                      <Row>
                        <Col md={6}>
                          <FormGroup>
                            <Label for="purchaseOrderNumber">Purchase Order Number * (Max 50 chars)</Label>
                            <Input
                              type="text"
                              id="purchaseOrderNumber"
                              value={purchaseOrderNumber}
                              onChange={(e) => {
                                const newValue = e.target.value;
                                this.setState({ purchaseOrderNumber: newValue });
                                this.validateProcurementField('purchaseOrderNumber', newValue, 50);
                              }}
                              placeholder="Enter Purchase Order Number"
                              maxLength="50"
                              invalid={!!procurementValidationErrors.purchaseOrderNumber}
                              required
                            />
                            {procurementValidationErrors.purchaseOrderNumber && (
                              <div className="text-danger small mt-1">
                                {procurementValidationErrors.purchaseOrderNumber}
                              </div>
                            )}
                          </FormGroup>
                        </Col>
                      </Row>
                    )}
                    
                    {/* Contract Asset specific field */}
                    {this.isProcurementType('contract') && (
                      <Row>
                        <Col md={6}>
                          <FormGroup>
                            <Label for="contractNumber">Contract Number * (Max 50 chars)</Label>
                            <Input
                              type="text"
                              id="contractNumber"
                              value={contractNumber}
                              onChange={(e) => {
                                const newValue = e.target.value;
                                this.setState({ contractNumber: newValue });
                                this.validateProcurementField('contractNumber', newValue, 50);
                              }}
                              placeholder="Enter Contract Number"
                              maxLength="50"
                              invalid={!!procurementValidationErrors.contractNumber}
                              required
                            />
                            {procurementValidationErrors.contractNumber && (
                              <div className="text-danger small mt-1">
                                {procurementValidationErrors.contractNumber}
                              </div>
                            )}
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
                  <h5 className="mb-0">3. Assets</h5>
                  <span>{assetsCollapsed ? '▼' : '▲'}</span>
                </CardHeader>
                <Collapse isOpen={!assetsCollapsed}>
                  <CardBody>
                    <div className="d-flex justify-content-between align-items-center mb-3">
                      <div>
                      <h6>Asset Details</h6>
                        {!isReportSaved && (
                          <small className="text-muted">Save the Receivable Report first to add assets</small>
                        )}
                      </div>
                      <Button 
                        color="success" 
                        onClick={() => this.openAssetModal()}
                        disabled={!isReportSaved}
                      >
                        <i className="fas fa-plus me-2"></i>Add New Asset
                      </Button>
                    </div>
                    
                    {assets.length > 0 ? (
                      <Table responsive striped>
                        <thead>
                          <tr>
                            <th>Floor</th>
                            <th>Room</th>
                            <th>Asset Tag</th>
                            <th>Brand</th>
                            <th>Make</th>
                            <th>Model</th>
                            <th>Asset Value</th>
                            <th>Serial Number</th>
                            <th>Unique Tag #</th>
                            <th>Object Code</th>
                            <th>Assigned To</th>
                            <th>County Owned</th>
                            <th>Status</th>
                            <th>Actions</th>
                          </tr>
                        </thead>
                        <tbody>
                          {assets.map(asset => (
                            <tr key={asset.id}>
                              <td>{asset.floor || 'N/A'}</td>
                              <td>{asset.room || 'N/A'}</td>
                              <td>
                                <div className="d-flex align-items-center">
                                  <code className="me-2">{asset.assetTag || 'Not Set'}</code>
                                  {!asset.assetTag ? (
                                    // Show Generate button when no tag exists
                                    <Button
                                      size="sm"
                                      color="primary"
                                      onClick={() => this.handleGenerateAssetTag(asset.id)}
                                      title="Generate Unique Asset Tag"
                                      disabled={isSaving}
                                    >
                                      <i className="fas fa-pencil-alt"></i>
                                    </Button>
                                  ) : (
                                    // Show Reset button when tag exists
                                    <Button
                                      size="sm"
                                      color="warning"
                                      onClick={() => this.handleResetAssetTag(asset.id)}
                                      title="Reset Asset Tag"
                                      disabled={isSaving}
                                    >
                                      <i className="fas fa-undo"></i>
                                    </Button>
                                  )}
                                </div>
                              </td>
                              <td>{asset.brand}</td>
                              <td>{asset.make}</td>
                              <td>{asset.model}</td>
                              <td>${(parseFloat(asset.assetValue) || 0).toFixed(2)}</td>
                              <td>{asset.serialNumber}</td>
                              <td>
                                <code>{asset.uniqueTagNumber || 'Not Generated'}</code>
                              </td>
                              <td>{asset.objectCode || 'N/A'}</td>
                              <td>{asset.assignedTo || 'N/A'}</td>
                              <td>
                                {asset.isOwnedByCounty ? (
                                  <span className="text-success">
                                    ✓ {asset.countyCode ? `(${asset.countyCode})` : ''}
                                  </span>
                                ) : (
                                  <span className="text-muted">No</span>
                                )}
                              </td>
                              <td>
                                <span className={`badge ${this.getAssetStatusBadgeClass(asset.assetStatus)}`}>
                                  {asset.assetStatus || 'Open'}
                                </span>
                              </td>
                              <td>
                                <Button
                                  size="sm"
                                  color="info"
                                  onClick={() => this.openAssetModal(asset)}
                                  className="me-1"
                                  title="Edit Asset"
                                >
                                  <i className="fas fa-edit"></i>
                                </Button>
                                <Button
                                  size="sm"
                                  color="danger"
                                  onClick={() => this.handleDeleteAsset(asset.id)}
                                  className="me-1"
                                  title="Delete Asset"
                                >
                                  <i className="fas fa-trash"></i>
                                </Button>
                                {/* Print Asset Tag button - only show when tag exists AND status is Open/Not Set */}
                                {isEditMode && asset.assetTag && (asset.assetStatus === 'Open' || !asset.assetStatus) && (
                                  <Button
                                    size="sm"
                                    color="warning"
                                    onClick={() => this.handlePrintAssetTag(asset.id)}
                                    className="me-1"
                                    title="Print Asset Tag"
                                  >
                                    <i className="fas fa-print"></i>
                                  </Button>
                                )}
                                {/* Attest Asset Tag button - only show when status is 'Printed Tag' */}
                                {isEditMode && asset.assetStatus === 'Printed Tag' && (
                                  <Button
                                    size="sm"
                                    color="success"
                                    onClick={() => this.handleAttestAssetTag(asset.id)}
                                    className="me-1"
                                    title="Attest Asset Tag"
                                  >
                                    <i className="fas fa-check-circle"></i>
                                  </Button>
                                )}
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </Table>
                    ) : (
                      <div className="text-center text-muted p-4">
                        <i className="fas fa-info-circle me-2"></i>
                        No assets added yet. Click "Add New Asset" to get started.
                      </div>
                    )}
                  </CardBody>
                </Collapse>
              </Card>
            </Col>
          </Row>

          {/* RR Status Information (Edit Mode Only) */}
          {isEditMode && (
            <Row className="mb-3">
              <Col>
                <Card>
                  <CardHeader>
                    <h5 className="mb-0">4. Receivable Report Status</h5>
                  </CardHeader>
                  <CardBody>
                    <Row>
                      <Col md={6}>
                        <div>
                          <strong>Current Status:</strong>
                          <span className={`badge ms-2 ${this.getRRStatusBadgeClass(rrStatus)}`}>
                            {rrStatus}
                          </span>
                        </div>
                      </Col>
                      <Col md={6}>
                        <div className="text-muted">
                          <small>
                            {rrStatus === 'Draft' && 'Ready for submission when all data is complete'}
                            {rrStatus === 'Submitted' && 'Awaiting asset tag attestation'}
                            {rrStatus === 'Complete' && 'All assets have been attested and report is complete'}
                          </small>
                        </div>
                      </Col>
                    </Row>
                  </CardBody>
                </Card>
              </Col>
            </Row>
          )}

          {/* Form Actions */}
          <Row>
            <Col>
              <div className="d-flex justify-content-end">
                <Button color="secondary" tag={Link} to="/receivable-report/search" className="me-2">
                  <i className="fas fa-times me-2"></i>Cancel
                </Button>
                 {/* Workflow buttons - Show only one at a time based on status */}
                 {isEditMode && rrStatus === 'Draft' && (
                   /* Submit RR Button - Only when status is Draft */
                   <Button color="warning" onClick={this.handleSubmitRR} className="me-2" disabled={isSaving}>
                     <i className="fas fa-paper-plane me-2"></i>Submit RR
                   </Button>
                 )}
                 {isEditMode && rrStatus === 'Submitted' && this.allAssetsAttested() && (
                   /* Complete RR Button - Only when status is Submitted and all assets are attested */
                   <Button color="success" onClick={this.handleCompleteRR} className="me-2" disabled={isSaving}>
                     <i className="fas fa-check-double me-2"></i>Complete RR
                   </Button>
                 )}
                 {/* Update RR Button - Always available */}
                 <Button color="primary" type="submit" disabled={isSaving}>
                   {isSaving && <Spinner size="sm" className="me-2" />}
                   {!isSaving && <i className="fas fa-save me-2"></i>}
                   {isSaving 
                     ? (isEditMode ? 'Updating...' : 'Creating...') 
                     : (isEditMode ? 'Update Receivable Report' : 'Create Receivable Report')
                   }
                </Button>
              </div>
            </Col>
          </Row>
        </Form>

        {/* Asset Modal */}
        <Modal isOpen={assetModalOpen} toggle={this.closeAssetModal} size="xl">
          <ModalHeader toggle={this.closeAssetModal}>
            Add New Asset
          </ModalHeader>
          <ModalBody>
            <Form>
              {/* Row 1: Floor and Room */}
              <Row>
                <Col md={6}>
                  <FormGroup>
                    <Label for="floor">Floor</Label>
                    <Input
                      type="text"
                      id="floor"
                      value={currentAsset.floor}
                      onChange={(e) => this.handleAssetChange('floor', e.target.value)}
                      placeholder="Enter floor number"
                    />
                  </FormGroup>
                </Col>
                <Col md={6}>
                  <FormGroup>
                    <Label for="room">Room</Label>
                    <Input
                      type="text"
                      id="room"
                      value={currentAsset.room}
                      onChange={(e) => this.handleAssetChange('room', e.target.value)}
                      placeholder="Enter room number"
                    />
                  </FormGroup>
                </Col>
              </Row>
              
              {/* Row 2: Assigned To and County Owned checkbox */}
              <Row>
                <Col md={6}>
                  <FormGroup>
                    <Label for="assignedTo">Assigned To</Label>
                    <Input
                      type="text"
                      id="assignedTo"
                      value={currentAsset.assignedTo}
                      onChange={(e) => this.handleAssetChange('assignedTo', e.target.value)}
                      placeholder="Enter person or department"
                    />
                  </FormGroup>
                </Col>
                <Col md={6}>
                  <FormGroup check style={{ paddingTop: '32px' }}>
                    <Label check>
                      <Input
                        type="checkbox"
                        checked={currentAsset.isOwnedByCounty}
                        onChange={(e) => this.handleAssetChange('isOwnedByCounty', e.target.checked)}
                      />
                      {' '}Owned by County
                    </Label>
                  </FormGroup>
                </Col>
              </Row>
              
              {/* Row 3: County Dropdown (conditional - required when Owned by County) */}
              {currentAsset.isOwnedByCounty && (
                <Row>
                  <Col md={6}>
                    <FormGroup>
                      <Label for="countyId">County *</Label>
                      <Input
                        type="select"
                        id="countyId"
                        value={currentAsset.countyId ? String(currentAsset.countyId) : ''}
                        onChange={(e) => this.handleAssetChange('countyId', e.target.value)}
                        required
                      >
                        <option value="">Select County</option>
                        {countyCodes.map(county => (
                          <option key={county.id} value={String(county.id)}>
                            {county.code} - {county.name}
                          </option>
                        ))}
                      </Input>
                    </FormGroup>
                  </Col>
                </Row>
              )}
              
              {/* Row 4: Asset Group and Asset Subgroup */}
              <Row>
                <Col md={6}>
                  <FormGroup>
                    <Label for="assetGroup">Asset Group</Label>
                    <Input
                      type="select"
                      id="assetGroup"
                      value={currentAsset.assetGroupId ? String(currentAsset.assetGroupId) : ''}
                      onChange={(e) => this.handleAssetGroupChange(e.target.value)}
                    >
                      <option value="">Select Asset Group (Optional)</option>
                      {assetGroups.map(group => (
                        <option key={group.id} value={String(group.id)}>
                          {group.name}
                        </option>
                      ))}
                    </Input>
                    {currentAsset.assetGroupId && (
                      <small className="text-muted">
                        {assetGroups.find(g => g.id === currentAsset.assetGroupId)?.description || ''}
                      </small>
                    )}
                  </FormGroup>
                </Col>
                <Col md={6}>
                  <FormGroup>
                    <Label for="assetSubGroup">Asset Subgroup</Label>
                    <Input
                      type="select"
                      id="assetSubGroup"
                      value={currentAsset.assetSubGroupId ? String(currentAsset.assetSubGroupId) : ''}
                      onChange={(e) => this.handleAssetChange('assetSubGroupId', e.target.value)}
                      disabled={!currentAsset.assetGroupId || assetSubGroups.length === 0}
                    >
                      <option value="">
                        {!currentAsset.assetGroupId 
                          ? 'Select Group First' 
                          : assetSubGroups.length === 0 
                            ? 'No Subgroups Available' 
                            : 'Select Subgroup (Optional)'}
                      </option>
                      {assetSubGroups.map(subgroup => (
                        <option key={subgroup.id} value={String(subgroup.id)}>
                          {subgroup.name}
                        </option>
                      ))}
                    </Input>
                    {currentAsset.assetSubGroupId && (
                      <small className="text-muted">
                        {assetSubGroups.find(sg => sg.id === currentAsset.assetSubGroupId)?.description || ''}
                      </small>
                    )}
                  </FormGroup>
                </Col>
              </Row>
              
              {/* Row 5: Brand and Make */}
              <Row>
                <Col md={6}>
                  <FormGroup>
                    <Label for="assetBrand">Brand *</Label>
                    <Input
                      type="text"
                      id="assetBrand"
                      value={currentAsset.brand}
                      onChange={(e) => this.handleAssetChange('brand', e.target.value)}
                      required
                    />
                  </FormGroup>
                </Col>
                <Col md={6}>
                  <FormGroup>
                    <Label for="assetMake">Make *</Label>
                    <Input
                      type="text"
                      id="assetMake"
                      value={currentAsset.make}
                      onChange={(e) => this.handleAssetChange('make', e.target.value)}
                      required
                    />
                  </FormGroup>
                </Col>
              </Row>
              
              {/* Row 6: Model and Serial Number */}
              <Row>
                <Col md={6}>
                  <FormGroup>
                    <Label for="assetModel">Model *</Label>
                    <Input
                      type="text"
                      id="assetModel"
                      value={currentAsset.model}
                      onChange={(e) => this.handleAssetChange('model', e.target.value)}
                      required
                    />
                  </FormGroup>
                </Col>
                <Col md={6}>
                  <FormGroup>
                    <Label for="serialNumber">Serial Number *</Label>
                    <Input
                      type="text"
                      id="serialNumber"
                      value={currentAsset.serialNumber}
                      onChange={(e) => this.handleAssetChange('serialNumber', e.target.value)}
                      required
                    />
                  </FormGroup>
                </Col>
              </Row>
              
              {/* Row 7: Object Code and Asset Value */}
              <Row>
                <Col md={6}>
                  <FormGroup>
                    <Label for="objectCode">Object Code *</Label>
                    <Input
                      type="select"
                      id="objectCode"
                      value={currentAsset.objectCodeId ? String(currentAsset.objectCodeId) : ''}
                      onChange={(e) => this.handleAssetChange('objectCodeId', e.target.value)}
                      required
                    >
                      <option value="">Select Object Code</option>
                      {objectCodes.map(obj => (
                        <option key={obj.id} value={String(obj.id)}>
                          {obj.code} - {obj.name}
                        </option>
                      ))}
                    </Input>
                  </FormGroup>
                </Col>
                <Col md={6}>
                  <FormGroup>
                    <Label for="assetValue">Asset Value *</Label>
                    <Input
                      type="number"
                      id="assetValue"
                      value={currentAsset.assetValue}
                      onChange={(e) => this.handleAssetChange('assetValue', e.target.value)}
                      min="0"
                      step="0.01"
                      required
                    />
                  </FormGroup>
                </Col>
              </Row>
            </Form>
            
            {/* Assets Grid Section */}
            <hr className="my-4" />
            <div className="mt-4">
              <h6 className="mb-3">
                <i className="fas fa-table me-2"></i>
                All Assets in Receivable Report ({assets.length} items)
              </h6>
              
              {assets.length > 0 ? (
                <div className="table-responsive" style={{ maxHeight: '400px', overflowY: 'auto' }}>
                  <Table responsive striped hover size="sm">
                    <thead className="table-dark sticky-top">
                      <tr>
                        <th>Floor</th>
                        <th>Room</th>
                        <th>Asset Tag</th>
                        <th>Brand</th>
                        <th>Make</th>
                        <th>Model</th>
                        <th>Asset Value</th>
                        <th>Serial Number</th>
                        <th>Unique Tag #</th>
                        <th>Object Code</th>
                        <th>Assigned To</th>
                        <th>County Owned</th>
                        <th>Status</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {assets.map(asset => (
                        <tr key={asset.id}>
                          <td>{asset.floor || 'N/A'}</td>
                          <td>{asset.room || 'N/A'}</td>
                          <td>
                            <code>{asset.assetTag || 'Not Set'}</code>
                          </td>
                          <td>{asset.brand}</td>
                          <td>{asset.make}</td>
                          <td>{asset.model}</td>
                          <td>${(parseFloat(asset.assetValue) || 0).toFixed(2)}</td>
                          <td>{asset.serialNumber}</td>
                          <td>
                            <code>{asset.uniqueTagNumber || 'Not Generated'}</code>
                          </td>
                          <td>{asset.objectCode || 'N/A'}</td>
                          <td>{asset.assignedTo || 'N/A'}</td>
                          <td>
                            {asset.isOwnedByCounty ? (
                              <span className="text-success">
                                ✓ {asset.countyCode ? `(${asset.countyCode})` : ''}
                              </span>
                            ) : (
                              <span className="text-muted">No</span>
                            )}
                          </td>
                          <td>
                            <span className={`badge ${this.getAssetStatusBadgeClass(asset.assetStatus)}`}>
                              {asset.assetStatus || 'Open'}
                            </span>
                          </td>
                          <td>
                            <Button
                              size="sm"
                              color="danger"
                              onClick={() => this.handleDeleteAsset(asset.id)}
                              className="me-1"
                              title="Delete Asset"
                            >
                              <i className="fas fa-trash"></i>
                            </Button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                    <tfoot className="table-info">
                      <tr>
                        <td colSpan="6" className="text-end fw-bold">Total Asset Value:</td>
                        <td className="fw-bold">
                          ${assets.reduce((sum, asset) => sum + (parseFloat(asset.assetValue) || 0), 0).toFixed(2)}
                        </td>
                        <td colSpan="7"></td>
                      </tr>
                    </tfoot>
                  </Table>
                </div>
              ) : (
                <div className="text-center text-muted p-4">
                  <i className="fas fa-info-circle me-2"></i>
                  No assets added yet. Fill out the form above and click "Save Asset" to add your first asset.
                </div>
              )}
            </div>
          </ModalBody>
          <ModalFooter>
            <Button color="secondary" onClick={this.closeAssetModal} disabled={isSaving}>
              <i className="fas fa-times me-2"></i>Cancel
            </Button>
            <Button color="primary" onClick={this.handleSaveAsset} disabled={isSaving}>
              {isSaving ? (
                <>
                  <Spinner size="sm" className="me-2" />
                  Saving...
                </>
              ) : (
                <>
                  <i className="fas fa-save me-2"></i>
                  Save Asset
                </>
              )}
            </Button>
          </ModalFooter>
        </Modal>

        {/* Organization Search Modal */}
        <Modal isOpen={orgSearchModalOpen} toggle={this.closeOrgSearchModal} size="xl">
          <ModalHeader toggle={this.closeOrgSearchModal}>
            <i className="fas fa-search me-2"></i>
            Search Organizations
          </ModalHeader>
          <ModalBody>
            <div className="mb-3">
              <h6>Filter Organizations</h6>
              <Row>
                <Col md={3}>
                  <FormGroup>
                    <Label for="filterOrganization">Organization Code</Label>
                    <Input
                      type="text"
                      id="filterOrganization"
                      value={orgSearchFilters.organization}
                      onChange={(e) => this.handleOrgSearchFilterChange('organization', e.target.value)}
                      placeholder="Filter by code..."
                    />
                  </FormGroup>
                </Col>
                <Col md={3}>
                  <FormGroup>
                    <Label for="filterOrganizationDesc">Organization Name</Label>
                    <Input
                      type="text"
                      id="filterOrganizationDesc"
                      value={orgSearchFilters.organizationDesc}
                      onChange={(e) => this.handleOrgSearchFilterChange('organizationDesc', e.target.value)}
                      placeholder="Filter by name..."
                    />
                  </FormGroup>
                </Col>
                <Col md={3}>
                  <FormGroup>
                    <Label for="filterShortDesc">Short Description</Label>
                    <Input
                      type="text"
                      id="filterShortDesc"
                      value={orgSearchFilters.organizationShortDesc}
                      onChange={(e) => this.handleOrgSearchFilterChange('organizationShortDesc', e.target.value)}
                      placeholder="Filter by short desc..."
                    />
                  </FormGroup>
                </Col>
                <Col md={3}>
                  <FormGroup>
                    <Label for="filterEffectiveStatus">Status</Label>
                    <Input
                      type="select"
                      id="filterEffectiveStatus"
                      value={orgSearchFilters.effectiveStatus}
                      onChange={(e) => this.handleOrgSearchFilterChange('effectiveStatus', e.target.value)}
                    >
                      <option value="">All Status</option>
                      <option value="A">Active</option>
                      <option value="1">Active (1)</option>
                      <option value="I">Inactive</option>
                    </Input>
                  </FormGroup>
                </Col>
              </Row>
            </div>

            <div className="table-responsive" style={{ maxHeight: '500px', overflowY: 'auto' }}>
              <Table responsive striped hover size="sm">
                <thead className="table-dark sticky-top">
                  <tr>
                    <th>Organization Code</th>
                    <th>Organization Name</th>
                    <th>Short Description</th>
                    <th>Long Description</th>
                    <th>Effective Date</th>
                    <th>Status</th>
                    <th>Set ID</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredOrganizations.map(org => (
                    <tr key={org.id}>
                      <td>
                        <code>{org.code}</code>
                      </td>
                      <td>{org.name || 'N/A'}</td>
                      <td>{org.shortDesc || 'N/A'}</td>
                      <td>
                        <div style={{ maxWidth: '200px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                          {org.longDesc || 'N/A'}
                        </div>
                      </td>
                      <td>
                        {org.effectiveDate ? new Date(org.effectiveDate).toLocaleDateString() : 'N/A'}
                      </td>
                      <td>
                        <span className={`badge ${org.effectiveStatus === 'A' || org.effectiveStatus === '1' ? 'bg-success' : 'bg-secondary'}`}>
                          {org.effectiveStatus || 'N/A'}
                        </span>
                      </td>
                      <td>{org.setId || 'N/A'}</td>
                      <td>
                        <Button
                          size="sm"
                          color="primary"
                          onClick={() => this.selectOrganization(org)}
                          title="Select this organization"
                        >
                          <i className="fas fa-check me-1"></i>
                          Select
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </Table>
            </div>

            {filteredOrganizations.length === 0 && (
              <div className="text-center text-muted p-4">
                <i className="fas fa-info-circle me-2"></i>
                No organizations found matching the current filters.
              </div>
            )}
          </ModalBody>
          <ModalFooter>
            <Button color="secondary" onClick={this.closeOrgSearchModal}>
              <i className="fas fa-times me-2"></i>Close
            </Button>
          </ModalFooter>
        </Modal>

        {/* Fund Search Modal */}
        <Modal isOpen={fundSearchModalOpen} toggle={this.closeFundSearchModal} size="xl">
          <ModalHeader toggle={this.closeFundSearchModal}>
            <i className="fas fa-search me-2"></i>
            Search Funds
          </ModalHeader>
          <ModalBody>
            <div className="mb-3">
              <h6>Filter Funds</h6>
              <Row>
                <Col md={3}>
                  <FormGroup>
                    <Label for="filterFund">Fund Code</Label>
                    <Input
                      type="text"
                      id="filterFund"
                      value={fundSearchFilters.fund}
                      onChange={(e) => this.handleFundSearchFilterChange('fund', e.target.value)}
                      placeholder="Filter by code..."
                    />
                  </FormGroup>
                </Col>
                <Col md={3}>
                  <FormGroup>
                    <Label for="filterFundDesc">Fund Name</Label>
                    <Input
                      type="text"
                      id="filterFundDesc"
                      value={fundSearchFilters.fundDesc}
                      onChange={(e) => this.handleFundSearchFilterChange('fundDesc', e.target.value)}
                      placeholder="Filter by name..."
                    />
                  </FormGroup>
                </Col>
                <Col md={3}>
                  <FormGroup>
                    <Label for="filterFundShortDesc">Short Description</Label>
                    <Input
                      type="text"
                      id="filterFundShortDesc"
                      value={fundSearchFilters.fundShortDesc}
                      onChange={(e) => this.handleFundSearchFilterChange('fundShortDesc', e.target.value)}
                      placeholder="Filter by short desc..."
                    />
                  </FormGroup>
                </Col>
                <Col md={3}>
                  <FormGroup>
                    <Label for="filterFundEffectiveStatus">Status</Label>
                    <Input
                      type="select"
                      id="filterFundEffectiveStatus"
                      value={fundSearchFilters.effectiveStatus}
                      onChange={(e) => this.handleFundSearchFilterChange('effectiveStatus', e.target.value)}
                    >
                      <option value="">All Status</option>
                      <option value="A">Active</option>
                      <option value="1">Active (1)</option>
                      <option value="I">Inactive</option>
                    </Input>
                  </FormGroup>
                </Col>
              </Row>
            </div>

            <div className="table-responsive" style={{ maxHeight: '500px', overflowY: 'auto' }}>
              <Table responsive striped hover size="sm">
                <thead className="table-dark sticky-top">
                  <tr>
                    <th>Fund Code</th>
                    <th>Fund Name</th>
                    <th>Short Description</th>
                    <th>Long Description</th>
                    <th>Effective Date</th>
                    <th>Status</th>
                    <th>Set ID</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredFunds.map(fund => (
                    <tr key={fund.id}>
                      <td>
                        <code>{fund.code}</code>
                      </td>
                      <td>{fund.name || 'N/A'}</td>
                      <td>{fund.shortDesc || 'N/A'}</td>
                      <td>
                        <div style={{ maxWidth: '200px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                          {fund.longDesc || 'N/A'}
                        </div>
                      </td>
                      <td>
                        {fund.effectiveDate ? new Date(fund.effectiveDate).toLocaleDateString() : 'N/A'}
                      </td>
                      <td>
                        <span className={`badge ${fund.effectiveStatus === 'A' || fund.effectiveStatus === '1' ? 'bg-success' : 'bg-secondary'}`}>
                          {fund.effectiveStatus || 'N/A'}
                        </span>
                      </td>
                      <td>{fund.setId || 'N/A'}</td>
                      <td>
                        <Button
                          size="sm"
                          color="primary"
                          onClick={() => this.selectFund(fund)}
                          title="Select this fund"
                        >
                          <i className="fas fa-check me-1"></i>
                          Select
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </Table>
            </div>

            {filteredFunds.length === 0 && (
              <div className="text-center text-muted p-4">
                <i className="fas fa-info-circle me-2"></i>
                No funds found matching the current filters.
              </div>
            )}
          </ModalBody>
          <ModalFooter>
            <Button color="secondary" onClick={this.closeFundSearchModal}>
              <i className="fas fa-times me-2"></i>Close
            </Button>
          </ModalFooter>
        </Modal>

        {/* OA1 Search Modal */}
        <Modal isOpen={oa1SearchModalOpen} toggle={this.closeOa1SearchModal} size="xl">
          <ModalHeader toggle={this.closeOa1SearchModal}>
            <i className="fas fa-search me-2"></i>
            Search OA1s
          </ModalHeader>
          <ModalBody>
            <div className="mb-3">
              <h6>Filter OA1s</h6>
              <Row>
                <Col md={3}>
                  <FormGroup>
                    <Label for="filterOa1">OA1 Code</Label>
                    <Input
                      type="text"
                      id="filterOa1"
                      value={oa1SearchFilters.oa1}
                      onChange={(e) => this.handleOa1SearchFilterChange('oa1', e.target.value)}
                      placeholder="Filter by code..."
                    />
                  </FormGroup>
                </Col>
                <Col md={3}>
                  <FormGroup>
                    <Label for="filterOa1Desc">OA1 Name</Label>
                    <Input
                      type="text"
                      id="filterOa1Desc"
                      value={oa1SearchFilters.oa1Desc}
                      onChange={(e) => this.handleOa1SearchFilterChange('oa1Desc', e.target.value)}
                      placeholder="Filter by name..."
                    />
                  </FormGroup>
                </Col>
                <Col md={3}>
                  <FormGroup>
                    <Label for="filterOa1ShortDesc">Short Description</Label>
                    <Input
                      type="text"
                      id="filterOa1ShortDesc"
                      value={oa1SearchFilters.oa1ShortDesc}
                      onChange={(e) => this.handleOa1SearchFilterChange('oa1ShortDesc', e.target.value)}
                      placeholder="Filter by short desc..."
                    />
                  </FormGroup>
                </Col>
                <Col md={3}>
                  <FormGroup>
                    <Label for="filterOa1EffectiveStatus">Status</Label>
                    <Input
                      type="select"
                      id="filterOa1EffectiveStatus"
                      value={oa1SearchFilters.effectiveStatus}
                      onChange={(e) => this.handleOa1SearchFilterChange('effectiveStatus', e.target.value)}
                    >
                      <option value="">All Status</option>
                      <option value="A">Active</option>
                      <option value="1">Active (1)</option>
                      <option value="I">Inactive</option>
                    </Input>
                  </FormGroup>
                </Col>
              </Row>
            </div>

            <div className="table-responsive" style={{ maxHeight: '500px', overflowY: 'auto' }}>
              <Table responsive striped hover size="sm">
                <thead className="table-dark sticky-top">
                  <tr>
                    <th>OA1 Code</th>
                    <th>OA1 Name</th>
                    <th>Short Description</th>
                    <th>Long Description</th>
                    <th>Effective Date</th>
                    <th>Status</th>
                    <th>Set ID</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredOa1s.map(oa1 => (
                    <tr key={oa1.id}>
                      <td>
                        <code>{oa1.code}</code>
                      </td>
                      <td>{oa1.name || 'N/A'}</td>
                      <td>{oa1.shortDesc || 'N/A'}</td>
                      <td>
                        <div style={{ maxWidth: '200px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                          {oa1.longDesc || 'N/A'}
                        </div>
                      </td>
                      <td>
                        {oa1.effectiveDate ? new Date(oa1.effectiveDate).toLocaleDateString() : 'N/A'}
                      </td>
                      <td>
                        <span className={`badge ${oa1.effectiveStatus === 'A' || oa1.effectiveStatus === '1' ? 'bg-success' : 'bg-secondary'}`}>
                          {oa1.effectiveStatus || 'N/A'}
                        </span>
                      </td>
                      <td>{oa1.setId || 'N/A'}</td>
                      <td>
                        <Button
                          size="sm"
                          color="primary"
                          onClick={() => this.selectOa1(oa1)}
                          title="Select this OA1"
                        >
                          <i className="fas fa-check me-1"></i>
                          Select
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </Table>
            </div>

            {filteredOa1s.length === 0 && (
              <div className="text-center text-muted p-4">
                <i className="fas fa-info-circle me-2"></i>
                No OA1s found matching the current filters.
              </div>
            )}
          </ModalBody>
          <ModalFooter>
            <Button color="secondary" onClick={this.closeOa1SearchModal}>
              <i className="fas fa-times me-2"></i>Close
            </Button>
          </ModalFooter>
        </Modal>
      </Container>
    );
  }
}
