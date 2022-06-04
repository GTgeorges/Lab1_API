#!bin/bash

dotnet build
./bin/Debug/net6.0/LAB1_API.exe 1 false &
./bin/Debug/net6.0/LAB1_API.exe 2 false &
./bin/Debug/net6.0/LAB1_API.exe 3 false &
./bin/Debug/net6.0/LAB1_API.exe 0 true