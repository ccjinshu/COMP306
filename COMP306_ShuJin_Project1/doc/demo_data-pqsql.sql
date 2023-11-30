 
-- Insert data into 'Rooms'
INSERT INTO "Rooms" ("Id", "Number", "Type", "Price", "Status", "Desc") VALUES
(1, '101', 'Single', 100.00, 'Available', 'Cozy single room with a view'),
(2, '102', 'Single', 100.00, 'Occupied', 'Single room with excellent lighting'),
(3, '103', 'Double', 150.00, 'Available', 'Spacious double room'),
(4, '104', 'Double', 150.00, 'Occupied', 'Double room with two beds'),
(5, '105', 'Suite', 300.00, 'Available', 'Luxurious suite with a balcony'),
(6, '201', 'Single', 120.00, 'Available', 'Single room with extra amenities'),
(7, '202', 'Double', 180.00, 'Available', 'Double room with modern decor'),
(8, '203', 'Double', 180.00, 'Occupied', 'Elegant room with a double bed'),
(9, '204', 'Suite', 350.00, 'Available', 'Exclusive suite with a minibar'),
(10, '301', 'Single', 130.00, 'Available', 'Single room on top floor');

-- Insert data into 'Users'
INSERT INTO "Users" ("Id", "Name", "Email", "PhoneNumber", "Password") VALUES
(1, 'John Doe', 'john.doe@example.com', '1234567890', 'hashed_password'),
(2, 'Jane Smith', 'jane.smith@example.com', '0987654321', 'hashed_password'),
(3, 'Alice Johnson', 'alice.johnson@example.com', '2345678901', 'hashed_password'),
(4, 'Bob Brown', 'bob.brown@example.com', '3456789012', 'hashed_password'),
(5, 'Charlie Davis', 'charlie.davis@example.com', '4567890123', 'hashed_password'),
(6, 'Diana Evans', 'diana.evans@example.com', '5678901234', 'hashed_password'),
(7, 'Edward Fox', 'edward.fox@example.com', '6789012345', 'hashed_password'),
(8, 'Grace Hill', 'grace.hill@example.com', '7890123456', 'hashed_password'),
(9, 'Henry Ice', 'henry.ice@example.com', '8901234567', 'hashed_password'),
(10, 'Irene Joker', 'irene.joker@example.com', '9012345678', 'hashed_password');

-- Insert data into 'Bookings'
INSERT INTO "Bookings" ("Id", "UserId", "RoomId", "StartDate", "EndDate", "TotalPrice") VALUES
(1, 1, 1, '2023-07-01', '2023-07-03', 200.00),
(2, 2, 2, '2023-07-04', '2023-07-06', 200.00);
