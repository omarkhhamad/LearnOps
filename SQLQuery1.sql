SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET NOCOUNT ON;

-- 0. CLEANUP EXISTING DATA (To prevent duplicate errors)
-- ----------------------------------------------------------------------------
DELETE FROM Attendances;
DELETE FROM Payments;
DELETE FROM ExamResults;
DELETE FROM Exams;
DELETE FROM Enrollments;
DELETE FROM ClassGroups;
DELETE FROM Courses;
DELETE FROM Students;
DELETE FROM Instructors;
DELETE FROM AspNetUserRoles WHERE RoleId IN (SELECT Id FROM AspNetRoles WHERE Name IN ('Student', 'Instructor'));
DELETE FROM AspNetUsers WHERE Id NOT IN (SELECT UserId FROM AspNetUserRoles WHERE RoleId IN (SELECT Id FROM AspNetRoles WHERE Name = 'Admin'));

-- Reset Identity Seeds
DBCC CHECKIDENT ('Students', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('Instructors', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('Courses', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('ClassGroups', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('Exams', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('Enrollments', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('Attendances', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('Payments', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('ExamResults', RESEED, 0) WITH NO_INFOMSGS;

-- 1. SETUP ROLES
-- ----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Student')
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES (NEWID(), 'Student', 'STUDENT', NEWID());
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Instructor')
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES (NEWID(), 'Instructor', 'INSTRUCTOR', NEWID());
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Admin')
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES (NEWID(), 'Admin', 'ADMIN', NEWID());

DECLARE @StudentRoleId UNIQUEIDENTIFIER = (SELECT Id FROM AspNetRoles WHERE Name = 'Student');
DECLARE @InstructorRoleId UNIQUEIDENTIFIER = (SELECT Id FROM AspNetRoles WHERE Name = 'Instructor');
DECLARE @DefaultPasswordHash NVARCHAR(MAX) = 'AQAAAAIAAYagAAAAEJ/yX+y+S0h1X1+YWMf8Y8A6u/2Y6vL6K8yY1WqF7A=='; -- Placeholder for 'P@ssword123'

-- 2. SEED STUDENTS (Profiles + Users) - 20 ROWS
-- ----------------------------------------------------------------------------
DECLARE @StudentData TABLE (Name NVARCHAR(100), Email NVARCHAR(100), Phone NVARCHAR(20), DOB DATE);
INSERT INTO @StudentData VALUES 
('Ahmed Mohamed Ali', 'ahmed.ali@example.com', '01011111101', '2000-01-10'),
('Sara Mahmoud Hassan', 'sara.hassan@example.com', '01011111102', '1999-05-15'),
('Mohamed Ibrahim Khalil', 'm.ibrahim@example.com', '01011111103', '2001-02-20'),
('Mona Ahmed Tawfik', 'mona.ahmed@example.com', '01011111104', '2000-11-11'),
('Ali Mahmoud Zaki', 'ali.mahmoud@example.com', '01011111105', '1998-12-30'),
('Fatma Ali Nabawy', 'fatma.ali@example.com', '01011111106', '2002-03-03'),
('Omar Said Mansour', 'omar.said@example.com', '01011111107', '2001-10-15'),
('Layla Nabil Fathy', 'layla.nabil@example.com', '01011111108', '1999-08-22'),
('Hany Gaber Radwan', 'hany.gaber@example.com', '01011111109', '2000-06-06'),
('Yasmine Adel Fawzy', 'yasmine.adel@example.com', '01011111110', '1998-04-05'),
('Mostafa Samy Hegazy', 'mostafa.samy@example.com', '01011111111', '2002-09-18'),
('Reem Karim Sultan', 'reem.karim@example.com', '01011111112', '2001-01-01'),
('Tamer Adel Bakr', 'tamer.adel@example.com', '01011111113', '1999-03-03'),
('Nour Gamal Eldin', 'nour.gamal@example.com', '01011111114', '2000-05-05'),
('Amira Youssef Sabry', 'amira.youssef@example.com', '01011111115', '2001-07-07'),
('Ibrahim Salah Zaid', 'ibrahim.salah@example.com', '01011111116', '1998-09-09'),
('Salma Hossam Eissa', 'salma.hossam@example.com', '01011111117', '2002-11-11'),
('Hussein Fady Morsy', 'hussein.fady@example.com', '01011111118', '2000-12-12'),
('Rania Khalil Shaker', 'rania.khalil@example.com', '01011111119', '1999-02-02'),
('Mohamed Samir Hamad', 'mohamed.samir@example.com', '01011111120', '2001-04-04');

DECLARE @S_Name NVARCHAR(100), @S_Email NVARCHAR(100), @S_Phone NVARCHAR(20), @S_DOB DATE, @U_Id UNIQUEIDENTIFIER;
DECLARE Student_Cursor CURSOR FOR SELECT Name, Email, Phone, DOB FROM @StudentData;
OPEN Student_Cursor; FETCH NEXT FROM Student_Cursor INTO @S_Name, @S_Email, @S_Phone, @S_DOB;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @U_Id = NEWID();
    INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, FullName, CreatedAt, PhoneNumber)
    VALUES (@U_Id, @S_Email, UPPER(@S_Email), @S_Email, UPPER(@S_Email), 1, @DefaultPasswordHash, NEWID(), NEWID(), 0, 0, 1, 0, @S_Name, GETDATE(), @S_Phone);
    INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@U_Id, @StudentRoleId);
    INSERT INTO Students (UserId, DateOfBirth, CreatedAt, IsDeleted)
    VALUES (@U_Id, @S_DOB, GETDATE(), 0);
    FETCH NEXT FROM Student_Cursor INTO @S_Name, @S_Email, @S_Phone, @S_DOB;
END
CLOSE Student_Cursor; DEALLOCATE Student_Cursor;

-- 3. SEED INSTRUCTORS (Profiles + Users) - 20 ROWS

-- ----------------------------------------------------------------------------
DECLARE @InstData TABLE (Name NVARCHAR(100), Email NVARCHAR(100), Phone NVARCHAR(20), Rate DECIMAL(18,2));
INSERT INTO @InstData VALUES 
('Dr. Ahmed Ibrahim', 'a.ibrahim@university.com', '01222222201', 250),
('Eng. Sara Mahmoud', 's.mahmoud@training.com', '01222222202', 180),
('Dr. Mahmoud Zaid', 'm.zaid@university.com', '01222222203', 220),
('Eng. Ali Hassan', 'a.hassan@training.com', '01222222204', 160),
('Dr. Mona Said', 'm.said@university.com', '01222222205', 240),
('Eng. Hany Mohamed', 'h.mohamed@training.com', '01222222206', 170),
('Dr. Omar Fathy', 'o.fathy@university.com', '01222222207', 230),
('Eng. Layla Ali', 'l.ali@training.com', '01222222208', 190),
('Dr. Fatma Nabil', 'f.nabil@university.com', '01222222209', 245),
('Eng. Mostafa Gamal', 'm.gamal@training.com', '01222222210', 175),
('Dr. Reem Tarek', 'r.tarek@university.com', '01222222211', 215),
('Eng. Tamer Fadi', 't.fadi@training.com', '01222222212', 165),
('Dr. Nour Adel', 'n.adel@university.com', '01222222213', 225),
('Eng. Amira Hany', 'a.hany@training.com', '01222222214', 155),
('Dr. Ibrahim Sami', 'i.sami@university.com', '01222222215', 210),
('Eng. Salma Fathy', 's.fathy@training.com', '01222222216', 185),
('Eng. Hussein Gamal', 'h.gamal@training.com', '01222222217', 165),
('Dr. Rania Khalil', 'r.khalil@university.com', '01222222218', 255),
('Eng. Mohamed Samir', 'm.samir@training.com', '01222222219', 195),
('Dr. Eman Yousry', 'e.yousry@university.com', '01222222220', 240);

DECLARE @I_Name NVARCHAR(100), @I_Email NVARCHAR(100), @I_Phone NVARCHAR(20), @I_Rate DECIMAL(18,2);
DECLARE Inst_Cursor CURSOR FOR SELECT Name, Email, Phone, Rate FROM @InstData;
OPEN Inst_Cursor; FETCH NEXT FROM Inst_Cursor INTO @I_Name, @I_Email, @I_Phone, @I_Rate;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @U_Id = NEWID();
    INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, FullName, CreatedAt, PhoneNumber)
    VALUES (@U_Id, @I_Email, UPPER(@I_Email), @I_Email, UPPER(@I_Email), 1, @DefaultPasswordHash, NEWID(), NEWID(), 0, 0, 1, 0, @I_Name, GETDATE(), @I_Phone);
    INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@U_Id, @InstructorRoleId);
    INSERT INTO Instructors (UserId, HourlyRate, CreatedAt, IsDeleted)
    VALUES (@U_Id, @I_Rate, GETDATE(), 0);
    FETCH NEXT FROM Inst_Cursor INTO @I_Name, @I_Email, @I_Phone, @I_Rate;
END
CLOSE Inst_Cursor; DEALLOCATE Inst_Cursor;

-- 4. COURSES - 20 ROWS
-- ----------------------------------------------------------------------------
INSERT INTO Courses (Title, Description, DurationWeeks, Price, MaxStudents, IsDeleted)
VALUES 
('Full-Stack Web Development', 'Master Frontend and Backend', 12, 5000.00, 30, 0),
('Data Science with Python', 'Pandas, Scikit-Learn and ML', 10, 4500.00, 25, 0),
('Mobile App Development', 'Build Cross-platform with Flutter', 8, 4000.00, 20, 0),
('UI/UX Design Essentials', 'Figma and user-centric design', 6, 3000.00, 30, 0),
('Cloud Architecture (AWS)', 'Deploy and scale apps on cloud', 8, 5500.00, 15, 0),
('Advanced SQL & Database Design', 'Deep dive into SQL Server', 4, 2500.00, 40, 0),
('Artificial Intelligence', 'Neural networks and deep learning', 14, 7000.00, 20, 0),
('Cyber Security Basics', 'Ethical hacking and defense', 8, 4800.00, 25, 0),
('Software Testing (QA)', 'Automated testing with Selenium', 6, 3200.00, 30, 0),
('Game Development with Unity', 'Build 2D and 3D games', 10, 5000.00, 20, 0),
('DevOps Roadmap', 'CI/CD, Docker and Kubernetes', 12, 6000.00, 15, 0),
('Network Administration', 'Cisco and CCNA preparation', 8, 3500.00, 25, 0),
('Digital Marketing', 'SEO, SEM and Social Media', 6, 2800.00, 50, 0),
('Ethical Hacking Advanced', 'Advanced penetration testing', 10, 6500.00, 10, 0),
('Machine Learning Ops', 'Deploying ML models at scale', 12, 7500.00, 15, 0),
('React Native Mastery', 'Build native mobile apps with JS', 8, 4200.00, 25, 0),
('Kubernetes Administration', 'Managing container orchestrators', 6, 5000.00, 20, 0),
('Project Management (PMP)', 'Professional project management', 12, 8000.00, 20, 0),
('Business Analysis', 'Bridging business and tech', 6, 3500.00, 30, 0),
('Graphic Design Pro', 'Adobe Suite and branding', 8, 4000.00, 25, 0);

-- 5. CLASS GROUPS - 20 ROWS
-- ----------------------------------------------------------------------------
INSERT INTO ClassGroups (Name, Room, Days, Time, StartDate, EndDate, InstructorId, CourseId, IsDeleted)
VALUES 
('G01-Web-Morn', 'Room 101', 'Sun,Tue,Thu', '09:00', '2026-02-01', '2026-04-30', 1, 1, 0),
('G02-Web-Even', 'Room 102', 'Mon,Wed', '18:00', '2026-02-01', '2026-05-30', 2, 1, 0),
('G03-Data-Sat', 'Room 201', 'Sat,Mon', '15:00', '2026-02-10', '2026-04-10', 3, 2, 0),
('G04-Mob-Lab', 'Lab A', 'Fri,Sat', '10:00', '2026-03-01', '2026-05-01', 4, 3, 0),
('G05-UX-Des', 'Studio 1', 'Tue,Thu', '16:00', '2026-02-15', '2026-03-30', 5, 4, 0),
('G06-AWS-Cloud', 'Lab B', 'Mon,Wed', '20:00', '2026-02-20', '2026-04-20', 6, 5, 0),
('G07-SQL-Design', 'Room 303', 'Sun,Tue', '13:00', '2026-03-05', '2026-04-05', 7, 6, 0),
('G08-AI-Research', 'Main Hall', 'Sat,Wed', '10:00', '2026-03-01', '2026-06-15', 8, 7, 0),
('G09-Cyber-Def', 'Lab C', 'Mon,Thu', '19:00', '2026-02-25', '2026-04-25', 9, 8, 0),
('G10-Game-Unity', 'Game Lab', 'Sun,Tue', '15:00', '2026-02-01', '2026-04-15', 10, 10, 0),
('G11-DevOps-CI', 'Room 404', 'Mon,Wed', '18:00', '2026-03-01', '2026-06-01', 11, 11, 0),
('G12-Net-Admin', 'Network Lab', 'Tue,Thu', '10:00', '2026-02-10', '2026-04-10', 12, 12, 0),
('G13-Digital-Mkt', 'Room 105', 'Sat,Mon', '19:00', '2026-02-15', '2026-03-30', 13, 13, 0),
('G14-Ethical-Adv', 'Secure Lab', 'Fri,Sat', '14:00', '2026-03-01', '2026-05-15', 14, 14, 0),
('G15-MLOps-Prod', 'Lab B', 'Tue,Thu', '20:00', '2026-03-10', '2026-06-10', 15, 15, 0),
('G16-React-Mob', 'Room 101', 'Sun,Tue', '16:00', '2026-02-01', '2026-04-01', 16, 16, 0),
('G17-K8s-Master', 'Room 202', 'Mon,Wed', '14:00', '2026-03-01', '2026-04-15', 17, 17, 0),
('G18-PMP-Lead', 'Board Room', 'Sat,Tue', '18:00', '2026-02-15', '2026-05-15', 18, 18, 0),
('G19-Biz-Analyst', 'Room 303', 'Sun,Wed', '10:00', '2026-03-01', '2026-04-15', 19, 19, 0),
('G20-Graphics-Br', 'Studio 2', 'Mon,Thu', '13:00', '2026-02-01', '2026-04-01', 20, 20, 0);

-- 6. ENROLLMENTS - 20 ROWS
-- ----------------------------------------------------------------------------
INSERT INTO Enrollments (EnrollmentDate, Status, GroupId, StudentId, IsDeleted)
VALUES 
(GETDATE(), 'Active', 1, 1, 0), (GETDATE(), 'Active', 2, 2, 0),
(GETDATE(), 'Active', 3, 3, 0), (GETDATE(), 'Active', 4, 4, 0),
(GETDATE(), 'Active', 5, 5, 0), (GETDATE(), 'Active', 6, 6, 0),
(GETDATE(), 'Active', 7, 7, 0), (GETDATE(), 'Active', 8, 8, 0),
(GETDATE(), 'Active', 9, 9, 0), (GETDATE(), 'Active', 10, 10, 0),
(GETDATE(), 'Active', 11, 11, 0), (GETDATE(), 'Active', 12, 12, 0),
(GETDATE(), 'Active', 13, 13, 0), (GETDATE(), 'Active', 14, 14, 0),
(GETDATE(), 'Active', 15, 15, 0), (GETDATE(), 'Active', 16, 16, 0),
(GETDATE(), 'Active', 17, 17, 0), (GETDATE(), 'Active', 18, 18, 0),
(GETDATE(), 'Active', 19, 19, 0), (GETDATE(), 'Active', 20, 20, 0);

-- 7. EXAMS - 20 ROWS
-- ----------------------------------------------------------------------------
INSERT INTO Exams (Title, ExamDate, MaxScore, GroupId)
VALUES 
('Midterm-G01', '2026-03-01', 100, 1), ('Final-G01', '2026-04-25', 100, 1),
('Quiz-G02', '2026-02-20', 50, 2), ('Final-G02', '2026-05-25', 100, 2),
('Assessment-G03', '2026-03-10', 100, 3), ('Final-G03', '2026-04-05', 100, 3),
('Practicum-G04', '2026-04-01', 100, 4), ('Final-G04', '2026-05-01', 100, 4),
('Design-Review-G05', '2026-03-01', 50, 5), ('Final-G05', '2026-03-25', 100, 5),
('Cloud-Quiz-G06', '2026-03-15', 50, 6), ('Final-G06', '2026-04-15', 100, 6),
('SQL-Basic-G07', '2026-03-20', 100, 7), ('Final-G07', '2026-04-01', 100, 7),
('AI-Foundations-G08', '2026-04-10', 100, 8), ('Final-G08', '2026-06-10', 100, 8),
('Security-Lab-G09', '2026-03-25', 100, 9), ('Final-G09', '2026-04-20', 100, 9),
('Unity-Intro-G10', '2026-03-05', 100, 10), ('Final-G10', '2026-04-10', 100, 10);

-- 8. EXAM RESULTS - 20 ROWS
-- ----------------------------------------------------------------------------
INSERT INTO ExamResults (Score, Result, ExamId, StudentId)
VALUES 
(95, 'Excellent', 1, 1), (88, 'Very Good', 2, 1),
(76, 'Passed', 3, 2), (82, 'Good', 4, 2),
(91, 'Outstanding', 5, 3), (85, 'Great', 6, 3),
(68, 'Passed', 7, 4), (72, 'Fair', 8, 4),
(48, 'Failed - Retake needed', 9, 5), (94, 'Excellent', 10, 5),
(80, 'Good', 11, 6), (77, 'Passed', 12, 6),
(89, 'Very Good', 13, 7), (93, 'Excellent', 14, 7),
(70, 'Passed', 15, 8), (81, 'Good', 16, 8),
(87, 'Great', 17, 9), (90, 'Outstanding', 18, 9),
(75, 'Passed', 19, 10), (83, 'Good', 20, 10);

-- 9. PAYMENTS - 20 ROWS
-- ----------------------------------------------------------------------------
INSERT INTO Payments (Amount, PaymentDate, Method, EnrollmentId)
VALUES 
(5000, GETDATE(), 'Cash', 1), (4500, GETDATE(), 'Visa', 2),
(4000, GETDATE(), 'PayPal', 3), (3000, GETDATE(), 'MasterCard', 4),
(5500, GETDATE(), 'Vodafone Cash', 5), (2500, GETDATE(), 'InstaPay', 6),
(7000, GETDATE(), 'Cash', 7), (4800, GETDATE(), 'Bank Transfer', 8),
(3200, GETDATE(), 'Fawry', 9), (5000, GETDATE(), 'Cash', 10),
(6000, GETDATE(), 'Credit Card', 11), (3500, GETDATE(), 'Cash', 12),
(2800, GETDATE(), 'InstaPay', 13), (6500, GETDATE(), 'Visa', 14),
(7500, GETDATE(), 'Bank Transfer', 15), (4200, GETDATE(), 'Fawry', 16),
(5000, GETDATE(), 'PayPal', 17), (8000, GETDATE(), 'Cash', 18),
(3500, GETDATE(), 'Visa', 19), (4000, GETDATE(), 'MasterCard', 20);

-- 10. ATTENDANCES - 20 ROWS
-- ----------------------------------------------------------------------------
INSERT INTO Attendances (SessionDate, Status, EnrollmentId)
VALUES 
('2026-02-01', 1, 1), ('2026-02-01', 1, 2), ('2026-02-01', 1, 3), ('2026-02-01', 1, 4),
('2026-02-02', 1, 5), ('2026-02-02', 0, 6), ('2026-02-02', 1, 7), ('2026-02-02', 1, 8),
('2026-02-03', 0, 1), ('2026-02-03', 1, 2), ('2026-02-03', 1, 3), ('2026-02-03', 1, 4),
('2026-02-04', 1, 9), ('2026-02-04', 1, 10), ('2026-02-04', 1, 11), ('2026-02-04', 1, 12),
('2026-02-05', 1, 13), ('2026-02-05', 0, 14), ('2026-02-05', 1, 15), ('2026-02-05', 1, 16);

PRINT 'Seeding Completed: 20 Students, 20 Instructors, 20 Courses, 20 Groups, and associated data.';
