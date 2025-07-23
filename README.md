
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

## Design Notes

### One actor versus one per connection

There are several reasons for using a single actor to handle all TCP client communication rather than spawning one actor per connection:

#### Resource Efficiency

Using one actor per connection would increase the total number of actors in the system, which consumes more memory and CPU resources. In applications with many connections, this could lead to unnecessary overhead and reduced performance.

#### Reduced Complexity

In a model where each TCP connection has its own actor:

- Each actor would need to know about all the other actors to broadcast messages, resulting in duplicated references and increased state management complexity.
- Alternatively, you would need a central coordinator actor to track all active connection actors. However, this creates a centralized bottleneck, which defeats the purpose of distributing logic across actors.

#### Shared State and Persistence

In this system, the actor is responsible for saving messages to a repository (e.g., a database). If each connection had its own actor:

- All actors would either share a single Repository instance (risking concurrent access issues),
- Or each would maintain a separate instance (introducing redundant connections).

### Back-Pressure and Scaling

Using a single actor can lead to back-pressure, where the actor is unable to process incoming messages fast enough and its mailbox fills up.

In a larger system, a better solution is to use a worker pool:

A master actor receives messages or TCP clients and distributes them to multiple worker actors, each responsible for handling and broadcasting messages to their own set of clients.

### Supervision and Fault Tolerance

In a production system, actor failures are typically managed using supervision strategies. A supervisor actor monitors its child actors and defines how to respond when a child fails, for example by:

- Restarting the failed actor (restoring its old state if possible).
- Stopping the actor if the failure is unrecoverable.

## Architecture
![sequence diagram(https://github.com/Linas490/NotificationBroadcastService/blob/138d475e82bce18afeb8af482f2d506618b6b62c/sequenceDiagram.png)]
