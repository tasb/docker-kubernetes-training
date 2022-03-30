# Lab 08 - Storage in Kubernetes

On this lab you'll use Persistent Volumes, ConfigMaps and Secret for a full configuration of Echo App.

## On this lab

- [Prepare your cluster](lab08.md#prepare-your-cluster)
- [Create persistent volume](lab08.md#create-persistent-volume)
- [Create ConfigMaps and Secrets](lab08.md#create-configmaps-and-secrets)
- [Add database](lab08.md#add-database)
- [Update EchoApp](lab08.md#update-echoapp)
- [Restart Database](lab08.md#restart-database)

## Prepare your cluster

**You should follow this step if you didn't finish [previous lab](/lab07.md) or cleared your cluster after that.**

Then create your first ingress, you need to install an ingress controller on your cluster.

```bash
minikube addons enable ingress
```

You need to check if ingress controller is already available on your cluster.

To do that, run the following command to check the status of the pod that implements the ingress controller.

```bash
kubectl get pods --namespace=ingress-nginx
```

You should get an output similar like this.

```bash
NAME                                        READY   STATUS      RESTARTS       AGE
ingress-nginx-admission-create--1-mzkx6     0/1     Completed   0              5h11m
ingress-nginx-admission-patch--1-4hzxw      0/1     Completed   0              5h11m
ingress-nginx-controller-54d8b558d4-jbnwx   1/1     Running     1 (103m ago)   5h11m
```

Your ingress controller is ready to be used when you see `Running` as the status of the pod with the name starting with `ingress-nginx-controller`.

Then, navigate to this [link](https://raw.githubusercontent.com/tasb/docker-kubernetes-training/main/src/EchoApp/manifests/echo-app-all.yaml) to get a manifest that will create all objects for EchoApp.

Create a file named `echo-app-all.yaml`, add the content that you found on previous link and apply the file.

```bash
kubectl apply -f echo-app-all.yaml
```

Check if everything is working properly with this command.

```bash
kubectl get all -n echo-app-ns
```

Now you're ready to continue to full configuration of this app.

## Create persistent volume

EchoApp will use a Database to keep record of requests that the API received.

To have a proper database you need to have a persistent volume to keep the database files.

Next, let's create the persistent volume. Add new file named `echo-app-pv.yaml` and add the following content.

```yaml
apiVersion: v1
kind: PersistentVolume
metadata:
  name: echo-app-pv
spec:
  capacity:
    storage: 10Gi
  volumeMode: Filesystem
  storageClassName: standard
  accessModes:
  - ReadWriteOnce
  persistentVolumeReclaimPolicy: Retain
  hostPath:
    path: /data/EchoAppData
```

Then apply this file with the following command.

```bash
kubectl apply -f echo-app-pv.yaml
```

Now you need a Persistent Volume Claim to integrate this volume with your pods.

Create a file named `echo-app-pvc.yaml`and add the following content.

```yaml
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: echo-app-pv-claim
spec:
  accessModes:
    - ReadWriteOnce
  storageClassName: standard
  resources:
    requests:
      storage: 3Gi
```

Since amount of storage requested `storage: 3Gi` is smaller that `10Gi` defined on the volume, you should get a bound between this claim and the previously created volume.

Then apply this file with the following command.

```bash
kubectl apply -f echo-app-pvc.yaml -n echo-app-ns
```

Let's check if the bound was created properly. Get Persistent Volume Claim list to check the status.

```bash
kubectl get pvc -A
```

On previous command you used the flag `-A`. With this flag you will get Persistent Volume Claims (pvc) available on all namespaces.

You should get an output similar with this.

```bash
NAMESPACE     NAME                STATUS   VOLUME        CAPACITY   ACCESS MODES   STORAGECLASS    AGE
echo-app-ns   echo-app-pv-claim   Bound    echo-app-pv   10Gi       RWO            local-storage   38m
```

If the claim worked perfectly you should see the value `Bound` on Status column.

## Create ConfigMaps and Secrets

Now you need to create the ConfigMap and Secret to use on your application.

First, the ConfigMap will have the address where WebApp will find the API to make the Echo request.

Create a file named `echo-app-config.yaml`and add the following command.

```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: echo-app-cfg
data:
  echo-api-server: echo-api-svc:8080
```

You may check that you're creating a property called `echo-api-server` with value `echo-api-svc:8080` that is exactly the name and port of the Echo API ClusterIP service.

Apply this ConfigMap on your cluster.

```bash
kubectl apply -f echo-app-config.yaml -n echo-app-ns
```

Then, let's create a secret that will have the sensitive data on how to connect with you database.

If you recall about how Secret are stored on Kubernetes, you need to create secret data encoded with Base64 algorithm.

The easiest way to do that if leveraging kubectl command. Run the following command to create your secret manifest.

```bash
kubectl create secret generic echo-api-db-secret --from-literal=dbpass="P@ssw0rd" --from-literal=connString="Server=echo-db-svc,1433;Initial Catalog=echo-log;User ID=SA;Password=P@ssw0rd;" --dry-run=client -o yaml > echo-app-secret.yaml
```

You're creating one secret with two properties:

- dbpass: "P@ssw0rd"
- connString: "Server=echo-db-svc,1433;Initial Catalog=echo-log;User ID=SA;Password=P@ssw0rd;"

Let's create the secret on your cluster.

```bash
kubectl apply -f echo-app-secret.yaml -n echo-app-ns
```

On connection string your are using `echo-db-svc` as database server name. This is the next step on this tutorial.

## Add Database

The store Echo API requests log you need a database. Let's put the discussion about using databases inside a Kubernetes cluster out of this lab. :)

Start to create a new file named `echo-db-dep.yaml` and add the following content.

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: echo-db-dep
spec:
  replicas: 1
  selector:
    matchLabels:
      app: echo-app
      tier: db
  template:
    metadata:
      labels:
        app: echo-app
        tier: db
    spec:
      containers:
      - name: echo-db
        image: mcr.microsoft.com/mssql/server:2017-latest
        ports:
        - containerPort: 80  
        env:
        - name: ACCEPT_EULA
          value: "Y"
        - name: SA_PASSWORD
          valueFrom:
            secretKeyRef:
              name: echo-api-db-secret
              key: dbpass
        volumeMounts:
          - mountPath: "/var/opt/mssql/data"
            name: db-storage
      volumes:
        - name: db-storage
          persistentVolumeClaim:
            claimName: echo-app-pv-claim
```

Let's look into some details of this deployment.

First, your using image `mcr.microsoft.com/mssql/server:2017-latest` that have a Microsoft SQL Server 2017 inside.

Then, look into the `volume` block.

```yaml
volumes:
  - name: db-storage
    persistentVolumeClaim:
      claimName: echo-app-pv-claim
```

The volume `db-storage` is using the Persistent Volume Claim created previously.

To use this volume inside your pod you are creating a volume mount with the following `volumeMount` block.

```yaml
volumeMounts:
  - mountPath: "/var/opt/mssql/data"
    name: db-storage 
```

This mount will mount your volume on path `/var/opt/mssql/data`. This path is where MS SQL Server keeps its data by default.

Finally, you are using the secret created previously to set the password for SA account with the following environment variable configuration.

```yaml
- name: SA_PASSWORD
  valueFrom:
    secretKeyRef:
      name: echo-api-db-secret
      key: dbpass
```

Now you should create the service that will serve this database. Create a file named `echo-db-svc.yaml` and add the following content.

```yaml
apiVersion: v1
kind: Service
metadata:
  name: echo-db-svc
spec:
  ports:
    - port: 1433
      targetPort: 1433
      name: db
  selector:
    app: echo-app
    tier: db
  type: ClusterIP
```

Check that the name of this service `echo-db-svc`is exactly the server address you have used on connection string added to the secret your created previously.

Now let's add both objects to the cluster.

Add your deployment to the cluster to startup your database.

```bash
kubectl apply -f echo-db-dep.yaml -n echo-app-ns
```

Then create the service that will expose your database inside your cluster.

```bash
kubectl apply -f echo-db-svc.yaml -n echo-app-ns
```

Check if everything is running properly with the following command.

```bash
kubectl get all -n echo-app-ns
```

This command show you all main objects (Deployments, ReplicaSets, Services and Pods) available on namespace `echo-app-ns`.

## Update EchoApp

Now you can make the final configurations on API deployment to connect to the database and configure properly the connection between WebApp and API.

Create (or edit) `echo-api-dep.yaml` file and make sure that have the following content.

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: echo-api-dep
spec:
  replicas: 1
  selector:
    matchLabels:
      app: echo-app
      tier: back
  template:
    metadata:
      labels:
        app: echo-app
        tier: back
    spec:
      containers:
      - name: echo-api
        image: tasb/echo-api:k8s-v2
        ports:
        - containerPort: 80
        imagePullPolicy: Always
        env:
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: echo-api-db-secret
              key: connString
```

The changes are:

- Change on image to use the new `tasb/echo-api:k8s-v2`
- Add new environment variable `ConnectionStrings__DefaultConnection` to use `connString` property of secret `echo-api-db-secret`

The Echo API will automatically update the database to create the schema needed to store the requests log.

Finally, you should create (or edit) `echo-webapp-dep.yaml` file to have the following content.

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: echo-webapp-dep
spec:
  replicas: 1
  selector:
    matchLabels:
      app: echo-app
      tier: front
  template:
    metadata:
      labels:
        app: echo-app
        tier: front
    spec:
      containers:
      - name: echo-webapp
        image: tasb/echo-webapp:k8s-v2
        ports:
        - containerPort: 80
        imagePullPolicy: Always
        env:
        - name: EchoAPIServer
          valueFrom:
            configMapKeyRef:
              name: echo-app-cfg
              key: echo-api-server
```

The changes are:

- Change on image to use the new `tasb/echo-webapp:k8s-v2`
- Add new environment variable `EchoAPIServer` to use `echo-api-server` property of ConfigMap `echo-api-server`.

Now you may update this deployments on your cluster.

```bash
kubectl apply -f echo-webapp-dep.yaml -n echo-app-ns
kubectl apply -f echo-api-dep.yaml -n echo-app-ns
```

Check if everything is running properly with the following command.

```bash
kubectl get all -n echo-app-ns
```

After you check that your pods are running, you can test your application.

Navigate to <http://echo-app.ingress.test> and have access to WebApp. On the page try your application adding some text on the input text and click on 'Make echo!' button. You may do this several times.

Then to check if your are keeping the logs properly, navigate to <http://echo-app.ingress.test/api/logs> and have a access to a list (on JSON format) with all the requests that you've done.

## Restart Database

At last this last step, you will test how volumes are useful for keeping your pods state even when they are recreated.

Let's get the database pod name using following command.

```bash
kubectl get pods -n echo-app-ns -l tier=db
```

You should get an output like this.

```bash
NAME                           READY   STATUS    RESTARTS        AGE
echo-db-dep-54cff96b58-sv8lb   1/1     Running   1 (5m59s ago)   4h30m
```

Now using the name of your pod you can delete the pod and then wait for a new one to be created. Don't forget to change the pod name from the next command.

```bash
kubectl delete pod echo-db-dep-54cff96b58-sv8lb -n echo-app-ns
```

If you run again the command to list database pods you'll check that a new pod was created since you get a new pod name.

```bash
kubectl get pods -n echo-app-ns -l tier=db
```

You should get an output like this.

```bash
NAME                           READY   STATUS    RESTARTS        AGE
echo-db-dep-54cff96b58-tjqbv   1/1     Running   1 (5m59s ago)   4h30m
```

To confirm that you have the same logs, navigate to <http://echo-app.ingress.test/api/logs> and check the list is the same that you got previously.

## Next Lab: [Monitoring and Operation >](lab09.md)

[Return home >](../README.md#labs)
