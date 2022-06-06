// See https://aka.ms/new-console-template for more information
using LOG736_Labo1;
Console.WriteLine("Solution Client");


var client = new Client();
client.RequestTime(25000, client.GetTime(), 5);