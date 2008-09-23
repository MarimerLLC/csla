using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace NavigationApp
{

  public class StateEventArgs : EventArgs
  {
    public ScriptObject State { get; set; }
  }

  public class NavigationHandler
  {
    public const string BookmarkKey = "bookmark";

    public NavigationHandler() { 
      RaiseEvents = true;
      AesManaged manager = new AesManaged();
      manager.GenerateKey();
      _key = manager.Key;
      manager.GenerateIV();
      _iv = manager.IV;
    }

    public bool RaiseEvents
    { get; set; }

    /// <summary>   
    /// Events   
    /// </summary>   
    public event EventHandler<StateEventArgs> Navigate;
    /// <summary>   
    /// Handler for Sys.Application navigate event   
    /// </summary>   
    [ScriptableMember]
    public void HandleNavigate(ScriptObject state)
    {
      if (Navigate != null && RaiseEvents)
      {
        Navigate(this, new StateEventArgs() { State = state });
      }
    }


    private static byte[] _key;
    private static byte[] _iv;

    public static string Encrypt(string dataToEncrypt)
    {
      return (Encrypt(_key,_iv, dataToEncrypt));
    }

    public static string Decrypt(string dataToDecrypt)
    {
      return(Decrypt(_key,_iv,dataToDecrypt));
    }

    internal static string Encrypt(byte[] key, byte[] iv, string dataToEncrypt)
    {
      // Initialise
      AesManaged encryptor = new AesManaged();



      // Set the key
      encryptor.Key = key;
      encryptor.IV = iv;

      // create a memory stream
      using (MemoryStream encryptionStream = new MemoryStream())
      {
        // Create the crypto stream
        using (CryptoStream encrypt = new CryptoStream(encryptionStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
        {
          // Encrypt
          byte[] utfD1 = UTF8Encoding.UTF8.GetBytes(dataToEncrypt);
          encrypt.Write(utfD1, 0, utfD1.Length);
          encrypt.FlushFinalBlock();
          encrypt.Close();

          // Return the encrypted data
          return Convert.ToBase64String(encryptionStream.ToArray());
        }
      }
    }


    internal static string Decrypt(byte[] key, byte[] iv, string encryptedString)
    {
      // Initialise
      AesManaged decryptor = new AesManaged();
      byte[] encryptedData = Convert.FromBase64String(encryptedString);

      // Set the key
      decryptor.Key = key;
      decryptor.IV = iv;

      // create a memory stream
      using (MemoryStream decryptionStream = new MemoryStream())
      {
        // Create the crypto stream
        using (CryptoStream decrypt = new CryptoStream(decryptionStream, decryptor.CreateDecryptor(), CryptoStreamMode.Write))
        {
          // Encrypt
          decrypt.Write(encryptedData, 0, encryptedData.Length);
          decrypt.Flush();
          decrypt.Close();

          // Return the unencrypted data
          byte[] decryptedData = decryptionStream.ToArray();
          return UTF8Encoding.UTF8.GetString(decryptedData, 0, decryptedData.Length);
        }
      }
    }


  }
}
