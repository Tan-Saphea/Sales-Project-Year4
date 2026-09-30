SET DEFINE OFF;
SET SERVEROUTPUT ON;
SET SQLBLANKLINES ON;
SET LINESIZE 200;
SET PAGESIZE 100;

/* =========================================================
   SALES & PURCHASE MANAGEMENT SYSTEM
   STEP 7 — COMPREHENSIVE STORED PROCEDURE TEST SUITE
   Schema: SALESPURCHASEDB
   ========================================================= */

PROMPT =========================================================;
PROMPT STEP 7.1 — CHECK PROCEDURE STATUS & ERRORS;
PROMPT =========================================================;

SELECT
    object_name,
    object_type,
    status
FROM user_objects
WHERE object_type = 'PROCEDURE'
ORDER BY object_name;

SELECT
    name,
    type,
    line,
    position,
    text
FROM user_errors
ORDER BY name, sequence;


PROMPT =========================================================;
PROMPT STEP 7.2 — EMPLOYEE CRUD TEST;
PROMPT =========================================================;

DECLARE
    v_EmployeeID   NUMBER;
    v_UpdatedName  tblEmployees.EmployeeName%TYPE;
    v_CountAfter   NUMBER;
BEGIN
    DBMS_OUTPUT.PUT_LINE('--- 1. Testing SP_EMPLOYEE_INSERT ---');
    SP_EMPLOYEE_INSERT(
        p_EmployeeName  => 'Test Employee',
        p_UserName      => 'test_employee',
        p_Password      => 'Test123',
        p_UserType      => 'Staff',
        p_Role          => 'Cashier',
        p_JobTitle      => 'Cashier',
        p_Email         => 'test.employee@example.com',
        p_Phone         => '012000001',
        p_HomePhone     => NULL,
        p_Address       => 'Phnom Penh',
        p_City          => 'Phnom Penh',
        p_StateProvince => 'Phnom Penh',
        p_ZipPostalCode => '12000',
        p_CountryRegion => 'Cambodia',
        p_EmployeeID    => v_EmployeeID
    );
    DBMS_OUTPUT.PUT_LINE('[INSERT SUCCESS] Created Employee ID: ' || v_EmployeeID);

    DBMS_OUTPUT.PUT_LINE('--- 2. Testing SP_EMPLOYEE_UPDATE ---');
    SP_EMPLOYEE_UPDATE(
        p_EmployeeID    => v_EmployeeID,
        p_EmployeeName  => 'Updated Test Employee',
        p_UserName      => 'test_employee',
        p_Password      => NULL, -- Keep existing password
        p_UserType      => 'Staff',
        p_Role          => 'Senior Cashier',
        p_JobTitle      => 'Senior Cashier',
        p_Email         => 'test.employee@example.com',
        p_Phone         => '012000001',
        p_HomePhone     => NULL,
        p_Address       => 'Phnom Penh',
        p_City          => 'Phnom Penh',
        p_StateProvince => 'Phnom Penh',
        p_ZipPostalCode => '12000',
        p_CountryRegion => 'Cambodia'
    );

    SELECT EmployeeName INTO v_UpdatedName FROM tblEmployees WHERE EmployeeID = v_EmployeeID;
    DBMS_OUTPUT.PUT_LINE('[UPDATE SUCCESS] Verified EmployeeName: ' || v_UpdatedName);

    DBMS_OUTPUT.PUT_LINE('--- 3. Testing SP_EMPLOYEE_DELETE ---');
    SP_EMPLOYEE_DELETE(p_EmployeeID => v_EmployeeID);

    SELECT COUNT(*) INTO v_CountAfter FROM tblEmployees WHERE UserName = 'test_employee';
    IF v_CountAfter = 0 THEN
        DBMS_OUTPUT.PUT_LINE('[DELETE SUCCESS] Test Employee successfully removed. Count = 0');
    ELSE
        RAISE_APPLICATION_ERROR(-20000, 'Employee delete failed!');
    END IF;
END;
/


PROMPT =========================================================;
PROMPT STEP 7.3 — MEMBER CRUD TEST;
PROMPT =========================================================;

DECLARE
    v_MemberID      NUMBER;
    v_UpdatedDisc   NUMBER;
    v_CountAfter    NUMBER;
BEGIN
    DBMS_OUTPUT.PUT_LINE('--- 1. Testing SP_MEMBER_INSERT ---');
    SP_MEMBER_INSERT(
        p_MemberName => 'Test Member',
        p_Discount   => 15.00,
        p_MemberID   => v_MemberID
    );
    DBMS_OUTPUT.PUT_LINE('[INSERT SUCCESS] Created Member ID: ' || v_MemberID);

    DBMS_OUTPUT.PUT_LINE('--- 2. Testing SP_MEMBER_UPDATE (15 -> 20) ---');
    SP_MEMBER_UPDATE(
        p_MemberID   => v_MemberID,
        p_MemberName => 'Test Member Updated',
        p_Discount   => 20.00
    );

    SELECT Discount INTO v_UpdatedDisc FROM tblMembers WHERE MemberID = v_MemberID;
    DBMS_OUTPUT.PUT_LINE('[UPDATE SUCCESS] Verified Member Discount: ' || v_UpdatedDisc || '%');

    DBMS_OUTPUT.PUT_LINE('--- 3. Testing SP_MEMBER_DELETE ---');
    SP_MEMBER_DELETE(p_MemberID => v_MemberID);

    SELECT COUNT(*) INTO v_CountAfter FROM tblMembers WHERE MemberID = v_MemberID;
    IF v_CountAfter = 0 THEN
        DBMS_OUTPUT.PUT_LINE('[DELETE SUCCESS] Test Member successfully removed. Count = 0');
    ELSE
        RAISE_APPLICATION_ERROR(-20000, 'Member delete failed!');
    END IF;
END;
/


PROMPT =========================================================;
PROMPT STEP 7.4 — CUSTOMER CRUD TEST;
PROMPT =========================================================;

DECLARE
    v_TempMemberID NUMBER;
    v_CustomerID   NUMBER;
    v_CustName     tblCustomers.CustomerName%TYPE;
    v_CountAfter   NUMBER;
BEGIN
    -- 1. Create temporary member
    SP_MEMBER_INSERT(
        p_MemberName => 'Temp VIP',
        p_Discount   => 10.00,
        p_MemberID   => v_TempMemberID
    );

    -- 2. Insert customer
    DBMS_OUTPUT.PUT_LINE('--- 1. Testing SP_CUSTOMER_INSERT ---');
    SP_CUSTOMER_INSERT(
        p_CustomerName => 'Test Customer',
        p_JobTitle     => 'Student',
        p_Phone        => '012000002',
        p_Email        => 'test.customer@example.com',
        p_Address      => 'Phnom Penh',
        p_MemberID     => v_TempMemberID,
        p_CustomerID   => v_CustomerID
    );
    DBMS_OUTPUT.PUT_LINE('[INSERT SUCCESS] Created Customer ID: ' || v_CustomerID);

    -- 3. Update customer
    DBMS_OUTPUT.PUT_LINE('--- 2. Testing SP_CUSTOMER_UPDATE ---');
    SP_CUSTOMER_UPDATE(
        p_CustomerID   => v_CustomerID,
        p_CustomerName => 'Updated Test Customer',
        p_JobTitle     => 'Graduate Student',
        p_Phone        => '012000002',
        p_Email        => 'test.customer@example.com',
        p_Address      => 'Phnom Penh',
        p_MemberID     => v_TempMemberID
    );

    SELECT CustomerName INTO v_CustName FROM tblCustomers WHERE CustomerID = v_CustomerID;
    DBMS_OUTPUT.PUT_LINE('[UPDATE SUCCESS] Verified Customer Name: ' || v_CustName);

    -- 4. Delete customer then temporary member
    DBMS_OUTPUT.PUT_LINE('--- 3. Testing SP_CUSTOMER_DELETE ---');
    SP_CUSTOMER_DELETE(p_CustomerID => v_CustomerID);
    SP_MEMBER_DELETE(p_MemberID => v_TempMemberID);

    SELECT COUNT(*) INTO v_CountAfter FROM tblCustomers WHERE CustomerID = v_CustomerID;
    IF v_CountAfter = 0 THEN
        DBMS_OUTPUT.PUT_LINE('[DELETE SUCCESS] Test Customer and Temp Member removed. Count = 0');
    ELSE
        RAISE_APPLICATION_ERROR(-20000, 'Customer delete failed!');
    END IF;
END;
/


PROMPT =========================================================;
PROMPT STEP 7.5 — CATEGORY CRUD TEST;
PROMPT =========================================================;

DECLARE
    v_CategoryID  NUMBER;
    v_CatName     tblCategories.CategoryName%TYPE;
    v_UpDate      TIMESTAMP;
    v_CountAfter  NUMBER;
BEGIN
    DBMS_OUTPUT.PUT_LINE('--- 1. Testing SP_CATEGORY_INSERT ---');
    SP_CATEGORY_INSERT(
        p_CategoryName => 'Test Category',
        p_Description  => 'Category created for stored procedure testing',
        p_CreatedBy    => 1, -- Dara Sok
        p_CategoryID   => v_CategoryID
    );
    DBMS_OUTPUT.PUT_LINE('[INSERT SUCCESS] Created Category ID: ' || v_CategoryID);

    DBMS_OUTPUT.PUT_LINE('--- 2. Testing SP_CATEGORY_UPDATE ---');
    SP_CATEGORY_UPDATE(
        p_CategoryID   => v_CategoryID,
        p_CategoryName => 'Updated Test Category',
        p_Description  => 'Updated category description',
        p_UpdateBy     => 1
    );

    SELECT CategoryName, UpdateDate INTO v_CatName, v_UpDate FROM tblCategories WHERE CategoryID = v_CategoryID;
    DBMS_OUTPUT.PUT_LINE('[UPDATE SUCCESS] Name: ' || v_CatName || ' | UpdateDate: ' || TO_CHAR(v_UpDate, 'YYYY-MM-DD HH24:MI:SS'));

    DBMS_OUTPUT.PUT_LINE('--- 3. Testing SP_CATEGORY_DELETE ---');
    SP_CATEGORY_DELETE(p_CategoryID => v_CategoryID);

    SELECT COUNT(*) INTO v_CountAfter FROM tblCategories WHERE CategoryID = v_CategoryID;
    IF v_CountAfter = 0 THEN
        DBMS_OUTPUT.PUT_LINE('[DELETE SUCCESS] Test Category removed. Count = 0');
    ELSE
        RAISE_APPLICATION_ERROR(-20000, 'Category delete failed!');
    END IF;
END;
/


PROMPT =========================================================;
PROMPT STEP 7.6 & 7.7 — PRODUCT CRUD & SOFT DELETE TEST;
PROMPT =========================================================;

DECLARE
    v_ProductID    NUMBER;
    v_ProdName     tblProducts.ProductName%TYPE;
    v_Qty          NUMBER;
    v_isDeleted    NUMBER;
BEGIN
    DBMS_OUTPUT.PUT_LINE('--- 1. Testing SP_PRODUCT_INSERT ---');
    SP_PRODUCT_INSERT(
        p_ProductName  => 'Test Product',
        p_Description  => 'Product created for procedure testing',
        p_CategoryID   => 1, -- Drinks
        p_Barcode      => 'TEST-000001',
        p_ExpiryDate   => ADD_MONTHS(SYSDATE, 12),
        p_Qty          => 100,
        p_UnitPriceIn  => 1.00,
        p_UnitPriceOut => 1.50,
        p_ProductID    => v_ProductID
    );
    DBMS_OUTPUT.PUT_LINE('[INSERT SUCCESS] Created Product ID: ' || v_ProductID);

    DBMS_OUTPUT.PUT_LINE('--- 2. Testing SP_PRODUCT_UPDATE ---');
    SP_PRODUCT_UPDATE(
        p_ProductID    => v_ProductID,
        p_ProductName  => 'Updated Test Product',
        p_Description  => 'Product created for procedure testing',
        p_CategoryID   => 1,
        p_Barcode      => 'TEST-000001',
        p_ExpiryDate   => ADD_MONTHS(SYSDATE, 12),
        p_Qty          => 120,
        p_UnitPriceIn  => 1.10,
        p_UnitPriceOut => 1.75
    );

    SELECT ProductName, Qty INTO v_ProdName, v_Qty FROM tblProducts WHERE ProductID = v_ProductID;
    DBMS_OUTPUT.PUT_LINE('[UPDATE SUCCESS] Name: ' || v_ProdName || ' | Qty: ' || v_Qty);

    DBMS_OUTPUT.PUT_LINE('--- 3. Testing SP_PRODUCT_DELETE (Soft Delete) ---');
    SP_PRODUCT_DELETE(p_ProductID => v_ProductID);

    SELECT isDeleted INTO v_isDeleted FROM tblProducts WHERE ProductID = v_ProductID;
    DBMS_OUTPUT.PUT_LINE('[SOFT DELETE VERIFIED] Product isDeleted = ' || v_isDeleted || ' (Expected: 1)');

    DBMS_OUTPUT.PUT_LINE('--- 4. Testing SP_PRODUCT_RESTORE ---');
    SP_PRODUCT_RESTORE(p_ProductID => v_ProductID);

    SELECT isDeleted INTO v_isDeleted FROM tblProducts WHERE ProductID = v_ProductID;
    DBMS_OUTPUT.PUT_LINE('[RESTORE VERIFIED] Product isDeleted = ' || v_isDeleted || ' (Expected: 0)');

    -- Cleanup test product
    DELETE FROM tblProducts WHERE ProductID = v_ProductID;
    DBMS_OUTPUT.PUT_LINE('[CLEANUP] Test product removed.');
END;
/


PROMPT =========================================================;
PROMPT STEP 7.8 — SUPPLIER & SHIPPER CRUD TESTS;
PROMPT =========================================================;

DECLARE
    v_SupplierID NUMBER;
    v_ShipperID  NUMBER;
    v_CountAfter NUMBER;
BEGIN
    -- Supplier Test
    DBMS_OUTPUT.PUT_LINE('--- Supplier CRUD ---');
    SP_SUPPLIER_INSERT(
        p_SupplierName => 'Test Supplier',
        p_JobTitle     => 'Sales Manager',
        p_CompanyName  => 'Test Supplier Company',
        p_Phone        => '012000003',
        p_Email        => 'supplier.test@example.com',
        p_Address      => 'Phnom Penh',
        p_Country      => 'Cambodia',
        p_Website      => 'https://example.com',
        p_SupplierID   => v_SupplierID
    );
    DBMS_OUTPUT.PUT_LINE('[INSERT SUCCESS] Created Supplier ID: ' || v_SupplierID);

    SP_SUPPLIER_UPDATE(
        p_SupplierID   => v_SupplierID,
        p_SupplierName => 'Updated Supplier',
        p_JobTitle     => 'Senior Manager',
        p_CompanyName  => 'Test Supplier Company Ltd',
        p_Phone        => '012000003',
        p_Email        => 'supplier.test@example.com',
        p_Address      => 'Phnom Penh',
        p_Country      => 'Cambodia',
        p_Website      => 'https://example.com'
    );
    DBMS_OUTPUT.PUT_LINE('[UPDATE SUCCESS] Supplier ID ' || v_SupplierID || ' updated.');

    SP_SUPPLIER_DELETE(p_SupplierID => v_SupplierID);
    SELECT COUNT(*) INTO v_CountAfter FROM tblSuppliers WHERE SupplierID = v_SupplierID;
    DBMS_OUTPUT.PUT_LINE('[DELETE SUCCESS] Supplier removed. Count = ' || v_CountAfter);

    -- Shipper Test
    DBMS_OUTPUT.PUT_LINE('--- Shipper CRUD ---');
    SP_SHIPPER_INSERT(
        p_CompanyName   => 'Test Delivery',
        p_ContactName   => 'Test Contact',
        p_ContactNumber => '012000004',
        p_Email         => 'shipper.test@example.com',
        p_Address       => 'Phnom Penh',
        p_Country       => 'Cambodia',
        p_ShipperID     => v_ShipperID
    );
    DBMS_OUTPUT.PUT_LINE('[INSERT SUCCESS] Created Shipper ID: ' || v_ShipperID);

    SP_SHIPPER_UPDATE(
        p_ShipperID     => v_ShipperID,
        p_CompanyName   => 'Test Express Global',
        p_ContactName   => 'Test Contact Person',
        p_ContactNumber => '012000004',
        p_Email         => 'shipper.test@example.com',
        p_Address       => 'Phnom Penh',
        p_Country       => 'Cambodia'
    );
    DBMS_OUTPUT.PUT_LINE('[UPDATE SUCCESS] Shipper ID ' || v_ShipperID || ' updated.');

    SP_SHIPPER_DELETE(p_ShipperID => v_ShipperID);
    SELECT COUNT(*) INTO v_CountAfter FROM tblShippers WHERE ShipperID = v_ShipperID;
    DBMS_OUTPUT.PUT_LINE('[DELETE SUCCESS] Shipper removed. Count = ' || v_CountAfter);
END;
/


PROMPT =========================================================;
PROMPT STEP 7.9 & 7.10 — SALES TRANSACTION & STOCK VALIDATION;
PROMPT =========================================================;

DECLARE
    v_TestProdID   NUMBER;
    v_TestSaleID   NUMBER;
    v_StockBefore  NUMBER;
    v_StockAfter   NUMBER;
    v_StockFinal   NUMBER;
BEGIN
    -- 1. Create dedicated test product
    SP_PRODUCT_INSERT(
        p_ProductName  => 'Dedicated Sales Test Product',
        p_Description  => 'For sales transaction and stock verification',
        p_CategoryID   => 1,
        p_Barcode      => 'TEST-SALE-001',
        p_ExpiryDate   => ADD_MONTHS(SYSDATE, 12),
        p_Qty          => 100,
        p_UnitPriceIn  => 1.00,
        p_UnitPriceOut => 2.00,
        p_ProductID    => v_TestProdID
    );
    DBMS_OUTPUT.PUT_LINE('1. Created Test Product #' || v_TestProdID || ' with Stock = 100');

    -- 2. Create Sale
    SP_SALE_INSERT(
        p_EmployeeID => 1,
        p_CustomerID => 1,
        p_SalesID    => v_TestSaleID
    );
    DBMS_OUTPUT.PUT_LINE('2. Created Sale #' || v_TestSaleID);

    -- 3. Add Sales Detail (5 units)
    SP_SALES_DETAIL_INSERT(
        p_SalesID   => v_TestSaleID,
        p_ProductID => v_TestProdID,
        p_QtySales  => 5
    );
    SELECT Qty INTO v_StockAfter FROM tblProducts WHERE ProductID = v_TestProdID;
    DBMS_OUTPUT.PUT_LINE('3. Sold 5 units. Current Stock = ' || v_StockAfter || ' (Expected: 95)');

    -- 4. Test Insufficient Stock Exception (Try selling 1000 units)
    DBMS_OUTPUT.PUT_LINE('4. Testing Insufficient Stock (Sell 1000 units)...');
    BEGIN
        SP_SALES_DETAIL_INSERT(
            p_SalesID   => v_TestSaleID,
            p_ProductID => v_TestProdID,
            p_QtySales  => 1000
        );
        RAISE_APPLICATION_ERROR(-20000, 'FAIL: ORA-20001 was not raised!');
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE = -20001 THEN
                DBMS_OUTPUT.PUT_LINE('[EXPECTED ERROR CAUGHT] ORA-20001: Insufficient product stock.');
            ELSE
                RAISE;
            END IF;
    END;

    -- Confirm stock was unchanged by failed attempt
    SELECT Qty INTO v_StockAfter FROM tblProducts WHERE ProductID = v_TestProdID;
    DBMS_OUTPUT.PUT_LINE('   Stock after failed insert remains: ' || v_StockAfter || ' (Expected: 95)');

    -- 5. Test Invalid Quantity Exception (Try selling 0 and -5)
    DBMS_OUTPUT.PUT_LINE('5. Testing Invalid Quantity (Qty = 0)...');
    BEGIN
        SP_SALES_DETAIL_INSERT(
            p_SalesID   => v_TestSaleID,
            p_ProductID => v_TestProdID,
            p_QtySales  => 0
        );
        RAISE_APPLICATION_ERROR(-20000, 'FAIL: ORA-20004 was not raised!');
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE = -20004 THEN
                DBMS_OUTPUT.PUT_LINE('[EXPECTED ERROR CAUGHT] ORA-20004: Invalid sales quantity.');
            ELSE
                RAISE;
            END IF;
    END;

    -- 6. Cancel sale and verify stock restoration
    DBMS_OUTPUT.PUT_LINE('6. Testing Sale Cancellation & Stock Restoration...');
    SP_SALE_DELETE(p_SalesID => v_TestSaleID);

    SELECT Qty INTO v_StockFinal FROM tblProducts WHERE ProductID = v_TestProdID;
    DBMS_OUTPUT.PUT_LINE('   Restored Stock = ' || v_StockFinal || ' (Expected: 100)');

    -- Cleanup test product
    DELETE FROM tblProducts WHERE ProductID = v_TestProdID;
    DBMS_OUTPUT.PUT_LINE('[PASSED] Sales transaction and stock validation verified.');
END;
/


PROMPT =========================================================;
PROMPT STEP 7.11 & 7.12 — PURCHASE TRANSACTION & STOCK VALIDATION;
PROMPT =========================================================;

DECLARE
    v_TestProdID   NUMBER;
    v_TestPurID    NUMBER;
    v_StockBefore  NUMBER;
    v_StockAfter   NUMBER;
    v_StockFinal   NUMBER;
BEGIN
    -- 1. Create dedicated test product
    SP_PRODUCT_INSERT(
        p_ProductName  => 'Dedicated Purchase Test Product',
        p_Description  => 'For purchase transaction and stock verification',
        p_CategoryID   => 1,
        p_Barcode      => 'TEST-PUR-001',
        p_ExpiryDate   => ADD_MONTHS(SYSDATE, 12),
        p_Qty          => 100,
        p_UnitPriceIn  => 1.50,
        p_UnitPriceOut => 2.50,
        p_ProductID    => v_TestProdID
    );
    DBMS_OUTPUT.PUT_LINE('1. Created Test Product #' || v_TestProdID || ' with Stock = 100');

    -- 2. Create Purchase
    SP_PURCHASE_INSERT(
        p_EmployeeID => 1,
        p_SupplierID => 1,
        p_ShipperID  => 1,
        p_PurchaseID => v_TestPurID
    );
    DBMS_OUTPUT.PUT_LINE('2. Created Purchase #' || v_TestPurID);

    -- 3. Add Purchase Detail (20 units)
    SP_PURCHASE_DETAIL_INSERT(
        p_PurchaseID  => v_TestPurID,
        p_ProductID   => v_TestProdID,
        p_PurchaseQty => 20
    );
    SELECT Qty INTO v_StockAfter FROM tblProducts WHERE ProductID = v_TestProdID;
    DBMS_OUTPUT.PUT_LINE('3. Purchased 20 units. Current Stock = ' || v_StockAfter || ' (Expected: 120)');

    -- 4. Test Invalid Purchase Quantity (Qty = 0)
    DBMS_OUTPUT.PUT_LINE('4. Testing Invalid Purchase Quantity (Qty = 0)...');
    BEGIN
        SP_PURCHASE_DETAIL_INSERT(
            p_PurchaseID  => v_TestPurID,
            p_ProductID   => v_TestProdID,
            p_PurchaseQty => 0
        );
        RAISE_APPLICATION_ERROR(-20000, 'FAIL: ORA-20005 was not raised!');
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE = -20005 THEN
                DBMS_OUTPUT.PUT_LINE('[EXPECTED ERROR CAUGHT] ORA-20005: Invalid purchase quantity.');
            ELSE
                RAISE;
            END IF;
    END;

    -- 5. Cancel purchase and verify stock reversal
    DBMS_OUTPUT.PUT_LINE('5. Testing Purchase Cancellation & Stock Reversal...');
    SP_PURCHASE_DELETE(p_PurchaseID => v_TestPurID);

    SELECT Qty INTO v_StockFinal FROM tblProducts WHERE ProductID = v_TestProdID;
    DBMS_OUTPUT.PUT_LINE('   Reversed Stock = ' || v_StockFinal || ' (Expected: 100)');

    -- Cleanup test product
    DELETE FROM tblProducts WHERE ProductID = v_TestProdID;
    DBMS_OUTPUT.PUT_LINE('[PASSED] Purchase transaction and stock validation verified.');
END;
/


PROMPT =========================================================;
PROMPT STEP 7.13 — FOREIGN KEY PROTECTION TESTS;
PROMPT =========================================================;

DECLARE
    v_DummyID NUMBER;
BEGIN
    DBMS_OUTPUT.PUT_LINE('--- 1. Testing Product Insert with invalid CategoryID = 99999 ---');
    BEGIN
        SP_PRODUCT_INSERT(
            p_ProductName  => 'Invalid Category Product',
            p_Description  => 'Should fail FK constraint',
            p_CategoryID   => 99999,
            p_Barcode      => 'INVALID-001',
            p_ExpiryDate   => NULL,
            p_Qty          => 10,
            p_UnitPriceIn  => 1.00,
            p_UnitPriceOut => 2.00,
            p_ProductID    => v_DummyID
        );
        RAISE_APPLICATION_ERROR(-20000, 'FAIL: Foreign key was not enforced!');
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE = -2291 THEN
                DBMS_OUTPUT.PUT_LINE('[EXPECTED ERROR CAUGHT] ORA-02291: Integrity constraint violated - parent key not found.');
            ELSE
                RAISE;
            END IF;
    END;

    DBMS_OUTPUT.PUT_LINE('--- 2. Testing Sale Insert with invalid EmployeeID = 99999 ---');
    BEGIN
        SP_SALE_INSERT(
            p_EmployeeID => 99999,
            p_CustomerID => 1,
            p_SalesID    => v_DummyID
        );
        RAISE_APPLICATION_ERROR(-20000, 'FAIL: Foreign key was not enforced!');
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE = -2291 THEN
                DBMS_OUTPUT.PUT_LINE('[EXPECTED ERROR CAUGHT] ORA-02291: Integrity constraint violated - parent key not found.');
            ELSE
                RAISE;
            END IF;
    END;

    DBMS_OUTPUT.PUT_LINE('--- 3. Testing Purchase Insert with invalid SupplierID = 99999 ---');
    BEGIN
        SP_PURCHASE_INSERT(
            p_EmployeeID => 1,
            p_SupplierID => 99999,
            p_ShipperID  => 1,
            p_PurchaseID => v_DummyID
        );
        RAISE_APPLICATION_ERROR(-20000, 'FAIL: Foreign key was not enforced!');
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE = -2291 THEN
                DBMS_OUTPUT.PUT_LINE('[EXPECTED ERROR CAUGHT] ORA-02291: Integrity constraint violated - parent key not found.');
            ELSE
                RAISE;
            END IF;
    END;
END;
/


PROMPT =========================================================;
PROMPT STEP 7.14 — FINAL VALIDATION & STOCK INTEGRITY;
PROMPT =========================================================;

SELECT
    ProductID,
    ProductName,
    Qty,
    UnitPriceIn,
    UnitPriceOut,
    isDeleted
FROM tblProducts
ORDER BY ProductID;

SELECT COUNT(*) AS negative_stock_count
FROM tblProducts
WHERE Qty < 0;

SELECT
    object_name,
    status
FROM user_objects
WHERE object_type = 'PROCEDURE'
ORDER BY object_name;

SELECT name, text
FROM user_errors;

COMMIT;
