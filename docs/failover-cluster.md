# Starcounter failover cluster

## Starcounter failover on Linux

Starcounter 3 Release Candidate does not yet support failover on Linux operating systems out of the box.

## Starcounter failover on Windows Failover Cluster

### Introduction

[Windows Failover Cluster](https://docs.microsoft.com/en-us/windows-server/failover-clustering/failover-clustering-overview) (WFC) is a component of Windows Server OS, that allows several machines (here nodes) to function together as a failover cluster. The purpose of a cluster is to administer resources assigned to it. A cluster monitors the health of resources and can restart or migrate them on another node if needed. Resources can be dependent on other resources and also belong to resource groups so that all resources from a group are running on the same node. WFC supports multiple different resources, three of them are of our particular interest – Generic Application, Generic Script, and ip-address.

A Generic Application is a resource that is backed by a customer-specified executable. WFC starts the executable on one of its nodes when a resource should go online and then tracks the process. If the process terminates or the node goes irresponsive, the cluster takes corrective actions, like restarting the process or migrating it to another node.

A Generic Script resource is a customer provided [WSH](https://en.wikipedia.org/wiki/Windows_Script_Host)-script. This script is used to manage some resource, and the cluster calls the script for tasks like setting the resource's online/offline status and retrieving the resource's current state.

An IP-address resource is, as its name implies, just an IP address. Whenever a node with this resource is online, it gets this address assigned to it.

WCF also provides for [Clustered Shared Volume](https://docs.microsoft.com/en-us/windows-server/failover-clustering/failover-cluster-csvs) (CSV) services. CSV is a shared synchronized storage that is available for all cluster nodes, and which is presented to a cluster node as a regular NTFS volume. It provides all regular storage services, for example file-system locks that effectively become distributed locks in a cluster.

### Basic setup

***Note**: `WebApp` is just a name of a .NET Core Starcounter 3.0 Web Application.*

Now we've covered all the resources we need to setup a Starcounter failover cluster. First, we start with an easy setup and show how it recovers from possible faults. Then we point to the drawbacks of this setup and show how we address it with a more advanced approach that would be appropriate for real deployments.

An easy setup could look like this:
```
Group: Starcounter
    └─ Resource: WebApp (Type: Generic Application, executable: WebApp.exe)
        ├─ Resource: IP Address
        └─ Resource: Database (Type: Generic Application, executable: scdata.exe)
```       

Here "Starcounter" is a resource group containg three resources: `WebApp`, a Starcounter application we want to make highly available, and two other resources that `WebApp` depends on: an IP Address and a Database.

When starting a resource group, the cluster assigns it to some cluster node. On this node, it first starts `WebApp`'s dependencies, i.e. the IP Address and Database resources, and then the application.

The Database resource is a Generic Application resource. Starting it just starts the `scdata` process, thus making the database available to connect to. The `scdata` process locks the transaction log on CSV, reads it and then gets ready to serve incoming requests. 

All write transactions also go to CSV. Once its prerequisite resources are started, the cluster starts the `WebApp` resource by launching `WebApp.exe`. `WebApp.exe`, being a web app, binds to all local addresses, including the IP Address resource we assigned to it.

Now the group is fully started and ready to serve requests.

Let's consider possible faults and correcting actions:

| Fault                | Recovery action |
|----------------------|-----------------|
| `WebApp.exe` crashes | Cluster detects that `WebApp.exe` has terminated and restarts it. The new `WebApp.exe` process connects to the Database and binds to the IP Address. |
| `scdata.exe` crashes | Cluster detects `scdata.exe` has terminated. It shuts down `WebApp.exe`, as it is a dependent resource. Then the usual starting sequence occurs. `scdata.exe` locks and reads the transaction log from CSV and `WebApp.exe` binds to the IP Address. |
| Node goes offline (network failure or power outage) | Cluster detects that the node is offline and decides to move the role to another node. First it dismounts CSV from the old node, so that all locks are released. Then it selects a new hosting node and starts all resources on it. Due to the locks being released, `scdata.exe` on the new node can lock the transaction log. IP Address is also transferred. |

***Note**: the basic setup is not recommended for production use and described for educational purposes only.*

### Setup with `scdata` in standby mode

The setup described above has an important drawback. In certain cases, recovery will require a fresh start of the `scdata` process, which can take a significant amount of time. To overcome this, we need to run our `scdata` instance in a special standby mode, in which it can:

 - Function without locking the transaction log.
 - Periodically read and apply the transaction log.
 - Switch to active mode upon request.

To keep `scdata` running on a non-active cluster node we can't use cluster resources, as WFC ensures that all resources are online on a single node. Instead we must provide an auto-started windows service, `starservice`, that administers `scdata.exe` for us.

This is how it works:

| Event                | Reaction                                                                                              |
|----------------------|-------------------------------------------------------------------------------------------------------|
| `starservice` starts | `starservice` starts `scdata` in standby mode and periodically sends requests to poll transaction log |
| `starservice` stops  | `starservice` stops `scdata`                                                                          |
| `starservice` terminates unexpectedly                             | OS kills `scdata`                                        |
| `scdata` terminates unexpectedly                                  | `starservice` detects it and stops itself                |
| `starservice` receives a request to promote `scdata` to active mode | `starservice` passes this request on to `scdata`           |

To properly control `starservice`, i.e. starting, stopping and sending promotion requests, we use a Generic Script resource.

**Now the setup looks like this**: 

- Every cluster node has an instance of configured `starservice`.
- We configure these cluster resources as such:

```
Group: Starcounter
    └─ Resource: WebApp (Type: Generic Application, executable: WebApp.exe)
        ├─ Resouce: IP Address
        └─ Resource: Database (Type: Generic Script)
```

The Database script has the following workflow:

| Cluster event  | Action                                        |
|----------------|-----------------------------------------------|
| Go Online      | Start `starservice` ¹. Send activate request. |
| Go Offline     | Restart starservice ².                        |

- ¹ As a safety measure. The normal condition for a service is to be always started.
- ² As of now we can't switch `scdata` from active to standby mode, so we restart the service and thus `scdata`, so it restarts in standby mode. It's not a problem since the resource goes offline most likely because we're transferring the group to another node, so we have enough time to load the database on this node. Next time the cluster decides to host the group again on this node, `scdata` will already be prepared.

Now instead of starting `scdata` when the group moves to a new node, the cluster will start the Database script resource, which in turn ensures that `scdata` is started and active. WCF will handle migration of CSV, IP Address, and `WebApp`.

This new setup shares one drawback with the first one: if `scdata` crashes, the cluster will restart it on the same node first. And it might take time. This issue could be seen as marginal, however, as `scdata` should never crash. A crashing `scdata` process is in of itself a more severe problem than a slow recovery.

### Future directions

- We plan to design `scdata` to allow it to serve read requests in standby mode. With this feature, every cluster node will become an eventually consistent read-only replica.

### Practical setup steps

***Note**: It is important to specify the database path using exactly the same value in all places where it occurs. Values such as `C:\Path\To\Db` & `C:/Path/To/Db` are treated as different.*

*See also the article about the [Database connection string](database-connection-string.md#the-database-value).*

#### 1. Setup cluster and CSV

- [Create a failover cluster](https://docs.microsoft.com/en-us/windows-server/failover-clustering/create-failover-cluster).
- [Use Cluster Shared Volumes in a failover cluster](https://docs.microsoft.com/en-us/windows-server/failover-clustering/failover-cluster-csvs).

#### 2. Create database on CSV

Using the `star` tool:

```
star new /csv/path/to/db
```

Using the native `sccreatedb` tool:

```
sccreatedb -ip <path on csv volume> <database name>
```

#### 3. Setup `starservice` on every node

- Download, unzip, and copy the `starservice` files to all nodes. These files should have the same lication on all nodes.
- Create a service to start the database. The service name should be the same on all nodes.

**Using the `sc.exe` tool:**

```
sc create <database name> start=auto binPath="<path to starservice.exe> service <path to the database>"
```

**Using the `starservice.exe` tool itself:**

```
starservice.exe install <database name> <path to the database>
```
   
#### 4. Create a new cluster resource group

```
Add-ClusterGroup Starcounter
```
  
#### 5. Create and configure the IP Adress resource

```
Add-ClusterResource -Name "IP Address" -ResourceType "IP Address" -group "Starcounter"
Get-ClusterResource "IP Address" | Set-ClusterParameter –Multiple @{"Address"="<ip address>";"SubnetMask"="<subnet mask>";"EnableDhcp"=0}
```
  
#### 6. Create and configure the Database script resource

Copy the `scripts` folder from the previously downloaded archive to all nodes. The local path should be the same on all nodes. Don't use the CSV volume as it will complicate resource upgrade and troubleshooting.

**Then**:
        
```
Add-ClusterResource -name Database -group "Starcounter" -ResourceType "Generic Script"
Get-ClusterResource "Database" | Set-ClusterParameter  -name ScriptFilepath -value "<path to script.js>"
Get-ClusterResource "Database" | Set-ClusterParameter  -name DbName -value "<database name>" -create
```

#### 7. Create and configure the `WebApp` resource

Copy the `WebApp` files to all nodes. The local path should be the same across nodes. Don't use CSV volume as it will complicate resource upgrade and troubleshooting.

**Then**:
   
```
Add-ClusterResource -Name "WebApp" -Group "Starcounter" -ResourceType "Generic Application"
Get-ClusterResource "WebApp" | Set-ClusterParameter -Name CommandLine -Value "<path to webapp.exe>"
```
  
#### 8. Setup resource dependencies

```
Set-ClusterResourceDependency -Resource WebApp -Dependency "([IP Address]) AND ([Database])"
```
  
#### 9. Start the group

```
Start-ClusterGroup Starcounter
```

#### 10. Extra notes and resources

- Make sure to specify a reasonable configuration for the maximum allowed failures over time for the required resources. The default configuration is very limiting.
- Make sure to have at least three nodes in a cluster, or a file share witness to keep the cluster alive when a node goes down.
- [Deploying Scale-Out file Server in Cluster (Server 2016)](https://geekdudes.wordpress.com/2016/12/26/deploying-scale-out-file-server-in-cluster-server-2016/).
- [How to Configure a File Share Witness for a Hyper-V Cluster](https://www.altaro.com/hyper-v/file-share-witness-hyper-v-cluster/).