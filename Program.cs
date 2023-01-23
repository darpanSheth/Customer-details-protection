using Module2AssignmentQuestion2;
using static System.Console;

Write("Please enter the customers name: ");
string name = ReadLine();
Write("Please enter customer's credit car number: ");
string creditCard = ReadLine();
Write("Please enter the password for your account: ");
string password = ReadLine();
WriteLine();

Customer.NormalXml(name, creditCard, password);