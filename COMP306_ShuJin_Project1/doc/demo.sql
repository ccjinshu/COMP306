-- �����û�����
INSERT INTO dbo.Users (Name, Email, PhoneNumber, Password) 
VALUES ('John Doe', 'john.doe@example.com', '123-456-7890', 'password123');

INSERT INTO dbo.Users (Name, Email, PhoneNumber, Password) 
VALUES ('Jane Smith', 'jane.smith@example.com', '098-765-4321', 'password456');

-- ���뷿������
INSERT INTO dbo.Rooms (Number, Type, Price, Status) 
VALUES ('101', 'Standard', 80.00, 'Available');

INSERT INTO dbo.Rooms (Number, Type, Price, Status) 
VALUES ('102', 'Deluxe', 120.00, 'Available');

INSERT INTO dbo.Rooms (Number, Type, Price, Status) 
VALUES ('103', 'Suite', 200.00, 'Occupied');

-- ����Ԥ������
-- ע��: StartDate �� EndDate Ӧ��ѭ 'YYYY-MM-DD' ��ʽ��
INSERT INTO dbo.Bookings (UserId, RoomId, StartDate, EndDate, TotalPrice) 
VALUES ((SELECT Id FROM dbo.Users WHERE Email = 'john.doe@example.com'), 
        (SELECT Id FROM dbo.Rooms WHERE Number = '101'), 
        '2023-07-01', '2023-07-05', 320.00);

INSERT INTO dbo.Bookings (UserId, RoomId, StartDate, EndDate, TotalPrice) 
VALUES ((SELECT Id FROM dbo.Users WHERE Email = 'jane.smith@example.com'), 
        (SELECT Id FROM dbo.Rooms WHERE Number = '102'), 
        '2023-08-15', '2023-08-20', 600.00);
