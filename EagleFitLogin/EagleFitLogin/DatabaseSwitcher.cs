using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Windows;


namespace EagleFitLogin
{
    /**
 * 
 * @Author Joshua Montgomery
 * 
 * this file contains all the database switching and connection string storage
 * 
 * file contains 2 serialized files "DatabaseData" , and "DbList". 
 * they stores the connection data needed to connect to the available database(s).
 * DatabaseData stores the connection data, and DbList stored an ArrayList of
 * DatabaseData objects.
 * 
 * the file also contains the DatabaseSwitcher class which is used to open
 * database connections, read and save the serialized data, and encript the contents.
 */
   

       
        [Serializable]
        class DataBaseData
        {
            private String dbIP;
            private String dbName;
            private String dbUserName;
            private String dbPassword;

            public DataBaseData(String serverIP, String DBName, String DBUserName, String DBPassword)
            {
                dbIP = serverIP;
                dbName = DBName;
                dbUserName = DBUserName;
                dbPassword = DBPassword;
            }

            public String serverIP
            {
                get
                {
                    return dbIP;
                }
                set
                {
                    dbIP = value;
                }
            }


            public String DBName
            {
                get
                {
                    return dbName;
                }
                set
                {
                    dbName = value;
                }
            }


            public String DBUserName
            {
                get
                {
                    return dbUserName;
                }
                set
                {
                    dbUserName = value;
                }
            }

            public String DBPassword
            {
                get
                {
                    return dbPassword;
                }
                set
                {
                    dbPassword = value;
                }
            }

            public override String ToString()
            {
                return serverIP + " - " + dbName;
            }

        }

        [Serializable]
        public class DbList
        {
            private ArrayList list = new ArrayList();
            public DbList()
            { }
            public ArrayList DataBaseList
            {
                get
                {
                    return list;
                }
                set
                {
                    list = value;
                }
            }

            public void addDatabase(String serverIP, String DBName, String DBUserName, String DBPassword)
            {
                list.Add(new DataBaseData(serverIP, DBName, DBUserName, DBPassword));
            }
        }


        class DatabaseSwitcher
        {
            private DBHandler data;
            private DbList dbData = null;
            private String fileName = "";

            private Byte[] KEY_64 = new Byte[8];

            private Byte[] IV_64 = new Byte[8];

            public DatabaseSwitcher(DBHandler d, String f)
            {
                String key = "?<Fr(3&=";
                String iv = ")!&$h~7=";
                for (int x = 0; x < 8; x++)
                {
                    KEY_64[x] = (Byte)key[x];
                    IV_64[x] = (Byte)iv[x];
                }
                data = d;
                fileName = f;
            }

            // attempts to opens the database connection
            public bool openDatabase(int userRole, String DB, String adminPassword)
            {
                String ip, user, db, pass;
                String conn = "";
                for (int x = 0; x < dbData.DataBaseList.Count; x++)
                {
                    ip = ((DataBaseData)dbData.DataBaseList[x]).serverIP;
                    user = ((DataBaseData)dbData.DataBaseList[x]).DBUserName;
                    pass = ((DataBaseData)dbData.DataBaseList[x]).DBPassword;
                    db = ((DataBaseData)dbData.DataBaseList[x]).DBName;
                    conn = buildConnectionString(ip, user, pass, db);

                    //DB == ((DataBaseData)dbData.DataBaseList[x]).serverIP
                    if (data.isSettingsConnect(conn))
                    {
                        return changeDatabaseConnection(ip, user, pass, db, adminPassword);
                    }
                }
                return false;
            }

            //changes the database connection
            private bool changeDatabaseConnection(String server, String user, String DBPassword, String databaseName, String adminPassword)
            {
                // has to connect to get vital data
                data.iP = server;
                data.userName = user;
                data.nameOfDb = databaseName;
                data.pwd = DBPassword;
                if (server == "localhost" || data.isAdminPasswordCorrect(adminPassword))
                {
                    return true;
                }
                return false;
            }

            public String buildConnectionString(String server, String user, String password, String database)
            {
                //"server=localhost; user id=root; password=redalert; database=bodyshop; pooling=false";
                return "server = " + server + "; user id =" + user + "; password =" + password +
                    "; database = " + database + "; pooling = false";
            }

            //opens the serialized data 
            //loads all database connection data into memory
            public DbList openDatabaseData()
            {
                Stream myStream = new FileStream(fileName, FileMode.OpenOrCreate);
                if (myStream != null)
                {
                    IFormatter formatter = new BinaryFormatter();
                    try
                    {

                        dbData = (DbList)formatter.Deserialize(myStream);
                        myStream.Close();
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            myStream.Close();// file is invalid
                            dbData = new DbList();// opens 
                            saveDatabaseData();
                            myStream = new FileStream(fileName, FileMode.OpenOrCreate);
                            dbData = (DbList)formatter.Deserialize(myStream);
                            myStream.Close();
                        }
                        catch (Exception e2)
                        {
                            // this will never be called, but it saves an exception just in case
                            return new DbList();
                        }
                    }

                    ArrayList list = dbData.DataBaseList;
                    for (int x = 0; x < list.Count; x++)
                    {
                        ((DataBaseData)list[x]).DBPassword = decrypt(((DataBaseData)list[x]).DBPassword);
                        ((DataBaseData)list[x]).DBUserName = decrypt(((DataBaseData)list[x]).DBUserName);
                        ((DataBaseData)list[x]).DBName = decrypt(((DataBaseData)list[x]).DBName);
                        ((DataBaseData)list[x]).serverIP = decrypt(((DataBaseData)list[x]).serverIP);
                    }
                    return dbData;
                }
                return new DbList();
            }

            //saves the database data and encrypts the data so others may not see the passwords
            public bool saveDatabaseData()
            {
                ArrayList list = dbData.DataBaseList;
                for (int x = 0; x < list.Count; x++)
                {
                    ((DataBaseData)list[x]).DBPassword = encrypt(((DataBaseData)list[x]).DBPassword);
                    ((DataBaseData)list[x]).DBUserName = encrypt(((DataBaseData)list[x]).DBUserName);
                    ((DataBaseData)list[x]).DBName = encrypt(((DataBaseData)list[x]).DBName);
                    ((DataBaseData)list[x]).serverIP = encrypt(((DataBaseData)list[x]).serverIP);
                }

                Stream myStream = new FileStream(fileName, FileMode.OpenOrCreate);
                if (myStream != null)
                {
                    IFormatter formatter =
                        /*(IFormatter) */ new BinaryFormatter();
                    // serialize shapes
                    formatter.Serialize(myStream, dbData);
                    myStream.Close();
                    for (int x = 0; x < list.Count; x++)
                    {
                        ((DataBaseData)list[x]).DBPassword = decrypt(((DataBaseData)list[x]).DBPassword);
                        ((DataBaseData)list[x]).DBUserName = decrypt(((DataBaseData)list[x]).DBUserName);
                        ((DataBaseData)list[x]).DBName = decrypt(((DataBaseData)list[x]).DBName);
                        ((DataBaseData)list[x]).serverIP = decrypt(((DataBaseData)list[x]).serverIP);
                    }
                    return true;
                }

                return false;
            }
            //***OLD STUFF*****************************************************
            private String encript(String password)
            {
                byte[] b = new byte[password.Length];
                for (int x = 0; x < password.Length; x++)
                {
                    b[x] = (byte)password[x];
                }
                return Convert.ToBase64String(b);
            }

            private String decript(String password)
            {
                String temp = "";
                byte[] b = Convert.FromBase64String(password);
                for (int x = 0; x < b.Length; x++)
                {
                    temp += (Char)b[x];
                }
                return temp;
            }
            //*************************************************************************8

            //*******NEW STUFF*****************        
            protected string encrypt(string value)
            {
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateEncryptor(KEY_64, IV_64), CryptoStreamMode.Write);
                StreamWriter sw = new StreamWriter(cs);

                sw.Write(value);
                sw.Flush();
                cs.FlushFinalBlock();
                ms.Flush();

                // convert back to a string
                return Convert.ToBase64String(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length));
            }

            protected string decrypt(string value)
            {
                try
                {
                    DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                    Byte[] buffer = Convert.FromBase64String(value);
                    MemoryStream ms = new MemoryStream(buffer);
                    CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateDecryptor(KEY_64, IV_64), CryptoStreamMode.Read);
                    StreamReader sr = new StreamReader(cs);
                    return sr.ReadToEnd();
                }
                catch (CryptographicException e)
                {
                    try
                    {
                        dbData.DataBaseList.Clear();
                        saveDatabaseData();
                        MessageBox.Show("the encription algorithm was modified, the program deleted " +
                            "the old connections because the encription key that created them was lost");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Encription Algorithm was modified from within the" +
                            "sourse code, an attempt was made to correct the problem but has failed. " +
                            " the only remaining option is to manually delete the .myManagementSerial file");
                    }
                    finally
                    {
                        MessageBox.Show("the program will now exit");
                        Environment.Exit(0);
                    }
                }

                return "bad things";

            }
            //***************************

            public bool isDatabaseConnected()
            {
                String conn = buildConnectionString(data.iP, data.userName, data.pwd, data.nameOfDb);
                return data.isSettingsConnect(conn);
            }
        }
    }


