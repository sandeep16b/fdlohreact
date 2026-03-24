/**
 * Service layer for Receivable Report API operations
 */

const BASE_URL = '/api/receivablereport';

class ReceivableReportService {
  /**
   * Search receivable reports with filtering and pagination
   */
  async searchReceivableReports(searchCriteria) {
    try {
      const response = await fetch(`${BASE_URL}/search`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(searchCriteria),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error searching receivable reports:', error);
      throw error;
    }
  }

  /**
   * Get all receivable reports
   */
  async getAllReceivableReports() {
    try {
      const response = await fetch(BASE_URL);
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching receivable reports:', error);
      throw error;
    }
  }

  /**
   * Get a specific receivable report by ID
   */
  async getReceivableReportById(id) {
    try {
      const response = await fetch(`${BASE_URL}/${id}`);
      
      if (!response.ok) {
        if (response.status === 404) {
          throw new Error('Receivable report not found');
        }
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error(`Error fetching receivable report ${id}:`, error);
      throw error;
    }
  }

  /**
   * Create a new receivable report
   */
  async createReceivableReport(receivableReportData) {
    try {
      const response = await fetch(BASE_URL, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(receivableReportData),
      });

      if (!response.ok) {
        const errorData = await response.text();
        throw new Error(errorData || `HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error creating receivable report:', error);
      throw error;
    }
  }

  /**
   * Update an existing receivable report
   */
  async updateReceivableReport(id, receivableReportData) {
    try {
      console.log('Updating receivable report with data:', receivableReportData);
      
      const response = await fetch(`${BASE_URL}/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(receivableReportData),
      });

      if (!response.ok) {
        // Read response as text first, then try to parse as JSON
        const errorText = await response.text();
        let errorData;
        try {
          errorData = JSON.parse(errorText);
        } catch {
          errorData = errorText;
        }
        console.error('Update failed with error:', errorData);
        throw new Error(typeof errorData === 'string' ? errorData : JSON.stringify(errorData));
      }

      return await response.json();
    } catch (error) {
      console.error(`Error updating receivable report ${id}:`, error);
      throw error;
    }
  }

  /**
   * Delete a receivable report (soft delete)
   */
  async deleteReceivableReport(id) {
    try {
      const response = await fetch(`${BASE_URL}/${id}`, {
        method: 'DELETE',
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return true;
    } catch (error) {
      console.error(`Error deleting receivable report ${id}:`, error);
      throw error;
    }
  }

  /**
   * Attest tags for a receivable report
   */
  async attestTags(id) {
    try {
      const response = await fetch(`${BASE_URL}/${id}/attest-tags`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error(`Error attesting tags for receivable report ${id}:`, error);
      throw error;
    }
  }

  /**
   * Print tags for a receivable report (placeholder for actual print functionality)
   */
  async printTags(id) {
    try {
      // In a real application, this might generate a PDF or trigger a print dialog
      console.log(`Printing tags for receivable report ${id}`);
      
      // For now, we'll simulate a successful print operation
      return new Promise((resolve) => {
        setTimeout(() => {
          resolve({ message: 'Tags printed successfully', reportId: id });
        }, 1000);
      });
    } catch (error) {
      console.error(`Error printing tags for receivable report ${id}:`, error);
      throw error;
    }
  }

  /**
   * Submit a receivable report (change status from Draft to Submitted)
   */
  async submitReceivableReport(id) {
    try {
      const response = await fetch(`${BASE_URL}/${id}/submit`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        const errorData = await response.text();
        throw new Error(errorData || `HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error(`Error submitting receivable report ${id}:`, error);
      throw error;
    }
  }

  /**
   * Complete a receivable report
   */
  async completeReceivableReport(id) {
    try {
      const response = await fetch(`${BASE_URL}/${id}/complete`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        const errorData = await response.text();
        throw new Error(errorData || `HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error(`Error completing receivable report ${id}:`, error);
      throw error;
    }
  }

  /**
   * Get lookup data for dropdowns
   */
  async getLookupData() {
    try {
      const response = await fetch(`${BASE_URL}/lookup-data`);
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching lookup data:', error);
      throw error;
    }
  }

  /**
   * Get all active organizations from FDW database
   */
  async getOrganizations() {
    try {
      const response = await fetch('/api/organization');
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching organizations:', error);
      throw error;
    }
  }

  /**
   * Get all active funds from FDW database
   */
  async getFunds() {
    try {
      const response = await fetch('/api/fund');
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching funds:', error);
      throw error;
    }
  }

  /**
   * Get all active OA1s from FDW database
   */
  async getOA1s() {
    try {
      const response = await fetch('/api/oa1');
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching OA1s:', error);
      throw error;
    }
  }

  /**
   * Build search criteria object
   */
  buildSearchCriteria({
    organizationId = null,
    organizationCode = null,
    fundId = null,
    fundCode = null,
    oa1Id = null,
    oa1Code = null,
    locationCode = null,
    orderStatus = null,
    rrStatus = null,
    createdDateFrom = null,
    createdDateTo = null,
    createdBy = null,
    includeDeleted = false,
    pageNumber = 1,
    pageSize = 10,
    sortBy = 'CreatedDate',
    sortDirection = 'DESC'
  } = {}) {
    return {
      organizationId,
      organizationCode,
      fundId,
      fundCode,
      oa1Id,
      oa1Code,
      locationCode,
      orderStatus,
      rrStatus,
      createdDateFrom,
      createdDateTo,
      createdBy,
      includeDeleted,
      pageNumber,
      pageSize,
      sortBy,
      sortDirection
    };
  }

  /**
   * Print tag for a specific asset
   */
  async printAssetTag(assetId) {
    try {
      const response = await fetch(`${BASE_URL}/asset/${assetId}/print-tag`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        const errorData = await response.text();
        throw new Error(errorData || `HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error(`Error printing asset tag for asset ${assetId}:`, error);
      throw error;
    }
  }

  /**
   * Attest tag for a specific asset
   */
  async attestAssetTag(assetId) {
    try {
      const response = await fetch(`${BASE_URL}/asset/${assetId}/attest-tag`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        const errorData = await response.text();
        throw new Error(errorData || `HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error(`Error attesting asset tag for asset ${assetId}:`, error);
      throw error;
    }
  }

  /**
   * Build receivable report data object for create/update operations
   */
  buildReceivableReportData({
    organizationId,
    fundId,
    oa1Id,
    locationId = null,
    locationCode = null,
    orderStatus,
    rrStatus = 'Draft',
    addressLine1 = null,
    addressLine2 = null,
    city = null,
    county = null,
    state = null,
    postalCode = null,
    // Procurement method fields
    procurementMethod = null,
    assets = []
  }) {
    const data = {
      organizationId: parseInt(organizationId) || 0,
      fundId: parseInt(fundId) || 0,
      oa1Id: parseInt(oa1Id) || 0,
      locationId: locationId ? parseInt(locationId) : 0,
      locationCode,
      orderStatus,
      rrStatus,
      addressLine1,
      addressLine2,
      city,
      county,
      state,
      postalCode,
      assets: assets.map(asset => {
        // Parse ID carefully - must be a valid integer or default to 0
        // SQL Server int max value is 2,147,483,647
        // Temporary IDs from Date.now() are too large and should be treated as new assets (id: 0)
        const SQL_SERVER_INT_MAX = 2147483647;
        let parsedId = 0;
        
        if (asset.id !== null && asset.id !== undefined && asset.id !== '') {
          const tempId = parseInt(asset.id, 10);
          if (!isNaN(tempId) && tempId > 0 && tempId <= SQL_SERVER_INT_MAX) {
            // Valid existing asset ID
            parsedId = tempId;
          }
          // else: ID is 0, negative, or exceeds SQL Server int max - treat as new asset (id: 0)
        }
        
        return {
        id: parsedId,
        brand: asset.brand || '',
        make: asset.make || '',
        model: asset.model || '',
        assetTag: asset.assetTag || '',
        serialNumber: asset.serialNumber || '',
        assetValue: parseFloat(asset.assetValue) || 0,
        objectCodeId: asset.objectCodeId && !isNaN(parseInt(asset.objectCodeId)) ? parseInt(asset.objectCodeId) : null,
        assignedTo: asset.assignedTo || '',
        floor: asset.floor || '',
        room: asset.room || '',
        isOwnedByCounty: asset.isOwnedByCounty || false,
        countyId: asset.countyId && !isNaN(parseInt(asset.countyId)) ? parseInt(asset.countyId) : null,
        uniqueTagNumber: asset.uniqueTagNumber || '',
        assetStatus: asset.assetStatus || 'Open'
        };
      })
    };

    // DEBUG: Log asset IDs after mapping
    console.log('🔧 Service buildReceivableReportData - Asset IDs:', data.assets.map(a => ({ 
      id: a.id, 
      type: typeof a.id, 
      brand: a.brand 
    })));

    // Add procurement method if provided
    if (procurementMethod) {
      // Format chargeDate properly for API
      let formattedChargeDate = null;
      if (procurementMethod.chargeDate) {
        try {
          if (typeof procurementMethod.chargeDate === 'string' && procurementMethod.chargeDate.trim() !== '') {
            const parsedDate = new Date(procurementMethod.chargeDate);
            if (!isNaN(parsedDate.getTime())) {
              formattedChargeDate = parsedDate.toISOString();
            }
          } else if (procurementMethod.chargeDate instanceof Date) {
            formattedChargeDate = procurementMethod.chargeDate.toISOString();
          }
        } catch (error) {
          console.warn('Error formatting chargeDate:', error);
          formattedChargeDate = null;
        }
      }

      data.procurementMethod = {
        procurementTypeId: procurementMethod.procurementTypeId ? parseInt(procurementMethod.procurementTypeId) : null,
        procurementTypeCode: procurementMethod.procurementTypeCode,
        chargeDate: formattedChargeDate,
        pcardHolderFirstName: procurementMethod.pcardHolderFirstName,
        pcardHolderLastName: procurementMethod.pcardHolderLastName,
        groupId: procurementMethod.groupId,
        purchaseOrderNumber: procurementMethod.purchaseOrderNumber,
        contractNumber: procurementMethod.contractNumber
      };
    }

    return data;
  }
}

// Export a singleton instance
const receivableReportService = new ReceivableReportService();
export default receivableReportService;
