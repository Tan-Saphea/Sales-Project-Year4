SET SQLBLANKLINES ON;

/* =============================================
   SAMPLE DATA
   Sales & Purchase Management System
   ============================================= */

/* 1. EMPLOYEES */
INSERT INTO tblEmployees
(
    EmployeeName,
    UserName,
    Password,
    UserType,
    Role,
    JobTitle,
    Email,
    Phone,
    Address,
    City,
    CountryRegion
)
VALUES
(
    'Dara Sok',
    'dara',
    '123456',
    'Admin',
    'Administrator',
    'Manager',
    'dara@example.com',
    '012345678',
    'Phnom Penh',
    'Phnom Penh',
    'Cambodia'
);

INSERT INTO tblEmployees
(
    EmployeeName,
    UserName,
    Password,
    UserType,
    Role,
    JobTitle,
    Email,
    Phone,
    Address,
    City,
    CountryRegion
)
VALUES
(
    'Sokha Chan',
    'sokha',
    '123456',
    'Staff',
    'Cashier',
    'Sales Staff',
    'sokha@example.com',
    '098765432',
    'Phnom Penh',
    'Phnom Penh',
    'Cambodia'
);

/* 2. MEMBERS */
INSERT INTO tblMembers (MemberName, Discount)
VALUES ('Normal', 0);

INSERT INTO tblMembers (MemberName, Discount)
VALUES ('Silver', 5);

INSERT INTO tblMembers (MemberName, Discount)
VALUES ('Gold', 10);

/* 3. CUSTOMERS */
INSERT INTO tblCustomers
(
    CustomerName,
    JobTitle,
    Phone,
    Email,
    Address,
    MemberID
)
VALUES
(
    'Vanna Kim',
    'Teacher',
    '010111222',
    'vanna@example.com',
    'Phnom Penh',
    1
);

INSERT INTO tblCustomers
(
    CustomerName,
    JobTitle,
    Phone,
    Email,
    Address,
    MemberID
)
VALUES
(
    'Srey Mom',
    'Student',
    '011222333',
    'sreymom@example.com',
    'Phnom Penh',
    2
);

/* 4. CATEGORIES */
INSERT INTO tblCategories
(
    CategoryName,
    Description,
    CreatedBy
)
VALUES
(
    'Drinks',
    'Soft drinks and beverages',
    1
);

INSERT INTO tblCategories
(
    CategoryName,
    Description,
    CreatedBy
)
VALUES
(
    'Snacks',
    'Snack products',
    1
);

INSERT INTO tblCategories
(
    CategoryName,
    Description,
    CreatedBy
)
VALUES
(
    'Food',
    'Food products',
    1
);

/* 5. PRODUCTS */
INSERT INTO tblProducts
(
    ProductName,
    Description,
    CategoryID,
    Barcode,
    ExpiryDate,
    Qty,
    UnitPriceIn,
    UnitPriceOut
)
VALUES
(
    'Coca Cola',
    'Coca Cola 330ml',
    1,
    '885000000001',
    ADD_MONTHS(SYSDATE, 12),
    100,
    0.50,
    0.75
);

INSERT INTO tblProducts
(
    ProductName,
    Description,
    CategoryID,
    Barcode,
    ExpiryDate,
    Qty,
    UnitPriceIn,
    UnitPriceOut
)
VALUES
(
    'Potato Chips',
    'Original potato chips',
    2,
    '885000000002',
    ADD_MONTHS(SYSDATE, 8),
    50,
    0.75,
    1.25
);

INSERT INTO tblProducts
(
    ProductName,
    Description,
    CategoryID,
    Barcode,
    ExpiryDate,
    Qty,
    UnitPriceIn,
    UnitPriceOut
)
VALUES
(
    'Instant Noodle',
    'Instant noodle package',
    3,
    '885000000003',
    ADD_MONTHS(SYSDATE, 10),
    80,
    0.30,
    0.50
);

/* 6. SUPPLIERS */
INSERT INTO tblSuppliers
(
    SupplierName,
    JobTitle,
    CompanyName,
    Phone,
    Email,
    Address,
    Country,
    Website
)
VALUES
(
    'David Lee',
    'Sales Manager',
    'ABC Supply Co.',
    '012333444',
    'david@example.com',
    'Phnom Penh',
    'Cambodia',
    'www.example.com'
);

/* 7. SHIPPERS */
INSERT INTO tblShippers
(
    CompanyName,
    ContactName,
    ContactNumber,
    Email,
    Address,
    Country
)
VALUES
(
    'Fast Delivery',
    'Sok Dara',
    '012555666',
    'delivery@example.com',
    'Phnom Penh',
    'Cambodia'
);

COMMIT;
