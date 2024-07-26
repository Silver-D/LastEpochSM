#!/bin/bash

# ! install dotnet-sdk-6.0
# ! fix your paths in <HintPath> in the .csproj files to where your Game and MelonLoader are installed
# ! To compile the Mods, LastEpochSM.dll must first be compiled via LastEpochSM.csproj
# ! On Linux there are issues building directly from the .sln file for the first time, due to project dependencies
# ! so we build the modules one at a time using the .csproj files

dotnet build LastEpochSM.csproj -v n -c Release
[[ $? != 0 ]] && exit

dotnet build LastEpochSM_Mini.csproj -v n -c Release
[[ $? != 0 ]] && exit

dotnet build LastEpochSM_Skins.csproj -v n -c Release
