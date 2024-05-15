
## Authors

- [@vpanev](https://www.github.com/vpanev)

# DirectoryMonitoring
Directory monitoring task, made for Progress.


The project was meticulously crafted around the N-Layer architecture design, emphasizing modularity, scalability, and maintainability throughout the development process.
## Components

- Application (Simple WPF application representing front end layer)
- Domain (Business logic layer)
- Unit Tests


## Setup and Installation
To ensure smooth operation of the application and enable access to the MOVEIT transfer API, follow these steps:

### Configuring the Business Logic Layer
The business logic layer is configured to run in the background, simulating API behavior. By default, the option to open a browser upon application startup is disabled. To enable this feature and access the Swagger UI:

    1. Locate the launchSettings.json file.
    2. Find the "launchBrowser" property.
    3. Set it to true to enable browser opening.

### Starting the Application

    1. Right-click on the solution in the IDE.
    2. Navigate to Properties
    3. Under Common Properties, select Startup Project.
    4. In the startup project settings, choose Multiple startup projects.
    5. Set the Action to Start for the following projects
        - MoveIT.Transfer.Task.Application
        - MoveIT.Transfer.Task.Application.Domain

### Running the Application
* Once the setup is complete, click the green arrow to start the project. This will initiate a simple window where you can enter your credentials for accessing the MOVEIT transfer API.
    
## File Management Capabilities

The application offers comprehensive file management features seamlessly integrated with MoveIT Transfer. These capabilities ensure efficient handling of files within the chosen monitored directory.

- Creation of files
- Renaming already existing file
- Deleting a file

