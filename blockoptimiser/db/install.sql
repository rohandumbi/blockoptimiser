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

IF OBJECT_ID('ProjectData', 'U') IS NOT NULL 
DROP TABLE ProjectData;

CREATE TABLE ProjectData (
   Id INT IDENTITY(1,1) PRIMARY KEY,
   ProjectId INT,
   Name VARCHAR(50) NOT NULL,
   Bearing DECIMAL(18,10),
   UNIQUE (ProjectId, Name)
);