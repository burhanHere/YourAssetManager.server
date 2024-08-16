CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET = utf8mb4;

START TRANSACTION;

ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `AspNetRoles` (
    `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Name` varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedName` varchar(256) CHARACTER SET utf8mb4 NULL,
    `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetRoles` PRIMARY KEY (`Id`)
) CHARACTER SET = utf8mb4;

CREATE TABLE `AssetStatuses` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `StatusName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_AssetStatuses` PRIMARY KEY (`Id`)
) CHARACTER SET = utf8mb4;

CREATE TABLE `Organizations` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `OrganizationName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Description` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
    `ActiveOrganization` tinyint(1) NOT NULL,
    `CreatedDate` datetime(6) NOT NULL,
    `UpdatedDate` datetime(6) NOT NULL,
    `OrganizationDomain` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Organizations` PRIMARY KEY (`Id`)
) CHARACTER SET = utf8mb4;

CREATE TABLE `AspNetRoleClaims` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `RoleId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
    `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetRoleClaims` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
) CHARACTER SET = utf8mb4;

CREATE TABLE `AspNetUsers` (
    `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `OrganizationId` int NOT NULL,
    `UserName` varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 NULL,
    `Email` varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 NULL,
    `EmailConfirmed` tinyint(1) NOT NULL,
    `PasswordHash` longtext CHARACTER SET utf8mb4 NULL,
    `SecurityStamp` longtext CHARACTER SET utf8mb4 NULL,
    `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
    `PhoneNumber` longtext CHARACTER SET utf8mb4 NULL,
    `PhoneNumberConfirmed` tinyint(1) NOT NULL,
    `TwoFactorEnabled` tinyint(1) NOT NULL,
    `LockoutEnd` datetime(6) NULL,
    `LockoutEnabled` tinyint(1) NOT NULL,
    `AccessFailedCount` int NOT NULL,
    CONSTRAINT `PK_AspNetUsers` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetUsers_Organizations_OrganizationId` FOREIGN KEY (`OrganizationId`) REFERENCES `Organizations` (`Id`) ON DELETE RESTRICT
) CHARACTER SET = utf8mb4;

CREATE TABLE `AssetCategories` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `CategoryName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Description` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
    `RelevantInputFields` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
    `CategoryOrganizationId` int NOT NULL,
    CONSTRAINT `PK_AssetCategories` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AssetCategories_Organizations_CategoryOrganizationId` FOREIGN KEY (`CategoryOrganizationId`) REFERENCES `Organizations` (`Id`) ON DELETE CASCADE
) CHARACTER SET = utf8mb4;

CREATE TABLE `AssetTypes` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `AssetTypeName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Description` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
    `OrganizationId` int NOT NULL,
    CONSTRAINT `PK_AssetTypes` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AssetTypes_Organizations_OrganizationId` FOREIGN KEY (`OrganizationId`) REFERENCES `Organizations` (`Id`) ON DELETE CASCADE
) CHARACTER SET = utf8mb4;

CREATE TABLE `Vendors` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Email` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `OfficeAddress` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
    `PhoneNumber` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
    `OrganizationId` int NOT NULL,
    CONSTRAINT `PK_Vendors` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Vendors_Organizations_OrganizationId` FOREIGN KEY (`OrganizationId`) REFERENCES `Organizations` (`Id`) ON DELETE CASCADE
) CHARACTER SET = utf8mb4;

CREATE TABLE `AspNetUserClaims` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
    `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetUserClaims` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) CHARACTER SET = utf8mb4;

CREATE TABLE `AspNetUserLogins` (
    `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProviderKey` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProviderDisplayName` longtext CHARACTER SET utf8mb4 NULL,
    `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_AspNetUserLogins` PRIMARY KEY (
        `LoginProvider`,
        `ProviderKey`
    ),
    CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) CHARACTER SET = utf8mb4;

CREATE TABLE `AspNetUserRoles` (
    `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `RoleId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_AspNetUserRoles` PRIMARY KEY (`UserId`, `RoleId`),
    CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) CHARACTER SET = utf8mb4;

CREATE TABLE `AspNetUserTokens` (
    `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Value` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetUserTokens` PRIMARY KEY (
        `UserId`,
        `LoginProvider`,
        `Name`
    ),
    CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) CHARACTER SET = utf8mb4;

CREATE TABLE `DeactivatedOrganizations` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `OrganizationId` int NOT NULL,
    `ApplicationUserId` varchar(256) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_DeactivatedOrganizations` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_DeactivatedOrganizations_AspNetUsers_ApplicationUserId` FOREIGN KEY (`ApplicationUserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_DeactivatedOrganizations_Organizations_OrganizationId` FOREIGN KEY (`OrganizationId`) REFERENCES `Organizations` (`Id`) ON DELETE RESTRICT
) CHARACTER SET = utf8mb4;

CREATE TABLE `Assets` (
    `AssetId` int NOT NULL AUTO_INCREMENT,
    `AssetName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Description` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
    `PurchaseDate` datetime(6) NOT NULL,
    `PurchasePrice` double NOT NULL,
    `SerialNumber` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `CreatedDate` datetime(6) NOT NULL,
    `UpdatedDate` datetime(6) NOT NULL,
    `Problems` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
    `AssetIdentificationNumber` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Manufacturer` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Model` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `AssetStatusId` int NOT NULL,
    `OrganizationId` int NOT NULL,
    `AssetCategoryId` int NOT NULL,
    `AssetTypeId` int NOT NULL,
    `VendorId` int NOT NULL,
    CONSTRAINT `PK_Assets` PRIMARY KEY (`AssetId`),
    CONSTRAINT `FK_Assets_AssetCategories_AssetCategoryId` FOREIGN KEY (`AssetCategoryId`) REFERENCES `AssetCategories` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Assets_AssetStatuses_AssetStatusId` FOREIGN KEY (`AssetStatusId`) REFERENCES `AssetStatuses` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Assets_AssetTypes_AssetTypeId` FOREIGN KEY (`AssetTypeId`) REFERENCES `AssetTypes` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Assets_Organizations_OrganizationId` FOREIGN KEY (`OrganizationId`) REFERENCES `Organizations` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Assets_Vendors_VendorId` FOREIGN KEY (`VendorId`) REFERENCES `Vendors` (`Id`) ON DELETE SET NULL
) CHARACTER SET = utf8mb4;

CREATE TABLE `AssetAssignments` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `AssignedDate` datetime(6) NOT NULL,
    `Notes` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
    `AssignedToId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `AssignedById` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `AssetId` int NOT NULL,
    CONSTRAINT `PK_AssetAssignments` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AssetAssignments_AspNetUsers_AssignedById` FOREIGN KEY (`AssignedById`) REFERENCES `AspNetUsers` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_AssetAssignments_AspNetUsers_AssignedToId` FOREIGN KEY (`AssignedToId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_AssetAssignments_Assets_AssetId` FOREIGN KEY (`AssetId`) REFERENCES `Assets` (`AssetId`) ON DELETE CASCADE
) CHARACTER SET = utf8mb4;

CREATE TABLE `AssetMaintenances` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `MaintenanceDate` datetime(6) NOT NULL,
    `Description` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
    `AssetId` int NOT NULL,
    CONSTRAINT `PK_AssetMaintenances` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AssetMaintenances_Assets_AssetId` FOREIGN KEY (`AssetId`) REFERENCES `Assets` (`AssetId`) ON DELETE CASCADE
) CHARACTER SET = utf8mb4;

CREATE TABLE `AssetRetires` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `RetiredOn` datetime(6) NOT NULL,
    `RetirementReason` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
    `AssetId` int NOT NULL,
    CONSTRAINT `PK_AssetRetires` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AssetRetires_Assets_AssetId` FOREIGN KEY (`AssetId`) REFERENCES `Assets` (`AssetId`) ON DELETE CASCADE
) CHARACTER SET = utf8mb4;

CREATE TABLE `AssetReturns` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ReturnedDate` datetime(6) NOT NULL,
    `ReturnCondition` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
    `Notes` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
    `AssetAssignmentId` int NOT NULL,
    CONSTRAINT `PK_AssetReturns` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AssetReturns_AssetAssignments_AssetAssignmentId` FOREIGN KEY (`AssetAssignmentId`) REFERENCES `AssetAssignments` (`Id`) ON DELETE CASCADE
) CHARACTER SET = utf8mb4;

INSERT INTO
    `AspNetRoles` (
        `Id`,
        `ConcurrencyStamp`,
        `Name`,
        `NormalizedName`
    )
VALUES (
        '1',
        '1',
        'OrganizationOwner',
        'ORGANIZATIONOWNER'
    ),
    (
        '2',
        '2',
        'AssetManager',
        'ASSETMANAGER'
    ),
    (
        '3',
        '3',
        'Employee',
        'EMPLOYEE'
    );

INSERT INTO
    `AssetStatuses` (`Id`, `StatusName`)
VALUES (1, 'Assigned'),
    (2, 'Retired'),
    (3, 'UnderMaintenance'),
    (4, 'Returned'),
    (5, 'Idle');

CREATE INDEX `IX_AspNetRoleClaims_RoleId` ON `AspNetRoleClaims` (`RoleId`);

CREATE UNIQUE INDEX `RoleNameIndex` ON `AspNetRoles` (`NormalizedName`);

CREATE INDEX `IX_AspNetUserClaims_UserId` ON `AspNetUserClaims` (`UserId`);

CREATE INDEX `IX_AspNetUserLogins_UserId` ON `AspNetUserLogins` (`UserId`);

CREATE INDEX `IX_AspNetUserRoles_RoleId` ON `AspNetUserRoles` (`RoleId`);

CREATE INDEX `EmailIndex` ON `AspNetUsers` (`NormalizedEmail`);

CREATE INDEX `IX_AspNetUsers_OrganizationId` ON `AspNetUsers` (`OrganizationId`);

CREATE UNIQUE INDEX `UserNameIndex` ON `AspNetUsers` (`NormalizedUserName`);

CREATE UNIQUE INDEX `IX_AssetAssignments_AssetId` ON `AssetAssignments` (`AssetId`);

CREATE INDEX `IX_AssetAssignments_AssignedById` ON `AssetAssignments` (`AssignedById`);

CREATE INDEX `IX_AssetAssignments_AssignedToId` ON `AssetAssignments` (`AssignedToId`);

CREATE INDEX `IX_AssetCategories_CategoryOrganizationId` ON `AssetCategories` (`CategoryOrganizationId`);

CREATE UNIQUE INDEX `IX_AssetMaintenances_AssetId` ON `AssetMaintenances` (`AssetId`);

CREATE UNIQUE INDEX `IX_AssetRetires_AssetId` ON `AssetRetires` (`AssetId`);

CREATE UNIQUE INDEX `IX_AssetReturns_AssetAssignmentId` ON `AssetReturns` (`AssetAssignmentId`);

CREATE INDEX `IX_Assets_AssetCategoryId` ON `Assets` (`AssetCategoryId`);

CREATE INDEX `IX_Assets_AssetStatusId` ON `Assets` (`AssetStatusId`);

CREATE INDEX `IX_Assets_AssetTypeId` ON `Assets` (`AssetTypeId`);

CREATE INDEX `IX_Assets_OrganizationId` ON `Assets` (`OrganizationId`);

CREATE INDEX `IX_Assets_VendorId` ON `Assets` (`VendorId`);

CREATE INDEX `IX_AssetTypes_OrganizationId` ON `AssetTypes` (`OrganizationId`);

CREATE INDEX `IX_DeactivatedOrganizations_ApplicationUserId` ON `DeactivatedOrganizations` (`ApplicationUserId`);

CREATE UNIQUE INDEX `IX_DeactivatedOrganizations_OrganizationId` ON `DeactivatedOrganizations` (`OrganizationId`);

CREATE INDEX `IX_Vendors_OrganizationId` ON `Vendors` (`OrganizationId`);

INSERT INTO
    `__EFMigrationsHistory` (
        `MigrationId`,
        `ProductVersion`
    )
VALUES (
        '20240816060638_InitialCreate',
        '8.0.7'
    );

COMMIT;