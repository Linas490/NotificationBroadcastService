
# Mini Notification Broadcast Service

This service allows sending messages through TCP clients or an API, then persists and broadcasts them to all connected clients.

## Build

### 1. Start PostgreSQL

- Download and install from: [https://www.postgresql.org/download/windows/](https://www.postgresql.org/download/windows/)
- Install the PostgreSQL Server and optionally pgAdmin for easier database management.
- Use default or custom settings (commonly: port `5432`, user `postgres`, your chosen password).

### 2. Configure the Connection String

In both `NotificationApi/appsettings.json` and `NotificationTCPServer/appsettings.json`, add your PostgreSQL connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=yourport;Database=notificationsdb;Username=postgres;Password=yourpassword"
}
```
Change port, username and password fields to same ones as   

### 3. Run the Services

You must start **both** the API and the TCP server.

### Option A: Using Visual Studio Code

 **Start the API:**
  1. Open the solution in **VS Code**.
  2. Set **NotificationApi** as the startup project.
  3. Press `F5` or go to **Run > Start Debugging**.

  API will be accessible at: `http://localhost:5000` or `https://localhost:5001`



**Start the TCP Server:**

1. Switch startup project to **NotificationTCPServer**.
2. Press `F5` or start debugging again.

#### Option B: Using the Command Line

Use two terminal windows or tabs:

**Run Notification API:**

```bash
cd path/to/NotificationApi
dotnet run
```

**Run TCP Server:**

```bash
cd path/to/NotificationTCPServer
dotnet run
```

---

## Connect to the TCP Server

You can connect to the TCP server using:

### Option 1: Telnet (Command Line)

```bash
telnet localhost 5000
```

### Option 2: PuTTY (GUI)

1. Download PuTTY: [https://www.chiark.greenend.org.uk/~sgtatham/putty/latest.html](https://www.chiark.greenend.org.uk/~sgtatham/putty/latest.html)  
2. Open PuTTY  
3. In the **Host Name** field, enter: `localhost`  
4. Set **Connection type** to: `Raw`  
5. Set **Port** to: `5000`  
6. Click **Open** to connect
