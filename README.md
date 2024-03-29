# Training on Containers and Kubernetes

On this repo you can find 10 different labs that allow you to gradually have an hands-on experience on containers (using Docker) and local Kubernetes cluster.

Navigate to <https://tasb.github.io/docker-kubernetes-training/> to have access a better version of these instructions.

## On this page

- [Prerequisites](README.md#prerequisites)
  - [Windows](#windows)
  - [Ubuntu](#ubuntu)
  - [macOS](#macos)
- [Labs](README.md#labs)
- [ToDo App project](README.md#todo-app-project)
- [Slides](README.md#slides)
- [Feedback](README.md#feedback)

## Prerequisites

To perform the labs on this repo you need to have the following software installed on your machine.

### Windows

1. Windows 10+ (Windows 11 is recommended)
2. [Windows Terminal](https://www.microsoft.com/en-us/p/windows-terminal/9n0dx20hk701?activetab=pivot:overviewtab)
3. [Windows Subsystem for Linux](https://docs.microsoft.com/en-us/windows/wsl/install)
4. [Docker Desktop](https://www.docker.com/products/docker-desktop)
5. Configure WSL integration with Docker Desktop. More [here](https://docs.microsoft.com/en-us/windows/wsl/tutorials/wsl-containers#install-docker-desktop)
6. Install [Visual Studio Code](https://code.visualstudio.com/) (or other code editor of your preference)
7. Enable [Kubernetes on Docker Desktop](https://docs.docker.com/desktop/kubernetes/) (you may use any other kubernetes cluster at your choice)
8. (Optional) Some VS Code extension helpful for Docker and Kubernetes integration

    - [Docker](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-docker)
    - [Docker compose](https://marketplace.visualstudio.com/items?itemName=p1c2u.docker-compose)
    - [Kubernetes](https://marketplace.visualstudio.com/items?itemName=ms-kubernetes-tools.vscode-kubernetes-tools)
    - [YAML](https://marketplace.visualstudio.com/items?itemName=redhat.vscode-yaml)

### Ubuntu

1. Ubuntu 20.04
2. Docker. [How to install Docker Engine on Ubuntu](https://docs.docker.com/engine/install/ubuntu/)
3. Minikube. [How to install Minikube on Ubuntu](https://www.linuxtechi.com/how-to-install-minikube-on-ubuntu/)
4. Kubectl. [How to install Kubectl on Ubuntu](https://kubernetes.io/docs/tasks/tools/install-kubectl-linux/#install-using-native-package-management)
5. Install [Visual Studio Code](https://code.visualstudio.com/) (or other code editor of your preference)
6. (Optional) Some VS Code extension helpful for Docker and Kubernetes integration

    - [Docker](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-docker)
    - [Docker compose](https://marketplace.visualstudio.com/items?itemName=p1c2u.docker-compose)
    - [Kubernetes](https://marketplace.visualstudio.com/items?itemName=ms-kubernetes-tools.vscode-kubernetes-tools)
    - [YAML](https://marketplace.visualstudio.com/items?itemName=redhat.vscode-yaml)

This setup works on top of [Windows Subsystem for Linux](https://docs.microsoft.com/en-us/windows/wsl/install).

### macOS

1. Docker. [Install Docker Desktop on Mac](https://docs.docker.com/desktop/install/mac-install/)
2. Minikube. [How to install Minikube on Mac](https://minikube.sigs.k8s.io/docs/start/)
3. Kubectl. [How to install Kubectl on Mac](https://kubernetes.io/docs/tasks/tools/install-kubectl-macos/)
4. Install [Visual Studio Code](https://code.visualstudio.com/docs/setup/mac) (or other code editor of your preference)
5. (Optional) Some VS Code extension helpful for Docker and Kubernetes integration

    - [Docker](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-docker)
    - [Docker compose](https://marketplace.visualstudio.com/items?itemName=p1c2u.docker-compose)
    - [YAML](https://marketplace.visualstudio.com/items?itemName=redhat.vscode-yaml)

## Labs

On next links you may find the hands-on exercises to give you more experience on this topics.

You may navigate for each one individually or you may follow the sequence starting on first one and proceed to the next using the navigation links at the end of each lab.

1. [Introduction to containers](labs/lab01.md)
2. [How to create my own container](labs/lab02.md)
3. [Persistence in containers](labs/lab03.md)
4. [Let's compose your containers](labs/lab04.md)
5. [Introduction to Kubernetes](labs/lab05.md)
6. [Deployment lifecycle](labs/lab06.md)
7. [Managing services](labs/lab07.md)
8. [Storage in Kubernetes](labs/lab08.md)
9. [Monitoring and Operation](labs/lab09.md)

## ToDo App project

With this simple ToDo App you have the hands-on experience to create all needed artifacts to deploy an app on Kubernetes.

- Step #1 [Create images and run using docker compose](project/step01.md)
- Step #2 [Create Kubernetes manifests and run on your cluster](project/step02.md)

## Slides

Get access to the content used to share Kubernetes concepts during sessions.

1. [Introduction to containers](slides/Session01.pdf)
2. [Dockerfile and Tags](slides/Session02.pdf)
3. [Persistence in containers](slides/Session03.pdf)
4. [Docker compose](slides/Session04.pdf)
5. [Introduction to Kubernetes](slides/Session05.pdf)
6. [Deployments & ReplicaSets](slides/Session06.pdf)
7. [Kubernetes services](slides/Session07.pdf)
8. [Storage in Kubernetes](slides/Session08.pdf)
9. [Monitoring and Operation](slides/Session09.pdf)
10. [Mini project](slides/Session10.pdf)

## Feedback

For any feedback open up an issue describing what have you found and I'll return to you!

[Back to top…](README.md#on-this-page)
