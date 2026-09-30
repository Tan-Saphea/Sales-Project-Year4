SET DEFINE OFF;
SET SERVEROUTPUT ON;
SET SQLBLANKLINES ON;
SET LINESIZE 200;
SET PAGESIZE 100;

/* =========================================================
   STEP 6 TEST SCRIPT
   Oracle Database / SQL Developer Test Suite
   Schema: SALESPURCHASEDB
   ========================================================= */


-- =========================================================
-- SECTION 1: VERIFY PROCEDURE STATUS & COMPILATION ERRORS
-- =========================================================
PROMPT =========================================================;
PROMPT 1. VERIFYING ALL STORED PROCEDURES (Must be VALID);
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
    line,
    position,
    text
FROM user_errors
ORDER BY name, sequence;


-- =========================================================
-- SECTION 2: TEST INSERT PROCEDURES
-- =========================================================
PROMPT =========================================================;
PROMPT 2. TESTING INSERT PROCEDURES;
PROMPT =========================================================;

DECLARE
    v_emp_id  NUMBER;
    v_mem_id  NUMBER;
    v_cust_id NUMBER;
    v_cat_id  NUMBER;
    v_prod_id NUMBER;
    v_sup_id  NUMBER;
    v_shp_id  NUMBER;
BEGIN
    DBMS_OUTPUT.PUT_LINE('--- Testing Master Table Inserts ---');

    -- 1. Employee Insert
    SP_EMPLOYEE_INSERT(
        p_EmployeeName   => 'Leakhena Meas',
        p_UserName       => 'leakhena.m',
        p_Password       => 'SecurePass123',
        p_UserType       => 'Staff',
        p_Role           => 'Sales',
        p_JobTitle       => 'Sales Associate',
        p_Email          => 'leakhena@example.com',
        p_Phone          => '012999001',
        p_HomePhone      => '023888004',
        p_Address        => 'St 1986, Sen Sok',
        p_City           => 'Phnom Penh',
        p_StateProvince  => 'Phnom Penh',
        p_ZipPostalCode  => '12000',
        p_CountryRegion  => 'Cambodia',
        p_EmployeeID     => v_emp_id
    );
    DBMS_OUTPUT.PUT_LINE('[SUCCESS] New Employee ID: ' || v_emp_id);

    -- 2. Member Insert
    SP_MEMBER_INSERT(
        p_MemberName => 'Platinum VIP',
        p_Discount   => 15.00,
        p_MemberID   => v_mem_id
    );
    DBMS_OUTPUT.PUT_LINE('[SUCCESS] New Member ID: ' || v_mem_id);

    -- 3. Customer Insert
    SP_CUSTOMER_INSERT(
        p_CustomerName => 'Borith Heng',
        p_JobTitle     => 'Architect',
        p_Phone        => '017888999',
        p_Email        => 'borith@example.com',
        p_Address      => 'BKK1, Phnom Penh',
        p_MemberID     => v_mem_id,
        p_CustomerID   => v_cust_id
    );
    DBMS_OUTPUT.PUT_LINE('[SUCCESS] New Customer ID: ' || v_cust_id);

    -- 4. Category Insert
    SP_CATEGORY_INSERT(
        p_CategoryName => 'Bakery & Sweets',
        p_Description  => 'Fresh baked bread and pastries',
        p_CreatedBy    => v_emp_id,
        p_CategoryID   => v_cat_id
    );
    DBMS_OUTPUT.PUT_LINE('[SUCCESS] New Category ID: ' || v_cat_id);

    -- 5. Product Insert
    SP_PRODUCT_INSERT(
        p_ProductName  => 'Butter Croissant',
        p_Description  => 'French style flaky butter croissant',
        p_CategoryID   => v_cat_id,
        p_Barcode      => '885000000004',
        p_ExpiryDate   => SYSDATE + 7,
        p_Qty          => 40,
        p_UnitPriceIn  => 0.80,
        p_UnitPriceOut => 1.50,
        p_ProductID    => v_prod_id
    );
    DBMS_OUTPUT.PUT_LINE('[SUCCESS] New Product ID: ' || v_prod_id);

    -- 6. Supplier Insert
    SP_SUPPLIER_INSERT(
        p_SupplierName => 'Angkor Food Distributors',
        p_JobTitle     => 'Managing Director',
        p_CompanyName  => 'Angkor Supply Ltd',
        p_Phone        => '023444555',
        p_Email        => 'orders@angkorsupply.com',
        p_Address      => 'National Road 4',
        p_Country      => 'Cambodia',
        p_Website      => 'www.angkorsupply.com',
        p_SupplierID   => v_sup_id
    );
    DBMS_OUTPUT.PUT_LINE('[SUCCESS] New Supplier ID: ' || v_sup_id);

    -- 7. Shipper Insert
    SP_SHIPPER_INSERT(
        p_CompanyName   => 'Express Link Courier',
        p_ContactName   => 'Kosal Chea',
        p_ContactNumber => '092111333',
        p_Email         => 'dispatch@expresslink.com',
        p_Address       => 'Airport Road',
        p_Country       => 'Cambodia',
        p_ShipperID     => v_shp_id
    );
    DBMS_OUTPUT.PUT_LINE('[SUCCESS] New Shipper ID: ' || v_shp_id);
END;
/


-- =========================================================
-- SECTION 3: TEST SELECT ALL PROCEDURES (via SYS_REFCURSOR)
-- =========================================================
PROMPT =========================================================;
PROMPT 3. TESTING SELECT ALL PROCEDURES;
PROMPT =========================================================;

VARIABLE rc REFCURSOR;

PROMPT --- 1. SP_EMPLOYEE_SELECT_ALL ---;
EXEC SP_EMPLOYEE_SELECT_ALL(:rc);
PRINT rc;

PROMPT --- 2. SP_MEMBER_SELECT_ALL ---;
EXEC SP_MEMBER_SELECT_ALL(:rc);
PRINT rc;

PROMPT --- 3. SP_CUSTOMER_SELECT_ALL ---;
EXEC SP_CUSTOMER_SELECT_ALL(:rc);
PRINT rc;

PROMPT --- 4. SP_CATEGORY_SELECT_ALL ---;
EXEC SP_CATEGORY_SELECT_ALL(:rc);
PRINT rc;

PROMPT --- 5. SP_PRODUCT_SELECT_ALL ---;
EXEC SP_PRODUCT_SELECT_ALL(:rc);
PRINT rc;

PROMPT --- 6. SP_SUPPLIER_SELECT_ALL ---;
EXEC SP_SUPPLIER_SELECT_ALL(:rc);
PRINT rc;

PROMPT --- 7. SP_SHIPPER_SELECT_ALL ---;
EXEC SP_SHIPPER_SELECT_ALL(:rc);
PRINT rc;

PROMPT --- 8. SP_SALE_SELECT_ALL ---;
EXEC SP_SALE_SELECT_ALL(:rc);
PRINT rc;

PROMPT --- 9. SP_PURCHASE_SELECT_ALL ---;
EXEC SP_PURCHASE_SELECT_ALL(:rc);
PRINT rc;


-- =========================================================
-- SECTION 4: TEST UPDATE PROCEDURES
-- =========================================================
PROMPT =========================================================;
PROMPT 4. TESTING UPDATE PROCEDURES;
PROMPT =========================================================;

-- Update Category
EXEC SP_CATEGORY_UPDATE(p_CategoryID => 1, p_CategoryName => 'Beverages & Drinks', p_Description => 'Cold beverages, juices, and soft drinks', p_UpdateBy => 1);

-- Verify Updated Category
EXEC SP_CATEGORY_SELECT_BY_ID(1, :rc);
PRINT rc;

-- Update Product
EXEC SP_PRODUCT_UPDATE(p_ProductID => 1, p_ProductName => 'Coca Cola 330ml Can', p_Description => 'Classic Coca Cola original taste 330ml', p_CategoryID => 1, p_Barcode => '885000000001', p_ExpiryDate => ADD_MONTHS(SYSDATE, 12), p_Qty => 100, p_UnitPriceIn => 0.55, p_UnitPriceOut => 0.80);

-- Verify Updated Product
EXEC SP_PRODUCT_SELECT_BY_ID(1, :rc);
PRINT rc;

-- Update Member
EXEC SP_MEMBER_UPDATE(p_MemberID => 1, p_MemberName => 'Regular Member', p_Discount => 2.00);
EXEC SP_MEMBER_SELECT_BY_ID(1, :rc);
PRINT rc;


-- =========================================================
-- SECTION 5: TEST DELETE & RESTORE (SOFT DELETE FOR PRODUCT)
-- =========================================================
PROMPT =========================================================;
PROMPT 5. TESTING SOFT DELETE & RESTORE FOR PRODUCT;
PROMPT =========================================================;

PROMPT --- Deleting Product #2 (Soft Delete: isDeleted becomes 1) ---;
EXEC SP_PRODUCT_DELETE(2);

SELECT ProductID, ProductName, isDeleted
FROM tblProducts
WHERE ProductID = 2;

PROMPT --- Notice Product #2 is excluded from SP_PRODUCT_SELECT_ALL ---;
EXEC SP_PRODUCT_SELECT_ALL(:rc);
PRINT rc;

PROMPT --- Restoring Product #2 (isDeleted becomes 0) ---;
EXEC SP_PRODUCT_RESTORE(2);

SELECT ProductID, ProductName, isDeleted
FROM tblProducts
WHERE ProductID = 2;


-- =========================================================
-- SECTION 6: SALES TRANSACTION COMPLETE TEST (10 STEPS)
-- =========================================================
PROMPT =========================================================;
PROMPT 6. COMPLETE SALES TRANSACTION TEST SCENARIO;
PROMPT =========================================================;

DECLARE
    v_initial_qty NUMBER;
    v_after_sale_qty NUMBER;
    v_restored_qty NUMBER;
    v_test_sale_id NUMBER;
    v_test_prod_id NUMBER := 1; -- Coca Cola
    v_sale_qty NUMBER := 10;
BEGIN
    DBMS_OUTPUT.PUT_LINE('=== SALES WORKFLOW TEST ===');

    -- Step 1: Check initial stock
    SELECT Qty INTO v_initial_qty FROM tblProducts WHERE ProductID = v_test_prod_id;
    DBMS_OUTPUT.PUT_LINE('Step 1: Product #' || v_test_prod_id || ' Initial Stock: ' || v_initial_qty);

    -- Step 2 & 3: Create Sale header and retrieve generated SalesID
    SP_SALE_INSERT(
        p_EmployeeID => 1,
        p_CustomerID => 1,
        p_SalesID    => v_test_sale_id
    );
    DBMS_OUTPUT.PUT_LINE('Step 2 & 3: Created Sale header. Generated SalesID: ' || v_test_sale_id);

    -- Step 4: Add Sales Detail (10 units)
    SP_SALES_DETAIL_INSERT(
        p_SalesID   => v_test_sale_id,
        p_ProductID => v_test_prod_id,
        p_QtySales  => v_sale_qty
    );
    DBMS_OUTPUT.PUT_LINE('Step 4: Added SalesDetail for ' || v_sale_qty || ' units.');

    -- Step 5: Verify product stock decreased
    SELECT Qty INTO v_after_sale_qty FROM tblProducts WHERE ProductID = v_test_prod_id;
    DBMS_OUTPUT.PUT_LINE('Step 5: Stock after sale: ' || v_after_sale_qty || ' (Expected: ' || (v_initial_qty - v_sale_qty) || ')');

    IF v_after_sale_qty != (v_initial_qty - v_sale_qty) THEN
        RAISE_APPLICATION_ERROR(-20001, 'Stock deduction validation failed!');
    END IF;

    -- Step 9: Cancel/delete the test sale
    DBMS_OUTPUT.PUT_LINE('Step 9: Cancelling/Deleting test sale #' || v_test_sale_id || '...');
    SP_SALE_DELETE(p_SalesID => v_test_sale_id);

    -- Step 10: Verify product stock was fully restored
    SELECT Qty INTO v_restored_qty FROM tblProducts WHERE ProductID = v_test_prod_id;
    DBMS_OUTPUT.PUT_LINE('Step 10: Product stock after cancellation: ' || v_restored_qty || ' (Expected: ' || v_initial_qty || ')');

    IF v_restored_qty = v_initial_qty THEN
        DBMS_OUTPUT.PUT_LINE('[PASSED] Sales stock decrement and restoration lifecycle successfully verified.');
    ELSE
        RAISE_APPLICATION_ERROR(-20001, 'Stock restoration validation failed!');
    END IF;
END;
/


-- =========================================================
-- SECTION 7: PURCHASE TRANSACTION COMPLETE TEST (9 STEPS)
-- =========================================================
PROMPT =========================================================;
PROMPT 7. COMPLETE PURCHASE TRANSACTION TEST SCENARIO;
PROMPT =========================================================;

DECLARE
    v_initial_qty NUMBER;
    v_after_purchase_qty NUMBER;
    v_restored_qty NUMBER;
    v_test_pur_id NUMBER;
    v_test_prod_id NUMBER := 3; -- Instant Noodle
    v_pur_qty NUMBER := 25;
BEGIN
    DBMS_OUTPUT.PUT_LINE('=== PURCHASE WORKFLOW TEST ===');

    -- Step 1: Check initial stock
    SELECT Qty INTO v_initial_qty FROM tblProducts WHERE ProductID = v_test_prod_id;
    DBMS_OUTPUT.PUT_LINE('Step 1: Product #' || v_test_prod_id || ' Initial Stock: ' || v_initial_qty);

    -- Step 2 & 3: Create Purchase header and retrieve generated PurchaseID
    SP_PURCHASE_INSERT(
        p_EmployeeID => 1,
        p_SupplierID => 1,
        p_ShipperID  => 1,
        p_PurchaseID => v_test_pur_id
    );
    DBMS_OUTPUT.PUT_LINE('Step 2 & 3: Created Purchase header. Generated PurchaseID: ' || v_test_pur_id);

    -- Step 4: Add Purchase Detail (25 units)
    SP_PURCHASE_DETAIL_INSERT(
        p_PurchaseID  => v_test_pur_id,
        p_ProductID   => v_test_prod_id,
        p_PurchaseQty => v_pur_qty
    );
    DBMS_OUTPUT.PUT_LINE('Step 4: Added PurchaseDetail for ' || v_pur_qty || ' units.');

    -- Step 5: Verify product stock increased
    SELECT Qty INTO v_after_purchase_qty FROM tblProducts WHERE ProductID = v_test_prod_id;
    DBMS_OUTPUT.PUT_LINE('Step 5: Stock after purchase: ' || v_after_purchase_qty || ' (Expected: ' || (v_initial_qty + v_pur_qty) || ')');

    IF v_after_purchase_qty != (v_initial_qty + v_pur_qty) THEN
        RAISE_APPLICATION_ERROR(-20001, 'Stock addition validation failed!');
    END IF;

    -- Step 8: Cancel/delete the test purchase
    DBMS_OUTPUT.PUT_LINE('Step 8: Cancelling/Deleting test purchase #' || v_test_pur_id || '...');
    SP_PURCHASE_DELETE(p_PurchaseID => v_test_pur_id);

    -- Step 9: Verify stock returned to original quantity
    SELECT Qty INTO v_restored_qty FROM tblProducts WHERE ProductID = v_test_prod_id;
    DBMS_OUTPUT.PUT_LINE('Step 9: Product stock after cancellation: ' || v_restored_qty || ' (Expected: ' || v_initial_qty || ')');

    IF v_restored_qty = v_initial_qty THEN
        DBMS_OUTPUT.PUT_LINE('[PASSED] Purchase stock increment and reversal lifecycle successfully verified.');
    ELSE
        RAISE_APPLICATION_ERROR(-20001, 'Stock reversal validation failed!');
    END IF;
END;
/

-- Rollback any temporary modifications from the test session to keep sample data clean
ROLLBACK;
