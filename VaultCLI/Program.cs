/* 
    VaultCLI
    Very simple Vault command line interface to login and get secret from a path using token authentication
    C. Winters
    Notes: Non-threaded
*/
using System;
using System.Collections.Generic;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods.UserPass;
using VaultSharp.V1.AuthMethods.Okta;
using VaultSharp.V1.Commons;
using VaultSharp.V1.AuthMethods.Token.Models;

namespace VaultCLI
{
     class VaultCLIProgram
    {

        public static string sUser =null;
        public static string sOktaID = null;
        public static string sPassword = null;
        public static string sToken = null;
        public static string sServer = "http://127.0.0.1:8200";
        public static string sPath = null;
        public static string sPathMountPoint = "secret";
        public static int iAuthentication = 0;


        enum AUTHENTICATION_METHOD
        {
            token = 1010,
            userpassword,
            oktaid
        };



        public class GetVaultCreds
        {
            // VaultCLI https://server:port "token" "path" "secret"
            public void GetCredsByToken(string VaultURL, string Token, string Path, string mountPoint="secret")
            {
                if (String.IsNullOrEmpty(VaultURL))
                    VaultURL = Environment.GetEnvironmentVariable("VAULT_ADDR");

                if (String.IsNullOrEmpty(Token))
                    Token = Environment.GetEnvironmentVariable("VAULT_TOKEN");

                VaultURL = VaultURL.Trim().Replace("\"", "");
                Token = Token.Trim().Replace("\"", "");

                //lazy logins
                IAuthMethodInfo authMethod = new TokenAuthMethodInfo(Token);
                VaultClientSettings vaultClientSettings = new VaultClientSettings(VaultURL, authMethod);
                IVaultClient vaultClient = null;
                

                try
                {
                    vaultClient = new VaultClient(vaultClientSettings); 
                }
                catch(Exception ex)
                {
                    //{"invalid URI: The URI scheme is not valid"}
                    Console.Write("{0}", ex.GetType(), ex.Message);
                }

                try
                {
                    //If using KV V1.0
                    //Secret<System.Collections.Generic.Dictionary<string, object>> kv1Secret = vaultClient.V1.Secrets.KeyValue.V1.ReadSecretAsync(path: Path, mountPoint: mountPoint).Result;

                    //if (kv1Secret.Data.Count == 1)
                    //{
                    //    var keys = kv1Secret.Data.Keys;
                    //    foreach (System.Collections.Generic.KeyValuePair<string, object> kvp in kv1Secret.Data)
                    //    {
                    //        Console.WriteLine("\nPath={0}", mountPoint + Path);
                    //        Console.WriteLine("\nKey (User)={0}\nValue (password)={1}", kvp.Key, kvp.Value);
                    //    }
                    //}

                    //info on token
                    Secret<CallingTokenInfo> tokenData = vaultClient.V1.Auth.Token.LookupSelfAsync().Result;

                    int LeaseDurationSeconds = tokenData.LeaseDurationSeconds;
                    var ClientToken = tokenData.AuthInfo.ClientToken;
                    Console.WriteLine("\nToken Authentication Method.\nLease Duration in seconds:{0}\nClient Token:{1}", LeaseDurationSeconds.ToString(), ClientToken);


                    Secret<SecretData> kv2Secret = vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: Path, mountPoint: mountPoint).Result;

                    if (kv2Secret.Data.Data.Count ==1)
                    {
                        var keys = kv2Secret.Data.Data.Keys;

                        foreach(System.Collections.Generic.KeyValuePair<string, object> kvp in kv2Secret.Data.Data)
                        {
                            Console.WriteLine("\nPath={0}", mountPoint + Path);
                            Console.WriteLine("\nKey (User)={0}\nValue (password)={1}", kvp.Key, kvp.Value);
                        }
                    }
                }
                catch(Exception exc)
                {
                    if (exc.InnerException.Message.Contains("A connection attempt failed"))
                        Console.WriteLine("Cannot connect to server. Port is probably blocked");

//                        Console.WriteLine(" -- {0} {1}", exc.GetType(), exc.Message);

                    if (exc.InnerException.Message.Contains("machine actively refused it"))
                        Console.WriteLine("No connection could be made because the target machine actively refused it. Faulty URL or port not in URL.");

                }
            } //End GetCredsByToken()


            public void GetCredsByUserPassword(string VaultURL, string User, string Password, string Path, string mountPoint = "secret")
            {
                if (String.IsNullOrEmpty(VaultURL))
                    VaultURL = Environment.GetEnvironmentVariable("VAULT_ADDR");

                VaultURL = VaultURL.Trim().Replace("\"", "");
                User = User.Trim().Replace("\"", "");
                Password = Password.Trim().Replace("\"", "");

                IAuthMethodInfo authMethod = new UserPassAuthMethodInfo(User, Password);
                VaultClientSettings vaultClientSettings = new VaultClientSettings(VaultURL, authMethod);
                IVaultClient vaultClient = null;

                try
                {
                    vaultClient = new VaultClient(vaultClientSettings);
                }
                catch (Exception ex)
                {
                    //{"invalid URI: The URI scheme is not valid"}
                    Console.Write("{0}", ex.GetType(), ex.Message);
                }

                try
                {
                    //If using KV V1.0
                    //Secret<System.Collections.Generic.Dictionary<string, object>> kv1Secret = vaultClient.V1.Secrets.KeyValue.V1.ReadSecretAsync(path: Path, mountPoint: mountPoint).Result;

                    //if (kv1Secret.Data.Count == 1)
                    //{
                    //    var keys = kv1Secret.Data.Keys;
                    //    foreach (System.Collections.Generic.KeyValuePair<string, object> kvp in kv1Secret.Data)
                    //    {
                    //        Console.WriteLine("\nPath={0}", mountPoint + Path);
                    //        Console.WriteLine("\nKey (User)={0}\nValue (password)={1}", kvp.Key, kvp.Value);
                    //    }
                    //}
                    //info on token
                    Secret<CallingTokenInfo> tokenData = vaultClient.V1.Auth.Token.LookupSelfAsync().Result;

                    int LeaseDurationSeconds = tokenData.LeaseDurationSeconds;
                    var ClientToken = tokenData.AuthInfo.ClientToken;
                    Console.WriteLine("\nUserPassword Authentication Method.\nLease Duration in seconds:{0}\nClient Token:{1}", LeaseDurationSeconds.ToString(), ClientToken);


                    Secret<SecretData> kv2Secret = vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: Path, mountPoint: mountPoint).Result;

                    if (kv2Secret.Data.Data.Count == 1)
                    {
                        var keys = kv2Secret.Data.Data.Keys;

                        foreach (System.Collections.Generic.KeyValuePair<string, object> kvp in kv2Secret.Data.Data)
                        {
                            Console.WriteLine("\nPath={0}", mountPoint + Path);
                            Console.WriteLine("\nKey (User)={0}\nValue (password)={1}", kvp.Key, kvp.Value);
                        }
                    }
                }
                catch (Exception exc)
                {
                    if (exc.InnerException.Message.Contains("A connection attempt failed"))
                        Console.WriteLine("Cannot connect to server. Port is probably blocked");
                    else
                        Console.WriteLine(" -- {0} {1}", exc.GetType(), exc.Message);
                }
            } //End GetCredsByUserPassword()


            public void GetCredsByOktaID(string VaultURL, string OKtaID, string Password, string Path, string mountPoint = "secret")
            {
                if (String.IsNullOrEmpty(VaultURL))
                    VaultURL = Environment.GetEnvironmentVariable("VAULT_ADDR");

                VaultURL = VaultURL.Trim().Replace("\"", "");
                OKtaID = OKtaID.Trim().Replace("\"", "");
                Password = Password.Trim().Replace("\"", "");

                IAuthMethodInfo authMethod = new OktaAuthMethodInfo(OKtaID, Password);
                VaultClientSettings vaultClientSettings = new VaultClientSettings(VaultURL, authMethod);
                IVaultClient vaultClient = null;

                try
                {
                    vaultClient = new VaultClient(vaultClientSettings);
                }
                catch (Exception ex)
                {
                    //{"invalid URI: The URI scheme is not valid"}
                    Console.Write("{0}", ex.GetType(), ex.Message);
                }

                try
                {
                    //If using KV V1.0
                    //Secret<System.Collections.Generic.Dictionary<string, object>> kv1Secret = vaultClient.V1.Secrets.KeyValue.V1.ReadSecretAsync(path: Path, mountPoint: mountPoint).Result;

                    //if (kv1Secret.Data.Count == 1)
                    //{
                    //    var keys = kv1Secret.Data.Keys;
                    //    foreach (System.Collections.Generic.KeyValuePair<string, object> kvp in kv1Secret.Data)
                    //    {
                    //        Console.WriteLine("\nPath={0}", mountPoint + Path);
                    //        Console.WriteLine("\nKey (User)={0}\nValue (password)={1}", kvp.Key, kvp.Value);
                    //    }
                    //}
                    //info on token
                    Secret<CallingTokenInfo> tokenData = vaultClient.V1.Auth.Token.LookupSelfAsync().Result;

                    int LeaseDurationSeconds = tokenData.LeaseDurationSeconds;
                    var ClientToken = tokenData.AuthInfo.ClientToken;
                    Console.WriteLine("\nOkta ID Authentication Method.\nLease Duration in seconds:{0}\nClient Token:{1}", LeaseDurationSeconds.ToString(), ClientToken);


                    Secret<SecretData> kv2Secret = vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: Path, mountPoint: mountPoint).Result;

                    if (kv2Secret.Data.Data.Count == 1)
                    {
                        var keys = kv2Secret.Data.Data.Keys;

                        foreach (System.Collections.Generic.KeyValuePair<string, object> kvp in kv2Secret.Data.Data)
                        {
                            Console.WriteLine("\nPath={0}", mountPoint + Path);
                            Console.WriteLine("\nKey (User)={0}\nValue (password)={1}", kvp.Key, kvp.Value);
                        }
                    }
                }
                catch (Exception exc)
                {
                    if (exc.InnerException.Message.Contains("A connection attempt failed"))
                        Console.WriteLine("Cannot connect to server. Port is probably blocked");
                    else
                        Console.WriteLine(" -- {0} {1}", exc.GetType(), exc.Message);
                }
            } //End GetCredsByOktaID()


        } //End of GetVaultCreds class.


        static void Main(string[] args)
        {

            //Command line arguments. Supports example params like /param {choice}, -param {choice}, and --param {choice}

            // VaultCLI --server=https://server:port --token "token" --path="path" --mountpoint="secret"
            // VaultCLI --server=https://server:port --user=me --password=passwd --path="/my/awesome/secret/path" --mountpoint="secret"
            // VaultCLI --server=https://server:port --oktaid=me --password=passwd --path="/my/awesome/secret/path" --mountpoint="secret"

            string[] sPrgArgs = Environment.GetCommandLineArgs();

            string s1;

            Dictionary<string, string> dictArgs = new Dictionary<string, string>();

            for (int iIndex = 1; iIndex < sPrgArgs.Length; iIndex += 2)
            {
                string sIndexArg = sPrgArgs[iIndex].Replace("/", "").Replace("-", "").Replace("--", "");

                try
                {
                    if (sPrgArgs[iIndex].Length > 1)
                        s1 = sPrgArgs[iIndex + 1];

                    dictArgs.Add(sIndexArg, sPrgArgs[iIndex + 1]);
                }
                catch
                {
                    //Trying to add the same key - won't work.
                    dictArgs.Add(sIndexArg, "");
                }

            }

            if ((dictArgs.ContainsKey("help")) || (dictArgs.ContainsKey("?")))
            {
                System.Console.WriteLine("Examples:");
                System.Console.WriteLine("Auth by token\nVaultCLI --server=https://server:port --token=\"token\" --path=\"/my/awesome/secret/path\" --mountpoint=\"secret\"\n");
                System.Console.WriteLine("Auth by user/password\nVaultCLI --server=https://server:port --user=TheUserIsMe --password=passwd --path=\"/my/awesome/secret/path\" --mountpoint=\"secret\"\n ");
                System.Console.WriteLine("Auth by OKta ID\nVaultCLI --server=https://server:port --oktaid= --password=passwd --path=\"/my/awesome/secret/path\" --mountpoint=\"secret\"\n ");
                System.Console.WriteLine("\n--server .   -Can be used for local default server " + sServer);
                System.Console.WriteLine("\n--mountpoint -Can be omitted to use default mountpoint '{0}'", sPathMountPoint);
                return;
            }


            //
            if (dictArgs.ContainsKey("server"))
            {
                string sDefaultServer = string.Empty;

                if (dictArgs.TryGetValue("server", out sDefaultServer))
                {
                    if (sDefaultServer != ".")
                        sServer = sDefaultServer;
                }
            }

            //
            if (dictArgs.ContainsKey("token"))
            {
                iAuthentication = (int)AUTHENTICATION_METHOD.token;
                string sDefaultToken = string.Empty;

                if (dictArgs.TryGetValue("token", out sDefaultToken))
                    sToken = sDefaultToken;
            }

            //
            if (dictArgs.ContainsKey("user"))
            {
                iAuthentication = (int)AUTHENTICATION_METHOD.userpassword;
                string sDefaultUser = string.Empty;

                if (dictArgs.TryGetValue("user", out sDefaultUser))
                   sUser = sDefaultUser;
            }

            //
            if (dictArgs.ContainsKey("oktaid"))
            {
                iAuthentication = (int)AUTHENTICATION_METHOD.oktaid;
                string sDefaultOktaID = string.Empty;

                if (dictArgs.TryGetValue("oktaid", out sDefaultOktaID))
                    sOktaID = sDefaultOktaID;
            }


            //
            if (dictArgs.ContainsKey("password"))
            {
                string sDefaultUserPassword = string.Empty;

                if (dictArgs.TryGetValue("password", out sDefaultUserPassword))
                    sPassword = sDefaultUserPassword;
            }

            //
            if (dictArgs.ContainsKey("path"))
            {
                string sDefaultPath = string.Empty;

                if (dictArgs.TryGetValue("path", out sDefaultPath))
                    sPath = sDefaultPath;
            }

            //
            if (dictArgs.ContainsKey("mountpoint"))
            {
                string sDefaultPathMountPoint = string.Empty;

                if (dictArgs.TryGetValue("mountpoint", out sDefaultPathMountPoint))
                    sPathMountPoint = sDefaultPathMountPoint;
            }

            if (args.Length <= 3)
            {
                Console.WriteLine("Use -? or --help");
                return;
            }

            GetVaultCreds getCredsTest = new GetVaultCreds();

            switch(iAuthentication)
            {
                case (int)AUTHENTICATION_METHOD.token:
                    getCredsTest.GetCredsByToken(sServer, sToken, sPath, sPathMountPoint);
                    break;

                case (int)AUTHENTICATION_METHOD.userpassword:
                    getCredsTest.GetCredsByUserPassword(sServer, sUser, sPassword, sPath, sPathMountPoint);
                    break;

                case (int)AUTHENTICATION_METHOD.oktaid:
                    break;

                default: Console.WriteLine("Use -? or --help. Inconsistent parameters.");
                    break;
            }



        } //End of main

    } //End of class VaultCLIProgram
} //Namespace
