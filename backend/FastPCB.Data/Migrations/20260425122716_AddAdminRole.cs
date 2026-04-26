using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastPCB.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET @column_exists = (
    SELECT COUNT(*)
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME = 'Users'
      AND COLUMN_NAME = 'Role'
);
SET @sql = IF(
    @column_exists = 0,
    'ALTER TABLE `Users` ADD `Role` int NOT NULL DEFAULT 0',
    'DO 0'
);
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
");

            migrationBuilder.Sql(@"
INSERT INTO `Users` (`Address`, `CreatedAt`, `Email`, `FirstName`, `LastName`, `PasswordHash`, `Phone`, `Role`, `UpdatedAt`)
SELECT
    'Isparta, Turkey',
    '2026-04-23 22:20:37.869429',
    'admin@fastpcb.com',
    'Admin',
    'User',
    '100000.AQIDBAUGBwgJCgsMDQ4PEA==.FMliGP5Jcjft/GMzn7MtR2CZciACjUsOHdwvp/0h/cs=',
    '+905551111111',
    1,
    '2026-04-23 22:20:37.869429'
WHERE NOT EXISTS (
    SELECT 1 FROM `Users` WHERE `Email` = 'admin@fastpcb.com'
);

UPDATE `Users`
SET `Role` = 1
WHERE `Email` = 'admin@fastpcb.com';
");

            migrationBuilder.Sql(@"
SET @index_exists = (
    SELECT COUNT(*)
    FROM INFORMATION_SCHEMA.STATISTICS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME = 'Users'
      AND INDEX_NAME = 'IX_Users_Role'
);
SET @sql = IF(
    @index_exists = 0,
    'CREATE INDEX `IX_Users_Role` ON `Users` (`Role`)',
    'DO 0'
);
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET @index_exists = (
    SELECT COUNT(*)
    FROM INFORMATION_SCHEMA.STATISTICS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME = 'Users'
      AND INDEX_NAME = 'IX_Users_Role'
);
SET @sql = IF(
    @index_exists = 1,
    'DROP INDEX `IX_Users_Role` ON `Users`',
    'DO 0'
);
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
");

            migrationBuilder.Sql(@"
DELETE FROM `Users`
WHERE `Email` = 'admin@fastpcb.com';
");

            migrationBuilder.Sql(@"
SET @column_exists = (
    SELECT COUNT(*)
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME = 'Users'
      AND COLUMN_NAME = 'Role'
);
SET @sql = IF(
    @column_exists = 1,
    'ALTER TABLE `Users` DROP COLUMN `Role`',
    'DO 0'
);
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
");
        }
    }
}
