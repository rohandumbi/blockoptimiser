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