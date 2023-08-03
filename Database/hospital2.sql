CREATE DATABASE HOSPITAL2
USE HOSPITAL2

CREATE TABLE Patient(
	PatientID INT PRIMARY KEY IDENTITY(1,1),
	Name VARCHAR(50),
	Surname VARCHAR(50),
	Gender CHAR(1)
	CHECK (Gender IN ('M','F')),
	Birthday DATE,
	Address VARCHAR(255),
	Phone VARCHAR(20)
);

CREATE TABLE Receptionist(
	ReceptionistID INT PRIMARY KEY IDENTITY(1,1),
	Name VARCHAR(50),
	Surname VARCHAR(50),
	Email VARCHAR(50)
);

CREATE TABLE Specialization( --Sherbime
	SpecializationID INT PRIMARY KEY IDENTITY(1,1),
	Name VARCHAR(50),
	Description VARCHAR(255),
	PhotoURL VARCHAR(255)
);

CREATE TABLE Doctor(
	DoctorID INT PRIMARY KEY IDENTITY(1,1),
	Name VARCHAR(50),
	Surname VARCHAR(50),
	Email VARCHAR(50),
	Education VARCHAR(255),
	PhotoURL VARCHAR(255),
	Specialization INT FOREIGN KEY REFERENCES Specialization(SpecializationID)
);

CREATE TABLE PatientDoctor(
	PatientID INT,
	DoctorID INT,
	PRIMARY KEY (PatientID, DoctorID),
	Patient INT FOREIGN KEY REFERENCES Patient(PatientID),
	Doctor INT FOREIGN KEY REFERENCES Doctor(DoctorID)
);

CREATE TABLE Room( 
	RoomID INT PRIMARY KEY IDENTITY(1,1),
	Patient INT UNIQUE FOREIGN KEY REFERENCES Patient(PatientID)
);

CREATE TABLE Report(
	ReportID INT PRIMARY KEY IDENTITY(1,1),
	ReportType VARCHAR(255),
	ReportDate DATE,
	ReportDescription VARCHAR(255),
	Patient INT FOREIGN KEY REFERENCES Patient(PatientID),
	Doctor INT FOREIGN KEY REFERENCES Doctor(DoctorID)
);

CREATE TABLE Reservation(
	ReservationID INT PRIMARY KEY IDENTITY(1,1),
	ReservationDate DATE,
	ReservationTime TIME,
	Patient INT FOREIGN KEY REFERENCES Patient(PatientID),
	Doctor INT FOREIGN KEY REFERENCES Doctor(DoctorID),
	Service INT FOREIGN KEY REFERENCES Specialization(SpecializationID)
);

CREATE TABLE Payment(
	PaymentID INT PRIMARY KEY IDENTITY(1,1),
	PaymentAmount VARCHAR(20),
	PaymentDate DATE,
	Receptionist INT FOREIGN KEY REFERENCES Receptionist(ReceptionistID),
	Patient INT FOREIGN KEY REFERENCES Patient(PatientID),
	Report INT FOREIGN KEY REFERENCES Report(ReportID)
);

CREATE TABLE Complaints(
	ComplaintID INT PRIMARY KEY IDENTITY(1,1),
	ComplaintDate DATE,
	ComplaintDetails VARCHAR(255),
	Patient INT FOREIGN KEY REFERENCES Patient(PatientID)
);

CREATE TABLE ContactForm(
	ContactID INT PRIMARY KEY IDENTITY(1,1),
	Subject VARCHAR(100),
	Message VARCHAR(255),
	Patient INT FOREIGN KEY REFERENCES Patient(PatientID)
);
CREATE TABLE Role (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(50)
);
ALTER TABLE Patient
ADD RoleID INT FOREIGN KEY REFERENCES Role(RoleID);

ALTER TABLE Doctor
ADD RoleID INT FOREIGN KEY REFERENCES Role(RoleID);

ALTER TABLE Receptionist
ADD RoleID INT FOREIGN KEY REFERENCES Role(RoleID);

CREATE TABLE UserRole (
    UserID INT,
    RoleID INT,
    PRIMARY KEY (UserID, RoleID),
    FOREIGN KEY (UserID) REFERENCES Patient(PatientID) ON DELETE CASCADE,
    FOREIGN KEY (UserID) REFERENCES Doctor(DoctorID) ON DELETE CASCADE,
    FOREIGN KEY (UserID) REFERENCES Receptionist(ReceptionistID) ON DELETE CASCADE,
    FOREIGN KEY (RoleID) REFERENCES Role(RoleID)
);
INSERT INTO Role (Name) VALUES
('Administrator'),
('Doctor'),
('Receptionist'),
('Patient');

SELECT * FROM Role



