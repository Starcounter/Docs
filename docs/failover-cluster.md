# Failover cluster

## Starcounter failover on Windows Failover Cluster

### Introduction

[Windows Failover Cluster](https://docs.microsoft.com/en-us/windows-server/failover-clustering/failover-clustering-overview) \(WFC\) is a component of Windows Server OS, that allows several machines \(here nodes\) to function together as a failover cluster. The purpose of a cluster is to administer resources assigned to it. A cluster monitors the health of resources and can restart or migrate them on another node if needed. Resources can be dependent on other resources and also belong to resource groups so that all resources from a group are running on the same node. WFC supports multiple different resources, three of them are of our particular interest – Generic Application, Generic Script, and IP Address.

A Generic Application is a resource that is backed by a customer-specified executable. WFC starts the executable on one of its nodes when a resource should go online and then tracks the process. If the process terminates or the node goes irresponsive, the cluster takes corrective actions, like restarting the process or migrating it to another node.

A Generic Script resource is a customer provided [WSH](https://en.wikipedia.org/wiki/Windows_Script_Host)-script. This script is used to manage some resource, and the cluster calls the script for tasks like setting the resource's online/offline status and retrieving the resource's current state.

An IP Address resource is, as its name implies, just an IP address. Whenever a node with this resource is online, it gets this address assigned to it.

WCF also provides for [Clustered Shared Volume](https://docs.microsoft.com/en-us/windows-server/failover-clustering/failover-cluster-csvs) \(CSV\) services. CSV is a shared synchronized storage that is available for all cluster nodes, and which is presented to a cluster node as a regular NTFS volume. It provides all regular storage services, for example file-system locks that effectively become distributed locks in a cluster.

### Basic setup

**\*Note**: `WebApp` is just a name of a .NET Core Starcounter 3.0 Web Application.\*

Now we've covered all the resources we need to setup a Starcounter failover cluster. First, we start with an easy setup and show how it recovers from possible faults. Then we point to the drawbacks of this setup and show how we address it with a more advanced approach that would be appropriate for real deployments.

An easy setup could look like this:

```text
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

| Fault | Recovery action |
| :--- | :--- |
| `WebApp.exe` crashes | Cluster detects that `WebApp.exe` has terminated and restarts it. The new `WebApp.exe` process connects to the Database and binds to the IP Address. |
| `scdata.exe` crashes | Cluster detects `scdata.exe` has terminated. It shuts down `WebApp.exe`, as it is a dependent resource. Then the usual starting sequence occurs. `scdata.exe` locks and reads the transaction log from CSV and `WebApp.exe` binds to the IP Address. |
| Node goes offline \(network failure or power outage\) | Cluster detects that the node is offline and decides to move the role to another node. First it dismounts CSV from the old node, so that all locks are released. Then it selects a new hosting node and starts all resources on it. Due to the locks being released, `scdata.exe` on the new node can lock the transaction log. IP Address is also transferred. |

**\*Note**: the basic setup is not recommended for production use and described for educational purposes only.\*

### Setup with `scdata` in standby mode

The setup described above has an important drawback. In certain cases, recovery will require a fresh start of the `scdata` process, which can take a significant amount of time. To overcome this, we need to run our `scdata` instance in a special standby mode, in which it can:

* Function without locking the transaction log.
* Periodically read and apply the transaction log.
* Switch to active mode upon request.

To keep `scdata` running on a non-active cluster node we can't use cluster resources, as WFC ensures that all resources are online on a single node. Instead we must provide an auto-started windows service, `starservice`, that administers `scdata.exe` for us.

This is how it works:

| Event | Reaction |
| :--- | :--- |
| `starservice` starts | `starservice` starts `scdata` in standby mode and periodically sends requests to poll transaction log |
| `starservice` stops | `starservice` stops `scdata` |
| `starservice` terminates unexpectedly | OS kills `scdata` |
| `scdata` terminates unexpectedly | `starservice` detects it and stops itself |
| `starservice` receives a request to promote `scdata` to active mode | `starservice` passes this request on to `scdata` |

To properly control `starservice`, i.e. starting, stopping and sending promotion requests, we use a Generic Script resource.

**Now the setup looks like this**:

* Every cluster node has an instance of configured `starservice`.
* We configure these cluster resources as such:

```text
Group: Starcounter
    └─ Resource: WebApp (Type: Generic Application, executable: WebApp.exe)
        ├─ Resouce: IP Address
        └─ Resource: Database (Type: Generic Script)
```

The Database script has the following workflow:

| Cluster event | Action |
| :--- | :--- |
| Go Online | Start `starservice` ¹. Send activate request. |
| Go Offline | Restart starservice ². |

* ¹ As a safety measure. The normal condition for a service is to be always started.
* ² As of now we can't switch `scdata` from active to standby mode, so we restart the service and thus `scdata`, so it restarts in standby mode. It's not a problem since the resource goes offline most likely because we're transferring the group to another node, so we have enough time to load the database on this node. Next time the cluster decides to host the group again on this node, `scdata` will already be prepared.

Now instead of starting `scdata` when the group moves to a new node, the cluster will start the Database script resource, which in turn ensures that `scdata` is started and active. WCF will handle migration of CSV, IP Address, and `WebApp`.

This new setup shares one drawback with the first one: if `scdata` crashes, the cluster will restart it on the same node first. And it might take time. This issue could be seen as marginal, however, as `scdata` should never crash. A crashing `scdata` process is in of itself a more severe problem than a slow recovery.

### Future directions

* We plan to design `scdata` to allow it to serve read requests in standby mode. With this feature, every cluster node will become an eventually consistent read-only replica.

### Practical setup steps

**\*Note**: It is important to specify the database path using exactly the same value in all places where it occurs. Values such as `C:\Path\To\Db` & `C:/Path/To/Db` are treated as different.\*

_See also the article about the_ [_Database connection string_](database-connection-string.md#the-database-value)_._

#### 1. Setup cluster and CSV

* [Create a failover cluster](https://docs.microsoft.com/en-us/windows-server/failover-clustering/create-failover-cluster).
* [Use Cluster Shared Volumes in a failover cluster](https://docs.microsoft.com/en-us/windows-server/failover-clustering/failover-cluster-csvs).

#### 2. Create database on CSV

Using the `star` tool:

```text
star new /csv/path/to/db
```

Using the native `sccreatedb` tool:

```text
sccreatedb -ip <path on csv volume> <database name>
```

#### 3. Setup `starservice` on every node

* Download, unzip, and copy the `starservice` files to all nodes. These files should have the same lication on all nodes.
* Create a service to start the database. The service name should be the same on all nodes.

**Using the `sc.exe` tool:**

```text
sc create <database name> start=auto binPath="<path to starservice.exe> service <path to the database>"
```

**Using the `starservice.exe` tool itself:**

```text
starservice.exe install <database name> <path to the database>
```

#### 4. Create a new cluster resource group

```text
Add-ClusterGroup Starcounter
```

#### 5. Create and configure the IP Adress resource

```text
Add-ClusterResource -Name "IP Address" -ResourceType "IP Address" -group "Starcounter"
Get-ClusterResource "IP Address" | Set-ClusterParameter –Multiple @{"Address"="<ip address>";"SubnetMask"="<subnet mask>";"EnableDhcp"=0}
```

#### 6. Create and configure the Database script resource

Copy the `scripts` folder from the previously downloaded archive to all nodes. The local path should be the same on all nodes. Don't use the CSV volume as it will complicate resource upgrade and troubleshooting.

**Then**:

```text
Add-ClusterResource -name Database -group "Starcounter" -ResourceType "Generic Script"
Get-ClusterResource "Database" | Set-ClusterParameter  -name ScriptFilepath -value "<path to script.js>"
Get-ClusterResource "Database" | Set-ClusterParameter  -name DbName -value "<database name>" -create
```

#### 7. Create and configure the `WebApp` resource

Copy the `WebApp` files to all nodes. The local path should be the same across nodes. Don't use CSV volume as it will complicate resource upgrade and troubleshooting.

**Then**:

```text
Add-ClusterResource -Name "WebApp" -Group "Starcounter" -ResourceType "Generic Application"
Get-ClusterResource "WebApp" | Set-ClusterParameter -Name CommandLine -Value "<path to webapp.exe>"
```

#### 8. Setup resource dependencies

```text
Set-ClusterResourceDependency -Resource WebApp -Dependency "([IP Address]) AND ([Database])"
```

#### 9. Start the group

```text
Start-ClusterGroup Starcounter
```

#### 10. Extra notes and resources

* Make sure to specify a reasonable configuration for the maximum allowed failures over time for the required resources. The default configuration is very limiting.
* Make sure to have at least three nodes in a cluster, or a file share witness to keep the cluster alive when a node goes down.
* [Deploying Scale-Out file Server in Cluster \(Server 2016\)](https://geekdudes.wordpress.com/2016/12/26/deploying-scale-out-file-server-in-cluster-server-2016/).
* [How to Configure a File Share Witness for a Hyper-V Cluster](https://www.altaro.com/hyper-v/file-share-witness-hyper-v-cluster/).

## Starcounter failover on Linux

### Introduction
The idea of starounter failover cluster is to boundle a starcounter database and a starcounter-based application into an entity that can be health monitored and automatically restarted or migrated to a standby cluster node should a disaster happens. Due to in-memory nature of a starcounter database, when failover happens it may take significant time to load data from media on a cold standby node. Thus it would be beneficial to keep starcounter running as a hot standby. Another requirement to the system concers data integrity. Our goal is to provide consistent solution in terms of [CAP](https://en.wikipedia.org/wiki/CAP_theorem), i.e. no committed transactions can be lost during migration.
### Setup Explained
Starcounter failover cluster is build on top of proven stack consisting of [pacemaker](https://clusterlabs.org/), [DRBD](https://www.linbit.com/drbd/) and [GFS2](https://access.redhat.com/documentation/en-us/red_hat_enterprise_linux/6/html/global_file_system_2/index). Pacemaker is responsible for managing cluster nodes, control resources it manages and perform appropriate failover actions. DRBD is responsible for synchronizing starcounter transaction log on a block level. And GFS2 provides Starcounter file level access to a shared transacton log. Starcounter role in this is:
* supporting hot standby mode so that in-memory data on a standy node is up-to-date with an active node
* providing pacemaker control scripts for starcounter database.

Here is a system diagram of a typical starcounter failover cluster:

![cluster](images/Starcounter%20cluster.png)

Let's go through all cluster resource we have under pacemaker control:

* IP address

This is a resource of type `ocf:heartbeat:IPaddr2`, which we use as a virtual public ip address flowing in the cluster along with a starcounter application. It allows external clients to access the application by the signle ip address regardless of which node hosts it. Should be configured to start on the same node as the starcounter application.
* Starcounter application

Controls your starcounter application. A good fit for resource type would be `ocf:heartbeat:anything` which can control any long-running daemon like processes. Should be configured to start on the same node where an active instance of starcounter database is running.
* Starcounter database

Controls running instance of starcounter database required for the starcounter application. It has a type of `ocf:starcounter:database` and this is the only resource in this setup which is authored by starcounter. You must install `resource-agents-starcounter` package to use it. Unlike IP address and Starcounter application resources that can have  only one running instance per cluster, this resource runs on every cluster node, but in different states - only one node can run it as a "master" while the rest are "slaves". "Master" and "slave" are pacemaker terms that directly correspond to starcounter database modes named "active" and "standby", so that if starcounter database resource in pacemaker is master, then the database it controls is in active mode. The same connection exists between "slave" pacemaker resource state and "standby" mode of starcounter database. In the active mode starcounter is able to accept client connections and perform database operations. And in the standby mode starcounter constantly pulls latest transactions from a transaction log and applies it to in-memory state, thus accelerating possible failover.
* GFS2

A resource to build a GFS2 cluster file system on top of a shared DRBD volume. This resource is mostly technical as DRBD itself is just a raw syncronized block device while Starounter stores transaction log in a conventional file thus requiring a file-system. The need of a cluster file system (and not a more common local one like ext4) stems form a fact that we use DRBD in dual primary mode. The necessity of dual-primary mode is covered in the section concerning DRBD resource. More on cluster file system requirement for dual-primary DRBD: https://www.linbit.com/drbd-user-guide/drbd-guide-9_0-en/#s-dual-primary-mode.
* DRBD

DRBD resource provides us with a shared block storage so that standby instances of starcounter could have an access to up-to-date transaction log. Using DRBD has befits of ensuring data high-availability and data consistency due to DRBD's synchronous replication. There is one caveat of DRBD usage in starcounter scenario - we need to run DRBD in not so common dual-primary mode. Only dual-primary allows mounting of DRBD volume on several nodes at the same time, thus allowing starcounter standby instance to read the transaction log at the same time active instance writes to it. In order to avoid split-brain and keep data consistent, it's strongly advised to use pacemaker fencing when DRBD is running as dual-primary. Without fencing, a cluster can end up in split-brain situation (for instance due to communication problems) and each instance saves a write transaction in the shared transaction log overwriting a transaction saved by another instance. As a result all transactions from the moment of split-brain might be lost. More on fencing: https://clusterlabs.org/pacemaker/doc/en-US/Pacemaker/2.0/html/Clusters_from_Scratch/ch05.html#_what_is_fencing.

### Alternative setups

As shown, starcouner failover cluster requires conistent shared data storage to maintain up-to-date in-memory state on standby node. It gives us possiblity tweak cluster setup in two dimensions:
1. If we give up on keeping in-memory state and we're fine with longer starcounter startup on failover, then we can use DRBD in single-primary mode. Using DRBD in single primary let us avoid strict fencing requirement if DRBD quorum is configured: https://www.linbit.com/drbd-user-guide/drbd-guide-9_0-en/#s-feature-quorum
2. We can use another storage alternatives given they provide two required features. Namely, being accesible from active and standby starcounter nodes (1) and ensure data consistency in split-brain situation (2). Possible solutions include:
    - using [OCFS2] (https://oss.oracle.com/projects/ocfs2/) instead of GFS2
    - using iSCSI shared volume with scsci fencing instead of DRBD
    - using NFS-based transaction log given NFS server supports fencing instead of GFS2+DRBD

### Future directions

Given enough interest, it should be possible to develop a setup that allows running starcounter in standby mode in a cluster with signle-primary DRBD. Such configuration has an advantage of not requiring pacemaker fencing while still be consistent and highly available.

### Practical setup steps

The backbone of a starcounter cluster is pretty standard mix of pacemaker, DRBD, GFS2 and IP Address resources. Please refer to [Cluster from scratch](https://clusterlabs.org/pacemaker/doc/en-US/Pacemaker/2.0/html/Clusters_from_Scratch/index.html) for detailed configuring steps. Here we'll briefly list required steps to set it up:

#### 1. SETUP PACEMAKER CLUSTER

```text
#install and run cluster software (on both nodes)
apt-get install pacemaker pcs psmisc corosync
systemctl start pcsd.service
systemctl enable pcsd.service

#set cluster user password (on both nodes)
passwd hacluster

#authenticate cluster nodes (on any node)
pcs host auth node1 node2

#create cluster (on any node)
pcs cluster setup mycluster node1.mshome.net node2.mshome.net --force
```

#### 2. ADD QUORUM NODE TO THE CLUSTER (https://access.redhat.com/documentation/en-us/red_hat_enterprise_linux/7/html/high_availability_add-on_reference/s1-quorumdev-haar)

```text
#on a quorum node (it would be a third machine)
apt install pcs corosync-qnetd
systemctl start pcsd.service
systemctl enable pcsd.service
passwd hacluster

#install quorum defince (on both nodes)
apt install corosync-qdevice

#add quorum to the cluster (on any node)
pcs host auth node3.mshome.net
pcs quorum device add model net host=node3.mshome.net algorithm=lms
```

#### 3. CONFIGURE FENCING

```text
#configure diskless sbd (https://documentation.suse.com/sle-ha/12-SP4/html/SLE-HA-all/cha-ha-storage-protect.html#pro-ha-storage-protect-confdiskless)

#configure sbd (on both nodes)
apt install sbd
mkdir /etc/sysconfig
cat <<EOF >/etc/sysconfig/sbd
	SBD_PACEMAKER=yes
	SBD_STARTMODE=always
	SBD_DELAY_START=no
	SBD_WATCHDOG_DEV=/dev/watchdog
	SBD_WATCHDOG_TIMEOUT=5
	EOF
systemctl enable sbd
    
#enable stonith for the cluster
pcs property set stonith-enabled="true"
pcs property set stonith-watchdog-timeout=10
```

#### 4. CONFIGURE DRDB PARTITIONS

Prerequisite: empty partition \dev\sdb1 on both nodes

```text
#intall and configure drbd (on both nodes)
apt-get install drbd-utils
cat <<END > /etc/drbd.d/test.res
resource test {
 protocol C;
 meta-disk internal;
 device /dev/drbd1;
 syncer {
  verify-alg sha1;
 }
 net {
  allow-two-primaries;
  fencing resource-only;
 }
 handlers {
    fence-peer "/usr/lib/drbd/crm-fence-peer.9.sh";
    unfence-peer "/usr/lib/drbd/crm-unfence-peer.9.sh";
 }
 on node1 {
  disk   /dev/sdb1;
  address  node1_ip_address:7789;
 }
 on node2 {
  disk   /dev/sdb1;
  address  node2_ip_address:7789;
 }
}
END
drbdadm create-md test
drbdadm up test

#make one of the nodes primary (on any node)
drbdadm primary --force test
```

#### 5. SETUP GFS

```text
#setup gfs packages (on both nodes)
#For Ubuntu:
apt-get install gfs2-utils dlm-controld 

#For CentOS:
yum localinstall https://repo.cloudlinux.com/cloudlinux/8.1/BaseOS/x86_64/dlm-4.0.9-3.el8.x86_64.rpm

#setup dlm resource (on one node) (https://clusterlabs.org/pacemaker/doc/en-US/Pacemaker/2.0/html/Clusters_from_Scratch/_configure_the_cluster_for_the_dlm.html)
pcs cluster cib dlm_cfg
pcs -f dlm_cfg resource create dlm ocf:pacemaker:controld op monitor interval=60s
pcs -f dlm_cfg resource clone dlm clone-max=2 clone-node-max=1
pcs cluster cib-push dlm_cfg --config

#create GFS2 filysystem (on both nodes)
mkfs.gfs2 -p lock_dlm -j 2 -t mycluster:gfs_fs /dev/drbd1
```

#### 6. CONFIGURE DRBD CLUSTER RESOURCE(https://clusterlabs.org/pacemaker/doc/en-US/Pacemaker/2.0/html/Clusters_from_Scratch/_configure_the_cluster_for_the_drbd_device.html)

```text
pcs cluster cib drbd_cfg
pcs -f drbd_cfg resource create drbd_drive ocf:linbit:drbd drbd_resource=test op monitor interval=60s
pcs -f drbd_cfg resource promotable drbd_drive promoted-max=2 promoted-node-max=1 clone-max=2 clone-node-max=1 notify=true
pcs cluster cib-push drbd_cfg --config
```

#### 7. CONFIGURE GFS CLUSTER RESOURCE(https://clusterlabs.org/pacemaker/doc/en-US/Pacemaker/2.0/html/Clusters_from_Scratch/_configure_the_cluster_for_the_filesystem.html)

```text
pcs cluster cib fs_cfg
pcs -f fs_cfg resource create drbd_fs Filesystem  device="/dev/drbd1" directory="/mnt/drbd" fstype="gfs2"
pcs -f fs_cfg constraint colocation add drbd_fs with drbd_drive-clone INFINITY with-rsc-role=Master
pcs -f fs_cfg constraint order promote drbd_drive-clone then start drbd_fs
pcs -f fs_cfg constraint colocation add drbd_fs with dlm-clone INFINITY
pcs -f fs_cfg constraint order dlm-clone then drbd_fs
pcs -f fs_cfg resource clone drbd_fs meta interleave=true
pcs cluster cib-push fs_cfg --config
```

#### 8. CONFIGURE CLUSTER VIRTUAL IP (on any node)

```text
pcs resource create ClusterIP ocf:heartbeat:IPaddr2 ip=192.168.52.235 cidr_netmask=28 op monitor interval=30s
```

#### 9. CONFIGURE STARCOUNTER AND STARCOUNTER BASED APPLICATION
Now we move on to configuring a starcounter database and a starcounter based application.

Let's start with setting default resource strickiness to avoid resources meving back after failed node recovery (https://clusterlabs.org/pacemaker/doc/en-US/Pacemaker/1.1/html/Clusters_from_Scratch/_prevent_resources_from_moving_after_recovery.html):
```text
pcs resource defaults resource-stickiness=100
```

Create a starcounter database resource - `db`. It requires two parameters: `dbpath` - a path to the database folder and `starpath` - a path to `star` utility:
```text
pcs resource create db database dbpath="/mnt/drbd/databases/db" starpath="/home/user/starcounter/star"
```

Configure `db` as promotable. After this, pacemaker will start `db` as a slave on all nodes:
```text
pcs resource promotable db meta interleave=true
```

Create a resource to control starcounter based application. We use `anything` resource type for it and the name of resource is `webapp`.
We configure connection string so that webapp connect to an existing and running database instance and doesn't start its own. This is to avoid possible interference with the databases that should be started by `db` resource:
```text
pcs resource create webapp anything binfile=/home/wad/WebApp/WebApp cmdline_options="ConnectionString='Database=/mnt/drbd/databases/db;OpenMode=Open;StartMode=RequireStarted'"
```

GFS2 fle system should be mounted before the database start:
```text
pcs constraint order start drbd_fs-clone then start db-clone
```

Webapp and ClusterIP should run on the same node:
```text
pcs constraint colocation add ClusterIP with webapp
```

Webapp requires a promoted instance of db to run on the same node as webapp itself. After this command, one instance of the database will be promoted to active state, while another one  will keep running as standby.
```text
pcs constraint order promote db-clone then start webapp
pcs constraint colocation add db-clone with webapp rsc-role=Master
```
