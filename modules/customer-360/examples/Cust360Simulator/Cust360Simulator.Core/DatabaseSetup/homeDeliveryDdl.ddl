-- This script alters the debezium example to
-- change the 'customers' table to be more like
-- my Customer 360 tutorial requirements

ALTER TABLE `customers`
	ALTER `first_name` DROP DEFAULT;
ALTER TABLE `customers`
	CHANGE COLUMN `first_name` `name` VARCHAR(500) NOT NULL AFTER `id`,
	ADD COLUMN `password` VARCHAR(255) NOT NULL AFTER `email`,
	ADD COLUMN `address_line_1` VARCHAR(255) NOT NULL AFTER `password`,
	ADD COLUMN `address_line_2` VARCHAR(255) NOT NULL AFTER `address_line_1`,
	ADD COLUMN `city` VARCHAR(255) NOT NULL AFTER `address_line_2`,
	ADD COLUMN `state` VARCHAR(2) NOT NULL AFTER `city`,
	ADD COLUMN `zipcode` VARCHAR(10) NOT NULL AFTER `state`,
	ADD COLUMN `phonenumber` VARCHAR(25) NOT NULL AFTER `zipcode`,
	DROP COLUMN `last_name`;

ALTER TABLE `customers`
	ALTER `address_line_2` DROP DEFAULT;
ALTER TABLE `customers`
	CHANGE COLUMN `address_line_2` `address_line_2` VARCHAR(255) NULL AFTER `address_line_1`;
