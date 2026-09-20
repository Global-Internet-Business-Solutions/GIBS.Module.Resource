#!/bin/bash

TargetFramework=$1
ProjectName=$2

cp -f "../Client/bin/Debug/$TargetFramework/$ProjectName$.Client.Oqtane.dll" "../../oqtane.framework/Oqtane.Server/bin/Debug/$TargetFramework/"
cp -f "../Client/bin/Debug/$TargetFramework/$ProjectName$.Client.Oqtane.pdb" "../../oqtane.framework/Oqtane.Server/bin/Debug/$TargetFramework/"
cp -f "../Server/bin/Debug/$TargetFramework/$ProjectName$.Server.Oqtane.dll" "../../oqtane.framework/Oqtane.Server/bin/Debug/$TargetFramework/"
cp -f "../Server/bin/Debug/$TargetFramework/$ProjectName$.Server.Oqtane.pdb" "../../oqtane.framework/Oqtane.Server/bin/Debug/$TargetFramework/"
cp -f "../Shared/bin/Debug/$TargetFramework/$ProjectName$.Shared.Oqtane.dll" "../../oqtane.framework/Oqtane.Server/bin/Debug/$TargetFramework/"
cp -f "../Shared/bin/Debug/$TargetFramework/$ProjectName$.Shared.Oqtane.pdb" "../../oqtane.framework/Oqtane.Server/bin/Debug/$TargetFramework/"
cp -rf "../Server/wwwroot/"* "../../oqtane.framework/Oqtane.Server/wwwroot/_content/%ProjectName%/"

# Copy Twilio dependencies
cp -f "../Server/bin/Debug/$TargetFramework/Twilio.dll" "../../oqtane.framework/Oqtane.Server/bin/Debug/$TargetFramework/"
[ -f "../Server/bin/Debug/$TargetFramework/Twilio.pdb" ] && cp -f "../Server/bin/Debug/$TargetFramework/Twilio.pdb" "../../oqtane.framework/Oqtane.Server/bin/Debug/$TargetFramework/"
cp -f "../Server/bin/Debug/$TargetFramework/Twilio.AspNet.Common.dll" "../../oqtane.framework/Oqtane.Server/bin/Debug/$TargetFramework/"
[ -f "../Server/bin/Debug/$TargetFramework/Twilio.AspNet.Common.pdb" ] && cp -f "../Server/bin/Debug/$TargetFramework/Twilio.AspNet.Common.pdb" "../../oqtane.framework/Oqtane.Server/bin/Debug/$TargetFramework/"
cp -f "../Server/bin/Debug/$TargetFramework/Twilio.AspNet.Core.dll" "../../oqtane.framework/Oqtane.Server/bin/Debug/$TargetFramework/"
[ -f "../Server/bin/Debug/$TargetFramework/Twilio.AspNet.Core.pdb" ] && cp -f "../Server/bin/Debug/$TargetFramework/Twilio.AspNet.Core.pdb" "../../oqtane.framework/Oqtane.Server/bin/Debug/$TargetFramework/"

# Copy QuestPDF dependencies
cp -f "../Server/bin/Debug/$TargetFramework/QuestPDF.dll" "../../oqtane.framework/Oqtane.Server/bin/Debug/$TargetFramework/"
[ -f "../Server/bin/Debug/$TargetFramework/QuestPDF.pdb" ] && cp -f "../Server/bin/Debug/$TargetFramework/QuestPDF.pdb" "../../oqtane.framework/Oqtane.Server/bin/Debug/$TargetFramework/"
# Copy QuestPDF native dependencies (runtimes folder)
[ -d "../Server/bin/Debug/$TargetFramework/runtimes" ] && cp -rf "../Server/bin/Debug/$TargetFramework/runtimes" "../../oqtane.framework/Oqtane.Server/bin/Debug/$TargetFramework/"
