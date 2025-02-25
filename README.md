# ChatBot

A real-time chatbot application using **.NET 8**, **Entity Framework**, **SignalR**, **CQRS**, and **MediatR** on the backend, with **Angular 19**, **Angular Material**, and **SignalR** on the frontend.

## Technologies Used

### **Backend (Server)**
- **.NET 8**
- **Entity Framework Core**
- **SignalR** (real-time communication)
- **CQRS** (Command Query Responsibility Segregation)
- **MediatR** (for handling commands and queries)

### **Frontend (Client)**
- **Angular 19**
- **Angular Material**
- **SignalR** (real-time WebSockets for chat functionality)

## Setup & Installation

### **Backend (Server)**
1. **Update Database Connection String**  
   - Navigate to:  
     ```
     /ChatBot.API/appsettings.Development.json
     ```
   - Modify the `"ConnectionStrings"` section with your **SQL Server** connection details.

2. **Run the API**  
   - run API
   - **Database migration and seeding** will execute automatically.

---

### **Frontend (Client)**
1. **Install dependencies**  
   Run the following command in the **Angular project root**:
   ```sh
   npm install
   ```
   ng serve

## TODOs
1. Improve cancel mechanism
2. Add authorization and authentication
3. More useful reposenses.