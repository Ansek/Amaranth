-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)
--
-- Host: localhost    Database: amaranth
-- ------------------------------------------------------
-- Server version	8.0.32

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `Order`
--

DROP TABLE IF EXISTS `Order`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Order` (
  `idOrder` int NOT NULL AUTO_INCREMENT,
  `CreationDate` datetime NOT NULL,
  `CompletionDate` datetime DEFAULT NULL,
  PRIMARY KEY (`idOrder`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Order`
--

LOCK TABLES `Order` WRITE;
/*!40000 ALTER TABLE `Order` DISABLE KEYS */;
/*!40000 ALTER TABLE `Order` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Product_Tag`
--

DROP TABLE IF EXISTS `Product_Tag`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Product_Tag` (
  `idProduct` int NOT NULL,
  `idTag` int NOT NULL,
  PRIMARY KEY (`idProduct`,`idTag`),
  KEY `fk_Tag_Product_idx` (`idTag`) /*!80000 INVISIBLE */,
  CONSTRAINT `fk_Product_Tag` FOREIGN KEY (`idProduct`) REFERENCES `Product` (`idProduct`),
  CONSTRAINT `fk_Tag_Product` FOREIGN KEY (`idTag`) REFERENCES `Tag` (`idTag`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Product_Tag`
--

LOCK TABLES `Product_Tag` WRITE;
/*!40000 ALTER TABLE `Product_Tag` DISABLE KEYS */;
/*!40000 ALTER TABLE `Product_Tag` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Description`
--

DROP TABLE IF EXISTS `Description`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Description` (
  `idDescription` int NOT NULL,
  `idCategory` int NOT NULL,
  `Title` varchar(64) NOT NULL,
  PRIMARY KEY (`idDescription`,`idCategory`),
  KEY `fk_Descriptiont_Category_idx` (`idCategory`) /*!80000 INVISIBLE */,
  CONSTRAINT `fk_Description_Category` FOREIGN KEY (`idCategory`) REFERENCES `Category` (`idCategory`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Description`
--

--
-- Table structure for table `Category`
--

DROP TABLE IF EXISTS `Category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Category` (
  `idCategory` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(32) NOT NULL,
  PRIMARY KEY (`idCategory`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Category`
--

--
-- Temporary view structure for view `ProductView`
--

DROP TABLE IF EXISTS `ProductView`;
/*!50001 DROP VIEW IF EXISTS `ProductView`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `ProductView` AS SELECT 
 1 AS `idProduct`,
 1 AS `Title`,
 1 AS `Price`,
 1 AS `Count`,
 1 AS `Prescription`,
 1 AS `idCategory`,
 1 AS `Reserve`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `SaleView`
--

DROP TABLE IF EXISTS `SaleView`;
/*!50001 DROP VIEW IF EXISTS `SaleView`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `SaleView` AS SELECT 
 1 AS `idProduct`,
 1 AS `Title`,
 1 AS `Count`,
 1 AS `Cost`,
 1 AS `Prescription`,
 1 AS `Category`,
 1 AS `Date`*/;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `Product`
--

DROP TABLE IF EXISTS `Product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Product` (
  `idProduct` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(64) NOT NULL,
  `Price` decimal(10,2) NOT NULL,
  `Count` int NOT NULL DEFAULT '0',
  `Prescription` bit(1) NOT NULL,
  `idCategory` int NOT NULL,
  PRIMARY KEY (`idProduct`),
  KEY `fk_Product_Category_idx` (`idCategory`) /*!80000 INVISIBLE */,
  CONSTRAINT `fk_Product_Category` FOREIGN KEY (`idCategory`) REFERENCES `Category` (`idCategory`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Product`
--

LOCK TABLES `Product` WRITE;
/*!40000 ALTER TABLE `Product` DISABLE KEYS */;
/*!40000 ALTER TABLE `Product` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Tag`
--

DROP TABLE IF EXISTS `Tag`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Tag` (
  `idTag` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`idTag`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Tag`
--

LOCK TABLES `Tag` WRITE;
/*!40000 ALTER TABLE `Tag` DISABLE KEYS */;
/*!40000 ALTER TABLE `Tag` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `User`
--

DROP TABLE IF EXISTS `User`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `User` (
  `Login` varchar(32) NOT NULL,
  `FirstName` varchar(32) NOT NULL,
  `LastName` varchar(64) NOT NULL,
  `Password` char(60) DEFAULT '',
  `IsAdministrator` bit(1) NOT NULL,
  PRIMARY KEY (`Login`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `User`
--

--
-- Table structure for table `Order_Product`
--

DROP TABLE IF EXISTS `Order_Product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Order_Product` (
  `idOrder` int NOT NULL,
  `idProduct` int NOT NULL,
  `Count` int NOT NULL,
  `Price` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`idOrder`,`idProduct`),
  KEY `fk_Product_Order_idx` (`idProduct`) /*!80000 INVISIBLE */,
  CONSTRAINT `fk_Order_Product` FOREIGN KEY (`idOrder`) REFERENCES `Order` (`idOrder`),
  CONSTRAINT `fk_Product_Order` FOREIGN KEY (`idProduct`) REFERENCES `Product` (`idProduct`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Order_Product`
--

LOCK TABLES `Order_Product` WRITE;
/*!40000 ALTER TABLE `Order_Product` DISABLE KEYS */;
/*!40000 ALTER TABLE `Order_Product` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Final view structure for view `ProductView`
--

/*!50001 DROP VIEW IF EXISTS `ProductView`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`alexander`@`%` SQL SECURITY DEFINER */
/*!50001 VIEW `ProductView` AS select `p`.`idProduct` AS `idProduct`,`p`.`Title` AS `Title`,`p`.`Price` AS `Price`,`p`.`Count` AS `Count`,`p`.`Prescription` AS `Prescription`,`p`.`idCategory` AS `idCategory`,(select sum(`op`.`Count`) from (`Order` `o` join `Order_Product` `op`) where ((`o`.`idOrder` = `op`.`idOrder`) and (`op`.`idProduct` = `p`.`idProduct`) and (`o`.`CompletionDate` is null)) group by `op`.`idProduct`) AS `Reserve` from (`Product` `p` left join `Order_Product` `op` on((`p`.`idProduct` = `op`.`idProduct`))) group by `p`.`idProduct` order by `p`.`Title` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `SaleView`
--

/*!50001 DROP VIEW IF EXISTS `SaleView`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`alexander`@`%` SQL SECURITY DEFINER */
/*!50001 VIEW `SaleView` AS select `p`.`idProduct` AS `idProduct`,`p`.`Title` AS `Title`,sum(`op`.`Count`) AS `Count`,(sum(`op`.`Count`) * `p`.`Price`) AS `Cost`,`p`.`Prescription` AS `Prescription`,`c`.`Title` AS `Category`,cast(`o`.`CompletionDate` as date) AS `Date` from (((`Product` `p` join `Category` `c` on((`p`.`idCategory` = `c`.`idCategory`))) left join `Order_Product` `op` on((`p`.`idProduct` = `op`.`idProduct`))) left join `Order` `o` on((`op`.`idOrder` = `o`.`idOrder`))) where (`o`.`CompletionDate` is not null) group by `p`.`idProduct`,`Date` order by `Count` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-08-29  2:18:45
