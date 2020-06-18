using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    
    public class Encryption
    {
        //hashing a is one way encryption.
        public static string HashPassword(string password) {

            byte[] myPasswordAsBytes = Encoding.UTF32.GetBytes(password);

            // SHA 512 most secure
            //bytes numbers between 0-255
            var myalg = SHA512.Create();
            byte[] digestAsBytes = myalg.ComputeHash(myPasswordAsBytes);

            //use Convert.toBase64 when you are converting cryptographic bytes
            return Convert.ToBase64String(digestAsBytes); //Converting bk to string.
        }


        #region Symmetric

        //SymmetricEnc version for query string
        public static string SymmetricEncrypt(string input) {

            //choose alg and initilize it.      
            Rijndael myAlg = Rijndael.Create();

            //input to array of bytes conversion.
            byte[] inputAsBytes = Encoding.UTF32.GetBytes(input);

            //Generating the key and the IV
            //salt is an extention to the password (2 inputs 1 in salt another in password)
            string password = ConfigurationManager.AppSettings["password"];

            byte[] salt = { 22, 57, 234, 74, 234, 211, 22, 48, 85 };
            Rfc2898DeriveBytes myKeyGenerator = new Rfc2898DeriveBytes(password, salt);

            myAlg.Key = myKeyGenerator.GetBytes(myAlg.KeySize / 8); //convertion from bits to bytes(1 byte = 8 bits)
            myAlg.IV = myKeyGenerator.GetBytes(myAlg.BlockSize / 8); //ijsa key imma algorithim differenti

            //preparing the input to be encrypted as a MemoryStream
            MemoryStream msInput = new MemoryStream(inputAsBytes);

            //Declaring the object that will encrypt the input
            CryptoStream cs = new CryptoStream(msInput, myAlg.CreateEncryptor(), CryptoStreamMode.Read);

            //Prepare a place where to store the encrypted data
            MemoryStream msOutput = new MemoryStream();

            //Extracting the encrypted data
            cs.CopyTo(msOutput); //copies everything from the cryptoStream to the memoryStream.

            return Convert.ToBase64String(msOutput.ToArray());
        }

        //Using SymmetricEncryption for QueryString Encryption
        public static string EncryptQueryString(string input) {

            string cipher = SymmetricEncrypt(input);

            //Escape characters which are used in the Encryption
            cipher = cipher.Replace('/', '|');
            cipher = cipher.Replace('+', '!');
            cipher = cipher.Replace('=', '$');

            return cipher;
        }

     



        public static string SymmetricDecrypt(string input) {

            //for decryption only step 2, 8 and 5 are different.
            Rijndael myAlg = Rijndael.Create();

            byte[] cipherBytes = Convert.FromBase64String(input);

            string password = ConfigurationManager.AppSettings["password"];

            byte[] salt = { 22, 57, 234, 74, 234, 211, 22, 48, 85 };
            Rfc2898DeriveBytes myKeyGenerator = new Rfc2898DeriveBytes(password, salt);

            myAlg.Key = myKeyGenerator.GetBytes(myAlg.KeySize / 8);
            myAlg.IV = myKeyGenerator.GetBytes(myAlg.BlockSize / 8);

            MemoryStream msInput = new MemoryStream(cipherBytes);

            CryptoStream cs = new CryptoStream(msInput, myAlg.CreateDecryptor(), CryptoStreamMode.Read);

            MemoryStream msOutput = new MemoryStream();

            cs.CopyTo(msOutput);

            return Encoding.UTF32.GetString(msOutput.ToArray());
        }

        //Using SymmetricDecryption for QueryString Decryption
        public static string DecryptQueryString(string input)
        {
            input = input.Replace('|', '/');
            input = input.Replace('!', '+');
            input = input.Replace('$', '=');

            string decryptedValue = SymmetricDecrypt(input);

            return decryptedValue;
        }

        #endregion


        #region Assymmetric
        //Asymmetric (uses 2 keys), public to encrypt and private to decrypt
        //on registration both keys are generated and stored for each user in DB.
        //Asymmetrickeys, Asyencrypt and Asydec must all use same alg (RSA in this case.)

        //key generation
        public static AsymmetricKeys GenerateAsymmetricKeys() {
            RSA myAlg = RSA.Create();

            AsymmetricKeys myKeys = new AsymmetricKeys();
            myKeys.PublicKey = myAlg.ToXmlString(false); //false to be a public key
            myKeys.PrivateKey = myAlg.ToXmlString(true); //make true to be a private key

            return myKeys;
        }

        
        public static byte[] AsymmetricallyEncrypt(byte[] input, string publicKey) {
            var rsa = RSA.Create();
            rsa.FromXmlString(publicKey);

            byte[] output = rsa.Encrypt(input, RSAEncryptionPadding.Pkcs1); 
            return output;
        }

        public static byte[] AsymmetricallyDecrypt(byte[] input, string privateKey) {
            var rsa = RSA.Create();
            rsa.FromXmlString(privateKey);

            byte[] output = rsa.Decrypt(input, RSAEncryptionPadding.Pkcs1); //ad digital signiture nahseb irrid nuza il pkcs1 ukoll 
            return output;
        }
        #endregion


        //hybrid

        #region Hybrid
        //SymmetricEnc version for hybridEncrption
        public static byte[] SymmetricEncrypt(Stream input, byte[] key, byte[]iv) {
            input.Position = 0;
            Rijndael myAlg = Rijndael.Create();

            
            myAlg.Key = key;
            myAlg.IV = iv;

            MemoryStream msInput = new MemoryStream();
            input.CopyTo(msInput);
            msInput.Position = 0;
            CryptoStream cs = new CryptoStream(msInput, myAlg.CreateEncryptor(), CryptoStreamMode.Read);

            MemoryStream msOutput = new MemoryStream();
            cs.CopyTo(msOutput);

            //convert memory stream back to bytes[]
            byte[] byteOutput = msOutput.ToArray();
            return byteOutput;
        }

        
        //Hybrid symmetric Decrypt
        public static byte[] SymmetricDecrypt(Stream input, byte[] key, byte[] iv)
        {
            input.Position = 0;
            Rijndael myAlg = Rijndael.Create();

            myAlg.Key = key;
            myAlg.IV = iv;

            CryptoStream cs = new CryptoStream(input, myAlg.CreateDecryptor(), CryptoStreamMode.Read);

            MemoryStream msOutput = new MemoryStream();
            cs.CopyTo(msOutput);

            msOutput.Position = 0;

            byte[] output = msOutput.ToArray();
            return output;
        }

        


        public static MemoryStream HybridEncrypt(Stream input, string publicKey) {

            input.Position = 0; //reset position (when validating the bytes to check if the file is audio the position moves. so we reset the position here.)

            //keys generated (key,IV)
            var myAlg = Rijndael.Create();
            myAlg.GenerateKey();
            myAlg.GenerateIV();

        
            byte[] key =  myAlg.Key;
            byte[] encryptedKey = AsymmetricallyEncrypt(key, publicKey);

            byte[] iv = myAlg.IV;
            byte[] encryptedIv = AsymmetricallyEncrypt(iv, publicKey);

            byte[] seOutput = SymmetricEncrypt(input, key, iv);
 

            MemoryStream msEncryptedFile = new MemoryStream(seOutput);
            msEncryptedFile.Position = 0;

            MemoryStream msOut = new MemoryStream();

            msOut.Write(encryptedKey, 0, encryptedKey.Length);

            //6. save the encrypted iv
            msOut.Write(encryptedIv, 0, encryptedIv.Length);

            //7. save the encrypted file contents
            msEncryptedFile.CopyTo(msOut);
           
            return msOut;
        }


       
        //Hybrid Decryption.
        public static MemoryStream HybridDecrypt(Stream input, string privateKey) {

            //2
            byte[] encKey = new byte[128];
            input.Read(encKey, 0, 128); //will read first 128 bytes.


            byte[] encIV = new byte[128];
            input.Read(encIV, 0, 128);//will read second 128 bytes

            //3
            byte[] decryptedKey = AsymmetricallyDecrypt(encKey, privateKey);
            byte[] decryptedIv = AsymmetricallyDecrypt(encIV, privateKey);

            MemoryStream msRemainingFileData = new MemoryStream(); //da ha jkun il file data (ghax first 128 + 128 huma key u iv)
            input.CopyTo(msRemainingFileData);

            //4
            byte[] seOutput = SymmetricDecrypt(msRemainingFileData, decryptedKey, decryptedIv);

            MemoryStream output = new MemoryStream(seOutput);
            return output;
        }

        #endregion

        #region DigitalSigniture
        //Digital Signiture (to ensure that the file/data was not tempered/altered)
        //we use the privateKey instead of public (those are inverted)
        //digitalSignature column of type text was created in the Audio Table.

        //will return the digitalSigniture.
        public string SignData(byte[] input, string privateKey) {
            //RSA myAlg = RSA.Create();
            RSACryptoServiceProvider myAlg = new RSACryptoServiceProvider();
            myAlg.FromXmlString(privateKey);

            //try SignHash
            byte[] signiture = myAlg.SignData(input, new HashAlgorithmName("SHA512"), RSASignaturePadding.Pkcs1); //padding is used like a salt.
            return Convert.ToBase64String(signiture);
        }

       
        public bool VerifyData(byte[] input, string publicKey, string signature) {
            //RSA myAlg = RSA.Create(); 
            RSACryptoServiceProvider myAlg = new RSACryptoServiceProvider();
            myAlg.FromXmlString(publicKey);

            //returns true is data was not altered, false otherwise.
            bool result = myAlg.VerifyData(input, Convert.FromBase64String(signature), new HashAlgorithmName("SHA512"), RSASignaturePadding.Pkcs1);
            return result;
        }

        #endregion

    }

    public class AsymmetricKeys {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }


}
