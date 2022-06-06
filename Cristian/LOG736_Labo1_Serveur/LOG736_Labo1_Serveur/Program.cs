// See https://aka.ms/new-console-template for more information
using LOG736_Labo1;
Console.WriteLine("Solution Serveur");
var serveur = new Serveur();
serveur.StartServer(25000);
Environment.Exit(0);