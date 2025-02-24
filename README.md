🔥 **MOVEit** <br>
🔥 ***Interview assignment*** <br>

![MoveitDiagram](https://github.com/user-attachments/assets/8f0aafc8-187e-4664-b21a-d8781f514e29)

✨ Features <br/>
✅ Real-Time File Sync with SignalR <br/>
✅ File Uploads (Supports 5GB+ files) <br/>
✅ Auto Token Renewal & Expiry Handling <br/>
✅ WPF Desktop UI with File Tree <br/>
✅ Full API Integration with .NET 9 <br/>
✅ Multi-file Operations: Batch Upload and Delete <br/>
✅ .NET ASPIRE Hosting <br/>
✅ .NUnit framework for unit test <br/>
✅ Swagger - [localhost](https://localhost:7040/swagger/index.html) <br/>

🛠️ Tech Stack <br/>
🔹 Backend: .NET 9, Aspire, SignalR, Minimal API <br/>
🔹 Frontend: WPF <br/>
🔹 File Handling: Background proces <br/>

🚀 Getting Started <br/>
    .NET 9 SDK <br/>
    .Use AspireHosting as startup project, to be able to start all the services at once <br/>

  
🔌 API Endpoints <br/>
| Method	| Endpoint	                | Description <br/>
| POST	  | /authenticate/token	        | Get token <br/>
| POST	  | /authenticate/revoke	    | Get token <br/>
| GET	    | /files/getall	            | Get all files from cloud folder <br/>
| POST	  | /files/upload	            | Upload a file <br/>
| DELETE	| /files/delete?fileId={id}	| Delete a file <br/>

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


