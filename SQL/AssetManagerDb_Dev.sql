AssetManagerDb_Dev:
--====================================================================
--1. All Receiving Reports
--====================================================================
SELECT                                                      
rr.Id,
rr.RRStatus, 
rr.OrderStatus,
rr.CreatedBy,
rr.CreatedDate,
rr.ModifiedDate,
rr.CompletedDate,
--l.LocationName,
COUNT(a.Id) AS AssetCount,
SUM(a.AssetValue) AS TotalValue
FROM ReceivableReports rr
LEFT JOIN Locations l ON rr.LocationId = l.Id
LEFT JOIN Assets a ON a.ReceivableReportId = rr.Id
AND   a.IsDeleted = 0
WHERE rr.IsDeleted = 0
GROUP BY 
rr.Id, rr.RRStatus, rr.OrderStatus, rr.CreatedBy,
rr.CreatedDate, rr.ModifiedDate,
rr.CompletedDate--, l.LocationName
ORDER BY rr.CreatedDate DESC
--====================================================================
--- 2. All Assets linked to RRs
--====================================================================
SELECT
      a.Id,
      a.ReceivableReportId,                                   a.Brand,
      a.Make,                                                 a.Model,
      a.SerialNumber,
      a.AssetTag,
      a.AssetValue,                                           a.AssetStatus,
      a.CreatedBy,                                            a.CreatedDate
  FROM Assets a
  WHERE a.IsDeleted = 0
  ORDER BY a.ReceivableReportId, a.Id  
--====================================================================  
---3. RR History (audit trail)
--====================================================================
SELECT
      --h.Id,
      h.ReceivableReportId,
      h.RRStatus,
      h.OriginalCreatedBy,
      h.OriginalCreatedDate                                       
      FROM ReceivableReportHistory h
  ORDER BY h.ReceivableReportId, h.OriginalCreatedDate DESC  
--====================================================================  
---4. Users & Roles (Application Security)
--====================================================================
SELECT
      s.Id,                                                   s.Username,                                             r.Name,
      s.IsActive,                                             s.CreatedBy,
      s.CreatedDate
  FROM ApplicationSecurity s
  JOIN ApplicationRoles r ON s.ApplicationRoleId = r.Id
  ORDER BY r.Name, s.Username 
--====================================================================
---  5. Full RR detail — assets joined to their RR
--====================================================================
SELECT
      rr.Id AS RR_Id,                                         rr.RRStatus,                                            rr.OrderStatus,
      rr.CreatedBy,                                           a.Id AS Asset_Id,                                       a.Brand,
      a.Make,                                                 a.Model,
      a.SerialNumber,
      a.AssetValue,
      a.AssetStatus                                       FROM ReceivableReports rr
  LEFT JOIN Assets a ON a.ReceivableReportId = rr.Id AND   a.IsDeleted = 0
  WHERE rr.IsDeleted = 0
  ORDER BY rr.Id, a.Id