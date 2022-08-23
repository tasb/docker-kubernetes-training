# Lab 07 - Managing services

On this lab you'll create deployments and services to deploy an Echo App.

## On this lab

- [Create deployments](lab07.md#create-deployments)
- [Create services](lab07.md#create-services)
- [Define ingress](lab07.md#define-ingress)

## Create deployments

First step to get your app (or services) running on a kubernetes cluster you need to have a deployment for each component: webapp and api.

Let's try to create deployment manifests on your own.

For `echo-api-dep`deployment:

- Filename: echo-api-dep.yaml
- Deployment name: echo-api-dep
- Replicas: 3
- Labels:
  - app: echo-app
  - tier: back
- Pod
  - Name: echo-api
  - Image: tasb/echo-api:k8s
  - Port: 80

If you want to get full content for this deployment, navigate to this [link](https://raw.githubusercontent.com/tasb/docker-kubernetes-training/main/src/EchoApp/manifests/echo-api-dep.yaml).

For `echo-webapp-dep`deployment:

- Filename: echo-webapp-dep.yaml
- Deployment name: echo-webapp-dep
- Replicas: 3
- Labels:
  - app: echo-app
  - tier: front
- Pod
  - Name: echo-webapp
  - Image: tasb/echo-webapp:k8s
  - Port: 80

If you want to get full content for this deployment, navigate to this [link](https://raw.githubusercontent.com/tasb/docker-kubernetes-training/main/src/EchoApp/manifests/echo-webapp-dep.yaml).

Now you can apply your manifests on your cluster. To better manage these deployments, let's create a namespace.

```bash
kubectl create ns echo-app-ns
```

Then create the deployments.

```bash
kubectl apply -f echo-api-dep.yaml -n echo-app-ns
kubectl apply -f echo-webapp-dep.yaml -n echo-app-ns
```

Check if your deployments are running properly.

```bash
kubectl get deploy -n echo-app-ns
```

And you should get an output like this.

```bash
NAME              READY   UP-TO-DATE   AVAILABLE   AGE
echo-api-dep      3/3     3            3           19s
echo-webapp-dep   3/3     3            3           18s
```

## Create services

Now you should create the services to get access to your Echo App.

To have external access to `LoadBalancer` services on `minikube` you need to create a tunnel between cluster and your machine.

Open a new terminal and run the following command. This terminal needs to be open until the end of this lab.

```bash
minikube tunnel
```

First, create a file called `echo-api-svc.yaml` and add the following content.

```yaml
apiVersion: v1
kind: Service
metadata:
  name: echo-api-svc
spec:
  ports:
    - port: 8080
      targetPort: 80
      name: api
  selector:
    app: echo-app
    tier: back
  type: LoadBalancer
```

This is a Load Balancer type service to grant you access directly from outside of your cluster (on this lab, using `localhost`).

The `selector` block will make the match with the labels you added on the pods on your deployment.

On `ports` block you define your service port to be `8080` and any request to it will forward to port `80` on the container.

Let's create the service on the cluster.

```bash
kubectl apply -f echo-api-svc.yaml -n echo-app-ns
```

To check if everything is working properly navigate to <http://localhost:8080/echo/message>.

This service have another endpoint that you can call several time to check that you're being served by different pods.

Navigate (or use `curl` command on command line) to <http://localhost:8080/hostname> and check the output to change (not on every call since your cluster uses a random algorithm as balancing algorithm).

Now let's create Echo Webapp service. Create a file called `echo-webapp-svc.yaml` and add the following content.

```yaml
apiVersion: v1
kind: Service
metadata:
  name: echo-webapp-svc
spec:
  ports:
    - port: 9000
      targetPort: 80
      name: web
  selector:
    app: echo-app
    tier: front
  type: LoadBalancer
```

Let's create the service on the cluster.

```bash
kubectl apply -f echo-webapp-svc.yaml -n echo-app-ns
```

To check if everything is working properly navigate to <http://localhost:9000>.

Now your already have your frontend and backend available from outside of your cluster.

## Define ingress

Regarding production environment you don't want to have services accessed directly on different port.

To better handle external communication with your cluster, let's create an ingress controller.

First you need to turn off `minikube tunnel`. Go to terminal where you run that command and press `Ctrl-c`.

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

Now your cluster is ready to receive and handle an ingress object.

Before you need to change the service of your already created services.

To do that you need only to change the line `type: LoadBalancer` on both services files to `type: ClusterIP`

After that, apply again the services to the cluster. Since we are using a declarative configuration, the cluster will make the change about service type.

```bash
kubectl apply -f echo-webapp-svc.yaml -n echo-app-ns
kubectl apply -f echo-api-svc.yaml -n echo-app-ns
```

Let's check that services type have changed to `ClusterIP`.

```bash
NAME              TYPE        CLUSTER-IP      EXTERNAL-IP   PORT(S)    AGE
echo-api-svc      ClusterIP   10.97.110.110   <none>        8080/TCP   8m33s
echo-webapp-svc   ClusterIP   10.96.27.91     <none>        9000/TCP   7m12s
```

Let's create the ingress. Create a file called `echo-app-ingress.yaml` and add the following content.

```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: echo-app-ingress
  annotations:
    nginx.ingress.kubernetes.io/rewrite-target: /$1
spec:
  ingressClassName: nginx
  rules:
  - host: echo-app.ingress.test
    http:
      paths:
      - path: /(.*)
        pathType: Prefix
        backend:
          service:
            name: echo-webapp-svc
            port:
              number: 9000
      - path: /api/(.*)
        pathType: Prefix
        backend:
          service:
            name: echo-api-svc
            port:
              number: 8080
```

With this ingress you will get:

- Requests to <http://echo-app.ingress.test/> will be forward to `echo-webapp-svc` service
- Requests to <http://echo-app.ingress.test/api/echo/message> will be forward to `echo-api-svc` service

Let's dive on the ingress file.

You have set an URL on `host` property.

```yaml
- host: echo-app.ingress.test
```

This means that the requests for this URL will be handle by this ingress.

On API path you have this definition.

```yaml
- path: /api/(.*)
```

This means that ingress will use a regex to match everything on the URL after `/api/`. This allow to send this text to the service that will serve this request.

The way for your to send this to the service is defined on the following lines.

```yaml
annotations:
  nginx.ingress.kubernetes.io/rewrite-target: /$1
```

`$1` means the first matching by regex on the URL. Knowing this, you may have several matches on URL and send them to the service.

Now it's time to apply the ingress on your cluster.

```bash
kubectl apply -f echo-app-ingress.yaml -n echo-app-ns
```

Now you need to restart the tunnel to get an IP on your ingress.

Run `minikube` command to start the tunnel.

```bash
minikube tunnel
```

Notice if the tunnel ask to enter `sudo`password since you're using reserved `80` port.

You can check if you ingress is properly configured running this command.

```bash
kubectl describe ingress echo-app-ingress -n echo-app-ns
```

You need an additional step to make everything working. First, let's get the ingress IP.

```bash
kubectl get ingress echo-app-ingress -n echo-app-ns
```

You should get an output like this.

```bash
NAME               CLASS   HOSTS                   ADDRESS        PORTS   AGE
echo-app-ingress   nginx   echo-app.ingress.test   192.168.49.2   80      6m
```

The IP you need is on `ADDRESS` column.

You need to change `hosts` file on your machine to have a match between URL defined on the ingress and this IP.

```bash
sudo sh -c 'echo "192.168.49.2\techo-app.ingress.test\n" >> /etc/hosts'
```

Finally, let's test if everything is working properly.

Navigate to <http://echo-app.ingress.test/> and you should see Echo App Webapp.

Then navigate to <http://echo-app.ingress.test/api/echo/message> and you should receive `"message"` as output.

On webapp, if you fill the input box with same text and click on `Make Echo!` button, you should see an error.

Let's check next lab to fix this!

## Next Lab: [Persistence in Kubernetes >](lab08.md)

[Return home >](../README.md#labs)
