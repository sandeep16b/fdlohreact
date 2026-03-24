/**
 * Service for managing individual assets
 */

const API_BASE_URL = '/api/receivablereport';

/**
 * Save an asset (create new or update existing)
 * @param {number} rrId - Receivable Report ID
 * @param {object} asset - Asset data
 * @returns {Promise<object>} Saved asset with navigation properties
 */
export async function saveAsset(rrId, asset) {
  try {
    // Parse IDs properly - empty string should become null, valid numbers should be parsed
    const parseOptionalId = (value) => {
      if (value === null || value === undefined || value === '') return null;
      const parsed = parseInt(value, 10);
      return isNaN(parsed) ? null : parsed;
    };

    const payload = {
      id: asset.id || 0,
      brand: asset.brand || '',
      make: asset.make || '',
      model: asset.model || '',
      // AssetTag is NOT included - will be generated via grid button only
      serialNumber: asset.serialNumber || '',
      assetValue: parseFloat(asset.assetValue) || 0,
      objectCodeId: parseOptionalId(asset.objectCodeId),
      assetGroupId: parseOptionalId(asset.assetGroupId),
      assetSubGroupId: parseOptionalId(asset.assetSubGroupId),
      assignedTo: asset.assignedTo || '',
      floor: asset.floor || '',
      room: asset.room || '',
      isOwnedByCounty: asset.isOwnedByCounty || false,
      countyId: parseOptionalId(asset.countyId),
      uniqueTagNumber: asset.uniqueTagNumber || '',
      assetStatus: asset.assetStatus || 'Open'
    };

    console.log('🔍 Saving asset with payload:', payload);

    const response = await fetch(`${API_BASE_URL}/${rrId}/asset`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(payload)
    });

    if (!response.ok) {
      const errorText = await response.text();
      let errorMessage = errorText;
      
      try {
        const errorData = JSON.parse(errorText);
        // Extract the most useful error message
        if (errorData.title) {
          errorMessage = errorData.title;
        } else if (errorData.errors) {
          // Validation errors object
          const errorMessages = Object.values(errorData.errors).flat();
          errorMessage = errorMessages.join('; ');
        } else if (typeof errorData === 'string') {
          errorMessage = errorData;
        }
      } catch {
        // If not JSON, use the raw text (might be BadRequest message)
        errorMessage = errorText;
      }
      
      console.error('Asset save failed:', errorMessage);
      throw new Error(errorMessage);
    }

    return await response.json();
  } catch (error) {
    console.error('Error saving asset:', error);
    throw error;
  }
}

/**
 * Get all assets for a receivable report
 * @param {number} rrId - Receivable Report ID
 * @returns {Promise<Array>} List of assets
 */
export async function getAssetsByReceivableReport(rrId) {
  try {
    const response = await fetch(`${API_BASE_URL}/${rrId}/assets`);

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(errorText);
    }

    return await response.json();
  } catch (error) {
    console.error('Error fetching assets:', error);
    throw error;
  }
}

/**
 * Delete an asset
 * @param {number} rrId - Receivable Report ID
 * @param {number} assetId - Asset ID
 * @returns {Promise<void>}
 */
export async function deleteAsset(rrId, assetId) {
  try {
    const response = await fetch(`${API_BASE_URL}/${rrId}/asset/${assetId}`, {
      method: 'DELETE'
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(errorText);
    }
  } catch (error) {
    console.error('Error deleting asset:', error);
    throw error;
  }
}

