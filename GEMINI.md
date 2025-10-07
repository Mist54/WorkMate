# Project Overview

This is a C# ASP.NET MVC project called WorkMate. The main goal of this project is to provide practical, real-life examples and connections for each Data Structures and Algorithms (DSA) concept, moving beyond theoretical explanations to demonstrate their application in a tangible software project.

## Building and Running

To build and run this project, you will need Visual Studio with the .NET Framework 4.8 SDK.

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/Mist54/WorkMate.git
    ```
2.  **Open the solution:**
    Open the `WorkMate.sln` file in Visual Studio.
3.  **Restore NuGet packages:**
    Visual Studio should automatically restore the NuGet packages. If not, open the Package Manager Console and run:
    ```powershell
    Update-Package -reinstall
    ```
4.  **Build the solution:**
    Build the solution by pressing `Ctrl+Shift+B` or by going to `Build > Build Solution`.
5.  **Run the application:**
    Press `F5` to run the application. The application will typically launch on `https://localhost:44380/`.

## Development Conventions

*   **Coding Style:** The project follows standard C# coding conventions.
*   **Testing:** There are no tests in this project.
*   **Database:** The project uses Entity Framework for database access. The connection string is located in the `Web.config` file.
*   **Dependencies:** The project uses several open-source libraries, including:
    *   ASP.NET MVC
    *   Entity Framework
    *   Bootstrap
    *   jQuery
    *   FontAwesome
    *   ClosedXML
    *   Newtonsoft.Json
    *   Serilog