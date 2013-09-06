using System;
using Microsoft.SPOT;
using System.Text;
using System.IO;
using System.Collections;

namespace HttpLibrary
{
    /// <summary>
    /// Credential class for holding the server security parameters
    /// </summary>
    public class Credential
    {
        /// <summary>
        /// Server name
        /// </summary>
        public string ServerOwner;
        
        /// <summary>
        ///  Authentication username
        /// </summary>
        public string UserName;
        
        /// <summary>
        /// Authentication password
        /// </summary>
        public string Password;
        
        /// <summary>
        /// Base64 encrypted password
        /// </summary>
        public string Key;
        
        /// <summary>
        /// The webpage to direct users to for cookie based log-ins
        /// </summary>
        public string loginFile;
        
        /// <summary>
        /// Used to determine the authentication method
        /// </summary>
        public bool useCookie;
        
        /// <summary>
        /// List of files that don't need authentication to access
        /// </summary>
        public string[] authList;

        public ArrayList Keys = new ArrayList();
        
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="serverowner">Server name</param>
        /// <param name="username">Authentication username</param>
        /// <param name="password">Authentication password</param>
        public Credential(string serverowner, string username, string password)
        {
            useCookie = false;
            ServerOwner = serverowner;
            UserName = username;
            Password = password;
            Key = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(username + ":" + password));
        }
        
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="password">Authentication password</param>
        /// <param name="loginfile">Path to the log in web page</param>
        /// <param name="authlist">List of files that don't require authorization</param>
        public Credential(string[] passwords, string loginfile, string[] authlist)
        {
            useCookie = true;
            foreach (string password in passwords)
                Keys.Add(Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(password)));
            loginFile = loginfile;
            authList = authlist;
        }
        
        /// <summary>
        /// Reads a saved credential from memory card (does not work with cookie based authentication)
        /// </summary>
        /// <returns>A credential object with the settings</returns>
        public static Credential ReadFromFile()
        {
            FileStream fs = new FileStream(@"\SD\NsC.crdn", FileMode.Open, FileAccess.Read);
            StreamReader Reader = new StreamReader(fs);
            string owner = Reader.ReadLine();
            string keeey = Reader.ReadLine();
            Reader.Close();
            fs.Close();
            string[] unpass = new string(UTF8Encoding.UTF8.GetChars(Convert.FromBase64String(keeey))).Split(':');
            return new Credential(owner, unpass[0], unpass[1]);
        }
        
        /// <summary>
        /// Saves a credential to memory card (does not work with cookie based authentication)
        /// </summary>
        /// <param name="Credentials"></param>
        public static void WriteToFile(Credential Credentials)
        {
            FileStream fs = new FileStream(@"\SD\NsC.crdn", FileMode.Create, FileAccess.Write);
            StreamWriter Writer = new StreamWriter(fs);
            Writer.WriteLine(Credentials.ServerOwner);
            Writer.WriteLine(Credentials.Key);
            Writer.Close();
            fs.Close();
        }
        
        /// <summary>
        /// Override of ToString() method
        /// </summary>
        /// <returns>Returns a string with credential parameters each followed by a new line</returns>
        public override string ToString()
        {
            return "Server Owner : " + ServerOwner + "\n" +
                "UserName : " + UserName + "\n" +
                "Password : " + Password + "\n" +
                "Encrypted User & Password : " + Key;
        }
    }
}
