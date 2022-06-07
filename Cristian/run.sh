#!bin/bash


cd LOG736_Labo1_Serveur/LOG736_Labo1_Serveur
dotnet build

cd ../../LOG736_Labo1_Client/LOG736_Labo1_Client
dotnet build
./bin/Debug/net6.0/LOG736_Labo1_Client.exe &

cd ../../LOG736_Labo1_Serveur/LOG736_Labo1_Serveur
./bin/Debug/net6.0/LOG736_Labo1_Serveur.exe 
