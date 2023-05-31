using System.Security.Cryptography;
using System.Text;

namespace CryptoLibrary {
/// <summary>
/// Class for file transformation operations.
/// </summary>
public class CryptoLibraryClass {

  /// <summary>
  /// The size of the encryption key in bytes.
  /// </summary>
  private const int KeySizeInBytes = 16;
  /// <summary>
  /// The size of the encryption block in bytes.
  /// </summary>
  private const int BlockSizeInBytes = 16;
  /// <summary>
  /// The encryption key.
  /// </summary>
  private readonly byte[] key;
  /// <summary>
  /// Initializes a new instance of the <see cref="CryptoTrailClass"/> class with the specified key.
  /// </summary>
  /// <param name="key">The encryption key.</param>
  /// <exception cref="ArgumentException">Thrown when the key size is not equal to <see cref="KeySizeInBytes"/>.</exception>
  public CryptoLibraryClass(byte[] key) {
    if (key.Length != KeySizeInBytes) {
      throw new ArgumentException($"Key size must be {KeySizeInBytes} bytes.");
    }

    this.key = new byte[key.Length];
    Array.Copy(key, this.key, key.Length);
  }
  /// <summary>
  /// Transforms a file based on the specified operation.
  /// </summary>
  /// <param name="sourceFilePath">The path of the source file.</param>
  /// <param name="destFilePath">The path of the destination file.</param>
  /// <param name="operation">The operation to perform. 0 for decryption, 1 for encryption.</param>
  /// <returns>If operation is 1, returns the password obtained from the SHA256 digest. Otherwise, returns 0.</returns>
  /// <exception cref="ArgumentException">Thrown when an invalid operation is specified.</exception>
  public int TransformFile(string sourceFilePath, string destFilePath, int operation) {
    CryptoLibraryClass crypto = new CryptoLibraryClass(key);
    string newsourceFilePath = "Hello.bin";
    crypto.ConvertToBinary(sourceFilePath, newsourceFilePath);

    if (operation == 1) {
      byte[] fileData = File.ReadAllBytes(newsourceFilePath);
      byte[] sha1Digest = crypto.ComputeSHA1(fileData);
      byte[] sha256Digest = crypto.ComputeSHA256(fileData);
      byte[] buffer = crypto.CreateBuffer(sha1Digest, fileData, sha256Digest);
      byte[] encryptedBuffer = crypto.EncryptData(buffer);
      File.WriteAllBytes(destFilePath, encryptedBuffer);
      return crypto.GetPasswordFromDigest(sha256Digest);
    } else if (operation == 0) {
      byte[] encryptedBuffer = File.ReadAllBytes(sourceFilePath);
      byte[] decryptedData = crypto.DecryptData(encryptedBuffer);
      int length = BitConverter.ToInt32(decryptedData, 0);
      int sha1DigestOffset = 4;
      int fileDataOffset = 24;
      int sha256DigestOffset = 24 + length;
      byte[] sha1Digest = new byte[20];
      Buffer.BlockCopy(decryptedData, sha1DigestOffset, sha1Digest, 0, 20);
      byte[] fileData = new byte[length];
      Buffer.BlockCopy(decryptedData, fileDataOffset, fileData, 0, length);
      byte[] sha256Digest = new byte[32];
      Buffer.BlockCopy(decryptedData, sha256DigestOffset, sha256Digest, 0, 32);
      byte[] calculatedSha1Digest = crypto.ComputeSHA1(fileData);
      byte[] calculatedSha256Digest = crypto.ComputeSHA256(fileData);
      bool sha1Validation = crypto.CompareHashes(sha1Digest, calculatedSha1Digest);
      bool sha256Validation = crypto.CompareHashes(sha256Digest, calculatedSha256Digest);

      if (sha1Validation && sha256Validation) {
        File.WriteAllBytes(destFilePath, fileData);
      }

      return 0;
    } else {
      throw new ArgumentException("Invalid operation specified. Operation must be 0 or 1.");
    }
  }
  /// <summary>
  /// Calculates the HOTP (HMAC-based One-Time Password) value based on the specified key and counter.
  /// </summary>
  /// <param name="key">The HMAC key.</param>
  /// <param name="counter">The counter value.</param>
  /// <returns>The calculated HOTP value.</returns>
  public int HOTP(byte[] key, int counter) {
    CryptoLibraryClass crypto = new CryptoLibraryClass(key);
    byte[] hmacBytes = crypto.ComputeHMACSHA1(key, BitConverter.GetBytes(counter));
    int sbits = CalculateDynamicTruncation(hmacBytes);
    int hotpValue = (int)(sbits % 1000000);
    return hotpValue;
  }
  /// <summary>
  /// Calculates the dynamic truncation value based on the provided HMAC-SHA1 hash.
  /// </summary>
  /// <param name="hmacBytes">The HMAC-SHA1 hash as a byte array.</param>
  /// <returns>The dynamic truncation value as an unsigned integer.</returns>
  private int CalculateDynamicTruncation(byte[] hmacBytes) {
    int offset = hmacBytes[19] & 0xf;
    int bin_code = ((hmacBytes[offset] & 0x7f) << 24)
                   | ((hmacBytes[offset + 1] & 0xff) << 16)
                   | ((hmacBytes[offset + 2] & 0xff) << 8)
                   | (hmacBytes[offset + 3] & 0xff);
    return bin_code;
  }
  /// <summary>
  /// Computes the SHA-1 hash of the given data.
  /// </summary>
  /// <param name="data">The data to compute the hash for.</param>
  /// <returns>The computed SHA-1 hash.</returns>
  public byte[] ComputeSHA1(byte[] data) {
    using (SHA1 sha1 = SHA1.Create()) {
      return sha1.ComputeHash(data);
    }
  }

  /// <summary>
  /// Computes the HMAC-SHA1 hash of the given data using the provided key.
  /// </summary>
  /// <param name="data">The data to compute the HMAC-SHA1 hash for.</param>
  /// <param name="key">The HMAC-SHA1 key.</param>
  /// <returns>The computed HMAC-SHA1 hash.</returns>
  public byte[] ComputeHMACSHA1(byte[] data, byte[] key) {
    using (HMACSHA1 hmacSha1 = new HMACSHA1(key)) {
      return hmacSha1.ComputeHash(data);
    }
  }

  /// <summary>
  /// Computes the SHA-256 hash of the given data.
  /// </summary>
  /// <param name="data">The data to compute the hash for.</param>
  /// <returns>The computed SHA-256 hash.</returns>
  public byte[] ComputeSHA256(byte[] data) {
    using (SHA256 sha256 = SHA256.Create()) {
      return sha256.ComputeHash(data);
    }
  }

  /// <summary>
  /// Compares two hash values for equality.
  /// </summary>
  /// <param name="hash1">The first hash value as a byte array.</param>
  /// <param name="hash2">The second hash value as a byte array.</param>
  /// <returns>True if the hash values are equal, otherwise false.</returns>
  public bool CompareHashes(byte[] hash1, byte[] hash2) {
    if (hash1.Length != hash2.Length) {
      return false;
    }

    for (int i = 0; i < hash1.Length; i++) {
      if (hash1[i] != hash2[i]) {
        return false;
      }
    }

    return true;
  }

  /// <summary>
  /// Converts the contents of the input file to binary and saves it to the output file.
  /// </summary>
  /// <param name="inputFile">The path of the input file.</param>
  /// <param name="outputFile">The path of the output file.</param>
  private void ConvertToBinary(string inputFile, string outputFile) {
    byte[] buffer;
    using (FileStream fileStream = File.OpenRead(inputFile)) {
      buffer = new byte[fileStream.Length];
      fileStream.Read(buffer, 0, buffer.Length);
    }
    using (FileStream fileStream = File.OpenWrite(outputFile)) {
      fileStream.Write(buffer, 0, buffer.Length);
    }
  }

  /// <summary>
  /// Creates a buffer by combining the SHA1 digest, file data, and SHA256 digest.
  /// </summary>
  /// <param name="sha1Digest">The SHA1 digest.</param>
  /// <param name="fileData">The file data.</param>
  /// <param name="sha256Digest">The SHA256 digest.</param>
  /// <returns>The combined buffer.</returns>
  private byte[] CreateBuffer(byte[] sha1Digest, byte[] fileData, byte[] sha256Digest) {
    int bufferLength = 4 + sha1Digest.Length + fileData.Length + sha256Digest.Length + BlockSizeInBytes;
    byte[] buffer = new byte[bufferLength];
    Buffer.BlockCopy(BitConverter.GetBytes(fileData.Length), 0, buffer, 0, 4);
    Buffer.BlockCopy(sha1Digest, 0, buffer, 4, sha1Digest.Length);
    Buffer.BlockCopy(fileData, 0, buffer, 4 + sha1Digest.Length, fileData.Length);
    Buffer.BlockCopy(sha256Digest, 0, buffer, 4 + sha1Digest.Length + fileData.Length, sha256Digest.Length);
    return buffer;
  }

  /// <summary>
  /// Encrypts the data using AES encryption.
  /// </summary>
  /// <param name="data">The data to be encrypted.</param>
  /// <returns>The encrypted data.</returns>

  public byte[] EncryptData(byte[] data) {
    using (Aes aes = Aes.Create()) {
      aes.Key = key;
      aes.IV = new byte[16];
      aes.Mode = CipherMode.CBC;
      aes.Padding = PaddingMode.PKCS7;
      using (ICryptoTransform encryptor = aes.CreateEncryptor()) {
        return encryptor.TransformFinalBlock(data, 0, data.Length);
      }
    }
  }

  /// <summary>
  /// Decrypts the data using AES decryption.
  /// </summary>
  /// <param name="data">The data to be decrypted.</param>
  /// <returns>The decrypted data.</returns>

  public byte[] DecryptData(byte[] data) {
    using (System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create()) {
      aes.Key = key;
      aes.IV = new byte[16];
      aes.Mode = CipherMode.CBC;
      aes.Padding = PaddingMode.PKCS7;
      using (ICryptoTransform decryptor = aes.CreateDecryptor()) {
        return decryptor.TransformFinalBlock(data, 0, data.Length);
      }
    }
  }

  /// <summary>
  /// Gets the password from the SHA256 digest.
  /// </summary>
  /// <param name="digest">The SHA256 digest.</param>
  /// <returns>The password as an integer.</returns>
  private int GetPasswordFromDigest(byte[] digest) {
    return BitConverter.ToInt32(digest, 0);
  }

  /// <summary>
  /// Converts a byte array to a hexadecimal string.
  /// </summary>
  /// <param name="bytes">The byte array to convert.</param>
  /// <returns>The hexadecimal representation of the byte array.</returns>
  public string ByteArrayToHex(byte[] bytes) {
    StringBuilder hex = new StringBuilder(bytes.Length * 2);

    foreach (byte b in bytes)
      hex.AppendFormat("{0:x2}", b);

    return hex.ToString();
  }

}
}
