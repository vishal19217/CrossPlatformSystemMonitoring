
#  A. Build
1. Clone the repositry containing the solution from the https://github.com/vishal19217/CrossPlatformSystemMonitoring.
2. Change the working directory to the base folder containing (the solution and projects folder). 
3. build the solution using below commands:- 
    1. dotnet build -c Release
4. To publish the package using below commands:
    1. dotnet publish -c Release -r win-x64 --self-contained true

# B. Running the application
1. After successfully publishing. Go to published folder : "%BASE_FOLDER%\Cross Platform System Monitor\bin\Release\net9.0\win-x64\publish"
2. Create a new Plugins folder(if not already exists)
3. Add the plugins(Logger.dll, Publisher.dll) in this folder(From Step C.)
4. Double click and run the "%BASE_FOLDER%Cross Platform System Monitor\bin\Release\net9.0\win-x64\publish\Cross Platform System Monitor.exe"


# C. Adding the plguins into the publish folder
1. For adding each plugins:- 
    1. Go to the plugin publish folder "%BASE_FOLDER%\Cross Platform System Monitor\Logger\bin\Release\net9.0\win-x64\publish" and copy the Logger.dll file .
       Add the Logger.dll file to the Plugin folder created in B.2 step.
    2. Go to the plugin publish folder "%BASE_FOLDER%\Cross Platform System Monitor\Publisher\bin\Release\net9.0\win-x64\publish" and copy the Publisher.dll file .
       Add the Publisher.dll file to the Plugin folder created in B.2 step.

     
