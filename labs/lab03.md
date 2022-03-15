# Lab 03 - Persistency in containers

On this lab you'll use volumes to share data between containers.
Then we'll use environment variables to execute containers with different values.

## On this lab

- [Create Volume](lab03.md#create-volume)
- [Create Scripts](lab03.md#create-scripts)
- [Create Images](lab03.md#create-images)
- [Run Containers](lab03.md#run-containers)

## Create volume

Before sharing data between container, you need a volume. Let's create a volume.

```bash
docker volume create lab03-volume
```

Getting a list of volumes allow us to confirm that the volume was created.

```bash
docker volume ls --filter name=lab03-volume
```

You may get a list like this.

```bash
DRIVER    VOLUME NAME
local     lab03-volume
```

## Create Scripts

Now let's create the scripts and the images to use persistent storage on containers.

First, create a folder to keep all your files.

Then, create a bash script called `run-reader.sh` with the following content.

```bash
#!/bin/sh

echo "Log: $LOG_FILE";
touch $LOG_FILE

tail -f $LOG_FILE
```

This script will run command `tail` on a file to show the changes on that file.

The path for that file is available on environment variable `LOG_FILE`.

Now let's create another bash script called `run-writer.sh` with following content.

```bash
#!/bin/sh

echo "Log: $LOG_FILE";
touch $LOG_FILE

while $true
do 
    echo $(date +"%Y-%m-%d %H:%M:%S") >> $LOG_FILE
    sleep 1
done
```

This script will create a new entry on a file every second.

The path for that file is available on environment variable `LOG_FILE`.

## Create Images

Now it's time to create the Dockerfiles for each image.

Create a new file called `Dockerfile.reader` for reader image with this content.

```bash
FROM busybox

ARG LOG_FILE
ENV LOG_FILE=$LOG_FILE

WORKDIR /script

COPY run-reader.sh .

RUN chmod +x run-reader.sh

CMD ["/script/run-reader.sh" ]
```

Create a new file called `Dockerfile.writer` for writer image with this content.

```bash
FROM busybox

ARG LOG_FILE
ENV LOG_FILE=$LOG_FILE

WORKDIR /script

COPY run-writer.sh .

RUN chmod +x run-writer.sh

CMD ["/script/run-writer.sh" ]
```

Let's look to the Dockerfiles and check that you have a line with the following content.

```bash
ARG LOG_FILE
ENV LOG_FILE=$LOG_FILE
```

This code means that you may define the default value for the environment variable `LOG_FILE` during image build.

To create the reader image use the next `docker build` command.

```bash
docker build -f Dockerfile.reader --build-arg "LOG_FILE=/share/data.log" -t busybox-logger:reader .
```

To create the reader image use the next `docker build` command.

```bash
docker build -f Dockerfile.writer --build-arg "LOG_FILE=/share/data.log" -t busybox-logger:writer .
```

Run `docker images` command to check if the images were created properly.

## Run Containers

On a command line, run `docker run` command to run reader container and block your terminal to check the changes on the file.

```bash
docker run -it --rm -v lab03-volume:/share busybox-logger:reader
```

On another command line, run `docker run` command to run writer command.

```bash
docker run -d -v lab03-volume:/share --name busybox-writer busybox-logger:writer
```

Now check on the first terminal that you're seeing the changes on the file that the writer is doing.

Stop the writer container to check that the reader stops to show changes.

```bash
docker stop busybox-writer
```

On your first terminal you don't see any updates anymore.

Now let's stop the reader container doing `CTRL+c` on the first terminal.

Let's start a new reader container. `tail` command by default shows the last 10 lines from the file that you run the command.

Since we're using a docker volume, starting another command you may see the last 10 lines previously written by writer container.

```bash
docker run -it --rm -v lab03-volume:/share busybox-logger:reader
```

Finally, let's start again writer container to see new lines on reader container terminal.

```bash
docker start busybox-writer
```

We're done! You can clean all containers on your machine.

```bash
docker rm -f $(docker ps -aq)
```

## Next Lab: [Let’s put all together >](lab04.md)

[Return home >](../README.md#labs)
