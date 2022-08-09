#!/bin/bash

while getopts "s:" arg; do
  case $arg in
    s) session=$OPTARG;;
  esac
done

code "./demos/helpers/session0$session"

if [ $session -lt 5 ]
then
    code "./demos/docker/session0$session"
    cd "./demos/docker/session0$session"
else
    code "./demos/k8s/session0$session"
    cd "./demos/k8s/session0$session"
fi