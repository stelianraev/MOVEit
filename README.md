# MOVEit
Interview assignment 
![MoveitDiagram](https://github.com/user-attachments/assets/8f0aafc8-187e-4664-b21a-d8781f514e29)

✨ Features
✅ Real-Time File Sync with SignalR
✅ File Uploads (Supports 5GB+ files)
✅ Auto Token Renewal & Expiry Handling
✅ WPF Desktop UI with File Tree
✅ Full API Integration with .NET 9
✅ Multi-file Operations: Batch Upload and Delete
✅ .NET ASPIRE Hosting
✅ .NUnit framework for unit test
✅ Swagger - [localhost](https://localhost:7040/swagger/index.html)

🛠️ Tech Stack
🔹 Backend: .NET 9, Aspire, SignalR, Minimal API
🔹 Frontend: WPF
🔹 File Handling: Background proces

🚀 Getting Started
    .NET 9 SDK
    .Use AspireHosting as startup project, to be able to start all the services at once

  
🔌 API Endpoints
| Method	| Endpoint	                | Description
| POST	  | /authenticate/token	      | Get token
| POST	  | /authenticate/revoke	    | Get token
| GET	    | /files/getall	            | Get all files from cloud folder
| POST	  | /files/upload	            | Upload a file 
| DELETE	| /files/delete?fileId={id}	| Delete a file

💡 Code Structure
  MOVEit/
  │── src/
  │   ├── MoveitApi/        # Backend API
  │   ├── MoveitDesktopUI/  # WPF Frontend
  │   ├── MoveitClient/     # API Client Library
  │   ├── MoveitFileObserverService/  # File Monitoring Service
  │── tests/                # Unit & Integration Tests
  │── README.md             # Project Documentation


📸 Screenshots

