# AMV-Theater-Assistant
Software to work alongside MPC-HC for use in AMV rooms at conventions.

Features:
- Controls for MPC-HC.
- Is able to modify MPC-HC settings to easily set it up for a multi-screen setup.
- Generates website with auto-updating page to display panel and video information.
- Customizable options for page styling.

General Information:

This application is currently a standalone EXE file that will run from any location.  Settings and the generated website for use with the MPC-HC web server are located in the C:\Users\<current-user>\AppData\Local\AMVTheaterAssistant folder for the current user.

User Saved Settings Information:

Settings are currently saved using the default method provided by Visual Studio, which means they are saved and loaded based on the location of the exe file.  A new settings file is generated for each location it is executed from in a subfolder of the folder mentioned above.  This is something I plan to change in a future release.

Additional Information:
A copy of jQuery is included in the application and it follows the same MIT license that this software is distributed under, pelease refer to the license document for more information.
