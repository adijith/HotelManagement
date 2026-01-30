USE ClientCalendarDB;
GO

-- Create Users table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        UserId INT IDENTITY(1,1) PRIMARY KEY,
        Username VARCHAR(100) NOT NULL UNIQUE,
        Email VARCHAR(100) NOT NULL UNIQUE,
        PasswordHash VARCHAR(500) NOT NULL,
        CreatedAt DATETIME DEFAULT GETDATE()
    );
    
    PRINT 'Users table created successfully';
END
ELSE
BEGIN
    PRINT 'Users table already exists';
END
GO

-- Insert a default admin user (password: admin123)
-- Password hash is SHA256 of "admin123"
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, Email, PasswordHash)
    VALUES ('admin', 'admin@example.com', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=');

    PRINT 'Default admin user created (username: admin, password: admin123)';
END
GO

