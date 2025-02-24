🔥 **MOVEit** <br>
🔥 ***Interview assignment*** <br>
<br/>

**Diagram**

![MoveitDiagram](https://github.com/user-attachments/assets/8f0aafc8-187e-4664-b21a-d8781f514e29)

<br/>

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

<br/>

🛠️ Tech Stack <br/>
🔹 Arhitecture: Microservice architecture <br/>
🔹 Backend: .NET 9, Aspire, SignalR, Minimal API <br/>
🔹 Frontend: WPF <br/>
🔹 File Handling: Background proces <br/>

<br/>

🚀 Getting Started <br/>
    .NET 9 SDK <br/>
    .Use AspireHosting as startup project, to be able to start all the services at once <br/>
    <img src="https://github.com/user-attachments/assets/e2c5b4b4-1ae1-464e-bcfc-00b471415f74" alt="![Startup]" width="300" height="200"><br/>

 <br/>
 
🔌 API Endpoints <br/>
| Method	| Endpoint	                    | Description <br/>
| POST	    | /authenticate/token	        | Get token <br/>
| POST	    | /authenticate/revoke	        | Get token <br/>
| GET	    | /files/getall	                | Get all files from cloud folder <br/>
| POST	    | /files/upload	                | Upload a file <br/>
| DELETE	| /files/delete?fileId={id}	    | Delete a file <br/>

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
1. **WPF UI Service** - *It is an entry point where user is getting authenticated and can monitor in real time what have in a local folder and what is having on cloud (remote) folder. In real time can monitor if add one or many files in local folder, how it is going to cloud, or the same when delete something in a local.* <br/>
 🔹 Login View: <br/> <img src="https://github.com/user-attachments/assets/658e2002-aa6d-415a-add4-08880cf93d55" alt="Login view" width="300" height="200"> <br/>
 🔹 Home View without files View: <br/> <img src="https://github.com/user-attachments/assets/84988926-60fa-4505-b903-f28ce11051b8" alt="Login view" width="300" height="200">  *#There is a pop up message when no files found in remote folder* <br/>
 🔹 Home View **with** files View: <br/> <img src="https://github.com/user-attachments/assets/b2e98a64-261b-4f5b-940f-cb645fe81767" alt="Login view" width="300" height="200"> <br/>
 
    *In UI Servce is implemented Token authentication also Token Revoke mechanism, so when the token expire will be revoked and user ill see login screen again. There will be need new authentication using username and password (Username and Password are provided by MOVEit support). The Token is keeped locally on the machine used .dat file on path C:\Users\{your user}\AppData\Roaming\MoveitAccessToken.dat. This process is setted in TokenStorage, if you want to change token storage.* <br/>

2. FileObserverService - *It is a Background service, witch listen for any local flder changes. The specified local folder is C:\\MOVEit (*if folder doesnt exist, he service will create it*), At the moment BackGroundService is listening for FileCreatee and FileDelete. FileObserverService, can listen for multiple file changes as many files are created, or deleted. To be sure each file process will be finished is used Semaphore. In the service also have TokenStorage, witch can say when token is expired service stop listen for any changes and to block folder access, as can set access to **READ ONLY** (Future implementation).* <br/>

3. MinimalApi (MoveitApi) - *The Api idea is to give us decoupling from real MoveitApi, so we can easy extend the logic of the product, for example if want to keep some analytcs of usage, store data, transform it or even implement different cloud storage provide as AWS or Amazon.<br/> At current implementation MoveitApi is using MoveitClent (*Class Library*) as MoveitClient give us easy access to real MoveitAPI. In MoveitApi is used SignalR registration, to be able to notify when file is Updated/Created or Deleted so UI and the cliend can get updte in real time.<br/> At the moment there is only two notifictions = "FileUploaded" and "FileDeleted"* <br/>
4. MoveitClient (Class Library) - *Helper for Moveit integration, could be a nuget and used for multiple services if needed.* <br/>
5. *.Net Aspire - Local hosting container. Easy to run all services at once and keep traking logs and metrics.* <br/>

🚀Improvements:
<br/>
🔹 Loging in WPF service, not logged with ConoleWriteline  <br/>
🔹 Better loging in API service <br/>
🔹 Better logging in FileObservableService <br/>
🔹 Add monitoring as Prometheus <br/>
🔹 Add Logic when token expire Background service stop looking the local folder, until next successfully authentication as I mentioned above <br/>
🔹 BackgroundService if observing is stopped to retrict folder access to read only, so if there is not restriction on current logic the customer could add files to folder but they will be never sync or add EventBrokr to handle all events in a quequ as Kafka, RabbitMQ or etc. Possible             triggers for this bug - Api is dowon, User is unauthenticated but could add files in a local folder, MoveitApi is down and integration could not be established and etc  <br/>
🔹 Adding retry mechanim in Background service if response from our api is failed, or some token problem. Retry Mechanism t UI if SignalR is down, or bad response. Add RetryMechanism to our API, to handle if we hit some limits or temporary problem with MOVEit. Could be used Polly for             retry <br/>
🔹 Add E2E and Automation and Integration test <br/>
🔹 Refactoring some code is dublicated can be more abstract <br/>
🔹 For sure there is a lot more improvements, if more than one head think on it <br/>
