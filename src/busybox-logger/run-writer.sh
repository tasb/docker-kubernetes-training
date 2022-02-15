#!/bin/sh

echo "Log: $LOG_FILE";
touch $LOG_FILE

while $true
do 
    echo $(date +"%Y-%m-%d %H:%M:%S") >> $LOG_FILE
    sleep 1
done

