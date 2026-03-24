/**
 * Service layer for Application Security API operations
 */

const BASE_URL = '/api/applicationsecurity';

class ApplicationSecurityService {
  /**
   * Search application security records with filtering and pagination
   */
  async searchApplicationSecurity(searchCriteria) {
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
      console.error('Error searching application security records:', error);
      throw error;
    }
  }

  /**
   * Get all application security records
   */
  async getAllApplicationSecurity() {
    try {
      const response = await fetch(BASE_URL);
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching application security records:', error);
      throw error;
    }
  }

  /**
   * Get a specific application security record by ID
   */
  async getApplicationSecurityById(id) {
    try {
      const response = await fetch(`${BASE_URL}/${id}`);
      
      if (!response.ok) {
        if (response.status === 404) {
          throw new Error('Application security record not found');
        }
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error(`Error fetching application security record ${id}:`, error);
      throw error;
    }
  }

  /**
   * Create a new application security record
   */
  async createApplicationSecurity(data) {
    try {
      const response = await fetch(BASE_URL, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        const errorData = await response.text();
        throw new Error(errorData || `HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error creating application security record:', error);
      throw error;
    }
  }

  /**
   * Update an existing application security record
   */
  async updateApplicationSecurity(id, data) {
    try {
      const response = await fetch(`${BASE_URL}/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
      });

      if (!response.ok) {
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
      console.error(`Error updating application security record ${id}:`, error);
      throw error;
    }
  }

  /**
   * Delete an application security record (soft delete)
   */
  async deleteApplicationSecurity(id) {
    try {
      const response = await fetch(`${BASE_URL}/${id}`, {
        method: 'DELETE',
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return true;
    } catch (error) {
      console.error(`Error deleting application security record ${id}:`, error);
      throw error;
    }
  }

  /**
   * Get available application roles
   */
  async getApplicationRoles() {
    try {
      const response = await fetch(`${BASE_URL}/roles`);
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching application roles:', error);
      throw error;
    }
  }

  /**
   * Build search criteria object
   */
  buildSearchCriteria({
    username = null,
    applicationRoleId = null,
    applicationRoleName = null,
    createdDateFrom = null,
    createdDateTo = null,
    createdBy = null,
    updatedDateFrom = null,
    updatedDateTo = null,
    updatedBy = null,
    isActive = null,
    includeDeleted = false,
    pageNumber = 1,
    pageSize = 10,
    sortBy = 'CreatedDate',
    sortDirection = 'DESC'
  } = {}) {
    return {
      username,
      applicationRoleId,
      applicationRoleName,
      createdDateFrom,
      createdDateTo,
      createdBy,
      updatedDateFrom,
      updatedDateTo,
      updatedBy,
      isActive,
      includeDeleted,
      pageNumber,
      pageSize,
      sortBy,
      sortDirection
    };
  }
}

// Export a singleton instance
const applicationSecurityService = new ApplicationSecurityService();
export default applicationSecurityService;

