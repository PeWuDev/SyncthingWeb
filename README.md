# SyncthingWeb
Web application that manages Syncthing folders. Access your Syncthing data everwhere from server, share files and manage users.

## Disclaimer
The project is under (not heavly) development. If you wish some features - please contact me at patryk@pewudev.pl or create pull request.
Use fo your own risk.

If you wanna buy a beer? Contact me at mail.

**Read installation and configuration sections before use!**

## Features

Basic features implemented so far:
* Watching syncthing file changes
* Reading folders content
* Downloading files & folders
* Sharing files
* Users management (with folders permissions) 

What is not present?
* Previewing files
* Uploading
* Advanced user management
* Sending mails
* More...?

## Screenshots

![Setup](https://raw.githubusercontent.com/pwasiewicz/SyncthingWebUI/master/Assets/Setup.jpg)
![Dashboard](https://raw.githubusercontent.com/pwasiewicz/SyncthingWebUI/master/Assets/Dashboard.jpg)
![Files](https://raw.githubusercontent.com/pwasiewicz/SyncthingWebUI/master/Assets/Files.jpg)
![Sharing](https://raw.githubusercontent.com/pwasiewicz/SyncthingWebUI/master/Assets/Sharing.jpg)

## Requirements
* None!

## Installation
Download newest pre-compiled binaries from https://github.com/pwasiewicz/SyncthingWeb/releases.
Run `SyncthingWeb.exe` to start built-in kestrel server that will host your application.

## Configuration
All basic configuration value is stored in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultMSSQLConnection": "Server=(localdb)\\MSSQLLocalDB;Database=aspnet-SyncthingWeb-d58d8c7f-f8b7-4c3d-94f3-66d1ee2ee957;Trusted_Connection=True;MultipleActiveResultSets=true",
    "DefaultSQLiteConnection": "Filename=SyncthingWebDatabase.db"
  },
  "DatabaseProvider": "sqlite",  
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  }
}

```

You can also specifiy more advanced option via command line arguments.

| Argument     | Description                                  | Default value  | 
| ------------ |:--------------------------------------------:| --------------:|
| --port, -p   | Port number for listening of built-in server | 8385           |

### Database providers
Currently, there are two databse providers:
- MSSQL Server
- SQLite (**default**)


## Release notes
### v1.0.0

* First release