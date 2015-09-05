/*
Navicat MySQL Data Transfer

Source Server         : remote
Source Server Version : 50544
Source Host           : localhost:3306
Source Database       : Auth

Target Server Type    : MYSQL
Target Server Version : 50544
File Encoding         : 65001

Date: 2015-08-17 12:43:53
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for Accounts
-- ----------------------------
DROP TABLE IF EXISTS `Accounts`;
CREATE TABLE `Accounts` (
  `Id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Alias` varchar(255) NOT NULL DEFAULT '',
  `Email` varchar(255) NOT NULL,
  `PasswordVerifier` varchar(256) NOT NULL,
  `Salt` varchar(16) NOT NULL,
  `GatewayTicket` varchar(32) DEFAULT NULL,
  `Online` tinyint(1) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of Accounts
-- ----------------------------

-- ----------------------------
-- Table structure for Realms
-- ----------------------------
DROP TABLE IF EXISTS `Realms`;
CREATE TABLE `Realms` (
  `Id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) DEFAULT NULL,
  `IP` varchar(255) DEFAULT '127.0.0.1',
  `Port` smallint(5) unsigned NOT NULL DEFAULT '24000',
  `Type` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `Status` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `Population` tinyint(3) unsigned NOT NULL DEFAULT '0'
  `Index` int(10) unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of Realms
-- ----------------------------
INSERT INTO `Realms` VALUES ('1', 'Arctium Emulation', '127.0.0.1', '24000', '0', '1', '0');

-- ----------------------------
-- Table structure for Redirects
-- ----------------------------
DROP TABLE IF EXISTS `Redirects`;
CREATE TABLE `Redirects` (
  `AccountId` int(10) unsigned NOT NULL,
  `IP` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`AccountId`),
  CONSTRAINT `Redirects_ibfk_1` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of Redirects
-- ----------------------------
