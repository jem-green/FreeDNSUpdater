FreeDNSUpdater
==============

A Windows Console application for updating afraid.org Free DNS ddns.

Configured it in the FreeDnsConsole.exe.config file.

1. Compile.
2. Copy FreeDnsSConsole.exe, FreeDnsCore.dll and FreeDnsonsole.exe.config to some installation directory.
3. Execute FreeConsole.exe 

You can look at the console for information about when the updater syncs your ip to Free DNS.

or

A Windows Service client for updating afraid.org Free DNS ddns.

Configured it in the FreeDnsService.exe.config file.

1. Compile.
2. Copy FreeDnsService.exe, FreeDnsCore.dll and FreeDnsService.exe.config to some installation directory.
3. Open an administrative terminal in the installation directory.
4. Execute FreeDnsService.exe with no args for usage.  This is the installer.<pre>
    --install installs the service
    --uninstall uninstalls the service</pre>
5. Manually start the service for the first time or reboot to trigger the auto-start.

You can look at the Windows Event Viewer for information about when the updater syncs your ip to Free DNS.

In the config you can specify any number of hostnames under your account that the service instance should update dns for:
If you happen to want 2 different names to resolve to one IP, this is for you.  Otherwise, a single name works just fine too.

You can provide username + password OR you can provide your API key
If you choose to enter your username + password the service will compute your API key, store it in the config and use it
  from there on out.  I.e., you could remove your user name and password after the service first starts.

Be kind to afraid.org.  It's largely a free service
FYI, there is a RefreshMillis setting which allows you to change your dns refresh interval.  It defaults to 10 minutes,
  which is probably completely adequate (and maybe a little aggressive) for any home use.
