# nsupdateapi
RESTful service to execute nsupdate from NetBox webhooks

Background: Got tired of weirdness with Windows Server IPAM. GestioIP is very opaque and its DNS integration is unreliable.
Going to try NetBox which doesn't have built-in DNS integration, but does have good webhook support. This service will be invoked by NetBox and will run nsupdate. May reuse for other purposes as well.

Requires secrets/secrets.json, e.g.:
```
{
    "dnsServers":
    [
        "192.168.1.1",
        "192.168.1.2"
    ],
    "updateAllDnsServers": false
}
```

updateAllDnsServers true = run nsupdate for each DNS server defined. false = only iterate on failure