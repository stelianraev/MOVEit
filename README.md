**🔥 MOVEit - Interview Assignment** <br>
  *🚀 Real-Time File Synchronization* <br/>

**Diagram**

![MoveitDiagram](https://github.com/user-attachments/assets/8f0aafc8-187e-4664-b21a-d8781f514e29)

<br/>

**✨ Features** <br/>
✅ Real-Time File Sync with SignalR <br/>
✅ File Uploads (Supports 5GB+ files) <br/>
✅ Auto Token Renewal & Expiry Handling <br/>
✅ WPF Desktop UI with File Tree <br/>
✅ Full API Integration with .NET 9 <br/>
✅ Multi-file Operations: Batch Upload and Delete <br/>
✅ .NET ASPIRE Hosting <br/>
✅ .NUnit framework for unit test <br/>
✅ Swagger - [localhost](https://localhost:7040/swagger/index.html) <br/>
<br/>

**🛠️ Tech Stack** <br/>
🔹 Arhitecture: Microservice architecture <br/>
🔹 Backend: .NET 9, Aspire, SignalR, Minimal API <br/>
🔹 Frontend: WPF <br/>
🔹 File Handling: Background proces <br/>
<br/>

**🚀 Getting Started** <br/>
🔧 Prerequisites
    🔹 NET 9 SDK <br/>
    🔹 Visual Studio 2022 (latest version) <br>
    <br/>
    
**▶ Running the Application** <br/>
    🔹 Set AspireHosting as the startup project in Visual Studio <br/>
    🔹 Run the project, and it will start all services automatically<br/>
    🔹 Open Swagger UI at [http://localhost:{PORT}/swagger](https://localhost:7040/swagger/index.html) to test the API<br/>
    <img src="https://github.com/user-attachments/assets/e2c5b4b4-1ae1-464e-bcfc-00b471415f74" alt="![Startup]" width="300" height="200"><br/>
 <br/>
 
🔌 API Endpoints <br/>
| Method	| Endpoint	                    | Description <br/>
| POST	    | /authenticate/token	        | Get authentication token <br/>
| POST	    | /authenticate/revoke	        | Revoke current token <br/>
| GET	    | /files/getall	                | Retrieve all files from cloud folder <br/>
| POST	    | /files/upload	                | Upload a file <br/>
| DELETE	| /files/delete?fileId={id}	    | Delete a file <br/>
📌 Swagger UI Available at: [http://localhost:{PORT}/swagger](https://localhost:7040/swagger/index.html)

<br/>

💡 Code Structure <br/>
 ```bash
  MOVEit/
  │── src/
  │   ├── MoveitApi/        # Backend API <br/>
  │   ├── MoveitDesktopUI/  # WPF Frontend <br/>
  │   ├── MoveitClient/     # API Client Library <br/>
  │   ├── MoveitFileObserverService/  # File Monitoring Service <br/>
  │── tests/                # Unit test <br/>
  │── README.md             # Project Documentation <br/>
```
 <br/>

💡**Short Service Summary**  <br/>
1. **WPF UI Service** - *This is the entry point where users authenticate and monitor, in real time, the contents of both a local folder and a cloud (remote) folder. Users can observe changes as files are added or deleted locally and see       how these changes sync with the cloud.* <br/>
 🔹 Entry Point: Allows users to authenticate and view local & remote files in real time. <br/>
 🔹 Real-Time Monitoring: Tracks file uploads and deletions locally and syncs them with the cloud. <br/>
 🔹 Authentication: Implements Token Authentication and Token Revocation. <br/>
 🔹 Token Storage: Stored at C:\Users\{your_user}\AppData\Roaming\MoveitAccessToken.dat <br/>

    *The UI service implements both Token Authentication and a Token Revocation mechanism. When the token expires, it is revoked, and the user will be redirected to the login screen. A new authentication using a username and password (provided by MOVEit support) will be required. The token is stored locally on the machine in a .dat file at: C:\Users\{your_user}\AppData\Roaming\MoveitAccessToken.dat. This process is managed by TokenStorage, and if needed, you can modify the token storage settings.* <br/>

🖥️ Screenshots:
 🔹 Login View: <br/> <img src="https://github.com/user-attachments/assets/658e2002-aa6d-415a-add4-08880cf93d55" alt="Login view" width="300" height="200"> <br/>
 🔹 Home View without files View: <br/> <img src="https://github.com/user-attachments/assets/84988926-60fa-4505-b903-f28ce11051b8" alt="Login view" width="300" height="200">  *#There is a pop up message when no files found in remote folder* <br/>
 🔹 Home View **with** files View: <br/> <img src="https://github.com/user-attachments/assets/b2e98a64-261b-4f5b-940f-cb645fe81767" alt="Login view" width="300" height="200"> <br/>

2. FileObserverService - *A background service that listens for any changes in the local folder. The specified local folder is C:\MOVEit (if the folder does not exist, the service will create it). Currently, the Background Service listens for file creation and file deletion events. FileObserverService can detect multiple file changes as files are created or deleted. To ensure each file process completes properly, a Semaphore is used. The service also includes TokenStorage, which determines when a token has expired. If the token expires, the service stops listening for changes and blocks folder access by setting it to read-only (future implementation).* <br/>
 🔹 Purpose: Monitors the C:\MOVEit folder (automatically created if missing). <br/>
 🔹 Monitored Events:<br/>
 &nbsp;&nbsp;&nbsp;&nbsp; ✅ File Created - Triggers an upload <br/>
 &nbsp;&nbsp;&nbsp;&nbsp; ✅ File Deleted - Triggers a delete request <br/>
 🔹 Concurrency Handling: Uses Semaphore to manage multiple file changes efficiently <br/>

3. Minimal API (MoveitApi) - *The MoveitApi serves as a middleware layer, decoupling the real Moveit API from other services. This allows for easier expansion of the product’s logic, such as analytics tracking, data storage, transformation, or integration with different cloud storage providers (e.g., AWS, Azure). <br/> Currently, MoveitApi uses MoveitClient (Class Library) for seamless access to the real Moveit API. SignalR is implemented for real-time notifications when a file is updated, created, or deleted, ensuring that both the UI and the client receive updates in real time.* <br/>
🔹 Acts as a Middleware API - Decouples MoveitAPI from the UI & File Observer <br/>
🔹 Allows Easy Future Expansion (e.g., Analytics, Multi-Cloud Support) <br/>
🔹 Implements SignalR Notifications <br/>
 &nbsp;&nbsp;&nbsp;&nbsp; 📢 "FileUploaded" Event <br/>
 &nbsp;&nbsp;&nbsp;&nbsp; 📢 "FileDeleted" Event <br/>
<br/>

4. MoveitClient (Class Library) - *A helper library for Moveit API integration. It could be packaged as a NuGet library to be reused across multiple services.* <br/>
🔹 Provides seamless Moveit API integration. <br/>
🔹 Can be converted into a NuGet package for multiple service integrations. <br/>
<br/>

5. .Net Aspire - *A local hosting container that simplifies running all services at once while keeping track of logs and metrics.* <br/>
🔹 Service container for running all services simultaneously. <br/>
🔹 Provides logging, monitoring, and observability. <br/>

**🚀Improvements:** <br/>
&nbsp;✅ Refactor Logging: <br/>
&nbsp;&nbsp;🔹 Improve WPF Logging (currently using Console.WriteLine) <br/>
&nbsp;&nbsp;🔹 Implement Structured Logging in API & Background Service <br/>
<br/>
&nbsp;✅ Monitoring & Observability: <br/>
&nbsp;&nbsp;🔹 Integrate Prometheus / Grafana for real-time metrics & logging <br/>
<br/>
&nbsp;✅ Enhanced Security & Reliability: <br/>
&nbsp;&nbsp;🔹 Implement retry mechanisms using Polly for API & SignalR failures
&nbsp;&nbsp;🔹 UI Retries if SignalR goes down <br/>
&nbsp;&nbsp;🔹 API retry logic for handling Moveit API Rate Limits<br/>
<br/>
&nbsp;✅ Background Service Enhancements: <br/>
&nbsp;&nbsp;🔹 Stop folder monitoring if token expires <br/>
&nbsp;&nbsp;🔹 Restrict folder to Read-Only when unauthorized <br/>
<br/>
&nbsp;✅ Testing & Automation: <br/>
&nbsp;&nbsp;🔹 Add End-to-End (E2E) Tests <br/>
&nbsp;&nbsp;🔹 Expand Unit Tests & Integration Tests <br/>
<br/>
&nbsp;✅ Future Cloud Storage Support: <br/>
&nbsp;&nbsp;🔹 AWS S3, Azure Blob Storage, or Google Drive <br/>
