CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `Authors` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Authors` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Users` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Username` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `PasswordHash` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Role` int NOT NULL,
    CONSTRAINT `PK_Users` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Books` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Title` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Isbn` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `PublicationYear` int NOT NULL,
    `AuthorId` int NOT NULL,
    CONSTRAINT `PK_Books` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Books_Authors_AuthorId` FOREIGN KEY (`AuthorId`) REFERENCES `Authors` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_Books_AuthorId` ON `Books` (`AuthorId`);

CREATE UNIQUE INDEX `IX_Books_Isbn` ON `Books` (`Isbn`);

CREATE UNIQUE INDEX `IX_Users_Username` ON `Users` (`Username`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260619033515_InitialCreate', '9.0.0');

COMMIT;

