@echo off
set TargetFramework=%1
set ProjectName=%2

XCOPY "..\Client\bin\Debug\%TargetFramework%\%ProjectName%.Client.Oqtane.dll" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\%TargetFramework%\" /Y
XCOPY "..\Client\bin\Debug\%TargetFramework%\%ProjectName%.Client.Oqtane.pdb" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\%TargetFramework%\" /Y
XCOPY "..\Server\bin\Debug\%TargetFramework%\%ProjectName%.Server.Oqtane.dll" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\%TargetFramework%\" /Y
XCOPY "..\Server\bin\Debug\%TargetFramework%\%ProjectName%.Server.Oqtane.pdb" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\%TargetFramework%\" /Y
XCOPY "..\Shared\bin\Debug\%TargetFramework%\%ProjectName%.Shared.Oqtane.dll" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\%TargetFramework%\" /Y
XCOPY "..\Shared\bin\Debug\%TargetFramework%\%ProjectName%.Shared.Oqtane.pdb" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\%TargetFramework%\" /Y
XCOPY "..\Server\wwwroot\*" "..\..\oqtane.framework\Oqtane.Server\wwwroot\_content\%ProjectName%\" /Y /S /I

REM Copy Twilio dependencies
XCOPY "..\Server\bin\Debug\%TargetFramework%\Twilio.dll" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\%TargetFramework%\" /Y
IF EXIST "..\Server\bin\Debug\%TargetFramework%\Twilio.pdb" XCOPY "..\Server\bin\Debug\%TargetFramework%\Twilio.pdb" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\%TargetFramework%\" /Y
XCOPY "..\Server\bin\Debug\%TargetFramework%\Twilio.AspNet.Common.dll" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\%TargetFramework%\" /Y
IF EXIST "..\Server\bin\Debug\%TargetFramework%\Twilio.AspNet.Common.pdb" XCOPY "..\Server\bin\Debug\%TargetFramework%\Twilio.AspNet.Common.pdb" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\%TargetFramework%\" /Y
XCOPY "..\Server\bin\Debug\%TargetFramework%\Twilio.AspNet.Core.dll" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\%TargetFramework%\" /Y
IF EXIST "..\Server\bin\Debug\%TargetFramework%\Twilio.AspNet.Core.pdb" XCOPY "..\Server\bin\Debug\%TargetFramework%\Twilio.AspNet.Core.pdb" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\%TargetFramework%\" /Y

REM Copy QuestPDF dependencies
XCOPY "..\Server\bin\Debug\%TargetFramework%\QuestPDF.dll" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\%TargetFramework%\" /Y
IF EXIST "..\Server\bin\Debug\%TargetFramework%\QuestPDF.pdb" XCOPY "..\Server\bin\Debug\%TargetFramework%\QuestPDF.pdb" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\%TargetFramework%\" /Y
REM Copy QuestPDF native dependencies (runtimes folder) - using ROBOCOPY to handle locked files
IF EXIST "..\Server\bin\Debug\%TargetFramework%\runtimes" (
    ROBOCOPY "..\Server\bin\Debug\%TargetFramework%\runtimes" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\%TargetFramework%\runtimes" /E /NFL /NDL /NJH /NJS /nc /ns /np
    IF %ERRORLEVEL% LEQ 7 SET ERRORLEVEL=0
)
