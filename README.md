# Bruteforce-SolarPutty

Inspired by [SolarPuttyDecrypt](https://github.com/VoidSec/SolarPuttyDecrypt), I thought I would make something that allows you to pass a wordlist as an argument, along with the session file from a Solar-Putty installation and bruteforce the password and decrypt it.

A precompiled binary can be downloaded from the [Releases](https://github.com/LazyTitan33/Bruteforce-SolarPutty/releases/download/v1.0/Bruteforce-SolarPutty.exe).

## Usage

```text
Bruteforce-SolarPutty.exe <wordlist_file_path> <sessions_file_path>
```
After running it, if the password is found, it will be outputted and the session file will be decrypted:

![image](https://github.com/user-attachments/assets/445935fd-b421-4bd0-a235-4270022035d8)

## Python script

The python script works the same way and can be used in a Linux environment.

```bash
./Bruteforce_SolarPutty.py <wordlist_file_path> <sessions_file_path>
```
