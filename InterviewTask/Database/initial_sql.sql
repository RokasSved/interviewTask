CREATE TABLE Vessels
    (
            Id INTEGER PRIMARY KEY,
            Name TEXT NOT NULL
    );
CREATE TABLE CrewMember
    (
            Id INTEGER PRIMARY KEY,
            VesselId INTEGER NOT NULL,
            FirstName TEXT NOT NULL,
            LastName TEXT NOT NULL,
            EmailAddress TEXT NULL
    );