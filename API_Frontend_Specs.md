# LearnOps API Frontend Integration Specifications 🚀

This document provides a comprehensive technical specification for the LearnOps API. It is designed to be used as a context for AI Agents or Frontend Developers to implement the client-side of the Learning Management System.

---

## 🏗️ 1. Global API Configuration

- **Base URL:** `http://localhost:5093`
- **Prefix:** `/api`
- **Content Type:** `application/json`

### 📦 Standard Response Wrapper (Result<T>)

Every endpoint returns a unified structure:

```typescript
interface ApiResponse<T> {
  isSuccess: boolean;
  message: string;
  statusCode: number;
  data: T; // The actual content
}
```

---

## 🎓 2. Student Module (`/api/Student`)

### 📋 Get All Students (Paginated)

- **Method:** `GET`
- **Query Params:**
  - `search?: string` (Full name or Email)
  - `page?: number` (Default: 1)
  - `pageSize?: number` (Default: 10)
- **Data (T):** `StudentDto[]`
- **Item Properties:**
  ```typescript
  interface StudentDto {
    studentId: number;
    fullName: string;
    phone: string;
    email?: string;
    dateOfBirth: string; // ISO Date Format (e.g., 2002-07-21T00:00:00)
  }
  ```

### 🔍 Get Student Detailed

- **Method:** `GET`
- **URL:** `/api/Student/{id}`
- **Data (T):** `StudentDetailedDto`
- **Structure:**
  ```typescript
  interface StudentDetailedDto {
    studentId: number;
    fullName: string;
    phone: string;
    email: string;
    dateOfBirth: string;
    courses: {
      courseId: number;
      courseName: string;
      groupId: number;
      groupName: string;
    }[];
  }
  ```

### ➕ Add/Update Student

- **Method:** `POST` (Add) / `PUT` (Update: `/api/Student/{id}`)
- **Request Body:**
  ```typescript
  interface AddUpdateStudentDto {
    fullName: string; // Required, max 100
    phone: string; // Required, valid phone
    email?: string; // Optional, valid email
    dateOfBirth: string; // Required, YYYY-MM-DD
  }
  ```

### 🗑️ Delete Student(s)

- **Single:** `DELETE /api/Student/{id}`
- **Bulk:** `DELETE /api/Student/bulk-delete`
  - **Body:** `number[]` (e.g., `[1, 2, 3]`)

---

## 📚 3. Course Module (`/api/Course`)

### 📋 Get All Courses

- **Method:** `GET`
- **Query Params:** `search`, `page`, `pageSize`
- **Data (T):** `CourseDto[]`
- **Structure:**
  ```typescript
  interface CourseDto {
    courseId: number;
    title: string;
    description: string;
    durationWeeks: number;
    price: number;
    maxStudents: number;
  }
  ```

### 🔍 Get Course Detailed

- **Method:** `GET`
- **URL:** `/api/Course/{id}`
- **Data (T):** `CourseDetailedDto`
- **Structure:**
  ```typescript
  interface CourseDetailedDto {
    courseId: number;
    title: string;
    description: string;
    durationWeeks: number;
    price: number;
    maxStudents: number;
    totalEnrolledStudents: number;
    activeGroups: number;
    groups: {
      groupId: number;
      name: string;
      room: string;
      days: string;
      time: string;
      startDate: string;
      endDate?: string;
      studentsCount: number;
    }[];
  }
  ```

---

## 👥 4. Class Group Module (`/api/ClassGroup`)

### 📋 Get All Groups

- **Method:** `GET`
- **Data (T):** `ClassGroupDto[]`
- **Structure:**
  ```typescript
  interface ClassGroupDto {
    groupId: number;
    name: string;
    room: string;
    days: string;
    time: string;
    startDate: string;
    endDate?: string;
    courseName: string;
    instructorName: string;
    studentsCount: number;
    status: "Active" | "Inactive";
  }
  ```

### ➕ Add/Update Group

- **Method:** `POST` / `PUT`
- **Request Body:**
  ```typescript
  interface AddUpdateClassGroupDto {
    name: string;
    room: string;
    days: string;
    time: string;
    startDate: string; // YYYY-MM-DD
    endDate: string; // YYYY-MM-DD
    courseId: number;
    instructorId: number;
  }
  ```

---

## 👨‍🏫 5. Instructor Module (`/api/Instructor`)

### 📋 Get All Instructors

- **Method:** `GET`
- **Data (T):** `InstructorDto[]`
- **Structure:**
  ```typescript
  interface InstructorDto {
    instructorId: number;
    fullName: string;
    phone: string;
    email?: string;
    hourlyRate: number;
  }
  ```

### 🔍 Get Instructor Detailed

- **Method:** `GET`
- **URL:** `/api/Instructor/{id}`
- **Data (T):** `InstructorDetailedDto`
- **Structure:**
  ```typescript
  interface InstructorDetailedDto {
    instructorId: number;
    fullName: string;
    phone: string;
    email: string;
    hourlyRate: number;
    totalStudents: number;
    activeGroups: number;
    courses: {
      courseId: number;
      courseName: string;
      groups: {
        groupId: number;
        groupName: string;
        room: string;
        days: string;
        time: string;
        studentsCount: number;
        status: "Active" | "Inactive";
      }[];
    }[];
  }
  ```

---

## 📝 6. Exam Module (`/api/Exam`)

### 📋 Get All Exams

- **Method:** `GET`
- **Data (T):** `ExamWithClassGroupDto[]`
- **Structure:**
  ```typescript
  interface ExamWithClassGroupDto {
    examId: number;
    title: string;
    examDate: string;
    maxScore: number;
    groupId: number;
    classGroup?: ClassGroupDto;
    results: {
      studentName: string;
      score: number;
      result?: string; // Percentage or Grade
    }[];
  }
  ```

### ➕ Create/Update Exam

- **Method:** `POST` / `PUT`
- **Request Body:**
  ```typescript
  interface ExamDto {
    title: string;
    examDate: string;
    maxScore: number;
    groupId: number;
  }
  ```

---

## 🛠️ Integration Notes

1. **Bulk Delete:** Always send an array of IDs `[id1, id2]` for `bulk-delete` endpoints.
2. **Date Format:** Use `YYYY-MM-DD` for sending dates to the server.
3. **Status:** The `status` property in Groups and Exams is calculated on the server but important for UI badges.
4. **Pagination:** The `GetAll` methods return arrays, ensure the UI handles the standard `ApiResponse` wrapper.
