/*
Date: 2015-07-30 20:18:31
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for Accounts
-- ----------------------------
DROP TABLE IF EXISTS `Accounts`;
CREATE TABLE `Accounts` (
  `Id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Email` varchar(255) NOT NULL DEFAULT '',
  `LoginName` varchar(255) NOT NULL,
  `PasswordVerifier` varchar(256) NOT NULL,
  `Salt` varchar(16) NOT NULL,
  `Online` tinyint(1) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of Accounts
-- ----------------------------

-- ----------------------------
-- Table structure for GameAccounts
-- ----------------------------
DROP TABLE IF EXISTS `GameAccounts`;
CREATE TABLE `GameAccounts` (
  `Id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `AccountId` int(10) unsigned NOT NULL,
  `Alias` varchar(255) CHARACTER SET latin1 DEFAULT '',
  `Created` varchar(255) CHARACTER SET latin1 DEFAULT '',
  PRIMARY KEY (`Id`,`AccountId`),
  KEY `AccountId` (`AccountId`),
  FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of GameAccounts
-- ----------------------------
