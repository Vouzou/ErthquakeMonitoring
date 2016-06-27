# ErthquakeMonitoring

You can use Visual Studio 2013 to build the project. A wix installer was created so you need to download wix from: http://wixtoolset.org/releases/
The startup project is PlethoraApp. 
All the outputs of the projects are in the folder Binaries/$(Configuration).
You can either run the project from Visual Studio or the executable from the Binaries folder "Plethora.exe".

To use the installer double click at the "Binaries\Release\EarthquakeMonitoringInstaller.msi". This will install the software to your system in C:\Program Files\Plethora\Earthqauke Monitoring App and you can run the "Plethora.exe" from there.
It will also create a Desktop shortcut and an application shortcut in the start menu. 

The application monitors earthquake activity. The first time that is launched the earthquake activity of the last hour is displayed and then the application keeps monitoring. The feed is updated every 5 minutes.

Dependencies:
Wix for creating the installer.
Paraffin for Wix Bundler to create the installer fragments.
Prism 6 for WPF to create the UI and the bindings.
CommonServiceLocator, GeoJSON.Net and Json.Net.

For questions email: Georgios.Vouzounaras@gmail.com
