using System.Text;
using CryptoLibrary;


internal class Program {

  private static void Main(string[] args) {
    Console.WriteLine("Crypto Application Running..");
    /// Create a key for encryption
    byte[] key = Encoding.UTF8.GetBytes("D2F4A67B890CDE51");
    /// Initialize the CryptoTrailClass with the key
    var cryptoLibrary = new CryptoLibraryClass(key);
    /// Create a sample message
    string message = "Hello, world!";
    byte[] data = Encoding.UTF8.GetBytes(message);
    /// Compute SHA-1 hash
    byte[] sha1Hash = cryptoLibrary.ComputeSHA1(data);
    Console.WriteLine("SHA-1: " + cryptoLibrary.ByteArrayToHex(sha1Hash));
    /// Compute SHA-256 hash
    byte[] sha256Hash = cryptoLibrary.ComputeSHA256(data);
    Console.WriteLine("SHA-256: " + cryptoLibrary.ByteArrayToHex(sha256Hash));
    /// Encrypt and decrypt data using AES
    byte[] aesEncryptedData = cryptoLibrary.EncryptData(data);
    byte[] aesDecryptedData = cryptoLibrary.DecryptData(aesEncryptedData);
    Console.WriteLine("AES Decrypted: " + Encoding.UTF8.GetString(aesDecryptedData));
    /// Compute HMAC-SHA1 hash
    byte[] hmacSha1Key = Encoding.UTF8.GetBytes("myhmackey");
    byte[] hmacSha1Hash = cryptoLibrary.ComputeHMACSHA1(data, hmacSha1Key);
    Console.WriteLine("HMAC-SHA1: " + cryptoLibrary.ByteArrayToHex(hmacSha1Hash));
    Console.ReadLine();
  }
}
