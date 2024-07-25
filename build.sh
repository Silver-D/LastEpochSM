#!/bin/bash

# ! install dotnet-sdk-6.0 !
# ! fix your paths in <HintPath> to where your Game and MelonLoader is installed in the .csproj files !

dotnet build LastEpochSM.sln -v n -c Release
