# Lab 06 - Deployment lifecycle

On this lab you'll create a new deployment and work with revisions.

## On this lab

- [Create your first Deployment](lab06.md#create-your-first-deployment)
- [Update deployment](lab06.md#update-deployment)
- [Rollback deployment](lab06.md#rollback-deployment)

## Create your first Deployment

Create a file named `mydep-v1.yml` and add the following content.

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: mydep
spec:
  replicas: 3
  selector:
    matchLabels:
      app: myapp
  template:
    metadata:
      labels:
        app: myapp
        version: v1
    spec:
      containers:
        - name: myapp
          image: nginx:1.17
          ports:
            - containerPort: 80
```

Let's create this deployment on your cluster.

```bash
kubectl apply -f mydep-v1.yml
```

Check if everything is running properly. For that you need to check deployments, replica sets and pods.

```bash
kubectl get deployments

kubectl get rs

kubectl get pods --show-labels
```

After last command you must see an output similar with this.

```bash
NAME                     READY   STATUS    RESTARTS   AGE   LABELS
mydep-74c8d46695-jmrmm   1/1     Running   0          7s    app=myapp,pod-template-hash=74c8d46695,version=v1
mydep-74c8d46695-m8rgh   1/1     Running   0          7s    app=myapp,pod-template-hash=74c8d46695,version=v1
mydep-74c8d46695-stfk9   1/1     Running   0          7s    app=myapp,pod-template-hash=74c8d46695,version=v1
```

Pay attention on label `version=v1` on all pods.

You have used `nginx:1.17` as image for your container. Let's use a newer version.

Edit `mydep-v1.yml` file and change the value of property `image` to `nginx:1.18`.

Run the `apply` command again to make this change on your deployment.

```bash
kubectl apply -f mydep-v1.yml
```

Let's check the new pods. If you are fast enough you may see new pods to be created and older pods to be terminated.

```bash
kubectl get pods --show-labels
```

You may notice that new pods are being created (`STATUS`with value `ContainerCreating`) and older pods are being terminated (`STATUS`with value `Terminating`)

Take the name of one of the pods to check if the image was changed. Use that name on next command.

```bash
kubectl describe pod <POD_NAME>
```

Scroll up on the output and check the image used.

```bash
Containers:
  myapp:
    Container ID:   docker://c06e9537476ef35ed845cfac276cac1cbf2c5fe6a777376f7ea0300bc453c312
    Image:          nginx:1.18
```

You're done with your first deployment.

## Update deployment

Create 3 new files named `mydep-v2.yml`, `mydep-v3.yml` and `mydep-v4.yml`.

Use the same content that you used on file `mydep-v1.yml` changing only line 14 with following content:

- `mydep-v2.yml` file: `version: v2`
- `mydep-v3.yml` file: `version: v3`
- `mydep-v4.yml` file: `version: v4`

Now let's apply each deployment and after the update of each one, check the labels on your pods.

Starting on file `mydep-v2.yml`.

```bash
kubectl apply -f mydep-v2.yml
kubectl get pods --show-labels
```

You must see `version=v2`on pod's labels list.

Next file `mydep-v3.yml`.

```bash
kubectl apply -f mydep-v3.yml
kubectl get pods --show-labels
```

You must see `version=v3`on pod's labels list.

Finally file `mydep-v4.yml`.

```bash
kubectl apply -f mydep-v4.yml
kubectl get pods --show-labels
```

You must see `version=v4`on pod's labels list.

With this process you confirm how it's easy to make updates on pods that are the workloads of your application.

## Rollback deployment

Finally, let's check how can we use the revisions that where created automatically to revert to previous versions.

First, let's rollback to previous version.

```bash
kubectl rollout undo deploy mydep
```

To check if was properly applied, check pods label again.

```bash
kubectl get pods --show-labels
```

You must see `version=v3`on pod's labels list.

Now let's rollback to initial version.

```bash
kubectl rollout undo deploy mydep --to-revision=1
```

You must see `version=v1`on pod's labels list.

And you're done working with deployments. Let's cleanup your cluster.

```bash
kubectl delete deployment mydep
```

Next you'll see how you can work with this deployments as a unique object.

## Next Lab: [Managing services >](lab07.md)

[Return home >](../README.md#labs)
