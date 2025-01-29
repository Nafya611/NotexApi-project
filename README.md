# 📝 Note Management System API

This is a **.NET 6 Web API** for managing notes and categories. Users can **register, log in, create, update, and delete notes and categories**. The API uses **JWT authentication** for secure access.

## 🚀 Features

- **User Authentication** (Signup, Login, Logout)
- **JWT-Based Authorization**
- **CRUD for Notes** (Create, Read, Update, Delete)
- **CRUD for Categories**
- **Search Notes by Title or Content**
- **Retrieve Notes by Category**
- **Token Blacklisting for Logout**
- **MongoDB Database Integration**
- **Swagger UI for API Documentation**

---

## 🏠 **Technology Stack**

- **Backend:** ASP.NET 6, C#
- **Database:** MongoDB
- **Security:** JWT Authentication, BCrypt for password hashing
- **Documentation:** Swagger UI

---

## 📂 **Project Structure**

```
NotexApi
📂 Auth/
    📄 JwtService.cs       # Handles JWT token creation & validation

📂 Config/
    📄 DatabaseSettings.cs # MongoDB connection settings
    📄 JwtSettings.cs      # JWT settings configuration

📂 Controllers/
    📄 UserController.cs   # Handles user authentication (signup, login, logout)
    📄 NotesController.cs  # Manages notes (CRUD, search, filtering)
    📄 CategoriesController.cs # Manages categories (CRUD)

📂 Models/
    📄 User.cs             # User model (MongoDB schema)
    📄 Note.cs             # Note model
    📄 Category.cs         # Category model

📂 Services/
    📄 UserService.cs      # Handles database operations for users
    📄 NoteService.cs      # Handles database operations for notes
    📄 CategoryService.cs  # Handles database operations for categories

📄 Program.cs              # Entry point & middleware setup
📄 appsettings.json        # Configuration file
📄 NotexApi.csproj  # .NET project file
📄 README.md               # Project documentation
```

---

## ⚙️ **Setup Instructions**

### 📌 **Prerequisites**

- **.NET 6 SDK** installed
- **MongoDB** installed and running
- **Postman** or any API testing tool

### 🔹 **1. Clone the Repository**
```sh
git clone https://github.com/yourusername/NoteManagementSystem.git
cd NoteManagementSystem
```

### 🔹 **2. Configure Environment Variables**
Update `appsettings.json` with your MongoDB connection string and JWT settings.

### 🔹 **3. Install Dependencies**
```sh
dotnet restore
```

### 🔹 **4. Build and Run the Application**
```sh
dotnet build
dotnet run
```

The API should now be running at `https://localhost:7057` or `http://localhost:5076`.

### 🔹 **5. Test API Endpoints with Swagger**
Open your browser and navigate to:
```sh
https://localhost:7057/swagger
```
Or use **Postman** to send requests.

---

## 🔐 **Authentication**

### **Signup**
**POST** `/api/User/signup`
```json
{
  "username": "john",
  "email": "john@gmail.com",
  "passwordHash": "john1234"
}
```

### **Login**
**POST** `/api/User/login`
```json
{
  "email": "john@gmail.com",
  "password": "john1234"
}
```
_Response:_
```json
{
  "token": "your-jwt-token"
}
```

### **Logout**
**POST** `/api/User/logout`
Send the JWT token in the `Authorization` header as `Bearer <token>`.

---

## 📌 **CRUD Operations**

### **Notes**
- **Create Note** → `POST /api/Notes`
- **Get All Notes** → `GET /api/Notes`
- **Get Note by ID** → `GET /api/Notes/{id}`
- **Update Note** → `PUT /api/Notes/{id}`
- **Delete Note** → `DELETE /api/Notes/{id}`

### **Categories**
- **Create Category** → `POST /api/Categories`
- **Get All Categories** → `GET /api/Categories`
- **Get Category by ID** → `GET /api/Categories/{id}`
- **Update Category** → `PUT /api/Categories/{id}`
- **Delete Category** → `DELETE /api/Categories/{id}`

---

---

🚀 **Happy Coding!**
