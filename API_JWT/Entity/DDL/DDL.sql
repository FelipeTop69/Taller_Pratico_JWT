CREATE TABLE [User] (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL,
    Password NVARCHAR(100) NOT NULL,
);

CREATE TABLE Role  (
	Id INT PRIMARY KEY IDENTITY(1,1),
	Name NVARCHAR(50),
	Description NVARCHAR(200)
);

CREATE TABLE UserRole(
	Id INT PRIMARY KEY IDENTITY(1,1),
	UserId INT,
	RoleId INT
);

-- REFERENCIAS
ALTER TABLE UserRole
ADD CONSTRAINT FK_UserRole_User FOREIGN KEY (UserId) REFERENCES [User](Id) ON DELETE CASCADE,
CONSTRAINT FK_UserRole_Role FOREIGN KEY (RoleId) REFERENCES  Role(Id) ON DELETE CASCADE;
