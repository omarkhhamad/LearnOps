import re
import os

filepath = r"e:\ITI\Review\LearnOps\SQLQuery1.sql"
output_path = r"e:\ITI\Review\LearnOps\SQLQuery1_Updated.sql"

with open(filepath, 'r', encoding='utf-8') as f:
    content = f.read()

# 1. Setup Roles and initial common data
header = """-- =============================================
-- Global Setup (Roles and Common Data)
-- =============================================
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Student')
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES (NEWID(), 'Student', 'STUDENT', NEWID());
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Instructor')
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES (NEWID(), 'Instructor', 'INSTRUCTOR', NEWID());
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Admin')
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES (NEWID(), 'Admin', 'ADMIN', NEWID());

DECLARE @StudentRoleId UNIQUEIDENTIFIER = (SELECT Id FROM AspNetRoles WHERE Name = 'Student');
DECLARE @InstructorRoleId UNIQUEIDENTIFIER = (SELECT Id FROM AspNetRoles WHERE Name = 'Instructor');
DECLARE @DefaultPasswordHash NVARCHAR(MAX) = 'AQAAAAIAAYagAAAAEJ/yX+y+S0h1X1+Y...'; -- Student@123 placeholder

"""

# 2. Extract Students and create Users
student_match = re.search(r"INSERT INTO Students \((.*?)\)\s*VALUES\s*(.*?);", content, re.DOTALL | re.IGNORECASE)
if student_match:
    cols = student_match.group(1).split(',')
    vals = student_match.group(2)
    # values look like ('Ahmed Ali','01012345601','ahmed.ali@example.com','2000-01-01',GETDATE(),0),
    items = re.findall(r"\((.*?)\)", vals)
    
    student_sql = "-- =========================\n-- Students & Users\n-- =========================\n"
    for idx, item in enumerate(items, 1):
        parts = [p.strip().strip("'") for p in re.split(r",(?=(?:[^']*'[^']*')*[^']*$)", item)]
        # Expected: FullName, Phone, Email, DateOfBirth, CreatedAt, IsDeleted
        name = parts[0]
        phone = parts[1]
        email = parts[2]
        dob = parts[3]
        
        student_sql += f"""
DECLARE @U_S{idx} UNIQUEIDENTIFIER = NEWID();
INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, FullName, CreatedAt, PhoneNumber)
VALUES (@U_S{idx}, '{email}', '{email.upper()}', '{email}', '{email.upper()}', 1, @DefaultPasswordHash, NEWID(), NEWID(), 0, 0, 1, 0, '{name}', GETDATE(), '{phone}');
INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@U_S{idx}, @StudentRoleId);
SET IDENTITY_INSERT Students ON;
INSERT INTO Students (StudentId, UserId, FullName, Phone, Email, DateOfBirth, CreatedAt, IsDeleted)
VALUES ({idx}, @U_S{idx}, '{name}', '{phone}', '{email}', '{dob}', GETDATE(), 0);
SET IDENTITY_INSERT Students OFF;
"""
else:
    student_sql = "-- No Student data found to migrate\n"

# 3. Extract Instructors and create Users
instructor_match = re.search(r"INSERT INTO Instructors \((.*?)\)\s*VALUES\s*(.*?);", content, re.DOTALL | re.IGNORECASE)
if instructor_match:
    vals = instructor_match.group(2)
    items = re.findall(r"\((.*?)\)", vals)
    
    instructor_sql = "\n-- =========================\n-- Instructors & Users\n-- =========================\n"
    for idx, item in enumerate(items, 1):
        # parts: FullName, Phone, Email, HourlyRate, IsDeleted
        parts = [p.strip().strip("'") for p in re.split(r",(?=(?:[^']*'[^']*')*[^']*$)", item)]
        name = parts[0]
        phone = parts[1]
        email = parts[2]
        rate = parts[3]
        
        instructor_sql += f"""
DECLARE @U_I{idx} UNIQUEIDENTIFIER = NEWID();
INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, FullName, CreatedAt, PhoneNumber)
VALUES (@U_I{idx}, '{email}', '{email.upper()}', '{email}', '{email.upper()}', 1, @DefaultPasswordHash, NEWID(), NEWID(), 0, 0, 1, 0, '{name}', GETDATE(), '{phone}');
INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@U_I{idx}, @InstructorRoleId);
SET IDENTITY_INSERT Instructors ON;
INSERT INTO Instructors (InstructorId, UserId, FullName, Phone, Email, HourlyRate, IsDeleted)
VALUES ({idx}, @U_I{idx}, '{name}', '{phone}', '{email}', {rate}, 0);
SET IDENTITY_INSERT Instructors OFF;
"""
else:
    instructor_sql = "-- No Instructor data found to migrate\n"

# 4. Keep the rest of the tables as is
# Remove the old Students and Instructors inserts from the content
cleaned_content = content
cleaned_content = re.sub(r"INSERT INTO Students \(.*?\)\s*VALUES\s*.*?;", "", cleaned_content, flags=re.DOTALL | re.IGNORECASE)
cleaned_content = re.sub(r"INSERT INTO Instructors \(.*?\)\s*VALUES\s*.*?;", "", cleaned_content, flags=re.DOTALL | re.IGNORECASE)

# Remove comments about Students/Instructors and their separators to avoid duplication
cleaned_content = re.sub(r"-- =+.*?Students.*?-- =+", "", cleaned_content, flags=re.DOTALL | re.IGNORECASE)
cleaned_content = re.sub(r"-- =+.*?Instructors.*?-- =+", "", cleaned_content, flags=re.DOTALL | re.IGNORECASE)

with open(output_path, 'w', encoding='utf-8') as f:
    f.write(header)
    f.write(student_sql)
    f.write(instructor_sql)
    f.write("\n-- =========================\n-- Rest of the data\n-- =========================\n")
    f.write(cleaned_content.strip())

print("Updated SQL generated at " + output_path)
