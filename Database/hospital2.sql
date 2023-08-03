CREATE DATABASE HOSPITAL2
USE HOSPITAL2

CREATE TABLE Patient(
	PatientID VARCHAR(10) PRIMARY KEY,
	Name VARCHAR(50),
	Surname VARCHAR(50),
	Gender CHAR(1)
	CHECK (Gender IN ('M','F')),
	Birthday DATE,
	Address VARCHAR(255),
	Phone VARCHAR(20)
);

CREATE TABLE Receptionist(
	ReceptionistID VARCHAR(10) PRIMARY KEY,
	Name VARCHAR(50),
	Surname VARCHAR(50),
	Email VARCHAR(50)
);

CREATE TABLE Specialization( --Sherbime
	SpecializationID VARCHAR(10) PRIMARY KEY,
	Name VARCHAR(50),
	Description VARCHAR(255),
	PhotoURL VARCHAR(255)
);

CREATE TABLE Doctor(
	DoctorID VARCHAR(10) PRIMARY KEY,
	Name VARCHAR(50),
	Surname VARCHAR(50),
	Email VARCHAR(50),
	Education VARCHAR(255),
	PhotoURL VARCHAR(255),
	Specialization VARCHAR(10) FOREIGN KEY REFERENCES Specialization(SpecializationID)
);

CREATE TABLE PatientDoctor(
	PatientID VARCHAR(10),
	DoctorID VARCHAR(10),
	PRIMARY KEY (PatientID, DoctorID),
	Patient VARCHAR(10) FOREIGN KEY REFERENCES Patient(PatientID),
	Doctor VARCHAR(10) FOREIGN KEY REFERENCES Doctor(DoctorID)
);

CREATE TABLE Room( 
	RoomID VARCHAR(10) PRIMARY KEY,
	Patient VARCHAR(10) UNIQUE FOREIGN KEY REFERENCES Patient(PatientID)
);

CREATE TABLE Report(
	ReportID VARCHAR(10) PRIMARY KEY,
	ReportType VARCHAR(255),
	ReportDate DATE,
	ReportDescription VARCHAR(255),
	Patient VARCHAR(10) FOREIGN KEY REFERENCES Patient(PatientID),
	Doctor VARCHAR(10) FOREIGN KEY REFERENCES Doctor(DoctorID)
);

CREATE TABLE Reservation(
	ReservationID VARCHAR(10) PRIMARY KEY,
	ReservationDate DATE,
	ReservationTime TIME,
	Patient VARCHAR(10) FOREIGN KEY REFERENCES Patient(PatientID),
	Doctor VARCHAR(10) FOREIGN KEY REFERENCES Doctor(DoctorID),
	Service VARCHAR(10) FOREIGN KEY REFERENCES Specialization(SpecializationID)
);

CREATE TABLE Payment(
	PaymentID VARCHAR(10) PRIMARY KEY,
	PaymentAmount VARCHAR(20),
	PaymentDate DATE,
	Receptionist VARCHAR(10) FOREIGN KEY REFERENCES Receptionist(ReceptionistID),
	Patient VARCHAR(10) FOREIGN KEY REFERENCES Patient(PatientID),
	Report VARCHAR(10) FOREIGN KEY REFERENCES Report(ReportID)
);

CREATE TABLE Complaints(
	ComplaintID VARCHAR(10) PRIMARY KEY,
	ComplaintDate DATE,
	ComplaintDetails VARCHAR(255),
	Patient VARCHAR(10) FOREIGN KEY REFERENCES Patient(PatientID)
);

CREATE TABLE ContactForm(
	ContactID VARCHAR(10) PRIMARY KEY,
	Subject VARCHAR(100),
	Message VARCHAR(255),
	Patient VARCHAR(10) FOREIGN KEY REFERENCES Patient(PatientID)
);