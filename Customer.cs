using System;
using System.IO;
using static System.Console;
using System.Xml;
using static System.Environment;
using static System.IO.Path;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Xml.Linq;
using static System.Convert;
using System.Collections.Generic;
using System.Data.SqlTypes;


namespace Module2AssignmentQuestion2
{
	public class Customer
	{
        private static byte[] salt = new byte[16];
        
        private static readonly int iterations = 2000;

        public static void NormalXml(string name, string creditCard, string password)
		{
            string xmlFile = Combine(CurrentDirectory, "Customer.xml");
            using (FileStream xmlFileStream = File.Create(xmlFile))
            {
                using (XmlWriter xml = XmlWriter.Create(xmlFileStream, new XmlWriterSettings { NewLineOnAttributes = true, Indent = true }))
                {
                    xml.WriteStartElement("Customers");
                    xml.WriteStartElement("customer");
                    xml.WriteElementString("name", name);
                    xml.WriteElementString("creditcard", creditCard);
                    xml.WriteElementString("password", password);
                }
            }
            WriteLine("Below is the normal Xml File with clear creditCard and password text");
            WriteLine();
            WriteLine(File.ReadAllText(xmlFile));
            WriteLine();


            byte[] encryptedBytes;
            byte[] plainBytes = Encoding.Unicode.GetBytes(creditCard);
            var aes = Aes.Create();
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            aes.Key = pbkdf2.GetBytes(32); // set a 256-bit key 
            aes.IV = pbkdf2.GetBytes(16); // set a 128-bit IV 
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(
                  ms, aes.CreateEncryptor(),
                  CryptoStreamMode.Write))
                {
                    cs.Write(plainBytes, 0, plainBytes.Length);
                }
                encryptedBytes = ms.ToArray();
            }

             string encryptedCreditCard = Convert.ToBase64String(encryptedBytes);
             string saltText = Convert.ToBase64String(salt);

             var sha = SHA256.Create();
             var saltedPassword = password + saltText;

            string saltedAndHashedPassword = Convert.ToBase64String(sha.ComputeHash(Encoding.Unicode.GetBytes(saltedPassword)));


            string updatedXmlFile = Combine(CurrentDirectory, "updatedCustomer.xml");
            using (FileStream updatedXmlFileStream = File.Create(updatedXmlFile))
            {
                using (XmlWriter xml = XmlWriter.Create(updatedXmlFileStream, new XmlWriterSettings { NewLineOnAttributes = true, Indent = true }))
                {
                    xml.WriteStartElement("Customers");
                    xml.WriteStartElement("customer");
                    xml.WriteElementString("name", name);
                    xml.WriteElementString("creditcard", encryptedCreditCard);
                    xml.WriteElementString("password", saltedAndHashedPassword);
                }
            }
            WriteLine("Below is the Xml File with Encrypted creditCard and SaltedAndHashed Password");
            WriteLine();
            WriteLine(File.ReadAllText(updatedXmlFile));

            WriteLine();
            Write("Please enter the correct password for extracting the decrypted credit card number: ");
            string Password2 = ReadLine();

            try
            {
               string clearText = DecryptPassword(encryptedCreditCard, Password2);
                WriteLine($"Decrypted text: {clearText}");
            }
            catch(CryptographicException ex)
            {
                WriteLine("{0}\nMore details: {1}",
                arg0: "You entered the wrong password!",
                arg1: ex.Message);
            }
        }

        public static string DecryptPassword(string encryptdText, string Password)
        {
            

            byte[] plainBytes;
            byte[] cryptoBytes = Convert
              .FromBase64String(encryptdText);
            var aes = Aes.Create();
            var pbkdf2 = new Rfc2898DeriveBytes(Password, salt, iterations);
            aes.Key = pbkdf2.GetBytes(32);
            aes.IV = pbkdf2.GetBytes(16);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(
                  ms, aes.CreateDecryptor(),
                  CryptoStreamMode.Write))
                {
                    cs.Write(cryptoBytes, 0, cryptoBytes.Length);
                }
                plainBytes = ms.ToArray();
            }
            return Encoding.Unicode.GetString(plainBytes);
        }
    }
}


