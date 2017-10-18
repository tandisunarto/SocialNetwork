Generating private key and certificate file
============================================================
openssl req -newkey rsa:2048 -nodes -keyout socialnetwork.key -x509 -days 365 -out socialnetwork.cer

Create a .pfx container for the private key and certificate file
============================================================
openssl pkcs12 -export -in socialnetwork.cer -inkey socialnetwork.key -out socialnetwork.pfx
password: socialnetwork


iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/IdentityServer/IdentityServer4.QuickStart.UI/release/get.ps1'))