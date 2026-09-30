SET DEFINE OFF;
SET SQLBLANKLINES ON;

/* =========================================================
   SALES & PURCHASE MANAGEMENT SYSTEM
   STEP 6 — COMPLETE ORACLE STORED PROCEDURES
   Oracle Database / Schema: SALESPURCHASEDB
   ========================================================= */


-- ==========================================
-- 1. EMPLOYEE STORED PROCEDURES
-- ==========================================

CREATE OR REPLACE PROCEDURE SP_EMPLOYEE_INSERT (
    p_EmployeeName    IN  tblEmployees.EmployeeName%TYPE,
    p_UserName        IN  tblEmployees.UserName%TYPE,
    p_Password        IN  tblEmployees.Password%TYPE,
    p_UserType        IN  tblEmployees.UserType%TYPE,
    p_Role            IN  tblEmployees.Role%TYPE,
    p_JobTitle        IN  tblEmployees.JobTitle%TYPE,
    p_Email           IN  tblEmployees.Email%TYPE,
    p_Phone           IN  tblEmployees.Phone%TYPE,
    p_HomePhone       IN  tblEmployees.HomePhone%TYPE,
    p_Address         IN  tblEmployees.Address%TYPE,
    p_City            IN  tblEmployees.City%TYPE,
    p_StateProvince   IN  tblEmployees.StateProvince%TYPE,
    p_ZipPostalCode   IN  tblEmployees.ZipPostalCode%TYPE,
    p_CountryRegion   IN  tblEmployees.CountryRegion%TYPE,
    p_EmployeeID      OUT NUMBER
)
AS
BEGIN
    INSERT INTO tblEmployees (
        EmployeeName, UserName, Password, UserType, Role, JobTitle,
        Email, Phone, HomePhone, Address, City, StateProvince, ZipPostalCode, CountryRegion
    )
    VALUES (
        p_EmployeeName, p_UserName, p_Password, p_UserType, p_Role, p_JobTitle,
        p_Email, p_Phone, p_HomePhone, p_Address, p_City, p_StateProvince, p_ZipPostalCode, p_CountryRegion
    )
    RETURNING EmployeeID INTO p_EmployeeID;
END SP_EMPLOYEE_INSERT;
/

CREATE OR REPLACE PROCEDURE SP_EMPLOYEE_SELECT_ALL (
    p_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            EmployeeID,
            EmployeeName,
            UserName,
            UserType,
            Role,
            JobTitle,
            Email,
            Phone,
            HomePhone,
            Address,
            City,
            StateProvince,
            ZipPostalCode,
            CountryRegion
        FROM tblEmployees
        ORDER BY EmployeeID ASC;
END SP_EMPLOYEE_SELECT_ALL;
/

CREATE OR REPLACE PROCEDURE SP_EMPLOYEE_SELECT_BY_ID (
    p_EmployeeID IN  tblEmployees.EmployeeID%TYPE,
    p_cursor     OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            EmployeeID,
            EmployeeName,
            UserName,
            UserType,
            Role,
            JobTitle,
            Email,
            Phone,
            HomePhone,
            Address,
            City,
            StateProvince,
            ZipPostalCode,
            CountryRegion
        FROM tblEmployees
        WHERE EmployeeID = p_EmployeeID;
END SP_EMPLOYEE_SELECT_BY_ID;
/

CREATE OR REPLACE PROCEDURE SP_EMPLOYEE_UPDATE (
    p_EmployeeID      IN tblEmployees.EmployeeID%TYPE,
    p_EmployeeName    IN tblEmployees.EmployeeName%TYPE,
    p_UserName        IN tblEmployees.UserName%TYPE,
    p_Password        IN tblEmployees.Password%TYPE,
    p_UserType        IN tblEmployees.UserType%TYPE,
    p_Role            IN tblEmployees.Role%TYPE,
    p_JobTitle        IN tblEmployees.JobTitle%TYPE,
    p_Email           IN tblEmployees.Email%TYPE,
    p_Phone           IN tblEmployees.Phone%TYPE,
    p_HomePhone       IN tblEmployees.HomePhone%TYPE,
    p_Address         IN tblEmployees.Address%TYPE,
    p_City            IN tblEmployees.City%TYPE,
    p_StateProvince   IN tblEmployees.StateProvince%TYPE,
    p_ZipPostalCode   IN tblEmployees.ZipPostalCode%TYPE,
    p_CountryRegion   IN tblEmployees.CountryRegion%TYPE
)
AS
BEGIN
    UPDATE tblEmployees
    SET EmployeeName  = p_EmployeeName,
        UserName      = p_UserName,
        Password      = CASE 
                            WHEN p_Password IS NOT NULL AND LENGTH(TRIM(p_Password)) > 0 
                            THEN p_Password 
                            ELSE Password 
                        END,
        UserType      = p_UserType,
        Role          = p_Role,
        JobTitle      = p_JobTitle,
        Email         = p_Email,
        Phone         = p_Phone,
        HomePhone     = p_HomePhone,
        Address       = p_Address,
        City          = p_City,
        StateProvince = p_StateProvince,
        ZipPostalCode = p_ZipPostalCode,
        CountryRegion = p_CountryRegion
    WHERE EmployeeID  = p_EmployeeID;

    IF SQL%ROWCOUNT = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Employee not found with ID: ' || p_EmployeeID);
    END IF;
END SP_EMPLOYEE_UPDATE;
/

CREATE OR REPLACE PROCEDURE SP_EMPLOYEE_DELETE (
    p_EmployeeID IN tblEmployees.EmployeeID%TYPE
)
AS
BEGIN
    DELETE FROM tblEmployees
    WHERE EmployeeID = p_EmployeeID;

    IF SQL%ROWCOUNT = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Employee not found with ID: ' || p_EmployeeID);
    END IF;
END SP_EMPLOYEE_DELETE;
/


-- ==========================================
-- 2. MEMBER STORED PROCEDURES
-- ==========================================

CREATE OR REPLACE PROCEDURE SP_MEMBER_INSERT (
    p_MemberName IN  tblMembers.MemberName%TYPE,
    p_Discount   IN  tblMembers.Discount%TYPE,
    p_MemberID   OUT NUMBER
)
AS
BEGIN
    INSERT INTO tblMembers (
        MemberName, Discount
    )
    VALUES (
        p_MemberName, p_Discount
    )
    RETURNING MemberID INTO p_MemberID;
END SP_MEMBER_INSERT;
/

CREATE OR REPLACE PROCEDURE SP_MEMBER_SELECT_ALL (
    p_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            MemberID,
            MemberName,
            Discount
        FROM tblMembers
        ORDER BY MemberID ASC;
END SP_MEMBER_SELECT_ALL;
/

CREATE OR REPLACE PROCEDURE SP_MEMBER_SELECT_BY_ID (
    p_MemberID IN  tblMembers.MemberID%TYPE,
    p_cursor   OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            MemberID,
            MemberName,
            Discount
        FROM tblMembers
        WHERE MemberID = p_MemberID;
END SP_MEMBER_SELECT_BY_ID;
/

CREATE OR REPLACE PROCEDURE SP_MEMBER_UPDATE (
    p_MemberID   IN tblMembers.MemberID%TYPE,
    p_MemberName IN tblMembers.MemberName%TYPE,
    p_Discount   IN tblMembers.Discount%TYPE
)
AS
BEGIN
    UPDATE tblMembers
    SET MemberName = p_MemberName,
        Discount   = p_Discount
    WHERE MemberID = p_MemberID;

    IF SQL%ROWCOUNT = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Member not found with ID: ' || p_MemberID);
    END IF;
END SP_MEMBER_UPDATE;
/

CREATE OR REPLACE PROCEDURE SP_MEMBER_DELETE (
    p_MemberID IN tblMembers.MemberID%TYPE
)
AS
BEGIN
    DELETE FROM tblMembers
    WHERE MemberID = p_MemberID;

    IF SQL%ROWCOUNT = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Member not found with ID: ' || p_MemberID);
    END IF;
END SP_MEMBER_DELETE;
/


-- ==========================================
-- 3. CUSTOMER STORED PROCEDURES
-- ==========================================

CREATE OR REPLACE PROCEDURE SP_CUSTOMER_INSERT (
    p_CustomerName IN  tblCustomers.CustomerName%TYPE,
    p_JobTitle     IN  tblCustomers.JobTitle%TYPE,
    p_Phone        IN  tblCustomers.Phone%TYPE,
    p_Email        IN  tblCustomers.Email%TYPE,
    p_Address      IN  tblCustomers.Address%TYPE,
    p_MemberID     IN  tblCustomers.MemberID%TYPE,
    p_CustomerID   OUT NUMBER
)
AS
BEGIN
    INSERT INTO tblCustomers (
        CustomerName, JobTitle, Phone, Email, Address, MemberID
    )
    VALUES (
        p_CustomerName, p_JobTitle, p_Phone, p_Email, p_Address, p_MemberID
    )
    RETURNING CustomerID INTO p_CustomerID;
END SP_CUSTOMER_INSERT;
/

CREATE OR REPLACE PROCEDURE SP_CUSTOMER_SELECT_ALL (
    p_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            c.CustomerID,
            c.CustomerName,
            c.JobTitle,
            c.Phone,
            c.Email,
            c.Address,
            c.MemberID,
            m.MemberName,
            m.Discount
        FROM tblCustomers c
        LEFT JOIN tblMembers m
            ON c.MemberID = m.MemberID
        ORDER BY c.CustomerID ASC;
END SP_CUSTOMER_SELECT_ALL;
/

CREATE OR REPLACE PROCEDURE SP_CUSTOMER_SELECT_BY_ID (
    p_CustomerID IN  tblCustomers.CustomerID%TYPE,
    p_cursor     OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            c.CustomerID,
            c.CustomerName,
            c.JobTitle,
            c.Phone,
            c.Email,
            c.Address,
            c.MemberID,
            m.MemberName,
            m.Discount
        FROM tblCustomers c
        LEFT JOIN tblMembers m
            ON c.MemberID = m.MemberID
        WHERE c.CustomerID = p_CustomerID;
END SP_CUSTOMER_SELECT_BY_ID;
/

CREATE OR REPLACE PROCEDURE SP_CUSTOMER_UPDATE (
    p_CustomerID   IN tblCustomers.CustomerID%TYPE,
    p_CustomerName IN tblCustomers.CustomerName%TYPE,
    p_JobTitle     IN tblCustomers.JobTitle%TYPE,
    p_Phone        IN tblCustomers.Phone%TYPE,
    p_Email        IN tblCustomers.Email%TYPE,
    p_Address      IN tblCustomers.Address%TYPE,
    p_MemberID     IN tblCustomers.MemberID%TYPE
)
AS
BEGIN
    UPDATE tblCustomers
    SET CustomerName = p_CustomerName,
        JobTitle     = p_JobTitle,
        Phone        = p_Phone,
        Email        = p_Email,
        Address      = p_Address,
        MemberID     = p_MemberID
    WHERE CustomerID = p_CustomerID;

    IF SQL%ROWCOUNT = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Customer not found with ID: ' || p_CustomerID);
    END IF;
END SP_CUSTOMER_UPDATE;
/

CREATE OR REPLACE PROCEDURE SP_CUSTOMER_DELETE (
    p_CustomerID IN tblCustomers.CustomerID%TYPE
)
AS
BEGIN
    DELETE FROM tblCustomers
    WHERE CustomerID = p_CustomerID;

    IF SQL%ROWCOUNT = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Customer not found with ID: ' || p_CustomerID);
    END IF;
END SP_CUSTOMER_DELETE;
/


-- ==========================================
-- 4. CATEGORY STORED PROCEDURES
-- ==========================================

CREATE OR REPLACE PROCEDURE SP_CATEGORY_INSERT (
    p_CategoryName IN  tblCategories.CategoryName%TYPE,
    p_Description  IN  tblCategories.Description%TYPE,
    p_CreatedBy    IN  tblCategories.CreatedBy%TYPE,
    p_CategoryID   OUT NUMBER
)
AS
BEGIN
    INSERT INTO tblCategories (
        CategoryName, Description, CreatedDate, CreatedBy
    )
    VALUES (
        p_CategoryName, p_Description, CURRENT_TIMESTAMP, p_CreatedBy
    )
    RETURNING CategoryID INTO p_CategoryID;
END SP_CATEGORY_INSERT;
/

CREATE OR REPLACE PROCEDURE SP_CATEGORY_SELECT_ALL (
    p_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            c.CategoryID,
            c.CategoryName,
            c.Description,
            c.CreatedDate,
            c.CreatedBy,
            e1.EmployeeName AS CreatedByName,
            c.UpdateDate,
            c.UpdateBy,
            e2.EmployeeName AS UpdatedByName
        FROM tblCategories c
        LEFT JOIN tblEmployees e1
            ON c.CreatedBy = e1.EmployeeID
        LEFT JOIN tblEmployees e2
            ON c.UpdateBy = e2.EmployeeID
        ORDER BY c.CategoryID ASC;
END SP_CATEGORY_SELECT_ALL;
/

CREATE OR REPLACE PROCEDURE SP_CATEGORY_SELECT_BY_ID (
    p_CategoryID IN  tblCategories.CategoryID%TYPE,
    p_cursor     OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            c.CategoryID,
            c.CategoryName,
            c.Description,
            c.CreatedDate,
            c.CreatedBy,
            e1.EmployeeName AS CreatedByName,
            c.UpdateDate,
            c.UpdateBy,
            e2.EmployeeName AS UpdatedByName
        FROM tblCategories c
        LEFT JOIN tblEmployees e1
            ON c.CreatedBy = e1.EmployeeID
        LEFT JOIN tblEmployees e2
            ON c.UpdateBy = e2.EmployeeID
        WHERE c.CategoryID = p_CategoryID;
END SP_CATEGORY_SELECT_BY_ID;
/

CREATE OR REPLACE PROCEDURE SP_CATEGORY_UPDATE (
    p_CategoryID   IN tblCategories.CategoryID%TYPE,
    p_CategoryName IN tblCategories.CategoryName%TYPE,
    p_Description  IN tblCategories.Description%TYPE,
    p_UpdateBy     IN tblCategories.UpdateBy%TYPE
)
AS
BEGIN
    UPDATE tblCategories
    SET CategoryName = p_CategoryName,
        Description  = p_Description,
        UpdateDate   = CURRENT_TIMESTAMP,
        UpdateBy     = p_UpdateBy
    WHERE CategoryID = p_CategoryID;

    IF SQL%ROWCOUNT = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Category not found with ID: ' || p_CategoryID);
    END IF;
END SP_CATEGORY_UPDATE;
/

CREATE OR REPLACE PROCEDURE SP_CATEGORY_DELETE (
    p_CategoryID IN tblCategories.CategoryID%TYPE
)
AS
BEGIN
    DELETE FROM tblCategories
    WHERE CategoryID = p_CategoryID;

    IF SQL%ROWCOUNT = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Category not found with ID: ' || p_CategoryID);
    END IF;
END SP_CATEGORY_DELETE;
/


-- ==========================================
-- 5. PRODUCT STORED PROCEDURES
-- ==========================================

CREATE OR REPLACE PROCEDURE SP_PRODUCT_INSERT (
    p_ProductName  IN  tblProducts.ProductName%TYPE,
    p_Description  IN  tblProducts.Description%TYPE,
    p_CategoryID   IN  tblProducts.CategoryID%TYPE,
    p_Barcode      IN  tblProducts.Barcode%TYPE,
    p_ExpiryDate   IN  tblProducts.ExpiryDate%TYPE,
    p_Qty          IN  tblProducts.Qty%TYPE,
    p_UnitPriceIn  IN  tblProducts.UnitPriceIn%TYPE,
    p_UnitPriceOut IN  tblProducts.UnitPriceOut%TYPE,
    p_ProductID    OUT NUMBER
)
AS
BEGIN
    INSERT INTO tblProducts (
        ProductName, Description, CategoryID, Barcode, ExpiryDate,
        Qty, UnitPriceIn, UnitPriceOut, isDeleted
    )
    VALUES (
        p_ProductName, p_Description, p_CategoryID, p_Barcode, p_ExpiryDate,
        p_Qty, p_UnitPriceIn, p_UnitPriceOut, 0
    )
    RETURNING ProductID INTO p_ProductID;
END SP_PRODUCT_INSERT;
/

CREATE OR REPLACE PROCEDURE SP_PRODUCT_SELECT_ALL (
    p_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            p.ProductID,
            p.ProductName,
            p.Description,
            p.CategoryID,
            c.CategoryName,
            p.Barcode,
            p.ExpiryDate,
            p.Qty,
            p.UnitPriceIn,
            p.UnitPriceOut,
            p.isDeleted
        FROM tblProducts p
        LEFT JOIN tblCategories c
            ON p.CategoryID = c.CategoryID
        WHERE p.isDeleted = 0
        ORDER BY p.ProductID ASC;
END SP_PRODUCT_SELECT_ALL;
/

CREATE OR REPLACE PROCEDURE SP_PRODUCT_SELECT_BY_ID (
    p_ProductID IN  tblProducts.ProductID%TYPE,
    p_cursor    OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            p.ProductID,
            p.ProductName,
            p.Description,
            p.CategoryID,
            c.CategoryName,
            p.Barcode,
            p.ExpiryDate,
            p.Qty,
            p.UnitPriceIn,
            p.UnitPriceOut,
            p.isDeleted
        FROM tblProducts p
        LEFT JOIN tblCategories c
            ON p.CategoryID = c.CategoryID
        WHERE p.ProductID = p_ProductID;
END SP_PRODUCT_SELECT_BY_ID;
/

CREATE OR REPLACE PROCEDURE SP_PRODUCT_UPDATE (
    p_ProductID    IN tblProducts.ProductID%TYPE,
    p_ProductName  IN tblProducts.ProductName%TYPE,
    p_Description  IN tblProducts.Description%TYPE,
    p_CategoryID   IN tblProducts.CategoryID%TYPE,
    p_Barcode      IN tblProducts.Barcode%TYPE,
    p_ExpiryDate   IN tblProducts.ExpiryDate%TYPE,
    p_Qty          IN tblProducts.Qty%TYPE,
    p_UnitPriceIn  IN tblProducts.UnitPriceIn%TYPE,
    p_UnitPriceOut IN tblProducts.UnitPriceOut%TYPE
)
AS
BEGIN
    UPDATE tblProducts
    SET ProductName   = p_ProductName,
        Description   = p_Description,
        CategoryID    = p_CategoryID,
        Barcode       = p_Barcode,
        ExpiryDate    = p_ExpiryDate,
        Qty           = p_Qty,
        UnitPriceIn   = p_UnitPriceIn,
        UnitPriceOut  = p_UnitPriceOut
    WHERE ProductID   = p_ProductID;

    IF SQL%ROWCOUNT = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Product not found with ID: ' || p_ProductID);
    END IF;
END SP_PRODUCT_UPDATE;
/

CREATE OR REPLACE PROCEDURE SP_PRODUCT_DELETE (
    p_ProductID IN tblProducts.ProductID%TYPE
)
AS
BEGIN
    -- Soft delete product
    UPDATE tblProducts
    SET isDeleted = 1
    WHERE ProductID = p_ProductID;

    IF SQL%ROWCOUNT = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Product not found with ID: ' || p_ProductID);
    END IF;
END SP_PRODUCT_DELETE;
/

CREATE OR REPLACE PROCEDURE SP_PRODUCT_RESTORE (
    p_ProductID IN tblProducts.ProductID%TYPE
)
AS
BEGIN
    UPDATE tblProducts
    SET isDeleted = 0
    WHERE ProductID = p_ProductID;

    IF SQL%ROWCOUNT = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Product not found with ID: ' || p_ProductID);
    END IF;
END SP_PRODUCT_RESTORE;
/


-- ==========================================
-- 6. SUPPLIER STORED PROCEDURES
-- ==========================================

CREATE OR REPLACE PROCEDURE SP_SUPPLIER_INSERT (
    p_SupplierName IN  tblSuppliers.SupplierName%TYPE,
    p_JobTitle     IN  tblSuppliers.JobTitle%TYPE,
    p_CompanyName  IN  tblSuppliers.CompanyName%TYPE,
    p_Phone        IN  tblSuppliers.Phone%TYPE,
    p_Email        IN  tblSuppliers.Email%TYPE,
    p_Address      IN  tblSuppliers.Address%TYPE,
    p_Country      IN  tblSuppliers.Country%TYPE,
    p_Website      IN  tblSuppliers.Website%TYPE,
    p_SupplierID   OUT NUMBER
)
AS
BEGIN
    INSERT INTO tblSuppliers (
        SupplierName, JobTitle, CompanyName, Phone, Email, Address, Country, Website
    )
    VALUES (
        p_SupplierName, p_JobTitle, p_CompanyName, p_Phone, p_Email, p_Address, p_Country, p_Website
    )
    RETURNING SupplierID INTO p_SupplierID;
END SP_SUPPLIER_INSERT;
/

CREATE OR REPLACE PROCEDURE SP_SUPPLIER_SELECT_ALL (
    p_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            SupplierID,
            SupplierName,
            JobTitle,
            CompanyName,
            Phone,
            Email,
            Address,
            Country,
            Website
        FROM tblSuppliers
        ORDER BY SupplierID ASC;
END SP_SUPPLIER_SELECT_ALL;
/

CREATE OR REPLACE PROCEDURE SP_SUPPLIER_SELECT_BY_ID (
    p_SupplierID IN  tblSuppliers.SupplierID%TYPE,
    p_cursor     OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            SupplierID,
            SupplierName,
            JobTitle,
            CompanyName,
            Phone,
            Email,
            Address,
            Country,
            Website
        FROM tblSuppliers
        WHERE SupplierID = p_SupplierID;
END SP_SUPPLIER_SELECT_BY_ID;
/

CREATE OR REPLACE PROCEDURE SP_SUPPLIER_UPDATE (
    p_SupplierID   IN tblSuppliers.SupplierID%TYPE,
    p_SupplierName IN tblSuppliers.SupplierName%TYPE,
    p_JobTitle     IN tblSuppliers.JobTitle%TYPE,
    p_CompanyName  IN tblSuppliers.CompanyName%TYPE,
    p_Phone        IN tblSuppliers.Phone%TYPE,
    p_Email        IN tblSuppliers.Email%TYPE,
    p_Address      IN tblSuppliers.Address%TYPE,
    p_Country      IN tblSuppliers.Country%TYPE,
    p_Website      IN tblSuppliers.Website%TYPE
)
AS
BEGIN
    UPDATE tblSuppliers
    SET SupplierName = p_SupplierName,
        JobTitle     = p_JobTitle,
        CompanyName  = p_CompanyName,
        Phone        = p_Phone,
        Email        = p_Email,
        Address      = p_Address,
        Country      = p_Country,
        Website      = p_Website
    WHERE SupplierID = p_SupplierID;

    IF SQL%ROWCOUNT = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Supplier not found with ID: ' || p_SupplierID);
    END IF;
END SP_SUPPLIER_UPDATE;
/

CREATE OR REPLACE PROCEDURE SP_SUPPLIER_DELETE (
    p_SupplierID IN tblSuppliers.SupplierID%TYPE
)
AS
BEGIN
    DELETE FROM tblSuppliers
    WHERE SupplierID = p_SupplierID;

    IF SQL%ROWCOUNT = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Supplier not found with ID: ' || p_SupplierID);
    END IF;
END SP_SUPPLIER_DELETE;
/


-- ==========================================
-- 7. SHIPPER STORED PROCEDURES
-- ==========================================

CREATE OR REPLACE PROCEDURE SP_SHIPPER_INSERT (
    p_CompanyName   IN  tblShippers.CompanyName%TYPE,
    p_ContactName   IN  tblShippers.ContactName%TYPE,
    p_ContactNumber IN  tblShippers.ContactNumber%TYPE,
    p_Email         IN  tblShippers.Email%TYPE,
    p_Address       IN  tblShippers.Address%TYPE,
    p_Country       IN  tblShippers.Country%TYPE,
    p_ShipperID     OUT NUMBER
)
AS
BEGIN
    INSERT INTO tblShippers (
        CompanyName, ContactName, ContactNumber, Email, Address, Country
    )
    VALUES (
        p_CompanyName, p_ContactName, p_ContactNumber, p_Email, p_Address, p_Country
    )
    RETURNING ShipperID INTO p_ShipperID;
END SP_SHIPPER_INSERT;
/

CREATE OR REPLACE PROCEDURE SP_SHIPPER_SELECT_ALL (
    p_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            ShipperID,
            CompanyName,
            ContactName,
            ContactNumber,
            Email,
            Address,
            Country
        FROM tblShippers
        ORDER BY ShipperID ASC;
END SP_SHIPPER_SELECT_ALL;
/

CREATE OR REPLACE PROCEDURE SP_SHIPPER_SELECT_BY_ID (
    p_ShipperID IN  tblShippers.ShipperID%TYPE,
    p_cursor    OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            ShipperID,
            CompanyName,
            ContactName,
            ContactNumber,
            Email,
            Address,
            Country
        FROM tblShippers
        WHERE ShipperID = p_ShipperID;
END SP_SHIPPER_SELECT_BY_ID;
/

CREATE OR REPLACE PROCEDURE SP_SHIPPER_UPDATE (
    p_ShipperID     IN tblShippers.ShipperID%TYPE,
    p_CompanyName   IN tblShippers.CompanyName%TYPE,
    p_ContactName   IN tblShippers.ContactName%TYPE,
    p_ContactNumber IN tblShippers.ContactNumber%TYPE,
    p_Email         IN tblShippers.Email%TYPE,
    p_Address       IN tblShippers.Address%TYPE,
    p_Country       IN tblShippers.Country%TYPE
)
AS
BEGIN
    UPDATE tblShippers
    SET CompanyName   = p_CompanyName,
        ContactName   = p_ContactName,
        ContactNumber = p_ContactNumber,
        Email         = p_Email,
        Address       = p_Address,
        Country       = p_Country
    WHERE ShipperID   = p_ShipperID;

    IF SQL%ROWCOUNT = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Shipper not found with ID: ' || p_ShipperID);
    END IF;
END SP_SHIPPER_UPDATE;
/

CREATE OR REPLACE PROCEDURE SP_SHIPPER_DELETE (
    p_ShipperID IN tblShippers.ShipperID%TYPE
)
AS
BEGIN
    DELETE FROM tblShippers
    WHERE ShipperID = p_ShipperID;

    IF SQL%ROWCOUNT = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Shipper not found with ID: ' || p_ShipperID);
    END IF;
END SP_SHIPPER_DELETE;
/


-- ==========================================
-- 8. SALES STORED PROCEDURES
-- ==========================================

CREATE OR REPLACE PROCEDURE SP_SALE_INSERT (
    p_EmployeeID IN  tblSales.EmployeeID%TYPE,
    p_CustomerID IN  tblSales.CustomerID%TYPE,
    p_SalesID    OUT NUMBER
)
AS
BEGIN
    INSERT INTO tblSales (
        EmployeeID, CustomerID, SalesDate
    )
    VALUES (
        p_EmployeeID, p_CustomerID, CURRENT_TIMESTAMP
    )
    RETURNING SalesID INTO p_SalesID;
END SP_SALE_INSERT;
/

CREATE OR REPLACE PROCEDURE SP_SALE_SELECT_ALL (
    p_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            s.SalesID,
            s.SalesDate,
            s.EmployeeID,
            e.EmployeeName,
            s.CustomerID,
            c.CustomerName
        FROM tblSales s
        LEFT JOIN tblEmployees e
            ON s.EmployeeID = e.EmployeeID
        LEFT JOIN tblCustomers c
            ON s.CustomerID = c.CustomerID
        ORDER BY s.SalesID DESC;
END SP_SALE_SELECT_ALL;
/

CREATE OR REPLACE PROCEDURE SP_SALE_SELECT_BY_ID (
    p_SalesID IN  tblSales.SalesID%TYPE,
    p_cursor  OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            s.SalesID,
            s.SalesDate,
            s.EmployeeID,
            e.EmployeeName,
            s.CustomerID,
            c.CustomerName
        FROM tblSales s
        LEFT JOIN tblEmployees e
            ON s.EmployeeID = e.EmployeeID
        LEFT JOIN tblCustomers c
            ON s.CustomerID = c.CustomerID
        WHERE s.SalesID = p_SalesID;
END SP_SALE_SELECT_BY_ID;
/

CREATE OR REPLACE PROCEDURE SP_SALE_DELETE (
    p_SalesID IN tblSales.SalesID%TYPE
)
AS
    v_SaleCount NUMBER := 0;
BEGIN
    -- Verify sale exists
    SELECT COUNT(*) INTO v_SaleCount
    FROM tblSales
    WHERE SalesID = p_SalesID;

    IF v_SaleCount = 0 THEN
        RAISE_APPLICATION_ERROR(-20006, 'Sale not found with ID: ' || p_SalesID);
    END IF;

    -- Step 1: Restore product quantities from tblSalesDetails
    FOR rec IN (
        SELECT ProductID, QtySales
        FROM tblSalesDetails
        WHERE SalesID = p_SalesID
    ) LOOP
        UPDATE tblProducts
        SET Qty = Qty + rec.QtySales
        WHERE ProductID = rec.ProductID;
    END LOOP;

    -- Step 2: Delete details and parent sale
    DELETE FROM tblSalesDetails WHERE SalesID = p_SalesID;
    DELETE FROM tblSales WHERE SalesID = p_SalesID;
END SP_SALE_DELETE;
/


-- ==========================================
-- 9. SALES DETAILS STORED PROCEDURES
-- ==========================================

CREATE OR REPLACE PROCEDURE SP_SALES_DETAIL_INSERT (
    p_SalesID  IN tblSalesDetails.SalesID%TYPE,
    p_ProductID IN tblSalesDetails.ProductID%TYPE,
    p_QtySales  IN tblSalesDetails.QtySales%TYPE
)
AS
    v_CurrentQty  tblProducts.Qty%TYPE;
    v_isDeleted   tblProducts.isDeleted%TYPE;
BEGIN
    -- 1. Validate quantity
    IF p_QtySales IS NULL OR p_QtySales <= 0 THEN
        RAISE_APPLICATION_ERROR(-20004, 'Invalid sales quantity. Quantity must be greater than zero.');
    END IF;

    -- 2. Lock product row and verify existence
    BEGIN
        SELECT Qty, isDeleted
        INTO v_CurrentQty, v_isDeleted
        FROM tblProducts
        WHERE ProductID = p_ProductID
        FOR UPDATE;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RAISE_APPLICATION_ERROR(-20002, 'Product not found with ID: ' || p_ProductID);
    END;

    -- 3. Check if deleted / inactive
    IF v_isDeleted = 1 THEN
        RAISE_APPLICATION_ERROR(-20003, 'Product is deleted/inactive.');
    END IF;

    -- 4. Check available stock
    IF p_QtySales > v_CurrentQty THEN
        RAISE_APPLICATION_ERROR(-20001, 'Insufficient product stock. Available: ' || v_CurrentQty || ', Requested: ' || p_QtySales);
    END IF;

    -- 5. Insert sales detail
    INSERT INTO tblSalesDetails (
        SalesID, ProductID, QtySales
    )
    VALUES (
        p_SalesID, p_ProductID, p_QtySales
    );

    -- 6. Decrease product quantity
    UPDATE tblProducts
    SET Qty = Qty - p_QtySales
    WHERE ProductID = p_ProductID;
END SP_SALES_DETAIL_INSERT;
/

CREATE OR REPLACE PROCEDURE SP_SALES_DETAIL_SELECT (
    p_SalesID IN  tblSalesDetails.SalesID%TYPE,
    p_cursor  OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            sd.SalesID,
            sd.ProductID,
            p.ProductName,
            sd.QtySales,
            p.UnitPriceOut,
            (sd.QtySales * p.UnitPriceOut) AS Subtotal
        FROM tblSalesDetails sd
        INNER JOIN tblProducts p
            ON sd.ProductID = p.ProductID
        WHERE sd.SalesID = p_SalesID
        ORDER BY sd.ProductID ASC;
END SP_SALES_DETAIL_SELECT;
/

CREATE OR REPLACE PROCEDURE SP_SALES_DETAIL_DELETE (
    p_SalesID   IN tblSalesDetails.SalesID%TYPE,
    p_ProductID IN tblSalesDetails.ProductID%TYPE
)
AS
    v_QtySales tblSalesDetails.QtySales%TYPE;
BEGIN
    -- Check if line item exists
    BEGIN
        SELECT QtySales
        INTO v_QtySales
        FROM tblSalesDetails
        WHERE SalesID = p_SalesID AND ProductID = p_ProductID;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RAISE_APPLICATION_ERROR(-20006, 'Sales detail line not found.');
    END;

    -- Restore product stock
    UPDATE tblProducts
    SET Qty = Qty + v_QtySales
    WHERE ProductID = p_ProductID;

    -- Delete line item
    DELETE FROM tblSalesDetails
    WHERE SalesID = p_SalesID AND ProductID = p_ProductID;
END SP_SALES_DETAIL_DELETE;
/


-- ==========================================
-- 10. PURCHASE STORED PROCEDURES
-- ==========================================

CREATE OR REPLACE PROCEDURE SP_PURCHASE_INSERT (
    p_EmployeeID IN  tblPurchase.EmployeeID%TYPE,
    p_SupplierID IN  tblPurchase.SupplierID%TYPE,
    p_ShipperID  IN  tblPurchase.ShipperID%TYPE,
    p_PurchaseID OUT NUMBER
)
AS
BEGIN
    INSERT INTO tblPurchase (
        EmployeeID, SupplierID, ShipperID, PurchaseDate
    )
    VALUES (
        p_EmployeeID, p_SupplierID, p_ShipperID, CURRENT_TIMESTAMP
    )
    RETURNING PurchaseID INTO p_PurchaseID;
END SP_PURCHASE_INSERT;
/

CREATE OR REPLACE PROCEDURE SP_PURCHASE_SELECT_ALL (
    p_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            pu.PurchaseID,
            pu.PurchaseDate,
            pu.EmployeeID,
            e.EmployeeName,
            pu.SupplierID,
            sup.SupplierName,
            pu.ShipperID,
            sh.CompanyName AS ShipperName
        FROM tblPurchase pu
        LEFT JOIN tblEmployees e
            ON pu.EmployeeID = e.EmployeeID
        LEFT JOIN tblSuppliers sup
            ON pu.SupplierID = sup.SupplierID
        LEFT JOIN tblShippers sh
            ON pu.ShipperID = sh.ShipperID
        ORDER BY pu.PurchaseID DESC;
END SP_PURCHASE_SELECT_ALL;
/

CREATE OR REPLACE PROCEDURE SP_PURCHASE_SELECT_BY_ID (
    p_PurchaseID IN  tblPurchase.PurchaseID%TYPE,
    p_cursor     OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            pu.PurchaseID,
            pu.PurchaseDate,
            pu.EmployeeID,
            e.EmployeeName,
            pu.SupplierID,
            sup.SupplierName,
            pu.ShipperID,
            sh.CompanyName AS ShipperName
        FROM tblPurchase pu
        LEFT JOIN tblEmployees e
            ON pu.EmployeeID = e.EmployeeID
        LEFT JOIN tblSuppliers sup
            ON pu.SupplierID = sup.SupplierID
        LEFT JOIN tblShippers sh
            ON pu.ShipperID = sh.ShipperID
        WHERE pu.PurchaseID = p_PurchaseID;
END SP_PURCHASE_SELECT_BY_ID;
/

CREATE OR REPLACE PROCEDURE SP_PURCHASE_DELETE (
    p_PurchaseID IN tblPurchase.PurchaseID%TYPE
)
AS
    v_PurchaseCount NUMBER := 0;
    v_CurrentQty    tblProducts.Qty%TYPE;
BEGIN
    -- Verify purchase exists
    SELECT COUNT(*) INTO v_PurchaseCount
    FROM tblPurchase
    WHERE PurchaseID = p_PurchaseID;

    IF v_PurchaseCount = 0 THEN
        RAISE_APPLICATION_ERROR(-20007, 'Purchase not found with ID: ' || p_PurchaseID);
    END IF;

    -- Step 1: Validate that reversing purchase will not result in negative stock
    FOR rec IN (
        SELECT ProductID, PurchaseQty
        FROM tblPurchaseDetails
        WHERE PurchaseID = p_PurchaseID
    ) LOOP
        SELECT Qty INTO v_CurrentQty
        FROM tblProducts
        WHERE ProductID = rec.ProductID
        FOR UPDATE;

        IF (v_CurrentQty - rec.PurchaseQty) < 0 THEN
            RAISE_APPLICATION_ERROR(-20001, 'Cannot cancel purchase: reversing stock would cause product ID ' 
                                            || rec.ProductID || ' to become negative.');
        END IF;

        UPDATE tblProducts
        SET Qty = Qty - rec.PurchaseQty
        WHERE ProductID = rec.ProductID;
    END LOOP;

    -- Step 2: Delete details and parent purchase
    DELETE FROM tblPurchaseDetails WHERE PurchaseID = p_PurchaseID;
    DELETE FROM tblPurchase WHERE PurchaseID = p_PurchaseID;
END SP_PURCHASE_DELETE;
/


-- ==========================================
-- 11. PURCHASE DETAILS STORED PROCEDURES
-- ==========================================

CREATE OR REPLACE PROCEDURE SP_PURCHASE_DETAIL_INSERT (
    p_PurchaseID  IN tblPurchaseDetails.PurchaseID%TYPE,
    p_ProductID   IN tblPurchaseDetails.ProductID%TYPE,
    p_PurchaseQty IN tblPurchaseDetails.PurchaseQty%TYPE
)
AS
    v_isDeleted tblProducts.isDeleted%TYPE;
BEGIN
    -- 1. Validate quantity
    IF p_PurchaseQty IS NULL OR p_PurchaseQty <= 0 THEN
        RAISE_APPLICATION_ERROR(-20005, 'Invalid purchase quantity. Quantity must be greater than zero.');
    END IF;

    -- 2. Lock product row and verify existence
    BEGIN
        SELECT isDeleted
        INTO v_isDeleted
        FROM tblProducts
        WHERE ProductID = p_ProductID
        FOR UPDATE;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RAISE_APPLICATION_ERROR(-20002, 'Product not found with ID: ' || p_ProductID);
    END;

    -- 3. Check if deleted / inactive
    IF v_isDeleted = 1 THEN
        RAISE_APPLICATION_ERROR(-20003, 'Product is deleted/inactive.');
    END IF;

    -- 4. Insert purchase detail
    INSERT INTO tblPurchaseDetails (
        PurchaseID, ProductID, PurchaseQty
    )
    VALUES (
        p_PurchaseID, p_ProductID, p_PurchaseQty
    );

    -- 5. Increase product stock
    UPDATE tblProducts
    SET Qty = Qty + p_PurchaseQty
    WHERE ProductID = p_ProductID;
END SP_PURCHASE_DETAIL_INSERT;
/

CREATE OR REPLACE PROCEDURE SP_PURCHASE_DETAIL_SELECT (
    p_PurchaseID IN  tblPurchaseDetails.PurchaseID%TYPE,
    p_cursor     OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            pd.PurchaseID,
            pd.ProductID,
            p.ProductName,
            pd.PurchaseQty,
            p.UnitPriceIn,
            (pd.PurchaseQty * p.UnitPriceIn) AS Subtotal
        FROM tblPurchaseDetails pd
        INNER JOIN tblProducts p
            ON pd.ProductID = p.ProductID
        WHERE pd.PurchaseID = p_PurchaseID
        ORDER BY pd.ProductID ASC;
END SP_PURCHASE_DETAIL_SELECT;
/

CREATE OR REPLACE PROCEDURE SP_PURCHASE_DETAIL_DELETE (
    p_PurchaseID IN tblPurchaseDetails.PurchaseID%TYPE,
    p_ProductID  IN tblPurchaseDetails.ProductID%TYPE
)
AS
    v_PurchaseQty tblPurchaseDetails.PurchaseQty%TYPE;
    v_CurrentQty  tblProducts.Qty%TYPE;
BEGIN
    -- Check if line item exists
    BEGIN
        SELECT PurchaseQty
        INTO v_PurchaseQty
        FROM tblPurchaseDetails
        WHERE PurchaseID = p_PurchaseID AND ProductID = p_ProductID;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RAISE_APPLICATION_ERROR(-20007, 'Purchase detail line not found.');
    END;

    -- Lock product and check negative stock
    SELECT Qty INTO v_CurrentQty
    FROM tblProducts
    WHERE ProductID = p_ProductID
    FOR UPDATE;

    IF (v_CurrentQty - v_PurchaseQty) < 0 THEN
        RAISE_APPLICATION_ERROR(-20001, 'Cannot delete purchase detail: stock would become negative.');
    END IF;

    -- Decrease product stock back
    UPDATE tblProducts
    SET Qty = Qty - v_PurchaseQty
    WHERE ProductID = p_ProductID;

    -- Delete line item
    DELETE FROM tblPurchaseDetails
    WHERE PurchaseID = p_PurchaseID AND ProductID = p_ProductID;
END SP_PURCHASE_DETAIL_DELETE;
/

-- ==========================================
-- 12. EMPLOYEE AUTHENTICATION PROCEDURE
-- ==========================================

CREATE OR REPLACE PROCEDURE SP_EMPLOYEE_LOGIN (
    p_UserName IN  tblEmployees.UserName%TYPE,
    p_Password IN  tblEmployees.Password%TYPE,
    p_cursor   OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT
            EmployeeID,
            EmployeeName,
            UserName,
            UserType,
            Role,
            JobTitle,
            Email
        FROM tblEmployees
        WHERE LOWER(UserName) = LOWER(p_UserName)
          AND Password = p_Password;
END SP_EMPLOYEE_LOGIN;
/

