%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe FileZillaService.exe
Net Start FileMonitoringService
sc config FileMonitoringService start= auto
pause