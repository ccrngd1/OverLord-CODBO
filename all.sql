SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL';

DROP SCHEMA IF EXISTS `mydb` ;
CREATE SCHEMA IF NOT EXISTS `mydb` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci ;
USE `mydb` ;

-- -----------------------------------------------------
-- Table `mydb`.`weapons`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `mydb`.`weapons` ;

CREATE  TABLE IF NOT EXISTS `mydb`.`weapons` (
  `Name` VARCHAR(25) NOT NULL ,
  `Code` VARCHAR(45) NOT NULL ,
  `isPrimary` TINYINT(1) NOT NULL ,
  `Class` VARCHAR(15) NOT NULL ,
  PRIMARY KEY (`Code`) ,
  UNIQUE INDEX `Code_UNIQUE` (`Code` ASC) )
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`attachments`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `mydb`.`attachments` ;

CREATE  TABLE IF NOT EXISTS `mydb`.`attachments` (
  `ID` INT NOT NULL ,
  `Name` VARCHAR(45) NOT NULL ,
  `Code` VARCHAR(15) NOT NULL ,
  UNIQUE INDEX `ID_UNIQUE` (`ID` ASC) ,
  PRIMARY KEY (`Code`) ,
  UNIQUE INDEX `Code_UNIQUE` (`Code` ASC) )
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Players`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `mydb`.`Players` ;

CREATE  TABLE IF NOT EXISTS `mydb`.`Players` (
  `GUID` VARCHAR(40) NOT NULL ,
  `Name` VARCHAR(45) NOT NULL ,
  `Kills` INT NOT NULL ,
  `Deaths` INT NOT NULL ,
  `status` INT NOT NULL ,
  `banned` TINYINT(1) NOT NULL ,
  PRIMARY KEY (`GUID`) ,
  UNIQUE INDEX `GUID_UNIQUE` (`GUID` ASC) )
ENGINE = InnoDB;



SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
