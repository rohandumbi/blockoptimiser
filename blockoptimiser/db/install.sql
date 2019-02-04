IF OBJECT_ID('Project', 'U') IS NOT NULL 
DROP TABLE Project;

CREATE TABLE Project (
   Id INT IDENTITY(1,1) PRIMARY KEY,
   Name VARCHAR(50) NOT NULL,
   Description VARCHAR(400),
   CreatedDate	DATETIME,
   ModifiedDate DATETIME,
   UNIQUE (Name)
);

IF OBJECT_ID('Model', 'U') IS NOT NULL 
DROP TABLE Model;

CREATE TABLE Model (
   Id INT IDENTITY(1,1) PRIMARY KEY,
   ProjectId INT,
   Name VARCHAR(50) NOT NULL,
   Bearing INT,
   HasData BIT,
   UNIQUE (ProjectId, Name)
);

IF OBJECT_ID('ModelDimension', 'U') IS NOT NULL 
DROP TABLE ModelDimension;

CREATE TABLE ModelDimension (
   Id INT IDENTITY(1,1) PRIMARY KEY,
   ModelId INT,
   Type VARCHAR(50) NOT NULL,
   XDim DECIMAL(18, 10),
   YDim DECIMAL(18, 10),
   ZDim DECIMAL(18, 10),
   UNIQUE (ModelId, Type)
);

IF OBJECT_ID('Field', 'U') IS NOT NULL 
DROP TABLE Field;

CREATE TABLE Field(
   Id INT IDENTITY(1,1) PRIMARY KEY,
   ProjectId INT,
   Name VARCHAR(100),
   DataType INT,
   AssociatedField INT,
   UNIQUE (ProjectId, Name)
);

IF OBJECT_ID('CsvColumnMapping', 'U') IS NOT NULL 
DROP TABLE CsvColumnMapping;

CREATE TABLE CsvColumnMapping(
   Id INT IDENTITY(1,1) PRIMARY KEY,
   ModelId INT,
   ColumnName VARCHAR(100),
   FieldId INT,
   DefaultValue VARCHAR(100),
   UNIQUE (ModelId, FieldId)
);

IF OBJECT_ID('RequiredFieldMapping', 'U') IS NOT NULL 
DROP TABLE RequiredFieldMapping;

CREATE TABLE RequiredFieldMapping(
   Id INT IDENTITY(1,1) PRIMARY KEY,
   ProjectId INT,
   RequiredFieldName VARCHAR(100),
   MappedColumnName VARCHAR(100),
   UNIQUE (ProjectId, RequiredFieldName)
);

IF OBJECT_ID('Geotech', 'U') IS NOT NULL 
DROP TABLE Geotech

CREATE TABLE Geotech(
   Id INT IDENTITY(1,1) PRIMARY KEY,
   ProjectId INT,
   ModelId INT,
   Type INT,
   FieldId INT,
   UseScript BIT,
   Script VARCHAR(200),
   UNIQUE (ModelId)
);

IF OBJECT_ID('Process', 'U') IS NOT NULL 
DROP TABLE Process

CREATE TABLE Process(
   Id INT IDENTITY(1,1) PRIMARY KEY,
   ProjectId INT,
   Name VARCHAR(100),
   UNIQUE (ProjectId, Name)
);

IF OBJECT_ID('Product', 'U') IS NOT NULL 
DROP TABLE Product

CREATE TABLE Product(
   Id INT IDENTITY(1,1) PRIMARY KEY,
   ProjectId INT,
   Name VARCHAR(100),
   UNIQUE (ProjectId, Name)
);

IF OBJECT_ID('Expression', 'U') IS NOT NULL 
DROP TABLE Expression

CREATE TABLE Expression (
   id INT IDENTITY(1,1) PRIMARY KEY,
   ProjectId INT,
   Name VARCHAR(100),
   UNIQUE (ProjectId, Name)
);
IF OBJECT_ID('ExprModelMapping', 'U') IS NOT NULL 
DROP TABLE ExprModelMapping

CREATE TABLE ExprModelMapping (
   id INT IDENTITY(1,1) PRIMARY KEY,
   ExprId INT,
   ModelId INT,
   ExprString VARCHAR(400),
   UNIQUE (ExprId, ModelId)
);