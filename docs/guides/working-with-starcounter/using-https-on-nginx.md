# Using HTTPS on NGINX

## Introduction

Secure Socket Layer \(SSL\) is a protocol that offers security through encryption for communications between client and server. The encryption process is made possible through the use of digital certificates verified by a third party Certificate Authority and as we know it is the most commonly implemented in the HTTPS combination protocol.

The SSL protocol aims to provide solutions to two simple security problems:

1. Secure data transmission between the `client` and `NGINX`.
2. Obtain proof that an involved party is the one we want to grant access to the encrypted data.

All input traffic passes through SSL to NGINX in private and integral way, where it is being unwrapped and forwarded to the Starcounter server.

## Prerequisites

To get started you will need to ensure some basic things are on your server.  
Further, you also need to have the NGINX web server installed. Click here to chose your OS and then follow installation documentation.

We will generate 2 files: the Private-Key file for the decryption of your SSL Certificate, and a certificate signing request \(CSR\) file used to apply for your SSL Certificate. If you remember two problems we aim to avoid with SSL from above, now things should fall into place.

> It is highly important to set proper file permissions on private key file and NGINX server configuration file containing password to the handshake!

## Generate Private Key

You might have purchased a SSL certificate already from trusted SSL vendor.  
Otherwise you might be interested to use let's encrypt, a free certificate provider. It doesn't compromise security at all and website has a convenient tools that help you go through the process.  
Staging, testing and other non-production apps can use a free self-signed SSL certificate instead of purchasing one. In that case the connection will be encrypted however every time we try to access the corresponding website protected by the SSL certificate we will receive a warning that the certificate was not issued by trusted SSL vendor.

Generating SSL private key in your local environment will require you to install OpenSSL tool.  
Use openssl to generate a new private key.

```bash
$openssl genrsa -des3 -out /var/www/master.oops-email.pass.com.key
...
Enter pass phrase for master.oops-email.pass.com.key:
Verifying - Enter pass phrase for master.oops-email.pass.com.key:
```

Generated private key can stripped of its password so it can be loaded without manual password entry. You can do it if it's your test key.

```bash
$ openssl rsa -in /var/www/master.oops-email.pass.com.key -out /var/www/master.oops-email.com.key
```

## Generate CSR

Now that you've created your key, it's time to create the Certificate Signing Request, which will be used by the Certificate Authority of your choice to generate the Certificate that SSL will present to other parties during the handshake.

```bash
$ openssl req -x509 -new -key master.oops-email.com.key -out master.oops-email.com.csr
```

The result will be a master.oops-email.com.csr file in your local directory \(alongside the master.oops-email.pass.com.key private key file from the above step\).  
You will be asked to enter a set of information. Before we proceed let's see which information we actually stated in the above command:



You will be asked to enter some information about your location and company. The most important part is the Common Name field which should match the name that you want to use your certificate with - your domain name.  
Example of fill-in:

```bash
Country Name (2 letter code) [SE]:Sweden
State or Province Name (full name) [Some-State]:Stockholm
Locality Name (eg, city) []:Stockholm
Organization Name (eg, company) [Internet Widgits Pty Ltd]:YourName, Inc.
Organizational Unit Name (eg, section) []:YourUnitName
Common Name (e.g. server FQDN or YOUR name) []:your_domain.com
Email Address []:admin@your_domain.com
```

## Configure NGINX to Use SSL

We have created our key and certificate files under the NGINX configuration directory.  
The .key file is your private key, and should be kept secure. The .csr file is what you will send to the CA to request your SSL certificate.  
Now we just need to modify server configuration to use those by adjusting our server block file the way it is written below:

```bash
server {
listen 80;
server_name master.oops-email.com;
return 301 https://$host$request_uri;
}

server {
listen 443 ssl;
ssl_certificate     /var/www/master.oops-email.com.csr;
ssl_certificate_key /var/www/master.oops-email.com.key;
ssl on;

location / {
proxy_pass http://191.239.210.246:8080;
proxy_set_header Host $host;
proxy_set_header X-Real-IP $remote_addr; }

location /website/update {
root /var/www/master.oops-email.com;
}

location ~ /__(.*)/[A-Z0-9]+ {
                proxy_pass              http://191.239.210.246:8080;
                proxy_set_header        Host            $host;
                proxy_set_header        X-Real-IP       $remote_addr;
                proxy_set_header        Upgrade         $http_upgrade;
                proxy_set_header        Connection      "upgrade";
                proxy_read_timeout      86400;
        }
}
```

When you are done restart NGINX server. This should reload your configuration, now allowing it to respond to both HTTP and HTTPS \(SSL\) requests.  
Don't forget to test your setup, first using normal HTTP

```http
http://server_domain_or_IP
```

then using SSL to communicate

```http
https://server_domain_or_IP
```

