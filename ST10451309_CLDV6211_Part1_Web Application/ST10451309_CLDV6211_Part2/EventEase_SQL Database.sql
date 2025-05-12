Create database EventEase2;

USE EventEase2;

--DROP TABLE IF EXISTS Booking;
--DROP TABLE IF EXISTS Event;
--DROP TABLE IF EXISTS Venue;

CREATE TABLE Venue (
    VenueId INT IDENTITY(1,1) PRIMARY KEY,  -- Auto-increments the VenueId
    VenueName NVARCHAR(100) NOT NULL UNIQUE,  -- Ensures no duplicate venue names
    Location NVARCHAR(200) NOT NULL,
    Capacity INT NOT NULL,
    ImageUrl NVARCHAR(2000)  -- Stores a URL for venue images
);

CREATE TABLE Event (
    EventId INT IDENTITY(1,1) PRIMARY KEY,  -- Auto-increments the EventId
    EventName NVARCHAR(100) NOT NULL,
    EventDate DATETIME NOT NULL,
    Description NVARCHAR(MAX),
    VenueId INT,  -- Foreign key linking to the Venue table
    FOREIGN KEY (VenueId) REFERENCES Venue(VenueId) ON DELETE CASCADE
);

CREATE TABLE Booking (
    BookingId INT IDENTITY(1,1) PRIMARY KEY,  -- Auto-increments the BookingId
    EventId INT NOT NULL,  -- Foreign key linking to Event
    VenueId INT NOT NULL,  -- Foreign key linking to Venue
    BookingDate DATETIME NOT NULL DEFAULT GETDATE(),  -- It will automatically adds the current date
    FOREIGN KEY (EventId) REFERENCES Event(EventId) ON DELETE CASCADE, -- (ON DELETE CASCADE) If an event is deleted, the booking gets deleted as well
    FOREIGN KEY (VenueId) REFERENCES Venue(VenueId) ON DELETE NO ACTION,   -- sinec i cant have them both on "ON DELETE CASCADE" I will use "On delete set null
	
);

Create Unique Index UQ_Venue_Booking ON Booking (VenueId, BookingDate);

INSERT INTO Venue (VenueName, Location, Capacity, ImageUrl)
VALUES ('The FNB Staduim', 'South Africa',100000, 'https://populous.com/uploads/2018/01/GettyImages-102810965-Do-not-use-after-31MAR2025.jpg');

-- Insert sample event data (linked to the existing venue)
INSERT INTO Event (VenueId, EventName, EventDate, Description)
VALUES (2, 'Birthday party', '2025-07-28', 'A grand music festival.');

-- Insert sample booking data (linked to the event and venue)
INSERT INTO Booking (EventId, VenueId, BookingDate)
VALUES (3, 2, '2025-05-15');

Alter table Booking
Add Constraint UQ_Venue_Event UNIQUE (VenueId, EventId)



-- View the inserted data
SELECT * FROM Venue;
SELECT * FROM Event;
SELECT * FROM Booking;