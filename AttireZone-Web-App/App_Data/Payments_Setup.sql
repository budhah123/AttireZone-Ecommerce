CREATE TABLE Payments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    TransactionUuid NVARCHAR(100) NOT NULL UNIQUE,
    GatewayTransactionId NVARCHAR(200) NULL,
    PaymentMethod NVARCHAR(20) NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    GatewayResponse NVARCHAR(MAX) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    VerifiedAt DATETIME NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id)
);
