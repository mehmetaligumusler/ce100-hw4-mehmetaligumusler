using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using Xunit;
using CryptoLibrary;
using System.Drawing;

namespace CryptoLibrary.Test {
/// <summary>
/// Test class for the CryptoTransformer class.
/// </summary>
public class CryptoLibraryTest {
  /// <summary>
  /// Test method to compare file contents. Returns true if the contents of the files are equal.
  /// </summary>
  [Fact]
  public void CompareFiles_ContentsAreEqual_ReturnsTrue() {
    string inputFile = "Hello.txt";
    string decinputFile = "Hello2.txt";
    string decfile = "Hello.enc";
    DeleteContents();
    string content = "Hello, World!";
    File.WriteAllText(inputFile, content);
    byte[] key = Encoding.UTF8.GetBytes("D2F4A67B890CDE51");
    CryptoLibraryClass transformer = new CryptoLibraryClass(key);
    transformer.TransformFile(inputFile, decfile, 1);
    transformer.TransformFile(decfile, decinputFile, 0);
    bool result = CompareFileContents(inputFile, decinputFile);
    Assert.True(result);
  }
  /// <summary>
  /// Tests the TransformFile method with an invalid operation and expects an ArgumentException to be thrown.
  /// </summary>
  [Fact]
  public void TransformFile_InvalidOperation_ThrowsArgumentException() {
    // Arrange
    string inputFile = "Hello.txt";
    string decfile = "Hello.enc";
    DeleteContents();
    string content = "Hello, World!";
    File.WriteAllText(inputFile, content);
    byte[] key = Encoding.UTF8.GetBytes("D2F4A67B890CDE51");
    var crypto = new CryptoLibraryClass(key);
    int operation = 2;
    // Act & Assert
    Assert.Throws<ArgumentException>(() => crypto.TransformFile(inputFile, decfile, operation));
  }
  /// <summary>
  /// Tests the CryptoLibraryClass constructor with an invalid key size and expects an ArgumentException to be thrown.
  /// </summary>
  [Fact]
  public void CryptoLibraryClass_InvalidKeySize_ThrowsArgumentException() {
    // Arrange
    byte[] invalidKey = new byte[12];
    // Act & Assert
    Assert.Throws<ArgumentException>(() => new CryptoLibraryClass(invalidKey));
  }
  /// <summary>
  /// Tests the CompareHashes method when the hashes are not equal and expects false as the result.
  /// </summary>
  [Fact]
  public void CompareHashes_HashesNotEqual_ReturnsFalse() {
    byte[] key = Encoding.UTF8.GetBytes("D2F4A67B890CDE51");
    var crypto = new CryptoLibraryClass(key);
    // Arrange
    byte[] hash1 = new byte[] { 0x12, 0x34, 0x56, 0x78 };
    byte[] hash2 = new byte[] { 0xAB, 0xCD, 0xEF, 0x00 };
    // Act
    bool result = crypto.CompareHashes(hash1, hash2);
    // Assert
    Assert.False(result);
  }
  /// <summary>
  /// Tests the CompareHashes method when the hashes have different lengths and expects false as the result.
  /// </summary>
  [Fact]
  public void CompareHashes_DifferentLength_ReturnsFalse() {
    byte[] key = Encoding.UTF8.GetBytes("D2F4A67B890CDE51");
    var crypto = new CryptoLibraryClass(key);
    // Arrange
    byte[] hash1 = new byte[] { 0x01, 0x02, 0x03 };
    byte[] hash2 = new byte[] { 0x01, 0x02, 0x03, 0x04 };
    // Act
    bool result = crypto.CompareHashes(hash1, hash2);
    // Assert
    Assert.False(result);
  }
  /// <summary>
  /// Tests the ComputeSHA1 method with valid data and expects the correct hash to be computed.
  /// </summary>
  [Fact]
  public void ComputeSHA1_ValidData_ComputesCorrectHash() {
    // Arrange
    byte[] data = Encoding.UTF8.GetBytes("Hello, World!"); //hex : 48 65 6C 6C 6F 2C 20 57 6F 72 6C 64 21
    byte[] key = Encoding.UTF8.GetBytes("D2F4A67B890CDE51");
    var crypto = new CryptoLibraryClass(key);
    // Act
    byte[] hash = crypto.ComputeSHA1(data);
    // Assert
    string expectedHash = "0A 0A 9F 2A 67 72 94 25 57 AB 53 55 D7 6A F4 42 F8 F6 5E 01";
    expectedHash = expectedHash.Trim().Replace(" ", "").ToLower();
    Assert.Equal(expectedHash, ByteArrayToHex(hash));
  }
  /// <summary>
  /// Tests the ComputeSHA256 method with valid data and expects the correct hash to be computed.
  /// </summary>
  [Fact]
  public void ComputeSHA256_ValidData_ComputesCorrectHash() {
    // Arrange
    byte[] data = Encoding.UTF8.GetBytes("Hello, World!"); //hex : 48 65 6C 6C 6F 2C 20 57 6F 72 6C 64 21
    byte[] key = Encoding.UTF8.GetBytes("D2F4A67B890CDE51");
    var crypto = new CryptoLibraryClass(key);
    // Act
    byte[] hash = crypto.ComputeSHA256(data);
    // Assert
    string expectedHash = "DF FD 60 21 BB 2B D5 B0 AF 67 62 90 80 9E C3 A5 31 91 DD 81 C7 F7 0A 4B 28 68 8A 36 21 82 98 6F";
    expectedHash = expectedHash.Trim().Replace(" ", "").ToLower();
    Assert.Equal(expectedHash, ByteArrayToHex(hash));
  }
  /// <summary>
  /// Tests the AESEncryptAndDecrypt method with valid data and expects the encryption and decryption to be done correctly.
  /// </summary>
  [Fact]
  public void AESEncryptAndDecrypt_ValidData_EncryptsAndDecryptsCorrectly() {
    // Arrange
    byte[] data = Encoding.UTF8.GetBytes("Hello, World!"); //hex : 48 65 6C 6C 6F 2C 20 57 6F 72 6C 64 21
    byte[] key = Encoding.UTF8.GetBytes("D2F4A67B890CDE51");
    var crypto = new CryptoLibraryClass(key);
    // Act
    byte[] encrypted = crypto.EncryptData(data);
    byte[] decrypted = crypto.DecryptData(encrypted);
    // Assert
    string decryptedText = Encoding.UTF8.GetString(decrypted);
    Assert.Equal("Hello, World!", decryptedText);
  }
  /// <summary>
  /// Tests the ComputeHMACSHA1 method with valid data and expects the correct HMAC to be computed.
  /// </summary>
  [Fact]
  public void ComputeHMACSHA1_ValidData_ComputesCorrectHMAC() {
    // Arrange
    byte[] data = Encoding.UTF8.GetBytes("Hello, World!"); //hex : 48 65 6C 6C 6F 2C 20 57 6F 72 6C 64 21
    byte[] key = Encoding.UTF8.GetBytes("SampleKey1234567");
    var crypto = new CryptoLibraryClass(key);
    // Act
    byte[] hmac = crypto.ComputeHMACSHA1(data, key);
    // Assert
    string expectedHmac = "2e fa a6 3d 8e ae 14 a3 e6 7f d9 62 cc 9c 2c 65 e0 76 5e ae";
    expectedHmac = expectedHmac.Trim().Replace(" ", "").ToLower();
    Assert.Equal(expectedHmac, ByteArrayToHex(hmac));
  }
  /// <summary>
  /// Tests the ByteArrayToHex method to ensure correct conversion from byte array to hexadecimal string.
  /// </summary>
  [Fact]
  public void Test_ByteArrayToHex() {
    // Arrange
    byte[] bytes = new byte[] { 0x12, 0xAB, 0x34, 0xCD };
    string expectedHex = "12ab34cd";
    byte[] key = Encoding.UTF8.GetBytes("SampleKey1234567");
    var crypto = new CryptoLibraryClass(key);
    // Act
    string actualHex = crypto.ByteArrayToHex(bytes);
    // Assert
    Assert.Equal(expectedHex, actualHex);
  }
  /// <summary>
  /// Converts a byte array to a hexadecimal string.
  /// </summary>
  /// <param name="bytes">The byte array to convert.</param>
  /// <returns>The hexadecimal representation of the byte array.</returns>
  private string ByteArrayToHex(byte[] bytes) {
    StringBuilder hex = new StringBuilder(bytes.Length * 2);

    foreach (byte b in bytes)
      hex.AppendFormat("{0:x2}", b);

    return hex.ToString();
  }
  /// <summary>
  /// Compares the contents of two files.
  /// </summary>
  /// <param name="filePath1">The path of the first file.</param>
  /// <param name="filePath2">The path of the second file.</param>
  /// <returns>True if the contents of the files are equal, otherwise false.</returns>
  private bool CompareFileContents(string filePath1, string filePath2) {
    string file1Contents = File.ReadAllText(filePath1);
    string file2Contents = File.ReadAllText(filePath2);
    return file1Contents == file2Contents;
  }
  /// <summary>
  /// Deletes the contents of the files used in the tests.
  /// </summary>
  private void DeleteContents() {
    string hello2Path = "Hello2.txt";
    string decfile = "Hello.enc";
    string Binfile = "Hello.bin";
    string inputFile = "Hello.txt";

    if (File.Exists(hello2Path)) {
      File.Delete(hello2Path);
    }

    if (File.Exists(decfile)) {
      File.Delete(decfile);
    }

    if (File.Exists(Binfile)) {
      File.Delete(Binfile);
    }

    if (File.Exists(inputFile)) {
      File.Delete(inputFile);
    }
  }
}
/// <summary>
/// Test class for the HOTPGenerator class.
/// </summary>
public class HOTPTests {
  /// <summary>
  /// Test method for HOTP generation.
  /// </summary>
  [Fact]
  public void HOTP_Test() {
    byte[] key = new byte[] { 0x1f, 0x86, 0x98, 0x69, 0x0e, 0x02, 0xca, 0x16, 0x61, 0x85, 0x50, 0xef, 0x7f, 0x19, 0xda, 0x8e };
    CryptoLibraryClass crypto = new CryptoLibraryClass(key);
    int counter = 86351;
    int expected = 406818;
    int result = crypto.HOTP(key, counter);
    Assert.Equal(result, expected);
  }
  /// <summary>
  /// Test method for HOTP generation.
  /// </summary>
  [Fact]
  public void HOTP_Test2() {
    byte[] key = new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x30, 0x7f, 0x19, 0xda, 0x8e, 0x7f, 0x19 };
    CryptoLibraryClass crypto = new CryptoLibraryClass(key);
    int counter = 332677;
    int expected = 276093;
    int result = crypto.HOTP(key, counter);
    Assert.Equal(result, expected);
  }
  /// <summary>
  /// Test method for HOTP generation.
  /// </summary>
  [Fact]
  public void HOTP_Test3() {
    byte[] key = Encoding.ASCII.GetBytes("O344A661890CDE51");
    CryptoLibraryClass crypto = new CryptoLibraryClass(key);
    int counter = 1;
    int expected = 997037;
    int result = crypto.HOTP(key, counter);
    Assert.Equal(result, expected);
  }
}
}
