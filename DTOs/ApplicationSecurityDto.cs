namespace ReactAspNetApp.DTOs
{
    /// <summary>
    /// Data Transfer Object for ApplicationRole
    /// </summary>
    public class ApplicationRoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for ApplicationSecurity
    /// </summary>
    public class ApplicationSecurityDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public int ApplicationRoleId { get; set; }
        public string ApplicationRoleName { get; set; } = string.Empty; // For display purposes
        public ApplicationRoleDto? ApplicationRole { get; set; } // Full role details if needed
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Search criteria DTO for ApplicationSecurity
    /// </summary>
    public class ApplicationSecuritySearchDto
    {
        public string? Username { get; set; }
        public int? ApplicationRoleId { get; set; }
        public string? ApplicationRoleName { get; set; } // For searching by role name
        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDateFrom { get; set; }
        public DateTime? UpdatedDateTo { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? IsActive { get; set; }
        public bool IncludeDeleted { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedDate";
        public string SortDirection { get; set; } = "DESC";
    }

    /// <summary>
    /// Paged result wrapper
    /// </summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

