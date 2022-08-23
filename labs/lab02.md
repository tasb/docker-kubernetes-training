# Lab 02 - How to create my own containers

On this lab you'll create your first image using a Dockerfile and publish on a public registry.

## On this lab

- [Prerequisites](lab02.md#prerequisites)
- [Simple Dockerfile](lab02.md#simple-dockerfile)
- [Multi-stage Dockerfile](lab02.md#multi-stage-dockerfile)
- [Publish an Image](lab02.md#publish-an-image)

## Prerequisites

To complete this lab you need to create (if you don't have already) a Docker account to allow you to push your image to a public registry.

Navigate to <https://hub.docker.com/signup> to create your account.

Please copy your **`Docker ID`** that you'll need later on this lab when pushing the image to Docker Hub.

## Simple Dockerfile

Open your favorite code editor (I can recommend [Visual Studio Code](https://code.visualstudio.com/) :)) and create a simple HTML file (you can use a more creative page if you want).

```html
<html>
    <body>
        <h1>Welcome!</h1>
        <p>My first container using Dockerfile</p>
    </body>
</html>
```

Save the file with name 'index.html'.

Now create a new file, called it Dockerfile (without extension) and add the following content.

```Dockerfile
FROM nginx:stable-alpine

COPY index.html /usr/share/nginx/html/index.html

CMD ["nginx", "-g", "daemon off;"]
```

Let's have a look into the contents of Dockerfile.

```Dockerfile
FROM nginx:stable-alpine
```

For base image you use an nginx image with tah 'stable-alpine' meaning that you get stable version of nginx using 'alpine' as Linux distro.

```Dockerfile
COPY index.html /usr/share/nginx/html/index.html
```

Then you are replacing standard nginx default page with the HTML that you created previously.

```Dockerfile
CMD ["nginx", "-g", "daemon off;"]
```

Finally, you're defining the command that will be executed when you instantiate your image.

Let's build this image.

```bash
docker build -t my-nginx .
```

Don't forget the '.' at the end of the command. This is essential to share the context between the host and Docker daemon.

But need to be careful about this context. This is used as root folder for copy operation and this operation makes a recursive copy.

You may list the images on your local cache to make sure everything went well.

```bash
docker images
```

Now let's run your container creating an instance of this image.

```bash
docker run -d --rm -p 8080:80 --name my-first-nginx my-nginx
```

You are using the flag '--rm' that will remove the container automatically as soon as it is stopped.

And let's test it navigating to <http://localhost:8080> and take a look on this amazing HTML page! :)

Now that we're done on this step you can stop the container and automatically will be removed.

```bash
docker stop my-first-nginx
```

## Multi-stage Dockerfile

Now let's make everything a little bit more complex.

Let's start downloading the source code to use on this lab [Echo API](https://github.com/tasb/docker-kubernetes-training/releases/download/echo-api/echo-api.zip).

After unzip, open a console and navigate to the created folder.

Open the folder with your favorite code editor and check the contents of Dockerfile file. You must have a file similar like this.

```Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR "/src/echo-api"
COPY ["echo-api.csproj", "."]

RUN dotnet restore "echo-api.csproj"
COPY . .
RUN dotnet build "echo-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "echo-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "echo-api.dll"]
```

Let's build this image using the Dockerfile.

```bash
docker build -t my-echo-api .
```

Let's check the images you have on your local cache

```bash
docker images
```

And you should get an output similar with this.

```bash
REPOSITORY   TAG       IMAGE ID       CREATED              SIZE
echo-api     latest    c36601aec6ad   5 seconds ago        213MB
```

If you run again the same build command you will see that most of the stepped will be performed quicker and get a CACHED on log.

```bash
docker build -t my-echo-api .
```

And you get an output like this.

```bash
=> [base 1/2] FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal@sha256:344352a571b0f9b17fb32fd9ab6cdac7415f2a5c1f35
=> CACHED [base 2/2] WORKDIR /app
=> CACHED [final 1/2] WORKDIR /app
=> CACHED [build 2/6] WORKDIR /src/echo-api
=> CACHED [build 3/6] COPY [echo-api.csproj, .]
=> CACHED [build 4/6] RUN dotnet restore "echo-api.csproj"
=> CACHED [build 5/6] COPY . .
=> CACHED [build 6/6] RUN dotnet build "echo-api.csproj" -c Release -o /app/build
=> CACHED [publish 1/1] RUN dotnet publish "echo-api.csproj" -c Release -o /app/publish /p:UseAppHost=false
=> CACHED [final 2/2] COPY --from=publish /app/publish .
```

If you want to force docker to build all layers again without using cache, you may use '--no-cache' flag.

```bash
docker build --no-cache -t my-echo-api .
```

Let's check the images you have on your local cache

```bash
docker images
```

And you should get an output similar with this.

```bash
REPOSITORY   TAG       IMAGE ID       CREATED              SIZE
echo-api     latest    c36601aec6ad   5 seconds ago        213MB
<none>       <none>    c36601aec6ad   4 minutes ago    213MB
```

You may find strange the image called '< none >'. This image represents the first created image that was replaced by this new build command.

This images are called dangling images and cannot be instantiate. Is a good practice to regularly clear them from your local cache.

You may use filters on docker rmi command.

```bash
docker rmi $(docker images -q --filter "dangling=true")
```

Or use docker prune command.

```bash
docker images prune
```

Now you can create a container from our newly created image.

```bash
docker run -d -p 9090:80 my-echo-api
```

And let's test it navigating to <http://localhost:9090/echo/message> and get a "message" as response.

You can clear all your containers since we're done with the Echo API!

## Publish an Image

In order to publish an image you need to tag it first. Now you'll need you Docker ID.

```bash
docker tag my-echo-api <DOCKER_ID>/my-echo-api:v1
```

Now you're ready to push your image to your public Docker Hub registry.

```bash
docker push <DOCKER_ID>/my-echo-api:v1
```

You may get an unauthorized error since you need to login into your docker account.

Run login command and enter your credentials that are your `DOCKER_ID` and password.

```bash
docker login
```

Ypu should get an output like this (after entering your credentials).

```bash
Login with your Docker ID to push and pull images from Docker Hub. If you don't have a Docker ID, head over to https://hub.docker.com to create one.
Username: DOCKER_ID
Password:

Login Succeeded
```

After push is finished you may test if your image is properly available.

Before you need to remove it from your local cache to make sure you are using the image published on docker.

```bash
docker rmi <DOCKER_ID>/my-echo-api:v1
```

Finally you may run a container using your published image.

```bash
docker run -d -p 9000:80 <DOCKER_ID>/my-echo-api:v1
```

And let's test it navigating to <http://localhost:9000/echo/message> and get a "message" as response.

You can clear all your containers since we're done with this lab!

## Next Lab: [Persistency in containers >](lab03.md)

[Return home >](../README.md#labs)
