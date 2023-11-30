-- 插入用户数据
INSERT INTO dbo.Users (Name, Email, PhoneNumber, Password) 
VALUES ('John Doe', 'john.doe@example.com', '123-456-7890', 'password123');

INSERT INTO dbo.Users (Name, Email, PhoneNumber, Password) 
VALUES ('Jane Smith', 'jane.smith@example.com', '098-765-4321', 'password456');

-- 插入房间数据
INSERT INTO dbo.Rooms (Number, Type, Price, Status) 
VALUES ('101', 'Standard', 80.00, 'Available');

INSERT INTO dbo.Rooms (Number, Type, Price, Status) 
VALUES ('102', 'Deluxe', 120.00, 'Available');

INSERT INTO dbo.Rooms (Number, Type, Price, Status) 
VALUES ('103', 'Suite', 200.00, 'Occupied');

-- 插入预订数据
-- 注意: StartDate 和 EndDate 应遵循 'YYYY-MM-DD' 格式。
INSERT INTO dbo.Bookings (UserId, RoomId, StartDate, EndDate, TotalPrice) 
VALUES ((SELECT Id FROM dbo.Users WHERE Email = 'john.doe@example.com'), 
        (SELECT Id FROM dbo.Rooms WHERE Number = '101'), 
        '2023-07-01', '2023-07-05', 320.00);

INSERT INTO dbo.Bookings (UserId, RoomId, StartDate, EndDate, TotalPrice) 
VALUES ((SELECT Id FROM dbo.Users WHERE Email = 'jane.smith@example.com'), 
        (SELECT Id FROM dbo.Rooms WHERE Number = '102'), 
        '2023-08-15', '2023-08-20', 600.00);
