#!/bin/sh

if [ -z "$1" ]
  then
    echo "No filename supplied. Usage: ./run-cycle.sh <filename>"
    exit 0
fi

echo "Log: $1";

for i in $(seq 10)
do 
    echo $(date +"%Y-%m-%d %H:%M:%S") >> $1
    echo "Doing something..."
    sleep 1
done

>&2 echo "ERROR! Something went wrong. Please check the log file."
