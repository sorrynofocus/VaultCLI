# VaultCLI
Vault CLI is a small application that works with Hashicorp's Vault utility. The utility makes it easy to pull out secrets.

**This project is a work in progress.**

The project is .NET 5.0. The compiler used is VS2022.
The _build.cmd_ builds windows, linux, and mac targets.

Examples:
Auth by token
_VaultCLI --server=https://server:port --token="token" --path="/my/awesome/secret/path" --mountpoint="secret"_

Auth by user/password
_VaultCLI --server=https://server:port --user=TheUserIsMe --password=passwd --path="/my/awesome/secret/path" --mountpoint="secret"_

Auth by OKta ID
_VaultCLI --server=https://server:port --oktaid= --password=passwd --path="/my/awesome/secret/path" --mountpoint="secret"_


_--server .   -Can be used for local default server http://127.0.0.1:8200_

_--mountpoint -Can be omitted to use default mountpoint 'secret'_
